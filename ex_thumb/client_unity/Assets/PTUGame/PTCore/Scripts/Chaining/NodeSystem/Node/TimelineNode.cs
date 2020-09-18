/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using System.Collections.Generic;
	using System;

	/// <summary>
	/// 时间轴执行节点
	/// </summary>
	public class TimelineNode : ExecuteNode
	{
		private float mCurTime = 0;

		public readonly Action<string> OnKeyEventsReceivedCallback = null;

		public class TimelinePair
		{
			public readonly float Time;
			public IExecuteNode Node;

			public TimelinePair(float time, IExecuteNode node)
			{
				Time = time;
				Node = node;
			}
		}

		/// <summary>
		/// refator 2 one list? all in one list;
		/// </summary>
		public List<TimelinePair> TimeLinePairList = new List<TimelinePair>();

		/// <summary>
		/// 应该有个 OnResetForAgain()
		/// </summary>
		protected override void OnResetState()
		{
			mCurTime = 0.0f;
			mCurPairIndex = 0;
			mCurTimelinePair = null;
			TimeLinePairList.ForEach(pair => pair.Node.ResetState());
			mCurTimelinePair = null;
		}

		private int mCurPairIndex = 0;
		private TimelinePair mCurTimelinePair;
		
		protected override void OnExecute(float dt)
		{
			mCurTime += dt;
			mCurTimelinePair = TimeLinePairList[mCurPairIndex];
			if (mCurTime >= mCurTimelinePair.Time && !mCurTimelinePair.Node.Finished)
			{
				if (mCurTimelinePair.Node.Execute(dt))
				{
					mCurPairIndex++;
					Finished = TimeLinePairList.Count == mCurPairIndex;
				}
			}
		}

		public TimelineNode(params TimelinePair[] pairs)
		{
			TimeLinePairList.AddRange(pairs);
		}

		public void Append(TimelinePair pair)
		{
			TimeLinePairList.Add(pair);
		}

		public void Append(float time, IExecuteNode node)
		{
			TimeLinePairList.Add(new TimelinePair(time, node));
		}

		protected override void OnDispose()
		{
			foreach (var timelinePair in TimeLinePairList)
			{
				timelinePair.Node.Dispose();
			}

			if (mCurTimelinePair != null)
			{
				mCurTimelinePair.Node.Dispose();
				mCurTimelinePair.Node = null;
			}

			mCurTimelinePair = null;
			
			TimeLinePairList.Clear();
			TimeLinePairList = null;
		}
	}
}