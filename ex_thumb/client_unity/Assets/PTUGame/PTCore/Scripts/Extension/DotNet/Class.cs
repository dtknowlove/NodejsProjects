/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	public static class ClassExtention
	{
		public static bool IsNull<T>(this T selfObj) where T : class
		{
			return null == selfObj;
		}
		
		public static bool IsNotNull<T>(this T selfObj) where T : class
		{
			return null != selfObj;
		}
	}
}