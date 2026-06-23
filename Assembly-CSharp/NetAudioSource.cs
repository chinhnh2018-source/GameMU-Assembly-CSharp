using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NetAudioSource : TTMonoBehaviour
{
	private void Awake()
	{
		this.mAudioSource = base.GetComponent<AudioSource>();
		if (this.mAudioSource != null && this.isAddGlobelController)
		{
			this.AudioVolume = this.mAudioSource.volume;
			if (Global.Data != null && Global.Data.SysSetting != null)
			{
				if (Global.Data.SysSetting.CloseGameMusic)
				{
					this.mAudioSource.volume = 0f;
				}
				else
				{
					this.mAudioSource.volume = this.AudioVolume;
				}
			}
		}
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(this.URL))
		{
			return;
		}
		MuAssetManager.Instance.StopLoadAsset(this.AudioSourceID, new Action<Object>(this.LoadComplete));
		this.AudioSourceID = this.URL;
		MuAssetManager.Instance.BeginLoadAsset(this.URL, new Action<Object>(this.LoadComplete), base.gameObject);
	}

	private void SetState(bool isClose)
	{
		if (this.mAudioSource != null)
		{
			this.mAudioSource.volume = ((!isClose) ? this.AudioVolume : 0f);
		}
	}

	public void PlayAudio(string url, bool loop = false, bool force = false)
	{
		if (force)
		{
			this.URL = null;
		}
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		if (url == this.URL && this.mAudioSource.clip)
		{
			if (!this.mAudioSource.isPlaying)
			{
				this.mAudioSource.Play();
			}
			else
			{
				this.PlayAgain();
			}
			return;
		}
		this.StopPlay();
		this.URL = url;
		this.LoopAudio = loop;
		MuAssetManager.Instance.StopLoadAsset(this.AudioSourceID, new Action<Object>(this.LoadComplete));
		this.AudioSourceID = this.URL;
		try
		{
			MuAssetManager.Instance.BeginLoadAsset(this.URL, new Action<Object>(this.LoadComplete), base.gameObject);
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"the audio file is " + this.URL + " : the Exception is " + ex.Message
			});
		}
	}

	private void LoadComplete(Object go)
	{
		AudioClip audioClip = (AudioClip)go;
		if (audioClip)
		{
			if (this.mAudioSource.isPlaying)
			{
				this.mAudioSource.Stop();
				this.mAudioSource.clip = null;
			}
			this.mAudioSource.clip = audioClip;
			this.mAudioSource.loop = this.LoopAudio;
			if (this.DelayPlaySecs > 0f)
			{
				this.mAudioSource.PlayDelayed(this.DelayPlaySecs);
			}
			else
			{
				this.mAudioSource.Play();
			}
		}
	}

	public bool IsPlaying()
	{
		return !(this.mAudioSource == null) && this.mAudioSource.isPlaying;
	}

	public bool IsPlaying(string path)
	{
		return this.URL == path && this.mAudioSource.isPlaying;
	}

	public void StopPlay()
	{
		if (null == this.mAudioSource)
		{
			return;
		}
		if (this.mAudioSource.isPlaying)
		{
			this.mAudioSource.Stop();
			this.mAudioSource.clip = null;
		}
		if (!this.clipManager)
		{
			this.clipManager = base.GetComponent<MuAsset>();
		}
		if (this.clipManager)
		{
			this.clipManager.ReleaseResource(this.AudioSourceID);
		}
	}

	public void PausePlay(bool isPause = true)
	{
		if (null == this.mAudioSource)
		{
			return;
		}
		if (isPause && this.mAudioSource.isPlaying)
		{
			this.mAudioSource.Pause();
		}
		else if (!isPause)
		{
			this.mAudioSource.UnPause();
		}
	}

	public void PlayAgain()
	{
		if (null == this.mAudioSource.clip)
		{
			return;
		}
		this.mAudioSource.Stop();
		this.mAudioSource.Play();
	}

	public string URL;

	public bool LoopAudio;

	public float DelayPlaySecs;

	private AudioSource mAudioSource;

	private MuAsset clipManager;

	public bool isAddGlobelController = true;

	private float AudioVolume;

	private string AudioSourceID = string.Empty;
}
