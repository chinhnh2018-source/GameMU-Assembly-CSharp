using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShowNetImage : URLImage
{
	public Sprite mask { get; set; }

	public void CleanUpChildWindows()
	{
	}

	public void ShowDownloadedErrImage(object obj)
	{
		this.ImageDownloadedErr = null;
		this.FaildURL = this.ImageURL;
		this.ImageURL = Global.GetGoodsIconString(-1);
		base.ForceShow();
	}

	public void DestroyImmediateTexture()
	{
		if (null != this && null != this.Texture && this.Texture.mainTexture != null)
		{
			Object.Destroy(this.Texture.mainTexture);
			Object.DestroyImmediate(this.Texture.mainTexture, true);
			this.Texture.mainTexture = null;
			Resources.UnloadUnusedAssets();
		}
	}

	public string FaildURL;
}
