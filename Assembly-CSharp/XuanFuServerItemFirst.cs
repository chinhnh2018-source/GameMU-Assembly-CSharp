using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class XuanFuServerItemFirst : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_BtnServerInfo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Data = this.ServerInfo
				});
			}
		};
	}

	public void RefreshServerInfo(ZtBuffServerInfo info)
	{
		this.ServerInfo = info;
		if (this.ServerInfo.nFirstLevelServerID != 10)
		{
			this.m_Title.text = info.strServerName + Global.GetLang("区");
		}
		else
		{
			this.m_Title.text = info.strServerName;
		}
		this.ServerNum = info.nOnlineNum;
		if (info.nStatus == 1)
		{
			this.m_BtnServerInfo.isEnabled = false;
			this.ServerNum = -1;
		}
		else
		{
			this.m_BtnServerInfo.isEnabled = true;
		}
	}

	public int ServerNum
	{
		set
		{
			if (value < 0)
			{
				this.m_txtState.text = Global.GetColorStringForNGUIText(new object[]
				{
					"757575",
					Global.GetLang("维护")
				});
			}
			else if (value <= 1)
			{
				this.m_txtState.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("畅通")
				});
			}
			else if (value == 2)
			{
				this.m_txtState.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("拥挤")
				});
			}
			else
			{
				this.m_txtState.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("繁忙")
				});
			}
		}
	}

	public GButton m_BtnServerInfo;

	public UILabel m_Title;

	public UILabel m_txtState;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ZtBuffServerInfo ServerInfo;
}
