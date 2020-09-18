/****************************************************************************
 * 2017 ~ 2018.5 liqingyun
 ****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
	using System;
	
	public class AudioSoundNode : ExecuteNode
	{
		public string SoundName;

		public Action OnSoundBeganCallback
		{
			get { return OnBeganCallback; }
			set { OnBeganCallback = value; }
		}

		public Action OnSoundEndedCallback
		{
			get { return OnEndedCallback; }
			set { OnEndedCallback = value; }
		}

		public AudioSoundNode(string soundName)
		{
			SoundName = soundName;
		}

		protected override void OnBegin()
		{
			base.OnBegin();
			AudioManager.PlaySound(SoundName, onEnd: () => { Finished = true; });
		}

		protected override void OnExecute(float dt)
		{
			
		}

		protected override void OnFinish()
		{
			OnSoundEndedCallback.InvokeGracefully();
		}

		protected override void OnDispose()
		{
			SoundName = null;
		}
	}
}