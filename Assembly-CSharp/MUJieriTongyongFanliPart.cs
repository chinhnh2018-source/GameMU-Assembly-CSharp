using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriTongyongFanliPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.rewardList.ItemsSource;
		this.labNum.gameObject.SetActive(false);
	}

	public void InitData(string strXML)
	{
		this.ItemCollection.Clear();
		XElement xelement = XElement.Parse(strXML);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		XElement xelement2 = xelementList[0];
		if (xelement2 == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ActivityType");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间: "),
			"ffffff",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("领取时间: "),
			"ffffff",
			this.awardStartStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.awardEndStr
		});
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "GiftList");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容: "),
			"ffffff",
			xelementAttributeStr5
		});
		List<XElement> xelementList3 = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList3.Count; i++)
		{
			XElement xelement4 = xelementList3[i];
			if (xelement4 == null)
			{
				return;
			}
			MUJieriTongyongFanliItem mujieriTongyongFanliItem = U3DUtils.NEW<MUJieriTongyongFanliItem>();
			this.ItemCollection.Add(mujieriTongyongFanliItem);
			this.listItem.Add(mujieriTongyongFanliItem);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "ID");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, this.GetXMLNode());
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			string xelementAttributeStr9 = Global.GetXElementAttributeStr(xelement4, "GoodsThr");
			string xelementAttributeStr10 = Global.GetXElementAttributeStr(xelement4, "EffectiveTime");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr8))
			{
				goodsIDs = xelementAttributeStr7 + "@" + xelementAttributeStr8;
			}
			else
			{
				goodsIDs = xelementAttributeStr7;
			}
			mujieriTongyongFanliItem.Id = xelementAttributeInt2;
			mujieriTongyongFanliItem.Need1 = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				this.GetDesc(xelementAttributeStr6)
			});
			string[] array = xelementAttributeStr6.Split(new char[]
			{
				','
			});
			mujieriTongyongFanliItem.Need = array[0].SafeToInt32(0);
			mujieriTongyongFanliItem.NeedExt = ((array.Length != 2) ? 0 : array[1].SafeToInt32(0));
			mujieriTongyongFanliItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			mujieriTongyongFanliItem.ActivityTypes = this.m_ActivityTypes;
			Super.LoadGoodsList(goodsIDs, mujieriTongyongFanliItem.ItemCollection);
			Super.LoadOtherGoodsList(xelementAttributeStr9, mujieriTongyongFanliItem.ItemCollection, xelementAttributeStr10);
			mujieriTongyongFanliItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.DPSelectedItem(s, e);
			};
			UIPanel component = mujieriTongyongFanliItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void SetActivityType(int index)
	{
		switch (index)
		{
		case 16:
			this.m_ActivityTypes = ActivityTypes.JieriWing;
			this.SetBak("mujierihuodong_chibang");
			break;
		case 17:
			this.m_ActivityTypes = ActivityTypes.JieriAddon;
			this.SetBak("mujierihuodong_zhuijia");
			break;
		case 18:
			this.m_ActivityTypes = ActivityTypes.JieriStrengthen;
			this.SetBak("mujierihuodong_qianghua");
			break;
		case 19:
			this.m_ActivityTypes = ActivityTypes.JieriAchievement;
			this.SetBak("mujierihuodong_chengjiu");
			break;
		case 20:
			this.m_ActivityTypes = ActivityTypes.JieriMilitaryRank;
			this.SetBak("mujierihuodong_junxian");
			break;
		case 21:
			this.m_ActivityTypes = ActivityTypes.JieriVIPFanli;
			this.SetBak("mujierihuodong_vip");
			break;
		case 22:
			this.m_ActivityTypes = ActivityTypes.JieriAmulet;
			this.SetBak("mujierihuodong_hushenfu");
			break;
		case 23:
			this.m_ActivityTypes = ActivityTypes.JieriArchangel;
			this.SetBak("mujierihuodong_datianshi");
			break;
		case 24:
			this.m_ActivityTypes = ActivityTypes.JieriShouli;
			this.SetBak("mujierihuodong_jierishouli");
			this.labNum.gameObject.SetActive(true);
			this.labNum.gameObject.transform.localPosition = new Vector3(275f, 28f, -1f);
			break;
		case 25:
			this.m_ActivityTypes = ActivityTypes.JieriJiehun;
			this.SetBak("mujierihuodong_jiehun");
			break;
		case 38:
			this.m_ActivityTypes = ActivityTypes.JieRiHuiJiFanLi;
			this.SetBak("mujierihuodong_jiehun");
			break;
		case 39:
			this.m_ActivityTypes = ActivityTypes.JieRiFuWenFanLi;
			this.SetBak("mujierihuodong_jiehun");
			break;
		}
	}

	public void GetDataInfo()
	{
		Super.ShowNetWaiting(null);
		if (this.m_ActivityTypes == ActivityTypes.JieriShouli)
		{
			GameInstance.Game.GetJieriShouliInfoCmd();
		}
		else if (this.m_ActivityTypes >= ActivityTypes.JieriWing && this.m_ActivityTypes <= ActivityTypes.JieriArchangel)
		{
			GameInstance.Game.GetJieriTongyongInfoCmd((int)this.m_ActivityTypes);
		}
		else if (this.m_ActivityTypes == ActivityTypes.JieriJiehun)
		{
			GameInstance.Game.GetJieriTongyongInfoCmd(62);
		}
		else if (this.m_ActivityTypes == ActivityTypes.JieRiHuiJiFanLi)
		{
			GameInstance.Game.GetJieriTongyongInfoCmd((int)this.m_ActivityTypes);
		}
		else if (this.m_ActivityTypes == ActivityTypes.JieRiFuWenFanLi)
		{
			GameInstance.Game.GetShenShiMainData();
			GameInstance.Game.GetJieriTongyongInfoCmd((int)this.m_ActivityTypes);
		}
	}

	public void SetBak(string bakName)
	{
		this.bak.ImageURL = string.Format("NetImages/GameRes/Images/Plate/{0}.jpg", bakName);
		this.bak.gameObject.SetActive(false);
		this.bak.gameObject.SetActive(true);
	}

	public ActivityTypes ActivityTypes
	{
		get
		{
			return this.m_ActivityTypes;
		}
		set
		{
			this.m_ActivityTypes = value;
		}
	}

	private string GetXMLNode()
	{
		string result = string.Empty;
		switch (this.m_ActivityTypes)
		{
		case ActivityTypes.JieriWing:
			result = "WingLevel";
			break;
		case ActivityTypes.JieriAddon:
			result = "ZhuiJiaLevel";
			break;
		case ActivityTypes.JieriStrengthen:
			result = "QiangHuaLevel";
			break;
		case ActivityTypes.JieriAchievement:
			result = "ChengJiuLevel";
			break;
		case ActivityTypes.JieriMilitaryRank:
			result = "JunXianLevel";
			break;
		case ActivityTypes.JieriVIPFanli:
			result = "VIPLevel";
			break;
		case ActivityTypes.JieriAmulet:
			result = "HuShenFuLevel";
			break;
		case ActivityTypes.JieriArchangel:
			result = "DaTianShiLevel";
			break;
		case ActivityTypes.JieriShouli:
			result = "Num";
			break;
		case ActivityTypes.JieriJiehun:
			result = "GoodWillSuit";
			break;
		case ActivityTypes.JieRiHuiJiFanLi:
			result = "EmblemLevel";
			break;
		case ActivityTypes.JieRiFuWenFanLi:
			result = "FuWenLevel";
			break;
		}
		return result;
	}

	private string GetDesc(string values)
	{
		string result = string.Empty;
		switch (this.m_ActivityTypes)
		{
		case ActivityTypes.JieriWing:
		{
			string[] array = values.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				result = string.Format(Global.GetLang("需要翅膀达到{0}阶{1}级"), array[0], array[1]);
			}
			break;
		}
		case ActivityTypes.JieriAddon:
			result = string.Format(Global.GetLang("追加总等级达到{0}级"), values);
			break;
		case ActivityTypes.JieriStrengthen:
			result = string.Format(Global.GetLang("强化总等级达到{0}级"), values);
			break;
		case ActivityTypes.JieriAchievement:
			result = string.Format(Global.GetLang("获得成就{0}称号"), this.GetChengjiuName(Global.SafeConvertToInt32(values)));
			break;
		case ActivityTypes.JieriMilitaryRank:
			result = string.Format(Global.GetLang("获得军衔{0}称号"), this.GetJunxianName(Global.SafeConvertToInt32(values)));
			break;
		case ActivityTypes.JieriVIPFanli:
			result = string.Format(Global.GetLang("需要VIP{0}级"), values);
			break;
		case ActivityTypes.JieriAmulet:
			result = string.Format(Global.GetLang("需要{0}阶护身符"), values);
			break;
		case ActivityTypes.JieriArchangel:
			result = string.Format(Global.GetLang("佩戴{0}阶大天使武器"), values);
			break;
		case ActivityTypes.JieriShouli:
			result = string.Format(Global.GetLang("收取{0}个礼物"), values);
			break;
		case ActivityTypes.JieriJiehun:
			if (this.isJiehun())
			{
				result = string.Format(Global.GetLang("奉献度达到{0}阶"), values);
			}
			else
			{
				result = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("需要角色已婚")
				});
			}
			break;
		case ActivityTypes.JieRiHuiJiFanLi:
		{
			string[] array2 = values.Split(new char[]
			{
				','
			});
			if (array2.Length == 2)
			{
				result = string.Format(Global.GetLang("徽记达到{0}阶{1}星"), array2[0], array2[1]);
			}
			break;
		}
		case ActivityTypes.JieRiFuWenFanLi:
			result = string.Format(Global.GetLang("符文总等级达到{0}级"), values);
			break;
		}
		return result;
	}

	private string GetChengjiuName(int chengjiuLevel)
	{
		string result = string.Empty;
		switch (chengjiuLevel)
		{
		case 1:
			result = Global.GetLang("守护者");
			break;
		case 2:
			result = Global.GetLang("先锋者");
			break;
		case 3:
			result = Global.GetLang("无畏者");
			break;
		case 4:
			result = Global.GetLang("讨伐者");
			break;
		case 5:
			result = Global.GetLang("不败者");
			break;
		case 6:
			result = Global.GetLang("至高者");
			break;
		case 7:
			result = Global.GetLang("屠戮者");
			break;
		case 8:
			result = Global.GetLang("终结者");
			break;
		case 9:
			result = Global.GetLang("毁灭者");
			break;
		case 10:
			result = Global.GetLang("征服者");
			break;
		case 11:
			result = Global.GetLang("统治者");
			break;
		case 12:
			result = Global.GetLang("救世主");
			break;
		case 13:
			result = Global.GetLang("破军者");
			break;
		case 14:
			result = Global.GetLang("歼灭者");
			break;
		case 15:
			result = Global.GetLang("不灭者");
			break;
		case 16:
			result = Global.GetLang("创世者");
			break;
		}
		return result;
	}

	private string GetJunxianName(int chengjiuLevel)
	{
		string result = string.Empty;
		switch (chengjiuLevel)
		{
		case 1:
			result = Global.GetLang("列兵");
			break;
		case 2:
			result = Global.GetLang("下士");
			break;
		case 3:
			result = Global.GetLang("中士");
			break;
		case 4:
			result = Global.GetLang("军士");
			break;
		case 5:
			result = Global.GetLang("骑士");
			break;
		case 6:
			result = Global.GetLang("中尉");
			break;
		case 7:
			result = Global.GetLang("少校");
			break;
		case 8:
			result = Global.GetLang("中将");
			break;
		case 9:
			result = Global.GetLang("司令");
			break;
		case 10:
			result = Global.GetLang("统帅");
			break;
		case 11:
			result = Global.GetLang("督军");
			break;
		case 12:
			result = Global.GetLang("元帅");
			break;
		case 13:
			result = Global.GetLang("上将");
			break;
		case 14:
			result = Global.GetLang("公爵");
			break;
		case 15:
			result = Global.GetLang("大公");
			break;
		case 16:
			result = Global.GetLang("亲王");
			break;
		}
		return result;
	}

	private bool isJiehun()
	{
		return GameInstance.Game.CurrentSession != null && GameInstance.Game.CurrentSession.MarriageData != null && (int)GameInstance.Game.CurrentSession.MarriageData.byMarrytype != -1;
	}

	public void SetBtnState(int activity, int flag, int shouliNum = 0)
	{
		if (activity != (int)this.m_ActivityTypes)
		{
			return;
		}
		Super.HideNetWaiting();
		int count = this.listItem.Count;
		int num = 0;
		bool flag2 = false;
		int num2 = -1;
		int num3 = 1;
		int num4 = 1;
		bool flag3 = true;
		int num5 = 0;
		switch (activity)
		{
		case 53:
			num2 = Global.Data.roleData.MyWingData.WingID;
			num3 = Global.Data.roleData.MyWingData.ForgeLevel;
			num4 = Global.Data.roleData.MyWingData.Using;
			break;
		case 54:
			num2 = Global.GetSumOfAllEquipZhuijiaValue();
			break;
		case 55:
			num2 = Global.GetSumOfAllEquipQianghuaValue();
			break;
		case 56:
			num2 = Global.GetRoleCommonUseParamsValue(22);
			break;
		case 57:
			num2 = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
			break;
		case 58:
			num2 = Global.GetVIPLeve();
			break;
		case 59:
			num2 = Global.GetHuShengFuLevel();
			break;
		case 60:
			num2 = Global.GetMaxDaTianShiLevel();
			break;
		case 61:
			num = 1;
			num2 = shouliNum;
			this.labNum.text = string.Format(Global.GetLang("已收取数量: {0}"), shouliNum);
			break;
		case 62:
			flag3 = this.isJiehun();
			num2 = (int)Global.Data.MarryData.byGoodwilllevel;
			break;
		case 75:
			if (Global.Data.roleData != null && Global.Data.roleData.HuiJiData != null)
			{
				int huiji = Global.Data.roleData.HuiJiData.huiji;
				Dictionary<int, EmblemStarXml> dictionary = Global.AddDicEmblemStart();
				if (dictionary != null && dictionary.ContainsKey(huiji))
				{
					num2 = dictionary[huiji].EmblemLevel;
					num3 = dictionary[huiji].EmblemStar;
				}
			}
			break;
		case 76:
		{
			Dictionary<int, FuWen> dicFuWen = ShenShiPart.GetDicFuWen();
			if (dicFuWen != null && dicFuWen.Count > 0)
			{
				if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.FuWenTabList != null && Global.Data.roleData.FuWenTabList.Count > 0)
				{
					FuWenTabData fuWenTabData = null;
					if (Global.Data.FuWenFanLiMainData != null)
					{
						int fuWenTabId = Global.Data.FuWenFanLiMainData.FuWenTabId;
						if (fuWenTabId <= Global.Data.roleData.FuWenTabList.Count - 1)
						{
							fuWenTabData = Global.Data.roleData.FuWenTabList[fuWenTabId];
						}
					}
					Dictionary<int, FuWen>.Enumerator itr = dicFuWen.GetEnumerator();
					if (fuWenTabData != null)
					{
						while (itr.MoveNext())
						{
							List<int> list = fuWenTabData.FuWenEquipList.FindAll(delegate(int result)
							{
								KeyValuePair<int, FuWen> keyValuePair = itr.Current;
								return result == keyValuePair.Key;
							});
							if (list != null && list.Count > 0)
							{
								for (int i = 0; i < list.Count; i++)
								{
									int num6 = list[i];
									if (dicFuWen.ContainsKey(num6))
									{
										num5 += dicFuWen[num6].Level;
									}
								}
							}
						}
					}
				}
				num2 = num5;
			}
			break;
		}
		}
		for (int j = 0; j < count; j++)
		{
			MUJieriTongyongFanliItem mujieriTongyongFanliItem = this.listItem[j];
			if (Global.GetIntSomeBit(flag, j + num) == 0)
			{
				if (num2 == mujieriTongyongFanliItem.Need)
				{
					flag2 = (num3 >= mujieriTongyongFanliItem.NeedExt);
				}
				else if (num2 > mujieriTongyongFanliItem.Need)
				{
					flag2 = true;
				}
				else if (num2 < mujieriTongyongFanliItem.Need)
				{
					flag2 = false;
				}
				if (flag2 && num4 == 1 && flag3)
				{
					mujieriTongyongFanliItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			else
			{
				mujieriTongyongFanliItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			}
		}
	}

	public void SetCompletedInfo(int result, int position)
	{
		if (result < 0)
		{
			return;
		}
		MUJieriTongyongFanliItem mujieriTongyongFanliItem = this.listItem[position - 1];
		mujieriTongyongFanliItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
	}

	public void SetLingquResult(int result, int index)
	{
		switch (result)
		{
		case 0:
		{
			MUJieriTongyongFanliItem mujieriTongyongFanliItem = this.listItem[index - 1];
			mujieriTongyongFanliItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			Super.HintMainText(Global.GetLang("领取成功"), 10, 3);
			break;
		}
		case 1:
			Super.HintMainText(Global.GetLang("活动未开启"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("不是领奖时间"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("数据库出错"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("服务器配置出错"), 10, 3);
			break;
		case 9:
			Super.HintMainText(Global.GetLang("背包不足"), 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("不满足领奖条件"), 10, 3);
			break;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public TextBlock descText;

	public ListBox rewardList;

	public ShowNetImage bak;

	public TextBlock labNum;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private List<MUJieriTongyongFanliItem> listItem = new List<MUJieriTongyongFanliItem>();

	private ObservableCollection _ItemCollection;

	private ActivityTypes m_ActivityTypes = ActivityTypes.JieriWing;
}
