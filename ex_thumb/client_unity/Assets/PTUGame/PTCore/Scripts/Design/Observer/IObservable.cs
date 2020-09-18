/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

// defined from .NET Framework 4.0 and NETFX_CORE
#if !(NETFX_CORE || ENABLE_MONO_BLEEDING_EDGE_EDITOR || ENABLE_MONO_BLEEDING_EDGE_STANDALONE)

namespace PTGame.Core
{
    using System;

    public interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}

#endif

namespace PTGame.Core
{
    public interface IGroupedObservable<TKey, TElement> : IObservable<TElement>
    {
        TKey Key { get; }
    }
}