using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PTGame.Core;
using UnityEngine;

public class PBPrefabLiteracy 
{
	#region read

	 /// <summary>
    /// 开放接口：加载block config info
    /// </summary>
    public static void LoadPrefabInfo(XmlDocument xml, PPBlockConfigInfo configInfo)
    {
	    var node = xml.SelectSingleNode ("config").SelectSingleNode ("blocks");
	    if (node == null)
	    {
		    Debug.Log("<color=cyan>【注意】====>当前配置文件不是3.0.0!</color>");
		    return;
	    }
	    ProcessBlockNode(node, configInfo.PrefabInfos);
	    ProcessStickerNode(node, configInfo.PrefabInfos);
	    ProcessTextureNode(node, configInfo.PrefabInfos);
    }

	private static void ProcessBlockNode(XmlNode node, List<PPPrefabInfo> prefabInfos)
	{
		XmlNodeList prefabNodes = node.SelectNodes("block");

		for (int i = 0; i < prefabNodes.Count; i++)
		{
			XmlNode prefabNode = prefabNodes[i];
			PPPrefabInfo prefabInfo = new PPPrefabInfo();
			prefabInfo.Name = prefabNode.Attributes["name"].Value;
			prefabInfo.Model = prefabNode.Attributes["model"].Value;
			prefabInfo.Material = prefabNode.Attributes["material"].Value;
			prefabInfo.Category = prefabNode.Attributes["category"].Value;
			prefabInfos.Add(prefabInfo);
		}
	}
	
	private static void ProcessStickerNode(XmlNode node, List<PPPrefabInfo> prefabInfos)
	{
		XmlNodeList prefabNodes = node.SelectNodes("sticker");

		for (int i = 0; i < prefabNodes.Count; i++)
		{
			XmlNode prefabNode = prefabNodes[i];
			PPPrefabInfo prefabInfo = new PPPrefabInfo();
			prefabInfo.Name = prefabNode.Attributes["name"].Value;
			prefabInfo.Model = prefabNode.Attributes["model"].Value;
			prefabInfo.Texture = prefabNode.Attributes["texture"].Value;
			prefabInfos.Add(prefabInfo);
		}
	}
	
	private static void ProcessTextureNode(XmlNode node, List<PPPrefabInfo> prefabInfos)
	{
		XmlNodeList prefabNodes = node.SelectNodes("texture");

		for (int i = 0; i < prefabNodes.Count; i++)
		{
			XmlNode prefabNode = prefabNodes[i];
			PPPrefabInfo prefabInfo = new PPPrefabInfo();
			prefabInfo.Name = prefabNode.Attributes["name"].Value;
			prefabInfo.Model = prefabNode.Attributes["model"].Value;
			prefabInfo.Material = prefabNode.Attributes["material"].Value;
			prefabInfo.Category = prefabNode.Attributes["category"].Value;
			prefabInfo.Thumb = prefabNode.Attributes["thumb"].Value;
			ProcessPrefabTexInfo(prefabNode, prefabInfo.Texs);
			prefabInfos.Add(prefabInfo);
		}
	}

	private static void ProcessPrefabTexInfo(XmlNode node,List<PPPrefabTexInfo> texInfo)
	{
		var texNodes = node.SelectNodes("tex");
		
		for (int i = 0; i < texNodes.Count; i++)
		{
			XmlNode texNode = texNodes[i];
			PPPrefabTexInfo prefabInfo = new PPPrefabTexInfo();
			prefabInfo.Name = texNode.Attributes["name"].Value;
			prefabInfo.Texture = texNode.Attributes["texture"].Value;
			prefabInfo.Model = texNode.Attributes["model"].Value;
			texInfo.Add(prefabInfo);
		}
	}

	#endregion
	
	#region write

	private static HashSet<string> mPrefabNames = new HashSet<string>();
	
	public static void SavePrefabInfo(Transform transform, XmlElement parentElement, XmlDocument xml)
	{
		mPrefabNames.Clear();
		XmlElement sectionElement = xml.CreateElement("blocks");
		parentElement.AppendChild(sectionElement);
		InnerSavePrefabInfo(transform, sectionElement, xml);
		mPrefabNames.Clear();
	}

	#region process block


	private static void InnerSavePrefabInfo(Transform transform, XmlElement parentElement, XmlDocument xml)
	{
		var uniqueblocks = new List<BlockInfo>();
		GetBlocks(transform, uniqueblocks);
		
		var stickerList = uniqueblocks.Where(t => t.IsSticker).ToList();
		var textureList = uniqueblocks.Where(t => t.IsStamp).ToList();
		var blockList = uniqueblocks.Except(stickerList).Except(textureList).ToList();

		blockList.ForEach(b =>
		{
			var blockElement = ProcessBlock(b.Prefab, xml);
			parentElement.AppendChild(blockElement);
		});
		stickerList.ForEach(s =>
		{
			var blockElement = ProcessSticker(s.Prefab, xml);
			parentElement.AppendChild(blockElement);
		});
		textureList.ForEach(t =>
		{
			var blockElement = ProcessBlock(t.Prefab, xml, t.Thumb);
			t.Textures.ForEach(ct =>
			{
				var texElement = ProcessTexture(ct.type, xml);
				blockElement.AppendChild(texElement);
			});
			parentElement.AppendChild(blockElement);
		});
	}
	
	private static void GetBlocks(Transform transform,List<BlockInfo> blocks)
	{
		foreach (Transform trans in transform)
		{
			var section = trans.GetComponent<PBSection>();
			if (section != null)
			{
				GetBlocks(section.transform, blocks);
			}
			var block = trans.GetComponent<PBBlock>();
			if (block == null) continue;
			var prefab = block.type;
			var thumb = GetThumbName(block);
			var unique = thumb.IsNullOrEmpty() ? prefab : thumb;
			if(mPrefabNames.Contains(unique))
				continue;
			mPrefabNames.Add(unique);
			blocks.Add(new BlockInfo(prefab, thumb, block.GetTextures()));
		}
	}
	
	private static string GetThumbName(PBBlock block)
	{
		string thumbName = GetStampThumbName(block);
		if (!string.IsNullOrEmpty(thumbName))
			return thumbName;

		return null;
	}

	/// <summary>
	/// 获取带有丝印的缩略图
	/// </summary>
	private static string GetStampThumbName(PBBlock block)
	{
		string[] texNames = block.GetTextureNames();
		if (texNames.Length == 0)
			return null;
		var stampName = block.type;
		foreach (var texName in texNames)
		{
			stampName += "-" + texName;
		}
		return stampName;
	}

	private static XmlElement ProcessBlock(string blockName, XmlDocument xml, string thumb = "")
	{
		var isStamp = thumb.IsNotNullAndEmpty();
		XmlElement blockElement = xml.CreateElement(isStamp ? "texture" : "block");
		var blockdata = PBDataBaseManager.Instance.GetDataWithPrefabName(blockName) as BlockData;
		blockElement.SetAttribute("name", blockdata.prefab);
		blockElement.SetAttribute("model", blockdata.model);
		blockElement.SetAttribute("material", blockdata.material);
		blockElement.SetAttribute("category", blockdata.category);
		if (isStamp)
			blockElement.SetAttribute("thumb", thumb);
		return blockElement;
	}

	private static XmlElement ProcessSticker(string blockName, XmlDocument xml)
	{
		XmlElement blockElement = xml.CreateElement("sticker");
		var stickerData= PBDataBaseManager.Instance.GetDataWithPrefabName(blockName) as StickerData;
		blockElement.SetAttribute("name", stickerData.prefab);
		blockElement.SetAttribute("texture", stickerData.texture);
		blockElement.SetAttribute("model", stickerData.model);
		return blockElement;
	}

	private static XmlElement ProcessTexture(string texName, XmlDocument xml)
	{
		XmlElement texElement = xml.CreateElement("tex");
		var textureData = PBDataBaseManager.Instance.GetDataWithPrefabName(texName) as PBTextureData;
		texElement.SetAttribute("name", textureData.prefab);
		texElement.SetAttribute("texture", textureData.texture);
		texElement.SetAttribute("model", textureData.model);
		return texElement;
	}

	class BlockInfo
	{
		public string Prefab;
		public string Thumb;

		public PBTexture[] Textures;
		
		public bool IsSticker
		{
			get { return Prefab.StartsWith("sticker"); }
		}

		public bool IsStamp
		{
			get { return Textures.Length > 0; }
		}

		public BlockInfo(string p,string t,PBTexture[] textures)
		{
			Prefab = p;
			Thumb = t;
			Textures = textures;
		}
	}

	#endregion
	#endregion
}
