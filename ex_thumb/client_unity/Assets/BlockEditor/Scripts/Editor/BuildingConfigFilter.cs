using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Block.Editor;
using PTGame.Core;
using UnityEditor;
using UnityEngine;

public class BuildingConfigFilter :EditorWindow {


	[MenuItem("BlockTool/配置文件查找")]
	public static void Init()
	{
		var window = GetWindow<BuildingConfigFilter>("配置文件查找");
		window.Show();
	}

	private const string ROOTDIR_BUILDINGANIM = "config_paibloks_buildanim";
	private  void CheckConfigs(string prefabName)
	{
		var dirInfo = new DirectoryInfo(ROOTDIR_BUILDINGANIM);
		var files = dirInfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
		int finishedCount = 0;
		int totalCount = files.Length;

		if (prefabName.EndsWith(".txt"))
		{
			prefabName = Path.GetFileNameWithoutExtension(prefabName);
		}


		foreach (var file in files)
		{
			string fileName = Path.GetFileNameWithoutExtension(file.FullName);

			if (ResPublisher.EXCLUDE_BUILDINGS.Contains(fileName))
			{
				continue;
			}
			
			var configText = File.ReadAllText(file.FullName);
			var blockConfigInfo = new PPBlockConfigInfo();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(configText);
			PPLiteracy.LoadBlockInfo(xml, blockConfigInfo);
		
			var configFile = blockConfigInfo.BlockInfos.FirstOrDefault(s=>s.PrefabName == prefabName);
			if (configFile!=null)
			{
				checkResult.Add(fileName);
			}
			
			bool result = EditorUtility.DisplayCancelableProgressBar("配置文件过滤",string.Format(fileName +"检测中....{0}/{1}",finishedCount,totalCount),finishedCount*1.0f/totalCount);
			if (result)
			{
				break;
			}
			finishedCount++;
		}
		EditorUtility.ClearProgressBar();
		
		EditorUtility.DisplayDialog("配置文件过滤","完成","确认");
	}

	private void ReplacePrefabName()
	{
		foreach (var item in checkResult)
		{
			var filePath = Path.Combine(ROOTDIR_BUILDINGANIM, item + ".txt");
			var content = File.ReadAllText(filePath);
			var origin = string.Format("type=\"{0}\"",prefabName);
			var newStr =  string.Format("type=\"{0}\"",replacedName);
			content = content.Replace(origin, newStr);
			File.WriteAllText(filePath,content);
		}
		EditorUtility.DisplayDialog("替换文件名称完成", prefabName + "替换为" + replacedName,"确定");
	}

	private string prefabName="请输入零件名称，注意不是模型名称";
	private List<string> checkResult = new List<string>();
	private Vector2 scrollPos = Vector2.zero;

	private string replacedName;
	private void OnGUI()
	{
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		prefabName = GUILayout.TextField(prefabName);
		if (GUILayout.Button("检测"))
		{
			checkResult.Clear();
			CheckConfigs(prefabName);
		}
		GUILayout.EndHorizontal();
		if (checkResult.Count>0)
		{
			GUILayout.Label("下面的配置文件包含了输入的零件");
			scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.Height(300));
			foreach (var item in checkResult)
			{
				GUILayout.TextField(item);
			}
			GUILayout.EndScrollView();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			
			replacedName = EditorGUILayout.TextField("替换零件名为", replacedName);
			if (GUILayout.Button("替换"))
			{
				ReplacePrefabName();
			}
			
			EditorGUILayout.EndHorizontal();

		}

	}
}
