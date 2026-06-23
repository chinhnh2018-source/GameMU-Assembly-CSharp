using System;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
	private void Start()
	{
		Texture texture = base.transform.GetComponent<Renderer>().material.GetTexture("_MainTex");
		if (null == texture)
		{
			return;
		}
		if (this.rowCount == 0 || this.colCount == 0)
		{
			return;
		}
		float num = (float)texture.height * 1f / (float)this.rowCount;
		float num2 = (float)texture.width * 1f / (float)this.colCount;
		if (num == 0f)
		{
			return;
		}
		float num3 = num2 / num;
		if (num3 != 0f)
		{
			if (this.Is_XZ_Plane)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * this.manualScale * this.manualScaleX, base.transform.localScale.y * this.manualScaleY, base.transform.localScale.z / num3 * this.manualScale * this.manualScaleZ);
			}
			else
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * this.manualScale * this.manualScaleX, base.transform.localScale.y / num3 * this.manualScale * this.manualScaleY, base.transform.localScale.z * this.manualScaleZ);
			}
		}
		base.transform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f / (float)this.colCount, 1f / (float)this.rowCount);
		base.transform.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, 1f - 1f / (float)this.rowCount));
		this.curRow = this.rowCount - 1;
	}

	private void Update()
	{
		if (this.isEnd || this.fps == 0)
		{
			return;
		}
		if (this.curTime >= this.lastTime + 1f / (float)this.fps)
		{
			this.curCol++;
			if (this.curCol >= this.colCount)
			{
				this.curCol = 0;
				this.curRow--;
				if (this.curRow < 0)
				{
					if (!this.isLoop)
					{
						this.isEnd = true;
						return;
					}
					this.curRow = this.rowCount - 1;
				}
			}
			if (this.colCount != 0 && this.rowCount != 0)
			{
				base.transform.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)this.curCol * 1f / (float)this.colCount, (float)this.curRow * 1f / (float)this.rowCount));
			}
			this.lastTime = this.curTime;
		}
		else
		{
			this.curTime += Time.deltaTime;
		}
	}

	public int rowCount = 1;

	public int colCount = 1;

	public int fps = 25;

	public float manualScale = 1f;

	public float manualScaleX = 1f;

	public float manualScaleY = 1f;

	public float manualScaleZ = 1f;

	public bool isLoop = true;

	public bool Is_XZ_Plane = true;

	public string TexName = "_MainTex";

	private float curTime;

	private float lastTime;

	private int curRow;

	private int curCol;

	private bool isEnd;
}
