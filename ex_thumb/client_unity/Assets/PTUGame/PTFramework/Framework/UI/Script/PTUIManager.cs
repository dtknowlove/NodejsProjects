/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
 *
 * UI层级管理,可以考虑用分段技术。类似EventSystem
 ****************************************************************************/

using System;
using System.Collections.Generic;
using PTGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Framework
{
#if SLUA_SUPPORT
	using SLua;
#endif

	public enum UILevel
	{
		Bg = -2,  //背景层UI
		AnimationUnderPage = -1, //动画层
		Common = 0, //普通层UI
		AnimationOnPage = 1, // 动画层
		PopUI = 2, //弹出层UI
		Guide = 3, //新手引导层
		Const = 4, //持续存在层UI
		Toast = 5, //对话框层UI
		Forward = 6, //最高UI层用来放置UI特效和模型
	}

#if SLUA_SUPPORT
	[CustomLuaClass]
#endif
	//// <summary>
	/// UGUI UI界面管理器
	/// </summary>
	public class PTUIManager : PTMgrBehaviour, ISingleton
	{
		Dictionary<string, IUIView> mAllUI = new Dictionary<string, IUIView>();

		[SerializeField] Transform mBgTrans;
		[SerializeField] Transform mAnimationUnderPageTrans;
		[SerializeField] Transform mCommonTrans;
		[SerializeField] Transform mAnimationOnPageTrans;
		[SerializeField] Transform mPopUITrans;
		[SerializeField] Transform mConstTrans;
		[SerializeField] Transform mToastTrans;
		[SerializeField] Transform mForwardTrans;
		[SerializeField] Transform mDesignTrans;
		[SerializeField] Camera mUICamera;
		[SerializeField] Canvas mCanvas;
		[SerializeField] CanvasScaler mCanvasScaler;
		[SerializeField] GraphicRaycaster mGraphicRaycaster;

		public Action<UIPanelInfo> OnPanelOpen = null;
		public Action<UIPanelInfo> OnPanelClose = null;
		
		public Stack<UIPanelInfo> mUIStack = new Stack<UIPanelInfo>(); 

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			if (mDesignTrans)
			{
				mDesignTrans.Hide();
			}
		}

		void ISingleton.OnSingletonInit()
		{
		}

		private static PTUIManager mInstance;

		public static PTUIManager Instance
		{
			get
			{
				if (null == mInstance)
				{
					mInstance = FindObjectOfType<PTUIManager>();
				}

				if (null == mInstance)
				{
					Instantiate(Resources.Load<GameObject>("UIRoot"));
					mInstance = PTMonoSingletonProperty<PTUIManager>.Instance;
					mInstance.name = "UIRoot";
				}

				return mInstance;
			}
		}

		public Canvas RootCanvas
		{
			get { return mCanvas; }
		}

		public Camera UICamera
		{
			get { return mUICamera; }
		}

		// TODO: 全局唯一事件管理
		public GraphicRaycaster GlobalGraphicRaycaster
		{
			get { return mGraphicRaycaster; }
		}

		public void SetResolution(int width, int height)
		{
			mCanvasScaler.referenceResolution = new Vector2(width, height);
		}

		public void SetMatchOnWidthOrHeight(float heightPercent)
		{
			mCanvasScaler.matchWidthOrHeight = heightPercent;
		}

		public float GetCurMatchOrHeight()
		{
			return mCanvasScaler.matchWidthOrHeight;
		}

		public IUIView OpenUI(string uiBehaviourName, UILevel canvasLevel,IUIData uiData, string assetBundleName)
		{
			if (!mAllUI.ContainsKey(uiBehaviourName))
			{
				InnerCreateUIView(uiBehaviourName, canvasLevel,uiData,assetBundleName);
			}

			mAllUI[uiBehaviourName].Show();
			OnPanelOpen.InvokeGracefully(mAllUI[uiBehaviourName].PanelInfo);
			return mAllUI[uiBehaviourName];
		}


		/// <summary>
		/// 创建UIPanel
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		/// <param name="uiLevel"></param>
		/// <param name="initData"></param>
		/// <returns></returns>
		public GameObject CreateUIObj(string uiBehaviourName, UILevel uiLevel,string assetBundleName = null)
		{
			IUIView ui;
			if (mAllUI.TryGetValue(uiBehaviourName, out ui))
			{
				PTDebug.LogWarning("{0}: already exist", uiBehaviourName);
				// 直接返回,不要再调一次Init(),Init()应该只能调用一次
				return ui.Transform.gameObject;
			}

			ui = UIView.Load(uiBehaviourName,assetBundleName);

			switch (uiLevel)
			{
				case UILevel.Bg:
					ui.Transform.SetParent(mBgTrans);
					break;
				case UILevel.AnimationUnderPage:
					ui.Transform.SetParent(mAnimationUnderPageTrans);
					break;
				case UILevel.Common:
					ui.Transform.SetParent(mCommonTrans);
					break;
				case UILevel.AnimationOnPage:
					ui.Transform.SetParent(mAnimationOnPageTrans);
					break;
				case UILevel.PopUI:
					ui.Transform.SetParent(mPopUITrans);
					break;
				case UILevel.Const:
					ui.Transform.SetParent(mConstTrans);
					break;
				case UILevel.Toast:
					ui.Transform.SetParent(mToastTrans);
					break;
				case UILevel.Forward:
					ui.Transform.SetParent(mForwardTrans);
					break;
			}

			var uiGoRectTrans = ui.Transform as RectTransform;

			uiGoRectTrans.offsetMin = Vector2.zero;
			uiGoRectTrans.offsetMax = Vector2.zero;
			uiGoRectTrans.anchoredPosition3D = Vector3.zero;
			uiGoRectTrans.anchorMin = Vector2.zero;
			uiGoRectTrans.anchorMax = Vector2.one;

			ui.Transform.LocalScaleIdentity();
			ui.Transform.gameObject.name = uiBehaviourName;

			ui.PanelInfo = new UIPanelInfo {AssetBundleName = assetBundleName, Level = uiLevel, PanelName = uiBehaviourName};
			return ui.Transform.gameObject;
		}

		/// <summary>
		/// 显示UIBehaiviour
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		public void ShowUI(string uiBehaviourName)
		{
			IUIView iuiView = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out iuiView))
			{
				iuiView.Show();
			}
		}

		/// <summary>
		/// 隐藏UI
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		public void HideUI(string uiBehaviourName)
		{
			IUIView iuiView = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out iuiView))
			{
				iuiView.Hide();
			}
		}

		/// <summary>
		/// 删除所有UI层
		/// </summary>
		public void CloseAllUI()
		{
			foreach (var layer in mAllUI)
			{
				Destroy(layer.Value.Transform.gameObject);
			}

			mAllUI.Clear();
		}

		/// <summary>
		/// 关闭并卸载UI
		/// </summary>
		/// <param name="behaviourName"></param>
		public void CloseUI(string behaviourName)
		{
			IUIView behaviour = null;

			mAllUI.TryGetValue(behaviourName, out behaviour);

			if (null != behaviour)
			{
				OnPanelClose.InvokeGracefully(behaviour.PanelInfo);
				behaviour.Close();
				mAllUI.Remove(behaviourName);				
			}
		}

		public void Push<T>() where T : UIView
		{
			Push(GetUI<T>());
		}

		public void Push(IUIView view)
		{
			if (view != null)
			{
				mUIStack.Push(view.PanelInfo);
				CloseUI(view.PanelInfo.PanelName);
			}
		}

		public void Back(string currentPanelName)
		{
			var previousPanelInfo = mUIStack.Pop();
			CloseUI(currentPanelName);
			OpenUI(previousPanelInfo.PanelName, previousPanelInfo.Level, previousPanelInfo.UIData,
				previousPanelInfo.AssetBundleName);
		}

		/// <summary>
		/// 获取UIBehaviour
		/// </summary>
		/// <param name="uiBehaviourName"></param>
		/// <returns></returns>
		public UIView GetUI(string uiBehaviourName)
		{
			IUIView retIuiView = null;
			if (mAllUI.TryGetValue(uiBehaviourName, out retIuiView))
			{
				return retIuiView as UIView;
			}
			return null;
		}

		/// <summary>
		/// 获取UI相机
		/// </summary>
		/// <returns></returns>
		public Camera GetUICamera()
		{
			return mUICamera;
		}

		protected override int MgrId
		{
			get { return PTMgrID.UI; }
		}

		/// <summary>
		/// 命名空间对应名字的缓存
		/// </summary>
		private Dictionary<string, string> mFullname4UIBehaviourName = new Dictionary<string, string>();

		public string GetUIBehaviourName<T>()
		{
			var fullBehaviourName = typeof(T).ToString();
			string retValue = null;

			if (mFullname4UIBehaviourName.ContainsKey(fullBehaviourName))
			{
				retValue = mFullname4UIBehaviourName[fullBehaviourName];
			}
			else
			{
				var nameSplits = fullBehaviourName.Split('.');
				retValue = nameSplits[nameSplits.Length - 1];
				mFullname4UIBehaviourName.Add(fullBehaviourName, retValue);
			}

			return retValue;
		}

		private IUIView InnerCreateUIView(string uiBehaviourName, UILevel level, IUIData uiData = null,string assetBundleName = null)
		{
			IUIView ui;
			if (mAllUI.TryGetValue(uiBehaviourName, out ui))
			{
				return ui;
			}
			var uiObj = CreateUIObj(uiBehaviourName, level, assetBundleName);

			ui = uiObj.GetComponent<IUIView>();

			mAllUI.Add(uiBehaviourName, ui);
			
			ui.PanelInfo.UIData = uiData;
			ui.Init(uiData);

			return ui;
		}

		#region UnityCSharp Generic Support

		/// <summary>
		/// Create&ShowUI
		/// </summary>
		public T OpenUI<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null,string assetBundleName = null,string prefabName = null) where T : UIView
		{
			var behaviourName = prefabName ?? GetUIBehaviourName<T>();
			return OpenUI(behaviourName, canvasLevel, uiData, assetBundleName) as T;
		}


		public void ShowUI<T>() where T : UIView
		{
			ShowUI(GetUIBehaviourName<T>());
		}

		public void HideUI<T>() where T : UIView
		{
			HideUI(GetUIBehaviourName<T>());
		}

		public void CloseUI<T>() where T : UIView
		{
			CloseUI(GetUIBehaviourName<T>());
		}

		public void CloseLevel(UILevel uiLevel)
		{
			switch (uiLevel)
			{
				case UILevel.Bg:
					foreach (Transform bgTran in mBgTrans)
					{
						CloseUI(bgTran.name);
					}
					break;
				case UILevel.AnimationUnderPage:
					foreach (Transform animationUnderPage in mAnimationUnderPageTrans)
					{
						CloseUI(animationUnderPage.name);
					}
					break;
				case UILevel.Common:
					foreach (Transform commonTran in mCommonTrans)
					{
						CloseUI(commonTran.name);
					}
					break;
				case UILevel.AnimationOnPage:
					foreach (Transform animationOnPageTrans in mAnimationOnPageTrans)
					{
						CloseUI(animationOnPageTrans.name);
					}
					break;
				case UILevel.PopUI:
					foreach (Transform popUiTran in mPopUITrans)
					{
						CloseUI(popUiTran.name);
					}
					break;
				case UILevel.Const:
					foreach (Transform constTran in mConstTrans)
					{
						CloseUI(constTran.name);
					}
					break;
				case UILevel.Toast:
					foreach (Transform toastTran in mToastTrans)
					{
						CloseUI(toastTran.name);
					}
					break;
				case UILevel.Forward:
					foreach (Transform forwardTrans in mForwardTrans)
					{
						CloseUI(forwardTrans.name);
					}
					break;
			}
		}

		public T GetUI<T>() where T : UIView
		{
			return GetUI(GetUIBehaviourName<T>()) as T;
		}

		#endregion

		#region LuaSupport

		#endregion
	}

	public static class UIMgr
	{
		public static void Push<T>() where T : UIView
		{
			PTUIManager.Instance.Push<T>();
		}

		public static void Push(UIView view)
		{
			PTUIManager.Instance.Push(view);
		}

		#region 高频率用的 Getter Setter
		public static Camera Camera
		{
			get { return PTUIManager.Instance.UICamera; }
		}

		public static void SetResolution(int width, int height, float matchOnWidthOrHeight)
		{
			PTUIManager.Instance.SetResolution(width, height);
			PTUIManager.Instance.SetMatchOnWidthOrHeight(matchOnWidthOrHeight);
		}

		#endregion
		
		#region 高频率用的api 只能在Mono层使用
		public static T OpenPanel<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null,
			string prefabName = null) where T : UIView
		{
			return PTUIManager.Instance.OpenUI<T>(canvasLevel, uiData, assetBundleName,prefabName);
		}
		
		public static T OpenPanel<T>(IUIData uiData, string assetBundleName = null,
			string prefabName = null) where T : UIView
		{
			return PTUIManager.Instance.OpenUI<T>(UILevel.Common, uiData, assetBundleName,prefabName);
		}

		public static void ClosePanel<T>() where T : UIView
		{
			PTUIManager.Instance.CloseUI<T>();
		}

		public static T GetPanel<T>() where T : UIView
		{
			return PTUIManager.Instance.GetUI<T>();
		}

		[Obsolete("这个API使用起来会有隐患，建议还是使用ClosePanel进行精确控制,这个API的出现是为了重构旧项目")]
		public static void CloseLevel(UILevel uiLevel)
		{
			PTUIManager.Instance.CloseLevel(uiLevel);
		}
		#endregion

		#region 给脚本层用的api
		public static UIView GetPanel(string panelName)
		{
			return PTUIManager.Instance.GetUI(panelName);
		}

		public static UIView OpenPanel(string panelName, UILevel level = UILevel.Common, IUIData uiData = null, string assetBundleName = null)
		{
			return PTUIManager.Instance.OpenUI(panelName, level, uiData, assetBundleName) as UIView;
		}

		public static void ClosePanel(string panelName)
		{
			PTUIManager.Instance.CloseUI(panelName);
		}
		#endregion
	}
}