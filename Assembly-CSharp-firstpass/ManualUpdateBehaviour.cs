using System;
using UnityEngine;

public abstract class ManualUpdateBehaviour : MonoBehaviour
{
	protected virtual void OnEnable()
	{
		ManualUpdateManager.Add(this);
	}

	protected virtual void OnDisable()
	{
		ManualUpdateManager.Remove(this);
	}

	public abstract void ManualUpdate();
}
