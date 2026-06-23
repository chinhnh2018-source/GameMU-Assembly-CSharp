using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class JingLingSkillTips : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.xml_Magics = Global.GetGameResXml("Config/Magics.xml");
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void SetSkillId(int skillID, int lev)
	{
		string text = lev.ToString();
		string text2 = string.Empty;
		string magicScripts = string.Empty;
		if (this.m_DidXML.ContainsKey(skillID))
		{
			text2 = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "Description");
			magicScripts = Global.GetXElementAttributeStr(this.m_DidXML[skillID], "MagicScripts");
		}
		else if (this.xml_Magics != null)
		{
			XElement xelement = Global.GetXElementList(this.xml_Magics, "Magic").Find((XElement s) => s.Attribute("ID").Value == skillID.ToString());
			if (xelement != null)
			{
				this.m_DidXML.Add(skillID, xelement);
				text2 = Global.GetXElementAttributeStr(xelement, "Description");
				magicScripts = Global.GetXElementAttributeStr(xelement, "MagicScripts");
			}
		}
		this.m_SkillName.text = Global.GetJingLinfSkillName(skillID, true);
		this.m_Content.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(text2, string.Format("{0}%", Global.GetJIngLingSkillAddAttack(magicScripts, lev)))
		});
		this.m_Lev.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("Lv{0}", text)
		});
	}

	public UILabel m_SkillName;

	public UILabel m_Lev;

	public UILabel m_Content;

	private Dictionary<int, XElement> m_DidXML = new Dictionary<int, XElement>();

	private XElement xml_Magics;
}
