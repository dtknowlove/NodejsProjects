/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;
using PTGame;

namespace Putao.PaiBloks.Common
{
    public class PBBlockConfigManager
    {
        public static PPBlockConfigInfo LoadBlockInfos(string configFile, bool loadAnim = true)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                return null;
            }

#if BLOCK_EDITOR
            string configText = File.ReadAllText(configFile);
#else
            bool isApp = true;
            string persistentPath=null;
 #if BLOCK_MODEL|| BLOCK_EDITOR
            isApp = false;
            persistentPath = Application.persistentDataPath;
#else
            persistentPath = PTUGame.persistentDataPath;
 #endif
#if BLOCK_MODEL
            string filePath = configFile;    
#else
            string basePath = isApp ? persistentPath : "ftpres";
            string filePath = string.Format("{0}/config_paibloks_buildanim/" + configFile, basePath);
#endif
            
            if (!File.Exists(filePath))
            {
                Debug.LogError("LoadBlockInfos 文件不存在:"+filePath);
                return null;
            }
            string configText =  File.ReadAllText(filePath,Encoding.UTF8);

          
#endif
            return InnerLoadBlockInfos(configFile, configText, loadAnim);
        }

        public static PPBlockConfigInfo LoadBlockInfosWithoutAnim(string configFile)
        {
            return LoadBlockInfos(configFile, false);
        }
        
        public static PPBlockConfigInfo LoadBlockInfosWithoutAnim(string configFile,string configText)
        {
            if (string.IsNullOrEmpty(configFile) || string.IsNullOrEmpty(configText))
            {
                return null;
            }
            return InnerLoadBlockInfos(configFile, configText, false);
        }

        public static PPBlockConfigInfo LoadBlockInfosFromLocalFile(string configFile, bool loadAnim = true)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                return null;
            }
            string configText = File.ReadAllText(configFile);
            return InnerLoadBlockInfos(configFile, configText, loadAnim);
        }

        private static PPBlockConfigInfo InnerLoadBlockInfos(string configName, string configText, bool loadAnim)
        {
            PPBlockConfigInfo blockConfigInfo = new PPBlockConfigInfo();
            
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(configText);

            blockConfigInfo.ConfigName = configName;
            PPLiteracy.LoadGlobalInfo(xml, blockConfigInfo);
            PPLiteracy.LoadBlockInfo(xml, blockConfigInfo);
            PBPrefabLiteracy.LoadPrefabInfo(xml, blockConfigInfo);

            if (loadAnim)
                PPLiteracy.LoadFrameInfo(xml, blockConfigInfo);

            xml = null;
            return blockConfigInfo;
        }

        /// <summary>
        /// 获取传入的config列表中有传入零件的列表
        /// </summary>
        /// <param name="unitName">零件名</param>
        /// <param name="configNames">被查询的列表</param>
        /// <returns></returns>
        public static List<string> GetConfigNamesOfOwnUnit(string unitName,List<string> configNames)
        {
            List<string> resNames=new List<string>();

            List<PPBlockConfigInfo> configInfoList = configNames.Select(t => LoadBlockInfos(t)).ToList();
            bool findInAnimNode;
            foreach (PPBlockConfigInfo blockConfigInfo in configInfoList)
            {
                findInAnimNode = false;
                findInAnimNode = blockConfigInfo.AnimNodeInfos.Values.ToList().Any(t =>
                {
                    if (t.Type == NodeType.Block)
                    {
                        return ((PPBlockInfo) t).Prefab.Equals(unitName);
                    }
                    return false;
                });
                if (findInAnimNode)
                {
                    resNames.Add(blockConfigInfo.ConfigName);
                }
            }
            return resNames;
        }

        /// <summary>
        /// 获取config的所有零件列表
        /// </summary>
        public static List<PBPartInfo> GetAllItems(string configFile)
        {
            PPBlockConfigInfo blockConfig = LoadBlockInfos(configFile);
            return blockConfig.BlockInfos;
        }
    }
}