using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class ImageURL
	{
		public ImageURL(string value = null, bool toGrayBitmap = false, int imageType = 0)
		{
			this._URL = value;
			this._ToGrayBitmap = toGrayBitmap;
			this._ImageType = imageType;
		}

		public string ImageSource
		{
			get
			{
				return this._URL;
			}
			set
			{
				this._URL = value;
			}
		}

		public bool ToGrayBitmap
		{
			get
			{
				return this._ToGrayBitmap;
			}
			set
			{
				this._ToGrayBitmap = value;
			}
		}

		public int ImageType
		{
			get
			{
				return this._ImageType;
			}
			set
			{
				this._ImageType = value;
			}
		}

		private string _URL;

		private bool _ToGrayBitmap;

		private int _ImageType;
	}
}
