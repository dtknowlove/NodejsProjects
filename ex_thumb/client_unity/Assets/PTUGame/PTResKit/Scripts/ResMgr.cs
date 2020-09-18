/****************************************************************************
 * Copyright (c) 2018 ptgame@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using PTGame.Core;
using UnityEngine;

namespace PTGame.ResKit
{
    public class ResMgr : PTMonoSingleton<ResMgr>
    {
        private Dictionary<string, AbstractRes> mResDict = new Dictionary<string, AbstractRes>();
        private bool mIsResMapDirty = false;

        private Queue<ResTask> mResTask = new Queue<ResTask>();
        private int mCurTaskCount = 0;
        private const int MAX_COTASK_COUNT = 8;
        
        #if UNITY_EDITOR
        //for debug
        [SerializeField] private List<LoadedABInfo> m_LoadedABList;
        #endif

        public bool IsInited
        {
            get { return ResDataMgr.Instance.IsInited; }
        }

        public void Init()
        {
            StartCoroutine(ResDataMgr.Instance.Init());
        }

        /// <summary>
        /// Only call in UnityEditor!!!
        /// </summary>
        public void SimulateInit()
        {
            ResDataMgr.Instance.SimulateInit();
        }

        public void LoadResGroup(string groupName, Action onLoaded = null)
        {
            ResTask resTask = new ResTask(ResDataMgr.Instance.LoadResGroup(groupName));
            StartCoroutine(resTask.Start(onLoaded));
        }

        /// <summary>
        /// Release ResGroup
        /// Be careful, it will unload all resources in the group
        /// </summary>
        public void ReleaseResGroup(string groupName)
        {
            bool success = ResDataMgr.Instance.ReleaseResGroup(groupName);
            if (!success) return;
            
            //first release all zero ref resources
            if (mIsResMapDirty)
            {
                mIsResMapDirty = false;
                CleanResMap(res => res.RefCount <= 0);
            }

            //release all res with groupName
            //1. unload ab
            CleanResMap(res => res.ResType == eResType.Assetbundle && string.Equals(res.Name.ABGroup, groupName));
            //2. unload others
            CleanResMap(res => string.Equals(res.Name.ABGroup, groupName));
            //3. clean all resloaders
            ResLoader.CleanAll();
            GlobalResLoader.CleanAll();
            
            GetAllLoadedAssetBundles();
        }

        public void SetDirty()
        {
            mIsResMapDirty = true;
        }

        public T GetRes<T>(ResName resName, bool createNew = false) where T : AbstractRes
        {
            return GetRes(resName, createNew) as T;
        }

        public AbstractRes GetRes(ResName resName, bool createNew = false)
        {
            AbstractRes res;
            if (mResDict.TryGetValue(resName.FullName, out res))
                return res;

            if (!createNew)
                return null;

            res = ResFactory.Create(resName);
            if (res != null)
            {
                mResDict.Add(resName.FullName, res);
                GetAllLoadedAssetBundles();
            }
            return res;
        }

        public void PostResTask(ResTask task)
        {
            mResTask.Enqueue(task);
            StartNextResTask();
        }

        private void StartNextResTask()
        {
            if (mResTask.Count == 0 || mCurTaskCount == MAX_COTASK_COUNT)
                return;
            
            ResTask task = mResTask.Dequeue();
            mCurTaskCount++;
            StartCoroutine(task.Start(() =>
            {
                mCurTaskCount--;
                StartNextResTask();
            }));
        }

        private void LateUpdate()
        {
            if (mIsResMapDirty)
            {
                mIsResMapDirty = false;
                CleanResMap(res => res.RefCount <= 0);
                GetAllLoadedAssetBundles();
            }
        }

        private void CleanResMap(Func<AbstractRes, bool> condition)
        {
            string[] keys = mResDict.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                AbstractRes res = mResDict[keys[i]];
                if (condition(res))
                {
                    if (res.ReleaseRes())
                    {
                        mResDict.Remove(keys[i]);
                        ResFactory.Recycle(res);    
                    }
                }
            }
        }

        private void GetAllLoadedAssetBundles()
        {
#if UNITY_EDITOR
            //for debug
            if (m_LoadedABList == null)
                m_LoadedABList = new List<LoadedABInfo>();
            m_LoadedABList.Clear();
            foreach (var res in mResDict.Values)
            {
                if (res.ResType == eResType.Assetbundle)
                    m_LoadedABList.Add(new LoadedABInfo(res.Name.ABGroup, res.Name.ABName, res.RefCount));
            }
#endif
        }

        #region Simulation Mode

        static int mSimulationMode = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles"; //此处跟editor中保持统一，不能随意更改

        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        public static bool SimulationMode
        {
            get
            {
                #if UNITY_EDITOR
                if (mSimulationMode == -1)
                    mSimulationMode = UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;
                return mSimulationMode != 0;
                #else
                return false;
                #endif
            }
            set
            {
                #if UNITY_EDITOR
                int newValue = value ? 1 : 0;
                if (newValue != mSimulationMode)
                {
                    mSimulationMode = newValue;
                    UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
                #endif
            }
        }

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("PuTaoTool/ResKit/Simulation Mode")]
        private static void SwitchSimulationMode()
        {
            SimulationMode = !SimulationMode;
        }

        [UnityEditor.MenuItem("PuTaoTool/ResKit/Simulation Mode", true)]
        private static bool SwitchSimulationModeValidate()
        {
            UnityEditor.Menu.SetChecked("PuTaoTool/ResKit/Simulation Mode", SimulationMode);
            return true;
        }        
        #endif
        #endregion
    }
}