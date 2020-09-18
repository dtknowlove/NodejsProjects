/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/


using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Xml;
using PTGame.Core;
using Putao.PaiBloks.Common;

#region  keyframe

public class PPKeyFrameInfo
{
    public List<PPFrameItemInfo> itemInfos = new List<PPFrameItemInfo>();

    /// <summary>
    /// 同id
    /// </summary>
    public int index;

    /// <summary>
    /// 节点信息
    /// </summary>
    public PBPointInfo pointInfo;

    public void AddFrameItemInfo(PPFrameItemInfo itemInfo)
    {
        itemInfos.Add(itemInfo);
    }
}

public class PPFrameItemInfo
{
    public int index;
    public int targetId = 0;
    public Vector3 initPos;
    public Vector3 initAngle;

    /// <summary>
    /// 是否是零件
    /// </summary>
    public bool IsUnit;
    
    public List<PPMovementInfo> movements = new List<PPMovementInfo>();

    public void ParseMovements(XmlNodeList xmlNodes)
    {
        for (int i = 0; i < xmlNodes.Count; i++)
        {
            PPMovementInfo movementInfo = new PPMovementInfo();
            movementInfo.Parse(xmlNodes[i]);
            movements.Add(movementInfo);
        }
    }
}

public class PPMovementInfo
{
    public float delay = 0;
    public float duration;

    public PPMovementType type = PPMovementType.Normal;
    
    public Vector3 destPosition;
    public Vector3 destAngle;

    public int destKeyframe = -1;
    
    public void Parse(XmlNode xmlNode)
    {
        delay = float.Parse(xmlNode.Attributes["delay"].Value);
        duration = float.Parse(xmlNode.Attributes["duration"].Value);
        if (xmlNode.Attributes["type"] != null)
            type = (PPMovementType) int.Parse(xmlNode.Attributes["type"].Value);
        if (type == PPMovementType.Normal)
        {
            xmlNode.GetPosInfo(out destPosition).GetAngleInfo(out destAngle);
        }
        else if (type == PPMovementType.Spline || type == PPMovementType.Transmission)
        {
            destKeyframe = int.Parse(xmlNode.Attributes["destSplineKeyframe"].Value);
        }
    }
}

public enum PPMovementType
{
    Normal = 0,
    Spline,
    Animation,    //预留，暂时不用
    Transmission,    //预留，暂时不用
}

#endregion


public enum NodeType
{
    Block,
    Section
}

[System.Serializable]
public class PPAnimNodeInfo
{
    public Vector3 EditorPos;
    public Vector3 EditorAngle;
    public int Index = 0;
    public int Id = 0;
    public NodeType Type;

    //编辑器1.2.0版本之前用
    public List<PPFrameItemInfo> frameItemInfos;
    
    //专用于创建具有特殊功能的零件
    public PPVersatileInfo VersatileInfo = new PPVersatileInfo();
}

public class PPSectionInfo : PPAnimNodeInfo
{
    public string Name;
    public List<PPAnimNodeInfo> NodeInfos = new List<PPAnimNodeInfo>();

    public void AddSubNode(PPAnimNodeInfo nodeInfo)
    {
        if (NodeInfos.Count > 0 && nodeInfo.Index < NodeInfos[NodeInfos.Count - 1].Index)
        {
            NodeInfos.Insert(NodeInfos.Count - 1, nodeInfo);
        }
        else
        {
            NodeInfos.Add(nodeInfo);
        }
    }
}

[System.Serializable]
public class PPBlockInfo : PPAnimNodeInfo
{
    public string Prefab;
    public string Thumb;

    // 零件个数，默认1；特殊情况：履带有多个
    public int Count = 1;
    
    //是否为搭建辅助件，需要隐藏
    public bool Hide;

    //零件文字信息
    public string Detail;
    
    //零件丝印贴图
    public PPTextureInfo[] Textures;
    
    /// <summary>
    /// 是否是贴纸零件
    /// </summary>
    public bool IsSticker
    {
        get { return Prefab.StartsWith("sticker"); }
    }

    /// <summary>
    /// 是否是丝印零件
    /// </summary>
    public bool IsStamp
    {
        get { return Textures.Length > 0; }
    }

    private string mMd5StampName;
    /// <summary>
    /// 获取实际丝印图
    /// </summary>
    public string GetStampThumb
    {
        get { return string.IsNullOrEmpty(mMd5StampName) ? mMd5StampName = PTUtils.Md5Sum(Thumb) : mMd5StampName; }
    }
    
    public void ParseTextures(XmlNodeList nodeList)
    {
        Textures = new PPTextureInfo[nodeList.Count];
        for (int texIndex = 0; texIndex < nodeList.Count; texIndex++)
        {
            PPTextureInfo texInfo = new PPTextureInfo();
            texInfo.Parse(nodeList[texIndex]);
            Textures[texIndex] = texInfo;
        }
    }
}

[System.Serializable]
public class PPTextureInfo
{
    public string Prefab;
    public Vector3 EditorPos;
    public Vector3 EditorAngle;

    public void Parse(XmlNode node)
    {
        Prefab = node.Attributes["type"].Value;
        node.GetPosInfo(out EditorPos).GetAngleInfo(out EditorAngle);
    }
}

[System.Serializable]
public class PPPrefabInfo
{
    public string Name;
    public string Model;
    public string Material;
    public string Texture;
    public string Category;
    public string Thumb;
    public List<PPPrefabTexInfo> Texs;
    public List<MaterialInfo> MaterialInfos;
    
    public PPPrefabInfo()
    {
        Texs = new List<PPPrefabTexInfo>();
        MaterialInfos = new List<MaterialInfo>();
    }

    public bool IsSticker
    {
        get { return Name.ToLower().StartsWith("sticker"); }
    }
    
    public bool IsTexture
    {
        get { return Texs.Count > 0; }
    }

    public override string ToString()
    {
        return string.Format("name:{0} model:{1} material:{2} texture:{3} thumb:{4}", Name, Model, Material, Texture, Thumb);
    }
    
    public PPPrefabTexInfo GetPrefabTexInfo(string name)
    {
        return Texs.FirstOrDefault(t => t.Name.Equals(name));
    }
}

[System.Serializable]
public class PPPrefabTexInfo
{
    public string Name;
    public string Model;
    public string Texture;
    public Vector3 editor_pos;
    public Vector3 editor_angle;
    public string bin_url;
    public string gltf_url;
    public string skin_url;
    
    public override string ToString()
    {
        return string.Format("name:{0} model:{1} texture:{2}", Name, Model, Texture);
    }

    public bool EqualsModelUrl(string url)
    {
        return bin_url.Equals(url) || gltf_url.Equals(url);
    }
}


public class PPBlockConfigInfo
{
    public string ConfigName;
    public PosAngle CameraComplete;
    public PosAngle CameraMoreInfo;
    public Bounds BoundBox;
    public float CameraOffsetY;
    public float CameraOffsetDepth;
    public string LightName;
    
    public Dictionary<int, PPAnimNodeInfo> AnimNodeInfos;
    public PPSectionInfo SectionInfo;
    public List<PPKeyFrameInfo> KeyfameInfos;
    public List<PPPrefabInfo> PrefabInfos;

    public PPBlockConfigInfo()
    {
        AnimNodeInfos = new Dictionary<int, PPAnimNodeInfo>();
        SectionInfo = new PPSectionInfo();
        KeyfameInfos = new List<PPKeyFrameInfo>();
        PrefabInfos = new List<PPPrefabInfo>();
    }

    private int mBlockNum = -1;
    private int mBlockTypeNum = -1;
    private List<PBPartInfo> mBlockInfos;

    public int BlockNum
    {
        get
        {
            if (mBlockNum == -1)
                CalculateBlockNum();
            return mBlockNum;
        }
    }

    public int BlockTypeNum
    {
        get
        {
            if (mBlockTypeNum == -1)
                CalculateBlockNum();
            return mBlockTypeNum;
        }
    }

    /// <summary>
    /// 所有的零件个数
    /// </summary>
    public List<PBPartInfo> BlockInfos
    {
        get
        {
            if (mBlockInfos == null)
                CalculateBlockNum();
            return mBlockInfos;
        }
    }

    private void CalculateBlockNum()
    {
        Dictionary<string, PBPartInfo> blockInfos = new Dictionary<string, PBPartInfo>();
        mBlockNum = 0;
        foreach (PPAnimNodeInfo node in AnimNodeInfos.Values)
        {
            if (node.Type == NodeType.Block)
            {
                PPBlockInfo blockInfo = node as PPBlockInfo;
                if (blockInfo.Hide)
                    continue;
                if (blockInfos.ContainsKey(blockInfo.Thumb))
                    blockInfos[blockInfo.Thumb].Count += blockInfo.Count;
                else
                    blockInfos[blockInfo.Thumb] = new PBPartInfo(blockInfo, blockInfo.Count);

                mBlockNum += blockInfo.Count;
            }
        }
        mBlockInfos = blockInfos.Values.ToList();
        mBlockTypeNum = mBlockInfos.Count;
    }

    public PPPrefabInfo GetPrefabInfoByName(string prefabName)
    {
        return PrefabInfos.FirstOrDefault(t => t.Name.Equals(prefabName));
    }
}

public class PosAngle
{
    public Vector3 Pos;
    public Vector3 Angle;

    public PosAngle()
    {
        Pos = new Vector3();
        Angle = new Vector3();
    }

    public PosAngle(Vector3 p,Vector3 a)
    {
        Pos = p;
        Angle = a;
    }

    public string ToString()
    {
        return string.Format("Pos:{0} Angle:{1}",Pos,Angle);
    }
}
