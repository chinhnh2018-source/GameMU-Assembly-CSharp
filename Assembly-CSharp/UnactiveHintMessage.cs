using System;

public class UnactiveHintMessage : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void InitMessage(string msg)
	{
		if (null != this.unacitiveTxt)
		{
			this.unacitiveTxt.Text = msg;
		}
	}

	public TextBlock unacitiveTxt;
}
