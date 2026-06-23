using System;
using UnityEngine;

[RequireComponent(typeof(Camera)), AddComponentMenu("NGUI/Tween/Orthographic Size")]
public class TweenOrthoSize : UITweener
{
	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	public float orthoSize
	{
		get
		{
			return this.cachedCamera.orthographicSize;
		}
		set
		{
			this.cachedCamera.orthographicSize = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.cachedCamera.orthographicSize = this.from * (1f - factor) + this.to * factor;
	}

	public static TweenOrthoSize Begin(GameObject go, float duration, float to)
	{
		TweenOrthoSize tweenOrthoSize = UITweener.Begin<TweenOrthoSize>(go, duration);
		tweenOrthoSize.from = tweenOrthoSize.orthoSize;
		tweenOrthoSize.to = to;
		if (duration <= 0f)
		{
			tweenOrthoSize.Sample(1f, true);
			tweenOrthoSize.enabled = false;
		}
		return tweenOrthoSize;
	}

	public float from;

	public float to;

	private Camera mCam;
}
