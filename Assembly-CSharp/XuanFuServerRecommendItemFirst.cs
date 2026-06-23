using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class XuanFuServerRecommendItemFirst : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			if (this.ServerInfo != null && this.ServerInfo.nStatus == 1)
			{
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Data = this.ServerInfo
				});
			}
		};
		this.m_SpriteNew.gameObject.SetActive(false);
	}

	public void RefreshUI(ZtBuffServerInfo serverInfoVO, int index)
	{
		this.ServerInfo = serverInfoVO;
		if (this.ServerInfo.nFirstLevelServerID != 10)
		{
			this.m_txtServerName.text = serverInfoVO.strServerName + Global.GetLang("区");
		}
		else
		{
			this.m_txtServerName.text = serverInfoVO.strServerName;
		}
		this.SetState(serverInfoVO.nStatus);
		this.Bak.URL = string.Format("NetImages/GameRes/Images/XuanFu/{0}.png", (index + 1) % 4);
		if (serverInfoVO.nStatus == 1)
		{
			this.Bak.ToGrayBitmap = true;
		}
		else
		{
			this.Bak.ToGrayBitmap = false;
		}
	}

	private void SetState(int state)
	{
		if (state == 3)
		{
			this.m_SpriteNew.gameObject.SetActive(false);
		}
		else if (state == 1)
		{
			this.m_SpriteNew.gameObject.SetActive(false);
		}
		else if (state == 4)
		{
			this.m_SpriteNew.gameObject.SetActive(true);
		}
		else if (state == 5)
		{
			this.m_SpriteNew.gameObject.SetActive(true);
		}
	}

	public UISprite m_SpriteNew;

	public TextBlock m_txtServerName;

	public ShowNetImage Bak;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ZtBuffServerInfo ServerInfo;
}
