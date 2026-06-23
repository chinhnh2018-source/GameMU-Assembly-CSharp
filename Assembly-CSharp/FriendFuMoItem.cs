using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class FriendFuMoItem : UserControl
{
	public string ShowNetImg
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.m_ShowTouXiang.URL = value;
			}
		}
	}

	public string Cotent
	{
		set
		{
			this.m_LabContent.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(value)
			});
		}
	}

	public new string Name
	{
		set
		{
			this.m_LabName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang(value)
			});
		}
	}

	public int RoleID
	{
		get
		{
			return this.roleID;
		}
		set
		{
			this.roleID = value;
		}
	}

	public int Time
	{
		set
		{
			if (value <= 0)
			{
				this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("今天")
				});
			}
			else
			{
				this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(string.Format(Global.GetLang("{0}天前"), value))
				});
			}
		}
	}

	public string Img
	{
		set
		{
			this.m_ShowTouXiang.URL = value;
		}
	}

	public bool IsRed
	{
		set
		{
			this.m_GameTip.SetActive(value);
		}
	}

	public int MailID
	{
		get
		{
			return this.m_MailID;
		}
		set
		{
			this.m_MailID = value;
		}
	}

	[SerializeField]
	private ShowNetImage m_ShowTouXiang;

	[SerializeField]
	private UILabel m_LabName;

	[SerializeField]
	private UILabel m_LabContent;

	[SerializeField]
	private UILabel m_LabTime;

	[SerializeField]
	public UISprite m_SpBack;

	[SerializeField]
	private GameObject m_GameTip;

	public GameObject m_GamePanel;

	private int m_MailID = -1;

	private int roleID = -1;
}
