using System;
using HSGameEngine.GameEngine.Logic;

public class JingLingSkillsIcon : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
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

	public void InitData(int skillId, int Lev)
	{
		int skillMagicColor = Global.GetSkillMagicColor(skillId);
		this.m_Sgin.URL = Global.GetJingLingSkillSignURL(skillId);
		if (1 > Lev)
		{
			Lev = 1;
		}
		this.m_LvLabel.text = string.Format("Lv{0}", Lev);
		this.m_NameLabel.text = Global.GetJingLinfSkillName(skillId, true);
		this.m_Steps.gameObject.SetActive(true);
		if (skillMagicColor == 1)
		{
			this.m_Steps.spriteName = "iconState_zuoyue";
		}
		else if (skillMagicColor == 2)
		{
			this.m_Steps.spriteName = "iconState_zuoyue1";
		}
		else if (skillMagicColor == 3)
		{
			this.m_Steps.spriteName = "iconState_zuoyue2";
		}
		else
		{
			this.m_Steps.gameObject.SetActive(false);
		}
	}

	public void InitHorseSkillData(int SkillId, int Lev)
	{
		MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(SkillId);
		if (skillXmlNode != null)
		{
			this.m_Sgin.URL = "NetImages/GameRes/Images/Skill/" + skillXmlNode.MagicIcon + ".png";
			this.m_LvLabel.text = string.Format("Lv{0}", Lev);
			this.m_NameLabel.text = skillXmlNode.Name;
		}
		int skillMagicColor = Global.GetSkillMagicColor(SkillId);
		if (skillMagicColor == 1)
		{
			this.m_Steps.spriteName = "iconState_zuoyue";
		}
		else if (skillMagicColor == 2)
		{
			this.m_Steps.spriteName = "iconState_zuoyue1";
		}
		else if (skillMagicColor == 3)
		{
			this.m_Steps.spriteName = "iconState_zuoyue2";
		}
		else
		{
			this.m_Steps.gameObject.SetActive(false);
		}
	}

	public ShowNetImage m_Sgin;

	public UISprite m_Bg;

	public UILabel m_NameLabel;

	public UILabel m_LvLabel;

	public UISprite m_Steps;
}
