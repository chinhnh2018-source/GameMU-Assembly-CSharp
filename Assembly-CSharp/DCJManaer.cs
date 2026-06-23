using System;
using UnityEngine;

public class DCJManaer : MonoBehaviour
{
	private void ModifyEffectManager(GameObject go)
	{
		if (null == this.EffectMgr)
		{
			return;
		}
		EffectManager[] componentsInChildren = go.GetComponentsInChildren<EffectManager>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OwnerName = this.EffectMgr.OwnerName;
				componentsInChildren[i].TriggerType = this.EffectMgr.TriggerType;
			}
		}
	}

	private void AddEffectRoot(GameObject go)
	{
		go.AddComponent<EffectRoot>();
	}

	private void Start()
	{
		this.EffectMgr = base.GetComponent<EffectManager>();
		if (null != this.JianTouPrefab && null != this.EffectMgr)
		{
			GameObject gameObject = null;
			if (!string.IsNullOrEmpty(this.EffectMgr.OwnerName))
			{
				gameObject = GameObject.Find(string.Format("/{0}", this.EffectMgr.OwnerName));
			}
			GameObject gameObject2 = SpawnManager.Instantiate(this.JianTouPrefab) as GameObject;
			if (null != gameObject)
			{
				gameObject2.transform.localPosition = gameObject.transform.localPosition;
				gameObject2.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.eulerAngles.x, gameObject.transform.localRotation.eulerAngles.y - 15f, gameObject.transform.localRotation.eulerAngles.z);
			}
			else
			{
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject2.transform.localRotation = Quaternion.Euler(0f, -15f, 0f);
			}
			this.ModifyEffectManager(gameObject2);
			this.AddEffectRoot(gameObject2);
			gameObject2 = (SpawnManager.Instantiate(this.JianTouPrefab) as GameObject);
			if (null != gameObject)
			{
				gameObject2.transform.localPosition = gameObject.transform.localPosition;
				gameObject2.transform.localRotation = gameObject.transform.localRotation;
			}
			else
			{
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject2.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			this.ModifyEffectManager(gameObject2);
			this.AddEffectRoot(gameObject2);
			gameObject2 = (SpawnManager.Instantiate(this.JianTouPrefab) as GameObject);
			if (null != gameObject)
			{
				gameObject2.transform.localPosition = gameObject.transform.localPosition;
				gameObject2.transform.localRotation = gameObject.transform.localRotation;
				gameObject2.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.eulerAngles.x, gameObject.transform.localRotation.eulerAngles.y + 15f, gameObject.transform.localRotation.eulerAngles.z);
			}
			else
			{
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject2.transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
			}
			this.ModifyEffectManager(gameObject2);
			this.AddEffectRoot(gameObject2);
		}
	}

	public GameObject JianTouPrefab;

	private EffectManager EffectMgr;
}
