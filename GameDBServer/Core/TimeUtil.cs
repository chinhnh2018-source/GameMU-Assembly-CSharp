using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameDBServer.Core
{
	public class TimeUtil
	{
		public static long CurrentTicksInexact
		{
			get
			{
				return TimeUtil.CurrentTicks;
			}
		}

		static TimeUtil()
		{
			TimeUtil.UnixStartTicks = TimeUtil.UnixStartDateTime.Ticks / 10000L;
			TimeUtil.Before1970Ticks = TimeUtil.UnixStartTicks;
			TimeUtil.Init();
			TimeUtil.TryInitMMTimer();
		}

		public static void TryInitMMTimer()
		{
		}

		public static void WaitStart()
		{
			if (TimeUtil.waitStartEvent.WaitOne(5000))
			{
				long currentTicks = TimeUtil.CurrentTicks;
				for (int i = 0; i < 1000; i++)
				{
					while (currentTicks == Thread.VolatileRead(ref TimeUtil.CurrentTicks))
					{
						Thread.Yield();
					}
					currentTicks = TimeUtil.CurrentTicks;
				}
			}
		}

		public static DateTime SetTime(string timeStr)
		{
			DateTime dateTime;
			DateTime result;
			if (DateTime.TryParse(timeStr, out dateTime))
			{
				TimeUtil.CorrectTimeSecs = (int)(dateTime - DateTime.Now).TotalSeconds;
				TimeUtil.CurrentTickCount = 0;
				result = dateTime;
			}
			else
			{
				result = TimeUtil._Now;
			}
			return result;
		}

		public static bool AsyncNetTicks(long nowTicks, long netTicks)
		{
			long num = netTicks - nowTicks;
			bool result;
			if (Math.Abs(num - TimeUtil.CorrectNetTicks) > TimeUtil.MaxTimeDriftTicks)
			{
				TimeUtil.TimeDriftTicks = nowTicks;
				TimeUtil.CorrectNetTicks = nowTicks;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static long NOW()
		{
			long result;
			if (!TimeUtil.UpdateByTimer)
			{
				int tickCount = Environment.TickCount;
				if (tickCount != TimeUtil.CurrentTickCount)
				{
					DateTime now = DateTime.Now;
					long num = now.Ticks - TimeUtil._Now.Ticks;
					if (num < 0L && num > -3000000000L)
					{
						return TimeUtil.CurrentTicks;
					}
					if (TimeUtil.CorrectTimeSecs != 0)
					{
						now = now.AddSeconds((double)TimeUtil.CorrectTimeSecs);
					}
					TimeUtil._Now = now;
					TimeUtil.CurrentTickCount = tickCount;
					TimeUtil.CurrentTicks = now.Ticks / 10000L;
				}
				result = TimeUtil.CurrentTicks;
			}
			else
			{
				result = Thread.VolatileRead(ref TimeUtil.CurrentTicks);
			}
			return result;
		}

		public static long TimeStamp()
		{
			return Convert.ToInt64((TimeUtil._Now - new DateTime(1970, 1, 1)).TotalSeconds);
		}

		public static long NowRealTime()
		{
			return DateTime.Now.Ticks / 10000L;
		}

		public static DateTime NowDateTime()
		{
			if (!TimeUtil.UpdateByTimer)
			{
				int tickCount = Environment.TickCount;
				if (tickCount != TimeUtil.CurrentTickCount)
				{
					DateTime now = DateTime.Now;
					long num = now.Ticks - TimeUtil._Now.Ticks;
					if (num < 0L && num > -3000000000L)
					{
						return TimeUtil._Now;
					}
					if (TimeUtil.CorrectTimeSecs != 0)
					{
						now = now.AddSeconds((double)TimeUtil.CorrectTimeSecs);
					}
					TimeUtil._Now = now;
					TimeUtil.CurrentTickCount = tickCount;
					TimeUtil.CurrentTicks = now.Ticks / 10000L;
				}
			}
			return TimeUtil._Now;
		}

		public static long UTCTicks()
		{
			return TimeUtil.NowDateTime().ToUniversalTime().Ticks / 1000L;
		}

		public static DateTime UTCTime()
		{
			return TimeUtil.NowDateTime().ToUniversalTime();
		}

		public static string NowDataTimeString(string format = "yyyy-MM-dd HH:mm:ss")
		{
			int currentTickCount = TimeUtil.CurrentTickCount;
			DateTime dateTime = TimeUtil.NowDateTime();
			if (dateTime.Ticks != TimeUtil._CurrentDataTimeStringTicks)
			{
				TimeUtil._CurrentDataTimeStringTicks = dateTime.Ticks;
				TimeUtil._CurrentDataTimeString = dateTime.ToString(format);
			}
			return TimeUtil._CurrentDataTimeString;
		}

		public static string DataTimeToString(DateTime now, string format = "yyyy-MM-dd HH:mm:ss")
		{
			return now.ToString(format);
		}

		public static long ConvertDateTimeInt(DateTime time)
		{
			return (time.Ticks - TimeUtil.UnixStartDateTime.Ticks) / 10000000L;
		}

		public static DateTime ConvertIntDateTime(double d)
		{
			return TimeUtil.UnixStartDateTime.AddMilliseconds(d);
		}

		public static int GetOffsetDays(DateTime startTime)
		{
			return (int)(TimeUtil._Now - startTime).TotalDays;
		}

		public static DateTime GetWeekStartTime(DateTime now)
		{
			int num = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return now.Date.AddDays((double)(-(double)num));
		}

		public static DateTime GetWeekStartTimeNow()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int num = (int)((DayOfWeek.Saturday + (int)dateTime.DayOfWeek) % (DayOfWeek)7);
			return dateTime.Date.AddDays((double)(-(double)num));
		}

		public static int GetWeekStartDayIdNow()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int num = (int)((DayOfWeek.Saturday + (int)dateTime.DayOfWeek) % (DayOfWeek)7);
			return TimeUtil.GetOffsetDay(dateTime.Date.AddDays((double)(-(double)num)));
		}

		public static TimeSpan GetTimeOfWeek(DateTime now)
		{
			int days = (int)((DayOfWeek.Saturday + (int)now.DayOfWeek) % (DayOfWeek)7);
			return new TimeSpan(days, now.Hour, now.Minute, now.Second);
		}

		public static TimeSpan GetTimeOfWeekNow()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int days = (int)((DayOfWeek.Saturday + (int)dateTime.DayOfWeek) % (DayOfWeek)7);
			return new TimeSpan(days, dateTime.Hour, dateTime.Minute, dateTime.Second);
		}

		public static TimeSpan GetTimeOfWeekNow2()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int num = (int)dateTime.DayOfWeek;
			if (num == 0)
			{
				num = 7;
			}
			return new TimeSpan(num, dateTime.Hour, dateTime.Minute, dateTime.Second);
		}

		public static TimeSpan GetTimeOfWeek2(DateTime now)
		{
			int num = (int)now.DayOfWeek;
			if (num == 0)
			{
				num = 7;
			}
			return new TimeSpan(num, now.Hour, now.Minute, now.Second);
		}

		public static int GetOffsetMonth(DateTime now)
		{
			return now.Year * 100 + now.Month;
		}

		[DllImport("kernel32.dll")]
		private static extern long GetTickCount64();

		public static long NowTickCount64()
		{
			return TimeUtil.GetTickCount64() + TimeUtil.ServerStartTicks;
		}

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(ref long x);

		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(ref long x);

		public static long CounterPerSecs
		{
			get
			{
				return TimeUtil._CounterPerSecs;
			}
		}

		public static long Init()
		{
			TimeUtil._EnabelPerformaceCounter = TimeUtil.QueryPerformanceFrequency(ref TimeUtil._CounterPerSecs);
			TimeUtil.QueryPerformanceCounter(ref TimeUtil._StartCounter);
			TimeUtil._EnabelPerformaceCounter = (TimeUtil._EnabelPerformaceCounter && TimeUtil._CounterPerSecs > 0L && TimeUtil._StartCounter > 0L);
			TimeUtil.BasicDateTime = DateTime.Now;
			TimeUtil.BasicPerfromanceCounter = TimeUtil._StartCounter;
			TimeUtil._StartTicks = TimeUtil.BasicDateTime.Ticks;
			TimeUtil.ServerStartTicks = TimeUtil._StartTicks / 10000L - TimeUtil.GetTickCount64();
			return TimeUtil._StartTicks;
		}

		public static long NowEx()
		{
			return TimeUtil.CurrentTicks;
		}

		public static double TimeMS(long time, int round = 2)
		{
			return (double)time;
		}

		private static long TimeDiffEx(long timeEnd, long timeStart)
		{
			long a = timeEnd - timeStart;
			long num2;
			long num = Math.DivRem(a, TimeUtil._CounterPerSecs, out num2);
			return num * 10000000L + num2 * 10000000L / TimeUtil._CounterPerSecs;
		}

		public static long TimeDiff(long timeEnd, long timeStart = 0L)
		{
			return timeEnd - timeStart;
		}

		[DllImport("winmm.dll")]
		public static extern uint timeGetTime();

		public static void RecordTimeAnchor()
		{
			if (TimeUtil._EnabelPerformaceCounter)
			{
				long num = TimeUtil.NOW();
				long num2 = num - TimeUtil.LastAnchorTicks;
				long num3 = 0L;
				TimeUtil.QueryPerformanceCounter(ref num3);
				long num5;
				long num4 = Math.DivRem(num3 - TimeUtil.LastAnchorCounter, TimeUtil._CounterPerSecs, out num5);
				long num6 = num4 * 1000L + num5 * 1000L / TimeUtil._CounterPerSecs;
				if (Math.Abs(num2 - num6) >= num6 / 10L)
				{
					TimeUtil.TimeDriftTicks = num;
				}
				TimeUtil.LastAnchorTicks = num;
				TimeUtil.LastAnchorCounter = num3;
			}
		}

		public static bool HasTimeDrift()
		{
			return TimeUtil.NOW() - TimeUtil.TimeDriftTicks < 180000L;
		}

		public static int MakeYear(DateTime time)
		{
			return time.Year;
		}

		public static int MakeYearMonth(DateTime time)
		{
			return TimeUtil.MakeYear(time) * 100 + time.Month;
		}

		public static int MakeYearMonthDay(DateTime time)
		{
			return TimeUtil.MakeYearMonth(time) * 100 + time.Day;
		}

		public static int MakeFirstWeekday(DateTime time)
		{
			time = time.AddDays((double)((time.DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)(-6)) : (DayOfWeek.Monday - time.DayOfWeek)));
			return TimeUtil.MakeYearMonthDay(time);
		}

		public static int GetWeekDay1To7(DateTime time)
		{
			int result;
			switch (time.DayOfWeek)
			{
			case DayOfWeek.Sunday:
				result = 7;
				break;
			case DayOfWeek.Monday:
				result = 1;
				break;
			case DayOfWeek.Tuesday:
				result = 2;
				break;
			case DayOfWeek.Wednesday:
				result = 3;
				break;
			case DayOfWeek.Thursday:
				result = 4;
				break;
			case DayOfWeek.Friday:
				result = 5;
				break;
			case DayOfWeek.Saturday:
				result = 6;
				break;
			default:
				throw new Exception("unbelievable");
			}
			return result;
		}

		public static bool RandomDispatchTime(DateTime ctime, DateTime stime, int minSecs = 180, int randomSecs = 60, int times = 10)
		{
			long num = (long)Math.Abs((stime - ctime).TotalSeconds);
			bool result;
			if (num < (long)minSecs)
			{
				result = false;
			}
			else
			{
				if (num >= (long)(randomSecs + minSecs))
				{
					times = times / 2 + 1;
				}
				int num2 = TimeUtil.Rnd.Next(randomSecs + 1);
				int num3 = randomSecs / times;
				result = (num2 <= num3);
			}
			return result;
		}

		public static long AgeByNow(ref long age)
		{
			long num = TimeUtil.NOW();
			long result;
			if (age < num)
			{
				result = (age = num);
			}
			else
			{
				result = (age += 1L);
			}
			return result;
		}

		public static long AgeByNow(long age)
		{
			long num = TimeUtil.NOW();
			long result;
			if (age < num)
			{
				result = num;
			}
			else
			{
				result = age + 1L;
			}
			return result;
		}

		public static int AgeByUnixTime(int age)
		{
			int num = TimeUtil.UnixSecondsNow();
			int result;
			if (age < num)
			{
				result = num;
			}
			else
			{
				result = age + 1;
			}
			return result;
		}

		public static double GetOffsetSecond(DateTime date)
		{
			return (date - TimeUtil.StartDate).TotalSeconds;
		}

		public static int GetOffsetDay(DateTime now)
		{
			return (int)(now - TimeUtil.StartDate).TotalDays;
		}

		public static int GetOffsetDay2(DateTime now)
		{
			return now.Year * 10000 + now.Month * 100 + now.Day;
		}

		public static DateTime GetRealDate2(int day)
		{
			return new DateTime(day / 10000, day % 10000 / 100, day % 100);
		}

		public static int GetOffsetDayNow()
		{
			return TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
		}

		public static DateTime GetRealDate(int day)
		{
			return TimeUtil.StartDate.AddDays((double)day);
		}

		public static long UnixSecondsToTicks(int secs)
		{
			return TimeUtil.UnixStartTicks + (long)secs * 1000L;
		}

		public static long UnixSecondsToTicks(string secs)
		{
			int secs2 = Convert.ToInt32(secs);
			return TimeUtil.UnixSecondsToTicks(secs2);
		}

		public static int UnixSecondsNow()
		{
			long ticks = TimeUtil.NOW();
			return TimeUtil.SysTicksToUnixSeconds(ticks);
		}

		public static int SysTicksToUnixSeconds(long ticks)
		{
			long num = (ticks - TimeUtil.UnixStartTicks) / 1000L;
			return (int)num;
		}

		public const int MILLISECOND = 1;

		public const int SECOND = 1000;

		public const int MINITE = 60000;

		public const int HOUR = 3600000;

		public const int DAY = 86400000;

		public const int DAYFLAGS = 100000000;

		public const long BackFreezeTicks = -3000000000L;

		public static long Before1970Ticks;

		public static long ServerStartTicks;

		private static int CurrentTickCount = 0;

		private static long CurrentTicks = DateTime.Now.Ticks / 10000L;

		private static DateTime _Now = DateTime.Now;

		private static volatile int CorrectTimeSecs = 0;

		private static bool UpdateByTimer = false;

		private static long CorrectNetTicks = 0L;

		private static long MaxTimeDriftTicks = 500L;

		public static string _CurrentDataTimeString = "2011-01-01 00:00:00";

		private static long _CurrentDataTimeStringTicks = 0L;

		private static DateTime BasicDateTime;

		private static long BasicPerfromanceCounter;

		private static long CorrectPerfromanceCounterTicks;

		private static ManualResetEvent waitStartEvent = new ManualResetEvent(false);

		private static long _StartCounter = 0L;

		private static long _CounterPerSecs = 0L;

		private static bool _EnabelPerformaceCounter = false;

		private static long _StartTicks = 0L;

		private static long TimeDriftTicks;

		private static long LastAnchorTicks;

		private static long LastAnchorCounter;

		private static Random Rnd = new Random();

		private static DateTime StartDate = new DateTime(2011, 11, 11);

		private static long UnixStartTicks;

		private static DateTime UnixStartDateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
	}
}
