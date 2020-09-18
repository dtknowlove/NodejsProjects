using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PTGame.Core;
using UnityEngine;

namespace PTGame.ResKit
{
    using AssetBundle = UnityEngine.AssetBundle;
    
    public class ResDataGroup
    {
        /// <summary>
        /// ab包分组名，即根目录名，默认为""
        /// </summary>
        public readonly string GroupName;

        private AssetBundle mABManifestAB;
        private AssetBundleManifest mABManifest;

        /// <summary>
        /// 只记录ab，key值为ab name
        /// </summary>
        private Dictionary<string, ResName> mABDict;

        /// <summary>
        /// 只记录asset，key值为asset name (without extension)
        /// </summary>
        private Dictionary<string, List<ResName>> mAssetDict;

        public ResDataGroup(string groupName)
        {
            this.GroupName = groupName ?? "";
        }

        public IEnumerator Load()
        {
//            if (ResMgr.SimulationMode)
//            {
//                SimulateLoad();
//                yield break;
//            }

            //1. load AssetBundleManifest
            string url = GetABUrl(string.IsNullOrEmpty(GroupName) ? PTPlatformUtil.GetPlatformName() : GroupName, true);
            mABManifestAB = AssetBundle.LoadFromFile(url);
            if (mABManifestAB == null)
            {
                Debug.LogError(">>>[ResKit]: AssetBundleManifest is not valid, please rebuild assetbundles.");
                yield break;
            }
            mABManifest = mABManifestAB.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            //mABManifestAB.Unload(false);
            
            //2. load res data
            url = GetAssetConfigUrl();
            string jsonText = null;
            if (!url.Contains("://"))
            {
                jsonText = File.ReadAllText(url);
            }
            else
            {
                using (WWW www = new WWW(url))
                {
                    yield return www;
                    if (www.isDone)
                    {
                        if (!string.IsNullOrEmpty(www.error))
                        {
                            Debug.LogError(">>>[ResKit]: resdata.json is not valid.");
                            yield break;
                        }
                        jsonText = www.text;
                    }
                }
            }

            //3. parse res data
            ParseResData(JsonUtility.FromJson<ABDataList>(jsonText));
        }

        public void SimulateLoad()
        {
#if UNITY_EDITOR
            ABDataList dataList = new ABDataList();
            dataList.abInfos = new List<ABData>();

            string[] abs = UnityEditor.AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < abs.Length; i++)
            {
                string[] assets = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(abs[i]);
                if (assets.Length == 0)
                    continue;
                ABData abData = new ABData
                {
                    name = abs[i],
                    assets = assets
                };
                dataList.abInfos.Add(abData);
            }
            ParseResData(dataList);
#endif
        }

        public void Release()
        {
            if (!ResMgr.SimulationMode)
            {
                mABManifestAB.Unload(true);
            }

            mABDict.Clear();
            mABDict = null;

            mAssetDict.Clear();
            mAssetDict = null;
        }

        private void ParseResData(ABDataList dataList)
        {
            mABDict = new Dictionary<string, ResName>();
            mAssetDict = new Dictionary<string, List<ResName>>();

            foreach (ABData abInfo in dataList.abInfos)
            {
                ResName abName = new ResName(GroupName, abInfo.name, null, null);
                mABDict.Add(abName.ABName, abName);
                
                foreach (string asset in abInfo.assets)
                {
                    ResName assetName = new ResName(GroupName, abInfo.name, Path.GetFileNameWithoutExtension(asset), Path.GetExtension(asset));
                    List<ResName> resList;
                    if (!mAssetDict.TryGetValue(assetName.AssetName, out resList))
                    {
                        resList = new List<ResName>();
                        mAssetDict.Add(assetName.AssetName, resList);
                    }
                    else if (resList.Exists(r => r.Equals(assetName)))
                    {
                        throw new Exception(string.Format(">>>[ResKit]: assetbundle \"{0}\" 中存在同名且同后缀文件: {1}", assetName.ABName,
                            assetName.AssetNameWithExtension));
                    }
                    resList.Add(assetName);
                }
            }
        }
        
        public ResName GetResName(string assetName, string abName, out SearchResNameFailInfo failInfo)
        {
            failInfo = null;

            if (string.IsNullOrEmpty(assetName))
            {
                ResName resName;
                if (mABDict.TryGetValue(abName, out resName))
                    return resName;

                failInfo = new SearchResNameFailInfo(SearchResNameFailInfo.NOTFOUND_AB);
                return null;
            }

            string nameWithoutExt = assetName;
            string ext = null;
            int index = assetName.LastIndexOf('.');
            if (index > 0)
            {
                nameWithoutExt = assetName.Substring(0, index);
                ext = assetName.Substring(index);
            }

            List<ResName> resList;
            if (!mAssetDict.TryGetValue(nameWithoutExt, out resList))
            {
                failInfo = new SearchResNameFailInfo(SearchResNameFailInfo.NOTFOUND_ASSET);
                return null;
            }

            ResName result = null;
            foreach (ResName resName in resList)
            {
                if (!string.IsNullOrEmpty(abName) && !abName.Equals(resName.ABName, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!string.IsNullOrEmpty(ext) && !ext.Equals(resName.Extension, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (result != null)
                {
                    failInfo = new SearchResNameFailInfo(SearchResNameFailInfo.NAME_AMBIGUOUS);
                    return null;
                }
                
                result = resName;
            }
            if (result == null)
                failInfo = new SearchResNameFailInfo(SearchResNameFailInfo.NOTFOUND_ASSET);
            
            return result;
        }
        
        public string GetABUrl(string abName, bool isManifest = false)
        {
            if (!isManifest && !mABDict.ContainsKey(abName))
                return null;
            
            string relativeDir = "AssetBundles/" + PTPlatformUtil.GetPlatformName();
            if (!string.IsNullOrEmpty(GroupName))
                relativeDir += "/" + GroupName;
            relativeDir += "/" + abName;
            
            string url = Path.Combine(Application.persistentDataPath, relativeDir);

            //先找可写路径
            if (File.Exists(url))
                return url;

            //再找内存
            url = Path.Combine(Application.streamingAssetsPath, relativeDir);
            return url;
        }

        public string GetAssetConfigUrl()
        {
            string relativeDir = "AssetBundles/" + PTPlatformUtil.GetPlatformName();
            if (!string.IsNullOrEmpty(GroupName))
                relativeDir += "/" + GroupName;
            relativeDir += "/assetconfig.json";
            
            string url = Path.Combine(Application.persistentDataPath, relativeDir);

            //先找可写路径
            if (File.Exists(url))
                return url;

            //再找内存
            url = Path.Combine(Application.streamingAssetsPath, relativeDir);
            return url;
        }

        public ResName[] GetABDepencecies(string abName, bool recursive = false)
        {
            if (!mABDict.ContainsKey(abName))
                return null;

            string[] dependencies;

#if UNITY_EDITOR
            if (ResMgr.SimulationMode)
            {
                dependencies = UnityEditor.AssetDatabase.GetAssetBundleDependencies(abName, recursive);

                foreach (var group in ResKitConfig.GetResGroups())
                {
                    if (group.SimulateLoad && group.GroupName == GroupName && group.SimulateLoad)
                    {
                        dependencies = recursive ? mABManifest.GetAllDependencies(abName) : mABManifest.GetDirectDependencies(abName);
                    }
                }
            }
            else
            #endif
            {
                dependencies = recursive ? mABManifest.GetAllDependencies(abName) : mABManifest.GetDirectDependencies(abName);
            }

            ResName[] resNames = new ResName[dependencies.Length];
            for (int i = 0; i < dependencies.Length; i++)
            {
                SearchResNameFailInfo failInfo;
                resNames[i] = GetResName(null, dependencies[i], out failInfo);
            }
            return resNames;
        }
    }
}