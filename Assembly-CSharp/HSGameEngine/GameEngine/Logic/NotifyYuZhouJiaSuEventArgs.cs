using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class NotifyYuZhouJiaSuEventArgs : EventArgs
	{
		public long CorrectLocalTime { get; set; }

		public double ServerTicks { get; set; }

		public double AvgTicks { get; set; }

		public double LastClientServerSubNum { get; set; }

		public double AllowTicks { get; set; }
	}
}
