using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
	public void AddMaterial(Material mat)
	{
		this.matList.Add(mat);
	}

	private void Start()
	{
		if (base.transform.GetComponent<Renderer>() != null)
		{
			this.arySize = this.colorSet.Length;
			this.fromIdx = 0;
			this.toIdx = 1;
		}
	}

	private void Update()
	{
		if (this.matList.Count > 0 && this.arySize > 1)
		{
			if (this.delayCount-- > 0)
			{
				return;
			}
			for (int i = 0; i < this.matList.Count; i++)
			{
				this.newColor = Color.Lerp(this.colorSet[this.fromIdx], this.colorSet[this.toIdx], this.lerpT);
				this.matList[i].SetColor("_Color", this.newColor);
				this.lerpT += this.lerpSwitch * Time.deltaTime;
				if (this.lerpT >= this.lerpSwitch)
				{
					this.lerpT = 0f;
					this.UpdateIdx();
				}
			}
			this.delayCount = 6;
		}
	}

	private void UpdateIdx()
	{
		this.toIdx++;
		if (this.toIdx == this.arySize)
		{
			this.toIdx = 0;
		}
		this.fromIdx = this.toIdx - 1;
		if (this.fromIdx < 0)
		{
			this.fromIdx = this.arySize - 1;
		}
	}

	public Color[] colorSet = new Color[]
	{
		new Color(1f, 0.745098054f, 0.431372553f, 1f),
		new Color(0.392156869f, 0.627451f, 1f, 1f),
		new Color(1f, 0.745098054f, 0.431372553f, 1f)
	};

	private int arySize;

	private int fromIdx;

	private int toIdx;

	private List<Material> matList = new List<Material>();

	private Color newColor;

	private float lerpT;

	public float lerpSwitch = 1f;

	private int delayCount = 6;
}
