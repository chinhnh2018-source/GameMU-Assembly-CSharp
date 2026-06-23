using System;
using UnityEngine;

public class DelayDeactivateObj : MonoBehaviour
{
	private void Update()
	{
		this.delayTime -= Time.deltaTime;
		if (0f >= this.delayTime && NGUITools.GetActive(base.gameObject))
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
		else if (!NGUITools.GetActive(base.gameObject))
		{
			Object.Destroy(this);
		}
	}

	public float delayTime;
}
