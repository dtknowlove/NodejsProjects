/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
	using System;

	/// <summary>
	/// manager基类
	/// </summary>
	public abstract class PTMgrBehaviour : PTMonoBehaviour, IManager
	{
		private PTEventSystem mEventSystem = SafeObjectPool<PTEventSystem>.Instance.Allocate();

		#region IManager

		public virtual void Init()
		{
		}

		#endregion

		protected abstract int MgrId { get ; }

		protected override IManager mMgr
		{
			get { return this; }
		}

		void IManager.RegisterEvent<T>(T msgId, OnEvent process)
		{
			mEventSystem.Register(msgId, process);
		}

		void IManager.UnRegistEvent<T>(T msgEvent, OnEvent process)
		{
			mEventSystem.UnRegister(msgEvent, process);
		}

		public void SendMsg(PTMsg msg)
		{
			if (msg.GetMgrID() == MgrId)
			{
				Process(msg.msgId, msg);
			}
			else
			{
				PTMsgCenter.SendMsg(msg);
			}
		}

		public void SendEvent<T>(T eventId) where T : IConvertible
		{
			SendMsg(PTMsg.Allocate(eventId));
		}

		// 来了消息以后,通知整个消息链
		protected override void ProcessMsg(int eventId, PTMsg msg)
		{
			mEventSystem.Send(msg.msgId, msg);
		}

		protected override void OnBeforeDestroy()
		{
			if (mEventSystem.IsNotNull())
			{
				mEventSystem.OnRecycled();
			}
		}
	}
}