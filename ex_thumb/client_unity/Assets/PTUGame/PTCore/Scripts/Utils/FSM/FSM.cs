/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using System.Collections.Generic;

	public class PTFSMState
	{
		public PTFSMState(ushort stateName)
		{
			Name = stateName;
		}

		public ushort Name; // 字符串

		public virtual void OnEnter()
		{
		} // 进入状态(逻辑)

		public virtual void OnExit()
		{
		} // 离开状态(逻辑)

		/// <summary>
		/// translation for name
		/// </summary>
		public Dictionary<ushort, PTFSMTranslation> TranslationDict = new Dictionary<ushort, PTFSMTranslation>();
	}

	/// <summary>
	/// 跳转类
	/// </summary>
	public class PTFSMTranslation
	{
		public PTFSMState FromState;
		public ushort EventName;
		public PTFSMState ToState;

		public PTFSMTranslation(PTFSMState fromState, ushort eventName, PTFSMState toState)
		{
			FromState = fromState;
			ToState = toState;
			EventName = eventName;
		}
	}

	public class PTFSM
	{
		PTFSMState mCurState;

		public PTFSMState State
		{
			get { return mCurState; }
		}

		/// <summary>
		/// The m state dict.
		/// </summary>
		Dictionary<ushort, PTFSMState> mStateDict = new Dictionary<ushort, PTFSMState>();

		/// <summary>
		/// Adds the state.
		/// </summary>
		/// <param name="state">State.</param>
		public void AddState(PTFSMState state)
		{
			mStateDict.Add(state.Name, state);
		}


		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="translation">Translation.</param>
		public void AddTranslation(PTFSMTranslation translation)
		{
			mStateDict[translation.FromState.Name].TranslationDict.Add(translation.EventName, translation);
		}


		/// <summary>
		/// Adds the translation.
		/// </summary>
		/// <param name="fromState">From state.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="toState">To state.</param>
		public void AddTranslation(PTFSMState fromState, ushort eventName, PTFSMState toState)
		{
			mStateDict[fromState.Name].TranslationDict.Add(eventName, new PTFSMTranslation(fromState, eventName, toState));
		}

		/// <summary>
		/// Start the specified startState.
		/// </summary>
		/// <param name="startState">Start state.</param>
		public void Start(PTFSMState startState)
		{
			mCurState = startState;
			mCurState.OnEnter();
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		public void HandleEvent(ushort eventName)
		{
			if (mCurState != null && mStateDict[mCurState.Name].TranslationDict.ContainsKey(eventName))
			{
				var tempTranslation = mStateDict[mCurState.Name].TranslationDict[eventName];
				tempTranslation.FromState.OnExit();
				mCurState = tempTranslation.ToState;
				tempTranslation.ToState.OnEnter();
			}
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			mStateDict.Clear();
		}
	}
}