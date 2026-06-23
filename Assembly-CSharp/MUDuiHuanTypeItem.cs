using System;

public class MUDuiHuanTypeItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.info.Pivot = 3;
		this.info.MaxWidth = 200.0;
		this.info.X = -57.0;
	}

	public ShowNetImage image;

	public TextBlock title;

	public TextBlock info;

	public int DuiHuanType;

	public int SaleType;

	public UISprite bak;
}
