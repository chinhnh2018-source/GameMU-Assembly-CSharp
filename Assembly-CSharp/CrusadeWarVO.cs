using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CrusadeWarVO
{
	public void CopyForm(XElement xle)
	{
		if (xle != null)
		{
			this.ID = Global.GetXElementAttributeInt(xle, "ID");
			this.Name = Global.GetXElementAttributeStr(xle, "Name");
			this.mApplyBegin = Global.GetXElementAttributeStr(xle, "ApplyBegin");
			this.mApplyEnd = Global.GetXElementAttributeStr(xle, "ApplyEnd");
			this.MapCode = Global.GetXElementAttributeInt(xle, "MapCode");
			this.TimePoints = Global.GetXElementAttributeStr(xle, "TimePoints");
			this.PrepareSecs = Global.GetXElementAttributeInt(xle, "PrepareSecs");
			this.FightingSecs = Global.GetXElementAttributeInt(xle, "FightingSecs");
			this.ClearRolesSecs = Global.GetXElementAttributeInt(xle, "ClearRolesSecs");
			this.AttackerMaxNum = Global.GetXElementAttributeInt(xle, "AttackerMaxNum");
			this.DefenderMaxNum = Global.GetXElementAttributeInt(xle, "DefenderMaxNum");
			this.Exp = Global.GetXElementAttributeInt(xle, "Exp");
			this.BandJinBi = Global.GetXElementAttributeInt(xle, "BandJinBi");
			this.JueXingNum = Global.GetXElementAttributeInt(xle, "JueXingNum");
			this.ShowGoods = Global.GetXElementAttributeStr(xle, "ShowGoods");
		}
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

	public List<CrusadeWarVO.TimePoint> ApplyEnd
	{
		get
		{
			List<CrusadeWarVO.TimePoint> list = new List<CrusadeWarVO.TimePoint>();
			try
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				string[] array = this.mApplyEnd.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						CrusadeWarVO.TimePoint timePoint = new CrusadeWarVO.TimePoint();
						timePoint.Week = array2[0].SafeToInt32(0);
						string[] array3 = array2[1].Split(new char[]
						{
							':'
						});
						timePoint.SetTime1(array3[0].SafeToInt32(0), array3[1].SafeToInt32(0), 0);
						list.Add(timePoint);
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

	public List<CrusadeWarVO.TimePoint> ApplyBegin
	{
		get
		{
			List<CrusadeWarVO.TimePoint> list = new List<CrusadeWarVO.TimePoint>();
			try
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				string[] array = this.mApplyBegin.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						CrusadeWarVO.TimePoint timePoint = new CrusadeWarVO.TimePoint();
						timePoint.Week = array2[0].SafeToInt32(0);
						string[] array3 = array2[1].Split(new char[]
						{
							':'
						});
						timePoint.SetTime1(array3[0].SafeToInt32(0), array3[1].SafeToInt32(0), 0);
						list.Add(timePoint);
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

	public CrusadeWarVO.TimePoint MatchTime
	{
		get
		{
			CrusadeWarVO.TimePoint timePoint = new CrusadeWarVO.TimePoint();
			DateTime correctDateTime = Global.GetCorrectDateTime();
			try
			{
				if (!string.IsNullOrEmpty(this.TimePoints))
				{
					string[] array = this.TimePoints.Split(new char[]
					{
						','
					});
					if (array.Length == 2)
					{
						timePoint.Week = array[0].SafeToInt32(0);
						string[] array2 = array[1].Split(new char[]
						{
							'-'
						});
						if (array2.Length == 2)
						{
							string[] array3 = array2[0].Split(new char[]
							{
								':'
							});
							if (array3.Length == 2)
							{
								timePoint.SetTime1(array3[0].SafeToInt32(0), array3[1].SafeToInt32(0), 0);
							}
							string[] array4 = array2[1].Split(new char[]
							{
								':'
							});
							if (array4.Length == 2)
							{
								timePoint.SetTime2(array4[0].SafeToInt32(0), array4[1].SafeToInt32(0), 0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
			return timePoint;
		}
	}

	public int ID;

	public string Name;

	private string mApplyBegin;

	private string mApplyEnd;

	public int MapCode;

	public string TimePoints;

	public int PrepareSecs;

	public int FightingSecs;

	public int ClearRolesSecs;

	public int AttackerMaxNum;

	public int DefenderMaxNum;

	public int Exp;

	public int BandJinBi;

	public int JueXingNum;

	private string ShowGoods;

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
			return new DateTime(Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day, this.Time1[0], this.Time1[1], this.Time1[2]);
		}

		public DateTime ToDateTime2()
		{
			return new DateTime(Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day, this.Time2[0], this.Time2[1], this.Time2[2]);
		}

		public int AddDays;

		public int Week;

		public int[] Time1;

		public int[] Time2;
	}
}
