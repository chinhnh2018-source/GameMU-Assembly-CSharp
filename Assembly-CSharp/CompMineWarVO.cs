using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CompMineWarVO
{
	public void CopyForm(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.MapCode = Global.GetXElementAttributeInt(xml, "MapCode");
		this.TimePoints = Global.GetXElementAttributeStr(xml, "TimePoints");
		this.MaxEnterNum = Global.GetXElementAttributeInt(xml, "MaxEnterNum");
		this.EnterCD = Global.GetXElementAttributeInt(xml, "EnterCD");
		this.PrepareSecs = Global.GetXElementAttributeInt(xml, "PrepareSecs");
		this.FightingSecs = Global.GetXElementAttributeInt(xml, "FightingSecs");
		this.ClearRolesSecs = Global.GetXElementAttributeInt(xml, "ClearRolesSecs");
		this.Exp = Global.GetXElementAttributeFloat(xml, "Exp");
		this.BandJinBi = Global.GetXElementAttributeFloat(xml, "BandJinBi");
		this.ShowGoods = Global.GetXElementAttributeStr(xml, "ShowGoods");
	}

	public List<int> AwardGoods
	{
		get
		{
			List<int> list = new List<int>();
			try
			{
				string[] array = this.ShowGoods.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					int num = array[i].SafeToInt32(0);
					if (0 < num)
					{
						list.Add(num);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>" + ex.Message + "</color>"
				});
			}
			return list;
		}
	}

	public CompMineWarVO.TimePoint ActivityTimeEX
	{
		get
		{
			CompMineWarVO.TimePoint timePoint = new CompMineWarVO.TimePoint();
			try
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				string[] array = this.TimePoints.Split(new char[]
				{
					'-'
				});
				if (!string.IsNullOrEmpty(array[0]))
				{
					string[] array2 = array[0].Split(new char[]
					{
						','
					});
					timePoint.Week = array2[0].SafeToInt32(0);
					string[] array3 = array2[1].Split(new char[]
					{
						':'
					});
					timePoint.SetTime1(array3[0].SafeToInt32(0), array3[1].SafeToInt32(0), 0);
				}
				if (!string.IsNullOrEmpty(array[1]))
				{
					string[] array4 = array[1].Split(new char[]
					{
						','
					});
					string[] array5 = array4[0].Split(new char[]
					{
						':'
					});
					timePoint.SetTime2(array5[0].SafeToInt32(0), array5[1].SafeToInt32(0), 0);
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>" + ex.Message + "</color>"
				});
			}
			return timePoint;
		}
	}

	public int ID;

	public string Name;

	public int MapCode;

	public string TimePoints;

	public int MaxEnterNum;

	public int EnterCD;

	public int PrepareSecs;

	public int FightingSecs;

	public int ClearRolesSecs;

	public float Exp;

	public float BandJinBi;

	public string ShowGoods;

	public class TimePoint
	{
		public TimePoint()
		{
			this.Time1 = new int[3];
			this.Time2 = new int[3];
		}

		public void SetTime1(int a, int b, int c)
		{
			this.Time1[0] = a;
			this.Time1[1] = b;
			this.Time1[2] = c;
		}

		public void SetTime2(int a, int b, int c)
		{
			this.Time2[0] = a;
			this.Time2[1] = b;
			this.Time2[2] = c;
		}

		public DateTime ToDateTime1(double AddDays)
		{
			DateTime dateTime;
			dateTime..ctor(Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day, this.Time1[0], this.Time1[1], this.Time1[2]);
			return dateTime.AddDays(AddDays);
		}

		public DateTime ToDateTime2(double AddDays)
		{
			DateTime dateTime;
			dateTime..ctor(Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day, this.Time2[0], this.Time2[1], this.Time2[2]);
			return dateTime.AddDays(AddDays);
		}

		public DateTime ToDateTime1()
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime result;
			result..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, this.Time1[0], this.Time1[1], this.Time1[2]);
			int num = this.WeekToIntValue(correctDateTime.DayOfWeek);
			int num2 = this.WeekToIntValue(this.Week);
			if (num < num2)
			{
				result = result.AddDays((double)(num2 - num));
			}
			else if (num > num2)
			{
				result = result.AddDays((double)(num2 + 7 - num));
			}
			return result;
		}

		public DateTime ToDateTime2()
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime result;
			result..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, this.Time2[0], this.Time2[1], this.Time2[2]);
			int num = this.WeekToIntValue(correctDateTime.DayOfWeek);
			int num2 = this.WeekToIntValue(this.Week);
			if (num < num2)
			{
				result = result.AddDays((double)(num2 - num));
			}
			else if (num > num2)
			{
				result = result.AddDays((double)(num2 + 7 - num));
			}
			return result;
		}

		private int WeekToIntValue(DayOfWeek week)
		{
			if (week == null)
			{
				return 7;
			}
			return week;
		}

		private int WeekToIntValue(int week)
		{
			if (week == 0)
			{
				return 7;
			}
			return week;
		}

		public int AddDays;

		public int Week;

		public int[] Time1;

		public int[] Time2;
	}
}
