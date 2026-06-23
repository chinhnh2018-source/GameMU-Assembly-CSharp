using System;
using System.Collections.Generic;
using Server.Tools.Pattern;

namespace GameServer.Logic.Goods
{
	public class GoodsCanUseManager : SingletonTemplate<GoodsCanUseManager>
	{
		private GoodsCanUseManager()
		{
		}

		public void Init()
		{
			this.canUseDict["WingSuit".ToLower()] = new CondJudger_WingSuit();
			this.canUseDict["ChengJiuLevel".ToLower()] = new CondJudger_ChengJiuLvl();
			this.canUseDict["JunXianLevel".ToLower()] = new CondJudger_JunXianLvl();
			this.canUseDict["ZhuanShengLevel".ToLower()] = new CondJudger_ChangeLife();
			this.canUseDict["Level".ToLower()] = new CondJudger_RoleLevel();
			this.canUseDict["VIP".ToLower()] = new CondJudger_VIPLvl();
			this.canUseDict["HuFuSuit".ToLower()] = new CondJudger_HuFuSuit();
			this.canUseDict["DaTianShiSuit".ToLower()] = new CondJudger_DaTianShiSuit();
			this.canUseDict["NeedMarry".ToLower()] = new CondJudger_NeedMarry();
			this.canUseDict["NeedTask".ToLower()] = new CondJudger_NeedTask();
			this.canUseDict["AddIntoBH".ToLower()] = new CondJudger_AddIntoBH();
			this.canUseDict["CanNotBeyondLevel".ToLower()] = new CondJudger_CannotBeyongLevel();
			this.canUseDict["FEIANQUANQU".ToLower()] = new CondJudger_NotSafeRegion();
			this.canUseDict["UseYuanBao".ToLower()] = new CondJudger_YuanBaoMoreThan();
			this.canUseDict["NeedOpen".ToLower()] = new CondJudger_NeedOpen();
			this.canUseDict["NoBuff".ToLower()] = new CondJudger_NoBuff();
			this.canUseDict["WingSuitLess".ToLower()] = new CondJudger_WingSuitLess();
			this.canUseDict["WingNotPerfect".ToLower()] = new CondJudger_WingNotPerfect();
			this.canUseDict["EquipUsingAll".ToLower()] = new CondJudger_EquipUsingAll();
			this.canUseDict["EquipUsing".ToLower()] = new CondJudger_EquipUsing();
			this.canUseDict["FruitSuitNotMax".ToLower()] = new CondJudger_FruitSuitNotMax();
			this.canUseDict["XingHunNotMax".ToLower()] = new CondJudger_XingHunNotMax();
			this.canUseDict["MerlinLess".ToLower()] = new CondJudger_MerlinLess();
		}

		public bool CheckCanUse_ByToType(GameClient client, int goodsID)
		{
			string empty = string.Empty;
			return this.CheckCanUse_ByToType(client, goodsID, out empty);
		}

		public bool CheckCanUse_ByToType(GameClient client, int goodsID, out string failedMsg)
		{
			failedMsg = "";
			SystemXmlItem systemXmlItem = null;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
			{
				result = false;
			}
			else
			{
				string text = systemXmlItem.GetStringValue("ToType");
				string stringValue = systemXmlItem.GetStringValue("ToTypeProperty");
				if (string.IsNullOrEmpty(text) || text == "-1")
				{
					result = true;
				}
				else
				{
					text = text.ToLower();
					ICondJudger condJudger = null;
					result = (!this.canUseDict.TryGetValue(text, out condJudger) || condJudger.Judge(client, stringValue, out failedMsg));
				}
			}
			return result;
		}

		private Dictionary<string, ICondJudger> canUseDict = new Dictionary<string, ICondJudger>();
	}
}
