/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using UnityEngine;

	public static class LightmapUtil 
	{
		public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
		{
			RenderSettings.ambientLight = ColorUtil.HtmlStringToColor(htmlStringColor);
		}
	}
}