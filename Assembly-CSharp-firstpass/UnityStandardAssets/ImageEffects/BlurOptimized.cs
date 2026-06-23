using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[RequireComponent(typeof(Camera)), AddComponentMenu("Image Effects/Blur/Blur (Optimized)"), ExecuteInEditMode]
	public class BlurOptimized : PostEffectsBase
	{
		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.blurMaterial = base.CheckShaderAndCreateMaterial(this.blurShader, this.blurMaterial);
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		public void OnDisable()
		{
			if (this.blurMaterial)
			{
				Object.DestroyImmediate(this.blurMaterial);
			}
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			float num = 1f / (1f * (float)(1 << this.downsample));
			this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num, -this.blurSize * num, 0f, 0f));
			source.filterMode = 1;
			int num2 = source.width >> this.downsample;
			int num3 = source.height >> this.downsample;
			RenderTexture renderTexture = RenderTexture.GetTemporary(num2, num3, 0, source.format);
			renderTexture.filterMode = 1;
			Graphics.Blit(source, renderTexture, this.blurMaterial, 0);
			int num4 = (this.blurType != BlurOptimized.BlurType.StandardGauss) ? 2 : 0;
			for (int i = 0; i < this.blurIterations; i++)
			{
				float num5 = (float)i * 1f;
				this.blurMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num + num5, -this.blurSize * num - num5, 0f, 0f));
				RenderTexture temporary = RenderTexture.GetTemporary(num2, num3, 0, source.format);
				temporary.filterMode = 1;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 1 + num4);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(num2, num3, 0, source.format);
				temporary.filterMode = 1;
				Graphics.Blit(renderTexture, temporary, this.blurMaterial, 2 + num4);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			Graphics.Blit(renderTexture, destination);
			RenderTexture.ReleaseTemporary(renderTexture);
		}

		[Range(0f, 2f)]
		public int downsample = 1;

		[Range(0f, 10f)]
		public float blurSize = 3f;

		[Range(1f, 4f)]
		public int blurIterations = 2;

		public BlurOptimized.BlurType blurType;

		public Shader blurShader;

		private Material blurMaterial;

		public enum BlurType
		{
			StandardGauss,
			SgxGauss
		}
	}
}
