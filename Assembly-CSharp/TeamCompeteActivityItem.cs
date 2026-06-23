using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class TeamCompeteActivityItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblType.Text = Global.GetLang(string.Empty);
		this.LblTime.Text = Global.GetLang(string.Empty);
	}

	private void InitEvent()
	{
	}

	public void InitValue(XElement element)
	{
		this.vo = new ZhanDuiHuoDongTabVo(element);
		if (this.vo.ID == 200002)
		{
			this.SetBakUrl(this.vo.Preview.ToString(), true);
		}
		else
		{
			this.SetBakUrl(this.vo.Preview.ToString(), false);
		}
		this.CountDown(this.vo.ID);
	}

	private string LblShowTime
	{
		set
		{
			this.LblTime.Text = value;
		}
	}

	private string LblShowType
	{
		set
		{
			this.LblType.Text = value;
		}
	}

	private void SetBakUrl(string name, bool beChongSheng = false)
	{
		if (beChongSheng)
		{
			this.img.URL = string.Format("NetImages/GameRes/Images/Preview/{0}.jpg", name);
			this.img.transform.localScale = new Vector3(220f, 360f, 1f);
		}
		else
		{
			this.img.URL = string.Format("NetImages/GameRes/Images/Preview/{0}.jpg", name);
		}
	}

	private void CountDown(int id)
	{
		switch (id)
		{
		case 200000:
			this.RefreshTeamCompeteTimeState();
			break;
		case 200001:
			this.DisplayTeamZhengBaActivityTime();
			break;
		case 200002:
			this.RefreshTeamCompeteTimeState();
			break;
		case 200003:
			this.DisplayDaTaoShaActivityTime();
			break;
		}
	}

	private void LoadTeamCompeteTimeXmlCfg(bool isAnotherDay = false)
	{
		XElement gameResXml = Global.GetGameResXml("Config/TeamBattle.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "TeamBattle");
		string attributeStr = xelementList[0].GetAttributeStr("TimePoints");
		string[] array = attributeStr.Split(new char[]
		{
			'-'
		});
		this.mTeamCompeteClearResTime = xelementList[0].GetAttributeInt("ClearRolesSecs", -1);
		DateTime dateTime = Global.GetCorrectDateTime();
		if (isAnotherDay)
		{
			dateTime = dateTime.AddDays(1.0);
		}
		this.GetAcitivtyStartAndEndDateTime(this.GetNormalWeekday(dateTime.DayOfWeek), array[0], array[1], out this.mTeamCompeteStartTime, out this.mTeamCompeteEndTime);
	}

	private void LoadMoYuDuoBaoXmlCfg()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ZorkActivityRules.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZorkActivityRules");
		string attributeStr = xelementList[0].GetAttributeStr("TimePoints");
		this.mTeamCompeteClearResTime = xelementList[0].GetAttributeInt("ClearRolesSecs", -1);
		int num = attributeStr.Split(new char[]
		{
			','
		})[0].SafeToInt32(0);
		string[] array = attributeStr.Split(new char[]
		{
			','
		})[1].Split(new char[]
		{
			'-'
		});
		DateTime correctDateTime = Global.GetCorrectDateTime();
		this.GetNextStartAndEndTime(correctDateTime, attributeStr, out this.mTeamCompeteStartTime, out this.mTeamCompeteEndTime);
	}

	public void GetNextStartAndEndTime(DateTime nowTime, string timePoints, out DateTime startDateTime, out DateTime endDateTime)
	{
		startDateTime = DateTime.MaxValue;
		endDateTime = DateTime.MaxValue;
		string[] array = timePoints.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			int dayOfWeak = array[i].Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			string[] array2 = array[i].Split(new char[]
			{
				','
			})[1].Split(new char[]
			{
				'-'
			});
			DateTime dateTime;
			DateTime dateTime2;
			this.GetNextStartTime(nowTime, dayOfWeak, array2[0], array2[1], out dateTime, out dateTime2);
			if (dateTime < startDateTime)
			{
				startDateTime = dateTime;
				endDateTime = dateTime2;
			}
		}
	}

	public void GetNextStartTime(DateTime nowTime, int dayOfWeak, string startTimeStr, string endTimeStr, out DateTime startDateTime, out DateTime endDateTime)
	{
		startDateTime = default(DateTime);
		endDateTime = default(DateTime);
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			nowTime.Year,
			nowTime.Month,
			nowTime.Day,
			startTimeStr
		});
		DateTime.TryParse(text, ref dateTime);
		string text2 = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			nowTime.Year,
			nowTime.Month,
			nowTime.Day,
			endTimeStr
		});
		DateTime.TryParse(text2, ref dateTime2);
		int normalWeekday = this.GetNormalWeekday(nowTime.DayOfWeek);
		int normalWeekday2 = this.GetNormalWeekday(dayOfWeak);
		int num;
		if (normalWeekday2 < normalWeekday)
		{
			num = 7 + normalWeekday2 - normalWeekday;
		}
		else if (normalWeekday2 > normalWeekday)
		{
			num = normalWeekday2 - normalWeekday;
		}
		else if (nowTime > dateTime2)
		{
			num = 7;
		}
		else
		{
			num = 0;
		}
		startDateTime = dateTime.AddDays((double)num);
		endDateTime = dateTime2.AddDays((double)num);
	}

	private void LoadXmlCfg()
	{
		if (this.vo != null)
		{
			switch (this.vo.ID)
			{
			case 200000:
			{
				bool flag = false;
				if (this.teamCompeteTimeState == TeamCompeteActivityItem.ActivityTimeState.Start)
				{
					flag = true;
				}
				this.LoadTeamCompeteTimeXmlCfg(flag);
				if (flag)
				{
				}
				break;
			}
			case 200002:
				this.LoadMoYuDuoBaoXmlCfg();
				break;
			}
		}
	}

	public void RefreshTeamCompeteTimeState()
	{
		this.LoadXmlCfg();
		long remainderTicks = this.GetRemainderTicks(this.mTeamCompeteStartTime, Global.GetCorrectDateTime());
		long remainderTicks2 = this.GetRemainderTicks(this.mTeamCompeteEndTime, Global.GetCorrectDateTime());
		if (remainderTicks > 0L)
		{
			this.TeamCompeteNotStart(remainderTicks);
		}
		else if (remainderTicks <= 0L && remainderTicks2 > 0L)
		{
			this.TeamCompeteStarting(remainderTicks2);
		}
		else
		{
			this.teamCompeteTimeState = TeamCompeteActivityItem.ActivityTimeState.Start;
			this.RefreshTeamCompeteTimeState();
		}
	}

	private void TeamCompeteNotStart(long ticks)
	{
		this.LblShowType = Global.GetLang("距离活动开始");
		this.LblShowTime = string.Empty;
		this.remainderTeamCompeteTicks = ticks / 10000000L;
		this.teamCompeteTimeState = TeamCompeteActivityItem.ActivityTimeState.NotStart;
		this.color = "ff0000";
		this.StartTeamCompeteCountDown();
	}

	private void TeamCompeteStarting(long ticks)
	{
		this.LblShowType = Global.GetLang("距离活动结束");
		this.LblShowTime = string.Empty;
		this.remainderTeamCompeteTicks = ticks / 10000000L;
		this.teamCompeteTimeState = TeamCompeteActivityItem.ActivityTimeState.Start;
		this.StartTeamCompeteCountDown();
	}

	private void StartTeamCompeteCountDown()
	{
		this.TeamCompeteUITimer = new DispatcherTimer("TeamCompeteUITimer");
		this.TeamCompeteUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.TeamCompeteUITimer.Tick = new DispatcherTimerEventHandler(this.TeamCompeteUITimer_Tick);
		this.TeamCompeteUITimer.Start();
	}

	protected void TeamCompeteUITimer_Tick(object sender, object e)
	{
		if (this.remainderTeamCompeteTicks > 0L)
		{
			this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				string.Format(Global.GetLang("剩余 {0}"), this.GetTimeStrBySecEx((double)this.remainderTeamCompeteTicks))
			});
			this.remainderTeamCompeteTicks -= 1L;
		}
		else
		{
			this.color = "17e43e";
			this.LblShowType = Global.GetLang(string.Empty);
			this.LblShowTime = string.Empty;
			this.StopTeamCompeteTimer();
			this.RefreshTeamCompeteTimeState();
		}
	}

	private void StopTeamCompeteTimer()
	{
		if (this.TeamCompeteUITimer != null)
		{
			this.TeamCompeteUITimer.Tick = null;
			this.TeamCompeteUITimer.Stop();
			this.TeamCompeteUITimer = null;
		}
	}

	private void DisplayTeamZhengBaActivityTime()
	{
		long seconds = 0L;
		DateTime correctDateTime = Global.GetCorrectDateTime();
		Dictionary<int, TeamMatchVo>.Enumerator enumerator = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchXMLDict().GetEnumerator();
		List<string[]> list = new List<string[]>();
		List<int> list2 = new List<int>();
		while (enumerator.MoveNext())
		{
			List<string[]> list3 = list;
			KeyValuePair<int, TeamMatchVo> keyValuePair = enumerator.Current;
			list3.Add(keyValuePair.Value.TimePoints.Split(new char[]
			{
				','
			}));
			List<int> list4 = list2;
			KeyValuePair<int, TeamMatchVo> keyValuePair2 = enumerator.Current;
			list4.Add(keyValuePair2.Key);
		}
		this.DayOfOpenActivity = list[0][0].SafeToInt32(0);
		string[] array = list[0][1].Split(new char[]
		{
			'-'
		});
		if (correctDateTime.Day == this.DayOfOpenActivity)
		{
			DateTime dateTime = ActivityTimeManager.ParseStringToNowDateTime(list[0][1].Split(new char[]
			{
				'-'
			})[0]);
			DateTime dateTime2 = ActivityTimeManager.ParseStringToNowDateTime(list[list.Count - 1][1].Split(new char[]
			{
				'-'
			})[1]);
			this.IsInCurrentDayActivityTime(dateTime, dateTime2);
			switch (this.mTimeStatus)
			{
			case TeamCompeteActivityItem.TimeStatus.NotBegin:
				seconds = ActivityTimeManager.GetRemainderSeconds(dateTime, correctDateTime);
				break;
			case TeamCompeteActivityItem.TimeStatus.Beginning:
				seconds = ActivityTimeManager.GetRemainderSeconds(dateTime2, correctDateTime);
				break;
			case TeamCompeteActivityItem.TimeStatus.End:
			{
				DateTime dateTime3 = correctDateTime.AddMonths(1);
				DateTime newDateTime = this.GetNewDateTime(dateTime3, this.DayOfOpenActivity, array[0]);
				seconds = ActivityTimeManager.GetRemainderSeconds(newDateTime, correctDateTime);
				break;
			}
			}
		}
		else if (correctDateTime.Day < this.DayOfOpenActivity)
		{
			this.mTimeStatus = TeamCompeteActivityItem.TimeStatus.NotBegin;
			DateTime newDateTime2 = this.GetNewDateTime(correctDateTime, this.DayOfOpenActivity, array[0]);
			seconds = ActivityTimeManager.GetRemainderSeconds(newDateTime2, correctDateTime);
		}
		else
		{
			this.mTimeStatus = TeamCompeteActivityItem.TimeStatus.NotBegin;
			DateTime dateTime4 = correctDateTime.AddMonths(1);
			DateTime newDateTime3 = this.GetNewDateTime(dateTime4, this.DayOfOpenActivity, array[0]);
			seconds = ActivityTimeManager.GetRemainderSeconds(newDateTime3, correctDateTime);
		}
		base.StartCoroutine<bool>(this.TeamZhengBaCountDown(seconds));
	}

	private DateTime GetNewDateTime(DateTime _dateTime, int _DayOfOpenActivity, string _timeOfDay)
	{
		DateTime result = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			_dateTime.Year,
			_dateTime.Month,
			_DayOfOpenActivity,
			_timeOfDay
		});
		DateTime.TryParse(text, ref result);
		return result;
	}

	private void IsInCurrentDayActivityTime(DateTime begin, DateTime end)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime < begin)
		{
			this.mTimeStatus = TeamCompeteActivityItem.TimeStatus.NotBegin;
		}
		else if (begin <= correctDateTime && correctDateTime <= end)
		{
			this.mTimeStatus = TeamCompeteActivityItem.TimeStatus.Beginning;
		}
		else
		{
			this.mTimeStatus = TeamCompeteActivityItem.TimeStatus.End;
		}
	}

	private IEnumerator TeamZhengBaCountDown(long seconds)
	{
		switch (this.mTimeStatus)
		{
		case TeamCompeteActivityItem.TimeStatus.NotBegin:
			this.color = "ff0000";
			this.LblShowType = this.ActivityNotBegin;
			break;
		case TeamCompeteActivityItem.TimeStatus.Beginning:
			this.color = "17e43e";
			this.LblShowType = this.ActivityBegin;
			break;
		case TeamCompeteActivityItem.TimeStatus.End:
			this.color = "ff0000";
			this.LblShowType = this.ActivityNotBegin;
			break;
		}
		while (seconds > 0L)
		{
			yield return new WaitForSeconds(1f);
			switch (this.mTimeStatus)
			{
			case TeamCompeteActivityItem.TimeStatus.NotBegin:
				this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					Global.GetString(new object[]
					{
						this.leftDes,
						this.GetTimeStrBySecEx((double)seconds)
					})
				});
				break;
			case TeamCompeteActivityItem.TimeStatus.Beginning:
				this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					Global.GetString(new object[]
					{
						this.leftDes,
						this.GetTimeStrBySecEx((double)seconds)
					})
				});
				break;
			case TeamCompeteActivityItem.TimeStatus.End:
				this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					Global.GetString(new object[]
					{
						this.leftDes,
						this.GetTimeStrBySecEx((double)seconds)
					})
				});
				break;
			}
			seconds -= 1L;
		}
		this.DisplayTeamZhengBaActivityTime();
		yield break;
	}

	private void DisplayDaTaoShaActivityTime()
	{
		EscapeActivityRulesVO escapeActivityRulesVODataById = IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeActivityRulesVODataById(1);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		int dayOfWeek = ActivityTimeManager.GetDayOfWeek(correctDateTime.DayOfWeek);
		string[] timeStr = null;
		string[] timeStr2 = null;
		ActivityTimeManager.SplitStrTimePoints(escapeActivityRulesVODataById.TimePoints, out timeStr, out timeStr2);
		int battleSignSecs = escapeActivityRulesVODataById.BattleSignSecs;
		CfgActivityTimeData cfgActivityTimeData = this.ParseTimeData(timeStr, battleSignSecs);
		DateTime beginTime = default(DateTime);
		DateTime endTime = default(DateTime);
		ActivityTimeManager.GetAcitivtyBeginAndEndDateTime(cfgActivityTimeData.dayOfWeek, cfgActivityTimeData.beginPoint, cfgActivityTimeData.endPoint, out beginTime, out endTime);
		MUDebug.Log<string>(new string[]
		{
			"tmpBegin1 " + beginTime.ToUniversalTime()
		});
		MUDebug.Log<string>(new string[]
		{
			"tmpEnd1 " + endTime.ToUniversalTime()
		});
		cfgActivityTimeData.beginTime = beginTime;
		cfgActivityTimeData.endTime = endTime;
		CfgActivityTimeData cfgActivityTimeData2 = this.ParseTimeData(timeStr2, battleSignSecs);
		DateTime beginTime2 = default(DateTime);
		DateTime endTime2 = default(DateTime);
		ActivityTimeManager.GetAcitivtyBeginAndEndDateTime(cfgActivityTimeData2.dayOfWeek, cfgActivityTimeData2.beginPoint, cfgActivityTimeData2.endPoint, out beginTime2, out endTime2);
		MUDebug.Log<string>(new string[]
		{
			"tmpBegin2 " + beginTime.ToUniversalTime()
		});
		MUDebug.Log<string>(new string[]
		{
			"tmpEnd2 " + endTime.ToUniversalTime()
		});
		cfgActivityTimeData2.beginTime = beginTime2;
		cfgActivityTimeData2.endTime = endTime2;
		long seconds = 0L;
		if (correctDateTime < cfgActivityTimeData.beginTime)
		{
			if (cfgActivityTimeData.beginTime > cfgActivityTimeData2.beginTime)
			{
				if (correctDateTime < cfgActivityTimeData2.beginTime)
				{
					this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.NotStart;
					seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData2.beginTime, correctDateTime);
				}
				else if (cfgActivityTimeData2.beginTime <= correctDateTime && correctDateTime <= cfgActivityTimeData2.endTime)
				{
					this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.Start;
					seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData2.endTime, correctDateTime);
				}
				else if (cfgActivityTimeData2.endTime < correctDateTime)
				{
					this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.NotStart;
					seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData.beginTime, correctDateTime);
				}
			}
			else
			{
				this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.NotStart;
				seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData.beginTime, correctDateTime);
			}
		}
		else if (cfgActivityTimeData.beginTime <= correctDateTime && correctDateTime <= cfgActivityTimeData.endTime)
		{
			this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.Start;
			seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData.endTime, correctDateTime);
		}
		else if (cfgActivityTimeData.endTime < correctDateTime && correctDateTime < cfgActivityTimeData2.beginTime)
		{
			this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.NotStart;
			seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData2.beginTime, correctDateTime);
		}
		else if (cfgActivityTimeData2.beginTime <= correctDateTime && correctDateTime <= cfgActivityTimeData2.endTime)
		{
			this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.Start;
			seconds = ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData2.endTime, correctDateTime);
		}
		else if (cfgActivityTimeData2.endTime < correctDateTime)
		{
			this.daTaoShaTimeState = TeamCompeteActivityItem.ActivityTimeState.NotStart;
			this.DisplayDaTaoShaActivityTime();
			return;
		}
		base.StartCoroutine<bool>(this.DaTaoShaCountDown(seconds));
	}

	private IEnumerator DaTaoShaCountDown(long seconds)
	{
		switch (this.daTaoShaTimeState)
		{
		case TeamCompeteActivityItem.ActivityTimeState.NotStart:
			this.color = "ff0000";
			this.LblShowType = this.ActivityNotBegin;
			break;
		case TeamCompeteActivityItem.ActivityTimeState.Start:
			this.color = "17e43e";
			this.LblShowType = this.ActivityBegin;
			break;
		case TeamCompeteActivityItem.ActivityTimeState.End:
			this.color = "ff0000";
			this.LblShowType = this.ActivityNotBegin;
			break;
		}
		while (seconds > 0L)
		{
			yield return new WaitForSeconds(1f);
			switch (this.daTaoShaTimeState)
			{
			case TeamCompeteActivityItem.ActivityTimeState.NotStart:
				this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					Global.GetString(new object[]
					{
						this.leftDes,
						this.GetTimeStrBySecEx((double)seconds)
					})
				});
				break;
			case TeamCompeteActivityItem.ActivityTimeState.Start:
				this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					Global.GetString(new object[]
					{
						this.leftDes,
						this.GetTimeStrBySecEx((double)seconds)
					})
				});
				break;
			case TeamCompeteActivityItem.ActivityTimeState.End:
				this.LblShowTime = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					Global.GetString(new object[]
					{
						this.leftDes,
						this.GetTimeStrBySecEx((double)seconds)
					})
				});
				break;
			}
			seconds -= 1L;
		}
		this.DisplayDaTaoShaActivityTime();
		yield break;
	}

	private CfgActivityTimeData ParseTimeData(string[] timeStr, int interval = 0)
	{
		CfgActivityTimeData result = default(CfgActivityTimeData);
		result.dayOfWeek = timeStr[0].SafeToInt32(0);
		string[] array = timeStr[1].Split(new char[]
		{
			'-'
		});
		result.beginPoint = array[0];
		result.endPoint = array[1];
		result.timeInterval = interval;
		return result;
	}

	private void GetAcitivtyStartAndEndDateTime(int cfgDayOfWeek, string startTimeStr, string endTimeStr, out DateTime startDateTime, out DateTime endDateTime)
	{
		startDateTime = default(DateTime);
		endDateTime = default(DateTime);
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			startTimeStr
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
		int normalWeekday = this.GetNormalWeekday(correctDateTime.DayOfWeek);
		cfgDayOfWeek = this.GetNormalWeekday(cfgDayOfWeek);
		int num;
		if (normalWeekday == 7)
		{
			if (cfgDayOfWeek == 7)
			{
				num = 0;
			}
			else
			{
				num = 7 - Mathf.Abs(cfgDayOfWeek - normalWeekday);
			}
		}
		else if (cfgDayOfWeek > normalWeekday)
		{
			num = cfgDayOfWeek - normalWeekday;
		}
		else if (cfgDayOfWeek < normalWeekday)
		{
			num = 7 - (normalWeekday - cfgDayOfWeek);
		}
		else
		{
			num = cfgDayOfWeek - normalWeekday;
		}
		startDateTime = dateTime.AddDays((double)num);
		endDateTime = dateTime2.AddDays((double)num);
	}

	private long GetRemainderTicks(DateTime max, DateTime min)
	{
		return max.Ticks - min.Ticks;
	}

	private int GetNormalWeekday(int day)
	{
		return (day != 0) ? day : 7;
	}

	private string GetTimeStrBySecEx(double sec)
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

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.StopTeamCompeteTimer();
		base.StopAllCoroutines();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblType;

	public TextBlock LblTime;

	public ShowNetImage img;

	private string color = "17e43e";

	public ZhanDuiHuoDongTabVo vo;

	private TeamCompeteActivityItem.ActivityTimeState teamCompeteTimeState;

	private DateTime mTeamCompeteStartTime = default(DateTime);

	private DateTime mTeamCompeteEndTime = default(DateTime);

	private int mTeamCompeteClearResTime = -1;

	private long remainderTeamCompeteTicks = -1L;

	private DispatcherTimer TeamCompeteUITimer;

	private TeamCompeteActivityItem.TimeStatus mTimeStatus;

	private int DayOfOpenActivity;

	private string ActivityNotBegin = Global.GetLang("距离活动开始");

	private string ActivityBegin = Global.GetLang("距离活动结束");

	private string leftDes = Global.GetLang("剩余");

	private TeamCompeteActivityItem.ActivityTimeState daTaoShaTimeState;

	private enum ActivityTimeState
	{
		None,
		NotStart,
		Start,
		End
	}

	private enum TimeStatus
	{
		NotBegin,
		Beginning,
		End
	}
}
