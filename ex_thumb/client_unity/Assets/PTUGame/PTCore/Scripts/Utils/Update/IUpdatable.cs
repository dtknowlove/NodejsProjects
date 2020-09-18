/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * https://blogs.unity3d.com/cn/2015/12/23/1k-update-calls/
 * https://github.com/thexa4/UnityScheduler
 ****************************************************************************/

namespace PTGame.Core
{
    public interface IUpdatable
    {
        void OnUpdate();
    }
}