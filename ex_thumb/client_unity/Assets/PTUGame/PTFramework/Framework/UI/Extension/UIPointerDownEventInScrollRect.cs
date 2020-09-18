/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using PTGame.Core;

    public class UIPointerDownEventInScrollRect : MonoBehaviour,IPointerDownHandler
    {
        public ScrollRect ScrollRect { set; protected get; }

        private UnityAction mOnPointerDownAction;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            mOnPointerDownAction.InvokeGracefully();
        }
    }
}