using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class OlympicsGuessPart : UserControl
{
	private string CountDownTimeStr
	{
		get
		{
			return this.countDownTime;
		}
		set
		{
			this.countDownTime = value;
			this.ShowGuessCountDownTime(this.countDownTime);
		}
	}

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

	protected override void InitializeComponent()
	{
		this.btnYesterday.Text = Global.GetLang("昨日记录");
		this.ItemCollection = this.itemList.ItemsSource;
		base.InitializeComponent();
		NGUITools.SetActive(this.finishTipsLabel.gameObject, false);
		NGUITools.SetActive(this.btnSubmit.gameObject, true);
		this.btnSubmit.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (Global.IsInOlympicsAwardActivity())
			{
				Super.HintMainText(Global.GetLang("竞猜已结束"), 10, 3);
				return;
			}
			for (int i = 0; i < this.itemList.transform.childCount; i++)
			{
				if (this.itemList[i].GetComponent<OlympicsGuessItem>().GetSingleOption == null)
				{
					Super.HintMainText(Global.GetLang("请全部选择再提交"), 10, 3);
					return;
				}
			}
			this.answers = string.Format("{0},{1},{2}", this.itemList[0].GetComponent<OlympicsGuessItem>().GetSingleOption, this.itemList[1].GetComponent<OlympicsGuessItem>().GetSingleOption, this.itemList[2].GetComponent<OlympicsGuessItem>().GetSingleOption);
			this.itemList[0].GetComponent<OlympicsGuessItem>().SetCheckBoxsUnenable();
			this.itemList[1].GetComponent<OlympicsGuessItem>().SetCheckBoxsUnenable();
			this.itemList[2].GetComponent<OlympicsGuessItem>().SetCheckBoxsUnenable();
			GameInstance.Game.SendOlympicsGuessAnswerRequest(this.answers, OlympicsDataManage.guessDayId);
		};
		this.btnYesterday.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			GameInstance.Game.SendOlympicsGuessResultRequest(2);
		};
		base.InvokeRepeating("CountDownTime", 0f, 1f);
	}

	public void RefreshData(int type)
	{
		if (type == 1)
		{
			this.tmpDataList = OlympicsDataManage.GetGuessData();
			if (this.tmpDataList == null || this.tmpDataList.Count == 0)
			{
				Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
				return;
			}
			this.InitData();
		}
		else if (type == 2)
		{
			this.yesterdayTmpDataList = OlympicsDataManage.GetYesterdayGuessData();
			if (this.yesterdayTmpDataList == null || this.yesterdayTmpDataList.Count == 0)
			{
				Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
				return;
			}
			if (null == this.olympicsYesterdayRecordWind)
			{
				this.olympicsYesterdayRecordWind = U3DUtils.NEW<GChildWindow>();
				this.olympicsYesterdayRecordWind.ModalType = ChildWindowModalType.Translucent;
				Super.InitChildWindow(this.olympicsYesterdayRecordWind, "OlympicsYesterdayRecordWindow");
				Super.GData.GlobalPlayZone.Children.Add(this.olympicsYesterdayRecordWind);
			}
			this.olympicsYesterdayRecordPart = U3DUtils.NEW<OlympicsYesterdayRecordPart>();
			this.olympicsYesterdayRecordWind.Body.Add(this.olympicsYesterdayRecordPart);
			this.olympicsYesterdayRecordPart.Hander = delegate(object s, DPSelectedItemEventArgs args)
			{
				if (args.ID == 0 && null != this.olympicsYesterdayRecordPart)
				{
					Object.Destroy(this.olympicsYesterdayRecordPart.gameObject);
					this.olympicsYesterdayRecordPart = null;
					Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsYesterdayRecordWind);
				}
			};
			this.InitYesterdayData();
		}
	}

	public void InitData()
	{
		if (this.tmpDataList == null || this.tmpDataList.Count <= 0)
		{
			return;
		}
		this.ItemCollection.Clear();
		for (int i = 0; i < this.tmpDataList.Count; i++)
		{
			OlympicsGuessItem olympicsGuessItem = U3DUtils.NEW<OlympicsGuessItem>();
			OlympicsGuessData olympicsGuessData = this.tmpDataList[i];
			olympicsGuessItem.SetValue(i, olympicsGuessData);
			if (olympicsGuessData.Select != -1)
			{
				NGUITools.SetActive(this.finishTipsLabel.gameObject, true);
				NGUITools.SetActive(this.btnSubmit.gameObject, false);
			}
			UIPanel component = olympicsGuessItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, olympicsGuessItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(olympicsGuessItem);
		}
	}

	public void InitYesterdayData()
	{
		if (null != this.olympicsYesterdayRecordPart)
		{
			this.olympicsYesterdayRecordPart.InitData(this.yesterdayTmpDataList);
		}
	}

	private void ShowGuessCountDownTime(string tmpTimStr)
	{
		this.countDownTimeLabel.text = tmpTimStr;
	}

	private void CountDownTime()
	{
		this.currentHour = Global.GetCorrectDateTime().Hour * 3600;
		this.currentMinute = Global.GetCorrectDateTime().Minute * 60;
		this.currentSeconds = Global.GetCorrectDateTime().Second;
		int sec = this.secondsOfOneDay - (this.currentHour + this.currentMinute + this.currentSeconds);
		this.ShowGuessCountDownTime(this.GetTimeStrBySec(sec));
	}

	public string GetTimeStrBySec(int sec)
	{
		int num = sec / 3600;
		int num2 = sec % 3600 / 60;
		int num3 = sec % 3600 % 60;
		if (num == 0 && num2 == 0 && num3 > 0 && num3 < 59)
		{
			return Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				StringUtil.substitute(Global.GetLang("剩余答题时间：{0}小时{1}分{2}秒"), new object[]
				{
					num,
					num2,
					num3
				})
			});
		}
		return Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			StringUtil.substitute(Global.GetLang("剩余答题时间：{0}小时{1}分{2}秒"), new object[]
			{
				num,
				num2,
				num3
			})
		});
	}

	public void StartTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer.Dispose();
			this.m_Timer = null;
		}
		this.m_Timer = new DispatcherTimer("OlympicsGuessPart_Timer");
		this.m_Timer.Interval = TimeSpan.FromSeconds(1.0);
		this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.m_Timer.Start();
	}

	public void StopTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer = null;
		}
		this.Visibility = false;
	}

	private void Timer_Tick(object sender, object e)
	{
	}

	public void ShowSubmitResult(int result)
	{
		if (result == 0)
		{
			Super.HintMainText(Global.GetLang("提交错误，已超时"), 10, 3);
		}
		else if (result == 1)
		{
			Super.HintMainText(Global.GetLang("提交成功"), 10, 3);
			NGUITools.SetActive(this.finishTipsLabel.gameObject, true);
			NGUITools.SetActive(this.btnSubmit.gameObject, false);
		}
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("CountDownTime");
		this.StopTimer();
		base.OnDestroy();
	}

	public void OnDisable()
	{
		this.StopTimer();
	}

	public GButton btnYesterday;

	public GButton btnSubmit;

	public UILabel finishTipsLabel;

	public ListBox itemList;

	public UILabel countDownTimeLabel;

	private List<OlympicsGuessData> tmpDataList;

	private List<OlympicsGuessData> yesterdayTmpDataList;

	private DispatcherTimer m_Timer;

	private string countDownTime;

	private ObservableCollection _ItemCollection;

	private string answers;

	private OlympicsYesterdayRecordPart olympicsYesterdayRecordPart;

	private GChildWindow olympicsYesterdayRecordWind;

	private int secondsOfOneDay = 86400;

	public int currentHour;

	public int currentMinute;

	public int currentSeconds;
}
