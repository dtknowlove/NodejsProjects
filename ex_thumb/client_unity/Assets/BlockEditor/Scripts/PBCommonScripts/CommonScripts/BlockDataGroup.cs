using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using PTGame.Core;

public class BlockDataItem
{
	public string colorName;
	public string prefabName;
	public string imageName;
	public string material;
	public string materialHigh;
	public string partnum;
}

public class BlockDataGroup
{
	public string modelName;
	public string type;
	public string size;
	public Category category;
	public bool isAllColor;

	private List<BlockDataItem> blockDataItems;
	private List<string> colors;
	private List<string> materials;
	private List<string> prefabs;

	public List<GUIContent> subPreviews;

	public int BlockCount
	{
		get { return prefabs.Count; }
	}
	
	public BlockDataGroup()
	{
		colors = new List<string>();
		materials = new List<string>();
		prefabs = new List<string>();
		blockDataItems = new List<BlockDataItem>();
	}

	public void AddBlockDataItem(BlockDataItem item)
	{
		blockDataItems.Add(item);
		if (!colors.Contains(item.colorName))
			colors.Add(item.colorName);
		materials.Add(item.material);
		prefabs.Add(item.prefabName);
	}

	public bool IsContainPrefab(string prefabName)
	{
		return prefabs.Contains(prefabName);
	}

	public List<string> GetColors()
	{
		return colors;
	}

	public List<string> GetMaterials()
	{
		return materials;
	}

	public List<string> GetPrefabs()
	{
		return prefabs;
	}
	
	public List<BlockDataItem> GetItems()
	{
		return blockDataItems;
	}
	
	public List<string> GetPrefabsByColor(string colorName)
	{
		return blockDataItems.Where(item => item.colorName.Equals(colorName)).Select(item => item.prefabName).ToList();
	}

	public void OrderByColors()
	{
		prefabs = prefabs.OrderBy(s => s).ToList();
	}

	public void LoadSubPrefabs()
	{
		#if UNITY_EDITOR
		if (subPreviews == null)
		{
			subPreviews = new List<GUIContent>();
		}
		foreach (string prefab in prefabs)
		{
			if (subPreviews.All(s => s.tooltip != prefab))
			{
				var texture2D = LoadPrefabPreview(prefab);
				subPreviews.Add(new GUIContent() {image = texture2D, tooltip = prefab});
			}
		}
		#endif
	}

	public GUIContent GetPreviewOfPrefab(string prefab)
	{
		if (subPreviews == null)
		{
			subPreviews = new List<GUIContent>();
		}

		var t = subPreviews.FirstOrDefault(s => s.tooltip == prefab);
		
		#if UNITY_EDITOR
		if (t == null)
		{
			var texture2D = LoadPrefabPreview(prefab);
			var previewContent = new GUIContent() {image = texture2D, tooltip = prefab};
			subPreviews.Add(previewContent);
			return previewContent;
		}
		#endif
		return t;
	}


	#if UNITY_EDITOR
	private Texture2D LoadPrefabPreview(string prefabName)
	{
		string prefabPath = string.Format("{0}/{1}.prefab", BlockPath.Prefab(category, PolygonType.LOW), prefabName);
		if(!File.Exists(prefabPath)){
			PEPrefabGeneratorUtil.CreateSinglePrefab(prefabName,this,category,PolygonType.LOW);
			UnityEditor.AssetDatabase.Refresh();
		}
		var texture2D = GetPreviewTex(prefabPath);
		return texture2D;
	}
	#endif

	private Texture2D GetPreviewTex(string astPath)
	{
		#if UNITY_EDITOR
		var guid = UnityEditor.AssetDatabase.AssetPathToGUID(astPath);
		var texPath = "Library/metadata/"+guid.Substring(0, 2) + "/" + guid + ".info";
		var texN = TextureUtils.LoadTexture(texPath);
		return texN;
		#else
		return null;
		#endif
	}

	public bool IsBeta(string prefabName)
	{
		foreach (BlockDataItem item in blockDataItems)
		{
			if (item.prefabName.Equals(prefabName))
			{
				return item.partnum == "0" || item.partnum == null;
			}
		}
		return false;
	}
}


public class BlockDataGroupFactory : PTSingleton<BlockDataGroupFactory>
{
	private List<BlockDataGroup> blockDataGroups;
	private List<BlockDataGroup> blockDataGroupsHigh;

	private BlockDataGroupFactory()
	{
		Init();
	}

	private void Init()
	{
		if (blockDataGroups == null)
		{

			blockDataGroups = new List<BlockDataGroup>();
			blockDataGroupsHigh = new List<BlockDataGroup>();

			ReadFromDB(PBDataBaseManager.Instance.GetBlockDatas(), blockDataGroups);
			ReadFromDB(PBDataBaseManager.Instance.GetBlockDatas(), blockDataGroupsHigh);
		}
	}

	private void ReadFromDB(List<BlockData> blockDatas, List<BlockDataGroup> dataGoups)
	{
		foreach (BlockData blockData in blockDatas)
		{
			BlockDataGroup blockGroup = dataGoups.Find(s => s.modelName.Equals(blockData.model));
			if (blockGroup == null)
			{
				blockGroup = new BlockDataGroup();
				blockGroup.modelName = blockData.model;
				blockGroup.type = blockData.type;
				blockGroup.size = blockData.size;
				blockGroup.category = (Category) Convert.ToInt32(Enum.Parse(typeof(Category), blockData.category, true));
				blockGroup.isAllColor = blockData.isAllColor;

				dataGoups.Add(blockGroup);
			}
			BlockDataItem dataItem = new BlockDataItem();
			dataItem.colorName = blockData.color;
			dataItem.prefabName = blockData.prefab;
			dataItem.material = blockData.material;
			dataItem.materialHigh = blockData.material_high;
			dataItem.partnum = blockData.partnum;
			blockGroup.AddBlockDataItem(dataItem);
		}
	}


	public BlockDataGroup FindColorGroup(string prefabName)
	{
		if (blockDataGroups == null)
		{
			return null;
		}

		foreach (BlockDataGroup group in blockDataGroups)
		{
			if (group.IsContainPrefab(prefabName))
			{
				return group;
			}
		}

		return null;
	}

	public List<BlockDataGroup> GetAllGroups(PolygonType pType = PolygonType.LOW)
	{
		return pType == PolygonType.HIGH ? blockDataGroupsHigh : blockDataGroups;
	}

	public List<BlockDataGroup> GetGroupsByCategory(Category category, PolygonType pType = PolygonType.LOW)
	{
		List<BlockDataGroup> allGroups = GetAllGroups(pType);
		return allGroups.FindAll(g => g.category == category);
	}
}