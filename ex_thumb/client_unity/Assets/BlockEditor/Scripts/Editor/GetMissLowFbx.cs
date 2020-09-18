using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PTGame.Core;
using UnityEditor;
using UnityEngine;

public class GetMissLowFbx : MonoBehaviour {

	[Serializable]
	public class FBXItem
	{
		public string name;
//		public string time_created;
	}
	[Serializable]
	public class FBXSheet
	{
		public List<FBXItem> items;
	}

	private static List<string> lowPolygonFiles;
	private static void GetLowpolygonFiles()
	{
		var files = Directory.GetFiles("Assets/BlockRes/LowPolygon","*.FBX",SearchOption.AllDirectories);

		lowPolygonFiles = files.ToList();

	}

	[MenuItem("Test/TTT")]
	public static void GetMissFiles()
	{
//		GetLowpolygonFiles();
		var files = Directory.GetFiles("Assets/BlockRes/LowPolygon","*.FBX",SearchOption.AllDirectories);
		
		FBXSheet sheet = new FBXSheet();
		sheet.items = new List<FBXItem>();
		string names = "";
		foreach (var file in files)
		{
			DateTime createdTime = File.GetCreationTime(Path.GetFullPath(file));

			var result = DateTime.Compare(createdTime, Convert.ToDateTime("2019-12-19"));

//			if (!lowPolygonFiles.Exists(s => s.GetFileName() == file.GetFileName()))
			if(result>0)
			{
				sheet.items.Add(new FBXItem()
				{
					name = file.GetFileName(),
//					time_created = File.GetCreationTime(Path.GetFullPath(file)).ToString()
				});
				names += file.GetFileName() + "\n";
				FileUtil.CopyFileOrDirectory(Path.GetFullPath(file),"outsource/"+file.GetFileName());
			}
		}

		var content = JsonUtility.ToJson(sheet, true);
		File.WriteAllText("ttt.json",content);
		
		File.WriteAllText("ttt1111.json",names);

	}
}
