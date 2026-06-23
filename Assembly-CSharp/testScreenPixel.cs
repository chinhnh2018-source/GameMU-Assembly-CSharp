using System;
using UnityEngine;

public class testScreenPixel : MonoBehaviour
{
	private void OnGUI()
	{
		Rect pixelRect = this.uiCamera.pixelRect;
		this.Lable.text = string.Concat(new object[]
		{
			"rect=",
			pixelRect,
			",sw=",
			Screen.width,
			",sh=",
			Screen.height
		});
	}

	public Camera uiCamera;

	public UILabel Lable;
}
