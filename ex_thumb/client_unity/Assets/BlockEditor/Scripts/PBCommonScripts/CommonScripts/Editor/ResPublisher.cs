using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using PTGame.Core;
using Putao.PaiBloks.Common;
using UnityEditor;
using UnityEngine;

namespace Block.Editor
{
	public static class ResPublisher
	{
		private const string NAME_MENUITEM_LEYUANAPP = "资源发布/Mini 乐园 /AppConfig";
		private const string NAME_MENUITEM_LEYUANAPPIMAGE = "资源发布/Mini 乐园 /AppImagesConfig";
		
		
		private const string DIR_BUILDANIM   = "config_paibloks_buildanim";
		private const string DIR_PLATFORM    = "config_paibloks_platform";
		private const string DIR_BLOCKBUILD  = "config_paibloks_blockbuild";
		private const string DIR_BLOCKDATA   = "config_blockdata";
		private const string DIR_THUMBCONFIG = "config_paibloks_buildanim_thumbs";
		private const string DIR_THEMEIMAGES = "config_paibloks_themebuild_images";
		private const string DIR_MINI_APP    = "config_paibloks_mini";
		private const string DIR_MINI_APPIMG = "config_paibloks_mini_images";
		private const string DIR_HASHTHUMBS  = "block_thumbs_hash";
		private const string FILENAME_RESCONFIG = "resconfig.json";
		private const string DIR_FTP = "ftpres";
		
		public static readonly string[] EXCLUDE_BUILDINGS =
		{
			"config_41005_carrousel_small_v1_0",
			"config_41001_ferris_wheel_v1_1",
			"config_41001_ferris_wheel_v1_1"
		};
		
		
		public static void CreateResDatabaseConfig()
		{
			var bytes = GetFileBytes(PublisherGlobal.PrimitiveThumbsConfig);
			var md5Tumb = GetMD5(bytes);
			bytes = GetFileBytes(PublisherGlobal.PrimitiveAndroidResConfig);
			var md5Android= GetMD5(bytes);
			bytes = GetFileBytes(PublisherGlobal.PrimitiveiOSResConfig);
			var md5iOS= GetMD5(bytes);

			string newVersion = "100";
			var md5 = Md5Encrypt(md5Tumb + md5Android + md5iOS);
			var resConfigPath = Path.Combine(DIR_FTP,PublisherGlobal.DIR_RESDATABASE)+"/"+FILENAME_RESCONFIG;
			if (File.Exists(resConfigPath))
			{
				var content = File.ReadAllText(resConfigPath);
				var oldConfig = JsonUtility.FromJson<ResConfig>(content);
				if (oldConfig.items[0].hash != md5)
				{
					newVersion = (int.Parse(oldConfig.resversion) + 1).ToString();
				}
				else
				{
					newVersion = oldConfig.resversion;
				}
			}

			var destDir = Path.GetDirectoryName(resConfigPath);
			
			if (!Directory.Exists(destDir))
			{
				Directory.CreateDirectory(destDir);
			}

			var zipFilePath = Path.Combine(destDir, "resdatabase.zip");
			FastZip fastZip = new FastZip();
			fastZip.CreateZip(zipFilePath,"resdatabase",false,".json");
			
			ResConfig newResConfig = new ResConfig();
			newResConfig.resversion = newVersion;
			newResConfig.items = new List<ResItem>();
			newResConfig.items.Add(new ResItem()
			{
				name = "resdatabase",
				path = "resdatabase.zip",
				hash = md5,
				size = GetFileBytes(zipFilePath).Length
			});
			var t = JsonUtility.ToJson(newResConfig, true);
			
			File.WriteAllText(resConfigPath,t);
		}

		/// <summary>
		/// 生成搭建动画的配置文件
		/// </summary>
		private static void GenerateBuildingAnimConifg()
		{
			string dir =   Path.Combine(DIR_FTP,DIR_BUILDANIM);
			GenerateResConfig(new [] {dir}, Path.Combine(dir,FILENAME_RESCONFIG), dir,null, ".txt");
			ShowFinishDialog(DIR_BUILDANIM);
		}

		/// <summary>
		/// 生成platform 所有配置表的 对应配置文件
		/// </summary>
		public static void PlatformConfig()
		{
			string dir = Path.Combine(DIR_FTP,DIR_PLATFORM);
			GenerateResConfig(new [] {dir}, Path.Combine(dir,FILENAME_RESCONFIG), dir,null, ".csv");
			ShowFinishDialog(DIR_PLATFORM);
		}
		
		/// <summary>
		/// 生成BlockBuild 对应的配置文件
		/// </summary>
		public static void BlockBuildConfig()
		{
			string dir = Path.Combine(DIR_FTP,DIR_BLOCKBUILD);
			GenerateResConfig(new[] {dir}, Path.Combine(dir, FILENAME_RESCONFIG), dir,null, ".csv", ".png");
			ShowFinishDialog(DIR_BLOCKBUILD);
		}

		/// <summary>
		/// 生成积木零件配置文件
		/// </summary>
		public static void BlockDataConfig()
		{
			var dir = Path.Combine(DIR_FTP,DIR_BLOCKDATA);	
			if (!Directory.Exists(dir))Directory.CreateDirectory(dir);
			DirectoryCopy(Path.Combine(Application.dataPath,"BlockData"),dir,".meta");
			GenerateResConfig(new [] {dir}, Path.Combine(dir,FILENAME_RESCONFIG), dir,null, ".json");
			ShowFinishDialog(DIR_BLOCKDATA);
		}

		public static void ThemeBuildImages()
		{
			DirectoryInfo parentDir = new DirectoryInfo(Path.Combine(DIR_FTP, DIR_THEMEIMAGES));
			DirectoryInfo[] dirInfos = parentDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
			foreach (DirectoryInfo dirInfo in dirInfos)
			{
				string dir = Path.Combine(DIR_FTP, DIR_THEMEIMAGES + "/" + dirInfo.Name);
				GenerateResConfig(new[] {dir}, Path.Combine(dir, FILENAME_RESCONFIG), dir,null, ".png");
			}
			ShowFinishDialog(DIR_THEMEIMAGES);
		}
		
		#region 缩略图相关

		public static void CreateBuildingThumbConfigs(string[] files, bool isOnline = true)
		{
			var finalDir = DIR_THUMBCONFIG + (isOnline ? string.Empty : "_test");
			string outputDir = Path.Combine(DIR_FTP, finalDir);

			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}

			var thumbInfoContent = File.ReadAllText(PublisherGlobal.PrimitiveThumbsConfig);
			var thumbInfoItems = JsonUtility.FromJson<ResConfig>(thumbInfoContent).items;

			int counter = 0;
			int totalCount = files.Length;
			mThumbErrors.Clear();
			foreach (var file in files)
			{
				CreateBuildingThumbConfigFile(file, thumbInfoItems, outputDir, isOnline);

				bool result = EditorUtility.DisplayCancelableProgressBar("生成缩略图资源配置表"+finalDir, string.Format("生成中....{0}/{1}", counter, totalCount), counter * 1.0f / totalCount);
				if (result)
				{
					break;
				}
				counter++;
			}
			EditorUtility.ClearProgressBar();
			if (mThumbErrors.Count > 0)
				EditorUtility.DisplayDialog("提示", "这些搭建的丝印有问题:" + mThumbErrors.Join("\n"), "好");
			ShowFinishDialog(finalDir);
		}

		private static List<string> mThumbErrors = new List<string>();
		private static bool CreateBuildingThumbConfigFile(string filePath, List<ResItem> thumbInfoItems, string outputDir,bool isOnline)
		{

			var fileName = Path.GetFileNameWithoutExtension(filePath);
			
			if (EXCLUDE_BUILDINGS.Contains(fileName))return false;
			
#if BLOCK_EDITOR
			PPBlockConfigInfo configInfo = PBBlockConfigManager.LoadBlockInfosWithoutAnim(filePath);
#elif BLOCK_MODEL
			var path = "./" + BlockServerUtil.GetBuildAnimPath(isOnline) + "/" + fileName + ".txt";
			var configText = File.Exists(path) ? File.ReadAllText(path, Encoding.UTF8) : string.Empty;
			PPBlockConfigInfo configInfo = PBBlockConfigManager.LoadBlockInfosWithoutAnim(fileName + ".txt", configText);
			if (configInfo == null)
			{
				Debug.LogError(path + ">>>" + (File.Exists(path) ? "搭建文件内容有问题!" : "搭建文件不存在!"));
				return false;
			}
#endif
			var buildingThumbs = new ResConfig {resversion = "100", items = new List<ResItem>()};

			foreach (var item in configInfo.BlockInfos)
			{
				var equal = item.BlockInfo.IsStamp ? item.BlockInfo.GetStampThumb : item.ThumbName;
				if (thumbInfoItems.Any(s => s.name.Equals(equal)))
				{
					var t = thumbInfoItems.FirstOrDefault(s => s.name.Equals(equal));
					buildingThumbs.items.Add(t);
				}
				else
				{
					if (item.ThumbName != "diban")
					{
						if (!mThumbErrors.Contains(fileName))
							mThumbErrors.Add(fileName);
						Debug.LogError(item.ThumbName + ">>>>*找不到缩率图******" + equal + ".png");
					}
				}
			}
			
			var config = JsonUtility.ToJson(buildingThumbs, true);
			File.WriteAllText(Path.Combine(outputDir, fileName), config);
			return true;
		}

		/// <summary>
		/// 生成 缩略图 md5 配置文件
		/// </summary>
		public static void CreateThumbPrimitiveConfig()
		{
			string[] dirs =
			{
				"block_thumbs/LowPolygon/Category_1/Block_Thumbs",
				"block_thumbs/LowPolygon/Category_fig/Block_Thumbs",
				"block_thumbs/LowPolygon/Category_pbl/Block_Thumbs",
				"block_thumbs/LowPolygon/Category_pbs/Block_Thumbs",
				"block_thumbs/LowPolygon/Category_tech/Block_Thumbs",
				"block_thumbs/Stickers/Sticker_Textures"
			};
			
			var refPath ="block_thumbs";
			GenerateResConfig(dirs, PublisherGlobal.PrimitiveThumbsConfig, refPath,OnPostProcessItem, ".png", ".jpg");
		}

		private static void OnPostProcessItem(ResItem item)
		{
			if (item.name!=item.name.ToLower())
			{
				Debug.LogError("严重错误：缩略图名称存在大写>>>>"+item.path);
			}
		}

		/// <summary>
		/// 生成缩略图的 以其 hash 值命名的 对应文件
		/// </summary>
		private static void CreateHashThumbs()
		{
			string outputDir = Path.Combine(DIR_FTP,DIR_HASHTHUMBS);

			ReCreateDirectory(outputDir);

			ResConfig resConfig = JsonUtility.FromJson<ResConfig>(File.ReadAllText(PublisherGlobal.PrimitiveThumbsConfig));

			int counter = 0;
			foreach (ResItem item in resConfig.items)
			{
				counter++;
				EditorUtility.DisplayProgressBar("生成缩率图hash文件",string.Format("生成中....{0}/{1}",counter,resConfig.items.Count),counter*1.0f/resConfig.items.Count);
				
				string directoryName = Path.GetDirectoryName(outputDir + "/" + item.path);

				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				string destFilePath = Path.Combine(directoryName, item.hash);
				File.Copy("block_thumbs/"+item.path, destFilePath, true);
			}
			EditorUtility.ClearProgressBar();
		}
		
		#endregion

		#region Mini乐园相关
		
		/// <summary>
		/// 生成mini乐园app所用的配置表 配置文件
		/// </summary>
		#if BLOCK_MODEL
		[MenuItem(NAME_MENUITEM_LEYUANAPP, false, PublisherGlobal.ItemOrder.MINI)]
		#endif
		private static void GenerateMiniAppConfig()
		{
			string dir = Path.Combine(DIR_FTP,DIR_MINI_APP);
			GenerateResConfig(new[] {dir}, Path.Combine(dir,FILENAME_RESCONFIG), dir,null, ".csv");
			ShowFinishDialog(DIR_MINI_APP);
		}

		/// <summary>
		/// 生成mini乐园app所用的图标配置文件
		/// </summary>
		#if BLOCK_MODEL
		[MenuItem(NAME_MENUITEM_LEYUANAPPIMAGE, false, PublisherGlobal.ItemOrder.MINI)]
		#endif
		private static void GenerateMiniAppImagesConfig()
		{
			string dir = Path.Combine(DIR_FTP, DIR_MINI_APPIMG);
			GenerateResConfig(new[] {dir}, Path.Combine(dir,FILENAME_RESCONFIG), dir, null,".png");
			ShowFinishDialog(DIR_MINI_APPIMG);
		}
		
		#endregion

		/// <summary>
		/// 生成指定资源目录内 所有资源的  resconfig.json 配置文件
		/// </summary>
		/// <param name="resDirs">需要生成的资源所在目录</param>
		/// <param name="configFilePath">生成的配置文件路径</param>
		/// <param name="resRefPath">配置文件中 资源下载 相对路径</param>
		/// <param name="filters">需要生成配置项的资源类型</param>
		public static void GenerateResConfig(string[] resDirs, string configFilePath, string resRefPath,Action<ResItem> OnPostProcess,
			params string[] filters)
		{
			resRefPath = Path.GetFullPath(resRefPath);
			if (!resRefPath.EndsWith("/")) resRefPath += "/";
			
			string resVersion = "100";

			if (File.Exists(configFilePath))
			{
				ResConfig oldResConfig = JsonUtility.FromJson<ResConfig>(File.ReadAllText(configFilePath));
				resVersion = (int.Parse(oldResConfig.resversion) + 1).ToString();
			}

			ResConfig resConfig = new ResConfig {items = new List<ResItem>(), resversion = resVersion};

			foreach (string dir in resDirs)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(dir);

				FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories)
					.Where(s => !s.Name.EndsWith(".DS_Store") &&
					            (filters == null || filters.Contains(Path.GetExtension(s.FullName)))).ToArray();
				int total = fileInfos.Length;
				int counter = 0;
				foreach (var fileInfo in fileInfos)
				{
					string fullName = fileInfo.FullName;
					string refPath = fullName.Replace(resRefPath, "");
					ResItem item = CreateConfigItem(fullName, Path.GetFileNameWithoutExtension(fullName), refPath);
					if (OnPostProcess != null)
					{
						OnPostProcess(item);
					}
					resConfig.items.Add(item);
					
					counter++;
					EditorUtility.DisplayProgressBar("generating resconfig","generating"+counter+"/"+total,counter*1.0f/total);
				}
			}

			string configDir = Path.GetDirectoryName(configFilePath);
			if (!Directory.Exists(configDir)){
				Directory.CreateDirectory(configDir);
			}
			string contents = JsonUtility.ToJson(resConfig, true);
			File.WriteAllText(configFilePath, contents);
			AssetDatabase.Refresh();
			
			EditorUtility.ClearProgressBar();
		}

		private static ResItem CreateConfigItem(string realPath, string fileName, string refPath)
		{
			ResItem resItem = new ResItem {name = fileName, path = refPath};
			byte[] fileBytes = GetFileBytes(realPath);
			resItem.hash = GetMD5(fileBytes);
			resItem.size = fileBytes.Length;
			return resItem;
		}
		
		private static void ShowFinishDialog(string content)
		{
			EditorUtility.DisplayDialog(string.Format("生成{0}文件",content), "完成", "确定");
		}

		private static byte[] GetFileBytes(string filePath)
		{
			FileStream fileStream = new FileStream(filePath, FileMode.Open);
			int num = (int) fileStream.Length;
			byte[] array = new byte[num];
			fileStream.Read(array, 0, num);
			fileStream.Close();
			return array;
		}

		private static string GetMD5(byte[] data)
		{
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] array = mD.ComputeHash(data);
			string text = "";
			byte[] array2 = array;
			foreach (byte value in array2)
			{
				text += Convert.ToString(value, 16);
			}

			return text;
		}

		public static void ReCreateDirectory(string dir)
		{
			if (Directory.Exists(dir))
			{
				Directory.Delete(dir, true);
			}

			Directory.CreateDirectory(dir);
		}
		
	
		private static void DirectoryCopy(string sourceDirectory, string targetDirectory,params string[] excludes)
		{
			DirectoryInfo sourceInfo = new DirectoryInfo(sourceDirectory);
			FileInfo[] fileInfo = sourceInfo.GetFiles();
			foreach (FileInfo fiTemp in fileInfo)
			{
				if (excludes != null && excludes.Contains(Path.GetExtension(fiTemp.Name)))
				{
					continue;
				}
				File.Copy(sourceDirectory + "/" + fiTemp.Name, targetDirectory + "/" + fiTemp.Name, true);
			}

			DirectoryInfo[] diInfo = sourceInfo.GetDirectories();
			foreach (DirectoryInfo diTemp in diInfo)
			{
				string sourcePath = diTemp.FullName;
				string targetPath = diTemp.FullName.Replace(sourceDirectory, targetDirectory);
				Directory.CreateDirectory(targetPath);
				DirectoryCopy(sourcePath, targetPath);
			}
		}

		private static String Md5Encrypt(String strSource)
		{
			byte[] result = Encoding.Default.GetBytes(strSource);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] output = md5.ComputeHash(result);
			return BitConverter.ToString(output).Replace("-", "");

		}

	}
	
	
	public class ResConfig
	{
		public string resversion = "";

		public List<ResItem> items;
	}

	[Serializable]
	public class ResItem
	{
		public string name = "";

		public string path = "";

		public string hash = "";

		public float size = 0f;
	}
}


