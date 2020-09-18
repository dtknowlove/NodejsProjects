using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPath
{
	private const string ROOT_DIR = "Assets/BlockRes/";
	private const string LOW_DIR = "Assets/BlockRes/LowPolygon/";
	private const string HIGH_DIR = "Assets/BlockRes/HighPolygon/";
	
	private const string PREFAB_NAME = "Block_Prefabs";
	private const string FBX_NAME = "Block_Fbxs";
	private const string MATERIAL_NAME = "Block_Materials";
	private const string THUMB_NAME = "Block_Thumbs";

	private static Dictionary<Category, string> blockDirMap = new Dictionary<Category, string>
	{
		{Category.large, "1"},
		{Category.largesuper, "pbl"},
		{Category.fig, "fig"},
		{Category.small, "pbs"},
		{Category.tech, "tech"},
	};

	public static string Prefab(Category category, PolygonType pType)
	{
		if (category == Category.sticker)
		{
			return Sticker_Prefab_Dir;
		}
		if (category == Category.custom)
		{
			return Custom_Prefab_Dir;
		}

		return GetPath(PREFAB_NAME, category, pType);
	}

	public static string Fbx(Category category, PolygonType pType)
	{
		if (category == Category.sticker)
		{
			return Sticker_Fbx_Dir;
		}
		if (category == Category.custom)
		{
			return Custom_Fbx_Dir;
		}
		return GetPath(FBX_NAME, category, pType);
	}

	public static string Material(Category category, PolygonType pType)
	{
		if (category == Category.sticker)
			return Sticker_Material_Dir;
		return GetPath(MATERIAL_NAME, category, pType);
	}

	public static string MaterialCommon()
	{
		return string.Format("{0}CommonRes/{1}", ROOT_DIR, MATERIAL_NAME);
	}

	public static string Thumb(Category category, PolygonType pType)
	{
		string path = category == Category.sticker ? Sticker_Texture_Dir : GetPath(THUMB_NAME, category, pType);
		
#if BLOCK_EDITOR
		path = path.Replace("Assets/BlockRes", "block_thumbs");
#endif
		return path;
	}

	private static string GetPath(string name, Category category, PolygonType pType)
	{
		if (!blockDirMap.ContainsKey(category))
		{
			Debug.LogError("can not find "+category.ToString());
			return null;
		}

		string categoryStr = blockDirMap[category];
		string path;
		if (pType == PolygonType.LOW)
		{
			path = LOW_DIR;
			path += string.Format("Category_{0}/{1}", categoryStr, name);
		}
		else
		{
			path = HIGH_DIR;
			path += string.Format("Category_H_{0}/{1}", categoryStr, name);
		}
		return path;
	}
	
	private const string Texture_Dir = "Assets/BlockRes/Textures/";
	public const string Texture_Prefab_Dir = Texture_Dir + "Texture_Prefabs";
	public const string Texture_Fbx_Dir = Texture_Dir + "Texture_Fbxs";
	public const string Texture_Texture_Dir = Texture_Dir + "Texture_Textures";
	public const string Texture_Material_Dir = Texture_Dir + "Texture_Materials";
	
	private const string Sticker_Dir = "Assets/BlockRes/Stickers/";
	public const string Sticker_Prefab_Dir = Sticker_Dir + "Sticker_Prefabs";
	public const string Sticker_Fbx_Dir = Sticker_Dir + "Sticker_Fbxs";
	public const string Sticker_Texture_Dir = Sticker_Dir + "Sticker_Textures";
	public const string Sticker_Material_Dir = Sticker_Dir + "Sticker_Materials";

	private const string Custom_Dir = "Assets/BlockRes/Custom/";
	public const string Custom_Prefab_Dir = Custom_Dir + "Custom_Prefabs";
	public const string Custom_Fbx_Dir = Custom_Dir + "Custom_Fbxs";
	
}
