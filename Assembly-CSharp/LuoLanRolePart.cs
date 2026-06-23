using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class LuoLanRolePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
	}

	public GButton close;

	public DPSelectedItemEventHandler DPSelectedItem;
}
