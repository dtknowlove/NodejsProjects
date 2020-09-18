/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;
using System.Net;
using System;
using UnityEditor;
using System.IO;
namespace PTGame.Editor.PluginManager
{
	public static class PTPluginUpdate {

		public static void UpdatePlugin(string url,string pluginName)
		{
			
			string tempFile = pluginName+".unitypackage";

			Debug.Log (url+">>>>>>:");

			EditorUtility.DisplayProgressBar ("插件更新","插件下载中....",0.5f);

			WebClient client = new WebClient(); 

			client.DownloadFile(new Uri(url), tempFile); 
		
			EditorUtility.ClearProgressBar();

			AssetDatabase.ImportPackage (tempFile,true);

			File.Delete(tempFile);

		}


	}
}
