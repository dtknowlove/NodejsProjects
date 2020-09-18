/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PTGame.Core;
using UniRx;
using UnityEngine;

namespace PTGame.Framework
{
	[Serializable]
	public class ActionRecordFile
	{
		public string FileName;

		public List<ActionExecuteTimePair> RecordData;
	}

	[Serializable]
	public class ActionExecuteTimePair
	{
		public float ExecuteTime;
		public string ActionName;
	}
	
	public class ActionRecorder
	{
		private static ActionRecordFile mActionRecordFile;
		
		public static bool Recording { get; private set; }

		public static void RecordStart()
		{
			mActionRecordFile = new ActionRecordFile
			{
				FileName = "131576376315678780",
				RecordData = new List<ActionExecuteTimePair>()
			};

			Observable.EveryUpdate().Subscribe(_ =>
			{
				mCurTime += Time.deltaTime;
			});
		}

		
		private static float mCurTime = 0.0f;

		public static void RecordAction(IAction action)
		{
//			mActionRecordFile.RecordData.Add(new ActionExecuteTimePair()
//			{
//				ExecuteTime = mCurTime,
//				ActionName = action.Name,
//			});
		}

		public static void RecordStop()
		{
			mActionRecordFile.SaveJson(Application.dataPath.CombinePath(mActionRecordFile.FileName));
		}
	}

	
	public class ActionPlayer : PTMonoSingleton<ActionPlayer>
	{
		public static void PlayRecordAction(string fileName)
		{
			var actionFile = SerializeHelper.LoadJson<ActionRecordFile>(Application.dataPath.CombinePath("131576376315678780"));
			
			var timelineNode = new TimelineNode();

			foreach (var recordData in actionFile.RecordData)
			{
				var actionName = recordData.ActionName;
				timelineNode.Append(new TimelineNode.TimelinePair(recordData.ExecuteTime, EventAction.Allocate(() =>
				{
					PTUIManager.Instance.SendMsg(new ActionMsg(actionName));
				})));
			}

			Instance.ExecuteNode(timelineNode);
		}
	}
}