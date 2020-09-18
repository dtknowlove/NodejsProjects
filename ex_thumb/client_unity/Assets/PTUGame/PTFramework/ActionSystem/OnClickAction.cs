/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using System;
using PTGame.Core;
using UniRx;
using UnityEngine.UI;

namespace PTGame.Framework
{
	/// <summary>
	/// 按钮点击动作
	/// </summary>
	public class OnClickAction : IAction,IDisposable
	{
		public string Name { get; private set; }
		
		public bool Finished { get; set; }

		private Action mOnClickAction;

		public OnClickAction(Button button, Action onClickAction)
		{
			mOnClickAction = onClickAction;
			button.onClick.AddListener(() =>
			{
				ActionRecorder.RecordAction(this);
				Execute();
			});

			Name = button.name;
			
			this.AddTo(button);
			
			ActionMgr.Push(this);
		}

		public bool Execute()
		{
			return mOnClickAction.InvokeGracefully();
		}

		public void Dispose()
		{
			ActionMgr.Pop(this);
			Name = null;
			mOnClickAction = null;
		}
	}

	public static class OnClickActionExtension
	{
		public static void OnClick(this Button selfBtn, Action onClicAction)
		{
			new OnClickAction(selfBtn, onClicAction);
		}
	}
}