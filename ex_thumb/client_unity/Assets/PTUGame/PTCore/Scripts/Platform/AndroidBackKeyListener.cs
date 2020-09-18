using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Core
{
	[PTMonoSingletonPath("[PTGame]/AndroidBackKeyListener")]
	public class AndroidBackKeyListener : MonoBehaviour,ISingleton
	{
		public static AndroidBackKeyListener Instance
		{
			get { return PTMonoSingletonProperty<AndroidBackKeyListener>.Instance; }
		}
		
		private float mClickEscape = 0;
		private bool mSimalutionKeyBack = false;

		private string mStrTip
		{
			get { return IsChinese ? "再按一次退出应用" : "Press again to exit the app"; }
		}
		
		private static bool IsChinese
		{
			get
			{
				var systemLan = Application.systemLanguage;
				return systemLan == SystemLanguage.Chinese ||
				       systemLan == SystemLanguage.ChineseSimplified ||
				       systemLan == SystemLanguage.ChineseTraditional;
			}
		}

		/// <summary>
		/// 全局控制
		/// </summary>
		public bool GlobalEnable = true;
		public bool Enable = true;

		void Update()
		{
			if (!GlobalEnable)
				return;
			if (!Enable || !Platform.IsAndroid) return;
			if (mClickEscape > 0)
			{
				mClickEscape -= Time.deltaTime;
				mClickEscape = Mathf.Max(mClickEscape, 0);
			}

			//android 返回键
			if (Input.GetKeyDown(KeyCode.Escape) || mSimalutionKeyBack)
			{
				if (mEventPool.Count <= 0)
				{
					if (mClickEscape > 0)
					{
						Debug.Log("[========>:L>>>>>Game Over!]");
						Application.Quit();
					}
					if (Mathf.Abs(mClickEscape) < 0.01f || mSimalutionKeyBack)
					{
						if (!Platform.IsEditor)
						{
							PTAndroidInterface.Instance.ShowExitAppTip(mStrTip);
						}
						mClickEscape = 1;
					}
				}

				InvokeEvent();

				mSimalutionKeyBack = false;
			}
			else if (Input.GetMouseButtonDown(0)) //除返回键外任意键被点击，重置这个时间
			{
				mClickEscape = 0;
			}
		}

		private Stack<KeyValuePair<string, Button.ButtonClickedEvent>> mEventPool =
			new Stack<KeyValuePair<string, Button.ButtonClickedEvent>>();

		/// <summary>
		/// 在页面需要监听返回键时注册
		/// </summary>
		/// <param name="uiName"></param>
		/// <param name="action"></param>
		public void Push(string uiName, Button.ButtonClickedEvent btnEvent)
		{
			if (!GlobalEnable)
				return;
			if (!Enable || !Platform.IsAndroid) return;
			mEventPool.Push(new KeyValuePair<string, Button.ButtonClickedEvent>(uiName, btnEvent));
			foreach (KeyValuePair<string, Button.ButtonClickedEvent> pair in mEventPool)
			{
				Debug.Log(">>>>>>>> register android back key event: " + pair.Key);
			}
		}

		/// <summary>
		/// 在页面不需要监听返回键时推出
		/// </summary>
		public void Pop()
		{
			if (!GlobalEnable)
				return;
			if (!Enable || !Platform.IsAndroid) return;
			if (mEventPool.Count > 0)
				mEventPool.Pop();
		}

		/// <summary>
		/// 模拟触发button点击事件
		/// </summary>
		private void InvokeEvent()
		{
			if (!GlobalEnable)
				return;
			if (Enable && mEventPool.Count > 0 && Platform.IsAndroid)
			{
				KeyValuePair<string, Button.ButtonClickedEvent> pair = mEventPool.Peek();
				pair.Value.Invoke();
				Debug.Log(">>>>>>> invoke Android back key event: " + pair.Key);
			}
		}

		public void OnSingletonInit()
		{
			
		}

		void OnGUI()
		{
			if (!GlobalEnable)
				return;
			if (!Platform.IsEditor || !Platform.IsAndroid) return;
			if (GUI.Button(new Rect(1000, 10, 100, 100), "Back"))
			{
				mSimalutionKeyBack = true;
			}
		}
	}
}
