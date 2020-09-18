using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class PTBlockResLoader {

	public static PTBlockResLoader m_instance;

	public static PTBlockResLoader Instance
	{
		get
		{ 
		
			if(m_instance == null)
			{
				Debug.Log ("Init>>>>>>");
				m_instance = new PTBlockResLoader ();
			}
			return m_instance;
		}
	}

	private Dictionary<string,GameObject> highBlocks;

	public GameObject LoadHighBlock(string blockName)
	{
	
		if(highBlocks == null)
		{
			highBlocks = new Dictionary<string,GameObject> ();
		}
		if(highBlocks.ContainsKey(blockName)){

			return GameObject.Instantiate (highBlocks[blockName]);
		}

		string platform = EditorUserBuildSettings.activeBuildTarget.ToString ().ToLower ();


		if (abm == null) {

			AssetBundle ab = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/highblocks_"+platform+"/category_1/highblocks_"+platform);

			abm = ab.LoadAsset ("AssetBundleManifest") as AssetBundleManifest;

			ab.Unload (false);

		} 


		string[] dependcies = abm.GetAllDependencies ("category_1/block_prefabs/"+blockName);

		List<AssetBundle> subabs = new List<AssetBundle> ();

		for(int i=0;i<dependcies.Length;i++)
		{

			AssetBundle ab = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/highblocks_"+platform+"/"+dependcies[i]);

			subabs.Add (ab);

		}

		AssetBundle abblock = AssetBundle.LoadFromFile (Application.streamingAssetsPath+"/highblocks_"+platform+"/category_1/block_prefabs/"+blockName);

		GameObject t = abblock.LoadAsset (blockName) as GameObject;


		highBlocks.Add (blockName,t);

		foreach( var b in subabs){

			b.Unload (false);
		}


		abblock.Unload (false);


		return GameObject.Instantiate (t);
	}


	private  Dictionary<string,Sprite> thumbs;
	private Dictionary<string,GameObject> blocks;

	private AssetBundleManifest abm;

	private Dictionary<string,Texture2D> textures;





	public GameObject LoadBlock(string blockName)
	{
		
		if(blocks == null)
		{
			blocks = new Dictionary<string,GameObject> ();
		}

		if(blocks.ContainsKey(blockName)){

			return GameObject.Instantiate (blocks[blockName]);
		}

		string platform = EditorUserBuildSettings.activeBuildTarget.ToString ().ToLower ();

		if (abm == null) {

			AssetBundle ab = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/blocks_"+platform+"/category_1/blocks_"+platform);

			abm = ab.LoadAsset ("AssetBundleManifest") as AssetBundleManifest;

			ab.Unload (false);

		} 
		
		string[] dependcies = abm.GetAllDependencies ("category_1/block_prefabs/"+blockName);

		List<AssetBundle> subabs = new List<AssetBundle> ();

		for(int i=0;i<dependcies.Length;i++)
		{

			AssetBundle ab = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/blocks_"+platform+"/"+dependcies[i]);

			subabs.Add (ab);

		}

		AssetBundle abblock = AssetBundle.LoadFromFile (Application.streamingAssetsPath+"/blocks_"+platform+"/category_1/block_prefabs/"+blockName);

		GameObject t = abblock.LoadAsset (blockName) as GameObject;


		blocks.Add (blockName,t);


		foreach( var b in subabs){
				
			b.Unload (false);
		}

		abblock.Unload (false);


		return GameObject.Instantiate (t);

	}

	public Texture2D LoadTexture2D(string spriteName)
	{

		if(textures==null)
		{
			textures = new Dictionary<string, Texture2D> ();
		}

		if(textures.ContainsKey(spriteName)){

			return textures[spriteName];
		}

		string platform = EditorUserBuildSettings.activeBuildTarget.ToString ().ToLower ();

		string abPath = Application.streamingAssetsPath + "/blocks_" + platform + "/category_1/block_thumbs/" + spriteName;
	
		AssetBundle ab = AssetBundle.LoadFromFile (abPath);

		Texture2D s = ab.LoadAsset<Texture2D>(spriteName) as Texture2D;

		textures.Add (spriteName, s);

		ab.Unload (false);

		return s;

	}

	public Sprite LoadThumb(string spriteName)
	{
		
		if(thumbs==null)
		{
			thumbs = new Dictionary<string, Sprite> ();
		}

		if(thumbs.ContainsKey(spriteName)){

			return thumbs[spriteName];
		}

		string platform = EditorUserBuildSettings.activeBuildTarget.ToString ().ToLower ();

		string sPath = Application.streamingAssetsPath + "/blocks_" + platform + "/category_1/block_thumbs/" + spriteName;

		AssetBundle ab = AssetBundle.LoadFromFile (sPath);

		Sprite s = ab.LoadAsset<Sprite>(spriteName) as Sprite;

		thumbs.Add (spriteName, s);

		ab.Unload (false);

		return s;

	}
}
