/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework
{
    using UnityEngine;
    
    public interface IUIPanelLoader
    {
        GameObject LoadPanelPrefab(string panelName);

        GameObject LoadPanelPrefab(string assetBundleName, string panelName);
        
        void Unload();
    }
}