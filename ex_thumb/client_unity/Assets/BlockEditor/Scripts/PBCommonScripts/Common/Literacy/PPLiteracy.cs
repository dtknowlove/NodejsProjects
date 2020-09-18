/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using Putao.PaiBloks.Common;
using UnityEngine;

public class PPLiteracy 
{
    #region GlobalData 读入

    public static void LoadGlobalInfo(XmlDocument xml, PPBlockConfigInfo configInfo)
    {
        configInfo.BoundBox = ReadBoundInfo(xml.SelectSingleNode("config/boundbox"));

        XmlNode xn = xml.SelectSingleNode("config/cameraoffset");
        configInfo.CameraOffsetY = xn != null ? float.Parse(xn.Attributes["y"].Value) : PPConstant.CAMERA_OFFSET_Y;
        configInfo.CameraOffsetDepth = xn != null ? float.Parse(xn.Attributes["depth"].Value) : PPConstant.CAMERA_OFFSET_DEPTH;

        xn = xml.SelectSingleNode("config/lighting");
        configInfo.LightName = xn != null ? xn.Attributes["data"].Value : PBLighting.DEFAULT;
    }

    private static Bounds ReadBoundInfo(XmlNode xmlNode)
    {
        float sizex = float.Parse(xmlNode.Attributes["sizex"].Value);
        float sizey = float.Parse(xmlNode.Attributes["sizey"].Value);
        float sizez = float.Parse(xmlNode.Attributes["sizez"].Value);
        float centerx = float.Parse(xmlNode.Attributes["centerx"].Value);
        float centery = float.Parse(xmlNode.Attributes["centery"].Value);
        float centerz = float.Parse(xmlNode.Attributes["centerz"].Value);
        return new Bounds(new Vector3(centerx, centery, centerz), new Vector3(sizex, sizey, sizez));
    }

    #endregion
    
    #region Block 读入
    
    /// <summary>
    /// 开放接口：加载block config info
    /// </summary>
    public static void LoadBlockInfo(XmlDocument xml, PPBlockConfigInfo configInfo)
    {
        LoadBlockInfo(xml, configInfo.AnimNodeInfos, configInfo.SectionInfo);
    }

    /// <summary>
    /// 开放接口：加载effect config info
    /// </summary>
    public static void LoadBlockInfo(XmlDocument xml, PPEffectConfigInfo configInfo)
    {
        LoadBlockInfo(xml, configInfo.AnimNodeInfos, configInfo.SectionInfo);
    }

    /// <summary>
    /// 非开放接口
    /// </summary>
    private static void LoadBlockInfo(XmlDocument xml, Dictionary<int, PPAnimNodeInfo> animNodeInfos, PPSectionInfo parentSection)
    {
        var node = xml.SelectSingleNode ("config").SelectSingleNode ("section");
        ProcessNodes(node,animNodeInfos,parentSection);
    }

    private static void ProcessNodes(XmlNode node, Dictionary<int, PPAnimNodeInfo> animNodeInfos, PPSectionInfo parentSection)
    {
        XmlNodeList sections = node.SelectNodes("section");

        for (int i = 0; i < sections.Count; i++)
        {
            XmlNode sectionNode = sections[i];
            PPSectionInfo sectionInfo = new PPSectionInfo();
            sectionInfo.Index = sectionInfo.Id = int.Parse(sectionNode.Attributes["id"].Value);
            sectionInfo.Name = sectionNode.Attributes["name"] != null ? sectionNode.Attributes["name"].Value : "section_" + sectionInfo.Index;
            sectionInfo.Type = NodeType.Section;

            XmlNode editorNode = sectionNode.SelectSingleNode("editor");
            editorNode.GetPosInfo(out sectionInfo.EditorPos)
                .GetAngleInfo(out sectionInfo.EditorAngle);
                
            //for versatile                                                     
            PBVersatileInterface.LoadFromXml(sectionNode, sectionInfo.VersatileInfo);
            
            animNodeInfos.Add(sectionInfo.Index, sectionInfo);
            parentSection.AddSubNode(sectionInfo);

            ProcessNodes(sectionNode, animNodeInfos, sectionInfo);
        }

        XmlNodeList blocks = node.SelectNodes("block");

        for (int i = 0; i < blocks.Count; i++)
        {
            XmlNode blockNode = blocks[i];
            PPBlockInfo blockInfo = new PPBlockInfo();
            XmlNode editorNode = blockNode.SelectSingleNode("editor");
            editorNode.GetPosInfo(out blockInfo.EditorPos)
                .GetAngleInfo(out blockInfo.EditorAngle);
            
            blockInfo.Prefab = blockNode.Attributes["type"].Value;
            blockInfo.Thumb = blockNode.Attributes["thumb"] != null ? blockNode.Attributes["thumb"].Value : blockInfo.Prefab;
            blockInfo.Detail = blockNode.Attributes["partDetail"] != null ? blockNode.Attributes["partDetail"].Value : "";
            blockInfo.Count = blockNode.Attributes["count"] != null ? int.Parse(blockNode.Attributes["count"].Value) : 1;
            blockInfo.Hide = blockNode.Attributes["hide"] != null && blockNode.Attributes["hide"].Value.Equals("1");
            blockInfo.Type = NodeType.Block;
            blockInfo.Index = blockInfo.Id = int.Parse(blockNode.Attributes["id"].Value);
            
            //for textures
            blockInfo.ParseTextures(blockNode.SelectNodes("texture"));

            //for versatile blocks
            PBVersatileInterface.LoadFromXml(blockNode, blockInfo.VersatileInfo);

            animNodeInfos.Add(blockInfo.Index, blockInfo);
            parentSection.AddSubNode(blockInfo);
        }
    }

    #endregion
    
    #region Effect读入

    public static void LoadEffectInfo(XmlDocument xml,List<PPEffectInfo> effectInfoList)
    {
        var nodeKeys = xml.SelectSingleNode ("config").SelectSingleNode ("effects");
        if (null == nodeKeys)
        {
            Debug.Log("=====>>>>[无特效]");
            return;
        }
        XmlNodeList effectList = nodeKeys.SelectNodes("effect");
        ParseFromData(effectList,effectInfoList);
    }

    private static void ParseFromData(XmlNodeList effectList, List<PPEffectInfo> effectInfoList)
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            XmlNode effectNode = effectList[i];
            EffectType tmpType = (EffectType)Enum.Parse(typeof(EffectType),effectNode.Attributes["effecttype"].Value);
            PPEffectInfo effectInfo=null;
            switch (tmpType)
            {
                case EffectType.Animation:
                    effectInfo = new PPAnimEffectInfo();
                    break;
                case EffectType.Particle:
                    effectInfo=new PPParticleEffectInfo();
                    break;
            }
            effectInfo.Parse(effectNode);
            effectInfoList.Add(effectInfo);
        }
    }
    #endregion

    #region Frame读入

    /// <summary>
    /// 开放接口：加载动画frame info
    /// </summary>
    public static void LoadFrameInfo(XmlDocument xml, PPBlockConfigInfo configInfo)
    {
        LoadFrameInfo(xml, configInfo.KeyfameInfos, configInfo.AnimNodeInfos);
    }
    
    /// <summary>
    /// 非开放接口
    /// 从xml中读入frame信息，用于运行时播放动画
    /// 区别于PBFrameLiteracy.LoadFrameInfo，详见其注释
    /// </summary>
    private static void LoadFrameInfo(XmlDocument xml, List<PPKeyFrameInfo> keyframeInfos, Dictionary<int, PPAnimNodeInfo> animNodeInfos)
    {
        var nodeKeys = xml.SelectSingleNode("config").SelectSingleNode("keys");
        if (null == nodeKeys)
        {
            Debug.Log("=====>>>>[无帧动画]");
            return;
        }
        ProcessKeyframes(nodeKeys, keyframeInfos, animNodeInfos);
    }

    private static void ProcessKeyframes(XmlNode nodeKeys, List<PPKeyFrameInfo> keyframeInfos, Dictionary<int, PPAnimNodeInfo> animNodeInfos)
    {
        XmlNodeList keyframeNodes = nodeKeys.SelectNodes("keyframe");

        for (int i = 0; i < keyframeNodes.Count; i++)
        {
            XmlNode keyFrameNode = keyframeNodes[i];

            PPKeyFrameInfo keyFrameInfo = new PPKeyFrameInfo();
            keyFrameInfo.index = int.Parse(keyFrameNode.Attributes["index"].Value);

            PBPointInfo pointInfo = null;
            if (keyFrameNode.Attributes["keyPointType"] != null)
            {
                PBPointType pointType = (PBPointType) int.Parse(keyFrameNode.Attributes["keyPointType"].Value);
                
                string pointInfoStr = null;
                if (pointType != PBPointType.None && keyFrameNode.Attributes["keyPointInfo"] != null)
                    pointInfoStr = keyFrameNode.Attributes["keyPointInfo"].Value;
                
                string pointTipStr = null;
                if (pointType != PBPointType.None && keyFrameNode.Attributes["keyPointTip"] != null)
                    pointTipStr = keyFrameNode.Attributes["keyPointTip"].Value;
                
                pointInfo = new PBPointInfo(keyFrameInfo.index, pointType, pointInfoStr, pointTipStr);
            }
            else if (keyFrameNode.Attributes["custom"] != null)
            {
                //兼容旧版本逻辑
                pointInfo = new PBPointInfo(keyFrameInfo.index, PBPointType.GuidePoint, keyFrameNode.Attributes["custom"].Value, null);
            }
            keyFrameInfo.pointInfo = pointInfo;
            
            ProcessFrameItems(keyFrameNode.SelectNodes("item"), keyFrameInfo, animNodeInfos);
            keyframeInfos.Add(keyFrameInfo);
        }
    }

    private static void ProcessFrameItems(XmlNodeList itemNodes, PPKeyFrameInfo keyFrameInfo, Dictionary<int, PPAnimNodeInfo> animNodeInfos)
    {
        for (int i = 0; i < itemNodes.Count; i++)
        {
            XmlNode xmlNode = itemNodes[i];
            PPFrameItemInfo frameItemInfo = new PPFrameItemInfo();

            XmlNode iniInfoNode = xmlNode.SelectSingleNode("init");
            iniInfoNode.GetPosInfo(out frameItemInfo.initPos)
                .GetAngleInfo(out frameItemInfo.initAngle);
            frameItemInfo.index = i;
            frameItemInfo.targetId = int.Parse(xmlNode.Attributes["targetid"].Value);

            //根据targetId去处理
            XmlAttribute isunit = xmlNode.Attributes["isunit"];
            if (null == isunit)
            {
                frameItemInfo.IsUnit = animNodeInfos[frameItemInfo.targetId].Type == NodeType.Block;
            }
            else
            {
                frameItemInfo.IsUnit = !isunit.Value.Equals("0");
            }
            
            frameItemInfo.ParseMovements(xmlNode.SelectNodes("movement"));

            keyFrameInfo.AddFrameItemInfo(frameItemInfo);
        }
    }

    #endregion
}
