using System;

public class FuBenTiShiItem : UserControl
{
	public string Img
	{
		set
		{
			this.m_show.URL = value;
		}
	}

	public ShowNetImage m_show;

	public GButton m_Btn;
}
