using System;
using UnityEngine;

public class ModelShowHide : MonoBehaviour
{
	private void Start()
	{
		this.pos = this.goShowHide.transform.position;
		if (this.goShowHide)
		{
			this.goShowHide.transform.localPosition = new Vector3(1000000f, 0f, 0f);
		}
	}

	private void OnDestroy()
	{
		if (this.goShowHide)
		{
			this.goShowHide.transform.position = this.pos;
		}
	}

	public GameObject goShowHide;

	private Vector3 pos = Vector3.zero;
}
