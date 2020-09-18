/****************************************************************************
 * Copyright (c) 2018.5 liqingyun
 ****************************************************************************/

namespace PTGame.Framework
{
    using System;

    /// <summary>
    /// 可能是个 Action
    /// </summary>
    public class UITransition
    {
        // serc panel  dst panel
        // do transition? make transition
        
    }

    public static class UITransitionExtension
    {
        /// <summary>
        /// 绑定跳转逻辑
        /// </summary>
        /// <param name="selfPanel"></param>
        /// <param name="btn"></param>
        /// <typeparam name="T"></typeparam>
        public static Action Transition<TDstPanel>(this UIView selfBehaviour,IUIData uidata = null) where TDstPanel : UIView
        {
            return () =>
            {
                UIMgr.ClosePanel(selfBehaviour.name);
                UIMgr.OpenPanel<TDstPanel>(uidata);
            };
        }

        // TODO:这里要想办法抽象下
        public static void Do(this Action action)
        {
            action();
        }
        
        public static void Start(this Action action)
        {
            action();
        }
        
        public static void Begin(this Action action)
        {
            action();
        }
    }
}