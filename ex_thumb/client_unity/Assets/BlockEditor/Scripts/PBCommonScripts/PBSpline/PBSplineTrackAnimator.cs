/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PBSplineTrackAnimator : MonoBehaviour, IPBSplineAnimator
{
    private PBSplineTrack m_Track;
    private PBSplineTrack track
    {
        get { return m_Track ?? (m_Track = GetComponent<PBSplineTrack>()); }
    }
    
    public enum TrackState
    {
        Normal = 0,    //正常状态
        Spread,        //展开平铺
        RotateClockwise,    //顺时针转
        RotateCounterclockwise,    //逆时针转
        
        Count
    }

    private TrackState curState = TrackState.Normal;

    public int KeyframeCount
    {
        get { return (int) TrackState.Count; }
    }

    public string[] KeyframeLiterals
    {
        get
        {
            string[] literals = new string[KeyframeCount];
            literals[0] = "收拢";
            literals[1] = "平铺";
            literals[2] = "顺时针转";
            literals[3] = "逆时针转";
            return literals;
        }
    }

    public int[] KeyframeValues
    {
        get
        {
            int[] values = new int[KeyframeCount];
            for (int i = 0; i < KeyframeCount; i++)
            {
                values[i] = i;
            }
            return values;
        }
    }

    public void SetKeyframe(int index)
    {
        if (index < 0 || index >= (int) TrackState.Count)
            return;

        track.IsSpread = index == (int) TrackState.Spread;
        track.StartOffset = track.StartOffset - track.Curve.Length * Mathf.FloorToInt(track.StartOffset / track.Curve.Length);

        track.Curve.Clear();
        track.ForceUpdate();

        curState = (TrackState) index;
    }

    private Sequence tweenSeq = null;
    
    public Sequence Play(int index, float speed, Action onComplete)
    {
        if (index < 0 || index >= (int) TrackState.Count)
            return null;
        
        tweenSeq = DOTween.Sequence();
        tweenSeq.AppendInterval(speed);

        if (index == (int) TrackState.Normal)
        {
            StartSpreadAnim(false, speed);
            tweenSeq.OnKill(StopSpreadAnim);
        }
        else if (index == (int) TrackState.Spread)
        {
            StartSpreadAnim(true, speed);
            tweenSeq.OnKill(StopSpreadAnim);
        }
        else
        {
            SetKeyframe(index);
            tweenSeq.AppendCallback(() =>
            {
                if (index == (int) TrackState.RotateClockwise || index == (int) TrackState.RotateCounterclockwise)
                    SetKeyframe((int) TrackState.Normal);
            });
            tweenSeq.OnKill(() =>
            {
                if (index == (int) TrackState.RotateClockwise || index == (int) TrackState.RotateCounterclockwise)
                    SetKeyframe((int) TrackState.Normal);
            });
        }

        return tweenSeq;
    }

    public void Stop()
    {
        if (tweenSeq != null)
        {
            tweenSeq.Kill();
            tweenSeq = null;
        }
    }

    public void ResetToStartOfKeyframe(int index)
    {
        SetKeyframe(index == (int) TrackState.Normal ? (int) TrackState.Spread : (int) TrackState.Normal);
    }

    public void SetEditorKeyframe()
    {
        //first ensure all circles are editor transform
        foreach (PBCircle circle in track.Circles)
        {
            circle.transform.GetComponent<PBBlock>().SetEditorTransform();
        }
        SetKeyframe((int) TrackState.Normal);
    }

    private void Update()
    {
        if (curState == TrackState.RotateClockwise)
        {
            track.StartOffset -= track.Spacing * Time.deltaTime;
            track.ForceUpdate();
        }
        else if (curState == TrackState.RotateCounterclockwise)
        {
            track.StartOffset += track.Spacing * Time.deltaTime;
            track.ForceUpdate();
        }
        else if (isSpreadAnim)
        {
            UpdateSpreadAnim();
        }
    }

    private bool isSpreadAnim = false;
    private float totalTime;
    private float curTime;
    private Vector3 circleForward;
    private Vector3 midPos;
    
    internal class AnimChild
    {
        internal Transform transform;

        internal Vector3 startPosVec;
        internal Vector3 endPosVec;

        internal Vector3 startTangent;
        internal Vector3 endTangent;
        internal float rotAngle;
        internal Vector3 rotAxis;
        internal float lastAngle;

        internal bool modifyAngle;
    }

    private List<AnimChild> animChilds = new List<AnimChild>();

    private void StartSpreadAnim(bool normalToSpread, float speed)
    {
        totalTime = speed;
        curTime = 0;
        circleForward = track.Circles[0].forward;
        
        Vector3 tangent, up;
        float distance;
        track.Curve.Build(track.Circles, track.CurveType);
        track.Curve.GetBottomMidSample(out midPos, out tangent, out up, out distance);
        float halfLength = 0.5f * track.Curve.Length;

        //计算起点偏移，使中点为midPos
        track.StartOffset = distance - halfLength;
        if (normalToSpread)
        {
            //从收拢到平铺
            SetKeyframe((int) TrackState.Normal);
            track.IsSpread = true;
        }
        else
        {
            //从平铺到收拢
            SetKeyframe((int) TrackState.Spread);
            track.IsSpread = false;
        }

        int midIndex = Mathf.FloorToInt(halfLength / track.Spacing);     

        List<TrackChildInfo> endTransInfos = track.ComputeTrack();
        animChilds.Clear();
        for (int i = 0; i < track.transform.childCount; i++)
        {
            Transform child = track.transform.GetChild(i);
            AnimChild animChild = new AnimChild();
            animChilds.Add(animChild);

            animChild.transform = child;
            animChild.startPosVec = child.position - midPos;
            animChild.endPosVec = endTransInfos[i].position - midPos;
            animChild.startTangent = child.rotation * Vector3.left;
            animChild.endTangent = endTransInfos[i].rotation * Vector3.left;

            animChild.rotAngle = Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(Vector3.Dot(animChild.startTangent, animChild.endTangent), -1, 1));
            
            //修正旋转动画角度
            Vector3 cross = Vector3.Cross(animChild.startTangent, animChild.endTangent);
            if (i <= midIndex)
            {
                animChild.rotAxis = circleForward;
            }
            else
            {
                animChild.rotAxis = -circleForward;
            }

            if (Vector3.Dot(cross, animChild.rotAxis) < -0.0001f)
                animChild.rotAngle = 360 - animChild.rotAngle;
        }

        if (!track.IsSpread)
        {
            //反过来转
            for (int i = 0; i < animChilds.Count; i++)
            {
                if (animChilds[i].rotAngle > 0.001f)
                {
                    animChilds[i].rotAxis = -animChilds[i].rotAxis;
                    animChilds[i].rotAngle = 360 - animChilds[i].rotAngle;
                }
            }
        }

        isSpreadAnim = true;
    }

    private void UpdateSpreadAnim()
    {
        curTime += Time.deltaTime;
        if (curTime > totalTime)
        {
            isSpreadAnim = false;
        }

        float t = Mathf.Clamp01(curTime / totalTime);
        for (int i = 0; i < animChilds.Count; i++)
        {
            AnimChild child = animChilds[i];
            float angle = child.rotAngle * t;
            child.transform.Rotate(child.rotAxis, angle - child.lastAngle, Space.World);
            child.lastAngle = angle;

            Vector3 posVec = Vector3.Slerp(child.startPosVec, child.endPosVec, t);
            child.transform.position = midPos + posVec;
        }   
    }

    private void StopSpreadAnim()
    {
        isSpreadAnim = false;
        SetKeyframe(track.IsSpread ? (int) TrackState.Spread : (int) TrackState.Normal);
    }
}