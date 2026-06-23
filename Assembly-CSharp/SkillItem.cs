using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Server.Tools;

public class SkillItem : UserControl
{
	public int skillLeve
	{
		get
		{
			return this.skilllev;
		}
		set
		{
			this.skilllev = value;
		}
	}

	public int skillUsedNum
	{
		get
		{
			return this.skillShuLianDu;
		}
		set
		{
			this.skillShuLianDu = value;
		}
	}

	public int skillDBid
	{
		get
		{
			return this.skilldbid;
		}
		set
		{
			this.skilldbid = value;
		}
	}

	public int ID
	{
		get
		{
			return this.jiNengid;
		}
	}

	public string selectStat
	{
		set
		{
			this.Bak.spriteName = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.ItemName.Text = Global.GetLang("敬请期待");
		this.ItemName.FontSize = 18;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public void SetSkillLevel(int skillLevel)
	{
		if (Global.getSkillAddPoin(this.jiNengid) != 0)
		{
			this.ItemName.Text = string.Format("LV.{{fac60d}}{0}{{-}}{{17e43e}}+{1}{{-}}\n{{3681f3}}{2}{{-}}", skillLevel, Global.getSkillAddPoin(this.jiNengid), this.strTitle);
		}
		else
		{
			this.ItemName.Text = string.Format("LV.{{fac60d}}{0}{{-}}\n{{3681f3}}{1}{{-}}", skillLevel, this.strTitle);
		}
	}

	public void InitSkill(MagicInfoVO xmlItem, int skillID, SkillData skillData)
	{
		this.jiNengid = skillID;
		int id = xmlItem.ID;
		int magicIcon = xmlItem.MagicIcon;
		this.strTitle = xmlItem.Name;
		if (magicIcon < 0)
		{
		}
		this.icon.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
		{
			skillID
		});
		if (skillData == null)
		{
			this.ItemName.Text = string.Format(Global.GetLang("未学习\n{{3681f3}}{0}{{-}}"), this.strTitle);
			this.ItemName.textColor = 7697781U;
			this.ItemState.textColor = 7697781U;
		}
		else if (Global.getSkillAddPoin(this.jiNengid) != 0)
		{
			this.ItemName.Text = string.Format("LV.{{ff9d08}}{0}{{-}}{{17e43e}}+{1}{{-}}\n{{3681f3}}{2}{{-}}", skillData.SkillLevel, Global.getSkillAddPoin(this.jiNengid), this.strTitle);
		}
		else
		{
			this.ItemName.Text = string.Format("LV.{{ff9d08}}{0}{{-}}\n{{3681f3}}{1}{{-}}", skillData.SkillLevel, this.strTitle);
		}
	}

	public TextBlock ItemName;

	public TextBlock ItemState;

	public ShowNetImage icon;

	public UISprite Bak;

	public UISprite UpLevStat;

	private int jiNengid;

	private string strTitle;

	private int skilllev;

	private int skillShuLianDu;

	private int skilldbid;
}
