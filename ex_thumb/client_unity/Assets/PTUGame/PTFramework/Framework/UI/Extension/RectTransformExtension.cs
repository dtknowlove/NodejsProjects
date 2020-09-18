/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;

namespace PTGame.Framework
{
    public static class RectTransformExtension
    {
        public static Vector2 GetLocalPosInRect(this RectTransform selfRectTrans)
        {
            Vector2 retLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTrans, Input.mousePosition,
                PTUIManager.Instance.RootCanvas.worldCamera, out retLocalPos);
            return retLocalPos;
        }

        public static bool InRect(this RectTransform selfRectTrans, Camera camera = null)
        {
            if (null == camera)
                camera = PTUIManager.Instance.RootCanvas.worldCamera;

            return RectTransformUtility.RectangleContainsScreenPoint(selfRectTrans, Input.mousePosition, camera);
        }

        public static Vector2 ToScreenPoint(this RectTransform selfRectTrans)
        {
            return RectTransformUtility.WorldToScreenPoint(PTUIManager.Instance.RootCanvas.worldCamera, selfRectTrans.position);
        }

        public static Vector2 ToScreenPoint(this RectTransform selfRectTrans, Vector2 worldPos)
        {
            return RectTransformUtility.WorldToScreenPoint(PTUIManager.Instance.RootCanvas.worldCamera, worldPos);
        }

        public static bool InRootTransRect(this RectTransform selfRectTrans, RectTransform rootTrans, Camera camera = null)
        {
            if (null == camera)
                camera = PTUIManager.Instance.RootCanvas.worldCamera;
            return RectTransformUtility.RectangleContainsScreenPoint(rootTrans, selfRectTrans.ToScreenPoint(), camera);
        }

        public static Vector2 ConvertWorldPosToLocalPosInSelf(this RectTransform selfRectTrans, Vector2 worldPos)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(PTUICameraUtil.UICamera, worldPos);
            Vector2 retLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRectTrans, screenPos, PTUICameraUtil.UICamera,
                out retLocalPos);
            return retLocalPos;
        }
    }
}