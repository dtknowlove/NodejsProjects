using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PTGame.ResKit;
using UnityEditor;
using UnityEngine;

public class ResKitTool
{

	private static string GroupPath
	{
		get
		{
			var path = "Android";
#if UNITY_IOS
			path = "iOS";
#endif
			return Application.streamingAssetsPath + "/AssetBundles/" + path + "/";
		}
	}
		
	[MenuItem("Assets/PutaoTool/Update ResGroup")]
	private static void UpdateResGroup()
	{
		var dirs = Directory.GetDirectories(GroupPath);
		var dataGroups = new List<ResGroupConfig>();
			
		foreach (var s in dirs)
		{
			var di = new DirectoryInfo(s);
			dataGroups.Add(new ResGroupConfig()
			{
				GroupName = di.Name,
				DefaultLoad = false,
				SimulateLoad = true
			});
		}
		var resConfig = ResKitConfig.GetConfig();
		foreach (var resGroup in resConfig.ResGroups)
		{
			var name = resGroup.GroupName;
			var startStr = name.Substring(0, name.LastIndexOf("_", StringComparison.Ordinal));
			dataGroups.ForEach(item =>
			{
				if (item.GroupName.StartsWith(startStr))
				{
					item.DefaultLoad = resGroup.DefaultLoad;
					item.SimulateLoad = resGroup.SimulateLoad;
				}
			});
		}
		resConfig.ResGroups = dataGroups;

		EditorUtility.SetDirty(resConfig);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}
