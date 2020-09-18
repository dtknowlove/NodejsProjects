/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<T> 
    {
        void Execute(T args);
    }
}