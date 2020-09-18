/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System;
using UnityEditor;
using UnityEngine;


namespace PTGame.Editor.PluginManager
{
	public class PTPluginReadmeWin :EditorWindow
	{
		private PluginReadme mReadme;

		private Vector2 mScrollPos =  Vector2.zero;

		private GUIStyle mTitleStyle;

		private PluginInfo serverPlugin;

		public static void Init(PluginInfo serverPlugin,PluginReadme readme)
		{
			PTPluginReadmeWin readmeWin = (PTPluginReadmeWin)EditorWindow.GetWindow (typeof(PTPluginReadmeWin), true,"PTPlugin Reame",true);
			readmeWin.mReadme = readme;
			readmeWin.serverPlugin = serverPlugin;
			readmeWin.position = new Rect (Screen.width / 2, Screen.height / 2, 560, 300);
			readmeWin.Show ();
		}

		void OnEnable()
		{
			mTitleStyle = new GUIStyle ();
			mTitleStyle.fontStyle = FontStyle.Bold;
			mTitleStyle.fontSize = 12;
			mTitleStyle.alignment = TextAnchor.LowerLeft;
			mTitleStyle.normal.textColor = Color.white;
		}

		public void OnGUI()
		{
			mScrollPos = GUILayout.BeginScrollView (mScrollPos, true,true ,GUILayout.Width(560), GUILayout.Height(300));

			for(int i= mReadme.items.Count-1;i>=0;i--)
			{
				ReadmeItem item = mReadme.items [i];
				GUILayout.BeginHorizontal (EditorStyles.helpBox);
				GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
				 
				GUILayout.Label ("version: "+item.version,mTitleStyle,GUILayout.Width(130));
				GUILayout.Label (""+item.date,mTitleStyle,GUILayout.Width(130));
				GUILayout.Label ("author: "+item.author);
				if (GUILayout.Button("download"))
				{
					string fileUrl = PTPluginConfigData.GetServerUrl() + "/" +
					                 string.Format("{0}/{1}_v{2}.unitypackage", serverPlugin.type, serverPlugin.name,
						                 item.version);
					Application.OpenURL(fileUrl);

				}

				GUILayout.EndHorizontal ();
				GUILayout.Label (item.content);
				GUILayout.EndVertical ();


				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();

		}
	}
}

