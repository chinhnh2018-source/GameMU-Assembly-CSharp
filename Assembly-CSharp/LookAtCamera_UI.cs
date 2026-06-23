using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class LookAtCamera_UI : MonoBehaviour
{
	private void Start()
	{
		this.cameraToLookAt = Global.UICamera;
		if (null == this.cameraToLookAt)
		{
			this.cameraToLookAt = Camera.main;
		}
	}

	private void OnDisable()
	{
		Object.Destroy(this);
	}

	private void Update()
	{
		if (this.cameraToLookAt != null)
		{
			base.transform.rotation = Quaternion.LookRotation(this.cameraToLookAt.transform.position - base.transform.position);
		}
	}

	private Camera cameraToLookAt;
}
