/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;
using System.Collections;
using PTGame;
//using PTGame.HotUpdate;
using PTGame.Core;
using PTGame.ModuleUpdate;


public class TestModuleResUpdate:MonoBehaviour 
	{
		
		private void Start()
		{	
			PTModuleUpdate.Instance.StartModuleUpdate("","",null,OnFinishedOneItem,OnUpdateFinish,OnDownloadProgress);
		}

	 	void OnGUI()
		{
			GUI.Label(new Rect(100,100,100,50), PTGame.PTUGame.Instance.Runtime.ToString());
		}
		
		private void OnDownloadProgress(float finishedSize, float totalSize)
		{
			Debug.LogError(finishedSize+" >>>> "+totalSize);
		}

		private void OnUpdateFinish(UpdateStatus obj,string msg,ResUpdateData resUpdateData)
		{
			Debug.Log(obj+">>>>>>>>>>>>"+msg);
		}
//
		private void OnFinishedOneItem(int finishedCount, int totalCount)
		{	
			Debug.LogError("已完成个数:"+finishedCount+"  已完成字节数："+totalCount);
		}
	}
  
