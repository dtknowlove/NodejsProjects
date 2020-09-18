/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 * Copyright (c) 2018.3 liqingyun@putao.com
 ****************************************************************************/

using PTGame.Core;

namespace PTGame.Framework
{
    using System;

    public class AudioVoiceNode : ExecuteNode
    {
        public string VoiceName;

        public Action OnVoiceBeganCallback
        {
            get { return OnBeganCallback; }
            set { OnBeganCallback = value; }
        }

        public Action OnVoiceEndedCallback
        {
            get { return OnEndedCallback; }
            set { OnEndedCallback = value; }
        }

        public AudioVoiceNode(string voiceName)
        {
            VoiceName = voiceName;
        }

        protected override void OnBegin()
        {
            OnVoiceBeganCallback.InvokeGracefully();
            AudioManager.PlayVoice(new AudioVoiceData()
            {
                VoiceName = VoiceName,
                OnVoiceBegin = OnVoiceBeganCallback,
                OnVoiceEnd = () =>
                {
                    Finished = true;
                    OnFinish();
                }
            });
        }

        protected override void OnFinish()
        {
            OnVoiceEndedCallback.InvokeGracefully();
        }

        protected override void OnDispose()
        {
            VoiceName = null;
        }
    }
}