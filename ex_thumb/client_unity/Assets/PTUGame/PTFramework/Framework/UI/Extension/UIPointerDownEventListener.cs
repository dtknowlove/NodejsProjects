/****************************************************************************
 * Copyright (c) 2017 yuanhuibin@putao.com
 ****************************************************************************/

namespace PTGame.Framework
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    
    public class UIPointerDownEventListener : MonoBehaviour,IPointerDownHandler
    {
        public Action<PointerEventData> OnPointerDownEvent;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (OnPointerDownEvent != null) OnPointerDownEvent(eventData);
        }

        public static UIPointerDownEventListener CheckAndAddListener(GameObject obj)
        {
            var listener = obj.GetComponent<UIPointerDownEventListener>();
            if (listener == null) listener = obj.AddComponent<UIPointerDownEventListener>();

            return listener;
        }
        public static  UIPointerDownEventListener Get(GameObject obj)
        {
            return CheckAndAddListener (obj);
        }

        private void OnDestroy()
        {
            OnPointerDownEvent = null;
        }
    }
    
    public static class UIPointerDownEventListenerExtension
    {
        public static void RegisterOnPointerDown(this GameObject selfObj, Action<PointerEventData> onPointerUpAction)
        {
            UIPointerDownEventListener.Get(selfObj).OnPointerDownEvent += onPointerUpAction;
        }

        public static void RegisterOnPointerDown<T>(this T selfComponent, Action<PointerEventData> onPointerUpAction)
            where T : Component
        {
            UIPointerDownEventListener.Get(selfComponent.gameObject).OnPointerDownEvent += onPointerUpAction;
        }

        public static void UnRegisterOnPointerDown(this GameObject selfObj, Action<PointerEventData> onPointerUpAction)
        {
            UIPointerDownEventListener.Get(selfObj).OnPointerDownEvent -= onPointerUpAction;
        }

        public static void UnRegisterOnPointerDown<T>(this T selfComponent, Action<PointerEventData> onPointerUpAction)
            where T : Component
        {
            UIPointerDownEventListener.Get(selfComponent.gameObject).OnPointerDownEvent -= onPointerUpAction;
        }
    }
}