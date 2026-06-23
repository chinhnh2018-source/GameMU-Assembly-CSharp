using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class BaseEventArgs : EventArgs
	{
		public int type { get; set; }

		public object Tag { get; set; }

		public string Text { get; set; }

		public object target { get; set; }

		public int IDType { get; set; }

		public int ID { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public EventArgs e { get; set; }

		public static readonly HitGoodEventArgs Empty;
	}
}
