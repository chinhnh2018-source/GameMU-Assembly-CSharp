using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class SpecailTimeManager
	{
		private static DateTimeRange[] GetTimeLimitsByID(int timeLimitsID)
		{
			DateTimeRange[] array = null;
			lock (SpecailTimeManager._TimeLimitsDict)
			{
				if (SpecailTimeManager._TimeLimitsDict.TryGetValue(timeLimitsID, out array))
				{
					return array;
				}
			}
			SystemXmlItem systemXmlItem = null;
			DateTimeRange[] result;
			if (!GameManager.systemSpecialTimeMgr.SystemXmlItemDict.TryGetValue(timeLimitsID, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				string stringValue = systemXmlItem.GetStringValue("TimeLimits");
				if (string.IsNullOrEmpty(stringValue))
				{
					result = null;
				}
				else
				{
					array = Global.ParseDateTimeRangeStr(stringValue);
					lock (SpecailTimeManager._TimeLimitsDict)
					{
						SpecailTimeManager._TimeLimitsDict[timeLimitsID] = array;
					}
					result = array;
				}
			}
			return result;
		}

		public static int ResetSpecialTimeLimits()
		{
			int result = GameManager.systemSpecialTimeMgr.ReloadLoadFromXMlFile();
			lock (SpecailTimeManager._TimeLimitsDict)
			{
				SpecailTimeManager._TimeLimitsDict.Clear();
			}
			return result;
		}

		public static void ProcessDoulbeExperience()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			if (dateTime.Ticks - SpecailTimeManager.JugeDoulbeExperienceTicks >= 50000000L)
			{
				SpecailTimeManager.JugeDoulbeExperienceTicks = dateTime.Ticks;
				SpecailTimeManager.IsDoulbeExperienceAndLingli = SpecailTimeManager.InDoubleExperienceAndLingLiTimeRange(dateTime);
				SpecailTimeManager.IsDoulbeKaoHuo = SpecailTimeManager.InDoubleKaoHuoTimeRange(dateTime);
			}
		}

		private static bool InDoubleExperienceAndLingLiTimeRange(DateTime dateTime)
		{
			DateTimeRange[] timeLimitsByID = SpecailTimeManager.GetTimeLimitsByID(1);
			bool result;
			if (null == timeLimitsByID)
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, timeLimitsByID, out num, true);
			}
			return result;
		}

		public static bool JugeIsDoulbeExperienceAndLingli()
		{
			return SpecailTimeManager.IsDoulbeExperienceAndLingli;
		}

		private static bool InDoubleKaoHuoTimeRange(DateTime dateTime)
		{
			DateTimeRange[] timeLimitsByID = SpecailTimeManager.GetTimeLimitsByID(2);
			bool result;
			if (null == timeLimitsByID)
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, timeLimitsByID, out num, true);
			}
			return result;
		}

		public static bool JugeIsDoulbeKaoHuo()
		{
			return SpecailTimeManager.IsDoulbeKaoHuo;
		}

		public static bool InSpercailTime(int spercailid)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTimeRange[] timeLimitsByID = SpecailTimeManager.GetTimeLimitsByID(spercailid);
			bool result;
			if (null == timeLimitsByID)
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, timeLimitsByID, out num, true);
			}
			return result;
		}

		private static Dictionary<int, DateTimeRange[]> _TimeLimitsDict = new Dictionary<int, DateTimeRange[]>();

		private static long JugeDoulbeExperienceTicks = 0L;

		private static bool IsDoulbeExperienceAndLingli = false;

		private static bool IsDoulbeKaoHuo = false;
	}
}
