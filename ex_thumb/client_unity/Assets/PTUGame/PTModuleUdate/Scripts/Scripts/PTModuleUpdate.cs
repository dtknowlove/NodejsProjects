/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;
using PTGame.Core;
using System;
using Application = UnityEngine.Application;

namespace PTGame.ModuleUpdate
{
	public enum UpdateStatus
	{
		Success,
		Failed,
		ReadyToDown,
		FinishOneItem,
		DownloadProgress
	}

	[RequireComponent(typeof(PTModuleResDown))]
	public class PTModuleUpdate : PTMonoSingleton<PTModuleUpdate>
	{	
		private PTModuleResDown mResDown;
		
		private Action<int,float> mOnReadyToDownload;

		private Action<int, int> mOnFinishedOneItem;

		private Action<float, float> mOnDownloadProgress;

		private Action<UpdateStatus,string,ResUpdateData> mOnUpdateFinish;

		public void Reset()
		{
			if (mResDown != null)
			{
				this.mResDown.StopAllDownload();
			}

			this.mOnReadyToDownload = null;
			this.mOnFinishedOneItem = null;
			this.mOnUpdateFinish = null;
			this.mOnDownloadProgress = null;
		}

		/// <summary>
		/// res config 配置文件 和 资源文件 位于 同一目录下时使用该方法
		/// </summary>
		/// <param name="remoteConfigPath"></param>
		/// <param name="localConfigPath"></param>
		/// <param name="readyToDownload"></param>
		/// <param name="finishedOneItem"></param>
		/// <param name="updateFinish"></param>
		/// <param name="onDownloadProgress"></param>
		/// <param name="checkVersion">是否判断res config的版本号，false:忽略版本号检测</param>
		/// <param name="checkItemExist">文件如果存在就不下载，不存在的时候再下载</param>
		public void StartModuleUpdate(string remoteConfigPath,string localConfigPath,Action<int,float> readyToDownload,
			Action<int,int> finishedOneItem,Action<UpdateStatus,string,ResUpdateData> updateFinish,Action<float,float> onDownloadProgress,
			bool checkVersion = true,bool checkItemExist = false)
		{
			this.gameObject.AddSingleComponent<PTModuleUpdateDispatcher>();
			mResDown =  this.gameObject.AddSingleComponent<PTModuleResDown>();
			
			Reset();
			
			this.mOnReadyToDownload = readyToDownload;
			this.mOnFinishedOneItem = finishedOneItem;
			this.mOnUpdateFinish = updateFinish;
			this.mOnDownloadProgress = onDownloadProgress;
	
			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				OnResUpdateCallback(new ResUpdateData(){status = UpdateStatus.Failed,msg = "没有网络"});
				return;
			}
			
			mResDown.StartResUpdate(remoteConfigPath,localConfigPath,OnResUpdateCallback,checkVersion,checkItemExist);
		}
		
		/// <summary>
		///  res config 配置文件 和 资源文件不在同一目录下时使用该方法
		/// </summary>
		/// <param name="remoteConfigPath"></param>
		/// <param name="localConfigPath"></param>
		/// <param name="remoteResPath"></param>
		/// <param name="localResPath"></param>
		/// <param name="readyToDownload"></param>
		/// <param name="finishedOneItem"></param>
		/// <param name="updateFinish"></param>
		/// <param name="onDownloadProgress"></param>
		/// <param name="checkVersion">是否判断res config的版本号;false:忽略版本号对比;true:进行版本号对比</param>
		/// <param name="checkItemExist">文件如果存在就不下载，不存在的时候再下载</param>
		public void StartModuleUpdate(string remoteConfigPath,string localConfigPath,string remoteResPath,string localResPath,
			Action<int,float> readyToDownload,Action<int,int> finishedOneItem,Action<UpdateStatus,string,ResUpdateData> updateFinish,
			Action<float,float> onDownloadProgress,bool checkVersion,bool checkItemExist)
		{
			this.gameObject.AddSingleComponent<PTModuleUpdateDispatcher>();
			mResDown =  this.gameObject.AddSingleComponent<PTModuleResDown>();
			
			Reset();
			
			this.mOnReadyToDownload = readyToDownload;
			this.mOnFinishedOneItem = finishedOneItem;
			this.mOnUpdateFinish = updateFinish;
			this.mOnDownloadProgress = onDownloadProgress;
	

			if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				OnResUpdateCallback(new ResUpdateData(){status = UpdateStatus.Failed,msg = "没有网络"});
				return;
			}
			
			mResDown.StartResUpdate(remoteConfigPath,localConfigPath,remoteResPath,localResPath,OnResUpdateCallback,checkVersion,checkItemExist);
		}


		public void StartDownloadRes(int threadNum = 1)
		{
			mResDown.StartDownLoadRes(threadNum);
		}

		private void OnResUpdateCallback(ResUpdateData resUpdateData)
		{
			switch (resUpdateData.status)
			{
				case UpdateStatus.FinishOneItem:
					mOnFinishedOneItem.InvokeGracefully(resUpdateData.finishedCount, resUpdateData.totalCount);
					break;
				case UpdateStatus.ReadyToDown:
					mOnReadyToDownload.InvokeGracefully(resUpdateData.totalCount, resUpdateData.totalSize);
					if (mOnReadyToDownload == null)
					{
						mResDown.StartDownLoadRes();
					}
					break;
				case UpdateStatus.DownloadProgress:
					mOnDownloadProgress.InvokeGracefully(resUpdateData.finishedSize, resUpdateData.totalSize);
					break;
				default:
					mOnUpdateFinish.InvokeGracefully(resUpdateData.status,resUpdateData.msg,resUpdateData);
					break;
			}
		}
	}
}