/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;

namespace PTGame.Core
{
	using System.Collections.Generic;

	/// <summary>
	/// 序列执行节点
	/// </summary>
	public class SequenceNode : ExecuteNode ,INodeContainer
	{
		protected readonly List<IExecuteNode> mNodes = new List<IExecuteNode>();
		protected readonly List<IExecuteNode> mExcutingNodes = new List<IExecuteNode>();
		
		public int TotalCount
		{
			get { return mExcutingNodes.Count; }
		}

		public IExecuteNode CurrentExecutingNode
		{
			get
			{
				var currentNode = mExcutingNodes[0];
				var node = currentNode as INodeContainer;
				return node == null ? currentNode : node.CurrentExecutingNode;
			}
		}

		public override void Finish()
		{
			if (!Finished)
			{
				mExcutingNodes.ForEach(node => node.Finish());
				mExcutingNodes.Clear();
				base.Finish();
			}
		}

		protected override void OnResetState()
		{
			mExcutingNodes.Clear();
			foreach (var node in mNodes)
			{
				node.ResetState();
				mExcutingNodes.Add(node);
			}
		}

		protected override void OnExecute(float dt)
		{
			if (mExcutingNodes.Count > 0)
			{
				// 如果有node被主动dipose，则进行销毁，不再进行下边的操作
				if (mExcutingNodes[0].Disposed && !mExcutingNodes[0].Finished)
				{
					Debug.Log(string.Format("<color=orange>SequenceNode Negatively Disposed: {0} is disposed. Nodes count: {1}.</color>", mExcutingNodes[0].GetType(), mNodes.Count));
					foreach (IExecuteNode node in mExcutingNodes)
					{
						Debug.Log(string.Format("<color=orange>SequenceNode Negatively Disposed: {0} disposed: {1}.</color>", node.GetType(), node.Disposed));
					}
					Debug.Log("<color=orange>" +
					      "如果遇此log，请先确定是否在调用 Sequence 时，主动 Dispose 了其中的 ExecuteNode. \n" +
					      "如果不是，则为异常，请修改代码：在该类里的 Allocate 静态函数里，强制调一次 OnRecycled，来reset状态，参考 EventAction. " +
					      "</color>");
					
					Dispose();
					return;
				}

				while (mExcutingNodes[0].Execute(dt))
				{
					mExcutingNodes.RemoveAt(0);

					OnCurrentActionFinished();

					if (mExcutingNodes.Count == 0)
					{
						break;
					}
				}
			}

			Finished = mExcutingNodes.Count == 0;
		}

		protected virtual void OnCurrentActionFinished() {}

		public SequenceNode(params IExecuteNode[] nodes)
		{
			foreach (var node in nodes)
			{
				mNodes.Add(node);
				mExcutingNodes.Add(node);
			}
		}

		public SequenceNode Append(IExecuteNode appendedNode)
		{
			mNodes.Add(appendedNode);
			mExcutingNodes.Add(appendedNode);
			return this;
		}

		public SequenceNode Remove(IExecuteNode removedNode)
		{
			mNodes.Remove(removedNode);
			mExcutingNodes.Remove(removedNode);
			return this;
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			
			if (null != mNodes)
			{
				mNodes.ForEach(node => node.Dispose());
				mNodes.Clear();
			}

			if (null != mExcutingNodes)
			{
				mExcutingNodes.Clear();
			}
		}	
	}
}