/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Putao.PaiBloks.Common
{
    public class PBKeyFrame
    {
        public List<PBKeyFrameItem> KeyFrameItems { get; private set; }
        public readonly KeyframeStart OnStart = new KeyframeStart();
        public readonly KeyframeComplete OnComplete = new KeyframeComplete();
        public readonly KeyframeReset OnReset = new KeyframeReset();

        public PBKeyFrame Previous { get; set; }
        public PBKeyFrame Next { get; set; }

        private PPKeyFrameInfo mInfo;
        private bool mIsComplete = false;
        private bool mStopped = false;

        /// <summary>
        /// 当前正在播放的小步
        /// </summary>
        public int CurItemIndex { get; set; }

        public int ItemCount
        {
            get { return KeyFrameItems.Count; }
        }

        /// <summary>
        /// 节点信息
        /// </summary>
        public PBPointInfo PointInfo
        {
            get { return mInfo.pointInfo; }
        }

        /// <summary>
        /// 是否是节点
        /// </summary>
        public bool IsSectionPoint
        {
            get { return mInfo.pointInfo != null && mInfo.pointInfo.PointType == PBPointType.SectionPoint; }
        }

        /// <summary>
        /// 是否是特殊说明
        /// </summary>
        public bool IsGuidePoint
        {
            get { return mInfo.pointInfo != null && mInfo.pointInfo.PointType == PBPointType.GuidePoint && mInfo.pointInfo.HasTipInfo; }
        }

        public int Index
        {
            get { return mInfo.index; }
        }

        public void Init(PPKeyFrameInfo keyFrameInfo, Dictionary<int, GameObject> targets)
        {
            mInfo = keyFrameInfo;
            KeyFrameItems = new List<PBKeyFrameItem>();

            foreach (PPFrameItemInfo itemInfo in mInfo.itemInfos)
            {
#if !BLOCK_EDITOR
                PBKeyFrameItem frameItem = new PBKeyFrameItem();
#else
                PBKeyFrameItem frameItem = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals(BlockEditorGlobal.SCENE_PROSPECTUS)
                    ? new PBKeyFrameItem_Prospectus()
                    : new PBKeyFrameItem();
#endif
                frameItem.Init(itemInfo, targets[itemInfo.targetId]);
                frameItem.KeyframeIndex = Index;
                KeyFrameItems.Add(frameItem);
            }

            for (int i = 0; i < KeyFrameItems.Count; i++)
            {
                if (i > 0)
                    KeyFrameItems[i].Previous = KeyFrameItems[i - 1];
                if (i < KeyFrameItems.Count - 1)
                    KeyFrameItems[i].Next = KeyFrameItems[i + 1];
            }
        }

        public IEnumerator Play()
        {
            mIsComplete = false;
            mStopped = false;
            OnStart.Invoke(this);

            foreach (var item in KeyFrameItems)
            {
                CurItemIndex = item.Index;
                yield return item.Play();

                if (mStopped) 
                    yield break;
            }

            mIsComplete = true;
            OnComplete.Invoke(this);
        }

        public IEnumerator PlayFast()
        {
            mIsComplete = false;
            mStopped = false;
            OnStart.Invoke(this);

            foreach (var item in KeyFrameItems)
            {
                CurItemIndex = item.Index;
                yield return item.PlayFast();

                if (mStopped) 
                    yield break;
            }

            mIsComplete = true;
            OnComplete.Invoke(this);
        }

        public void PlayWithoutAnim()
        {
            mIsComplete = false;
            OnStart.Invoke(this);
            
            foreach (var item in KeyFrameItems)
            {
                item.PlayWithoutAnim();
            }
            
            mIsComplete = true;
            OnComplete.Invoke(this);
        }

        public void Reset(bool complete = false)
        {
            mIsComplete = false;
            foreach (var item in KeyFrameItems)
            {
                item.Reset(complete);
            }
            OnReset.Invoke(this);
        }

        public void Stop()
        {
            mStopped = true;
            for (int i = 0; i < KeyFrameItems.Count; i++)
            {
                KeyFrameItems[i].Stop();
            }
        }

        public void SetSpeed(float speed)
        {
            foreach (var item in KeyFrameItems)
            {
                item.SetSpeed(speed);
            }
        }
        
        private List<MeshRenderer> mPoss = new List<MeshRenderer>();
        public Vector3 GetObjCenter()
        {
            if (!IsUnit)
                return Vector3.zero;
            mPoss.Clear();
            foreach (PBKeyFrameItem t in KeyFrameItems)
            {
                if (t.Target.GetComponent<PBBlock>())
                {
                    mPoss.AddRange(t.Target.GetComponentsInChildren<MeshRenderer>());
                }
            }
            if (mPoss.Count < 0)
                return Vector3.zero;
            var bounds = new Bounds();
            foreach (var t in mPoss)
            {
                bounds.Encapsulate(t.bounds);
            }
            return bounds.center;
        }

        public bool IsComplete
        {
            get { return mIsComplete; }
        }

        public bool IsUnit
        {
            get { return KeyFrameItems.Exists(i => i.IsUnit); }
        }
    }
}