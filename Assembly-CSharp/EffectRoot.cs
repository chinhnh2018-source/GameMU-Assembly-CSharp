using System;
using UnityEngine;

public class EffectRoot : MonoBehaviour
{
	public void OnEffectDestroy()
	{
		if (this.EffectRootNotify != null)
		{
			this.EffectRootNotify(this, EventArgs.Empty);
			Object.Destroy(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public EffectRootNotifyHandler EffectRootNotify;
}
