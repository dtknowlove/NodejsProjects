/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public class ObjectIsNotRetainedByOwnerExceptionWithHint : ExceptionWithHint
    {
        public ObjectIsNotRetainedByOwnerExceptionWithHint(object obj, object owner)
            : base("'" + owner + "' cannot release " + obj + "!\n" +
                   "Object is not retained by this object!",
                "An entity can only be released from objects that retain it.")
        {
        }    
    }
}