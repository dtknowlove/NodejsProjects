/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

namespace PTGame.Framework
{
    using UnityEngine;

    public enum UIMarkType
    {
        DefaultUnityElement,
        Element,
        Component
    }
    
    public interface IUIMark
    {
        UIMarkType GetUIMarkType();
        
        string ComponentName { get; }
        
        Transform Transform { get; }
    }
}