/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using System;

	/// <inheritdoc />
	/// <summary>
	/// 延时执行节点
	/// </summary>
	public class DelayAction : ExecuteNode, IPoolable
	{
		public float DelayTime;
		
		public static DelayAction Allocate(float delayTime, Action onEndCallback = null)
		{
			var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
			
			retNode.OnRecycled();    //maoling: just for sure. For some reason, the 'disposed' property value is 'true'
			
			retNode.DelayTime = delayTime;
			retNode.OnEndedCallback = onEndCallback;
			return retNode;
		}

		public DelayAction()
		{
		}

		public DelayAction(float delayTime)
		{
			DelayTime = delayTime;
		}

		private float mCurrentSeconds = 0.0f;

		protected override void OnResetState()
		{
			mCurrentSeconds = 0.0f;
		}

		protected override void OnExecute(float dt)
		{
			mCurrentSeconds += dt;
			Finished = mCurrentSeconds >= DelayTime;
		}

		protected override void OnDispose()
		{
			SafeObjectPool<DelayAction>.Instance.Recycle(this);
		}

		public void OnRecycled()
		{
			DelayTime = 0.0f;
			ResetState();
		}

		public bool IsRecycled { get; set; }
	}
}