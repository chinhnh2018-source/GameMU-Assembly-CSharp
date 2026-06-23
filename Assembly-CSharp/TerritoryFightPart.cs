using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class TerritoryFightPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BackGround.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/lingdi.jpg.qj";
		this.BtnRule.Label.text = Global.GetLang("查看规则");
		this.BtnJoin.Label.text = Global.GetLang("参加战斗");
		this.ActTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("活动时间：每周日20:00-20:30"), new object[0])
		});
	}

	public void SetLingDiZhanLingName(string xiName, string dongName)
	{
		if (xiName == null || string.IsNullOrEmpty(xiName))
		{
			this.AKaLunXiLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("阿卡伦-西：暂无占领")
			});
		}
		else
		{
			this.AKaLunXiLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("阿卡伦-西：{0}"), xiName)
			});
		}
		if (dongName == null || string.IsNullOrEmpty(dongName))
		{
			this.AKaLunDongLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("阿卡伦-东：暂无占领")
			});
		}
		else
		{
			this.AKaLunDongLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("阿卡伦-东：{0}"), dongName)
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.GetConstTimes();
		this.InitZhengDuoTime();
		this.BtnGongXianTop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowArmyGroupGongXianRank();
		};
		this.BtnZhanLingFuLi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenTFZhanLingWindow();
		};
		this.BtnRule.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenTFRuleWindowWindow();
		};
		this.BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.state == 4)
			{
				Super.HintMainText(Global.GetLang("军团未获得参战资格"), 10, 3);
				return;
			}
			if (this.state == 3)
			{
				Super.HintMainText(Global.GetLang("只有军团精英可参加活动"), 10, 3);
				return;
			}
			if (this.state == 1 || this.state == 0)
			{
				Super.HintMainText(Global.GetLang("当前不在领地争夺时间"), 10, 3);
				return;
			}
			PlayZone.GlobalPlayZone.OpenTFJoinWindow();
		};
		GameInstance.Game.JionFightingState();
		Super.ShowNetWaiting(null);
	}

	private void OpenTFZhanLingWindow()
	{
		if (this.TFZhanLingWindow == null)
		{
			this.TFZhanLingWindow = U3DUtils.NEW<GChildWindow>();
			this.TFZhanLingWindow.IsShowModal = true;
			this.TFZhanLingWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.TFZhanLingWindow, Global.GetLang("占领界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.TFZhanLingWindow);
		}
		if (this.TFZhanLingPart == null)
		{
			this.TFZhanLingPart = U3DUtils.NEW<TerritoryFightPartZhanLingFuLi>();
			this.TFZhanLingPart.DPSelectedClose = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseTFZhanLingWindow();
			};
		}
		this.TFZhanLingWindow.SetContent(this.TFZhanLingWindow.BodyPresenter, this.TFZhanLingPart, 0.0, 0.0, true);
	}

	private void CloseTFZhanLingWindow()
	{
		if (null != this.TFZhanLingPart)
		{
			this.TFZhanLingPart.transform.parent = null;
			Object.Destroy(this.TFZhanLingPart.gameObject);
			this.TFZhanLingPart = null;
		}
		if (null != this.TFZhanLingWindow)
		{
			Super.CloseChildWindow(base.Children, this.TFZhanLingWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.TFZhanLingWindow, true);
			this.TFZhanLingWindow = null;
		}
	}

	private void OpenTFRuleWindowWindow()
	{
		if (this.TFRuleWindow == null)
		{
			this.TFRuleWindow = U3DUtils.NEW<GChildWindow>();
			this.TFRuleWindow.IsShowModal = true;
			this.TFRuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.TFRuleWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.TFRuleWindow);
		}
		if (this.TFRulePart == null)
		{
			this.TFRulePart = U3DUtils.NEW<TerritoryFightPartRule>();
			this.TFRulePart.DPSelectedClose = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseTFRuleWindow();
			};
		}
		this.TFRuleWindow.SetContent(this.TFRuleWindow.BodyPresenter, this.TFRulePart, 0.0, 0.0, true);
	}

	private void CloseTFRuleWindow()
	{
		if (null != this.TFRulePart)
		{
			this.TFRulePart.transform.parent = null;
			Object.Destroy(this.TFRulePart.gameObject);
			this.TFRulePart = null;
		}
		if (null != this.TFRuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.TFRuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.TFRuleWindow, true);
			this.TFRuleWindow = null;
		}
	}

	private void GetConstTimes()
	{
		this.ConstStartTime = TerritoryFightPart.GetDicLegionsWar()[1].TimePoints.Split(new char[]
		{
			'-'
		})[0].Split(new char[]
		{
			','
		})[1];
		this.ConstEndTime = TerritoryFightPart.GetDicLegionsWar()[1].TimePoints.Split(new char[]
		{
			'-'
		})[1];
		this.dayOfWeek = int.Parse(TerritoryFightPart.GetDicLegionsWar()[1].TimePoints.Split(new char[]
		{
			'-'
		})[0].Split(new char[]
		{
			','
		})[0]);
	}

	private void InitZhengDuoTime()
	{
		this.currentTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		if (this.currentTime.DayOfWeek < this.dayOfWeek)
		{
			this.DaoJiShiLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("距活动开始：")
			});
			dateTime = this.currentTime.AddDays(this.dayOfWeek - this.currentTime.DayOfWeek);
			this.GetStartTime(dateTime);
			this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
			this.color = "FF0000";
		}
		else if (this.currentTime.DayOfWeek == this.dayOfWeek)
		{
			dateTime = this.currentTime;
			this.GetStartTime(dateTime);
			this.GetEndTime(dateTime);
			if (this.currentTime.Ticks < this.startTime1.Ticks)
			{
				this.DaoJiShiLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开始：")
				});
				this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
			else if (this.currentTime.Ticks >= this.startTime1.Ticks && this.currentTime.Ticks <= this.endTime1.Ticks)
			{
				this.DaoJiShiLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("距活动结束：")
				});
				this.countdowntimes = (this.endTime1.Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "17e43e";
			}
			else
			{
				this.DaoJiShiLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("距活动开始：")
				});
				this.countdowntimes = (this.startTime1.AddDays(7.0).Ticks - this.currentTime.Ticks) / 10000000L;
				this.color = "FF0000";
			}
		}
		else if (this.currentTime.DayOfWeek > this.dayOfWeek)
		{
			this.DaoJiShiLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("距活动开始：")
			});
			dateTime = this.currentTime.AddDays(7 - (this.currentTime.DayOfWeek - this.dayOfWeek));
			this.GetStartTime(dateTime);
			this.countdowntimes = (this.startTime1.Ticks - this.currentTime.Ticks) / 10000000L;
			this.color = "FF0000";
		}
		this.StartUITimer();
	}

	public void GetStartTime(DateTime StartDay)
	{
		string text = string.Format("{0}-{1}-{2}", StartDay.Year, StartDay.Month, StartDay.Day);
		string text2 = string.Format(" {0}", this.ConstStartTime);
		DateTime.TryParse(text + text2, ref this.startTime1);
	}

	public void GetEndTime(DateTime EndDay)
	{
		string text = string.Format("{0}-{1}-{2}", EndDay.Year, EndDay.Month, EndDay.Day);
		string text2 = string.Format(" {0}", this.ConstEndTime);
		DateTime.TryParse(text + text2, ref this.endTime1);
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("LogionsWar_Timer");
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
				string.Format(Global.GetLang("剩余 {0}"), TerritoryFightPart.GetTimeStrBySecEx((double)this.countdowntimes))
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
			this.InitZhengDuoTime();
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

	public static Dictionary<int, LegionsWar> GetDicLegionsWar()
	{
		if (TerritoryFightPart.dicLegionsWar.Count > 0)
		{
			return TerritoryFightPart.dicLegionsWar;
		}
		XElement gameResXml = Global.GetGameResXml("Config/LegionsWar.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "LegionsWar");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			LegionsWar legionsWar = new LegionsWar();
			legionsWar.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			legionsWar.TimePoints = Global.GetXElementAttributeStr(xelementList[i], "TimePoints");
			legionsWar.WinGoods = Global.GetXElementAttributeStr(xelementList[i], "WinGoods");
			legionsWar.LoseGoods = Global.GetXElementAttributeStr(xelementList[i], "LoseGoods");
			legionsWar.MaxEnterNum = Global.GetXElementAttributeInt(xelementList[i], "MaxEnterNum");
			if (!TerritoryFightPart.dicLegionsWar.ContainsKey(legionsWar.ID))
			{
				TerritoryFightPart.dicLegionsWar.Add(legionsWar.ID, legionsWar);
			}
			i++;
		}
		return TerritoryFightPart.dicLegionsWar;
	}

	public static void ClearXMLData()
	{
		if (0 < TerritoryFightPart.dicLegionsWar.Count)
		{
			TerritoryFightPart.dicLegionsWar.Clear();
		}
	}

	public DPSelectedItemEventHandler Handler;

	public GButton BtnGongXianTop;

	public GButton BtnZhanLingFuLi;

	public GButton BtnRule;

	public GButton BtnJoin;

	public UILabel AKaLunXiLab;

	public UILabel AKaLunDongLab;

	public UILabel ActTime;

	public UILabel DaoJiShiLab;

	public UILabel DaoJiShi;

	public ShowNetImage BackGround;

	public int state;

	protected GChildWindow TFZhanLingWindow;

	protected TerritoryFightPartZhanLingFuLi TFZhanLingPart;

	protected GChildWindow TFRuleWindow;

	protected TerritoryFightPartRule TFRulePart;

	private string color = "17e43e";

	private long countdowntimes;

	private string ConstStartTime;

	private string ConstEndTime;

	private DateTime currentTime;

	private DateTime startTime1 = default(DateTime);

	private DateTime endTime1 = default(DateTime);

	private DayOfWeek dayOfWeek;

	private DispatcherTimer UITimer;

	private static Dictionary<int, LegionsWar> dicLegionsWar = new Dictionary<int, LegionsWar>();
}
