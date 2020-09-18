/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点，不会占用一帧
    /// </summary>
    public class EventAction : ExecuteNode, IPoolable
    {
        private Action mOnExecuteEvent;

        /// <summary>
        /// TODO:这里填可变参数会有问题
        /// </summary>
        /// <param name="onExecuteEvents"></param>
        /// <returns></returns>
        public static EventAction Allocate(params Action[] onExecuteEvents)
        {
            var retNode = SafeObjectPool<EventAction>.Instance.Allocate();
            
            retNode.OnRecycled();    //maoling: just for sure. For some reason, the 'disposed' property value is 'true' 
            
            Array.ForEach(onExecuteEvents, onExecuteEvent => retNode.mOnExecuteEvent += onExecuteEvent);
            return retNode;
        }


        /// <inheritdoc />
        /// <summary>
        /// finished
        /// </summary>
        protected override void OnExecute(float dt)
        {
            mOnExecuteEvent.InvokeGracefully();
            Finished = true;
        }

        protected override void OnDispose()
        {
            SafeObjectPool<EventAction>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            ResetState();
            mOnExecuteEvent = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
}