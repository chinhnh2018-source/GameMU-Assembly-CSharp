using System;
using HSGameEngine.GameEngine.Logic;

public class ShiLiZhengBaData
{
	public static string GetShiLiNameByType(ShiLiType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case ShiLiType.ShenShengJiaoTuan:
			result = Global.GetLang("神圣教团");
			break;
		case ShiLiType.ZiYouTongMeng:
			result = Global.GetLang("织梦协会");
			break;
		case ShiLiType.ZhiMengXieHui:
			result = Global.GetLang("自由同盟");
			break;
		}
		return result;
	}
}
