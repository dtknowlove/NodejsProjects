/****************************************************************************
 * 2017 xiaojun
 * 2017 liqingyun
 * 2017 maoling
 * 2018.5 liqingyun
 ****************************************************************************/

using System;
using System.Collections.Generic;
using PTGame.Core;

namespace PTGame.Framework
{
	using UnityEngine;
	
	/// <summary>
	/// 每个UIbehaviour对应的Data
	/// </summary>
	public interface IUIData
	{
	}

	public class UIData : IUIData
	{

	}

	[Serializable]
	public class SubPanelInfo
	{
		public string  PanelName;
		public UILevel Level;
	}

	public abstract class UIView : PTMonoBehaviour, IUIView
	{
		#region mvvm data binding

		public string                 ContextName;

		/* DataBinding
		private IViewModel mContext;

		/// <summary>
		/// Context. Data Binding to View
		/// </summary>
		public IViewModel Context 
		{
			get { return mContext; } 
			set 
			{
				mContext = value;
				if (mContext != null) { ContextName = mContext.ToString(); }
			}
		}
		*/
		
		#endregion

		private bool mOpenByUIMgr = false;

		[SerializeField] private List<SubPanelInfo> mSubPanelInfos = new List<SubPanelInfo>();
		
		public Transform Transform
		{
			get { return transform; }
		}
		
		public UIPanelInfo PanelInfo { get; set; }

		private IUIPanelLoader mUiPanelLoader;
		protected IUIData mUIData;
		GameObject mPrefab;

		public static UIView Load(string panelName, string assetBundleName = null)
		{
			var panelLoader = new DefaultUIPanelLoader();
			var panelPrefab = assetBundleName.IsNullOrEmpty()
				? panelLoader.LoadPanelPrefab(panelName)
				: panelLoader.LoadPanelPrefab(assetBundleName, panelName);
			var obj = Instantiate(panelPrefab);
			var retScript = obj.GetComponent<UIView>();
			retScript.mUiPanelLoader = panelLoader;
			retScript.mPrefab = panelPrefab;
			retScript.mOpenByUIMgr = true;
			return retScript;
		}

		private void Start()
		{
			if (!mOpenByUIMgr)
			{
				Init();
			}
		}

		protected bool mClosed = false;

		protected override IManager mMgr
		{
			get { return PTUIManager.Instance; }
		}

		protected sealed override void OnBeforeDestroy()
		{
			DestroyUI();
		}

		public void Init(IUIData uiData = null)
		{
			mUIData = uiData;
			/*Context = this; */
			InitUI(uiData);
			RegisterUIEvent();


			mSubPanelInfos.ForEach(subPanelInfo => UIMgr.OpenPanel(subPanelInfo.PanelName, subPanelInfo.Level));
		}

		protected virtual void InitUI(IUIData uiData = null)
		{
		}

		protected virtual void RegisterUIEvent()
		{
		}

		protected virtual void DestroyUI()
		{
		}

		/// <summary>
		/// avoid override in child class
		/// </summary>
		protected sealed override void OnDestroy()
		{
			base.OnDestroy();
		}

		/// <summary>
		/// 关闭,不允许子类调用
		/// </summary>
		void IUIView.Close(bool destroyed = true)
		{
			if (PanelInfo.IsNotNull())
				PanelInfo.UIData = mUIData;
			mOnClosed.InvokeGracefully();
			mOnClosed = null;
			OnClose();
			if (destroyed)
			{
				Destroy(gameObject);
			}

			if (mOpenByUIMgr)
			{
				mUiPanelLoader.Unload();
				mUiPanelLoader = null;
			}

			mSubPanelInfos.ForEach(subPanelInfo => UIMgr.ClosePanel(subPanelInfo.PanelName));
			mSubPanelInfos.Clear();
		}

		private Action mOnClosed = null;
		public void OnClosed(Action onClosed)
		{
			mOnClosed = onClosed;
		}

		protected void Back()
		{
			PTUIManager.Instance.Back(name);
		}

		protected void CloseSelf()
		{
			if (mOpenByUIMgr)
			{
				PTUIManager.Instance.CloseUI(name);
				mUIData = null;
			}
			else
			{
				(this as IUIView).Close();
			}
		}

		/// <summary>
		/// 关闭
		/// </summary>
		protected virtual void OnClose()
		{
		}
	}
}