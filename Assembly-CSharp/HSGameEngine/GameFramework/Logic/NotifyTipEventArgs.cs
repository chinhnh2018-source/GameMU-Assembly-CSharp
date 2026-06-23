using System;
using HSGameEngine.GameEngine.SilverLight;

namespace HSGameEngine.GameFramework.Logic
{
	public class NotifyTipEventArgs
	{
		public bool MouseState { get; set; }

		public TipTypes TipType { get; set; }

		public string Tip { get; set; }

		public MouseEvent MouseEvent { get; set; }
	}
}
