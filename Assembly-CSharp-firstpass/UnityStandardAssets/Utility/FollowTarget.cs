using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class FollowTarget : MonoBehaviour
	{
		private void LateUpdate()
		{
			base.transform.position = this.target.position + this.offset;
		}

		public Transform target;

		public Vector3 offset = new Vector3(0f, 7.5f, 0f);
	}
}
