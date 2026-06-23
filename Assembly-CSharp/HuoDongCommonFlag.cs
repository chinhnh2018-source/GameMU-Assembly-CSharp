using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public class HuoDongCommonFlag : CommonFlagX
{
	public static LinkedList<string> TopIconTreeActivityActivedLst()
	{
		HuoDongCommonFlag._TopIconTreeActivityActivedLst.Clear();
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_RegressActive))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("HuiGuiHuoDong");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_OneDollarChongZhi))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("XingYunYiYuan");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.ChaoJiYiYuan))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("ChaoJiYiYuan");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_InputFanLiNew))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("EveryDayFanLi");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.JieRi) || Global.IsInJieriActivity())
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("JieRi");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.ZhaoHui) || ConfigLaoWanJiaZhaoHui.IsInPlayerRecallActivity())
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("ZhaoHui");
		}
		if (Global.IsInOlympicsActivity())
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("Olympics");
		}
		if (Global.IsTuiGuangOpen(true))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("TuiGuang");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.ZhuanShu))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("ZhuanShu");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_ThemeActivity) || Global.IsInZhuTiFuActivity())
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("ZhuTiFu");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_Everyday))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("EveryDay");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_JunTuanEra))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("JiYuan");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AoYunDaTi))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("AoYunDaTi");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SpecialPrirotiy))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("TeQuanHuoDong");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_BoCaiCaiShuZi))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("CaiShuZi");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_BoCaiCaiDaXiao))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("CaiDaXiao");
		}
		if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_DreamStone))
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.AddLast("MengHuanStone");
		}
		return HuoDongCommonFlag._TopIconTreeActivityActivedLst;
	}

	public static Dictionary<OpenActivityType, OpenActivityState> OpenActivityStateDic
	{
		get
		{
			if (HuoDongCommonFlag._OpenActivityStateDic == null)
			{
				HuoDongCommonFlag._OpenActivityStateDic = new Dictionary<OpenActivityType, OpenActivityState>();
			}
			return HuoDongCommonFlag._OpenActivityStateDic;
		}
	}

	public static void SetActivityState(OpenActivityType type, OpenActivityState state)
	{
		if (!HuoDongCommonFlag.OpenActivityStateDic.ContainsKey(type))
		{
			HuoDongCommonFlag.OpenActivityStateDic.Add(type, state);
		}
		else
		{
			HuoDongCommonFlag.OpenActivityStateDic[type] = state;
		}
	}

	public static bool IsActivityStateBegin(OpenActivityType type)
	{
		return HuoDongCommonFlag.OpenActivityStateDic.ContainsKey(type) && HuoDongCommonFlag.OpenActivityStateDic[type] == OpenActivityState.Begin;
	}

	public static LinkedList<string> GetTopIconBoxDownRightIconIsOpen()
	{
		LinkedList<string> linkedList = new LinkedList<string>();
		if (!Global.IsXinFuActivityEnd())
		{
			linkedList.AddLast("XinFu");
		}
		if (Global.IsInHefuActivity())
		{
			linkedList.AddLast("Hefu");
		}
		if (Global.GetQiRiKuanghuanDaysNum() <= 7)
		{
			linkedList.AddLast("QiRiKuanghuan");
		}
		if (Global.IsOpenGrowFund())
		{
			linkedList.AddLast("Fund");
		}
		return linkedList;
	}

	public static Dictionary<ActivityTipTypes, bool> LaoWanJiaActivityTipTypesStateDic
	{
		get
		{
			if (HuoDongCommonFlag._LaoWanJiaActivityTipTypesStateDic == null)
			{
				HuoDongCommonFlag._LaoWanJiaActivityTipTypesStateDic = new Dictionary<ActivityTipTypes, bool>();
			}
			return HuoDongCommonFlag._LaoWanJiaActivityTipTypesStateDic;
		}
	}

	public static void SetLaoWanJiaActivityTipTypesState(ActivityTipTypes type, bool state)
	{
		if (HuoDongCommonFlag.LaoWanJiaActivityTipTypesStateDic.ContainsKey(type))
		{
			HuoDongCommonFlag.LaoWanJiaActivityTipTypesStateDic[type] = state;
		}
		else
		{
			HuoDongCommonFlag.LaoWanJiaActivityTipTypesStateDic.Add(type, state);
		}
	}

	public static bool GetLaoWanJiaActivityTipTypesState(ActivityTipTypes type)
	{
		if (type == ActivityTipTypes.Recall_GiftSet || type == ActivityTipTypes.Recall_SignIn)
		{
		}
		return HuoDongCommonFlag.LaoWanJiaActivityTipTypesStateDic.ContainsKey(type) && HuoDongCommonFlag.LaoWanJiaActivityTipTypesStateDic[type];
	}

	public new static void ClearStaticData()
	{
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(14100))
		{
			ActivityTipManager.ActivityTipItemDict[14100].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(14101))
		{
			ActivityTipManager.ActivityTipItemDict[14101].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(14115))
		{
			ActivityTipManager.ActivityTipItemDict[14115].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(14102))
		{
			ActivityTipManager.ActivityTipItemDict[14102].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(14103))
		{
			ActivityTipManager.ActivityTipItemDict[14103].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(14104))
		{
			ActivityTipManager.ActivityTipItemDict[14104].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(18000))
		{
			ActivityTipManager.ActivityTipItemDict[18000].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(15053))
		{
			ActivityTipManager.ActivityTipItemDict[15053].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(11501))
		{
			ActivityTipManager.ActivityTipItemDict[11501].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(11502))
		{
			ActivityTipManager.ActivityTipItemDict[11502].IsActive = false;
		}
		if (ActivityTipManager.ActivityTipItemDict.ContainsKey(15054))
		{
			ActivityTipManager.ActivityTipItemDict[15054].IsActive = false;
		}
		HuoDongCommonFlag._OpenActivityStateDic = null;
		if (HuoDongCommonFlag._TopIconTreeActivityActivedLst != null)
		{
			HuoDongCommonFlag._TopIconTreeActivityActivedLst.Clear();
		}
		HuoDongCommonFlag.LaoWanJiaActivityTipTypesStateDic.Clear();
		ServerBufferZhaoHui.ClearStaticData();
		JingLingMap.ClearStaticData();
		KuaFuPlunderMap.ClearStaticData();
		CommonFlagX.ClearStaticData();
	}

	private static LinkedList<string> _TopIconTreeActivityActivedLst = new LinkedList<string>();

	private static Dictionary<OpenActivityType, OpenActivityState> _OpenActivityStateDic = null;

	private static Dictionary<ActivityTipTypes, bool> _LaoWanJiaActivityTipTypesStateDic = null;
}
