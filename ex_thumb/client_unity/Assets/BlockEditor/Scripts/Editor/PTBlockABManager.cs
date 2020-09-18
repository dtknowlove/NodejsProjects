using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Security.Cryptography;
using System;
using Putao.BlockRes;

public class PTBlockABManager : MonoBehaviour {


	//[MenuItem("BlockTool/ABManager/ClearABNames", false, GlobalDefine.Menu_ABManager)]
	public static void ClearAB()
	{
		var absNames = AssetDatabase.GetAllAssetBundleNames ();
		foreach(string n in absNames)
		{
			
			AssetDatabase.RemoveAssetBundleName (n,true);
		}
		AssetDatabase.Refresh ();
	}

	private static string GetProjectRootPath()
	{
	
		return Application.dataPath.Substring (0,Application.dataPath.LastIndexOf("/Assets"));
	}

	//[MenuItem("BlockTool/ABManager/BuildHighBlocks", false, GlobalDefine.Menu_ABManager)]

	public static void BuildHighBlocks()
	{
		string platform = EditorUserBuildSettings.activeBuildTarget.ToString ().ToLower ();
		BuildHighBlocksCategory (1,platform);
	}
	public static void BuildHighBlocksCategory(int categoryId,string platform)
	{

		string categoryDir = Application.dataPath + "/BlockRes/HighPolygon/Category_"+categoryId;
		string refPath = Application.dataPath + "/BlockRes/HighPolygon/";

		DirectoryInfo prefabDirInfo = new DirectoryInfo (Path.Combine(categoryDir,"Block_Prefabs"));

		var fileInfos = prefabDirInfo.GetFiles ().Where (s=>s.Name.ToLower().EndsWith(".prefab"));

		foreach (FileInfo fileInfo in fileInfos)
		{

			SetABName (fileInfo,refPath);
		}

		DirectoryInfo fbxDirInfo = new DirectoryInfo (Path.Combine(categoryDir,"Block_Fbxs"));
		fileInfos = fbxDirInfo.GetFiles ().Where (s=>s.Name.ToLower().EndsWith(".fbx"));

		foreach (FileInfo fileInfo in fileInfos)
		{
			SetABName (fileInfo,refPath);
		}
			

		string materialsPath = "Assets/BlockRes/HighPolygon/Category_" + categoryId+"/Block_Materials";
		AssetImporter matAI = AssetImporter.GetAtPath (materialsPath);
		matAI.assetBundleName = "Category_" + categoryId + "/Block_Materials";


		string shadersPath = "Assets/BlockRes/HighPolygon/Category_" + categoryId+"/Block_Shaders";
		AssetImporter shaderAI = AssetImporter.GetAtPath (shadersPath);
		shaderAI.assetBundleName = "Category_" + categoryId + "/Block_Shaders";

		SetShareContent (categoryId);


		AssetDatabase.Refresh ();

		string destDir = "highblocks_"+platform;
		string abPath = Path.Combine(Application.streamingAssetsPath,destDir);
		if (!Directory.Exists (abPath)) {

			Directory.CreateDirectory (abPath);
		}
		BuildPipeline.BuildAssetBundles (abPath,BuildAssetBundleOptions.ChunkBasedCompression,EditorUserBuildSettings.activeBuildTarget);


		MoveBundleInfoToCatetory ("highblocks_ios",categoryId);

		GenerateInfoFile (categoryId,"highblocks_ios");

		AssetDatabase.Refresh ();
	}


	private static void SetShareContent(int categoryId)
	{
	    string categoryDir = Application.dataPath + "/BlockRes/BlockModels/Category_"+categoryId;

		string refPath = Application.dataPath + "/BlockRes/BlockModels/";

		DirectoryInfo texturesDirInfo = new DirectoryInfo (Path.Combine(categoryDir,"Block_Textures"));

		var fileInfos = texturesDirInfo.GetFiles ("*",SearchOption.AllDirectories).Where (s=>!s.Name.ToLower().EndsWith(".meta") && 
			!s.Name.EndsWith(".DS_Store"));

		foreach (FileInfo fileInfo in fileInfos)
		{
			SetABName(fileInfo,refPath);
		}




	}

	//[MenuItem("BlockTool/ABManager/BuildBlocks", false, GlobalDefine.Menu_ABManager)]
	public static void BuildBlocks()
	{
		string platform = EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();
		BuildBlockCategroy(1, platform);
	}

	public static void BuildBlockCategroy(int categoryId,string platform)
	{
		string categoryDir = Application.dataPath + "/BlockRes/BlockModels/Category_"+categoryId;


		string refPath = Application.dataPath + "/BlockRes/BlockModels/";


		DirectoryInfo fbxDirInfo = new DirectoryInfo (Path.Combine(categoryDir,"Block_Fbxs"));

		var fileInfos = fbxDirInfo.GetFiles ().Where(s=>s.Name.ToLower().EndsWith(".fbx"));

		foreach (FileInfo fileInfo in fileInfos)
		{
			SetABName (fileInfo,refPath);
		}

		DirectoryInfo prefabDirInfo = new DirectoryInfo (Path.Combine(categoryDir,"Block_Prefabs"));

		fileInfos = prefabDirInfo.GetFiles ().Where (s=>s.Name.ToLower().EndsWith(".prefab"));
		foreach (FileInfo fileInfo in fileInfos)
		{
			SetABName (fileInfo,refPath);
		}


		DirectoryInfo thumbDirInfo = new DirectoryInfo (Path.Combine(categoryDir,"Block_Thumbs"));

		fileInfos = thumbDirInfo.GetFiles ().Where (s=>s.Name.ToLower().EndsWith(".png"));
		foreach (FileInfo fileInfo in fileInfos)
		{
			SetABName (fileInfo,refPath);
		}


		string materialsPath = "Assets/BlockRes/BlockModels/Category_" + categoryId+"/Block_Materials";
		AssetImporter matAI = AssetImporter.GetAtPath (materialsPath);
		matAI.assetBundleName = "Category_" + categoryId + "/Block_Materials";


		string shadersPath = "Assets/BlockRes/BlockModels/Category_" + categoryId+"/Block_Shaders";
		AssetImporter shaderAI = AssetImporter.GetAtPath (shadersPath);
		shaderAI.assetBundleName = "Category_" + categoryId + "/Block_Shaders";

		SetShareContent (categoryId);




		AssetDatabase.Refresh ();

		string destDir = "blocks_"+platform;
		string abPath = Path.Combine(Application.streamingAssetsPath,destDir);

//		string abPath = Application.streamingAssetsPath+"/blocks_ios";
		if (!Directory.Exists (abPath)) {
		
			Directory.CreateDirectory (abPath);
		}
		BuildPipeline.BuildAssetBundles (abPath,BuildAssetBundleOptions.ChunkBasedCompression,EditorUserBuildSettings.activeBuildTarget);

		MoveBundleInfoToCatetory (destDir,categoryId);

		GenerateInfoFile (categoryId,destDir);

		AssetDatabase.Refresh ();
	}

	public enum ResType{
		Block_Thumbs,
		Block_Fbxs,
		Block_Prefabs,
		Block_Textures,
		Texture,
		Shader
	}

	private static void SetABName(FileInfo fileInfo, string refPath)
	{
		int length = GetProjectRootPath ().Length;
		string astPath = Path.GetFullPath (fileInfo.FullName).Substring(length+1);

		AssetImporter asi = AssetImporter.GetAtPath (astPath);

		string  respath = Path.GetFullPath (fileInfo.Directory.FullName);
		#if !UNITY_EDITOR_OSX
		refPath = refPath.Replace ("/","\\");
		#endif
		respath = respath.Replace (refPath,"");

		asi.assetBundleName = respath+"/" + Path.GetFileNameWithoutExtension (fileInfo.Name);
	}


		
	private static void GenerateInfoFile(int categoryId,string dir)
	{
		DirectoryInfo dirInfo = new DirectoryInfo (Application.streamingAssetsPath+"/"+dir+"/category_"+categoryId);
		var fileInfos = dirInfo.GetFiles ("*",SearchOption.AllDirectories).Where(s=>!s.Name.ToLower().EndsWith(".meta") && !s.Name.EndsWith(".DS_Store"));

		ResConfig resConfig=new ResConfig();
		int counter = 0;
		foreach (FileInfo fileInfo in fileInfos) {
		
			ResConfigItem configItem = new ResConfigItem ();

			configItem.index = counter++;

			configItem.name = Path.GetFullPath(fileInfo.FullName).Replace(Application.streamingAssetsPath+"/"+dir+"/","");
			configItem.size = fileInfo.Length.ToString();

			byte[] platformFileBytes = GetFileBytes(Path.GetFullPath(fileInfo.FullName));
			configItem.md5 = GetMD5 (platformFileBytes);

			resConfig.Items.Add (configItem);

		}

		string jsonContent = JsonUtility.ToJson (resConfig,true);

		File.WriteAllText (Application.streamingAssetsPath+"/"+dir+"/category_"+categoryId+"/config.json",jsonContent);
			
	}


	private static void MoveBundleInfoToCatetory(string dir,int categoryId)
	{
//		string destAsbundleFile = Application.streamingAssetsPath +"/"+platform+ "/"+type+"/category_"+categoryId+"/"+type;
//
//		if (File.Exists (destAsbundleFile)) 
//		{
//			File.Delete (destAsbundleFile);
//		}
//
//		string destAsundleManiFile = Application.streamingAssetsPath + "/" + platform +"/"+type+"/category_"+categoryId+"/"+type+".manifest";
//
//		if(File.Exists(destAsundleManiFile))
//		{
//			File.Delete (destAsundleManiFile);
//		}
//
//
//		File.Move (Application.streamingAssetsPath + "/"+platform + "/"+type+"/"+type, destAsbundleFile);
//		File.Move (Application.streamingAssetsPath + "/"+platform +  "/"+type+"/"+type+".manifest",destAsundleManiFile);

		string src = Path.Combine (Application.streamingAssetsPath, dir + "/" + dir);
		string destDir = Application.streamingAssetsPath  + "/" + dir + "/category_" + categoryId+"/"+dir;
		string src1 = Path.Combine (Application.streamingAssetsPath, dir + "/" + dir+".manifest");
		string destDir1 = Application.streamingAssetsPath  + "/" + dir + "/category_" + categoryId+"/"+dir+".manifest";;
		MoveTo (src,destDir);
		MoveTo (src1,destDir1);
	}

	private static void MoveTo(string src,string dest)
	{
		if(File.Exists(dest))
		{
			File.Delete (dest);
		}
		File.Move (src, dest);
	}

	public static string GetMD5(byte[] data)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] result = md5.ComputeHash(data);
		string fileMD5 = "";
		foreach (byte b in result)
		{
			fileMD5 += Convert.ToString(b, 16);
		}
		return fileMD5;   
	}
	public static byte[] GetFileBytes(string filePath)
	{
		FileStream fs = new FileStream(filePath, FileMode.Open);
		int len = (int)fs.Length;
		byte[] data = new byte[len];
		fs.Read(data, 0, len);
		fs.Close();
		return data;
	}
}
