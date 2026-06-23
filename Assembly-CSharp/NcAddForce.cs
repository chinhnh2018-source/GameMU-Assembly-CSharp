using System;
using UnityEngine;

public class NcAddForce : NcEffectBehaviour
{
	private void Start()
	{
		if (!base.enabled)
		{
			return;
		}
		this.AddForce();
	}

	private void AddForce()
	{
		if (base.GetComponent<Rigidbody>() != null)
		{
			Vector3 vector;
			vector..ctor(Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + this.m_AddForce.x, Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + this.m_AddForce.y, Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + this.m_AddForce.z);
			base.GetComponent<Rigidbody>().AddForce(vector, this.m_ForceMode);
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public Vector3 m_AddForce = new Vector3(0f, 300f, 0f);

	public Vector3 m_RandomRange = new Vector3(100f, 100f, 100f);

	public ForceMode m_ForceMode;
}
