using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class SpriteChangeActionEventArgs : EventArgs
	{
		public int OldAction { get; set; }

		public int Action { get; set; }
	}
}
