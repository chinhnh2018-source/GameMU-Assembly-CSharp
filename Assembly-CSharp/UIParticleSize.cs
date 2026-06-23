using System;
using UnityEngine;

public class UIParticleSize : MonoBehaviour
{
	private void Start()
	{
		if (this.ps != null && this.ps.Length > 0 && base.transform.root.gameObject.layer == LayerMask.NameToLayer("MUUI"))
		{
			for (int i = 0; i < this.ps.Length; i++)
			{
				if (this.ps[i] != null)
				{
					this.ps[i].startSize = this.scale * this.ps[i].startSize;
				}
			}
		}
	}

	public ParticleSystem[] ps;

	public float scale = 0.4f;
}
