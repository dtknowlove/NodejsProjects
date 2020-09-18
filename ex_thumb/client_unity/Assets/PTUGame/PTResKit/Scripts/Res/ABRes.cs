using System;
using System.Collections;
using UnityEngine;

namespace PTGame.ResKit
{
    using AssetBundle = UnityEngine.AssetBundle;
    
    public class ABRes : AbstractRes
    {
        private ResName[] mDependencies;
        
        public AssetBundle Assetbundle { get { return mAsset as AssetBundle; } }

        protected override void OnInit()
        {
            mDependencies = ResDataMgr.Instance.GetABDepencecies(Name.ABName);
        }

        public override ResName[] GetDependencies()
        {
            return mDependencies;
        }
        
        public override void OnRecycled()
        {
            base.OnRecycled();
            mDependencies = null;
        }

        protected override bool OnLoadSync()
        {
            if (!CheckSimulateLoad())
            {
                string url = ResDataMgr.Instance.GetABUrl(Name.ABName);
                AssetBundle bundle = AssetBundle.LoadFromFile(url);
                if (bundle == null)
                {
                    Debug.LogError(">>>[ResKit]: Failed to Sync Load AssetBundle: " + url);
                    OnLoadFail();
                    return false;
                }

                mAsset = bundle;
            }
            
            OnLoadSuccess();
            return true;
        }

        protected override IEnumerator OnLoadAsync()
        {
            //开启的时候已经结束了
            if (RefCount <= 0)
            {
                OnLoadFail();
                yield break;
            }

            if (!CheckSimulateLoad())
            {
                string url = ResDataMgr.Instance.GetABUrl(Name.ABName);
                mLoadRequest = AssetBundle.LoadFromFileAsync(url);
                yield return mLoadRequest;

                if (!mLoadRequest.isDone)
                {
                    Debug.LogError(">>>[ResKit]: Failed to Async Load AssetBundle: " + url);
                    OnLoadFail();
                    yield break;
                }

                mAsset = ((AssetBundleCreateRequest) mLoadRequest).assetBundle;
            }

            OnLoadSuccess();
            mLoadRequest = null;
        }

        protected override void OnReleaseRes()
        {
            if (mAsset != null)
            {
                ((AssetBundle)mAsset).Unload(true);
                mAsset = null;
            }
        }

        private bool CheckSimulateLoad()
        {
            if (!ResMgr.SimulationMode)
                return false;

#if UNITY_EDITOR
            string[] assets = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(Name.ABName);
            for (int i = 0; i < assets.Length; i++)
            {
                //isStreamedSceneAssetBundle can't use similation load. Reason is given in SceneRes.cs
                if (assets[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            foreach (var group in ResKitConfig.GetResGroups())
            {
                if (group.GroupName == Name.ABGroup && group.SimulateLoad)
                {
                    return false;
                }
            }
#endif
            return true;
        }
    }
}