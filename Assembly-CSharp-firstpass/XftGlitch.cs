using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class XftGlitch : MonoBehaviour
{
	public void Init(Shader glitch, Texture2D mask)
	{
		this.GlitchShader = glitch;
		this.Mask = mask;
	}

	public Material GlitchMaterial
	{
		get
		{
			if (this.m_material == null)
			{
				this.m_material = new Material(this.GlitchShader);
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
		if (!this.GlitchMaterial.shader.isSupported)
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
		if (this.Mask == null)
		{
			return;
		}
		this.GlitchMaterial.SetVector("_displace", this.Offset);
		this.GlitchMaterial.SetTexture("_Mask", this.Mask);
		Graphics.Blit(source, destination, this.GlitchMaterial);
	}

	protected Material m_material;

	public Shader GlitchShader;

	public Vector3 Offset;

	public Texture2D Mask;
}
