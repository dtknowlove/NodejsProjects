/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 * 特殊功能零件数据封装，以及对外统一接口，e.g. spline、transmission...
 ****************************************************************************/

using System.Xml;
using UnityEngine;
using DG.Tweening;
using System;

public class PPVersatileInfo
{
    public string SplineInfo;
}

public interface IPBVersatileAnimator
{
    int KeyframeCount { get; }
    string[] KeyframeLiterals { get; }
    int[] KeyframeValues { get; }
    
    void SetKeyframe(int index);
    Sequence Play(int index, float speed, Action onComplete);
    void Stop();

    void ResetToStartOfKeyframe(int index);
    void SetEditorKeyframe();
}

public static class PBVersatileInterface
{
    public static void SaveToXml(GameObject obj, XmlDocument xml, XmlElement parent)
    {
        //1. spline
        string splineStr = PBSpline.SaveToString(obj);
        if (!string.IsNullOrEmpty(splineStr))
        {
            XmlElement ele = xml.CreateElement("spline");
            ele.SetAttribute("data", splineStr);
            parent.AppendChild(ele);
        }

        //零件个数
        int blockCount = PBSpline.GetBlockCount(obj);
        if (blockCount > 1)
            parent.SetAttribute("count", blockCount.ToString());
    }

    public static void LoadFromXml(XmlNode parent, PPVersatileInfo info)
    {
        //1. spline
        XmlNode splineNode = parent.SelectSingleNode("spline");
        if (splineNode != null)
            info.SplineInfo = splineNode.Attributes["data"].Value;
    }

    public static bool BuildFromInfo(GameObject obj)
    {
        PBAnimNode pbNode = obj.GetComponent<PBAnimNode>();
            
        //1. spline
        if (pbNode.animNodeInfo.VersatileInfo.SplineInfo != null)
        {
            Renderer[] renderers = pbNode.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                renderer.enabled = true;

            PBSpline.CreateFromString(pbNode.gameObject, pbNode.animNodeInfo.VersatileInfo.SplineInfo);
            return true;
        }

        return false;
    }

    public static bool OnLoadBlockObj(GameObject blockObj, PPVersatileInfo info, bool hide)
    {
        //1. spline
        if (!string.IsNullOrEmpty(info.SplineInfo))
        {
            Renderer[] renderers = blockObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                renderer.enabled = !hide;
            return true;
        }

        return false;
    }
}
