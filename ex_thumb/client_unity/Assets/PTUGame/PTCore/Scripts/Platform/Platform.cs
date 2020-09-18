/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	public class Platform
	{
		public static bool IsAndroid
		{
			get
			{
				var retValue = false;
#if UNITY_ANDROID
                retValue = true;    
#endif
				return retValue;
			}
		}
        
		public static bool IsRuntime
		{
			get { return !IsEditor; }
		}

		public static bool IsNotEditor
		{
			get { return !IsEditor; }
		}
		
		public static bool IsEditor
		{
			get
			{
				var retValue = false;
#if UNITY_EDITOR
				retValue = true;    
#endif
				return retValue;
			}
		}

		public static bool IsStandardalone
		{
			get
			{
				var retValue = false;
				#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
				retValue = true;    
				#endif			
				return retValue;
			}
		}
		
        
        
		public static bool IsiOS
		{
			get
			{
				var retValue = false;
#if UNITY_IOS
				retValue = true;    
#endif
				return retValue;
			}
		}
	}
}