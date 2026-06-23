using System;
using UnityEngine;

public class PrintPos : MonoBehaviour
{
	private void OnGUI()
	{
		this.label.text = string.Concat(new object[]
		{
			"lp=",
			base.transform.localPosition,
			", wp=",
			base.transform.position,
			",sp",
			UICamera.currentCamera.WorldToScreenPoint(base.transform.position)
		});
	}

	public UILabel label;
}
