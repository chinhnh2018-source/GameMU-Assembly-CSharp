using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class HuoDongRiLiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		DateTime correctDateTime = Global.GetCorrectDateTime();
		this.m_Time = correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second;
		this.m_Weekday = Global.GetCorrectDateTime().DayOfWeek;
		if (Global.GetCorrectDateTime().DayOfWeek == null)
		{
			this.m_Weekday = 7;
		}
		this.OnBtns();
		this.InitXml("Config/EventCalendar.xml");
		this.curSelectWeekday = this.m_Weekday;
		this.RefreshList(this.curSelectWeekday);
		this.m_Scroll[this.curSelectWeekday - 1].gameObject.SetActive(true);
		this.m_Tab.SetActivePage(this.curSelectWeekday - 1);
		this.TabPniBool[this.curSelectWeekday - 1] = true;
		this.NewItemList();
	}

	private void NewItemList()
	{
		base.StartCoroutine(this.AddItem(this.curSelectWeekday));
	}

	private void NewItemListElse()
	{
		if (this._is_elseinit)
		{
			return;
		}
		this._is_elseinit = true;
		for (int i = 0; i < 7; i++)
		{
			if (i != this.curSelectWeekday - 1)
			{
				this.RefreshList(i + 1);
				base.StartCoroutine(this.AddItem(i + 1));
			}
			this.TabPniBool[i] = true;
		}
	}

	private void OnSelectTab(int index)
	{
		for (int i = 0; i < this.m_Scroll.Count; i++)
		{
			NGUITools.SetActive(this.m_Scroll[i], false);
			this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				this.TabBtnsColor[i]
			}));
			this.m_Tab.TabBtns[i].transform.localPosition = new Vector3(this.m_Tab.TabBtns[i].transform.localPosition.x, this.m_Tab.TabBtns[i].transform.localPosition.y, 0f);
			if (i == index)
			{
				this.m_Tab.TabBtns[i].transform.localPosition = new Vector3(this.m_Tab.TabBtns[i].transform.localPosition.x, this.m_Tab.TabBtns[i].transform.localPosition.y, -0.5f);
				this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					this.TabBtnsColor[i]
				}));
				NGUITools.SetActive(this.m_Scroll[i], true);
				if (!this.TabPniBool[i])
				{
					this.TabPniBool[i] = true;
					this.m_Tab.SetActivePage(i);
					this.m_Tab.TabPages[i].gameObject.SetActive(true);
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.StopTimer();
	}

	private void OnBtns()
	{
		for (int i = 0; i < this.m_Tab.TabBtns.Count; i++)
		{
			this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				this.TabBtnsColor[i]
			}));
		}
		this.m_Tab.TabBtns[this.m_Weekday - 1].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.TabBtnsColor[this.m_Weekday - 1]
		}));
		this.m_Tab.TabBtns[this.m_Weekday - 1].transform.localPosition = new Vector3(this.m_Tab.TabBtns[this.m_Weekday - 1].transform.localPosition.x, this.m_Tab.TabBtns[this.m_Weekday - 1].transform.localPosition.y, -0.5f);
		this.m_Tab.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
		{
			this.curSelectWeekday = s.ID + 1;
			this.OnSelectTab(s.ID);
			return true;
		};
	}

	private IEnumerator AddItem(int selectWeekday)
	{
		ObservableCollection itemSource = this.m_List[selectWeekday - 1].ItemsSource;
		itemSource.Clear();
		if (!Global.Data.HuoDongRiLiMap.ContainsKey(selectWeekday))
		{
			yield break;
		}
		Dictionary<int, EventCalendar> mps = Global.Data.HuoDongRiLiMap[selectWeekday];
		Dictionary<int, EventCalendar>.Enumerator mp = mps.GetEnumerator();
		Dictionary<int, int> m_ListLinkID = new Dictionary<int, int>();
		while (mp.MoveNext())
		{
			Dictionary<int, int> dictionary = m_ListLinkID;
			KeyValuePair<int, EventCalendar> keyValuePair = mp.Current;
			if (!dictionary.ContainsKey(keyValuePair.Value.LinkID))
			{
				Dictionary<int, int> dictionary2 = m_ListLinkID;
				KeyValuePair<int, EventCalendar> keyValuePair2 = mp.Current;
				int linkID = keyValuePair2.Value.LinkID;
				KeyValuePair<int, EventCalendar> keyValuePair3 = mp.Current;
				dictionary2.Add(linkID, keyValuePair3.Value.ID);
				HuoDongRiLiItem item = U3DUtils.NEW<HuoDongRiLiItem>();
				itemSource.AddNoUpdate(item);
				HuoDongRiLiItem huoDongRiLiItem = item;
				KeyValuePair<int, EventCalendar> keyValuePair4 = mp.Current;
				huoDongRiLiItem.Data = keyValuePair4.Value;
				NGUITools.SetActive(this.m_Tab.TabPages[selectWeekday - 1], true);
				UIPanel p = item.GetComponentInChildren<UIPanel>();
				if (p != null)
				{
					Object.Destroy(p);
				}
				UIDragPanelContents dragPanelContents = item.GetComponentInChildren<UIDragPanelContents>();
				if (dragPanelContents != null && this.m_Tab.TabPages[selectWeekday - 1].GetComponentInChildren<UIDraggablePanel>() != null)
				{
					dragPanelContents.draggablePanel = this.m_Tab.TabPages[selectWeekday - 1].GetComponentInChildren<UIDraggablePanel>();
				}
				ObservableCollection boxOBC = item.m_List.ItemsSource;
				boxOBC.Clear();
				KeyValuePair<int, EventCalendar> keyValuePair5 = mp.Current;
				if (!keyValuePair5.Value.EventAward.Equals(string.Empty))
				{
					string goodsid = string.Empty;
					KeyValuePair<int, EventCalendar> keyValuePair6 = mp.Current;
					string[] goodsids = keyValuePair6.Value.EventAward.Split(new char[]
					{
						','
					});
					for (int i = 0; i < goodsids.Length; i++)
					{
						if (i >= goodsids.Length - 1)
						{
							goodsid = goodsid + goodsids[i] + ",0,0,0,0,0,0";
						}
						else
						{
							goodsid = goodsid + goodsids[i] + ",0,0,0,0,0,0|";
						}
					}
					Super.LoadGoodsList(goodsid, boxOBC);
					GGoodIcon[] lists = item.m_List.GetComponentsInChildren<GGoodIcon>();
					for (int y = 0; y < lists.Length; y++)
					{
						if (lists[y].GetComponentInChildren<UIPanel>() != null)
						{
							Object.Destroy(lists[y].GetComponentInChildren<UIPanel>());
						}
						UILabel[] listLab = item.m_List.GetComponentsInChildren<UILabel>();
						for (int z = 0; z < listLab.Length; z++)
						{
							Object.Destroy(listLab[z].gameObject);
						}
					}
				}
				item.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 0)
					{
						this.GoBtn(e.Index);
					}
					else if (e.ID != 1)
					{
						if (e.ID == 2)
						{
						}
					}
				};
				if (selectWeekday != this.m_Weekday)
				{
					NGUITools.SetActive(this.m_Tab.TabPages[selectWeekday - 1], false);
				}
				yield return null;
			}
		}
		NGUITools.SetActive(this.mZheZhaoObj, false);
		Super.HideNetWaiting();
		this.NewItemListElse();
		yield return null;
		yield break;
	}

	private void InitXml(string XmlData = "Config/EventCalendar.xml")
	{
		if (Global.GetGameResXml(XmlData) == null)
		{
			return;
		}
		if (Global.Data.HuoDongRiLiMap.Count <= 0)
		{
			this.RefreshXml("Config/EventCalendar.xml");
		}
		this.StartTimer();
	}

	public override void Destroy()
	{
		this.RefreshXml("Config/EventCalendar.xml");
		base.Destroy();
	}

	private void RefreshXml(string XmlData = "Config/EventCalendar.xml")
	{
		Global.Data.HuoDongRiLiMap.Clear();
		XElement gameResXml = Global.GetGameResXml(XmlData);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "EventCalendar");
		if (xelementList == null)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			Dictionary<int, EventCalendar> dictionary = new Dictionary<int, EventCalendar>();
			EventCalendar eventCalendar = new EventCalendar();
			eventCalendar.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			eventCalendar.Weekday = Global.GetXElementAttributeInt(xelementList[i], "Weekday");
			eventCalendar.Level = Global.GetXElementAttributeStr(xelementList[i], "Level");
			eventCalendar.CompletedTaskID = Global.GetXElementAttributeInt(xelementList[i], "CompletedTaskID");
			eventCalendar.VipLevel = Global.GetXElementAttributeInt(xelementList[i], "VipLevel");
			eventCalendar.Time = Global.GetXElementAttributeStr(xelementList[i], "Time").Split(new char[]
			{
				'|'
			});
			eventCalendar.EventName = Global.GetXElementAttributeStr(xelementList[i], "EventName");
			eventCalendar.EventAward = Global.GetXElementAttributeStr(xelementList[i], "EventAward");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[i], "LinkID");
			eventCalendar.LinkID = xelementAttributeStr.SafeToInt32(0);
			if (!dictionary.ContainsKey(eventCalendar.ID))
			{
				dictionary.Add(eventCalendar.ID, eventCalendar);
			}
			else
			{
				dictionary[eventCalendar.ID] = eventCalendar;
			}
			if (!Global.Data.HuoDongRiLiMap.ContainsKey(eventCalendar.Weekday))
			{
				Global.Data.HuoDongRiLiMap.Add(eventCalendar.Weekday, dictionary);
			}
			else if (!Global.Data.HuoDongRiLiMap[eventCalendar.Weekday].ContainsKey(eventCalendar.ID))
			{
				Global.Data.HuoDongRiLiMap[eventCalendar.Weekday].Add(eventCalendar.ID, eventCalendar);
			}
		}
	}

	private void RefresData()
	{
		foreach (KeyValuePair<int, Dictionary<int, EventCalendar>> keyValuePair in Global.Data.HuoDongRiLiMap)
		{
			this.RefreshList(keyValuePair.Key);
		}
	}

	private void RefreshList(int Weekday)
	{
		UIScrollBar component = this.m_Scroll[Weekday - 1].GetComponent<UIScrollBar>();
		if (component != null)
		{
			component.scrollValue = 0f;
		}
		NGUITools.SetActive(this.mZheZhaoObj, true);
		if (Weekday == this.m_Weekday)
		{
			this.m_TimerIDList.Clear();
			this.m_TimerList.Clear();
			this.m_KeyList.Clear();
		}
		List<EventCalendar> list = new List<EventCalendar>();
		List<EventCalendar> list2 = new List<EventCalendar>();
		List<EventCalendar> list3 = new List<EventCalendar>();
		Dictionary<int, EventCalendar>.Enumerator enumerator = Global.Data.HuoDongRiLiMap[Weekday].GetEnumerator();
		while (enumerator.MoveNext())
		{
			int viplevel = Global.Data.roleData.VIPLevel;
			KeyValuePair<int, EventCalendar> keyValuePair = enumerator.Current;
			if (viplevel >= keyValuePair.Value.VipLevel)
			{
				int completedMainTaskID = Global.Data.roleData.CompletedMainTaskID;
				KeyValuePair<int, EventCalendar> keyValuePair2 = enumerator.Current;
				if (completedMainTaskID >= keyValuePair2.Value.CompletedTaskID)
				{
					KeyValuePair<int, EventCalendar> keyValuePair3 = enumerator.Current;
					string[] array = keyValuePair3.Value.Level.Split(new char[]
					{
						','
					});
					if (array.Length == 2)
					{
						if (Global.Data.roleData.ChangeLifeCount < array[0].SafeToInt32(0))
						{
							continue;
						}
						if (Global.Data.roleData.ChangeLifeCount == array[0].SafeToInt32(0) && Global.Data.roleData.Level < array[1].SafeToInt32(0))
						{
							continue;
						}
					}
					KeyValuePair<int, EventCalendar> keyValuePair4 = enumerator.Current;
					string[] time = keyValuePair4.Value.Time;
					if (Weekday > this.m_Weekday)
					{
						string beginTime = time[0].Split(new char[]
						{
							'-'
						})[0];
						string endTime = time[0].Split(new char[]
						{
							'-'
						})[1];
						KeyValuePair<int, EventCalendar> keyValuePair5 = enumerator.Current;
						keyValuePair5.Value.BeginTime = beginTime;
						KeyValuePair<int, EventCalendar> keyValuePair6 = enumerator.Current;
						keyValuePair6.Value.EndTime = endTime;
						KeyValuePair<int, EventCalendar> keyValuePair7 = enumerator.Current;
						keyValuePair7.Value.Key = 1;
						List<EventCalendar> list4 = list2;
						KeyValuePair<int, EventCalendar> keyValuePair8 = enumerator.Current;
						list4.Add(keyValuePair8.Value);
					}
					else if (Weekday < this.m_Weekday)
					{
						string beginTime2 = time[0].Split(new char[]
						{
							'-'
						})[0];
						string endTime2 = time[0].Split(new char[]
						{
							'-'
						})[1];
						KeyValuePair<int, EventCalendar> keyValuePair9 = enumerator.Current;
						keyValuePair9.Value.BeginTime = beginTime2;
						KeyValuePair<int, EventCalendar> keyValuePair10 = enumerator.Current;
						keyValuePair10.Value.EndTime = endTime2;
						KeyValuePair<int, EventCalendar> keyValuePair11 = enumerator.Current;
						keyValuePair11.Value.Key = 2;
						List<EventCalendar> list5 = list3;
						KeyValuePair<int, EventCalendar> keyValuePair12 = enumerator.Current;
						list5.Add(keyValuePair12.Value);
					}
					else
					{
						for (int i = 0; i < time.Length; i++)
						{
							if (time[i].Split(new char[]
							{
								'-'
							}).Length <= 1)
							{
								return;
							}
							string text = time[i].Split(new char[]
							{
								'-'
							})[0];
							string text2 = time[i].Split(new char[]
							{
								'-'
							})[1];
							int num = int.Parse(text.Split(new char[]
							{
								':'
							})[0]) * 3600 + int.Parse(text.Split(new char[]
							{
								':'
							})[1]) * 60;
							int num2 = int.Parse(text2.Split(new char[]
							{
								':'
							})[0]) * 3600 + int.Parse(text2.Split(new char[]
							{
								':'
							})[1]) * 60;
							if (this.m_Time >= num && this.m_Time <= num2)
							{
								KeyValuePair<int, EventCalendar> keyValuePair13 = enumerator.Current;
								keyValuePair13.Value.Key = 0;
								List<EventCalendar> list6 = list;
								KeyValuePair<int, EventCalendar> keyValuePair14 = enumerator.Current;
								list6.Add(keyValuePair14.Value);
								if (Weekday == this.m_Weekday)
								{
									KeyValuePair<int, EventCalendar> keyValuePair15 = enumerator.Current;
									keyValuePair15.Value.BeginTime = text;
									KeyValuePair<int, EventCalendar> keyValuePair16 = enumerator.Current;
									keyValuePair16.Value.EndTime = text2;
									List<int> timerIDList = this.m_TimerIDList;
									KeyValuePair<int, EventCalendar> keyValuePair17 = enumerator.Current;
									timerIDList.Add(keyValuePair17.Value.ID);
									List<string> timerList = this.m_TimerList;
									object[] array2 = new object[5];
									array2[0] = num;
									array2[1] = ",";
									array2[2] = num2;
									array2[3] = ",";
									int num3 = 4;
									KeyValuePair<int, EventCalendar> keyValuePair18 = enumerator.Current;
									array2[num3] = keyValuePair18.Value.Key;
									timerList.Add(string.Concat(array2));
									List<int> keyList = this.m_KeyList;
									KeyValuePair<int, EventCalendar> keyValuePair19 = enumerator.Current;
									keyList.Add(keyValuePair19.Value.Key);
								}
								break;
							}
							if (this.m_Time < num)
							{
								KeyValuePair<int, EventCalendar> keyValuePair20 = enumerator.Current;
								keyValuePair20.Value.Key = 1;
								KeyValuePair<int, EventCalendar> keyValuePair21 = enumerator.Current;
								keyValuePair21.Value.BeginTime = text;
								KeyValuePair<int, EventCalendar> keyValuePair22 = enumerator.Current;
								keyValuePair22.Value.EndTime = text2;
								List<EventCalendar> list7 = list2;
								KeyValuePair<int, EventCalendar> keyValuePair23 = enumerator.Current;
								list7.Add(keyValuePair23.Value);
								if (Weekday == this.m_Weekday)
								{
									List<int> timerIDList2 = this.m_TimerIDList;
									KeyValuePair<int, EventCalendar> keyValuePair24 = enumerator.Current;
									timerIDList2.Add(keyValuePair24.Value.ID);
									List<string> timerList2 = this.m_TimerList;
									object[] array3 = new object[5];
									array3[0] = num;
									array3[1] = ",";
									array3[2] = num2;
									array3[3] = ",";
									int num4 = 4;
									KeyValuePair<int, EventCalendar> keyValuePair25 = enumerator.Current;
									array3[num4] = keyValuePair25.Value.Key;
									timerList2.Add(string.Concat(array3));
									List<int> keyList2 = this.m_KeyList;
									KeyValuePair<int, EventCalendar> keyValuePair26 = enumerator.Current;
									keyList2.Add(keyValuePair26.Value.Key);
								}
								break;
							}
							KeyValuePair<int, EventCalendar> keyValuePair27 = enumerator.Current;
							keyValuePair27.Value.BeginTime = text;
							KeyValuePair<int, EventCalendar> keyValuePair28 = enumerator.Current;
							keyValuePair28.Value.EndTime = text2;
							KeyValuePair<int, EventCalendar> keyValuePair29 = enumerator.Current;
							keyValuePair29.Value.Key = 2;
							List<EventCalendar> list8 = list3;
							KeyValuePair<int, EventCalendar> keyValuePair30 = enumerator.Current;
							list8.Add(keyValuePair30.Value);
						}
					}
				}
			}
		}
		List<EventCalendar> list9 = new List<EventCalendar>();
		list9.AddRange(this.RefreshListData(list));
		list9.AddRange(this.RefreshListData(list2));
		list9.AddRange(list3);
		Global.Data.HuoDongRiLiMap[Weekday].Clear();
		for (int j = 0; j < list9.Count; j++)
		{
			if (!Global.Data.HuoDongRiLiMap[Weekday].ContainsKey(list9[j].ID))
			{
				Global.Data.HuoDongRiLiMap[Weekday].Add(list9[j].ID, list9[j]);
			}
		}
	}

	public List<EventCalendar> RefreshListData(List<EventCalendar> list)
	{
		List<EventCalendar> list2 = new List<EventCalendar>();
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			EventCalendar eventCalendar = new EventCalendar();
			eventCalendar = list[0];
			int num = int.Parse(list[0].BeginTime.Split(new char[]
			{
				':'
			})[0]) * 3600 + int.Parse(list[0].BeginTime.Split(new char[]
			{
				':'
			})[1]) * 60;
			int num2 = int.Parse(list[0].EndTime.Split(new char[]
			{
				':'
			})[0]) * 3600 + int.Parse(list[0].EndTime.Split(new char[]
			{
				':'
			})[1]) * 60;
			int num3 = 0;
			for (int j = 0; j < list.Count; j++)
			{
				int num4 = int.Parse(list[j].BeginTime.Split(new char[]
				{
					':'
				})[0]) * 3600 + int.Parse(list[j].BeginTime.Split(new char[]
				{
					':'
				})[1]) * 60;
				int num5 = int.Parse(list[j].EndTime.Split(new char[]
				{
					':'
				})[0]) * 3600 + int.Parse(list[j].EndTime.Split(new char[]
				{
					':'
				})[1]) * 60;
				if (num > num4)
				{
					eventCalendar = list[j];
					num3 = j;
					num = int.Parse(eventCalendar.BeginTime.Split(new char[]
					{
						':'
					})[0]) * 3600 + int.Parse(eventCalendar.BeginTime.Split(new char[]
					{
						':'
					})[1]) * 60;
					num2 = int.Parse(eventCalendar.EndTime.Split(new char[]
					{
						':'
					})[0]) * 3600 + int.Parse(eventCalendar.EndTime.Split(new char[]
					{
						':'
					})[1]) * 60;
				}
				else if (num == num4 && num2 > num5)
				{
					eventCalendar = list[j];
					num3 = j;
					num = int.Parse(eventCalendar.BeginTime.Split(new char[]
					{
						':'
					})[0]) * 3600 + int.Parse(eventCalendar.BeginTime.Split(new char[]
					{
						':'
					})[1]) * 60;
					num2 = int.Parse(eventCalendar.EndTime.Split(new char[]
					{
						':'
					})[0]) * 3600 + int.Parse(eventCalendar.EndTime.Split(new char[]
					{
						':'
					})[1]) * 60;
				}
			}
			list2.Add(eventCalendar);
			list.Remove(list[num3]);
		}
		return list2;
	}

	private void GoBtn(int LinkID)
	{
		PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
		{
			ID = LinkID
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
		this.m_Timer = new DispatcherTimer("HuoDongRiLiPart_Timer");
		this.m_Timer.Interval = TimeSpan.FromSeconds(1.0);
		this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.m_Timer.Start();
	}

	private void Timer_Tick(object sender, object e)
	{
		if (this.mZheZhaoObj.activeSelf)
		{
			return;
		}
		this.m_Time++;
		if (this.curSelectWeekday != this.m_Weekday)
		{
			return;
		}
		for (int i = 0; i < this.m_TimerList.Count; i++)
		{
			if (this.m_Time < this.m_TimerList[i].Split(new char[]
			{
				','
			})[0].SafeToInt32(0))
			{
				if (this.m_KeyList[i] != 1)
				{
					Global.Data.HuoDongRiLiMap[this.m_Weekday][this.m_TimerIDList[i]].Key = 1;
					this.RefreshList(this.m_Weekday);
					this.NewItemList();
				}
			}
			else if (this.m_Time >= this.m_TimerList[i].Split(new char[]
			{
				','
			})[0].SafeToInt32(0) && this.m_Time <= this.m_TimerList[i].Split(new char[]
			{
				','
			})[1].SafeToInt32(0))
			{
				if (this.m_KeyList[i] != 0)
				{
					Global.Data.HuoDongRiLiMap[this.m_Weekday][this.m_TimerIDList[i]].Key = 1;
					this.RefreshList(this.m_Weekday);
					this.NewItemList();
				}
			}
			else if (this.m_KeyList[i] != 2)
			{
				Global.Data.HuoDongRiLiMap[this.m_Weekday][this.m_TimerIDList[i]].Key = 1;
				this.RefreshList(this.m_Weekday);
				this.NewItemList();
			}
		}
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

	private new void Update()
	{
		for (int i = 0; i < this.m_Tab.TabPages.Count; i++)
		{
			GameObject gameObject = this.m_Tab.TabPages[i].gameObject;
			if (i == this.curSelectWeekday - 1)
			{
				gameObject.gameObject.SetActive(true);
			}
			else
			{
				gameObject.gameObject.SetActive(false);
			}
		}
	}

	public GTab m_Tab;

	public List<ListBox> m_List = new List<ListBox>();

	public List<GameObject> m_Scroll = new List<GameObject>();

	public GameObject mZheZhaoObj;

	private DispatcherTimer m_Timer;

	private int m_Time = -1;

	private string[] TabBtnsColor = new string[]
	{
		Global.GetLang("周一"),
		Global.GetLang("周二"),
		Global.GetLang("周三"),
		Global.GetLang("周四"),
		Global.GetLang("周五"),
		Global.GetLang("周六"),
		Global.GetLang("周日")
	};

	private bool[] TabPniBool = new bool[7];

	private int m_Weekday = -1;

	private List<int> m_TimerIDList = new List<int>();

	private List<string> m_TimerList = new List<string>();

	private List<int> m_KeyList = new List<int>();

	private int curSelectWeekday;

	private bool _is_elseinit;
}
