using System;
using UnityEngine;

public class DontUnloadObject : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (this.IntantDisable)
		{
			base.gameObject.SetActive(false);
		}
	}

	public bool IntantDisable;
}
