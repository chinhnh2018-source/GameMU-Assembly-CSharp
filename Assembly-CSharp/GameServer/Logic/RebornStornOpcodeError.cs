using System;
using HSGameEngine.GameEngine.Logic;

namespace GameServer.Logic
{
	public static class RebornStornOpcodeError
	{
		public static string ErrorChongShengLianLu(int error)
		{
			string result = string.Empty;
			switch (error)
			{
			case 1:
				result = Global.GetLang("孔操作成功");
				break;
			case 2:
				result = Global.GetLang("重生背包不存在该物品");
				break;
			case 3:
				result = Global.GetLang("要打孔的装备不是重生装备");
				break;
			case 4:
				result = Global.GetLang("生成孔信息错误");
				break;
			case 5:
				result = Global.GetLang("使用道具出错");
				break;
			case 6:
				result = Global.GetLang("更新数据出错");
				break;
			case 7:
				result = Global.GetLang("重生背包不存在该物品");
				break;
			case 8:
				result = Global.GetLang("没有发现当前装备的最优孔");
				break;
			case 9:
				result = Global.GetLang("不存在当前道具");
				break;
			case 10:
				result = Global.GetLang("消耗材料为空");
				break;
			case 11:
				result = Global.GetLang("更改对应空位置出错");
				break;
			case 12:
				result = Global.GetLang("得到随机品质凹槽失败");
				break;
			case 13:
				result = Global.GetLang("已经是最优品质");
				break;
			case 14:
				result = Global.GetLang("该位置已经打孔");
				break;
			case 15:
				result = Global.GetLang("绑定参数出错");
				break;
			case 16:
				result = Global.GetLang("该位置还没有打孔");
				break;
			case 20:
				result = Global.GetLang("重生宝石镶嵌成功");
				break;
			case 21:
				result = Global.GetLang("背包里没有当前装备");
				break;
			case 22:
				result = Global.GetLang("背包里没有当前宝石");
				break;
			case 23:
				result = Global.GetLang("该位置上已经有宝石");
				break;
			case 24:
				result = Global.GetLang("不是重生装备");
				break;
			case 25:
				result = Global.GetLang("该重生装备还没有打孔");
				break;
			case 26:
				result = Global.GetLang("该位置没有孔");
				break;
			case 27:
				result = Global.GetLang("镶嵌必须为装备");
				break;
			case 28:
				result = Global.GetLang("获取孔信息出错");
				break;
			case 29:
				result = Global.GetLang("当前宝石不是炫彩宝石");
				break;
			case 30:
				result = Global.GetLang("当前宝石不是重生宝石");
				break;
			case 31:
				result = Global.GetLang("更新装备数据出错");
				break;
			case 32:
				result = Global.GetLang("当前位置有宝石");
				break;
			case 33:
				result = Global.GetLang("更新背包宝石数据出错");
				break;
			case 35:
				result = Global.GetLang("当前位置没有炫彩宝石");
				break;
			case 36:
				result = Global.GetLang("当前玩家没有炫彩宝石");
				break;
			case 37:
				result = Global.GetLang("当前位置没有重生宝石");
				break;
			case 38:
				result = Global.GetLang("当前玩家没有重生宝石");
				break;
			case 39:
				result = Global.GetLang("要卸下的宝石信息出错");
				break;
			case 40:
				result = Global.GetLang("卸下宝石更新数据出错");
				break;
			case 45:
				result = Global.GetLang("没有找到重生宝石");
				break;
			case 46:
				result = Global.GetLang("封印晶石不足");
				break;
			case 47:
				result = Global.GetLang("重生晶石不足");
				break;
			case 48:
				result = Global.GetLang("炫彩晶石不足");
				break;
			case 49:
				result = Global.GetLang("封印晶石消耗出错");
				break;
			case 50:
				result = Global.GetLang("重生晶石消耗出错");
				break;
			case 51:
				result = Global.GetLang("炫彩晶石消耗出错");
				break;
			case 52:
				result = Global.GetLang("重生宝石合成出错");
				break;
			case 53:
				result = Global.GetLang("要合成重生宝石不是物品");
				break;
			case 54:
				result = Global.GetLang("重生宝石合成成功");
				break;
			case 55:
				result = Global.GetLang("重生宝石分解成功");
				break;
			case 56:
				result = Global.GetLang("没有发现分解的宝石");
				break;
			case 57:
				result = Global.GetLang("该宝石正在被使用");
				break;
			case 58:
				result = Global.GetLang("要分解的物品不是宝石");
				break;
			case 59:
				result = Global.GetLang("分解宝石失败");
				break;
			case 60:
				result = Global.GetLang("分解添加封印晶石出错");
				break;
			case 61:
				result = Global.GetLang("分解添加重生晶石出错");
				break;
			case 62:
				result = Global.GetLang("分解添加炫彩晶石出错");
				break;
			case 63:
				result = Global.GetLang("要分解重生宝石不是物品");
				break;
			case 64:
				result = Global.GetLang("物品配置出错");
				break;
			case 70:
				result = Global.GetLang("炫彩宝石合成成功");
				break;
			case 71:
				result = Global.GetLang("没有找到对应的炫彩宝石");
				break;
			case 72:
				result = Global.GetLang("物品配置出错");
				break;
			case 73:
				result = Global.GetLang("不是相同等级");
				break;
			case 74:
				result = Global.GetLang("最大等级");
				break;
			case 75:
				result = Global.GetLang("使用道具出错");
				break;
			case 76:
				result = Global.GetLang("不存在合成后的炫彩宝石");
				break;
			case 77:
				result = Global.GetLang("不能使用当前宝石合成");
				break;
			case 78:
				result = Global.GetLang("合成添加到背包出错");
				break;
			case 80:
				result = Global.GetLang("重生宝石一键分解成功");
				break;
			case 81:
				result = Global.GetLang("一键分解添加封印晶石出错");
				break;
			case 82:
				result = Global.GetLang("一键分解添加重生晶石出错");
				break;
			case 83:
				result = Global.GetLang("一键分解添加炫彩晶石出错");
				break;
			case 85:
				result = Global.GetLang("重生宝石购买个数出错");
				break;
			case 86:
				result = Global.GetLang("重生宝石分解次数出错");
				break;
			case 87:
				result = Global.GetLang("当前重生宝石要分解的个数不足");
				break;
			case 90:
				result = Global.GetLang("炫彩宝石等级获取出错");
				break;
			case 91:
				result = Global.GetLang("具信息错误");
				break;
			}
			return result;
		}

		public const int RebornHoleSucc = 1;

		public const int RebornNotExist = 2;

		public const int RebornNotEquip = 3;

		public const int RebornMakeHoleErr = 4;

		public const int RebornUseMaterrislErr = 5;

		public const int RebornUpdateInfoErr = 6;

		public const int RebornNotInfo = 7;

		public const int RebornNotFindMaxQuality = 8;

		public const int RebornNotExistGoods = 9;

		public const int RebornUseMatterrislNull = 10;

		public const int RebornHoleSiteErr = 11;

		public const int RebornRandomHoleErr = 12;

		public const int RebornMaxQuality = 13;

		public const int RebornHasHole = 14;

		public const int RebornUseBind = 15;

		public const int RebornNotHasHole = 16;

		public const int RebornInlaySucc = 20;

		public const int RebornInlayNotExistEquip = 21;

		public const int RebornInlayNotExistStone = 22;

		public const int RebornInlayHaveStone = 23;

		public const int RebornInlayNotEquip = 24;

		public const int RebornInlayNotMakeHole = 25;

		public const int RebornInlayNotHoleSite = 26;

		public const int RebornInlayMustEquip = 27;

		public const int RebornInlayGetInfoErr = 28;

		public const int RebornInlayNotXuanCai = 29;

		public const int RebornInlayNotChongSheng = 30;

		public const int RebornInlayUpdateInfoErr = 31;

		public const int RebornInlayCurrSiteHasStone = 32;

		public const int RebornInlayUpdateStoneInfoErr = 33;

		public const int RebornDisInlayCurrSiteNotHasXStone = 35;

		public const int RebornDisInlayCurrUserNotHasXStone = 36;

		public const int RebornDisInlayCurrSiteNotHasStone = 37;

		public const int RebornDisInlayCurrUserNotHasStone = 38;

		public const int RebornDisInlayStoneInfoError = 39;

		public const int RebornDisInlayUpdateInfoErr = 40;

		public const int RebornComplexStoneNotFind = 45;

		public const int RebornComplexFengYinNotEnough = 46;

		public const int RebornComplexChongShengNotEnough = 47;

		public const int RebornComplexXuanCaiNotEnough = 48;

		public const int RebornComplexNeedFengYinErr = 49;

		public const int RebornComplexNeedChongShengErr = 50;

		public const int RebornComplexNeedXuanCaiErr = 51;

		public const int RebornComplexNewStoneErr = 52;

		public const int RebornComplexStoneNotGood = 53;

		public const int RebornComplexNewStoneSucc = 54;

		public const int RebornResolveStoneSucc = 55;

		public const int RebornResolveNotFind = 56;

		public const int RebornResolveIsUsing = 57;

		public const int RebornResolveNotStone = 58;

		public const int RebornResolveDeleteErr = 59;

		public const int RebornResolveAddFengYinErr = 60;

		public const int RebornResolveAddChongShengErr = 61;

		public const int RebornResolveAddXuanCaiErr = 62;

		public const int RebornResolveStoneNotGood = 63;

		public const int RebornResolveGoodXmlErr = 64;

		public const int RebornXuanCaiComplexSucc = 70;

		public const int RebornXuanCaiNotFind = 71;

		public const int RebornXuanCaiGoodXmlErr = 72;

		public const int RebornXuanCaiNotSameLevel = 73;

		public const int RebornXuanCaiMaxLevel = 74;

		public const int RebornXuanCaiUseGoodErr = 75;

		public const int RebornXuanCaiNotFindComplex = 76;

		public const int RebornXuanCaiNotUseGoodComplex = 77;

		public const int RebornXuanCaiComplexAddBagErr = 78;

		public const int RebornBatchResolveStoneSucc = 80;

		public const int RebornBatchResolveAddFengYinErr = 81;

		public const int RebornBatchResolveAddChongShengErr = 82;

		public const int RebornBatchResolveAddXuanCaiErr = 83;

		public const int RebornComplexCountErr = 85;

		public const int RebornResolveCountErr = 86;

		public const int RebornResolveGoodNotEnoughErr = 87;

		public const int RebornXuanCaiSuitErr = 90;

		public const int RebornXuanGoodInfoErr = 91;
	}
}
