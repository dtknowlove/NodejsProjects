using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PETexThumbGenerator {

	[MenuItem("BlockTool/生成丝印零件缩略图配置文件",false,GlobalDefine.Menu_TEXTHUMB)]
	public static void GenerateTexThumbConfig()
	{
		TexThumbConfig thumbConfig = new TexThumbConfig();
		
		var files = Directory.GetFiles("block_thumbs/LowPolygon", "*", SearchOption.AllDirectories);
		var t = files.Where(s => Path.GetFileNameWithoutExtension(s).Contains("-"));
		foreach (var item in t)
		{	
			thumbConfig.items.Add(new ConfigItem()
			{
				thumbPath = Path.GetDirectoryName(item),
				thumbFile = Path.GetFileNameWithoutExtension(item)
			});
		}
		
		File.WriteAllText(Application.dataPath+"/BlockData/texthumb.json",JsonUtility.ToJson(thumbConfig,true));

		EditorUtility.DisplayDialog("生成textthumbconfig", "完整", "确定");
	}
	
	[System.Serializable]
	private class TexThumbConfig
	{
		public List<ConfigItem> items;

		public TexThumbConfig()
		{
			items = new List<ConfigItem>();
		}
	}

	[System.Serializable]
	private class ConfigItem
	{
		public string thumbPath;
		public string thumbFile;
	}


}
