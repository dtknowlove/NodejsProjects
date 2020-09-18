using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Block.Editor;
using PTGame;
using UnityEditor;
using UnityEngine;

public class ResPublisherWin : EditorWindow {

#if BLOCK_MODEL
	[MenuItem("资源发布/布鲁可资源发布",false,PublisherGlobal.ItemOrder.AppConfigs)]
#endif
	static void Init()
	{
		var window = GetWindow<ResPublisherWin>("资源发布");
		window.Show();
	}
	private static string BuildAnimDir = "./config_paibloks_buildanim";
	private readonly string[] tableNames = {"积木模型Ab", "资源库", "搭建动画配置"};
	private const int INDEX_TAB_AB = 0;
	private const int INDEX_TAB_DATABASE = 1;
	private const int INDEX_TAB_CONFIGS = 2;

	private const string ENVSTRING = "ENVSTRING";

	private List<string> mBuildingAnimFiles = new List<string>();
	
	private List<string> mOtherAnimFiles;
	
	private List<SkuCarData> mCarItems;

	private List<SkuData> mSkuDatas;

	private string mFocusSkuName = "";
	
	private int mTabIndex = 0;
	

	private void OnGUI()
	{
		mTabIndex = GUILayout.Toolbar(mTabIndex, tableNames, GUILayout.Width(700),GUILayout.Height(30));
		switch (mTabIndex)
		{
			case INDEX_TAB_AB:
				ABBuilderWindow.Draw();
				break;
			case INDEX_TAB_DATABASE:
				DrawResGenaerteView();
				break;
			case INDEX_TAB_CONFIGS:
				DrawBuidingAnimConfig();
				break;
		}
	}
	
	private void OnEnable()
	{
		ABBuilderWindow.Init();
		mBuildingAnimFiles = new List<string>();
		mEnv = (PTRuntime) EditorPrefs.GetInt(ENVSTRING, 0);

		var files = Directory.GetFiles(BuildAnimDir, "*.txt", SearchOption.TopDirectoryOnly);
		mBuildingAnimFiles.AddRange(files);

		UpdateData();
	}

	private void OnDisable()
	{
		SaveData();
	}
	
	void OnDestroy()
	{
		ABBuilderWindow.Dispose();
	}

	private void UpdateData()
	{
		BuildAnimDir = BlockServerUtil.GetBuildAnimPath(IsOnline) + "/";
		
		Debug.Log("当前目录:" + BuildAnimDir);
		mSkuDatas = BlockServerUtil.GetBlokDatas(IsOnline);
		
		mOtherAnimFiles = mBuildingAnimFiles
			.Where(s => mSkuDatas.SkuCarDatas().All(k => k.model_sku_id != Path.GetFileNameWithoutExtension(s))).ToList();
	}

	private void SaveData()
	{
		EditorPrefs.SetInt(ENVSTRING, (int) mEnv);	
		Debug.Log("===>>>Save ResPublisherWin Data!");
	}

	private bool mSkuFolderOut = false, mNoSkuFolderOut = false;
	private PTRuntime mEnv = PTRuntime.ONLINE;
	private bool IsOnline
	{
		get { return mEnv == PTRuntime.ONLINE; }
	}

	private string EnvText
	{
		get { return IsOnline ? "(正式环境)" : "(测试环境)"; }
	}

	private void DrawBuidingAnimConfig()
	{
		GUILayout.Space(10);
		//选择环境
		GUILayout.BeginHorizontal(GUILayout.Width(200));
		GUILayout.Label("当前环境:", GetTipsStyle(18, Color.green), GUILayout.Height(30));
		mEnv = (PTRuntime) EditorGUILayout.EnumPopup(mEnv, GetEnumPopStyle(13, 20), GUILayout.Width(100), GUILayout.Height(30));
		GUILayout.Space(20);
		DrawUpdateSkuData(IsOnline);
		GUILayout.EndHorizontal();

		GUILayout.Label("注意:1.确保已更新本地sku数据！\n       2.需要先生成[单个搭建]的模型的配置文件,再生成[SKU]对应的模型配置文件", GetTipsStyle(16, Color.red));

		GUILayout.Space(10);
		mSkuFolderOut = EditorGUILayout.Foldout(mSkuFolderOut, "Sku搭建动画配置", GetFloderOutStyle(18,Color.green));
		if (mSkuFolderOut)
			DrawSkusView();
//		GUILayout.Space(10);
//		mNoSkuFolderOut = EditorGUILayout.Foldout(mNoSkuFolderOut, "非Sku搭建动画配置", GetFloderOutStyle(18,Color.green));
//		if (mNoSkuFolderOut)
//			DrawNotSkusView();
	}

	private void DrawUpdateSkuData(bool isOnline)
	{
		if (GUILayout.Button("从服务器更新本地sku数据" + EnvText,GUILayout.Width(300),GUILayout.Height(20)))
		{
			EditorUtility.DisplayProgressBar("请求数据", "请求中...", 0.5f);
			BlockServerUtil.RequestData(isOnline, success =>
			{
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayDialog("请求结果", success ? "请求成功" : "请求失败，请重试!", "好");
				if (success)
				{
					UpdateData();
					Repaint();
				}
			});
		}
	}


	#region Sku config view
	
	private Vector2 mScrollPosNotSku = Vector2.zero;
	private void DrawNotSkusView()
	{
		GUILayout.Space(10);

		GUILayout.BeginHorizontal();

		if (MainButton("生成所有文件对应的缩略图配置文件",30))
		{
			ResPublisher.CreateBuildingThumbConfigs(mOtherAnimFiles.ToArray(),IsOnline);
		}
		if (MainButton("生成所有文件对应的模型配置文件",30))
		{
			ModelPublisher.CreateCustomConfig(mOtherAnimFiles.ToArray(),IsOnline);
		}

		GUILayout.EndHorizontal();

		GUILayout.Space(10);
		
		mScrollPosNotSku= GUILayout.BeginScrollView(mScrollPosNotSku, GUILayout.Width(600));
		foreach (var item in mOtherAnimFiles)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(Path.GetFileNameWithoutExtension(item),GUILayout.Width(150));
			if (GUILayout.Button("缩略图配置",GUILayout.Width(120)))
			{
				ResPublisher.CreateBuildingThumbConfigs(new []{item},IsOnline);
			}
			if (GUILayout.Button("模型配置",GUILayout.Width(120)))
			{
				ModelPublisher.CreateCustomConfig(new []{item},IsOnline);
			}
			
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
	}

	private void DrawSkusView()
	{
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();

		if (MainButton("生成所有搭建动画对应的缩略图配置文件", 30))
		{
			HashSet<string> configFiles = new HashSet<string>();
			mSkuDatas.SkuCarDatas().ForEach(s =>
			{
				if (File.Exists("./" + BuildAnimDir + s.model_sku_id + ".txt"))
				{
					configFiles.Add(s.model_sku_id + ".txt");
				}
				else
				{
					Debug.LogErrorFormat("文件>>>>{0}不存在,请确保已更新数据！","./" + BuildAnimDir + s.model_sku_id + ".txt");
				}

			});

			ResPublisher.CreateBuildingThumbConfigs(configFiles.ToArray(), IsOnline);
		}
		if (MainButton("生成所有搭建动画对应的模型配置文件", 30))
		{
			HashSet<string> configFiles = new HashSet<string>();
			mSkuDatas.SkuCarDatas().ForEach(s =>
			{
				if (File.Exists("./" + BuildAnimDir + s.model_sku_id + ".txt"))
				{
					configFiles.Add(s.model_sku_id + ".txt");
				}
				else
				{
					Debug.LogErrorFormat("文件>>>>{0}不存在,请确保已更新数据！", s.model_sku_id);
				}

			});
			ModelPublisher.CreateCustomConfig(configFiles.ToArray(), IsOnline);
		}

		if (MainButton("生成所有SKU对应的共用模型配置文件", 30))
		{
			ModelPublisher.CreateCustomSkuModelResConfig(mSkuDatas, IsOnline);
		}

		GUILayout.EndHorizontal();

		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		DrawSkuView();
		if (!string.IsNullOrEmpty(mFocusSkuName))
		{
			DrawSkuCarView();
		}
		GUILayout.EndHorizontal();
	}


	private Vector2 mScrollSkuPos  = Vector2.zero;
	private void DrawSkuView()
	{
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		mScrollSkuPos = GUILayout.BeginScrollView(mScrollSkuPos,GUILayout.Width(550));
		foreach (var item in mSkuDatas)
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox);
			GUILayout.Label(item.block_sku_id.ToString(),GUILayout.Width(100));
			if (GUILayout.Button(item.title, GUILayout.Width(200)))
			{
				mFocusSkuName = item.title;
				mCarItems = item.Models.ToList();
			}
			GUILayout.Space(10);
			if (GUILayout.Button("生成SKU共用模型配置",GUILayout.Width(150)))
			{
				var t = new List<SkuData>();
				t.Add(item);
				ModelPublisher.CreateCustomSkuModelResConfig(t, IsOnline);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}
	
	private void DrawSkuCarView()
	{
		if (mCarItems == null)
		{
			return;
		}
		GUILayout.Space(10);
		if (EditorGUILayout.Foldout(true, mFocusSkuName))
		{
			GUILayout.BeginVertical();
			GUILayout.Space(30);
			GUILayout.BeginHorizontal();
			if (MainButton("生成下面所有文件对应的缩略图配置文件",30))
			{
				List<string> configFiles = new List<string>();
				mCarItems.ForEach(s => { configFiles.Add(s.model_sku_id);});
				ResPublisher.CreateBuildingThumbConfigs(configFiles.ToArray(),IsOnline);
			}
			if (MainButton("生成下面所有文件对应的模型配置文件",30))
			{
				List<string> configFiles = new List<string>();
				mCarItems.ForEach(s => { configFiles.Add(s.model_sku_id);});
				ModelPublisher.CreateCustomConfig(configFiles.ToArray(),IsOnline);
			}
			GUILayout.EndHorizontal();
		
			GUILayout.Space(5);
		
			foreach (var item in mCarItems)
			{
				GUILayout.BeginHorizontal();
			
				GUILayout.Label(item.model_sku_id,GUILayout.Width(150));
				GUILayout.Label(item.title, GUILayout.Width(150));
				if (GUILayout.Button("生成缩略图配置",GUILayout.Width(120)))
				{
					ResPublisher.CreateBuildingThumbConfigs(new []{item.model_sku_id},IsOnline);
				}
				if (GUILayout.Button("生成模型配置",GUILayout.Width(120)))
				{
					ModelPublisher.CreateCustomConfig(new []{item.model_sku_id},IsOnline);
				}

				GUILayout.EndHorizontal();
			}
		
			GUILayout.EndVertical();
		}
	}

	private bool MainButton(string text, float height)
	{
		return GUILayout.Button(text + EnvText, GUILayout.Height(height));
	}

	#endregion

	private void DrawResGenaerteView()
	{
		DrawResItem("1.缩略图有更新时需生成", "缩略图的resdatabse文件 -->resdatabse/resconfig_thumbs_primitive.json", "缩略图hash文件", ResPublisher.CreateThumbPrimitiveConfig);
		DrawResItem("2.BlockData数据库中零件、模型、颜色等有更新时需生成", "BlockData配置文件 -->ftpres/config_blockdata", "BlockData配置文件", ResPublisher.BlockDataConfig);
		DrawResItem("3.APP搭建模块配表(如:搭建步骤提示更改)有更新时需生成", "APP搭建模块配表 -->ftpres/config_paibloks_blockbuild", "APP搭建模块配表", ResPublisher.BlockBuildConfig);;
	}

	private void DrawResItem(string title,string content,string finishtitle,Action doAction)
	{
		GUILayout.Space(10);
		if (EditorGUILayout.Foldout(true, title,GetFloderOutStyle(18,Color.green)))
		{
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.Space(30);
			GUILayout.TextField(content, GetTextAreaStyle(15, Color.cyan), GUILayout.Width(700));
			if (GUILayout.Button("生成",GUILayout.Width(200),GUILayout.Height(20)))
			{
				doAction();
				EditorUtility.DisplayDialog(finishtitle, "生成完成", "确定");
			}
			GUILayout.EndHorizontal();
		}
	}
	
	[MenuItem("资源发布/刷新CDN缓存")]
	private static void CleanCDNCache()
	{
		var dirInfo = new DirectoryInfo(Application.dataPath+"/../../");
		//  Application.dataPath + "/../../update_cdn.sh"; 
		
		var ShellPath = "."+dirInfo.FullName+"update_cdn.sh";
		Debug.LogError(ShellPath);
		StringBuilder sbArgs = new StringBuilder();
		sbArgs.Append(ShellPath);
		sbArgs.AppendFormat(" {0}", "game_buluke");

		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo = new System.Diagnostics.ProcessStartInfo("/bin/bash", sbArgs.ToString());
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.Start();

		string outputStr = process.StandardOutput.ReadToEnd() + "\n";
		process.WaitForExit();
		process.Close();

		Debug.Log(">>>>> " + outputStr);
		
		EditorUtility.DisplayDialog("App资源生成","生成完成\n"+outputStr,"确定");
	}

	private static GUIStyle GetTipsStyle(int fontsize,Color textColor)
	{
		var style = new GUIStyle(GUI.skin.label)
		{
			fontSize = fontsize,
			richText = true
		};
		style.normal.textColor = textColor;
		return style;
	}
	
	private static GUIStyle GetEnumPopStyle(int fontsize,float fixheight)
	{
		var style = new GUIStyle(EditorStyles.popup)
		{
			fontSize = fontsize,
			richText = true,
			alignment = TextAnchor.MiddleCenter
		};
		style.fixedHeight = fixheight;
		return style;
	}
	
	private static GUIStyle GetFloderOutStyle(int fontsize,Color color)
	{
		var style = new GUIStyle(EditorStyles.foldout)
		{
			fontSize = fontsize,
			richText = true,
		};
		style.normal.textColor=color;
		style.active.textColor=color;
		style.focused.textColor=color;
		style.hover.textColor=color;
		return style;
	}
	
	private static GUIStyle GetTextAreaStyle(int fontsize,Color color)
	{
		var style = new GUIStyle(GUI.skin.textArea)
		{
			fontSize = fontsize,
			richText = true
		};
		style.normal.textColor = color;
		style.focused.textColor = color;
		return style;
	}
}
