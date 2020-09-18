/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIPointerUpEventListener : MonoBehaviour, IPointerUpHandler
    {
        public Action<PointerEventData> OnPointerUpEvent;

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (OnPointerUpEvent != null) OnPointerUpEvent(eventData);
        }

        public static UIPointerUpEventListener CheckAndAddListener(GameObject obj)
        {
            var listener = obj.GetComponent<UIPointerUpEventListener>();
            if (listener == null) listener = obj.AddComponent<UIPointerUpEventListener>();

            return listener;
        }

        public static UIPointerUpEventListener Get(GameObject obj)
        {
            return CheckAndAddListener(obj);
        }

        private void OnDestroy()
        {
            OnPointerUpEvent = null;
        }
    }

    public static class UIPointerUpEventListenerExtension
    {
        public static void RegisterOnPointerUp(this GameObject selfObj, Action<PointerEventData> onPointerUpAction)
        {
            UIPointerUpEventListener.Get(selfObj).OnPointerUpEvent += onPointerUpAction;
        }

        public static void RegisterOnPointerUp<T>(this T selfComponent, Action<PointerEventData> onPointerUpAction)
            where T : Component
        {
            UIPointerUpEventListener.Get(selfComponent.gameObject).OnPointerUpEvent += onPointerUpAction;
        }

        public static void UnRegisterOnPointerUp(this GameObject selfObj, Action<PointerEventData> onPointerUpAction)
        {
            UIPointerUpEventListener.Get(selfObj).OnPointerUpEvent -= onPointerUpAction;
        }

        public static void UnRegisterOnPointerUp<T>(this T selfComponent, Action<PointerEventData> onPointerUpAction)
            where T : Component
        {
            UIPointerUpEventListener.Get(selfComponent.gameObject).OnPointerUpEvent -= onPointerUpAction;
        }
    }
}