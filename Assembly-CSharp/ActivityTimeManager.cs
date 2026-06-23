using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ActivityTimeManager
{
	public static int GetDayOfWeek(int day)
	{
		return (day != 0) ? day : 7;
	}

	public static void SplitStrTimePoints(string timePoints, out string[] activityTime1, out string[] activityTime2)
	{
		activityTime1 = null;
		activityTime2 = null;
		if (string.IsNullOrEmpty(timePoints))
		{
			MUDebug.LogError<string>(new string[]
			{
				"解析时间为空！"
			});
			return;
		}
		string[] array = timePoints.Split(new char[]
		{
			'|'
		});
		activityTime1 = array[0].Split(new char[]
		{
			','
		});
		activityTime2 = array[1].Split(new char[]
		{
			','
		});
	}

	public static void ParseCfgTimeToDateTime(string beginTime, string endTime, out DateTime begin, out DateTime end)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		begin = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			beginTime
		});
		DateTime.TryParse(text, ref begin);
		end = default(DateTime);
		string text2 = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			endTime
		});
		DateTime.TryParse(text2, ref end);
	}

	public static void GetAcitivtyBeginAndEndDateTimeByNowTime(DateTime now, int cfgDayOfWeek, string beginTimeStr, string endTimeStr, out DateTime beginDateTime, out DateTime endDateTime)
	{
		beginDateTime = default(DateTime);
		endDateTime = default(DateTime);
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			now.Year,
			now.Month,
			now.Day,
			beginTimeStr
		});
		DateTime.TryParse(text, ref dateTime);
		string text2 = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			now.Year,
			now.Month,
			now.Day,
			endTimeStr
		});
		DateTime.TryParse(text2, ref dateTime2);
		int dayOfWeek = ActivityTimeManager.GetDayOfWeek(now.DayOfWeek);
		cfgDayOfWeek = ActivityTimeManager.GetDayOfWeek(cfgDayOfWeek);
		int num;
		if (dayOfWeek == 7)
		{
			if (cfgDayOfWeek == 7)
			{
				num = 0;
			}
			else
			{
				num = 7 - Mathf.Abs(cfgDayOfWeek - dayOfWeek);
			}
		}
		else if (cfgDayOfWeek > dayOfWeek)
		{
			num = cfgDayOfWeek - dayOfWeek;
		}
		else if (cfgDayOfWeek < dayOfWeek)
		{
			num = 7 - (dayOfWeek - cfgDayOfWeek);
		}
		else
		{
			num = cfgDayOfWeek - dayOfWeek;
		}
		beginDateTime = dateTime.AddDays((double)num);
		endDateTime = dateTime2.AddDays((double)num);
	}

	public static void GetAcitivtyBeginAndEndDateTime(int cfgDayOfWeek, string beginTimeStr, string endTimeStr, out DateTime beginDateTime, out DateTime endDateTime)
	{
		beginDateTime = default(DateTime);
		endDateTime = default(DateTime);
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			beginTimeStr
		});
		DateTime.TryParse(text, ref dateTime);
		string text2 = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			endTimeStr
		});
		DateTime.TryParse(text2, ref dateTime2);
		int dayOfWeek = ActivityTimeManager.GetDayOfWeek(correctDateTime.DayOfWeek);
		cfgDayOfWeek = ActivityTimeManager.GetDayOfWeek(cfgDayOfWeek);
		int num;
		if (dayOfWeek == 7)
		{
			if (cfgDayOfWeek == 7)
			{
				num = 0;
			}
			else
			{
				num = 7 - Mathf.Abs(cfgDayOfWeek - dayOfWeek);
			}
		}
		else if (cfgDayOfWeek > dayOfWeek)
		{
			num = cfgDayOfWeek - dayOfWeek;
		}
		else if (cfgDayOfWeek < dayOfWeek)
		{
			num = 7 - (dayOfWeek - cfgDayOfWeek);
		}
		else
		{
			num = cfgDayOfWeek - dayOfWeek;
		}
		beginDateTime = dateTime.AddDays((double)num);
		endDateTime = dateTime2.AddDays((double)num);
	}

	public static long GetRemainderTicks(DateTime max, DateTime min)
	{
		return max.Ticks - min.Ticks;
	}

	public static long GetRemainderSeconds(DateTime max, DateTime min)
	{
		long num = (max.Ticks - min.Ticks) / 10000000L;
		if (num < 0L)
		{
			MUDebug.LogError<string>(new string[]
			{
				"DateTime max - min < 0"
			});
			num *= -1L;
		}
		return num;
	}

	public static string GetTimeStrBySecEx(double sec)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		int[] array3;
		string[] array4;
		if (sec > (double)num)
		{
			int[] array = new int[]
			{
				(int)(sec / (double)num),
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3)
			};
			string[] array2 = new string[]
			{
				Global.GetLang("天"),
				Global.GetLang("小时"),
				Global.GetLang("分")
			};
			array3 = array;
			array4 = array2;
		}
		else
		{
			int[] array5 = new int[]
			{
				(int)(sec / (double)num),
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3),
				(int)(sec % (double)num % (double)num2 % (double)num3)
			};
			string[] array6 = new string[]
			{
				Global.GetLang("天"),
				Global.GetLang("小时"),
				Global.GetLang("分"),
				Global.GetLang("秒")
			};
			array3 = array5;
			array4 = array6;
		}
		List<int> list = Enumerable.ToList<int>(array3);
		List<string> list2 = Enumerable.ToList<string>(array4);
		while (list.Count > 0 && list[0] == 0)
		{
			list.RemoveAt(0);
			list2.RemoveAt(0);
		}
		string text = string.Empty;
		for (int i = 0; i < list.Count; i++)
		{
			text += list[i].ToString();
			text += list2[i];
		}
		return text;
	}

	public static DateTime ParseStringToNowDateTime(string timeOfDay)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime result = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			timeOfDay
		});
		DateTime.TryParse(text, ref result);
		return result;
	}

	public static DateTime ParseDateTimeToNewDateTimeByTimeOfDay(DateTime dateTime, string timeOfDay)
	{
		DateTime result = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			timeOfDay
		});
		DateTime.TryParse(text, ref result);
		return result;
	}

	public static string GetChineseDayOfWeek(int dayOfWeek)
	{
		switch (dayOfWeek)
		{
		case 0:
			return Global.GetLang("星期日 ");
		case 1:
			return Global.GetLang("星期一 ");
		case 2:
			return Global.GetLang("星期二 ");
		case 3:
			return Global.GetLang("星期三 ");
		case 4:
			return Global.GetLang("星期四 ");
		case 5:
			return Global.GetLang("星期五 ");
		case 6:
			return Global.GetLang("星期六 ");
		case 7:
			return Global.GetLang("星期日 ");
		default:
			return string.Empty;
		}
	}
}
