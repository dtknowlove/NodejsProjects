/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using PTGame.Core;
namespace PTGame.Framework
{
	using UnityEngine;
	using System;

	public enum UIFilterEvent
	{
		Began = PTMgrID.UIFilter,
		DelayLock,
		Lock,
		UnLock,
		LockObjEvent,
		UnlockObjEvent,
		Ended
	}

	/// <summary>
	/// 互斥事件
	/// </summary>
	public class UILockOnClickEventMsg : PTMsg
	{
		public float LockTime = 0.2f;

		public UILockOnClickEventMsg(float lockTime) : base((ushort) UIFilterEvent.DelayLock)
		{
			LockTime = lockTime;
		}
	}

	public class LockMsg : PTMsg
	{
		public LockMsg() : base((ushort) UIFilterEvent.Lock)
		{
		}
	}

	public class UnLockMsg : PTMsg
	{
		public UnLockMsg() : base((ushort) UIFilterEvent.UnLock)
		{
		}
	}

	public class UILockObjEventMsg : PTMsg
	{
		public GameObject LockedObj;

		public UILockObjEventMsg(GameObject lockedObj) : base((ushort) UIFilterEvent.LockObjEvent)
		{
			LockedObj = lockedObj;
		}
	}

	public class UIUnlockObjEventMsg : PTMsg
	{
		public GameObject UnlockedObj;

		public UIUnlockObjEventMsg(GameObject unlockedObj) : base((ushort) UIFilterEvent.UnlockObjEvent)
		{
			this.UnlockedObj = unlockedObj;
		}
	}

	[PTMonoSingletonPath("[Event]/UIEventLockManager")]
	public class UIEventLockManager : PTMgrBehaviour, ISingleton
	{
		protected override int MgrId
		{
			get { return PTMgrID.UIFilter; }
		}

		public static UIEventLockManager Instance
		{
			get { return PTMonoSingletonProperty<UIEventLockManager>.Instance; }
		}

		public void OnSingletonInit()
		{
		}

		public void Dispose()
		{
		}

		public bool LockBtnOnClick { get; private set; }
		public bool Lock { protected get; set; }
		public GameObject LockedObj { get;protected set; }

		void Awake()
		{
			RegisterEvent(UIFilterEvent.DelayLock);
			RegisterEvent(UIFilterEvent.UnLock);
			RegisterEvent(UIFilterEvent.Lock);

			LockBtnOnClick = false;
			Lock = false;
		}

		protected override void ProcessMsg(int key, PTMsg msg)
		{
			PTDebug.Log("{0}",msg.EventID);
			switch (key)
			{
				case (ushort) UIFilterEvent.DelayLock:
					PTDebug.Log("receive");
					var lockOnClickEventMsg = msg as UILockOnClickEventMsg;
					LockBtnOnClick = true;
					var delayNode = new DelayAction(lockOnClickEventMsg.LockTime)
					{
						OnEndedCallback = delegate
						{
							LockBtnOnClick = false;
						}
					};
					StartCoroutine(delayNode.Execute());
					break;
				case (ushort) UIFilterEvent.Lock:
					PTDebug.Log("Lock");
					Lock = true;
					break;
				case (ushort) UIFilterEvent.UnLock:
					PTDebug.Log("UnLock");
					Lock = false;
					break;

				case (int) UIFilterEvent.LockObjEvent:
				{
					var lockObjEventMsg = msg as UILockObjEventMsg;
					if (null == LockedObj)
					{
						LockedObj = lockObjEventMsg.LockedObj;
					}
					else if (LockedObj == lockObjEventMsg.LockedObj)
					{
						// maybe two finger in one obj
						PTDebug.LogWarning("error: curLockedObj is already seted");
					}
					else if (LockedObj != lockObjEventMsg.LockedObj)
					{
						throw new Exception("error: pre obj need unlocked");
					}
				}
					break;
				case (int) UIFilterEvent.UnlockObjEvent:
				{
					var unlockObjEventMsg = msg as UIUnlockObjEventMsg;
					if (unlockObjEventMsg.UnlockedObj == LockedObj)
					{
						unlockObjEventMsg.UnlockedObj = null;
						LockedObj = null;
					}
					else if (LockedObj == null)
					{
						PTDebug.LogWarning ("error: curLockedObj is already unlocked");
					}
					else if (LockedObj != unlockObjEventMsg.UnlockedObj)
					{
						throw new Exception("error: pre obj need unlocked");
					}
				}
					break;
			}
		}
	}
}