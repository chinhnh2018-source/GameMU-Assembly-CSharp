using System;
using System.Runtime.InteropServices;
using HSGameEngine.Drawing;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class BitmapData : IAsyncLoad
	{
		public BitmapData(Texture texture)
		{
			this.TextureData = texture;
		}

		public BitmapData(double _width = 0.0, double _height = 0.0, bool _transparent = true, uint fillColor = 4294967295U)
		{
			this.transparent = _transparent;
		}

		public Texture TextureData { get; set; }

		public Object target
		{
			set
			{
				this.TextureData = (value as Texture);
			}
		}

		public double width
		{
			get
			{
				return (double)((!(this.TextureData == null)) ? this.TextureData.width : 0);
			}
		}

		public double height
		{
			get
			{
				return (double)((!(this.TextureData == null)) ? this.TextureData.height : 0);
			}
		}

		public uint fillColor { get; set; }

		public bool transparent { get; set; }

		public RectangleSL rect { get; set; }

		public RectangleSL rect1 { get; set; }

		public void copyPixels(BitmapData sourceBitmapData, Rectangle sourceRect, Point destPoint, int unknown = 0)
		{
			this.TextureData = sourceBitmapData.TextureData;
		}

		public void copyPixels(BitmapData sourceBitmapData, Rectangle sourceRect, Point destPoint, BitmapData alphaBitmapData = null, [Optional] Point alphaPoint, bool mergeAlpha = false)
		{
			this.TextureData = sourceBitmapData.TextureData;
		}
	}
}
