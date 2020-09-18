using System.Collections;
using UnityEngine;

namespace PTGame.ResKit
{
    public class InternalRes : AbstractRes
    {
        protected override bool OnLoadSync()
        {
            string assetName = Name.AssetName.Substring(ResName.RESOURCES_PREFIX.Length);
            
            mAsset = Resources.Load(assetName);
            if (mAsset == null)
            {
                Debug.LogError(">>>[ResKit]: Failed to Sync Resources.Load: " + assetName);
                OnLoadFail();
                return false;
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

            string assetName = Name.AssetName.Substring(ResName.RESOURCES_PREFIX.Length);
            
            mLoadRequest = Resources.LoadAsync(assetName);
            yield return mLoadRequest;

            if (!mLoadRequest.isDone)
            {
                Debug.LogError(">>>[ResKit]: Failed to Async Resources.Load: " + assetName);
                OnLoadFail();
                yield break;
            }

            mAsset = ((ResourceRequest)mLoadRequest).asset;
            OnLoadSuccess();
            mLoadRequest = null;
        }

        protected override void OnReleaseRes()
        {
            if (mAsset != null)
            {
                if (!(mAsset is GameObject))
                {
                    Resources.UnloadAsset((Object)mAsset);
                }
                mAsset = null;
            }
        }
    }
}