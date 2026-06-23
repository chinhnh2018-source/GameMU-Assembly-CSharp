using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class UIHelper
{
	public static string FormatLevelLimit(int minLevel, int maxLevel, int minZhuanSheng, int maxZhuanSheng)
	{
		string text = Global.GetLang("不限");
		if (minLevel != -1 || maxLevel != -1 || minZhuanSheng != -1 || maxZhuanSheng != -1)
		{
			if (minLevel <= 0 && minZhuanSheng <= 0)
			{
				if (maxZhuanSheng == -1)
				{
					text = string.Format(Global.GetLang("最高{0}级"), maxLevel);
				}
				else
				{
					text = string.Format(Global.GetLang("最高{0}转{1}级"), maxZhuanSheng, maxLevel);
				}
			}
			else if (maxLevel == -1 && maxZhuanSheng == -1)
			{
				if (minZhuanSheng <= 0)
				{
					text = string.Format(Global.GetLang("最低{0}级"), minLevel);
				}
				else
				{
					text = string.Format(Global.GetLang("最低{0}转{1}级"), minZhuanSheng, minLevel);
				}
			}
			else
			{
				text = ((minZhuanSheng < 0) ? string.Empty : (minZhuanSheng.ToString() + Global.GetLang("转")));
				text += ((minLevel < 0) ? string.Empty : (minLevel.ToString() + Global.GetLang("级")));
				text += " - ";
				text += ((maxZhuanSheng < 0) ? string.Empty : (maxZhuanSheng.ToString() + Global.GetLang("转")));
				text += ((maxLevel < 0) ? string.Empty : (maxLevel.ToString() + Global.GetLang("级")));
			}
		}
		return text;
	}

	public static string FormatLevelLimit(int minLevel, int minZhuanSheng)
	{
		if (minZhuanSheng >= 0)
		{
			return string.Format(Global.GetLang("{0}转{1}级"), minZhuanSheng, minLevel);
		}
		return string.Format(Global.GetLang("{0}级"), minLevel);
	}

	public static string FormatLevelLimit2(int minZhuanSheng, int maxZhuanSheng)
	{
		string result = Global.GetLang("不限");
		if (minZhuanSheng != -1 || maxZhuanSheng != -1)
		{
			if (minZhuanSheng <= 0)
			{
				if (maxZhuanSheng >= 0)
				{
					result = string.Format(Global.GetLang("最高{0}转"), maxZhuanSheng);
				}
			}
			else if (maxZhuanSheng == -1)
			{
				if (minZhuanSheng >= 0)
				{
					result = string.Format(Global.GetLang("最低{0}转"), minZhuanSheng);
				}
			}
			else
			{
				result = string.Format(Global.GetLang("{0}转 - {1}转"), minZhuanSheng, maxZhuanSheng);
			}
		}
		return result;
	}

	public static int GetUnionZhuanShengLevel(int level, int zhuanSheng)
	{
		return level + zhuanSheng * 65536;
	}

	public static bool AvalidLevel(int MinLevel, int minZhuanSheng, bool strict = false)
	{
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return false;
		}
		if (minZhuanSheng < 0)
		{
			minZhuanSheng = 0;
		}
		if (strict)
		{
			if (Global.Data.roleData.Level < MinLevel || Global.Data.roleData.ChangeLifeCount < minZhuanSheng)
			{
				return false;
			}
		}
		else
		{
			if (Global.Data.roleData.ChangeLifeCount < minZhuanSheng)
			{
				return false;
			}
			if (Global.Data.roleData.ChangeLifeCount == minZhuanSheng && Global.Data.roleData.Level < MinLevel)
			{
				return false;
			}
		}
		return true;
	}

	public static int AvalidLevel(int minLevel, int maxLevel, int minZhuanSheng, int maxZhuanSheng)
	{
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return -1;
		}
		minLevel = ((minLevel != -1) ? minLevel : 0);
		maxLevel = ((maxLevel != -1) ? maxLevel : 4095);
		minZhuanSheng = ((minZhuanSheng != -1) ? minZhuanSheng : 0);
		maxZhuanSheng = ((maxZhuanSheng != -1) ? maxZhuanSheng : 4095);
		int num = minLevel + minZhuanSheng * 65536;
		int num2 = maxLevel + maxZhuanSheng * 65536;
		int num3 = Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 65536;
		if (num <= num3 && num3 <= num2)
		{
			return 0;
		}
		if (num3 < num)
		{
			return -1;
		}
		return 1;
	}

	public static string FormatGoodsName(GoodsData goodsData, bool isShowCount = false, bool isShowAllName = false, bool isJuHun = false)
	{
		string text = string.Empty;
		string text2 = "FFFFFF";
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return string.Empty;
		}
		int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
		if (goodsCatetoriy == 10 || goodsCatetoriy == 9)
		{
			if (goodsData.ExcellenceInfo != 0)
			{
				text2 = "FF08FF";
			}
			else if (goodsXmlNodeByID.SuitID == 1)
			{
				text2 = "0099FF";
			}
			else
			{
				text2 = "FF08FF";
			}
		}
		else if (goodsXmlNodeByID.Categoriy == 980)
		{
			int suitID = goodsXmlNodeByID.SuitID;
			if (suitID <= 3)
			{
				text2 = "00FF00";
			}
			else if (suitID == 4)
			{
				text2 = "0099FF";
			}
			else if (suitID > 4 && suitID <= 6)
			{
				text2 = "ff08ff";
			}
		}
		else if ((goodsCatetoriy >= 0 && goodsCatetoriy < 25) || (goodsCatetoriy >= 30 && goodsCatetoriy <= 38) || Global.IsRebornEquip(goodsCatetoriy) || goodsCatetoriy == 980)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount > 0)
			{
				if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
				{
					text2 = "00ff00";
					text += UIHelper.ZuoyueTitleNames[0];
				}
				else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
				{
					text2 = "0099ff";
					text += UIHelper.ZuoyueTitleNames[1];
				}
				else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
				{
					text2 = "ff08ff";
					text += UIHelper.ZuoyueTitleNames[2];
				}
			}
			else if (goodsData.Lucky > 0)
			{
				text2 = "0099FF";
			}
			else if (goodsData.Forge_level == 3 || goodsData.Forge_level == 4)
			{
				text2 = "FF9900";
			}
			else if (goodsData.Forge_level == 5 || goodsData.Forge_level == 6)
			{
				text2 = "0099FF";
			}
			else if (goodsData.Forge_level >= 7)
			{
				text2 = "FFFF00";
			}
		}
		else if (40 <= goodsCatetoriy && 45 >= goodsCatetoriy)
		{
			int zhuoyueAttributeCount2 = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount2 > 0 && zhuoyueAttributeCount2 <= 2)
			{
				text2 = "00FF00";
			}
			else if (zhuoyueAttributeCount2 >= 3 && zhuoyueAttributeCount2 <= 4)
			{
				text2 = "0099FF";
			}
			else if (zhuoyueAttributeCount2 >= 5 && zhuoyueAttributeCount2 <= 6)
			{
				text2 = "ff08ff";
			}
		}
		else
		{
			text2 = goodsXmlNodeByID.GoodsColor;
		}
		text += goodsXmlNodeByID.Title;
		if (isShowCount)
		{
			text = string.Format("{0} x{1}", text, goodsData.GCount);
		}
		else if (isShowAllName)
		{
			string text3 = null;
			string text4 = null;
			if (isJuHun)
			{
				if (goodsData.JuHunID > 0)
				{
					text3 = string.Format(" +{0}%", ParseJuHunConfig.GetJuHunDataById(goodsData.JuHunID).GrowProportion * 100f);
					text4 = null;
					text = string.Format("{0} {1}{2}", text, text3, text4);
				}
				else
				{
					text = string.Format("{0} {1}{2}", text, text3, text4);
				}
			}
			else
			{
				text3 = ((goodsData.Forge_level <= 0) ? string.Empty : string.Format("+{0}", goodsData.Forge_level));
				text4 = ((goodsData.AppendPropLev <= 0) ? string.Empty : string.Format(Global.GetLang("追{0}"), goodsData.AppendPropLev));
				text = string.Format("{0} {1}{2}", text, text3, text4);
			}
		}
		return Global.GetColorStringForNGUIText(new object[]
		{
			text2,
			text
		});
	}

	public static string FormatGoodsListName(List<GoodsData> gdsList, bool isBr = false)
	{
		string text = string.Empty;
		if (gdsList == null)
		{
			return text;
		}
		for (int i = 0; i < gdsList.Count; i++)
		{
			text += UIHelper.FormatGoodsName(gdsList[i], true, false, false);
			text += ((!isBr) ? "  " : "\n");
		}
		return text;
	}

	public static TaskAwardsData ParseFuBenAwards(int fuBenID)
	{
		TaskAwardsData taskAwardsData = new TaskAwardsData();
		XElement fuBenMapElement = Global.GetFuBenMapElement(-1, fuBenID);
		if (fuBenMapElement == null)
		{
			return null;
		}
		taskAwardsData.Experienceaward = (long)Global.GetXElementAttributeInt(fuBenMapElement, "Experienceaward");
		taskAwardsData.Moneyaward = Global.GetXElementAttributeInt(fuBenMapElement, "Moneyaward");
		string xelementAttributeStr = Global.GetXElementAttributeStr(fuBenMapElement, "GoodsIDs");
		taskAwardsData.TaskawardList = UIHelper.ParseGoodsList5(xelementAttributeStr, int.MaxValue);
		return taskAwardsData;
	}

	public static List<AwardsItemData> ParseGoodsList5(string GoodsListString, int max = 2147483647)
	{
		if (string.IsNullOrEmpty(GoodsListString) || max <= 0)
		{
			return null;
		}
		List<AwardsItemData> list = new List<AwardsItemData>();
		string[] array = GoodsListString.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (--max < 0)
			{
				break;
			}
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				list.Add(new AwardsItemData
				{
					GoodsID = array2[0].SafeToInt32(0),
					GoodsNum = array2[1].SafeToInt32(0),
					Binding = array2[2].SafeToInt32(0),
					Level = array2[3].SafeToInt32(0),
					AppendLev = array2[4].SafeToInt32(0),
					IsHaveLuckyProp = array2[5].SafeToInt32(0),
					ExcellencePorpValue = array2[6].SafeToInt32(0)
				});
			}
		}
		return list;
	}

	public static List<AwardsItemData> ParseFallPackage(int fallID)
	{
		XElement fallPackageElement = Global.GetFallPackageElement(fallID);
		if (fallPackageElement == null)
		{
			return null;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(fallPackageElement, "MaxList");
		string xelementAttributeStr = Global.GetXElementAttributeStr(fallPackageElement, "GoodsID");
		return UIHelper.ParseGoodsList5(xelementAttributeStr, xelementAttributeInt);
	}

	public static string GetAwardsName(AwardsTypes awardType, int nameType = 0)
	{
		string result = null;
		switch (awardType)
		{
		case AwardsTypes.Exp:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励经验:");
			}
			else
			{
				result = "exp";
			}
			break;
		case AwardsTypes.JinBi:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励金币:");
			}
			else
			{
				result = "gold";
			}
			break;
		case AwardsTypes.BindJinBi:
			if (nameType == 0)
			{
				result = Global.GetLang("绑定金币:");
			}
			else
			{
				result = "bindmoney";
			}
			break;
		case AwardsTypes.ZuanShi:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励钻石:");
			}
			else
			{
				result = "diamond";
			}
			break;
		case AwardsTypes.BindZuanShi:
			if (nameType == 0)
			{
				result = Global.GetLang("绑定钻石:");
			}
			else
			{
				result = "binddiamond";
			}
			break;
		case AwardsTypes.MoJing:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励魔晶:");
			}
			else
			{
				result = "mojing";
			}
			break;
		case AwardsTypes.ShengWang:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励声望:");
			}
			else
			{
				result = "shengwang";
			}
			break;
		case AwardsTypes.ZhanGong:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励战功:");
			}
			else
			{
				result = "zhangong";
			}
			break;
		case AwardsTypes.ChengJiu:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励成就:");
			}
			else
			{
				result = "chengjiu";
			}
			break;
		case AwardsTypes.XingHun:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励星魂:");
			}
			else
			{
				result = "xinghun";
			}
			break;
		case AwardsTypes.CangBaoXueZuan:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励血钻:");
			}
			else
			{
				result = "XueZuan";
			}
			break;
		case AwardsTypes.CangBaoJiFen:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励宝藏积分:");
			}
			else
			{
				result = "CangBaoJiFen";
			}
			break;
		case AwardsTypes.FenMo:
			if (nameType == 0)
			{
				result = Global.GetLang("奖励粉末:");
			}
			else
			{
				result = "yuansu";
			}
			break;
		}
		return result;
	}

	public static CText AddTextAwards(ObservableCollection ItemCollection, AwardsTypes awardstype, long value, string prefab = "CTextAwards2")
	{
		int num = 0;
		if (prefab == "CTextAwards2")
		{
			num = 1;
		}
		CText ctext = U3DUtils.NEW<CText>(prefab);
		ItemCollection.AddNoUpdate(ctext);
		ctext.Name = UIHelper.GetAwardsName(awardstype, num);
		ctext.type = num;
		ctext.Value = value;
		ctext.FontSize = 18;
		return ctext;
	}

	public static void SetTextAwards(CText item, AwardsTypes awardstype, long value)
	{
		if (awardstype != AwardsTypes.None)
		{
			item.Name = UIHelper.GetAwardsName(awardstype, item.type);
		}
		item.Value = value;
	}

	public static void AddAwardData(ObservableCollection ItemCollection, TaskAwardsData taskAwards, string prefab = "CTextAwards")
	{
		if (taskAwards == null)
		{
			return;
		}
		int num = 0;
		if (prefab == "CTextAwards2")
		{
			num = 1;
		}
		if (taskAwards.Experienceaward > 0L)
		{
			CText ctext = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext);
			ctext.type = num;
			ctext.Name = UIHelper.GetAwardsName(AwardsTypes.Exp, num);
			ctext.Value = taskAwards.Experienceaward;
			ctext.FontSize = 18;
		}
		if (taskAwards.Moneyaward > 0)
		{
			CText ctext2 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext2);
			ctext2.type = num;
			ctext2.Name = UIHelper.GetAwardsName(AwardsTypes.BindJinBi, num);
			ctext2.Value = (long)taskAwards.Moneyaward;
			ctext2.FontSize = 18;
		}
		if (taskAwards.YinLiangaward > 0)
		{
			CText ctext3 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext3);
			ctext3.type = num;
			ctext3.Name = UIHelper.GetAwardsName(AwardsTypes.JinBi, num);
			ctext3.Value = (long)taskAwards.YinLiangaward;
			ctext3.FontSize = 18;
		}
		if (taskAwards.BindYuanBaoaward > 0)
		{
			CText ctext4 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext4);
			ctext4.type = num;
			ctext4.Name = UIHelper.GetAwardsName(AwardsTypes.BindZuanShi, num);
			ctext4.Value = (long)taskAwards.BindYuanBaoaward;
			ctext4.FontSize = 18;
		}
		if (taskAwards.ZhenQiaward > 0)
		{
			CText ctext5 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext5);
			ctext5.type = num;
			ctext5.Name = UIHelper.GetAwardsName(AwardsTypes.None, num);
			ctext5.Value = (long)taskAwards.ZhenQiaward;
			ctext5.FontSize = 18;
		}
		if (taskAwards.LieShaaward > 0)
		{
			CText ctext6 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext6);
			ctext6.type = num;
			ctext6.Name = UIHelper.GetAwardsName(AwardsTypes.None, num);
			ctext6.Value = (long)taskAwards.LieShaaward;
			ctext6.FontSize = 18;
		}
		if (taskAwards.WuXingaward > 0)
		{
			CText ctext7 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext7);
			ctext7.type = num;
			ctext7.Name = UIHelper.GetAwardsName(AwardsTypes.None, num);
			ctext7.Value = (long)taskAwards.WuXingaward;
			ctext7.FontSize = 18;
		}
		if (taskAwards.JunGongaward > 0)
		{
			CText ctext8 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext8);
			ctext8.type = num;
			ctext8.Name = UIHelper.GetAwardsName(AwardsTypes.ZhanGong, num);
			ctext8.Value = (long)taskAwards.JunGongaward;
			ctext8.FontSize = 18;
		}
		if (taskAwards.RongYuaward > 0)
		{
			CText ctext9 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext9);
			ctext9.type = num;
			ctext9.Name = UIHelper.GetAwardsName(AwardsTypes.ChengJiu, num);
			ctext9.Value = (long)taskAwards.RongYuaward;
			ctext9.FontSize = 18;
		}
		if (taskAwards.MoJingaward > 0)
		{
			CText ctext10 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext10);
			ctext10.Name = UIHelper.GetAwardsName(AwardsTypes.MoJing, num);
			ctext10.Value = (long)taskAwards.MoJingaward;
			ctext10.type = num;
			ctext10.FontSize = 18;
		}
		if (taskAwards.XingHunaward > 0)
		{
			CText ctext11 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext11);
			ctext11.type = num;
			ctext11.Name = UIHelper.GetAwardsName(AwardsTypes.XingHun, num);
			ctext11.Value = (long)taskAwards.XingHunaward;
			ctext11.FontSize = 18;
		}
		if (taskAwards.FenMoAward > 0)
		{
			CText ctext12 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext12);
			ctext12.type = num;
			ctext12.Name = UIHelper.GetAwardsName(AwardsTypes.FenMo, num);
			ctext12.Value = (long)taskAwards.FenMoAward;
			ctext12.FontSize = 18;
		}
		if (taskAwards.ShengwangAward > 0)
		{
			CText ctext13 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext13);
			ctext13.type = num;
			ctext13.Name = UIHelper.GetAwardsName(AwardsTypes.ShengWang, num);
			ctext13.Value = (long)taskAwards.ShengwangAward;
			ctext13.FontSize = 18;
		}
		ItemCollection.DelayUpdate();
	}

	public static void AddAwardData(ObservableCollection ItemCollection, MoneyTypes Moneytype, int taskAwards, string prefab = "CTextAwards")
	{
		int num = 0;
		if (prefab == "CTextAwards2")
		{
			num = 1;
		}
		if (Moneytype == MoneyTypes.BaoZangJiFen)
		{
			CText ctext = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext);
			ctext.type = num;
			ctext.Name = UIHelper.GetAwardsName(AwardsTypes.CangBaoJiFen, num);
			ctext.Value = (long)taskAwards;
			ctext.FontSize = 18;
		}
		else if (Moneytype == MoneyTypes.BaoZangXueZuan)
		{
			CText ctext2 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext2);
			ctext2.type = num;
			ctext2.Name = UIHelper.GetAwardsName(AwardsTypes.CangBaoXueZuan, num);
			ctext2.Value = (long)taskAwards;
			ctext2.FontSize = 18;
		}
		ItemCollection.DelayUpdate();
	}

	public static void AddAwardDataRate(ObservableCollection ItemCollection, TaskAwardsData taskAwards, int Rate, string prefab = "CTextAwards")
	{
		if (taskAwards == null)
		{
			return;
		}
		int num = 0;
		if (prefab == "CTextAwards2")
		{
			num = 1;
		}
		if (taskAwards.Experienceaward > 0L)
		{
			CText ctext = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext);
			ctext.type = num;
			ctext.Name = UIHelper.GetAwardsName(AwardsTypes.Exp, num);
			ctext.Text = ((Rate <= 1) ? (string.Empty + taskAwards.Experienceaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.Experienceaward,
				" x ",
				Rate
			}));
			ctext.FontSize = 18;
		}
		if (taskAwards.Moneyaward > 0)
		{
			CText ctext2 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext2);
			ctext2.type = num;
			ctext2.Name = UIHelper.GetAwardsName(AwardsTypes.BindJinBi, num);
			ctext2.Text = ((Rate <= 1) ? (string.Empty + taskAwards.Moneyaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.Moneyaward,
				" x ",
				Rate
			}));
			ctext2.FontSize = 18;
		}
		if (taskAwards.YinLiangaward > 0)
		{
			CText ctext3 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext3);
			ctext3.type = num;
			ctext3.Name = UIHelper.GetAwardsName(AwardsTypes.JinBi, num);
			ctext3.Text = ((Rate <= 1) ? (string.Empty + taskAwards.YinLiangaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.YinLiangaward,
				" x ",
				Rate
			}));
			ctext3.FontSize = 18;
		}
		if (taskAwards.BindYuanBaoaward > 0)
		{
			CText ctext4 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext4);
			ctext4.type = num;
			ctext4.Name = UIHelper.GetAwardsName(AwardsTypes.BindZuanShi, num);
			ctext4.Text = ((Rate <= 1) ? (string.Empty + taskAwards.BindYuanBaoaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.BindYuanBaoaward,
				" x ",
				Rate
			}));
			ctext4.FontSize = 18;
		}
		if (taskAwards.ZhenQiaward > 0)
		{
			CText ctext5 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext5);
			ctext5.type = num;
			ctext5.Name = UIHelper.GetAwardsName(AwardsTypes.None, num);
			ctext5.Text = ((Rate <= 1) ? (string.Empty + taskAwards.ZhenQiaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.ZhenQiaward,
				" x ",
				Rate
			}));
			ctext5.FontSize = 18;
		}
		if (taskAwards.LieShaaward > 0)
		{
			CText ctext6 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext6);
			ctext6.type = num;
			ctext6.Name = UIHelper.GetAwardsName(AwardsTypes.None, num);
			ctext6.Text = ((Rate <= 1) ? (string.Empty + taskAwards.LieShaaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.LieShaaward,
				" x ",
				Rate
			}));
			ctext6.FontSize = 18;
		}
		if (taskAwards.WuXingaward > 0)
		{
			CText ctext7 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext7);
			ctext7.type = num;
			ctext7.Name = UIHelper.GetAwardsName(AwardsTypes.None, num);
			ctext7.Text = ((Rate <= 1) ? (string.Empty + taskAwards.WuXingaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.WuXingaward,
				" x ",
				Rate
			}));
			ctext7.FontSize = 18;
		}
		if (taskAwards.JunGongaward > 0)
		{
			CText ctext8 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext8);
			ctext8.type = num;
			ctext8.Name = UIHelper.GetAwardsName(AwardsTypes.ZhanGong, num);
			ctext8.Text = ((Rate <= 1) ? (string.Empty + taskAwards.JunGongaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.JunGongaward,
				" x ",
				Rate
			}));
			ctext8.FontSize = 18;
		}
		if (taskAwards.RongYuaward > 0)
		{
			CText ctext9 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext9);
			ctext9.type = num;
			ctext9.Name = UIHelper.GetAwardsName(AwardsTypes.ChengJiu, num);
			ctext9.Text = ((Rate <= 1) ? (string.Empty + taskAwards.RongYuaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.RongYuaward,
				" x ",
				Rate
			}));
			ctext9.FontSize = 18;
		}
		if (taskAwards.MoJingaward > 0)
		{
			CText ctext10 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext10);
			ctext10.Name = UIHelper.GetAwardsName(AwardsTypes.MoJing, num);
			ctext10.Text = ((Rate <= 1) ? (string.Empty + taskAwards.MoJingaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.MoJingaward,
				" x ",
				Rate
			}));
			ctext10.type = num;
			ctext10.FontSize = 18;
		}
		if (taskAwards.XingHunaward > 0)
		{
			CText ctext11 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext11);
			ctext11.type = num;
			ctext11.Name = UIHelper.GetAwardsName(AwardsTypes.XingHun, num);
			ctext11.Text = ((Rate <= 1) ? (string.Empty + taskAwards.XingHunaward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.XingHunaward,
				" x ",
				Rate
			}));
			ctext11.FontSize = 18;
		}
		if (taskAwards.FenMoAward > 0)
		{
			CText ctext12 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext12);
			ctext12.type = num;
			ctext12.Name = UIHelper.GetAwardsName(AwardsTypes.FenMo, num);
			ctext12.Text = ((Rate <= 1) ? (string.Empty + taskAwards.FenMoAward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.FenMoAward,
				" x ",
				Rate
			}));
			ctext12.FontSize = 18;
		}
		if (taskAwards.ShengwangAward > 0)
		{
			CText ctext13 = U3DUtils.NEW<CText>(prefab);
			ItemCollection.AddNoUpdate(ctext13);
			ctext13.type = num;
			ctext13.Name = UIHelper.GetAwardsName(AwardsTypes.ShengWang, num);
			ctext13.Text = ((Rate <= 1) ? (string.Empty + taskAwards.ShengwangAward) : string.Concat(new object[]
			{
				string.Empty,
				taskAwards.ShengwangAward,
				" x ",
				Rate
			}));
			ctext13.FontSize = 18;
		}
		ItemCollection.DelayUpdate();
	}

	public static List<GoodsData> ParseFallGoodsList(string goodsListStr)
	{
		List<GoodsData> list = new List<GoodsData>();
		string[] array = goodsListStr.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			int goodsID = Convert.ToInt32(array2[0]);
			double num = Convert.ToDouble(array2[1]);
			int quality = Convert.ToInt32(array2[2]);
			int binding = Convert.ToInt32(array2[3]);
			int appendPropLev = Convert.ToInt32(array2[4]);
			int bornIndex = Convert.ToInt32(array2[5]);
			list.Add(new GoodsData
			{
				GoodsID = goodsID,
				Quality = quality,
				Binding = binding,
				AppendPropLev = appendPropLev,
				BornIndex = bornIndex
			});
		}
		return list;
	}

	public static List<GoodsData> ParseChouJiangGoodsList(string goodsListStr)
	{
		List<GoodsData> list = new List<GoodsData>();
		string[] array = goodsListStr.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			int goodsID = Convert.ToInt32(array2[0]);
			double num = Convert.ToDouble(array2[1]);
			int quality = Convert.ToInt32(array2[2]);
			int binding = Convert.ToInt32(array2[3]);
			int appendPropLev = Convert.ToInt32(array2[4]);
			int bornIndex = Convert.ToInt32(array2[5]);
			list.Add(new GoodsData
			{
				GoodsID = goodsID,
				Quality = quality,
				Binding = binding,
				AppendPropLev = appendPropLev,
				BornIndex = bornIndex
			});
		}
		return list;
	}

	public static List<GoodsData> ParseTaskAwardsGoodsList(TaskAwardsData taskAwards)
	{
		if (taskAwards == null)
		{
			return null;
		}
		List<GoodsData> list = new List<GoodsData>();
		if (taskAwards.TaskawardList != null)
		{
			for (int i = 0; i < taskAwards.TaskawardList.Count; i++)
			{
				UIHelper.ParseAwardsItem(taskAwards.TaskawardList[i], list, true, true);
			}
		}
		if (taskAwards.OtherTaskawardList != null)
		{
			for (int j = 0; j < taskAwards.OtherTaskawardList.Count; j++)
			{
				UIHelper.ParseAwardsItem(taskAwards.OtherTaskawardList[j], list, false, false);
			}
		}
		return list;
	}

	public static void ParseTaskAwardsItem(AwardsItemData awardsItemData, List<GoodsData> goodsDataList, bool occupation, bool sex)
	{
		if (awardsItemData == null)
		{
			return;
		}
		GoodsData goodsData = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(awardsItemData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			bool flag = true;
			if (occupation)
			{
				int mainOccupation = goodsXmlNodeByID.MainOccupation;
				if (mainOccupation >= 0 && Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) != mainOccupation)
				{
					return;
				}
			}
			if (sex && goodsXmlNodeByID.ToSex >= 0 && Global.Data.roleData.RoleSex != goodsXmlNodeByID.ToSex)
			{
				return;
			}
			if (flag)
			{
				goodsData = new GoodsData();
				goodsData.Id = goodsDataList.Count + 1;
				goodsData.GoodsID = awardsItemData.GoodsID;
				goodsData.Using = 0;
				goodsData.Forge_level = awardsItemData.Level;
				goodsData.Starttime = "1900-01-01 12:00:00";
				goodsData.Endtime = awardsItemData.EndTime;
				goodsData.Site = 0;
				goodsData.Quality = awardsItemData.Quality;
				goodsData.Props = string.Empty;
				goodsData.GCount = awardsItemData.GoodsNum;
				goodsData.Binding = awardsItemData.Binding;
				goodsData.Jewellist = string.Empty;
				goodsData.BagIndex = 0;
				goodsData.AddPropIndex = 0;
				goodsData.BornIndex = 0;
				goodsData.Lucky = awardsItemData.IsHaveLuckyProp;
				goodsData.Strong = 0;
				goodsData.ExcellenceInfo = awardsItemData.ExcellencePorpValue;
				goodsData.AppendPropLev = awardsItemData.AppendLev;
			}
		}
		if (goodsData != null)
		{
			goodsDataList.Add(goodsData);
		}
	}

	public static void ParseAwardsItem(AwardsItemData awardsItemData, List<GoodsData> goodsDataList, bool occupation, bool sex)
	{
		if (awardsItemData == null)
		{
			return;
		}
		GoodsData goodsData = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(awardsItemData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			bool flag = true;
			if (occupation)
			{
				int toOccupation = goodsXmlNodeByID.ToOccupation;
				if (toOccupation >= 0 && !Global.ValidOccupation(toOccupation, Global.Data.roleData.Occupation))
				{
					return;
				}
			}
			if (sex && goodsXmlNodeByID.ToSex >= 0 && Global.Data.roleData.RoleSex != goodsXmlNodeByID.ToSex)
			{
				return;
			}
			if (flag)
			{
				goodsData = new GoodsData();
				goodsData.Id = goodsDataList.Count + 1;
				goodsData.GoodsID = awardsItemData.GoodsID;
				goodsData.Using = 0;
				goodsData.Forge_level = awardsItemData.Level;
				goodsData.Starttime = "1900-01-01 12:00:00";
				goodsData.Endtime = awardsItemData.EndTime;
				goodsData.Site = 0;
				goodsData.Quality = awardsItemData.Quality;
				goodsData.Props = string.Empty;
				goodsData.GCount = awardsItemData.GoodsNum;
				goodsData.Binding = awardsItemData.Binding;
				goodsData.Jewellist = string.Empty;
				goodsData.BagIndex = 0;
				goodsData.AddPropIndex = 0;
				goodsData.BornIndex = 0;
				goodsData.Lucky = awardsItemData.IsHaveLuckyProp;
				goodsData.Strong = 0;
				goodsData.ExcellenceInfo = awardsItemData.ExcellencePorpValue;
				goodsData.AppendPropLev = awardsItemData.AppendLev;
			}
		}
		if (goodsData != null)
		{
			goodsDataList.Add(goodsData);
		}
	}

	public static GGoodIcon AddGoodsIcon(SpriteSL parent, GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null, bool disabled = false)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		parent.Add(ggoodIcon);
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		if (goodsData != null)
		{
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsData.GoodsID,
				0,
				goodsData.Id,
				15
			});
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.GoodsID = goodsData.GoodsID;
			ggoodIcon.TipType = 1;
			ggoodIcon.GoodsCount = goodsData.GCount;
			ggoodIcon.Binding = goodsData.Binding;
			ggoodIcon.Lucky = goodsData.Lucky;
			ggoodIcon.ForgeLevel = goodsData.Forge_level;
			ggoodIcon.ZhuijiaLevel = goodsData.AppendPropLev;
			ggoodIcon.ExcellenceInfo = goodsData.ExcellenceInfo;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				string goodsIconString = Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode));
				ggoodIcon.BodyURL = new ImageURL(goodsIconString, false, 0);
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(-1), false, 0);
			}
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		}
		UIButtonOffset component = ggoodIcon.GetComponent<UIButtonOffset>();
		if (null != component)
		{
			component.enabled = false;
		}
		if (disabled)
		{
			U3DUtils.EnableCollider(ggoodIcon.gameObject, false);
		}
		else if (handler != null)
		{
			ggoodIcon.MouseLeftButtonUp = handler;
		}
		else
		{
			ggoodIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(ggoodIcon.NormalGoodsTipsHandler);
		}
		return ggoodIcon;
	}

	public static GGoodIcon AddGoodsIcon(ObservableCollection ItemCollection, GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null, bool disabled = false, string bakName0 = "bagGrid4_bak")
	{
		if (ItemCollection == null || goodsData == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ItemCollection.Add(ggoodIcon);
		ggoodIcon.isAutoSize = true;
		ggoodIcon.isAutoInnerSize = true;
		ggoodIcon.isAutoItemPos = true;
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.OutSizeX = 78;
		ggoodIcon.OutSizeY = 78;
		ggoodIcon.BackSpriteName0 = bakName0;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			15
		});
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.BoxTypes = -1;
		ggoodIcon.GoodsID = goodsData.GoodsID;
		ggoodIcon.TipType = 1;
		ggoodIcon.GoodsCount = goodsData.GCount;
		ggoodIcon.Binding = goodsData.Binding;
		ggoodIcon.Lucky = goodsData.Lucky;
		ggoodIcon.ForgeLevel = goodsData.Forge_level;
		ggoodIcon.ZhuijiaLevel = goodsData.AppendPropLev;
		ggoodIcon.ExcellenceInfo = goodsData.ExcellenceInfo;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsIconString = Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode));
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BodyURL = new ImageURL(goodsIconString, false, 0);
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(-1), false, 0);
		}
		UIButtonOffset component = ggoodIcon.GetComponent<UIButtonOffset>();
		if (null != component)
		{
			component.enabled = false;
		}
		if (disabled)
		{
			U3DUtils.EnableCollider(ggoodIcon.gameObject, false);
		}
		else if (handler != null)
		{
			ggoodIcon.MouseLeftButtonUp = handler;
		}
		else
		{
			ggoodIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(ggoodIcon.NormalGoodsTipsHandler);
		}
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	public static GGoodIcon AddGoodsIcon2(ObservableCollection ItemCollection, GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null, bool disabled = false, string bakName0 = "bagGrid4_bak")
	{
		if (ItemCollection == null || goodsData == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ItemCollection.Add(ggoodIcon);
		ggoodIcon.isAutoSize = false;
		ggoodIcon.isAutoInnerSize = true;
		ggoodIcon.isAutoItemPos = true;
		ggoodIcon.Width = 64.0;
		ggoodIcon.Height = 64.0;
		ggoodIcon.OutSizeX = 72;
		ggoodIcon.OutSizeY = 72;
		ggoodIcon.BackSpriteName0 = bakName0;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			15
		});
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.BoxTypes = -1;
		ggoodIcon.GoodsID = goodsData.GoodsID;
		ggoodIcon.TipType = 1;
		ggoodIcon.GoodsCount = goodsData.GCount;
		ggoodIcon.Binding = goodsData.Binding;
		ggoodIcon.Lucky = goodsData.Lucky;
		ggoodIcon.ForgeLevel = goodsData.Forge_level;
		ggoodIcon.ZhuijiaLevel = goodsData.AppendPropLev;
		ggoodIcon.ExcellenceInfo = goodsData.ExcellenceInfo;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsIconString = Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode));
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BodyURL = new ImageURL(goodsIconString, false, 0);
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(-1), false, 0);
		}
		UIButtonOffset component = ggoodIcon.GetComponent<UIButtonOffset>();
		if (null != component)
		{
			component.enabled = false;
		}
		if (disabled)
		{
			U3DUtils.EnableCollider(ggoodIcon.gameObject, false);
		}
		else if (handler != null)
		{
			ggoodIcon.MouseLeftButtonUp = handler;
		}
		else
		{
			ggoodIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(ggoodIcon.NormalGoodsTipsHandler);
		}
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	public static void AddGoodsIcon3(ObservableCollection ItemCollection, GoodsData gd, bool isDrag, bool isShowTextBySuitID = false, bool grayShow = false, string bakName0 = "bagGrid4_bak")
	{
		if (ItemCollection == null || gd == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = bakName0;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.TextShadowColor = 4278190080U;
			icon.GoodsID = gd.GoodsID;
			icon.TipType = 1;
			icon.GoodsCount = gd.GCount;
			icon.Binding = gd.Binding;
			icon.Lucky = gd.Lucky;
			icon.ForgeLevel = gd.Forge_level;
			icon.ZhuijiaLevel = gd.AppendPropLev;
			icon.ExcellenceInfo = gd.ExcellenceInfo;
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			if (isShowTextBySuitID)
			{
				if (goodsXmlNodeByID.SuitID > 0)
				{
					icon.Text = string.Format(Global.GetLang("{0}阶"), goodsXmlNodeByID.SuitID);
					int goodsQuality = Super.GetGoodsQuality(gd.GoodsID);
					if (goodsQuality == 1)
					{
						icon.BackSpriteName1 = "iconState_zuoyue";
						icon.TextColor = 65280U;
					}
					else if (goodsQuality == 2)
					{
						icon.BackSpriteName1 = "iconState_zuoyue1";
						icon.TextColor = 39423U;
					}
					else if (goodsQuality == 3 || goodsQuality == 4)
					{
						icon.TextColor = 16713983U;
					}
					else if (goodsQuality == 6)
					{
						icon.TextColor = 16736512U;
					}
				}
				icon.PaddingX = 8;
				icon.PaddingY = 12;
				icon.TextHorizontalAlignment = global::Layout.Left;
				icon.TextVerticalAlignment = global::Layout.Top;
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			ItemCollection.Add(icon);
			if (isDrag)
			{
				icon.gameObject.AddComponent<UIDragPanelContents>();
				icon.addEventListener("click", delegate(MouseEvent e)
				{
					GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
					if (null == ggoodIcon)
					{
						return;
					}
					GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
					if (goodsData == null)
					{
						return;
					}
					GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
				});
			}
			else
			{
				icon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(icon.NormalGoodsTipsHandler);
			}
		}
	}

	public static List<GoodsData> ParseTaskAwardsItemList(List<AwardsItemData> awardsItemDataList, ref List<GoodsData> goodsDataList)
	{
		if (awardsItemDataList == null)
		{
			return goodsDataList;
		}
		if (goodsDataList == null)
		{
			goodsDataList = new List<GoodsData>();
		}
		if (awardsItemDataList != null)
		{
			for (int i = 0; i < awardsItemDataList.Count; i++)
			{
				UIHelper.ParseTaskAwardsItem(awardsItemDataList[i], goodsDataList, true, true);
			}
		}
		return goodsDataList;
	}

	public static List<GoodsData> ParseAwardsItemList(List<AwardsItemData> awardsItemDataList, ref List<GoodsData> goodsDataList)
	{
		if (awardsItemDataList == null)
		{
			return goodsDataList;
		}
		if (goodsDataList == null)
		{
			goodsDataList = new List<GoodsData>();
		}
		if (awardsItemDataList != null)
		{
			for (int i = 0; i < awardsItemDataList.Count; i++)
			{
				UIHelper.ParseAwardsItem(awardsItemDataList[i], goodsDataList, true, true);
			}
		}
		return goodsDataList;
	}

	public static void AddTaskAwardGoods(ObservableCollection ItemCollection, TaskAwardsData taskAwards)
	{
		Super.GData.ViewTaskInfoGoodsDataList = UIHelper.ParseTaskAwardsGoodsList(taskAwards);
		if (Super.GData.ViewTaskInfoGoodsDataList != null && Super.GData.ViewTaskInfoGoodsDataList.Count > 0)
		{
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				UIHelper.AddGoodsIcon(ItemCollection, Super.GData.ViewTaskInfoGoodsDataList[i], null, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	public static void AddAwardGoods(ObservableCollection ItemCollection, List<GoodsData> goodsList, MouseLeftButtonUpEventHandler handler = null, bool disabled = false, string bakName0 = "bagGrid4_bak", bool isShowTextBySuitID = false)
	{
		if (goodsList != null && goodsList.Count > 0)
		{
			for (int i = 0; i < goodsList.Count; i++)
			{
				if (!isShowTextBySuitID)
				{
					GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(ItemCollection, goodsList[i], handler, disabled, bakName0);
				}
				else
				{
					UIHelper.AddGoodsIcon3(ItemCollection, goodsList[i], true, true, false, "bagGrid4_bak");
				}
			}
			ItemCollection.DelayUpdate();
		}
	}

	public static void AddAwardGoods2(ObservableCollection ItemCollection, List<GoodsData> goodsList, MouseLeftButtonUpEventHandler handler = null, bool disabled = false, string bakName0 = "bagGrid4_bak")
	{
		if (goodsList != null && goodsList.Count > 0)
		{
			for (int i = 0; i < goodsList.Count; i++)
			{
				GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon2(ItemCollection, goodsList[i], handler, disabled, bakName0);
			}
			ItemCollection.DelayUpdate();
		}
	}

	public static void AddAwardGoods(ObservableCollection ItemCollection, string goodsIDs)
	{
		if (ItemCollection == null || string.IsNullOrEmpty(goodsIDs))
		{
			return;
		}
		List<GoodsData> list = UIHelper.ParseRewardGoodsList(goodsIDs, 0, int.MaxValue);
		for (int i = 0; i < list.Count; i++)
		{
			UIHelper.AddGoodsIcon(ItemCollection, list[i], null, false, "bagGrid4_bak");
		}
		ItemCollection.DelayUpdate();
	}

	public static List<GoodsData> ParseRewardGoodsList(string goodsListStr, int index = 0, int count = 2147483647)
	{
		List<GoodsData> list = new List<GoodsData>();
		if (string.IsNullOrEmpty(goodsListStr))
		{
			return list;
		}
		int num = 0;
		string[] array = goodsListStr.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[num];
			if (num++ >= index)
			{
				GoodsData goodsData = new GoodsData();
				string[] array2 = text.Split(new char[]
				{
					','
				});
				int num2 = array2.Length;
				goodsData.GoodsID = array2[0].SafeToInt32(-1);
				if (num2 >= 2)
				{
					goodsData.GCount = array2[1].SafeToInt32(0);
				}
				if (num2 >= 3)
				{
					goodsData.Binding = array2[2].SafeToInt32(0);
				}
				if (num2 >= 4)
				{
					goodsData.Forge_level = array2[3].SafeToInt32(0);
				}
				if (num2 >= 5)
				{
					goodsData.AppendPropLev = array2[4].SafeToInt32(0);
				}
				if (num2 >= 6)
				{
					goodsData.Lucky = array2[5].SafeToInt32(0);
				}
				if (num2 >= 7)
				{
					goodsData.ExcellenceInfo = array2[6].SafeToInt32(0);
				}
				list.Add(goodsData);
				if (num >= index + count)
				{
					break;
				}
			}
		}
		return list;
	}

	public static void AddAwardGoods(ObservableCollection ItemCollection, List<AwardsItemData> awardsItemList, MouseLeftButtonUpEventHandler handler = null)
	{
		if (awardsItemList == null)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		if (awardsItemList != null)
		{
			for (int i = 0; i < awardsItemList.Count; i++)
			{
				UIHelper.ParseAwardsItem(awardsItemList[i], list, false, false);
			}
		}
		if (list != null && list.Count > 0)
		{
			for (int j = 0; j < list.Count; j++)
			{
				UIHelper.AddGoodsIcon(ItemCollection, list[j], handler, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	public static string FormatBirthTimes(int birthType, string timePoints, int row = 2147483647)
	{
		string text = string.Empty;
		if (string.IsNullOrEmpty(timePoints))
		{
			return string.Empty;
		}
		if (birthType == 1)
		{
			return timePoints;
		}
		if (birthType == 7)
		{
			int num = 0;
			string[] array = timePoints.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length == 2)
				{
					int dayOfWeek = ConvertExt.SafeConvertToInt32(array2[0]) % 7;
					text += string.Format("{0} {1} |", UIHelper.FormatDayOfWeek(dayOfWeek), array2[1]);
				}
				if (row != 0 && ++num % row == 0)
				{
					text += "\r\n";
				}
			}
			text.TrimEnd(new char[]
			{
				'|'
			});
			return text;
		}
		return timePoints;
	}

	public static string FormatDayOfWeek(int dayOfWeek)
	{
		string result = string.Empty;
		switch (dayOfWeek)
		{
		case 0:
			result = Global.GetLang("周日");
			break;
		case 1:
			result = Global.GetLang("周一");
			break;
		case 2:
			result = Global.GetLang("周二");
			break;
		case 3:
			result = Global.GetLang("周三");
			break;
		case 4:
			result = Global.GetLang("周四");
			break;
		case 5:
			result = Global.GetLang("周五");
			break;
		case 6:
			result = Global.GetLang("周六");
			break;
		}
		return result;
	}

	public static string FormatSecs(long secs, string def = "-")
	{
		if (secs < 0L)
		{
			return def;
		}
		long num = secs / 3600L;
		long num2 = secs % 3600L / 60L;
		long num3 = secs % 60L;
		if (num > 0L)
		{
			return StringUtil.substitute(Global.GetLang("{0:00}时{1}分{2}秒"), new object[]
			{
				num,
				Global.FormatStr("00", (double)num2),
				Global.FormatStr("00", (double)num3)
			});
		}
		if (num2 > 0L)
		{
			return StringUtil.substitute(Global.GetLang("{0:00}分{1}秒"), new object[]
			{
				num2,
				Global.FormatStr("00", (double)num3)
			});
		}
		return StringUtil.substitute(Global.GetLang("{0:00}秒"), new object[]
		{
			num3
		});
	}

	public static string FormatSecs1(long secs, string def = "-")
	{
		if (secs < 0L)
		{
			return def;
		}
		long num = secs / 3600L;
		long num2 = secs % 3600L / 60L;
		long num3 = secs % 60L;
		if (num >= 24L)
		{
			return StringUtil.substitute(Global.GetLang("{0:00}天"), new object[]
			{
				num / 24L
			});
		}
		if (num > 0L)
		{
			return StringUtil.substitute(Global.GetLang("{0:00}:{1}:{2}"), new object[]
			{
				num,
				Global.FormatStr("00", (double)num2),
				Global.FormatStr("00", (double)num3)
			});
		}
		if (num2 > 0L)
		{
			return StringUtil.substitute(Global.GetLang("{0:00}:{1}"), new object[]
			{
				num2,
				Global.FormatStr("00", (double)num3)
			});
		}
		return StringUtil.substitute(Global.GetLang("{0:00}"), new object[]
		{
			num3
		});
	}

	public static string FormatSecsShort(long secs, string def = "-")
	{
		if (secs <= 0L)
		{
			return def;
		}
		long num = secs / 3600L;
		long num2 = secs % 3600L / 60L;
		long num3 = secs % 60L;
		string text = string.Empty;
		if (num >= 24L)
		{
			text += StringUtil.substitute(Global.GetLang("{0}天"), new object[]
			{
				num / 24L
			});
		}
		if (num > 0L)
		{
			text += StringUtil.substitute(Global.GetLang("{0}小时"), new object[]
			{
				num % 24L
			});
		}
		if (num2 > 0L || (num > 0L && num3 > 0L))
		{
			text += StringUtil.substitute(Global.GetLang("{0}分钟"), new object[]
			{
				num2
			});
		}
		if (num3 > 0L)
		{
			text += StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
			{
				num3
			});
		}
		return text;
	}

	public static string FormatSecs2(long secs, string def = "-")
	{
		if (secs < 0L)
		{
			return def;
		}
		return StringUtil.substitute(Global.GetLang("{0:00}:{1}:{2}"), new object[]
		{
			secs / 3600L,
			Global.FormatStr("00", (double)(secs % 3600L / 60L)),
			Global.FormatStr("00", (double)(secs % 60L))
		});
	}

	public static string FormatSecsHM(long secs, string def = "-")
	{
		if (secs < 0L)
		{
			return def;
		}
		return StringUtil.substitute(Global.GetLang("{0:00}:{1}"), new object[]
		{
			secs / 3600L,
			Global.FormatStr("00", (double)(secs % 3600L / 60L))
		});
	}

	public static string FormatSecsMS(long secs, string def = "-")
	{
		if (secs < 0L)
		{
			return def;
		}
		return StringUtil.substitute(Global.GetLang("{0}:{1}"), new object[]
		{
			Global.FormatStr("00", (double)(secs % 3600L / 60L)),
			Global.FormatStr("00", (double)(secs % 60L))
		});
	}

	public static void SetReanderQueue(GameObject obj)
	{
		if (null == obj)
		{
			return;
		}
		if (null != obj.gameObject.GetComponent<Renderer>())
		{
			Material[] materials = obj.gameObject.GetComponent<Renderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				if (!(null == materials[i]))
				{
					if (materials[i].renderQueue < 3000)
					{
						materials[i].renderQueue = 3000;
					}
				}
			}
		}
		else
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Material[] materials2 = componentsInChildren[j].gameObject.GetComponent<Renderer>().materials;
				for (int k = 0; k < materials2.Length; k++)
				{
					if (materials2[k].renderQueue < 3000)
					{
						materials2[k].renderQueue = 3000;
					}
				}
			}
		}
	}

	public static MonsterNPCResLoader LoadNPCRes(Modal3DShow parant, int NpcResID, float scale, OnLoadMonsterNPCComplete completeCallback = null)
	{
		scale = ((scale > 0f) ? scale : 1f);
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(NpcResID);
		parant.MonsterID = NpcResID;
		MonsterNPCLoaderData monsterNPCLoaderData = new MonsterNPCLoaderData();
		monsterNPCLoaderData.parent = parant.gameObject;
		monsterNPCLoaderData.MonsterID = NpcResID;
		monsterNPCLoaderData.resName = npcvobyID.ResName;
		monsterNPCLoaderData.spriteType = GSpriteTypes.NPC;
		monsterNPCLoaderData.ReplaceChildLayer = true;
		monsterNPCLoaderData.layer = parant.gameObject.layer;
		monsterNPCLoaderData.scale = scale;
		MonsterNPCResLoader result;
		if (completeCallback == null)
		{
			result = new MonsterNPCResLoader(monsterNPCLoaderData, new OnLoadMonsterNPCComplete(UIHelper.NPCLoaderComplete));
		}
		else
		{
			result = new MonsterNPCResLoader(monsterNPCLoaderData, completeCallback);
		}
		return result;
	}

	public static void NPCLoaderComplete(MonsterNPCLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		int monsterID = loader.MonsterID;
		if (monsterID > 0)
		{
			Global.AddSpecialGameObjects4Monster(go, monsterID, loader.layer, loader.scale * 0.3f);
		}
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (null != modal3DShow)
		{
			U3DUtils.AddChild(modal3DShow.gameObject, go, false);
			modal3DShow._Target = go;
			go.name = "UI_Boss_Monster_" + loader.MonsterID;
			go.transform.localScale = Vector3.one * loader.scale;
		}
		U3DUtils.ReplaceLayerInChildren(go, loader.layer, null);
		go.transform.localRotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
		LoadRoleShaderAgain loadRoleShaderAgain = go.AddComponent<LoadRoleShaderAgain>();
	}

	public static MonsterNPCResLoader LoadMonsterRes(Modal3DShow parant, int monsterResID, float scale)
	{
		scale = ((scale > 0f) ? scale : 1f);
		parant.MonsterID = monsterResID;
		MonsterNPCLoaderData monsterNPCLoaderData = new MonsterNPCLoaderData();
		monsterNPCLoaderData.parent = parant.gameObject;
		monsterNPCLoaderData.MonsterID = monsterResID;
		monsterNPCLoaderData.resName = ConfigMonsters.GetMonster3DResNameByID(monsterResID);
		monsterNPCLoaderData.spriteType = GSpriteTypes.Monster;
		monsterNPCLoaderData.ReplaceChildLayer = true;
		monsterNPCLoaderData.layer = parant.gameObject.layer;
		monsterNPCLoaderData.scale = scale;
		Global.HandleHandWeapon(monsterNPCLoaderData, monsterResID, GSpriteTypes.Monster);
		return new MonsterNPCResLoader(monsterNPCLoaderData, new OnLoadMonsterNPCComplete(UIHelper.MonsterLoaderComplete));
	}

	public static void MonsterLoaderComplete(MonsterNPCLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		int monsterID = loader.MonsterID;
		if (monsterID > 0)
		{
			Global.AddSpecialGameObjects4Monster(go, monsterID, loader.layer, loader.scale * 0.3f);
			UIHelper.ReplaceMaterials4UIMonster(go, monsterID);
			U3DUtils.ModifyAnimationSpeed(go, DataObject.Instance.GetMonsterSpeed(monsterID));
		}
		Animator component = go.GetComponent<Animator>();
		if (component != null)
		{
			component.SetBool("Stand", true);
			component.speed = DataObject.Instance.GetMonsterSpeed(loader.MonsterID);
		}
		else
		{
			AnimationManager animationManager = go.AddComponent<AnimationManager>();
			animationManager.ChangeAnimation("Stand", 2, false, null, 0f);
			animationManager.EndAnimation = new AnimationChangeEventHandler(Modal3DShow.EndAnimation);
		}
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (null != modal3DShow)
		{
			U3DUtils.AddChild(modal3DShow.gameObject, go, false);
			modal3DShow._Target = go;
			go.name = "UI_Boss_Monster_" + loader.MonsterID;
			go.transform.localScale = Vector3.one * loader.scale;
			if (loader.MonsterID == 609108)
			{
				Vector3 localPosition = go.transform.localPosition;
				go.transform.localPosition = new Vector3(localPosition.x, 0.7f, localPosition.z);
				go.transform.Rotate(75f, 180f, 0f);
			}
			U3DUtils.ReplaceLayerInChildren(go, loader.layer, null);
			if (loader.MonsterID != 609108)
			{
				go.transform.localRotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
			}
			LoadRoleShaderAgain loadRoleShaderAgain = go.AddComponent<LoadRoleShaderAgain>();
			return;
		}
		Object.Destroy(go);
	}

	public static void ReplaceMaterials4UIMonster(GameObject go, int monsterID)
	{
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
		if (monsterXmlNodeByID == null)
		{
			return;
		}
		if (monsterID == 4090)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				if (go.transform.GetChild(i).name.Equals("Monster_121_di_effect"))
				{
					U3DUtils.DestoryAllChild(go.transform.GetChild(i).gameObject);
					break;
				}
			}
		}
		go.AddComponent<LoadUIShaderAgain>();
		if (monsterXmlNodeByID.ShaderID <= 0)
		{
			return;
		}
		int shaderID = monsterXmlNodeByID.ShaderID;
		U3DUtils.ReplaceMaterials(go, shaderID, false);
	}

	public static MonsterNPCResLoader LoadBianShenRes(Modal3DShow parant, int monsterResID, int weaponId = -1)
	{
		float scale = 1f;
		parant.MonsterID = monsterResID;
		return new MonsterNPCResLoader(new MonsterNPCLoaderData
		{
			parent = parant.gameObject,
			MonsterID = monsterResID,
			rightWeaponID = weaponId,
			resName = ConfigMonsters.GetMonster3DResNameByID(monsterResID),
			spriteType = GSpriteTypes.Monster,
			ReplaceChildLayer = true,
			layer = parant.gameObject.layer,
			scale = scale
		}, new OnLoadMonsterNPCComplete(UIHelper.BianShenLoaderComplete));
	}

	public static void BianShenLoaderComplete(MonsterNPCLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		Animator component = go.GetComponent<Animator>();
		if (component != null)
		{
			component.SetBool("Stand", true);
			component.speed = 1f;
		}
		else
		{
			AnimationManager animationManager = go.AddComponent<AnimationManager>();
			animationManager.ChangeAnimation("Stand", 2, false, null, 0f);
			animationManager.EndAnimation = new AnimationChangeEventHandler(Modal3DShow.EndAnimation);
		}
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (null != modal3DShow)
		{
			U3DUtils.AddChild(modal3DShow.gameObject, go, false);
			modal3DShow._Target = go;
			go.name = "UI_Boss_Monster_" + loader.MonsterID;
			go.transform.localScale = Vector3.one * loader.scale;
			go.transform.Rotate(0f, 180f, 0f);
			U3DUtils.ReplaceLayerInChildren(go, loader.layer, null);
			UIHelper.SetReanderQueue(go);
			try
			{
				Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					for (int j = 0; j < componentsInChildren[i].sharedMaterials.Length; j++)
					{
						if (componentsInChildren[i].sharedMaterials[j] != null)
						{
							int renderQueue = componentsInChildren[i].sharedMaterials[j].renderQueue;
							componentsInChildren[i].sharedMaterials[j].shader = Shader.Find(componentsInChildren[i].sharedMaterials[j].shader.name);
							componentsInChildren[i].sharedMaterials[j].renderQueue = ((renderQueue != 2000) ? renderQueue : -1);
						}
						else
						{
							MUDebug.LogError<string>(new string[]
							{
								"sharedMaterial is null : " + componentsInChildren[i].name
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return;
		}
		Object.Destroy(go);
	}

	public static WingsResLoader LoadGoodsRes(Modal3DShow parant, int goodsID, float scale = 1f, float particleScale = 0.005f, int forge_Level = 0, string path = "Equip", bool canRotate = true)
	{
		WingsResLoader result = null;
		GameObject gameObject = new GameObject();
		if (null != gameObject)
		{
			scale = ((scale > 0f) ? scale : 1f);
			U3DUtils.AddChild(parant.gameObject, gameObject, false);
			parant._Target = gameObject;
			parant.CanRotate = canRotate;
			gameObject.name = "UI_Goods_" + goodsID;
			GoodsData fakeEquipGoodsData = Global.GetFakeEquipGoodsData(goodsID, forge_Level, 0);
			if (fakeEquipGoodsData != null)
			{
				WingsLoadData wingsLoadData = new WingsLoadData();
				wingsLoadData.parent = gameObject;
				wingsLoadData.ReplaceChildLayer = true;
				wingsLoadData.ToLayer = gameObject.layer;
				wingsLoadData.scale = scale * particleScale;
				wingsLoadData.Path = path;
				string name = gameObject.name;
				wingsLoadData.data = fakeEquipGoodsData;
				wingsLoadData.hangPointName = name;
				result = new WingsResLoader(wingsLoadData, new OnWingsLoadComplete(UIHelper.GoodsLoaderComplete));
			}
			gameObject.transform.localScale = new Vector3(scale, scale, scale);
		}
		return result;
	}

	public static void GoodsLoaderComplete(WingsLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		U3DUtils.LoadRoleShaderAgain(go);
		U3DUtils.AddChild(loader.parent, go, false);
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
		UIHelper.SetReanderQueue(go);
	}

	public static WingsLingyuResLoader LoadWuPinRes(Modal3DShow parant, string resName, string path, float scale = 1f)
	{
		GameObject gameObject = new GameObject();
		WingsLingyuResLoader result = null;
		if (null != gameObject)
		{
			U3DUtils.AddChild(parant.gameObject, gameObject, false);
			parant._Target = gameObject;
			gameObject.name = resName;
			gameObject.transform.localScale = new Vector3(scale, scale, scale);
			result = new WingsLingyuResLoader(new WingsLingYuLoadData
			{
				parent = gameObject,
				ReplaceChildLayer = true,
				ToLayer = gameObject.layer,
				EmptyName = resName,
				path = path,
				resName = resName
			}, new OnWingsLingYuLoadComplete(UIHelper.LingyuLoaderComplete));
		}
		return result;
	}

	public static void LingyuLoaderComplete(WingsLingYuLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		U3DUtils.LoadRoleShaderAgain(go);
		U3DUtils.AddChild(loader.parent, go, false);
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
	}

	public static ResourceResLoader LoadModelResource(Modal3DShow parant, int modelID, float scale = 1f, DPSelectedItemEventHandler handler = null)
	{
		ResourceResLoader result = null;
		GameObject gameObject = new GameObject();
		if (null != gameObject)
		{
			U3DUtils.AddChild(parant.gameObject, gameObject, false);
			parant._Target = gameObject;
			parant.LoadCompleteCallBack = handler;
			gameObject.name = "UI_Model_" + modelID;
			result = new ResourceResLoader(new ResourceLoadData
			{
				parent = gameObject,
				ReplaceChildLayer = true,
				ToLayer = gameObject.layer,
				modelID = modelID
			}, new OnResourceLoadComplete(UIHelper.ResourceLoaderComplete));
			gameObject.transform.localScale = new Vector3(scale, scale, scale);
		}
		return result;
	}

	public static void ResourceLoaderComplete(ResourceLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		U3DUtils.LoadRoleShaderAgain(go);
		U3DUtils.AddChild(loader.parent, go, false);
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (null != modal3DShow && modal3DShow.LoadCompleteCallBack != null)
		{
			modal3DShow.LoadCompleteCallBack(modal3DShow.gameObject, DPSelectedItemEventArgs.Empty);
		}
	}

	public static RoleResLoader LoadRoleRes(Modal3DShow parant, long SettingBitFlags, int occupation, int subOcc, string roleName, List<GoodsData> goodsDataList, List<GoodsData> PetDataList, WingData wingData, float scale = 1f, int FashionWingGoodsId = 0, DPSelectedItemEventHandler handler = null, bool LoadRebirthEquit = false)
	{
		RoleResLoader result = null;
		int myTimer = Global.GetMyTimer();
		string skeletonNameByOccupation = Global.GetSkeletonNameByOccupation(occupation);
		GameObject gameObject = U3DUtils.LoadSkeletonByName(skeletonNameByOccupation, false);
		myTimer = Global.GetMyTimer();
		parant.LoadCompleteCallBack = handler;
		if (null != gameObject)
		{
			parant.Add(gameObject, true);
			List<GameObject> list = new List<GameObject>();
			string[] nakePartsList = Global.GetNakePartsList(occupation);
			GameObject parent = gameObject;
			RoleLoaderData roleLoaderData = new RoleLoaderData();
			roleLoaderData.parent = parent;
			roleLoaderData.ForceSyncLoad = false;
			roleLoaderData.SubOccupation = subOcc;
			roleLoaderData.GoodsDataList = new List<GoodsData>();
			List<GoodsData> list2 = new List<GoodsData>();
			if (goodsDataList != null)
			{
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					int goodsID = goodsDataList[i].GoodsID;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
					if (goodsXmlNodeByID != null && goodsDataList[i].Using == 1 && ((goodsXmlNodeByID.Categoriy >= 11 && goodsXmlNodeByID.Categoriy <= 21) || goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10))
					{
						list2.Add(Global.GetFakeEquipGoodsData(goodsDataList[i].GoodsID, goodsDataList[i].Forge_level, goodsDataList[i].BagIndex));
					}
				}
				byte b = 0;
				int j = 0;
				int count = goodsDataList.Count;
				while (j < count)
				{
					if (Global.IsFashion(goodsDataList[j].GoodsID) && goodsDataList[j].Using == 1 && Global.GetCategoriyByGoodsID(goodsDataList[j].GoodsID) == 24)
					{
						roleLoaderData.GoodsDataList.AddRange(Global.GetFashionEquipGoodsDataList(goodsDataList[j].GoodsID, occupation, goodsDataList[j].Forge_level));
						roleLoaderData.GoodsDataList.AddRange(list2);
						b = 1;
						break;
					}
					j++;
				}
				if (b == 0)
				{
					roleLoaderData.GoodsDataList.AddRange(goodsDataList);
				}
				else
				{
					int k = 0;
					int count2 = goodsDataList.Count;
					while (k < count2)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataList[k].GoodsID);
						if (categoriyByGoodsID == 26 || categoriyByGoodsID == 25 || categoriyByGoodsID == 8)
						{
							roleLoaderData.GoodsDataList.Add(goodsDataList[k]);
						}
						k++;
					}
				}
			}
			roleLoaderData.PetDataList = PetDataList;
			roleLoaderData.SkeletonName = skeletonNameByOccupation;
			roleLoaderData.DefaultPartNames = nakePartsList;
			roleLoaderData.Occupation = occupation;
			roleLoaderData.SubOccupation = subOcc;
			roleLoaderData.ReplaceChildLayer = true;
			roleLoaderData.ToLayer = gameObject.layer;
			roleLoaderData.Scale = scale * 0.4f;
			roleLoaderData.RoleName = roleName;
			roleLoaderData.FashionWingGoodsID = FashionWingGoodsId;
			roleLoaderData.wingData = wingData;
			if (LoadRebirthEquit)
			{
				roleLoaderData.LoadRebitrhEquit = 1;
			}
			else
			{
				roleLoaderData.LoadRebitrhEquit = 0;
			}
			result = new RoleResLoader(roleLoaderData, new OnLoadRoleComplete(UIHelper.RoleLoaderComplete));
		}
		else if (parant.LoadCompleteCallBack != null)
		{
			parant.LoadCompleteCallBack(parant.gameObject, DPSelectedItemEventArgs.Empty);
		}
		return result;
	}

	public static void RoleLoaderComplete(RoleLoaderData loader, GameObject go)
	{
		Object.Destroy(loader.parent);
		U3DUtils.LoadRoleShaderAgain(go);
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (null != modal3DShow)
		{
			U3DUtils.AddChild(modal3DShow.gameObject, go, false);
			modal3DShow.ChildGameObjectList.Add(go);
			modal3DShow._Target = go;
		}
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
		go.transform.localPosition = new Vector3(0f, 0f, 0f);
		go.transform.localScale = new Vector3(225f, 225f, 225f) * loader.Scale;
		go.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
		GoodsData goodsData = null;
		string text = null;
		List<GoodsData> goodsDataList = loader.GoodsDataList;
		if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text))
		{
			Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text, loader.Occupation);
		}
		if (loader.FashionWingGoodsID > 0)
		{
			if (goodsData == null)
			{
				int @using = loader.wingData.Using;
				loader.wingData.Using = 1;
				Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text, loader.Occupation);
				loader.wingData.Using = @using;
			}
			if (goodsData != null)
			{
				goodsData.GoodsID = loader.FashionWingGoodsID;
				goodsData.Using = 1;
			}
		}
		if (goodsData != null)
		{
			new WingsResLoader(new WingsLoadData
			{
				parent = go,
				ReplaceChildLayer = loader.ReplaceChildLayer,
				ToLayer = loader.ToLayer,
				scale = loader.Scale,
				data = goodsData,
				hangPointName = text
			}, new OnWingsLoadComplete(UIHelper.WingsLoaderComplete));
		}
		ShouHuChongLoadData shouHuChongLoadData = new ShouHuChongLoadData();
		shouHuChongLoadData.parent = go;
		shouHuChongLoadData.Occupation = loader.Occupation;
		shouHuChongLoadData.ReplaceChildLayer = loader.ReplaceChildLayer;
		shouHuChongLoadData.ToLayer = loader.ToLayer;
		shouHuChongLoadData.Scale = loader.Scale;
		GoodsData goodsData2 = null;
		text = null;
		List<GoodsData> goodsDataList2;
		if (loader.PetDataList != null)
		{
			goodsDataList2 = loader.PetDataList;
		}
		else
		{
			goodsDataList2 = loader.GoodsDataList;
		}
		Global.ParseShouHuChongGoodsDataInfo(goodsDataList2, out goodsData2, out text);
		shouHuChongLoadData.data = goodsData2;
		shouHuChongLoadData.EmptyName = text;
		if (goodsData2 != null)
		{
			shouHuChongLoadData.Categoriy = (ItemCategories)Global.GetCategoriyByGoodsID(goodsData2.GoodsID);
		}
		shouHuChongLoadData.SpecialGameObjectsComplete = new AssetbundleLoaderComplete(UIHelper.ShouHuChongLoaderSpecialGameObjectsComplete);
		new ShouHuChongResLoader(shouHuChongLoadData, new OnShouHuChongLoadComplete(UIHelper.ShouHuChongLoaderComplete));
		List<GoodsData> goodsDataList3 = loader.GoodsDataList;
		List<GoodsData> list = new List<GoodsData>();
		List<string> list2 = new List<string>();
		List<string> safeEmptyNamesList = new List<string>();
		if (loader.LoadRebitrhEquit == 1)
		{
			Global.CheckBagIndex(goodsDataList3, Global.CheckRoleOcc(loader.Occupation, loader.SubOccupation));
		}
		Global.ParseWeaponGoodsDataInfo(goodsDataList3, list, list2, safeEmptyNamesList, Global.CheckRoleOcc(loader.Occupation, loader.SubOccupation));
		new WeaponResLoader(new WeaponLoadData
		{
			parent = go,
			occupation = Global.CheckRoleOcc(loader.Occupation, loader.SubOccupation),
			ReplaceChildLayer = loader.ReplaceChildLayer,
			ToLayer = loader.ToLayer,
			Scale = loader.Scale,
			weaponList = list,
			hangPointList = list2
		}, new OnWeaponLoadComplete(UIHelper.WeaponLoaderComplete));
		if (list.Count == 0)
		{
			UIHelper.SetPlayRoleActions(Global.CheckRoleOcc(loader.Occupation, loader.SubOccupation), list, go);
			if (modal3DShow != null && modal3DShow.LoadCompleteCallBack != null)
			{
				modal3DShow.LoadCompleteCallBack(modal3DShow.gameObject, DPSelectedItemEventArgs.Empty);
			}
		}
		try
		{
			if (loader.GoodsDataList != null && 0 < loader.GoodsDataList.Count)
			{
				GoodsData goodsData3 = null;
				for (int i = 0; i < loader.GoodsDataList.Count; i++)
				{
					GoodsData goodsData4 = loader.GoodsDataList[i];
					if (goodsData4 != null && Global.GetCategoriyByGoodsID(goodsData4.GoodsID) == 26 && 0 < goodsData4.Using)
					{
						goodsData3 = goodsData4;
						break;
					}
				}
				if (goodsData3 != null)
				{
					FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(ItemCategories.ShiZhuang_JiaoYin, goodsData3.GoodsID, goodsData3.Forge_level);
					if (fashionLevelupVO != null)
					{
						GDecoration decoration = Global.GetDecoration(fashionLevelupVO.Previev, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, LayerMask.NameToLayer("MUUI"), true, false);
						decoration.Parent = go.transform;
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"加载时装脚印出错"
			});
			MUDebug.LogException(ex);
		}
		UIHelper.SetReanderQueue(go);
	}

	public static HorseResLoader LoadHorseRes(Modal3DShow parant, int GoodsID, int Level, Quaternion quaternion, Vector3 scale, HorseMountCallBask Hander = null)
	{
		return new HorseResLoader(new HorseLoaderData
		{
			GoodsID = GoodsID,
			HorseLevel = Level,
			resName = Global.GetGoods3DResNameByID(GoodsID, -1),
			parent = parant.gameObject,
			Quaternion = quaternion,
			TransScale = scale,
			Hander = Hander
		}, new OnHorserLoaderComplete(UIHelper.HorseLoaderComplete));
	}

	private static void HorseLoaderComplete(HorseLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			if (loader.Hander != null)
			{
				loader.Hander(null);
			}
			return;
		}
		U3DUtils.LoadRoleShaderAgain(go);
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (null != modal3DShow)
		{
			U3DUtils.AddChild(modal3DShow.gameObject, go, false);
			modal3DShow.ChildGameObjectList.Add(go);
			modal3DShow._Target = go;
		}
		U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer("MUUI"), null);
		go.AddComponent<HorseAnimatorController>();
		go.transform.localPosition = new Vector3(0f, 0f, 0f);
		go.transform.localScale = loader.TransScale;
		go.transform.localRotation = loader.Quaternion;
		EffectArrayControl component = go.GetComponent<EffectArrayControl>();
		if (null != component)
		{
			component.ShowtParticle(true, true);
		}
		UIHelper.SetReanderQueue(go);
		if (loader.Hander != null)
		{
			loader.Hander(go);
		}
		LookAtCamera[] componentsInChildren = go.GetComponentsInChildren<LookAtCamera>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (null != componentsInChildren[i])
				{
					componentsInChildren[i].enabled = false;
					LookAtCamera_UI lookAtCamera_UI = componentsInChildren[i].GetComponent<LookAtCamera_UI>();
					if (null == lookAtCamera_UI)
					{
						lookAtCamera_UI = componentsInChildren[i].gameObject.AddComponent<LookAtCamera_UI>();
					}
				}
			}
		}
	}

	public static void WingsLoaderComplete(WingsLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		U3DUtils.LoadRoleShaderAgain(go);
		GameObject gameObject = U3DUtils.FindGameObjectByName(loader.parent, loader.hangPointName);
		if (null == gameObject)
		{
			Object.Destroy(go);
			return;
		}
		if (!Modal3DShow.AddChildToList(loader.parent, go))
		{
			Object.Destroy(go);
			return;
		}
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
		U3DUtils.AddChild(gameObject, go, true);
		UIHelper.SetReanderQueue(go);
	}

	public static void ShouHuChongLoaderComplete(ShouHuChongLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		U3DUtils.LoadRoleShaderAgain(go);
		if (null == go)
		{
			return;
		}
		if (!Modal3DShow.AddChildToList(loader.parent, go))
		{
			Object.Destroy(go);
			return;
		}
		ShouHuChongController shouHuChongController = go.GetComponent<ShouHuChongController>();
		if (shouHuChongController != null)
		{
			shouHuChongController.Dispose();
			Object.Destroy(shouHuChongController);
			shouHuChongController = null;
		}
		shouHuChongController = go.AddComponent<ShouHuChongController>();
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
		shouHuChongController.InitController(go, loader.parent.transform);
		U3DUtils.AddChild(loader.parent.transform.parent.gameObject, go, true);
		shouHuChongController.LoaderURL = loader.LoaderURL;
		go.transform.localScale = shouHuChongController.Target.localScale;
		Vector3 localScale = go.transform.localScale;
		PetFollow component = go.GetComponent<PetFollow>();
		if (component != null)
		{
			Transform transform = go.transform;
			if (loader.Categoriy == ItemCategories.ShouHuChong)
			{
				component.offsetX = localScale.x * -0.5f;
				component.offsetY = localScale.y * 1.5f;
				component.offsetZ = localScale.z * 0.5f;
			}
			else if (loader.Categoriy == ItemCategories.ChongWu)
			{
				component.offsetX = localScale.x * -0.5f;
				component.offsetY = 0f;
				component.offsetZ = localScale.z * 0.5f;
			}
			transform.localPosition = transform.localPosition + transform.forward * component.offsetX + transform.up * component.offsetY + transform.right * component.offsetZ;
			component.ActionRange = localScale.x * 0.2f;
			component.stopRange = localScale.x * 0.2f;
			component.PetItemEvent = delegate(object s, PetEventArgs e)
			{
				if (e.StepType == 1)
				{
					if (shouHuChongController != null && shouHuChongController.Action != GPetActions.Walk)
					{
						shouHuChongController.Action = GPetActions.Walk;
					}
				}
				else if (e.StepType == 2 && shouHuChongController != null && shouHuChongController.Action == GPetActions.Walk)
				{
					shouHuChongController.Action = GPetActions.Stand;
				}
			};
		}
		go.layer = LayerMask.NameToLayer("GUI");
		go.AddComponent<LoadRoleShaderAgain>();
		LookAtCamera[] componentsInChildren = go.GetComponentsInChildren<LookAtCamera>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (null != componentsInChildren[i])
				{
					componentsInChildren[i].enabled = false;
					LookAtCamera_UI lookAtCamera_UI = componentsInChildren[i].GetComponent<LookAtCamera_UI>();
					if (null == lookAtCamera_UI)
					{
						lookAtCamera_UI = componentsInChildren[i].gameObject.AddComponent<LookAtCamera_UI>();
					}
				}
			}
		}
		UIHelper.SetReanderQueue(go);
	}

	public static void ShouHuChongLoaderSpecialGameObjectsComplete(AssetbundleLoader loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		go.AddComponent<LoadRoleShaderAgain>();
	}

	public static void WeaponLoaderComplete(WeaponLoadData loader, List<GameObject> gameObjectList)
	{
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
		if (gameObjectList != null && gameObjectList.Count > 0)
		{
			for (int i = 0; i < gameObjectList.Count; i++)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(loader.parent, loader.hangPointList[i]);
				if (null == gameObject)
				{
					Object.Destroy(gameObjectList[i]);
				}
				else if (!Modal3DShow.AddChildToList(loader.parent, gameObjectList[i]))
				{
					Object.Destroy(gameObjectList[i]);
				}
				else
				{
					if (loader.ReplaceChildLayer)
					{
						U3DUtils.ReplaceLayerInChildren(gameObjectList[i], loader.ToLayer, null);
					}
					U3DUtils.LoadRoleShaderAgain(gameObject);
					U3DUtils.AddChild(gameObject, gameObjectList[i], true);
				}
			}
			UIHelper.SetPlayRoleActions(loader.occupation, loader.weaponList, loader.parent);
		}
		if (null != modal3DShow && modal3DShow.LoadCompleteCallBack != null)
		{
			modal3DShow.LoadCompleteCallBack(modal3DShow.gameObject, DPSelectedItemEventArgs.Empty);
		}
		if (gameObjectList != null)
		{
			for (int j = 0; j < gameObjectList.Count; j++)
			{
				UIHelper.SetReanderQueue(gameObjectList[j]);
			}
		}
	}

	public static void SetPlayRoleActions(int occupation, List<GoodsData> weaponGoodsDataList, GameObject go)
	{
		WeaponStates weaponState = Global.CalcWeaponState(weaponGoodsDataList, null, occupation);
		string text = string.Empty;
		PlayRoleActions playRoleActions = go.AddComponent<PlayRoleActions>();
		byte b = 0;
		List<GoodsData> list = new List<GoodsData>();
		for (int i = weaponGoodsDataList.Count - 1; i >= 0; i--)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(weaponGoodsDataList[i].GoodsID);
			if (categoriyByGoodsID == 25 && 0 < weaponGoodsDataList[i].Using)
			{
				WuQiShiZhuangMoXingVO wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion = ConfigFashion.GetWuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion(weaponGoodsDataList[i].GoodsID, occupation);
				if (wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion != null)
				{
					GoodsData goodsData = weaponGoodsDataList[i].Clone();
					GoodsData goodsData2 = weaponGoodsDataList[i].Clone();
					goodsData.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Left;
					goodsData.Id = -1;
					goodsData2.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Right;
					goodsData2.Id = -1;
					goodsData2.BagIndex = 1;
					if (0 < goodsData.GoodsID)
					{
						list.Add(goodsData);
						b += 1;
					}
					if (0 < goodsData2.GoodsID)
					{
						list.Add(goodsData2);
						b += 1;
					}
					break;
				}
			}
		}
		if (0 >= b)
		{
			for (int j = 0; j < weaponGoodsDataList.Count; j++)
			{
				int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(weaponGoodsDataList[j].GoodsID);
				if (11 <= categoriyByGoodsID2 && 21 >= categoriyByGoodsID2)
				{
					list.Add(weaponGoodsDataList[j]);
				}
			}
		}
		text = Global.GetSpecialSkillActionName(occupation, text, weaponState, out playRoleActions.StandName, list);
		playRoleActions.AttackName = text;
		playRoleActions.Occupation = occupation;
	}

	public static GDecoration GetDecoration(int code, GDecorationTypes decoType, Point pos, GameObject parent, int layer = -1, bool forceSyncLoad = true)
	{
		DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(code);
		string resName = decorationVOByCode.ResName;
		GDecoration gdecoration = new GDecoration(resName);
		gdecoration.OrigCoordinate = new Point(pos.X, pos.Y);
		gdecoration.cx = pos.X;
		gdecoration.cy = pos.Y;
		gdecoration.DecorationType = decoType;
		gdecoration.OwnerName = null;
		gdecoration.TriggerType = -1;
		gdecoration.Layer = layer;
		gdecoration.HangPos = decorationVOByCode.HangPos;
		gdecoration.SoundFileName = decorationVOByCode.Sound;
		gdecoration.ForceSyncLoad = forceSyncLoad;
		gdecoration.Start();
		return gdecoration;
	}

	public static List<int> ParserTimeArrayString2(string str)
	{
		List<int> list = new List<int>();
		if (str.Length >= 3 && str.IndexOf(':') >= 0)
		{
			string[] array = str.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					':'
				});
				if (array2.Length == 2)
				{
					int num = Convert.ToInt32(array2[0]);
					int num2 = Convert.ToInt32(array2[1]);
					list.Add(num * 3600 + num2 * 60);
				}
			}
		}
		return list;
	}

	public static int[] ParserGoodsString2(string goodsStr)
	{
		int[] result = null;
		if (!string.IsNullOrEmpty(goodsStr))
		{
			string[] array = goodsStr.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				int num = array[0].SafeToInt32(0);
				int num2 = array[1].SafeToInt32(0);
				if (num > 0 && num2 > 0)
				{
					result = new int[]
					{
						num,
						num2
					};
				}
			}
		}
		return result;
	}

	public static void ResetColiderBound(GameObject go, Collider collider = null)
	{
		if (collider == null)
		{
			collider = go.GetComponentInChildren<Collider>();
		}
		if (collider == null)
		{
			return;
		}
		if (collider is BoxCollider)
		{
			BoxCollider boxCollider = collider as BoxCollider;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(go.transform, collider.transform);
			boxCollider.isTrigger = true;
			boxCollider.center = new Vector3(bounds.center.x, bounds.center.y, 0.001f);
			boxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
		}
	}

	public static Camera FindCameraForLayer(int layer, int layerMask = -1, string name = null, string tag = null, bool activeOnly = true)
	{
		Camera[] array = NGUITools.FindActive<Camera>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			Camera camera = array[i];
			if (string.IsNullOrEmpty(name) || !(camera.name != name))
			{
				if (string.IsNullOrEmpty(tag) || camera.CompareTag(tag))
				{
					if (!activeOnly || (camera.gameObject.activeInHierarchy && camera.enabled))
					{
						if ((layerMask & camera.cullingMask) != 0)
						{
							if (camera.gameObject.layer == layer)
							{
								return camera;
							}
						}
					}
				}
			}
			i++;
		}
		return null;
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform root, Transform child, bool includeInactive = true)
	{
		UIWidget[] componentsInChildren = child.GetComponentsInChildren<UIWidget>(true);
		if (componentsInChildren.Length == 0)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		Vector3 vector;
		vector..ctor(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2;
		vector2..ctor(float.MinValue, float.MinValue, float.MinValue);
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
		int i = 0;
		int num = componentsInChildren.Length;
		while (i < num)
		{
			UIWidget uiwidget = componentsInChildren[i];
			Vector2 vector3 = uiwidget.relativeSize;
			Vector2 pivotOffset = uiwidget.pivotOffset;
			Transform cachedTransform = uiwidget.cachedTransform;
			float num2 = (pivotOffset.x + 0.5f) * vector3.x;
			float num3 = (pivotOffset.y - 0.5f) * vector3.y;
			vector3 *= 0.5f;
			Vector3 vector4;
			vector4..ctor(num2 - vector3.x, num3 - vector3.y, 0f);
			vector4 = cachedTransform.TransformPoint(vector4);
			vector4 = worldToLocalMatrix.MultiplyPoint3x4(vector4);
			vector2 = Vector3.Max(vector4, vector2);
			vector = Vector3.Min(vector4, vector);
			vector4..ctor(num2 - vector3.x, num3 + vector3.y, 0f);
			vector4 = cachedTransform.TransformPoint(vector4);
			vector4 = worldToLocalMatrix.MultiplyPoint3x4(vector4);
			vector2 = Vector3.Max(vector4, vector2);
			vector = Vector3.Min(vector4, vector);
			vector4..ctor(num2 + vector3.x, num3 - vector3.y, 0f);
			vector4 = cachedTransform.TransformPoint(vector4);
			vector4 = worldToLocalMatrix.MultiplyPoint3x4(vector4);
			vector2 = Vector3.Max(vector4, vector2);
			vector = Vector3.Min(vector4, vector);
			vector4..ctor(num2 + vector3.x, num3 + vector3.y, 0f);
			vector4 = cachedTransform.TransformPoint(vector4);
			vector4 = worldToLocalMatrix.MultiplyPoint3x4(vector4);
			vector2 = Vector3.Max(vector4, vector2);
			vector = Vector3.Min(vector4, vector);
			i++;
		}
		Bounds result;
		result..ctor(vector, Vector3.zero);
		result.Encapsulate(vector2);
		return result;
	}

	public static Vector3 CalculateRelativePosition(Transform to, Transform from, Vector3 position)
	{
		Matrix4x4 worldToLocalMatrix = to.worldToLocalMatrix;
		Vector3 vector = from.TransformPoint(position);
		return worldToLocalMatrix.MultiplyPoint3x4(vector);
	}

	public static Vector3 SetPosY(Transform trans, float VarValue, byte VType = 0)
	{
		Vector3 localPosition = trans.localPosition;
		if (VType == 0)
		{
			localPosition.x = VarValue;
		}
		else if (VType == 1)
		{
			localPosition.y = VarValue;
		}
		else if (VType == 2)
		{
			localPosition.z = VarValue;
		}
		trans.localPosition = localPosition;
		return localPosition;
	}

	public static void SetModalPosZ(Transform trans)
	{
		Vector3 localPosition = trans.localPosition;
		localPosition.z = -500f;
		trans.localPosition = localPosition;
	}

	public static IEnumerator DoActionOnEndFrame(Action action)
	{
		yield return new WaitForEndOfFrame();
		action.Invoke();
		yield break;
	}

	public static void DelayInvoke(float delay, EventHandler handler)
	{
		DispatcherTimer UITimer = new DispatcherTimer("DelayInvoke_Timer" + Random.Range(0, int.MaxValue));
		UITimer.Interval = TimeSpan.FromMilliseconds((double)(delay * 1000f));
		UITimer.Tick = delegate(object s, EventArgs e)
		{
			UITimer.Stop();
			UITimer.Tick = null;
			if (handler != null)
			{
				handler.Invoke(s, e);
			}
		};
		UITimer.Start();
	}

	public static void ResetDelayInvokeRunning()
	{
		UIHelper.Running = false;
	}

	public static void DelayInvokeOnlyOnce(float delay, EventHandler handler)
	{
		if (UIHelper.Running)
		{
			return;
		}
		UIHelper.Running = true;
		DispatcherTimer UITimer = new DispatcherTimer("DelayInvokeOnlyOnce_Timer" + Random.Range(0, int.MaxValue));
		UITimer.Interval = TimeSpan.FromMilliseconds((double)(delay * 1000f));
		UITimer.Tick = delegate(object s, EventArgs e)
		{
			UIHelper.Running = false;
			UITimer.Stop();
			UITimer.Tick = null;
			if (handler != null)
			{
				handler.Invoke(s, e);
			}
		};
		UITimer.Start();
	}

	public static void AddPorpsStringToDict(string str, Dictionary<int, int> dict)
	{
		if (dict == null)
		{
			return;
		}
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2 != null)
				{
					int num = ExtPropIndexes.ExtPropIndexNames.IndexOf(array2[0].ToLower());
					int num2 = array2[1].SafeToInt32(0);
					if (!dict.ContainsKey(num))
					{
						dict.Add(num, num2);
					}
					else
					{
						int num4;
						int num3 = num4 = num;
						num4 = dict[num4];
						dict[num3] = num4 + num2;
					}
				}
			}
		}
	}

	public static string GetFormatPropsStrFromPropsDict(Dictionary<int, int> dict)
	{
		string text = string.Empty;
		if (dict == null)
		{
			return string.Empty;
		}
		Dictionary<int, int>.Enumerator enumerator = dict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			string text2 = text;
			object[] array = new object[4];
			array[0] = "E5BB6F";
			int num = 1;
			string text3 = "{0}: ";
			string[] extPropIndexChineseNames = ExtPropIndexes.ExtPropIndexChineseNames;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			array[num] = string.Format(text3, Global.GetLang(extPropIndexChineseNames[keyValuePair.Key]));
			array[2] = "F5E3BB";
			int num2 = 3;
			string text4 = "{0}";
			KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
			array[num2] = string.Format(text4, keyValuePair2.Value);
			text = text2 + Global.GetColorStringForNGUIText(array);
			text += "\n";
		}
		return text;
	}

	public static string GetBaseAttributeStr(GoodsData gd, double[] equipFields_1, int categoriy = -1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 1; i <= 10; i += 2)
		{
			text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
			if (i == 1)
			{
				if (equipFields_1[i] != 0.0)
				{
					text += string.Format("{0}: {1}%", text2, (int)equipFields_1[i]);
					text += "\n";
				}
			}
			else
			{
				int num = i;
				int num2 = i + 1;
				if (equipFields_1[num] != 0.0 || equipFields_1[num2] != 0.0)
				{
					double num3 = equipFields_1[num];
					double num4 = equipFields_1[num2];
					text += string.Format("{0}: {1} - {2}", text2, (int)num3, (int)num4);
					text += "\n";
				}
			}
		}
		for (int i = 11; i < 177; i++)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num5 = equipFields_1[i];
				double equipForgeAddBaseValue = Global.GetEquipForgeAddBaseValue(gd, i);
				if (ExtPropIndexes.ExtPropIndexPercents[i] == 1)
				{
					text += string.Format("{0}: {1}%", text2, (int)(num5 * 100.0));
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[i] == 0)
				{
					text += string.Format("{0}: {1}", text2, (int)num5);
				}
				text += "\n";
			}
		}
		return text.TrimEnd(new char[0]);
	}

	public static bool IsGongNengOpenedOrHint(GongNengIDs id, bool hint)
	{
		int trigger = 0;
		int param = 0;
		int param2 = 0;
		if (GongnengYugaoMgr.IsGongNengOpened(id, ref trigger, ref param, ref param2))
		{
			return true;
		}
		UIHelper.HintGongNengOpenCondition(id, trigger, param, param2, true);
		return false;
	}

	public static bool IsGongNengOpenedOrHint(GongNengIDs id, bool hint, out string msg)
	{
		int trigger = 0;
		int param = 0;
		int param2 = 0;
		if (GongnengYugaoMgr.IsGongNengOpened(id, ref trigger, ref param, ref param2))
		{
			msg = string.Empty;
			return true;
		}
		msg = UIHelper.HintGongNengOpenCondition(id, trigger, param, param2, hint);
		return false;
	}

	public static string HintGongNengOpenCondition(GongNengIDs id, int trigger, int param1, int param2, bool hint = true)
	{
		string text = string.Empty;
		if (trigger == 7)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(param1);
			if (taskXmlNodeByID != null)
			{
				if (Global.GetUnionLevel(taskXmlNodeByID.MinZhuanSheng, taskXmlNodeByID.MinLevel) > 0)
				{
					text = string.Format(Global.GetLang("需要完成{0}转{1}级的【{2}】主线任务才可开启{3}系统"), new object[]
					{
						taskXmlNodeByID.MinZhuanSheng,
						taskXmlNodeByID.MinLevel,
						taskXmlNodeByID.Title,
						GongnengYugaoMgr.GetGongNengName(id)
					});
				}
				else
				{
					text = string.Format(Global.GetLang("需要完成【{2}】主线任务才可开启{3}系统"), new object[]
					{
						taskXmlNodeByID.MinZhuanSheng,
						taskXmlNodeByID.MinLevel,
						taskXmlNodeByID.Title,
						GongnengYugaoMgr.GetGongNengName(id)
					});
				}
			}
			else
			{
				text = string.Format(Global.GetLang("完成【{0}】主线任务可开启该系统"), param1);
			}
		}
		else if (trigger == 14)
		{
			text = string.Format(Global.GetLang("翅膀达到【{0}】阶可开启{1}系统"), param1, GongnengYugaoMgr.GetGongNengName(id));
		}
		else if (trigger == 20)
		{
			text = string.Format(Global.GetLang("战盟等级达到【{0}】级可开启{1}系统"), param1, GongnengYugaoMgr.GetGongNengName(id));
		}
		else if (trigger == 21)
		{
			text = string.Format(Global.GetLang("需要重生等级到达【{0}】级才可开启{1}系统"), param1, GongnengYugaoMgr.GetGongNengName(id));
		}
		else if (trigger == 17)
		{
			int days = (Global.GetCorrectDateTime() - Global.GetServerStartTime()).Days;
			int num = param1 - days;
			if (num == 0)
			{
				text = string.Empty;
			}
			else if (num == 1)
			{
				text = Global.GetLang("明日开启");
			}
			else
			{
				text = string.Format(Global.GetLang("{0}天后开启"), num);
			}
		}
		else if (trigger == 18)
		{
			if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.MoneyData != null)
			{
				int num2 = (int)Global.Data.roleData.MoneyData[137];
				int num3 = param1 - num2;
				text = string.Format(Global.GetLang("登陆{0}天后开启"), num3);
			}
		}
		else
		{
			text = string.Format(Global.GetLang("达到【{0}】可开启{1}系统"), UIHelper.FormatLevelLimit(param2, param1), GongnengYugaoMgr.GetGongNengName(id));
		}
		if (hint)
		{
			Super.HintMainText(text, 10, 3);
		}
		return text;
	}

	public static string[] ZuoyueTitleNames = new string[]
	{
		Global.GetLang("卓越的"),
		Global.GetLang("完美的"),
		Global.GetLang("传说的")
	};

	private static bool Running = false;

	public class TextStyle
	{
		public static UIHelper.TextStyle AwardsText;
	}
}
