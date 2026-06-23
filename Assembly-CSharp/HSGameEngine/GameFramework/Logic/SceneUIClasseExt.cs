using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

namespace HSGameEngine.GameFramework.Logic
{
	public static class SceneUIClasseExt
	{
		public static SceneUIClasses GetMapSceneUIClass()
		{
			int mapCode = Global.Data.roleData.MapCode;
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
			SceneUIClasses mapType;
			if (settingMapVOByCode != null)
			{
				mapType = (SceneUIClasses)settingMapVOByCode.MapType;
			}
			else
			{
				mapType = (SceneUIClasses)Global.GetMapType(mapCode);
			}
			return mapType;
		}

		private static void _Init()
		{
			if (SceneUIClasseExt._InitSuc)
			{
				return;
			}
			SceneUIClasseExt._InitSuc = true;
			Dictionary<int, SettingMapVO> settingsMapVODict = ConfigSettings.GetSettingsMapVODict();
			foreach (KeyValuePair<int, SettingMapVO> keyValuePair in settingsMapVODict)
			{
				SceneUIClasses mapType = (SceneUIClasses)keyValuePair.Value.MapType;
				Dictionary<int, SettingMapVO>.Enumerator enumerator;
				KeyValuePair<int, SettingMapVO> keyValuePair2 = enumerator.Current;
				SceneUIClasseExt._AddMapCode(mapType, keyValuePair2.Key);
			}
		}

		public static int FubenID(this SceneUIClasses _SceneUIClasses)
		{
			SceneUIClasseExt._Init();
			if (SceneUIClasseExt.HasFubenID(_SceneUIClasses))
			{
				return SceneUIClasseExt.dicFubenID[_SceneUIClasses][0];
			}
			return -1;
		}

		public static int MapCode(this SceneUIClasses _SceneUIClasses)
		{
			SceneUIClasseExt._Init();
			if (SceneUIClasseExt.HasMapCode(_SceneUIClasses))
			{
				return SceneUIClasseExt.dicMapCode[_SceneUIClasses][0];
			}
			return -1;
		}

		public static List<int> MapCodeList(this SceneUIClasses _SceneUIClasses)
		{
			SceneUIClasseExt._Init();
			if (SceneUIClasseExt.HasMapCode(_SceneUIClasses))
			{
				return SceneUIClasseExt.dicMapCode[_SceneUIClasses];
			}
			return null;
		}

		public static int MapCodeFisrt(this SceneUIClasses _SceneUIClasses)
		{
			SceneUIClasseExt._Init();
			if (SceneUIClasseExt.HasMapCode(_SceneUIClasses))
			{
				return SceneUIClasseExt.dicMapCode[_SceneUIClasses][0];
			}
			return -1;
		}

		public static int MapCodeLast(this SceneUIClasses _SceneUIClasses)
		{
			SceneUIClasseExt._Init();
			if (SceneUIClasseExt.HasMapCode(_SceneUIClasses))
			{
				List<int> list = SceneUIClasseExt.dicMapCode[_SceneUIClasses];
				return list[list.Count - 1];
			}
			return -1;
		}

		private static bool HasMapCode(SceneUIClasses uiClass)
		{
			return SceneUIClasseExt.dicMapCode.ContainsKey(uiClass);
		}

		private static void _AddMapCode(SceneUIClasses uiClass, int mapCode)
		{
			if (SceneUIClasseExt.dicMapCode.ContainsKey(uiClass))
			{
				SceneUIClasseExt.dicMapCode[uiClass].Add(mapCode);
			}
			else
			{
				Dictionary<SceneUIClasses, List<int>> dictionary = SceneUIClasseExt.dicMapCode;
				List<int> list = new List<int>();
				list.Add(mapCode);
				dictionary.Add(uiClass, list);
			}
		}

		private static bool HasFubenID(SceneUIClasses uiClass)
		{
			return SceneUIClasseExt.dicFubenID.ContainsKey(uiClass);
		}

		private static void _AddFubenID(SceneUIClasses uiClass, int CopyID)
		{
			if (SceneUIClasseExt.dicFubenID.ContainsKey(uiClass))
			{
				SceneUIClasseExt.dicFubenID[uiClass].Add(CopyID);
			}
			else
			{
				Dictionary<SceneUIClasses, List<int>> dictionary = SceneUIClasseExt.dicFubenID;
				List<int> list = new List<int>();
				list.Add(CopyID);
				dictionary.Add(uiClass, list);
			}
		}

		private static Dictionary<SceneUIClasses, List<int>> dicMapCode = new Dictionary<SceneUIClasses, List<int>>();

		private static Dictionary<SceneUIClasses, List<int>> dicFubenID = new Dictionary<SceneUIClasses, List<int>>();

		private static bool _InitSuc = false;
	}
}
