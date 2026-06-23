using System;
using UnityEngine;

public class EffectsScript : MonoBehaviour
{
	public float TimeElapse
	{
		get
		{
			return this._TimeElapse;
		}
	}

	public void SetParams(float timeTotal, float timeElapse, bool autoRepeat, bool autoRemove)
	{
		this.TimeTotal = timeTotal;
		this._TimeElapse = timeElapse;
		this.AutoRepeat = autoRepeat;
		this.AutoRemove = autoRemove;
	}

	private void Start()
	{
		if (null == this.Target)
		{
			this.Target = base.gameObject.transform.GetChild(0).gameObject;
		}
	}

	private void Update()
	{
		this._TimeElapse += Time.deltaTime;
		if (this.TimeTotal > 0f && this._TimeElapse > this.TimeTotal)
		{
			this.Stop();
			return;
		}
		if (this.CheckChildState)
		{
			if (null == this.Target || this.isStoped)
			{
				if (base.transform.GetChildCount() == 0)
				{
					return;
				}
				this.Target = base.transform.GetChild(0).gameObject;
			}
			if (!this.IsPlaying())
			{
				if (this.AutoRepeat)
				{
					this.Replay();
				}
				else
				{
					this.Stop();
				}
			}
		}
	}

	public bool IsPlaying()
	{
		return (null != this.Target.GetComponent<ParticleSystem>() && this.Target.GetComponent<ParticleSystem>().isPlaying) || (null != this.Target.GetComponent<Animation>() && this.Target.GetComponent<Animation>().isPlaying) || (null != this.Target.GetComponent<AudioSource>() && this.Target.GetComponent<AudioSource>().isPlaying);
	}

	public bool Replay()
	{
		if (null != this.Target.GetComponent<ParticleSystem>() && !this.Target.GetComponent<ParticleSystem>().isPlaying)
		{
			this.Target.GetComponent<ParticleSystem>().Play();
		}
		if (null != this.Target.GetComponent<Animation>() && !this.Target.GetComponent<Animation>().isPlaying)
		{
			this.Target.GetComponent<Animation>().Play();
		}
		if (null != this.Target.GetComponent<AudioSource>() && !this.Target.GetComponent<AudioSource>().isPlaying)
		{
			this.Target.GetComponent<AudioSource>().Play();
		}
		return true;
	}

	public bool DestoryGameObject()
	{
		Object.DestroyObject(base.gameObject);
		return true;
	}

	public bool Stop()
	{
		this.isStoped = true;
		if (this.AutoRemove)
		{
			this.DestoryGameObject();
		}
		return true;
	}

	public float TimeTotal;

	private float _TimeElapse;

	public bool AutoRepeat;

	public bool AutoRemove = true;

	public bool isStoped;

	public bool CheckChildState;

	public GameObject Target;
}
