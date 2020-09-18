using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PTGame.ResKit
{
    public class SceneRes : AbstractRes
    {
        private ResName[] mABs;
        
        protected override void OnInit()
        {
            ResName abName = ResDataMgr.Instance.GetResName(null, Name.ABName);
            mABs = new[] {abName};
        }

        public override ResName[] GetDependencies()
        {
            return mABs;
        }

        protected override bool OnLoadSync()
        {
            //Can't use simulation load, because the scene has to be added to BuildSettings for loading it when Play.
            //But then the scene will be built into app package, which is not wanted.
            //The scene only needs to be built into assetbundle. 

            ABRes abR = ResMgr.Instance.GetRes<ABRes>(mABs[0]);
            if (abR == null || abR.Assetbundle == null)
            {
                Debug.LogError(">>>[ResKit]: Failed to Load Scene, Can't Find AssetBundle: " + mABs[0].ABName);
                OnLoadFail();
                return false;
            }
            if (!abR.Assetbundle.isStreamedSceneAssetBundle)
            {
                Debug.LogErrorFormat(">>>[ResKit]: Failed to Load Scene, Can't find scene {0} in assetbundle {1}", Name.AssetName, abR.Name);
                OnLoadFail();
                return false;
            }

            SceneManager.LoadScene(Name.AssetName, LoadSceneMode.Additive);
            OnLoadSuccess();
            return true;
        }

        protected override IEnumerator OnLoadAsync()
        {
            ABRes abR = ResMgr.Instance.GetRes<ABRes>(mABs[0]);
            if (abR == null || abR.Assetbundle == null)
            {
                Debug.LogError(">>>[ResKit]: Failed to Load Scene, Can't Find AssetBundle: " + mABs[0].ABName);
                OnLoadFail();
                yield break;
            }
            
            if (!abR.Assetbundle.isStreamedSceneAssetBundle)
            {
                Debug.LogErrorFormat(">>>[ResKit]: Failed to Load Scene, Can't find scene {0} in assetbundle {1}", Name.AssetName, abR.Name);
                OnLoadFail();
                yield break;
            }

            mLoadRequest = SceneManager.LoadSceneAsync(Name.AssetName, LoadSceneMode.Additive);
            yield return mLoadRequest;

            if (mLoadRequest.isDone)
            {
                OnLoadSuccess();
            }
            else
            {
                Debug.LogError(">>>[ResKit]: Failed to Async Load Scene: " + Name.AssetName);
                OnLoadFail();
            }
            
            mLoadRequest = null;
        }

        protected override void OnReleaseRes()
        {
            SceneManager.UnloadSceneAsync(Name.AssetName);
        }
    }
}