using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class FontFamilySL
	{
		public FontFamilySL(string value)
		{
			this._fontName = value;
		}

		public string FontName
		{
			get
			{
				return this._fontName;
			}
		}

		private string _fontName = string.Empty;
	}
}
