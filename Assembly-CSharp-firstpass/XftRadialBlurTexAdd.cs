using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class XftRadialBlurTexAdd : MonoBehaviour
{
	public void Init(Shader shader)
	{
		this.RadialBlurShader = shader;
		if (this.m_material == null)
		{
			this.m_material = new Material(shader);
			this.m_material.hideFlags = 61;
		}
	}

	public Material MyMaterial
	{
		get
		{
			if (this.m_material == null)
			{
				this.m_material = new Material(this.RadialBlurShader);
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
		this.MyMaterial.SetTexture("_Mask", this.Mask);
		this.MyMaterial.SetFloat("_SampleDist", this.SampleDist);
		this.MyMaterial.SetFloat("_SampleStrength", this.SampleStrength);
		Graphics.Blit(source, destination, this.m_material);
	}

	protected Material m_material;

	public Shader RadialBlurShader;

	public float SampleDist;

	public float SampleStrength;

	public Texture2D Mask;
}
