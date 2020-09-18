/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System.Collections.Generic;

    /// Automatic Reference Counting (ARC)
    /// is used internally to prevent pooling retained Objects.
    /// If you use retain manually you also have to
    /// release it manually at some point.
    /// SafeARC checks if the object has already been
    /// retained or released. It's slower, but you keep the information
    /// about the owners.
    public sealed class SafeARC : IRefCounter
    {
        public int RefCount
        {
            get { return mOwners.Count; }
        }

        public HashSet<object> Owners
        {
            get { return mOwners; }
        }

        readonly object mObj;
        readonly HashSet<object> mOwners = new HashSet<object>();

        public SafeARC(object obj)
        {
            mObj = obj;
        }

        public void Retain(object refOwner)
        {
            if (!Owners.Add(refOwner))
            {
                throw new ObjectIsAlreadyRetainedByOwnerException(mObj, refOwner);
            }
        }

        public void Release(object refOwner)
        {
            if (!Owners.Remove(refOwner))
            {
                throw new ObjectIsNotRetainedByOwnerExceptionWithHint(mObj, refOwner);
            }
        }
    }
}