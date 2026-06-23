using System;
using HSGameEngine.GameEngine.Logic;

namespace GameServer.Logic
{
	public static class ZuoQiActionResultTypeErr
	{
		public static string GetErrMsg(int errCode, bool hefuluolan = false, bool inKuafuhuodong = false)
		{
			string result = Global.GetLang("未知错误...错误码[") + errCode + "]";
			switch (errCode)
			{
			case 1:
				result = Global.GetLang("参数错误");
				break;
			case 2:
				result = Global.GetLang("背包满");
				break;
			case 3:
				result = Global.GetLang("配置出错！");
				break;
			case 4:
				result = Global.GetLang("钻石不足");
				break;
			case 5:
				result = Global.GetLang("到最高级");
				break;
			case 6:
				result = Global.GetLang("魂晶不足");
				break;
			case 7:
				result = Global.GetLang("道具不足");
				break;
			case 8:
				result = Global.GetLang("数据库操作失败");
				break;
			case 9:
				result = Global.GetLang("坐骑道具不存在");
				break;
			case 10:
				result = Global.GetLang("等阶到达上限");
				break;
			case 11:
				result = Global.GetLang("该坐骑没有升阶信息");
				break;
			case 12:
				result = Global.GetLang("无法佩戴技能");
				break;
			case 13:
				result = Global.GetLang("地图限制");
				break;
			case 14:
				result = Global.GetLang("钻石不足");
				break;
			default:
				result = Global.GetLang("其他错误！错误码[") + errCode + "]";
				break;
			}
			return result;
		}

		public const int Success = 0;

		public const int EParam = 1;

		public const int EBagFull = 2;

		public const int EConfig = 3;

		public const int ENoZuanShi = 4;

		public const int ELevelMax = 5;

		public const int ENoSiLiao = 6;

		public const int ENoGoods = 7;

		public const int EDBOpt = 8;

		public const int ENoMount = 9;

		public const int EGradeMax = 10;

		public const int ENoGradeUp = 11;

		public const int ENoEquipSkill = 12;

		public const int EMapLimit = 13;

		public const int ENeedPay = 14;
	}
}
