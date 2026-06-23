using System;
using UnityEngine;

public class FxGrabPass : MonoBehaviour
{
	private FxGrabPassObject GrabPassObj
	{
		get
		{
			if (!this.grabPassObj)
			{
				Camera camera = (!this.cam) ? Camera.main : this.cam;
				if (camera)
				{
					this.grabPassObj = camera.GetComponent<FxGrabPassObject>();
					if (!this.grabPassObj)
					{
						this.grabPassObj = camera.gameObject.AddComponent<FxGrabPassObject>();
					}
				}
			}
			return this.grabPassObj;
		}
	}

	private void Awake()
	{
		this.mRenderer = base.GetComponent<Renderer>();
	}

	private void OnEnable()
	{
		this.OnBecameVisible();
	}

	private void OnDisable()
	{
		this.OnBecameInvisible();
	}

	private void OnBecameVisible()
	{
		if (this.mVisible)
		{
			return;
		}
		this.mVisible = true;
		this.GrabPassObj.SetGrabPassEnable();
	}

	private void OnBecameInvisible()
	{
		if (!this.mVisible)
		{
			return;
		}
		this.mVisible = false;
		this.GrabPassObj.SetGrabPassDisable();
	}

	public Camera cam;

	private bool mVisible;

	private Renderer mRenderer;

	private FxGrabPassObject grabPassObj;
}
