using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ZhuTiFuItem : UserControl
{
	public string Title
	{
		set
		{
			this.m_Button.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang(value)
			});
		}
	}

	public string XmlList
	{
		get
		{
			return this.m_XmlList;
		}
		set
		{
			this.m_XmlList = value;
		}
	}

	public int ID
	{
		get
		{
			return this.m_ID;
		}
		set
		{
			this.m_ID = value;
		}
	}

	public int Type
	{
		get
		{
			return this.m_Type;
		}
		set
		{
			this.m_Type = value;
		}
	}

	public void IsOnClick(bool falg)
	{
		if (falg)
		{
			this.m_Button.normalSprite = "leftbtn_normal";
			this.m_Button.hoverSprite = "leftbtn_normal";
			this.m_Button.disabledSprite = "leftbtn_normal";
			this.m_Button.target.spriteName = "leftbtn_normal";
			this.m_Button.Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{0}", Global.GetLang(base.name))
			});
		}
		else
		{
			this.m_Button.normalSprite = "leftbtn_disable";
			this.m_Button.hoverSprite = "leftbtn_disable";
			this.m_Button.disabledSprite = "leftbtn_disable";
			this.m_Button.target.spriteName = "leftbtn_disable";
			this.m_Button.Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				string.Format("{0}", Global.GetLang(base.name))
			});
		}
	}

	public void TipActive(bool flag)
	{
		this.m_GameTip.SetActive(flag);
	}

	public GButton m_Button;

	private int m_Type;

	public GameObject m_GameTip;

	private int m_ID = -1;

	private string m_XmlList = string.Empty;
}
