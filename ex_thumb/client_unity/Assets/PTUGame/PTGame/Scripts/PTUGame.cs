/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;
using System.Collections;
using PTGame.Core;
using System;
using System.IO;

namespace PTGame
{
	public class PTUGame :PTMonoSingleton<PTUGame>
	{
	    public string PTDeviceId = "deviceid";

		public string ChannelName = "putao";
		
		private PTRuntime mRuntime = PTRuntime.ONLINE;

		private PTGameConfig.ConfigData mConfigData;
		
		public override void OnSingletonInit()
		{
			
			this.name = "PTUGame";
			
			PTDeviceId = PTUniInterface.GetPTDeviceId();

			mConfigData= PTGameConfig.GetConfigData();
			
			mRuntime = mConfigData.runtime;

			ChannelName = mConfigData.chanelName;

		}

		public PTRuntime Runtime
		{
			get { return mRuntime; }
		}

		private void OnGUI()
		{
			if (mRuntime!=PTRuntime.ONLINE)
			{
				GUI.Label(new Rect(10,10,200,100),"Runtime:"+mRuntime.ToString()+"_"+mConfigData.gameConfig.custom);
			}
		}
	}

	
}