/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Xml;

namespace PTGame.Editor.PluginManager
{
	
	[InitializeOnLoad]
	public class PTPluginCheck
	{
		enum CheckStatus
		{
			WAIT,
			COMPARE,
			NONE
		}

		private CheckStatus mCheckStatus;

		private double mNextCheckTime = 0;
		
		private double mCheckInterval = 60;

		private int mConfigIndex = 0;

		private PTPluginConfigData mPluginConfig;

		static PTPluginCheck ()
		{
			
			if (!EditorApplication.isPlayingOrWillChangePlaymode) 
			{
				PTPluginCheck pluginCheck = new PTPluginCheck ();
				pluginCheck.mCheckStatus = CheckStatus.WAIT;
				pluginCheck.mNextCheckTime = EditorApplication.timeSinceStartup;

				pluginCheck.mPluginConfig = PTPluginConfigData.GetPluginConfig ();
				pluginCheck.mCheckInterval = pluginCheck.mPluginConfig.checkInterval;
				EditorApplication.update = pluginCheck.CustomUpdate;
				
			}
			

		}

		public void ForceUpdate(){
		
			if(this.mCheckStatus!=CheckStatus.WAIT)
			{
				return;
			}
			mNextCheckTime = EditorApplication.timeSinceStartup;
		}

		private void CustomUpdate ()
		{
			switch (mCheckStatus) {
			case CheckStatus.WAIT:
				if (EditorApplication.timeSinceStartup >= mNextCheckTime) 
				{
					GoToCompare();
				}
				break;

			case CheckStatus.COMPARE:
				
				ProcessCompare ();

				break;
			}
		}

		private void GoToCompare ()
		{

			bool hasConfig = ReCheckConfigDatas ();

			if (hasConfig) 
			{
				mCheckStatus = CheckStatus.COMPARE;
			
			} else {
				
				mCheckStatus = CheckStatus.WAIT;
			}

		}

		private void GoToWait ()
		{
			mCheckStatus = CheckStatus.WAIT;
			
			mNextCheckTime = EditorApplication.timeSinceStartup + mCheckInterval;
		}
			

	
		private bool ReCheckConfigDatas ()
		{
			List<ConfigData> configDatas = mPluginConfig.configDatas;

			if (configDatas.Count == 0)
			{
				return false;
			}
		
			mCheckInterval = mPluginConfig.checkInterval;

			return true;

		}
			
		private void ProcessCompare ()
		{
	
			mConfigIndex++;

			if(mConfigIndex>=mPluginConfig.configDatas.Count)
			{
				mConfigIndex = 0;
			}

			ConfigData configData = mPluginConfig.configDatas[mConfigIndex];

			if(!configData.enableCheck)
			{
				GoToWait ();

				return;
			}

			Debug.Log ("***** update res check ******");

			Debug.Log ("check plugin :"+configData.type+"  configIndex:"+mConfigIndex+"   count:"+mPluginConfig.configDatas.Count);

			List<PluginCategory> categories = PTPluginUtil.GetRemoteCategories(mPluginConfig);
			PluginCategory remoteCategory = PTPluginUtil.GetRemoteCategoryByType(categories,configData.type);

			if(remoteCategory == null)
			{
				GoToWait ();

				return;
			}
				
			List<PluginCategory> localCategories = PTPluginUtil.GetLocalCategories(mPluginConfig);
			
			PluginCategory localCategory = PTPluginUtil.GetLocalCategoryByType(localCategories,configData.type);


			if (localCategory != null) {
									
				foreach (var remotePlugin in remoteCategory.plugins) {
				
					PluginInfo localPlugin = localCategory.GetPluginInfoByName (remotePlugin.name);
				
					if (localPlugin == null) {

						if (configData.enableNew) 
						{
							ShowNewVersionDialog (configData.type);
							break;
						}

					} else {
										
						bool remoteHasNewVersion = PTPluginUtil.HasNewVersion (remotePlugin.version, localPlugin.version);

						if (remoteHasNewVersion) {
							ShowNewVersionDialog (configData.type);

							break;
						}
					}
									
				}
			}
			else 
			{
				
				ShowNewVersionDialog (configData.type);
			}
			
			GoToWait ();
		}

		private void ShowNewVersionDialog(string pluginType)
		{
			
			bool result = EditorUtility.DisplayDialog ("PTPluginManager", string.Format ("{0}有插件更新,请前往查看", pluginType), "前往查看", "稍后查看");

			if(result)
			{
				EditorApplication.ExecuteMenuItem ("PuTaoTool/PluginManager");
			}
			
		}


		public static void ClearLocalDir (string localDir)
		{
			if (Directory.Exists (localDir))
			{
				Directory.Delete (localDir, true);
			}
		}


	}
}
