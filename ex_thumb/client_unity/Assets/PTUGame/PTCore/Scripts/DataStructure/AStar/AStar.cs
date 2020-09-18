/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;
    using System.Collections.Generic;

    public class AStar
    {
        public static PriorityQueue ClosedList, OpenList;

        private static float HeuristicEstimateCost(AStarNode curNode, AStarNode goalNode)
        {
            var vecCost = curNode.Position - goalNode.Position;
            return vecCost.magnitude;
        }

        public static List<AStarNode> FindPath(AStarNode start, AStarNode goal)
        {
            OpenList = new PriorityQueue();
            OpenList.Push(start);
            start.NodeTotalCost = 0.0f;
            start.EstimatedCost = HeuristicEstimateCost(start, goal);
            ClosedList = new PriorityQueue();
            AStarNode node = null;
            while (OpenList.Length != 0)
            {
                node = OpenList.First();
                //Check if the current node is the goal node  
                if (node.Position == goal.Position)
                {
                    return CalculatePath(node);
                }
                //Create an ArrayList to store the neighboring nodes  
                var neighbours = new List<AStarNode>();
                GridManager.Instance.GetNeighbours(node, neighbours);
                for (var i = 0; i < neighbours.Count; i++)
                {
                    var neighbourNode = neighbours[i] as AStarNode;
                    if (!ClosedList.Contains(neighbourNode))
                    {
                        var cost = HeuristicEstimateCost(node,
                            neighbourNode);
                        var totalCost = node.NodeTotalCost + cost;
                        var neighbourNodeEstCost = HeuristicEstimateCost(
                            neighbourNode, goal);
                        neighbourNode.NodeTotalCost = totalCost;
                        neighbourNode.Parent = node;
                        neighbourNode.EstimatedCost = totalCost +
                                                      neighbourNodeEstCost;
                        if (!OpenList.Contains(neighbourNode))
                        {
                            OpenList.Push(neighbourNode);
                        }
                    }
                }
                //Push the current node to the closed list  
                ClosedList.Push(node);
                //and remove it from openList  
                OpenList.Remove(node);
            }
            if (node.Position != goal.Position)
            {
                Debug.LogError("Goal Not Found");
                return null;
            }
            return CalculatePath(node);
        }

        private static List<AStarNode> CalculatePath(AStarNode node)
        {
            var list = new List<AStarNode>();
            while (node != null)
            {
                list.Add(node);
                node = node.Parent;
            }
            list.Reverse();
            return list;
        }
    }
}