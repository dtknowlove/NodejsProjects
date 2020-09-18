/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PTGame.Core;
using System.Linq;
using System.Net;
using UnityEngine.Networking;
using BestHTTP;

namespace PTGame.ModuleUpdate
{
	public class ResUpdateData
	{
		public UpdateStatus status;
		public float totalSize    = 0;
		public int totalCount     = 0;
		public float finishedSize = 0;
		public int finishedCount  = 0;
		public string msg;
	}

	public class ResConfig
	{
		public List<ResItem> items;
		public string resversion;
		
		public ResConfig()
		{
			items = new List<ResItem>();
			resversion = "0";
		}

		public ResItem FindItemWithPath(string path)
		{
			if ( items ==null ||items.Count == 0 )
			{
				return null;
			}
			return items.FirstOrDefault(s => s.path == path);
		}
	}

	[System.Serializable]
	public class ResItem
	{
		public string name = "";
		public string path = "";
		public string hash = "";
		public int size = 0;
		public string custom ="";

		public int retryTimes = 0;

		public bool EqualToItem(ResItem item)
		{
            if(item.hash.Equals(hash))
            {
	            return true;
            }
			return false;
		}
	}
	
	public class PTModuleResDown : MonoBehaviour
	{
		private Action<ResUpdateData> onUpdateResCallback = null;
		private ResUpdateData mResUpdateData;
		private ResConfig mServerConfig;
		private ResConfig mLocalConfig;
		private Queue<ResItem> mUpdateItems;
		private string mLocalConfigFilePath;
		private string mRemoteResPath;
		private string mLocalResPath;
		private bool   mCheckVersion = true;
		private bool   mCheckItemExist = false;
		private float  mFinishedSize = 0;
		private List<DownloadThread> mDownloadThreads;
		
		private const string TempModuleResDir = "ptgame_moduleres_temp";
		private string mLocalTempDir;
		private bool mIsDownloading = false;
	
		/// <summary>
		///  res config配置文件 和 资源文件 位于 同一目录下时使用该方法
		/// </summary>
		/// <param name="remoteConfigFilePath">远程配置文件路径</param>
		/// <param name="localConfigFilePath">本地配置文件路径</param>
		/// <param name="callback"></param>
		/// <param name="checkVersion">是否判断res config的版本号，false:忽略版本号检测</param>
		/// <param name="checkItemExist">文件如果存在就不下载，不存在的时候再下载</param>
		public void StartResUpdate(string remoteConfigFilePath,string localConfigFilePath,Action<ResUpdateData> callback,
			bool checkVersion = true ,bool checkItemExist = false)
		{
			mCheckVersion = checkVersion;

			mCheckItemExist = checkItemExist;
			
			onUpdateResCallback = callback;

			mRemoteResPath = remoteConfigFilePath.Substring(0, remoteConfigFilePath.LastIndexOf("/"));

			mLocalConfigFilePath = localConfigFilePath;

			mLocalResPath = Path.GetDirectoryName(localConfigFilePath);
			
			mResUpdateData = new ResUpdateData();
			
			mUpdateItems = new Queue<ResItem>();
			
			mLocalTempDir = Application.persistentDataPath + "/" + TempModuleResDir;
			
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
			
			StartCoroutine(PTModuleUpdateUtil.GetTextFromServer(remoteConfigFilePath,3,OnGetServerConfigCallback));

		}

		/// <summary>
		/// res config配置文件 和 资源文件不在同一目录下时使用该方法
		/// </summary>
		/// <param name="remoteConfigFilePath"></param>
		/// <param name="localConfigFilePath"></param>
		/// <param name="remoteResPath"></param>
		/// <param name="localResPath"></param>
		/// <param name="callback"></param>
		/// <param name="checkVersion">是否判断res config的版本号，false:忽略版本号检测</param>
		/// <param name="checkItemExist">文件如果存在就不下载，不存在的时候再下载</param>
		public void StartResUpdate(string remoteConfigFilePath, string localConfigFilePath,string remoteResPath,string localResPath,
			Action<ResUpdateData> callback,bool checkVersion = true, bool checkItemExist = false)
		{
			mCheckVersion = checkVersion;
			
			mCheckItemExist = checkItemExist;
			
			onUpdateResCallback = callback;

			mRemoteResPath = remoteResPath;

			mLocalConfigFilePath = localConfigFilePath;

			mLocalResPath = localResPath;

			mResUpdateData = new ResUpdateData();
			
			mUpdateItems = new Queue<ResItem>();
			
			mLocalTempDir = Application.persistentDataPath + "/" + TempModuleResDir;
			
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
			
			PTDebug.LogWarning("获取远程配置文件："+remoteConfigFilePath);
			StartCoroutine(PTModuleUpdateUtil.GetTextFromServer(remoteConfigFilePath,3,OnGetServerConfigCallback));
		}


		private void OnGetServerConfigCallback(int result,string content)
		{
			if (result == 0)
			{
				try
				{
					mServerConfig = JsonUtility.FromJson<ResConfig>(content);
				}
				catch (Exception e)
				{
					ResUpdateCallback (UpdateStatus.Failed, string.Format("获取远程配置文件失败,错误信息: {0}","解析远程配置文件失败"));
					
					return;
				}

				mLocalConfig = File.Exists(mLocalConfigFilePath) ? JsonUtility.FromJson<ResConfig>(File.ReadAllText(mLocalConfigFilePath)) : new ResConfig();
				
				GetUpdateItemList();
			} 
			else 
			{
				ResUpdateCallback (UpdateStatus.Failed, string.Format("获取远程配置文件失败,错误信息: {0}",content));
			}
		}

		private void GetUpdateItemList()
		{
			if (mCheckVersion)
			{
				if (mLocalConfig.items!=null && int.Parse(mLocalConfig.resversion)>=int.Parse(mServerConfig.resversion))
				{
					ResUpdateCallback (UpdateStatus.Success, "没有新资源:RES_VERSOIN 一致");
				
					return;
				}
			}
			
			int totalSize = 0;
			
			foreach(var item in mServerConfig.items)
			{
				ResItem localItem = mLocalConfig.FindItemWithPath(item.path);
			
				bool needUpdate = localItem == null || !item.EqualToItem(localItem);
				
				if (mCheckItemExist)
				{
					var path = mLocalResPath + "/" + item.path;				
					if (File.Exists(path))
					{
						needUpdate = false;
					}
				}
				
				if (needUpdate)
				{
					totalSize += item.size;
					mUpdateItems.Enqueue(item);
				}
			}
			
			PTDebug.Log(string.Format("{0} items need to be updated >>>>>",mUpdateItems.Count));
			
			if ( mUpdateItems.Count == 0)
			{
				SaveServerConfig();
				
				ResUpdateCallback (UpdateStatus.Success, "没有新资源");
				
				return;
			}
			
			mResUpdateData.totalSize = (totalSize / 1024.0f) / 1024.0f;
			
			mResUpdateData.totalCount =  mUpdateItems.Count;
			
			ResUpdateCallback (UpdateStatus.ReadyToDown, "准备更新");	
		}


		public void StartDownLoadRes(int threadNum = 1)
		{
			if (Directory.Exists(mLocalTempDir))
			{
				Directory.Delete(mLocalTempDir,true);
			}
			mFinishedSize = 0;
			
			StopAllDownload();
			
			mDownloadThreads = new List<DownloadThread>();
			mIsDownloading = true;
			for (int i=0;i<threadNum;i++)
			{
				if (mUpdateItems.Count>0)
				{
					var downloadThread = new DownloadThread(this);
					mDownloadThreads.Add(downloadThread);
					downloadThread.StartDownloadItem(mRemoteResPath,mLocalTempDir,mUpdateItems.Dequeue());
				}
			}
		}


		class DownloadThread
		{
			private PTModuleResDown moduleResDown;

			private bool mIsSuccess = false;
			private bool mIsFailed = false;
			private bool mIsFinished = false;
			public ResItem ResItem;
//			private UnityWebRequest mUnityWebRequest;
			private HTTPRequest mHttpRequest;

			public bool IsFinished
			{
				get { return mIsFinished; }
			}

			public bool IsSuccess
			{
				get { return mIsSuccess; }
			}

			public bool IsFailed
			{
				get { return mIsFailed; }
			}

			public  DownloadThread(PTModuleResDown moduleResDown)
			{
				this.moduleResDown = moduleResDown;
			}

			public void ChangeToDeactive()
			{
				this.mIsFinished = false;
			}

			public void StopDownload()
			{
				try
				{
					if (mHttpRequest != null)
					{
						mHttpRequest.Abort();
						mHttpRequest.Dispose();
					}
				}
				catch (Exception e)
				{
					
				}
			}

			public float GetItemSize()
			{
				if(ResItem == null){
					Debug.LogError("DownloadThread>>> GetItemSize() ResItem is null");
					return 0;
				}
				return  (ResItem.size / 1024.0f) / 1024.0f;
			}
		
			public void StartDownloadItem(string remoteResPath,string localTempDir,ResItem resItem)
			{
				mIsFinished = false;
				mIsFailed = false;
				mIsSuccess = false;
				this.ResItem = resItem;
				
				moduleResDown.StartCoroutine(DownloadFileWebRequest(remoteResPath,localTempDir,resItem, (result, info) =>
				{
					if (result)
					{
						mIsSuccess = true;
						mIsFinished = true;
					}
					else
					{
						if (resItem.retryTimes < 5)
						{
							Debug.LogError("retry >>>>>" + Path.Combine(remoteResPath,resItem.path) + "  >>>> " + resItem.retryTimes);
							resItem.retryTimes++;
							StartDownloadItem(remoteResPath,localTempDir,resItem);
						}
						else
						{
							Debug.LogError("下载出错>>>>>:"+Path.Combine(remoteResPath,resItem.path));
							mIsFailed = true;
							mIsFinished = true;
						}
					}
				}));
			}
			
			private  IEnumerator DownloadFileWebRequest(string remoteResPath,string localTempDir,ResItem resItem,Action<bool, string> callback)
			{	
				string remoteUrl = Path.Combine(remoteResPath,resItem.path);
				string localDestPath = Path.Combine(localTempDir,resItem.path);
				PTDebug.Log(string.Format("***********从{0}下载到{1}：",remoteUrl,localDestPath));

				var fileDir = Path.GetDirectoryName(localDestPath);
			
				if (!Directory.Exists(fileDir)){Directory.CreateDirectory(fileDir);}

				mHttpRequest = new HTTPRequest(new Uri(remoteUrl),
					(req, resp) =>
					{
						if (resp != null && req.Exception  == null)
						{
							File.WriteAllBytes(localDestPath, resp.Data);
							if (mHttpRequest != null)
							{
								mHttpRequest.Abort();
								mHttpRequest.Dispose();
								mHttpRequest = null;
							}
							callback(true, "success");
						}
						else
						{
							var errorInfo = "错误信息:" + req.Exception;
							Debug.LogError(errorInfo);
							if (mHttpRequest != null)
							{
								mHttpRequest.Abort();
								mHttpRequest.Dispose();
								mHttpRequest = null;
							}
							callback(false, errorInfo);
						}
					})
				{
					ConnectTimeout = TimeSpan.FromSeconds(5), Timeout = TimeSpan.FromSeconds(10), DisableCache = true
				};

				mHttpRequest.Send();

				yield return 0;
			}
			
		}
			
			
//			private  IEnumerator DownloadFileWebRequest(string remoteResPath,string localTempDir,ResItem resItem,Action<bool, string> callback)
//			{	
//				string remoteUrl = Path.Combine(remoteResPath,resItem.path);
//				string localDestPath = Path.Combine(localTempDir,resItem.path);
//				PTDebug.Log(string.Format("***********从{0}下载到{1}：",remoteUrl,localDestPath));
//
//				var fileDir = Path.GetDirectoryName(localDestPath);
//			
//				if (!Directory.Exists(fileDir)){Directory.CreateDirectory(fileDir);}
//				
//				mUnityWebRequest = UnityWebRequest.Get(remoteUrl);
//
//				mUnityWebRequest.timeout = 10;
//				mUnityWebRequest.SendWebRequest();
//			
//				float timer = Time.realtimeSinceStartup;
//				float timeOut = 10;
//				bool failed = false;
//
//				while (!mUnityWebRequest.isDone)
//				{
//					if (Time.realtimeSinceStartup - timer > timeOut){failed = true;break;}
//					yield return 0;
//				}
//
//				if (!failed&&string.IsNullOrEmpty(mUnityWebRequest.error))
//				{
//					File.WriteAllBytes(localDestPath,mUnityWebRequest.downloadHandler.data);
//					mUnityWebRequest.Dispose();
//					yield return new WaitForEndOfFrame();
//					callback(true, "success");
//				}
//				else
//				{
//					var errorInfo = "错误信息:" + mUnityWebRequest.error;
//					Debug.LogError(errorInfo);
//					mUnityWebRequest.Dispose();
//					yield return new WaitForEndOfFrame();
//					callback(false, errorInfo);
//				}
//	     	}
//		}
		
		public void StopAllDownload()
		{
			mIsDownloading = false;
			this.StopAllCoroutines();
			if (mDownloadThreads != null)
			{
				mDownloadThreads.ForEach(s => s.StopDownload());
				mDownloadThreads.Clear();
				mDownloadThreads = null;
			}
		}

	
		private void Update()
		{
			if (!mIsDownloading) return;

			for (int i= mDownloadThreads.Count-1;i>=0;i--)
			{
				var downloadThread = mDownloadThreads[i];
				if (downloadThread.IsFinished)
				{
					if (downloadThread.IsSuccess)
					{
						mResUpdateData.finishedCount++;
						mResUpdateData.finishedSize = mFinishedSize = mFinishedSize + 1 * downloadThread.GetItemSize();
						ResUpdateCallback (UpdateStatus.FinishOneItem,downloadThread.ResItem.path);
						ResUpdateCallback(UpdateStatus.DownloadProgress,string.Format("下载进度，已经完成{0}M", mResUpdateData.finishedSize));
						
						if (mResUpdateData.finishedCount >= mResUpdateData.totalCount)
						{
							StopAllDownload();
							FinishDownLoadAll();
							break;
						}
						if (mUpdateItems.Count > 0)
						{
							downloadThread.StartDownloadItem(mRemoteResPath,mLocalTempDir,mUpdateItems.Dequeue());
						}
						else
						{
							downloadThread.ChangeToDeactive();
						}
					}
					else
					{
						StopAllDownload();
						ResUpdateCallback(UpdateStatus.Failed, "下载出错");
						break;
					}
				}
			}
		}

		private void SaveServerConfig()
		{
			string jsonContent = JsonUtility.ToJson(mServerConfig,true);
			var configDir = Path.GetDirectoryName(mLocalConfigFilePath);
			
			if (!Directory.Exists(configDir))
			{
				Directory.CreateDirectory(configDir);
			}
			
			File.WriteAllText(mLocalConfigFilePath, jsonContent);
		}

		private void FinishDownLoadAll()
		{
			PTModuleUpdateUtil.DirectoryCopy(mLocalTempDir, mLocalResPath, true, true); //从temp复制文件到最终assetbundle加载目录
			SaveServerConfig();
			//移除临时下载文件
			if (Directory.Exists(mLocalTempDir))
			{
				Directory.Delete(mLocalTempDir,true);
			}			
			ResUpdateCallback(UpdateStatus.Success, "FinishDownloadRes");
		}

		private void ResUpdateCallback(UpdateStatus status,string msg)
		{
			mResUpdateData.status = status;
			
			mResUpdateData.msg = msg;
			
			onUpdateResCallback.InvokeGracefully(mResUpdateData);
		}
		
		
		
//		private void EnqueueAction(Action action)
//		{
//			if (isAppRunning)
//			{
//				PTModuleUpdateDispatcher.Instance().Enqueue(action);
//			}
//			else
//			{
//				#if UNITY_EDITOR
//				EditorAsyncPump.Instance.Enqueue(action);
//				#endif
//			}
//		}
		
		

//		private void DownLoadResItem (ResItem item)
//		{	
//			string remoteUrl = Path.Combine(mRemoteResPath,item.path);
//
//			float itemSize = (item.size / 1024.0f) / 1024.0f;
//			
//			string localDestPath = Path.Combine(mLocalTempDir,item.path);
//			
//			PTDebug.Log(string.Format("***********从{0}下载到{1}：",remoteUrl,localDestPath));
//
//			var fileDir = Path.GetDirectoryName(localDestPath);
//			
//			if (!Directory.Exists(fileDir))
//			{
//				Directory.CreateDirectory(fileDir);
//			}
//
//			Debug.LogError("download >>>>______________"+remoteUrl);
//			StartCoroutine(DownloadFileWebRequest(remoteUrl, (result,info) =>
//			{
//				Debug.LogError(result+">>>>>>"+remoteUrl);
//				if (result)
//				{
//					ResUpdateCallback (UpdateStatus.FinishOneItem,localDestPath);
//					mResUpdateData.finishedCount++;
//					Debug.LogError(mResUpdateData.finishedCount+">> mResUpdateData.finishedCount>>>>"+remoteUrl+">>>>>>>>"+mResUpdateData.totalCount);
//					if (mResUpdateData.finishedCount == mResUpdateData.totalCount)
//					{
//						StopAllWebRequest();
//						
//						FinishDownLoadAll();
//					}
//					else
//					{
//						Debug.LogError(mUpdateItems.Count+">>>>>kkkkk>>"+mUpdateItems.Count+" >>>>>>>"+mHasFailed);
//						if (mUpdateItems.Count > 0 && !mHasFailed)
//						{
//							DownLoadResItem(mUpdateItems.Dequeue());
//						}
//					}
//				}
//				else
//				{
//					if (!mHasFailed)
//					{
//						if (item.retryTimes<2)
//						{
//							Debug.LogError("retry >>>>>"+localDestPath+"  >>>> "+item.retryTimes);
//							item.retryTimes++;
//							mUpdateItems.Enqueue(item);
//							DownLoadResItem(mUpdateItems.Dequeue());
//							Debug.LogError(mUpdateItems.Count+">>>>>>>length >>>>");
//						}
//						else
//						{
//							mHasFailed = true;
//							Debug.LogError(mUpdateItems.Count+">>> retry finish >>>>length >>>>");
//							StopAllWebRequest();
//							ResUpdateCallback(UpdateStatus.Failed, "下载出错");
//						}
//					}
//				}
//
//			},itemSize,localDestPath));
//		}
		
//		private  IEnumerator DownloadFileWebRequest(string remotePath,Action<bool, string> callback, float itemSize,string destPath)
//		{	
//			var unityWebRequest = UnityWebRequest.Get(remotePath);
//
//			unityWebRequest.timeout = 10;
//			
//			mWebRequests.Add(unityWebRequest);
//			
////			Debug.LogError("ready to  downloading >>>>>>>>>>"+remotePath);
//		     unityWebRequest.SendWebRequest();
////			Debug.LogError("is  downloading >>>>>>>>>>"+remotePath+unityWebRequest.isDone);
//			
//			float timer = Time.realtimeSinceStartup;
//			float timeOut = 10;
//			bool failed = false;
//
//			while (!unityWebRequest.isDone)
//			{
//				if (Time.realtimeSinceStartup - timer > timeOut)
//				{
//					failed = true;
//					break;
//				}
//				
//				mResUpdateData.finishedSize = mFinishedSize + unityWebRequest.downloadProgress*itemSize;
////				Debug.LogError("is downloading 222>>>>>>>>>>"+remotePath+"  "+unityWebRequest.isDone);
//				ResUpdateCallback(UpdateStatus.DownloadProgress,string.Format("下载进度，已经完成{0}M", mResUpdateData.finishedSize));
//				
////				Debug.LogError("is downloading 333 >>>>>>>>>>"+remotePath+"  "+unityWebRequest.isDone);
//				yield return 0;
//			}
//
//			if (!failed&&string.IsNullOrEmpty(unityWebRequest.error))
//			{
//				File.WriteAllBytes(destPath,unityWebRequest.downloadHandler.data);
//				
//				mResUpdateData.finishedSize = mFinishedSize = mFinishedSize + 1 * itemSize;
//					
//				ResUpdateCallback(UpdateStatus.DownloadProgress,string.Format("下载进度，已经完成{0}M", mResUpdateData.finishedSize));
//			   
//				unityWebRequest.Dispose();
//				
//				mWebRequests.Remove(unityWebRequest);
//				
//				yield return new WaitForEndOfFrame();
//				
////				EnqueueAction(() => callback(true,"success"));
//				Debug.LogError("success --->>>>>>>>"+remotePath);
//				callback(true, "success");
//			}
//			else
//			{
//				var errorInfo = "错误信息:" + unityWebRequest.error;
//				
//				Debug.LogError(errorInfo);
//				
//				unityWebRequest.Dispose();
//
//				mWebRequests.Remove(unityWebRequest);
//				
//				yield return new WaitForEndOfFrame();
//				callback(false, errorInfo);
////				EnqueueAction(() => callback(false,errorInfo));
//			}
//		}

	}

}