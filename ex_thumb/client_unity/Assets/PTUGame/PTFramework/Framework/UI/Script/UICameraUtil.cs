/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using UnityEngine;

namespace PTGame.Framework
{
	public static class PTUICameraUtil
	{
		public static Camera UICamera
		{
			get { return PTUIManager.Instance.UICamera; }
		}

		public static void SetPerspectiveMode()
		{
			UICamera.orthographic = false;
		}

		public static void SetOrthographicMode()
		{
			UICamera.orthographic = true;
		}

		public static Texture2D CaptureCamera(Rect rect)
		{
			var camera = UICamera;
			var rt = new RenderTexture(Screen.width, Screen.height, 0);
			camera.targetTexture = rt;
			camera.Render();

			RenderTexture.active = rt;

			var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
			screenShot.ReadPixels(rect, 0, 0);
			screenShot.Apply();

			camera.targetTexture = null;
			RenderTexture.active = null;
			rt.Release();
			Object.Destroy(rt);

			return screenShot;
		}
	}
}