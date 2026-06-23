using System;
using UnityEngine;

public class TextureAnim : MonoBehaviour
{
	private void Start()
	{
		this.modValue = this.AnimTextures.Length;
	}

	private void Update()
	{
		if (this.fps == 0)
		{
			return;
		}
		if (this.curTime >= this.lastTime + 1f / (float)this.fps)
		{
			base.transform.GetComponent<Renderer>().material.SetTexture(this.AnimTextureName, this.AnimTextures[this.index]);
			if (++this.index >= this.AnimTextures.Length)
			{
				this.index = 0;
			}
			this.lastTime = this.curTime;
		}
		else
		{
			this.curTime += Time.deltaTime;
		}
	}

	public Texture[] AnimTextures;

	public int fps = 25;

	private float curTime;

	private float lastTime;

	private int modValue;

	private int index;

	private string AnimTextureName = "_SecondTex";
}
