/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace PTGame.Editor.PluginManager
{
	
	public class PTPluginConfigData :ScriptableObject
	{
		public float checkInterval;

		public string serverUrl  = "http://10.1.223.240:5010/ptplugin";

		public List<ConfigData> configDatas;
	
		private const string DIR_CONFIGFILE = "Assets/PTUGame/customconfig";
		private const string NAME_CONFIGFILE = "ptpluginconfig.asset";

		private static void CreateDefaultPluginConfig()
		{

			PTPluginConfigData nameInfoObj = ScriptableObject.CreateInstance<PTPluginConfigData>();
			nameInfoObj.configDatas = new List<ConfigData>();

			nameInfoObj.checkInterval = 60;

			nameInfoObj.serverUrl = "http://10.1.223.240:5010/ptplugin";

			nameInfoObj.configDatas = SetDefaultValues(nameInfoObj);

			if(!Directory.Exists(DIR_CONFIGFILE))
			{
				Directory.CreateDirectory (DIR_CONFIGFILE);	
			}
			UnityEditor.AssetDatabase.CreateAsset(nameInfoObj, Path.Combine(DIR_CONFIGFILE,NAME_CONFIGFILE));

			AssetDatabase.Refresh ();

		}

		public static List<ConfigData> SetDefaultValues(PTPluginConfigData pluginConfig)
		{
			PluginCategory[] mPluginTypes = PTPluginUtil.GetServerAllTypes (pluginConfig.serverUrl);
			
			List<ConfigData> mActiveConfigs = new List<ConfigData>();
			
			mActiveConfigs.AddRange(pluginConfig.configDatas);
			
			List<ConfigData> mConfigDatas = new List<ConfigData>();

			foreach (var type in mPluginTypes)
			{
				ConfigData config = new ConfigData();
				
				config.type = type.type;
			
				ConfigData activeConfig = mActiveConfigs.FirstOrDefault(s => s.type == type.type);
				config.enableCheck = activeConfig == null ? false : activeConfig.enableCheck;
				config.enableNew = activeConfig == null ? false : activeConfig.enableNew;
				config.active = activeConfig == null ? false : activeConfig.active;
				if (type.type == "ptgame")
				{
					config.active = true;
				}

				config.name = type.name;
				
				mConfigDatas.Add(config);
			}

			return mConfigDatas;
		}

		public static string GetServerUrl()
		{
			PTPluginConfigData resConfig = GetPluginConfig ();
			return resConfig.serverUrl;
		}
		public static PTPluginConfigData GetPluginConfig()
		{
			string configFile = Path.Combine(DIR_CONFIGFILE,NAME_CONFIGFILE);
			
			if(!File.Exists(configFile))
			{
				CreateDefaultPluginConfig ();
			}
			PTPluginConfigData resConfig = AssetDatabase.LoadAssetAtPath<PTPluginConfigData> (configFile) as PTPluginConfigData;
			
			return resConfig;
		}

		public static string[] GetTypes()
		{
			
			PTPluginConfigData resConfig = GetPluginConfig ();

			string[] types = new string[resConfig.configDatas.Count];

			for(int i=0;i<types.Length;i++)
			{
				types [i] = resConfig.configDatas [i].type;
			}

			return types;
		}
	}


	[System.Serializable]
	public class ConfigData
	{
		public string type;
	
		public bool enableCheck = false;
		public bool enableNew = false;

		public bool active = false;

		[HideInInspector]
		public string name;
	}
}
