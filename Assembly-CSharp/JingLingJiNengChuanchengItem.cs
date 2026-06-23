using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class JingLingJiNengChuanchengItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_SpOnClickBack.gameObject.SetActive(false);
		this.m_Game.SetActive(false);
		UIEventListener.Get(this.m_SpBack.gameObject).onClick = delegate(GameObject go)
		{
			this.m_SpOnClickBack.gameObject.SetActive(!this.m_Game.activeSelf);
			this.m_Game.SetActive(!this.m_Game.activeSelf);
		};
		UIEventListener.Get(this.m_Game).onClick = delegate(GameObject go)
		{
			this.m_SpOnClickBack.gameObject.SetActive(false);
			this.m_Game.SetActive(false);
		};
		this.xml_Magics = Global.GetGameResXml("Config/Magics.xml");
	}

	public int SkillId
	{
		get
		{
			return this.m_SkillId;
		}
		set
		{
			this.m_SkillId = value;
			this.SkillStep = Global.GetSkillMagicColor(this.m_SkillId);
			string jingLingSkillSignURL = Global.GetJingLingSkillSignURL(this.m_SkillId);
			if ("NoImage" != jingLingSkillSignURL)
			{
				this.m_ShowImg.URL = jingLingSkillSignURL;
			}
		}
	}

	public int Lev
	{
		get
		{
			return this.m_Lev;
		}
		set
		{
			if (0 <= this.m_SkillId)
			{
				this.m_Lev = ((1 < value) ? value : 1);
			}
			else
			{
				this.m_Lev = value;
			}
			this.m_LabLevel.text = ((0 >= this.m_Lev) ? string.Empty : Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format("Lv{0}", this.m_Lev)
			}));
		}
	}

	public int SkillStep
	{
		get
		{
			return this.m_Step;
		}
		set
		{
			this.m_Step = value;
			if (this.m_Step == 1)
			{
				this.m_SpBack.spriteName = "iconState_zuoyue";
			}
			else if (this.m_Step == 2)
			{
				this.m_SpBack.spriteName = "iconState_zuoyue1";
			}
			else if (this.m_Step == 3)
			{
				this.m_SpBack.spriteName = "iconState_zuoyue2";
			}
		}
	}

	public void SetSkillId(int skillID, int lev)
	{
		string text = string.Empty;
		string text2 = lev.ToString();
		string content = string.Empty;
		string magicScripts = string.Empty;
		if (this.m_DidXML.ContainsKey(skillID))
		{
			text = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "Name");
			content = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "Description");
			magicScripts = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "MagicScripts");
		}
		else if (this.xml_Magics != null)
		{
			XElement xelement = Global.GetXElementList(this.xml_Magics, "Magic").Find((XElement s) => s.Attribute("ID").Value == skillID.ToString());
			if (xelement != null)
			{
				this.m_DidXML.Add(skillID, xelement);
				text = Global.GetXElementAttributeStr(xelement, "Name");
				content = Global.GetXElementAttributeStr(xelement, "Description");
				magicScripts = Global.GetXElementAttributeStr(xelement, "MagicScripts");
			}
		}
		this.m_LabTitle.text = Global.GetJingLinfSkillName(skillID, true);
		this.m_LabContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("Lv{0}", text2)
		}) + Environment.NewLine + Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.ClearSpaceOfString(content), string.Format("{0}%", Global.GetJIngLingSkillAddAttack(magicScripts, lev)))
		});
	}

	public UILabel m_LabLevel;

	public UISprite m_SpBack;

	public UISprite m_SpOnClickBack;

	public ShowNetImage m_ShowImg;

	public UILabel m_LabTitle;

	public UILabel m_LabContent;

	public GameObject m_Game;

	private int m_SkillId = -1;

	private int m_Step = -1;

	private int m_Lev;

	private Dictionary<int, XElement> m_DidXML = new Dictionary<int, XElement>();

	private XElement xml_Magics;
}
