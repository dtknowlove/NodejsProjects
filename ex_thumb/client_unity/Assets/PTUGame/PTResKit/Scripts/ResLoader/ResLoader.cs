using System;
using System.Collections.Generic;
using System.Linq;
using PTGame.Core;
using UnityEngine;
using UnityEngine.Events; 

namespace PTGame.ResKit
{
    public class ResLoader : IPoolable, IPoolType
    {
        private List<AbstractRes> mResList = new List<AbstractRes>();
        private LinkedList<AbstractRes> mWaitLoadList = new LinkedList<AbstractRes>();
        
        //finish callbacks for async load
        private Dictionary<AbstractRes, List<UnityAction<bool, object>>> mFinishCbDict =
            new Dictionary<AbstractRes, List<UnityAction<bool, object>>>();

        public float Progress
        {
            get
            {
                float unit = 1.0f / mResList.Count;
                return mResList.Sum(res => unit * res.Progress);
            }
        }

        #region Recycle

        private static List<ResLoader> mResLoaderList = new List<ResLoader>();

        public static ResLoader Allocate()
        {
            ResLoader loader = SafeObjectPool<ResLoader>.Instance.Allocate();
            mResLoaderList.Add(loader);
            return loader;
        }

        public bool IsRecycled { get; set; }
        
        public void OnRecycled()
        {
            ReleaseAll();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<ResLoader>.Instance.Recycle(this);
            mResLoaderList.Remove(this);
        }

        public static void CleanAll()
        {
            for (int i = mResLoaderList.Count - 1; i >= 0; i--)
                mResLoaderList[i].Clean();
        }

        #endregion

        private AbstractRes AddToLoad(ResName resName)
        {
            AbstractRes res = mResList.Find(r => r.Name.Equals(resName));
            if (res != null)
                return res;

            res = ResMgr.Instance.GetRes(resName, true);

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            ResName[] depends = res.GetDependencies();
            if (depends != null)
            {
                for (int i = 0; i < depends.Length; i++)
                    AddToLoad(depends[i]);
            }

            //add to list
            res.AddRef();
            mResList.Add(res);

            if (res.State != eResState.Ready)
            {
                mWaitLoadList.AddLast(res);
            }
            return res;
        }

        private void DoLoadAsync()
        {
            var curNode = mWaitLoadList.First;
            while (curNode != null)
            {
                var node = curNode;
                curNode = curNode.Next;

                AbstractRes res = node.Value;
                //ensure the dependecies are loaded beforehand
                if (res.IsDependencyLoadFinish())
                {
                    mWaitLoadList.Remove(node);

                    if (res.State != eResState.Ready)
                    {
                        res.RegisterListener(OnLoadAsyncFinish);
                        res.LoadAsync();
                    }
                    else
                    {
                        InvokeFinishCb(true, res);
                    }
                }

                //curNode已经被Remove了，说明已经执行过一次DoLoadAsync的递归调用，直接跳出
                if (curNode != null && !mWaitLoadList.Contains(curNode.Value))
                    break;
            }
        }

        private void OnLoadAsyncFinish(bool success, AbstractRes res)
        {
            InvokeFinishCb(success, res);
            res.RemoveListener(OnLoadAsyncFinish);
            DoLoadAsync();
        }

        private void InvokeFinishCb(bool success, AbstractRes res)
        {
            List<UnityAction<bool, object>> callbacks;
            if (mFinishCbDict.TryGetValue(res, out callbacks))
            {
                foreach (UnityAction<bool, object> callback in callbacks)
                {
                    callback(success, res.Asset);
                }
                mFinishCbDict.Remove(res);
            }
        }
        
        
        #region Public Interfaces

        /// <summary>
        /// Load resource synchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public object LoadSync(string name, string abName = null)
        {
            AbstractRes res = AddToLoad(ResDataMgr.Instance.GetResName(name, abName));
            foreach (AbstractRes loadres in mWaitLoadList)
            {
                loadres.LoadSync();
            }
            mWaitLoadList.Clear();
            return res.Asset;
        }

        /// <summary>
        /// Load resource synchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public T LoadSync<T>(string name, string abName = null) where T : class
        {
            return LoadSync(name, abName) as T;
        }

        /// <summary>
        /// Load resource asynchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        /// <param name="onFinish">on finish callback, with param: success boolean, and loaded object.</param>
        public void LoadAsync(string name, string abName = null, UnityAction<bool, object> onFinish = null)
        {
            AbstractRes res = AddToLoad(ResDataMgr.Instance.GetResName(name, abName));
            if (res.State == eResState.Ready)
            {
                if (onFinish != null)
                    onFinish(true, res.Asset);
            }
            else
            {
                if (onFinish != null)
                {
                    if (!mFinishCbDict.ContainsKey(res))
                        mFinishCbDict.Add(res, new List<UnityAction<bool, object>>());
                    mFinishCbDict[res].Add(onFinish);
                }
                DoLoadAsync();
            }
        }

        /// <summary>
        /// Load resource asynchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        /// <param name="onFinish">on finish callback, with param: success boolean, and loaded object.</param>
        public void LoadAsync<T>(string name, string abName = null, UnityAction<bool, T> onFinish = null) where T : class
        {
            if (onFinish == null) LoadAsync(name, abName);
            else LoadAsync(name, abName, (success, obj) => onFinish(success, obj as T));
        }

        /// <summary>
        /// Load sprite synchronously
        /// </summary>
        /// <param name="spriteName">sprite name with extension, e.g. "a.png"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public Sprite LoadSprite(string spriteName, string abName = null)
        {
            if (ResMgr.SimulationMode)
            {
                LoadSync(spriteName, abName);
                AssetRes res = ResMgr.Instance.GetRes<AssetRes>(ResDataMgr.Instance.GetResName(spriteName, abName));
                return res != null ? res.GetSprite() : null;
            }
            else
            {
                return LoadSync(spriteName, abName) as Sprite;
            }
        }

        /// <summary>
        /// Load from Resources folder synchronously 
        /// </summary>
        public object LoadFromResourcesSync(string name)
        {
            return LoadSync(ResName.RESOURCES_PREFIX + name);
        }

        /// <summary>
        /// Load from Resources folder synchronously 
        /// </summary>
        public T LoadFromResourcesSync<T>(string name) where T : class
        {
            return LoadSync<T>(ResName.RESOURCES_PREFIX + name);
        }

        /// <summary>
        /// Load from Resources folder asynchronously
        /// </summary>
        public void LoadFromResourcesAsync(string name, UnityAction<bool, object> onFinish = null)
        {
            LoadAsync(ResName.RESOURCES_PREFIX + name, onFinish: onFinish);
        }
        
        /// <summary>
        /// Load from Resources folder asynchronously
        /// </summary>
        public void LoadFromResourcesAsync<T>(string name, UnityAction<bool, T> onFinish = null) where  T : class
        {
            LoadAsync(ResName.RESOURCES_PREFIX + name, onFinish: onFinish);
        }

        /// <summary>
        /// Load file synchronously, including: text, texture, bytes files
        /// </summary>
        public FileContent LoadFileSync(string name)
        {
            return LoadSync(ResName.FILE_PREFIX + name) as FileContent;
        }

        /// <summary>
        /// Load file asynchronously, including: text, texture, bytes files
        /// </summary>
        public void LoadFileAsync(string name, UnityAction<bool, FileContent> onFinish = null)
        {
            LoadAsync(ResName.FILE_PREFIX + name, onFinish: onFinish);
        }

        /// <summary>
        /// release resource
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        [Obsolete("只会卸载这个asset，如需卸载引用，请在合适的时机调用 Recycle2Cache()")]
        public void Release(string name, string abName = null)
        {
            AbstractRes res = ResMgr.Instance.GetRes(ResDataMgr.Instance.GetResName(name, abName));
            if (res == null)
                return;

            mWaitLoadList.Remove(res);
            mFinishCbDict.Remove(res);

            if (mResList.Remove(res))
            {
                res.SubRef();
                ResMgr.Instance.SetDirty();
            }
        }
        
        public void ReleaseFromResources(string name)
        {
            Release(ResName.RESOURCES_PREFIX + name);
        }

        public void ReleaseFile(string name)
        {
            Release(ResName.FILE_PREFIX, name);
        }

        /// <summary>
        /// release all loaded resources
        /// </summary>
        public void ReleaseAll()
        {
            mWaitLoadList.Clear();
            mFinishCbDict.Clear();

            if (mResList.Count > 0)
            {
                foreach (AbstractRes res in mResList)
                {
                    res.SubRef();
                }
                mResList.Clear();
                ResMgr.Instance.SetDirty();
            }
        }

        /// <summary>
        /// remove all recycled resouces
        /// </summary>
        public void Clean()
        {
            var recycledList = mResList.FindAll(res => res.IsRecycled);
            foreach (var res in recycledList)
            {
                mWaitLoadList.Remove(res);
                mFinishCbDict.Remove(res);
                mResList.Remove(res);
            }
            if (mResList.Count == 0)
            {
                Recycle2Cache();
            }
        }

        #endregion 
    }
}