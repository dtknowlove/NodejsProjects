/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public interface IPBSplineAnimator : IPBVersatileAnimator
{
}

[Serializable]
public class PBSplineKeyframe
{
    public List<SplineNode> nodes;
}

public class PBSplineAnimator : MonoBehaviour, IPBSplineAnimator
{
    public List<PBSplineKeyframe> Keyframes = new List<PBSplineKeyframe>();
    public int CurIndex { get; private set; }

    private Spline m_Spline = null;
    private Spline spline
    {
        get { return m_Spline ?? (m_Spline = GetComponent<Spline>()); }
    }

    public int KeyframeCount
    {
        get { return Keyframes.Count; }
    }

    public string[] KeyframeLiterals
    {
        get
        {
            string[] literals = new string[KeyframeCount];
            for (int i = 0; i < KeyframeCount; i++)
            {
                literals[i] = "keyframe_" + i.ToString();
            }
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
    
    public void Clear()
    {
        Keyframes.Clear();
    }

    public void AddKeyframe(List<SplineNode> nodes)
    {
        List<SplineNode> list = new List<SplineNode>();
        foreach (SplineNode node in nodes)
        {
            list.Add(new SplineNode(node.position, node.direction));
        }
        Keyframes.Add(new PBSplineKeyframe()
        {
            nodes = list
        });
    }

    public void RemoveKeyframe(int index)
    {
        if (index >= 0 && index < Keyframes.Count)
            Keyframes.RemoveAt(index);
    }

    public void MoveKeyframe(int indexBefore, int indexAfter)
    {
        if (indexBefore == indexAfter)
            return;

        var temp = Keyframes[indexBefore];
        Keyframes.RemoveAt(indexBefore);
        indexAfter = Mathf.Clamp(indexAfter, 0, Keyframes.Count);
        Keyframes.Insert(indexAfter, temp);
    }

    public void ModifyKeyframe(List<SplineNode> nodes, int index)
    {
        if (Keyframes.Count > index)
        {
            List<SplineNode> list = new List<SplineNode>();
            foreach (SplineNode node in nodes)
            {
                list.Add(new SplineNode(node.position, node.direction));
            }
            PBSplineKeyframe keyframe = Keyframes[index];
            keyframe.nodes = list;
        }
    }

    public void SetKeyframe(int index)
    {
        if (index < 0 || index >= Keyframes.Count)
        {
            GetComponent<PBSpline>().ForceUpdate();
            return;
        }
        
        List<SplineNode> targetNodes = Keyframes[index].nodes;
        if (targetNodes.Count > spline.nodes.Count)
        {
            Debug.LogWarningFormat("Spline nodes' count is less than keyframe_{0} nodes' count, so extra nodes will be created.", index);
            for (int i = spline.nodes.Count; i < targetNodes.Count; i++)
            {
                SplineNode node = targetNodes[i];
                spline.AddNode(new SplineNode(node.position, node.direction));
            }
        }
        else if (targetNodes.Count < spline.nodes.Count)
        {
            Debug.LogWarningFormat("Spline nodes' count is more than keyframe_{0} nodes' count, so extra nodes will be deleted.", index);
            for (int i = targetNodes.Count; i < spline.nodes.Count; i++)
            {
                spline.RemoveNode(spline.nodes[i]);
            }
        }

        for (int i = 0; i < targetNodes.Count; i++)
        {
            spline.nodes[i].SetPosition(targetNodes[i].position);
            spline.nodes[i].SetDirection(targetNodes[i].direction);
        }

        CurIndex = index;
    }

    private Sequence tweenSeq = null;

    public Sequence Play(int index, float speed, Action onComplete)
    {
        if (index < 0 || index >= Keyframes.Count)
            return null;

        tweenSeq = DOTween.Sequence();

        if (index == 0)
        {
            SetKeyframe(0);
            return tweenSeq;
        }

        SetKeyframe(index - 1);

        List<SplineNode> targetNodes = Keyframes[index].nodes;
        if (targetNodes.Count > spline.nodes.Count)
        {
            Debug.LogWarningFormat("Spline nodes' count is less than keyframe_{0} nodes' count, so extra nodes will be created.", index);
            SplineNode lastNode = spline.nodes[spline.nodes.Count - 1];
            for (int i = spline.nodes.Count; i < targetNodes.Count; i++)
            {
                spline.AddNode(new SplineNode(lastNode.position, lastNode.direction));
            }
        }
        else if (targetNodes.Count < spline.nodes.Count)
        {
            Debug.LogWarningFormat("Spline nodes' count is more than keyframe_{0} nodes' count, so extra nodes will be deleted.", index);
            for (int i = targetNodes.Count; i < spline.nodes.Count; i++)
            {
                spline.RemoveNode(spline.nodes[i]);
            }
        }
        
        for (int i = 0; i < targetNodes.Count; i++)
        {
            SplineNode node = spline.nodes[i];
            SplineNode targetNode = targetNodes[i];

            Tweener posTweener = DOTween.To(() => node.position, x => node.SetPosition(x), targetNode.position, speed);
            if (i == 0)
            {
                tweenSeq.Append(posTweener);
            }
            else
            {
                tweenSeq.Join(posTweener);
            }

            Tweener dirTweener = DOTween.To(() => node.direction, x => node.SetDirection(x), targetNode.direction, speed);
            tweenSeq.Join(dirTweener);
        }

        tweenSeq.OnComplete(() =>
        {
            tweenSeq = null;
            if (onComplete != null)
                onComplete();
        });

        CurIndex = index;
        return tweenSeq;
    }

    public void Stop()
    {
        if (tweenSeq != null)
            tweenSeq.Kill();
        if (playCoroutine != null)
            StopCoroutine(playCoroutine);
    }

    public void ResetToStartOfKeyframe(int index)
    {
        SetKeyframe(index - 1);
    }

    public void SetEditorKeyframe()
    {
        SetKeyframe(KeyframeCount - 1);
    }

    private Coroutine playCoroutine = null;
    public void PlayAll(float speed, Action onComplete)
    {
        playCoroutine = StartCoroutine(PlayAllItor(speed, onComplete));
    }

    private IEnumerator PlayAllItor(float speed, Action onComplete)
    {
        int index = 1;
        while (index < Keyframes.Count)
        {
            Play(index, speed, null);
            while (tweenSeq != null)
                yield return null;

            index++;
        }
        
        playCoroutine = null;
        
        if (onComplete != null)
            onComplete();
    }
}
