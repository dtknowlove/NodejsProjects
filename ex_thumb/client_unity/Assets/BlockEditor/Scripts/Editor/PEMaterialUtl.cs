using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PTGame.Core;
using UnityEditor;
using UnityEngine;

public class PEMaterialUtl : MonoBehaviour {


	//测试
	public static void CreateMat()
	{
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Renderer rend = cube.GetComponent<Renderer> ();
		rend.material = new Material(Shader.Find("Standard"));

		var content =File.ReadAllText("materialInfo.txt");
		var materialInfo = JsonUtility.FromJson<MaterialInfo>(content);
		foreach (var item in materialInfo.propsVector4)
		{
			rend.material.SetVector(item.name,item.data);
		}
		foreach (var item in materialInfo.propsFloat)
		{
			Debug.LogError(item.name+" >>>1>>>"+item.data);
			rend.material.SetFloat(item.name,item.data);
		}
		
	}

	[MenuItem("Assets/PutaoTool/Material2Config", false)]
	public static void ExportFbx2GLTF()
	{
		MaterialInfos infos = new MaterialInfos(){materialInfos = new List<MaterialInfo>()};

		var matFiles = GetMatFiles();
		var t = Environment.CurrentDirectory + "/";
		foreach (var item in matFiles)
		{
			var path = item.Replace(t, "");
			var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
			
			var materialInfo = GetCertainMaterialTexturePaths(mat);
			infos.materialInfos.Add(materialInfo);
		}
		var jsonContent = JsonUtility.ToJson(infos, true);
		File.WriteAllText("Assets/BlockData/database_models_material.txt",jsonContent);
		
		EditorUtility.DisplayDialog("生成所有通用材质配置", "完成", "确定");
	}
	
	private static List<string> matDir=new List<string>()
	{
		"/BlockRes/CommonRes/Block_Materials",
		"/BlockRes/Stickers/Sticker_Materials",
		"/BlockRes/Textures/Texture_Materials",
	};

	private static List<string> GetMatFiles()
	{
		var reusult = new List<string>();
		foreach (var dir in matDir)
		{
			var path = Application.dataPath + dir;
			string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			reusult.AddRange(files.Where(s => Path.GetExtension(s).ToLower() == ".mat"));
		}
		return reusult;
	}

	public static MaterialInfos LoadMaterialConfig()
	{
		var content =File.ReadAllText("Assets/BlockData/database_models_material.txt");
		var materialInfos = JsonUtility.FromJson<MaterialInfos>(content);
		return materialInfos;
	}
	
	public class PropertyType
	{
		public string Color;//Vector4 
		public string Vector;//Vector4
		public string Float;//float
		public string Range;//float
		public string Texture;//Texture
	}
	
	[System.Serializable]
	public class PropInfo
	{
		public string type;
		public string name;
		
	}
	[System.Serializable]
	public class PropInfoVector4 :PropInfo
	{
		public Vector4 data;
	}

	[System.Serializable]
	public class PropInfoFloat :PropInfo
	{
		public float data;
	}
	
	[System.Serializable]
	public class PropInfoTexture : PropInfo
	{
		public string data;
	}



	[System.Serializable]
	public class MaterialInfo
	{
		public string name;
		public string shaderName;
		public List<PropInfoVector4> propsVector4;
		public List<PropInfoFloat> propsFloat;
		public List<PropInfoTexture> propsTex;
	}

	[System.Serializable]
	public class MaterialInfos
	{
		public List<MaterialInfo> materialInfos;
	}

	static MaterialInfo GetCertainMaterialTexturePaths(Material _mat)
	{
		List<string > results = new List<string >();
		Shader shader = _mat.shader;
		
		MaterialInfo  materialInfo = new MaterialInfo(){shaderName = shader.name,name = _mat.name};
		materialInfo.propsFloat = new List<PropInfoFloat>();
		materialInfo.propsVector4 = new List<PropInfoVector4>();
		materialInfo.propsTex = new List<PropInfoTexture>();

		for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); ++i)
		{
			var propName = ShaderUtil.GetPropertyName(shader, i);
			var propType = ShaderUtil.GetPropertyType(shader, i);
			
			if (propType == ShaderUtil.ShaderPropertyType.Color || propType == ShaderUtil.ShaderPropertyType.Vector)
			{
				materialInfo.propsVector4.Add(new PropInfoVector4()
					{
						name = propName,
						type= propType.ToString(),
						data = _mat.GetVector(propName)
					}
				);
			}else if (propType == ShaderUtil.ShaderPropertyType.Float || propType == ShaderUtil.ShaderPropertyType.Range)
			{
				
				materialInfo.propsFloat.Add(new PropInfoFloat()
					{
						name = propName,
						type= propType.ToString(),
						data = _mat.GetFloat(propName)
					}
				);
			}else if(propType == ShaderUtil.ShaderPropertyType.TexEnv)
			{
				Texture tex = _mat.GetTexture(propName);
				
				if (tex)
				{
					materialInfo.propsTex.Add(new PropInfoTexture()
						{
							name = propName,
							type= propType.ToString(),
							data = AssetDatabase.GetAssetPath(tex).Substring(7).ToLower()
						}
					);
				}
			}
		}
		
		
		
		return materialInfo;
	}
}
