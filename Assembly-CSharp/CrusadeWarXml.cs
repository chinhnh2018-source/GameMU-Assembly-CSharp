using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class CrusadeWarXml
{
	public int RecommendCount
	{
		get
		{
			return this.mRecommendCount;
		}
		set
		{
			this.mRecommendCount = value;
		}
	}

	private void initXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/CrusadeWar.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "CrusadeWar");
			if (xelementList != null && 0 < xelementList.Count)
			{
				int i = 0;
				int count = xelementList.Count;
				while (i < count)
				{
					CrusadeWarVO crusadeWarVO = new CrusadeWarVO();
					crusadeWarVO.CopyForm(xelementList[i]);
					if (!this.dic.ContainsKey(crusadeWarVO.ID))
					{
						this.dic[crusadeWarVO.ID] = crusadeWarVO;
					}
					else
					{
						this.dic.Add(crusadeWarVO.ID, crusadeWarVO);
					}
					i++;
				}
			}
		}
	}

	public CrusadeWarVO GetCrusadeWarVOByID(int ID = 1)
	{
		if (0 >= this.dic.Count)
		{
			this.initXml();
		}
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		MUDebug.Log<string>(new string[]
		{
			"<color=yellow>CrusadeWar 表中不存在ID = " + ID + "</color>"
		});
		return null;
	}

	public CrusadeWarVO.TimePoint GetNearTimeByID(DateTime time, List<CrusadeWarVO.TimePoint> timeList)
	{
		try
		{
			int i = 0;
			int count = timeList.Count;
			while (i < count)
			{
				CrusadeWarVO.TimePoint timePoint = timeList[i];
				int num = this.WeekToIntValue(timePoint.Week);
				if (this.WeekToIntValue(time.DayOfWeek) <= num)
				{
					int num2 = num - this.WeekToIntValue(time.DayOfWeek);
					int num3 = timePoint.Time1[0] - time.Hour;
					int num4 = timePoint.Time1[1] - time.Minute;
					int num5 = timePoint.Time1[2] - time.Second;
					long num6 = (long)num3 * 3600L + (long)num4 * 60L + (long)num5;
					long num7 = num6 * 10000000L + this.GetOneDayTicks() * (long)num2;
					if (0L < num7)
					{
						return timePoint;
					}
				}
				i++;
			}
			timeList[0].AddDays = this.WeekToIntValue(timeList[0].Week) + 7 - this.WeekToIntValue(time.DayOfWeek);
			return timeList[0];
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
		return new CrusadeWarVO.TimePoint();
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

	private long GetOneDayTicks()
	{
		return 864000000000L;
	}

	public DateTime GetNearTimeByRound(int Round, out byte applyIsBegin)
	{
		applyIsBegin = 0;
		try
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			CrusadeWarVO crusadeWarVOByID = this.GetCrusadeWarVOByID(1);
			if (crusadeWarVOByID != null)
			{
				if (Round == 1)
				{
					List<CrusadeWarVO.TimePoint> applyBegin = crusadeWarVOByID.ApplyBegin;
					CrusadeWarVO.TimePoint timePoint = applyBegin[0];
					if (this.WeekToIntValue(correctDateTime.DayOfWeek) < this.WeekToIntValue(timePoint.Week))
					{
						applyIsBegin = 0;
						return timePoint.ToDateTime1((double)(this.WeekToIntValue(timePoint.Week) - this.WeekToIntValue(correctDateTime.DayOfWeek)));
					}
					if (this.WeekToIntValue(correctDateTime.DayOfWeek) != this.WeekToIntValue(timePoint.Week))
					{
						applyIsBegin = 0;
						return timePoint.ToDateTime1();
					}
					DateTime dateTime = timePoint.ToDateTime1();
					if (dateTime > correctDateTime)
					{
						applyIsBegin = 0;
						return dateTime;
					}
					applyIsBegin = 1;
					List<CrusadeWarVO.TimePoint> applyEnd = crusadeWarVOByID.ApplyEnd;
					if (applyEnd != null && Round <= applyEnd.Count)
					{
						CrusadeWarVO.TimePoint timePoint2 = applyEnd[Round - 1];
						DateTime dateTime2 = timePoint2.ToDateTime1();
						if (dateTime2 > correctDateTime)
						{
							return dateTime2;
						}
					}
					CrusadeWarVO.TimePoint matchTime = crusadeWarVOByID.MatchTime;
					if (correctDateTime > matchTime.ToDateTime2())
					{
						applyIsBegin = 2;
						return timePoint.ToDateTime1(7.0);
					}
				}
				else
				{
					List<CrusadeWarVO.TimePoint> applyEnd2 = crusadeWarVOByID.ApplyEnd;
					if (applyEnd2 != null && Round <= applyEnd2.Count)
					{
						CrusadeWarVO.TimePoint timePoint3 = applyEnd2[Round - 2];
						if (this.WeekToIntValue(correctDateTime.DayOfWeek) < this.WeekToIntValue(timePoint3.Week))
						{
							applyIsBegin = 0;
							return timePoint3.ToDateTime1((double)(this.WeekToIntValue(timePoint3.Week) - this.WeekToIntValue(correctDateTime.DayOfWeek)));
						}
						if (this.WeekToIntValue(correctDateTime.DayOfWeek) != this.WeekToIntValue(timePoint3.Week))
						{
							applyIsBegin = 0;
							return timePoint3.ToDateTime1();
						}
						DateTime dateTime3 = timePoint3.ToDateTime1();
						if (dateTime3 > correctDateTime)
						{
							applyIsBegin = 0;
							return dateTime3;
						}
						CrusadeWarVO.TimePoint timePoint4 = applyEnd2[Round - 1];
						DateTime dateTime4 = timePoint4.ToDateTime1();
						if (dateTime4 > correctDateTime)
						{
							applyIsBegin = 1;
							return dateTime4;
						}
						if (correctDateTime > crusadeWarVOByID.MatchTime.ToDateTime2())
						{
							applyIsBegin = 2;
							return timePoint3.ToDateTime1(7.0);
						}
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>GetNearTimeByRound  times == null || time .count < " + Round + "</color>"
						});
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
		return DateTime.MinValue;
	}

	public int GetNextStateTimeData(out DateTime NextTime, out bool StateHasChange, KuaFuLueDuoGameStates type)
	{
		NextTime = DateTime.MinValue;
		StateHasChange = false;
		DateTime correctDateTime = Global.GetCorrectDateTime();
		List<CrusadeWarVO.TimePoint> applyBeginTimesByID = this.GetApplyBeginTimesByID(1);
		if (type == null)
		{
			if (this.WeekToIntValue(correctDateTime.DayOfWeek) < this.WeekToIntValue(applyBeginTimesByID[0].Week))
			{
				NextTime = applyBeginTimesByID[0].ToDateTime1((double)(this.WeekToIntValue(applyBeginTimesByID[0].Week) - this.WeekToIntValue(correctDateTime.DayOfWeek)));
			}
			else if (this.WeekToIntValue(correctDateTime.DayOfWeek) == this.WeekToIntValue(applyBeginTimesByID[0].Week))
			{
				DateTime dateTime = applyBeginTimesByID[0].ToDateTime1();
				if (dateTime > correctDateTime)
				{
					NextTime = dateTime;
				}
				else
				{
					CrusadeWarVO.TimePoint matchTime = this.GetCrusadeWarVOByID(1).MatchTime;
					DateTime dateTime2 = matchTime.ToDateTime1();
					DateTime dateTime3 = matchTime.ToDateTime2();
					if (correctDateTime > dateTime3)
					{
						StateHasChange = false;
						applyBeginTimesByID[0].AddDays = this.WeekToIntValue(applyBeginTimesByID[0].Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek);
						NextTime = applyBeginTimesByID[0].ToDateTime1((double)applyBeginTimesByID[0].AddDays);
					}
					else
					{
						StateHasChange = true;
					}
				}
			}
			else
			{
				NextTime = applyBeginTimesByID[0].ToDateTime1((double)(this.WeekToIntValue(applyBeginTimesByID[0].Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek)));
			}
		}
		else if (type == 2)
		{
			CrusadeWarVO.TimePoint matchTime2 = this.GetCrusadeWarVOByID(1).MatchTime;
			if (this.WeekToIntValue(correctDateTime.DayOfWeek) != matchTime2.Week)
			{
				StateHasChange = true;
				return -1;
			}
			DateTime dateTime4 = matchTime2.ToDateTime1();
			if (!(correctDateTime < dateTime4))
			{
				StateHasChange = true;
				return -1;
			}
			NextTime = dateTime4;
		}
		else if (type == 1)
		{
			applyBeginTimesByID.AddRange(this.GetApplyEndTimesByID(1));
			if (this.WeekToIntValue(correctDateTime.DayOfWeek) != this.WeekToIntValue(applyBeginTimesByID[0].Week))
			{
				StateHasChange = true;
				return -1;
			}
			if (correctDateTime < applyBeginTimesByID[0].ToDateTime1())
			{
				StateHasChange = true;
				return -1;
			}
			for (int i = 1; i < applyBeginTimesByID.Count; i++)
			{
				DateTime dateTime5 = applyBeginTimesByID[i].ToDateTime1();
				if (correctDateTime < dateTime5)
				{
					NextTime = dateTime5;
					return i;
				}
			}
			StateHasChange = true;
			return -1;
		}
		else if (type == 3)
		{
			CrusadeWarVO.TimePoint matchTime3 = this.GetCrusadeWarVOByID(1).MatchTime;
			if (this.WeekToIntValue(correctDateTime.DayOfWeek) != matchTime3.Week)
			{
				StateHasChange = true;
				return -1;
			}
			DateTime dateTime6 = matchTime3.ToDateTime1();
			DateTime dateTime7 = matchTime3.ToDateTime2();
			if (!(correctDateTime >= dateTime6) || !(correctDateTime < dateTime7))
			{
				StateHasChange = true;
				return -1;
			}
			NextTime = dateTime7;
		}
		else if (type == 4)
		{
			if (this.WeekToIntValue(correctDateTime.DayOfWeek) < this.WeekToIntValue(applyBeginTimesByID[0].Week))
			{
				NextTime = applyBeginTimesByID[0].ToDateTime1((double)(this.WeekToIntValue(applyBeginTimesByID[0].Week) - this.WeekToIntValue(correctDateTime.DayOfWeek)));
			}
			else if (this.WeekToIntValue(correctDateTime.DayOfWeek) == this.WeekToIntValue(applyBeginTimesByID[0].Week))
			{
				DateTime dateTime8 = applyBeginTimesByID[0].ToDateTime1();
				if (dateTime8 > correctDateTime)
				{
					NextTime = dateTime8;
				}
				else
				{
					CrusadeWarVO.TimePoint matchTime4 = this.GetCrusadeWarVOByID(1).MatchTime;
					DateTime dateTime9 = matchTime4.ToDateTime1();
					DateTime dateTime10 = matchTime4.ToDateTime2();
					if (correctDateTime > dateTime10)
					{
						StateHasChange = false;
						applyBeginTimesByID[0].AddDays = this.WeekToIntValue(applyBeginTimesByID[0].Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek);
						NextTime = applyBeginTimesByID[0].ToDateTime1((double)applyBeginTimesByID[0].AddDays);
					}
					else
					{
						StateHasChange = true;
					}
				}
			}
			else
			{
				NextTime = applyBeginTimesByID[0].ToDateTime1((double)(this.WeekToIntValue(applyBeginTimesByID[0].Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek)));
			}
		}
		return -1;
	}

	public List<CrusadeWarVO.TimePoint> GetApplyEndTimesByID(int ID)
	{
		CrusadeWarVO crusadeWarVOByID = this.GetCrusadeWarVOByID(ID);
		if (crusadeWarVOByID != null)
		{
			return crusadeWarVOByID.ApplyEnd;
		}
		return null;
	}

	public List<CrusadeWarVO.TimePoint> GetApplyBeginTimesByID(int ID)
	{
		CrusadeWarVO crusadeWarVOByID = this.GetCrusadeWarVOByID(ID);
		if (crusadeWarVOByID != null)
		{
			return crusadeWarVOByID.ApplyBegin;
		}
		return null;
	}

	public List<GoodsData> GetShowGoodsDataListByID(int ID)
	{
		List<GoodsData> list = new List<GoodsData>();
		CrusadeWarVO crusadeWarVOByID = this.GetCrusadeWarVOByID(ID);
		if (crusadeWarVOByID != null)
		{
			try
			{
				List<int> awardGoods = crusadeWarVOByID.AwardGoods;
				int i = 0;
				int count = awardGoods.Count;
				while (i < count)
				{
					GoodsData emptyGoodsData = Global.GetEmptyGoodsData(awardGoods[i], 0, 0, 0, 1, 0, 0, 0, 0);
					list.Add(emptyGoodsData);
					i++;
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>" + ex.Message + "</color>"
				});
			}
		}
		return list;
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	private const long mOneHourEqualSecond = 3600L;

	private const long mOneMinteEqualSecond = 60L;

	private const long mOneSecondEqualTicks = 10000000L;

	private const int mMillisPerDay = 86400000;

	private const long mTicksPerMillisecond = 10000L;

	private const string Path = "Config/CrusadeWar.xml";

	private int mRecommendCount;

	private Dictionary<int, CrusadeWarVO> dic = new Dictionary<int, CrusadeWarVO>();
}
