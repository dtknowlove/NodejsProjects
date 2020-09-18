/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * https://github.com/akbiggs/UnityTimer
 * 临时可用版本
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using UnityEngine;
    
    public class EventToMainThread : PTMonoSingleton<EventToMainThread>
    {
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            var init = EventToMainThread.Instance;
        }
        
        
        private static Queue<Action> mEventQueue = new Queue<Action>();

        
        public static void SendEventToMainThread(Action event2execute)
        {
            mEventQueue.Enqueue(event2execute);
        }
        
        public static void ClearAllEvent()
        {
            mEventQueue.Clear();
        }

        private void Update()
        {
            int count = mEventQueue.Count;

            for (int i = 0; i < count; i++)
            {
                mEventQueue.Dequeue().InvokeGracefully();
            }

        }
    }
    
    
    
    /// <summary>
    /// 高精度的Timer 
    ///  1.误差在+-5毫秒以内。
    ///  2.受TimeScale影响。
    ///  3.自动矫正功能。
    ///  4.生命周期安全可控。
    ///  4.在其他线程使用。
    /// </summary>
    public class TimeTicker : IDisposable,IPoolable,IPoolType
    {
        private Thread mTimeThread;

        private TimeTicker(){}

        public bool Invokable { get; private set; }

        public int CurTickCounts { get; private set; }
        private double mLastDelayTime;
        
        public void StartTick(int durationMilliSeconds, Action onTickAction)
        {
            if (mTimeThread.IsNotNull())
            {
                throw new Exception("thread is running");
            }
            
            Invokable = false;
            CurTickCounts = 0;
            
            mTimeThread = new Thread(() =>
            {
                var delayTime = durationMilliSeconds;
                var stopwatch =  new Stopwatch();
                stopwatch.Start();
                var nextDelayTime = delayTime;
                var lastTime = new TimeSpan();
                var delayTimeSpan = TimeSpan.FromMilliseconds(delayTime);

                while (true)
                {
                    lastTime = stopwatch.Elapsed;
                    Thread.Sleep(nextDelayTime);
                    onTickAction.Invoke();
                    var durationTime = stopwatch.Elapsed - lastTime;
                    mLastDelayTime = durationTime.TotalMilliseconds;
                    // 矫正功能
                    var deltaTimeSpan = durationTime - delayTimeSpan;
                    nextDelayTime += -(int) (deltaTimeSpan.TotalMilliseconds / 2);
                    Invokable = true;
                    CurTickCounts++;
                }
            });
            mTimeThread.Start();
        }

        public void StopTick()
        {
            if (mTimeThread.IsNotNull())
            {
                mTimeThread.Abort();
                mTimeThread = null;
            }
            else
            {
                throw new Exception("time thread already ticked");
            }
        }

        void IDisposable.Dispose()
        {
            Recycle2Cache();
        }

        void IPoolable.OnRecycled()
        {
            StopTick();
        }

        bool IPoolable.IsRecycled { get; set; }

        public static TimeTicker Allocate()
        {
            return SafeObjectPool<TimeTicker>.Instance.Allocate();
        }
        
        public void Recycle2Cache()
        {
            SafeObjectPool<TimeTicker>.Instance.Recycle(this);
        }

        [PTMonoSingletonPath("[Framework]/TimeTicker")]
        private class ThreadInvoker : PTMonoSingleton<ThreadInvoker>
        {
            private object mLockObj = new object();

            static Queue<Action> mActions = new Queue<Action>();
            public void ThreadInvokeMain(Action action)
            {
                lock (mLockObj)
                {
                    mActions.Enqueue(action);
                }
            }

            private void Update()
            {
                if (mActions.Count != 0)
                {
                    mActions.Dequeue().Invoke();
                }
            }
        }
    }
   
    [PTMonoSingletonPath("[Framework]/MonoTimer")]
    public class MonoTimer : PTMonoSingleton<MonoTimer>
    {
        private readonly BinaryHeap<TimeItem> mUnScaleTimeHeap = new BinaryHeap<TimeItem>(128, BinaryHeapSortMode.kMin);
        private readonly BinaryHeap<TimeItem> mScaleTimeHeap = new BinaryHeap<TimeItem>(128, BinaryHeapSortMode.kMin);
        private float mCurrentUnScaleTime = -1;
        private float mCurrentScaleTime = -1;

        public float CurrentScaleTime
        {
            get { return mCurrentScaleTime; }
        }

        public float CurrentUnScaleTime
        {
            get { return mCurrentUnScaleTime; }
        }

        public override void OnSingletonInit()
        {
            mUnScaleTimeHeap.Clear();
            mScaleTimeHeap.Clear();

            mCurrentUnScaleTime = Time.unscaledTime;
            mCurrentScaleTime = Time.time;
        }

        public void ResetMgr()
        {
            mUnScaleTimeHeap.Clear();
            mScaleTimeHeap.Clear();
        }

        public void StartMgr()
        {
            mCurrentUnScaleTime = Time.unscaledTime;
            mCurrentScaleTime = Time.time;
        }

        #region 投递受缩放影响定时器

        public TimeItem Post2Scale(Action<int> callback, float delay, int repeat)
        {
            var item = TimeItem.Allocate(callback, delay, repeat);
            Post2Scale(item);
            return item;
        }

        public TimeItem Post2Scale(Action<int> callback, float delay)
        {
            var item = TimeItem.Allocate(callback, delay);
            Post2Scale(item);
            return item;
        }

        public void Post2Scale(TimeItem item)
        {
            item.SortScore = mCurrentScaleTime + item.DelayTime();
            mScaleTimeHeap.Insert(item);
        }

        #endregion

        #region 投递真实时间定时器

        //投递指定时间计时器：只支持标准时间
        public TimeItem Post2Really(Action<int> callback, DateTime toTime)
        {
            float passTick = (toTime.Ticks - DateTime.Now.Ticks) / 10000000;
            if (passTick < 0)
            {
                UnityEngine.Debug.LogWarning("Timer Set Pass Time...");
                passTick = 0;
            }
            return Post2Really(callback, passTick);
        }

        public TimeItem Post2Really(Action<int> callback, float delay, int repeat)
        {
            var item = TimeItem.Allocate(callback, delay, repeat);
            Post2Really(item);
            return item;
        }

        public TimeItem Post2Really(Action<int> callback, float delay)
        {
            var item = TimeItem.Allocate(callback, delay);
            Post2Really(item);
            return item;
        }

        public void Post2Really(TimeItem item)
        {
            item.SortScore = mCurrentUnScaleTime + item.DelayTime();
            mUnScaleTimeHeap.Insert(item);
        }

        #endregion

        public void Update()
        {
            UpdateMgr();
        }

        public void UpdateMgr()
        {
            TimeItem item = null;
            mCurrentUnScaleTime = Time.unscaledTime;
            mCurrentScaleTime = Time.time;

            #region 不受缩放影响定时器更新

            while ((item = mUnScaleTimeHeap.Top()) != null)
            {
                if (!item.isEnable)
                {
                    mUnScaleTimeHeap.Pop();
                    item.Recycle2Cache();
                    continue;
                }

                if (item.SortScore < mCurrentUnScaleTime)
                {
                    mUnScaleTimeHeap.Pop();

                    item.OnTimeTick();

                    if (item.isEnable && item.NeedRepeat())
                    {
                        Post2Really(item);
                    }
                    else
                    {
                        item.Recycle2Cache();
                    }
                }
                else
                {
                    break;
                }
            }

            #endregion

            #region 受缩放影响定时器更新

            while ((item = mScaleTimeHeap.Top()) != null)
            {
                if (!item.isEnable)
                {
                    mScaleTimeHeap.Pop();
                    item.Recycle2Cache();
                    continue;
                }

                if (item.SortScore < mCurrentScaleTime)
                {
                    mScaleTimeHeap.Pop();

                    item.OnTimeTick();

                    if (item.isEnable && item.NeedRepeat())
                    {
                        Post2Scale(item);
                    }
                    else
                    {
                        item.Recycle2Cache();
                    }
                }
                else
                {
                    break;
                }
            }

            #endregion
        }

        public void Dump()
        {
        }
    }
}