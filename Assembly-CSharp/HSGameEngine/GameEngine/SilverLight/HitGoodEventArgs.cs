using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class HitGoodEventArgs : BaseEventArgs
	{
		public int pageIndex { get; set; }

		public int posX { get; set; }

		public int posY { get; set; }

		public new static readonly HitGoodEventArgs Empty;
	}
}
