using System;
using UnityEngine;

public class HuoXingLoader : MonoBehaviour
{
	public static HuoXingLoader singleton
	{
		get
		{
			return HuoXingLoader.ms_Singleton;
		}
	}

	private void Awake()
	{
		HuoXingLoader.ms_Singleton = this;
	}

	private void Start()
	{
		GameObject gameObject = Resources.Load("Prefabs/mapeffect/Prefabs/HuoXing") as GameObject;
		if (null != gameObject && null != Camera.main)
		{
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			gameObject.transform.parent = Camera.main.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 5f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 30f));
		}
	}

	private void Destroy()
	{
		HuoXingLoader.ms_Singleton = null;
	}

	private static HuoXingLoader ms_Singleton;
}
