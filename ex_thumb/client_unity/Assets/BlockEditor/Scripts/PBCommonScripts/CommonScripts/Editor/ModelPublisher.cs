using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Putao.PaiBloks.Common;
using UnityEditor;
using UnityEngine;

namespace Block.Editor
{
	public static class ModelPublisher
	{
		private static string GetPrimitiveAbInfoConfig(string dir)
		{
			return Path.Combine(PublisherGlobal.DIR_RESDATABASE, string.Format("resconfig_{0}.json", dir));
		}

		private static string GetBuildingModelConfigDir(string dir)
		{
			return Path.Combine(PublisherGlobal.DIR_FTPRES,"config_modelres_buildanim" + dir);
		}

		private static string GetSkuModelConfigDir(string dir)
		{
			 return Path.Combine(PublisherGlobal.DIR_FTPRES, "config_modelres_sku" + dir);
		}

		#region Database

		public static void CreateModelResDatabaseiOS()
		{
//			GenerateModeHashFiles(PublisherGlobal.ROOTDIR_IOS);
		}

		public static void CreateModelResDatabaseAndroid()
		{
//			GenerateModeHashFiles(PublisherGlobal.ROOTDIR_ANDROID);
		}

		private static bool GenerateModeHashFiles(string rootPath)
		{
			var resDir = Path.Combine(rootPath, "blockres");
			if (!Directory.Exists(resDir)){
				EditorUtility.DisplayDialog("create model res config", "please build asset bundle first", "");
				return false;
			}

			var configPath = GetPrimitiveAbInfoConfig(rootPath);
			var refPath = Path.Combine(Environment.CurrentDirectory,rootPath)+"/";
			ResPublisher.GenerateResConfig(new[] {resDir},configPath,refPath,null,"");
			return true;
		}
		
		#endregion


		private static bool mCurCreateAndroid = true;
		public static void CreateCustomConfig(string[] files, bool isOnline = true)
		{
			var outpathAndorid = GetBuildingModelConfigDir((isOnline ? string.Empty : "_test"));
//			var abInfoPathAndorid = GetPrimitiveAbInfoConfig(PublisherGlobal.ROOTDIR_ANDROID);
			mCurCreateAndroid = true;
			CreateConfigFiles(files, outpathAndorid,isOnline);
//			var outpathiOS = GetBuildingModelConfigDir(PublisherGlobal.ROOTDIR_IOS + (isOnline ? string.Empty : "_test"));
//			var abInfoPathiOS = GetPrimitiveAbInfoConfig(PublisherGlobal.ROOTDIR_IOS);
//			mCurCreateAndroid = false;
//			CreateConfigFiles(files, outpathiOS, abInfoPathiOS, isOnline);
		}

		private static void CreateConfigFiles(string[] fileNames,string outPath,bool isOnline)
		{
			string outputDir = outPath;

			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}
			int counter = 0;
			int totalCount = fileNames.Length;
//			var modeAbInfoItems = JsonUtility.FromJson<ResConfig>(File.ReadAllText(abinfoPath)).items;

			var modelAbInfoItems = JsonUtility.FromJson<ResConfig>(File.ReadAllText("database_models_gltf.txt")).items;
			var textureInfoItems = JsonUtility.FromJson<ResConfig>(File.ReadAllText("database_models_texture.txt")).items;
			var materialInfoItems = JsonUtility.FromJson<PEMaterialUtl.MaterialInfos>(File.ReadAllText("Assets/BlockData/database_models_material.txt")).materialInfos;
			
			var outputDirName = outPath.Substring(outPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
			foreach (var fileName in fileNames)
			{
				CreateBuildingModeResFile(fileName, modelAbInfoItems,textureInfoItems, materialInfoItems,outputDir, isOnline);
				bool result = EditorUtility.DisplayCancelableProgressBar("生成模型资源配置表"+outputDirName,
					string.Format("{0}生成中....{1}/{2}",Path.GetFileNameWithoutExtension(fileName),counter,totalCount),counter*1.0f/totalCount);
				if (result)
				{
					break;
				}
				counter++;
			}
			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("create block model res config files", "finished->"+outputDirName, "OK");
		}


		private static bool AddTextureResItem(string name,string path, List<ResItem> items,List<ResItem> textureInfoItems)
		{
			path = path.Replace("\\","/");
			var localPath = path.Substring(7).ToLower();
			
			var abInfoItem = textureInfoItems.FirstOrDefault(s => s.path == localPath+".png");
			
			
			if (abInfoItem!=null)
			{
//				var rootdir = mCurCreateAndroid ? PublisherGlobal.ROOTDIR_ANDROID : PublisherGlobal.ROOTDIR_IOS;
				var resItem = new ResItem {name = abInfoItem.name};
				resItem.hash = abInfoItem.hash;
				resItem.size = abInfoItem.size;
//				resItem.path = Path.GetDirectoryName(rootdir + "/" + localPath) + "/" + resItem.name;
				resItem.path = abInfoItem.path;
				if (items.All(s => s.path != resItem.path))
				{
					items.Add(resItem);
				}
				return true;
			}

			Debug.LogError("abinfo 里面 找不到>>>>>" + path);
//			EditorUtility.DisplayDialog("错误","abinfo 里面 找不到>>>>>" + path,"确定");
			return false;
		}


		private static bool AddMatResItem(string name, string path, List<ResItem> items, List<ResItem> modelAbInfoItems)
		{
			path = path.Replace("\\","/");
			var localPath = path.Substring(7).ToLower();
			return false;
		}

		private static bool AddResItem( string name,string path, List<ResItem> items,List<ResItem> modelAbInfoItems)
		{
			
//			Debug.LogError(name+"--ggggg>>>"+path);
			path = path.Replace("\\","/");
			
			var localPath = path.Substring(7).ToLower();
			var abInfoItems = modelAbInfoItems.Where(s => Path.GetDirectoryName(s.path) == localPath).ToArray();
		
	
			if (abInfoItems != null && abInfoItems.Length == 2)
			{
				foreach (var abInfoItem in abInfoItems)
				{
//					var rootdir = mCurCreateAndroid ? PublisherGlobal.ROOTDIR_ANDROID : PublisherGlobal.ROOTDIR_IOS;
					var resItem = new ResItem {name = abInfoItem.name};
					resItem.hash = abInfoItem.hash;
					resItem.size = abInfoItem.size;
					resItem.path =   abInfoItem.path;
					if (items.All(s => s.path != resItem.path))
					{
						items.Add(resItem);
					}
				}

				return true;
			}

			
//			if (abInfoItem!=null)
//			{
//				var rootdir = mCurCreateAndroid ? PublisherGlobal.ROOTDIR_ANDROID : PublisherGlobal.ROOTDIR_IOS;
//				resItem.hash = abInfoItem.hash;
//				resItem.size = abInfoItem.size;
//				resItem.path = Path.GetDirectoryName(rootdir + "/" + localPath) + "/" + resItem.name;
//				if (items.All(s => s.path != resItem.path))
//				{
//					items.Add(resItem);
//				}
//				return true;
//			}

			Debug.LogError("abinfo 里面 找不到>>>>>" + path);
//			EditorUtility.DisplayDialog("错误","abinfo 里面 找不到>>>>>" + path,"确定");
			return false;
		}

//		private static bool AddResItem( string name,string path, List<ResItem> items,List<ResItem> modelAbInfoItems)
//		{
//			
//			Debug.LogError(name+"--ggggg>>>"+path);
//			path = path.Replace("\\","/");
//			var resItem = new ResItem {name = name};
//			var localPath = path.Substring(7).ToLower();
//			var abInfoItem = modelAbInfoItems.FirstOrDefault(s => s.path == localPath);
//			
//			if (abInfoItem!=null)
//			{
//				var rootdir = mCurCreateAndroid ? PublisherGlobal.ROOTDIR_ANDROID : PublisherGlobal.ROOTDIR_IOS;
//				resItem.hash = abInfoItem.hash;
//				resItem.size = abInfoItem.size;
//				resItem.path = Path.GetDirectoryName(rootdir + "/" + localPath) + "/" + resItem.name;
//				if (items.All(s => s.path != resItem.path))
//				{
//					items.Add(resItem);
//				}
//				return true;
//			}
//
//			Debug.LogError("abinfo 里面 找不到>>>>>" + path);
//			EditorUtility.DisplayDialog("错误","abinfo 里面 找不到>>>>>" + path,"确定");
//			return false;
//		}

		private static void CreateStickerItem(string prefabName, List<ResItem> items,List<ResItem> modelAbInfoItems,List<ResItem> textureInfoItems)
		{
			var stickerData = PBDataBaseManager.Instance.GetDataWithPrefabName(prefabName) as StickerData;
			
			string abFilePath = Path.Combine(BlockPath.Sticker_Fbx_Dir, stickerData.model);
			AddResItem(stickerData.model,abFilePath,items,modelAbInfoItems);

//			string matPath = Path.Combine(BlockPath.Sticker_Material_Dir, stickerData.prefab);
//			AddResItem(stickerData.prefab,matPath,items,modelAbInfoItems);

			string texPath = Path.Combine(BlockPath.Sticker_Texture_Dir, stickerData.texture);
			AddTextureResItem(stickerData.prefab,texPath,items,textureInfoItems);

		}

		private static void CreateTextures(PBPartInfo item, List<ResItem> items,List<ResItem> modelAbInfoItems,List<ResItem> textureInfoItems)
		{
			foreach (var textureItem in item.BlockInfo.Textures)
			{
				var textureData = PBDataBaseManager.Instance.GetDataWithPrefabName(textureItem.Prefab) as PBTextureData;
				var filePath = Path.Combine(BlockPath.Texture_Fbx_Dir, textureData.model);			
				AddResItem(textureData.model,filePath,items,modelAbInfoItems);

				string texMatPath = Path.Combine(BlockPath.Texture_Texture_Dir, textureData.texture);
				AddTextureResItem(textureData.prefab,texMatPath,items,textureInfoItems);
			}
		}

		enum MaterialType
		{
			Default = 0,
			SingleMeshMultiMat = 1,
		}

		private static void CreateMaterials(string materialLabel, string materialDir, List<ResItem> items,List<ResItem> modelAbInfoItems,List<ResItem> textureInfoItems,List<PEMaterialUtl.MaterialInfo> materialInfos)
		{
			MaterialType matType = MaterialType.Default;
			List<string> singleMaterials = new List<string>();
			List<string> multiMaterials = new List<string>();

			if (materialLabel.IndexOf(":") < 0)
			{
				singleMaterials.Add(materialLabel);
			}
			else
			{
				MaterialType type = MaterialType.Default;

				string[] materials = materialLabel.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < materials.Length; i++)
				{
					string[] element = materials[i].Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					if (element[0].Equals("type"))
					{
						type = (MaterialType) int.Parse(element[1]);
					}
					else
					{
						if (type == MaterialType.Default)
							singleMaterials.Add(element[1]);
						else
							multiMaterials.Add(element[1]);
					}
				}
			}

			AssignSingleMaterials(singleMaterials, materialDir, items,modelAbInfoItems,textureInfoItems,materialInfos);
			AssignSingleMaterials(multiMaterials, materialDir, items,modelAbInfoItems,textureInfoItems,materialInfos);
		}

		private static void AssignSingleMaterials(List<string> materials, string materialDir,List<ResItem> items,List<ResItem> modelAbInfoItems,List<ResItem> textureInfoItems,List<PEMaterialUtl.MaterialInfo> materialInfos)
		{
			foreach (var materialStr in materials)
			{
				var abFile = Path.Combine(materialDir, materialStr);

				if (!File.Exists(abFile+".mat"))
				{
					abFile = Path.Combine(BlockPath.MaterialCommon(), materialStr);
				}

//				Debug.LogError(materialDir+">>>>>>>>materialDir>>>>>"+abFile+"   -->>>> "+materialStr);


				var materialInfo = materialInfos.FirstOrDefault(s => s.name == materialStr);
//				Debug.LogError(materialStr+"-->>>>>materialStr");
				if (materialInfo != null)
				{
					if (materialInfo.propsTex.Any())
					{
						foreach (var item in materialInfo.propsTex)
						{
//							Debug.LogError(item.data+" >>>item.data");
							var t = textureInfoItems.FirstOrDefault(b => b.path == item.data);
							if (t != null)
							{
								var resItem = new ResItem {name = item.name};
								resItem.hash = t.hash;
								resItem.size = t.size;
								resItem.path =  t.path;
								if (items.All(s => s.path != resItem.path))
								{
									items.Add(resItem);
								}
							}
						}
					}
				}

//				AddResItem(materialStr,abFile,items,modelAbInfoItems);
			}
		}


		private static bool CreateBuildingModeResFile(string filePath, List<ResItem> modelAbInfoItems,List<ResItem> textureInfoItems,List<PEMaterialUtl.MaterialInfo> materialInfos,string outputDir,bool isOnline)
		{
			var fileName = Path.GetFileNameWithoutExtension(filePath);
			
			if (ResPublisher.EXCLUDE_BUILDINGS.Contains(fileName))
			{
				return false;
			}
			
			#if BLOCK_EDITOR
			PPBlockConfigInfo configInfo = PBBlockConfigManager.LoadBlockInfosWithoutAnim(filePath);
			#elif BLOCK_MODEL
//			var path = "./" + BlockServerUtil.GetBuildAnimPath(isOnline) + "/" + fileName + ".txt";
			var path = "./" + BlockServerUtil.GetBuildAnimPath(isOnline) + "/" + fileName + ".txt";//here change later
			var configText = File.Exists(path) ? File.ReadAllText(path, Encoding.UTF8) : string.Empty;
			PPBlockConfigInfo configInfo = PBBlockConfigManager.LoadBlockInfosWithoutAnim(fileName , configText);
			if (configInfo == null)
			{
				Debug.LogError(path + ">>>" + (File.Exists(path) ? "搭建文件内容有问题!" : "搭建文件不存在!"));
				return false;
			}
			#endif
			var buildingModelResConfig = new ResConfig {resversion = "100", items = new List<ResItem>()};

			foreach (var item in configInfo.BlockInfos)
			{

				//地板特殊处理，配置表里面没有地板
				if (item.PrefabName == "diban")
				{
					AddResItem( "diban", "Assets/blockres/lowpolygon/category_1/block_fbxs/diban",buildingModelResConfig.items,modelAbInfoItems);
					continue;
				}

				if (item.PrefabName.StartsWith("sticker"))
				{
					CreateStickerItem(item.PrefabName, buildingModelResConfig.items,modelAbInfoItems,textureInfoItems);
					continue;
				}

				var blockData = PBDataBaseManager.Instance.GetDataWithPrefabName(item.PrefabName) as BlockData;

				Category category = (Category) Enum.Parse((typeof(Category)), blockData.category);

				string fbxDir = (BlockPath.Fbx(category, PolygonType.LOW));
				string matDir = (BlockPath.Material(category, PolygonType.LOW));
				
				AddResItem( blockData.model, (Path.Combine(fbxDir,blockData.model)),buildingModelResConfig.items,modelAbInfoItems);

				CreateMaterials(blockData.material, matDir, buildingModelResConfig.items,modelAbInfoItems,textureInfoItems,materialInfos);

				CreateTextures(item, buildingModelResConfig.items,modelAbInfoItems,textureInfoItems);
			}

			var config = JsonUtility.ToJson(buildingModelResConfig, true);

			File.WriteAllText(Path.Combine(outputDir, fileName), config);

			return true;
		}

		public static void CreateCustomSkuModelResConfig(List<SkuData> skuItems, bool isOnline = true)
		{
			var outpathAndorid = GetSkuModelConfigDir((isOnline ? string.Empty : "_test"));
			var buildConfigPathAndorid = GetBuildingModelConfigDir( isOnline ? string.Empty : "_test");
			CreateSkuModelResConfig(skuItems, outpathAndorid, buildConfigPathAndorid);

//			var outpathiOS = GetSkuModelConfigDir(PublisherGlobal.ROOTDIR_IOS + (isOnline ? string.Empty : "_test"));
//			var buildConfigPathiOS = GetBuildingModelConfigDir(PublisherGlobal.ROOTDIR_IOS) + (isOnline ? string.Empty : "_test");
//			CreateSkuModelResConfig(skuItems, outpathiOS, buildConfigPathiOS);
		}

		private static void CreateSkuModelResConfig(List<SkuData> skuItems,string outputpath,string buildconfigpath)
		{
			if (skuItems == null || skuItems.Count == 0)
			{
				Debug.LogError("当前Sku数据为空");
				return;
			}

			if (!Directory.Exists(outputpath))
			{
				Directory.CreateDirectory(outputpath);
			}

			int totalCount = skuItems.Count;
			int count = 0;
			var outputDirName = outputpath.Substring(outputpath.LastIndexOf("/", StringComparison.Ordinal) + 1);

			foreach (var skuItem in skuItems)
			{
				var skuModelResConfig = new ResConfig {resversion = "100", items = new List<ResItem>()};

				List<SkuCarData> skuCarItems = skuItem.Models.ToList();

				foreach (var itemCarItem in skuCarItems)
				{
					string filePath = Path.Combine(buildconfigpath, itemCarItem.model_sku_id);

					if (!File.Exists(filePath))
					{
						Debug.LogError("can not find :>>>" + filePath);
						continue;
					}
					var carModelConfig = File.ReadAllText(filePath);
					var resConfig = JsonUtility.FromJson<ResConfig>(carModelConfig);
					foreach (var configItem in resConfig.items)
					{
						if (skuModelResConfig.items.All(s => s.path != configItem.path))
						{
							skuModelResConfig.items.Add(configItem);
						}
					}
				}
				string resultContent = JsonUtility.ToJson(skuModelResConfig, true);
				File.WriteAllText(Path.Combine(outputpath, skuItem.block_sku_id.ToString()), resultContent);
				count++;
				bool result = EditorUtility.DisplayCancelableProgressBar("生成SKU模型资源配置表" + outputDirName,
					string.Format(skuItem.block_sku_id + "生成中....{0}/{1}", count, totalCount), count * 1.0f / totalCount);
				if (result)
				{
					break;
				}
			}
			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("create sku block model res config files", "finished->" + outputDirName, "OK");
		}
	}
}
