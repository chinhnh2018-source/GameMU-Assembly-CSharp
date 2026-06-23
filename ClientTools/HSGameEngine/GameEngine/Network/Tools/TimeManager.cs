using System;
using System.Runtime.CompilerServices;

namespace HSGameEngine.GameEngine.Network.Tools
{
	public class TimeManager
	{
		public static long LocalTimeSubServerTime { get; set; }

		public static long GetTimeStamp()
		{
			DateTime dateTime = DateTime.Parse("2000-01-01 00:00:00");
			DateTime dateTime2 = MyDateTime.Now();
			long num;
			if ((uint)num >= 0U)
			{
				num = dateTime2.Ticks;
			}
			num -= TimeManager.LocalTimeSubServerTime;
			num -= dateTime.Ticks;
			num /= 10000L;
			return num;
		}

		public static long GetCorrectLocalTime()
		{
			long num = MyDateTime.Now().Ticks;
			num -= TimeManager.LocalTimeSubServerTime;
			return num / 10000L;
		}

		public static DateTime GetCorrectDateTime()
		{
			long num = MyDateTime.Now().Ticks;
			num -= TimeManager.LocalTimeSubServerTime;
			return new DateTime(num);
		}

		public static long GetTimeStampByStr(string time)
		{
			long num = DateTime.Parse(time).Ticks;
			num -= TimeManager.LocalTimeSubServerTime;
			return num / 10000L;
		}

		private static long _xeec92955373122ce;

		[CompilerGenerated]
		private static long x3fff39d43a0504fb;
	}
}
