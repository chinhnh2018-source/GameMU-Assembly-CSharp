using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class XftRadialBlur : MonoBehaviour
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
		this.MyMaterial.SetFloat("_SampleStrength", this.SampleStrength);
		Vector4 zero = Vector4.zero;
		zero.x = this.Center.x / (float)Screen.width;
		zero.y = this.Center.y / (float)Screen.height;
		this.MyMaterial.SetVector("_Center", zero);
		Graphics.Blit(source, destination, this.m_material);
	}

	protected Material m_material;

	public float SampleDist = 1f;

	public float SampleStrength = 1f;

	public Vector3 Center = new Vector3(0.5f, 0.5f, 0f);

	public Shader RadialBlurShader;
}
