/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

namespace PTGame.Editor.PluginManager
{
	public static class PTPluginUtil {

		public static bool HasNewVersion(string remoteVersion,string localVersion)
		{

			Version v1 = new Version(remoteVersion);
				
			Version v2 = new Version(localVersion);


			if (v1.CompareTo(v2) > 0)
			{
				return true;
			}

			return false;
		}

		public static PTPluginInfos GetPluginWithName(string serverUrl ,string name )
		{
			WWW www = new WWW (serverUrl+string.Format("/getpluginwithname?name={0}",name));

			while (!www.isDone) 
			{

			}
			if(!string.IsNullOrEmpty(www.error) || String.IsNullOrEmpty(www.text))
			{
				return null;
			}
			
			PTPluginInfos pluginInfos = JsonUtility.FromJson<PTPluginInfos> (www.text);
			
	
			return pluginInfos;

		}

		
		public static PluginCategory[] GetServerAllTypes(string serverUrl)
		{
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				return null;
			}

			WWW www = new WWW (serverUrl+"/getplugintypes");

			while (!www.isDone) 
			{

			}
			if(www.error!=null)
			{
				return null;
			}

			string json = www.text;
			var types = JsonUtilityHelper.GetJsonArray<PluginCategory>(json);
			return types;
		}

		public static string[] GetServerAllTypeNames(string serverUrl)
		{
			PluginCategory[] categories = GetServerAllTypes(serverUrl);
			
			List<string> categoryNames = new List<string>();
			
			foreach (var category in categories)
			{
				categoryNames.Add(category.type);
			}

			return categoryNames.ToArray();
		}

		public static List<PluginCategory> GetRemoteCategories(PTPluginConfigData resConfig)
		{

			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				return null;
			}

			WWW www = new WWW (resConfig.serverUrl+"/getplugins");

			while (!www.isDone) 
			{

			}
			if(www.error!=null)
			{
				return null;
			}

			string json = www.text;

			PTPluginInfos pluginInfos = JsonUtility.FromJson<PTPluginInfos> (json);
			
			List<PluginCategory> categories = new List<PluginCategory>();

			foreach (var config in resConfig.configDatas)
			{
				
				if (config.active)
				{
					PluginCategory category = new PluginCategory();
					category.url = resConfig.serverUrl;
					category.type = config.type;
					category.plugins = pluginInfos.plugins.Where(s => s.type == config.type).ToList();
					categories.Add(category);
				}
			}

			return categories;

		}

		public static PluginCategory GetRemoteCategoryByType(List<PluginCategory> categories,string type)
		{
			return categories.FirstOrDefault(s => s.type == type);
		}

		public static PluginCategory GetLocalCategoryByType(List<PluginCategory> categories,string categoryType)
		{
			return categories.FirstOrDefault(s=>s.type == categoryType);
		}


		public static List<PluginCategory> GetLocalCategories(PTPluginConfigData resConfig)
		{
			List<PluginCategory> localCategories = new List<PluginCategory>();

			string[] files = System.IO.Directory.GetFiles (Application.dataPath, "ptplugin.txt", System.IO.SearchOption.AllDirectories);

			foreach (var config in resConfig.configDatas)
			{
			
				PluginCategory pluginCategory = new PluginCategory ();

				pluginCategory.type = config.type;

				foreach (var fileInfo in files) 
				{

					string content = File.ReadAllText (Path.GetFullPath (fileInfo));

					PluginInfo pluginInfo = JsonUtility.FromJson<PluginInfo> (content);

					if (pluginInfo.type == config.type)
					{
						pluginCategory.plugins.Add (pluginInfo);
					}
				}
				localCategories.Add (pluginCategory);

			}

			return localCategories;

		}
			

		[Serializable]
		public class PTPluginInfos
		{
			public List<PluginInfo> plugins;
		}

	}
}
