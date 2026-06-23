using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	[Serializable]
	public class LerpControlledBob
	{
		public float Offset()
		{
			return this.m_Offset;
		}

		public IEnumerator DoBobCycle()
		{
			float t = 0f;
			while (t < this.BobDuration)
			{
				this.m_Offset = Mathf.Lerp(0f, this.BobAmount, t / this.BobDuration);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			t = 0f;
			while (t < this.BobDuration)
			{
				this.m_Offset = Mathf.Lerp(this.BobAmount, 0f, t / this.BobDuration);
				t += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}
			this.m_Offset = 0f;
			yield break;
		}

		public float BobDuration;

		public float BobAmount;

		private float m_Offset;
	}
}
