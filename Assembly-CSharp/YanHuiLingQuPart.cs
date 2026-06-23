using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class YanHuiLingQuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.haveTimes.text = string.Empty;
		this.description.text = string.Empty;
		this.staticStr.text = string.Empty;
		this.staticStr.text = Global.GetLang("获得收益");
		this.getShouYi.Text = Global.GetLang("刷新");
		DateTime correctDateTime = Global.GetCorrectDateTime();
		this.haveTimes.pivot = 3;
		this.haveTimes.transform.localPosition = new Vector3(-460f, -246f, 0f);
		this.description.pivot = 5;
		this.description.transform.localPosition = new Vector3(460f, -246f, 0f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.getShouYi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetYanHuiInfo();
		};
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
	}

	public void InitPart(string[] strInfo)
	{
		int num = Convert.ToInt32(strInfo[1]);
		num--;
		XElement gameResXml = Global.GetGameResXml("Config/GleeFeastAward.Xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "YanHui");
		int num2 = Convert.ToInt32(Global.GetXElementAttributeInt(xelementList[num], "SumNum"));
		this.haveTimes.text = string.Concat(new object[]
		{
			Global.GetLang("总可参加次数:"),
			(num2 - Convert.ToInt32(strInfo[3])).ToString(),
			"/",
			num2
		});
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[num], "ZuanShiRatio");
		this.shouYiNum.text = (Convert.ToInt32(strInfo[4]) / xelementAttributeInt).ToString();
		this.OwnZuanShi.text = Global.Data.roleData.UserMoney.ToString();
		this.OwnMoney.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		DateTime dateTime = Global.GetCorrectDateTime().AddDays(1.0);
		string[] array = Global.GetXElementAttributeStr(xelementList[num], "OverTime").Split(new char[]
		{
			':'
		});
		int[] array2 = new int[]
		{
			dateTime.Hour,
			dateTime.Minute,
			dateTime.Second
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array2[0] < Convert.ToInt32(array[i]))
			{
				dateTime = dateTime.AddDays(-1.0);
				break;
			}
		}
		this.description.text = string.Concat(new object[]
		{
			Global.GetLang("摆放时间:"),
			Global.GetLang("年"),
			dateTime.Year,
			Global.GetLang("月"),
			dateTime.Month,
			Global.GetLang("日"),
			dateTime.Day,
			"  ",
			Global.GetXElementAttributeStr(xelementList[num], "BeginTime"),
			"-",
			Global.GetXElementAttributeStr(xelementList[num], "OverTime")
		});
	}

	public UILabel haveTimes;

	public UILabel description;

	public UILabel staticStr;

	public UILabel shouYiNum;

	public UILabel OwnMoney;

	public UILabel OwnZuanShi;

	public GButton getShouYi;

	public GButton close;

	public DPSelectedItemEventHandler DPSelectedItem;
}
