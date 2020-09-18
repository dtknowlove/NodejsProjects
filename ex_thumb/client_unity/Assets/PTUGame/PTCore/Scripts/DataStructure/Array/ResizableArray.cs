/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using System.Linq;

    public class ResizeableArray<T> where T : class
    {
        public T this[int index]
        {
            get { return mArray[index]; }
        }

        public int Count
        {
            get { return mArray.Length; }
        }

        private T[] mArray;

        public ResizeableArray(int initCount)
        {
            mArray = new T[initCount];
        }

        public void Append(T itemToAdd)
        {
            Array.Resize(ref mArray, mArray.Length + 1);
            mArray[mArray.Length - 1] = itemToAdd;
        }


        public void Remove(
            T itemToRemove)
        {
            var size = mArray.Length;

            var newIndex = 0;
            for (var i = 0; i < size; i++)
            {
                if (mArray[i] == itemToRemove)
                {
                    mArray[i] = null;
                    continue;
                }

                mArray[newIndex] = mArray[i];
                newIndex++;
            }

            Array.Resize(ref mArray, mArray.Length - 1);
        }

        public bool Contains(T item)
        {
           return mArray.Contains(item);
        }
    }
}