using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class EmblemCoolDownEventArgs : EventArgs
	{
		public int EmblemID { get; set; }

		public long CDTicks { get; set; }

		public long ContinuedTicks { get; set; }
	}
}
