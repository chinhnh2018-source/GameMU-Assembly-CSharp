using System;
using UnityEngine;

public interface IInputMockMouse
{
	Vector3 GetMousePosition();

	bool GetMouseButtonDown();

	bool GetMouseButtonUp();

	bool GetMouseButton();
}
