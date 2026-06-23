using System;
using System.Collections.Generic;
using UnityEngine;

public class ManualUpdateManager : MonoBehaviour
{
	public static void Add(ManualUpdateBehaviour behave)
	{
		if (!ManualUpdateManager.instance)
		{
			ManualUpdateManager.instance = Object.FindObjectOfType<ManualUpdateManager>();
			if (!ManualUpdateManager.instance)
			{
				GameObject gameObject = new GameObject("ManualUpdateManager");
				ManualUpdateManager.instance = gameObject.AddComponent<ManualUpdateManager>();
			}
		}
		if (ManualUpdateManager.instance)
		{
			ManualUpdateManager.instance.mUpdateList.Add(behave);
		}
	}

	public static void Remove(ManualUpdateBehaviour behave)
	{
		if (ManualUpdateManager.instance)
		{
			ManualUpdateManager.instance.mUpdateList.Remove(behave);
		}
	}

	private void Awake()
	{
		if (ManualUpdateManager.instance && ManualUpdateManager.instance != this)
		{
			base.enabled = false;
			return;
		}
		ManualUpdateManager.instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.layer = LayerMask.NameToLayer("MUUI");
	}

	private void Update()
	{
		for (int i = this.mUpdateList.Count - 1; i >= 0; i--)
		{
			ManualUpdateBehaviour manualUpdateBehaviour = this.mUpdateList[i];
			if (!manualUpdateBehaviour)
			{
				this.mUpdateList.RemoveAt(i);
			}
			else
			{
				manualUpdateBehaviour.ManualUpdate();
			}
		}
	}

	private static ManualUpdateManager instance;

	private List<ManualUpdateBehaviour> mUpdateList = new List<ManualUpdateBehaviour>();
}
