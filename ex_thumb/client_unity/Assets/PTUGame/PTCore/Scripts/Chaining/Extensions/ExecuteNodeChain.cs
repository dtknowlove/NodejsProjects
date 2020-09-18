/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;
    using System;

    public abstract class ExecuteNodeChain : ExecuteNode, IExecuteNodeChain, IDisposeWhenFinish
    {
        public MonoBehaviour Executer { get; set; }

        protected abstract ExecuteNode mNode { get; }

        public abstract IExecuteNodeChain Append(IExecuteNode node);

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition.InvokeGracefully())
            {
                //maoling: 这里直接dispose，否则会走finish逻辑，调用到外部注册的finish callback，从而导致逻辑错误。
                //Finished = true;
                Dispose();
            }
            else
            {
                Finished = mNode.Execute(dt);
            }
        }

        protected override void OnFinish()
        {
            base.OnFinish();

            if (mDisposeWhenCondition)
            {
                Dispose();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Executer = null;
            mDisposeWhenCondition = false;
            mDisposeCondition = null;
            mOnDisposedEvent.InvokeGracefully();
            mOnDisposedEvent = null;
        }

        public IDisposeWhenFinish Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }

        private bool mDisposeWhenCondition = false;
        private Func<bool> mDisposeCondition;
        private Action mOnDisposedEvent = null;

        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }

        public IDisposeEventRegister DisposeWhenFinish()
        {
            mDisposeWhenCondition = true;
            return this;
        }

        IDisposeEventRegister IDisposeEventRegister.OnFinished(Action onFinishedEvent)
        {
            OnEndedCallback += onFinishedEvent;
            return this;
        }

        public void OnDisposed(Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }
    }
}