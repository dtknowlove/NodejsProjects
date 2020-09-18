/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using UnityEngine.Events;

	public static class UnityActionUtil
	{

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <returns> call succeed</returns>
		public static bool InvokeGracefully(this UnityAction selfAction)
		{
			if (null != selfAction)
			{
				selfAction();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool InvokeGracefully<T>(this UnityAction<T> selfAction, T t)
		{
			if (null != selfAction)
			{
				selfAction(t);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Call action
		/// </summary>
		/// <param name="selfAction"></param>
		/// <returns> call succeed</returns>
		public static bool InvokeGracefully<T, K>(this UnityAction<T, K> selfAction, T t, K k)
		{
			if (null != selfAction)
			{
				selfAction(t, k);
				return true;
			}
			return false;
		}
	}
}