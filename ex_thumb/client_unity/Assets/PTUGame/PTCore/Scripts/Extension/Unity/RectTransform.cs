/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/
using UnityEngine;

namespace PTGame.Core
{
	public static class RectTransformExtension
	{
		public static Vector2 GetWorldposInRect(this RectTransform selfRectTrans)
		{
			var retWorldPos = Vector2.down;
			return retWorldPos;
		}

		public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
		{
			return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
		}

		public static void SetAnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
		{
			var anchorPos = selfRectTrans.anchoredPosition;
			anchorPos.x = anchorPosX;
			selfRectTrans.anchoredPosition = anchorPos;
		}
		
		public static void SetAnchorPosY(this RectTransform selfRectTrans, float anchorPosY)
		{
			var anchorPos = selfRectTrans.anchoredPosition;
			anchorPos.y = anchorPosY;
			selfRectTrans.anchoredPosition = anchorPos;
		}
		
		public static void SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
		{
			var sizeDelta = selfRectTrans.sizeDelta;
			sizeDelta.x = sizeWidth;
			selfRectTrans.sizeDelta = sizeDelta;
		}

		public static void SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
		{
			var sizeDelta = selfRectTrans.sizeDelta;
			sizeDelta.y = sizeHeight;
			selfRectTrans.sizeDelta = sizeDelta;
		}

		public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
		{
			return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
		}
	}
}