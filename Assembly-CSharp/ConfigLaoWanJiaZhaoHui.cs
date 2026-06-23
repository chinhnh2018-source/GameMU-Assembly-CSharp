using System;
using HSGameEngine.GameEngine.Logic;

public class ConfigLaoWanJiaZhaoHui
{
	public static void DateTimeTryParas(string st, out DateTime _recallTime)
	{
		string[] array = st.Split(new char[]
		{
			' ',
			'/',
			'-',
			':'
		});
		int[] array2 = new int[6];
		for (int i = 0; i < array2.Length; i++)
		{
			if (i < array.Length)
			{
				array2[i] = Global.SafeConvertToInt32(array[i]);
			}
		}
		string text = string.Format("{0}/{1}/{2} {3}:{4}:{5}", new object[]
		{
			array2[0],
			array2[1],
			array2[2],
			array2[3],
			array2[4],
			array2[5]
		});
		DateTime.TryParse(text, ref _recallTime);
	}

	public static DateTime PlayerRecallStartTime()
	{
		string playerRecallStartDay = Global.Data.roleData.PlayerRecallStartDay;
		if (string.IsNullOrEmpty(playerRecallStartDay))
		{
			return ConfigLaoWanJiaZhaoHui._recallTime;
		}
		ConfigLaoWanJiaZhaoHui.DateTimeTryParas(playerRecallStartDay, out ConfigLaoWanJiaZhaoHui._recallTime);
		return ConfigLaoWanJiaZhaoHui._recallTime;
	}

	public static DateTime PlayerRecallEndTime()
	{
		string playerRecallDaysNum = Global.Data.roleData.PlayerRecallDaysNum;
		if (string.IsNullOrEmpty(playerRecallDaysNum))
		{
			return ConfigLaoWanJiaZhaoHui._recallendTime;
		}
		ConfigLaoWanJiaZhaoHui.DateTimeTryParas(playerRecallDaysNum, out ConfigLaoWanJiaZhaoHui._recallendTime);
		if (ServerBufferZhaoHui.Instance.userData != null)
		{
			ConfigLaoWanJiaZhaoHui._recallendTime = ServerBufferZhaoHui.Instance.userData.TimeEnd;
		}
		return ConfigLaoWanJiaZhaoHui._recallendTime;
	}

	public static DateTime PlayerRecallEndTimeSixPlus()
	{
		return ConfigLaoWanJiaZhaoHui.PlayerRecallEndTime();
	}

	public static int PlayerRecallDaysNum()
	{
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return 6;
		}
		return (ConfigLaoWanJiaZhaoHui.PlayerRecallEndTime() - ConfigLaoWanJiaZhaoHui.PlayerRecallStartTime()).Days;
	}

	public static bool IsInPlayerRecallActivity()
	{
		DateTime dateTime = ConfigLaoWanJiaZhaoHui.PlayerRecallStartTime();
		DateTime dateTime2 = ConfigLaoWanJiaZhaoHui.PlayerRecallEndTimeSixPlus();
		DateTime correctDateTime = Global.GetCorrectDateTime();
		return correctDateTime >= dateTime && correctDateTime <= dateTime2;
	}

	public static DateTime CalPlayerRecallDateTime(int day, int hour, int minu, int sec)
	{
		if (day == -1 || hour == -1 || minu == -1 || sec == -1)
		{
			return new DateTime(2000, 1, 1);
		}
		return ConfigLaoWanJiaZhaoHui.PlayerRecallStartTime().Add(new TimeSpan(day, hour, minu, sec));
	}

	public static DateTime ServerDateStringToDateTime(string dateStr)
	{
		DateTime result;
		result..ctor(2000, 1, 1);
		if (string.IsNullOrEmpty(dateStr))
		{
			return result;
		}
		DateTime dateTime = default(DateTime);
		string[] array = dateStr.Split(new char[]
		{
			' '
		});
		ConfigLaoWanJiaZhaoHui.DateTimeTryParas(array[0] + " 00:00:00", out dateTime);
		result = dateTime;
		return result;
	}

	public static DateTime _recallTime = new DateTime(2000, 1, 1);

	public static DateTime _recallendTime = new DateTime(2000, 1, 1);

	public static int recallDelay = 6;
}
