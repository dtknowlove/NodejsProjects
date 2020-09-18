/****************************************************************************
 * 2017 maoling
 * 2017 ~ 2018.5 liqingyun
 ****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
	using UnityEngine;
	using System.IO;
#if UNITY_EDITOR
	using UnityEditor;

#endif
	[System.Serializable]
	public class FrameworkSettingData
	{
		public static string[] RES_LOADER_SUPPORT_TEXTS   = {"PTABManager", "PTResSystem"};
		public static string[] RES_LOADER_SUPPORT_SYMBOLS = {"PTABMANAGER_SUPPORT", "PTRESSYSTEM_SUPPORT"};
		public static string[] LUA_SUPPORT_TEXTS          = {"NoneLua", "uLua", "sLua", "xLua"};

		public static string[] LUA_SUPPORT_SYMBOLS =
			{"NONE_LUA_SUPPORT", "ULUA_SUPPORT", "SLUA_SUPPORT", "XLUA_SUPPORT"};

		public static string[] COCOS_SUPPORT_TEXTS   = {"NoneCocos", "Cocos"};
		public static string[] COCOS_SUPPORT_SYMBOLS = {"NONE_COCOS_SUPPORT", "COCOS_SUPPORT"};

		static string mConfigSavedDir      = Application.dataPath + "/PTGameData/ProjectConfig/";
		static string mConfigSavedFileName = "ProjectConfig.json";

		public string Namespace             = null;
		public int    ResLoaderSupportIndex = 0;
		public int    LuaSupportIndex       = 0;
		public int    CocosSupportIndex     = 0;

		public string ResLoaderSupportSymbol
		{
			get { return RES_LOADER_SUPPORT_SYMBOLS[ResLoaderSupportIndex]; }
		}

		public string LuaSupportSymbol
		{
			get { return LUA_SUPPORT_SYMBOLS[LuaSupportIndex]; }
		}

		public string CocosSupportSymbol
		{
			get { return COCOS_SUPPORT_SYMBOLS[CocosSupportIndex]; }
		}

		public string UIScriptDir = "/Scripts/UI";

		public string UIPrefabDir = "/ArtRes/AssetBundles/UIPrefab";

		public static FrameworkSettingData Load()
		{
			mConfigSavedDir.CreateDirIfNotExists();

			if (!File.Exists(mConfigSavedDir + mConfigSavedFileName))
			{
				using (var fileStream = File.Create(mConfigSavedDir + mConfigSavedFileName))
				{
					fileStream.Close();
				}
			}

			var frameworkConfigData = SerializeHelper.LoadJson<FrameworkSettingData>(mConfigSavedDir + mConfigSavedFileName);

			if (frameworkConfigData == null || string.IsNullOrEmpty(frameworkConfigData.Namespace))
			{
				frameworkConfigData = new FrameworkSettingData {Namespace = "Putao.ProjectName"};
			}

			return frameworkConfigData;
		}

		public void Save()
		{
			this.SaveJson(mConfigSavedDir + mConfigSavedFileName);
#if UNITY_EDITOR
			AssetDatabase.Refresh();
#endif
		}

		#region AssetBundle 相关

		public const string ABMANIFEST_ASSET_NAME = "assetbundlemanifest";

		public static string AB_RELATIVE_PATH
		{
			get { return "AssetBundles/" + PlatformUtil.GetPlatformName() + "/"; }
		}

		public static string AssetBundleUrl2Name(string url)
		{
			string retName = null;
			var parren = FilePath.StreamingAssetsPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/";
			retName = url.Replace(parren, "");

			parren = FilePath.PersistentDataPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/";
			retName = retName.Replace(parren, "");
			return retName;
		}

		public static string AssetBundleName2Url(string name)
		{
			var retUrl = FilePath.PersistentDataPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/" + name;
			if (File.Exists(retUrl))
			{
				return retUrl;
			}

			return FilePath.StreamingAssetsPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/" + name;
		}

		//导出目录

		/// <summary>
		/// AssetBundle存放路径
		/// </summary>
		public static string RELATIVE_AB_ROOT_FOLDER
		{
			get { return "/AssetBundles/" + PlatformUtil.GetPlatformName() + "/"; }
		}

		/// <summary>
		/// AssetBundle 配置路径
		/// </summary>
		public static string EXPORT_ASSETBUNDLE_CONFIG_FILENAME
		{
			get { return "asset_bindle_config.bin"; }
		}

		#endregion
	}
}