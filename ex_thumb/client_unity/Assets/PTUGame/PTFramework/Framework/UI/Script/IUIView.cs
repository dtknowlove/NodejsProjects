/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/


namespace PTGame.Framework 
{
	using UnityEngine;
//	using PTGame.UI;

	/// <summary>
	/// IUI behaviour.
	/// </summary>
	public interface IUIView /*: IViewModel */
	{
		Transform Transform { get; }
		
		UIPanelInfo PanelInfo { get; set; }
		
		void Init(IUIData uiData = null);

		void Show();

		void Hide();
		
		void Close(bool destroy = true);
	}
}