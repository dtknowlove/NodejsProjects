using System;
using System.Collections;
using System.Collections.Generic;
using PTGame.Core;

namespace PTGame.ResKit
{
    public class ResDataMgr : PTSingleton<ResDataMgr>
    {
        protected ResDataMgr() {}

        private List<ResDataGroup> mResGroups = new List<ResDataGroup>();

        private bool mIsInited = false;
        public bool IsInited
        {
            get { return mIsInited; }
        }

        public IEnumerator Init()
        {
            mIsInited = false;

            if (ResMgr.SimulationMode)
            {
                SimulateInit();
                yield break;
            }
            
            List<ResGroupConfig> abGroups = !ResMgr.SimulationMode ? ResKitConfig.GetResGroups() : null;
            mResGroups = new List<ResDataGroup>();
            if (abGroups == null || abGroups.Count == 0)
            {
                mResGroups.Add(new ResDataGroup(""));
            }
            else
            {
                abGroups.ForEach(group =>
                {
                    if (group.DefaultLoad)
                        mResGroups.Add(new ResDataGroup(group.GroupName));
                });
            }

            foreach (ResDataGroup group in mResGroups)
                yield return group.Load();

            mIsInited = true;
        }

        public void SimulateInit()
        {
            #if UNITY_EDITOR
            mResGroups = new List<ResDataGroup>()
            {
                new ResDataGroup("")
            };
            mResGroups[0].SimulateLoad();
            ResMgr.Instance.StartCoroutine(SimulateInitAB());
            #endif
        }

        private IEnumerator SimulateInitAB()
        {
            List<ResGroupConfig> abGroups = ResKitConfig.GetResGroups();
            abGroups.ForEach(group =>
            {
                if (group.SimulateLoad)
                    mResGroups.Add(new ResDataGroup(group.GroupName));
            });
            for (int i = 1; i < mResGroups.Count; i++)
            {
                yield return mResGroups[i].Load();
            }
            mIsInited = true;
        }

        public IEnumerator LoadResGroup(string groupName)
        {
            //SimulationMode doesn't need to load again, because all data is loaded on init
            if (ResMgr.SimulationMode)
                yield break;

            if (mResGroups.Exists(_ => string.Equals(_.GroupName, groupName)))
                yield break;

            ResDataGroup group = new ResDataGroup(groupName);
            mResGroups.Add(group);
            yield return group.Load();
        }

        public bool ReleaseResGroup(string groupName)
        {
            //SimulationMode can't unload, because all data is in one group
            if (ResMgr.SimulationMode)
                return false;
            
            ResDataGroup group = mResGroups.Find(_ => string.Equals(_.GroupName, groupName));
            if (group == null)
                return false;

            group.Release();
            mResGroups.Remove(group);
            return true;
        }

        public ResName GetResName(string assetName, string abName)
        {
            if (string.IsNullOrEmpty(assetName) && string.IsNullOrEmpty(abName))
                return null;

            //file name should keep letter case
            if (!string.IsNullOrEmpty(assetName) && ResName.NotABAsset(assetName))
            {
                int index = assetName.LastIndexOf('.');
                if (index < 0)
                    return new ResName(null, assetName, null);
                return new ResName(null, assetName.Substring(0, index), assetName.Substring(index));
            }

            if (!string.IsNullOrEmpty(assetName))
                assetName = assetName.ToLower();
            if (!string.IsNullOrEmpty(abName))
                abName = abName.ToLower();

            ResName resName = null;
            SearchResNameFailInfo failInfo = null;
            for (int i = 0; i < mResGroups.Count; i++)
            {
                resName = mResGroups[i].GetResName(assetName, abName, out failInfo);
                if (resName != null)
                    return resName;

                if (failInfo.FailType == SearchResNameFailInfo.NAME_AMBIGUOUS)
                    throw new Exception(string.Format(failInfo.ToString(), assetName));
            }
            if (failInfo.FailType == SearchResNameFailInfo.NOTFOUND_AB)
                throw new Exception(string.Format(failInfo.ToString(), abName));
            if (failInfo.FailType == SearchResNameFailInfo.NOTFOUND_ASSET)
                throw new Exception(string.Format(failInfo.ToString(), assetName));
            return null;
        }

        public string GetABUrl(string abName)
        {
            if (string.IsNullOrEmpty(abName))
                return null;

            abName = abName.ToLower();
            for (int i = 0; i < mResGroups.Count; i++)
            {
                var url = mResGroups[i].GetABUrl(abName);
                if (url != null)
                    return url;
            }
            return null;
        }

        /// <summary>
        /// get the dependencies for abName
        /// </summary>
        /// <param name="recursive">If false, get only direct dependencies; if true, get all dependencies including indirect ones.</param>
        public ResName[] GetABDepencecies(string abName, bool recursive = false)
        {
            if (string.IsNullOrEmpty(abName))
                return null;

            abName = abName.ToLower();
            for (int i = 0; i < mResGroups.Count; i++)
            {
                ResName[] dependencies = mResGroups[i].GetABDepencecies(abName, recursive);
                if (dependencies != null)
                    return dependencies;
            }
            return null;
        }
    }   
}