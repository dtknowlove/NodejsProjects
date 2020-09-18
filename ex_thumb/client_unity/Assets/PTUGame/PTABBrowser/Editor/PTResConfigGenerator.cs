using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using AssetBundleBrowser;
using PTGame.ABBrowser;
using PTGame.Core;
//using PTGame.Core;
using PTGame.Editor.PluginManager;
using UnityEditor;
using UnityEngine;

namespace PTGame.ABBrowser
{

	[System.Serializable]
	public class AssetBundleInfo
	{
		public string name = "";

		public string[] assets;
	}
	
	[System.Serializable]
	public class AssetConfig
	{

		public List<AssetBundleInfo> abInfos;
	}

	public static class PTResConfigGenerator
	{
		
		private const string PATH_CONFIGFILE = "Assets/PTUGame/customconfig/abfiles.json";

		private static List<AssetBundleInfo> assetBundleInfos = new List<AssetBundleInfo>();

		private static ResItem CreateConfigItem(string realPath, string fileName, string refPath)
		{
			ResItem resItem = new ResItem();
			resItem.name = fileName;
			resItem.path = refPath;
			byte[] platformFileBytes = getFileBytes(realPath);
			resItem.hash = getMD5(platformFileBytes);
			resItem.size = platformFileBytes.Length;

			return resItem;
		}

		private static void CopyResFiles(string outputPath)
		{
			if (!File.Exists(PATH_CONFIGFILE))
			{
			
				return;
			}
			string content = File.ReadAllText(PATH_CONFIGFILE);
		
			ResFiles resFiles = JsonUtility.FromJson<ResFiles>(content);

			foreach (var item in resFiles.items)
			{
				File.Copy(item.assetPath,outputPath+Path.DirectorySeparatorChar+Path.GetFileName(item.name),true);
			}
			
		}

		public static void GenerateResConfig(string outputPath, string resVersion, bool generateClass,string classPath)
		{
		
			List<AssetBundleInfo> assetBundleInfos = new List<AssetBundleInfo>();

			string[] abNames = AssetDatabase.GetAllAssetBundleNames();
			
			foreach (var abName in abNames)
			{
				string[] astNames =  AssetDatabase.GetAssetPathsFromAssetBundle(abName);

				AssetBundleInfo abInfo = new AssetBundleInfo();
				abInfo.name = abName;
				abInfo.assets = astNames;
					
				assetBundleInfos.Add(abInfo);
			}

		
			string assetConfig = JsonUtility.ToJson(new AssetConfig(){abInfos = assetBundleInfos},true);
			
			File.WriteAllText(outputPath + "/assetconfig.json", assetConfig);


			CopyResFiles(outputPath);
			
			AssetDatabase.Refresh();
			
			
			
			
			ABConfig abConfig = new ABConfig
			{
				items = new List<ResItem>(),
				resversion = resVersion
			};
			
			//避免把resconfig 本身也统计进去
			if (File.Exists(outputPath+"/resconfig.json"))
			{
				File.Delete(outputPath+"/resconfig.json");

			}


			//此处是在Asset目录之外 的output目录遍历的，所以没有.meta文件
			string[] files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories)
				.Where(s => s.GetFileExtendName() != ".manifest" && s.GetFileExtendName()!= ".DS_Store").ToArray();

			foreach (var file in files)
			{
				string realPath = Path.GetFullPath(file);
				string refPath = file.Replace(outputPath + Path.DirectorySeparatorChar, "");
				ResItem resItem = CreateConfigItem(realPath, Path.GetFileNameWithoutExtension(file), refPath);
				abConfig.items.Add(resItem);
			}

			string content = JsonUtility.ToJson(abConfig, true);

			File.WriteAllText(outputPath + "/resconfig.json", content);
			

			if (generateClass)
			{
				if (!classPath.EndsWith(".cs"))
				{
					classPath += ".cs";
				}

				string dir = Path.GetDirectoryName(classPath);
				
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

			
				var path = Path.GetFullPath(classPath);
				StreamWriter writer = new StreamWriter(File.Open(path, FileMode.Create));
				PTBundleInfoGenerator.WriteClass(writer, "PTGame.AssetBundle", assetBundleInfos);
				writer.Close();
				AssetDatabase.Refresh();
			}
		}
		
		private static string GetFileExtendName(this string absOrAssetsPath)
		{
			int lastIndex = absOrAssetsPath.LastIndexOf(".");

			if (lastIndex >= 0)
			{
				return absOrAssetsPath.Substring(lastIndex);
			}

			return string.Empty;
		}

		private static byte[] getFileBytes(string filePath)
		{
			FileStream fs = new FileStream(filePath, FileMode.Open);
			int len = (int) fs.Length;
			byte[] data = new byte[len];
			fs.Read(data, 0, len);
			fs.Close();
			return data;
		}

		private static string getMD5(byte[] data)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(data);
			string fileMD5 = "";
			foreach (byte b in result)
			{
				fileMD5 += Convert.ToString(b, 16);
			}

			return fileMD5;
		}

		private class ABConfig
		{
			public string resversion = "";
			public List<ResItem> items;
		}

		[System.Serializable]
		private class ResItem
		{
			public string name = "";
			public string path = "";
			public string hash = "";
			public float size = 0;
		}
	}
}
