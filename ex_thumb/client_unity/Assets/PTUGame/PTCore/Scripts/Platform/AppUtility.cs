
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Core
{
	public class AppUtility
	{
		//设计分辨率
		private static int RWidth = 1280;
		private static int RHeight = 720;

		/// <summary>
		/// 设置设计分辨率
		/// </summary>
		public static void SetReferenceResolution(int width, int height)
		{
			RWidth = width;
			RHeight = height;
		}

		/// <summary>
		/// 设置APP的分辨率
		/// </summary>
		public static void ApplyAppResoulution(CanvasScaler canvasScale)
		{
			if (canvasScale == null)
				return;
			canvasScale.referenceResolution=new Vector2(RWidth,RHeight);
			canvasScale.matchWidthOrHeight = IsMatchWidth() ? 0 : 1;
		}

		public static void ApplyRectTransResoulution(RectTransform rectTransform)
		{
			if (rectTransform == null)
				return;
			rectTransform.anchorMin = IsMatchWidth() ? new Vector2(0, 0.5f) : new Vector2(0.5f, 0);
			rectTransform.anchorMax = IsMatchWidth() ? new Vector2(1, 0.5f) : new Vector2(0.5f, 1);
			rectTransform.anchoredPosition=Vector2.zero;
			rectTransform.sizeDelta = IsMatchWidth() ? new Vector2(0, RHeight) : new Vector2(RWidth, 0);
		}

		/// <summary>
		/// 测试出来的一个比例
		/// </summary>
		private const float DPIRadio = 40.0f;
		
		/// <summary>
		/// 根据屏幕DPI设置EventSystem的拖拽阈值
		/// </summary>
		public static void ApplyAppDragThresholdByDPI()
		{
			int threshold=(int)Mathf.Ceil (Screen.dpi / DPIRadio);
			Debug.LogFormat("Screen:{0},now:{1}", Screen.dpi, threshold);
			UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = threshold;
		}
		
		private static bool IsMatchWidth()
		{
			var aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect < (16.0f / 9);
		}
		
		
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,bool enableOverWrite)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, enableOverWrite);
			}
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs,enableOverWrite);
				}
			}
		}
	}
}
