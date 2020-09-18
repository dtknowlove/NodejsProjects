/****************************************************************************
 * Copyright (c) 2018 ptgame@putao.com
 ****************************************************************************/


using System.Collections;
using PTGame.Core;
using UnityEngine;
using UnityEngine.Events; 

namespace PTGame.ResKit
{
    public abstract class AbstractRes : IPoolable
    {
        public ResName Name { get; protected set; }
        public eResState State { get; protected set; }

        protected object mAsset;
        protected AsyncOperation mLoadRequest;

        public object Asset
        {
            get { return mAsset; }
        }

        public eResType ResType
        {
            get { return Name.ResType; }
        }

        public float Progress
        {
            get
            {
                if (State == eResState.Ready)
                    return 1;
                return mLoadRequest == null ? 0 : mLoadRequest.progress;
            }
        }

        public void Init(ResName resName)
        {
            Name = resName;
            State = eResState.Waiting;
            RefCount = 0;
            OnInit();
        }

        protected virtual void OnInit() {}

        #region RefCounter

        public int RefCount { get; private set; }

        public void AddRef() { ++RefCount; }

        public void SubRef()
        {
            if (RefCount == 0)
                return;

            RefCount--;
            if (RefCount == 0)
            {
                ReleaseRes();
            }
        }

        #endregion
        

        #region Recycle

        public bool IsRecycled { get; set; }
        
        public virtual void OnRecycled()
        {
            Name = null;
            State = eResState.Waiting;
            RefCount = 0;
            mResLoadEvent.RemoveAllListeners();
        }

        #endregion
        

        #region Event

        private readonly ResLoadEvent mResLoadEvent = new ResLoadEvent();
        
        public void RegisterListener(UnityAction<bool, AbstractRes> listener)
        {
            if (State == eResState.Ready)
            {
                listener(true, this);
                return;
            }
            mResLoadEvent.AddListener(listener);
        }

        public void RemoveListener(UnityAction<bool, AbstractRes> listener)
        {
            mResLoadEvent.RemoveListener(listener);
        }
        
        #endregion
        
        
        #region Dependecies

        public virtual ResName[] GetDependencies()
        {
            return null;
        }

        protected void HoldDependency()
        {
            ResName[] depends = GetDependencies();
            if (depends == null || depends.Length == 0)
                return;

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.Instance.GetRes(depends[i]);
                if (res != null)
                {
                    res.AddRef();
                }
            }
        }

        protected void UnHoldDependency()
        {
            ResName[] depends = GetDependencies();
            if (depends == null || depends.Length == 0)
                return;

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.Instance.GetRes(depends[i]);
                if (res != null)
                {
                    res.SubRef();
                }
            }
        }
        
        public bool IsDependencyLoadFinish()
        {
            ResName[] depends = GetDependencies();
            if (depends == null || depends.Length == 0)
                return true;

            for (int i = depends.Length - 1; i >= 0; --i)
            {
                var res = ResMgr.Instance.GetRes(depends[i]);
                if (res == null || res.State != eResState.Ready)
                    return false;
            }
            return true;
        }
        
        #endregion 
        
        
        #region ResLoad

        public bool LoadSync()
        {
            if (State == eResState.Waiting)
            {
                State = eResState.Loading;
                return OnLoadSync();
            }
            return false;
        }

        protected ResTask mLoadAsyncTask = null;

        public void LoadAsync()
        {
            if (State == eResState.Waiting)
            {
                State = eResState.Loading;
                ResMgr.Instance.PostResTask(new ResTask(OnLoadAsync()));
            }
        }
        
        protected void OnLoadFail()
        {
            State = eResState.Waiting;
            mResLoadEvent.Invoke(false, this);
        }

        protected void OnLoadSuccess()
        {
            State = eResState.Ready;
            mResLoadEvent.Invoke(true, this);
        }

        public bool ReleaseRes()
        {
            if (State == eResState.Loading)
                return false;
            
            if (State == eResState.Waiting)
                return true;

            OnReleaseRes();

            State = eResState.Waiting;
            mResLoadEvent.RemoveAllListeners();
            return true;
        }

        protected abstract bool OnLoadSync();
        protected abstract IEnumerator OnLoadAsync();
        protected abstract void OnReleaseRes();

        #endregion
    }

}

