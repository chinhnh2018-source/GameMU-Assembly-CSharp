using System;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;

public class ChongChiFanLiUtils
{
	public static bool IsNewDay(int oldDay)
	{
		return oldDay != Global.GetCorrectDateTime().Day;
	}

	public static DateTime GetBeginTime(DateTime t1, DateTime t2)
	{
		return new DateTime(t1.Year, t1.Month, t1.Day, t2.Hour, t2.Minute, t2.Second);
	}

	public static DateTime GetEndTime(DateTime t1, DateTime t2)
	{
		return new DateTime(t1.Year, t1.Month, t1.Day, t2.Hour, t2.Minute, t2.Second);
	}

	public static bool IsXianGouCountOver(int boughtNum, ChongZhiFanLiData data)
	{
		return data != null && data.SinglePurchase - boughtNum <= 0;
	}

	public static ChongZhiFanLiState GetCurState(DateTime begin, DateTime end, int boughtNum = 0, ChongZhiFanLiData data = null, bool isXianGouCountOver = false, bool _isForceSelling = false)
	{
		if (isXianGouCountOver)
		{
			return ChongZhiFanLiState.End;
		}
		if (data != null && !isXianGouCountOver && data.SinglePurchase - boughtNum <= 0)
		{
			return ChongZhiFanLiState.End;
		}
		if (_isForceSelling)
		{
			return ChongZhiFanLiState.Selling;
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (begin < correctDateTime && correctDateTime < end)
		{
			return ChongZhiFanLiState.Selling;
		}
		if (end <= correctDateTime)
		{
			return ChongZhiFanLiState.End;
		}
		if (begin > correctDateTime)
		{
			return ChongZhiFanLiState.WillBegin;
		}
		return ChongZhiFanLiState.None;
	}

	public static string GetStrTime(double sec)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		return StringUtil.substitute(Global.GetLang("{0}:{1}:{2}"), new object[]
		{
			((int)(sec % (double)num / (double)num2)).ToString("00"),
			((int)(sec % (double)num % (double)num2 / (double)num3)).ToString("00"),
			((int)(sec % (double)num % (double)num2 % (double)num3)).ToString("00")
		});
	}
}
