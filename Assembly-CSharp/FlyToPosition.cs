using System;
using UnityEngine;

public class FlyToPosition : MonoBehaviour
{
	private void Start()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.ToPosition,
			"time",
			this.FlyTime,
			"oncomplete",
			"OnTweenToTartetComplete",
			"oncompletetarget",
			base.gameObject
		}));
	}

	public void OnTweenToTartetComplete()
	{
		if (this.FlyOverNotify != null)
		{
			this.FlyOverNotify(this, EventArgs.Empty);
			base.SendMessageUpwards("OnEffectDestroy", 1);
		}
		else
		{
			base.SendMessageUpwards("OnEffectDestroy", 1);
		}
	}

	public FlyOverNotifyHandler FlyOverNotify;

	public Vector3 ToPosition = Vector3.zero;

	public float FlyTime;
}
