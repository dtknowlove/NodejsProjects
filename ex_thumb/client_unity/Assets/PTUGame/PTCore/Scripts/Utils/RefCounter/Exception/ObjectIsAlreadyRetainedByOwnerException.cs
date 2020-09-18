/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public class ObjectIsAlreadyRetainedByOwnerException : ExceptionWithHint
    {
        public ObjectIsAlreadyRetainedByOwnerException(object obj, object owner)
            : base(string.Format("'{0}' cannot retain {1}!\n" +
                                 "Object is already retained by this object!", owner, obj),
                "The entity must be released by this object first.")
        {
        }
    }
}