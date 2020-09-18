/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    
    public class ListObserver<T> : IObserver<T>
    {
        private readonly ImmutableList<IObserver<T>> mObservers;

        public ListObserver(ImmutableList<IObserver<T>> observers)
        {
            mObservers = observers;
        }

        public void OnCompleted()
        {
            mObservers.Data.ForEach(observer => observer.OnCompleted());
        }

        public void OnError(Exception error)
        {
            mObservers.Data.ForEach(observer => observer.OnError(error));
        }

        public void OnNext(T value)
        {
            mObservers.Data.ForEach(observer => observer.OnNext(value));
        }

        public IObserver<T> Add(IObserver<T> observer)
        {
            return new ListObserver<T>(mObservers.Add(observer));
        }

        public IObserver<T> Remove(IObserver<T> observer)
        {
            var i = Array.IndexOf(mObservers.Data, observer);
            if (i < 0)
                return this;

            if (mObservers.Data.Length == 2)
            {
                return mObservers.Data[1 - i];
            }
            else
            {
                return new ListObserver<T>(mObservers.Remove(observer));
            }
        }
    }

    public class EmptyObserver<T> : IObserver<T>
    {
        public static readonly EmptyObserver<T> Instance = new EmptyObserver<T>();

        EmptyObserver()
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
        }
    }

    public class ThrowObserver<T> : IObserver<T>
    {
        public static readonly ThrowObserver<T> Instance = new ThrowObserver<T>();

        ThrowObserver()
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(T value)
        {
        }
    }

    public class DisposedObserver<T> : IObserver<T>
    {
        public static readonly DisposedObserver<T> Instance = new DisposedObserver<T>();

        DisposedObserver()
        {

        }

        public void OnCompleted()
        {
            throw new ObjectDisposedException("");
        }

        public void OnError(Exception error)
        {
            throw new ObjectDisposedException("");
        }

        public void OnNext(T value)
        {
            throw new ObjectDisposedException("");
        }
    }
}