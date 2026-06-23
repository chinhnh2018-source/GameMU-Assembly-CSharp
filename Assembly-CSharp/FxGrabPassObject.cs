using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FxGrabPassObject : MonoBehaviour
{
	private void Awake()
	{
	}

	public void SetGrabPassEnable()
	{
		if (!this.mRenderer && this.mEnableGrabPass)
		{
			GameObject gameObject = new GameObject("GrabPassObject");
			gameObject.AddComponent<MeshFilter>().mesh = new Mesh();
			gameObject.layer = LayerMask.NameToLayer("Water");
			this.mRenderer = gameObject.AddComponent<MeshRenderer>();
			this.mRenderer.material = new Material(Shader.Find("Hidden/GrabPass"));
			this.mRenderer.receiveShadows = false;
			this.mRenderer.shadowCastingMode = 0;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
			gameObject.AddComponent<DontUnloadObject>();
		}
		this.distortionRefCount++;
		if (this.mRenderer)
		{
			this.mRenderer.enabled = this.mEnableGrabPass;
		}
	}

	public void SetGrabPassDisable()
	{
		this.distortionRefCount--;
		if (this.distortionRefCount <= 0 && this.mRenderer)
		{
			this.mRenderer.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (this.mRenderer)
		{
			Object.Destroy(this.mRenderer.gameObject);
		}
	}

	private int distortionRefCount;

	private Renderer mRenderer;

	private bool mEnableGrabPass = true;
}
