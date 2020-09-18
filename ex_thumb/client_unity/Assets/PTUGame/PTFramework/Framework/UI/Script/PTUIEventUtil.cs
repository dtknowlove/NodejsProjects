/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using PTGame.Core;
using UniRx;

namespace PTGame.Framework
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public static class PTUIEventUtil
    {
        private static UnityAction mOnBeforeClickEvent;

        public static void OnBeforeClickEvent(UnityAction onBeforeClickEvent)
        {
            mOnBeforeClickEvent = onBeforeClickEvent;
        }

        public static void OnClick(this Button selfBtn, UnityAction onClick, UnityAction onBeforeClickEventOnce = null,
            UnityAction onPointerUpEventOnce = null)
        {
            selfBtn.RegOnClickEvent(delegate
            {
                if (!ButtonInteractable(selfBtn)) return;
                onClick.InvokeGracefully();
            }, delegate
            {
                if (!ButtonInteractable(selfBtn)) return;
                UIEventLockManager.Instance.SendMsg(new UILockObjEventMsg(selfBtn.gameObject));
                mOnBeforeClickEvent.InvokeGracefully();
                onBeforeClickEventOnce.InvokeGracefully();
            }, delegate
            {
                if (!ButtonInteractable(selfBtn)) return;
                onPointerUpEventOnce.InvokeGracefully();
                UIEventLockManager.Instance.SendMsg(new UIUnlockObjEventMsg(selfBtn.gameObject));
            });
        }

        public static bool ButtonInteractable(Button button)
        {
            return button.IsInteractable() && Interactable(button.gameObject) && button.isActiveAndEnabled;
        }

        public static bool Interactable(GameObject obj)
        {
            if (UIEventLockManager.Instance.LockedObj == null || UIEventLockManager.Instance.LockedObj == obj)
            {
                return true;
            }

            return false;
        }

        public static void OnMouseDown<T>(this T selfComponent, UnityAction onMouseDown) where T : Component
        {
            Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(delegate(long l)
            {
                onMouseDown.InvokeGracefully();
            }).AddTo(selfComponent);
        }
    }
}