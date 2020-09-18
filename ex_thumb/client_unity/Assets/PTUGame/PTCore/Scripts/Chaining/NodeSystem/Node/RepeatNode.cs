/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public class RepeatNode : ExecuteNode,INodeContainer
    {
        public RepeatNode(IExecuteNode node, int loopCount)
        {
            LoopCount = loopCount;
            mNode = node;
        }

        public IExecuteNode CurrentExecutingNode
        {
            get
            {
                var currentNode = mNode;
                var node = currentNode as INodeContainer;
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }

        private IExecuteNode mNode;
        
        public readonly int LoopCount = 1;

        private int mCurLoopIndex = 0;

        protected override void OnResetState()
        {
            if (null != mNode)
            {
                mNode.ResetState();
            }
            mCurLoopIndex = 0;
            Finished = false;
        }
        
        protected override void OnExecute(float dt)
        {
            if (LoopCount < 0)
            {
                if (mNode.Execute(dt))
                {
                    mNode.ResetState();
                }
                
                return;
            }

            if (mNode.Execute(dt))
            {
                mNode.ResetState();
                mCurLoopIndex++;
            }

            if (mCurLoopIndex == LoopCount)
            {
                Finished = true;
            }
        }

        public override void Finish()
        {
            if (!Finished)
            {
                mNode.Finish();
                base.Finish();
            }
        }

        protected override void OnDispose()
        {
            if (null != mNode)
            {
                mNode.Dispose();
                mNode = null;
            }
        }
    }
}