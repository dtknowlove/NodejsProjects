/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    
    /// <summary>
    /// 执行节点的基类
    /// </summary>
    public interface IExecuteNode : IExecutable<float>, IDisposable,IResetable
    {
        bool Disposed { get; }

        bool Finished { get; }

        void Finish();
    }
}