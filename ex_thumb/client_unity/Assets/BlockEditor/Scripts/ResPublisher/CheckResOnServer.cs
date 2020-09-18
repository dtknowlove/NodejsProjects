using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Block.Editor;
using Putao.GameCommon;
using UniRx;
//using UnityEditor;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class CheckResOnServer : MonoBehaviour {
	
//	private const string PATH_CSV_SKU = "ftpres/config_paibloks_platform/Sku.csv";
//	private List<AnimConfigItem> animConfigFiles;
//	private List<string> mSkuConfigFiles;
//	private string mAnimErrorMsg = "";
//	private int mTotalSkuCarCount = 0;
//	private int mCurAnimConfigIndex = 0;
//	private int mSuccessCounter = 0;
//
//	private SkuConfig mSkuConfig;
//
//	public bool isAndroidPlatform = false;
//	public string resVersion = "530";
//	public bool isOnlineList = true;
//	public ResCheckType mResCheckTypeChecked;
//	
//	public enum ResCheckType
//	{
//		Config_BuildingAnim,
//		Config_Thumbs,
//		Config_ModelRes,
//		Config_SKUModelRes
//	}
//	
//	class AnimConfigItem
//	{
//		public string configFile;
//		public int retryTimes = 0;
//	}
//	
//	// Use this for initialization
//	void Start ()
//	{
//		ServerDefine.isAndroidPlatform = isAndroidPlatform;
//		ServerDefine.mResVersion = resVersion;
//		
//		var content = File.ReadAllText(PATH_CSV_SKU);
//		mSkuConfig = SkuConfig.LoadConfig (content) as SkuConfig;
//
//		GetSkuConfig();
//	}
//
//	private void GetSkuConfig()
//	{
//		ObservableWWW.Get(Url).Subscribe((result) =>
//		{
//			mData = JsonUtility.FromJson<ResponseData>(result);
//			mData.Sort();
//			//设置进度默认值
//			mTotalSkuCarCount = GetSkuCarCount();
//
//			CheckRes(mResCheckTypeChecked);
//
//		}, (e) =>
//		{
//			EditorUtility.DisplayDialog("错误","拉取sku列表失败","确定");
//		});
//	}
//
//	private void CheckRes(ResCheckType resCheckType)
//	{
//		switch (resCheckType)
//		{
//			case ResCheckType.Config_BuildingAnim:
//				CheckAnimConfig();
//				break;
//			case ResCheckType.Config_Thumbs:
//				StartCoroutine(CheckThumbConfigs());
//				break;
//			case ResCheckType.Config_ModelRes:
//				StartCoroutine(CheckModelResConfigs());
//				break;
//			case ResCheckType.Config_SKUModelRes:
//				StartCoroutine(CheckSkuModelResConfigs());
//				break;
//		}
//	}
//
//	private void CheckAnimConfig()
//	{
//		StartCoroutine(DownloadConfig("config_paibloks_buildanim",animConfigFiles[mCurAnimConfigIndex].configFile, (result, content) =>
//		{
//			if (result == -1)
//			{
//				mAnimErrorMsg += animConfigFiles[mCurAnimConfigIndex].configFile+"\n";
//				Debug.LogError("download failed:"+animConfigFiles[mCurAnimConfigIndex].configFile);
//
//				if (animConfigFiles[mCurAnimConfigIndex].retryTimes < 3)
//				{
//					animConfigFiles[mCurAnimConfigIndex].retryTimes++;
//				}
//			}
//			else
//			{
//				mCurAnimConfigIndex++;
//				mSuccessCounter++;
//				Debug.LogFormat("完成{0}个/{1}",mSuccessCounter,mTotalSkuCarCount);
//			}
//		
//			if (mCurAnimConfigIndex < animConfigFiles.Count())
//			{
//				CheckAnimConfig();
//			}
//			else
//			{
//				EditorApplication.isPlaying = false;
//				string msg = "总共"+animConfigFiles.Count()+"个文件，成功下载"+mSuccessCounter+"个";
//				if (mSuccessCounter != animConfigFiles.Count())
//				{
//					msg += "下载失败的有" + mAnimErrorMsg;
//				}
//				EditorUtility.DisplayDialog("搭建文件检测", msg, "OK");
//			}
//		}));
//	}
//
//	private IEnumerator CheckThumbConfigs()
//	{
//		var items = new List<string>();
//		foreach (var item in animConfigFiles)
//		{
//			items.Add(item.configFile);
//		}
//		
//		UpdateThumbs updateThumbs = new UpdateThumbs(items.ToArray());
//		while (!updateThumbs.Execute(Time.deltaTime))
//		{
//			yield return null;
//		}
//		EditorApplication.isPlaying = false;
//		if (!updateThumbs.Success)
//		{
//			
//			EditorUtility.DisplayDialog("检测缩略图文件", "失败","确定");
//			yield break;
//		}
//		else
//		{
//			EditorUtility.DisplayDialog("检测缩略图文件", "成功", "确定");
//		}
//	}
//
//	private IEnumerator CheckModelResConfigs()
//	{
//		var items = new List<string>();
//		foreach (var item in animConfigFiles)
//		{
//			items.Add(item.configFile);
//		}
//		UpdateModelRes updateModelRes = new UpdateModelRes(items.ToArray(),false);
//		while (!updateModelRes.Execute(Time.deltaTime))
//		{
//			yield return null;
//		}
//		EditorApplication.isPlaying = false;
//		if (!updateModelRes.Success)
//		{
//			EditorUtility.DisplayDialog("检测搭建模型资源文件", "失败","确定");
//			yield break;
//		}
//		else
//		{
//			EditorUtility.DisplayDialog("检测搭建模型资源文件", "成功", "确定");
//		}
//	}
//
//	private IEnumerator CheckSkuModelResConfigs()
//	{
//		var items = new List<string>();
//		foreach (var item in animConfigFiles)
//		{
//			items.Add(item.configFile);
//		}
//		
//		UpdateModelRes updateModelRes = new UpdateModelRes(items.ToArray(),true);
//		while (!updateModelRes.Execute(Time.deltaTime))
//		{
//			yield return null;
//		}
//		EditorApplication.isPlaying = false;
//		if (!updateModelRes.Success)
//		{
//			EditorUtility.DisplayDialog("检测SKU搭建模型资源文件", "失败","确定");
//			yield break;
//		}else{
//			EditorUtility.DisplayDialog("检测SKU搭建模型资源文件", "成功","确定");
//		}
//	}
//
//	private IEnumerator DownloadConfig(string dir,string animConfigFile,Action<int,string> callback)
//	{
//		string url = "https://apk-download.putaocdn.com/game_buluke/version_{0}/"+dir+"/"+animConfigFile+".txt";
//		url = string.Format(url, resVersion);
//		UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
//		unityWebRequest.timeout = 10;
//		yield return unityWebRequest.SendWebRequest();
//
//		if (string.IsNullOrEmpty(unityWebRequest.error))
//		{
//			callback(0,unityWebRequest.downloadHandler.text);
//		}
//		else
//		{
//			Debug.LogError(url+">>>>"+unityWebRequest.error);
//			callback(-1,unityWebRequest.error);
//		}
//	}
//	
//	private int GetSkuCarCount(int blokindex = 0)
//	{
//		animConfigFiles = new List<AnimConfigItem>();
//		mSkuConfigFiles = new List<string>();
//		
//		if (mData == null || blokindex >= mData.data.Length)
//			return 0;
//		var result = 0;
//		for (var i = blokindex; i < mData.data.Length; i++)
//		{
//			int blockSkuId = mData.data[i].block_sku_id;
//
//			var  sku = mSkuConfig.Items.FirstOrDefault(s => s.SkuId == blockSkuId);
//			if (sku == null)
//			{
//				Debug.LogError(blockSkuId+"can not found >>>>>>");
//				continue;
//			}
//
//			mSkuConfigFiles.Add(sku.ConfigFile);
//			
//			result += mData.data[i].models.Length;
//			foreach (var item in mData.data[i].models)
//			{
//				animConfigFiles.Add(new AnimConfigItem(){configFile = item.model_sku_id});
//			}
//		}
//		Debug.Log(animConfigFiles.Count+" >>>>>>count >>>>");
//		return result;
//	}
//
//	private string Url
//	{
//		get
//		{
//			if (isOnlineList)
//			{
//				return "https://api-blocks.putao.com/blocks/title/list";
//			}
//			return "http://test-api-blocks.ptdev.cn/blocks/title/list";
//		}
//	}
//	
//	[Serializable]
//	private class ResponseData
//	{
//		public string code = "";
//		public BlokData[] data;
//
//		public bool IsDataNull
//		{
//			get { return data != null && data.Length <= 0; }
//		}
//
//		public void Sort()
//		{
//			if (IsDataNull)
//				return;
//			Array.Sort(data, (a, b) =>
//			{
//				if (a.block_sku_id < b.block_sku_id) return -1;
//				if (a.block_sku_id > b.block_sku_id) return 1;
//				return 0;
//			});
//		}
//	}
//		
//	[Serializable]
//	private class BlokData
//	{
//		public int block_sku_id;
//		public SkuData[] models;
//		public string title;
//
//	}
//		
//	[Serializable]
//	private class SkuData
//	{
//		public string model_sku_id;
//		public string title;
//			
//	}
//
//	private ResponseData mData = null;
//}
//public class CheckResOnServerWin : EditorWindow
//{
//	private CheckResOnServer.ResCheckType mResCheckType;
//	private bool isAndroidPlatform = true;
//	private ResCheckPlatform mResPlatform;
//	private string mResVersion = "530";
//	private bool mIsOnlineList = true;
//	
//	[MenuItem("资源发布/检测FTP资源")]
//	private static void Init()
//	{
//		var window = GetWindow<CheckResOnServerWin>();
//		window.Show();
//	}
//
//	enum ResCheckPlatform
//	{
//		iOS,
//		Android
//	}
//
//	void OnGUI()
//	{
//		mIsOnlineList = EditorGUILayout.Toggle("根据线上后台配置的SKU检测", mIsOnlineList);
//		mResCheckType = (CheckResOnServer.ResCheckType)EditorGUILayout.EnumPopup("检测类型",mResCheckType);
//		mResPlatform = (ResCheckPlatform)EditorGUILayout.EnumPopup("平台",mResPlatform);
//		mResVersion = EditorGUILayout.TextField("资源版本", mResVersion);
//		
//		if(GUILayout.Button("开始检测"))
//		{
//			if (Directory.Exists(Application.persistentDataPath))
//			{
//				Directory.Delete(Application.persistentDataPath,true);
//			}
//
//			Directory.CreateDirectory(Application.persistentDataPath);
//
//			EditorSceneManager.OpenScene("Assets/BlockEditor/Scenes/CheckResOnServer.unity");
//			var checkOnServer =  FindObjectOfType<CheckResOnServer>();
//			checkOnServer.isAndroidPlatform = (mResPlatform == ResCheckPlatform.Android);
//			checkOnServer.mResCheckTypeChecked = mResCheckType;
//			checkOnServer.resVersion = mResVersion;
//			checkOnServer.isOnlineList = mIsOnlineList;
//			AssetDatabase.SaveAssets();
//
//			EditorApplication.isPlaying = true;
//		}
//		
//		if (GUILayout.Button("打开下载目录"))
//		{
//			EditorUtility.RevealInFinder(Application.persistentDataPath);
//		}
//	}
//
//	[MenuItem("资源发布/刷新CDN缓存")]
//	private static void CleanCDNCache()
//	{
//		var dirInfo = new DirectoryInfo(Application.dataPath+"/../../");
//		//  Application.dataPath + "/../../update_cdn.sh"; 
//		
//		var ShellPath = "."+dirInfo.FullName+"/update_cdn.sh";
//		Debug.LogError(ShellPath);
//		StringBuilder sbArgs = new StringBuilder();
//		sbArgs.Append(ShellPath);
//		sbArgs.AppendFormat(" {0}", "game_buluke");
//
//		System.Diagnostics.Process process = new System.Diagnostics.Process();
//		process.StartInfo = new System.Diagnostics.ProcessStartInfo("/bin/bash", sbArgs.ToString());
//		process.StartInfo.UseShellExecute = false;
//		process.StartInfo.RedirectStandardOutput = true;
//		process.StartInfo.RedirectStandardError = true;
//		process.Start();
//
//		string outputStr = process.StandardOutput.ReadToEnd() + "\n";
//		process.WaitForExit();
//		process.Close();
//
//		Debug.Log(">>>>> " + outputStr);
//		
//		EditorUtility.DisplayDialog("App资源生成","生成完成\n"+outputStr,"确定");
//	}
}