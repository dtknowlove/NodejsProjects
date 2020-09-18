/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    /// <summary>
    /// 支持链式方法
    /// </summary>
    public class SequenceNodeChain : ExecuteNodeChain
    {
        protected override ExecuteNode mNode
        {
            get { return mSequenceNode; }
        }

        private SequenceNode mSequenceNode;

        public SequenceNodeChain()
        {
            mSequenceNode = new SequenceNode();
        }

        public override IExecuteNodeChain Append(IExecuteNode node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (null != mSequenceNode)
            {
                mSequenceNode.Dispose();
            }

            mSequenceNode = null;
        }
    }
}