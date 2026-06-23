using System;
using HSGameEngine.GameEngine.Logic;

public class JueXingActionResultType
{
	public static string GetMessage(int retCode)
	{
		string result = string.Concat(new object[]
		{
			Global.GetLang("未知错误...错误码"),
			"[",
			retCode,
			"]"
		});
		switch (retCode + 10)
		{
		case 0:
			result = Global.GetLang("道具不够");
			break;
		case 1:
			result = Global.GetLang("觉醒之尘不够");
			break;
		case 2:
			result = Global.GetLang("等级到达上限");
			break;
		case 3:
			result = Global.GetLang("未激活觉醒石");
			break;
		case 4:
			result = Global.GetLang("套装类型不匹配");
			break;
		case 5:
			result = Global.GetLang("觉醒石与套装不匹配");
			break;
		case 6:
			result = Global.GetLang("数据库存储失败");
			break;
		case 7:
			result = Global.GetLang("碎片不够");
			break;
		case 8:
			result = Global.GetLang("配置不存在");
			break;
		case 9:
			result = Global.GetLang("已经激活");
			break;
		}
		return result;
	}

	public const int Success = 0;

	public const int HaveActive = -1;

	public const int ErrConfig = -2;

	public const int NeedPart = -3;

	public const int ErrDB = -4;

	public const int ErrParent = -5;

	public const int ErrSuitType = -6;

	public const int ErrNotActivite = -7;

	public const int ErrLevelLimit = -8;

	public const int ErrChenLimit = -9;

	public const int ErrGoodsLimit = -10;
}
