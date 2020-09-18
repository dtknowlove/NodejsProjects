using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class CustomBlocks {

	public static bool GetCustomConfig(List<BlockModelData> blockModelDatas)
	{
		if (!Directory.Exists("Assets/BlockRes/Custom/Custom_Fbxs"))
		{
			return false;
		}

		var fbxFiles = Directory.GetFiles("Assets/BlockRes/Custom/Custom_Fbxs","*.*",SearchOption.TopDirectoryOnly);
		var files = fbxFiles.Where(s => s.EndsWith(".FBX")||s.EndsWith(".fbx"));
		foreach (var item in files)
		{
			var blockModelData = new BlockModelData()
			{
				model = Path.GetFileNameWithoutExtension(item),
				category = "custom",
				isAllColor = "1",
				partnum = "0",
				scale = "1",
				size = "1",
				type = "1"
			};
			blockModelDatas.Add(blockModelData);
		}

		return true;

	}
}
