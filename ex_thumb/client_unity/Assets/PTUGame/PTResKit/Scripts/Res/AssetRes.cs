using System.Collections;
using UnityEngine;

namespace PTGame.ResKit
{
    public class AssetRes : AbstractRes
    {
        private ResName[] mABs;
        private Sprite mSprite;

        protected override void OnInit()
        {
            ResName abName = ResDataMgr.Instance.GetResName(null, Name.ABName);
            mABs = new[] {abName};
        }

        public override ResName[] GetDependencies()
        {
            return mABs;
        }

        /// <summary>
        /// for use only in Unity Editor in simulation mode
        /// </summary>
        public Sprite GetSprite()
        {
#if UNITY_EDITOR
            Texture2D texture = mAsset as Texture2D;
            if (texture != null && mSprite == null)
            {
                //load sprite information
                string path = UnityEditor.AssetDatabase.GetAssetPath(texture);
                UnityEditor.TextureImporter ti = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
                ti.isReadable = true;
                mSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), ti.spritePivot, ti.spritePixelsPerUnit, 1, SpriteMeshType.Tight, ti.spriteBorder);
            }
#endif
            return mSprite;
        }

        public override void OnRecycled()
        {
            base.OnRecycled();
            mABs = null;
        }

        protected override bool OnLoadSync()
        {
            if (ResMgr.SimulationMode)
            {
                SimulateLoad();
            }
            
            if(mAsset == null)
            {
                ABRes abR = ResMgr.Instance.GetRes<ABRes>(mABs[0]);
                if (abR == null || abR.Assetbundle == null)
                {
                    if (abR != null && abR.State == eResState.Loading)
                        Debug.LogErrorFormat(">>>[ResKit]: Failed to Load Asset. AssetBundle {0} is async loading, please call SyncLoad after.", mABs[0].ABName);
                    else
                        Debug.LogError(">>>[ResKit]: Failed to Load Asset, Can't Find AssetBundle: " + mABs[0].ABName);
                    
                    OnLoadFail();
                    return false;
                }
                mAsset = abR.Assetbundle.LoadAsset(Name.AssetNameWithExtension);
            }

            if (mAsset == null)
            {
                Debug.LogError(">>>[ResKit]: Failed to Load Asset, Can't Find Asset: " + Name.AssetNameWithExtension);
                OnLoadFail();
                return false;
            }
            
            OnLoadSuccess();
            return true;
        }

        protected override IEnumerator OnLoadAsync()
        {
            if (RefCount <= 0)
            {
                OnLoadFail();
                yield break;
            }
            
            if (ResMgr.SimulationMode)
            {
                SimulateLoad();
            }
            if(mAsset == null)
            {
                ABRes abR = ResMgr.Instance.GetRes<ABRes>(mABs[0]);
                if (abR == null || abR.Assetbundle == null)
                {
                    Debug.LogError(">>>[ResKit]: Failed to Load Asset, Can't Find AssetBundle: " + mABs[0].ABName);
                    OnLoadFail();
                    yield break;
                }
                
                //确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
                HoldDependency();

                mLoadRequest = abR.Assetbundle.LoadAssetAsync(Name.AssetNameWithExtension);
                yield return mLoadRequest;
                
                UnHoldDependency();

                if (mLoadRequest.isDone)
                {
                    mAsset = ((AssetBundleRequest) mLoadRequest).asset;
                }
                mLoadRequest = null;
            }

            if (mAsset == null)
            {
                Debug.LogError(">>>[ResKit]: Failed to Async Load Asset: " + Name.AssetNameWithExtension);
                OnLoadFail();
                yield break;
            }
            
            OnLoadSuccess();
        }

        protected override void OnReleaseRes()
        {
            if (mSprite != null)
            {
                GameObject.Destroy(mSprite);
            }

            if (mAsset != null)
            {
                if (!(mAsset is GameObject))
                {
                    Resources.UnloadAsset((Object) mAsset);
                }
                mAsset = null;
            }
        }

        private void SimulateLoad()
        {
            #if UNITY_EDITOR
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(Name.ABName, Name.AssetName);
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (assetPaths[i].ToLower().EndsWith(Name.Extension))
                {
                    mAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[i]);
                    break;
                }
            }
            #endif
        }
    }
}