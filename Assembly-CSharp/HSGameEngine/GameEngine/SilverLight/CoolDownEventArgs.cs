using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class CoolDownEventArgs : EventArgs
	{
		public int SkillID { get; set; }

		public long ToDrawTicks { get; set; }
	}
}
