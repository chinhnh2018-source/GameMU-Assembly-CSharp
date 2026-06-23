using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class HongBaoQuanMinPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_LabNum.text = string.Empty;
		this.m_LabZuanShiTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("下一个红包总额：")
		}));
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
		this.m_BtnLeft.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.RefshreItem(-1);
		};
		this.m_BtnRight.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.RefshreItem(1);
		};
		base.InitializeComponent();
	}

	private void RefshreItem(int count)
	{
		if (this.m_OnDay + count < 1 || this.m_OnDay + count > this.m_MaxDay)
		{
			return;
		}
		this.m_OnDay += count;
		if (this.m_Map.ContainsKey(this.m_OnDay))
		{
			this.AddItem(this.m_OnDay);
		}
	}

	public void SetXml(string str = "")
	{
		XElement xelement;
		if (string.IsNullOrEmpty(str))
		{
			xelement = Global.GetGameResXml("Config/JieRiGifts/JieRiQuanMinHongBao.xml");
		}
		else
		{
			xelement = XElement.Parse(str);
		}
		XElement xelement2 = Global.GetXElement(xelement, "Activities");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "RedPacketPeople");
		for (int i = 0; i < xelementList.Count; i++)
		{
			RedPacketPeople redPacketPeople = new RedPacketPeople();
			redPacketPeople.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			redPacketPeople.Day = Global.GetXElementAttributeInt(xelementList[i], "Day");
			redPacketPeople.Time = Global.GetXElementAttributeStr(xelementList[i], "Time");
			redPacketPeople.RedPacketSize = Global.GetXElementAttributeInt(xelementList[i], "RedPacketSize");
			redPacketPeople.Interval = Global.GetXElementAttributeStr(xelementList[i], "Interval");
			redPacketPeople.DurationTime = Global.GetXElementAttributeInt(xelementList[i], "DurationTime");
			this.m_MaxDay = redPacketPeople.Day;
			if (!this.m_Map.ContainsKey(redPacketPeople.Day))
			{
				List<RedPacketPeople> list = new List<RedPacketPeople>();
				list.Add(redPacketPeople);
				this.m_Map.Add(redPacketPeople.Day, list);
			}
			else
			{
				this.m_Map[redPacketPeople.Day].Add(redPacketPeople);
			}
		}
		this.m_LabTime1.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间：")
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetXElementAttributeStr(xelement2, "FromDate") + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("至")
			}) + Global.GetXElementAttributeStr(xelement2, "ToDate")
		}));
		this.m_LabContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容：")
		}) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetXElementAttributeStr(Global.GetXElement(xelement, "GiftList"), "Description")
		}));
		DateTime dateTime = Global.SafeConvertDateTime(Global.GetXElementAttributeStr(xelement2, "FromDate"));
		this.m_OnDay = (int)(Global.GetCorrectLocalTime() - dateTime.Ticks / 10000L) / 86400000 + 1;
		this.m_Day = (int)(Global.GetCorrectLocalTime() - dateTime.Ticks / 10000L) / 86400000 + 1;
		if (this.m_Map.ContainsKey(this.m_OnDay))
		{
			this.AddItem(this.m_OnDay);
		}
		this.StartTimer();
	}

	private void AddItem(int key)
	{
		int num = -1;
		if (key <= 1)
		{
			NGUITools.SetActive(this.m_BtnLeft.gameObject, false);
			NGUITools.SetActive(this.m_BtnRight.gameObject, true);
		}
		else if (key >= this.m_MaxDay)
		{
			NGUITools.SetActive(this.m_BtnLeft.gameObject, true);
			NGUITools.SetActive(this.m_BtnRight.gameObject, false);
		}
		else
		{
			NGUITools.SetActive(this.m_BtnLeft.gameObject, true);
			NGUITools.SetActive(this.m_BtnRight.gameObject, true);
		}
		DateTime dateTime = default(DateTime);
		dateTime = Global.GetCorrectDateTime().AddDays((double)(key - this.m_Day));
		this.m_LabTimeToDay.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Concat(new object[]
			{
				dateTime.Year,
				"-",
				dateTime.Month,
				"-",
				dateTime.Day.ToString()
			})
		}));
		this.m_ObservableCollection.Clear();
		for (int i = 0; i < this.m_Map[key].Count; i++)
		{
			HongBaoQuanMinItem hongBaoQuanMinItem = U3DUtils.NEW<HongBaoQuanMinItem>();
			this.m_ObservableCollection.AddNoUpdate(hongBaoQuanMinItem);
			if (this.m_Map[key][i].Time.Split(new char[]
			{
				':'
			}) == null || this.m_Map[key][i].Time.Split(new char[]
			{
				':'
			}).Length != 2)
			{
				MUDebug.Log<string>(new string[]
				{
					"时间表配置错误"
				});
			}
			int num2 = this.m_Map[key][i].Time.Split(new char[]
			{
				':'
			})[0].SafeToInt32(0) * 3600 + this.m_Map[key][i].Time.Split(new char[]
			{
				':'
			})[1].SafeToInt32(0) * 60;
			int num3 = Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second;
			if (key < this.m_Day)
			{
				hongBaoQuanMinItem.EndBool = true;
				hongBaoQuanMinItem.m_Time.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("已结束")
				}));
			}
			else if (key > this.m_Day)
			{
				hongBaoQuanMinItem.m_Time.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Map[key][i].Time
				}));
				hongBaoQuanMinItem.EndBool = false;
			}
			else if (num2 < num3)
			{
				hongBaoQuanMinItem.EndBool = true;
				hongBaoQuanMinItem.m_Time.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("已结束")
				}));
			}
			else
			{
				if (num == -1 || num == this.m_Map[key][i].ID)
				{
					num = this.m_Map[key][i].ID;
					this.m_Key = num;
					hongBaoQuanMinItem.NextHongBao();
					this.m_LabNum.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						this.m_Map[key][i].RedPacketSize
					}));
				}
				hongBaoQuanMinItem.m_Time.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Map[key][i].Time
				}));
				hongBaoQuanMinItem.EndBool = false;
			}
			if (hongBaoQuanMinItem.transform.gameObject.GetComponent<TweenAlpha>() != null)
			{
				hongBaoQuanMinItem.transform.gameObject.GetComponent<TweenAlpha>().Reset();
				hongBaoQuanMinItem.transform.gameObject.GetComponent<TweenAlpha>().Play(true);
			}
			else
			{
				TweenAlpha tweenAlpha = hongBaoQuanMinItem.transform.gameObject.GetComponent<TweenAlpha>();
				if (!tweenAlpha)
				{
					tweenAlpha = hongBaoQuanMinItem.transform.gameObject.AddComponent<TweenAlpha>();
				}
				tweenAlpha.enabled = true;
				tweenAlpha.from = 0f;
				tweenAlpha.to = 1f;
				tweenAlpha.duration = 0.5f;
				tweenAlpha.Play(true);
			}
			UIPanel component = hongBaoQuanMinItem.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
		this.m_ListBox.transform.localPosition = new Vector3(0f, 0f, -1f);
		if (this.m_Map[key].Count > 0)
		{
			this.m_ListBox.transform.localPosition = new Vector3(this.m_ListBox.transform.localPosition.x - (float)((this.m_Map[key].Count - 1) * 40), this.m_ListBox.transform.localPosition.y, -1f);
		}
		if (num == -1)
		{
			if (this.m_Day == key)
			{
				NGUITools.SetActive(this.m_GameZuanShis, false);
				NGUITools.SetActive(this.m_LabEndTitle.gameObject, true);
				NGUITools.SetActive(this.m_LabZuanShiTitle, false);
				this.m_LabEndTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("今日红包已经全部发放")
				}));
			}
			else if (this.m_Day < key)
			{
				if (this.m_Map.ContainsKey(this.m_Day + 1))
				{
					NGUITools.SetActive(this.m_GameZuanShis, true);
					NGUITools.SetActive(this.m_LabEndTitle.gameObject, false);
					NGUITools.SetActive(this.m_LabZuanShiTitle, true);
					this.m_LabNum.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						this.m_Map[this.m_Day + 1][0].RedPacketSize
					}));
				}
			}
			else if (this.m_Day > key)
			{
				NGUITools.SetActive(this.m_GameZuanShis, false);
				NGUITools.SetActive(this.m_LabEndTitle.gameObject, true);
				NGUITools.SetActive(this.m_LabZuanShiTitle, false);
				this.m_LabEndTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("今日红包已经全部发放")
				}));
			}
		}
	}

	private void StartTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer.Dispose();
			this.m_Timer = null;
		}
		this.m_Timer = new DispatcherTimer("HongBaoQuanMinPart_Timer");
		this.m_Timer.Interval = TimeSpan.FromSeconds(1.0);
		this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.m_Timer.Start();
	}

	private void StopTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer = null;
		}
	}

	private void Timer_Tick(object sender, object e)
	{
		if (this.m_OnDay < 1 || this.m_OnDay > this.m_MaxDay)
		{
			return;
		}
		if (this.m_Day == this.m_OnDay)
		{
			for (int i = 0; i < this.m_Map[this.m_Day].Count; i++)
			{
				int num = this.m_Map[this.m_OnDay][i].Time.Split(new char[]
				{
					':'
				})[0].SafeToInt32(0) * 3600 + this.m_Map[this.m_OnDay][i].Time.Split(new char[]
				{
					':'
				})[1].SafeToInt32(0) * 60;
				int num2 = Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second;
				if (num2 > num)
				{
					HongBaoQuanMinItem component = this.m_ObservableCollection.GetAt(i).GetComponent<HongBaoQuanMinItem>();
					if (component != null && !component.EndBool)
					{
						this.AddItem(this.m_OnDay);
					}
				}
			}
		}
	}

	public override void Destroy()
	{
		this.StopTimer();
		base.Destroy();
	}

	public UILabel m_LabTime1;

	public UILabel m_LabContent;

	public UILabel m_LabNum;

	public UILabel m_LabTimeToDay;

	public ListBox m_ListBox;

	public GButton m_BtnLeft;

	public GButton m_BtnRight;

	public UILabel m_LabZuanShiTitle;

	public UILabel m_LabEndTitle;

	public GameObject m_GameZuanShis;

	private ObservableCollection m_ObservableCollection;

	private Dictionary<int, List<RedPacketPeople>> m_Map = new Dictionary<int, List<RedPacketPeople>>();

	private int m_OnDay;

	private int m_MaxDay = -1;

	private int m_Day;

	private int m_Key = -1;

	private DispatcherTimer m_Timer;
}
