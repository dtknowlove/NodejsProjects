/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using UnityEngine;

	public sealed class PTMonoSingletonProperty<T> where T : MonoBehaviour,ISingleton
	{
		private static T mInstance = null;

		public static T Instance
		{
			get 
			{
				if (null == mInstance) 
				{
					mInstance = PTSingletonCreator.CreateMonoSingleton<T> ();
				}

				return mInstance;
			}
		}

		public static void Dispose()
		{
//			if (PTSingletonCreator.IsUnitTestMode)
//			{
//				Object.DestroyImmediate(mInstance.gameObject);
//			}
//			else
			{
				Object.Destroy(mInstance.gameObject);
			}

			mInstance = null;
		}
	}
}