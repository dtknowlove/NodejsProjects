using System.Collections.Generic;
using PTGame.Core;
using UnityEngine;
using UnityEngine.Events;

namespace PTGame.ResKit
{
    public class GlobalResLoader : PTSingleton<GlobalResLoader>
    {
        private GlobalResLoader(){}
        
        private Dictionary<string, ResLoader> mResLoaderDict = new Dictionary<string, ResLoader>();

        public override void OnSingletonInit()
        {
            SafeObjectPool<ResLoader>.Instance.MaxCacheCount = 100;
        }

        private ResLoader GetResLoader(ResName resName, bool allocate = true)
        {
            ResLoader loader;
            if (!mResLoaderDict.TryGetValue(resName.FullName, out loader))
            {
                if (allocate)
                {
                    loader = ResLoader.Allocate();
                    mResLoaderDict.Add(resName.FullName, loader);
                }
            }
            return loader;
        }

        private void RemoveResLoader(ResName resName)
        {
            mResLoaderDict.Remove(resName.FullName);
        }

        
        #region static public interfaces

        /// <summary>
        /// Load resource synchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public static object LoadSync(string name, string abName = null)
        {
            ResName resName = ResDataMgr.Instance.GetResName(name, abName);
            return Instance.GetResLoader(resName).LoadSync(name, abName);
        }

        /// <summary>
        /// Load resource synchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public static T LoadSync<T>(string name, string abName = null) where T : class
        {
            return LoadSync(name, abName) as T;
        }

        /// <summary>
        /// Load resource asynchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        /// <param name="onFinish">on finish callback, with param: success boolean, and loaded object.</param>
        public static void LoadAsync(string name, string abName = null, UnityAction<bool, object> onFinish = null)
        {
            ResName resName = ResDataMgr.Instance.GetResName(name, abName);
            Instance.GetResLoader(resName).LoadAsync(name, abName, onFinish);
        }

        /// <summary>
        /// Load resource asynchronously
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        /// <param name="onFinish">on finish callback, with param: success boolean, and loaded object.</param>
        public static void LoadAsync<T>(string name, string abName = null, UnityAction<bool, T> onFinish = null) where T : class
        {
            if (onFinish == null) LoadAsync(name, abName);
            else LoadAsync(name, abName, (success, obj) => onFinish(success, obj as T));
        }

        /// <summary>
        /// Load sprite synchronously
        /// </summary>
        /// <param name="spriteName">sprite name with extension, e.g. "a.png"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public static Sprite LoadSprite(string spriteName, string abName = null)
        {
            ResName resName = ResDataMgr.Instance.GetResName(spriteName, abName);
            return Instance.GetResLoader(resName).LoadSprite(spriteName, abName);
        }

        /// <summary>
        /// Load from Resources folder synchronously 
        /// </summary>
        public static object LoadFromResourcesSync(string name)
        {
            ResName resName = ResDataMgr.Instance.GetResName(ResName.RESOURCES_PREFIX + name, null);
            return Instance.GetResLoader(resName).LoadFromResourcesSync(name);
        }

        /// <summary>
        /// Load from Resources folder synchronously 
        /// </summary>
        public static T LoadFromResourcesSync<T>(string name) where T : class
        {
            ResName resName = ResDataMgr.Instance.GetResName(ResName.RESOURCES_PREFIX + name, null);
            return Instance.GetResLoader(resName).LoadFromResourcesSync<T>(name);
        }

        /// <summary>
        /// Load from Resources folder asynchronously
        /// </summary>
        public static void LoadFromResourcesAsync(string name, UnityAction<bool, object> onFinish = null)
        {
            ResName resName = ResDataMgr.Instance.GetResName(ResName.RESOURCES_PREFIX + name, null);
            Instance.GetResLoader(resName).LoadFromResourcesAsync(name, onFinish);
        }
        
        /// <summary>
        /// Load from Resources folder asynchronously
        /// </summary>
        public static void LoadFromResourcesAsync<T>(string name, UnityAction<bool, T> onFinish = null) where  T : class
        {
            ResName resName = ResDataMgr.Instance.GetResName(ResName.RESOURCES_PREFIX + name, null);
            Instance.GetResLoader(resName).LoadFromResourcesAsync(name, onFinish);
        }

        /// <summary>
        /// Load file synchronously, including: text, texture, bytes files
        /// </summary>
        public static FileContent LoadFileSync(string name)
        {
            ResName resName = ResDataMgr.Instance.GetResName(ResName.FILE_PREFIX + name, null);
            return Instance.GetResLoader(resName).LoadFileSync(name);
        }

        /// <summary>
        /// Load file asynchronously, including: text, texture, bytes files
        /// </summary>
        public static void LoadFileAsync(string name, UnityAction<bool, FileContent> onFinish = null)
        {
            ResName resName = ResDataMgr.Instance.GetResName(ResName.FILE_PREFIX + name, null);
            Instance.GetResLoader(resName).LoadFileAsync(name, onFinish);
        }

        /// <summary>
        /// release resource, also release all its dependencies
        /// </summary>
        /// <param name="name">res name with extension, e.g. "a.prefab"</param>
        /// <param name="abName">optional assetbundle name, in case of same naming.</param>
        public static void Release(string name, string abName = null)
        {
            ResName resName = ResDataMgr.Instance.GetResName(name, abName);
            ResLoader loader = Instance.GetResLoader(resName, false);
            if (loader != null)
            {
                loader.Recycle2Cache();
                Instance.RemoveResLoader(resName);
                return;
            }
            
            Debug.LogFormat("<color=yellow> >>>[ResKit]- GlobalResLoader.Release: Can't find res {0} loaded.</color>", resName.FullName);
        }

        public static void ReleaseFromResources(string name)
        {
            Release(ResName.RESOURCES_PREFIX + name);
        }

        public static void ReleaseFile(string name)
        {
            Release(ResName.FILE_PREFIX + name);
        }

        /// <summary>
        /// clean all recycled resloaders
        /// </summary>
        public static void CleanAll()
        {
            foreach (string key in Instance.mResLoaderDict.Keys)
            {
                if (Instance.mResLoaderDict[key].IsRecycled)
                    Instance.mResLoaderDict.Remove(key);
            }
        }

        #endregion
    }
}