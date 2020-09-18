/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using UnityEditor;
namespace PTGame.Editor.PluginManager
{
	public static class PTPluginUploader{

		public static string UploadPlugin(string remoteUrl,string name,string type, string version,string readme,string filePath)
		{
			NameValueCollection postContent = new NameValueCollection();
			postContent.Add ("name", name);
			postContent.Add ("type", type);
			postContent.Add ("version", version);
			postContent.Add ("readme",readme);
			string result = HttpHelper.HttpUploadFile(remoteUrl, filePath, postContent);

			return result;
		}
	}

}