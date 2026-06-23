using System;
using UnityEngine;

public class XiaYuLoader : MonoBehaviour
{
	public static XiaYuLoader singleton
	{
		get
		{
			return XiaYuLoader.ms_Singleton;
		}
	}

	private void Awake()
	{
		XiaYuLoader.ms_Singleton = this;
	}

	private void Start()
	{
		GameObject gameObject = Resources.Load("Prefabs/mapeffect/Prefabs/XiaYu") as GameObject;
		if (null != gameObject)
		{
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			gameObject.transform.parent = Camera.main.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 5f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 30f));
		}
	}

	private void Destroy()
	{
		XiaYuLoader.ms_Singleton = null;
	}

	private static XiaYuLoader ms_Singleton;
}
