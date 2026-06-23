using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class Bitmap : MonoBehaviour, IAsyncLoad
{
	public BitmapData bitmapData
	{
		set
		{
			if (value != null && this.uiTexture != null)
			{
				this.uiTexture.mainTexture = value.TextureData;
				if (this.uiTexture.mainTexture != null)
				{
					base.gameObject.transform.localScale = new Vector3((float)this.uiTexture.mainTexture.width, (float)this.uiTexture.mainTexture.height, 0f);
				}
			}
			else if (this.uiTexture != null)
			{
				this.uiTexture.mainTexture = null;
			}
		}
	}

	protected virtual void Awake()
	{
		this.uiTexture = base.gameObject.GetComponent<UITexture>();
	}

	public Object target
	{
		set
		{
			this.uiTexture.mainTexture = (value as Texture);
		}
	}

	public Vector2 Size
	{
		get
		{
			if (this.uiTexture != null)
			{
				return new Vector2((float)this.uiTexture.mainTexture.width, (float)this.uiTexture.mainTexture.height);
			}
			return Vector2.zero;
		}
	}

	public UITexture uiTexture;
}
