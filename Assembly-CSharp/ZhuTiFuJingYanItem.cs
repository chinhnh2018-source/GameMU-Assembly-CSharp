using System;

public class ZhuTiFuJingYanItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public int Link
	{
		get
		{
			return this.m_Link;
		}
		set
		{
			this.m_Link = value;
		}
	}

	public UILabel m_Title;

	public int m_Link = -1;
}
