using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class DelayPlaySound : MonoBehaviour
{
	private void Awake()
	{
		if (Global.Data.SysSetting.CloseGameAudio)
		{
			if (this.SoundSource != null)
			{
				this.SoundSource.enabled = false;
			}
			return;
		}
	}

	private void Update()
	{
		if (Global.Data != null && Global.Data.SysSetting.CloseGameAudio)
		{
			if (this.SoundSource != null)
			{
				this.SoundSource.enabled = false;
			}
			return;
		}
		if (this.SoundSource != null && this.DelaySeconds <= 0f)
		{
			this.SoundSource.enabled = true;
		}
		if (this.DelaySeconds <= 0f)
		{
			return;
		}
		this.TheTimer += Time.deltaTime;
		if (this.TheTimer >= this.DelaySeconds)
		{
			if (null != this.SoundSource)
			{
				this.SoundSource.enabled = true;
			}
			return;
		}
	}

	public AudioSource SoundSource;

	private float TheTimer;

	public float DelaySeconds;
}
