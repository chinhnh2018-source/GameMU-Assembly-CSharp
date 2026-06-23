using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public static class SceneUIClassesGO
{
	public static bool IsTheScene(this SceneUIClasses Scene)
	{
		int num = -1;
		if (Global.Data != null && Global.Data.roleData != null)
		{
			num = Global.Data.roleData.MapCode;
		}
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(num);
		SceneUIClasses mapType;
		if (settingMapVOByCode != null)
		{
			mapType = (SceneUIClasses)settingMapVOByCode.MapType;
		}
		else
		{
			mapType = (SceneUIClasses)Global.GetMapType(num);
		}
		return mapType == Scene;
	}
}
