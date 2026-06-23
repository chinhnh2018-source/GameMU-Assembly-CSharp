using System;
using UnityEngine;

namespace Xft
{
	public struct STransform
	{
		public void Reset()
		{
			this.position = Vector3.zero;
			this.rotation = Quaternion.identity;
		}

		public void LookAt(Vector3 dir, Vector3 up)
		{
			this.rotation = Quaternion.LookRotation(dir, up);
		}

		public Vector3 position;

		public Quaternion rotation;
	}
}
