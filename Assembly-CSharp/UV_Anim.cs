using System;
using UnityEngine;

public class UV_Anim : NcEffectAniBehaviour
{
	private void Start()
	{
		this.enabled = new bool[this.m_name.Length];
		this.m_OriginalScale = new Vector3[this.m_name.Length];
		this.m_OriginalTiling = new Vector2[this.m_name.Length];
		this.m_EndOffset = new Vector2[this.m_name.Length];
		this.m_RepeatOffset = new Vector2[this.m_name.Length];
		this.m_Renderer = base.GetComponent<Renderer>();
		for (int i = 0; i < this.m_name.Length; i++)
		{
			if (string.Empty != this.m_name[i])
			{
				if (this.m_Renderer == null || this.m_Renderer.sharedMaterial == null || this.m_Renderer.sharedMaterial.GetTexture(this.m_name[i]) == null)
				{
					this.enabled[i] = false;
				}
				else
				{
					base.GetComponent<Renderer>().material.SetTextureScale(this.m_name[i], new Vector2(this.m_fTilingX[i], this.m_fTilingY[i]));
					float num = this.m_fOffsetX[i] + this.m_fTilingX[i];
					this.m_RepeatOffset[i].x = num - (float)((int)num);
					if (this.m_RepeatOffset[i].x < 0f)
					{
						Vector2[] repeatOffset = this.m_RepeatOffset;
						int num2 = i;
						repeatOffset[num2].x = repeatOffset[num2].x + 1f;
					}
					num = this.m_fOffsetY[i] + this.m_fTilingY[i];
					this.m_RepeatOffset[i].y = num - (float)((int)num);
					if (this.m_RepeatOffset[i].y < 0f)
					{
						Vector2[] repeatOffset2 = this.m_RepeatOffset;
						int num3 = i;
						repeatOffset2[num3].y = repeatOffset2[num3].y + 1f;
					}
					this.m_EndOffset[i].x = 1f - (this.m_fTilingX[i] - (float)((int)this.m_fTilingX[i]) + (float)((this.m_fTilingX[i] - (float)((int)this.m_fTilingX[i]) >= 0f) ? 0 : 1));
					this.m_EndOffset[i].y = 1f - (this.m_fTilingY[i] - (float)((int)this.m_fTilingY[i]) + (float)((this.m_fTilingY[i] - (float)((int)this.m_fTilingY[i]) >= 0f) ? 0 : 1));
					base.InitAnimationTimer();
				}
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.m_name.Length; i++)
		{
			if (string.Empty != this.m_name[i])
			{
				if (this.m_Renderer == null || this.m_Renderer.sharedMaterial == null || this.m_Renderer.sharedMaterial.GetTexture(this.m_name[i]) == null)
				{
					return;
				}
				if (this.m_bFixedTileSize[i])
				{
					if (this.m_fScrollSpeedX[i] != 0f && this.m_OriginalScale[i].x != 0f)
					{
						this.m_fTilingX[i] = this.m_OriginalTiling[i].x * (base.transform.lossyScale.x / this.m_OriginalScale[i].x);
					}
					if (this.m_fScrollSpeedY[i] != 0f && this.m_OriginalScale[i].y != 0f)
					{
						this.m_fTilingY[i] = this.m_OriginalTiling[i].y * (base.transform.lossyScale.y / this.m_OriginalScale[i].y);
					}
					base.GetComponent<Renderer>().material.SetTextureScale(this.m_name[i], new Vector2(this.m_fTilingX[i], this.m_fTilingY[i]));
				}
				this.m_fOffsetX[i] += this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedX[i];
				this.m_fOffsetY[i] += this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedY[i];
				bool flag = false;
				if (!this.m_bRepeat[i])
				{
					Vector2[] repeatOffset = this.m_RepeatOffset;
					int num = i;
					repeatOffset[num].x = repeatOffset[num].x + this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedX[i];
					if (this.m_RepeatOffset[i].x < 0f || 1f < this.m_RepeatOffset[i].x)
					{
						this.m_fOffsetX[i] = this.m_EndOffset[i].x;
						this.enabled[i] = false;
						flag = true;
					}
					Vector2[] repeatOffset2 = this.m_RepeatOffset;
					int num2 = i;
					repeatOffset2[num2].y = repeatOffset2[num2].y + this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedY[i];
					if (this.m_RepeatOffset[i].y < 0f || 1f < this.m_RepeatOffset[i].y)
					{
						this.m_fOffsetY[i] = this.m_EndOffset[i].y;
						this.enabled[i] = false;
						flag = true;
					}
				}
				this.m_Renderer.material.SetTextureOffset(this.m_name[i], new Vector2(this.m_fOffsetX[i], this.m_fOffsetY[i]));
				if (flag)
				{
					base.OnEndAnimation();
					if (this.m_bAutoDestruct[i])
					{
						Object.DestroyObject(base.gameObject);
					}
				}
			}
		}
	}

	public string[] m_name = new string[]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public float[] m_fScrollSpeedX = new float[]
	{
		1f,
		1f,
		1f,
		1f,
		1f
	};

	public float[] m_fScrollSpeedY = new float[5];

	public float[] m_fTilingX = new float[]
	{
		1f,
		1f,
		1f,
		1f,
		1f
	};

	public float[] m_fTilingY = new float[]
	{
		1f,
		1f,
		1f,
		1f,
		1f
	};

	public float[] m_fOffsetX = new float[5];

	public float[] m_fOffsetY = new float[5];

	public bool[] m_bFixedTileSize = new bool[5];

	public bool[] m_bRepeat = new bool[]
	{
		true,
		true,
		true,
		true,
		true
	};

	public bool[] m_bAutoDestruct = new bool[5];

	protected Vector3[] m_OriginalScale;

	protected Vector2[] m_OriginalTiling;

	protected Vector2[] m_EndOffset;

	protected Vector2[] m_RepeatOffset;

	protected Renderer m_Renderer;

	protected bool[] enabled;
}
