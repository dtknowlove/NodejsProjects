/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点，会占用一帧
    /// </summary>
    public class OneFrameEventAction : ExecuteNode, IPoolable
    {
        private Action mOnExecuteEvent;
        private bool mExecuted = false;

        /// <summary>
        /// TODO:这里填可变参数会有问题
        /// </summary>
        /// <param name="onExecuteEvents"></param>
        /// <returns></returns>
        public static OneFrameEventAction Allocate(params Action[] onExecuteEvents)
        {
            var retNode = SafeObjectPool<OneFrameEventAction>.Instance.Allocate();
            
            retNode.OnRecycled();    //maoling: just for sure. For some reason, the 'disposed' property value is 'true'
            
            retNode.mExecuted = false;
            Array.ForEach(onExecuteEvents, onExecuteEvent => retNode.mOnExecuteEvent += onExecuteEvent);
            return retNode;
        }

        /// <inheritdoc />
        /// <summary>
        /// finished
        /// </summary>
        protected override void OnExecute(float dt)
        {
            if (!mExecuted)
            {
                mOnExecuteEvent.InvokeGracefully();
                mExecuted = true;
            }
            else
            {
                Finished = true;
            }
        }

        protected override void OnDispose()
        {
            SafeObjectPool<OneFrameEventAction>.Instance.Recycle(this);
        }

        protected override void OnResetState()
        {
            mExecuted = false;
        }

        public void OnRecycled()
        {
            ResetState();
            mOnExecuteEvent = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
}