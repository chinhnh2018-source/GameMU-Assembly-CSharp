using System;
using UnityEngine;

public class MUInputMockMouse : IInputMockMouse
{
	public MUInputMockMouse(MUMockMouse mouse, MUJoystickController joystickController)
	{
		this.m_mouse = mouse;
		this.m_joystickController = joystickController;
	}

	public Vector3 GetMousePosition()
	{
		if (this.m_mouse == null)
		{
			return Vector3.zero;
		}
		if (this.m_mouse.MouseCamera == null)
		{
			return Vector3.zero;
		}
		if (this.m_mouse.mockMouse == null)
		{
			return Vector3.zero;
		}
		return this.m_mouse.MouseCamera.WorldToScreenPoint(this.m_mouse.mockMouse.transform.position);
	}

	public bool GetMouseButtonDown()
	{
		return !(this.m_joystickController == null) && this.m_joystickController.GetButtonDown(this.type);
	}

	public bool GetMouseButtonUp()
	{
		return !(this.m_joystickController == null) && this.m_joystickController.GetButtonUp(this.type);
	}

	public bool GetMouseButton()
	{
		return !(this.m_joystickController == null) && this.m_joystickController.GetButton(this.type);
	}

	public MUControllerButtons type = MUControllerButtons.RAS_BUTTON;

	private MUMockMouse m_mouse;

	private MUJoystickController m_joystickController;
}
