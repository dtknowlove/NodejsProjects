/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System;
using System.Collections;
using System.IO;
using BestHTTP;
using PTGame.Core;
using UnityEngine;

namespace PTGame.ModuleUpdate {
	
	public static class PTModuleUpdateUtil {
//		/// <summary>
//		/// 请求服务器文本
//		/// </summary>
//		/// <param name="url"></param>
//		/// <param name="timeout"></param>
//		/// <param name="callback"></param>
//		/// <returns></returns>
//		public static IEnumerator RequestTextFromServer (string url, int timeout, Action<int,string> callback)
//		{
//			UnityWebRequest request = UnityWebRequest.Get(url);
//			request.timeout = timeout;
//			yield return request.SendWebRequest();
//
//			if (request.error != null||string.IsNullOrEmpty(request.downloadHandler.text))
//			{
//				callback.InvokeGracefully(-1, request.error);
//
//			} else {
//				
//				callback.InvokeGracefully(0, request.downloadHandler.text);
//				
//			}
//			request.Dispose();
//		}
		
		
		public static IEnumerator GetTextFromServer (string from, float timeout, Action<int,string> callback)
		{
			 HTTPRequest mHttpRequest = null;
			 mHttpRequest = new HTTPRequest(new Uri(from),
				(req, resp) =>
				{
					if (resp != null && req.Exception  == null)
					{
						if (mHttpRequest!=null)
						{
							mHttpRequest.Abort();
							mHttpRequest.Dispose();
							mHttpRequest = null;
						}
						callback(0, resp.DataAsText);
					}
					else
					{
						var errorInfo = req.Exception;
						if (mHttpRequest != null)
						{
							mHttpRequest.Abort();
							mHttpRequest.Dispose();
							mHttpRequest = null;
						}
						Debug.LogError(from+"  >>>>>配置文件获取失败");
						callback(-1, errorInfo!=null?errorInfo.ToString():"ERROR");
					}
				})
			{
				ConnectTimeout = TimeSpan.FromSeconds(5), Timeout = TimeSpan.FromSeconds(10), DisableCache = true
			};

			mHttpRequest.Send();
	
			yield return 0;

		}
		
		
//		public static IEnumerator GetTextFromServer (string from, float timeout, Action<int,string> callback)
//		{
//			WWW loader = new WWW (from);
//			
//			float duration = 0.0f;
//			
//			while (!loader.isDone && duration < timeout) 
//			{
//				duration += Time.deltaTime;
//				yield return 0;
//			}
//
//			if (duration >= timeout)
//			{
//				if (callback != null) {callback (-1,"请求超时");}
//			}
//			else if(loader.error!=null)
//			{
//				if (callback != null) {callback (-1,loader.error);}
//			}
//			else if(!string.IsNullOrEmpty(loader.text))
//			{
//				if (callback != null) {callback (0,loader.text);}
//			}
//			else
//			{
//				if (callback != null) {callback (-1,"未知错误");}
//			}
//
//			loader.Dispose ();
//		}
		
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,bool enableOverWrite)
		{
			var dir = new DirectoryInfo(sourceDirName);
			var dirs = dir.GetDirectories();
			
			if (!dir.Exists)
			{
				PTDebug.LogError(string.Format("Source directory does not exist or could not be found: {0} ",sourceDirName));				
			}
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			
			var files = dir.GetFiles();
			
			foreach (var file in files)
			{
				string tempPath = Path.Combine(destDirName, file.Name);
				
				file.CopyTo(tempPath, enableOverWrite);
			}
			if (copySubDirs)
			{
				foreach (var subDir in dirs)
				{
					string tempPath = Path.Combine(destDirName, subDir.Name);
					
					DirectoryCopy(subDir.FullName, tempPath, copySubDirs,enableOverWrite);
				}
			}
		}

	}
}