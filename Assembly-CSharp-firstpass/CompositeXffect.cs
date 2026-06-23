using System;
using System.Collections;
using UnityEngine;

public class CompositeXffect : MonoBehaviour
{
	private void Awake()
	{
		this.Initialize();
	}

	public void Initialize()
	{
		if (this.XffectList.Count > 0)
		{
			return;
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			XffectComponent component = transform.GetComponent<XffectComponent>();
			if (!(component == null))
			{
				component.Initialize();
				this.XffectList.Add(component);
			}
		}
	}

	public void ActiveNoInterrupt()
	{
		if (XffectComponent.IsActive(base.gameObject))
		{
			return;
		}
		if (!XffectComponent.IsActive(base.gameObject))
		{
			XffectComponent.SetActive(base.gameObject, true);
		}
		foreach (object obj in this.XffectList)
		{
			XffectComponent xffectComponent = (XffectComponent)obj;
			xffectComponent.Active();
		}
	}

	public void Active()
	{
		if (!XffectComponent.IsActive(base.gameObject))
		{
			XffectComponent.SetActive(base.gameObject, true);
		}
		foreach (object obj in this.XffectList)
		{
			XffectComponent xffectComponent = (XffectComponent)obj;
			xffectComponent.Active();
		}
	}

	public void DeActive()
	{
		foreach (object obj in this.XffectList)
		{
			XffectComponent xffectComponent = (XffectComponent)obj;
			xffectComponent.DeActive();
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < this.XffectList.Count; i++)
		{
			XffectComponent xffectComponent = (XffectComponent)this.XffectList[i];
			if (XffectComponent.IsActive(xffectComponent.gameObject))
			{
				return;
			}
		}
		XffectComponent.SetActive(base.gameObject, false);
	}

	private ArrayList XffectList = new ArrayList();
}
