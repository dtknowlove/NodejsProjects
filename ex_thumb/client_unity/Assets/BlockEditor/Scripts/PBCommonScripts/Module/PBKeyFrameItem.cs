/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Putao.PaiBloks.Common
{
    public class PBKeyFrameItem
    {
        public readonly KeyframeItemComplete OnComplete = new KeyframeItemComplete();
        
        private GameObject mTarget;
        private PPFrameItemInfo mInfo;
        private float mBlockMoveSpeed = 1;
        
        private bool mDirNotChange = true;
        private bool mPosNotChange = true;

        private Sequence animSequence = null;
        private bool mStopped = false;
        
        private bool mIsComplete = false;
        public bool IsComplete
        {
            get { return mIsComplete; }
        }

        public PPFrameItemInfo MovementInfo
        {
            get { return mInfo; }
        }

        public int Index
        {
            get { return mInfo.index; }
        }
        
        public bool IsUnit
        {
            get { return mInfo.IsUnit; }
        }

        public bool DirNotChange
        {
            get { return mDirNotChange; }
        }
                
        public bool PosNotChange
        {
            get { return mPosNotChange; }
        }

        public GameObject Target
        {
            get { return mTarget; }
        }
        
        public int KeyframeIndex { get; set; }
        public PBKeyFrameItem Previous { get; set; }
        public PBKeyFrameItem Next { get; set; }

        protected bool mHasNormalMove = false;
        protected List<PPMovementInfo> mNormalMoves = new List<PPMovementInfo>();
        protected bool mHasSplineMove = false;
        protected List<PPMovementInfo> mSplineMoves = new List<PPMovementInfo>();

        public virtual void Init(PPFrameItemInfo itemInfo, GameObject target)
        {
            mInfo = itemInfo;
            mTarget = target;
            
            mDirNotChange = true;
            mPosNotChange = true;
            foreach (PPMovementInfo movement in mInfo.movements)
            {
                if (movement.type == PPMovementType.Normal)
                {
                    if (mInfo.initAngle != movement.destAngle)
                    {
                        mDirNotChange = false;
                    }
                    if (mInfo.initPos != movement.destPosition)
                    {
                        mPosNotChange = false;
                    }
                }
            }

            mHasNormalMove = mHasSplineMove = false;
            mNormalMoves.Clear();
            mSplineMoves.Clear();
            foreach (PPMovementInfo movement in mInfo.movements)
            {
                if (movement.type == PPMovementType.Normal)
                {
                    mHasNormalMove = true;
                    mNormalMoves.Add(movement);
                }
                else if (movement.type == PPMovementType.Spline)
                {
                    mHasSplineMove = true;
                    mSplineMoves.Add(movement);
                }
            }
        }
        
        public void SetSpeed(float speed)
        {
            mBlockMoveSpeed = speed;
        }

        public void PlayWithoutAnim()
        {
            if (mHasNormalMove)
                ProcessNormal(mNormalMoves[mNormalMoves.Count - 1], 0, false);
            if (mHasSplineMove)
                ProcessSpline(mSplineMoves[mSplineMoves.Count - 1], 0, false);

            mIsComplete = true;
            OnComplete.Invoke(this);
        }

        /// <summary>
        /// fully play
        /// </summary>
        public virtual IEnumerator Play()
        {
            int moveIndex = 0;
            mStopped = false;
            bool completeSequence;

            while (moveIndex < mInfo.movements.Count)
            {
                PPMovementInfo movement = mInfo.movements[moveIndex];

                //相机跟随
                if (PBCamManager.Instance.IsCameraFollow)
                {
                    if (moveIndex == 0)
                    {
                        if (mInfo.targetId != 0)
                        {
                            if (IsUnit)
                            {
                                MoveCamera();
                                yield return new WaitForSeconds(0.5f * mBlockMoveSpeed);
                            }
                            else
                            {
                                PBCamManager.Instance.MoveToShowAllInPlay();
                            }
                        }
                    }
                }

                if (mStopped)
                    yield break;

                //零件动画
                completeSequence = false;
                switch (movement.type)
                {
                    case PPMovementType.Normal:
                        ProcessNormal(movement, mBlockMoveSpeed * movement.duration);
                        break;
                    case PPMovementType.Spline:
                        ProcessSpline(movement, mBlockMoveSpeed * movement.duration);
                        break;
                }
                animSequence.OnComplete(() => completeSequence = true);
                if (!animSequence.IsPlaying())
                    animSequence.Play();//ensure to play, because DoTween.defaultAutoplay are not certain.
                
                while (!completeSequence && !mStopped)
                    yield return null;

                if (mStopped)
                    yield break;
                
                moveIndex++;
            }

            mIsComplete = true;
            OnComplete.Invoke(this);
        }
        
        /// <summary>
        /// only play the last move
        /// </summary>
        public IEnumerator PlayFast()
        {
            bool completeSequence = false;
            if (mHasNormalMove)
            {
                ProcessNormal(mNormalMoves[mNormalMoves.Count - 1], mBlockMoveSpeed);
            }
            if (mHasSplineMove)
            {
                //做normalmove动画，就不做spline动画，否则做
                ProcessSpline(mSplineMoves[mSplineMoves.Count - 1], mBlockMoveSpeed, !mHasNormalMove);
            }
            
            animSequence.OnComplete(() => completeSequence = true);
            if (!animSequence.IsPlaying())
                animSequence.Play();//ensure to play, because DoTween.defaultAutoplay are not certain.
                
            while (!completeSequence && !mStopped)
                yield return null;

            if (!mStopped)
            {
                mIsComplete = true;
                OnComplete.Invoke(this);    
            }
        }

        protected void ProcessNormal(PPMovementInfo movement, float speed, bool withAnim = true)
        {
            if (!withAnim)
            {
                mTarget.transform.localPosition = movement.destPosition;
                mTarget.transform.localEulerAngles = movement.destAngle;
                return;
            }
            
            animSequence = DOTween.Sequence();
            animSequence.Append(mTarget.transform.DOLocalMove(movement.destPosition, speed).SetEase(Ease.OutCubic))
                .Join(mTarget.transform.DOLocalRotate(movement.destAngle, speed));
        }

        protected void ProcessSpline(PPMovementInfo movement, float speed, bool withAnim = true)
        {
            if (!withAnim)
            {
                mTarget.GetComponentInChildren<IPBSplineAnimator>().SetKeyframe(movement.destKeyframe);
                return;
            }

            IPBSplineAnimator animator = mTarget.GetComponentInChildren<IPBSplineAnimator>();
            animSequence = animator.Play(movement.destKeyframe, speed, null);
        }

        public virtual void Reset(bool complete = false)
        {
            mIsComplete = complete;

            if (mHasSplineMove)
            {
                //先将物体至于目标位置，进行spline计算，后再设回初始位置
                //因为spline计算可能依赖于目标位置，如track（履带）
                if (mHasNormalMove)
                {
                    var finalMove = mNormalMoves[mNormalMoves.Count - 1];
                    mTarget.transform.localPosition = finalMove.destPosition;
                    mTarget.transform.localEulerAngles = finalMove.destAngle;
                }

                //reset spline
                mTarget.GetComponentInChildren<IPBSplineAnimator>().ResetToStartOfKeyframe(mSplineMoves[0].destKeyframe);
            }

            mTarget.transform.localPosition = mInfo.initPos;
            mTarget.transform.localEulerAngles = mInfo.initAngle;
        }

        public void Stop()
        {
            if (animSequence != null)
                animSequence.Kill(false);
            animSequence = null;

            mStopped = true;
        }

        protected void MoveCamera(bool withouAnim = false)
        {
            bool forceMove = Index == 0;
            
            Vector3 destPos = mInfo.movements[mInfo.movements.Count - 1].destPosition;
            if (mTarget.GetComponent<PBSection>() == null)
            {
                if (VectorUtil.AlmostEqual(destPos, mTarget.transform.localPosition, 0.01f))
                {
                    return;
                }

                PBCamManager.Instance.MoveToByKeyframe(mTarget.transform.parent.TransformPoint(destPos), forceMove, withouAnim);
            }
            else
            {
                if (VectorUtil.AlmostEqual(destPos, mTarget.transform.localPosition, 0.01f))
                {
                    return;
                }

                PBBlock[] blocks = mTarget.GetComponentsInChildren<PBBlock>();
                if (blocks.Length > 0)
                {
                    Vector3 dest = Vector3.zero;
                    for (int i = 0; i < blocks.Length; i++)
                    {
                        dest += blocks[i].transform.position;
                    }
                    dest /= blocks.Length;
                    Vector3 delta = dest - mTarget.transform.position;
                    PBCamManager.Instance.MoveToByKeyframe(
                        mTarget.transform.parent.TransformPoint(destPos) + delta, forceMove, withouAnim);
                }
            }
        }
    }
}