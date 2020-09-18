/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using UnityEngine;
    using UniRx;
    
    public static class IExecuteNodeChainExtention
    {
        public static IExecuteNodeChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count) {Executer = selfbehaviour};
            retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IExecuteNodeChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain {Executer = selfbehaviour};
            retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IExecuteNodeChain Delay(this IExecuteNodeChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }
        
        /// <summary>
        /// Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IExecuteNodeChain Wait(this IExecuteNodeChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        /// <summary>
        /// 并不会占用一帧，执行event
        /// </summary>
        public static IExecuteNodeChain Event(this IExecuteNodeChain selfChain,params Action[] onEvents)
        {
            return selfChain.Append(EventAction.Allocate(onEvents));
        }
        
        /// <summary>
        /// 占用一帧，执行event
        /// </summary>
        public static IExecuteNodeChain OneFrameEvent(this IExecuteNodeChain selfChain,params Action[] onEvents)
        {
            return selfChain.Append(OneFrameEventAction.Allocate(onEvents));
        }

        public static IExecuteNodeChain Until(this IExecuteNodeChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilNode.Allocate(condition));
        }
    }
}