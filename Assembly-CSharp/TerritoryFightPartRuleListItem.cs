using System;
using HSGameEngine.GameEngine.Logic;

public class TerritoryFightPartRuleListItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ZCRuleLab1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、军团领地争夺分东、西两个战场同时进行")
		});
		this.WinRule.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("获胜规则：")
		});
		this.WRuleLab1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、活动时间结束，积分最高的一方胜利")
		});
		this.WRuleLab2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("2、积分相同则先达到该积分的一方胜利")
		});
	}

	public int WestOrEast
	{
		set
		{
			this.ZhanChangRule.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("阿卡伦-{0}战场规则："), (value != 1) ? Global.GetLang("西") : Global.GetLang("东"))
			});
			if (value == 1)
			{
				this.ZCRuleLab2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("2、阿卡伦-东为四方混战，地图中4个基地")
				});
				this.ZCRuleLab3.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("3、基地范围内无其他军团成员5秒后则被占领，占领基地可少量获得战场积分")
				});
				this.ZCRuleLab4.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("4、地图中心会刷新旗帜，将旗帜护送回己方基地可获得大量战场积分")
				});
				this.WRuleLab3.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("3、胜利方可获得阿卡伦荒漠的控制权")
				});
			}
			else
			{
				this.ZCRuleLab2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("2、阿卡伦-西为四方混战，地图中有5个资源点")
				});
				this.ZCRuleLab3.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("3、将旗帜插在旗座上可占领资源点，资源点会持续获得战场积分")
				});
				this.ZCRuleLab4.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("4、死亡后会在附近的己方占领资源点附近复活")
				});
				this.WRuleLab3.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("3、胜利方可获得阿卡伦地宫的控制权")
				});
			}
			this.WRuleLab4.text = string.Empty;
		}
	}

	public string ImageName
	{
		set
		{
			this.LeftImage.URL = string.Format("NetImages/GameRes/Images/Plate/ArmayActivityBG/{0}", value);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public ShowNetImage LeftImage;

	public UILabel ZhanChangRule;

	public UILabel ZCRuleLab1;

	public UILabel ZCRuleLab2;

	public UILabel ZCRuleLab3;

	public UILabel ZCRuleLab4;

	public UILabel WinRule;

	public UILabel WRuleLab1;

	public UILabel WRuleLab2;

	public UILabel WRuleLab3;

	public UILabel WRuleLab4;
}
