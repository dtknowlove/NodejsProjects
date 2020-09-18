/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using PTGame.Core;
namespace PTGame.Framework
{
	using System;

	/// <summary>
	/// msgbody
	/// </summary>
	public class PTMsg : IPoolable, IPoolType
	{	
		/// <summary>
		/// EventID
		/// </summary>
		public virtual int EventID { get; set; }
		
		/// <summary>
		/// Processed or not
		/// </summary>
		public bool Processed { get; set; }
		
		/// <summary>
		/// reusable or not 
		/// </summary>
		public bool ReuseAble { get; set; }
		
		public int ManagerID
		{
			get { return EventID / PTMsgSpan.Count * PTMsgSpan.Count; }
		}

		public PTMsg(){}

		#region Object Pool
		public static PTMsg Allocate<T>(T eventId) where T : IConvertible
		{
			var msg = SafeObjectPool<PTMsg>.Instance.Allocate();
			msg.EventID = eventId.ToInt32(null);
			msg.ReuseAble = true;
			return msg;
		}

		public virtual void Recycle2Cache()
		{
			SafeObjectPool<PTMsg>.Instance.Recycle(this);
		}

		void IPoolable.OnRecycled()
		{
			Processed = false;
		}
		
		bool IPoolable.IsRecycled { get; set; }
		#endregion

		#region deprecated since v0.0.5
		// for proto buf;
		[Obsolete("deprecated since 0.0.5,use EventID instead")]
		public int msgId
		{
			get { return EventID; }
			set { EventID = value; }
		}
		
		[Obsolete("GetMgrID() is deprecated,please use ManagerID Property instead")]
		public int GetMgrID()
		{
			return ManagerID;
		}
		
		[Obsolete("deprecated,use allocate instead")]
		public PTMsg(int eventID)
		{
			EventID = eventID;
		}
		#endregion
	}
}