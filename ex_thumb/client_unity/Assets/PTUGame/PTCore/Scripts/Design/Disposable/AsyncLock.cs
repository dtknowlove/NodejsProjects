/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * this code is borrowed from RxOfficial(rx.codeplex.com) and modified
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Asynchronous lock.
    /// </summary>
    public sealed class AsyncLock : IDisposable
    {
        private readonly Queue<Action> queue = new Queue<Action>();
        
        private bool mIsAcquired;
        private bool mHasFaulted;

        /// <summary>
        /// Queues the action for execution. If the caller acquires the lock and becomes the owner,
        /// the queue is processed. If the lock is already owned, the action is queued and will get
        /// processed by the owner.
        /// </summary>
        /// <param name="action">Action to queue for execution.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public void Wait(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var isOwner = false;
            lock (queue)
            {
                if (!mHasFaulted)
                {
                    queue.Enqueue(action);
                    isOwner = !mIsAcquired;
                    mIsAcquired = true;
                }
            }

            if (isOwner)
            {
                while (true)
                {
                    var work = default(Action);
                    lock (queue)
                    {
                        if (queue.Count > 0)
                            work = queue.Dequeue();
                        else
                        {
                            mIsAcquired = false;
                            break;
                        }
                    }

                    try
                    {
                        work();
                    }
                    catch
                    {
                        lock (queue)
                        {
                            queue.Clear();
                            mHasFaulted = true;
                        }
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the work items in the queue and drops further work being queued.
        /// </summary>
        public void Dispose()
        {
            lock (queue)
            {
                queue.Clear();
                mHasFaulted = true;
            }
        }
    }
}
