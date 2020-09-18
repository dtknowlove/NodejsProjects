/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// like filter, add condition
    /// </summary>
    public class UntilNode : ExecuteNode, IPoolable
    {
        private Func<bool> mCondition;

        public static UntilNode Allocate(Func<bool> condition)
        {
            var retNode = SafeObjectPool<UntilNode>.Instance.Allocate();
            
            retNode.OnRecycled();    //maoling: just for sure. For some reason, the 'disposed' property value is 'true'
            
            retNode.mCondition = condition;
            return retNode;
        }

        protected override void OnExecute(float dt)
        {
            Finished = mCondition();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<UntilNode>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            ResetState();
            mCondition = null;
        }

        public bool IsRecycled { get; set; }
    }
}