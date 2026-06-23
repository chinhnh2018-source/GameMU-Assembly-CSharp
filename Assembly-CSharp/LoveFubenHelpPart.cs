using System;
using HSGameEngine.GameEngine.SilverLight;

public class LoveFubenHelpPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.CloseLoveFubenHelpWindow();
		};
	}

	public GButton Close;
}
