using System;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class XftColorInverse : MonoBehaviour
{
	public void Init(Shader shader)
	{
		this.ColorInverseShader = shader;
		if (this.m_material == null)
		{
			this.m_material = new Material(this.ColorInverseShader);
			this.m_material.hideFlags = 61;
		}
	}

	public Material MyMaterial
	{
		get
		{
			if (this.m_material == null)
			{
				this.m_material = new Material(this.ColorInverseShader);
				this.m_material.hideFlags = 61;
			}
			return this.m_material;
		}
	}

	public bool CheckSupport()
	{
		bool result = true;
		if (!SystemInfo.supportsImageEffects)
		{
			result = false;
		}
		if (!this.MyMaterial.shader.isSupported)
		{
			result = false;
		}
		return result;
	}

	private void Awake()
	{
		base.enabled = false;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.m_material == null)
		{
			return;
		}
		this.MyMaterial.SetFloat("_Strength", this.Strength);
		Graphics.Blit(source, destination, this.m_material);
	}

	protected Material m_material;

	public float Strength;

	public Shader ColorInverseShader;
}
