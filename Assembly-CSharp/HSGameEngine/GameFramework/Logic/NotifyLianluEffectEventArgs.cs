using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class NotifyLianluEffectEventArgs : EventArgs
	{
		public int EffectID { get; set; }

		public int PlayID { get; set; }
	}
}
