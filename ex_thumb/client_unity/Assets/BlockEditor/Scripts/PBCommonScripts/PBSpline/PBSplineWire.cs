/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PBSplineWire : PBSplineMesh
{
    public override SplineMode Mode
    {
        get { return SplineMode.Wire; }
    }
    
    private Transform m_EndPoint;
    private Transform endPoint
    {
        get { return m_EndPoint ?? (m_EndPoint = transform.parent.parent.Find("head_2")); }
    }

    private Vector3 endPointPos;
    private Vector3 endPointDir;
    private float endPointDirLength;
    
    protected override int meshChildStartIndex
    {
        get { return 0; }
    }
    
    protected override string SaveStrHead()
    {
        return "wire:";
    }

    protected override void Init()
    {
        spline = GetComponent<Spline>();
        
        spline.NodeCountChanged.AddListener(() =>
        {
            needUpdate = true;
            AdjustEndpoint();

            //重新register一下，因为curves变了
            foreach (CubicBezierCurve curve in spline.GetCurves())
            {
                curve.Changed.RemoveAllListeners();
                curve.Changed.AddListener(AdjustEndpoint);
            }
        });
        foreach (CubicBezierCurve curve in spline.GetCurves())
        {
            curve.Changed.RemoveAllListeners();
            curve.Changed.AddListener(AdjustEndpoint);
        }

        if (Mesh == null)
        {
            Transform root = transform.parent;
            
            Transform wire = root.Find("wire");
            Mesh = wire.GetComponent<MeshFilter>().sharedMesh;
            Material = wire.GetComponent<MeshRenderer>().sharedMaterial;
        
            GameObject.DestroyImmediate(wire.gameObject);
            
            Transform startPoint = root.Find("point00");

            transform.parent = startPoint;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, -180, -90);
            transform.localScale = Vector3.one;

            AdjustEndpoint();
        }

#if BLOCK_EDITOR
        //add block align 
        Transform head = transform.parent.parent.Find("head_2");
        if (head.GetComponent<PEBlockAlign>() == null)
            head.gameObject.AddComponent<PEBlockAlign>();
#endif
        needUpdate = true;
    }

    private void AdjustEndpoint()
    {
        if (spline.nodes.Count == 0)
            return;
        
        SplineNode endNode = spline.nodes[spline.nodes.Count - 1];
        endPoint.position = transform.TransformPoint(endNode.position);

        Vector3 dir = endNode.direction - endNode.position;
        endPoint.forward = transform.TransformDirection(Vector3.Normalize(dir));

        endPointPos = endPoint.position;
        endPointDir = endPoint.forward;
        endPointDirLength = dir.magnitude;
    }

    protected override void OnUpdate()
    {
        if (endPointPos != endPoint.position || endPointDir != endPoint.forward)
        {
            //Debug.Log(">>>> Update Endpoint and Spline");
            endPointPos = endPoint.position;
            endPointDir = endPoint.forward;
            SplineNode endNode = spline.nodes[spline.nodes.Count - 1];
            endNode.SetPosition(transform.InverseTransformPoint(endPointPos), true);
            endNode.SetDirection(transform.InverseTransformDirection(endPointDir) * endPointDirLength + endNode.position);
        }
    }

    public static PBSplineWire CreateNew(GameObject blockObj)
    {
        GameObject splineObj = new GameObject("spline");
        splineObj.transform.parent = blockObj.transform;
        splineObj.transform.localPosition = Vector3.zero;
        splineObj.transform.localRotation = Quaternion.identity;
        splineObj.transform.localScale = Vector3.one;

        return splineObj.AddComponent<PBSplineWire>();
    }
}