/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using System.Collections.Generic;

	/// <summary>
	/// 并发执行的协程
	/// </summary>
	public class SpawnNode : ExecuteNode
	{
		protected List<ExecuteNode> mNodes = new List<ExecuteNode>();
		
		protected override void OnResetState()
		{
			mNodes.ForEach(node => node.ResetState());
		}

		public override void Finish()
		{
			if (!Finished)
			{
				mNodes.ForEach(node => node.Finish());
				base.Finish();
			}
		}

		protected override void OnExecute(float dt)
		{
			foreach (ExecuteNode node in mNodes)
			{
				if (!node.Finished)
				{
					node.Execute(dt);
				}
			}

			Finished = mNodes.TrueForAll(n => n.Finished);
		}

		public SpawnNode(params ExecuteNode[] nodes)
		{
			mNodes.AddRange(nodes);
		}

		public void Add(params ExecuteNode[] nodes)
		{
			mNodes.AddRange(nodes);
		}

		protected override void OnDispose()
		{
			foreach (var node in mNodes)
			{
				node.Dispose();
			}

			mNodes.Clear();
			mNodes = null;
		}
	}
}