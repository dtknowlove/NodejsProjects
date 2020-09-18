/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Framework
{
	/// <summary>
	/// 动画播放控件
	/// http://www.cnblogs.com/mrblue/p/5191183.html
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class UISpriteAnimation : MonoBehaviour
	{
		private Image mImageSource;
		private int mCurFrame = 0;
		private float mDelta = 0;

		public float FPS = 5;
		public List<Sprite> SpriteFrames;
		public bool IsPlaying = false;
		public bool Forward = true;
		public bool AutoPlay = false;
		public bool Loop = false;

		public int FrameCount
		{
			get { return SpriteFrames.Count; }
		}

		public bool IsEnd
		{
			get 
			{return isEnd; }
		}
		private bool isEnd = false;
		void Awake()
		{
			mImageSource = GetComponent<Image>();
		}

		void Start()
		{
			if (AutoPlay)
			{
				Play();
			}
			else
			{
				IsPlaying = false;
			}
		}

		private void SetSprite(int idx)
		{
			mImageSource.sprite = SpriteFrames[idx];
			mImageSource.SetNativeSize();
		}

		public void Play()
		{
			IsPlaying = true;
			Forward = true;
		}

		public void PlayReverse()
		{
			IsPlaying = true;
			Forward = false;
		}



		void Update()
		{
			if (!IsPlaying || 0 == FrameCount)
			{
				isEnd = true;
				return;
			}

			mDelta += Time.deltaTime;
			if (mDelta > 1 / FPS)
			{
				mDelta = 0;
				if (Forward)
				{
					mCurFrame++;
				}
				else
				{
					mCurFrame--;
				}

				if (mCurFrame >= FrameCount)
				{
					if (Loop)
					{
						mCurFrame = 0;
					}
					else
					{
						IsPlaying = false;
						return;
					}
				}
				else if (mCurFrame < 0)
				{
					if (Loop)
					{
						mCurFrame = FrameCount - 1;
					}
					else
					{
						IsPlaying = false;
						return;
					}
				}

				SetSprite(mCurFrame);
			}
		}

		public void Pause()
		{
			IsPlaying = false;
		}

		public void Resume()
		{
			if (!IsPlaying)
			{
				IsPlaying = true;
			}
		}

		public void Stop()
		{
			mCurFrame = 0;
			SetSprite(mCurFrame);
			IsPlaying = false;
		}

		public void Rewind()
		{
			mCurFrame = 0;
			SetSprite(mCurFrame);
			Play();
		}
	}
}