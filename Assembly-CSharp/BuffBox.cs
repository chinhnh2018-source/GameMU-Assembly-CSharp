using System;

public class BuffBox : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public int BuffsNum
	{
		set
		{
			this.BuffNum.Text = string.Format("X{0}", value);
		}
	}

	public GButton BuffIcon;

	public TextBlock BuffNum;
}
