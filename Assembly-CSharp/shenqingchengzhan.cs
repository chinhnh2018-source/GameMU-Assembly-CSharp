using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class shenqingchengzhan : UserControl
{
	private void OnEnable()
	{
	}

	private void InitTextInPrefabs()
	{
		this.labelStatic1.text = Global.GetLang("当前战盟");
		this.labelStatic2.text = Global.GetLang("当前出价");
		this.labelStatic3.text = Global.GetLang("竞标价格");
		this.labelStatic4.text = Global.GetLang("开始竞标");
		this.labelStatic5.text = Global.GetLang("活动时间");
		if (Context.IsHaiwai)
		{
			this.staticText.text = Global.GetLang("战盟资金:");
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ConfigInit();
		this.InitTextInPrefabs();
		this.InitPromptInfo();
		this.bidItemCollection = this.bidListBox.ItemsSource;
		this.InitListboxDefault();
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.GetBangHuiLingDiItemData();
	}

	private void ConfigInit()
	{
		XElement gameResXml = Global.GetGameResXml("Config/SiegeWarfare.Xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		shenqingchengzhan.cfgOnceMoney = Global.GetXElementAttributeInt(xelementList[0], "BidZhangMengZiJin");
		shenqingchengzhan.cfgInitMoney = Global.GetXElementAttributeInt(xelementList[0], "ApplyZhangMengZiJin");
		shenqingchengzhan.cfgKaifuDay = Global.GetXElementAttributeStr(xelementList[0], "WeekPoints").Split(new char[]
		{
			'|'
		});
		shenqingchengzhan.cfgKaifuTime = Global.GetXElementAttributeStr(xelementList[0], "TimePoints");
		shenqingchengzhan.cfgBidStop = Global.GetXElementAttributeInt(xelementList[0], "EnrollTime");
		shenqingchengzhan.cfgFightingSecs = Global.GetXElementAttributeInt(xelementList[0], "FightingSecs");
		shenqingchengzhan.cfgClearRolesSecs = Global.GetXElementAttributeInt(xelementList[0], "ClearRolesSecs");
	}

	private void InitPromptInfo()
	{
		this.onceMoney.text = Global.GetLang("每次竞价、抬价消耗战盟资金") + shenqingchengzhan.cfgOnceMoney.ToString() + Global.GetLang("金币");
		shenqingchengzhan.realKaifuTime = this.GetKaifuDatetime();
		this.nextTime.text = Global.GetLang("下次活动时间：") + shenqingchengzhan.realKaifuTime;
		this.jingjia1.isEnabled = true;
		this.jingjia2.isEnabled = true;
		this.jingjia3.isEnabled = true;
		base.StartCoroutine<bool>(this.TimeProc());
	}

	private string GetKaifuDatetime()
	{
		string text = string.Empty;
		int num = Convert.ToInt32(Global.GetCorrectDateTime().DayOfWeek);
		if (num == 0)
		{
			num = 7;
		}
		bool[] array = new bool[7];
		for (int i = 0; i < shenqingchengzhan.cfgKaifuDay.Length; i++)
		{
			array[Convert.ToInt32(shenqingchengzhan.cfgKaifuDay[i]) - 1] = true;
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		string text2 = correctDateTime.ToShortTimeString();
		string text3 = correctDateTime.ToShortDateString();
		DateTime dateTime = Convert.ToDateTime(text3 + " " + shenqingchengzhan.cfgKaifuTime);
		if (array[num - 1] && correctDateTime > dateTime)
		{
			array[num - 1] = false;
		}
		if (array[num - 1])
		{
			text = text3 + " " + shenqingchengzhan.cfgKaifuTime;
		}
		else
		{
			int num2 = num - 1;
			int num3 = 0;
			while (!array[num2++])
			{
				num2 %= 7;
				num3++;
			}
			text3 = correctDateTime.AddDays((double)num3).ToShortDateString();
			text = text3 + " " + shenqingchengzhan.cfgKaifuTime;
		}
		return Convert.ToDateTime(text).ToString("yyyy-MM-dd HH:mm:ss");
	}

	private void InitListboxDefault()
	{
		for (int i = 0; i < 3; i++)
		{
			shenqingchengzhanItem shenqingchengzhanItem = U3DUtils.NEW<shenqingchengzhanItem>();
			this.bidItemCollection.AddNoUpdate(shenqingchengzhanItem);
			shenqingchengzhanItem.Site = i + 1;
			shenqingchengzhanItem.CurrentMoney = string.Empty + shenqingchengzhan.cfgInitMoney;
			shenqingchengzhanItem.NeedMoney = string.Empty + (shenqingchengzhan.cfgInitMoney + shenqingchengzhan.cfgOnceMoney);
			shenqingchengzhanItem.FamilyName = Global.GetLang("暂无竞价战盟");
			if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				shenqingchengzhanItem.RefreshBtnStatus(1);
			}
			else
			{
				shenqingchengzhanItem.RefreshBtnStatus(0);
			}
			UIPanel component = shenqingchengzhanItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void DataRefresh(List<LuoLanChengZhanRequestInfoEx> luolanRequestInfo)
	{
		if (luolanRequestInfo != null)
		{
			foreach (LuoLanChengZhanRequestInfoEx luoLanChengZhanRequestInfoEx in luolanRequestInfo)
			{
				shenqingchengzhanItem shenqingchengzhanItem = U3DUtils.AS<shenqingchengzhanItem>(this.bidItemCollection[luoLanChengZhanRequestInfoEx.Site - 1]);
				if (luoLanChengZhanRequestInfoEx.BHID > 0 && luoLanChengZhanRequestInfoEx.BidMoney > 0)
				{
					shenqingchengzhanItem.CurrentMoney = string.Empty + luoLanChengZhanRequestInfoEx.BidMoney;
					shenqingchengzhanItem.NeedMoney = string.Empty + (luoLanChengZhanRequestInfoEx.BidMoney + shenqingchengzhan.cfgOnceMoney);
					shenqingchengzhanItem.FamilyName = luoLanChengZhanRequestInfoEx.BHName;
					if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
					{
						if (Global.Data.roleData.Faction == luoLanChengZhanRequestInfoEx.BHID)
						{
							shenqingchengzhanItem.RefreshBtnStatus(2);
						}
						else
						{
							shenqingchengzhanItem.RefreshBtnStatus(1);
						}
					}
					else
					{
						shenqingchengzhanItem.RefreshBtnStatus(0);
					}
				}
			}
		}
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			DateTime kaifu = Convert.ToDateTime(shenqingchengzhan.realKaifuTime);
			DateTime currentDatetime = Global.GetCorrectDateTime();
			if (currentDatetime < kaifu.AddSeconds((double)(-(double)shenqingchengzhan.cfgBidStop)))
			{
				this.liftTime.text = Global.GetLang("竞价结束倒计时：") + UIHelper.FormatSecs((long)((int)(kaifu - currentDatetime).TotalSeconds - shenqingchengzhan.cfgBidStop), "-");
			}
			else
			{
				this.liftTime.text = Global.GetLang("竞价已结束");
				this.jingjia1.isEnabled = false;
				this.jingjia2.isEnabled = false;
				this.jingjia3.isEnabled = false;
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	public void SetZhanmengInfo(int ZhanmengId, int zhanmengzijin, string familyName, int familyId)
	{
		if (Global.Data.roleData.Faction == ZhanmengId)
		{
			this.zhanMengZiJin.text = string.Empty + zhanmengzijin;
		}
		this.zhanLingGongHui.text = ((familyName != null && !(familyName == string.Empty)) ? (Global.GetLang("占领战盟：") + familyName) : Global.GetLang("占领战盟：无战盟占领"));
		for (int i = 0; i < this.bidListBox.Length(); i++)
		{
			shenqingchengzhanItem shenqingchengzhanItem = U3DUtils.AS<shenqingchengzhanItem>(this.bidItemCollection[i]);
			int faction = Global.Data.roleData.Faction;
			shenqingchengzhanItem.btnJingjia.isEnabled = (familyId != faction);
			shenqingchengzhanItem.btnTaijia.isEnabled = (familyId != faction);
		}
	}

	public void RefreshBidResult(int result)
	{
		if (result < 0)
		{
			if (result == -4)
			{
				string lang = Global.GetLang("数据无效或已过期, 请在关闭窗口后重试！");
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
				{
					GameInstance.Game.GetBangHuiLingDiItemData();
				}, new string[]
				{
					Global.GetLang("确定")
				});
			}
			else if (result == -9)
			{
				string lang2 = Global.GetLang("战盟资金不足（战盟资金扣除后不能低于20000金币）！");
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang2, delegate(object s, DPSelectedItemEventArgs e)
				{
				}, new string[]
				{
					Global.GetLang("确定")
				});
			}
			else
			{
				string message = StdErrorCode.GetErrMsg(result, false, false);
				if (result == -2001)
				{
					message = Global.GetLang("竞价时间已结束！");
				}
				Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s, DPSelectedItemEventArgs e)
				{
				}, new string[]
				{
					Global.GetLang("确定")
				});
			}
		}
	}

	public UILabel staticText;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton close;

	public GButton jingjia1;

	public GButton jingjia2;

	public GButton jingjia3;

	public UILabel zhanLingGongHui;

	public UILabel zhanMengZiJin;

	public UILabel OwnMoney;

	public UILabel labelStatic1;

	public UILabel labelStatic2;

	public UILabel labelStatic3;

	public UILabel labelStatic4;

	public UILabel labelStatic5;

	public UILabel nextTime;

	public UILabel liftTime;

	public UILabel onceMoney;

	public UILabel[] zhanMengArr = new UILabel[3];

	public UILabel[] CurJiaGeArr = new UILabel[3];

	public UILabel[] NextJiaGeArr = new UILabel[3];

	public ListBox bidListBox;

	private ObservableCollection bidItemCollection;

	private static int cfgOnceMoney;

	private static int cfgInitMoney;

	private static string[] cfgKaifuDay;

	private static string cfgKaifuTime;

	private static int cfgBidStop;

	private static int cfgFightingSecs;

	private static int cfgClearRolesSecs;

	private static string realKaifuTime;
}
