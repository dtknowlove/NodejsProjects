﻿/****************************************************************************
 * 2018.5 liqingyun
 ****************************************************************************/

namespace PTGame.Framework
{
	using UnityEngine.UI;

	/// <inheritdoc />
	/// <summary>
	/// reference from :http://gad.qq.com/article/detail/37846
	/// 替换空 Image 扩大点击区域，降低 Drawcall。
	/// </summary>
	public class Empty4Raycast : MaskableGraphic
	{
		protected Empty4Raycast()
		{
			useLegacyMeshGeneration = false;
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			toFill.Clear();
		}
	}
}