using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.putao.paibloks.editor
{
	public class PEThumbEditor
	{
		private static string[] ExcludeCorrectPNGs =
		{
			"tex_track_n",
		};
		
		
		[MenuItem("BlockTool/缩略图生成", false, GlobalDefine.Menu_Thumbs)]
		public static void ThumbsGenerate()
		{
			PEThumbGenerator window = EditorWindow.GetWindow<PEThumbGenerator>(false);
			window.Show();
		}

//		[MenuItem("BlockTool/Thumbs/Correct PNG", false, GlobalDefine.Menu_Thumbs)]
//		public static void ThumbsCorrect()
//		{
//			List<string> thumbs = new List<string>();
//			
//			string[] directories = Directory.GetDirectories(Application.dataPath + "/BlockRes", "Block_Thumbs", SearchOption.AllDirectories);
//			for (int i = 0; i < directories.Length; i++)
//			{
//				string[] files = Directory.GetFiles(directories[i], "*.png", SearchOption.AllDirectories);
//				thumbs.AddRange(files);
//			}
//
//			directories = Directory.GetDirectories(Application.dataPath + "/BlockRes", "*_Textures", SearchOption.AllDirectories);
//			for (int i = 0; i < directories.Length; i++)
//			{
//				string[] files = Directory.GetFiles(directories[i], "*.png", SearchOption.AllDirectories);
//				thumbs.AddRange(files);
//			}
//
//			try
//			{
//				for (int i = 0; i < thumbs.Count; i++)
//				{
//					string tName = Path.GetFileNameWithoutExtension(thumbs[i]);
//					if (ExcludeCorrectPNGs.Contains(tName))
//						continue;
//					
//					string p = thumbs[i].Substring(thumbs[i].IndexOf("Assets"));
//					EditorUtility.DisplayProgressBar("矫正缩略图格式", p, (float) i / thumbs.Count);
//
//					if (p.Contains("Block_Thumbs"))
//						CorrectThumb(p);
//					else
//						CorrectTexture(p);
//				}
//
//				AssetDatabase.SaveAssets();
//				AssetDatabase.Refresh();
//			}
//			finally
//			{
//				EditorUtility.ClearProgressBar();	
//			}
//		}
//
//		public static void CorrectThumb(string path)
//		{
//			bool needUpdate = false;
//			TextureImporter ai = AssetImporter.GetAtPath(path) as TextureImporter;
//
//			if (ai.textureType != TextureImporterType.Default)
//			{
//				ai.textureType = TextureImporterType.Default;
//				needUpdate = true;
//			}
//			if (ai.npotScale != TextureImporterNPOTScale.ToNearest)
//			{
//				ai.npotScale = TextureImporterNPOTScale.ToNearest;
//				needUpdate = true;
//			}
//			if (ai.mipmapEnabled != false)
//			{
//				ai.mipmapEnabled = false;
//				needUpdate = true;
//			}
//			if (ai.alphaIsTransparency != true)
//			{
//				ai.alphaIsTransparency = true;
//				needUpdate = true;
//			}
//			if (ai.textureCompression != TextureImporterCompression.Uncompressed)
//			{
//				ai.textureCompression = TextureImporterCompression.Uncompressed;
//				needUpdate = true;
//			}
//
//			if (needUpdate)
//				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
//		}
//		
//		public static void CorrectTexture(string path)
//		{
//			bool needUpdate = false;
//			TextureImporter ai = AssetImporter.GetAtPath(path) as TextureImporter;
//
//			if (ai.textureType != TextureImporterType.Default)
//			{
//				ai.textureType = TextureImporterType.Default;
//				needUpdate = true;
//			}
//			if (ai.npotScale != TextureImporterNPOTScale.ToNearest)
//			{
//				ai.npotScale = TextureImporterNPOTScale.ToNearest;
//				needUpdate = true;
//			}
//			if (ai.mipmapEnabled != false)
//			{
//				ai.mipmapEnabled = false;
//				needUpdate = true;
//			}
//			if (ai.alphaIsTransparency != true)
//			{
//				ai.alphaIsTransparency = true;
//				needUpdate = true;
//			}
//			if (ai.textureCompression != TextureImporterCompression.Uncompressed)
//			{
//				ai.textureCompression = TextureImporterCompression.Uncompressed;
//				needUpdate = true;
//			}
//
//			if (needUpdate)
//				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
//		}
	}
}