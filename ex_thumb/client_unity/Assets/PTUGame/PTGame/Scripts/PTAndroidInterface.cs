/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/
using UnityEngine;
using System.Collections;
using PTGame.Core;

namespace PTGame
{
	public static class PTAndroidDeviceType
	{
		public const string DEVICE_PAIBOT = "Paibot";
		public const string DEVICE_PAIPAD = "Paipad";
		public const string DEVICE_UNKNOWN = "unknown";
	}

	public class PTAndroidInterface
	{

		private static PTAndroidInterface androidInterface;

		public static PTAndroidInterface Instance
		{
			get
			{
				if (androidInterface == null)
				{
					androidInterface = new PTAndroidInterface();
				}

				return androidInterface;
			}
		}

		private AndroidJavaObject jo;

		private AndroidJavaClass ptcustom;

		private AndroidJavaClass notchChecker; 


		public PTAndroidInterface()
		{

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

			ptcustom = new AndroidJavaClass("com.putao.ptgame.PTCustom");

			notchChecker = new AndroidJavaClass("com.putao.ptgame.PTNotchChecker");


		}

		public void ShowNewAppVersionInAppStore(string url, string content, string confirm, string cancle)
		{
			ptcustom.CallStatic("ShowNewAppVersionInAppStore", new object[] {jo, url, content, confirm, cancle});

		}

		public void ShowNewAppVersionInAppStoreForceUpdate(string url, string content, string confirm)
		{
			ptcustom.CallStatic("ShowNewAppVersionInAppStoreForceUpdate", new object[] {jo, url, content, confirm});

		}

		/// <summary>
		/// Executes the keyboard adjust.
		/// 为了解决unity 在android 上弹出键盘后，webcamtexture卡顿的问题
		/// </summary>
		public void ExecuteKeyboardAdjust()
		{

			ptcustom.CallStatic("StartKeyboardAdjust", jo);
		}

		public int GetAppVersionCode()
		{
			return ptcustom.CallStatic<int>("GetAppVersionCode", jo);
		}

		public AndroidJavaObject GetAndroidJavaObj()
		{
			return jo;
		}

		public AndroidJavaObject GetPTCustomJavaObj()
		{
			return ptcustom;
		}

		//SetPaiBotAngleForGame
		public void CallMethod(string methodName, params object[] args)
		{
			jo.Call(methodName, args);
		}

		public string GetPTDeviceType()
		{

			return jo.CallStatic<string>("GetPTDeviceType");
		}

		public bool IsEduVersion()
		{

			return jo.CallStatic<bool>("IsEduVersion");
		}

		public void ShowExitAppTip(string tipContent)
		{

			ptcustom.CallStatic<int>("ShowExitAppTip", jo, tipContent);
		}

		public void ExitApp()
		{

			ptcustom.CallStatic<int>("ExitApp", jo);
		}

		/// <summary>
		/// Shows the toast.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="type">Type.  0 short, 1 long </param>
		public void ShowToast(string content, int type)
		{
			ptcustom.CallStatic("ShowToast", jo, content, type);
		}

		public void ShowSystemAlertWithConfirm(string title, string msg, string confirm)
		{
			ptcustom.CallStatic<int>("ShowSystemAlertWithConfirm", jo, title, msg, confirm);
		}


		public const int TOAST_LENGTH_SHORT = 0;
		public const int TOAST_LENGTH_LONG = 1;

		/// <summary>
		/// Launchs the app.
		/// </summary>
		/// <returns>The app. 启动失败：-1  启动成功：0 </returns>
		/// <param name="pkgName">Package name.</param>
		public int LaunchApp(string pkgName)
		{
			int result = ptcustom.CallStatic<int>("LaunchApp", pkgName, jo);
			return result;

		}


		public void HidePutaoSplash()
		{
			PTAndroidInterface.Instance.CallMethod("hidePutaoSplash");
		}


		/// <summary>
		/// 获取音频输出外设类型
		/// </summary>
		/// <returns></returns>
		public string GetAudioSessionPortType()
		{
			bool isBluetoothA2dpOn = ptcustom.CallStatic<bool>("IsBluetoothA2dpOn",jo);
			
			if (isBluetoothA2dpOn)
			{
				return "BluetoothA2DPOutput";
			}
			
			return null;
		}

		
	    /// <summary>
	    /// 获取音频输出外设的名称
	    /// </summary>
	    /// <returns></returns>
		public string GetAudioSessionPortName()
		{
			string audioSessionName = ptcustom.CallStatic<string>("GetAudioSessionPortName",jo);


			return audioSessionName;
		}

		/// <summary>
		/// 获取当前连接着的经典蓝牙设备名称
		/// </summary>
		/// <returns></returns>
		public string GetConnectedBluetoothName()
		{
			string deviceName = ptcustom.CallStatic<string>("GetConnectedBluetoothName", jo);
			return deviceName;
		}

		/// <summary>
		/// 是否含有指定名称得输出设备音频输出设备；
		/// 注意判断先判断类型，在判断设备名称；因为有连接上，但可能并不是当前使用的设备；
		/// 因为红米note4  GetAudioSessionPortName（） 函数返回的为 BluetoothA2dp ，所以额外提供此方法。
		/// if(GetAudioSessionPortName()=="BluetoothA2dp"){
		///     if(IsContainAudioDevice(deviceName)){
		///      }
		/// }
		/// </summary>
		/// <param name="deviceName"></param>
		/// <returns></returns>
		public bool IsContainAudioDevice(string deviceName)
		{
			bool result = ptcustom.CallStatic<bool>("IsContainAudioDevice",jo,deviceName);

			return result;
		}

		public void OpenDeviceSetting(int settingPage = 0)
		{
			PTAndroidInterface.Instance.GetPTCustomJavaObj().CallStatic("OpenSetting",jo,settingPage);
		}

		public bool IsNotchScreen()
		{
			return notchChecker.CallStatic<bool>("hasNotchScreen", jo);
		}
	}

}

	
