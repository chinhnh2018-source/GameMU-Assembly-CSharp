using System;
using UnityEngine;

public class DelayActiveObj : MonoBehaviour
{
	private void Start()
	{
		this.toBeActiveGameObj.SetActive(false);
	}

	private void Update()
	{
		this.elapsedTime += Time.deltaTime;
		if (this.elapsedTime >= this.delayTime && !this.toBeActiveGameObj.activeInHierarchy)
		{
			this.toBeActiveGameObj.SetActive(true);
			Object.Destroy(this);
		}
	}

	public float delayTime;

	private float elapsedTime;

	public GameObject toBeActiveGameObj;
}
