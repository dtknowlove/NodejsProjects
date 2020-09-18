/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;

    public class ExceptionWithHint : Exception
    {
        public ExceptionWithHint(string message, string hint = null)
            : base(string.IsNullOrEmpty(hint) ? message : string.Format("{0}\n{1}", message, hint))
        {
        }
    }
}