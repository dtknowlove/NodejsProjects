/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
 ****************************************************************************/

namespace PTGame.Core
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Node类将处理代表我们地图的2D格子中其中的每个格子对象,一下是Node.cs文件.
	/// </summary>
	public class AStarNode : IComparable
	{
		public float NodeTotalCost; // G 它是从开始节点到当前节点的代价值
		public float EstimatedCost; // H 它是从当前节点到目标节点的估计值
		public bool IsObstacle;
		public AStarNode Parent;
		public Vector3 Position;

		public AStarNode()
		{
			EstimatedCost = 0.0f;
			NodeTotalCost = 1.0f;
			IsObstacle = false;
			Parent = null;
		}

		public AStarNode(Vector3 pos)
		{
			EstimatedCost = 0.0f;
			NodeTotalCost = 1.0f;
			IsObstacle = false;
			Parent = null;
			Position = pos;
		}

		public void MarkAsObstacle()
		{
			IsObstacle = true;
		}

		public int CompareTo(object obj)
		{
			var node = obj as AStarNode;
			return EstimatedCost.CompareTo(node.EstimatedCost);
		}
	}
}