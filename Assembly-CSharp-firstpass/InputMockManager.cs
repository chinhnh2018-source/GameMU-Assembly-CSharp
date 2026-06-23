using System;
using UnityEngine;

public class InputMockManager
{
	public static void RegisterMouse(IInputMockMouse mockMouse)
	{
		InputMockManager.m_mockMouse = mockMouse;
	}

	public static Vector3 mousePosition
	{
		get
		{
			if (InputMockManager.m_mockMouse == null)
			{
				return Vector3.zero;
			}
			return InputMockManager.m_mockMouse.GetMousePosition();
		}
	}

	public static bool GetMouseButtonDown()
	{
		return InputMockManager.m_mockMouse != null && InputMockManager.m_mockMouse.GetMouseButtonDown();
	}

	public static bool GetMouseButtonUp()
	{
		return InputMockManager.m_mockMouse != null && InputMockManager.m_mockMouse.GetMouseButtonUp();
	}

	public static bool GetMouseButton()
	{
		return InputMockManager.m_mockMouse != null && InputMockManager.m_mockMouse.GetMouseButton();
	}

	public static IInputMockMouse m_mockMouse;
}
