/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * this code is borrowed from RxOfficial(rx.codeplex.com) and modified
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
