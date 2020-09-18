/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System.Linq;
using Putao.PaiBloks.Common;
using UnityEngine;

public class PBBlock : PBAnimNode
{
    [HideInInspector] public string type;
    [HideInInspector] public PBPartDetail partDetail = null;
    public bool hide;

    public override void Init()
    {
        base.Init();
        
        PPBlockInfo info = animNodeInfo as PPBlockInfo;
        this.type = info.Prefab;
        this.hide = info.Hide;
        this.partDetail = PBPartDetail.Parse(info.Detail);
    }

    public override void SetEditorTransform()
    {
        base.SetEditorTransform();
        
        PPBlockInfo info = animNodeInfo as PPBlockInfo;
        if (!string.IsNullOrEmpty(info.VersatileInfo.SplineInfo))
        {
            var splineAnim = GetComponentInChildren<IPBSplineAnimator>();
            splineAnim.SetEditorKeyframe();
        }
    }
    
    public PBTexture[] GetTextures()
    {
        return GetComponentsInChildren<PBTexture>(true);
    }

    public string[] GetTextureNames()
    {
        PBTexture[] texes = GetTextures();
        return texes.Select(t => t.type).ToArray();
    }

    private void Start()
    {
        ShowHided(false);
    }

    /// <summary>
    /// 是否显示隐藏辅助block
    /// </summary>
    public void ShowHided(bool show)
    {
        if (hide)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = show;
            }
        }
    }

    /// <summary>
    /// for editor use: 是否显示/隐藏所有辅助block
    /// </summary>
    /// <param name="show"></param>
    public static void ShowHideAll(bool show)
    {
        PBBlock[] blocks = Object.FindObjectsOfType<PBBlock>();
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].ShowHided(show);
        }
    }
}