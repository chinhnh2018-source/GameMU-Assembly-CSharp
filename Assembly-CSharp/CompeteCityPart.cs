using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class CompeteCityPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.JoinLab.text = string.Empty;
		this.BtnJoin.Label.text = Global.GetLang("立即报名");
		this.BtnJoin.gameObject.SetActive(true);
		this.BtnJoin.isEnabled = false;
		this.GuanJun.text = string.Empty;
		this.ActTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("活动时间："),
			"fac60d",
			string.Format(Global.GetLang("每周三报名，周三，四，五{0}-{1}举行活动"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
		});
		this.BackGround.URL = "NetImages/GameRes/Images/CompeteCityBG/CompeteCityMainDiTu.png.qj";
	}

	private void UpdateBtnState()
	{
		if (this.IsBtnLastWeek())
		{
			this.BtnZhanKuang.normalSprite = (this.BtnZhanKuang.hoverSprite = (this.BtnZhanKuang.pressedSprite = "zhankuang1"));
		}
		else
		{
			this.BtnZhanKuang.normalSprite = (this.BtnZhanKuang.hoverSprite = (this.BtnZhanKuang.pressedSprite = "jinji"));
		}
	}

	private void GetConstTimes()
	{
		CompeteCityPart.ConstStartTime = CompeteCityPart.GetDicPlunderLands()[1].TimePoints.Split(new char[]
		{
			'-'
		})[0].Split(new char[]
		{
			','
		})[1];
		CompeteCityPart.ConstEndTime = CompeteCityPart.GetDicPlunderLands()[1].TimePoints.Split(new char[]
		{
			'-'
		})[1];
		this.ConstStartTime2 = CompeteCityPart.GetDicPlunderLands()[3].TimePoints.Split(new char[]
		{
			'-'
		})[0].Split(new char[]
		{
			','
		})[1];
		this.ConstEndTime2 = CompeteCityPart.GetDicPlunderLands()[2].TimePoints.Split(new char[]
		{
			'-'
		})[1];
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.GetConstTimes();
		this.InitTextInPrefabs();
		this.InitZhengDiTime();
		this.UpdateBtnState();
		this.ItemCollection = this.ListItems.ItemsSource;
		this.loadGoodsList(CompeteCityPart.GetDicPlunderLands()[1].ShowAward);
		this.BtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenRuleInterFace();
		};
		this.BtnZhanKuang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsHaiXuan)
			{
				this.OpenZhanKuangInterFace();
			}
			else
			{
				this.OpenJinJiInterFace();
			}
		};
		this.BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsBaoMing)
			{
				GameInstance.Game.SendCompeteCityAcitvityApplyData();
				Super.ShowNetWaiting(null);
			}
			else
			{
				GameInstance.Game.SendCompeteCityEnterGameData();
				Super.ShowNetWaiting(null);
			}
		};
		GameInstance.Game.SendCompeteCityMainActivityStateData();
		Super.ShowNetWaiting(null);
	}

	private string GetGrade(int grade, int lose)
	{
		string lang = Global.GetLang("输给");
		if (lose == 1 && grade <= 5)
		{
			return lang;
		}
		if (grade == 1 && lose == 0)
		{
			return Global.GetLang("16强晋级赛");
		}
		if (grade == 2 && lose == 0)
		{
			return Global.GetLang("8强晋级赛");
		}
		if (grade == 3 && lose == 0)
		{
			return Global.GetLang("4强晋级赛");
		}
		if (grade == 4 && lose == 0)
		{
			return Global.GetLang("半决赛");
		}
		if (grade == 5 && lose == 0)
		{
			return Global.GetLang("决赛");
		}
		if (grade > 5)
		{
			return Global.GetLang("上周冠军");
		}
		return lang;
	}

	public void DisposeMainData(CompeteCityStateData mainData)
	{
		if (mainData == null)
		{
			this.BtnZhanKuang.gameObject.SetActive(false);
			return;
		}
		this.BtnZhanKuang.gameObject.SetActive(true);
		if (Global.GetCorrectDateTime().DayOfWeek == 3)
		{
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = default(DateTime);
			string text = string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				Global.GetCorrectDateTime().Year,
				Global.GetCorrectDateTime().Month,
				Global.GetCorrectDateTime().Day,
				CompeteCityPart.ConstStartTime
			});
			string text2 = string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				Global.GetCorrectDateTime().Year,
				Global.GetCorrectDateTime().Month,
				Global.GetCorrectDateTime().Day,
				CompeteCityPart.ConstEndTime
			});
			DateTime.TryParse(text, ref dateTime);
			DateTime.TryParse(text2, ref dateTime2);
			if (Global.GetCorrectDateTime().Ticks >= dateTime.Ticks && Global.GetCorrectDateTime().Ticks <= dateTime2.Ticks)
			{
				this.IsHaiXuan = true;
			}
		}
		if (!this.IsFireTime())
		{
			this.currentJieDuan = mainData.Step + 1;
		}
		else
		{
			this.currentJieDuan = mainData.Step;
		}
		this.MyState = mainData.State;
		string text3 = string.Empty;
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(mainData.OtherZoneId, out ztBuffServerInfo))
		{
			if (this.currentJieDuan > 5 && mainData.Lose == 0)
			{
				text3 = this.GetGrade(this.currentJieDuan, mainData.Lose) + "\r\n" + Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, mainData.OtherName, 0);
			}
			else
			{
				text3 = this.GetGrade(this.currentJieDuan, mainData.Lose) + Global.GetLang("对手") + "\r\n" + Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, mainData.OtherName, 0);
			}
		}
		else if (this.currentJieDuan > 5 && mainData.Lose == 0)
		{
			text3 = string.Format("{0}\r\nS{1}.{2}", this.GetGrade(this.currentJieDuan, mainData.Lose), mainData.OtherZoneId, mainData.OtherName);
		}
		else
		{
			text3 = string.Format(Global.GetLang("{0}对手\r\nS{1}.{2}"), this.GetGrade(this.currentJieDuan, mainData.Lose), mainData.OtherZoneId, mainData.OtherName);
		}
		if (mainData.OtherZoneId == 0 && string.IsNullOrEmpty(mainData.OtherName))
		{
			this.GuanJun.text = string.Empty;
		}
		else
		{
			this.GuanJun.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				text3
			});
		}
		if (mainData.Step != 1 && mainData.SignUp == 0)
		{
			this.JoinLab.text = Global.GetLang("未报名");
			this.BtnJoin.gameObject.SetActive(false);
		}
		if (mainData.Step == 1 && mainData.SignUp == 0 && mainData.State == 1)
		{
			RoleData roleData = Global.Data.roleData;
			if (Global.IsBangHuiLeader(roleData, roleData.Faction))
			{
				this.JoinLab.text = string.Empty;
				this.BtnJoin.Label.text = Global.GetLang("立即报名");
				this.IsBaoMing = true;
				this.BtnJoin.gameObject.SetActive(true);
				this.BtnJoin.isEnabled = true;
			}
			else
			{
				this.JoinLab.text = Global.GetLang("需盟主报名参战");
				this.BtnJoin.gameObject.SetActive(false);
			}
		}
		if (mainData.SignUp == 1 && mainData.State == 1 && mainData.Lose == 0)
		{
			this.JoinLab.text = string.Empty;
			this.BtnJoin.Label.text = Global.GetLang("立即参战");
			this.IsBaoMing = false;
			this.BtnJoin.gameObject.SetActive(true);
			this.BtnJoin.isEnabled = true;
		}
		if (mainData.SignUp == 1 && mainData.Lose == 1)
		{
			this.JoinLab.text = Global.GetLang("未晋级");
			this.BtnJoin.gameObject.SetActive(false);
		}
		if (mainData.SignUp == 1 && mainData.State == 0 && mainData.Lose == 0)
		{
			this.JoinLab.text = string.Empty;
			this.BtnJoin.Label.text = Global.GetLang("立即参战");
			this.BtnJoin.gameObject.SetActive(true);
			this.IsBaoMing = false;
			this.BtnJoin.isEnabled = false;
		}
	}

	public void SetBtnState()
	{
		this.JoinLab.text = string.Empty;
		this.BtnJoin.Label.text = Global.GetLang("立即参战");
		this.IsBaoMing = false;
		this.BtnJoin.gameObject.SetActive(true);
	}

	private bool IsShowZhanKuangOrJinJiMingDan()
	{
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime.DayOfWeek == 3)
		{
			string text = string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				correctDateTime.Year,
				correctDateTime.Month,
				correctDateTime.Day,
				CompeteCityPart.ConstStartTime
			});
			string text2 = string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				correctDateTime.Year,
				correctDateTime.Month,
				correctDateTime.Day,
				CompeteCityPart.ConstEndTime
			});
			DateTime.TryParse(text, ref dateTime);
			DateTime.TryParse(text2, ref dateTime2);
			if (correctDateTime.Ticks >= dateTime.Ticks && correctDateTime.Ticks <= dateTime2.Ticks)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsBtnLastWeek()
	{
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime.DayOfWeek >= 3 && correctDateTime.DayOfWeek <= 5)
		{
			int num = 5 - correctDateTime.DayOfWeek;
			string text = string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				correctDateTime.Year,
				correctDateTime.Month,
				correctDateTime.AddDays(3 - correctDateTime.DayOfWeek).Day,
				CompeteCityPart.ConstStartTime
			});
			string text2 = string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				correctDateTime.Year,
				correctDateTime.Month,
				correctDateTime.AddDays((double)Math.Abs(correctDateTime.DayOfWeek - 5)).Day,
				CompeteCityPart.ConstEndTime
			});
			DateTime.TryParse(text, ref dateTime);
			DateTime.TryParse(text2, ref dateTime2);
			correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime.Ticks >= dateTime.Ticks && correctDateTime.Ticks <= dateTime2.Ticks)
			{
				return false;
			}
		}
		return true;
	}

	private void OpenRuleInterFace()
	{
		if (this.CCRuleWindow == null)
		{
			this.CCRuleWindow = U3DUtils.NEW<GChildWindow>();
			this.CCRuleWindow.IsShowModal = true;
			this.CCRuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.CCRuleWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.CCRuleWindow);
		}
		if (this.CCRule == null)
		{
			this.CCRule = U3DUtils.NEW<CompeteCityPartRule>();
			this.CCRule.CloseHandle = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseRuleInterFace();
			};
		}
		this.CCRuleWindow.SetContent(this.CCRuleWindow.BodyPresenter, this.CCRule, 0.0, 0.0, true);
	}

	private void CloseRuleInterFace()
	{
		if (null != this.CCRule)
		{
			this.CCRule.transform.parent = null;
			Object.Destroy(this.CCRule.gameObject);
			this.CCRule = null;
		}
		if (null != this.CCRuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.CCRuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.CCRuleWindow, true);
			this.CCRuleWindow = null;
		}
	}

	private void OpenZhanKuangInterFace()
	{
		if (this.CCZhanKuangWindow == null)
		{
			this.CCZhanKuangWindow = U3DUtils.NEW<GChildWindow>();
			this.CCZhanKuangWindow.IsShowModal = true;
			this.CCZhanKuangWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.CCZhanKuangWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.CCZhanKuangWindow);
		}
		if (this.CCZhanKuang == null)
		{
			this.CCZhanKuang = U3DUtils.NEW<CompeteCityPartLastZhanKuang>();
			this.CCZhanKuang.SetTimeLab = this.currentJieDuan;
			this.CCZhanKuang.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseZhanKuangInterFace();
			};
		}
		this.CCZhanKuangWindow.SetContent(this.CCZhanKuangWindow.BodyPresenter, this.CCZhanKuang, 0.0, 0.0, true);
	}

	private void CloseZhanKuangInterFace()
	{
		if (null != this.CCZhanKuang)
		{
			this.CCZhanKuang.transform.parent = null;
			Object.Destroy(this.CCZhanKuang.gameObject);
			this.CCZhanKuang = null;
		}
		if (null != this.CCZhanKuangWindow)
		{
			Super.CloseChildWindow(base.Children, this.CCZhanKuangWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.CCZhanKuangWindow, true);
			this.CCZhanKuangWindow = null;
		}
	}

	private void OpenJinJiInterFace()
	{
		if (this.CCJinJiWindow == null)
		{
			this.CCJinJiWindow = U3DUtils.NEW<GChildWindow>();
			this.CCJinJiWindow.IsShowModal = true;
			this.CCJinJiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.CCJinJiWindow, Global.GetLang("晋级界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.CCJinJiWindow);
		}
		if (this.CCJinJi == null)
		{
			this.CCJinJi = U3DUtils.NEW<CompeteCityPartJinJi>();
			this.CCJinJi.SetMyPaiMing = this.MyState;
			this.CCJinJi.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseJinJiInterFace();
			};
		}
		this.CCJinJiWindow.SetContent(this.CCJinJiWindow.BodyPresenter, this.CCJinJi, 0.0, 0.0, true);
	}

	private void CloseJinJiInterFace()
	{
		if (null != this.CCJinJi)
		{
			this.CCJinJi.transform.parent = null;
			Object.Destroy(this.CCJinJi.gameObject);
			this.CCJinJi = null;
		}
		if (null != this.CCJinJiWindow)
		{
			Super.CloseChildWindow(base.Children, this.CCJinJiWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.CCJinJiWindow, true);
			this.CCJinJiWindow = null;
		}
	}

	private bool IsFireTime()
	{
		this.currentTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		if (this.currentTime.DayOfWeek == 3)
		{
			dateTime = this.currentTime;
			this.GetStartTime(dateTime, 1);
			this.GetEndTime(dateTime, 1);
			if (this.currentTime.Ticks >= this.startTime1.Ticks && this.currentTime.Ticks <= this.endTime1.Ticks)
			{
				return true;
			}
		}
		else if (this.currentTime.DayOfWeek == 4 || this.currentTime.DayOfWeek == 5)
		{
			dateTime = this.currentTime;
			this.GetStartTime(dateTime, 1);
			this.GetEndTime(dateTime, 1);
			this.GetStartTime(dateTime, 2);
			this.GetEndTime(dateTime, 2);
			if (this.currentTime.Ticks >= this.startTime1.Ticks && this.currentTime.Ticks <= this.endTime1.Ticks)
			{
				return true;
			}
			if (this.currentTime.Ticks >= this.startTime2.Ticks && this.currentTime.Ticks <= this.endTime2.Ticks)
			{
				return true;
			}
		}
		return false;
	}

	private void InitZhengDiTime()
	{
		this.currentTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		if (this.currentTime.DayOfWeek < 3)
		{
			this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("距活动开启：")
			});
			dateTime = this.currentTime.AddDays(3 - this.currentTime.DayOfWeek);
			this.GetStartTime(dateTime, 1);
			this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
			this.color = "FF0000";
		}
		else if (this.currentTime.DayOfWeek == 3)
		{
			dateTime = this.currentTime;
			this.GetStartTime(dateTime, 1);
			this.GetEndTime(dateTime, 1);
			if (this.currentTime.Ticks < this.startTime1.Ticks)
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开启：")
				});
				this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
			else if (this.currentTime.Ticks >= this.startTime1.Ticks && this.currentTime.Ticks <= this.endTime1.Ticks)
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("距活动结束：")
				});
				this.countdowntimes = (this.endTime1.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "17e43e";
			}
			else
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开启：")
				});
				this.countdowntimes = (this.startTime1.AddDays(1.0).Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
		}
		else if (this.currentTime.DayOfWeek == 4 || this.currentTime.DayOfWeek == 5)
		{
			dateTime = this.currentTime;
			this.GetStartTime(dateTime, 1);
			this.GetEndTime(dateTime, 1);
			this.GetStartTime(dateTime, 2);
			this.GetEndTime(dateTime, 2);
			if (this.currentTime.Ticks < this.startTime1.Ticks)
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开启：")
				});
				this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
			else if (this.currentTime.Ticks >= this.startTime1.Ticks && this.currentTime.Ticks <= this.endTime1.Ticks)
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("距活动结束：")
				});
				this.countdowntimes = (this.endTime1.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "17e43e";
			}
			else if (this.currentTime.Ticks > this.endTime1.Ticks && this.currentTime.Ticks < this.startTime2.Ticks)
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开启：")
				});
				this.countdowntimes = (this.startTime2.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
			else if (this.currentTime.Ticks >= this.startTime2.Ticks && this.currentTime.Ticks <= this.endTime2.Ticks)
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("距活动结束：")
				});
				this.countdowntimes = (this.endTime2.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "17e43e";
			}
			else
			{
				this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开启：")
				});
				long num = (this.currentTime.DayOfWeek != 5) ? this.startTime1.AddDays(1.0).Ticks : this.startTime1.AddDays(5.0).Ticks;
				this.countdowntimes = (num - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
		}
		else if (this.currentTime.DayOfWeek == 6)
		{
			this.ActTimeLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("距活动开启：")
			});
			dateTime = this.currentTime.AddDays(4.0);
			this.GetStartTime(dateTime, 1);
			this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
			this.color = "FF0000";
		}
		this.StartUITimer();
	}

	public void GetStartTime(DateTime StartDay, int changci = 1)
	{
		string text = string.Format("{0}-{1}-{2}", StartDay.Year, StartDay.Month, StartDay.Day);
		string text2 = (changci != 1) ? string.Format(" {0}:00", this.ConstStartTime2) : string.Format(" {0}:00", CompeteCityPart.ConstStartTime);
		if (changci == 1)
		{
			DateTime.TryParse(text + text2, ref this.startTime1);
		}
		else
		{
			DateTime.TryParse(text + text2, ref this.startTime2);
		}
	}

	public void GetEndTime(DateTime EndDay, int changci = 1)
	{
		string text = string.Format("{0}-{1}-{2}", EndDay.Year, EndDay.Month, EndDay.Day);
		string text2 = (changci != 1) ? string.Format(" {0}:00", CompeteCityPart.ConstEndTime) : string.Format(" {0}:00", this.ConstEndTime2);
		if (EndDay.DayOfWeek == 3)
		{
			DateTime.TryParse(text + string.Format(" {0}:00", CompeteCityPart.ConstEndTime), ref this.endTime1);
			return;
		}
		if (changci == 1)
		{
			DateTime.TryParse(text + text2, ref this.endTime1);
		}
		else
		{
			DateTime.TryParse(text + text2, ref this.endTime2);
		}
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("CompeteCityPart_Timer");
		this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
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

	protected void UITimer_Tick(object sender, object e)
	{
		if (this.countdowntimes > 0L)
		{
			this.DaoJiShi.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				string.Format(Global.GetLang("剩余 {0}"), CompeteCityPart.GetTimeStrBySecEx((double)this.countdowntimes))
			});
			this.countdowntimes -= 1L;
		}
		else
		{
			this.DaoJiShi.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.color,
				string.Format(Global.GetLang("剩余0秒"), new object[0])
			});
			this.StopTimer();
			this.InitZhengDiTime();
			this.UpdateBtnState();
			GameInstance.Game.SendCompeteCityMainActivityStateData();
			Super.ShowNetWaiting(null);
		}
	}

	public override void Destroy()
	{
		this.StopTimer();
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

	private void loadGoodsList(string Goods)
	{
		this.ItemCollection.Clear();
		string text = StringUtil.trim(Goods);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				this.addGoodsIcon(dummyGoodsDataMu, false);
			}
		}
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = -1;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			this.ItemCollection.AddNoUpdate(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public static Dictionary<int, PlunderLands> GetDicPlunderLands()
	{
		if (CompeteCityPart.dicPlunderLands.Count > 0)
		{
			return CompeteCityPart.dicPlunderLands;
		}
		XElement gameResXml = Global.GetGameResXml("Config/PlunderLands.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "PlunderLands");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			PlunderLands plunderLands = new PlunderLands();
			plunderLands.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			plunderLands.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			plunderLands.TimePoints = Global.GetXElementAttributeStr(xelementList[i], "TimePoints");
			plunderLands.Applv = Global.GetXElementAttributeInt(xelementList[i], "Applv");
			plunderLands.PrepareSecs = Global.GetXElementAttributeInt(xelementList[i], "PrepareSecs");
			plunderLands.FightingSecs = Global.GetXElementAttributeInt(xelementList[i], "FightingSecs");
			plunderLands.ClearRolesSecs = Global.GetXElementAttributeInt(xelementList[i], "ClearRolesSecs");
			plunderLands.ShowAward = Global.GetXElementAttributeStr(xelementList[i], "ShowAward");
			if (!CompeteCityPart.dicPlunderLands.ContainsKey(plunderLands.ID))
			{
				CompeteCityPart.dicPlunderLands.Add(plunderLands.ID, plunderLands);
			}
			i++;
		}
		return CompeteCityPart.dicPlunderLands;
	}

	public static void ClearXMLData()
	{
		if (0 < CompeteCityPart.dicPlunderLands.Count)
		{
			CompeteCityPart.dicPlunderLands.Clear();
		}
	}

	public DPSelectedItemEventHandler Handler;

	public GButton BtnHelp;

	public GButton BtnZhanKuang;

	public GButton BtnJoin;

	public UILabel JoinLab;

	public UILabel GuanJun;

	public UILabel ActTime;

	public UILabel ActTimeLab;

	public UILabel DaoJiShi;

	public ListBox ListItems;

	public ShowNetImage BackGround;

	public GChildWindow CCRuleWindow;

	public CompeteCityPartRule CCRule;

	public GChildWindow CCZhanKuangWindow;

	public CompeteCityPartLastZhanKuang CCZhanKuang;

	public GChildWindow CCJinJiWindow;

	public CompeteCityPartJinJi CCJinJi;

	private string color = "17e43e";

	private long countdowntimes;

	private bool IsBaoMing;

	private bool IsHaiXuan;

	private int currentJieDuan;

	private int MyState;

	public static string ConstStartTime = null;

	public static string ConstEndTime = null;

	private string ConstStartTime2;

	private string ConstEndTime2;

	private ObservableCollection _ItemCollection;

	private DateTime currentTime;

	private DateTime startTime1 = default(DateTime);

	private DateTime endTime1 = default(DateTime);

	private DateTime startTime2 = default(DateTime);

	private DateTime endTime2 = default(DateTime);

	private DispatcherTimer UITimer;

	private static Dictionary<int, PlunderLands> dicPlunderLands = new Dictionary<int, PlunderLands>();
}
