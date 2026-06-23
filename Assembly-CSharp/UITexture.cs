using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Texture"), ExecuteInEditMode]
public class UITexture : UIWidget
{
	public Rect uvRect
	{
		get
		{
			if (this.mShader.name.Contains("Tint Particle"))
			{
				return this.mTiling;
			}
			return this.mRect;
		}
		set
		{
			if (this.mShader.name.Contains("Tint Particle"))
			{
				this.mTiling = value;
				this.mMat.SetVector("_Tiling", new Vector4(this.mTiling.width, this.mTiling.height, 0f, 0f));
			}
			else if (this.mRect != value)
			{
				this.mRect = value;
				this.MarkAsChanged();
			}
		}
	}

	public Shader shader
	{
		get
		{
			if (this.mShader == null)
			{
				Material material = this.material;
				if (material != null)
				{
					this.mShader = material.shader;
				}
				if (this.mShader == null)
				{
					return null;
				}
			}
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				this.mShader = value;
				Material material = this.material;
				if (material != null)
				{
					material.shader = value;
				}
				this.mPMA = -1;
			}
		}
	}

	public bool hasDynamicMaterial
	{
		get
		{
			return this.mDynamicMat != null;
		}
	}

	public override bool keepMaterial
	{
		get
		{
			return true;
		}
	}

	public override Material material
	{
		get
		{
			if (!this.mCreatingMat && this.mMat == null)
			{
				this.mCreatingMat = true;
				if (this.mainTexture != null)
				{
					if (this.mShader == null)
					{
						return null;
					}
					this.mDynamicMat = new Material(this.mShader);
					this.mDynamicMat.hideFlags = 52;
					this.mDynamicMat.mainTexture = this.mainTexture;
					if (this.mShader.name.Contains("Tint Particle"))
					{
						this.mDynamicMat.SetInt("_SrcBlend", 1);
						this.mDynamicMat.SetInt("_DstBlend", 1);
						if (this.uvTexture)
						{
							this.mDynamicMat.SetTexture("_Mask", this.uvTexture);
							this.mDynamicMat.EnableKeyword("_USE_MASK");
						}
						if (this.SpeedX != 0f || this.SpeedY != 0f)
						{
							this.mDynamicMat.EnableKeyword("UV_ANIM");
							this.mDynamicMat.SetVector("_Scrolls", new Vector4(this.SpeedX, this.SpeedY, 0f, 0f));
						}
						this.mDynamicMat.SetVector("_Tiling", new Vector4(this.mTiling.width, this.mTiling.height, 0f, 0f));
					}
					else
					{
						if (this.mDynamicMat.HasProperty("_UVAnimTex") && this.uvTexture != null)
						{
							this.mDynamicMat.SetTexture("_UVAnimTex", this.uvTexture);
						}
						if (this.mDynamicMat.HasProperty("_UVColor"))
						{
							this.mDynamicMat.SetColor("_UVColor", this.UVColor);
						}
						if (this.mDynamicMat.HasProperty("_SpeedX"))
						{
							this.mDynamicMat.SetFloat("_SpeedX", this.SpeedX);
						}
						if (this.mDynamicMat.HasProperty("_SpeedY"))
						{
							this.mDynamicMat.SetFloat("_SpeedY", this.SpeedY);
						}
					}
					base.material = this.mDynamicMat;
					this.mPMA = 0;
				}
				this.mCreatingMat = false;
			}
			return this.mMat;
		}
		set
		{
			if (this.mDynamicMat != value && this.mDynamicMat != null)
			{
				NGUITools.Destroy(this.mDynamicMat);
				this.mDynamicMat = null;
			}
			base.material = value;
			this.mPMA = -1;
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (this.mPMA == -1)
			{
				Material material = this.material;
				this.mPMA = ((!(material != null) || !(material.shader != null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			return (!(this.mTexture != null)) ? base.mainTexture : this.mTexture;
		}
		set
		{
			if (this.mPanel != null && this.mMat != null)
			{
				this.mPanel.RemoveWidget(this);
			}
			if (this.mMat == null)
			{
				this.mDynamicMat = new Material(this.shader);
				this.mDynamicMat.hideFlags = 52;
				this.mMat = this.mDynamicMat;
			}
			this.mPanel = null;
			this.mTex = value;
			this.mTexture = value;
			this.mMat.mainTexture = value;
			if (base.enabled)
			{
				base.CreatePanel();
			}
		}
	}

	public Texture UVTexture
	{
		get
		{
			return this.uvTexture;
		}
		set
		{
			if (this.mPanel != null && this.mMat != null)
			{
				this.mPanel.RemoveWidget(this);
			}
			if (this.mMat == null)
			{
				this.mDynamicMat = new Material(this.shader);
				this.mDynamicMat.hideFlags = 52;
				this.mMat = this.mDynamicMat;
			}
			this.mPanel = null;
			this.uvTexture = value;
			if (this.mMat.HasProperty("_UVAnimTex"))
			{
				this.mMat.SetTexture("_UVAnimTex", value);
			}
			if (base.enabled)
			{
				base.CreatePanel();
			}
		}
	}

	private void OnDestroy()
	{
		NGUITools.Destroy(this.mDynamicMat);
	}

	public override void MakePixelPerfect()
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture != null)
		{
			Vector3 localScale = base.cachedTransform.localScale;
			localScale.x = (float)mainTexture.width * this.uvRect.width;
			localScale.y = (float)mainTexture.height * this.uvRect.height;
			localScale.z = 1f;
			base.cachedTransform.localScale = localScale;
		}
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Color color = base.color;
		color.a *= this.mPanel.alpha;
		Color32 color2 = (!this.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(new Vector2(this.mRect.xMax, this.mRect.yMax));
		uvs.Add(new Vector2(this.mRect.xMax, this.mRect.yMin));
		uvs.Add(new Vector2(this.mRect.xMin, this.mRect.yMin));
		uvs.Add(new Vector2(this.mRect.xMin, this.mRect.yMax));
		cols.Add(color2);
		cols.Add(color2);
		cols.Add(color2);
		cols.Add(color2);
	}

	public Color UVColor
	{
		get
		{
			return this.uvColor;
		}
		set
		{
			if (!this.uvColor.Equals(value) && this.mMat.HasProperty("_UVColor"))
			{
				this.uvColor = value;
				this.mMat.SetColor("_UVColor", value);
			}
		}
	}

	public float SpeedX
	{
		get
		{
			return this.speedX;
		}
		set
		{
			if (this.speedX != value)
			{
				if (this.mMat.shader.name.Contains("Tint Particle"))
				{
					this.speedX = value;
					this.mMat.SetVector("_Scrolls", new Vector4(this.speedX, this.speedY, 0f, 0f));
				}
				else if (this.mMat.HasProperty("_SpeedX"))
				{
					this.speedX = value;
					this.mMat.SetFloat("_SpeedX", value);
				}
			}
		}
	}

	public float SpeedY
	{
		get
		{
			return this.speedY;
		}
		set
		{
			if (this.speedY != value)
			{
				if (this.mMat.shader.name.Contains("Tint Particle"))
				{
					this.speedY = value;
					this.mMat.SetVector("_Scrolls", new Vector4(this.speedX, this.speedY, 0f, 0f));
				}
				else if (this.mMat.HasProperty("_SpeedY"))
				{
					this.speedY = value;
					this.mMat.SetFloat("_SpeedY", value);
				}
			}
		}
	}

	[HideInInspector, SerializeField]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector, SerializeField]
	private Rect mTiling = new Rect(0f, 0f, 1f, 1f);

	[SerializeField, HideInInspector]
	private Shader mShader;

	[SerializeField, HideInInspector]
	private Texture mTexture;

	[SerializeField, HideInInspector]
	private Texture uvTexture;

	[SerializeField, HideInInspector]
	private Color uvColor;

	[SerializeField, HideInInspector]
	private float speedX;

	[SerializeField, HideInInspector]
	private float speedY;

	private Material mDynamicMat;

	private bool mCreatingMat;

	private int mPMA = -1;
}
