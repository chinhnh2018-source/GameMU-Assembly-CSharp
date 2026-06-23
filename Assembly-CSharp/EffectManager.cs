using System;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	private void Start()
	{
		if (null != base.GetComponent<Collider>() && this.TriggerType >= 0)
		{
			base.GetComponent<Collider>().isTrigger = true;
		}
		TrailRenderer[] componentsInChildren = base.GetComponentsInChildren<TrailRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].autodestruct = false;
		}
	}

	private void Update()
	{
		this._TimeElapse += Time.deltaTime;
		if (this.TimeTotal > 0f && this._TimeElapse > this.TimeTotal)
		{
			this.StopEffect();
			return;
		}
	}

	public bool DestoryGameObject()
	{
		Object.DestroyObject(base.gameObject);
		return true;
	}

	public bool StopEffect()
	{
		this.DestoryGameObject();
		return true;
	}

	public void OnTriggerEnter(Collider other)
	{
		other.SendMessageUpwards("OnEffectTrigger", new TriggerParams
		{
			OwnerName = this.OwnerName,
			TriggerType = this.TriggerType
		}, 1);
	}

	public void OnDestroy()
	{
		if (this.OnEffectDestroy != null)
		{
			this.OnEffectDestroy.Invoke();
		}
	}

	public float TimeTotal;

	public string OwnerName;

	public int TriggerType = -1;

	private float _TimeElapse;

	public Action OnEffectDestroy;
}
