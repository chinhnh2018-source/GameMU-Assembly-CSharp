using System;
using HSGameEngine.GameEngine.Logic;

public class JingLingSkillPreviewItem : UserControl
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

	public void SetSkillInf(int SkillId, string Name, string content)
	{
		this.m_SkillSign.URL = Global.GetJingLingSkillSignURL(SkillId);
		this.m_Namelabel.text = Name;
		this.m_Content.text = content;
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public ShowNetImage m_SkillSign;

	public UILabel m_Namelabel;

	public UILabel m_Content;
}
