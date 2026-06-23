using System;

public class LeagueEventItem : UserControl
{
	public string TimeStr
	{
		get
		{
			return this.m_TimeLabel.Text;
		}
		set
		{
			this.m_TimeLabel.Text = value;
		}
	}

	public string EventStr
	{
		get
		{
			return this.m_EventLabel.Text;
		}
		set
		{
			this.m_EventLabel.Text = value;
			this.m_EventLabel.Label.lineWidth = 435;
		}
	}

	public TextBlock m_TimeLabel;

	public TextBlock m_EventLabel;
}
