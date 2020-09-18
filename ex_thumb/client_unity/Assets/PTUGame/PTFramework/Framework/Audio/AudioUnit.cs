/****************************************************************************
 * 2018.8 maoling@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;
namespace PTGame.Framework
{
    using System;
    using UnityEngine;
    using PTGame.ResKit;

    public class AudioUnit : IPoolable, IPoolType
    {
        private ResLoader mLoader;
		public AudioSource mSource;
        public string mAudioName;
        public string mABName;
        private bool mUsedCache = true;
        
        private AudioClip mAudioClip;

        private bool mIsPause = false;
        private Action<AudioUnit> mOnStartListener;
        private Action<AudioUnit> mOnFinishListener;
        
        private Coroutine delayCoroutine;

        public static AudioUnit Allocate()
        {
            return SafeObjectPool<AudioUnit>.Instance.Allocate();
        }
        
        public bool UsedCache
        {
            get { return mUsedCache; }
            set { mUsedCache = false; }
        }

        public bool IsRecycled { get; set; }

        public void SetOnStartListener(Action<AudioUnit> l)
        {
            mOnStartListener = l;
        }

        public void SetOnFinishListener(Action<AudioUnit> l)
        {
            mOnFinishListener = l;
        }

        public void SetVolume(float volume)
        {
            if (mSource != null)
            {
                mSource.volume = volume;
            }
        }

        public void SetAudio(GameObject root, string name, string abName = null, bool syncLoad = false, bool loop = false, float volume = 1.0f)
        {
            if (string.IsNullOrEmpty(name))
                return;
            
            //ensure the last delay action is stopped
            if (delayCoroutine != null)
            {
                AudioManager.Instance.StopCoroutine(delayCoroutine);
                delayCoroutine = null;
            }

            if (mSource == null)
                mSource = root.AddComponent<AudioSource>();
            mSource.loop = loop;
            mSource.volume = volume;
            
            if (string.Equals(mAudioName, name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(mABName, abName, StringComparison.OrdinalIgnoreCase))
            {
                if (mAudioClip != null)
                {
                    PlayAudioClip();
                }
                return;
            }

            //防止卸载后立马加载的情况
            var preLoader = mLoader;
            mLoader = null;
            CleanResources();

            mLoader = ResLoader.Allocate();

            mAudioName = name;
            mABName = abName;

            if (syncLoad)
                OnResLoadFinish(true, mLoader.LoadSync<AudioClip>(name, abName));
            else
                mLoader.LoadAsync<AudioClip>(name, abName, OnResLoadFinish);

            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }
        }

        public void Stop()
        {
            Release();
        }

        public void Pause()
        {
            if (mIsPause || !mSource)
                return;

            mIsPause = true;
            mSource.Pause();
        }

        public void Resume()
        {
            if (!mIsPause || !mSource)
                return;

            mIsPause = false;
            mSource.Play();
        }

        private void OnResLoadFinish(bool result, AudioClip res)
        {
            if (!result)
            {
                Release();
                return;
            }

            mAudioClip = res;

            if (mAudioClip == null)
            {
                PTDebug.LogError("Asset Is Invalid AudioClip:" + mAudioName);
                Release();
                return;
            }

            PlayAudioClip();
        }

        private void PlayAudioClip()
        {
            mIsPause = false;
            mSource.Stop();
            mSource.clip = mAudioClip;

            var delayNode = new DelayAction(Mathf.Max(mAudioClip.length, 0.5f)) {OnEndedCallback = OnSoundPlayFinish};
            delayCoroutine = AudioManager.Instance.StartCoroutine(delayNode.Execute());

            if (null != mOnStartListener)
            {
                mOnStartListener(this);
            }

            mSource.Play();
        }

        private void OnSoundPlayFinish()
        {
            if (mOnFinishListener != null)
            {
                mOnFinishListener(this);
            }

            if (!mSource.loop)
            {
                Release();
            }
        }

        private void Release()
        {
            if (mUsedCache)
            {
                Recycle2Cache();
            }
            else
            {
                CleanResources();    
            }
        }

        private void CleanResources()
        {
            mAudioName = null;
            mABName = null;

            mIsPause = false;
            mOnFinishListener = null;
            mOnStartListener = null;

            if (mSource != null && mSource.clip != null)
            {
                if (mSource.clip == mAudioClip)
                {
                    mSource.Stop();
                    mSource.clip = null;
                }
            }

            mAudioClip = null;

            if (mLoader != null)
            {
                mLoader.Recycle2Cache();
                mLoader = null;
            }
        }

        public void OnRecycled()
        {
            CleanResources();
        }

        public void Recycle2Cache()
        {
            if (!SafeObjectPool<AudioUnit>.Instance.Recycle(this))
            {
                if (mSource != null)
                {
                    GameObject.Destroy(mSource);
                    mSource = null;
                }
            }
        }
    }
}