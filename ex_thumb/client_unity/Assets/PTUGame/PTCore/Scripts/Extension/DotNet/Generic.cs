/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public static class GenericUtil
    {
        public static string GetTypeName<T>()
        {
            return typeof(T).ToString();
        }
    }
}