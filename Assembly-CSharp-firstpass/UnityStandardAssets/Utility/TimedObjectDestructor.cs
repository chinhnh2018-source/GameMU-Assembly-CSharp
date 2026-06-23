using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class TimedObjectDestructor : MonoBehaviour
	{
		private void Awake()
		{
			base.Invoke("DestroyNow", this.m_TimeOut);
		}

		private void DestroyNow()
		{
			if (this.m_DetachChildren)
			{
				base.transform.DetachChildren();
			}
			Object.DestroyObject(base.gameObject);
		}

		[SerializeField]
		private float m_TimeOut = 1f;

		[SerializeField]
		private bool m_DetachChildren;
	}
}
