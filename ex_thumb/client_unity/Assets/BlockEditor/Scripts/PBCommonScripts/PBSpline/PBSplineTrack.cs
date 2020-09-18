/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct TrackChildInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public float distOnCurve; //距离curve起点的距离
}

[Serializable]
public struct CircleRadiusParam
{
    public string circleType;
    public float radius;

    public CircleRadiusParam(string type, float r)
    {
        circleType = type;
        radius = r;
    }
}

/// <summary>
/// 采用模拟circle、以及circle之间的公切线，模拟履带
/// </summary>
[RequireComponent(typeof(PBSplineTrackAnimator))]
public class PBSplineTrack : PBSpline
{
    public override SplineMode Mode
    {
        get { return SplineMode.Track; }
    }
    
    public GameObject Prefab;
    public float Scale = 1;
    public float StartOffset = 0;
    
    public float Spacing = 0.32f;
    public float Deviation = 0;
    
    public PBCircleCurveType CurveType;
    public PBCircle[] Circles;

    private PBCircleCurve circleCurve;

    public PBCircleCurve Curve
    {
        get { return circleCurve; }
    }
    
    /// <summary>
    /// 节点个数
    /// </summary>
    public int ChildCount
    {
        get { return transform.childCount; }
    }
    
    /// <summary>
    /// 是否平铺履带
    /// </summary>
    public bool IsSpread { get; set; }

    public bool ShowWindow { get; set; }

    protected override void Init()
    {
        circleCurve = new PBCircleCurve();
    }

    public void SetCircles(PBCircle[] circles, PBCircleCurveType curveType)
    {
        CurveType = curveType;
        Circles = circles;
        circleCurve.Build(Circles, CurveType);
        ForceUpdate();
    }

    private List<TrackChildInfo> transInfos = new List<TrackChildInfo>();

    public List<TrackChildInfo> ComputeTrack()
    {
        transInfos.Clear();

        if (!circleCurve.IsBuilt)
            circleCurve.Build(Circles, CurveType);

        if (IsSpread)
            return ComputeTrackSpread();

        float distance = 0;
        float totalLength = circleCurve.Length;
        int index = 0;
        while (distance <= totalLength)
        {
            Vector3 pos, tangent, up, forward;
            float curveDistance = (StartOffset + distance) - totalLength * Mathf.FloorToInt((StartOffset + distance) / totalLength);
            circleCurve.GetSampleAtDistance(curveDistance, out pos, out tangent, out up);

            TrackChildInfo tInfo = new TrackChildInfo();
            tInfo.distOnCurve = curveDistance;
            
            // scale 
            tInfo.scale = Scale * Vector3.one;
            // rotate, get forward from left and up
            forward = Vector3.Cross(up, tangent);
            tInfo.rotation = Quaternion.LookRotation(forward, up);
            
            // move along spline, according to spacing 
            tInfo.position = pos + forward * Deviation;

            transInfos.Add(tInfo);

            index++;
            distance += Spacing;
        }

        return transInfos;
    }

    /// <summary>
    /// 履带平铺计算
    /// </summary>
    private List<TrackChildInfo> ComputeTrackSpread()
    {
        //find the curve node that is parallel to the circle's right axis
        Vector3 midPos, midTangent, midUp;
        float midDistance;
        circleCurve.GetBottomMidSample(out midPos, out midTangent, out midUp, out midDistance);
        
        Vector3 midForward = Vector3.Cross(midUp, midTangent);
        Quaternion midRotation = Quaternion.LookRotation(midForward, midUp);

        Vector3 startPos = midPos - midTangent * 0.5f * circleCurve.Length;
        Vector3 scale = Scale * Vector3.one;
        
        float distance = 0;
        float totalLength = circleCurve.Length;
        int index = 0;
        while (distance <= totalLength)
        {
            TrackChildInfo tInfo = new TrackChildInfo();
            tInfo.distOnCurve = distance;
            tInfo.position = startPos + midTangent * distance + midForward * Deviation;
            tInfo.scale = scale;
            tInfo.rotation = midRotation;
            
            transInfos.Add(tInfo);
            
            index++;
            distance += Spacing;
        }
        
        return transInfos;
    }

    protected override void Create()
    {
        if (Spacing <= 0 || Prefab == null || Circles == null || Circles.Length == 0)
            return;

        ComputeTrack();

        int index = 0;
        while (index < transInfos.Count)
        {
            Transform child = null;
            if (index >= transform.childCount)
            {
                GameObject go = Instantiate(Prefab, transform, false);
                child = go.transform;
                child.name = Prefab.name + "_" + index;
            }
            else
            {
                child = transform.GetChild(index);
            }

            TrackChildInfo tInfo = transInfos[index];
            child.position = tInfo.position;
            child.rotation = tInfo.rotation;
            child.localScale = tInfo.scale;

            index++;
        }

        //delete unused
        int startIndex = transform.childCount - 1;
        for (int j = startIndex; j >= index; j--)
        {
            DestroyImmediate(transform.GetChild(j).gameObject);
        }
    }

    protected override string SaveString(string format)
    {
        StringBuilder sb = new StringBuilder("track:");

        //save the circles
        sb.Append(Spacing.ToString(format) + ";");
        sb.Append((int) CurveType + ";");
        for (int i = 0; i < Circles.Length; i++)
        {
            PBAnimNode pb = Circles[i].transform.GetComponent<PBAnimNode>();
            sb.AppendFormat("{0},{1}", pb.pbID, Circles[i].radius);
            if (i < Circles.Length - 1)
                sb.Append(",");
            else sb.Append(";");
        }
        sb.Append(Deviation.ToString(format) + ";");

        return sb.ToString();
    }

    protected override void LoadString(string data)
    {
        this.ShowWindow = false;
        
        string realData = data.Substring("track:".Length);
        string[] values = realData.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

        Spacing = float.Parse(values[0]);
        CurveType = (PBCircleCurveType) int.Parse(values[1]);
        Deviation = values.Length >= 4 ? float.Parse(values[3]) : 0;

        values = values[2].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        PBAnimNode[] blocks = GameObject.FindObjectsOfType<PBAnimNode>();
        PBCircle[] circles = new PBCircle[values.Length / 2];
        for (int i = 0; i < circles.Length; i++)
        {
            PBAnimNode block = blocks.FirstOrDefault(b => string.Equals(b.pbID, values[i * 2]));
            if (block == null)
                Debug.LogError(">>>>>履带：找不到齿轮，pbID: " + values[i * 2]);

            if (block.transform.localScale != Vector3.one)
                Debug.LogError(">>>>>履带：齿轮gameobject的localscale不为1");
            
            float radius = float.Parse(values[i * 2 + 1]);
#if BLOCK_EDITOR
            if (!Mathf.Approximately(block.transform.lossyScale.x, 1))
                Debug.LogError(">>>>>搭建积木块不允许有缩放，请从根节点开始检查，并修正！");
#else
            //app中在创建搭建前，可能会缩放根节点    
            radius *= block.transform.lossyScale.x;
#endif
            PBCircle circle = new PBCircle(block.transform, radius);
            circles[i] = circle;
        }

        SetCircles(circles, CurveType);
    }

    public static PBSplineTrack CreateNew(GameObject blockObj)
    {
        PBSplineTrack track = blockObj.GetComponent<PBSplineTrack>();
        if (track == null)
            track = blockObj.AddComponent<PBSplineTrack>();

        track.Prefab = blockObj.transform.Find("track").gameObject;
        
        #if UNITY_EDITOR
        track.ShowWindow = true;
        #endif

        return track;
    }

    /// <summary>
    /// 换轮子
    /// </summary>
    public static void SwitchCircle(GameObject oldCircleObj, GameObject newCircleObj)
    {
        PBSplineTrack[] tracks = UnityEngine.Object.FindObjectsOfType<PBSplineTrack>();
        foreach (PBSplineTrack track in tracks)
        {
            if (track.Circles != null && track.Circles.Length > 0)
            {
                foreach (PBCircle circle in track.Circles)
                {
                    if (circle.transform == oldCircleObj.transform)
                    {
                        circle.transform = newCircleObj.transform;
                        break;
                    }
                }
            }
        }
    }
}