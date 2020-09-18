/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * this code is borrowed from RxOfficial(rx.codeplex.com) and modified
 ****************************************************************************/

namespace PTGame.Core
{
    public sealed class BooleanDisposable : ICancelable
    {
        public bool IsDisposed { get; private set; }

        public BooleanDisposable()
        {

        }

        public BooleanDisposable(bool isDisposed)
        {
            IsDisposed = isDisposed;
        }

        public void Dispose()
        {
            if (!IsDisposed) IsDisposed = true;
        }
    }
}