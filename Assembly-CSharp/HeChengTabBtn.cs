using System;

public class HeChengTabBtn : UserControl
{
	protected override void InitializeComponent()
	{
		this.Btn.Label.color = NGUIMath.HexToColorEx(16772103U);
	}

	public GButton Btn;
}
