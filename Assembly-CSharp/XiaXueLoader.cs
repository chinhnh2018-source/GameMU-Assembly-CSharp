using System;
using UnityEngine;

public class XiaXueLoader : MonoBehaviour
{
	public static XiaXueLoader singleton
	{
		get
		{
			return XiaXueLoader.ms_Singleton;
		}
	}

	private void Awake()
	{
		XiaXueLoader.ms_Singleton = this;
	}

	private void Start()
	{
		GameObject gameObject = Resources.Load("Prefabs/mapeffect/Prefabs/XiaXue") as GameObject;
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
		XiaXueLoader.ms_Singleton = null;
	}

	private static XiaXueLoader ms_Singleton;
}
