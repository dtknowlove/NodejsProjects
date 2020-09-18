/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Threading;
using System;

namespace PTGame.Editor.PluginManager
{
	public class PTPluginGenerator : EditorWindow {

		private const byte STATE_GENERATE_INIT = 0;
		private const byte STATE_GENERATE_PACKING = 1;
		private const byte STATE_GENERATE_UPLOADING = 2;
		private const byte STATE_GENERATE_COMPLETE = 3;

		private byte mGenerateState = STATE_GENERATE_INIT;

		private string mUploadResult = "";

		private string mPluginDir;

		private PluginInfo mPluginInfo;
		private PluginInfo mRemotePluginInfo;
		
		

		private bool mHasRemotePlugin = false;

		private string mLocalVersionText = "---";

		private string mReadmeText = "";

		private string mVersionText = "";

		private string mUploadUrl ="";

		private int mPluginTypeIndex = 0;

		private string[] mPluginTypes;

		private string mPackagePath;

		[MenuItem("Assets/PutaoTool/Publish PTPlugin")]
		public static void ExportPTPlugin()
		{
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				EditorUtility.DisplayDialog("PTPluginManager", "请连接网络", "确定");
				return;
			}
			
			UnityEngine.Object[] selectObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

			if (selectObject == null || selectObject.Length > 1) {
			
				return;
			}

			if (!EditorUtility.IsPersistent (selectObject[0])) {

				return;
			}

			string path = AssetDatabase.GetAssetPath (selectObject[0]);

			if(!Directory.Exists(path)){
				
				return;
			}
			
			PTPluginGenerator m_instance = (PTPluginGenerator)EditorWindow.GetWindow (typeof(PTPluginGenerator), true);

			m_instance.position = new Rect (Screen.width / 2, Screen.height / 2, 300, 500);

			m_instance.Show ();

		}
			

		void OnEnable()
		{
			UnityEngine.Object[] selectObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

			if (selectObject == null || selectObject.Length > 1) {

				return;
			}

			mPluginDir = AssetDatabase.GetAssetPath (selectObject[0]);
			
			
			string dirName = Path.GetFileName (mPluginDir).ToLower();

			PTPluginConfigData resConfig = PTPluginConfigData.GetPluginConfig ();
			
			

			PTPluginUtil.PTPluginInfos remoteInfos= PTPluginUtil.GetPluginWithName(resConfig.serverUrl,dirName);

			PluginInfo localPluginInfo = GetLocalPluginInfo();
			
			if (remoteInfos!=null&&remoteInfos.plugins.Count>0)
			{
				foreach (var item in remoteInfos.plugins)
				{
					if (localPluginInfo!=null && item.type == localPluginInfo.type)
					{
						mRemotePluginInfo = item;
						
						break;
					}
				}

				if (mRemotePluginInfo == null)
				{
					mRemotePluginInfo = remoteInfos.plugins[0];
				}
			}
			

//			mRemotePluginInfo = PTPluginUtil.GetPluginWithName(resConfig.serverUrl,dirName);


			mPluginTypes =  PTPluginUtil.GetServerAllTypeNames(resConfig.serverUrl);

			if (mRemotePluginInfo != null)
			{
				mHasRemotePlugin = true;

				mPluginInfo = mRemotePluginInfo;
				
				mVersionText = mPluginInfo.version;
				
				mReadmeText = mPluginInfo.readme.GetItem(mPluginInfo.version).content;

				mPluginTypeIndex = Array.IndexOf(mPluginTypes,mPluginInfo.type);

		

				if (localPluginInfo != null)
				{
					mLocalVersionText = localPluginInfo.version;

					Version v1 = new Version(localPluginInfo.version);
				
					Version v2 = new Version(mPluginInfo.version);
				
					if (v1.CompareTo(v2) != 0)
					{
						EditorUtility.DisplayDialog("warning", "本地版本跟服务器版本不一致,请确认", "确定");
					}
					
				}

			}
			else
			{
				mHasRemotePlugin = false;
				mPluginInfo = new PluginInfo ();
				
			}

			mPluginInfo.name = Path.GetFileName (mPluginDir).ToLower();
			
			EditorApplication.update += Update;
			
		}

		private PluginInfo GetLocalPluginInfo()
		{
			
			string[] files = System.IO.Directory.GetFiles (mPluginDir, "ptplugin.txt", System.IO.SearchOption.TopDirectoryOnly);

			if (files.Length > 0) {

				string json = File.ReadAllText (Path.GetFullPath(files[0]));

				PluginInfo localPluginInfo = JsonUtility.FromJson<PluginInfo> (json);

				return localPluginInfo;
			}

			return null;
		}

		void OnDisable()
		{
			EditorApplication.update -= Update;
		}



		void Update()
		{
			switch(mGenerateState)
			{

			case STATE_GENERATE_PACKING:
				
				mPackagePath = mPluginInfo.name + "_v" + mPluginInfo.version + ".unitypackage";
				AssetDatabase.ExportPackage(mPluginDir,mPackagePath,ExportPackageOptions.Recurse);
				GotoUploading ();

				break;
			case STATE_GENERATE_COMPLETE:
				
				if (EditorUtility.DisplayDialog ("上传结果", mUploadResult, "OK"))
				{
					if(File.Exists(mPackagePath))
					{
						File.Delete (mPackagePath);
					}
					AssetDatabase.Refresh ();
					mGenerateState = STATE_GENERATE_INIT;
					Close ();
				}

				break;

			}

		}


		private bool IsVersionValide(string version)
		{
			if(version ==null){
				return false;
			}
			string[] t = version.Split ('.');
			if (t.Length != 3) {
			
				return false;
			}
		
			return true;
		}

		private  void GotoComplete()
		{

			mGenerateState = STATE_GENERATE_COMPLETE;
		
		}


		private void GotoPacking()
		{

			mGenerateState = STATE_GENERATE_PACKING;
			noticeMessage = "插件导出中,请稍后...";
		
		}

		private void GotoUploading()
		{
			noticeMessage = "插件上传中,请稍后...";
			mUploadResult = null;
			mGenerateState = STATE_GENERATE_UPLOADING;
			Thread t = new Thread (UploadPackage);
			t.Start ();

		}

		private string noticeMessage = "";

		private void DrawNotice()
		{
			EditorGUI.LabelField (new Rect(100,200,200,200),noticeMessage,EditorStyles.boldLabel);
		}

	

		private void DrawInit()
		{
			if (mHasRemotePlugin) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("本地版本号", GUILayout.Width (70));
				GUILayout.Label (mLocalVersionText, GUILayout.Width (100));

				GUIStyle newVersionStyle = new GUIStyle ();
				newVersionStyle.alignment = TextAnchor.LowerLeft;
				newVersionStyle.normal.textColor = new Color(1,1,0);
				GUILayout.Label ("远端版本号",newVersionStyle,GUILayout.Width(70));
				GUILayout.Label (mPluginInfo.version, newVersionStyle,GUILayout.Width (100));
				
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("插件类型", GUILayout.Width (70));
				mPluginTypeIndex = EditorGUILayout.Popup(mPluginTypeIndex, mPluginTypes,GUILayout.Width(100)); 
				GUILayout.EndHorizontal ();

			} else {
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("插件类型", GUILayout.Width (70));
				mPluginTypeIndex = EditorGUILayout.Popup(mPluginTypeIndex, mPluginTypes,GUILayout.Width(100));
				GUILayout.EndHorizontal ();
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("发布版本号",GUILayout.Width(70));
			mVersionText = GUILayout.TextField (mVersionText,GUILayout.Width(100));
			GUILayout.EndHorizontal ();
			GUILayout.Label ("发布说明:",GUILayout.Width(150));
			mReadmeText = GUILayout.TextArea (mReadmeText,GUILayout.Width(250),GUILayout.Height(300));

			if(GUILayout.Button ("发布",GUILayout.Width(150)))
			{
				if(mReadmeText.Length<2){
					ShowErrorMsg ("请输入版本修改说明");
					return;
				}
				if(!IsVersionValide(mVersionText)){
					ShowErrorMsg ("请输入正确的版本号");
					return;
				}

				mPluginInfo.version = mVersionText;
				mPluginInfo.readme.AddReadme(new ReadmeItem(mVersionText,mReadmeText,SystemInfo.deviceName,DateTime.Now.ToString("g")));


				
				mPluginInfo.type = mPluginTypes[mPluginTypeIndex];
				mPluginInfo.url = mPluginDir;
				

				mUploadUrl = PTPluginConfigData.GetServerUrl ();

				if(mUploadUrl == null){
					ShowErrorMsg (string.Format("请到 {0} 确认 类型 {0} 已配置" ,"Assets/PTUGame/ptpluginconfig",mPluginInfo.type));
					return;
				}

				string t = JsonUtility.ToJson (mPluginInfo,true);

				File.WriteAllText (mPluginDir+"/ptplugin.txt",t);

				AssetDatabase.Refresh ();

				GotoPacking ();

			}
		}

		public void OnGUI()
		{

			switch(mGenerateState)
			{
				case STATE_GENERATE_INIT:
					DrawInit ();
					break;
				default:
					DrawNotice ();
					break;
			}

		}

		private void UploadPackage()
		{
			string filePath = mPluginInfo.name + "_v" + mPluginInfo.version + ".unitypackage";

			string readmeJson = JsonUtility.ToJson(mPluginInfo.readme,true);

			mUploadResult = PTPluginUploader.UploadPlugin (mUploadUrl+"/post",mPluginInfo.name,mPluginInfo.type,mPluginInfo.version,readmeJson,filePath);
		
			GotoComplete ();
		}
			

		private void ShowErrorMsg(string content)
		{
			EditorUtility.DisplayDialog ("error",content,"OK");
		}
	}




}
