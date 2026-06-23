using System;
using UnityEngine;

public class MUMockMouse : ManualUpdateBehaviour
{
	public Camera MouseCamera
	{
		get
		{
			return this.m_camera;
		}
	}

	public UICamera UICamera
	{
		get
		{
			return this.m_uiCamera;
		}
	}

	private void Awake()
	{
		this.m_camera = this.m_uiCamera.gameObject.GetComponent<Camera>();
	}

	public override void ManualUpdate()
	{
		if (this.m_camera == null)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		if (MUBindingManager.Instance.GetJoystickController().GetButton(MUControllerButtons.UP))
		{
			num2 = 1f;
		}
		if (MUBindingManager.Instance.GetJoystickController().GetButton(MUControllerButtons.DOWN))
		{
			num2 = -1f;
		}
		if (MUBindingManager.Instance.GetJoystickController().GetButton(MUControllerButtons.LEFT))
		{
			num = -1f;
		}
		if (MUBindingManager.Instance.GetJoystickController().GetButton(MUControllerButtons.RIGHT))
		{
			num = 1f;
		}
		Vector3 localPosition = this.mockMouse.transform.localPosition;
		Vector3 localPosition2 = localPosition + new Vector3(this.moveSpeed * num, this.moveSpeed * num2, 0f);
		this.mockMouse.transform.localPosition = localPosition2;
		bool flag = true;
		Vector3 vector = this.m_camera.WorldToScreenPoint(this.mockMouse.transform.position);
		if (vector.x < -1f)
		{
			flag = false;
		}
		if (vector.x > (float)(Screen.width + 1))
		{
			flag = false;
		}
		if (vector.y < 0f)
		{
			flag = false;
		}
		if (vector.y > (float)(Screen.height + 1))
		{
			flag = false;
		}
		if (!flag)
		{
			this.mockMouse.transform.localPosition = localPosition;
		}
	}

	public GameObject mockMouse;

	public float moveSpeed = 3f;

	public UICamera m_uiCamera;

	private Camera m_camera;
}
