/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 * https://github.com/CoolYiWen/MOMFrame
 * https://github.com/zhutaorun
 * https://github.com/zhutaorun/Hearth-Stone
 * https://github.com/strangeioc
 * http://www.jianshu.com/p/10693fee70a5
 * https://github.com/handcircus/Unity-Resource-Checker
 * https://github.com/JefferiesTube/UnityEditorHelper
 * https://github.com/liortal53/MissingReferencesUnity
 ****************************************************************************/


using PTGame.Core;

namespace PTGame.Framework
{
	using System;

	/// <summary>
	/// Support Manager of Manager
	/// </summary>
	public interface IManager
	{
		void Init();

		void RegisterEvent<T>(T msgId, OnEvent process) where T : IConvertible;

		void UnRegistEvent<T>(T msgEvent, OnEvent process) where T : IConvertible;
		
		void SendEvent<T>(T eventId) where T : IConvertible;

		void SendMsg(PTMsg msg);
	}
}