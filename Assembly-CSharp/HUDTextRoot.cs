using System;
using UnityEngine;

public class HUDTextRoot : MonoBehaviour
{
	private void Awake()
	{
		HUDTextRoot.go = base.gameObject;
	}

	public static GameObject go;
}
