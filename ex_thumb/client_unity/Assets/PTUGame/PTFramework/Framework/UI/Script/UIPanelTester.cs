/****************************************************************************
 * Copyright (c) 2018.5 ~ 2018.6 liqingyun
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using PTGame.Core;
using UnityEngine;
using PTGame.ResKit;

namespace PTGame.Framework
{
	[System.Serializable]
	public class UIPanelTesterInfo
	{
		/// <summary>
		/// 页面的名字
		/// </summary>
		public string PanelName;

		/// <summary>
		/// 层级名字
		/// </summary>
		public UILevel Level;
	}

	public class UIPanelTester : MonoBehaviour
	{
		#region TODO: 要将这部分替换成 UIPanelTesterInfo

		/// <summary>
		/// 页面的名字
		/// </summary>
		public string PanelName;

		/// <summary>
		/// 层级名字
		/// </summary>
		public UILevel Level;

		#endregion

		private ResLoader mResLoader = ResLoader.Allocate();

		[SerializeField] private List<UIPanelTesterInfo> mOtherPanels;


		public string TestStateName;
		public bool   SendStateEvent = false;

		private void Awake()
		{
			ResMgr.Instance.SimulateInit();
//			PTLanguageMgr.Instance.Init(
//				I18nConfig.LoadConfig(mResLoader.LoadSync<TextAsset>("I18n").text) as I18nConfig);
			AudioManager.Instance.Init();
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(0.2f);

			UIMgr.OpenPanel(PanelName, Level);

			mOtherPanels.ForEach(panelTesterInfo =>
			{
				UIMgr.OpenPanel(panelTesterInfo.PanelName, panelTesterInfo.Level);
			});

			yield return new WaitForSeconds(0.2f);

			if (SendStateEvent && TestStateName.IsNotNullAndEmpty())
			{
//				PTEventSystem.SendEvent(GuideEvent.StateChanged, TestStateName);
			}
		}

		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;
		}
	}
}