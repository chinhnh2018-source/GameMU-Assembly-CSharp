using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class ObjectResetter : MonoBehaviour
	{
		private void Start()
		{
			this.originalStructure = new List<Transform>(base.GetComponentsInChildren<Transform>());
			this.originalPosition = base.transform.position;
			this.originalRotation = base.transform.rotation;
			this.Rigidbody = base.GetComponent<Rigidbody>();
		}

		public void DelayedReset(float delay)
		{
			base.StartCoroutine(this.ResetCoroutine(delay));
		}

		public IEnumerator ResetCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			foreach (Transform t in base.GetComponentsInChildren<Transform>())
			{
				if (!this.originalStructure.Contains(t))
				{
					t.parent = null;
				}
			}
			base.transform.position = this.originalPosition;
			base.transform.rotation = this.originalRotation;
			if (this.Rigidbody)
			{
				this.Rigidbody.velocity = Vector3.zero;
				this.Rigidbody.angularVelocity = Vector3.zero;
			}
			base.SendMessage("Reset");
			yield break;
		}

		private Vector3 originalPosition;

		private Quaternion originalRotation;

		private List<Transform> originalStructure;

		private Rigidbody Rigidbody;
	}
}
