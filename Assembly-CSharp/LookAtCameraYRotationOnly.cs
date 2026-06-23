using System;
using UnityEngine;

public class LookAtCameraYRotationOnly : MonoBehaviour
{
	private void Start()
	{
		if (this.cameraToLookAt == null)
		{
			this.cameraToLookAt = Camera.main;
		}
	}

	private void Update()
	{
		if (this.cameraToLookAt)
		{
			Vector3 vector = this.cameraToLookAt.transform.position - base.transform.position;
			vector.z = (vector.x = 0f);
			base.transform.LookAt(this.cameraToLookAt.transform.position - vector, Vector3.up);
		}
	}

	private Camera cameraToLookAt;
}
