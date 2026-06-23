using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class YanHuiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.haveTimes.text = string.Empty;
		this.description3.text = string.Empty;
		this.description2.text = string.Empty;
		this.staticStr.text = string.Empty;
		this.description1.text = string.Empty;
		this.juBan.Text = string.Empty;
		this.xiaoHaoNum.text = string.Empty;
		this.staticStr.text = Global.GetLang("{FFCC19}消耗金币{-}");
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("LuoLanYanHuiAward", true);
		this.description1.text = Global.GetLang("{FFCC19}宴会结束后,所有玩家在宴会中的{-}{17E43E}消费{-}{FFCC19}会按照{-}{17E43E}") + systemParamByName + Global.GetLang("{-}{FFCC19}的比例换算为{17E43E}[钻石]{-},用{-}{17E43E}邮件{-}{FFCC19}发放给举办者{-}");
		this.juBan.Text = Global.GetLang("举办宴会");
		this.haveTimes.transform.localPosition = new Vector3(-346f, -246f, 0f);
		this.description3.transform.localPosition = new Vector3(50f, -246f, 0f);
	}

	private void SelectYanHui(int type)
	{
		this.puTongYanHui.Check = false;
		this.fengShengYanHui.Check = false;
		this.HaoHuaYanHui.Check = false;
		if (type == 0)
		{
			this.puTongYanHui.Check = true;
		}
		else if (type == 1)
		{
			this.fengShengYanHui.Check = true;
		}
		else
		{
			this.HaoHuaYanHui.Check = true;
		}
		XElement gameResXml = Global.GetGameResXml("Config/GleeFeastAward.Xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "YanHui");
		if (type == 0)
		{
			this.description2.text = Global.GetLang("{DAC7AA}普通宴会{-}");
		}
		else if (type == 1)
		{
			this.description2.text = Global.GetLang("{DAC7AA}丰盛宴会{-}");
		}
		else
		{
			this.description2.text = Global.GetLang("{DAC7AA}豪华宴会{-}");
		}
		UILabel uilabel = this.description2;
		string text = uilabel.text;
		uilabel.text = string.Concat(new object[]
		{
			text,
			Global.GetLang("{DAC7AA}-参与者可获得:战功"),
			Global.GetXElementAttributeInt(xelementList[type], "ZhanGongAward"),
			Global.GetLang("、经验"),
			Global.GetXElementAttributeInt(xelementList[type], "EXPAward"),
			Global.GetLang("、星魂"),
			Global.GetXElementAttributeInt(xelementList[type], "XingHunAward"),
			"{-}"
		});
		this.xiaoHaoNum.text = Global.GetXElementAttributeInt(xelementList[type], "ConductBindJinBi").ToString();
		this.haveTimes.text = Global.GetLang("{E3B36C}可参加次数:{-}") + Global.GetXElementAttributeInt(xelementList[type], "SumNum");
		this.YanHuiID = Global.GetXElementAttributeInt(xelementList[type], "ID");
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = correctDateTime.AddDays(1.0);
		string[] array = Global.GetXElementAttributeStr(xelementList[type], "BeginTime").Split(new char[]
		{
			':'
		});
		int[] array2 = new int[]
		{
			correctDateTime.Hour,
			correctDateTime.Minute,
			correctDateTime.Second
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array2[0] < Convert.ToInt32(array[i]))
			{
				dateTime = dateTime.AddDays(-1.0);
				break;
			}
		}
		this.description3.text = string.Concat(new object[]
		{
			"{E3B36C}",
			Global.GetLang("摆放时间:"),
			Global.GetLang("年"),
			dateTime.Year,
			Global.GetLang("月"),
			dateTime.Month,
			Global.GetLang("日"),
			dateTime.Day,
			"  ",
			Global.GetXElementAttributeStr(xelementList[type], "BeginTime"),
			"-",
			Global.GetXElementAttributeStr(xelementList[type], "OverTime"),
			"{-}"
		});
	}

	public void RefreshMoney()
	{
		this.OwnZuanShi.text = Global.Data.roleData.UserMoney.ToString();
		this.OwnMoney.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.SelectYanHui(0);
		this.RefreshMoney();
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
		this.puTongYanHui.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectYanHui(0);
		};
		this.fengShengYanHui.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectYanHui(1);
		};
		this.HaoHuaYanHui.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectYanHui(2);
		};
		this.juBan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.ApplyYanHui(this.YanHuiID, 0);
		};
		base.InitializeComponent();
		PlayZone.GlobalPlayZone.yanhuiPart = this;
	}

	public UILabel haveTimes;

	public UILabel description1;

	public UILabel description2;

	public UILabel description3;

	public UILabel staticStr;

	public UILabel xiaoHaoNum;

	public UILabel OwnMoney;

	public UILabel OwnZuanShi;

	public GCheckBox puTongYanHui;

	public GCheckBox fengShengYanHui;

	public GCheckBox HaoHuaYanHui;

	public GButton juBan;

	public GButton close;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int YanHuiID;
}
