using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class MouseClickEventArgs : EventArgs
	{
		public bool Cancel;

		public object sender;

		public int ClickGetThingDbID;

		public int ClickGetThingType;

		public object e;

		public bool Handled;

		public bool NextClick;

		public bool Cancelue;
	}
}
