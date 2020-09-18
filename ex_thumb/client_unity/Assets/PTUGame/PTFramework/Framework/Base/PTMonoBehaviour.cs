/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using System;
using System.Collections.Generic;
using PTGame.Core;
using UnityEngine;

namespace PTGame.Framework 
{
	public abstract class PTMonoBehaviour : MonoBehaviour
	{
		protected bool mReceiveMsgOnlyObjActive = true;
		
		public void Process (int eventId, params object[] param)  
		{
			if ((!mReceiveMsgOnlyObjActive || !gameObject.activeInHierarchy) && mReceiveMsgOnlyObjActive) return;
			var msg = param[0] as PTMsg;
			ProcessMsg(eventId, msg);
			msg.Processed = true;
				
			if (msg.ReuseAble)
			{
				msg.Recycle2Cache();
			}
		}

		protected virtual void ProcessMsg (int eventId,PTMsg msg) {}

		protected abstract IManager mMgr { get; }
				
		public virtual void Show()
		{
			gameObject.SetActive (true);
			OnShow ();
		}

		protected virtual void OnShow() {}

		public virtual void Hide()
		{
			OnHide ();
			gameObject.SetActive (false);
		}

		protected virtual void OnHide() {}


		protected void RegisterEvent<T>(T eventId) where T : IConvertible
		{
			mCachedEventIds.Add(eventId.ToInt32(null));
			mMgr.RegisterEvent(eventId, Process);
		}
		
		protected void UnRegisterEvent<T>(T eventId) where T : IConvertible
		{
			mCachedEventIds.Remove(eventId.ToInt32(null));
			mMgr.UnRegistEvent(eventId, Process);
		}

		protected void UnRegisterAllEvent()
		{
			if (mPrivateEventIds.IsNotNull())
			{
				mPrivateEventIds.ForEach(id => mMgr.UnRegistEvent(id,Process));
			}
		}

		public void SendMsg(PTMsg msg)
		{
			mMgr.SendMsg(msg);
		}

		protected void SendEvent<T>(T eventId) where T : IConvertible
		{
			mMgr.SendEvent(eventId);
		}
		
		private List<int> mPrivateEventIds;
		
		private List<int> mCachedEventIds
		{
			get
			{
				if (mPrivateEventIds.IsNull())
				{
					mPrivateEventIds = new List<int>();
				}

				return mPrivateEventIds;
			}
		}

		protected virtual void OnDestroy()
		{
			if (Application.isPlaying)
			{
				OnBeforeDestroy();
				UnRegisterAllEvent();
			}
		}

		protected virtual void OnBeforeDestroy(){}
	}
}
