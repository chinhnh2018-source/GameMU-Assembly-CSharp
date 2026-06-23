using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.SilverLight;

public class MousePosition
{
	public MousePosition(MouseEvent e)
	{
		this.evt = e;
	}

	public Point GetPosition(SpriteSL canvas)
	{
		return new Point(0, 0);
	}

	public Point GetPosition(Displayobject value)
	{
		Point value2 = new Point(this.evt.stageX, this.evt.stageY);
		Point point = value.globalToLocal(value2);
		return new Point(point.X, point.Y);
	}

	private MouseEvent evt;
}
