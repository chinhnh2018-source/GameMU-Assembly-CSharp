using System;

namespace HTMLEngine
{
	public abstract class HtImage
	{
		public abstract int Width { get; }

		public abstract int Height { get; }

		public abstract void Draw(string id, HtRect rect, HtColor color, string linkText, object userData);
	}
}
