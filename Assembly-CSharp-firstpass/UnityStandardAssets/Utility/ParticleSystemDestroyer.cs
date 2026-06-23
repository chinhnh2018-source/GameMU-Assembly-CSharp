using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class ParticleSystemDestroyer : MonoBehaviour
	{
		private IEnumerator Start()
		{
			ParticleSystem[] systems = base.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem system in systems)
			{
				this.m_MaxLifetime = Mathf.Max(system.startLifetime, this.m_MaxLifetime);
			}
			float stopTime = Time.time + Random.Range(this.minDuration, this.maxDuration);
			while (Time.time < stopTime || this.m_EarlyStop)
			{
				yield return null;
			}
			Debug.Log("stopping " + base.name);
			foreach (ParticleSystem system2 in systems)
			{
				system2.enableEmission = false;
			}
			base.BroadcastMessage("Extinguish", 1);
			yield return new WaitForSeconds(this.m_MaxLifetime);
			Object.Destroy(base.gameObject);
			yield break;
		}

		public void Stop()
		{
			this.m_EarlyStop = true;
		}

		public float minDuration = 8f;

		public float maxDuration = 10f;

		private float m_MaxLifetime;

		private bool m_EarlyStop;
	}
}
