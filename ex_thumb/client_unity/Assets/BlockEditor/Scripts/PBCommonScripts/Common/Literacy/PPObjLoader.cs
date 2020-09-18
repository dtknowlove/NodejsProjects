/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PTGame.Core;
#if BLOCK_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PPObjLoader:MonoBehaviour
{

	private static Transform mRootTrans = null;
	private static Transform RootObj
	{
		get { return mRootTrans ?? (mRootTrans = new GameObject("[PPObjLoaderRoot]").transform); }
	}
	
	/// <summary>
	/// 外部调用，生成单个零件
	/// </summary>
	public static PPObjLoader LoadBlock(PPPrefabInfo prefabInfo,PPBlockInfo nodeInfo, Transform parent, Action<GameObject> onFinishWithObj)
	{
		GameObject obj = new GameObject("PPObjLoader");
		obj.transform.SetParent(RootObj);
		PPObjLoader loader = obj.AddComponent<PPObjLoader>();
		loader.mPrefabInfo = prefabInfo;
		loader.nodeInfo = nodeInfo;
		loader.parent = parent;
		loader.onFinishWithObj = onFinishWithObj;
		loader.StartCoroutine(loader.AsyncLoadBlock());
		return loader;
	}
	
	public static PPObjLoader LoadBlocks(PPBlockConfigInfo blockConfigInfo, Transform parent, Dictionary<int, GameObject> animNodes, Action onFinish = null)
	{
		return LoadBlocks(blockConfigInfo, parent, animNodes, new LoadConfig(), onFinish);
	}
	public static PPObjLoader LoadBlocks(PPBlockConfigInfo blockConfigInfo, Transform parent, Dictionary<int, GameObject> animNodes,LoadConfig loadConfig, Action onFinish = null)
	{
		GameObject obj = new GameObject("PPObjLoader");
		obj.transform.SetParent(RootObj);
		PPObjLoader loader = obj.AddComponent<PPObjLoader>();
		loader.mBlockConfigInfo = blockConfigInfo;
		loader.sectionInfo = loader.mBlockConfigInfo.SectionInfo;
		loader.parent = parent;
		loader.mAnimNodes = animNodes;
		loader.loadConfig = loadConfig;
		loader.onFinish = onFinish;
		loader.StartCoroutine(loader.AsyncLoadBlocks());
		return loader;
	}

	public static void ReleaseAll()
	{
		var loaders = RootObj.GetComponentsInChildren<PPObjLoader>();
		loaders.ForEach(t => t?.Dispose());
	}
	
	public void Dispose()
	{
		mIsStop = true;
		if (mPPLoader != null)
		{
			mPPLoader.Dispose();
			mPPLoader = null;
		}
		StopAllCoroutines();
		Destroy(gameObject);
	}

	private Transform parent;
	//blocks
	private PPBlockConfigInfo mBlockConfigInfo;
	private PPSectionInfo sectionInfo;
	private  Dictionary<int, GameObject> mAnimNodes;
	private LoadConfig loadConfig = new LoadConfig();
	private Action onFinish = null;
	//block
	private PPBlockInfo nodeInfo;
	private Action<GameObject> onFinishWithObj = null;
	private PPPrefabInfo mPrefabInfo = null;
	
	
	private bool mIsStop = false;

	private PolygonType mPolygonType;
	private BlockLoadInfo mBlockLoadInfo;

	private PPBlockLoader mPPLoader;
	private PPBlockLoader PPLoader => mPPLoader ?? (mPPLoader = new PPBlockLoader());

	#region Aysnc Load

	private IEnumerator AsyncLoadBlocks()
	{
		mPolygonType = loadConfig.polygonType;

		//init LoadBlockInfo
		var blockList = OrderLoadList(loadConfig, sectionInfo);
		var actualBlocks = blockList.Where(t => t.nodeInfo.Type == NodeType.Block).ToList();
		var texCount = 0;
		var blockCount = actualBlocks.Count;
		actualBlocks.ForEach(t =>
		{
			texCount += ((PPBlockInfo) t.nodeInfo).Textures.Length;
		});
		mBlockLoadInfo = new BlockLoadInfo()
			.SetBlockSumCount(blockCount)
			.SetTexSumCount(texCount);
		//start load blocks
		if (loadConfig.LoadSpeed == -1)
		{
			InnerLoadBlocks(blockList);
		}
		else
		{
			yield return AsyncInnerLoadBlocks(blockList);
		}
		while (!mIsStop && !mBlockLoadInfo.Finish)
		{
			yield return null;
		}
		//for spline
		foreach (GameObject obj in mAnimNodes.Values)
		{
			if (mIsStop)
				yield break;
			if (PBVersatileInterface.BuildFromInfo(obj))
				yield return null;
		}
		if (mIsStop)
			yield break;
		onFinish?.Invoke();

		mAnimNodes = null;
		mBlockLoadInfo = null;
	}

	private IEnumerator AsyncLoadBlock()
	{
		mBlockLoadInfo = new BlockLoadInfo()
			.SetBlockSumCount(1)
			.SetTexSumCount(nodeInfo.Textures.Length);
		var blockObj = InnerLoadBlock(nodeInfo, parent, mBlockLoadInfo);
		while (!mBlockLoadInfo.Finish)
		{
			yield return null;
		}

		PBVersatileInterface.OnLoadBlockObj(blockObj, nodeInfo.VersatileInfo, false);
		onFinishWithObj?.Invoke(blockObj);
		mBlockLoadInfo = null;
	}

	/// <summary>
	/// 不限速load blocks
	/// </summary>
	private void InnerLoadBlocks(List<LoadNodeInfo> blockList)
    {
	    foreach (LoadNodeInfo info in blockList)
	    {
		    if (info.nodeInfo.Type == NodeType.Section)
		    {
			    InnerLoadSection((PPSectionInfo) info.nodeInfo, info.parentID == -1 ? parent : mAnimNodes[info.parentID].transform);
		    }
		    else
		    {
			    InnerLoadBlock((PPBlockInfo) info.nodeInfo, info.parentID == -1 ? parent : mAnimNodes[info.parentID].transform, mBlockLoadInfo);
		    }
	    }
    }

	/// <summary>
	/// 限速 load blocks
	/// </summary>
	private IEnumerator AsyncInnerLoadBlocks(List<LoadNodeInfo> blockList)
	{
		int count = 0;
		foreach (LoadNodeInfo info in blockList)
		{
			if (mIsStop)
				yield break;
			if (info.nodeInfo.Type == NodeType.Section)
			{
				InnerLoadSection((PPSectionInfo) info.nodeInfo, info.parentID == -1 ? parent : mAnimNodes[info.parentID].transform);
			}
			else
			{
				InnerLoadBlock((PPBlockInfo) info.nodeInfo, info.parentID == -1 ? parent : mAnimNodes[info.parentID].transform, mBlockLoadInfo);
				if (++count % loadConfig.LoadSpeed == 0)
					yield return null;
			}
		}
	}
	
	#endregion
	
	private GameObject InnerLoadSection(PPSectionInfo nodeInfo, Transform parent)
    {
        GameObject section = new GameObject(nodeInfo.Name);
        section.transform.SetParent(parent, false);

        PBSection pbsection = section.AddComponent<PBSection>();
        pbsection.animNodeInfo = nodeInfo;
        pbsection.Init();

        mAnimNodes.Add(nodeInfo.Id, section);
        return section;
    }

	private GameObject InnerLoadBlock(PPBlockInfo nodeInfo, Transform parent,BlockLoadInfo blockLoadInfo)
	{
		if (mIsStop)
			return null;
		string prefabName = nodeInfo.Prefab;
		PPTextureInfo[] textures = nodeInfo.Textures;

		GameObject gameObj;
		GameObject[] texObjs = new GameObject[textures.Length];
        
#if BLOCK_EDITOR
	    gameObj = PEBlockLoader.CreateBlock(prefabName, (PolygonType) mPolygonType);
	    for (int tIndex = 0; tIndex < textures.Length; tIndex++)
	    {
		    texObjs[tIndex] = PEBlockLoader.CreateTexture(textures[tIndex].Prefab);
	    }
#else
		//Hide block 还是要创建出来，可能以后搭建中会用到该block的信息，它是作为搭建辅助件用的
		var prefabInfo = GetPrefabInfoByName(prefabName);
		gameObj = nodeInfo.Hide ? new GameObject() : PPLoader.CreateBlock(prefabInfo, () => blockLoadInfo.AllocateBlock());
		for (var tIndex = 0; tIndex < textures.Length; tIndex++)
		{
			texObjs[tIndex] = PPLoader.CreateTexture(prefabInfo.GetPrefabTexInfo(textures[tIndex].Prefab), () => blockLoadInfo.AllocateTexture());
		}
#endif

		gameObj.name = prefabName;
		gameObj.transform.SetParent(parent, false);
		PBBlock pbblock = gameObj.AddComponent<PBBlock>();
		pbblock.animNodeInfo = nodeInfo;
		pbblock.Init();
        
		//for texture
		for (int tIndex = 0; tIndex < texObjs.Length; tIndex++)
		{
			GameObject texObj = texObjs[tIndex];
			texObj.name = textures[tIndex].Prefab;
			texObj.transform.SetParent(gameObj.transform, false);
			PBTexture pbTex = texObj.AddComponent<PBTexture>();
			pbTex.info = textures[tIndex];
			pbTex.Init();
		}
        
		//for splines
		PBVersatileInterface.OnLoadBlockObj(gameObj, nodeInfo.VersatileInfo, true);

		if (mAnimNodes != null)
			mAnimNodes.Add(nodeInfo.Id, gameObj);
		return gameObj;
	}

	private PPPrefabInfo GetPrefabInfoByName(string name)
	{
		if (mBlockConfigInfo != null)
			return mBlockConfigInfo.GetPrefabInfoByName(name);
		if (mPrefabInfo != null)
			return mPrefabInfo;
		throw new NullReferenceException("[PPPrefabInfo] or [PPBlockConfigInfo] must afferented one!");
	}

	public enum LoadOrder
	{
		Default = 0,
		Keyframe = 1,
	}

	public class LoadConfig
	{
		public PolygonType polygonType = PolygonType.LOW;
		public LoadOrder loadOrder = LoadOrder.Default;
		public List<PPKeyFrameInfo> keyframeInfos;

		/// <summary>
		/// -1为不限速
		/// </summary>
		public int LoadSpeed = 5;
	}

	public class LoadNodeInfo
	{
		public PPAnimNodeInfo nodeInfo;
		public int parentID;
	}

	private static List<LoadNodeInfo> OrderLoadList(LoadConfig config, PPSectionInfo sectionInfo)
	{
		List<LoadNodeInfo> loadList = new List<LoadNodeInfo>();
		switch (config.loadOrder)
		{
			case LoadOrder.Keyframe:
				if (config.keyframeInfos.Count > 0)
				{
					OrderLoadList_Keyframe(loadList, sectionInfo, -1, config.keyframeInfos);
				}
				else
				{
					OrderLoadList_Default(loadList, sectionInfo, -1);
					Debug.Log("<color=yellow>Config没有搭建动画！</color>");
				}
				break;
			default:
				OrderLoadList_Default(loadList, sectionInfo, -1);
				break;
		}
		return loadList;
	}

	private static void OrderLoadList_Default(List<LoadNodeInfo> loadList, PPSectionInfo sectionInfo, int parentID)
	{
		foreach (PPAnimNodeInfo nodeInfo in sectionInfo.NodeInfos)
		{
			if (nodeInfo.Type == NodeType.Section)
			{
				loadList.Add(new LoadNodeInfo {nodeInfo = nodeInfo, parentID = parentID});
				OrderLoadList_Default(loadList, (PPSectionInfo) nodeInfo, nodeInfo.Id);
			}
			else if (nodeInfo.Type == NodeType.Block)
			{
				loadList.Add(new LoadNodeInfo {nodeInfo = nodeInfo, parentID = parentID});
			}
		}
	}

	private static void OrderLoadList_Keyframe(List<LoadNodeInfo> loadList, PPSectionInfo sectionInfo, int parentID, List<PPKeyFrameInfo> keyframeInfos)
	{
		//1. determine the parent of each node
		//	all section nodes will be added into loadList in advance, because sections will be all created before blocks.
		Dictionary<int, LoadNodeInfo> dict = new Dictionary<int, LoadNodeInfo>();
		PrepareLoadNodeInfos(loadList, dict, sectionInfo, parentID);

		//2. order by keyframes.
		HashSet<int> indexSet = new HashSet<int>();
		foreach (PPKeyFrameInfo keyframe in keyframeInfos)
		{
			foreach (PPFrameItemInfo keyframeItem in keyframe.itemInfos)
			{
				if (!dict.ContainsKey(keyframeItem.targetId))
					continue;

				if (indexSet.Contains(keyframeItem.targetId))
					loadList.RemoveAt(loadList.FindIndex(l => l.nodeInfo.Id == keyframeItem.targetId));

				indexSet.Add(keyframeItem.targetId);
				loadList.Add(dict[keyframeItem.targetId]);
			}
		}
		
		//3. load blocks not in keyframes, and load hide blocks
		dict.Values.Where(info => !indexSet.Contains(info.nodeInfo.Id) || ((PPBlockInfo) info.nodeInfo).Hide).ForEach(loadList.Add);
	}

	private static void PrepareLoadNodeInfos(List<LoadNodeInfo> loadList, Dictionary<int, LoadNodeInfo> blockInfoDict, PPSectionInfo sectionInfo, int parentID)
	{
		foreach (PPAnimNodeInfo nodeInfo in sectionInfo.NodeInfos)
		{
			if (nodeInfo.Type == NodeType.Section)
			{
				loadList.Add(new LoadNodeInfo {nodeInfo = nodeInfo, parentID = parentID});
				PrepareLoadNodeInfos(loadList, blockInfoDict, (PPSectionInfo) nodeInfo, nodeInfo.Id);
			}
			else if (nodeInfo.Type == NodeType.Block)
			{
				blockInfoDict.Add(nodeInfo.Id, new LoadNodeInfo {nodeInfo = nodeInfo, parentID = parentID});
			}
		}
	}

	[Serializable]
	public class BuildingModelConfig
	{
		public string resversion = "100";
		public List<ModeResItem> items;
	}

	[Serializable]
	public class ModeResItem
	{
		public string name;
		public string path;
		public string hash;
		public float size;
	}

	class BlockLoadInfo
	{
		public bool Finish => mIsBlockFinish && mIsTextureFinish;

		private int mBlockCount, mTexCount, mBlockSumCount, mTexSumCount;
		private bool mIsBlockFinish,mIsTextureFinish;

		public BlockLoadInfo()
		{
			Reset();
		}

		public BlockLoadInfo SetBlockSumCount(int count)
		{
			mBlockSumCount = count;
			mIsBlockFinish = mBlockSumCount == 0;
			return this;
		}
		
		public BlockLoadInfo SetTexSumCount(int count)
		{
			mTexSumCount = count;
			mIsTextureFinish = mTexSumCount == 0;
			return this;
		}

		public void AllocateBlock(int count = 1)
		{
			mBlockCount += count;
			if (mBlockCount >= mBlockSumCount)
				mIsBlockFinish = true;
		}

		public void AllocateTexture(int count = 1)
		{
			mTexCount += count;
			if (mTexCount >= mTexSumCount)
				mIsTextureFinish = true;
		}

		public void Reset()
		{
			mBlockCount = 0;
			mTexCount = 0;
			mIsBlockFinish = false;
			mIsTextureFinish = false;
		}
	}
}

