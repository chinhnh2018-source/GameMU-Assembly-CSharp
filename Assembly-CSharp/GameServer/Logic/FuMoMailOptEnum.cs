using System;
using HSGameEngine.GameEngine.Logic;

namespace GameServer.Logic
{
	public static class FuMoMailOptEnum
	{
		public static string ErrorFuMo(int error)
		{
			string result = string.Empty;
			string text = string.Empty;
			switch (error + 1)
			{
			case 0:
				result = Global.GetLang("失败");
				break;
			case 1:
				result = Global.GetLang("附魔币邮件接受失败");
				break;
			case 2:
				result = Global.GetLang("操作成功");
				break;
			case 3:
				text = ConfigSystemParam.GetSystemParamStringArrayByName("EnchantmentGiveLimit", ',')[1];
				result = string.Format(Global.GetLang("今日最多可领取{0}个好友赠送的附魔灵石，请明天再来吧~~"), text);
				break;
			case 4:
				result = Global.GetLang("接受条件不足");
				break;
			case 5:
				result = Global.GetLang("今日赠送附魔灵石数量已达到上限！");
				break;
			case 6:
				result = Global.GetLang("好友列表出错");
				break;
			case 7:
				text = ConfigSystemParam.GetSystemParamByName("FuMoHuoyue", true);
				result = string.Format(Global.GetLang("您今日的活跃不满{0}，完成每日可做任务可获得活跃值"), text);
				break;
			case 8:
				result = Global.GetLang("重复赠送");
				break;
			case 9:
				result = Global.GetLang("附魔灵石执行数据出错");
				break;
			case 10:
				result = Global.GetLang("获取接受次数出错");
				break;
			case 11:
				result = Global.GetLang("获取邮件列表出错");
				break;
			case 12:
				result = Global.GetLang("获取物品信息出错");
				break;
			case 13:
				result = Global.GetLang("装备职业不同");
				break;
			case 14:
				result = Global.GetLang("装备左右类型不同");
				break;
			case 15:
				result = Global.GetLang("装备不在背包中");
				break;
			case 16:
				result = Global.GetLang("当前没有可附魔的货币，快去找好友赠送吧~~");
				break;
			case 17:
				result = Global.GetLang("没有附魔属性");
				break;
			case 18:
				result = Global.GetLang("改装备不可附魔");
				break;
			case 19:
				result = Global.GetLang("该好友附魔功能未开启");
				break;
			case 20:
				result = Global.GetLang("功能未开启");
				break;
			case 21:
				result = Global.GetLang("获取随机属性出错");
				break;
			case 22:
				result = Global.GetLang("附魔保存出错");
				break;
			case 24:
				result = Global.GetLang("统一数据库执行出错");
				break;
			case 25:
				result = Global.GetLang("赠送玩家信息不存在");
				break;
			case 26:
				result = Global.GetLang(" 附魔传承金币不足");
				break;
			case 27:
				result = Global.GetLang("附魔传承钻石不足");
				break;
			}
			return result;
		}

		public const int FuMo_Fail = -1;

		public const int FuMo_AcceptFail = 0;

		public const int FuMo_AcceptSucc = 1;

		public const int FuMo_AcceptMaxTime = 2;

		public const int FuMo_AcceptCondition = 3;

		public const int FuMo_GiveFuMoMoneyMax = 4;

		public const int FuMo_NotFriend = 5;

		public const int FuMo_GiveFuMoMoneyActive = 6;

		public const int FuMo_GiveFuMoMoneyRepeat = 7;

		public const int FuMo_RunFuMoDBError = 8;

		public const int FuMo_GetAcceptError = 9;

		public const int FuMo_GetMailListError = 10;

		public const int FuMo_GetGoodInfo = 11;

		public const int FuMo_EquipJobDiff = 12;

		public const int FuMo_LiftRightEquipDiff = 13;

		public const int FuMo_EquipNotInGoods = 14;

		public const int FuMo_MoneyError = 15;

		public const int FuMo_FuMoAttrError = 16;

		public const int FuMo_NotFuMoType = 17;

		public const int FuMo_OtherGongNengWeiKaiQi = 18;

		public const int FuMo_GongNengWeiKaiQi = 19;

		public const int FuMo_GetRandomAttrError = 20;

		public const int FuMo_SaveError = 21;

		public const int FuMo_RoleInfoError = 22;

		public const int FuMo_DBError = 23;

		public const int FuMo_GiveOtherError = 24;

		public const int FuMo_JinBiLacking = 25;

		public const int FuMo_ZuanShiLacking = 26;
	}
}
