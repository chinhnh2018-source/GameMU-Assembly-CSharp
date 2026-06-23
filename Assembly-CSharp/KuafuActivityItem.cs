using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class KuafuActivityItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.labType.transform.localScale = new Vector3(18f, 18f, 1f);
		this.labTime.transform.localScale = new Vector3(18f, 18f, 1f);
		this.labTime.text = string.Empty;
	}

	public GKuafuActivityData.ItemData Data
	{
		get
		{
			return this.data;
		}
		set
		{
			this.data = value;
			this.configItem = this.data.configItem;
		}
	}

	public int ID
	{
		get
		{
			return this.data.configTabConfigLine.ID;
		}
	}

	public string StrName
	{
		get
		{
			return this.data.configTabConfigLine.Name;
		}
	}

	public void SetBakUrl(string name)
	{
		this.netBak.URL = this.GetImageUrlString(name);
	}

	private string GetImageUrlString(string name)
	{
		return string.Format("NetImages/GameRes/Images/Preview/{0}.jpg", name);
	}

	public string StrXmlname
	{
		get
		{
			return this.data.configTabConfigLine.GLXml;
		}
	}

	public bool IsHaveApplyTime
	{
		get
		{
			return this.data.configItem != null && this.data.configItem.IsHaveApplyTime;
		}
	}

	public string StrTime
	{
		get
		{
			return this.strTime;
		}
		set
		{
			this.strTime = value;
			this.labTime.text = Global.GetLang("剩余 ") + this.StrTime;
		}
	}

	public bool IsWaitingState
	{
		get
		{
			return this.data.isWaitingState;
		}
		set
		{
			this.data.isWaitingState = value;
			if (this.data.isWaitingState)
			{
				this.labType.text = Global.GetLang("活动开始");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fd010c",
					this.StrTime
				});
			}
			else
			{
				this.labType.text = Global.GetLang("活动结束");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.StrTime
				});
			}
		}
	}

	public DateTime NextDate
	{
		get
		{
			return this.data.nextDate;
		}
		set
		{
			this.data.nextDate = value;
		}
	}

	public void InitConfig(XElement args)
	{
		if (args == null)
		{
			return;
		}
		this.Data = GKuafuActivityData.InitConfig(args);
		this.SetBakUrl(this.data.configTabConfigLine.Preview);
		int xelementAttributeInt = Global.GetXElementAttributeInt(args, "ID");
		if (xelementAttributeInt == 20005)
		{
			this.PKLoversDownTime();
			this.isPKLovers = true;
		}
		else if (xelementAttributeInt == 20007)
		{
			GameInstance.Game.GetArmyCaiJiDaoJiShiTime();
			Super.ShowNetWaiting(null);
		}
		else if (xelementAttributeInt == 20008)
		{
			GameInstance.Game.GetZhanMengLianSaiMainInfo();
		}
		else if (xelementAttributeInt == 20009)
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
			this.StartUITimer(new DispatcherTimerEventHandler(this.UITimer_TickForKuaFuPlunder));
			this.UITimer_TickForKuaFuPlunder(null, null);
		}
		else if (xelementAttributeInt != 20004)
		{
			this.StartTickInvoke();
		}
		else
		{
			GameInstance.Game.GetDaoJiShiTime();
			Super.ShowNetWaiting(null);
		}
	}

	public void RefreshNextDate()
	{
		this.data.RefreshNextDate();
		this.IsWaitingState = this.data.isWaitingState;
	}

	protected void StartTickInvoke()
	{
		this.RefreshNextDate();
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.NextDate.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (this.IsHaveApplyTime)
			{
				if (this.Data.localState == KuafuActivityLocalState.Wait)
				{
					this.labType.text = Global.GetLang("距活动开始时间");
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
					});
				}
				else if (this.Data.localState == KuafuActivityLocalState.SignUp)
				{
					this.labType.text = Global.GetLang("距报名结束时间");
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
					});
				}
				else if (this.Data.localState == KuafuActivityLocalState.Start)
				{
					this.labType.text = Global.GetLang("距活动结束时间");
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
					});
				}
				else
				{
					string text;
					if (num2 >= 86400)
					{
						text = Global.GetTimeStrBySecEx((double)num2, true, -1);
						int num3 = text.IndexOf(Global.GetLang("分"));
						text = text.Substring(0, num3 + 1);
					}
					else
					{
						text = Global.GetTimeStrBySecEx((double)num2, true, -1);
					}
					this.labType.text = Global.GetLang("距报名开始时间");
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fd010c",
						string.Format(Global.GetLang("剩余 {0}"), text)
					});
				}
			}
			else if (this.IsWaitingState)
			{
				this.labType.text = Global.GetLang("活动开始");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fd010c",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else
			{
				this.labType.text = Global.GetLang("活动结束");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
		}
		else
		{
			base.CancelInvoke("TickProc");
			this.StartTickInvoke();
		}
	}

	private void GetTimeAttr(ZhengBaMiniStateData time, out long countdowntimes, out string color, out bool isopen)
	{
		color = "17e43e";
		countdowntimes = 0L;
		isopen = false;
		if (time.PkStartWaitSec > 0L)
		{
			color = "ff0000";
			countdowntimes = time.PkStartWaitSec;
			isopen = true;
		}
		else if (time.NextLoopWaitSec > 0L)
		{
			countdowntimes = time.NextLoopWaitSec;
		}
		else if (time.LoopEndWaitSec > 0L)
		{
			countdowntimes = time.LoopEndWaitSec;
		}
	}

	public void InitCaiJiTime(DateTime time)
	{
		long num = time.Ticks - Global.GetCorrectDateTime().Ticks;
		this.seconds = (int)(num / 10000000L);
		base.InvokeRepeating("TickProcs", 0f, 1f);
	}

	protected void TickProcs()
	{
		this.labTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)this.seconds, true, -1))
		});
		this.seconds--;
		if (this.seconds < 0)
		{
			base.CancelInvoke("TickProcs");
			UITexture component = this.netBak.GetComponent<UITexture>();
			component.shader = Shader.Find("Unlit/Gray");
			this.labTime.text = string.Empty;
			this.labType.text = Global.GetLang("活动未开启");
		}
	}

	public void InitZhengBaTime(ZhengBaMiniStateData time)
	{
		if (time == null)
		{
			return;
		}
		if (!time.IsZhengBaOpened)
		{
			UITexture component = this.netBak.GetComponent<UITexture>();
			component.shader = Shader.Find("Unlit/Gray");
			this.labType.text = string.Empty;
			this.labTime.text = string.Empty;
			return;
		}
		this.GetTimeAttr(time, out this.countdowntimes, out this.color, out this.isopen);
		if (time.PkStartWaitSec > 0L)
		{
			this.labType.text = Global.GetLang("距活动开始时间");
		}
		else if (time.NextLoopWaitSec > 0L)
		{
			this.labType.text = Global.GetLang("距离下轮开始时间");
		}
		else if (time.LoopEndWaitSec > 0L)
		{
			this.labType.text = Global.GetLang("距离本轮结束时间");
		}
		this.StartUITimer(null);
	}

	protected void StartUITimer(DispatcherTimerEventHandler Hander = null)
	{
		this.UITimer = new DispatcherTimer("zhongshenzhengbaPart_Timer");
		this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		if (Hander != null)
		{
			this.UITimer.Tick = Hander;
		}
		else
		{
			this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		}
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_TickForKuaFuPlunder(object sender, object e)
	{
		DateTime minValue = DateTime.MinValue;
		bool flag = false;
		int nextStateTimeData = this.Data.configItem.mCrusadeWarXml.GetNextStateTimeData(out minValue, out flag, this.mKuaFuLueDuoGameStates);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (!flag)
		{
			if (this.mKuaFuLueDuoGameStates == 3)
			{
				int sec = (int)(minValue - correctDateTime).TotalSeconds;
				this.labType.text = Global.GetLang("距活动结束");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec, true, 2)
				});
			}
			else if (this.mKuaFuLueDuoGameStates == null)
			{
				int sec2 = (int)(minValue - correctDateTime).TotalSeconds;
				this.labType.text = Global.GetLang("距竞价开始：");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec2, true, 2)
				});
			}
			else if (this.mKuaFuLueDuoGameStates == 1)
			{
				if (nextStateTimeData == -1)
				{
					this.labType.text = string.Empty;
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("竞价已开始")
					});
				}
				else if (0 < nextStateTimeData && 5 > nextStateTimeData)
				{
					string text = string.Empty;
					if (nextStateTimeData == 1)
					{
						text = Global.GetLang("距第一轮竞价结束：");
					}
					else if (nextStateTimeData == 2)
					{
						text = Global.GetLang("距第二轮竞价结束：");
					}
					else if (nextStateTimeData == 3)
					{
						text = Global.GetLang("距第三轮竞价结束：");
					}
					else
					{
						text = Global.GetLang("距第四轮竞价结束：");
					}
					this.labType.text = text;
					int sec3 = (int)(minValue - correctDateTime).TotalSeconds;
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec3, true, 2)
					});
				}
				else
				{
					this.labTime.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("竞价已开始")
					});
				}
			}
			else if (this.mKuaFuLueDuoGameStates == 2)
			{
				int sec4 = (int)(minValue - correctDateTime).TotalSeconds;
				this.labType.text = Global.GetLang("距掠夺开始：");
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetTimeStrBySecFilterZero(sec4, true, 2)
				});
			}
			else if (this.mKuaFuLueDuoGameStates == 4)
			{
				this.labType.text = Global.GetLang("结算中");
				this.labTime.text = string.Empty;
				if (this.mSynchronousData == null)
				{
					this.mSynchronousData = new KuafuActivityItem.SynchronousData();
					this.mSynchronousData.SenderGetData();
				}
			}
		}
		else
		{
			this.labType.text = string.Empty;
			if (this.mSynchronousData == null)
			{
				this.mSynchronousData = new KuafuActivityItem.SynchronousData();
				this.mSynchronousData.SenderGetData();
			}
		}
		if (this.mSynchronousData != null && this.mSynchronousData.UpDate())
		{
			this.mSynchronousData = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (this.countdowntimes > 0L)
		{
			if (this.isopen)
			{
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					string.Format(Global.GetLang("剩余 {0}"), KuafuActivityItem.GetTimeStrBySecEx((double)this.countdowntimes))
				});
			}
			else
			{
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					this.color,
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)this.countdowntimes, true, -1))
				});
			}
			this.countdowntimes -= 1L;
		}
		else
		{
			this.labTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				string.Format(Global.GetLang("剩余0秒"), new object[0])
			});
			this.StopTimer();
			if (this.isPKLovers)
			{
				this.PKLoversDownTime();
			}
			else
			{
				GameInstance.Game.GetDaoJiShiTime();
			}
		}
	}

	public override void Destroy()
	{
		this.StopJingCaiTimer();
		this.StopTimer();
		if (this.Data != null && this.Data.configItem.mCrusadeWarXml != null)
		{
			IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
		}
	}

	private static string GetTimeStrBySecEx(double sec)
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

	public void InitZhanMengLianSaiJingCaiData(BangHuiMatchMainInfo info)
	{
		if (info.timestate == 3)
		{
			this.labType.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("联赛进行中")
			});
			return;
		}
		if (info.timestate == 0)
		{
			this.labType.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("结算中")
			});
			return;
		}
		if (this.GetJingCaiLabelState(info))
		{
			this.labType.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("超级黄金联赛暂未开启")
			});
			return;
		}
		this.ZhanMengLianSaiJingCaiCountDown(info);
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
			if (this.GetNormalWeekday(tmpDay) < num)
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
				this.labType.text = Global.GetLang("距离停止竞猜时间");
				this.countdowntimes = ticks / 10000000L;
			}
			else
			{
				if (ticks <= 0L && Global.GetCorrectDateTime() <= dateTime3)
				{
					this.color = "17e43e";
					this.labType.text = Global.GetLang("竞猜时间已结束");
					this.countdowntimes = 0L;
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
							this.color = "ff0000";
							this.labType.text = Global.GetLang("距离停止竞猜时间");
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
						this.color = "ff0000";
						this.labType.text = Global.GetLang("距离停止竞猜时间");
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
				this.labType.text = Global.GetLang("距离停止竞猜时间");
				this.countdowntimes = remainderTicks3 / 10000000L;
			}
			else
			{
				if (remainderTicks3 <= 0L && Global.GetCorrectDateTime() <= this.endTime_JingCai)
				{
					this.color = "17e43e";
					this.labType.text = Global.GetLang("竞猜时间已结束");
					this.countdowntimes = 0L;
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
							this.color = "ff0000";
							this.labType.text = Global.GetLang("距离停止竞猜时间");
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
						this.color = "ff0000";
						this.labType.text = Global.GetLang("距离停止竞猜时间");
						this.countdowntimes = num3 / 10000000L;
					}
				}
			}
		}
		this.StartJingCaiCountDown();
	}

	private void StartJingCaiCountDown()
	{
		this.jingCaiUITimer = new DispatcherTimer("ZhanMengLianSaiJingCai_Timer");
		this.jingCaiUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.jingCaiUITimer.Tick = new DispatcherTimerEventHandler(this.JingCaiUITimer_Tick);
		this.jingCaiUITimer.Start();
	}

	protected void JingCaiUITimer_Tick(object sender, object e)
	{
		if (this.countdowntimes > 0L)
		{
			this.labTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				string.Format(Global.GetLang("剩余 {0}"), KuafuActivityItem.GetTimeStrBySecEx((double)this.countdowntimes))
			});
			this.countdowntimes -= 1L;
		}
		else
		{
			this.color = "17e43e";
			this.labType.text = Global.GetLang("竞猜时间已结束");
			this.labTime.text = string.Empty;
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

	private void PKLoversDownTime()
	{
		this.LoadCoupleWarXmlToTime();
		this.GetStartTime(Global.GetCorrectDateTime());
		this.GetEndTime(Global.GetCorrectDateTime());
		if (Global.GetCorrectDateTime().DayOfWeek == 5)
		{
			if (Global.GetCorrectDateTime().Ticks < this.startTime.Ticks)
			{
				this.color = "ff0000";
				this.labType.text = Global.GetLang("距活动开始时间");
				this.countdowntimes = (this.startTime.Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
			}
			else if (Global.GetCorrectDateTime().Ticks > this.startTime.Ticks && Global.GetCorrectDateTime().Ticks < this.endTime.Ticks)
			{
				this.color = "17e43e";
				this.labType.text = Global.GetLang("距活动结束时间");
				this.countdowntimes = (this.endTime.Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
			}
			else
			{
				this.color = "ff0000";
				this.labType.text = Global.GetLang("距活动开始时间");
				this.countdowntimes = (this.startTime.AddDays(1.0).Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
			}
		}
		else if (Global.GetCorrectDateTime().DayOfWeek == 6)
		{
			if (Global.GetCorrectDateTime().Ticks < this.startTime.Ticks)
			{
				this.color = "ff0000";
				this.labType.text = Global.GetLang("距活动开始时间");
				this.countdowntimes = (this.startTime.Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
			}
			else if (Global.GetCorrectDateTime().Ticks > this.startTime.Ticks && Global.GetCorrectDateTime().Ticks < this.endTime.Ticks)
			{
				this.color = "17e43e";
				this.labType.text = Global.GetLang("距活动结束时间");
				this.countdowntimes = (this.endTime.Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
			}
			else
			{
				this.color = "ff0000";
				this.labType.text = Global.GetLang("距活动开始时间");
				this.countdowntimes = (this.startTime.AddDays(6.0).Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
			}
		}
		else if (Global.GetCorrectDateTime().DayOfWeek < 5)
		{
			this.color = "ff0000";
			this.labType.text = Global.GetLang("距活动开始时间");
			int num = 5 - Global.GetCorrectDateTime().DayOfWeek;
			DateTime dateTime = this.startTime.AddDays((double)num);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			this.countdowntimes = (dateTime.Ticks - correctDateTime.Ticks) / 10000000L;
		}
		this.isopen = true;
		this.StartUITimer(null);
	}

	private void LoadCoupleWarXmlToTime()
	{
		XElement gameResXml = Global.GetGameResXml("Config/CoupleWar.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		string[] array = Global.GetXElementAttributeStr(xelementList[0], "TimePoints").Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			this.weeks[i] = array[i].Split(new char[]
			{
				','
			})[0];
			this.times[i] = array[i].Split(new char[]
			{
				','
			})[1];
		}
	}

	private void GetStartTime(DateTime dateTime)
	{
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			this.times[0].Split(new char[]
			{
				'-'
			})[0]
		});
		DateTime.TryParse(text, ref this.startTime);
	}

	private void GetEndTime(DateTime dateTime)
	{
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			this.times[0].Split(new char[]
			{
				'-'
			})[1]
		});
		DateTime.TryParse(text, ref this.endTime);
	}

	public void NocticeKuaFuPlunderStateCallBack(KuaFuLueDuoGameStates State)
	{
		this.mKuaFuLueDuoGameStates = State;
	}

	private GKuafuActivityData.ItemData data;

	private KuafuActivityItemConfig configItem;

	public ShowNetImage netBak;

	public TextBlock labType;

	public TextBlock labTime;

	private string color = "17e43e";

	private long countdowntimes;

	private bool isopen;

	private bool isPKLovers;

	private string strTime = string.Empty;

	private int seconds;

	private DispatcherTimer UITimer;

	private KuafuActivityItem.SynchronousData mSynchronousData;

	private KuaFuLueDuoGameStates mKuaFuLueDuoGameStates;

	private string[] jingCaiDayOfWeeks = new string[2];

	private string[] jingCaiTimeDayOfWeek = new string[2];

	private DateTime startTime_JingCai = default(DateTime);

	private DateTime endTime_JingCai = default(DateTime);

	private DispatcherTimer jingCaiUITimer;

	private int ApplyOverTime_JingCai;

	private string[] weeks = new string[2];

	private string[] times = new string[2];

	private DateTime startTime = default(DateTime);

	private DateTime endTime = default(DateTime);

	private class SynchronousData
	{
		public SynchronousData()
		{
			this.Synchronous = true;
		}

		public bool Synchronous
		{
			set
			{
				if (!this.mSynchronous)
				{
					this.mSynchronous = value;
					this.mSynchronousTime = 3;
				}
				if (!value)
				{
					this.mSynchronous = value;
				}
			}
		}

		public bool UpDate()
		{
			if (!this.mSynchronous)
			{
				return false;
			}
			this.mSynchronousTime--;
			if (0 > this.mSynchronousTime)
			{
				this.Synchronous = false;
				return true;
			}
			return false;
		}

		public void SenderGetData()
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
		}

		private int mSynchronousTime;

		private bool mSynchronous;
	}
}
