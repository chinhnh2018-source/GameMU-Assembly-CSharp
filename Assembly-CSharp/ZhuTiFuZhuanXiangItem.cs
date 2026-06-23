using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ZhuTiFuZhuanXiangItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("立即前往");
		this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = this.m_Link
			});
		};
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

	public GButton m_Btn;

	private int m_Link = -1;
}
