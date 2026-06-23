using System;

public class HuoyueInfoItem : UserControl
{
	public string DesText
	{
		set
		{
			this.m_DesLabel.Text = value;
		}
	}

	public string AwardText
	{
		set
		{
			this.m_AwardLabel.Text = value;
		}
	}

	public string CurrText
	{
		set
		{
			this.m_CurrLabel.Text = value;
		}
	}

	public string TotalText
	{
		set
		{
			this.m_TotalLabel.Text = value;
		}
	}

	public TextBlock m_DesLabel;

	public TextBlock m_AwardLabel;

	public TextBlock m_CurrLabel;

	public TextBlock m_TotalLabel;
}
