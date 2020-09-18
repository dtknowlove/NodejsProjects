/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public interface IExecutable
    {
        bool Execute();
    }
    
    public interface IExecutable<T>
    {
        bool Execute(T arg);
    }
}