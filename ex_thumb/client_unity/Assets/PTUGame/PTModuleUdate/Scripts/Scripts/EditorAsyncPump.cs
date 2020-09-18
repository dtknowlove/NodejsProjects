#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace PTGame.ModuleUpdate
{

	public class EditorAsyncPump
	{
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			Instance = new EditorAsyncPump();
			EditorApplication.update += ExecuteContinuations;
		}

		private static void ExecuteContinuations()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// Not in Edit mode, don't interfere
				return;
			}

//		var context = SynchronizationContext.Current;
//		if (_execMethod == null)
//		{
//			_execMethod = context.GetType().GetMethod("Exec", BindingFlags.NonPublic | BindingFlags.Instance);
//		}
//		_execMethod.Invoke(context, null);
			Instance.Exec();

		}

		private static MethodInfo _execMethod;


		private static readonly Queue<Action> _executionQueue = new Queue<Action>();

		public static EditorAsyncPump Instance;

		public void Exec()
		{
			lock (_executionQueue)
			{
				while (_executionQueue.Count > 0)
				{
					_executionQueue.Dequeue().Invoke();
				}
			}
		}

		/// <summary>
		/// Locks the queue and adds the IEnumerator to the queue
		/// </summary>
		/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
		public void Enqueue(IEnumerator action)
		{
			lock (_executionQueue)
			{
				_executionQueue.Enqueue(() => { EditorCoroutine.start(action); });
			}
		}

		/// <summary>
		/// Locks the queue and adds the Action to the queue
		/// </summary>
		/// <param name="action">function that will be executed from the main thread.</param>
		public void Enqueue(Action action)
		{
			Enqueue(ActionWrapper(action));
		}

		IEnumerator ActionWrapper(Action a)
		{
			a();
			yield return null;
		}

	}
}
#endif