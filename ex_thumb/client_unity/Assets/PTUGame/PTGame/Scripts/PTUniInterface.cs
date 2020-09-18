/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using PTGame.Core;


namespace PTGame
{
    public static class PTUniInterface
    {
		#if UNITY_IOS
		[DllImport ("__Internal")]
		private static extern string _GetCFBundleVersion ();

		[DllImport ("__Internal")]
		private static extern string GetBundleBuildIOS ();

        [DllImport("__Internal")]
        public static extern string GetDeviceId(string organizationCode);

        [DllImport("__Internal")]
		public static extern void  _ShowNewAppVersionInAppStore(string url,string content,string confirm,string cancle);

		[DllImport("__Internal")]
		public static extern void  _ShowNewAppVersionInAppStoreForeceUpdate(string url,string content,string confirm);

		[DllImport("__Internal")]
		private static extern bool cameraPermissionIsOpen();

		[DllImport("__Internal")]
		private static extern void openCameraPermissionSettingView(string content,string confirm,string cancle);

		[DllImport("__Internal")]
		private static extern bool AlbumsPermissionIsOpen();

		[DllImport("__Internal")]
		private static extern bool WriteDataAsImageToPhotosAlbumIOS(string fileName,byte[] datas,int length);

		[DllImport("__Internal")]
		private static extern bool OpenAlbumsPermissionSettingIOS(string content,string confirm,string cancle);

		[DllImport("__Internal")]
		private static extern bool micPermissionIsOpen();
		[DllImport("__Internal")]
		private static extern void openMicPermissionSettingView(string content,string confirm,string cancle);

		[DllImport("__Internal")]
		private static extern void ShowNotSupportSystemVersionTipsView(string content,string msg,string confirm);


		[DllImport("__Internal")]
		public static extern void  _ShowSystemAlertWithConfirm(string content,string msg,string confirm);


		[DllImport("__Internal")]
		public static extern void _LaunchApp(string appname);
		
		
		[DllImport("__Internal")]
        public static extern string _GetAudioSessionPortType();
        
        
        [DllImport("__Internal")]
        public static extern string _GetAudioSessionPortName();
		
	    [DllImport ("__Internal")]
	    public static extern string _GetSettingsURL();

	    [DllImport ("__Internal")]
	    public static extern void _OpenSettings();
	
		[DllImport("__Internal")]
	    static extern  void _LaunchUrlWithSafariview(string url);
	    
	    [DllImport("__Internal")]
	    static extern  void _DismissSafariView();

	    [DllImport("__Internal")]
	    private static extern string _GetDeviceName();

		#endif


	    
		public static string GetPTDeviceId(){

			#if UNITY_IOS&& !UNITY_EDITOR
			return GetDeviceId("");
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return SystemInfo.deviceUniqueIdentifier;
			#else
			return "deviceid";
			#endif
		}

		/// <summary>
		/// 保存相片到相册
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="datas">Datas.</param>
		/// <param name="length">Length.</param>
		public static void SavePhotoToAlbum(string fileName,byte[] datas,int length){
			#if UNITY_IOS&& !UNITY_EDITOR
			WriteDataAsImageToPhotosAlbumIOS(fileName,datas,length);
			#elif UNITY_ANDROID&&!UNITY_EDITOR
			PTAndroidUtils.SaveImgToAndroid(fileName,datas);
			#endif

		}
		public static void SaveVideoToSDCard(){
			#if UNITY_IOS&& !UNITY_EDITOR

			#endif
		}
	
		/// <summary>
		/// 显示相册权限提示框
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="confirm">Confirm.</param>
		/// <param name="cancle">Cancle.</param>
		public static void GotoAlbumPermissionSetting(string content,string confirm,string cancle){

			#if UNITY_IOS&& !UNITY_EDITOR
			OpenAlbumsPermissionSettingIOS(content,confirm,cancle);
			#endif

		}

		/// <summary>
		/// 检测是否有相册权限
		/// </summary>
		/// <returns><c>true</c> if has album permission; otherwise, <c>false</c>.</returns>
		public static bool HasAlbumPermission(){
			bool hasPermission = true;
			#if UNITY_IOS&& !UNITY_EDITOR
			hasPermission = AlbumsPermissionIsOpen();
			#endif
			return hasPermission;
		}

	
		/// <summary>
		/// Gets the system version.
		/// </summary>
		/// <returns>The system version.</returns>
		public static string GetSystemVersion(){
			#if UNITY_IOS && !UNITY_EDITOR
			return UnityEngine.iOS.Device.systemVersion;
			#endif

			return "7.0";
		}
		/// <summary>
		/// Shows the system version tips view.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="message">Message.</param>
		/// <param name="confirm">Confirm.</param>
		public static void ShowSystemVersionTipsView(string content,string message,string confirm,Action callback){
			PTSystemCallback.Instance.systemNotSupportCallback = callback;
			#if UNITY_IOS && !UNITY_EDITOR
			ShowNotSupportSystemVersionTipsView(content,message,confirm);
			#endif
		}
		/// <summary>
		/// 判断麦克风权限
		/// </summary>
		/// <returns><c>true</c> if has mic permission; otherwise, <c>false</c>.</returns>
		public static bool HasMicPermission()
		{
			bool isOpen = true;
			#if UNITY_IOS && !UNITY_EDITOR
			isOpen = micPermissionIsOpen();
			#endif
			return isOpen;
		}
		/// <summary>
		/// 跳转到麦克风权限设置页面
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="confirm">Confirm.</param>
		/// <param name="cancle">Cancle.</param>
		public static void GotoMicPermissionSettingView(string content,string confirm,string cancle)
		{
			#if UNITY_IOS&& !UNITY_EDITOR
			openMicPermissionSettingView(content,confirm,cancle);
			#endif
		}

	    /// <summary>
	    /// Gotos the camera permission setting view.
	    /// </summary>
		/// <param name="content">Content.未打开摄像头权限／Camera does not have access to game </param>
		/// <param name="confirm">Confirm.立即前往／Grant access</param>
		/// <param name="cancle">Cancle.暂不打开／Do not grant access</param>
		public static void GotoCameraPermissionSettingView(string content,string confirm,string cancle)
		{
			#if UNITY_IOS && !UNITY_EDITOR
			openCameraPermissionSettingView(content,confirm,cancle);
			#endif
		}

		/// <summary>
		/// 检测是否能够打开摄像头
		/// </summary>
		/// <returns><c>true</c> if has camera permission; otherwise, <c>false</c>.</returns>
		public static bool HasCameraPermission()
		{
			bool isOpen = true;
			#if UNITY_IOS&& !UNITY_EDITOR
				isOpen = cameraPermissionIsOpen ();
			#endif
			return isOpen;
		}

		/// <summary>
		/// 弹出app store 有新版本提示。为了适应中英文，需要传入对话框显示内容 content,confirm,cancle
		///https://itunes.apple.com/us/app/ha-ni-hai-yang/id1071378888?mt=8    哈尼海洋
		///https://itunes.apple.com/us/app/tu-tu-shi-jie/id1071377582?mt=8   涂涂世界
		///https://itunes.apple.com/us/app/pu-tao-tan-suo-hao-mo-huan/id1071378496?mt=8   Hello编程
		///https://itunes.apple.com/us/app/mai-si-si-er-tong-shu-xue/id1071377990?mt=8 麦丝斯
		/// </summary>
		/// <param name="url">游戏对应的app store地址,需要问运营部门，从itunues connect中查看</param>
		/// <param name="content">Content.有新版本更新</param>
		/// <param name="confirm">Confirm.现在更新</param>
		/// <param name="cancle">Cancle.暂不更新</param>
		public static void ShowAlert(string url,string content,string confirm,string cancle)
		{
			#if UNITY_IOS&& !UNITY_EDITOR
				_ShowNewAppVersionInAppStore(url,content,confirm,cancle);
			#endif
		}


		/// <summary>
		/// 通用系统提示弹框，只有一个确认键
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="content">Content.</param>
		/// <param name="confirm">Confirm.</param>
		/// <param name="confirmCllback">Confirm cllback.</param>
		public static void ShowSystemAlertWithConfirm(string content,string msg,string confirm,Action confirmCllback){
			#if UNITY_IOS&& !UNITY_EDITOR
			PTSystemCallback.Instance.systemAlertConfirmListener = confirmCllback;
			_ShowSystemAlertWithConfirm(content,msg,confirm);
			#elif UNITY_ANDROID
			PTSystemCallback.Instance.systemAlertConfirmListener = confirmCllback;
			PTAndroidInterface.Instance.ShowSystemAlertWithConfirm(content,msg,confirm);
			#endif
		}

		public static void ShowNewAppVersionInAppStore(string url,string content,string confirm,string cancle,Action confirmCallback,Action cancleCallback){
			PTSystemCallback.Instance.selectUpdateAppListener = confirmCallback;
			PTSystemCallback.Instance.cancleUpdateAppListener  = cancleCallback;
			#if UNITY_IOS && !UNITY_EDITOR
			_ShowNewAppVersionInAppStore(url,content,confirm,cancle);
			#elif UNITY_ANDROID && !UNITY_EDITOR
			PTAndroidInterface.Instance.ShowNewAppVersionInAppStore(url,content,confirm,cancle);
			#endif

		}
		public static void ShowNewAppVersionInAppStoreForeceUpdate(string url,string content,string confirm,Action confirmCallback){
			PTSystemCallback.Instance.selectUpdateAppListener = confirmCallback;
			#if UNITY_IOS&& !UNITY_EDITOR
			_ShowNewAppVersionInAppStoreForeceUpdate(url,content,confirm);
			#elif UNITY_ANDROID && !UNITY_EDITOR
			PTAndroidInterface.Instance.ShowNewAppVersionInAppStoreForceUpdate(url,content,confirm);
			#endif
		}

		/// <summary>
		/// 获取版本号
		/// </summary>
		/// <returns>The app version.</returns>
		public static string GetAppVersion ()
		{
			string m_bundleVersion = Application.version;
			#if UNITY_EDITOR

			#elif UNITY_IOS
			m_bundleVersion = _GetCFBundleVersion();
			#elif UNITY_ANDROID
			m_bundleVersion = Application.version;
			#endif
//			PTDebug.Log (m_bundleVersion+"APPVERSION");
			return m_bundleVersion;
		}

	    /// <summary>
	    ///  ios系统：获取build num 
	    /// Android系统：获取Version Code 
	    /// </summary>
	    /// <returns>The build number.</returns>
	    public static string GetBuildNumOrVersionCode ()
	    {

		    string m_bundleBuild = "0";
			#if UNITY_IOS && !UNITY_EDITOR
			m_bundleBuild = GetBundleBuildIOS();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			m_bundleBuild = PTAndroidInterface.Instance.GetAppVersionCode ().ToString();
			#elif UNITY_EDITOR && UNITY_IOS	
			m_bundleBuild = UnityEditor.PlayerSettings.iOS.buildNumber;
			#elif UNITY_EDITOR && UNITY_ANDROID
			m_bundleBuild = UnityEditor.PlayerSettings.Android.bundleVersionCode.ToString();
			#endif
		    return m_bundleBuild;
	    }

      /// <summary>
      /// Launchs the app.
      /// </summary>
      /// <param name="appurl">Appurl.</param>
	  /// <param name="callback">Callback. 启动失败：-1  启动成功：0 </param>
		public static void LaunchApp(string appurl,Action<int> callback){
			#if UNITY_IOS
			PTSystemCallback.Instance.launchAppCallback = callback;
//			Application.OpenURL (appurl+"://");
			_LaunchApp(appurl+"://");
			#elif UNITY_ANDROID
			int result =PTAndroidInterface.Instance.LaunchApp(appurl);
			if(callback!=null){
				callback(result);
			}
			#endif
		}

	    /// <summary>
	    /// 获取音频输出外设的类型
	    /// </summary>
	    /// <returns></returns>
	    public static string GetAudioSessionPortType()
	    {
		    #if UNITY_IOS && !UNITY_EDITOR

		    return _GetAudioSessionPortType();
	
			#endif

		    #if UNITY_ANDROID && !UNITY_EDITOR
			   return PTAndroidInterface.Instance.GetAudioSessionPortType();
			#endif
		    
		    return null;
	    }

	    /// <summary>
	    /// 获取音频输出外设的名称
	    /// </summary>
	    /// <returns></returns>
	    public static string GetAudioSessionPortName()
	    {
			#if UNITY_IOS && !UNITY_EDITOR

		    return _GetAudioSessionPortName();
	
			#endif
		    
			#if UNITY_ANDROID && !UNITY_EDITOR
			   return PTAndroidInterface.Instance.GetAudioSessionPortName();
			#endif
		    return null;
	    }

//	    /// <summary>
//	    /// 打开通用设置
//	    /// settingPage 1:蓝牙设置页面  2：WiFi设置页面  0 ：设置页面
//	    /// iOS不支持特定页面
//	    /// </summary>
//	    public static void OpenDeviceSetting(int settingPage =0)
//	    {
//		    #if UNITY_IOS&& !UNITY_EDITOR
//		    Application.OpenURL("App-Prefs:root=General");
//			#endif
//		    
//		
//		   
//		    #if UNITY_ANDROID && !UNITY_EDITOR
//                 PTAndroidInterface.Instance.OpenDeviceSetting(settingPage);
//		    #endif
//		    
//
//	    }

	    /// <summary>
	    /// 打开当前应用的设置 iOS 也可以 Application.OpenURL("app-settings:");
	    /// </summary>
	    public static void OpenApplicationSetting()
	    {
		    #if UNITY_IOS		   
		    _OpenSettings();
		    #endif
	    }
	   
	    
	    /// <summary>
	    /// ios 隐私协议跳转显示用
	    /// </summary>
	    /// <param name="url"></param>
	    public static void LaunchUrlWithSafariView(string url)
	    {
			#if UNITY_IOS && !UNITY_EDITOR
  			string t = UnityEngine.iOS.Device.systemVersion.Trim();
		    string[] datas = t.Split('.');
		    int iosVersion = int.Parse(datas[0]);
			if (iosVersion < 9.0)
			{
				Application.OpenURL(url);
			}
			else
			{
				_LaunchUrlWithSafariview(url);
			}
			#endif
		    
			#if UNITY_EDITOR
		    		Application.OpenURL(url);
		    #endif
	    }
 
	    public static void DismissSafariView()
	    {
#if UNITY_IOS
		_DismissSafariView();
#endif
	    }

	    public static bool IsNotchScreen()
	    {

		    #if UNITY_IOS && !UNITY_EDITOR
		    List<string> models = new List<string>(new string[]{"iPhone10,3","iPhone10,6","iPhone11,8","iPhone11,2","iPhone11,6","iPhone11,4"});
		    string modelStr = SystemInfo.deviceModel;
		    if (models.IndexOf(modelStr) >= 0)
		    {
			    return true;
		    }
			#endif
		    
		    #if UNITY_ANDROID && !UNITY_EDITOR
				return PTAndroidInterface.Instance.IsNotchScreen();
			#endif
		    
		    
		    return false;
	    }

	    public static string GetDeviceName()
	    {
#if UNITY_EDITOR
			return UnityEngine.iOS.Device.generation.ToString();
#elif UNITY_IOS
		    return _GetDeviceName();
#else
		    return SystemInfo.deviceModel;
#endif
	    }
    }
}
