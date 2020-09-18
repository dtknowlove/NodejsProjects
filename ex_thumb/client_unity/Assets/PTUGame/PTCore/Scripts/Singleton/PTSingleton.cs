/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

using System;
using System.Reflection;

namespace PTGame.Core
{	
	public abstract class PTSingleton<T> : ISingleton where T : PTSingleton<T>
	{
		protected static T mInstance;
		
		private static readonly object mLock = new object();

		protected PTSingleton()
		{
		}

		public static T Instance
		{
			get
			{
				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = PTSingletonCreator.CreateSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public virtual void Dispose()
		{
			mInstance = null;
		}

		public virtual void OnSingletonInit()
		{
		}
	}
}