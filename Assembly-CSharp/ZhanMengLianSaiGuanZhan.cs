using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhanMengLianSaiGuanZhan : UserControl
{
	public ObservableCollection LastItemCollection
	{
		get
		{
			return this._LastItemCollection;
		}
		set
		{
			this._LastItemCollection = value;
		}
	}

	public ObservableCollection CurItemCollection
	{
		get
		{
			return this._CurItemCollection;
		}
		set
		{
			this._CurItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.InitCurrentActiveType();
	}

	private void InitTextInPrefabs()
	{
		this.mBtnRank.Text = Global.GetLang("赛季排名");
		this.mBtnAward.Text = Global.GetLang("领取奖励");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetZhanMengLianSaiRankInfo(8);
		};
		this.mBtnAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendZhanMengLianSaiJingCaiRankInfoData();
		};
		this.mBtnJingCaiShop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenShop();
		};
	}

	private void InitValue()
	{
		this.LblTime.Text = string.Empty;
		this.mLblCurZhanKuang.Text = Global.GetLang("第零轮战况");
		this.LblLastZhanKuang.Text = Global.GetLang("上轮战况");
		this.LastItemCollection = this.mLastListBox.ItemsSource;
		this.CurItemCollection = this.mCurListBox.ItemsSource;
		this.RefreshJingCaiJiFen();
	}

	public void InitData(BangHuiMatchMainInfo data, bool isLastRound = false)
	{
		if (data != null)
		{
			this.curGameState = data.timestate;
			this.mLblCurZhanKuang.Text = string.Format("{0}{1}{2}", Global.GetLang("第"), this.GetChineseNumber(data.round), Global.GetLang("轮战况"));
			this.InitLastZhanKuangInfo(data.LastRoundPKInfo);
			this.InitCurrentZhanKuangInfo(data.CurRoundPKInfo, data, isLastRound);
			if ((data.LastRoundPKInfo == null || data.LastRoundPKInfo.Count <= 0) && (data.CurRoundPKInfo == null || data.CurRoundPKInfo.Count <= 0))
			{
				NGUITools.SetActive(this.mUpPanelObj, false);
				NGUITools.SetActive(this.mDownPanelObj, false);
				Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			}
		}
	}

	public void InitRankData(List<BangHuiMatchRankInfo> rankData)
	{
		if (rankData == null || rankData.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		ZhanMengLianSaiGuanZhanRank zhanMengLianSaiGuanZhanRank = this.OpenWindow();
		if (zhanMengLianSaiGuanZhanRank)
		{
			zhanMengLianSaiGuanZhanRank.InitRankData(rankData);
		}
	}

	public void InitJingCaiAwardData(List<BangHuiMatchGuessInfo> awardData)
	{
		if (awardData == null || awardData.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("无可领取奖励"), 10, 3);
			return;
		}
		this.mJingCaiAwardWindow = this.OpenWindow();
		if (this.mJingCaiAwardWindow)
		{
			this.mJingCaiAwardWindow.InitJingCaiAwardData(awardData);
		}
	}

	public void RefreshYaZhuStatusByServer(string[] feilds)
	{
		int num = Global.SafeConvertToInt32(feilds[0]);
		int num2 = Global.SafeConvertToInt32(feilds[1]);
		int num3 = Global.SafeConvertToInt32(feilds[2]);
		int result = Global.SafeConvertToInt32(feilds[3]);
		if (num >= 0)
		{
			Super.HintMainText(Global.GetLang("押注成功"), 10, 3);
			if (this.mJingCaiWindow != null)
			{
				this.mJingCaiWindow.RefreshYaZhuStatusByServer(true);
			}
			for (int i = 0; i < this.CurItemCollection.Count; i++)
			{
				ZhanMengLianSaiGuanZhanItem zhanMengLianSaiGuanZhanItem = U3DUtils.AS<ZhanMengLianSaiGuanZhanItem>(this.CurItemCollection.GetAt(i));
				if (zhanMengLianSaiGuanZhanItem.Bhid1 == num2 && zhanMengLianSaiGuanZhanItem.Bhid2 == num3)
				{
					zhanMengLianSaiGuanZhanItem.RefreshYaZhuIcon(result);
				}
			}
		}
		else if (num == -19)
		{
			this.ParseLeagueSustaionXML();
			if (!string.IsNullOrEmpty(this.levelLimit))
			{
				try
				{
					string[] array = this.levelLimit.Split(new char[]
					{
						'|'
					});
					Super.HintMainText(string.Format("{0}{1}{2}{3}{4}", new object[]
					{
						Global.GetLang("参与竞猜需要等级达到"),
						array[0],
						Global.GetLang("转"),
						array[1],
						Global.GetLang("级")
					}), 10, 3);
				}
				catch (Exception ex)
				{
					MUDebug.LogError<Exception>(new Exception[]
					{
						ex
					});
				}
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(num, false, false)), 10, 3);
		}
	}

	private void ParseLeagueSustaionXML()
	{
		if (!string.IsNullOrEmpty(this.levelLimit))
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/LeagueSustain.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueSustain");
			if (xelementList != null)
			{
				int count = xelementList.Count;
				for (int i = 0; i < count; i++)
				{
					this.levelLimit = Global.GetXElementAttributeStr(xelementList[i], "MinLevel");
					if (!string.IsNullOrEmpty(this.levelLimit))
					{
						break;
					}
				}
			}
		}
		if (string.IsNullOrEmpty(this.levelLimit))
		{
			MUDebug.LogError<string>(new string[]
			{
				"LeagueSustain.XML MinLevel配置有误！"
			});
		}
	}

	public void RefreshLingQuAwardByServer(int result)
	{
		if (result >= 0)
		{
			Super.HintMainText(Global.GetLang("领取成功"), 10, 3);
			if (this.mJingCaiAwardWindow != null)
			{
				this.mJingCaiAwardWindow.RefreshLingQuAwardByServer(true);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(result, false, false)), 10, 3);
		}
	}

	private void InitCurrentActiveType()
	{
		if (Global.Data != null && Global.Data.roleData.HideGM > 0)
		{
			NGUITools.SetActive(this.mBtnAward.gameObject, false);
			NGUITools.SetActive(this.mBtnJingCaiShop.gameObject, false);
			NGUITools.SetActive(this.mShopObj.gameObject, false);
		}
	}

	private void OpenShop()
	{
		if (this.mDuiHuanWindow != null)
		{
			this.CloseDuiHuanWindow();
		}
		this.mDuiHuanWindow = U3DUtils.NEW<GChildWindow>();
		this.mDuiHuanWindow.ModalType = ChildWindowModalType.Translucent;
		this.mDuiHuanWindow.IsShowModal = true;
		Super.InitChildWindow(this.mDuiHuanWindow, "mDuiHuanWindow");
		Super.GData.GlobalPlayZone.Children.Add(this.mDuiHuanWindow);
		this.mDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
		this.mDuiHuanWindow.Body.Add(this.mDuiHuanPart);
		this.mDuiHuanPart.InitPartData(7, 0);
		this.mDuiHuanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs args)
		{
			this.CloseDuiHuanWindow();
			return false;
		};
	}

	private void CloseDuiHuanWindow()
	{
		if (null != this.mDuiHuanPart)
		{
			Object.Destroy(this.mDuiHuanPart.gameObject);
			this.mDuiHuanPart = null;
		}
		if (null != this.mDuiHuanWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.mDuiHuanWindow);
		}
		this.RefreshJingCaiJiFen();
	}

	private void RefreshJingCaiJiFen()
	{
		try
		{
			this.mLblJingCaiHuoBi.Text = Global.Data.roleData.RoleCommonUseIntPamams[50].ToString();
		}
		catch (Exception ex)
		{
			MUDebug.LogError<Exception>(new Exception[]
			{
				ex
			});
		}
	}

	private void InitLastZhanKuangInfo(List<BangHuiMatchPKInfo> LastRoundPKInfo)
	{
		this.AddItems(this.LastItemCollection, this.mLastListBox, LastRoundPKInfo, ZhanMengLianSaiGuanZhan.ZhanKuangType.Last, false);
	}

	private void InitCurrentZhanKuangInfo(List<BangHuiMatchPKInfo> CurRoundPKInfo, BangHuiMatchMainInfo info, bool isLastRound = false)
	{
		this.AddItems(this.CurItemCollection, this.mCurListBox, CurRoundPKInfo, ZhanMengLianSaiGuanZhan.ZhanKuangType.Current, isLastRound);
		if (Global.Data != null && Global.Data.roleData.HideGM <= 0 && !isLastRound)
		{
			this.InitZhanMengLianSaiJingCaiData(info);
		}
	}

	public void InitZhanMengLianSaiJingCaiData(BangHuiMatchMainInfo info)
	{
		if (info.timestate == 3)
		{
			this.isCountDownOver = true;
		}
		else if (info.timestate == 0)
		{
			this.isCountDownOver = true;
		}
		else if (!this.GetJingCaiLabelState(info))
		{
			this.ZhanMengLianSaiJingCaiCountDown(info);
		}
		if (this.isCountDownOver)
		{
			this.HideJingCaiBtn();
		}
	}

	private bool GetJingCaiLabelState(BangHuiMatchMainInfo info)
	{
		return info == null || ((info.LastRoundPKInfo == null || info.LastRoundPKInfo.Count <= 0) && (info.CurRoundPKInfo == null || info.CurRoundPKInfo.Count <= 0));
	}

	private int[] GetLeapMatchTime(int mNextBeginTime)
	{
		if (0 < mNextBeginTime)
		{
			return new int[]
			{
				mNextBeginTime / 10000,
				mNextBeginTime % (mNextBeginTime / 10000) / 100,
				mNextBeginTime % (mNextBeginTime / 10000) % 100
			};
		}
		return null;
	}

	private int FindNearestDay(int tmpDay)
	{
		int num = 0;
		for (int i = 0; i < this.jingCaiDayOfWeeks.Length; i++)
		{
			num = this.GetNormalWeekday(Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[i]));
			if (tmpDay <= num)
			{
				break;
			}
		}
		return num;
	}

	private void ZhanMengLianSaiJingCaiCountDown(BangHuiMatchMainInfo info)
	{
		if (info == null)
		{
			return;
		}
		this.LoadJingCaiTimeXML();
		DateTime correctDateTime = Global.GetCorrectDateTime();
		int[] leapMatchTime = this.GetLeapMatchTime(info.seasonid);
		DateTime dateTime;
		dateTime..ctor(leapMatchTime[0], leapMatchTime[1], leapMatchTime[2]);
		if (correctDateTime < dateTime)
		{
			TimeSpan timeSpan = dateTime - correctDateTime;
			DateTime dateTime2 = this.GetJumpStartTime_JingCai(dateTime, 0);
			int subDays = this.GetSubDays(this.FindNearestDay(dateTime.DayOfWeek), dateTime.DayOfWeek);
			dateTime2 = dateTime2.AddDays((double)subDays);
			DateTime dateTime3 = this.GetJumpEndTime_JingCai(dateTime, 0);
			int subDays2 = this.GetSubDays(this.FindNearestDay(dateTime.DayOfWeek), dateTime.DayOfWeek);
			dateTime3 = dateTime2.AddDays((double)subDays2);
			TimeSpan timeSpan2 = dateTime2 - dateTime;
			long ticks = (timeSpan + timeSpan2).Ticks;
			if (ticks > 0L)
			{
				this.color = "ff0000";
				this.countdowntimes = ticks / 10000000L;
			}
			else
			{
				if (ticks <= 0L && Global.GetCorrectDateTime() <= dateTime3)
				{
					this.color = "17e43e";
					this.countdowntimes = 0L;
					this.isCountDownOver = true;
					return;
				}
				if (ticks <= 0L && this.IsInHuoDongdays(dateTime.DayOfWeek))
				{
					if (dateTime.DayOfWeek == Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]))
					{
						int subDays3 = this.GetSubDays(Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[1]), Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]));
						if (subDays3 > 0)
						{
							this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 0);
							this.startTime_JingCai = this.startTime_JingCai.AddDays((double)subDays3);
							this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
							this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 0);
							this.endTime_JingCai = this.endTime_JingCai.AddDays((double)subDays3);
							long remainderTicks = this.GetRemainderTicks(this.startTime_JingCai, Global.GetCorrectDateTime());
							this.color = "17e43e";
							this.countdowntimes = remainderTicks / 10000000L;
						}
						else
						{
							MUDebug.LogError<string>(new string[]
							{
								"竞猜计算天数有误！！！days"
							});
						}
					}
					else if (dateTime.DayOfWeek == Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[1]))
					{
						int num = this.GetNormalWeekday(0) - this.GetNormalWeekday(dateTime.DayOfWeek) + Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]);
						this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 0);
						this.startTime_JingCai = this.startTime_JingCai.AddDays((double)num);
						this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
						this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 0);
						this.endTime_JingCai = this.endTime_JingCai.AddDays((double)num);
						long remainderTicks2 = this.GetRemainderTicks(this.startTime_JingCai, Global.GetCorrectDateTime());
						this.color = "17e43e";
						this.countdowntimes = remainderTicks2 / 10000000L;
					}
				}
			}
		}
		else
		{
			DayOfWeek dayOfWeek = Global.GetCorrectDateTime().DayOfWeek;
			if (dayOfWeek == Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]))
			{
				this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 0);
				this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
				this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 0);
			}
			else if (dayOfWeek == Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[1]))
			{
				this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 1);
				this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
				this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 1);
			}
			else if (!this.IsInHuoDongdays(dayOfWeek))
			{
				int subDays4 = this.GetSubDays(this.FindNearestDay(dayOfWeek), dayOfWeek);
				this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 0);
				this.startTime_JingCai = this.startTime_JingCai.AddDays((double)subDays4);
				this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
				this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 0);
				this.endTime_JingCai = this.endTime_JingCai.AddDays((double)subDays4);
			}
			long remainderTicks3 = this.GetRemainderTicks(this.startTime_JingCai, Global.GetCorrectDateTime());
			if (remainderTicks3 > 0L)
			{
				this.color = "ff0000";
				this.countdowntimes = remainderTicks3 / 10000000L;
			}
			else
			{
				if (remainderTicks3 <= 0L && Global.GetCorrectDateTime() <= this.endTime_JingCai)
				{
					this.color = "17e43e";
					this.countdowntimes = 0L;
					this.isCountDownOver = true;
					return;
				}
				if (remainderTicks3 <= 0L && this.IsInHuoDongdays(dayOfWeek))
				{
					if (dayOfWeek == Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]))
					{
						int subDays5 = this.GetSubDays(Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[1]), Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]));
						if (subDays5 > 0)
						{
							this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 0);
							this.startTime_JingCai = this.startTime_JingCai.AddDays((double)subDays5);
							this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
							this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 0);
							this.endTime_JingCai = this.endTime_JingCai.AddDays((double)subDays5);
							long remainderTicks4 = this.GetRemainderTicks(this.startTime_JingCai, Global.GetCorrectDateTime());
							this.color = "17e43e";
							this.countdowntimes = remainderTicks4 / 10000000L;
						}
						else
						{
							MUDebug.LogError<string>(new string[]
							{
								"竞猜计算天数有误！！！days"
							});
						}
					}
					else if (dayOfWeek == Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[1]))
					{
						MUDebug.LogError<string>(new string[]
						{
							"Global.GetCorrectDateTime()" + Global.GetCorrectDateTime()
						});
						int num2 = this.GetNormalWeekday(0) - this.GetNormalWeekday(dayOfWeek) + Global.SafeConvertToInt32(this.jingCaiDayOfWeeks[0]);
						this.GetStartTime_JingCai(Global.GetCorrectDateTime(), 0);
						this.startTime_JingCai = this.startTime_JingCai.AddDays((double)num2);
						this.startTime_JingCai = this.startTime_JingCai.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
						this.GetEndTime_JingCai(Global.GetCorrectDateTime(), 0);
						this.endTime_JingCai = this.endTime_JingCai.AddDays((double)num2);
						long num3 = this.startTime_JingCai.Ticks - Global.GetCorrectDateTime().Ticks;
						this.color = "17e43e";
						this.countdowntimes = num3 / 10000000L;
					}
				}
			}
		}
		this.StartJingCaiCountDown();
	}

	private void StartJingCaiCountDown()
	{
		this.jingCaiUITimer = new DispatcherTimer("ZhanMengLianSaiJingCai_Timerr");
		this.jingCaiUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.jingCaiUITimer.Tick = new DispatcherTimerEventHandler(this.JingCaiUITimer_Tick);
		this.jingCaiUITimer.Start();
	}

	protected void JingCaiUITimer_Tick(object sender, object e)
	{
		if (this.countdowntimes > 0L)
		{
			this.LblTime.text = string.Format(Global.GetLang("竞猜剩余时间： {0}"), this.GetTimeStrBySecEx((double)this.countdowntimes));
			this.countdowntimes -= 1L;
		}
		else
		{
			this.LblTime.text = Global.GetLang("联赛进行中");
			this.isCountDownOver = true;
			this.HideJingCaiBtn();
			this.StopJingCaiTimer();
		}
	}

	private void StopJingCaiTimer()
	{
		if (this.jingCaiUITimer != null)
		{
			this.jingCaiUITimer.Tick = null;
			this.jingCaiUITimer.Stop();
			this.jingCaiUITimer = null;
		}
	}

	private bool IsInHuoDongdays(int value)
	{
		string text = Array.Find<string>(this.jingCaiDayOfWeeks, (string result) => result == value.ToString());
		return !string.IsNullOrEmpty(text);
	}

	private int GetSubDays(int max, int min)
	{
		int num = (max != 0) ? max : 7;
		int num2 = (min != 0) ? min : 7;
		return num - num2;
	}

	private int GetNormalWeekday(int day)
	{
		return (day != 0) ? day : 7;
	}

	private void LoadJingCaiTimeXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LeagueMatch.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueMatch");
		string[] array = Global.GetXElementAttributeStr(xelementList[0], "TimePoints").Split(new char[]
		{
			'|'
		});
		this.ApplyOverTime_JingCai = Global.GetXElementAttributeInt(xelementList[0], "ApplyOverTime");
		for (int i = 0; i < array.Length; i++)
		{
			this.jingCaiDayOfWeeks[i] = array[i].Split(new char[]
			{
				','
			})[0];
			this.jingCaiTimeDayOfWeek[i] = array[i].Split(new char[]
			{
				','
			})[1];
		}
	}

	private void GetStartTime_JingCai(DateTime dateTime, int index)
	{
		string stringDateTime = this.GetStringDateTime(dateTime, index, 0);
		DateTime.TryParse(stringDateTime, ref this.startTime_JingCai);
	}

	private void GetEndTime_JingCai(DateTime dateTime, int index)
	{
		string stringDateTime = this.GetStringDateTime(dateTime, index, 1);
		DateTime.TryParse(stringDateTime, ref this.endTime_JingCai);
	}

	private DateTime GetJumpStartTime_JingCai(DateTime dateTime, int index)
	{
		string stringDateTime = this.GetStringDateTime(dateTime, index, 0);
		DateTime result;
		DateTime.TryParse(stringDateTime, ref result);
		result = result.AddSeconds((double)(-(double)this.ApplyOverTime_JingCai));
		return result;
	}

	private DateTime GetJumpEndTime_JingCai(DateTime dateTime, int index)
	{
		string stringDateTime = this.GetStringDateTime(dateTime, index, 1);
		DateTime result;
		DateTime.TryParse(stringDateTime, ref result);
		return result;
	}

	private string GetStringDateTime(DateTime dateTime, int index, int arrIndexDayOfWeek)
	{
		return string.Format("{0}-{1}-{2} {3}", new object[]
		{
			dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			this.jingCaiTimeDayOfWeek[index].Split(new char[]
			{
				'-'
			})[arrIndexDayOfWeek]
		});
	}

	private long GetRemainderTicks(DateTime max, DateTime min)
	{
		return max.Ticks - min.Ticks;
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

	private void AddItems(ObservableCollection obc, ListBox list, List<BangHuiMatchPKInfo> listInfos, ZhanMengLianSaiGuanZhan.ZhanKuangType type, bool isLastRound = false)
	{
		if (listInfos == null || listInfos.Count <= 0)
		{
			if (type == ZhanMengLianSaiGuanZhan.ZhanKuangType.Last)
			{
				NGUITools.SetActive(this.mDownPanelObj, false);
				this.ResetDraggablePanelPosition();
			}
			else if (type == ZhanMengLianSaiGuanZhan.ZhanKuangType.Current)
			{
				Vector3 localPosition = this.mUpPanelObj.transform.localPosition;
				NGUITools.SetActive(this.mUpPanelObj, false);
				NGUITools.SetActive(this.mDownPanelObj, true);
				this.mDownPanelObj.transform.localPosition = localPosition;
				this.ResetDraggablePanelPosition();
			}
			return;
		}
		int count = listInfos.Count;
		for (int i = 0; i < count; i++)
		{
			ZhanMengLianSaiGuanZhanItem zhanMengLianSaiGuanZhanItem = U3DUtils.NEW<ZhanMengLianSaiGuanZhanItem>();
			zhanMengLianSaiGuanZhanItem.InitData(listInfos[i], (int)type, this.curGameState, isLastRound);
			zhanMengLianSaiGuanZhanItem.OpenJingCaiPart = delegate(BangHuiMatchPKInfo result)
			{
				if (result != null)
				{
					this.mJingCaiWindow = this.OpenWindow();
					if (this.mJingCaiWindow)
					{
						this.mJingCaiWindow.InitJingCaiData(result);
					}
				}
			};
			UIPanel component = zhanMengLianSaiGuanZhanItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(list.gameObject, zhanMengLianSaiGuanZhanItem.gameObject, true);
			obc.AddNoUpdate(zhanMengLianSaiGuanZhanItem);
		}
	}

	private ZhanMengLianSaiGuanZhanRank OpenWindow()
	{
		if (null != this.mGuanZhanRankWindow)
		{
			this.CloseWindow();
		}
		if (null == this.mGuanZhanRankWindow)
		{
			this.mGuanZhanRankWindow = U3DUtils.NEW<GChildWindow>();
			this.mGuanZhanRankWindow.ModalType = ChildWindowModalType.Translucent;
			this.mGuanZhanRankWindow.IsShowModal = true;
			Super.InitChildWindow(this.mGuanZhanRankWindow, "ZhanMengLianSaiGuanZhanRankWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.mGuanZhanRankWindow);
		}
		this.mGuanZhanRankPart = U3DUtils.NEW<ZhanMengLianSaiGuanZhanRank>();
		this.mGuanZhanRankWindow.Body.Add(this.mGuanZhanRankPart);
		this.mGuanZhanRankPart.CloseHandler = delegate(object s1, DPSelectedItemEventArgs args)
		{
			if (args.ID == 0)
			{
				this.CloseWindow();
			}
		};
		return this.mGuanZhanRankPart;
	}

	private void CloseWindow()
	{
		if (null != this.mGuanZhanRankPart)
		{
			Object.Destroy(this.mGuanZhanRankPart.gameObject);
			this.mGuanZhanRankPart = null;
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.mGuanZhanRankWindow);
		}
		if (this.mJingCaiWindow)
		{
			this.mJingCaiWindow = null;
		}
		if (this.mJingCaiAwardWindow)
		{
			this.mJingCaiAwardWindow = null;
		}
		this.RefreshJingCaiJiFen();
	}

	private void ResetDraggablePanelPosition()
	{
		if (this.draggablePanel != null)
		{
			this.draggablePanel.repositionClipping = true;
			this.draggablePanel.ResetPosition();
		}
	}

	private void HideJingCaiBtn()
	{
		for (int i = 0; i < this.CurItemCollection.Count; i++)
		{
			ZhanMengLianSaiGuanZhanItem zhanMengLianSaiGuanZhanItem = U3DUtils.AS<ZhanMengLianSaiGuanZhanItem>(this.CurItemCollection.GetAt(i));
			zhanMengLianSaiGuanZhanItem.HideJingCaiBtn();
		}
	}

	private string GetChineseNumber(int num)
	{
		string result = string.Empty;
		switch (num)
		{
		case 1:
			result = Global.GetLang("一");
			break;
		case 2:
			result = Global.GetLang("二");
			break;
		case 3:
			result = Global.GetLang("三");
			break;
		case 4:
			result = Global.GetLang("四");
			break;
		case 5:
			result = Global.GetLang("五");
			break;
		case 6:
			result = Global.GetLang("六");
			break;
		case 7:
			result = Global.GetLang("七");
			break;
		case 8:
			result = Global.GetLang("八");
			break;
		}
		return result;
	}

	public override void Destroy()
	{
		this.StopJingCaiTimer();
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mBtnRank = null;
		this.mBtnAward = null;
		this.mBtnJingCaiShop = null;
		this.mShopObj = null;
		this.mLblJingCaiHuoBi = null;
		this.mLblCurZhanKuang = null;
		this.LblTime = null;
		this.LblLastZhanKuang = null;
		this.mLastListBox = null;
		this._LastItemCollection = null;
		this.mCurListBox = null;
		this._CurItemCollection = null;
		this.mUpPanelObj = null;
		this.mDownPanelObj = null;
		this.draggablePanel = null;
	}

	private const string path = "Config/LeagueSustain.xml";

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public GButton mBtnRank;

	public GButton mBtnAward;

	public GButton mBtnJingCaiShop;

	public GameObject mShopObj;

	public TextBlock mLblJingCaiHuoBi;

	public TextBlock mLblCurZhanKuang;

	public TextBlock LblTime;

	public TextBlock LblLastZhanKuang;

	public ListBox mLastListBox;

	private ObservableCollection _LastItemCollection;

	public ListBox mCurListBox;

	private ObservableCollection _CurItemCollection;

	public GameObject mUpPanelObj;

	public GameObject mDownPanelObj;

	public UIDraggablePanel draggablePanel;

	private BangHuiMatchGameStates curGameState = 5;

	private bool isCountDownOver;

	private ZhanMengLianSaiGuanZhanRank mJingCaiAwardWindow;

	private string levelLimit = string.Empty;

	private GChildWindow mDuiHuanWindow;

	private MUDuiHuanPart mDuiHuanPart;

	private string[] jingCaiDayOfWeeks = new string[2];

	private string[] jingCaiTimeDayOfWeek = new string[2];

	private DateTime startTime_JingCai = default(DateTime);

	private DateTime endTime_JingCai = default(DateTime);

	private string color = string.Empty;

	private long countdowntimes;

	private DispatcherTimer jingCaiUITimer;

	private int ApplyOverTime_JingCai;

	private ZhanMengLianSaiGuanZhanRank mJingCaiWindow;

	private GChildWindow mGuanZhanRankWindow;

	private ZhanMengLianSaiGuanZhanRank mGuanZhanRankPart;

	private enum ZhanKuangType
	{
		None,
		Last,
		Current
	}
}
