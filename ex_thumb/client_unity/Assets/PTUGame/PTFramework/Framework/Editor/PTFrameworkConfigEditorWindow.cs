/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

/// <summary>
/// Project config editor window.
/// </summary>
namespace PTGame.Framework.Editor
{
	using UnityEngine;
	using UnityEditor;
	
	public class PTFrameworkConfigEditorWindow : EditorWindow
	{
//		[MenuItem("PuTaoTool/Framework/FrameworkConfig")]
//		static void Open() 
//		{
//			var frameworkConfigEditorWindow = (PTFrameworkConfigEditorWindow)EditorWindow.GetWindow(typeof(PTFrameworkConfigEditorWindow),true);
//			frameworkConfigEditorWindow.titleContent = new  GUIContent("PTFrameworkConfig");
//			frameworkConfigEditorWindow.CurSettingData = FrameworkSettingData.Load ();
//			frameworkConfigEditorWindow.Show ();
//		}

		public PTFrameworkConfigEditorWindow() {}

		public FrameworkSettingData CurSettingData;
	
		void OnGUI() 
		{
//			CurSettingData.Namespace = EditorGUIUtils.GUILabelAndTextField ("Namespace", CurSettingData.Namespace);
//		
//			#if NONE_LUA_SUPPORT 
//			CurSettingData.UIScriptDir = EditorGUIUtils.GUILabelAndTextField ("UI Script Generate Dir", CurSettingData.UIScriptDir);
//			CurSettingData.UIPrefabDir = EditorGUIUtils.GUILabelAndTextField ("UI Prefab Dir", CurSettingData.UIPrefabDir);
//			#endif
//
//			CurSettingData.ResLoaderSupportIndex = EditorGUIUtils.GUILabelAndPopup("AB Support",
//				CurSettingData.ResLoaderSupportIndex, FrameworkSettingData.RES_LOADER_SUPPORT_TEXTS);
//			CurSettingData.LuaSupportIndex = EditorGUIUtils.GUILabelAndPopup("Lua Support", CurSettingData.LuaSupportIndex,
//				FrameworkSettingData.LUA_SUPPORT_TEXTS);
//			CurSettingData.CocosSupportIndex = EditorGUIUtils.GUILabelAndPopup("Cocos Support", CurSettingData.CocosSupportIndex,
//				FrameworkSettingData.COCOS_SUPPORT_TEXTS);
//			if (GUILayout.Button ("Apply")) 
//			{
//				CurSettingData.Save ();
//				MicroEditor.ApplyAllPlatform ();
//			}
		}
	}

	[InitializeOnLoad]
	public class MicroEditor
	{
		static MicroEditor()
		{
			ApplyAllPlatform ();
		}

		public static void ApplyAllPlatform()
		{
			var frameworkConfigData = FrameworkSettingData.Load ();

			ApplySymbol (frameworkConfigData, BuildTargetGroup.iOS);
			ApplySymbol (frameworkConfigData, BuildTargetGroup.Android);
			ApplySymbol (frameworkConfigData, BuildTargetGroup.Standalone);
		}


		public static void ApplySymbol(FrameworkSettingData frameworkSettingData,BuildTargetGroup targetGroup)
		{
			var 	symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (targetGroup);

			if (string.IsNullOrEmpty (symbols)) {
				symbols = frameworkSettingData.ResLoaderSupportSymbol;
			}
			else {
				var symbolSplit = symbols.Split (new char[]{ ';' });

				symbols = "";

				for (var i = 0; i < symbolSplit.Length; i++) 
				{
					var symbol = symbolSplit [i];
					if (string.Equals (symbol, FrameworkSettingData.RES_LOADER_SUPPORT_SYMBOLS [0]) ||
						string.Equals (symbol, FrameworkSettingData.RES_LOADER_SUPPORT_SYMBOLS [1]) ||
						string.Equals (symbol, FrameworkSettingData.LUA_SUPPORT_SYMBOLS [0]) ||
						string.Equals (symbol, FrameworkSettingData.LUA_SUPPORT_SYMBOLS [1]) ||
						string.Equals (symbol, FrameworkSettingData.LUA_SUPPORT_SYMBOLS [2]) ||
						string.Equals (symbol, FrameworkSettingData.LUA_SUPPORT_SYMBOLS [3]) ||
					    string.Equals (symbol, FrameworkSettingData.COCOS_SUPPORT_SYMBOLS[0]) ||
					    string.Equals (symbol, FrameworkSettingData.COCOS_SUPPORT_SYMBOLS[1]))
					{

					}
					else {
						symbols += symbol + ";";
					}
				}

				symbols += frameworkSettingData.ResLoaderSupportSymbol + ";";
				symbols += frameworkSettingData.LuaSupportSymbol + ";";
				symbols += frameworkSettingData.CocosSupportSymbol + ";";
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup (targetGroup, symbols);
		}
	}
}
