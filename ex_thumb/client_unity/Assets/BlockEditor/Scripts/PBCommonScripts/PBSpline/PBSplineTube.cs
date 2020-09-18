/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using UnityEngine;

public class PBSplineTube : PBSplineMesh
{
    public override SplineMode Mode
    {
        get { return SplineMode.Tube; }
    }
    
    protected override int meshChildStartIndex
    {
        get { return 2; }
    }

    protected override string SaveStrHead()
    {
        return "tube:";
    }
    
    public class RefpointInfo
    {
        public Transform trans;

        public Vector3 lastPos;
        public Vector3 lastDir;

        public float nodeDirLen;
    }

    private RefpointInfo[] refPointInfos = new RefpointInfo[2];

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
        
        refPointInfos[0] = new RefpointInfo();
        refPointInfos[1] = new RefpointInfo();
        refPointInfos[1].trans = transform.GetChild(0);
        refPointInfos[0].trans = transform.GetChild(1);

        if (Mesh == null)
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            MeshRenderer mr = GetComponent<MeshRenderer>();

            Mesh = mf.sharedMesh;
            Material = mr.sharedMaterial;

            Component.DestroyImmediate(mf);
            Component.DestroyImmediate(mr);

            float x = 0.5f;

            if (spline.nodes.Count>1)
            {
                spline.nodes[0].SetPosition(new Vector3(-x, 0, 0), true);
                spline.nodes[1].SetPosition(new Vector3(x, 0, 0), true);
                spline.nodes[0].SetDirection(new Vector3(-x + 0.2f, 0, 0), true);
                spline.nodes[1].SetDirection(new Vector3(x + 0.2f, 0, 0), false);
            }
 
        }
        
        
#if BLOCK_EDITOR
        //add block align 
        for (int i = 1; i <= 2; i++)
        {
            Transform head = transform.Find("head_" + i);
            if (head.GetComponent<PEBlockAlign>() == null)
                head.gameObject.AddComponent<PEBlockAlign>();
        }
#endif
        needUpdate = true;
    }

    private Vector3[] refPointPos = new Vector3[2];
    private Vector3[] refPointDir = new Vector3[2];
    
    private void AdjustEndpoint()
    {
        if (spline.nodes.Count == 0)
            return;
        
        for (int i = 0; i < 2; i++)
        {
            SplineNode node = spline.nodes[i == 0 ? 0 : spline.nodes.Count - 1];
            RefpointInfo rInfo = refPointInfos[i];
            rInfo.trans.localPosition = node.position;
            Vector3 dir = node.direction - node.position;
            rInfo.nodeDirLen = dir.magnitude;

            dir.Normalize();
            rInfo.trans.right = transform.TransformDirection(dir);

            rInfo.lastPos = rInfo.trans.position;
            rInfo.lastDir = rInfo.trans.forward;
        }
    }
    
    protected override void OnUpdate()
    {
        for (int i = 0; i < 2; i++)
        {
            SplineNode node = spline.nodes[i == 0 ? 0 : spline.nodes.Count - 1];
            RefpointInfo rInfo = refPointInfos[i];

            if (rInfo.trans.position != rInfo.lastPos || rInfo.trans.forward != rInfo.lastDir)
            {
                rInfo.lastPos = rInfo.trans.position;
                rInfo.lastDir = rInfo.trans.forward;

                Vector3 dir = transform.InverseTransformDirection(rInfo.trans.right);
                node.SetDirection(dir * rInfo.nodeDirLen + node.position, true);
                node.SetPosition(rInfo.trans.localPosition);
            }
        }
    }
    
    public static PBSplineTube CreateNew(GameObject blockObj)
    {
        return blockObj.AddComponent<PBSplineTube>();
    }
}