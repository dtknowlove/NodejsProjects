/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using UnityEngine;

public enum SplineMode
{
    Wire = 0,
    Tube,
    Track,
}

public struct SplineNodeParams
{
    public Vector3 position;
    public Vector3 direction;

    public SplineNodeParams(Vector3 pos, Vector3 dir)
    {
        position = pos;
        direction = dir;
    }
}

[ExecuteInEditMode]
[SelectionBase]
public abstract class PBSpline : MonoBehaviour
{
    public abstract SplineMode Mode { get; }
    
    protected bool needUpdate = false;

    protected virtual void Init() {}
    protected abstract void Create();
    protected virtual void OnUpdate() {}
    protected abstract string SaveString(string format);
    protected abstract void LoadString(string data);

    private void OnEnable()
    {
        //Debug.Log(">>>>>> PBSpline OnEnable");
        Init();
    }
    
    private void OnValidate()
    {
        //Debug.Log(">>>>>> PBSpline OnValidate");
        needUpdate = true;
    }

    private void Update()
    {
        if (needUpdate)
        {
            Create();
            needUpdate = false;
        }
        
        OnUpdate();
    }

    public void ForceUpdate()
    {
        Create();
    }

    public void NeedUpdate()
    {
        needUpdate = true;
    }

    #region Public Static Functions

    public static PBSpline CreateNew(GameObject blockObj, string blockType)
    {
        switch (blockType)
        {
            case "wire":
                return PBSplineWire.CreateNew(blockObj);
            case "tube":
                return PBSplineTube.CreateNew(blockObj);
            case "track":
                return PBSplineTrack.CreateNew(blockObj);
        }
        return null;
    }

    public static PBSpline CreateFromString(GameObject blockObj, string data)
    {
        if (string.IsNullOrEmpty(data) || blockObj.GetComponent<PBBlock>() == null)
            return null;

        PBSpline spline = null;
        if (data.StartsWith("wire"))
            spline = PBSplineWire.CreateNew(blockObj);
        if (data.StartsWith("tube"))
            spline = PBSplineTube.CreateNew(blockObj);
        else if (data.StartsWith("track"))
            spline = PBSplineTrack.CreateNew(blockObj);

        if (spline != null)
            spline.LoadString(data);
        return spline;
    }

    //保留4位小数
    private const string FLOAT_STRING_FORMAT = "F4";

    public static string SaveToString(GameObject blockObj)
    {
        //必须是PBBlock
        if (blockObj.GetComponent<PBBlock>() == null)
            return null;
        
        PBSpline pbspline = blockObj.GetComponentInChildren<PBSpline>(true);
        if (pbspline == null)
            return null;

        return pbspline.SaveString(FLOAT_STRING_FORMAT);
    }

    public static void SwitchBlock(GameObject oldBlockObj, GameObject newBlockObj)
    {
        CreateFromString(newBlockObj, SaveToString(oldBlockObj));
        if (oldBlockObj.name.Contains("gears"))
            PBSplineTrack.SwitchCircle(oldBlockObj, newBlockObj);
    }

    /// <summary>
    /// 获取零件个数，默认1；履带特殊：由小零件组成一个完整的履带，需要计算小零件的个数
    /// </summary>
    public static int GetBlockCount(GameObject blockObj)
    {
        PBSplineTrack track = blockObj.GetComponent<PBSplineTrack>();
        return track != null ? track.ChildCount : 1;
    }

    #endregion
}