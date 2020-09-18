/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework
{
    using UnityEngine;
    using PTGame.ResKit;

    /// <summary>
    /// Default
    /// </summary>
    public class DefaultUIPanelLoader : IUIPanelLoader
    {
        ResLoader mResLoader = ResLoader.Allocate();

        public GameObject LoadPanelPrefab(string panelName)
        {
            #if COCOS_SUPPORT
            var retObj = mResLoader.LoadSync<GameObject>(string.Format("Resources/{0}", panelName));
            if (null != retObj) return retObj;
            #endif
            
            return mResLoader.LoadSync<GameObject>(panelName);
        }

        public GameObject LoadPanelPrefab(string assetBundleName, string panelName)
        {
            return mResLoader.LoadSync<GameObject>(panelName, assetBundleName);
        }

        public void Unload()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}