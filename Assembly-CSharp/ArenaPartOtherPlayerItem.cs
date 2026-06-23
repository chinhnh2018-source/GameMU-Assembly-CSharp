using System;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class ArenaPartOtherPlayerItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_TiaozhanBtn.Text = Global.GetLang("挑战");
		this.m_RankingLabel.Y = 303.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public void init(PlayerJingJiMiniData data, int index)
	{
		this.m_NameLabel.Text = data.roleName;
		this.Index = index;
		this.m_ZhanliLabel.Text = StringUtil.substitute(Global.GetLang("战力") + ":{0}", new object[]
		{
			data.combatForce
		});
		this.m_RankingLabel.Text = StringUtil.substitute("{0}", new object[]
		{
			data.ranking
		});
		this.m_TiaozhanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = int.Parse(this.m_TiaozhanBtn.tag),
					Type = -1,
					Flag = data.ranking
				});
			}
		};
	}

	public void ClearContent()
	{
		this.m_NameLabel.Text = string.Empty;
		this.Index = -1;
		this.m_ZhanliLabel.Text = string.Empty;
		this.m_RankingLabel.Text = string.Empty;
	}

	public TextBlock m_NameLabel;

	public TextBlock m_ZhanliLabel;

	public TextBlock m_RankingLabel;

	public GButton m_TiaozhanBtn;

	public ShowNetImage m_PersonTex;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int Index;
}
