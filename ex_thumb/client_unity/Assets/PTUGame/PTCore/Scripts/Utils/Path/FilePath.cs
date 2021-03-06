/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using UnityEngine;
	using System.IO;
	using System.Collections.Generic;
	
	public class FilePath
	{
		private static string           mPersistentDataPath;
		private static string           mStreamingAssetsPath;
		private static string           mPersistentDataPath4Res;
		private static string           mPersistentDataPath4Photo;

		// 外部目录  
		public static string PersistentDataPath
		{
			get
			{
				if (null == mPersistentDataPath)
				{
					mPersistentDataPath = Application.persistentDataPath + "/";
				}

				return mPersistentDataPath;
			}
		}

		// 内部目录
		public static string StreamingAssetsPath
		{
			get
			{
				if (null == mStreamingAssetsPath)
				{
					#if UNITY_IPHONE && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
					#elif UNITY_ANDROID && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
					#elif (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";//GetParentDir(Application.dataPath, 2) + "/BuildRes/";
					#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
					#else
					//mStreamingAssetsPath = GetParentDir(Application.dataPath, 1) + "/BuildRes/standalone/";
					mStreamingAssetsPath = Application.streamingAssetsPath + "/";
					#endif
				}

				return mStreamingAssetsPath;
			}
		}

		// 外部资源目录
		public static string PersistentDataPath4Res
		{
			get
			{
				if (null == mPersistentDataPath4Res)
				{
					mPersistentDataPath4Res = PersistentDataPath + "Res/";

					if (!Directory.Exists(mPersistentDataPath4Res))
					{
						Directory.CreateDirectory(mPersistentDataPath4Res);
						#if UNITY_IPHONE && !UNITY_EDITOR
						UnityEngine.iOS.Device.SetNoBackupFlag(mPersistentDataPath4Res);
						#endif
					}
				}

				return mPersistentDataPath4Res;
			}
		}

		// 外部头像缓存目录
		public static string PersistentDataPath4Photo
		{
			get
			{
				if (null == mPersistentDataPath4Photo)
				{
					mPersistentDataPath4Photo = PersistentDataPath + "Photos\\";

					IOExtension.CreateDirIfNotExists(mPersistentDataPath4Photo);
				}

				return mPersistentDataPath4Photo;
			}
		}

		// 资源路径，优先返回外存资源路径
		public static string GetResPathInPersistentOrStream(string relativePath)
		{
			var resPersistentPath = string.Format("{0}{1}", FilePath.PersistentDataPath4Res, relativePath);

			if (File.Exists(resPersistentPath))
			{
				return resPersistentPath;
			}
			else
			{
				return FilePath.StreamingAssetsPath + relativePath;
			}
		}

		// 上一级目录
		public static string GetParentDir(string dir, int floor = 1)
		{
			var subDir = dir;

			for (var i = 0; i < floor; ++i)
			{
				var last = subDir.LastIndexOf('/');
				subDir = subDir.Substring(0, last);
			}

			return subDir;
		}

		public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
		{
			if (outResult == null)
			{
				return;
			}

			var dir = new DirectoryInfo(dirName);

			if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System") > -1)
			{
				return;
			}

			var finfo = dir.GetFiles();
			var fname = string.Empty;
			for (var i = 0; i < finfo.Length; i++)
			{
				fname = finfo[i].Name;

				if (fname == fileName)
				{
					outResult.Add(finfo[i].FullName);
				}
			}

			var dinfo = dir.GetDirectories();
			for (var i = 0; i < dinfo.Length; i++)
			{
				GetFileInFolder(dinfo[i].FullName, fileName, outResult);
			}
		}
	}
}
