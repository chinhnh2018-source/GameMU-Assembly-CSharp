using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class LoveHunYanPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.description3.transform.localPosition = new Vector3(-20f, -161f, -1f);
		this.haveTimes.text = Global.GetLang("{e3b36c}可参加次数:{-}") + "100";
		this.GetGoodWillAttr.transform.localPosition = new Vector3(-105f, -122f, -1f);
		this.staticStr.text = Global.GetLang("{fac60d}消耗金币:{-}");
		this.ReduceAttr.text = Global.GetLang("{fac60d}消耗金币:{-}");
		this.juBan.Text = Global.GetLang("举办宴会");
		this.SelectTime.Text = Global.GetLang("选择日期");
		this.JoinBtn.Text = Global.GetLang("参加宴会");
		this.CancelBtn.Text = Global.GetLang("取消");
		this.GetGoodWillAttr.text = Global.GetLang("{fac60d}获得奉献值：{-}");
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
		XElement gameResXml = Global.GetGameResXml("Config/WeddingFeasttAward.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WeddingFeasttAward");
		this.description1.text = Global.GetLang("{fac60d}宴会结束后,所有玩家在宴会中的消费会按照") + Global.GetXElementAttributeStr(xelementList[type], "GoodWillRatio") + Global.GetLang(":1的比例换算为{b266ff}[奉献度]{-},发放给夫妻双方{-}");
		this.description2.text = string.Concat(new string[]
		{
			Global.GetLang("{dac7ae}参与者可获得:经验"),
			Global.GetXElementAttributeStr(xelementList[type], "EXPAward"),
			Global.GetLang("、星魂"),
			Global.GetXElementAttributeStr(xelementList[type], "XingHunAward"),
			Global.GetLang("、声望"),
			Global.GetXElementAttributeStr(xelementList[type], "ShengWangAward"),
			"{-}"
		});
		this.xiaoHaoNum.text = Global.GetXElementAttributeInt(xelementList[type], "ConductBindJinBi").ToString();
		this.haveTimes.text = Global.GetLang("{e3b36c}可参加次数:{-}") + Global.GetXElementAttributeInt(xelementList[type], "SumNum");
		this.YanHuiID = Global.GetXElementAttributeInt(xelementList[type], "ID");
		DateTime correctDateTime = Global.GetCorrectDateTime();
		this.SelectYear = correctDateTime.Year;
		this.SelectMonth = correctDateTime.Month;
		this.SelectDay = correctDateTime.Day;
		this.description3.text = string.Concat(new object[]
		{
			"{E3B36C}",
			Global.GetLang("举办时间："),
			Global.GetLang("年"),
			correctDateTime.Year,
			Global.GetLang("月"),
			correctDateTime.Month,
			Global.GetLang("日"),
			correctDateTime.Day,
			"{-}"
		});
	}

	public void RefreshMoney()
	{
		this.OwnZuanShi.text = Global.Data.roleData.UserMoney.ToString();
		this.OwnMoney.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.OwnBindMoney.text = Global.GetRoleOwnNumByMoneyType(1).ToString();
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
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
			Super.ShowNetWaiting(string.Empty);
			DateTime dateTime;
			dateTime..ctor(this.SelectYear, this.SelectMonth, this.SelectDay);
			GameInstance.Game.ApplyHunYan(this.YanHuiID, dateTime.Ticks / 10000L);
		};
		this.CancelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowCancelConfireWindow();
		};
		this.JoinBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = 0;
			int num2 = Global.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("HunYanUseMaxNum", true));
			foreach (int num3 in Global.Data.HunYanJointTimes.Values)
			{
				num += num3;
			}
			if (this.m_totalJoinTime >= this.m_canJoinTime || this.joinNum >= this.m_everyOneTime)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("此桌今日为您准备的食物已吃完，请去其它宴会看看吧！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			Super.ShowNetWaiting(string.Empty);
			if (this.isKuaFu)
			{
				GameInstance.Game.JoinPartyForCoupleWish(this.DbCoupleId);
			}
			else
			{
				GameInstance.Game.SpriteJoinLoveHunYan(this.JuBanRoleId);
			}
		};
		this.SelectTime.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenSelectDatePart();
		};
		base.InitializeComponent();
	}

	public void SetShowType(int showType, int hunyanType = 1, MarryPartyData partyData = null)
	{
		this.ShowType = showType;
		if (this.ShowType == 5)
		{
			this.isKuaFu = true;
			GameInstance.Game.GetPartyDataForCoupleWish();
			Super.ShowNetWaiting(null);
			return;
		}
		if (partyData != null)
		{
			this.PartyData = partyData;
			if (showType == 4)
			{
				this.ShowType = 4;
			}
			else if (partyData.PartyType == -1)
			{
				this.HunYanType = 1;
				this.ShowType = 1;
			}
			else
			{
				this.HunYanType = partyData.PartyType;
				if (partyData.StartTime > Global.GetCorrectDateTime().Ticks / 10000L)
				{
					this.ShowType = 2;
				}
				else
				{
					this.ShowType = 3;
				}
			}
		}
		else
		{
			this.HunYanType = hunyanType;
		}
		if (this.ShowType == 1)
		{
			this.SetApplyUI();
		}
		else if (this.ShowType == 2)
		{
			this.SetWaitStateUI(partyData);
		}
		else if (this.ShowType == 3)
		{
			this.SetShowingStateUI(partyData);
		}
		else if (this.ShowType == 4)
		{
			this.SetJoinHunYanUI(partyData);
		}
	}

	private void SetApplyUI()
	{
		this.HunYanWait.SetActive(false);
		this.ApplyUI.SetActive(true);
		this.JoinHunyanUI.SetActive(false);
		this.SelectYanHui(this.HunYanType - 1);
		this.RefreshMoney();
	}

	private void SetWaitStateUI(MarryPartyData data)
	{
		if (data == null)
		{
			return;
		}
		this.ApplyUI.SetActive(false);
		this.HunYanWait.SetActive(true);
		this.JoinHunyanUI.SetActive(false);
		this.RefreshMoney();
		this.GetGoodWillVal.text = "0";
		DateTime dateTime;
		dateTime..ctor(data.StartTime * 10000L);
		this.TimeAttr.text = Global.GetLang("{e3b36c}" + string.Format(Global.GetLang("举办日期:{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day) + "{-}");
		this.HunYanType = data.PartyType;
		XElement gameResXml = Global.GetGameResXml("Config/WeddingFeasttAward.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WeddingFeasttAward");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[this.HunYanType - 1], "SumNum");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[this.HunYanType - 1], "GoodWillRatio");
		this.CanJoinNum.text = Global.GetLang("{e3b36c}可参加次数:{-}") + xelementAttributeInt;
		this.Destrib4.text = string.Empty;
		this.description1.text = Global.GetLang("{fac60d}宴会结束后,所有玩家在宴会中的消费会按照") + xelementAttributeInt2 + Global.GetLang(":1的比例换算为{b266ff}[奉献度]{-},发放给夫妻双方{-}");
	}

	private void SetShowingStateUI(MarryPartyData data)
	{
		if (data == null)
		{
			return;
		}
		this.ApplyUI.SetActive(false);
		this.HunYanWait.SetActive(true);
		this.JoinHunyanUI.SetActive(false);
		this.RefreshMoney();
		DateTime dateTime;
		dateTime..ctor(data.StartTime * 10000L);
		this.TimeAttr.text = Global.GetLang("{dac7ae}" + string.Format(Global.GetLang("宴会正在举办"), new object[0]) + "{-}");
		this.CancelBtn.gameObject.SetActive(false);
		this.HunYanType = data.PartyType;
		XElement gameResXml = Global.GetGameResXml("Config/WeddingFeasttAward.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WeddingFeasttAward");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[this.HunYanType - 1], "SumNum");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[this.HunYanType - 1], "GoodWillRatio");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList[this.HunYanType - 1], "BindJinBi");
		this.GetGoodWillVal.text = string.Empty + data.JoinCount * xelementAttributeInt3 / xelementAttributeInt2 / 2;
		this.CanJoinNum.text = Global.GetLang("{e3b36c}可参加次数:{-}") + (xelementAttributeInt - data.JoinCount);
		this.Destrib4.text = string.Empty;
		this.description1.text = Global.GetLang("{fac60d}宴会结束后,所有玩家在宴会中的消费会按照") + xelementAttributeInt2 + Global.GetLang(":1的比例换算为{b266ff}[奉献度]{-},发放给夫妻双方{-}");
	}

	private void SetJoinHunYanUI(CoupleWishYanHuiData Data)
	{
		this.ApplyUI.SetActive(false);
		this.HunYanWait.SetActive(false);
		this.JoinHunyanUI.SetActive(true);
		this.RefreshMoney();
		this.JuBanRoleId = Data.DbCoupleId;
		XElement gameResXml = Global.GetGameResXml("Config/WishFeasttAward.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WishFeasttAward");
		XElement xelement = xelementList[0];
		this.m_canJoinTime = Global.GetXElementAttributeInt(xelement, "SumNum");
		this.m_everyOneTime = Global.GetXElementAttributeInt(xelement, "UseNum");
		this.m_totalJoinTime = Data.TotalJoinNum;
		this.joinNum = Data.MyJoinNum;
		this.Destrib6.text = string.Concat(new string[]
		{
			Global.GetLang("{dac7ae}每参与一次可获得:经验"),
			Global.GetXElementAttributeStr(xelement, "EXPAward"),
			Global.GetLang("、星魂"),
			Global.GetXElementAttributeStr(xelement, "XingHunAward"),
			Global.GetLang("、声望"),
			Global.GetXElementAttributeStr(xelement, "ShengWangAward"),
			"{-}"
		});
		this.ReduceNum.text = Global.GetXElementAttributeStr(xelement, "BindJinBi");
		this.JoinNum.text = string.Concat(new object[]
		{
			"{e3b36c}",
			Global.GetLang("总参与次数："),
			"{-}",
			this.m_totalJoinTime,
			"/",
			this.m_canJoinTime,
			"\n{e3b36c}",
			Global.GetLang("参与次数："),
			"{-}",
			this.joinNum,
			"/",
			this.m_everyOneTime
		});
		this.description1.text = string.Empty;
	}

	public void RefLabNum()
	{
		this.joinNum++;
		this.m_totalJoinTime++;
		this.JoinNum.text = string.Concat(new object[]
		{
			"{e3b36c}",
			Global.GetLang("总参与次数："),
			"{-}",
			this.m_totalJoinTime,
			"/",
			this.m_canJoinTime,
			"\n{e3b36c}",
			Global.GetLang("参与次数："),
			"{-}",
			this.joinNum,
			"/",
			this.m_everyOneTime
		});
		this.RefreshMoney();
	}

	private void SetJoinHunYanUI(MarryPartyData data)
	{
		this.ApplyUI.SetActive(false);
		this.HunYanWait.SetActive(false);
		this.JoinHunyanUI.SetActive(true);
		this.RefreshMoney();
		this.JuBanRoleId = data.RoleID;
		XElement gameResXml = Global.GetGameResXml("Config/WeddingFeasttAward.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WeddingFeasttAward");
		int num = data.PartyType - 1;
		XElement xelement = xelementList[num];
		this.m_canJoinTime = Global.GetXElementAttributeInt(xelement, "SumNum");
		this.m_everyOneTime = Global.GetXElementAttributeInt(xelement, "UseNum");
		this.m_totalJoinTime = data.JoinCount;
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GoodWillRatio");
		this.joinNum = 0;
		if (Global.Data.HunYanJointTimes.ContainsKey(data.RoleID))
		{
			this.joinNum = Global.Data.HunYanJointTimes[data.RoleID];
		}
		this.Destrib6.text = string.Concat(new string[]
		{
			Global.GetLang("{dac7ae}参与者可获得:经验"),
			Global.GetXElementAttributeStr(xelement, "EXPAward"),
			Global.GetLang("、星魂"),
			Global.GetXElementAttributeStr(xelement, "XingHunAward"),
			Global.GetLang("、声望"),
			Global.GetXElementAttributeStr(xelement, "ShengWangAward"),
			"{-}"
		});
		this.ReduceNum.text = Global.GetXElementAttributeStr(xelementList[data.PartyType - 1], "BindJinBi");
		this.JoinNum.text = string.Concat(new object[]
		{
			"{e3b36c}",
			Global.GetLang("总参与次数："),
			"{-}",
			this.m_totalJoinTime,
			"/",
			this.m_canJoinTime,
			"\n{e3b36c}",
			Global.GetLang("参与次数："),
			"{-}",
			this.joinNum,
			"/",
			this.m_everyOneTime
		});
		this.description1.text = Global.GetLang("{fac60d}宴会结束后,所有玩家在宴会中的消费会按照") + xelementAttributeInt + Global.GetLang(":1的比例换算为{b266ff}[奉献度]{-},发放给夫妻双方{-}");
	}

	public void RefreshJoinHunYanUI()
	{
		if (this.PartyData != null && Global.Data.HunYanListDatas.ContainsKey(this.PartyData.RoleID))
		{
			this.SetJoinHunYanUI(Global.Data.HunYanListDatas[this.PartyData.RoleID]);
		}
	}

	public void RefreshJoinHunYanUI(CoupleWishYanHuiData Data)
	{
		if (Data != null)
		{
			this.SetJoinHunYanUI(Data);
			this.DbCoupleId = Data.DbCoupleId;
		}
	}

	private void ShowCancelConfireWindow()
	{
		string lang = Global.GetLang("取消婚姻会返还举办费用，确定要取消婚姻？");
		string[] buttons = new string[]
		{
			Global.GetLang("确定")
		};
		Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
		{
			if (e1.ID == 0)
			{
				Super.ShowNetWaiting(string.Empty);
				GameInstance.Game.CancelHunYan();
			}
		}, buttons);
	}

	public void SetDateTime(int year, int month, int day)
	{
		this.SelectYear = year;
		this.SelectMonth = month;
		this.SelectDay = day;
		this.description3.text = string.Concat(new object[]
		{
			"{E3B36C}",
			Global.GetLang("举办时间："),
			year,
			Global.GetLang("年"),
			month,
			Global.GetLang("月"),
			day,
			Global.GetLang("日"),
			"{-}"
		});
	}

	public GameObject ApplyUI;

	public UILabel haveTimes;

	public UILabel description1;

	public UILabel description2;

	public UILabel description3;

	public UILabel staticStr;

	public UILabel xiaoHaoNum;

	public UILabel OwnMoney;

	public UILabel OwnBindMoney;

	public UILabel OwnZuanShi;

	public GCheckBox puTongYanHui;

	public GCheckBox fengShengYanHui;

	public GCheckBox HaoHuaYanHui;

	public GButton juBan;

	public GButton close;

	public GButton SelectTime;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int YanHuiID;

	public GameObject HunYanWait;

	public UILabel GetGoodWillAttr;

	public UILabel GetGoodWillVal;

	public UILabel TimeAttr;

	public UILabel CanJoinNum;

	public UILabel Destrib4;

	public GButton CancelBtn;

	private int SelectYear;

	private int SelectMonth;

	private int SelectDay;

	public GameObject JoinHunyanUI;

	public UILabel ReduceNum;

	public UILabel ReduceAttr;

	public UILabel Destrib6;

	public UILabel JoinNum;

	public GButton JoinBtn;

	private int HunYanType = 1;

	public int ShowType;

	private MarryPartyData PartyData;

	private int m_canJoinTime;

	private int m_totalJoinTime;

	private int m_everyOneTime;

	private int joinNum;

	private bool isKuaFu;

	private int DbCoupleId;

	private int JuBanRoleId;
}
