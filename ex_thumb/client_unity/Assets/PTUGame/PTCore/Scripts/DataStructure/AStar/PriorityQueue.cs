/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
 ****************************************************************************/

namespace PTGame.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Priority是Queue是一个简短的类,使得List处理节点变得容易一些,    
    /// </summary>
    public class PriorityQueue
    {
        private List<AStarNode> mNodes = new List<AStarNode>();

        public int Length
        {
            get { return this.mNodes.Count; }
        }

        public bool Contains(object node)
        {
            return this.mNodes.Contains(node as AStarNode);
        }

        public AStarNode First()
        {
            if (mNodes.Count > 0)
            {
                return mNodes[0];
            }
            return null;
        }

        public void Push(AStarNode node)
        {
            mNodes.Add(node);
            mNodes.Sort();
        }

        public void Remove(AStarNode node)
        {
            mNodes.Remove(node);
            //Ensure the list is sorted  
            mNodes.Sort();
        }
    }
}