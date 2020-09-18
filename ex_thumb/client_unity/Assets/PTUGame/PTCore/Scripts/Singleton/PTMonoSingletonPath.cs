/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class PTMonoSingletonPath : Attribute
    {
		private readonly string mPathInHierarchy;

        public PTMonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }
}