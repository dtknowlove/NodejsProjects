/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using System.Collections;
    using UniRx;

    /// <summary>
    /// TODO:找个时间测试下性能 对比PTWaitForSeconds
    /// </summary>
    public class PTWait
    {
        /// <summary>
        /// replace new WaitForSeconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IEnumerator ForSeconds(float seconds)
        {
            yield return Observable.Timer(TimeSpan.FromSeconds(seconds)).ToYieldInstruction();
        }

        public static IEnumerator ForFrames(int frameCount)
        {
            yield return Observable.TimerFrame(frameCount).ToYieldInstruction();
        }

        /// <summary>
        /// replace new WaitUntil
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerator Until(Func<bool> match)
        {
            while (!match())
            {
                yield return null;
            }
        }
    }
}