/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Putao.PaiBloks.Common;

public static class PBBlockConfigExtension 
{
    /// <summary>
    /// 根据动画帧索引获取帧内block信息
    /// </summary>
    /// <param name="blockConfigInfo"></param>
    /// <param name="frameIndex">帧索引</param>
    /// <returns></returns>
    public static List<PBPartInfo> GetPartInfo(this PPBlockConfigInfo blockConfigInfo, int frameIndex)
    {
        List<PBPartInfo> resList = new List<PBPartInfo>();
        Action<PPBlockInfo> addPartInfo = (blockInfo) =>
        {
            if (blockInfo.Hide)
                return;
            PBPartInfo info = resList.Find(p => p.ThumbName.Equals(blockInfo.Thumb));
            if (info == null)
            {
                resList.Add(new PBPartInfo(blockInfo, blockInfo.Count));
            }
            else
            {
                info.Count += blockInfo.Count;
            }
        };

        //获取当前帧所有是零件的targetId
        var tarId = blockConfigInfo.KeyfameInfos[frameIndex].itemInfos
            .Where(t => t.IsUnit)
            .Select(t => t.targetId)
            .ToList();

        foreach (int i in tarId)
        {
            PPAnimNodeInfo pni;
            if (!blockConfigInfo.AnimNodeInfos.TryGetValue(i, out pni))
                continue;

            if (pni.Type == NodeType.Block)
            {
                addPartInfo(pni as PPBlockInfo);
            }
            else
            {
                PPSectionInfo psi = pni as PPSectionInfo;
                List<PPBlockInfo> blockInfos = GetBlockInfoBySection(psi);
                foreach (PPBlockInfo blockInfo in blockInfos)
                {
                    addPartInfo(blockInfo);
                }
            }
        }

        return resList;
    }

    /// <summary>
    /// 获取全部零件的名称和使用个数信息
    /// </summary>
    public static List<PBPartInfo> GetAllPartInfo(this PPBlockConfigInfo blockConfigInfo)
    {
        return blockConfigInfo.BlockInfos;
    }

    /// <summary>
    /// 根据步骤获取当前步所有是零件的名称
    /// </summary>
    /// <param name="blockConfigInfo"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static List<string> GetPartNameByStep(this PPBlockConfigInfo blockConfigInfo,int step)
    {
        List<string> result=new List<string>();
        if (step < 0 || step >= blockConfigInfo.KeyfameInfos.Count)
        {
            return null;
        }

        var tarId = blockConfigInfo.KeyfameInfos[step].itemInfos
            .Where(t => t.IsUnit)
            .Select(t => t.targetId)
            .ToList();
        
        foreach (int index in tarId)
        {
            string name=((PPBlockInfo)blockConfigInfo.AnimNodeInfos[index]).Prefab;
            if (!result.Contains(name))
                result.Add(name);
        }
        
        return result;
    }

    /// <summary>
    /// 根据步骤获取当前步骤所有是零件的ID
    /// </summary>
    /// <param name="blockConfigInfo"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static List<int> GetPartIDByStep(this PPBlockConfigInfo blockConfigInfo, int step)
    {
        if (step < 0 || step >= blockConfigInfo.KeyfameInfos.Count)
        {
            return null;
        }
        List<int> result = new List<int>();

        result = blockConfigInfo.KeyfameInfos[step].itemInfos
            .Where(t => t.IsUnit)
            .Select(t => t.targetId)
            .ToList();

        return result;
    }

    private static List<PPBlockInfo> GetBlockInfoBySection(PPSectionInfo sectionInfo)
    {
        List<PPBlockInfo> res=new List<PPBlockInfo>();

        List<PPAnimNodeInfo> nodeInfos = sectionInfo.NodeInfos;
        int length = nodeInfos.Count;
        for (int i = 0; i < length; i++)
        {
            if (nodeInfos[i].Type == NodeType.Block)
            {
                res.Add(nodeInfos[i] as PPBlockInfo);
            }
            else
            {
                res.AddRange(GetBlockInfoBySection(nodeInfos[i] as PPSectionInfo));
            }
        }
        return res;
    }
}