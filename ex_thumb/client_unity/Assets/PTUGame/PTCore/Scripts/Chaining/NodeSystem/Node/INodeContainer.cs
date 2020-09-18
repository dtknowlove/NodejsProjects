/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public interface INodeContainer
    {
        IExecuteNode CurrentExecutingNode { get; }
    }
}