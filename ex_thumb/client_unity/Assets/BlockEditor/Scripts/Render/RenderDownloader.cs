using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BestHTTP;
using PTGame.Core;
using UnityEngine;

public class RenderDownloader:MonoBehaviour
{
	public static void Execute(RenderFile renderFile,Action<bool> onFinish)
	{
		var obj = new GameObject("RenderDownloader");
		var render = obj.AddComponent<RenderDownloader>();
		render.mRenderFile = renderFile;
		render.onFinish = onFinish;
	}

	public static void Cancel()
	{
		RenderDownloader loader = GameObject.FindObjectOfType<RenderDownloader>();
		if (loader != null)
		{
			loader.Stop();
		}
	}

	private DownloadPip mDownloadPip;
	private RenderFile mRenderFile;
	private string[] mUrls;
	private int counter = 0;
	private Action<bool> onFinish;

	void Start()
	{
		Download();
	}

	public void Download()
	{
		if (mRenderFile == null)
			return;
		mUrls = mRenderFile.info.GetUrls().ToArray();
		counter = 0;

		mDownloadPip = new DownloadPip();
		mDownloadPip.StartDownload(this, mUrls, 1, OnDownItemFinish);
	}

	private void OnDownItemFinish(DownInfo info)
	{
		counter++;
		if (info == null)
			return;
		bool isModel = info.Url.EndsWith(".bin") || info.Url.EndsWith(".gltf");
		var resName = GetUrlResName(info.Url);
		if (!info.HasError)
		{
			if (isModel)
			{
				var modelPath = ProccessModel(info, resName);
				PTDebug.Log("完成下载模型文件: " + modelPath);
			}
			else
			{
				var texPath = ProccessTextures(info, resName);
				PTDebug.Log("完成下载贴图文件: " + texPath);
			}

			if (counter >= mUrls.Length)
			{
				OnFinish(true);
			}
		}
		else
		{
			var path = GetPath(info.Url, isModel, resName);
			if (File.Exists(path))
			{
				PTDebug.LogError("未下载成功，使用本地缓存，错误:{0}", info.Error);
			}
			else
			{
				PTDebug.LogError(info.Error);
				mDownloadPip.StopDownload();
				OnFinish(false);
			}
		}
	}

	private string GetUrlResName(string url)
	{
		//处理普通零件及丝印材质纹理资源名称
		var renderItem = mRenderFile.info.GetRenderItemByModelUrl(url);
		if (renderItem != null)
		{
			return renderItem.ParentEqualUrl(url) ? renderItem.model : renderItem.GetTexModelByUrl(url).Model;
		}
		renderItem = mRenderFile.info.GetRenderItemByTexUrl(url); 
		var parent = renderItem.GetSkinInfoByUrl(url);
		return parent != null ? parent.skin_name : renderItem.GetTexInfoByUrl(url).Texture;
	}

	private string ProccessModel(DownInfo info,string resName)
	{
		var model = resName; 
		var fileWritePath = GetPath(info.Url, true, model);
		fileWritePath.DeleteFileIfExists();
		File.WriteAllBytes(fileWritePath, info.Data);
		return fileWritePath;
	}

	private string ProccessTextures(DownInfo info, string resName)
	{
		var fileWritePath = GetPath(info.Url, false, resName);
		fileWritePath.DeleteFileIfExists();
		File.WriteAllBytes(fileWritePath, info.Data);
		return fileWritePath;
	}

	private string GetPath(string url, bool isModel = false, string resName = "")
	{
		var fileName = url.Substring(url.LastIndexOf("/") + 1);
		string dir = isModel ? RenderDefine.ModelPrefixPath + resName + "/" : RenderDefine.TexPrefixPath;
		dir.CreateDirIfNotExists();
		if (isModel)
		{
			if (fileName.EndsWith(".bin"))
				fileName = "buffer.bin";
			if (fileName.EndsWith(".gltf"))
				fileName = resName + ".gltf";
		}
		else
		{
			fileName = resName + Path.GetExtension(fileName);
		}
		return dir + fileName;
	}

	private void OnFinish(bool success)
	{
		onFinish.InvokeGracefully(success);
		Stop();
	}

	private void Stop()
	{
		StopAllCoroutines();
		mDownloadPip?.StopDownload();
		Destroy(gameObject);
	}
}

public class DownInfo
{
	public byte[] Data;
	public string Error;
	public string Url;
	public int StatusCode;
	public float DownloadSpeed;
        
	private string dataAsText;
	public string DataAsText
	{
		get
		{
			if (Data == null)
				return string.Empty;

			if (!string.IsNullOrEmpty(dataAsText))
				return dataAsText;

			return dataAsText = System.Text.Encoding.UTF8.GetString(Data, 0, Data.Length);
		}
	}

	public bool HasError
	{
		get { return !string.IsNullOrEmpty(Error); }
	}

	public string FileName
	{
		get { return Url.Substring(Url.LastIndexOf('/') + 1).Split('.')[0]; }
	}

	public override string ToString()
	{
		return string.Format("Url:{0},Error:{1}", Url, Error);
	}

	public void Clear()
	{
		Data = null;
		dataAsText = string.Empty;
		Error = string.Empty;
		Url = string.Empty;
	}
}

public class DownloadPip
{
	class CorouteData
	{
		private string Url;
		private int Timeout;
		private Action<DownInfo> onCallback;

		private bool isStop;

		private bool mFinish = false;
		private DownInfo mDownInfo = null;
		private float mTimeConsuming = 0;
		private float mItemSize = 0;

		/// <summary>
		/// 下载速度 unit:Mb/s
		/// </summary>
		public float DownloadSpeed
		{
			get { return mTimeConsuming == 0 ? 0 : mItemSize / mTimeConsuming; }
		}

		public bool Finish
		{
			get { return mFinish; }
		}

		public void SetData(string url, int timeout = 10, Action<DownInfo> callback = null)
		{
			if (mDownInfo == null)
				mDownInfo = new DownInfo();
			mDownInfo.Clear();
			mDownInfo.Url = url;

			mFinish = false;
			Url = url;
			Timeout = timeout;
			onCallback = callback;
			isStop = false;
		}

		private HTTPRequest mRequest;

		public IEnumerator Download()
		{
			if (isStop)
			{
				onCallback.InvokeGracefully(null);
				mFinish = true;
				if (mRequest != null)
				{
					mRequest.Abort();
					mRequest.Dispose();
					mRequest = null;
				}
				yield break;
			}
			PTDebug.LogWarning("下载数据:" + Url);
			var requestFinish = false;
			mRequest = new HTTPRequest(new Uri(Url),
				(req, res) =>
				{
					requestFinish = true;
					mDownInfo.StatusCode = res != null ? res.StatusCode : 22222222;
					mFinish = true;
					mDownInfo.Error = req.Exception != null ? req.Exception.ToString() : (res != null ? (res.StatusCode == 404 ? "服务器不存在资源:" + Url : null) : null);
					mDownInfo.Data = res == null || res.StatusCode == 404 ? null : res.Data;
					if (res != null)
						mItemSize = res.Data.Length / 8.0f / 1024.0f / 1024.0f;
					if (mRequest != null)
					{
						mRequest.Abort();
						mRequest.Dispose();
						mRequest = null;
					}
				})
			{
				ConnectTimeout = TimeSpan.FromSeconds(5),
				Timeout = TimeSpan.FromSeconds(Timeout),
				DisableCache = true
			};
			mRequest.Send();
			while (!requestFinish)
			{
				mTimeConsuming += Time.deltaTime;
				yield return null;
			}
			mDownInfo.DownloadSpeed = DownloadSpeed;
			onCallback.InvokeGracefully(mDownInfo);
		}

		public void Stop()
		{
			isStop = true;
			try
			{
				if (mRequest != null)
				{
					mRequest.Abort();
					mRequest.Dispose();
					mRequest = null;
				}
			}
			catch (Exception e)
			{
				PTDebug.Log(e);
			}
		}
	}

	private Queue<string> mUrlQueues;
	private List<CorouteData> mCorouteDatas;

	private int mThreadNum = 1;
	private Action<DownInfo> OnCallBack;
	private MonoBehaviour mMono;
	private Coroutine mMonitorCoroutine;

	public DownloadPip()
	{
		mUrlQueues = new Queue<string>();
		mCorouteDatas = new List<CorouteData>();
	}

	public void StartDownload(MonoBehaviour mono, string[] urls, int threadnum = 1, Action<DownInfo> onCallback = null)
	{
		Clear();
		foreach (var t in urls)
		{
			mUrlQueues.Enqueue(t);
		}
		mMono = mono;
		mThreadNum = threadnum;
		OnCallBack = onCallback;

		for (int i = 0; i < mThreadNum; i++)
		{
			if (mUrlQueues.Count != 0)
			{
				var data = new CorouteData();
				data.SetData(mUrlQueues.Dequeue(), 10, OnCallBack);
				mCorouteDatas.Add(data);
				mono.StartCoroutine(data.Download());
			}
		}

		mMonitorCoroutine = mono.StartCoroutine(InnerDownload());
	}

	public void StopDownload()
	{
		if (mMono != null && mMonitorCoroutine != null)
			mMono.StopCoroutine(mMonitorCoroutine);
		mCorouteDatas.ForEach(t => t.Stop());
		Clear();
	}

	private IEnumerator InnerDownload()
	{
		while (mUrlQueues.Count != 0)
		{
			foreach (var data in mCorouteDatas)
			{
				if (data.Finish)
				{
					yield return null;
					if (mUrlQueues.Count == 0)
						yield break;
					data.SetData(mUrlQueues.Dequeue(), 10, OnCallBack);
					mMono.StartCoroutine(data.Download());
				}
			}
			yield return null;
		}
	}

	private void Clear()
	{
		if (mUrlQueues != null)
			mUrlQueues.Clear();
		if (mCorouteDatas != null)
			mCorouteDatas.Clear();
	}
}
