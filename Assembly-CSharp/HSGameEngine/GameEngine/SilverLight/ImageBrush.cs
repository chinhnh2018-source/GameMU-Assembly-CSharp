using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class ImageBrush : Brush, IAsyncLoad
	{
		public ImageBrush(BitmapData value = null)
		{
			this._BitmapData = value;
		}

		public BitmapData ImageSource
		{
			get
			{
				return this._BitmapData;
			}
			set
			{
				this._BitmapData = value;
			}
		}

		public int width
		{
			get
			{
				if (this._BitmapData == null || this._BitmapData.TextureData == null)
				{
					return 0;
				}
				return this._BitmapData.TextureData.width;
			}
		}

		public int height
		{
			get
			{
				if (this._BitmapData == null || this._BitmapData.TextureData == null)
				{
					return 0;
				}
				return this._BitmapData.TextureData.height;
			}
		}

		public Object target { get; set; }

		public static implicit operator Texture(ImageBrush imageBrush)
		{
			if (imageBrush != null && imageBrush.ImageSource != null)
			{
				return imageBrush.ImageSource.TextureData;
			}
			return null;
		}

		private BitmapData _BitmapData;
	}
}
