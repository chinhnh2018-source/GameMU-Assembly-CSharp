using System;
using UnityEngine;

public class XftVolumeFogObject : MonoBehaviour
{
	private void Awake()
	{
		this.FogObject = base.gameObject.GetComponent<MeshRenderer>();
		if (this.FogObject == null)
		{
			Debug.LogError("Volume Fog Object must have a MeshRenderer Component!");
		}
	}

	private void OnEnable()
	{
		if (Camera.main.depthTextureMode == null)
		{
			Camera.main.depthTextureMode = 1;
		}
	}

	private void Start()
	{
		this.FogObject.material = this.VolumFogMaterial;
		this.FogObject.material.SetColor("_FogColor", this.FogColor);
		this.Radius = (base.transform.localScale.x + base.transform.localScale.y + base.transform.localScale.z) / 6f;
		this.FogObject.material.SetVector("FogParam", new Vector4(base.transform.position.x, base.transform.position.y, base.transform.position.z, this.Radius));
	}

	protected MeshRenderer FogObject;

	protected float Radius = 10f;

	public Material VolumFogMaterial;

	public Color FogColor = Color.white;
}
