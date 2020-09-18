using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PTGame.Core;
using System.Linq;
using PTGame;

public class PBDataBaseManager : PTSingleton<PBDataBaseManager>
{
	private const string MODEL_DATA_PATH_APP = "config_blockdata/modeldata.json";
	private const string Prefab_DATA_PATH_APP = "config_blockdata/prefabdata.json";
	private const string COLOR_DATA_PATH_APP = "config_blockdata/colordata.json";
	private const string PARTNUM_DATA_PATH_APP = "config_blockdata/partnumdata.json";
	
	private const string TEXTURE_DATA_PATH_APP = "config_blockdata/texturedata.json";
	private const string STICKER_DATA_PATH_APP = "config_blockdata/stickerdata.json";
	private const string MAT_DATA_PATH_APP = "config_blockdata/database_models_material.txt";
	
	private const string MODEL_DATA_PATH = "Assets/BlockData/modeldata.json";
	private const string Prefab_DATA_PATH = "Assets/BlockData/prefabdata.json";
	private const string COLOR_DATA_PATH = "Assets/BlockData/colordata.json";
	private const string PARTNUM_DATA_PATH = "Assets/BlockData/partnumdata.json";
	
	private const string TEXTURE_DATA_PATH = "Assets/BlockData/texturedata.json";
	private const string STICKER_DATA_PATH = "Assets/BlockData/stickerdata.json";
	private const string MAT_DATA_PATH = "Assets/BlockData/database_models_material.txt";
	
	private List<BlockModelData> blockModelDatas;
	private List<BlockPrefabData> blockPrefabDatas;
	private List<BlockColorData> blockColorDatas;
	private List<PartNumData> partNumDatas;
	
	private List<BlockData> blockDatas;

	private List<PBTextureData> textureDatas;
	private List<StickerData> stickerDatas;
	private static bool mIsInitialize = false;

	private List<MaterialInfo> matDatas;
	
	private PBDataBaseManager()
	{
		if (blockDatas == null || !mIsInitialize)
		{
			Init();
		}
	}

	public static void Release()
	{
		if (!mIsInitialize)
			return;
		Instance.Dispose();
	}

#if BLOCK_MODEL || BLOCK_EDITOR
	private bool isApp = false;
#else
	private bool isApp = true;
#endif
	
	private void Init()
	{
#if BLOCK_MODEL || BLOCK_EDITOR
		string persistentPath = Application.persistentDataPath;
#else
		string persistentPath = PTUGame.persistentDataPath;
#endif
		
		var modelDatPath = isApp?Path.Combine(persistentPath,MODEL_DATA_PATH_APP):MODEL_DATA_PATH;
		var prefabDataPath = isApp?Path.Combine(persistentPath,Prefab_DATA_PATH_APP):Prefab_DATA_PATH;
		var colorDataPath = isApp?Path.Combine(persistentPath,COLOR_DATA_PATH_APP):COLOR_DATA_PATH;
		var partNumDataPath = isApp?Path.Combine(persistentPath,PARTNUM_DATA_PATH_APP):PARTNUM_DATA_PATH;
		var textureDataPath = isApp?Path.Combine(persistentPath,TEXTURE_DATA_PATH_APP):TEXTURE_DATA_PATH;
		var stickerDataPath = isApp?Path.Combine(persistentPath,STICKER_DATA_PATH_APP):STICKER_DATA_PATH;
		var matDataPath = isApp?Path.Combine(persistentPath,MAT_DATA_PATH_APP):MAT_DATA_PATH;
		
		blockModelDatas = LitJson.JsonMapper.ToObject<List<BlockModelData>>(File.ReadAllText(modelDatPath));
		PTDebug.LogWarning("{0} 解析完成。", modelDatPath);

		blockPrefabDatas = LitJson.JsonMapper.ToObject<List<BlockPrefabData>>(File.ReadAllText(prefabDataPath));
		PTDebug.LogWarning("{0} 解析完成。", prefabDataPath);

		blockColorDatas = LitJson.JsonMapper.ToObject<List<BlockColorData>>(File.ReadAllText(colorDataPath));
		PTDebug.LogWarning("{0} 解析完成。", colorDataPath);
		
		textureDatas = LitJson.JsonMapper.ToObject<List<PBTextureData>>(File.ReadAllText(textureDataPath));
		PTDebug.LogWarning("{0} 解析完成。", textureDataPath);

		stickerDatas = LitJson.JsonMapper.ToObject<List<StickerData>>(File.ReadAllText(stickerDataPath));
		PTDebug.LogWarning("{0} 解析完成。", stickerDataPath);

		matDatas = JsonUtility.FromJson<MaterialInfos>(File.ReadAllText(matDataPath)).materialInfos;
		
		if (!isApp)
		{
			CustomBlocks.GetCustomConfig(blockModelDatas);
			partNumDatas = LitJson.JsonMapper.ToObject<List<PartNumData>>(File.ReadAllText(partNumDataPath));

		}
		GenerateBlockDatas();
		mIsInitialize = true;
	}

	public List<BlockColorData> GetBlockColorDatas()
	{
		return blockColorDatas;
	}

	public List<BlockData> GetBlockDatas()
	{
		return blockDatas;
	}

	public List<PBTextureData> GetTextureDatas()
	{
		return textureDatas;
	}

	public List<StickerData> GetStickerDatas()
	{
		return stickerDatas;
	}
	
	public List<MaterialInfo> GetMatDatas()
	{
		return matDatas;
	}

	public MaterialInfo GetMatInfoByName(string matName)
	{
		try
		{
			return matDatas.First(t => t.name.Equals(matName));
		}
		catch (Exception e)
		{
			throw new Exception(">>>> 材料库中没有 " + matName);
		}
	}

	public object GetDataWithPrefabName(string prefabName)
	{
		if (prefabName.StartsWith("sticker"))
			return GetStickerWithPrefabName(prefabName);
		if (prefabName.StartsWith("tex"))
			return GetTextureWithPrefabName(prefabName);
		return GetBlockWithPrefabName(prefabName);
	}

	public BlockData GetBlockWithPrefabName(string prefabName)
	{
		try
		{
			return blockDatas.First(s => string.Equals(s.prefab, prefabName));
		}
		catch (Exception e)
		{
			throw new Exception(">>>> 零件库中没有 " + prefabName);
		}
	}

	public PBTextureData GetTextureWithPrefabName(string prefabName)
	{
		try
		{
			return textureDatas.First(s => string.Equals(s.prefab, prefabName));
		}
		catch (Exception e)
		{
			throw new Exception(">>>> 丝印库中没有 " + prefabName);
		}
	}

	public StickerData GetStickerWithPrefabName(string prefabName)
	{
		try
		{
			return stickerDatas.First(s => string.Equals(s.prefab, prefabName));
		}
		catch (Exception e)
		{
			throw new Exception(">>>> 贴纸库中没有 " + prefabName);
		}
	}

	public Category GetCategoryWithPrefabName(string prefabName)
	{
		if (prefabName.StartsWith("sticker"))
			return Category.sticker;

		string category = GetBlockWithPrefabName(prefabName).category;
		return (Category) Convert.ToInt32(Enum.Parse(typeof(Category), category, true));
	}

	public bool IsSticker(string prefabName)
	{
		return prefabName.StartsWith("sticker");
	}
	
	
	
	#region GenerateBlockDatas

	/// <summary>
	/// 合并 modeldata, prefabdata, colordata, partnumdata 四张表数据，生成最终的 blockdata
	/// </summary>
	private void GenerateBlockDatas()
	{
		//change list to dictionary for quick search
		Dictionary<string, List<BlockPrefabData>> blockPrefabDict = new Dictionary<string, List<BlockPrefabData>>();
		foreach (BlockPrefabData prefabData in blockPrefabDatas)
		{
			List<BlockPrefabData> prefabList;
			if (!blockPrefabDict.TryGetValue(prefabData.model, out prefabList))
			{
				prefabList = new List<BlockPrefabData>();
				blockPrefabDict.Add(prefabData.model, prefabList);
			}
			prefabList.Add(prefabData);
		}
		Dictionary<string, string> partNumDict = new Dictionary<string, string>();
		if (!isApp)
		{
			foreach (PartNumData numData in partNumDatas)
			{
				partNumDict[numData.new_num] = numData.old_num;
			}
		}

		
		//modeldata.json 文件里面包含了所有的model.包扩prefabdata.json中的，特殊的，不是all color。
		//generate blockdatas
		blockDatas = new List<BlockData>();

		foreach (BlockModelData modelData in blockModelDatas)
		{
			if (int.Parse(modelData.isAllColor) == 1)
			{
				GenerateBlockDatas_AllColor(modelData, partNumDict);
			}
			else
			{
				GenerateBlockDatas_NotAllColor(modelData, blockPrefabDict, partNumDict);
			}
		}

		if (!string.IsNullOrEmpty(mErrorMsg_NotAllColor))
		{
			Debug.LogError(mErrorMsg_NotAllColor);
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.DisplayDialog("Error!", mErrorMsg_NotAllColor, "OK");
			#endif
		}
	}

	private readonly Dictionary<string, string> CMF_Material_Dict = new Dictionary<string, string>()
	{
		{"M01", "ABS747"},//塑料
		{"M02", "MABS920"}, // 透明塑料
		{"M03", "TPE/TPR"}, // 橡胶，轮胎材料
		{"M04", "PP"}, // 例如旗子
		{"M05", "POM"}, // 例如轮轴
		{"M06", "Silicon"}, //硅胶大眼睛系列
		{"M07", "PVC"},
		{"M08", "PC"},
		{"M09", "PE"},
		{"M10", "EVA"},
		{"M11", "PU"},
		{"M12", "PA"},
		{"M13", "PMMA"},
		{"M14", "MABS920+0.04四边形银色闪粉"},
		{"M15", "MABS920+0.04四边形银色湖蓝色"}
	};

	private readonly Dictionary<string, string> CMF_Finish_Dict = new Dictionary<string, string>()
	{
		{"F01", "high gloss"}, // 高亮
		{"F02", "matte"}, //  哑光
		{"F03", "Texture VDI24"}, // 工艺纹理
		{"F04", "Texture VDI36"}, // 工艺纹理
		{"F05", "Painting"},
		{"F06", "Water Transfer"},
		{"F07", "电镀"},
		{"F08", "Glliter"},
	};
	
	//料号说明
	//  15-00007   -   001   -   01   -    00
	//  模具号         色号       材料号     
 	//Unity导出Bom表，料号直接引用Color Code 和 Material Code 中的数字，无需Finish Code(因模具表面处理不可改变，
	//不可以对应2个以上的Finish),描述内更改旧的色号为新的CMF编号。
	
	
	private void GenerateBlockDatas_AllColor(BlockModelData modelData, Dictionary<string, string> partNumDict)
	{
		foreach (BlockColorData colorData in blockColorDatas)
		{
			BlockData blockData = new BlockData()
			{
				model = modelData.model,
				prefab = modelData.model + "_" + colorData.color,
				category = modelData.category,
				type = modelData.type,
				size = modelData.size,
				scale = modelData.scale,
				color = colorData.color,
				material = colorData.color,
				material_high = colorData.color,
				isAllColor = true
			};
			
			blockDatas.Add(blockData);

			if (!isApp)
			{
				string[] cmfStrs = colorData.newCode.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
				string CMF_Material_Num = cmfStrs[2];
				string CMF_Finish_Num = cmfStrs[3];
				string CMF_Code = colorData.newCode;

				blockData.partnum = string.Format("{0}-{1}-{2}", modelData.partnum,
					colorData.code.Substring(1, 3) + CMF_Material_Num.Substring(1, 2),
					modelData.partversion);
				blockData.description = string.Format("{0}, {1}, {2}", CMF_Material_Dict[CMF_Material_Num], CMF_Finish_Dict[CMF_Finish_Num], CMF_Code);

				CorrectPartNum(blockData, partNumDict);

				//8月22日的需求来自王文武
				for (int i = 1; i <= 57; i++)
				{
					var t = "15-00256-0" + string.Format("{0:00}", i) + "01";
					if (blockData.partnum.Contains(t))
					{
						blockData.partnum = blockData.partnum.Replace(t, "15-00256-0" + string.Format("{0:00}", i) + "03");
					}
				}

				if (blockData.partnum.Contains("15-00256-00501"))
				{
					blockData.partnum = blockData.partnum.Replace("15-00256-00501", "15-00256-00503");
				}
			}
		}
	}

	private string mErrorMsg_NotAllColor = "";

	private void GenerateBlockDatas_NotAllColor(BlockModelData modelData, Dictionary<string, List<BlockPrefabData>> blockPrefabDict, Dictionary<string, string> partNumDict)
	{
		List<BlockPrefabData> prefabList;
		if (!blockPrefabDict.TryGetValue(modelData.model, out prefabList))
		{
			if (string.IsNullOrEmpty(mErrorMsg_NotAllColor))
				mErrorMsg_NotAllColor = "在modeldata.json里配的\"isAllColor\"为\"否\"，但是在prefabdata.json里找不到对应的model，请联系相关人员改表！\n";
			mErrorMsg_NotAllColor += modelData.model + "\n";
			return;
		}
		foreach (BlockPrefabData prefabData in prefabList)
		{
			BlockData blockData = new BlockData()
			{
				model = modelData.model,
				prefab = prefabData.prefab,
				category = modelData.category,
				type = modelData.type,
				size = modelData.size,
				scale = modelData.scale,
				color = prefabData.color,
				material = prefabData.material,
				material_high = prefabData.material_high,
				partnum = prefabData.partnum,
				description = prefabData.description,
				isAllColor = false
			};
			blockDatas.Add(blockData);

			if (!isApp)
				CorrectPartNum(blockData, partNumDict);
		}
	}

	private void CorrectPartNum(BlockData blockData, Dictionary<string, string> partNumDict)
	{
		string oldNum;
		if (partNumDict.TryGetValue(blockData.partnum, out oldNum))
			blockData.partnum = oldNum;
	}
	
	#endregion
}
