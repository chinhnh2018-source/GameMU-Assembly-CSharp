using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class MoYuTaskUserItem : UserControl
{
	public ZorkBattleRoleInfo RoleInfo
	{
		get
		{
			return this.m_roleInfo;
		}
		set
		{
			this.m_roleInfo = value;
			this.lblName.text = this.m_roleInfo.Name;
			this.lblLevel.text = this.m_roleInfo.RebornLevel.ToString();
			this.imgSlider.fillAmount = (float)this.m_roleInfo.LifeV * 1f / (float)this.m_roleInfo.MaxLifeV;
			this.IconName = this.GetTouXiangNameByOccu(this.RoleInfo.Occupation);
		}
	}

	private string IconName
	{
		set
		{
			this.imgIcon.spriteName = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblLevel.text = Global.GetLang("12");
		this.lblName.text = Global.GetLang("队伍1玩家名字");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void InitDaTaoShaData(EscapeBattleRoleInfo info)
	{
		this.lblName.text = info.Name;
		this.lblLevel.text = info.Level.ToString();
		this.imgSlider.fillAmount = (float)info.LifeV * 1f / (float)info.MaxLifeV;
		this.IconName = this.GetTouXiangNameByOccu(info.Occupation);
	}

	private string GetTouXiangNameByOccu(int occu)
	{
		string result = string.Empty;
		switch (occu)
		{
		case 0:
			result = "00_0";
			break;
		case 1:
			result = "10_0";
			break;
		case 2:
			result = "20_0";
			break;
		case 3:
			result = "30_0";
			break;
		case 5:
			result = "50_0";
			break;
		}
		return result;
	}

	public UILabel lblLevel;

	public UILabel lblName;

	public UISprite imgSlider;

	public UISprite imgIcon;

	private ZorkBattleRoleInfo m_roleInfo;
}
