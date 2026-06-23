using System;

public class ParcelChongShengPart : ParcelPart
{
	protected override void InitializeComponent()
	{
		base.IsRebornParcel = true;
		base.InitializeComponent();
	}
}
