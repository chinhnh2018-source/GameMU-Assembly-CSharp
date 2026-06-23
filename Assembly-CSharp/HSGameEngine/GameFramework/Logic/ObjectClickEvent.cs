using System;
using HSGameEngine.GameEngine.SilverLight;

namespace HSGameEngine.GameFramework.Logic
{
	public class ObjectClickEvent : EventArgs
	{
		public ObjectClickEvent(string type, bool bubbles = false, bool cancelable = false)
		{
		}

		public ClickGetThingTypes ClickGetThingType { get; set; }

		public int ClickGetThingDbID { get; set; }

		public object Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				this._Tag = value;
			}
		}

		public MouseEvent e
		{
			get
			{
				return this._e;
			}
			set
			{
				this._e = value;
			}
		}

		public bool NextClick { get; set; }

		public object sender { get; set; }

		public static string ObjectClickDown = "ObjectClickDown";

		private object _Tag;

		private MouseEvent _e;
	}
}
