/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using PTGame.Editor.PluginManager;
using System;
namespace PTGame.Editor.PluginManager
{
	public class PTPluginManager : EditorWindow {

		private List<PluginCategory> localCategories ;
		private List<PluginCategory> remoteCategories;

		private GUIStyle titleStyle;
		private GUIStyle newVersionStyle;
		private GUIStyle installedStyle;
		private GUIStyle defaultStype;
		private GUIStyle focusCategoryStyle;

		private PluginCategory curLocalCategory;

		private PTPluginConfigData resConfig;

		[MenuItem("PuTaoTool/PluginManager/Manager")]
		public static void Init()
		{
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				EditorUtility.DisplayDialog("PTPluginManager", "请连接网络", "确定");
				return;
			}
			
			PTPluginManager pluginManager = (PTPluginManager)EditorWindow.GetWindow (typeof(PTPluginManager), true,"PTPlugin Manager",true);
			pluginManager.position = new Rect (Screen.width / 2, Screen.height / 2, 600, 500);
			pluginManager.Show ();
		}


		public void OnEnable()
		{
			
			InitStyles ();

			resConfig = PTPluginConfigData.GetPluginConfig ();

			remoteCategories = PTPluginUtil.GetRemoteCategories(resConfig);

			localCategories = PTPluginUtil.GetLocalCategories(resConfig);
			
			curLocalCategory = PTPluginUtil.GetLocalCategoryByType (localCategories,resConfig.configDatas.FirstOrDefault(s=>s.active).type);

		}

		private Vector2 mScrollPos;
		public void OnGUI()
		{
			mScrollPos = GUILayout.BeginScrollView (mScrollPos, true,true ,GUILayout.Width(600), GUILayout.Height(480));
			
			GUILayout.BeginHorizontal ("box");
			foreach(var configData in resConfig.configDatas)
			{
				if (configData.active)
				{
					if (curLocalCategory.type == configData.type) {

						GUI.backgroundColor = Color.yellow;
						
						GUILayout.Button(configData.type, focusCategoryStyle, GUILayout.Width(100));
						
						GUI.backgroundColor = Color.white;
					} 
					else
					{
						if (GUILayout.Button (configData.type, GUILayout.Width (100))) 
						{

							curLocalCategory = PTPluginUtil.GetLocalCategoryByType (localCategories,configData.type);
						}
					}
				}

			}

			GUILayout.EndHorizontal ();

			DrawCategory (curLocalCategory);
			
			GUILayout.EndScrollView ();
		}

		public void DrawCategory(PluginCategory localCategory)
		{

				if (remoteCategories.Count == 0) {

					DrawNoServer (localCategory);
				} 
			    else 
				{
					PluginCategory remoteCategory = remoteCategories.FirstOrDefault(s=>s.type == localCategory.type);;

					DrawWithServer (localCategory,remoteCategory);
				}
		}

		private void InitStyles ()
		{
			titleStyle = new GUIStyle ();
			titleStyle.fontStyle = FontStyle.Bold;
			titleStyle.fontSize = 15;
			titleStyle.alignment = TextAnchor.MiddleCenter;
			titleStyle.normal.textColor = Color.white;

			defaultStype = new GUIStyle ();
			defaultStype.fontSize = 15;
			defaultStype.normal.textColor = new Color(1,1,1);
			defaultStype.alignment = TextAnchor.MiddleLeft;

			newVersionStyle = new GUIStyle ();
			newVersionStyle.fontSize = 15;
			newVersionStyle.alignment = TextAnchor.MiddleCenter;
			newVersionStyle.normal.textColor = new Color(1,0,0);


			installedStyle = new GUIStyle ();
			installedStyle.fontSize = 15;
			installedStyle.alignment = TextAnchor.MiddleCenter;
			installedStyle.normal.textColor = Color.green;


			focusCategoryStyle = EditorStyles.miniButton;
			focusCategoryStyle.fontStyle = FontStyle.Bold;
			focusCategoryStyle.fontSize = 15;
			focusCategoryStyle.alignment = TextAnchor.MiddleCenter;
			focusCategoryStyle.normal.textColor = Color.white;
			focusCategoryStyle.richText = true;


		}

		private void DrawWithServer(PluginCategory localCategory, PluginCategory serverCategory)
		{
			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Plugin", titleStyle, GUILayout.Width(150));
			GUILayout.Label ("Server",titleStyle, GUILayout.Width(100));
			GUILayout.Label ("Local",titleStyle, GUILayout.Width(100));
			GUILayout.Label ("Action",titleStyle, GUILayout.Width(100));
			GUILayout.Label ("Readme",titleStyle, GUILayout.Width(100));

			GUILayout.EndHorizontal ();


			foreach (var serverPlugin in serverCategory.plugins)
			{
				GUILayout.BeginHorizontal (EditorStyles.helpBox);

				GUILayout.Label (serverPlugin.name,defaultStype,GUILayout.Width(150));

				GUILayout.Label (serverPlugin.version,titleStyle,GUILayout.Width(100));

				PluginInfo localPlugin = localCategory.GetPluginInfoByName (serverPlugin.name);

				if (localPlugin != null) {

					if(PTPluginUtil.HasNewVersion(serverPlugin.version,localPlugin.version))
					{
						GUILayout.Label (localPlugin.version, newVersionStyle, GUILayout.Width (100));

						if (GUILayout.Button ("Update", GUILayout.Width (90))) 
						{
							if(EditorUtility.DisplayDialog("UpdatePlugin","是否移除本地旧版本?","是","否"))
							{
								if (!string.IsNullOrEmpty(localPlugin.url))
								{
									Directory.Delete(localPlugin.url,true);
									
									AssetDatabase.Refresh();
								}	
							}
							PTPluginUpdate.UpdatePlugin (serverCategory.url+"/"+serverPlugin.url,serverPlugin.name+"_v"+serverPlugin.version);
						}
					}else{
						GUILayout.Label (localPlugin.version, installedStyle, GUILayout.Width (100));

						if (GUILayout.Button ("Import", GUILayout.Width (90))) {
							
							if(EditorUtility.DisplayDialog("UpdatePlugin","是否移除本地旧版本?","是","否"))
							{
								if (!string.IsNullOrEmpty(localPlugin.url))
								{
									Directory.Delete(localPlugin.url,true);
									
									AssetDatabase.Refresh();
								}	
							}
							
							PTPluginUpdate.UpdatePlugin (serverCategory.url+"/"+serverPlugin.url,serverPlugin.name+"_v"+serverPlugin.version);
						}
					}

				} else {
					
					GUILayout.Label (" ", newVersionStyle,GUILayout.Width (100));

					if (GUILayout.Button ("Import", GUILayout.Width (90))) {
						
						PTPluginUpdate.UpdatePlugin (serverCategory.url+"/"+serverPlugin.url,serverPlugin.name+"_v"+serverPlugin.version);
					}
				}

				if(GUILayout.Button("Readme",GUILayout.Width(90))){

					ShowReadMe (serverPlugin,serverPlugin.readme);
				}
					
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndVertical ();

		}

		private void ShowReadMe(PluginInfo serverPlugin,PluginReadme readme)
		{
			PTPluginReadmeWin.Init (serverPlugin,readme);
		}

		private void DrawNoServer(PluginCategory category)
		{
			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Plugin", titleStyle, GUILayout.Width(150));
			GUILayout.Label ("Local", titleStyle, GUILayout.Width(100));

			GUILayout.EndHorizontal ();

			foreach (var plugin in category.plugins)
			{
				GUILayout.BeginHorizontal (EditorStyles.helpBox);

				GUILayout.Label (plugin.name,GUILayout.Width(150));

				GUILayout.Label (plugin.version,GUILayout.Width(100));

				GUILayout.EndHorizontal ();
			}

			GUILayout.EndVertical ();
		}
				
	}

	[Serializable]
	public class PluginCategory
	{
		public string url = "";
		public int id = 0;
		public string name;
		public string type;

		public List<PluginInfo> plugins = new List<PluginInfo>();

		public PluginInfo GetPluginInfoByName(string pluginName)
		{
			var pl = plugins.Where (s => s.name == pluginName);

			if (pl.Count() > 1) {
				Debug.LogError ("**** there should not  have multi plugin:"+ pluginName +" , please contact to wanzhenyu@putao.com **** ");
			
			}else if(pl.Count() ==0){
				return null;
			}
			return pl.ElementAt(0);
		}
	}


	[Serializable]
	public class PluginInfo
	{
		public int id ;
		public string name = "";
		public string version = "";
		public string type = "";
		public PluginReadme readme;
		public string fileContent = "";
		public string url= "";

		public PluginInfo()
		{
			readme = new PluginReadme ();
		}
	}

	[Serializable]
	public class ReadmeItem
	{
		public ReadmeItem()
		{

		}
		public ReadmeItem(string version,string content,string author,string date)
		{
			this.version = version;
			this.content = content;
			this.author = author;
			this.date = date;
		}
		public string version = "";
		public string content = "";
		public string author  = "";
		public string date    = "";
	}

	[Serializable]
	public class PluginReadme
	{
		public List<ReadmeItem> items;

		public ReadmeItem GetItem(string version)
		{
			if (items == null || items.Count == 0) {
				return null;
			}
			return items.First(s => s.version == version);
		}

		public void AddReadme(ReadmeItem pluginReadme)
		{
			if (items == null) {

				items = new List<ReadmeItem> ();
				items.Add (pluginReadme);

			} else {

				bool exist = false;
				foreach(var item in items)
				{
					if(item.version == pluginReadme.version){
						item.content = pluginReadme.content;
						item.author = pluginReadme.author;
						exist = true;
						break;
					}
				}
				if (!exist)
				{
					items.Add (pluginReadme);
				}

			}
		}
	
	}
}