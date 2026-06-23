using System;
using UnityEngine;

public class testLabelPos : MonoBehaviour
{
	private void OnGUI()
	{
		base.transform.localPosition = new Vector3(100f, -100f, 0f);
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
