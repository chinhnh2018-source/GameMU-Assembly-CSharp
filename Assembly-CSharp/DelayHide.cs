using System;
using UnityEngine;

public class DelayHide : MonoBehaviour
{
	private void OnEnable()
	{
		if (base.IsInvoking("ExecuteHide"))
		{
			base.CancelInvoke("ExecuteHide");
		}
		base.Invoke("ExecuteHide", this.delayTime);
	}

	private void ExecuteHide()
	{
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(false);
		}
	}

	public float delayTime;
}
