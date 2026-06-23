using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class ChangeEventArgs : EventArgs
	{
		public int ChangeType { get; set; }

		public static readonly ChangeEventArgs Empty;
	}
}
