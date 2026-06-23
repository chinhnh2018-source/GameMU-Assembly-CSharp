using System;

public class FuBenScoreItem : UserControl
{
	public new string Name
	{
		set
		{
			this._Name.Text = "{CC7432}" + value;
		}
	}

	public string Number
	{
		set
		{
			this._Number.Text = value;
		}
	}

	public string Score
	{
		set
		{
			this._Score.Text = value;
		}
	}

	public TextBlock _Name;

	public TextBlock _Number;

	public TextBlock _Score;
}
