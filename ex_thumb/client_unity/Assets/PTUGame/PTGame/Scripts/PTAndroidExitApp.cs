/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/
using UnityEngine;

namespace PTGame
{
	public class PTAndroidExitApp : MonoBehaviour
	{
		public string strTip = "再按一次退出应用";

		private float timeCounter = 0;

		private const byte STATE_EXIT_INIT = 0;
		private const byte STATE_EXIT_WAIT = 1;
		private const byte STATE_EXIT_EXIT = 2;
		private byte exitState = 0;

		void Update()
		{

			switch (exitState)
			{
				case STATE_EXIT_INIT:
					if (Input.GetKeyUp(KeyCode.Escape))
					{
						exitState = STATE_EXIT_WAIT;
						PTAndroidInterface.Instance.ShowExitAppTip(strTip);
						timeCounter = 0;
					}

					break;
				case STATE_EXIT_WAIT:
					timeCounter += Time.deltaTime;
					if (timeCounter < 2)
					{
						if (Input.GetKeyUp(KeyCode.Escape))
						{

							exitState = STATE_EXIT_EXIT;
							PTAndroidInterface.Instance.ExitApp();
						}
					}
					else
					{
						if (Input.GetKeyUp(KeyCode.Escape))
						{
							exitState = STATE_EXIT_INIT;
						}
					}

					break;
			}


		}
	}
}
