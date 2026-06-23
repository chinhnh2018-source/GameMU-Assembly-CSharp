using System;

namespace HTMLEngine
{
	public abstract class HtDevice
	{
		public abstract HtFont LoadFont(string face, int size, bool bold, bool italic);

		public abstract HtImage LoadImage(string src, int fps);

		public abstract void FillRect(HtRect rect, HtColor color, object userData);

		public abstract void OnRelease();
	}
}
