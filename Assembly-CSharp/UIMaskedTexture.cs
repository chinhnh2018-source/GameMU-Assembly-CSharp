using System;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UIMaskedTexture : MonoBehaviour
{
	public UITexture TexComponent
	{
		get
		{
			if (!this.mTex)
			{
				this.mTex = base.GetComponent<UITexture>();
			}
			return this.mTex;
		}
	}

	public Texture2D MaskTexture
	{
		get
		{
			return this.mMaskTexture;
		}
		set
		{
			if (this.mMaskTexture == value)
			{
				return;
			}
			this.mMaskTexture = value;
			if (this.TexComponent)
			{
				this.TexComponent.material.SetTexture("_MaskTex", this.mMaskTexture);
				this.TexComponent.MarkAsChanged();
			}
		}
	}

	public Vector4 MainTextUV
	{
		get
		{
			return this.mMainTextUV;
		}
		set
		{
			this.mMainTextUV = value;
			this.TexComponent.material.SetVector("_MainTexUV", this.mMainTextUV);
			this.TexComponent.panel.Refresh();
		}
	}

	private UITexture mTex;

	private Texture2D mMaskTexture;

	private Vector4 mMainTextUV = new Vector4(0f, 0f, 1f, 1f);
}
