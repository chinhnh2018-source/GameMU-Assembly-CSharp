using System;
using HSGameEngine.Drawing;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class MousePosition
	{
		public MousePosition(MouseEvent e)
		{
			this._e = e;
		}

		public Point GetPosition(object value)
		{
			return new Point(0, 0);
		}

		private MouseEvent _e;
	}
}
