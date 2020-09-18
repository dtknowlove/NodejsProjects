/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    public abstract class ExecuteNode : IExecuteNode
    {
        public Action OnBeganCallback = null;
        public Action OnEndedCallback = null;
        public Action OnDisposedCallback = null;
        
        protected bool mOnBeginCalled = false;

        #region IExecuteNode Support

        bool IExecuteNode.Disposed
        {
            get { return mDisposed; }
        }

        protected bool mDisposed = false;
        
        public bool Finished { get; protected set; }

        #endregion

        /// <summary>
        /// called by outer in force
        /// </summary>
        public virtual void Finish()
        {
            if (!Finished)
            {
                Finished = true;
                OnFinish();
                OnEndedCallback.InvokeGracefully();
            }
        }

        #region ResetableSupport

        public void ResetState()
        {
            Finished = false;
            mOnBeginCalled = false;
            mDisposed = false;
            OnResetState();
        }
        #endregion

        #region IExecutable Support

        public bool Execute(float dt)
        {
            // 有可能被别的地方调用
            if (Finished)
            {
                return Finished;
            }

            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();
                OnBeganCallback.InvokeGracefully();
            }

            if (!Finished)
            {
                OnExecute(dt);
            }

            if (Finished)
            {
                OnFinish();
                OnEndedCallback.InvokeGracefully();
            }

            return Finished || mDisposed;
        }

        #endregion

        protected virtual void OnResetState()
        {
        }

        protected virtual void OnBegin()
        {
        }

        protected virtual void OnExecute(float dt)
        {
        }

        protected virtual void OnFinish()
        {
            
        }

        protected virtual void OnDispose()
        {
            
        }

        #region IDisposable Support

        public void Dispose()
        {
            if (mDisposed) return;
            mDisposed = true;
            OnBeganCallback = null;
            OnEndedCallback = null;
            OnDisposedCallback.InvokeGracefully();
            OnDisposedCallback = null;
            OnDispose();
        }

        #endregion
    }
}