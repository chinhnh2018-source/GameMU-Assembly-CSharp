using System;
using UnityEngine;

public class ParticleSize : MonoBehaviour
{
	public void SetParticleSize()
	{
		if (this.ps != null && this.ps.Length > 0)
		{
			for (int i = 0; i < this.ps.Length; i++)
			{
				if (this.ps[i] != null)
				{
					this.ps[i].startSize = base.transform.root.localScale.x * this.ps[i].startSize;
				}
			}
		}
	}

	public ParticleSystem[] ps;
}
