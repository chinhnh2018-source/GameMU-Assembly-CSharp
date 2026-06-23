using System;
using UnityEngine;

public class LookAtCamera_Y : MonoBehaviour
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
		Vector3 vector = this.cameraToLookAt.transform.position - base.transform.position;
		vector.y = 0f;
		base.transform.rotation = Quaternion.LookRotation(vector);
	}

	private Camera cameraToLookAt;
}
