using System;
using UnityEngine;

[AddComponentMenu("Game/UI/Button Key Binding")]
public class UIButtonKeyBinding : MonoBehaviour
{
	private void Update()
	{
		if (!UICamera.inputHasFocus)
		{
			if (this.keyCode == null)
			{
				return;
			}
			if (Input.GetKeyDown(this.keyCode))
			{
				base.SendMessage("OnPress", true, 1);
			}
			if (Input.GetKeyUp(this.keyCode))
			{
				base.SendMessage("OnPress", false, 1);
				base.SendMessage("OnClick", 1);
			}
		}
	}

	public KeyCode keyCode;
}
