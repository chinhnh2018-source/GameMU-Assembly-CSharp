using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MoYuDuoBaoMainPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblSelfName.text = Global.GetLang(string.Empty);
		this.lblTimeWord.text = Global.GetLang("活动时间 :");
		this.lblEndTime.text = Global.GetLang(string.Empty);
		this.lblEndWord.text = Global.GetLang("距活动开启 :");
		this.btnBaoMing.Label.text = Global.GetLang("立即报名");
		this.lblRoundWord.text = Global.GetLang("活动天数 :");
		this.lblRoundNum.text = Global.GetLang(string.Empty);
		this.lblTitle1.text = Global.GetLang("最强战队");
		this.lblTitle2.text = Global.GetLang("魔域王者");
		this.lblTitleContent1.text = Global.GetLang("在魔域夺宝活动中获得赛季排名第一的战队，全队成员可获得最强战队的称号。");
		this.lblTitleContent2.text = Global.GetLang("在魔域夺宝活动中获得赛季个人击杀排名第一的战队成员，可获得魔域王者称号。");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChildWindowClose != null)
			{
				this.ChildWindowClose(this, null);
			}
		};
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			CommonHelpWindow commonHelpWindow = base.OpenWindow<CommonHelpWindow>(true);
			commonHelpWindow.SetHelpInfo(IConfigbase<ConfigMoYuDuoBao>.Instance.GetMoYuHelpHelpInfo().list);
		};
		this.btnChengJiu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.OpenWindow<MoYuDuoBaoPartRongYu>(true);
		};
		this.btnPaiMing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.OpenWindow<MoYuDuoBaoPartRank>(true);
		};
		this.btnJiangLi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.OpenWindow<MoYuDuoBaoPartJiangLi>(true);
		};
		this.btnChengHao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.goChengHao.SetActive(true);
		};
		this.btnChengHaoClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.goChengHao.SetActive(false);
		};
		this.btnBaoMing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.n_duoBaoState == DuoBaoTimeState.BaoMing)
			{
				if (MoYuDuoBaoData.DuoBaoState == 2)
				{
					Super.HintMainText(Global.GetLang("已报名"), 10, 3);
					return;
				}
				this.RequestBaoMing();
				return;
			}
			else if (this.n_duoBaoState == DuoBaoTimeState.InBattle)
			{
				if (MoYuDuoBaoData.DuoBaoState == 5)
				{
					Super.HintMainText(Global.GetLang("未参与此次比赛"), 10, 3);
					return;
				}
				if (MoYuDuoBaoData.DuoBaoState == 7)
				{
					Super.HintMainText(Global.GetLang("未匹配成功"), 10, 3);
					return;
				}
				this.RequestEnter();
				return;
			}
			else
			{
				if (this.n_duoBaoState == DuoBaoTimeState.End)
				{
					Super.HintMainText(Global.GetLang("活动未开启"), 10, 3);
					return;
				}
				return;
			}
		};
		this.InitActivityTime();
		ZorkMonsterVO bossInfo = IConfigbase<ConfigMoYuDuoBao>.Instance.GetBossInfo();
		List<string> rewardStr = new List<string>(bossInfo.BossLostSkill);
		Global.LoadReward(rewardStr, this.gridReward, 70f, false);
		this.RefreshDuoBaoState();
		this.RequestTeamInfo();
		this.RequestGetDuoBaoState();
	}

	private void RefershInfo()
	{
		if (!string.IsNullOrEmpty(MoYuDuoBaoData.BestTeam))
		{
			this.goBestTeam.SetActive(true);
			this.lblSelfName.text = Global.GetLang("最强战队:") + MoYuDuoBaoData.BestTeam;
		}
		else
		{
			this.goBestTeam.SetActive(false);
			this.lblSelfName.text = string.Empty;
		}
		int round = MoYuDuoBaoData.Round;
		ZorkActivityRulesVO zorkActivityRulesVODataById = IConfigbase<ConfigMoYuDuoBao>.Instance.GetZorkActivityRulesVODataById(1);
		if (round <= zorkActivityRulesVODataById.SeasonFightDay)
		{
			this.lblRoundNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				round + "/" + zorkActivityRulesVODataById.SeasonFightDay
			});
		}
		else
		{
			this.lblRoundNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				zorkActivityRulesVODataById.SeasonFightDay + "/" + zorkActivityRulesVODataById.SeasonFightDay
			});
		}
		this.RefreshDuoBaoState();
	}

	public DuoBaoTimeState GetDuoBaoState()
	{
		DuoBaoTimeState result = DuoBaoTimeState.NotStart;
		DateTime dateTime = Global.GetCorrectDateTime().AddSeconds((double)MoYuDuoBaoData.TimeOff);
		if (this.isInActivity(dateTime, this.m_activityTimePoints, out this.m_startTime, out this.m_endTime))
		{
			if (this.m_startTime < dateTime && this.m_endTime > dateTime)
			{
				int num = (int)(dateTime - this.m_startTime).TotalSeconds;
				int num2 = num % this.m_eachBattleTime;
				if (num2 < this.m_battleSignSecs)
				{
					result = DuoBaoTimeState.BaoMing;
				}
				else if ((float)num2 < (float)this.m_battleSignSecs + 2f)
				{
					result = DuoBaoTimeState.PreBattle;
				}
				else
				{
					result = DuoBaoTimeState.InBattle;
				}
			}
		}
		else
		{
			result = DuoBaoTimeState.NotStart;
		}
		return result;
	}

	private void RepositionEndTime()
	{
		int num = (int)(this.lblEndWord.relativeSize.x * this.lblEndWord.transform.localScale.x);
		Vector3 localPosition = this.lblEndTime.transform.localPosition;
		localPosition.x = this.lblEndWord.transform.localPosition.x + (float)num + 5f;
		this.lblEndTime.transform.localPosition = localPosition;
	}

	private void RefreshDuoBaoState()
	{
		this.n_duoBaoState = this.GetDuoBaoState();
		DateTime dateTime = Global.GetCorrectDateTime().AddSeconds((double)MoYuDuoBaoData.TimeOff);
		if (this.n_duoBaoState == DuoBaoTimeState.NotStart)
		{
			this.m_remainderSeconds = MoYuDuoBaoMainPart.GetRemainderSecond(this.m_startTime, dateTime);
			this.lblEndWord.text = Global.GetLang("距活动开启 :");
			this.btnBaoMing.Text = Global.GetLang("报名");
			this.btnBaoMing.isEnabled = false;
			this.m_color = "ff0000";
		}
		else if (this.n_duoBaoState == DuoBaoTimeState.BaoMing)
		{
			this.m_color = "17e43e";
			int num = (int)(dateTime - this.m_startTime).TotalSeconds;
			int num2 = num % this.m_eachBattleTime;
			this.m_remainderSeconds = (float)(this.m_battleSignSecs - num2);
			if (MoYuDuoBaoData.DuoBaoState == 2)
			{
				this.lblEndWord.text = Global.GetLang("距比赛开始 :");
				this.btnBaoMing.Text = Global.GetLang("已报名");
				this.btnBaoMing.isEnabled = false;
			}
			else if (MoYuDuoBaoData.DuoBaoState == 1)
			{
				this.lblEndWord.text = Global.GetLang("距报名截止 :");
				this.btnBaoMing.Text = Global.GetLang("报名");
				this.btnBaoMing.isEnabled = true;
			}
			else
			{
				this.lblEndWord.text = Global.GetLang("距报名截止 :");
				this.btnBaoMing.Text = Global.GetLang("报名");
				this.btnBaoMing.isEnabled = false;
			}
		}
		else if (this.n_duoBaoState == DuoBaoTimeState.PreBattle)
		{
			int num3 = (int)(dateTime - this.m_startTime).TotalSeconds;
			int num4 = num3 % this.m_eachBattleTime;
			this.m_remainderSeconds = (float)(this.m_battleSignSecs + 2 - num4);
			this.lblEndWord.text = string.Empty;
			this.btnBaoMing.Text = Global.GetLang("进入战斗");
			this.btnBaoMing.isEnabled = false;
		}
		else
		{
			this.m_color = "17e43e";
			int num5 = (int)(dateTime - this.m_startTime).TotalSeconds;
			int num6 = num5 % this.m_eachBattleTime;
			int num7 = (num5 - 2) / this.m_eachBattleTime;
			this.m_remainderSeconds = (float)(this.m_eachBattleTime - num6);
			if (MoYuDuoBaoData.DuoBaoState == 7)
			{
				this.lblEndWord.text = Global.GetLang("距本轮活动结束 : ");
				this.btnBaoMing.Text = Global.GetLang("轮空");
				this.btnBaoMing.isEnabled = false;
			}
			else if (MoYuDuoBaoData.DuoBaoState == 6)
			{
				this.lblEndWord.text = Global.GetLang("距本轮活动结束 : ");
				this.btnBaoMing.Text = Global.GetLang("报名");
				this.btnBaoMing.isEnabled = false;
			}
			else if (MoYuDuoBaoData.DuoBaoState == 5)
			{
				this.lblEndWord.text = Global.GetLang("距本轮活动结束 : ");
				this.btnBaoMing.Text = Global.GetLang("未报名");
				this.btnBaoMing.isEnabled = false;
			}
			else if (MoYuDuoBaoData.DuoBaoState == 3)
			{
				this.lblEndWord.text = Global.GetLang("距本轮活动结束 : ");
				this.btnBaoMing.Text = Global.GetLang("进入战斗");
				this.btnBaoMing.isEnabled = true;
			}
			else
			{
				this.lblEndWord.text = Global.GetLang("距本轮活动结束 : ");
				this.btnBaoMing.Text = Global.GetLang("进入战斗");
				this.btnBaoMing.isEnabled = false;
			}
		}
		this.m_nextUpdateState = dateTime.AddSeconds((double)this.m_remainderSeconds);
		this.RepositionEndTime();
		this.StartDuoBaoCountDown();
	}

	public static float GetRemainderSecond(DateTime max, DateTime min)
	{
		return (float)(max - min).TotalSeconds;
	}

	private void InitActivityTime()
	{
		ZorkActivityRulesVO zorkActivityRulesVODataById = IConfigbase<ConfigMoYuDuoBao>.Instance.GetZorkActivityRulesVODataById(1);
		string timePoints = zorkActivityRulesVODataById.TimePoints;
		this.m_activityTimePoints = timePoints;
		this.m_battleSignSecs = zorkActivityRulesVODataById.BattleSignSecs;
		this.m_eachBattleTime = zorkActivityRulesVODataById.BattleSignSecs + zorkActivityRulesVODataById.PrepareSecs + zorkActivityRulesVODataById.FightingSecs + zorkActivityRulesVODataById.ClearRolesSecs;
		string[] array = timePoints.Split(new char[]
		{
			'|'
		});
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			int dayOfWeek = array[i].Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			text += this.GetWeekName(dayOfWeek);
			if (i != array.Length - 1)
			{
				text += Global.GetLang("、");
			}
			else
			{
				text = text + "  " + array[i].Split(new char[]
				{
					','
				})[1];
			}
		}
		this.lblTime.text = text;
	}

	public override void Update()
	{
		base.Update();
		DateTime dateTime = Global.GetCorrectDateTime().AddSeconds((double)MoYuDuoBaoData.TimeOff);
		if (dateTime >= this.m_nextUpdateState)
		{
			this.RefreshDuoBaoState();
		}
		else if (this.n_duoBaoState == DuoBaoTimeState.PreBattle)
		{
			this.lblEndTime.text = string.Empty;
		}
		else
		{
			double totalSeconds = (this.m_nextUpdateState - dateTime).TotalSeconds;
			this.m_showTime = Global.GetColorStringForNGUIText(new object[]
			{
				this.m_color,
				string.Format(Global.GetLang("剩余 {0}"), this.GetTimeStrBySecEx(totalSeconds))
			});
			this.lblEndTime.text = this.m_showTime;
		}
		this.m_timeNum++;
		if (this.m_timeNum >= 100)
		{
			this.m_timeNum = 0;
			this.RequestGetDuoBaoState();
		}
	}

	private void StartDuoBaoCountDown()
	{
	}

	protected void TeamCompeteUITimer_Tick(object sender, object e)
	{
		if (this.m_remainderSeconds > 0f)
		{
			this.m_showTime = Global.GetColorStringForNGUIText(new object[]
			{
				this.m_color,
				string.Format(Global.GetLang("剩余 {0}"), this.GetTimeStrBySecEx((double)this.m_remainderSeconds))
			});
			this.m_remainderSeconds -= 1f;
			this.lblEndTime.text = this.m_showTime;
		}
		else
		{
			this.m_showTime = Global.GetLang(string.Empty);
			this.m_showTime = string.Empty;
			this.StopTeamCompeteTimer();
			this.RefreshDuoBaoState();
		}
		this.m_timeNum++;
		if (this.m_timeNum >= 3)
		{
			this.m_timeNum = 0;
			this.RequestGetDuoBaoState();
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

	private bool isInActivity(DateTime nowTime, string timePoints, out DateTime startDateTime, out DateTime endDateTime)
	{
		startDateTime = default(DateTime);
		endDateTime = default(DateTime);
		string[] array = timePoints.Split(new char[]
		{
			'|'
		});
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i].Split(new char[]
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
			int normalWeekday = this.GetNormalWeekday(nowTime.DayOfWeek);
			if (normalWeekday == num)
			{
				string text = string.Format("{0}-{1}-{2} {3}", new object[]
				{
					nowTime.Year,
					nowTime.Month,
					nowTime.Day,
					array2[0]
				});
				DateTime.TryParse(text, ref startDateTime);
				string text2 = string.Format("{0}-{1}-{2} {3}", new object[]
				{
					nowTime.Year,
					nowTime.Month,
					nowTime.Day,
					array2[1]
				});
				DateTime.TryParse(text2, ref endDateTime);
				if (nowTime >= startDateTime && nowTime <= endDateTime)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			this.GetNextStartAndEndTime(nowTime, timePoints, out startDateTime, out endDateTime);
		}
		return flag;
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
		int num;
		if (dayOfWeak < normalWeekday)
		{
			num = 7 + dayOfWeak - normalWeekday;
		}
		else if (dayOfWeak > normalWeekday)
		{
			num = dayOfWeak - normalWeekday;
		}
		else if (nowTime <= dateTime)
		{
			num = 0;
		}
		else
		{
			num = 7;
		}
		startDateTime = dateTime.AddDays((double)num);
		endDateTime = dateTime2.AddDays((double)num);
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
		if (text == string.Empty)
		{
			text = "0";
		}
		return text;
	}

	private string GetWeekName(int dayOfWeek)
	{
		string result = string.Empty;
		switch (dayOfWeek)
		{
		case 0:
			result = Global.GetLang("周日");
			break;
		case 1:
			result = Global.GetLang("周一");
			break;
		case 2:
			result = Global.GetLang("周二");
			break;
		case 3:
			result = Global.GetLang("周三");
			break;
		case 4:
			result = Global.GetLang("周四");
			break;
		case 5:
			result = Global.GetLang("周五");
			break;
		case 6:
			result = Global.GetLang("周六");
			break;
		case 7:
			result = Global.GetLang("周日");
			break;
		}
		return result;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.StopTeamCompeteTimer();
		base.StopAllCoroutines();
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO", new Action<MUSocketConnectEventArgs>(this.RespondTeamInfo));
		MUEventManager.AddEventListener<ZorkBattleGameStates>("CMD_SPR_ZORK_STATE", new Action<ZorkBattleGameStates>(this.RespondDuoBaostate));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO", new Action<MUSocketConnectEventArgs>(this.RespondTeamInfo));
		MUEventManager.RemoveEventListener<ZorkBattleGameStates>("CMD_SPR_ZORK_STATE", new Action<ZorkBattleGameStates>(this.RespondDuoBaostate));
	}

	public void RequestGetDuoBaoState()
	{
		GameInstance.Game.SendDuoBaoGetActivityState();
	}

	public void RequestBaoMing()
	{
		if (this.beCanClick)
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendBaoMing();
			this.beCanClick = false;
			base.Invoke("ResetCanClick", 1f);
		}
	}

	public void RequestEnter()
	{
		if (this.beCanClick)
		{
			GameInstance.Game.SendDuoBaoEnter();
			this.beCanClick = false;
			base.Invoke("ResetCanClick", 1f);
		}
	}

	private void ResetCanClick()
	{
		this.beCanClick = true;
	}

	public void RequestTeamInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.RequestSingleTeamInfoMsg();
	}

	public void RespondTeamInfo(MUSocketConnectEventArgs e)
	{
		TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = DataHelper.BytesToObject<TianTi5v5ZhanDuiData>(e.bytesData, 0, e.bytesData.Length);
		if (tianTi5v5ZhanDuiData == null)
		{
			this.m_isLeader = false;
		}
		else
		{
			TeamCompeteDataManager.MainZhanDuiData = tianTi5v5ZhanDuiData;
		}
		this.RefershInfo();
	}

	public void RespondDuoBaostate(ZorkBattleGameStates state)
	{
		if (this.m_formerServerState != state)
		{
			this.RefershInfo();
			this.m_formerServerState = state;
		}
	}

	public GameObject goBestTeam;

	public UILabel lblSelfName;

	public UILabel lblTimeWord;

	public UILabel lblTime;

	public UILabel lblEndTime;

	public UILabel lblEndWord;

	public UILabel lblRoundWord;

	public UILabel lblRoundNum;

	public GButton btnClose;

	public GButton btnHelp;

	public GButton btnChengJiu;

	public GButton btnPaiMing;

	public GButton btnJiangLi;

	public GButton btnChengHao;

	public GButton btnBaoMing;

	public UISprite imgBg;

	public UIGrid gridReward;

	public GameObject goChengHao;

	public UILabel lblTitle1;

	public UILabel lblTitle2;

	public UILabel lblTitleContent1;

	public UILabel lblTitleContent2;

	public GButton btnChengHaoClose;

	private bool m_isLeader;

	private string m_activityTimePoints;

	private int m_battleSignSecs;

	private int m_eachBattleTime;

	private DuoBaoTimeState n_duoBaoState = DuoBaoTimeState.NotStart;

	private ZorkBattleGameStates m_formerServerState = 4;

	private float m_remainderSeconds;

	private string m_color;

	private string m_showTime;

	private DateTime m_startTime = default(DateTime);

	private DateTime m_endTime = default(DateTime);

	private DateTime m_nextUpdateState = default(DateTime);

	private DispatcherTimer TeamCompeteUITimer;

	private int m_timeNum;

	private bool beCanClick = true;
}
