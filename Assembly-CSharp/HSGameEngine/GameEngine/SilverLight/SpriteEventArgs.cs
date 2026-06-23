using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class SpriteEventArgs : BaseEventArgs
	{
		public int ScriptID { get; set; }

		public int SpriteType { get; set; }

		public new static readonly SpriteEventArgs Empty;
	}
}
