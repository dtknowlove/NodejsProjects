/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PTGame.Framework
{
	public static class PTMsgCenter
	{
		private static List<IMsgCenter> mMsgCenters = null;
		private static bool mIsInit = false;
		
		public static void SendMsg(PTMsg tmpMsg)
		{
			// Framework Msg
			switch (tmpMsg.GetMgrID())
			{
				case PTMgrID.UI:
					PTUIManager.Instance.SendMsg(tmpMsg);
					break;
				case PTMgrID.Audio:
					//AudioManager.Instance.SendMsg(tmpMsg);
					break;
				case PTMgrID.Action:
					ActionMgr.Instance.SendMsg(tmpMsg);
					break;
				case PTMgrID.PCConnectMobile:
//					PCConnectMobileManager.Instance.SendMsg(tmpMsg);
					break;
			}

			if (!mIsInit)
			{
				mMsgCenters = new List<IMsgCenter>();
				var subTypeQuery =  AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IMsgCenter))))
					.ToArray();
				Debug.Log("===Inhert IMsgCenter>>>" + subTypeQuery.Count());
				foreach (var type in subTypeQuery)
				{
					Debug.Log("===Inhert IMsgCenter>>>" + type.Name);
					var data = Activator.CreateInstance(type);
					mMsgCenters.Add((IMsgCenter) data);
				}
				mIsInit = true;
			}
			mMsgCenters.ForEach(t => t.SendMsg(tmpMsg));
		}
	}

	public interface IMsgCenter
	{
		void SendMsg(PTMsg tmpMsg);
	}
}