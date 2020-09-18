/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework
{
	using UnityEngine.Events;
	using PTGame.Core;
	
	/// <summary>
	/// 全局唯一继承于MonoBehaviour的单例类，保证其他公共模块都以App的生命周期为准
	/// 这个东西很基类，没什么用。概念也不太清晰
	/// </summary>
	[PTMonoSingletonPath("[Framework]/PTFramework")]
	public class PTFramework : PTMgrBehaviour, ISingleton
	{
		/// <summary>
		/// 组合的方式实现单例的模板
		/// </summary>
		/// <value>The instance.</value>
		public static PTFramework Instance
		{
			get { return PTMonoSingletonProperty<PTFramework>.Instance; }
		}

		protected override int MgrId
		{
			get { return PTMgrID.Framework; }
		}

		public void OnSingletonInit()
		{
		}

		public void Dispose()
		{
		}

		private PTFramework()
		{
		}

		#region 全局生命周期回调

		public UnityAction OnUpdateEvent = delegate { };
		public UnityAction OnFixedUpdateEvent = delegate { };
		public UnityAction OnLateUpdateEvent = delegate { };
		public UnityAction OnGUIEvent = delegate { };
		public UnityAction OnDestroyEvent = delegate { };
		public UnityAction OnApplicationQuitEvent = delegate { };

		void Update()
		{
			OnUpdateEvent.InvokeGracefully();
		}

		void FixedUpdate()
		{
			OnFixedUpdateEvent.InvokeGracefully();
		}

		void LateUpdate()
		{
			OnLateUpdateEvent.InvokeGracefully();
		}

		void OnGUI()
		{
			OnGUIEvent.InvokeGracefully();
		}

		protected void OnDestroy()
		{
			OnDestroyEvent.InvokeGracefully();
		}

		void OnApplicationQuit()
		{
			OnApplicationQuitEvent.InvokeGracefully();
		}

		#endregion
	}
}