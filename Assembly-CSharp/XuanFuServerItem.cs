using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class XuanFuServerItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.m_BackNormal.gameObject).onClick = delegate(GameObject s)
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
		UIEventListener.Get(this.m_BackWeiHu.gameObject).onClick = delegate(GameObject s)
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
		this.m_SpriteNew.gameObject.SetActive(false);
		this.m_BackWeiHu.gameObject.SetActive(false);
	}

	public void RefreshUI(ZtBuffServerInfo serverInfoVO)
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
		this.ServerNum = serverInfoVO.nOnlineNum;
		this.SetState(serverInfoVO.nStatus);
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

	private void SetState(int state)
	{
		if (state == 3)
		{
			this.m_BackNormal.gameObject.SetActive(true);
			this.m_BackWeiHu.gameObject.SetActive(false);
			this.m_SpriteNew.gameObject.SetActive(false);
		}
		else if (state == 1)
		{
			this.m_BackNormal.gameObject.SetActive(false);
			this.m_BackWeiHu.gameObject.SetActive(true);
			this.m_SpriteNew.gameObject.SetActive(false);
			this.ServerNum = -1;
		}
		else if (state == 4)
		{
			this.m_BackNormal.gameObject.SetActive(true);
			this.m_BackWeiHu.gameObject.SetActive(false);
			this.m_SpriteNew.gameObject.SetActive(true);
		}
		else if (state == 5)
		{
			this.m_BackNormal.gameObject.SetActive(true);
			this.m_BackWeiHu.gameObject.SetActive(false);
			this.m_SpriteNew.gameObject.SetActive(true);
		}
	}

	public UISprite m_SpriteNew;

	public TextBlock m_txtServerName;

	public TextBlock m_txtState;

	public UISprite m_BackNormal;

	public UISprite m_BackWeiHu;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ZtBuffServerInfo ServerInfo;
}
