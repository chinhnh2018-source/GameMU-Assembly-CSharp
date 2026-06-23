using System;
using System.Collections;
using UnityEngine;

public class HunLiFlowerLoader : MonoBehaviour
{
	public static HunLiFlowerLoader singleton
	{
		get
		{
			return HunLiFlowerLoader.ms_Singleton;
		}
	}

	private void Awake()
	{
		HunLiFlowerLoader.ms_Singleton = this;
	}

	private void Start()
	{
		GameObject gameObject = Resources.Load("Prefabs/mapeffect/Prefabs/HunLi_flower_camera") as GameObject;
		if (null != gameObject && null != Camera.main)
		{
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			gameObject.transform.parent = Camera.main.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 5f);
			gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 30f));
			this.flowerObj = gameObject;
			base.StartCoroutine("DelayDestroy", gameObject);
		}
	}

	public IEnumerator DelayDestroy(GameObject target)
	{
		yield return new WaitForSeconds(15f);
		Object.Destroy(target);
		Object.Destroy(base.gameObject);
		yield break;
	}

	private void Destroy()
	{
		base.StopCoroutine("DelayDestroy");
		if (this.flowerObj != null)
		{
			Object.Destroy(this.flowerObj);
		}
		Object.Destroy(base.gameObject);
		HunLiFlowerLoader.ms_Singleton = null;
	}

	private static HunLiFlowerLoader ms_Singleton;

	private GameObject flowerObj;
}
