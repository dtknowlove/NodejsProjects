
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


public static class PEPrefabGeneratorUtil {

		public static void CreatePrefabs(List<BlockDataGroup> blockDataGroups, PolygonType polygonType)
		{
			if (blockDataGroups == null || blockDataGroups.Count == 0)
				return;
			
			Category category = blockDataGroups[0].category;
			CreatePrefabsWithDataGroup(blockDataGroups, category, polygonType);
		}
	
		public static GameObject CreateSinglePrefab(string prefabName,BlockDataGroup blockDataGroup,Category category, PolygonType polygonType,bool isTemp =false)
		{
			BlockDataItem dataItem = blockDataGroup.GetItems().FirstOrDefault(s => s.prefabName == prefabName);
			
			return CreateSinglePrefab(prefabName,blockDataGroup.modelName,dataItem.material,dataItem.materialHigh,category,polygonType);
		}

		public static GameObject CreateSinglePrefab(string prefabName,string modelName,string material,string materialHigh,Category category, PolygonType polygonType,bool isTemp=false)
		{
			string prefabDir = BlockPath.Prefab(category, polygonType);
			string fbxDir = BlockPath.Fbx(category, polygonType);
			string matDir = BlockPath.Material(category, polygonType);
			
			List<string> fbxErrors = new List<string>();
			List<string> scaleErrors = new List<string>();
			List<string> angleErrors = new List<string>();
			List<string> materialErrors = new List<string>();
			
			{
				
				GameObject t = AssetDatabase.LoadAssetAtPath<GameObject>(fbxDir + "/" + modelName + ".fbx");
					
				if (!File.Exists(fbxDir + "/" + modelName + ".fbx"))
				{
					Debug.LogError("fbx 模型文件不存在 >>>>>"+fbxDir + "/" + modelName + ".fbx");
					EditorUtility.DisplayDialog("错误","fbx文件不存在"+fbxDir + "/" + modelName + ".fbx","确定");
					return null;
				}

				GameObject block = GameObject.Instantiate(t);
				block.name = modelName;

				Animator animator = block.GetComponent<Animator>();
				if (animator != null)
				{
					GameObject.DestroyImmediate(animator);
				}

				if (polygonType == PolygonType.LOW)
				{
					AssignMaterials(block, material, matDir, materialErrors);
				}
				else
				{
					AssignMaterials(block, materialHigh, matDir, materialErrors);
					HighPolygonGenRefpoints(block, category);
				}

				if (!Directory.Exists(prefabDir))
				{
					Directory.CreateDirectory(prefabDir);
				}

				if (!isTemp)
				{
					PrefabUtility.CreatePrefab(prefabDir + "/" + prefabName + ".prefab", block, ReplacePrefabOptions.ReplaceNameBased);

				}

				if (block.transform.localScale != Vector3.one)
				{
					if (!scaleErrors.Contains(modelName))
						scaleErrors.Add(modelName);
				}
				if (block.transform.localEulerAngles != Vector3.zero)
				{
					if (!angleErrors.Contains(modelName))
						angleErrors.Add(modelName);
				}

				if (!isTemp)
				{
					GameObject.DestroyImmediate(block);
				}

				return block;
			}
		}

		private static void CreatePrefabsWithDataGroup(List<BlockDataGroup> blockDataGroups, Category category, PolygonType polygonType)
		{
			string prefabDir = BlockPath.Prefab(category, polygonType);
			string fbxDir = BlockPath.Fbx(category, polygonType);
			string matDir = BlockPath.Material(category, polygonType);
			
			List<string> fbxErrors = new List<string>();
			List<string> scaleErrors = new List<string>();
			List<string> angleErrors = new List<string>();
			List<string> materialErrors = new List<string>();
			
			for (int i = 0; i < blockDataGroups.Count; i++)
			{
				EditorUtility.DisplayProgressBar(string.Format("生成 {0} Prefabs", polygonType.ToString()), "正在生成，请稍后", i * 1.0f / blockDataGroups.Count);

				BlockDataGroup blockDataGroup = blockDataGroups[i];

				foreach (var dataItem in blockDataGroup.GetItems())
				{
					GameObject t = AssetDatabase.LoadAssetAtPath<GameObject>(fbxDir + "/" + blockDataGroup.modelName + ".fbx");

					if (t == null)
					{
						if (!fbxErrors.Contains(blockDataGroup.modelName))
							fbxErrors.Add(blockDataGroup.modelName);
						continue;
					}

					GameObject block = GameObject.Instantiate(t);
					block.name = blockDataGroup.modelName;

					Animator animator = block.GetComponent<Animator>();
					if (animator != null)
					{
						GameObject.DestroyImmediate(animator);
					}

					if (polygonType == PolygonType.LOW)
					{
						AssignMaterials(block, dataItem.material, matDir, materialErrors);
					}
					else
					{
						AssignMaterials(block, dataItem.materialHigh, matDir, materialErrors);
						HighPolygonGenRefpoints(block, category);
					}

					if (!Directory.Exists(prefabDir))
					{
						Directory.CreateDirectory(prefabDir);
					}

					PrefabUtility.CreatePrefab(prefabDir + "/" + dataItem.prefabName + ".prefab", block, ReplacePrefabOptions.ReplaceNameBased);

					if (block.transform.localScale != Vector3.one)
					{
						if (!scaleErrors.Contains(blockDataGroup.modelName))
							scaleErrors.Add(blockDataGroup.modelName);
					}
					if (block.transform.localEulerAngles != Vector3.zero)
					{
						if (!angleErrors.Contains(blockDataGroup.modelName))
							angleErrors.Add(blockDataGroup.modelName);
					}
					GameObject.DestroyImmediate(block);
				}
			}
			EditorUtility.ClearProgressBar();
			if (fbxErrors.Count > 0)
				DisplayErrorDialog(polygonType.ToString() + " 找不到以下fbx\n", fbxErrors);
			if (scaleErrors.Count > 0)
				DisplayErrorDialog("以下fbx的默认scale不为1\n", scaleErrors);
			if (angleErrors.Count > 0)
				DisplayErrorDialog("以下fbx的默认角度不为0\n", angleErrors);
			if (materialErrors.Count > 0)
				DisplayErrorDialog("以下材质球没找到，请找何屹、唐蜜Check数据库文件\n", materialErrors);
			
			EditorUtility.DisplayDialog("提示", string.Format("生成 {0} Prefab完成!", polygonType.ToString()), "确定");
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
		
		enum MaterialType
		{
			Default = 0,
			SingleMeshMultiMat = 1,
		}

		private static void AssignMaterials(GameObject block, string materialLabel, string materialDir, List<string> missingMaterials)
		{
			MaterialType matType = MaterialType.Default;
			Dictionary<string, string> singleMaterials = new Dictionary<string, string>();
			List<string> multiMaterials = new List<string>();

			if (materialLabel.IndexOf(":") < 0)
			{
				Renderer renderer = block.GetComponentInChildren<Renderer>(true);
				singleMaterials.Add(renderer.gameObject.name, materialLabel);
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
							singleMaterials.Add(element[0], element[1]);
						else
							multiMaterials.Add(element[1]);
					}
				}
			}

			if (singleMaterials.Count > 0)
				AssignSingleMaterials(block, singleMaterials, materialDir, missingMaterials);
			if (multiMaterials.Count > 0)
				AssignMultiMaterials(block, multiMaterials, materialDir, missingMaterials);
		}

		private static void AssignSingleMaterials(GameObject block, Dictionary<string, string> materials, string materialDir, List<string> missingMaterials)
		{
			Renderer[] renders = block.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < renders.Length; i++)
			{
				Renderer render = renders[i];
				render.receiveShadows = false;
				render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				string materialStr;
				if (!materials.TryGetValue(render.gameObject.name, out materialStr))
					continue;
				
				Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialDir + "/" + materialStr + ".mat");
				if (mat == null)
					mat = AssetDatabase.LoadAssetAtPath<Material>(BlockPath.MaterialCommon() + "/" + materialStr + ".mat");

				if (mat == null)
				{
					missingMaterials.Add(materialStr);
					continue;
				}
				render.sharedMaterial = mat;
			}
		}

		private static void AssignMultiMaterials(GameObject block, List<string> materials, string materialDir, List<string> missingMaterials)
		{
			Renderer render = block.GetComponent<Renderer>();
			render.receiveShadows = false;
			render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			int index = 0;
			Material[] mats = render.sharedMaterials;
			foreach (var materialStr in materials)
			{
				if (mats.Length <= index)
					break;

				Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialDir + "/" + materialStr + ".mat");
				if (mat == null)
					mat = AssetDatabase.LoadAssetAtPath<Material>(BlockPath.MaterialCommon() + "/" + materialStr + ".mat");

				if (mat == null)
				{
					missingMaterials.Add(materialStr);
					continue;
				}

				mats[index] = mat;
				index++;
			}
			render.sharedMaterials = mats;
		}

		private static void HighPolygonGenRefpoints(GameObject block, Category category)
		{
			string fbxDir = BlockPath.Fbx(category, PolygonType.LOW);
			GameObject t = AssetDatabase.LoadAssetAtPath<GameObject>(fbxDir + "/" + block.name + ".fbx");
			RecursiveGenRefpoints(block.transform, t.transform);
		}

		private static void RecursiveGenRefpoints(Transform highTrans, Transform lowTrans)
		{
			foreach (Transform child in lowTrans)
			{
				Transform highChild = highTrans.Find(child.name);
				if (child.name.StartsWith("ref_"))
				{
					if (highChild == null)
					{
						GameObject newChild = new GameObject(child.name);
						highChild = newChild.transform;
						highChild.SetParent(highTrans, false);
					}
					highChild.localPosition = child.localPosition;
					highChild.localRotation = child.localRotation;
					highChild.localScale = child.localScale;
				}
				else
				{
					if (highChild != null)
					{
						RecursiveGenRefpoints(highChild, child);
					}
				}
			}
		}
	
	
		public static void CreateStickerPrefab(string prefabName)
		{
			List<StickerData> datas = PBDataBaseManager.Instance.GetStickerDatas();


			StickerData data = datas.FirstOrDefault(s => s.prefab == prefabName);

			if (data == null)
			{
				Debug.LogError("failed:CreateStickerPrefab>>>"+prefabName);
				return;
			}

			try
			{
				GameObject fbx =
					AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(BlockPath.Sticker_Fbx_Dir,
						data.model + ".fbx"));


				string matPath = Path.Combine(BlockPath.Sticker_Material_Dir, data.prefab + ".mat");
				Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
				if (mat == null)
				{
					mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(BlockPath.Sticker_Material_Dir,
						data.material + ".mat"));


					AssetDatabase.CreateAsset(GameObject.Instantiate(mat), matPath);
					mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
				}

				Texture2D tex =
					AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(BlockPath.Sticker_Texture_Dir,
						data.texture + ".png"));


				mat.mainTexture = tex;

				GameObject go = GameObject.Instantiate(fbx);
				go.name = data.prefab;

				Animator animator = go.GetComponent<Animator>();
				if (animator != null)
					GameObject.DestroyImmediate(animator);

				Renderer render = go.GetComponent<Renderer>();
				render.receiveShadows = false;
				render.shadowCastingMode = ShadowCastingMode.Off;
				render.sharedMaterials = new[] {mat};

				if (!Directory.Exists(BlockPath.Sticker_Prefab_Dir))
				{
					Directory.CreateDirectory(BlockPath.Sticker_Prefab_Dir);
				}

				PrefabUtility.CreatePrefab(BlockPath.Sticker_Prefab_Dir + "/" + data.prefab + ".prefab", go,
					ReplacePrefabOptions.ReplaceNameBased);
				GameObject.DestroyImmediate(go);

			}
			finally
			{
//				EditorUtility.ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}
	
	

		public static void CreateTexturePrefab(string prefabName)
		{

			List<PBTextureData> datas = PBDataBaseManager.Instance.GetTextureDatas();

			PBTextureData data = datas.FirstOrDefault(s => s.prefab == prefabName);
			
			try
			{
				
				GameObject fbx =
					AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(BlockPath.Texture_Fbx_Dir,
						data.model + ".fbx"));


				string matPath = Path.Combine(BlockPath.Texture_Material_Dir, data.prefab + ".mat");
				Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
				if (mat == null)
				{
					mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(BlockPath.Texture_Material_Dir,
						data.material + ".mat"));

					AssetDatabase.CreateAsset(GameObject.Instantiate(mat), matPath);
					mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
				}

				Texture2D tex =
					AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(BlockPath.Texture_Texture_Dir,
						data.texture + ".png"));


				mat.mainTexture = tex;

				GameObject go = GameObject.Instantiate(fbx);
				go.name = data.prefab;

				Animator animator = go.GetComponent<Animator>();
				if (animator != null)
					GameObject.DestroyImmediate(animator);

				Renderer render = go.GetComponent<Renderer>();
				render.receiveShadows = false;
				render.shadowCastingMode = ShadowCastingMode.Off;
				render.sharedMaterials = new[] {mat};

				if (!Directory.Exists(BlockPath.Texture_Prefab_Dir))
				{
					Directory.CreateDirectory(BlockPath.Texture_Prefab_Dir);
				}
				PrefabUtility.CreatePrefab(BlockPath.Texture_Prefab_Dir + "/" + data.prefab + ".prefab", go,
					ReplacePrefabOptions.ReplaceNameBased);
				GameObject.DestroyImmediate(go);

			}
			finally
			{
				AssetDatabase.Refresh();
			}
		}
}

#endif

