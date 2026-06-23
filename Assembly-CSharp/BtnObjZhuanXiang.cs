using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class BtnObjZhuanXiang : UserControl
{
	public UIType MUIType
	{
		get
		{
			return this.m_MUIType;
		}
		set
		{
			this.m_MUIType = value;
		}
	}

	public string Label
	{
		get
		{
			return this.m_NameLabel;
		}
		set
		{
			this.m_NameLabel = value;
			this.m_Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang(this.m_NameLabel)
			});
		}
	}

	public int ActID
	{
		get
		{
			return this.m_ActID;
		}
		set
		{
			this.m_ActID = value;
		}
	}

	public int type
	{
		get
		{
			return this.m_type;
		}
		set
		{
			this.m_type = value;
			this.strUrl = string.Format(this.strUrl + "ImgType{0}.png", this.type);
			this.m_img.URL = this.strUrl;
		}
	}

	public ZhuanxiangHuodongItem Pni
	{
		get
		{
			return this.m_ZhuanxiangHuodongItem;
		}
		set
		{
			this.m_ZhuanxiangHuodongItem = value;
		}
	}

	public bool IsActiveItem
	{
		get
		{
			return this.m_IsActiveItem;
		}
		set
		{
			if (null != this.m_ZhuanxiangHuodongItem && this.m_IsActiveItem != value)
			{
				if (value)
				{
					this.m_Back.spriteName = "leftbtn_normal";
					this.m_Label.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}", Global.GetLang(base.name))
					});
					NGUITools.SetActive(this.m_ZhuanxiangHuodongItem, true);
				}
				else
				{
					this.m_Back.spriteName = "leftbtn_disable";
					this.m_Label.text = Global.GetColorStringForNGUIText(new object[]
					{
						"9d8667",
						string.Format("{0}", Global.GetLang(base.name))
					});
					NGUITools.SetActive(this.m_ZhuanxiangHuodongItem, false);
				}
			}
			this.m_IsActiveItem = value;
		}
	}

	public bool IsOK
	{
		get
		{
			return this.m_IsOK;
		}
		set
		{
			this.m_IsOK = value;
			if (this.m_IsOK)
			{
				NGUITools.SetActive(this.m_Icon, true);
			}
			else
			{
				NGUITools.SetActive(this.m_Icon, false);
			}
		}
	}

	public GameObject m_Icon;

	public ZhuanxiangHuodongItem m_ZhuanxiangHuodongItem;

	public string m_NameLabel = string.Empty;

	public UILabel m_Label;

	public UISprite m_Back;

	public GButton m_Button;

	public ShowNetImage m_img;

	private string strUrl = "NetImages/GameRes/Images/ZhuanXiang/";

	public int m_type;

	private int m_ActID = -1;

	public bool m_IsActiveItem;

	private bool m_IsOK;

	private UIType m_MUIType = UIType.Other;
}
