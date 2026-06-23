using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class DaTaoShaMainPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitValue();
		this.InitAwardGoods();
		this.InitEvent();
		this.RequestMainDataMsg();
	}

	private void InitTextInPrefabs()
	{
		this.BtnRule.Label.text = Global.GetLang("详细规则");
		this.BtnConfirm.Label.text = ((!TeamCompeteDataManager.IsTeamLeader) ? Global.GetLang("参与活动") : Global.GetLang("报  名"));
		this.mLblNotOpen.Text = Global.GetLang("活动时间未开始");
	}

	private bool IsShowBaoMingBtn
	{
		set
		{
			this.BtnConfirm.gameObject.SetActive(value);
			this.mLblNotOpen.gameObject.SetActive(!value);
			if (value)
			{
				if (!this.isOpenRepeating)
				{
					this.isOpenRepeating = true;
					base.InvokeRepeating("RepeatRequestMainData", 10f, 10f);
				}
			}
			else if (this.isOpenRepeating)
			{
				this.isOpenRepeating = false;
			}
			switch (this.ActivityStatus)
			{
			case EscapeBattleGameStates.Wait:
				if (Global.Data.roleData != null && Global.Data.roleData.ZhanDuiID > 0)
				{
					this.BtnConfirm.Label.text = Global.GetLang("参与活动");
				}
				break;
			case EscapeBattleGameStates.Start:
				if (Global.Data.roleData != null && Global.Data.roleData.ZhanDuiID > 0)
				{
					this.BtnConfirm.Label.text = Global.GetLang("参与活动");
				}
				break;
			}
		}
	}

	private void RepeatRequestMainData()
	{
		this.RequestMainDataMsg();
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnSeasonRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenDaTaoShaSeasonRankPart();
		};
		this.BtnAwardPreview.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenJiangLiYuLanPart();
		};
		this.BtnRule.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenCommonHelpWindow();
		};
		this.BtnShiPin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 6
				});
			}
		};
		this.IsShowBaoMingBtn = this.IsInActivityTime();
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData != null)
			{
				if (Global.Data.roleData.ZhanDuiID <= 0)
				{
					Super.HintMainText(Global.GetLang("您当前没有战队"), 10, 3);
					return;
				}
				if (!TeamCompeteDataManager.IsTeamLeader && this.ActivityStatus <= EscapeBattleGameStates.SignUp)
				{
					Super.HintMainText(Global.GetLang("战队尚未报名"), 10, 3);
					return;
				}
			}
			switch (this.ActivityStatus)
			{
			case EscapeBattleGameStates.SignUp:
				GameInstance.Game.RequestRegisterDaTaoSha();
				break;
			case EscapeBattleGameStates.Wait:
				GameInstance.Game.RequestEnterDaTaoShaScene();
				break;
			case EscapeBattleGameStates.Start:
				GameInstance.Game.RequestEnterDaTaoShaScene();
				break;
			}
		};
	}

	private void InitValue()
	{
	}

	private void InitAwardGoods()
	{
		string[] escapeActivityRulesAwardID = IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeActivityRulesAwardID();
		for (int i = 0; i < escapeActivityRulesAwardID.Length; i++)
		{
			GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(escapeActivityRulesAwardID[i], false, true, true);
			if (!(ggoodIcon == null))
			{
				ggoodIcon.transform.SetParent(this.AwardGoodsRoot.transform);
				ggoodIcon.isAutoSize = false;
				ggoodIcon.transform.localPosition = new Vector3((float)(i * 72), 0f, 0f);
				ggoodIcon.transform.localScale = Vector3.one;
			}
		}
	}

	private void InitActivityTime(float addSeconds = 0f)
	{
		EscapeActivityRulesVO escapeActivityRulesVODataById = IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeActivityRulesVODataById(1);
		DateTime dateTime = Global.GetCorrectDateTime().AddSeconds((double)addSeconds);
		int dayOfWeek = ActivityTimeManager.GetDayOfWeek(dateTime.DayOfWeek);
		string[] array = null;
		string[] array2 = null;
		ActivityTimeManager.SplitStrTimePoints(escapeActivityRulesVODataById.TimePoints, out array, out array2);
		this.registerTimeInterval = escapeActivityRulesVODataById.BattleSignSecs;
		StringBuilder stringBuilder = new StringBuilder();
		this.LblActivityTimeDes.Text = Global.GetString(new object[]
		{
			Global.GetLang("活动时间: "),
			ActivityTimeManager.GetChineseDayOfWeek(array[0].SafeToInt32(0)),
			array[1],
			" | ",
			ActivityTimeManager.GetChineseDayOfWeek(array2[0].SafeToInt32(0)),
			array2[1]
		});
		CfgActivityTimeData cfgActivityTimeData = this.ParseTimeData(array, this.registerTimeInterval);
		DateTime beginTime = default(DateTime);
		DateTime endTime = default(DateTime);
		ActivityTimeManager.GetAcitivtyBeginAndEndDateTimeByNowTime(dateTime, cfgActivityTimeData.dayOfWeek, cfgActivityTimeData.beginPoint, cfgActivityTimeData.endPoint, out beginTime, out endTime);
		cfgActivityTimeData.beginTime = beginTime;
		cfgActivityTimeData.endTime = endTime;
		cfgActivityTimeData.OpenActivityCount = (int)(ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData.endTime, cfgActivityTimeData.beginTime) / (long)cfgActivityTimeData.timeInterval);
		CfgActivityTimeData cfgActivityTimeData2 = this.ParseTimeData(array2, this.registerTimeInterval);
		DateTime beginTime2 = default(DateTime);
		DateTime endTime2 = default(DateTime);
		ActivityTimeManager.GetAcitivtyBeginAndEndDateTimeByNowTime(dateTime, cfgActivityTimeData2.dayOfWeek, cfgActivityTimeData2.beginPoint, cfgActivityTimeData2.endPoint, out beginTime2, out endTime2);
		cfgActivityTimeData2.beginTime = beginTime2;
		cfgActivityTimeData2.endTime = endTime2;
		cfgActivityTimeData2.OpenActivityCount = (int)(ActivityTimeManager.GetRemainderSeconds(cfgActivityTimeData2.endTime, cfgActivityTimeData2.beginTime) / (long)cfgActivityTimeData2.timeInterval);
		if (dateTime < cfgActivityTimeData.beginTime)
		{
			if (cfgActivityTimeData.beginTime > cfgActivityTimeData2.beginTime)
			{
				if (dateTime < cfgActivityTimeData2.beginTime)
				{
					this.IsActivityOpen = false;
					this.ActivityNotBegin(ActivityTimeManager.GetRemainderTicks(cfgActivityTimeData2.beginTime, dateTime));
				}
				else if (cfgActivityTimeData2.beginTime <= dateTime && dateTime <= cfgActivityTimeData2.endTime)
				{
					this.IsActivityOpen = true;
					this.RegisterCountDown(cfgActivityTimeData2.beginTime, cfgActivityTimeData2.endTime, cfgActivityTimeData2.OpenActivityCount);
				}
				else if (cfgActivityTimeData2.endTime < dateTime)
				{
					this.IsActivityOpen = false;
					this.ActivityNotBegin(ActivityTimeManager.GetRemainderTicks(cfgActivityTimeData.beginTime, dateTime));
				}
			}
			else
			{
				this.IsActivityOpen = false;
				this.ActivityNotBegin(ActivityTimeManager.GetRemainderTicks(cfgActivityTimeData.beginTime, dateTime));
			}
		}
		else if (cfgActivityTimeData.beginTime <= dateTime && dateTime <= cfgActivityTimeData.endTime)
		{
			this.IsActivityOpen = true;
			this.RegisterCountDown(cfgActivityTimeData.beginTime, cfgActivityTimeData.endTime, cfgActivityTimeData.OpenActivityCount);
		}
		else if (cfgActivityTimeData.endTime < dateTime && dateTime < cfgActivityTimeData2.beginTime)
		{
			this.IsActivityOpen = false;
			this.ActivityNotBegin(ActivityTimeManager.GetRemainderTicks(cfgActivityTimeData2.beginTime, dateTime));
		}
		else if (cfgActivityTimeData2.beginTime <= dateTime && dateTime <= cfgActivityTimeData2.endTime)
		{
			this.IsActivityOpen = true;
			this.RegisterCountDown(cfgActivityTimeData2.beginTime, cfgActivityTimeData2.endTime, cfgActivityTimeData2.OpenActivityCount);
		}
		else if (cfgActivityTimeData2.endTime < dateTime)
		{
			this.IsActivityOpen = false;
			this.InitActivityTime((float)this.intervalServerAndCenterSeconds);
		}
		this.IsShowBaoMingBtn = this.IsActivityOpen;
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

	private void RegisterCountDown(DateTime begin, DateTime end, int activitySumCount)
	{
		MUDebug.LogError<string>(new string[]
		{
			"###活动开始时间：" + begin.ToLongDateString()
		});
		MUDebug.LogError<string>(new string[]
		{
			"###活动结束时间：" + end.ToLongDateString()
		});
		MUDebug.LogError<string>(new string[]
		{
			"###报名总场次：" + activitySumCount
		});
		DateTime correctDateTime = Global.GetCorrectDateTime();
		int num = (int)ActivityTimeManager.GetRemainderSeconds(end, correctDateTime);
		int num2 = num / this.registerTimeInterval;
		MUDebug.LogError<string>(new string[]
		{
			"###剩余报名总场次：" + num2
		});
		int num3 = (int)ActivityTimeManager.GetRemainderSeconds(correctDateTime, begin);
		int lostRegisterCount = num3 / this.registerTimeInterval + 1;
		MUDebug.LogError<string>(new string[]
		{
			"###已过报名总场次：" + lostRegisterCount
		});
		int num4 = this.registerTimeInterval - num3 % this.registerTimeInterval;
		MUDebug.LogError<string>(new string[]
		{
			"###当前报名剩余时间：" + num4
		});
		base.StartCoroutine(this.BeginRegisterCountDown(num4, lostRegisterCount, delegate
		{
			if (lostRegisterCount >= activitySumCount)
			{
				this.InitActivityTime((float)this.intervalServerAndCenterSeconds);
			}
			else
			{
				this.mLeftRegisterCount = lostRegisterCount + 1;
				this.mInterval = this.registerTimeInterval;
				this.mActivitySumCount = activitySumCount;
				this.StartDaTaoShaBaoMingCountDown();
			}
		}));
	}

	private IEnumerator BeginRegisterCountDown(int interval, int lostRegisterCount, Action action)
	{
		while (interval > 0)
		{
			this.color = "17e43e";
			this.LblCountDown.Text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				Global.GetString(new object[]
				{
					Global.GetLang("距离第"),
					lostRegisterCount,
					Global.GetLang("场报名结束剩余"),
					interval,
					Global.GetLang("秒")
				})
			});
			interval--;
			yield return new WaitForSeconds(1f);
		}
		if (action != null)
		{
			action.Invoke();
		}
		yield break;
	}

	private void StartDaTaoShaBaoMingCountDown()
	{
		this.DaTaoShaBaoMingUITimer = new DispatcherTimer("DaTaoShaBaoMingCountDownUITimer");
		this.DaTaoShaBaoMingUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.DaTaoShaBaoMingUITimer.Tick = new DispatcherTimerEventHandler(this.DaTaoShaBaoMingUITimer_Tick);
		this.DaTaoShaBaoMingUITimer.Start();
	}

	protected void DaTaoShaBaoMingUITimer_Tick(object sender, object e)
	{
		if (this.mInterval > 0)
		{
			this.color = "17e43e";
			this.LblCountDown.Text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				Global.GetString(new object[]
				{
					Global.GetLang("距离第"),
					this.mLeftRegisterCount,
					Global.GetLang("场报名结束剩余"),
					this.mInterval,
					Global.GetLang("秒")
				})
			});
			this.mInterval--;
		}
		else
		{
			this.mLeftRegisterCount++;
			if (this.mLeftRegisterCount <= this.mActivitySumCount)
			{
				this.StopDaTaoShaBaoMingTimer();
				this.mInterval = this.registerTimeInterval;
			}
			else
			{
				this.mLeftRegisterCount = 0;
				this.mInterval = 0;
				this.mActivitySumCount = 0;
				this.InitActivityTime((float)this.intervalServerAndCenterSeconds);
			}
		}
	}

	private void StopDaTaoShaBaoMingTimer()
	{
		if (this.DaTaoShaBaoMingUITimer != null)
		{
			this.DaTaoShaBaoMingUITimer.Tick = null;
			this.DaTaoShaBaoMingUITimer.Stop();
			this.DaTaoShaBaoMingUITimer = null;
		}
	}

	private void OpenAcitivityCountDown(string cfgDayOfWeek, string beginTimeStr, string endTimeStr)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime max = default(DateTime);
		DateTime dateTime = default(DateTime);
		int cfgDayOfWeek2 = -1;
		if (int.TryParse(cfgDayOfWeek, ref cfgDayOfWeek2))
		{
			ActivityTimeManager.GetAcitivtyBeginAndEndDateTime(cfgDayOfWeek2, beginTimeStr, endTimeStr, out max, out dateTime);
			this.ActivityNotBegin(ActivityTimeManager.GetRemainderTicks(max, correctDateTime));
		}
	}

	private void ActivityNotBegin(long ticks)
	{
		this.color = "ff0000";
		this.LblCountDown.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.color,
			Global.GetLang("距离活动开始")
		});
		this.remainderTicks = ticks / 10000000L;
		this.StartDaTaoShaActivityCountDown();
	}

	private void StartDaTaoShaActivityCountDown()
	{
		this.DaTaoShaActivityUITimer = new DispatcherTimer("DaTaoShaUITimer");
		this.DaTaoShaActivityUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.DaTaoShaActivityUITimer.Tick = new DispatcherTimerEventHandler(this.DaTaoShaActivityUITimer_Tick);
		this.DaTaoShaActivityUITimer.Start();
	}

	protected void DaTaoShaActivityUITimer_Tick(object sender, object e)
	{
		if (this.remainderTicks > 0L)
		{
			this.LblCountDown.Text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				string.Format(Global.GetLang("距离活动开始剩余 {0}"), ActivityTimeManager.GetTimeStrBySecEx((double)this.remainderTicks))
			});
			this.remainderTicks -= 1L;
		}
		else
		{
			this.StopDaTaoShaActivityTimer();
			this.InitActivityTime((float)this.intervalServerAndCenterSeconds);
		}
	}

	private void StopDaTaoShaActivityTimer()
	{
		if (this.DaTaoShaActivityUITimer != null)
		{
			this.DaTaoShaActivityUITimer.Tick = null;
			this.DaTaoShaActivityUITimer.Stop();
			this.DaTaoShaActivityUITimer = null;
		}
	}

	private bool IsInActivityTime()
	{
		return this.IsActivityOpen;
	}

	public void OpenDaTaoShaSeasonRankPart()
	{
		if (this.mDaTaoShaSeasonRankPartWind != null || this.mDaTaoShaSeasonRankPart != null)
		{
			this.CloseDaTaoShaSeasonRankPart();
		}
		this.mDaTaoShaSeasonRankPartWind = U3DUtils.NEW<GChildWindow>();
		this.mDaTaoShaSeasonRankPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mDaTaoShaSeasonRankPartWind.Modal = true;
		this.mDaTaoShaSeasonRankPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mDaTaoShaSeasonRankPartWind, "mDaTaoShaSeasonRankPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mDaTaoShaSeasonRankPartWind);
		this.mDaTaoShaSeasonRankPart = U3DUtils.NEW<DaTaoShaSeasonRankPart>();
		this.mDaTaoShaSeasonRankPartWind.Body.Add(this.mDaTaoShaSeasonRankPart);
		this.mDaTaoShaSeasonRankPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseDaTaoShaSeasonRankPart();
		};
	}

	private void CloseDaTaoShaSeasonRankPart()
	{
		if (null != this.mDaTaoShaSeasonRankPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mDaTaoShaSeasonRankPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mDaTaoShaSeasonRankPartWind, true);
			this.mDaTaoShaSeasonRankPartWind = null;
		}
		if (null != this.mDaTaoShaSeasonRankPart)
		{
			this.mDaTaoShaSeasonRankPart.transform.parent = null;
			Object.Destroy(this.mDaTaoShaSeasonRankPart.gameObject);
			this.mDaTaoShaSeasonRankPart = null;
		}
	}

	public void OpenJiangLiYuLanPart()
	{
		if (this.mJiangLiYuLanPartWind != null || this.mJiangLiYuLanPart != null)
		{
			this.CloseJiangLiYuLanPart();
		}
		this.initXmlData();
		this.mJiangLiYuLanPartWind = U3DUtils.NEW<GChildWindow>();
		this.mJiangLiYuLanPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mJiangLiYuLanPartWind.Modal = true;
		this.mJiangLiYuLanPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mJiangLiYuLanPartWind, "mDaTaoShaJiangLiYuLanPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mJiangLiYuLanPartWind);
		this.mJiangLiYuLanPart = U3DUtils.NEW<JiangLiYuLanPart>();
		this.mJiangLiYuLanPartWind.Body.Add(this.mJiangLiYuLanPart);
		this.mJiangLiYuLanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseJiangLiYuLanPart();
		};
		base.StartCoroutine<bool>(this.mJiangLiYuLanPart.init(this.awardXmlList, 0, Global.GetLang("奖励预览")));
	}

	private void CloseJiangLiYuLanPart()
	{
		if (null != this.mJiangLiYuLanPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mJiangLiYuLanPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mJiangLiYuLanPartWind, true);
			this.mJiangLiYuLanPartWind = null;
		}
		if (null != this.mJiangLiYuLanPart)
		{
			this.mJiangLiYuLanPart.transform.parent = null;
			Object.Destroy(this.mJiangLiYuLanPart.gameObject);
			this.mJiangLiYuLanPart = null;
		}
	}

	private void initXmlData()
	{
		if (this.awardXmlList != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/EscapeRankAward.xml");
		if (gameResXml != null)
		{
			this.awardXmlList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		}
	}

	public void OpenCommonCompeteMonthAwardPart(int rankId = 0)
	{
		if (this.mCommonCompeteMonthAwardPartWind != null || this.mCommonCompeteMonthAwardPart != null)
		{
			this.CloseCommonCompeteMonthAwardPart();
		}
		this.mCommonCompeteMonthAwardPartWind = U3DUtils.NEW<GChildWindow>();
		this.mCommonCompeteMonthAwardPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mCommonCompeteMonthAwardPartWind.Modal = true;
		this.mCommonCompeteMonthAwardPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mCommonCompeteMonthAwardPartWind, "mCommonCompeteMonthAwardPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mCommonCompeteMonthAwardPartWind);
		this.mCommonCompeteMonthAwardPart = U3DUtils.NEW<CommonCompeteMonthAwardPart>();
		this.mCommonCompeteMonthAwardPartWind.Body.Add(this.mCommonCompeteMonthAwardPart);
		this.mCommonCompeteMonthAwardPart.InitDaTaoShaAward(rankId);
		this.mCommonCompeteMonthAwardPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCommonCompeteMonthAwardPart();
		};
	}

	private void CloseCommonCompeteMonthAwardPart()
	{
		if (null != this.mCommonCompeteMonthAwardPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mCommonCompeteMonthAwardPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mCommonCompeteMonthAwardPartWind, true);
			this.mCommonCompeteMonthAwardPartWind = null;
		}
		if (null != this.mCommonCompeteMonthAwardPart)
		{
			this.mCommonCompeteMonthAwardPart.transform.parent = null;
			Object.Destroy(this.mCommonCompeteMonthAwardPart.gameObject);
			this.mCommonCompeteMonthAwardPart = null;
		}
	}

	public void RespondRankAward(MUSocketConnectEventArgs e)
	{
		if (this.mCommonCompeteMonthAwardPart != null)
		{
			this.mCommonCompeteMonthAwardPart.RespondAcceptAward(e);
		}
	}

	public void OpenCommonHelpWindow()
	{
		if (this.mCommonHelpWindowWind != null || this.mCommonHelpWindow != null)
		{
			this.CloseCommonHelpWindow();
		}
		this.mCommonHelpWindowWind = U3DUtils.NEW<GChildWindow>();
		this.mCommonHelpWindowWind.ModalType = ChildWindowModalType.Translucent;
		this.mCommonHelpWindowWind.Modal = true;
		this.mCommonHelpWindowWind.IsShowModal = false;
		Super.InitChildWindow(this.mCommonHelpWindowWind, "mCommonHelpWindowWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mCommonHelpWindowWind);
		this.mCommonHelpWindow = U3DUtils.NEW<CommonHelpWindow>();
		this.mCommonHelpWindowWind.Body.Add(this.mCommonHelpWindow);
		this.mCommonHelpWindow.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCommonHelpWindow();
		};
		this.mCommonHelpWindow.SetHelpInfo(IConfigbase<ConfigTeamCompete>.Instance.GetTeamCompeteHelpInfo(2).list);
	}

	private void CloseCommonHelpWindow()
	{
		if (null != this.mCommonHelpWindowWind)
		{
			Super.CloseChildWindow(base.Children, this.mCommonHelpWindowWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mCommonHelpWindowWind, true);
			this.mCommonHelpWindowWind = null;
		}
		if (null != this.mCommonHelpWindow)
		{
			this.mCommonHelpWindow.transform.parent = null;
			Object.Destroy(this.mCommonHelpWindow.gameObject);
			this.mCommonHelpWindow = null;
		}
	}

	public void CloseAllWindow()
	{
		this.CloseDaTaoShaSeasonRankPart();
		this.CloseJiangLiYuLanPart();
		this.CloseCommonCompeteMonthAwardPart();
		this.CloseCommonHelpWindow();
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_ESCAPE_STATE", new Action<MUSocketConnectEventArgs>(this.RespondMainDataMsg));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_ESCAPE_STATE", new Action<MUSocketConnectEventArgs>(this.RespondMainDataMsg));
	}

	public void RequestMainDataMsg()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.RequestDaTaoShaMainData();
	}

	public void RespondMainDataMsg(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		this.StopAllTimer();
		this.intervalServerAndCenterSeconds = e.fields[0].SafeToInt32(0);
		MUDebug.LogError<string>(new string[]
		{
			"intervalServerAndCenterSeconds " + this.intervalServerAndCenterSeconds
		});
		this.ActivityStatus = (EscapeBattleGameStates)e.fields[1].SafeToInt32(0);
		MUDebug.LogError<string>(new string[]
		{
			"EscapeBattleGameStates 当前活动状态： " + this.ActivityStatus.ToString()
		});
		this.InitActivityTime((float)this.intervalServerAndCenterSeconds);
		int num = e.fields[2].SafeToInt32(0);
		if (num > 0 && (this.mCommonCompeteMonthAwardPartWind == null || this.mCommonCompeteMonthAwardPart == null))
		{
			this.OpenCommonCompeteMonthAwardPart(num);
		}
		this.IsShowBaoMingBtn = (this.ActivityStatus == EscapeBattleGameStates.Wait || this.ActivityStatus == EscapeBattleGameStates.Start || this.IsActivityOpen);
		if (TeamCompeteDataManager.IsTeamLeader)
		{
			this.IsShowBaoMingBtn = (this.ActivityStatus == EscapeBattleGameStates.SignUp || this.ActivityStatus == EscapeBattleGameStates.Wait || this.ActivityStatus == EscapeBattleGameStates.Start || this.IsActivityOpen);
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.StopAllTimer();
		base.CancelInvoke();
	}

	private void StopAllTimer()
	{
		this.StopDaTaoShaActivityTimer();
		this.StopDaTaoShaBaoMingTimer();
		base.StopAllCoroutines();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblCountDown;

	public TextBlock LblActivityTimeDes;

	public GButton BtnClose;

	public GButton BtnSeasonRank;

	public GButton BtnAwardPreview;

	public GButton BtnShiPin;

	public GButton BtnRule;

	public GButton BtnConfirm;

	public GameObject AwardGoodsRoot;

	public TextBlock mLblNotOpen;

	private bool isOpenRepeating;

	private int registerTimeInterval;

	private int sumSec1;

	private int sumSec2;

	private int openActivityCount1;

	private int openActivityCount2;

	private bool IsActivityOpen;

	private int mLeftRegisterCount;

	private int mInterval;

	private int mActivitySumCount;

	private DispatcherTimer DaTaoShaBaoMingUITimer;

	private long remainderTicks = -1L;

	private string color = "ff0000";

	private DispatcherTimer DaTaoShaActivityUITimer;

	protected GChildWindow mDaTaoShaSeasonRankPartWind;

	protected DaTaoShaSeasonRankPart mDaTaoShaSeasonRankPart;

	protected GChildWindow mJiangLiYuLanPartWind;

	protected JiangLiYuLanPart mJiangLiYuLanPart;

	private List<XElement> awardXmlList;

	protected GChildWindow mCommonCompeteMonthAwardPartWind;

	protected CommonCompeteMonthAwardPart mCommonCompeteMonthAwardPart;

	protected GChildWindow mCommonHelpWindowWind;

	protected CommonHelpWindow mCommonHelpWindow;

	private int intervalServerAndCenterSeconds;

	private EscapeBattleGameStates ActivityStatus;
}
