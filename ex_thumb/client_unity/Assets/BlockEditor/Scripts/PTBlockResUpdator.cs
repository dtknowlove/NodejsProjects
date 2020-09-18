using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Putao.BlockRes
{
	public class PTBlockResUpdator : MonoBehaviour
	{
		#region paths

		private const string ROOT_URL = "http://static-store.ptdev.cn/source/game_bloks_editor_res/blocks/";

		private static string LOCAL_ROOT_URL
		{
			get
			{
				#if UNITY_EDITOR
				return Application.dataPath + "/StreamingAssets/blocks/";
				#else
				return Application.persistentDataPath + "/blocks/";
				#endif
			}
		}

		private string GetResConfigUrl(bool isLocal, int categoryId)
		{
			return (isLocal ? LOCAL_ROOT_URL : ROOT_URL) + "category_" + categoryId + "/config.json";
		}

		private string GetABManifestUrl(bool isLocal, int categoryId)
		{
			return (isLocal ? LOCAL_ROOT_URL : ROOT_URL) + "category_" + categoryId + "/blocks";
		}

		#endregion

		private DownloadState mState = DownloadState.None;

		private int mCategoryId = -1;
		private ResConfig mUpdateResConfig = null;
		private ResConfig mLocalResConfig = null;
		private AssetBundleManifest mABManifest = null;

		public ResUpdateEvent OnResUpdate = new ResUpdateEvent();

		private void UpdateState(DownloadState state, ResUpdateMsg updateMsg = null)
		{
			if (mState == state) return;
			mState = state;

			if (updateMsg == null)
				updateMsg = new ResUpdateMsg {State = mState};
			OnResUpdate.Invoke(updateMsg);
		}

		/// <summary>
		/// 更新&下载资源
		/// 1.本地是否存在。
		//	2.存在则比对md5. 不一样则下载  根据block.manifest 遍历处理依赖
		//	3.不存在则下载               根据block.manifest 遍历处理依赖
		/// </summary>
		public void UpdateRes(ResUpdateInfo info)
		{
			StartCoroutine(WaitUpdateRes(info));
		}

		private IEnumerator WaitUpdateRes(ResUpdateInfo info)
		{
			mCategoryId = info.CategoryId;

			//1............................................
			GetResConfig(mCategoryId);
			while (mState != DownloadState.ResConfigFinish)
			{
				if (mState == DownloadState.Error) yield break;
				yield return null;
			}

			//2............................................
			GetABManifest(mCategoryId);
			while (mState != DownloadState.ABManifestFinish)
			{
				if (mState == DownloadState.Error) yield break;
				yield return null;
			}

			//3............................................
			if (info.PrefabList != null && info.PrefabList.Length > 0)
			{
				List<ResConfigItem> downloadList = GetDownloadList(info.PrefabList);
				if (mState == DownloadState.Error) yield break;
				if (downloadList.Count > 0)
				{
					UpdateState(DownloadState.Prefabs);
					yield return DownloadRes(downloadList);
					if (mState == DownloadState.Error) yield break;
					UpdateState(DownloadState.PrefabsFinish);
				}
			}

			//4............................................
			if (info.ThumbList != null && info.ThumbList.Length > 0)
			{
				List<ResConfigItem> downloadList = GetDownloadList(info.ThumbList);
				if (mState == DownloadState.Error) yield break;
				if (downloadList.Count > 0)
				{
					UpdateState(DownloadState.Thumbs);
					yield return DownloadRes(downloadList);
					if (mState == DownloadState.Error) yield break;
					UpdateState(DownloadState.ThumbsFinish);
				}
			}
		}


		/// <summary>
		/// 获取对应category的config 文件。每次都要先获取该文件
		/// </summary>
		/// <param name="categoryId">Category identifier.</param>
		private void GetResConfig(int categoryId)
		{
			UpdateState(DownloadState.ResConfig);
			mUpdateResConfig = null;
			mLocalResConfig = null;

			string url = GetResConfigUrl(false, categoryId);
			WWWLoadRes(url, www =>
			{
				mUpdateResConfig = JsonUtility.FromJson<ResConfig>(www.text);

				//local resconfig.json
				url = GetResConfigUrl(true, categoryId);
				if (!url.Contains("://"))
				{
					mLocalResConfig = JsonUtility.FromJson<ResConfig>(File.ReadAllText(url));
					UpdateState(DownloadState.ResConfigFinish);
				}
				else
				{
					WWWLoadRes(url, www2 =>
					{
						mLocalResConfig = JsonUtility.FromJson<ResConfig>(www.text);
						UpdateState(DownloadState.ResConfigFinish);
					});
				}
			});
		}

		/// <summary>
		/// update the local resconfig after successfully downloaded a res
		/// </summary>
		private void UpdateResConfig(ResConfigItem item)
		{
			ResConfigItem oldItem = mLocalResConfig.Items.Find(e => e.name.Equals(item.name));
			if (oldItem != null)
				mLocalResConfig.Items.Remove(oldItem);
			mLocalResConfig.Items.Add(item);

			WriteToLocal(GetResConfigUrl(true, mCategoryId), JsonUtility.ToJson(mLocalResConfig), null);
		}

		private void GetABManifest(int categoryId)
		{
			UpdateState(DownloadState.ABManifest);

			string url = GetABManifestUrl(false, categoryId);
			WWWLoadRes(url, www =>
			{
				AssetBundle ab = www.assetBundle;
				mABManifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
				ab.Unload(false);
				
				//write local manifest
				url = GetABManifestUrl(true, categoryId);
				WriteToLocal(url, null, www.bytes);

				UpdateState(DownloadState.ABManifestFinish);
			});
		}

		/// <summary>
		/// Compare the reslist in the downloaded resconfig with that in the local resconfig.
		/// Also consider the dependencies according to block.manifest 
		/// </summary>
		private List<ResConfigItem> GetDownloadList(string[] resList)
		{
			List<ResConfigItem> items = new List<ResConfigItem>();

			Func<string, bool> checkAdd = (resName) =>
			{
				ResConfigItem newItem = mUpdateResConfig.Items.Find(e => e.name.Equals(resName));
				if (newItem == null)
				{
					UpdateState(DownloadState.Error, new ResUpdateMsg()
					{
						State = DownloadState.Error,
						MsgText = string.Format("Can't find {0} in config.json", resName)
					});
					return false;
				}

				ResConfigItem oldItem = mLocalResConfig.Items.Find(e => e.name.Equals(resName));
				if (oldItem == null || !oldItem.md5.Equals(newItem.md5))
				{
					if (!items.Exists(e => e.name.Equals(newItem.name)))
						items.Add(newItem);
				}
				return true;
			};

			foreach (string res in resList)
			{
				if (!checkAdd(res)) return null;

				//find dependencies
				string[] dependencies = mABManifest.GetAllDependencies(res);
				foreach (string dpRes in dependencies)
				{
					if (!checkAdd(dpRes)) return null;
				}
			}

			return items;
		}

		/// <summary>
		///	Download process 
		/// </summary>
		private IEnumerator DownloadRes(List<ResConfigItem> resList)
		{
			long downloadSize = 0;
			long totalSize = 0;
			foreach (ResConfigItem item in resList)
			{
				totalSize += long.Parse(item.size);
			}

			foreach (ResConfigItem item in resList)
			{
				string url = ROOT_URL + item.name;
				long itemSize = long.Parse(item.size);
				using (WWW www = new WWW(url))
				{
					while (!www.isDone)
					{
						float progress = (www.progress * itemSize + downloadSize) / totalSize;
						OnResUpdate.Invoke(new ResUpdateMsg()
						{
							State = mState,
							Progress = progress,
						});
						yield return null;
					}

					if (!string.IsNullOrEmpty(www.error))
					{
						UpdateState(DownloadState.Error, new ResUpdateMsg()
						{
							State = DownloadState.Error,
							MsgText = www.error
						});
						yield break;
					}

					url = LOCAL_ROOT_URL + item.name;
					WriteToLocal(url, null, www.bytes);
				}
				downloadSize += itemSize;
				UpdateResConfig(item);
			}
		}

		private void WWWLoadRes(string url, Action<WWW> callback)
		{
			StartCoroutine(ResLoader(url, callback));
		}

		IEnumerator ResLoader(string url, Action<WWW> callback)
		{
			Debug.LogFormat("<color=orange>[PTBlockResUpdator] - get url: {0}</color>", url);
			using (WWW www = new WWW(url))
			{
				yield return www;
				if (www.isDone)
				{
					if (!string.IsNullOrEmpty(www.error))
					{
						UpdateState(DownloadState.Error, new ResUpdateMsg()
						{
							State = DownloadState.Error,
							MsgText = string.Format("request url: {0}, error: {1}", url, www.error),
						});
					}
					else
					{
						callback(www);
					}
				}
			}
		}

		private void WriteToLocal(string url, string text, byte[] bytes)
		{
			Debug.LogFormat("<color=orange>[PTBlockResUpdator] - write local url: {0}</color>", url);
			string directory = Path.GetDirectoryName(url);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			if (File.Exists(url))
				File.Delete(url);

			if (!string.IsNullOrEmpty(text))
				File.WriteAllText(url, text);
			else if (bytes != null)
				File.WriteAllBytes(url, bytes);
		}

		private void Awake()
		{
			OnResUpdate.AddListener((msg) =>
			{
				string log = "[PTBlockResUpdator] - state: " + msg.State;
				if (!string.IsNullOrEmpty(msg.MsgText))
					log += "\nmsg: " + msg.MsgText;
				if (msg.Progress > 0)
					log += "\ndownload progress: " + msg.Progress;
				Debug.LogFormat("<color=orange>{0}</color>", log);
			});
		}
	}
}
