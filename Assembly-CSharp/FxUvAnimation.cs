using System;
using UnityEngine;

public class FxUvAnimation : ManualUpdateBehaviour
{
	private void Awake()
	{
		if (!string.IsNullOrEmpty(this.texName))
		{
			this.mUseMainTex = false;
		}
		Renderer component = base.GetComponent<Renderer>();
		if (component == null || component.sharedMaterial == null)
		{
			base.enabled = false;
		}
		else
		{
			this.mMaterial = ((this.materialIndex < component.sharedMaterials.Length) ? component.materials[this.materialIndex] : component.materials[0]);
			if (!this.mUseMainTex)
			{
				this.mMaterial.SetTextureScale(this.texName, new Vector2(this.tilingX, this.tilingY));
			}
			else
			{
				this.mMaterial.mainTextureScale = new Vector2(this.tilingX, this.tilingY);
			}
		}
	}

	private void OnEnable()
	{
		base.OnEnable();
		this.mOffset = new Vector2(this.offsetX, this.offsetY);
	}

	public override void ManualUpdate()
	{
		if (!this.mMaterial)
		{
			return;
		}
		this.mOffset.x = this.mOffset.x + Time.deltaTime * this.scrollSpeedX;
		this.mOffset.y = this.mOffset.y + Time.deltaTime * this.scrollSpeedY;
		if (!this.mUseMainTex)
		{
			this.mMaterial.SetTextureOffset(this.texName, this.mOffset);
		}
		else
		{
			this.mMaterial.mainTextureOffset = this.mOffset;
		}
	}

	public float scrollSpeedX = 1f;

	public float scrollSpeedY;

	public float tilingX = 1f;

	public float tilingY = 1f;

	public float offsetX;

	public float offsetY;

	public string texName = string.Empty;

	public int materialIndex;

	private bool mUseMainTex = true;

	private Vector2 mOffset;

	private Material mMaterial;
}
