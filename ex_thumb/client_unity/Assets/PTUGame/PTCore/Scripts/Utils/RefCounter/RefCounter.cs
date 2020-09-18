/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }

    public class RefCounter : IRefCounter
    {
        private int mRefCount = 0;
        public int RefCount
        {
            get { return mRefCount; }
        }

        public void Retain(object refOwner = null)
        {
            ++mRefCount;
        }

        public void Release(object refOwner = null)
        {
            --mRefCount;
            if (mRefCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {
        }
    }
}