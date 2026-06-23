using System;
using HTMLEngine.Core;

namespace HTMLEngine
{
	public abstract class HtFont
	{
		protected HtFont(string face, int size, bool bold, bool italic)
		{
			this.Face = face;
			this.Size = size;
			this.Bold = bold;
			this.Italic = italic;
		}

		public string Face { get; private set; }

		public int Size { get; private set; }

		public bool Bold { get; private set; }

		public bool Italic { get; private set; }

		public abstract int LineSpacing { get; }

		public abstract int WhiteSize { get; }

		public abstract HtSize Measure(string text);

		public abstract void Draw(string id, HtRect rect, HtColor color, string text, bool isEffect, DrawTextEffect effect, HtColor effectColor, int effectAmount, string linkText, object userData);

		public override string ToString()
		{
			return string.Format("{0}{1}{2}{3}", new object[]
			{
				this.Face,
				this.Size,
				(!this.Bold) ? string.Empty : "b",
				(!this.Italic) ? string.Empty : "i"
			});
		}
	}
}
