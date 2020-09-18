/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 * 2018.11 modified maoling
 ****************************************************************************/

using PTGame.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PTGame.Framework
{
	public class UIDragEventInScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public enum State
		{
			Idle,
			Scroll,
			Drag
		}

		private State mCurState = State.Idle;

		private UnityAction mOnDragBeganEvent;
		private UnityAction mOnDragEvent;
		private UnityAction mOnDragEndedEvent;
		private UnityAction mOnScrollBeganEvent;
		private UnityAction mOnScrollEvent;
		private UnityAction mOnScrollEndedEvent;

		public ScrollRect ScrollRect;


		[SerializeField] private bool mIdleStateWhenDragEnd = false;
		
		public void SetIdleStateWhenDragEnd()
		{
			mIdleStateWhenDragEnd = true;
		}

		public void SetState(State state)
		{
			mCurState = state;
		}

		public void RegisterOnDragBeganEvent(UnityAction onDragBegan)
		{
			mOnDragBeganEvent = onDragBegan;
		}

		public void RegisterOnDragEvent(UnityAction onDragEvent)
		{
			mOnDragEvent = onDragEvent;
		}

		public void RegisterOnDragEndedEvent(UnityAction onDragEndedEvent)
		{
			mOnDragEndedEvent = onDragEndedEvent;
		}
		
		public void RegisterOnScrollBeganEvent(UnityAction onScrollBeganEvent)
		{
			mOnScrollBeganEvent = onScrollBeganEvent;
		}

		public void RegisterOnScrollEvent(UnityAction onScrollEvent)
		{
			mOnScrollEvent = onScrollEvent;
		}

		public void RegisterOnScrollEndedEvent(UnityAction onScrollEndedEvent)
		{
			mOnScrollEndedEvent = onScrollEndedEvent;
		}

		private Vector2 mDragBeganMousePos;

		private bool mCurrentDragCalculated;

		/// <summary>
		/// check if drag valid
		/// </summary>
		private bool mDragBegan;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{ 
			if (!PTUIEventUtil.Interactable(gameObject)) return;
			UIEventLockManager.Instance.SendMsg(new UILockObjEventMsg(gameObject));

			mCurrentDragCalculated = false;
			mDragBeganMousePos = (transform.parent as RectTransform).GetLocalPosInRect();
			mDragBegan = true;
		}


		private bool CheckBeganDragged(Vector2 offsetFromBegan)
		{
			if (ScrollRect.vertical)
			{
				return Mathf.Abs(offsetFromBegan.x) > Mathf.Abs(offsetFromBegan.y) * 0.25f;
			}

			if (ScrollRect.horizontal)
			{
				return Mathf.Abs(offsetFromBegan.y) > Mathf.Abs(offsetFromBegan.x) * 0.25f;
			}

			return false;
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (!PTUIEventUtil.Interactable(gameObject) || !mDragBegan) return;

			if (!mCurrentDragCalculated && mCurState == State.Idle)
			{
				var offsetFromBegan = (transform.parent as RectTransform).GetLocalPosInRect() - mDragBeganMousePos;
				if (offsetFromBegan.magnitude > 10)
				{
					mCurrentDragCalculated = true;

					if (CheckBeganDragged(offsetFromBegan))
					{
						mCurState = State.Drag;
						mOnDragBeganEvent.InvokeGracefully();
					}
					else
					{
						mCurState = State.Scroll;
						ExecuteEvents.Execute(ScrollRect.gameObject, eventData,
							delegate(ScrollRect handler, BaseEventData data) { handler.OnBeginDrag(data as PointerEventData); });
						mOnScrollBeganEvent.InvokeGracefully();

					}

					return;
				}
			}
			else if (!mCurrentDragCalculated && mCurState == State.Drag)
			{
				mOnDragBeganEvent.InvokeGracefully();
				mCurrentDragCalculated = true;
				return;
			}

			switch (mCurState)
			{
				case State.Drag:
					mOnDragEvent.InvokeGracefully();
					break;
				case State.Scroll:
					mOnScrollEvent.InvokeGracefully();
					ExecuteEvents.Execute(ScrollRect.gameObject, eventData,
						delegate(ScrollRect handler, BaseEventData data) { handler.OnDrag(data as PointerEventData); });
					break;
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (!PTUIEventUtil.Interactable(gameObject) || !mDragBegan) return;

			UIEventLockManager.Instance.SendMsg(new UIUnlockObjEventMsg(gameObject));

			if (mCurState == State.Scroll)
			{
				mOnScrollEndedEvent.InvokeGracefully();
				ExecuteEvents.Execute(ScrollRect.gameObject, eventData,
					delegate(ScrollRect handler, BaseEventData data) { handler.OnEndDrag(data as PointerEventData); });
				
				mCurState = State.Idle;
			}
			else if (mCurState == State.Drag)
			{
				mOnDragEndedEvent.InvokeGracefully();
				
				if (mIdleStateWhenDragEnd)
				{
					mCurState = State.Idle;
				}
			}

			mDragBegan = false;
		}

        private void Start()
        {
	        if (ScrollRect == null)
		        ScrollRect = transform.GetComponentInParent<ScrollRect>();

	        if (ScrollRect == null)
	        {
		        Debug.LogError(">>>> 父物体中找不到ScrollRect，删除该UIDragEventInScrollRect");
		        Destroy(this);
	        }
        }
	}
}