using System;
using System.IO;
using System.Linq;
using Block.Editor;
using com.putao.paibloks.editor;
using UnityEditor;
using UnityEngine;

public class ABBuilderWindow : EditorWindow
{
	private const int TimeIntervalDay = -7;//检查间隔周期为一周
	private const string LAST_BUILDTIME_FLAG = "LAST_BUILDTIME_FLAG";
	
	private static string mABVersion
	{
		get { return EditorPrefs.GetString("ABVersion", "100"); }
		set { EditorPrefs.SetString("ABVersion", value); }
	}

	private static DateTime mLastBuildTime
	{
		get { return mCurBuildTimeConfig.LastBuildDateTime; }
		set
		{
			mCurBuildTimeConfig.LastBuildTime = value.ToString("G");
			Save();
		}
	}
	
	private static bool mClearABFloder
	{
		get { return EditorPrefs.GetBool("ClearABFloder",false); }
		set { EditorPrefs.SetBool("ClearABFloder", value); }
	}

	private static string TimeConfigPath
	{
		get { return "./SaveData/BuildTimeConfig.json"; }
	}

	private static BuildTimeConfig mCurBuildTimeConfig;


	private static bool mIsInit = false;
	public static void Init()
	{
		if (mIsInit)
			return;
		if (!EditorPrefs.GetBool(LAST_BUILDTIME_FLAG, false))
		{
			var oldTime = EditorPrefs.GetString("LastBuildTime", DateTime.MinValue.ToString());
			if (!oldTime.Equals(DateTime.MinValue.ToString()))
			{
				var config = new BuildTimeConfig();
				config.LastBuildTime = DateTime.Parse(oldTime).ToString("G");
				config.OldTime = oldTime;
				File.WriteAllText(TimeConfigPath, JsonUtility.ToJson(config,true));
				Debug.LogError(oldTime);
				EditorPrefs.DeleteKey("LastBuildTime");
			}
			EditorPrefs.SetBool(LAST_BUILDTIME_FLAG, true);
		}
		if (File.Exists(TimeConfigPath))
		{
			mCurBuildTimeConfig = JsonUtility.FromJson<BuildTimeConfig>(File.ReadAllText(TimeConfigPath));
		}
		else
		{
			mCurBuildTimeConfig=new BuildTimeConfig();
			mCurBuildTimeConfig.LastBuildTime = DateTime.MinValue.ToString("G");
		}
		
		mIsInit = true;
	}

	public static void Save()
	{
		File.WriteAllText(TimeConfigPath, JsonUtility.ToJson(mCurBuildTimeConfig, true));
	}

	public static void Dispose()
	{
		mIsInit = false;
	}

	public static void Draw()
    {
//	    DrawSetABName();
//	    DrawResVersion();
	    DrawLastBuildTime();
	    DrawToggleClearFloder();
	    DrawBuild();
    }

	private static void DrawSetABName()
	{
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Label("如果有新增fbx、材质等文件，需要设置Assetbundle Name:", GetTipsStyle(15, Color.white), GUILayout.Width(430));
		if (GUILayout.Button("设置 Assetbundle Name",GUILayout.Width(200),GUILayout.Height(20)))
		{
			PETexPrefabGenerator.CreateStickAndTexMaterial();
		    
			SetBlockResAB();
		    
			EditorUtility.DisplayDialog("设置 AssetbundleName","设置AssetbundleName完成","确定");
		}
		GUILayout.EndHorizontal();
	}

	private static void DrawResVersion()
	{
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Label("当前资源版本号:", GetTipsStyle(15, Color.white), GUILayout.Width(120));
		mABVersion = GUILayout.TextField(mABVersion, GetTextAreaStyle(15, Color.cyan), GUILayout.Width(100));
		GUILayout.EndHorizontal();
	}
	
	private static void DrawLastBuildTime()
	{
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Label("上次build时间:", GetTipsStyle(15, Color.white), GUILayout.Width(110));
		GUILayout.Label(mLastBuildTime.ToString("G"), GetTipsStyle(15, Color.cyan), GUILayout.Width(200));
		GUILayout.EndHorizontal();
	}
	
	private static void DrawToggleClearFloder()
	{
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Label("是否完全清除上次build资源内容:", GetTipsStyle(15, Color.white), GUILayout.Width(230));
		mClearABFloder = GUILayout.Toggle(mClearABFloder, GUIContent.none);
		GUILayout.EndHorizontal();
	}

	private static void DrawBuild()
	{
		GUILayout.Space(10);
		if (GUILayout.Button("Build App"))
		{
			BuildABSetting();
			BuildAB(BuildTarget.iOS);
			BuildAB(BuildTarget.Android);
			mLastBuildTime = DateTime.Now;
			EditorUtility.DisplayDialog("App资源生成","生成完成","确定");
		}
		//后续用到后开放
//		GUILayout.Space(10);
//		if (GUILayout.Button("Build Mac", GUILayout.Width(100)))
//		{
//			BuildAB(BuildTarget.StandaloneOSX);
//		}
//	    
//		GUILayout.Space(10);
//		if (GUILayout.Button("Build Windows", GUILayout.Width(100)))
//		{
//			BuildAB(BuildTarget.StandaloneWindows64);
//		}
	}

	private static void BuildABSetting()
	{
		PETexPrefabGenerator.CreateStickAndTexMaterial();
		SetBlockResAB();
	}

	private static void BuildAB(BuildTarget target)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(target);
		CmdBuildAB.Build(TargetStr(target), mClearABFloder);
		if (target == BuildTarget.Android)
		{
			ModelPublisher.CreateModelResDatabaseAndroid();
			
		}else if (target == BuildTarget.iOS)
		{
			ModelPublisher.CreateModelResDatabaseiOS();
		}
	}

	private static string TargetStr(BuildTarget target)
	{
		if (target == BuildTarget.Android)
		{
			return "android";
		}

		if (target == BuildTarget.iOS)
		{
			return "ios";
		}

		if (target == BuildTarget.StandaloneOSX)
		{
			return "mac";
		}

		return "windows";
	}


	#region AB BlockRes
	
	private static void SetBlockResAB()
	{
		SetAbName("BlockRes/LowPolygon/Category_1/Block_Fbxs",".fbx");
		
		SetAbName("BlockRes/CommonRes/Block_Materials",".mat");
        
		SetAbName("BlockRes/LowPolygon/Category_fig/Block_Fbxs",".fbx");
		SetAbName("BlockRes/LowPolygon/Category_pbl/Block_Fbxs",".fbx");
        
		SetAbName("BlockRes/LowPolygon/Category_pbs/Block_Fbxs",".fbx");
        
		SetAbName("BlockRes/LowPolygon/Category_tech/Block_Fbxs",".fbx");
        
		SetAbName("BlockRes/LowPolygon/Category_tech/Block_Materials",".mat");
        
		SetAbName("BlockRes/Stickers/Sticker_Fbxs",".fbx");
		
		SetAbName("BlockRes/Stickers/Sticker_Materials",".mat");
        
		SetAbName("BlockRes/Textures/Texture_Fbxs",".fbx");
		
		SetAbName("BlockRes/Textures/Texture_Materials",".mat");
		
	}
		
	private static void SetAbName(string dir,string  suffix)
	{
		string[] files = Directory.GetFiles(Path.Combine(Application.dataPath,dir), "*", SearchOption.AllDirectories);

		var prefabFiles = files.Where(s => Path.GetExtension(s).ToLower() == suffix).ToArray();

		int totalNum = prefabFiles.Length;
		int counter = 0;
		var offsetdays = mLastBuildTime == DateTime.MinValue ? 0 : TimeIntervalDay;
		var lastTime = mLastBuildTime.AddDays(offsetdays);
		foreach (var item in prefabFiles)  
		{ 
			var prefabPath = item.Replace(Environment.CurrentDirectory + "/", "");
			var importer = AssetImporter.GetAtPath(prefabPath);
			var abName = string.Empty;
			if (mClearABFloder)
			{
				abName = dir + "/" + Path.GetFileNameWithoutExtension(item);
			}
			else
			{
				if (new FileInfo(item + ".meta").LastWriteTime >= lastTime || new FileInfo(item).LastWriteTime >= lastTime)
				{
					abName = dir + "/" + Path.GetFileNameWithoutExtension(item);
				}
				else
				{
					abName = string.Empty;
				}
			}
			if (importer.assetBundleName != abName)
			{
				importer.assetBundleName = abName;
				importer.SaveAndReimport();
			}

			counter++;
			EditorUtility.DisplayProgressBar("设置Assetbundle Name", "正在设置->" + item, counter * 1.0f / totalNum);
		}
		
		EditorUtility.ClearProgressBar();
		
		AssetDatabase.SaveAssets();
	}

	
	#endregion

	private static GUIStyle GetTipsStyle(int fontsize,Color textColor)
	{
		var style = new GUIStyle(GUI.skin.label)
		{
			fontSize = fontsize,
			richText = true,
			alignment = TextAnchor.MiddleLeft
		};
		style.normal.textColor = textColor;
		return style;
	}
	
	private static GUIStyle GetTextAreaStyle(int fontsize,Color color)
	{
		var style = new GUIStyle(GUI.skin.textArea)
		{
			fontSize = fontsize,
			richText = true,
			alignment = TextAnchor.MiddleLeft
		};
		style.normal.textColor = color;
		style.focused.textColor = color;
		return style;
	}


	[Serializable]
	class BuildTimeConfig
	{
		public string OldTime;
		public string LastBuildTime;

		public DateTime LastBuildDateTime
		{
			get { return DateTime.Parse(LastBuildTime); }
		}
	}
}
