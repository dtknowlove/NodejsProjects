/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PTGame
{
	public class PTAndroidUtils
	{

		public static void SaveImgToAndroid(string fileName, byte[] imagebytes)
		{
			SaveImgToAndroid("putao", fileName, imagebytes);
		}

		public static void SaveImgToAndroid(string albumName, string fileName, byte[] imagebytes)
		{
			string destination = GetAndroidAlbumPath(albumName, false);

			destination = destination + "/" + fileName;
			try
			{
				File.WriteAllBytes(destination, imagebytes);
			}
			catch (Exception ex)
			{
				Debug.LogError("SaveImgToAndroid : " + ex.Message);
			}

			AndroidJavaObject ajo = PTAndroidInterface.Instance.GetPTCustomJavaObj();

			if (SystemInfo.deviceModel.ToLower().Contains("meizu") || SystemInfo.deviceModel.ToLower().Contains("vivo") ||
			    SystemInfo.deviceModel.ToLower().Contains("gionee"))
				ajo.CallStatic("ScanSDCardFile", destination, PTAndroidInterface.Instance.GetAndroidJavaObj(), true);
			else
				ajo.CallStatic("ScanSDCardFile", destination, PTAndroidInterface.Instance.GetAndroidJavaObj(), false);
		}

		//Android 获取多媒体相关路径
		public static string GetAndroidAlbumPath(string albumName, bool bIsVideo)
		{
			string destination = string.Empty;
			destination = "/mnt/sdcard/DCIM/Camera";
			if (PTAndroidInterface.Instance.GetPTDeviceType() == PTAndroidDeviceType.DEVICE_UNKNOWN)
			{
				if (bIsVideo == true)
				{
					if (!Directory.Exists(destination))
					{
						destination = "/mnt/sdcard/DCIM/Video";
						if (!Directory.Exists(destination))
						{
							Directory.CreateDirectory(destination);
						}
					}
					else
					{

						if (SystemInfo.deviceModel.ToLower().Contains("meizu"))
						{
							destination = "/mnt/sdcard/DCIM/Video";
							Directory.CreateDirectory(destination);
						}
						else if (SystemInfo.deviceModel.ToLower().Contains("vivo"))
						{
							destination = "/mnt/sdcard/相机";
							Directory.CreateDirectory(destination);
						}

					}
				}
				else
				{
					if (!Directory.Exists(destination))
					{
						if (!Directory.Exists("/mnt/sdcard/DCIM"))
						{
							destination = "/storage/emulated/0/Camera";
						}
						else
						{
							destination = "/mnt/sdcard/DCIM/" + albumName;
							Directory.CreateDirectory(destination);
						}
					}
				}
			}
			else //paibot
			{
				if (!Directory.Exists(destination))
					Directory.CreateDirectory(destination);
			}

			return destination;
		}

	}
}