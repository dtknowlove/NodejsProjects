/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using System.Collections.Generic;
using PTGame.Core;

namespace PTGame.Framework
{
	public enum LagecyActionEvent
	{
		Start = PTMgrID.Action,
		ExecuteAction,
		Ended
	}
	
	public class ActionMsg : PTMsg
	{
		public string Name;
		
		public ActionMsg(string name) : base((int)LagecyActionEvent.ExecuteAction)
		{
			Name = name;
		}
	}
	
	/// <summary>
	/// 管理Action InputAction或者什么的
	/// </summary>
	public class ActionMgr : PTMgrBehaviour,ISingleton
	{
		private readonly List<IAction> mActiveActions = new List<IAction>();

		protected override int MgrId
		{
			get { return PTMgrID.Action; }
		}

		public static ActionMgr Instance
		{
			get { return PTMonoSingletonProperty<ActionMgr>.Instance; }
		}
		
		private ActionMgr(){}
		
		public void OnSingletonInit()
		{
			RegisterEvent(LagecyActionEvent.ExecuteAction);
		}

		public static void Push(IAction action)
		{
			Instance.mActiveActions.Add(action);
		}

		public static void Pop(IAction action)
		{
			Instance.mActiveActions.Remove(action);
		}

		protected override void ProcessMsg(int eventId, PTMsg msg)
		{
			switch (eventId)
			{
				case (int)LagecyActionEvent.ExecuteAction:

					var actionMsg = msg as ActionMsg;

					mActiveActions.Find(action => action.Name.Equals(actionMsg.Name)).Execute();
					
					break;
			}
		}
	}
}