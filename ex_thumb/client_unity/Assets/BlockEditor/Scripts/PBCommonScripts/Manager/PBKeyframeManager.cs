/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Putao.PaiBloks.Common
{
	public class PBKeyframeManager : MonoBehaviour
	{
#if BLOCK_EDITOR
		private static PBKeyframeManager instance;
		public static PBKeyframeManager Instance { get { return instance; } }

		void Awake()
		{
			if (instance != null)
				throw new Exception("There already exists one PBKeyframeManager in the scene!!!");
			instance = this;
		}

		void OnDestroy()
		{
			instance = null;
		}
#endif
		public KeyframeCompleteAll OnPlayComplete = new KeyframeCompleteAll();
		
		private List<PBKeyFrame> mKeyFrames = new List<PBKeyFrame>();
		private int mKeyFrameIndex = 0;
		private bool mIsAutoPlay = false;
		private int mCompleteCount = 0;

		public bool IsComplete
		{
			get { return mCompleteCount == mKeyFrames.Count; }
		}
		
		public int CurKeyFrameIndex
		{
			get { return mKeyFrameIndex; }
			set { mKeyFrameIndex = value; }
		}

		public PBKeyFrame CurKeyFrame
		{
			get { return mKeyFrameIndex >= 0 && mKeyFrameIndex < mKeyFrames.Count ? mKeyFrames[mKeyFrameIndex] : null; }
		}

		public List<PBKeyFrame> KeyFrames
		{
			get{ return mKeyFrames;}
		}

		/// <summary>
		/// 要减1，因为第1帧不算动画，从第2帧（index=1）开始播
		/// </summary>
		public int AllKeyCount
		{
			get { return mKeyFrames.Count - 1; }
		}

		public bool AutoPlay
		{
			get { return mIsAutoPlay; }
			set { mIsAutoPlay = value; }
		}

		void Update ()
		{
			if (mIsAutoPlay)
			{
				PlayNext(true);
			}
		}

		public void Init(List<PPKeyFrameInfo> keyFrameInfos, Dictionary<int, GameObject> targets)
		{
			mKeyFrames.Clear();
			mKeyFrameIndex = 0;
			mCompleteCount = 0;
			OnPlayComplete.RemoveAllListeners();

			foreach (PPKeyFrameInfo info in keyFrameInfos)
			{
				PBKeyFrame keyFrame = new PBKeyFrame();
				mKeyFrames.Add(keyFrame);
				keyFrame.Init(info, targets);
				keyFrame.OnComplete.AddListener(OnKeyframePlayComplete);
			}

			for (int i = mKeyFrames.Count - 1; i >= 0; i--)
			{
				//倒着reset，确保reset到最初的状态，否则后面的会覆盖掉前面的（针对target出现在多个frame中）
				mKeyFrames[i].Reset();
				
				if (i > 0)
					mKeyFrames[i].Previous = mKeyFrames[i - 1];
				if (i < mKeyFrames.Count - 1)
					mKeyFrames[i].Next = mKeyFrames[i + 1];
			}
		}

		public void Reset(int fromIndex = 2)
		{
			for (int i = mKeyFrames.Count - 1; i >= fromIndex; i--)
			{
				//倒着reset，确保reset到最初的状态，否则后面的会覆盖掉前面的（针对target出现在多个frame中）
				mKeyFrames[i].Reset();
			}
		}

		public void ResetIndex()
		{
			mKeyFrameIndex = 0;
		}

		public bool EnablePlayNext()
		{
			if (mKeyFrameIndex + 1 >= mKeyFrames.Count)
				return false;
			if (!CurKeyFrame.IsComplete)
				return false;

			return true;
		}

		public bool PlayNext(bool isAutoPlay = false, bool isFastPlay = false)
		{
			if (!isAutoPlay)
				mIsAutoPlay = false;
			
			//强制生效第0帧，因为第0帧不需要播放动画
			if (mKeyFrameIndex == 0 && !mKeyFrames[0].IsComplete)
				mKeyFrames[0].PlayWithoutAnim();

			if (!EnablePlayNext())
				return false;

			mKeyFrameIndex += 1;

			var curKeyframe = mKeyFrames[mKeyFrameIndex];
			curKeyframe.Reset();
			StartCoroutine(!isFastPlay ? curKeyframe.Play() : curKeyframe.PlayFast());
			
//			Debug.Log("当前播放步骤：" + mKeyFrameIndex);
			return true;
		}

		public bool EnablePlayLast()
		{
			if (mKeyFrameIndex - 1 < 0)
				return false;
			if (!CurKeyFrame.IsComplete)
				return false;
			return true;
		}

		public bool PlayLast(bool isFastPlay = false)
		{
			mIsAutoPlay = false;
			
			if (!EnablePlayLast())
				return false;

			CurKeyFrame.Reset();

			mKeyFrameIndex -= 1;
			
			var curKeyframe = mKeyFrames[mKeyFrameIndex];
			curKeyframe.Reset();
			if (mKeyFrameIndex > 0)
				StartCoroutine(!isFastPlay ? curKeyframe.Play() : curKeyframe.PlayFast());
			else 
				curKeyframe.PlayWithoutAnim();//第0帧不需要动画
			
			Debug.Log("当前播放步骤："+mKeyFrameIndex);
			return true;
		}

		public bool PlayWithOutAnim(int index)
		{
			mIsAutoPlay = false;

			if (index < 0 || index >= mKeyFrames.Count) return false;
			if (index == mKeyFrameIndex) return false;

			mKeyFrameIndex = index;

			for (int i = mKeyFrames.Count - 1; i >= index + 1; i--)
			{
				//倒着reset，确保reset到最初的状态，否则后面的会覆盖掉前面的（针对target出现在多个frame中）
				if (mKeyFrames[i].IsComplete)
					mKeyFrames[i].Reset();
			}

			for (int i = 0; i < index; i++)
			{
				if (!mKeyFrames[i].IsComplete)
					mKeyFrames[i].PlayWithoutAnim();
			}
			
			var curKeyframe = mKeyFrames[mKeyFrameIndex];
			curKeyframe.Reset();
			curKeyframe.PlayWithoutAnim();
//			Debug.Log("当前播放步骤：" + mKeyFrameIndex);
			return true;
		}

		public void ResetCurFrameToEditorState()
		{
			Stop();
			PlayWithOutAnim(mKeyFrameIndex);
		}

		public void ResetCurFrameToInitState()
		{
			Stop();
			mKeyFrames[mKeyFrameIndex].Reset(true);
		}
		
		public void Stop()
		{
			mKeyFrames[mKeyFrameIndex].Stop();
		}

		public bool RePlay()
		{
			if (mKeyFrameIndex <= 0 || mKeyFrameIndex >= mKeyFrames.Count)
				return false;

			var curKeyframe = mKeyFrames[mKeyFrameIndex];
			if (!curKeyframe.IsComplete)
				return false;
			curKeyframe.Reset();
			StartCoroutine(curKeyframe.Play());
			return true;
		}

		public void SetSpeed(float speed)
		{
			foreach (PBKeyFrame keyFrame in mKeyFrames)
			{
				keyFrame.SetSpeed(speed);
			}
		}

		private void OnKeyframePlayComplete(PBKeyFrame keyFrame)
		{
			mCompleteCount = keyFrame.Index + 1;
			if (mCompleteCount == mKeyFrames.Count)
				OnPlayComplete.Invoke();
		}

		#region keyframe event

		/// <summary>
		/// 注册当前步骤开始前事件
		/// </summary>
		public void RegisterKeyframeStart(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				keyFrame.OnStart.AddListener(action);
			}
		}
		/// <summary>
		/// 移除当前步骤开始前事件
		/// </summary>
		public void RemoveKeyframeStart(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				keyFrame.OnStart.RemoveListener(action);
			}
		}
		
		/// <summary>
		/// 注册当前步骤完成时事件
		/// </summary>
		public void RegisterKeyframeComplete(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				keyFrame.OnComplete.AddListener(action);
			}
		}
		
		/// <summary>
		/// 移除当前步骤完成时事件
		/// </summary>
		public void RemoveKeyframeComplete(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				keyFrame.OnComplete.RemoveListener(action);
			}
		}
		
		/// <summary>
		/// 注册当前步骤每一个target动画完成时事件
		/// </summary>
		public void RegisterKeyframeItemComplete(UnityAction<PBKeyFrameItem> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				foreach (PBKeyFrameItem item in keyFrame.KeyFrameItems)
				{
					item.OnComplete.AddListener(action);
				}
			}
		}
		
		/// <summary>
		/// 移除当前步骤每一个target动画完成时事件
		/// </summary>
		public void RemoveKeyframeItemComplete(UnityAction<PBKeyFrameItem> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				foreach (PBKeyFrameItem item in keyFrame.KeyFrameItems)
				{
					item.OnComplete.RemoveListener(action);
				}
			}
		}

		/// <summary>
		/// 注册全部步骤完成时事件
		/// </summary>
		public void RegisterCompleteAll(UnityAction action)
		{
			OnPlayComplete.AddListener(action);
		}
		
		/// <summary>
		/// 移除全部步骤完成时事件
		/// </summary>
		public void RemoveCompleteAll(UnityAction action)
		{
			OnPlayComplete.RemoveListener(action);
		}

		/// <summary>
		/// 注册到达节点事件
		/// </summary>
		public void RegisterMeetSectionPoint(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				if (keyFrame.IsSectionPoint)
					keyFrame.OnComplete.AddListener(action);
			}
		}

		/// <summary>
		/// 移除到达节点事件
		/// </summary>
		public void RemoveMeetSectionPoint(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				if (keyFrame.IsSectionPoint)
					keyFrame.OnComplete.RemoveListener(action);
			}
		}

		/// <summary>
		/// 注册当前步骤的reset事件
		/// </summary>
		public void RegisterKeyframeReset(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				keyFrame.OnReset.AddListener(action);
			}
		}

		/// <summary>
		/// 移除当前步骤的reset事件
		/// </summary>
		public void RemoveKeyframeReset(UnityAction<PBKeyFrame> action)
		{
			foreach (PBKeyFrame keyFrame in KeyFrames)
			{
				keyFrame.OnReset.RemoveListener(action);
			}
		}

		#endregion
	}
}
