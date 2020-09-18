/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Core;
    using System.IO;
    using UniRx;

    public delegate void OnZipFinished(string zipFilePath, string outDirPath);

    public delegate void OnZipError(string zipFilePath, string outDirPath, string errorMsg);

    public delegate void OnZipProgress(string zipFilePath, string outDirPath, float percent);

    class ZipMgr : PTSingleton<ZipMgr>
    {
        class ZipWorker
        {
            string mZipFilePath;
            string mOutDirPath;
            event OnZipFinished mOnZipFinished;
            event OnZipError mOnZipError;
            event OnZipProgress mOnZipProgress;
            Thread mThread;
            bool mFinish;
            bool mIsError;
            string mErrorMsg;
            long mFileTotalCount;
            long mFileCompletedCount;
            long mCurFileProcessByteCount;
            long mCurFileTotalByteCount;

            public bool Finish
            {
                get { return mFinish; }
            }

            public ZipWorker(string zipFilePath, string outDirPath, OnZipFinished finished, OnZipError error,
                OnZipProgress progress)
            {
                mZipFilePath = zipFilePath;
                mOutDirPath = outDirPath;
                mOnZipFinished = finished;
                mOnZipError = error;
                mOnZipProgress = progress;
                mThread = new Thread(Work);
                mFinish = false;
                mIsError = false;
                mErrorMsg = "";
                mFileTotalCount = 0;
                mFileCompletedCount = 0;
                mCurFileProcessByteCount = 0;
                mCurFileTotalByteCount = 0;
            }

            public void Update()
            {
                if (mIsError)
                {
                    if (mOnZipError != null)
                    {
                        mOnZipError(mZipFilePath, mOutDirPath, mErrorMsg);
                    }
                    mOnZipError = null;
                    mOnZipFinished = null;
                    mOnZipProgress = null;
                    return;
                }

                var percent = 0.0f;
                if (mFileTotalCount == 1)
                {
                    percent = (float) mCurFileProcessByteCount / (float) mCurFileTotalByteCount;
                    if (mCurFileProcessByteCount == 0)
                        percent = 0;
                    else if (mCurFileTotalByteCount == 0)
                        percent = 1f;
                }
                else
                {
                    percent = (float) mFileCompletedCount / (float) mFileTotalCount;
                    if (mFileCompletedCount == 0)
                        percent = 0;
                    else if (mFileTotalCount == 0)
                        percent = 1f;
                }

                //Debug.LogError(percent);
                if (mOnZipProgress != null)
                {
                    mOnZipProgress(mZipFilePath, mOutDirPath, percent);
                }

                if (mFinish)
                {
                    if (mOnZipFinished != null)
                    {
                        mOnZipFinished(mZipFilePath, mOutDirPath);
                    }
                    mOnZipError = null;
                    mOnZipFinished = null;
                    mOnZipProgress = null;
                }
            }

            public void Start()
            {
                mThread.Start();
            }

            public void Stop()
            {
                //m_Thread.Interrupt();
            }

            void Work()
            {
                try
                {
                    var zipFile = new ZipFile(mZipFilePath);
                    mFileTotalCount = zipFile.Count;
                    zipFile.Close();

                    var zipEvent = new FastZipEvents();
                    zipEvent.Progress = OnProcess;
                    zipEvent.CompletedFile = OnCompletedFile;

                    var fastZip = new FastZip(zipEvent);
                    fastZip.CreateEmptyDirectories = true;
                    fastZip.ExtractZip(mZipFilePath, mOutDirPath, null);
                    mFinish = true;
                }
                catch (Exception exception)
                {
                    mErrorMsg = exception.Message;
                    mIsError = true;
                    mFinish = true;
                    throw new Exception(exception.Message);
                }
            }

            void OnProcess(object sender, ProgressEventArgs e)
            {
                mCurFileProcessByteCount = e.Processed;
                mCurFileTotalByteCount = e.Target > 0 ? e.Target : e.Processed;
            }

            void OnCompletedFile(object sender, ScanEventArgs e)
            {
                ++mFileCompletedCount;
            }
        }

        List<ZipWorker> mZipWorkerList = new List<ZipWorker>();

        public override void OnSingletonInit()
        {
            //启动 update 指令
            Observable.EveryUpdate().Subscribe(_ => Update());
        }

        private void Update()
        {
            for (var i = mZipWorkerList.Count - 1; i >= 0; --i)
            {
                mZipWorkerList[i].Update();
                if (mZipWorkerList[i].Finish)
                {
                    mZipWorkerList[i].Stop();
                    mZipWorkerList.RemoveAt(i);
                }
            }
        }

        public void UnZip(string zipFilePath, string outDirPath, OnZipFinished finished, OnZipError error,
            OnZipProgress progress)
        {
            var worker = new ZipWorker(zipFilePath, outDirPath, finished, error, progress);
            worker.Start();
            mZipWorkerList.Add(worker);
        }

        public bool UnZipData(byte[] inputData, string outDirPaht)
        {
            try
            {
                var ms = new MemoryStream(inputData);
                var fastZip = new FastZip();
                fastZip.CreateEmptyDirectories = true;
                fastZip.ExtractZip(ms, outDirPaht, FastZip.Overwrite.Always, null, null, null, false, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Unzip Data error: " + ex.Message + ex.StackTrace);
            }

            return true;
        }

        public byte[] UnZipData(byte[] inputData)
        {
            try
            {
                using (var ms = new MemoryStream(inputData))
                {
                    using (var zipFile = new ZipFile(ms))
                    {
                        var zipEntry = zipFile[0];
                        var outData = new byte[zipEntry.Size];
                        var stream = zipFile.GetInputStream(zipEntry);
                        stream.Read(outData, 0, outData.Length);
                        return outData;
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return null;
        }

        void Clean()
        {
            foreach (var w in mZipWorkerList)
            {
                w.Stop();
            }
            mZipWorkerList.Clear();
        }
    }
}