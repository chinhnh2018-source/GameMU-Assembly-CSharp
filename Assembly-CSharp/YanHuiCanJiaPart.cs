using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class YanHuiCanJiaPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.staticStr.text = Global.GetLang("消耗金币");
		this.joinYanHui.Text = Global.GetLang("参加宴会");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.joinYanHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.JoinYanHui();
		};
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
	}

	public void InitPart(int type, int userTimes, int SumTimes)
	{
		XElement gameResXml = Global.GetGameResXml("Config/GleeFeastAward.Xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "YanHui");
		type--;
		if (type == 0)
		{
			this.description.text = Global.GetLang("普通宴会");
		}
		else if (type == 1)
		{
			this.description.text = Global.GetLang("丰盛宴会");
		}
		else
		{
			if (type != 2)
			{
				Super.HintMainText(Global.GetLang("罗兰城主尚未举办庆功"), 10, 3);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs());
				return;
			}
			this.description.text = Global.GetLang("豪华宴会");
		}
		if (!Global.HaveYanHui)
		{
			this.joinYanHui.normalSprite = "sbutton_disable";
			this.joinYanHui.hoverSprite = "sbutton_disable";
			this.joinYanHui.pressedSprite = "sbutton_disable";
			this.joinYanHui.disabledSprite = "sbutton_disable";
			this.joinYanHui.isEnabled = false;
		}
		UILabel uilabel = this.description;
		string text = uilabel.text;
		uilabel.text = string.Concat(new object[]
		{
			text,
			Global.GetLang("-参与者可获得:战功"),
			Global.GetXElementAttributeInt(xelementList[type], "ZhanGongAward"),
			Global.GetLang("、经验"),
			Global.GetXElementAttributeInt(xelementList[type], "EXPAward"),
			Global.GetLang("、星魂"),
			Global.GetXElementAttributeInt(xelementList[type], "XingHunAward")
		});
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[type], "UseNum");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[type], "SumNum");
		this.haveTimes.text = string.Concat(new object[]
		{
			Global.GetLang("{E3B36C}今日可参加次数:{-}"),
			"{FFFFFF}",
			xelementAttributeInt - userTimes,
			"/",
			xelementAttributeInt,
			"{-}\n"
		});
		UILabel uilabel2 = this.haveTimes;
		text = uilabel2.text;
		uilabel2.text = string.Concat(new object[]
		{
			text,
			Global.GetLang("{E3B36C}可参加总次数:{-}"),
			"{FFFFFF}",
			xelementAttributeInt2 - SumTimes,
			"/",
			xelementAttributeInt2,
			"{-}"
		});
		this.xiaoHaoMoney.text = Global.GetXElementAttributeInt(xelementList[type], "BindJinBi").ToString();
		this.OwnZuanShi.text = Global.Data.roleData.UserMoney.ToString();
		this.OwnMoney.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
	}

	public UILabel haveTimes;

	public UILabel description;

	public UILabel staticStr;

	public UILabel xiaoHaoMoney;

	public UILabel OwnMoney;

	public UILabel OwnZuanShi;

	public GButton joinYanHui;

	public GButton close;

	public DPSelectedItemEventHandler DPSelectedItem;
}
