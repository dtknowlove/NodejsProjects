using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace com.putao.paibloks.editor
{
	public class PETexPrefabGenerator : EditorWindow
	{

		public static void CreateStickAndTexMaterial()
		{
			CreateTextureMat();
			CreateStickerMat();
		}
		
		static void CreateTextureMat()
		{
			List<PBTextureData> datas = PBDataBaseManager.Instance.GetTextureDatas();

			List<string> fbxErrors = new List<string>();
			List<string> matErrors = new List<string>();
			List<string> texErrors = new List<string>();
			try
			{
				for (int i = 0; i < datas.Count; i++)
				{
					EditorUtility.DisplayProgressBar("生成丝印材质", "", (float) i / datas.Count);

					PBTextureData data = datas[i];
					GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(BlockPath.Texture_Fbx_Dir, data.model + ".fbx"));
					if (fbx == null)
					{
						if (!fbxErrors.Contains(data.model))
							fbxErrors.Add(data.model);
						continue;
					}

					string matPath = Path.Combine(BlockPath.Texture_Material_Dir, data.prefab + ".mat");
					Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
					if (mat == null)
					{
						mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(BlockPath.Texture_Material_Dir, data.material + ".mat"));
						if (mat == null)
						{
							if (!matErrors.Contains(data.material))
								matErrors.Add(data.material);
							continue;
						}
						
						AssetDatabase.CreateAsset(GameObject.Instantiate(mat), matPath);
						mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
					}
					
					Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(BlockPath.Texture_Texture_Dir, data.texture + ".png"));
					if (tex == null)
					{
						if (!texErrors.Contains(data.texture))
							texErrors.Add(data.texture);
						continue;
					}
					
					mat.mainTexture = tex;

				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();

				AssetDatabase.Refresh();
				
				if (fbxErrors.Count > 0)
					DisplayErrorDialog("找不到以下fbx\n", fbxErrors);
				if (matErrors.Count > 0)
					DisplayErrorDialog("找不到以下material\n", matErrors);
				if (texErrors.Count > 0)
					DisplayErrorDialog("找不到以下textures\n", texErrors);

			}
		}
		
		static void CreateStickerMat()
		{
			List<StickerData> datas = PBDataBaseManager.Instance.GetStickerDatas();
			
			List<string> fbxErrors = new List<string>();
			List<string> matErrors = new List<string>();
			List<string> texErrors = new List<string>();
			try
			{
				for (int i = 0; i < datas.Count; i++)
				{
					EditorUtility.DisplayProgressBar("生成贴纸材质", "", (float) i / datas.Count);

					StickerData data = datas[i];
					GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(BlockPath.Sticker_Fbx_Dir, data.model + ".fbx"));
					if (fbx == null)
					{
						if (!fbxErrors.Contains(data.model))
							fbxErrors.Add(data.model);
						continue;
					}

					string matPath = Path.Combine(BlockPath.Sticker_Material_Dir, data.prefab + ".mat");
					Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
					if (mat == null)
					{
						mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(BlockPath.Sticker_Material_Dir, data.material + ".mat"));
						if (mat == null)
						{
							if (!matErrors.Contains(data.material))
								matErrors.Add(data.material);
							continue;
						}
						
						AssetDatabase.CreateAsset(GameObject.Instantiate(mat), matPath);
						mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
					}
					
					Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(BlockPath.Sticker_Texture_Dir, data.texture + ".png"));
					if (tex == null)
					{
						if (!texErrors.Contains(data.texture))
							texErrors.Add(data.texture);
						continue;
					}
					
					mat.mainTexture = tex;

				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();

				AssetDatabase.Refresh();
				
				if (fbxErrors.Count > 0)
					DisplayErrorDialog("找不到以下fbx\n", fbxErrors);
				if (matErrors.Count > 0)
					DisplayErrorDialog("找不到以下material\n", matErrors);
				if (texErrors.Count > 0)
					DisplayErrorDialog("找不到以下textures\n", texErrors);

			}
		}
		
		private static void DisplayErrorDialog(string title, List<string> errors)
		{
			StringBuilder sb = new StringBuilder(title);
			foreach (string error in errors)
			{
				sb.AppendFormat("\n{0}.fbx", error);
			}
			EditorUtility.DisplayDialog("Error", sb.ToString(), "确定");
		}
	}
}