/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using UnityEngine;


/// <summary>
/// 零件上的贴图（丝印）
/// </summary>
public class PBTexture : MonoBehaviour, IPBNode
{
    [HideInInspector] public string type;
    [HideInInspector] public PPTextureInfo info;

    public void Init()
    {
        transform.localPosition = info.EditorPos;
        transform.localEulerAngles = info.EditorAngle;
        type = info.Prefab;
    }
}