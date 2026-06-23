using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class CoolDownItem
	{
		public int ID { get; set; }

		public long StartTicks { get; set; }

		public long CDTicks { get; set; }
	}
}
