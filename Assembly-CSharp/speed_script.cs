using System;
using UnityEngine;

public class speed_script : MonoBehaviour
{
	private void Start()
	{
		if (base.GetComponent<Animation>() != null && null != base.GetComponent<Animation>().GetClip(this.clip01))
		{
			base.GetComponent<Animation>()[this.clip01].speed = this.clip01_speed;
		}
	}

	public string clip01 = "Null";

	public float clip01_speed = 10f;
}
