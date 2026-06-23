using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

internal class ShuXingDuiBiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.ClosBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
	}

	public void InitPartData()
	{
	}

	public GButton ClosBtn;

	public DPSelectedItemBoolEventHandler DPSelectedItem;
}
