using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BestHTTP;
using PTGame.Core;
using PTGame.ModuleUpdate;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Putao.GameCommon
{
    public class UpdateBlockDataConfig : ExecuteNode
    {
        public const string DIR_CONFIG = "config_blockdata";

        public bool Success { get; private set; }

        protected override void OnBegin()
        {
            ServerDefine.ResUrl resUrl = ServerDefine.GetConfigUrl(DIR_CONFIG, "resconfig.json");
            PTModuleUpdate.Instance.StartModuleUpdate(resUrl.remoteConfigPath, resUrl.localConfigPath, OnReadyToDownload, null,
                OnUpdateFinish, null);
        }

        private void OnReadyToDownload(int totalCount, float totalSize)
        {
            PTModuleUpdate.Instance.StartDownloadRes(5);
        }
        
        private void OnUpdateFinish(UpdateStatus arg1, string arg2, ResUpdateData resUpdateData)
        {
            Finished = true;

            if (arg1 != UpdateStatus.Success)
            {
                Debug.LogWarning(arg1 + "OnUpdateFinish >>>>>>>" + arg2 + ">>>>>>");
                Success = false;
            }
            else
            {
                Success = true;
            }
        }
    }

    public class UpdateModelRes : ExecuteNode
    {
        private readonly string[] mConfigFiles;

        public bool Success { get; private set; }
 
        private bool mIsSukMode = false;
        public UpdateFileProgress UpdateProgress { get; private set; }

        public UpdateModelRes(string modelResConfigFile,bool isSku)
        {
            this.mIsSukMode = isSku;
            mConfigFiles = new[] {modelResConfigFile};

            UpdateProgress = new UpdateFileProgress
            {
                TotalSectionCount = mConfigFiles.Length
            };
        }

        public UpdateModelRes(string[] modelResConfigFiles,bool isSku)
        {
            this.mIsSukMode = isSku;
            
            mConfigFiles = modelResConfigFiles;

            UpdateProgress = new UpdateFileProgress
            {
                TotalSectionCount = mConfigFiles.Length
            };
        }

        protected override void OnBegin()
        {
            UpdateProgress.CurSectionCount = 0;
            StartDown(mConfigFiles[0]);
        }

        private void StartDown(string configFile)
        {
            ServerDefine.ResUrl resUrl = ServerDefine.GetBlockModeRes(configFile,mIsSukMode);
         
            PTModuleUpdate.Instance.StartModuleUpdate(resUrl.remoteConfigPath, resUrl.localConfigPath,
                resUrl.remoteResPath,
                resUrl.localResPath, OnReadyToDownload, OnFinishOneItem, OnUpdateFinish,
                null, false, true);
        }

        private void OnReadyToDownload(int totalCount, float totalSize)
        {
            UpdateProgress.CurCount = 0;
            UpdateProgress.TotalCount = totalCount;
            PTModuleUpdate.Instance.StartDownloadRes(20);
        }

        private void OnFinishOneItem(int finishCount, int totalCount)
        {
            UpdateProgress.CurCount = finishCount;
        }

        private void OnUpdateFinish(UpdateStatus arg1, string arg2, ResUpdateData resUpdateData)
        {
            if (arg1 != UpdateStatus.Success)
            {
                Debug.LogError(arg1 + "UpdateMoodels >>>>>>>" + arg2 + ">>>>>>");
                Finished = true;
                Success = false;
                return;
            }

            Debug.Log("完成下载模型：" + mConfigFiles[UpdateProgress.CurSectionCount]);
            UpdateProgress.CurSectionCount++;
            UpdateProgress.CurCount = 0;

            if (UpdateProgress.CurSectionCount < mConfigFiles.Length)
            {
                StartDown(mConfigFiles[UpdateProgress.CurSectionCount]);
            }
            else
            {
                Finished = true;
                Success = true;
            }
        }
    }
    
    public class UpdateBlokBuildRes : ExecuteNode
    {
        public const string DIR_CONFIG = "config_paibloks_blockbuild";

        public bool Success { get; private set; }

        protected override void OnBegin()
        {
            ServerDefine.ResUrl resUrl = ServerDefine.GetConfigUrl(DIR_CONFIG, "resconfig.json");
            PTModuleUpdate.Instance.StartModuleUpdate(resUrl.remoteConfigPath, resUrl.localConfigPath, null, null, OnUpdateFinish, null);
        }

        private void OnUpdateFinish(UpdateStatus arg1, string arg2,ResUpdateData resUpdateData)
        {
            Finished = true;

            if (arg1 != UpdateStatus.Success)
            {
                Debug.LogWarning(arg1 + "OnUpdateFinish >>>>>>>" + arg2 + ">>>>>>");
                Success = false;
            }
            else
            {
                Success = true;
            }
        }
    }

//    public class UpdatePlatformConfig : ExecuteNode
//    {
//        public const string DIR_CONFIG = "config_paibloks_platform";
//
//        public bool Success { get; private set; }
//
//        protected override void OnBegin()
//        {
//            ServerDefine.ResUrl resUrl = ServerDefine.GetConfigUrl(DIR_CONFIG, "resconfig.json");
//            PTModuleUpdate.Instance.StartModuleUpdate(resUrl.remoteConfigPath, resUrl.localConfigPath, null, null, OnUpdateFinish, null);
//        }
//
//        private void OnUpdateFinish(UpdateStatus arg1, string arg2,ResUpdateData resUpdateData)
//        {
//            Finished = true;
//
//            if (arg1 != UpdateStatus.Success)
//            {
//                Debug.LogWarning(arg1 + "OnUpdatePlatformConfig >>>>>>>" + arg2 + ">>>>>>");
//                Success = false;
//            }
//            else
//            {
//                Success = true;
//            }
//        }
//    }

//    public class UpdateAnimConfig : ExecuteNode
//    {
//        private readonly string[] mConfigFiles;
//        private readonly string mDir;
//        private readonly string mExtension;
//        private DownloadPip mDownloadPip;
//        private MonoBehaviour mMono;
//        private string mResDir = string.Empty;
//
//        public bool Success { get; private set; }
//        public UpdateFileProgress UpdateProgress { get; private set; }
//
//        public UpdateAnimConfig(string dir, string animConfigFile, string extension)
//        {
//            mDir = dir;
//            mExtension = extension;
//            mConfigFiles = new[] {animConfigFile};
//
//            UpdateProgress = new UpdateFileProgress
//            {
//                TotalCount = mConfigFiles.Length,
//                TotalSectionCount = 1
//            };
//        }
//
//        public UpdateAnimConfig(string dir, string[] animConfigFiles, string extension,MonoBehaviour mono)
//        {
//            mDir = dir;
//            mExtension = extension;
//            mConfigFiles = animConfigFiles;
//
//            UpdateProgress = new UpdateFileProgress
//            {
//                TotalCount = mConfigFiles.Length,
//                TotalSectionCount = 1
//            };
//            mMono = mono;
//            mDownloadPip=new DownloadPip();
//        }
//
//        protected override void OnBegin()
//        {
//            mResDir= Application.persistentDataPath + "/" + mDir;
//            if (!Directory.Exists(mResDir))
//                Directory.CreateDirectory(mResDir);
//            
//            UpdateProgress.CurCount = 0;
//            UpdateProgress.CurSectionCount = 0;
//            var mUlrs = new List<string>();
//            foreach (var t in mConfigFiles)
//            {
//                mUlrs.Add(ServerDefine.GetConfigUrl(mDir, t + mExtension).remoteConfigPath);
//            }
//            mDownloadPip.StartDownload(mMono, mUlrs.ToArray(), 5, OnCallBack);
//        }
//
//        private void OnCallBack(DownInfo info)
//        {
//            if (info == null)
//                return;
//            if (!info.HasError)
//            {
//                File.WriteAllText(mResDir + "/" + info.FileName, info.Content);
//
//                Debug.Log("完成下载搭建文件" + mResDir + "/" + info.FileName);
//                
//                UpdateProgress.CurCount++;
//                if (UpdateProgress.CurCount >= mConfigFiles.Length)
//                {
//                    Finished = true;
//                    Success = true;
//                }
//            }
//            else
//            {
//                Finished = true;
//                Success = false;
//                mDownloadPip.StopDownload();
//                PTDebug.LogError(info.Error);
//            }
//        }
//
//        private void DownLoadConfig(string configFile)
//        {
//            var serverUrl = ServerDefine.GetConfigUrl(mDir, configFile+mExtension).remoteConfigPath;
//
//            ObservableWWW.Get(serverUrl).Timeout(TimeSpan.FromSeconds(3)).Subscribe((content) =>
//            {
//                var dir = Application.persistentDataPath + "/" + mDir;
//                if (!Directory.Exists(dir))
//                    Directory.CreateDirectory(dir);
//
//                File.WriteAllText(dir + "/" + configFile, content);
//
//                Debug.Log("完成下载搭建文件" + dir + "/" + configFile);
//                UpdateProgress.CurCount++;
//
//                if (UpdateProgress.CurCount < mConfigFiles.Length)
//                {
//                    DownLoadConfig(mConfigFiles[UpdateProgress.CurCount]);
//                }
//                else
//                {
//                    Finished = true;
//                    Success = true;
//                }
//
//            }, error =>
//            {
//                PTDebug.LogError(">>>>>>****{0}>>>>>>{1}", serverUrl, error);
//                Finished = true;
//                Success = false;
//            });
//        }
//    }
    
    public class UpdateThumbs : ExecuteNode
    {
        private readonly string[] mConfigFiles;
        
        public bool Success { get; private set; }
        public UpdateFileProgress UpdateProgress { get; private set; }

        public UpdateThumbs(string thumbConfigFile)
        {
            mConfigFiles = new[] {thumbConfigFile};

            UpdateProgress = new UpdateFileProgress
            {
                TotalSectionCount = mConfigFiles.Length
            };
        }

        public UpdateThumbs(string[] thumbConfigFiles)
        {
            mConfigFiles = thumbConfigFiles;
            
            UpdateProgress = new UpdateFileProgress
            {
                TotalSectionCount = mConfigFiles.Length
            };
        }

        protected override void OnBegin()
        {
            UpdateProgress.CurSectionCount = 0;
            StartDown(mConfigFiles[0]);
        }

        private void StartDown(string configFile)
        {
            ServerDefine.ResUrl thumbUrl = ServerDefine.GetThumbUrl(configFile);

            PTModuleUpdate.Instance.StartModuleUpdate(thumbUrl.remoteConfigPath, thumbUrl.localConfigPath, thumbUrl.remoteResPath,
                thumbUrl.localResPath, OnReadyToDownload, OnFinishOneItem, OnUpdateFinish,
                null, false,true);
        }

        private void OnReadyToDownload(int totalCount, float totalSize)
        {
            UpdateProgress.CurCount = 0;
            UpdateProgress.TotalCount = totalCount;
            PTModuleUpdate.Instance.StartDownloadRes(10);
        }

        private void OnFinishOneItem(int finishCount, int totalCount)
        {
            UpdateProgress.CurCount = finishCount;
        }

        private void OnUpdateFinish(UpdateStatus arg1, string arg2, ResUpdateData resUpdateData)
        {
            if (arg1 != UpdateStatus.Success)
            {
                Debug.LogError(arg1 + "UpdateThumbs >>>>>>>" + arg2 + ">>>>>>");
                Finished = true;
                Success = false;
                return;
            }

            Debug.Log("完成下载搭建thumb：" + mConfigFiles[UpdateProgress.CurSectionCount]);
            UpdateProgress.CurSectionCount++;
            UpdateProgress.CurCount = 0;

            if (UpdateProgress.CurSectionCount < mConfigFiles.Length)
            {
                StartDown(mConfigFiles[UpdateProgress.CurSectionCount]);
            }
            else
            {
                Finished = true;
                Success = true;
            }
        }
    }
    
     public class UpdateBlockBotsRes : ExecuteNode
        {
            public bool Success { get; private set; }
     
            public UpdateFileProgress UpdateProgress { get; private set; }

            public UpdateBlockBotsRes()
            {
       
                UpdateProgress = new UpdateFileProgress
                {
                    TotalSectionCount = 1
                };
            }

            protected override void OnBegin()
            {
                UpdateProgress.CurSectionCount = 0;
                StartDown();
            }

            private void StartDown()
            {
                ServerDefine.ResUrl resUrl = ServerDefine.GetBlockBotsResUrl();
             
                
                PTModuleUpdate.Instance.StartModuleUpdate(resUrl.remoteConfigPath, resUrl.localConfigPath,
                OnReadyToDownload, OnFinishOneItem, OnUpdateFinish,
                    null, true, false);
            }

            private void OnReadyToDownload(int totalCount, float totalSize)
            {
                UpdateProgress.CurCount = 0;
                UpdateProgress.TotalCount = totalCount;
                PTModuleUpdate.Instance.StartDownloadRes(20);
            }

            private void OnFinishOneItem(int finishCount, int totalCount)
            {
                UpdateProgress.CurCount = finishCount;
            }

            private void OnUpdateFinish(UpdateStatus arg1, string arg2, ResUpdateData resUpdateData)
            {
                if (arg1 != UpdateStatus.Success)
                {
                    Debug.LogError(arg1 + "Update BlockBots Res >>>>>>>" + arg2 + ">>>>>>");
                    Finished = true;
                    Success = false;
                }
                else
                {
                    Finished = true;
                    Success = true;
                }
            }
        }


    #region 多协程下载
    
    public class DownInfo
    {
        public string Content;
        public string Error;
        public string Url;

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
            Content = string.Empty;
            Error = string.Empty;
            Url = string.Empty;
        }
    }

    public class CorouteData
    {
        private string Url;
        private int Timeout;
        private Action<DownInfo> onCallback;

        private bool isStop;

        private bool mFinish = false;
        private DownInfo mDownInfo = null;

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
            mRequest = new HTTPRequest(new Uri(Url),
                (req, res) =>
                {
                    mFinish = true;
                    mDownInfo.Content = res == null ? null : res.DataAsText;
                    mDownInfo.Error = req.Exception == null ? null : req.Exception.ToString();
                    onCallback.InvokeGracefully(mDownInfo);
                    if (mRequest != null)
                    {
                        mRequest.Abort();
                        mRequest.Dispose();
                        mRequest = null;
                    }
                })
            {
                ConnectTimeout = TimeSpan.FromSeconds(5), Timeout = TimeSpan.FromSeconds(Timeout), DisableCache = true
            };
            mRequest.Send();
            yield return null;
        }

//        public IEnumerator Download()
//        {
//            UnityWebRequest loader = UnityWebRequest.Get(Url);
//            loader.timeout = Timeout;
//
//            float duration = 0.0f;
//            var sync = loader.SendWebRequest();
//            while ((!loader.isDone || !sync.isDone) && !isStop && duration < Timeout)
//            {
//                duration += Time.deltaTime;
//                yield return null;
//            }
//
//            if (isStop)
//            {
//                onCallback.InvokeGracefully(null);
//                mFinish = true;
//                loader.Dispose();
//                yield break;
//            }
//
//            mFinish = true;
//            mDownInfo.Content = loader.downloadHandler.text;
//            mDownInfo.Error = duration >= Timeout ? "下载超时!" : loader.error;
//            onCallback.InvokeGracefully(mDownInfo);
//
//            loader.Dispose();
//        }

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
                throw;
            }
        }
    }

    public class DownloadPip
    {
        public Queue<string> mUrlQueues;
        public List<CorouteData> mCorouteDatas;

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
            Debug.Log(mUrlQueues.Count);
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
            while (mUrlQueues.Count!=0)
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

    #endregion
}
public class UpdateFileProgress
{
    public int CurCount = 0;
    public int TotalCount;

    public int CurSectionCount = 0;
    public int TotalSectionCount;

    public float Progress
    {
        get
        {
            if (TotalCount == 0)
                return (float) CurSectionCount / TotalSectionCount;
            return (CurSectionCount + (float) CurCount / TotalCount) / TotalSectionCount;
        }
    }
}
