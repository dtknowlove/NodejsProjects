/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Spline))]
[RequireComponent(typeof(PBSplineAnimator))]
public abstract class PBSplineMesh : PBSpline
{
    public Mesh Mesh;
    public Material Material;
    public float Scale = 1;
    
    protected Spline spline = null;

    protected abstract int meshChildStartIndex { get; }

    protected override void Create()
    {
        if (Mesh == null)
            return;
        
        int index = meshChildStartIndex;
        foreach (CubicBezierCurve curve in spline.GetCurves())
        {
            Transform child = null;
            if (index >= transform.childCount)
            {
                GameObject go = new GameObject("SplineMesh" + (index - meshChildStartIndex), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshBender));
                go.GetComponent<MeshRenderer>().material = Material;
                go.layer = transform.gameObject.layer;
                child = go.transform;
                child.transform.parent = transform;
            }
            else
            {
                child = transform.GetChild(index);
            }

            child.localRotation = Quaternion.identity;
            child.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;

            MeshBender mb = child.GetComponent<MeshBender>();
            mb.SetSourceMesh(Mesh, false);
            //mb.SetRotation(Quaternion.Euler(rotation), false);
            mb.SetCurve(curve, false);
            mb.SetStartScale(Scale, false);
            mb.SetEndScale(Scale);

            index++;
        }

        //delete unused
        int startIndex = transform.childCount - 1;
        for (int j = startIndex; j >= index; j--)
        {
            DestroyImmediate(transform.GetChild(j).gameObject);
        }
    }
    
    protected abstract string SaveStrHead();
    
    protected override string SaveString(string format)
    {
        StringBuilder sb = new StringBuilder(SaveStrHead());

        //save all keyframe nodes
        PBSplineAnimator animator = GetComponent<PBSplineAnimator>();
        if (animator.KeyframeCount == 0)
        {
            //default add current spline node as first keyframe
            animator.AddKeyframe(spline.nodes);
        }

        for (int i = 0; i < animator.KeyframeCount; i++)
        {
            PBSplineKeyframe keyframe = animator.Keyframes[i];
            for (int j = 0; j < keyframe.nodes.Count; j++)
            {
                SplineNode node = keyframe.nodes[j];
                sb.Append(node.position.x.ToString(format) + ",");
                sb.Append(node.position.y.ToString(format) + ",");
                sb.Append(node.position.z.ToString(format) + ",");
                sb.Append(node.direction.x.ToString(format) + ",");
                sb.Append(node.direction.y.ToString(format) + ",");
                sb.Append(node.direction.z.ToString(format));

                if (j < keyframe.nodes.Count - 1)
                    sb.Append(";");
            }

            if (i < animator.KeyframeCount - 1)
                sb.Append("|");
        }

        return sb.ToString();
    }

    protected override void LoadString(string data)
    {
        PBSplineAnimator animator = GetComponent<PBSplineAnimator>();
        animator.Clear();

        string realData = data.Substring(SaveStrHead().Length);
        
        string[] keyframeStrs = realData.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < keyframeStrs.Length; i++)
        {
            List<SplineNode> nodes = new List<SplineNode>();
            string[] nodeStrs = keyframeStrs[i].Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
            int nodeCount = nodeStrs.Length / 6;
            for (int j = 0; j < nodeCount; j++)
            {
                float posx = float.Parse(nodeStrs[j * 6]);
                float posy = float.Parse(nodeStrs[j * 6 + 1]);
                float posz = float.Parse(nodeStrs[j * 6 + 2]);
                float dirx = float.Parse(nodeStrs[j * 6 + 3]);
                float diry = float.Parse(nodeStrs[j * 6 + 4]);
                float dirz = float.Parse(nodeStrs[j * 6 + 5]);
                nodes.Add(new SplineNode(
                    new Vector3(posx, posy, posz),
                    new Vector3(dirx, diry, dirz)));
            }

            animator.AddKeyframe(nodes);
        }

        animator.SetKeyframe(0);
    }
}