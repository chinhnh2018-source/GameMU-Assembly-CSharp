using System;
using UnityEngine;

public class DelayDestroy : ManualUpdateBehaviour
{
	private void Start()
	{
	}

	public override void ManualUpdate()
	{
		this.delayTime -= Time.deltaTime;
		if (0f >= this.delayTime)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public float delayTime;
}
