/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/


using UnityEngine;

/// <summary>
/// only for identification
/// </summary>
public interface IPBNode
{
}

public class PBAnimNode : MonoBehaviour, IPBNode
{
    public string pbID;
    [HideInInspector] public PPAnimNodeInfo animNodeInfo;

    public virtual void Init()
    {
        this.transform.localPosition = animNodeInfo.EditorPos;
        this.transform.localEulerAngles = animNodeInfo.EditorAngle;
        this.pbID = animNodeInfo.Id.ToString();
    }

    /// <summary>
    /// 设置为搭建好的状态
    /// </summary>
    public virtual void SetEditorTransform()
    {
        this.transform.localPosition = animNodeInfo.EditorPos;
        this.transform.localEulerAngles = animNodeInfo.EditorAngle;
    }
}