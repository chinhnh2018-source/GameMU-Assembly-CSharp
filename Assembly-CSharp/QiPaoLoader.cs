using System;
using UnityEngine;

public class QiPaoLoader : MonoBehaviour
{
	public static QiPaoLoader singleton
	{
		get
		{
			return QiPaoLoader.ms_Singleton;
		}
	}

	private void Awake()
	{
		QiPaoLoader.ms_Singleton = this;
	}

	private void Start()
	{
		GameObject gameObject = Resources.Load("Prefabs/mapeffect/Prefabs/Fish") as GameObject;
		if (null != gameObject)
		{
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			gameObject.transform.parent = Camera.main.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}
	}

	private void Destroy()
	{
		QiPaoLoader.ms_Singleton = null;
	}

	private static QiPaoLoader ms_Singleton;
}
