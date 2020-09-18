/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;
    using System;

    /// <summary>
    /// 执行链表,
    /// </summary>
    public interface IExecuteNodeChain : IExecuteNode
    {
        MonoBehaviour Executer { get; set; }

        IExecuteNodeChain Append(IExecuteNode node);

        IDisposeWhenFinish Begin();
    }

    public interface IDisposeWhenFinish : IDisposeWhen
    {
        IDisposeEventRegister DisposeWhenFinish();
    }

    public interface IDisposeWhen : IDisposeEventRegister
    {
        IDisposeEventRegister DisposeWhen(Func<bool> condition);
    }

    public interface IDisposeEventRegister : IDisposable
    {
        void OnDisposed(Action onDisposedEvent);

        IDisposeEventRegister OnFinished(Action onFinishedEvent);
    }
}