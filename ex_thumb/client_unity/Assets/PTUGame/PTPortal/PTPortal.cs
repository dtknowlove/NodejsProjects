using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PTGame.Core;
using PTGame.Framework;
using Putao.PaiBloks;
using UnityEngine;


public class PTPortal :MonoBehaviour {

#if UNITY_IOS
		[DllImport ("__Internal")]
		public static extern void  _PortalReady ();
	
		[DllImport ("__Internal")]
		public static extern void  _PortalFinish (string skuId,string configName);
	
		[DllImport ("__Internal")]
		public static extern void  _PortalFinishClickDone (string skuId,string configName);

		[DllImport("__Internal")]
		public static extern void  _SetActivedSkus(string skuList);

		[DllImport("__Internal")]
		public static extern void _SetHasNewResToUpdate(bool value);
	
		[DllImport("__Internal")]
		public static extern void  _CompleteResUpdate();
	
		[DllImport("__Internal")]
	    public static extern string  _GetChildInfo();
	
		[DllImport("__Internal")]
	    public static extern bool  _GuideFinish();
	
	
		#region UMENG
	
     	[DllImport("__Internal")]
	    public static extern void  _StartLevel(string levelId);
	
	    [DllImport("__Internal")]
	    public static extern void  _FinishLevel(string levelId);
	
		[DllImport("__Internal")]
	    public static extern void  _PageBegin(string pageName);
	
	    [DllImport("__Internal")]
	    public static extern void  _EndPage(string pageName);
		
	    #endregion
	
#endif


	public static void GetPTGameVersion(Action<string,string> callback)
	{
		string url = Application.streamingAssetsPath+"/ptgameversion.json";
		if (!url.Contains("file://"))
		{
			url="file://" + url;
		}
		WWW www = new WWW(url);
		float timeCounter = 0;
		while (!www.isDone)
		{

		}
		if (string.IsNullOrEmpty(www.error))
		{
			PTGameVersion gameVersion = JsonUtility.FromJson<PTGameVersion>(www.text);
			callback.InvokeGracefully(gameVersion.version, gameVersion.buildnum);
		}
			
		www.Dispose();
	}

	public class PTGameVersion
	{
		public string version;
		public string buildnum;
	}



	private static AndroidJavaClass ptportal;
	private static void InitAndroid()
	{
		if (ptportal == null)
		{
			ptportal = new AndroidJavaClass("com.putao.ptgame.PTPortal");
		}
	}

	public void SetMusicState(string isOn)
	{
		if (isOn=="true")
		{
			AudioManager.MusicOn();
			AudioManager.SoundOn();
		}
		else
		{
			AudioManager.MusicOff();
			AudioManager.SoundOff();
		}
	}

	public void GetActivedSkuList()
	{
		#if !UNITY_EDITOR && NativeApp
		List<int> skuids = SKUManager.Instance.ActivedSkuList.Select(t => t.SKUCode).ToList();
		string skulist = "";
		if(skuids==null||skuids.Count==0)
		{
			Debug.LogError("ptportal >>>>> no actived ");
		}
		else
		{
			skulist = string.Join(",", skuids.ToStringArray());
		}
	
		#if UNITY_IOS && !UNITY_EDITOR
		{
			_SetActivedSkus(skulist);
		}
	    #endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("SetActivedSkus",skulist);
		#endif
	
		#endif
		
		
	
	}

	public static void PortalFinish()
	{
		
		Debug.LogWarning("unity>>>portalFinish");
		#if UNITY_IOS && !UNITY_EDITOR
			_PortalFinish(GlobalSkuInfo.skuID.ToString(),GlobalSkuInfo.configFile);
		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("PortalFinish");
		#endif
	}

	public static void PortalFinishClickDone()
	{
		Debug.LogWarning("unity>>>PortalFinishClickDone");
		#if UNITY_IOS && !UNITY_EDITOR
			_PortalFinishClickDone(GlobalSkuInfo.skuID.ToString(),GlobalSkuInfo.configFile);
		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("PortalFinishClickDone");
		#endif
	}

	public static void PortalReady()
	{
		Debug.LogWarning("unity>>>PortalReady");
		#if UNITY_IOS && !UNITY_EDITOR
		_PortalReady();
	       
		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("PortalReady");
		#endif
		
	}

	/// <summary>
	/// 通知原生层 开始资源热更
	/// </summary>
	public static void SetHasNewResToUpdate(bool value)
	{
		Debug.LogWarning("unity>>>SetHasNewResToUpdate： "+value);
		#if UNITY_IOS && !UNITY_EDITOR
		_SetHasNewResToUpdate(value);
		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("SetHasNewResToUpdate",value);
		#endif
	}

	/// <summary>
	/// 通知原生层 资源热更结束
	/// </summary>
	public static void CompleteResUpdate()
	{
		Debug.LogWarning("unity>>>CompleteResUpdate");
		#if UNITY_IOS && !UNITY_EDITOR
		_CompleteResUpdate();
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("CompleteResUpdate");
		#endif
	}

	public static void ShowKeyboard()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		InitAndroid();
		ptportal.CallStatic("ShowKeyboard");
		#endif
	}

	public class ChildInfoData
	{
		public string appId;
		public string appCid;
		public string appToken;

		public override string ToString()
		{
			return string.Format("childinfo >>> appid:{0}  appCid:{1} appToken:{2}",appId,appCid,appToken);
		}
	}

	public static ChildInfoData GetChildInfo()
	{
		var childInfoData = new ChildInfoData();

		
		//如果是非整合版本，则默认用1000;
		#if !NativeApp
		childInfoData.appCid = "10000";
		return childInfoData;
		#endif

		
		string childInfo = "1#1#1";
//		string childInfo = "8312#6658107#yPyeyGJuJQGDyAGGPsDGuDQNtKKsGGPPQQuQPPlNtwPPQQQQPPAtDwPPQQyePPJeQePPQQyuPPPPAQPPPPeAPPPPAAewGPlwPPPPKuAAAAGGDPPPKlADtPyAytPPoteK";
		
		#if UNITY_IOS && !UNITY_EDITOR
		 childInfo = _GetChildInfo();
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		 InitAndroid();
		 childInfo = ptportal.CallStatic<string>("GetChildInfo");
		#endif

		try
		{
			string[] datas = childInfo.Split('#');
			childInfoData.appId = datas[0];
			childInfoData.appCid = datas[1];
			childInfoData.appToken = datas[2];
		}
		catch(Exception e)
		{
			childInfoData = null;
		}
		
		Debug.LogWarning(childInfoData.ToString());
		return childInfoData;
	}

	public static bool IsNativeGuideFinish()
	{
		#if UNITY_IOS && !UNITY_EDITOR
		 return _GuideFinish();
		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		 InitAndroid();
		 return ptportal.CallStatic<bool>("GuideFinish");
		#endif
		return false;
	}


	#region UMENG

	#if UNITY_IOS
	public static void StartLevel(string levelId)
	{
			_StartLevel(levelId);
	}

	public static void FinishLevel(string levelId)
	{

			_FinishLevel(levelId);
	}

	public static void BeginPage(string PageName)
	{
			_PageBegin(PageName);
	}

	public static void EndPage(string PageName)
	{
			_EndPage(PageName);
	}
	#endif
	
	#if UNITY_ANDROID
	
	public  static void StartLevel(string levelId)
	{
		InitAndroid();
		ptportal.CallStatic("StartLevel",levelId);
	}

	public static void FinishLevel(string levelId)
	{
		InitAndroid();
		ptportal.CallStatic("FinishLevel",levelId);
	}

	public static void BeginPage(string PageName)
	{
		InitAndroid();
		ptportal.CallStatic("BeginPage",PageName);
	}

	public static void EndPage(string PageName)
	{
		InitAndroid();
		ptportal.CallStatic("EndPage",PageName);
	}
	#endif

	#endregion
	

}
