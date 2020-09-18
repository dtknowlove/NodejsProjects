
#if BLOCK_EDITOR

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using PTGame.EditorCoroutines;
using UnityEditor;
using UnityEngine;

namespace Block.Editor
{
	public class ResConfigCreator : EditorWindow
	{	
		[MenuItem("BlockTool/生成搭建资源配置",false,BlockEditorGlobal.Export_Obj)]
		
		static void Init()
		{
			var window = EditorWindow.GetWindow<ResConfigCreator>("生成搭建资源配置");
			window.Show();
		}

		private string configFilePath = "";

		private bool isShowAdvance = false;

		private void OnGUI()
		{
			GUILayout.Space(10);
			if (GUILayout.Button("同步数据"))
			{
				SyncDataFromServer();
			}
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.TextField(configFilePath);
			

			if (GUILayout.Button("选择搭建文件"))
			{
				configFilePath = EditorUtility.OpenFilePanelWithFilters("选择搭建文件", "", new string[] {"", "txt"});
				
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			if (GUILayout.Button("生成"))
			{
				if (!string.IsNullOrEmpty(configFilePath))
				{
					CopyBuildingFiles2FtpRes(new[]{configFilePath});
				
					ResPublisher.CreateBuildingThumbConfigs(new[]{configFilePath});
					ModelPublisher.CreateCustomConfig(new[]{configFilePath});
					EditorUtility.DisplayDialog("生成资源文件", "资源文件生成完成", "确定");
				}
			}
			GUILayout.Space(20);
			if (GUILayout.Button("上传"))
			{
				UploadFile(Path.GetFileNameWithoutExtension(configFilePath));
			}

			GUILayout.Space(20);
			
			isShowAdvance = GUILayout.Toggle(isShowAdvance, "高级");

			if (!isShowAdvance)
			{
				return;
			}

			GUILayout.Space(20);
			if (GUILayout.Button("选择目录"))
			{
				var folder = EditorUtility.OpenFolderPanel("选择搭建文件目录", "", "");

				if (!string.IsNullOrEmpty(folder))
				{
					string[] files = Directory.GetFiles(folder).Where(s=>s.EndsWith("txt")).ToArray();
					
					CopyBuildingFiles2FtpRes(files);
					ResPublisher.CreateBuildingThumbConfigs(files);
					ModelPublisher.CreateCustomConfig(files);
					EditorUtility.DisplayDialog("生成资源文件", "资源文件生成完成", "确定");
				}
			}

		}


		private void UploadFile(string configFile)
		{
			string[] dirs = { "config_buildanim_modelres_android","config_buildanim_modelres_ios",
				"config_paibloks_buildanim_thumbs","config_paibloks_buildanim" };

			int successCount = 0;
			int finishCounter = 0;
			for (int i=0;i<dirs.Length;i++)
			{
				var url = dirs[i];
				var filepath = Path.Combine("ftpres", dirs[i] + "/" + configFile);
				if (i ==3 )
				{
					filepath += ".txt";
				}

				this.StartCoroutine(UploadImage(url,filepath, (result, msg) =>
				{
					finishCounter++;
					if (result == 0) successCount++;
					if (finishCounter == 4)
					{
						EditorUtility.DisplayDialog("上传配置文件", successCount == 4 ? "上传成功" : "上传失败", "确定");
					}
				}));
			}
		}
		
		IEnumerator UploadImage(string url,string imageFile, Action<int, string> callback)
		{
			var fileName = Path.GetFileName(imageFile);
			byte[] levelData = File.ReadAllBytes(imageFile);
			WWWForm form = new WWWForm();
			form.AddBinaryData("upfile", levelData, fileName, "application/octet-stream");
			WWW w = new WWW("http://10.1.223.240:8081/"+url, form);
			yield return w;
			if (!string.IsNullOrEmpty(w.error))
			{
				Debug.LogError(w.error);
				callback(-1, w.error);
			}
			else
			{
				Debug.Log(w.text);
				callback(0, w.text);
			}
		}

		private void CopyBuildingFiles2FtpRes(string[] configFiles)
		{
			foreach (var configFile in configFiles)
			{
				if (!Directory.Exists(PublisherGlobal.ROOTDIR_BUILDINGANIM))
				{
					Directory.CreateDirectory(PublisherGlobal.ROOTDIR_BUILDINGANIM);
				}

				File.Copy(configFile,PublisherGlobal.ROOTDIR_BUILDINGANIM+"/"+Path.GetFileName(configFile),true);
			}
			
		}

		private void SyncDataFromServer()
		{
			if (!Directory.Exists("resdatabase"))
			{
				Directory.CreateDirectory("resdatabase");
			}

			try
			{
				string url = "http://code.putao.io/ptgame/Model_PTBloks/raw/master/client_unity/resdatabase";
				
				var webClient = new WebClientPro();
				
				var fileNames = new [] { "resconfig_modelres_ios.json","resconfig_modelres_android.json","resconfig_thumbs_primitive.json"};

				foreach (var fileName in fileNames)
				{
					if (File.Exists("resdatabase/"+fileName))
					{
						File.Delete("resdatabase/"+fileName);
					}
					webClient.DownloadFile(url+"/"+fileName,"resdatabase/"+fileName);
					
				}
				
				webClient.Dispose();
				
				EditorUtility.DisplayDialog("同步数据", "同步数据成功", "确定");

			}
			catch (Exception e)
			{
				EditorUtility.DisplayDialog("同步数据", "同步数据失败", "确定");
			}

		}

		private class WebClientPro : WebClient
		{
			/// <summary>
			/// 过期时间
			/// </summary>
			private int Timeout { get; set; }

			public WebClientPro(int timeout = 10000)
			{   //默认10秒
				Timeout = timeout;
			}

			/// <summary>
			/// 重写GetWebRequest,添加WebRequest对象超时时间
			/// </summary>
			/// <param name="address"></param>
			/// <returns></returns>
			protected override WebRequest GetWebRequest(Uri address)
			{
				HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
				request.Timeout = Timeout;
				request.ReadWriteTimeout = Timeout;
				return request;
			}
		}
		
	}
}
#endif