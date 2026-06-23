using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class LoveFuBenPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.StartBtn.Text = Global.GetLang("准备");
		this.ObsCollection = this.FubenList.ItemsSource;
		this.FubenList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxChanged);
		this.StartBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrFuBenItem.CanReady())
			{
				Super.ShowNetWaiting(string.Empty);
				GameInstance.Game.SpriteLoveFuben((!this.m_Ready) ? 4 : 5, this.CurrFuBenItem.CopyId);
			}
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_Ready)
			{
				this.ShowAlertDialog(1);
			}
			else
			{
				GameInstance.Game.SpriteLoveFuben(3, 0);
				PlayZone.GlobalPlayZone.CloseLoveFuBenWindow();
				PlayZone.GlobalPlayZone.OpenMarryLoveTockenPart();
			}
		};
		this.InitFubenConf();
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.SpriteLoveFuben(2, 0);
	}

	private void InitFubenConf()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		int num = 0;
		foreach (XElement xelement in xelementList)
		{
			if (Global.GetXElementAttributeInt(xelement, "FuBenUse") == 11)
			{
				LoveFuBenItem loveFuBenItem = U3DUtils.NEW<LoveFuBenItem>();
				loveFuBenItem.XmlItem = xelement;
				loveFuBenItem.SetTitle();
				loveFuBenItem.SetMyState(0);
				loveFuBenItem.SetOtherState(0);
				this.FubenItems.Add(loveFuBenItem.CopyId, loveFuBenItem);
				this.ObsCollection.AddNoUpdate(loveFuBenItem);
				this.ObsCollection.DelayUpdate();
				GameInstance.Game.SpriteQureyFuBenInfo(loveFuBenItem.MapCode, loveFuBenItem.CopyId);
				num++;
				UIPanel component = loveFuBenItem.GetComponent<UIPanel>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
		}
		this.FubenList.SelectedIndex = 0;
		if (num > 1)
		{
			this.LeftIcon.SetActive(true);
			this.RightIcon.SetActive(true);
		}
		else
		{
			this.LeftIcon.SetActive(false);
			this.RightIcon.SetActive(false);
		}
	}

	public void SetDupState(int m_dupId, int m_state, int otherDupId, int otherState, bool sendMsg, int myRoleId, int otherRoleId)
	{
		if (this.FubenItems.ContainsKey(m_dupId))
		{
			this.m_FubenId = m_dupId;
			this.m_State = m_state;
			this.FubenItems[m_dupId].SetMyState(m_state);
		}
		else
		{
			foreach (LoveFuBenItem loveFuBenItem in this.FubenItems.Values)
			{
				loveFuBenItem.SetMyState(m_state);
			}
			this.m_State = m_state;
		}
		if (m_state >= 1)
		{
			this.m_Ready = true;
		}
		else
		{
			this.m_Ready = false;
		}
		if (this.m_Ready)
		{
			this.StartBtn.Text = Global.GetLang("取消");
		}
		else
		{
			this.StartBtn.Text = Global.GetLang("准备");
		}
		if (this.FubenItems.ContainsKey(otherDupId))
		{
			this.other_FubenId = otherDupId;
			this.other_State = otherState;
			this.FubenItems[otherDupId].SetOtherState(otherState);
		}
		else
		{
			foreach (LoveFuBenItem loveFuBenItem2 in this.FubenItems.Values)
			{
				loveFuBenItem2.SetOtherState(otherState);
			}
			this.other_State = otherState;
		}
		if (this.m_FubenId == this.other_FubenId && this.m_State == 1 && this.other_State == 1 && sendMsg)
		{
			GameInstance.Game.SpriteEnterFuBen(this.m_FubenId);
		}
	}

	public void NotifyFuBenTime(int dupId, int finishNum)
	{
		if (this.FubenItems.ContainsKey(dupId))
		{
			this.FubenItems[dupId].SetLeftNum(finishNum);
		}
	}

	private void ListBoxChanged(object sender, MouseEvent e)
	{
		LoveFuBenItem currFuBenItem = U3DUtils.AS<LoveFuBenItem>(this.FubenList.SelectedItem);
		this.CurrFuBenItem = currFuBenItem;
		if (this.m_Ready)
		{
			this.ShowAlertDialog(0);
		}
	}

	private void ShowAlertDialog(int type)
	{
		string lang = Global.GetLang("是否确认取消当前副本准备状态？");
		string[] buttons = new string[]
		{
			Global.GetLang("确定")
		};
		Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
		{
			if (e1.ID == 0)
			{
				if (type == 0)
				{
					if (this.m_Ready)
					{
						Super.ShowNetWaiting(string.Empty);
						GameInstance.Game.SpriteLoveFuben(5, this.CurrFuBenItem.CopyId);
					}
				}
				else if (type == 1)
				{
					GameInstance.Game.SpriteLoveFuben(3, 0);
					PlayZone.GlobalPlayZone.CloseLoveFuBenWindow();
					PlayZone.GlobalPlayZone.OpenMarryLoveTockenPart();
				}
			}
		}, buttons);
	}

	public ListBox FubenList;

	public GButton StartBtn;

	public GButton CloseBtn;

	private ObservableCollection ObsCollection;

	private bool m_Ready;

	private int m_State;

	private int m_FubenId;

	private int other_State;

	private int other_FubenId;

	public GameObject LeftIcon;

	public GameObject RightIcon;

	private LoveFuBenItem CurrFuBenItem;

	private Dictionary<int, LoveFuBenItem> FubenItems = new Dictionary<int, LoveFuBenItem>();
}
