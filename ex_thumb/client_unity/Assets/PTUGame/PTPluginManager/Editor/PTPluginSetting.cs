/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PTGame.Editor.PluginManager
{
	public class PTPluginSetting : EditorWindow {

		
		private PTPluginConfigData mPlugincConfig;
		
		private List<ConfigData> mConfigDatas;
		
		[MenuItem("PuTaoTool/PluginManager/Setting")]
		public static void Init()
		{
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				EditorUtility.DisplayDialog("PTPluginManager", "请连接网络", "确定");
				return;
			}

			PTPluginSetting pluginSetting = (PTPluginSetting)EditorWindow.GetWindow (typeof(PTPluginSetting), true,"PTPlugin Setting",true);
			pluginSetting.position = new Rect (Screen.width / 2, Screen.height / 2, 550, 500);
			pluginSetting.Show ();
		}

		
		public void OnEnable()
		{
			

			mPlugincConfig = PTPluginConfigData.GetPluginConfig ();

			mConfigDatas = PTPluginConfigData.SetDefaultValues(mPlugincConfig);

		}

		void OnGUI()
		{

			int defaultToggleSize = GUI.skin.label.fontSize;
			
			GUILayout.Label("选择需要显示在PluginManager中的插件集", EditorStyles.boldLabel);
			
			foreach(var config in mConfigDatas)
			{
				GUILayout.BeginHorizontal (EditorStyles.helpBox);
				
				var active = EditorGUILayout.Toggle(config.type, config.active);
				
				if(active != config.active)
				{
					config.active = active;
				}
				
				GUILayout.Label(config.name);
				
				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Apply"))
			{
				mPlugincConfig.configDatas = mConfigDatas;
				
				
				EditorUtility.SetDirty(mPlugincConfig);
				
				AssetDatabase.Refresh();
				
				AssetDatabase.SaveAssets();
				
				this.Close();
			}

			GUI.skin.label.fontSize = defaultToggleSize;

		}

	}
}
