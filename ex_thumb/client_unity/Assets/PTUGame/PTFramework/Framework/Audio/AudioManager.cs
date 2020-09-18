/****************************************************************************
 * 2018.8 maoling@putao.com 去掉消息方式
 * Copyright (c) 2017 ouyanggongping@putao.com
 * Copyright (c) 2017 liqingyun@putao.com
 * Copyright (c) 2018.4 liqingyun@putao.com
 ****************************************************************************/

/// <summary>
/// 音频管理工具
/// </summary>
using System;
using PTGame.Core;

namespace PTGame.Framework
{
	using System.Collections.Generic;
	using UnityEngine;
	using PTGame.ResKit;

	#region 消息id定义

	public enum AudioEvent
	{
		Began = PTMgrID.Audio,
		PlaySound,
		StopSound,
		PlayVoice,
		StopVoice,
		AddRetainAudio,
		RemoveRetainAudioAudio,
		Ended
	}

	#endregion

	public enum VolumeLevel
	{
		Max,
		Normal,
		Min
	}

	public class AudioMusicData
	{
		public string MusicName;
		public string ABName;
		public float Volume = 1;//add by van
		public bool Loop = true;

		/// <summary>
		/// 是否受MusicOn(bool)管理
		/// </summary>
		public bool AllowMusicOff = true;

		public Action OnMusicBegin;
		public Action OnMusicEnd;
	}

	public class AudioSoundData
	{
		public string SoundName;
		public string ABName;
		public bool Loop = false;
		public float Volume = -1; //ignore AudioManager.SoundVolume
		public bool SyncLoad = true;
		public Action OnSoundBegin;
		public Action OnSoundEnd;
	}

	public class AudioVoiceData : PTMsg
	{
		public string VoiceName;
		public string ABName;
		public bool Loop = false;
		public float Volume = -1;
		public bool SyncLoad = false;
		public Action OnVoiceBegin;
		public Action OnVoiceEnd;
	}

	/// <inheritdoc />
	/// <summary>
	/// TODO:不支持本地化
	/// </summary>
	[PTMonoSingletonPath("[Framework]/AudioManager")]
	public class AudioManager : MonoBehaviour, ISingleton
	{
		[RuntimeInitializeOnLoadMethod]
		static void Initialize()
		{
			var init = AudioManager.Instance;
		}
		
		#region Audio设置数据

		// 用来存储的Key
		const string KEY_AUDIO_MANAGER_SOUND_ON = "KEY_AUDIO_MANAGER_SOUND_ON";

		const string KEY_AUDIO_MANAGER_MUSIC_ON     = "KEY_AUDIO_MANAGER_MUSIC_ON";
		const string KEY_AUDIO_MANAGER_VOICE_ON     = "KEY_AUDIO_MANAGER_VOICE_ON";
		const string KEY_AUDIO_MANAGER_VOICE_VOLUME = "KEY_AUDIO_MANAGER_VOICE_VOLUME";
		const string KEY_AUDIO_MANAGER_SOUND_VOLUME = "KEY_AUDIO_MANAGER_SOUND_VOLUME";
		const string KEY_AUDIO_MANAGER_MUSIC_VOLUME = "KEY_AUDIO_MANAGER_MUSIC_VOLUME";

		/// <summary>
		/// 读取音频设置
		/// </summary>
		private static void ReadAudioSetting()
		{
			IsSoundOn = PlayerPrefs.GetInt(KEY_AUDIO_MANAGER_SOUND_ON, 1) == 1;
			IsMusicOn = PlayerPrefs.GetInt(KEY_AUDIO_MANAGER_MUSIC_ON, 1) == 1;
			IsVoiceOn = PlayerPrefs.GetInt(KEY_AUDIO_MANAGER_VOICE_ON, 1) == 1;

			SoundVolume = PlayerPrefs.GetFloat(KEY_AUDIO_MANAGER_SOUND_VOLUME, 1.0f);
			MusicVolume = PlayerPrefs.GetFloat(KEY_AUDIO_MANAGER_MUSIC_VOLUME, 1.0f);
			VoiceVolume = PlayerPrefs.GetFloat(KEY_AUDIO_MANAGER_VOICE_VOLUME, 1.0f);
		}

		/// <summary>
		/// 保存音频数据
		/// </summary>
		private static void SaveAudioSetting()
		{
			PlayerPrefs.SetInt(KEY_AUDIO_MANAGER_SOUND_ON, IsSoundOn ? 1 : 0);
			PlayerPrefs.SetInt(KEY_AUDIO_MANAGER_MUSIC_ON, IsMusicOn ? 1 : 0);
			PlayerPrefs.SetInt(KEY_AUDIO_MANAGER_VOICE_ON, IsVoiceOn ? 1 : 0);
			PlayerPrefs.SetFloat(KEY_AUDIO_MANAGER_SOUND_VOLUME, SoundVolume);
			PlayerPrefs.SetFloat(KEY_AUDIO_MANAGER_MUSIC_VOLUME, MusicVolume);
			PlayerPrefs.SetFloat(KEY_AUDIO_MANAGER_VOICE_VOLUME, VoiceVolume);			
		}

		private void OnApplicationQuit()
		{
			SaveAudioSetting();
		}

		private void OnApplicationFocus(bool focus)
		{
			SaveAudioSetting();
		}

		private void OnApplicationPause(bool pause)
		{
			SaveAudioSetting();
		}

		#endregion

		protected int mMaxSoundCount = 3;
		protected AudioUnit mMainUnit;
		protected AudioUnit mVoiceUnit;
		private static List<AudioUnit> mAudioUnitList = new List<AudioUnit>();

		public void OnSingletonInit()
		{
			SafeObjectPool<AudioUnit>.Instance.Init(mMaxSoundCount, 1);
			mMainUnit = new AudioUnit {UsedCache = false};
			mVoiceUnit = new AudioUnit {UsedCache = false};

			// 确保有一个AudioListener
			if (FindObjectOfType<AudioListener>() == null)
			{
				gameObject.AddComponent<AudioListener>();
			}

			gameObject.transform.position = Vector3.zero;

			// 读取存储
			ReadAudioSetting();			
		}

		public void Init()
		{
			PTDebug.Log("AudioManager.Init");
		}

		public static bool IsOn
		{
			get { return IsSoundOn && IsMusicOn && IsVoiceOn; }
		}

		public static void On()
		{
			SoundOn();
			MusicOn();
			VoiceOn();
		}

		public static void Off()
		{
			SoundOff();
			MusicOff();
			VoiceOff();
		}

		public static void SoundOn()
		{
			IsSoundOn = true;
		}

		public static void SoundOff()
		{
			IsSoundOn = false;
		}

		public static void VoiceOn()
		{
			IsVoiceOn = true;
		}

		public static void VoiceOff()
		{
			IsVoiceOn = false;
		}

		private static AudioMusicData CurrentMusic;

		public static void MusicOn()
		{
			IsMusicOn = true;
			if (CurrentMusic != null)
				PlayMusic(CurrentMusic);
//			CurrentMusic = null;
		}

		public static void MusicOff()
		{
			IsMusicOn = false;
			//don't call StopMusic(), because CurrentMusic needs to be retained!!!
			Instance.mMainUnit.Stop();
		}

		public static bool IsSoundOn { get; private set; }

		public static bool IsMusicOn { get; private set; }

		public static bool IsVoiceOn { get; private set; }

		public static float SoundVolume { get; private set; }

		public static float MusicVolume { get; private set; }

		public static float VoiceVolume { get; private set; }

		private static void SetVolume(AudioUnit audioUnit, VolumeLevel volumeLevel)
		{
			switch (volumeLevel)
			{
				case VolumeLevel.Max:
					audioUnit.SetVolume(1.0f);
					break;
				case VolumeLevel.Normal:
					audioUnit.SetVolume(0.5f);
					break;
				case VolumeLevel.Min:
					audioUnit.SetVolume(0.2f);
					break;
			}
		}
		
		public static void PlayMusic(string musicName, bool loop = true, Action onBegan = null, Action onEnd = null, bool allowMusicOff = true)
		{
			PlayMusic(new AudioMusicData()
			{
				MusicName = musicName, Loop = loop, OnMusicBegin = onBegan, OnMusicEnd = onEnd, AllowMusicOff = allowMusicOff
			});
		}

		public static void PlayMusicWithVolume(string musicName,float volume, bool loop = true, Action onBegan = null, Action onEnd = null, bool allowMusicOff = true)
		{
			PlayMusic(new AudioMusicData()
			{
				MusicName = musicName, Volume = volume, Loop = loop, OnMusicBegin = onBegan, OnMusicEnd = onEnd, AllowMusicOff = allowMusicOff
			});
		}

		public static void PlayMusic(AudioMusicData musicData)
		{
			CurrentMusic = new AudioMusicData
			{
				MusicName = musicData.MusicName,
				ABName = musicData.ABName,
				Loop = musicData.Loop,
				Volume = musicData.Volume
				
			};
			
			var self = Instance;

			if (!IsMusicOn && musicData.AllowMusicOff)
			{
				musicData.OnMusicBegin.InvokeGracefully();
				musicData.OnMusicEnd.InvokeGracefully();
				return;
			}

			PTDebug.Log(">>>>>> Start Play Music");

			// TODO: 需要按照这个顺序去 之后查一下原因
			//需要先注册事件，然后再play
			self.mMainUnit.SetOnStartListener(musicUnit=>
			{
				musicData.OnMusicBegin.InvokeGracefully();

				//调用完就置为null，否则应用层每注册一个而没有注销，都会调用
				self.mMainUnit.SetOnStartListener(null);
			});

			self.mMainUnit.SetAudio(self.gameObject, musicData.MusicName, musicData.ABName,
									loop: musicData.Loop, volume: musicData.Volume);

			self.mMainUnit.SetOnFinishListener(musicUnit=>
			{
				musicData.OnMusicEnd.InvokeGracefully();

				//调用完就置为null，否则应用层每注册一个而没有注销，都会调用
				self.mMainUnit.SetOnFinishListener(null);
			});
		}

		/// <summary>
		/// 停止播放音乐 
		/// </summary>
		public static void StopMusic()
		{
			CurrentMusic = null;
			Instance.mMainUnit.Stop();
		}

		public static void PauseMusic()
		{
			Instance.mMainUnit.Pause();
		}

		public static void ResumeMusic()
		{
			Instance.mMainUnit.Resume();
		}

		public static AudioUnit PlaySound(string soundName, bool loop = false, Action onEnd = null)
		{
			return PlaySound(new AudioSoundData()
			{
				SoundName = soundName, Loop = loop, OnSoundEnd = onEnd
			});
		}

		public static AudioUnit PlaySound(AudioSoundData soundData,bool cache = false)
		{
			if (string.IsNullOrEmpty(soundData.SoundName)) return null;
			if (!IsSoundOn) return null;
			var unit = SafeObjectPool<AudioUnit>.Instance.Allocate();

			if (soundData.OnSoundBegin != null)
			{
				unit.SetOnFinishListener((audioUnit) =>
				{
					soundData.OnSoundBegin();
					unit.SetOnFinishListener(null);
				});
			}

			unit.SetAudio(Instance.gameObject, soundData.SoundName, soundData.ABName,
						  syncLoad: soundData.SyncLoad, loop: soundData.Loop, 
						  volume: soundData.Volume >= 0 ? soundData.Volume : SoundVolume);

			if (soundData.OnSoundEnd != null)
			{
				unit.SetOnFinishListener((audioUnit) =>
				{
					soundData.OnSoundEnd();
					unit.SetOnFinishListener(null);
				});
			}
			
			if (cache)
			{
				mAudioUnitList.Add(unit);	
			}
			return unit;
		}

		public static void PlayVoice(string voiceName, bool loop = false, Action onEnd = null)
		{
			PlayVoice(new AudioVoiceData()
			{
				VoiceName = voiceName,  Loop = loop, OnVoiceEnd = onEnd
			});
		}

		/// <summary>
		/// 播放音频，不受音效关闭与否控制
		/// </summary>
		public static void PlayVoice(AudioVoiceData voiceData)
		{
			if (string.IsNullOrEmpty(voiceData.VoiceName)) return;
			if (!IsVoiceOn) return;
			
			var voiceUnit = Instance.mVoiceUnit;
			
			voiceUnit.SetOnStartListener(delegate
			{
				SetVolume(Instance.mMainUnit, VolumeLevel.Min);
				if (voiceData.OnVoiceBegin != null)
					voiceData.OnVoiceBegin();
				voiceUnit.SetOnStartListener(null);
			});

			voiceUnit.SetAudio(Instance.gameObject, voiceData.VoiceName, voiceData.ABName,
							   syncLoad: voiceData.SyncLoad, loop: voiceData.Loop, volume: voiceData.Volume >= 0 ? voiceData.Volume : VoiceVolume);

			voiceUnit.SetOnFinishListener(delegate
			{
				SetVolume(Instance.mMainUnit, VolumeLevel.Max);
				if (voiceData.OnVoiceEnd != null)
					voiceData.OnVoiceEnd();
				voiceUnit.SetOnFinishListener(null);
			});
		}

		public static void StopVoice()
		{
			Instance.mVoiceUnit.Stop();
		}
		
		public static void PlayBtnSound(AudioClip clip)
		{
			//声音的播放跟随背景音乐
			if(!IsSoundOn)return;
			var audioSource = Instance.gameObject.AddComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.Play();
			audioSource.DestroySelfAfterDelay(clip.length);
		}

		#region 单例实现

		private AudioManager() {}

		public static AudioManager Instance
		{
			get { return PTMonoSingletonProperty<AudioManager>.Instance; }
		}

		#endregion

		//常驻内存不卸载音频资源
		protected ResLoader mRetainResLoader;

		protected List<string> mRetainAudioNames;

		/// <summary>
		/// 添加常驻音频资源，建议尽早添加，不要在用的时候再添加
		/// </summary>
		public void AddRetainAudio(string audioName, string abName = null)
		{
			if (mRetainResLoader == null)
				mRetainResLoader = ResLoader.Allocate();
			if (mRetainAudioNames == null)
				mRetainAudioNames = new List<string>();

			if (!mRetainAudioNames.Contains(audioName))
			{
				mRetainAudioNames.Add(audioName);
				mRetainResLoader.LoadAsync(audioName, abName);
			}
		}

		/// <summary>
		/// 删除常驻音频资源
		/// </summary>
		private void RemoveRetainAudio(string audioName, string abName = null)
		{
			if (mRetainAudioNames != null && mRetainAudioNames.Remove(audioName))
			{
				mRetainResLoader.Release(audioName, abName);
			}
		}
		
		/// <summary>
		/// 编程指令:StopAllSound
		/// </summary>
		public static void StopAllSound()
		{
			for (int i =  mAudioUnitList.Count - 1; i >= 0; i--)
			{
				if (mAudioUnitList[i] != null)
				{
					mAudioUnitList[i].Stop();
					mAudioUnitList[i] = null;
				}
			}
			mAudioUnitList.Clear();
		}
	}
}