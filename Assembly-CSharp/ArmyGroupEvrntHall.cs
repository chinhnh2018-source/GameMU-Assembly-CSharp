using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmyGroupEvrntHall : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitData();
	}

	private void InitData()
	{
		GameInstance.Game.SendGetArmyGroupJionEventLogList();
	}

	private IEnumerator RefreshUI(List<string[]> list)
	{
		Super.ShowNetWaiting(null);
		yield return new WaitForSeconds(0.6f);
		for (int i = 0; i < list.Count; i++)
		{
			if (i % 2 == 0 && i != 0)
			{
				yield return null;
			}
			ArmyGropListItem item = U3DUtils.NEW<ArmyGropListItem>();
			string[] comtent = new string[]
			{
				list[i][0],
				list[i][1]
			};
			item.SetContent(comtent, ArmyGropListItem.ItemUIType.Envent);
			item.Index = i;
			this.OBC.Add(item);
			item.DraggablePanel = this.DraggablePanel;
		}
		if (this.bFirstRefresh == 0)
		{
			yield return null;
			this.DraggablePanel.Press(false);
			this.bFirstRefresh = 1;
		}
		if (4 > list.Count)
		{
			Vector3 pos = this.ListBox.transform.localPosition;
			pos.y = 100f;
			this.ListBox.transform.localPosition = pos;
		}
		Super.HideNetWaiting();
		yield break;
	}

	private void ListBoxSelectChange(object sender, MouseEvent e)
	{
		GameObject selectedItem = this.ListBox.SelectedItem;
		if (null != selectedItem)
		{
			ArmyGropListItem component = selectedItem.GetComponent<ArmyGropListItem>();
			if (null != component)
			{
				component.BSelect = true;
			}
		}
		GameObject lastSelectedItem = this.ListBox.LastSelectedItem;
		if (null != lastSelectedItem && lastSelectedItem != selectedItem)
		{
			ArmyGropListItem component2 = lastSelectedItem.GetComponent<ArmyGropListItem>();
			if (null != component2)
			{
				component2.BSelect = false;
			}
		}
	}

	private void ItemClickHander(object sender, DPSelectedItemEventArgs args)
	{
	}

	private void InitPrefabText()
	{
		this.TitleTitleClase[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("发生时间")
		});
		this.TitleTitleClase[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("事件内容")
		});
	}

	private void InitTexture()
	{
		this.BgTexture.URL = "NetImages/GameRes/Images/ArmyGroup/ArmyGroupCreatBg1.jpg";
	}

	private void InitHandler()
	{
		this.OBC = this.ListBox.ItemsSource;
		this.DraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.ViewOnDragFinished);
		this.CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			Super.HideNetWaiting();
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	private void ViewOnDragFinished()
	{
		bool flag = false;
		bool flag2 = false;
		this.DraggablePanel.IsOnTopOrBottom(ref flag, ref flag2);
		if (flag2 && 0 < this.mLstJunTuanEventLog.Count)
		{
			this.RefreshEventUI(null);
		}
	}

	private string GetTime(DateTime time)
	{
		string empty = string.Empty;
		string text = (9 >= time.Month) ? ("0" + time.Month.ToString()) : time.Month.ToString();
		string text2 = (9 >= time.Day) ? ("0" + time.Day.ToString()) : time.Day.ToString();
		string text3 = (9 >= time.Hour) ? ("0" + time.Hour.ToString()) : time.Hour.ToString();
		string text4 = (9 >= time.Minute) ? ("0" + time.Minute.ToString()) : time.Minute.ToString();
		return string.Format("{0}-{1}-{2} {3}:{4}", new object[]
		{
			time.Year,
			text,
			text2,
			text3,
			text4
		});
	}

	private string GetEventStr(int index, string message)
	{
		if (index == 0)
		{
			if (string.IsNullOrEmpty(message))
			{
				return string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("军团创建成功")
					})
				}));
			}
			return string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("【{0}】"), message)
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("军团创建成功")
			}));
		}
		else
		{
			if (index == 1)
			{
				return string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("【{0}】"), message)
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("战盟加入军团")
				}));
			}
			if (index == 2)
			{
				return string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("【{0}】"), message)
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("被任命为团长")
				}));
			}
			if (index == 3)
			{
				return string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("【{0}】"), message)
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("战盟退出军团")
				}));
			}
			if (index == 4)
			{
				return string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("【{0}】"), message)
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("任务完成")
				}));
			}
			if (index == 5)
			{
				return string.Format("{1}{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("【{0}】"), this.GetLingDiName(message.SafeToInt32(0)))
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("成功占领")
				}));
			}
			return string.Empty;
		}
	}

	private string GetLingDiName(int index)
	{
		string result = string.Empty;
		switch (index)
		{
		case 0:
			result = Global.GetLang("无");
			break;
		case 1:
			result = Global.GetLang("阿卡伦西");
			break;
		case 2:
			result = Global.GetLang("阿卡伦东");
			break;
		case 3:
			result = Global.GetLang("全境");
			break;
		}
		return result;
	}

	public void RefreshEventUI(List<JunTuanEventLog> data)
	{
		List<string[]> list = new List<string[]>();
		if (data != null)
		{
			data.Sort((JunTuanEventLog b, JunTuanEventLog a) => (0L <= a.Time.Ticks - b.Time.Ticks) ? 0 : -1);
			if (0 >= this.mLstJunTuanEventLog.Count)
			{
				for (int i = 0; i < data.Count; i++)
				{
					string[] array = new string[]
					{
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							this.GetTime(data[i].Time)
						}),
						this.GetEventStr(data[i].EventType, data[i].Message)
					};
					this.mLstJunTuanEventLog.Add(array);
				}
			}
		}
		if (0 < this.mLstJunTuanEventLog.Count)
		{
			int num = this.mLstJunTuanEventLog.Count;
			if (10 < num)
			{
				num = 10;
			}
			byte b3 = 0;
			while ((int)b3 < num)
			{
				list.Add(this.mLstJunTuanEventLog[(int)b3]);
				b3 += 1;
			}
			byte b2 = 0;
			while ((int)b2 < num)
			{
				this.mLstJunTuanEventLog.RemoveAt(0);
				b2 += 1;
			}
		}
		if (list.Count == 0)
		{
			this.DraggablePanel.showScrollBars = 1;
		}
		else
		{
			this.DraggablePanel.showScrollBars = 0;
		}
		base.StartCoroutine<bool>(this.RefreshUI(list));
	}

	public ShowNetImage BgTexture;

	public UILabel[] TitleTitleClase;

	public ListBox ListBox;

	public UIDraggablePanel DraggablePanel;

	public GButton CloseBtn;

	private ObservableCollection OBC;

	private List<string[]> mLstJunTuanEventLog = new List<string[]>();

	private byte bFirstRefresh;

	public DPSelectedItemEventHandler Hander;
}
