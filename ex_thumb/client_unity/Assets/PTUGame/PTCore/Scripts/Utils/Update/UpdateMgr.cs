/****************************************************************************
 * Copyright (c) 2017 Feiko Joosten
 * Copyright (c) 2019 ptgame@putao.com
 * https://blogs.unity3d.com/cn/2015/12/23/1k-update-calls/
 * https://github.com/thexa4/UnityScheduler
 ****************************************************************************/

/// <summary>
/// Made by Feiko Joosten
/// 
/// I have based this code on this blogpost. Decided to give it more functionality. http://blogs.unity3d.com/2015/12/23/1k-update-calls/
/// Use this to speed up your performance when you have a lot of update, fixed update and or late update calls in your scene
/// Let the object you want to give increased performance inherit from OverridableMonoBehaviour
/// Replace your void Update() for public override void UpdateMe()
/// Or replace your void FixedUpdate() for public override void FixedUpdateMe()
/// Or replace your void LateUpdate() for public override void LateUpdateMe()
/// OverridableMonoBehaviour will add the object to the update manager
/// UpdateManager will handle all of the update calls
/// </summary>

namespace PTGame.Core
{
	using UnityEngine;

	[PTMonoSingletonPath("[Framework]/UpdateManager")]
	public class UpdateMgr : PTMonoSingleton<UpdateMgr>
	{
		private readonly ResizeableArray<IUpdatable> mRegularArray = new ResizeableArray<IUpdatable>(0);
		private readonly ResizeableArray<IFixedUpdatable> mFixedArray = new ResizeableArray<IFixedUpdatable>(0);
		private readonly ResizeableArray<ILateUpdatable> mLateArray = new ResizeableArray<ILateUpdatable>(0);

		public static void AddItem(object updatableObj)
		{
			Instance.AddItemToArray(updatableObj);
		}

		public static void RemoveSpecificItem(object updatableObj)
		{
			Instance.RemoveSpecificItemFromArray(updatableObj);
		}

		public static void RemoveSpecificItemAndDestroyIt(object updatableObj)
		{
			Instance.RemoveSpecificItemFromArray(updatableObj);

			if (updatableObj is MonoBehaviour)
			{
				((MonoBehaviour) updatableObj).DestroyGameObjGracefully();
			}
		}

		private void AddItemToArray(object updatableObj)
		{
			if (updatableObj is IUpdatable)
			{
				mRegularArray.Append(updatableObj as IUpdatable);
			}

			if (updatableObj is IFixedUpdatable)
			{
				mFixedArray.Append(updatableObj as IFixedUpdatable);
			}

			if (updatableObj is ILateUpdatable)
			{
				mLateArray.Append(updatableObj as ILateUpdatable);
			}
		}

		private void RemoveSpecificItemFromArray(object updatableObj)
		{
			if (updatableObj is IUpdatable && mRegularArray.Contains(updatableObj as IUpdatable))
			{
				mRegularArray.Remove(updatableObj as IUpdatable);
			}

			if (updatableObj is IFixedUpdatable && mFixedArray.Contains(updatableObj as IFixedUpdatable))
			{
				mFixedArray.Remove(updatableObj as IFixedUpdatable);
			}

			if (updatableObj is ILateUpdatable && mLateArray.Contains(updatableObj as ILateUpdatable))
			{
				mLateArray.Remove(updatableObj as ILateUpdatable);
			}
		}

		private void Update()
		{
			if (mRegularArray.Count == 0) return;

			for (var i = 0; i < mRegularArray.Count; i++)
			{
				if (mRegularArray[i] == null) continue;

				mRegularArray[i].OnUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (mFixedArray.Count == 0) return;

			for (var i = 0; i < mFixedArray.Count; i++)
			{
				if (mFixedArray[i] == null) continue;

				mFixedArray[i].OnFixedUpdate();
			}
		}

		private void LateUpdate()
		{
			if (mLateArray.Count == 0) return;

			for (var i = 0; i < mLateArray.Count; i++)
			{
				if (mLateArray[i] == null) continue;

				mLateArray[i].OnLateUpdate();
			}
		}
	}
}