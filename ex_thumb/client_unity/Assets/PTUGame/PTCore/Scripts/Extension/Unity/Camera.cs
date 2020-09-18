/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using UnityEngine;
	
	public static class CameraUtil 
	{
		public static Texture2D CaptureCamera(this Camera camera,Rect rect)
		{
			RenderTexture rt = new RenderTexture(Screen.width,Screen.height,0);
			camera.targetTexture = rt;
			camera.Render();

			RenderTexture.active = rt;

			Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
			screenShot.ReadPixels(rect,0,0);
			screenShot.Apply();

			camera.targetTexture = null;
			RenderTexture.active = null;
			Object.Destroy(rt);

			return screenShot;
		}
	}
}