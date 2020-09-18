using System;
using System.Collections;
using UnityEngine;

namespace PTGame.ResKit
{
    public class FileContent
    {
        public Texture2D texture;
        //public AssetBundle assetbundle;
        public string text;
        public byte[] bytes;
    }

    public class SingleFileRes : AbstractRes
    {
        private string mUrl;
        
        protected override void OnInit()
        {
            mUrl = Name.FullName.Substring(ResName.FILE_PREFIX.Length);
            if (!mUrl.Contains("://"))
            {
                mUrl = "file://" + mUrl;
            }
        }
        
        protected override bool OnLoadSync()
        {
            return false;
        }

        protected override IEnumerator OnLoadAsync()
        {
            //开启的时候已经结束了
            if (RefCount <= 0)
            {
                OnLoadFail();
                yield break;
            }

            WWW www = new WWW(mUrl);
            yield return www;

            bool failed = false;
            if (www.error != null)
            {
                Debug.LogError(">>>[ResKit]: Failed to load www: " + mUrl + "\n" + www.error);
                failed = true;
            }
            else if (!www.isDone)
            {
                Debug.LogError(">>>[ResKit]: Failed to load www: " + mUrl + "\nwww Not Done!");
                failed = true;
            }
            else if (RefCount <= 0)
            {
                failed = true;
            }

            if (failed)
            {
                OnLoadFail();

                www.Dispose();
                www = null;
                yield break;
            }

            mAsset = new FileContent();
            if (mUrl.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || mUrl.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                ((FileContent) mAsset).texture = www.texture;
            }
            /* currently not supported, please use ABRes instead
            else if (www.assetBundle != null)
            {
                ((SingleFileContent) mAsset).assetbundle = www.assetBundle;
            }*/
            else
            {
                ((FileContent) mAsset).text = www.text;
                ((FileContent) mAsset).bytes = www.bytes;
            }

            www.Dispose();
            www = null;

            OnLoadSuccess();
        }

        protected override void OnReleaseRes()
        {
            if (mAsset != null)
            {
                FileContent content = mAsset as FileContent;
                if (content.texture != null)
                {
                    GameObject.Destroy(content.texture);
                    content.texture = null;
                }
                mAsset = null;
            }
        }
    }
}