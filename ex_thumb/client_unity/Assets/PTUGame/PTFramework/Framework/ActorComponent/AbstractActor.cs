/****************************************************************************
 * Copyright (c) 2017 ouyanggongping@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
    using System;
    using UnityEngine;

    using System.Collections.Generic;

    public class AbstractActor : MonoBehaviour
    {
        [SerializeField] private List<string> mComponentsNameList = new List<string>();
        private bool mHasAwake = false;
        private bool mHasStart = false;
        private readonly List<IActorComponent> mComponentList = new List<IActorComponent>();
        private PTEventSystem mEventSystem;

        #region MonoBehaviour

        private void Awake()
        {
            OnActorAwake();
                
            mComponentList.ForEachReverse(AwakeCom);

            mHasAwake = true;
        }

        private void Start()
        {
            OnActorStart();

            mComponentList.ForEachReverse(StartComponent);

            mHasAwake = true;
        }

        //关于Update的优化方案，可以后续再做
        private void Update()
        {
            UpdateAllComponents();
        }

        private void LateUpdate()
        {
            LateUpdateAllComponents();
        }

        private void OnDestroy()
        {
            mComponentList.ForEachReverse(DestroyComponent);

            mComponentList.Clear();
            mComponentsNameList.Clear();

            OnActorDestroy();
        }

        #endregion

        #region Public

        public PTEventSystem EventSystem
        {
            get
            {
                if (mEventSystem == null)
                {
                    mEventSystem = SafeObjectPool<PTEventSystem>.Instance.Allocate();
                }
                return mEventSystem;
            }
        }

        public void AddComponent(IActorComponent actorComponent)
        {
            if (actorComponent == null)
            {
                return;
            }

            if (GetActorComponent(actorComponent.GetType()) != null)
            {
                PTDebug.LogWarning("Already Add Component:" + name);
                return;
            }

            mComponentList.Add(actorComponent);

            mComponentsNameList.Add(actorComponent.GetType().Name);

            mComponentList.Sort(ComWrapComparison);

            OnAddComponent(actorComponent);

            if (mHasAwake)
            {
                AwakeCom(actorComponent);
            }

            if (mHasStart)
            {
                StartComponent(actorComponent);
            }
        }

        public T AddComponent<T>() where T : IActorComponent, new()
        {
            T com = new T();
            AddComponent(com);
            return com;
        }

        public T AddMonoCom<T>() where T : MonoBehaviour, IActorComponent
        {
            var com = gameObject.AddComponent<T>();
            AddComponent(com);
            return com;
        }

        public void RemoveComponent<T>() where T : IActorComponent
        {
            for (int i = mComponentList.Count - 1; i >= 0; --i)
            {
                if (mComponentList[i] is T)
                {
                    IActorComponent actorComponent = mComponentList[i];

                    mComponentList.RemoveAt(i);
                    mComponentsNameList.RemoveAt(i);
                    OnRemoveComponent(actorComponent);

                    DestroyComponent(actorComponent);
                    return;
                }
            }
        }

        public void RemoveComponent(IActorComponent actorComponent)
        {
            int index = mComponentList.FindIndex(component => component == actorComponent);
            mComponentList.RemoveAt(index);
            mComponentsNameList.RemoveAt(index);
            OnRemoveComponent(actorComponent);
            DestroyComponent(actorComponent);
        }

        public T GetActorComponent<T>() where T : IActorComponent
        {
            return (T)mComponentList.Find(component => component is T);
        }

        #endregion

        #region Private

        private IActorComponent GetActorComponent(Type t)
        {
            return mComponentList.Find(component => component.GetType() == t);
        }

        //这玩意会产生alloac
        protected void ProcessAllCom(Action<IActorComponent> process)
        {
            mComponentList.ForEachReverse(component => process(component));
        }

        protected void LateUpdateAllComponents()
        {
            mComponentList.ForEachReverse(component => component.OnComponentLateUpdate(Time.deltaTime));
        }

        protected void UpdateAllComponents()
        {
            mComponentList.ForEachReverse(component => component.OnComponentUpdate(Time.deltaTime));
        }

        protected void AwakeCom(IActorComponent wrap)
        {
            wrap.AwakeComponent(this);
        }

        protected void StartComponent(IActorComponent wrap)
        {
            wrap.OnComponentStart();
        }

        protected void DestroyComponent(IActorComponent wrap)
        {
            wrap.DestroyComponent();
        }

        private int ComWrapComparison(IActorComponent a, IActorComponent b)
        {
            return a.ComponentOrder.CompareTo(b.ComponentOrder);
        }

        protected virtual void OnActorAwake()
        {

        }

        protected virtual void OnActorStart()
        {

        }

        protected virtual void OnActorDestroy()
        {

        }

        protected virtual void OnAddComponent(IActorComponent actorComponent)
        {

        }

        protected virtual void OnRemoveComponent(IActorComponent actorComponent)
        {

        }

        #endregion
    }
}
