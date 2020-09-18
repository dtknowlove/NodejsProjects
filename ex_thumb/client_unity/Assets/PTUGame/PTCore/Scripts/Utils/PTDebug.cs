///****************************************************************************
// * Copyright (c) 2017 ptgame@putao.com
// ****************************************************************************/
using System;
using UnityEngine;

#if PTGAME_NONE
// 生成PTDebug.dll之后 需要去掉该类
namespace PTGame.Core
{
	public class PTDebug
	{
		private static PTDebug.LogLevel mLogLevel = PTDebug.LogLevel.Normal;

		public static PTDebug.LogLevel Level
		{
			get
			{
				return PTDebug.mLogLevel;
			}
			set
			{
				PTDebug.mLogLevel = value;
			}
		}

		public static void Log(object msg, params object[] args)
		{
			if (PTDebug.mLogLevel < PTDebug.LogLevel.Normal)
				return;
			string str = string.Format(GetTime(), msg);
			if (args == null || args.Length == 0)
				Debug.Log(str);
			else
				Debug.LogFormat(str, args);
		}

		public static void LogException(Exception e)
		{
			if (PTDebug.mLogLevel < PTDebug.LogLevel.Exception)
				return;
			Debug.LogException(e);
		}

		public static void LogError(object msg, params object[] args)
		{
			if (PTDebug.mLogLevel < PTDebug.LogLevel.Error)
				return;
			string str = string.Format(GetTime(), msg);
			if (args == null || args.Length == 0)
				Debug.LogError(str);
			else
				Debug.LogErrorFormat(str, args);
		}

		public static void LogWarning(object msg, params object[] args)
		{
			if (PTDebug.mLogLevel < PTDebug.LogLevel.Warning)
				return;
			string str = string.Format(GetTime(), msg);
			if (args == null || args.Length == 0)
			
				Debug.LogWarning(str);
			else
				Debug.LogWarningFormat(str, args);
		}

		private static string GetTime()
		{
			return DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss:fff") + ": {0}";
		}
		
		public enum LogLevel
		{
			None,
			Exception,
			Error,
			Warning,
			Normal,
			Max,
		}
	}
}
#endif