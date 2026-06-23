using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class ClickGetThingEventArgs : EventArgs
	{
		public int ClickGetThingType { get; set; }

		public int ClickGetThingDbID { get; set; }

		public object e { get; set; }

		public bool Handled { get; set; }

		public bool NextClick { get; set; }

		public bool Cancel { get; set; }

		public object sender { get; set; }
	}
}
