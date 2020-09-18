/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;

namespace PTGame.Core 
{
	public abstract class PTMonoSingleton<T> : MonoBehaviour, ISingleton where T : PTMonoSingleton<T>
	{
		protected static T mInstance = null;

		public static T Instance
		{
			get
			{
				if (null == mInstance)
				{
					mInstance = PTSingletonCreator.CreateMonoSingleton<T>();
				}

				return mInstance;
			}
		}

		public virtual void OnSingletonInit()
		{

		}

		public virtual void OnSingletonDeinit()
		{
			
		}

		public virtual void Dispose()
		{
//			if (PTSingletonCreator.IsUnitTestMode)
//			{
//				var curTrans = transform;
//				do
//				{
//					var parent = curTrans.parent;
//					DestroyImmediate(curTrans.gameObject);
//					curTrans = parent;
//				} while (null != curTrans);
//
//				mInstance = null;
//			}
//			else
			{
				Destroy(gameObject);
			}
		}

		protected virtual void OnDestroy()
		{
			mInstance = null;
		}
	}
}