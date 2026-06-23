using System;
using System.Collections.Generic;
using GameServer.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class JinRiKeZuoPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		XElement isolateResXml = Global.GetIsolateResXml("JinRiKeZuoTab");
		this.TabFileList = Global.GetXElementList(isolateResXml, "MeiRiBiZuo");
		this.InitBtnProc();
		this.m_GiftItemOBC = this.m_ListGiftItems.ItemsSource;
		this.SetUIState(0);
	}

	private void TabBtnSelectChange(object sender, object e)
	{
		if (this.m_ListTabBtn.SelectedIndex >= 0)
		{
			this.SetUIState(this.m_ListTabBtn.SelectedIndex);
		}
	}

	private void InitBtnProc()
	{
		this.m_TabBtnOBC = this.m_ListTabBtn.ItemsSource;
		int count = this.TabFileList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.TabFileList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
			BtnObjSecond btnObjSecond = U3DUtils.NEW<BtnObjSecond>();
			btnObjSecond.m_BtnItem.Label.text = xelementAttributeStr;
			this.m_TabBtnOBC.AddNoUpdate(btnObjSecond);
		}
		this.m_TabBtnOBC.DelayUpdate();
		this.m_ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TabBtnSelectChange);
	}

	public void SetUIState(int nIndex)
	{
		this.m_PnlGift.transform.localPosition = new Vector3(74f, 73f, 0f);
		UIPanel component = this.m_PnlGift.GetComponent<UIPanel>();
		if (component != null)
		{
			component.clipRange = new Vector4(0f, -130f, 780f, 384f);
		}
		this.SetTabBtnIndex(nIndex);
		GameInstance.Game.QueryTodayCandoInfo(nIndex + 1);
		Super.ShowNetWaiting(string.Empty);
	}

	public void SetBtnActieve(GButton btn)
	{
		if (null != btn)
		{
			if (btn == this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.Label.color = NGUIMath.HexToColorEx(15790320U);
				return;
			}
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnLastSelect = this.m_BtnCurrentSelect;
				this.m_BtnCurrentSelect = btn;
			}
			this.m_BtnCurrentSelect = btn;
			if (null != this.m_BtnCurrentSelect)
			{
				this.m_BtnCurrentSelect.Label.color = NGUIMath.HexToColorEx(15790320U);
				this.m_BtnCurrentSelect.normalSprite = "btn_left_selected";
				this.m_BtnCurrentSelect.Refresh();
			}
			if (null != this.m_BtnLastSelect)
			{
				this.m_BtnLastSelect.Label.color = NGUIMath.HexToColorEx(10323559U);
				this.m_BtnLastSelect.normalSprite = "btn_left_normal";
				this.m_BtnLastSelect.Refresh();
			}
		}
	}

	private void SetTabBtnIndex(int nIndex)
	{
		this.m_nTabBtnIndex = nIndex;
		GameObject at = this.m_TabBtnOBC.GetAt(nIndex);
		BtnObjSecond component = at.GetComponent<BtnObjSecond>();
		this.SetBtnActieve(component.m_BtnItem);
	}

	public void RefreshByServerData(List<TodayCandoData> dataList)
	{
		this.m_GiftItemOBC.Clear();
		if (dataList != null)
		{
			int count = dataList.Count;
			for (int i = 0; i < count; i++)
			{
				JinRiKeZuoPartGiftItem jinRiKeZuoPartGiftItem = U3DUtils.NEW<JinRiKeZuoPartGiftItem>();
				jinRiKeZuoPartGiftItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				TodayCandoData todayCandoData = dataList[i];
				jinRiKeZuoPartGiftItem.RefreshByID(todayCandoData.ID, todayCandoData.LeftCount);
				this.m_GiftItemOBC.AddNoUpdate(jinRiKeZuoPartGiftItem);
			}
		}
		if (this.m_GiftItemOBC.Count == 0)
		{
			JinRiKeZuoPartGiftItem jinRiKeZuoPartGiftItem2 = U3DUtils.NEW<JinRiKeZuoPartGiftItem>();
			jinRiKeZuoPartGiftItem2.RefreshByID(-1, 0);
			this.m_GiftItemOBC.AddNoUpdate(jinRiKeZuoPartGiftItem2);
		}
	}

	public GameObject m_Btn;

	public GameObject m_PnlGift;

	private ObservableCollection m_TabBtnOBC;

	public ListBox m_ListTabBtn = new ListBox();

	private ObservableCollection m_GiftItemOBC;

	public ListBox m_ListGiftItems = new ListBox();

	public GButton m_BtnLastSelect;

	public GButton m_BtnCurrentSelect;

	public int m_nTabBtnIndex;

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<XElement> TabFileList;
}
