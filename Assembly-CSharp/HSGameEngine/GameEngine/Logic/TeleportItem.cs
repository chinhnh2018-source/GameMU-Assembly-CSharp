using System;
using HSGameEngine.Drawing;

namespace HSGameEngine.GameEngine.Logic
{
	public class TeleportItem
	{
		public int TeleportKey { get; set; }

		public Point TeleportPos { get; set; }

		public int ToMapID { get; set; }

		public Point ToMapPos { get; set; }
	}
}
