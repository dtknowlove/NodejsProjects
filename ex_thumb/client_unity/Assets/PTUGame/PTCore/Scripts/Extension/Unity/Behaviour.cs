/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;

    public static class BehaviourExtension
    {
        public static T Enable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = true;
            return selfBehaviour;
        }

        public static T Disable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = false;
            return selfBehaviour;
        }
    }
}