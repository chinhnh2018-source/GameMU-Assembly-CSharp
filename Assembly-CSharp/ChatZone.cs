using System;
using HSGameEngine.Drawing;

public class ChatZone : UserControl
{
	public Point Center
	{
		get
		{
			return this._Center;
		}
		set
		{
			this._Center = value;
		}
	}

	private Point _Center = new Point(0, 0);
}
