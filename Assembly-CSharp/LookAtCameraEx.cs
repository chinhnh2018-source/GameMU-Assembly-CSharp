using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class LookAtCameraEx : MonoBehaviour
{
	private void Start()
	{
		if (LayerMask.LayerToName(base.gameObject.layer) == "MUUI" || LayerMask.LayerToName(base.gameObject.layer) == "GUI")
		{
			this.cameraToLookAt = Global.UICamera;
		}
		if (this.cameraToLookAt == null)
		{
			this.cameraToLookAt = Camera.main;
		}
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
