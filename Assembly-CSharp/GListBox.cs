using System;

public class GListBox : GScrollView
{
	public GListBox() : base(0, 0, 0)
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public new StackPanel Viewer
	{
		set
		{
			base.Viewer = value;
			value.HorizontalAlignment = Layout.Left;
			value.VerticalAlignment = Layout.Top;
			value.Orientation = Layout.Vertical;
		}
	}
}
