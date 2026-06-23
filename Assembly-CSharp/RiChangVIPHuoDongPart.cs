using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RiChangVIPHuoDongPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._TaskList.ItemsSource;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._TaskList.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this._TaskList.ItemPerPage = 4;
		this._PageRadio.ItemPerPage = this.ItemPerPage;
		this._PageRadio.ItemCount = 0;
	}

	public void InitPartData(ActivityCategorys category)
	{
		XElement gameResXml = Global.GetGameResXml("Config/VipTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<RiChangHuoDongItem> list = new List<RiChangHuoDongItem>();
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "VipHuoDong");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			RiChangHuoDongItem riChangHuoDongItem = U3DUtils.NEW<RiChangHuoDongItem>();
			riChangHuoDongItem.Width = 205.0;
			riChangHuoDongItem.Height = 273.0;
			riChangHuoDongItem.name = this.ItemCollection.Count.ToString("0000");
			this.ItemCollection.AddNoUpdate(riChangHuoDongItem);
			riChangHuoDongItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			riChangHuoDongItem.m_strConfigPath = Global.GetXElementAttributeStr(xelement, "GLXml");
			riChangHuoDongItem.ItemName = Global.GetXElementAttributeStr(xelement, "Name");
			riChangHuoDongItem.ActivityCategory = category;
			riChangHuoDongItem.Init();
			riChangHuoDongItem.TabID = xelementAttributeInt;
			riChangHuoDongItem.HuoDongType = RiChangHuoDongTypes.VIPHuoDongBaseIndex + xelementAttributeInt;
			riChangHuoDongItem._Preview.URL = Super.GetTaskImageString(Global.GetXElementAttributeStr(xelement, "Preview"));
			string[] array = Global.GetXElementAttributeStr(xelement, "RewardExplain").Split(new char[]
			{
				'|'
			});
			int num = 0;
			while (num < riChangHuoDongItem._RewardType.Length && num < array.Length)
			{
				string[] array2 = array[num].Split(new char[]
				{
					','
				});
				if (array2.Length == 2)
				{
					riChangHuoDongItem._RewardType[num].text = Global.GetFuBenRewardTypeStr(array2[0].SafeToInt32(0)) + ":";
					riChangHuoDongItem._RewardType[num].gameObject.SetActive(true);
					riChangHuoDongItem._RewardLevel[num].Percent = (double)array2[1].SafeToInt32(0) / 5.0;
					riChangHuoDongItem._RewardLevel[num].gameObject.SetActive(true);
				}
				num++;
			}
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "KaiQiVipLevel");
			if (xelementAttributeInt2 >= 0 && xelementAttributeInt2 > Global.Data.roleData.VIPLevel)
			{
				riChangHuoDongItem.IsEnabled = false;
				riChangHuoDongItem.obj.gameObject.SetActive(true);
				riChangHuoDongItem.NeedLeve.Text = string.Format(Global.GetLang("【需要VIP{0}】"), xelementAttributeInt2);
			}
			else
			{
				riChangHuoDongItem.obj.gameObject.SetActive(false);
			}
			list.Add(riChangHuoDongItem);
		}
		this.ItemsList = list;
		this.ItemCollection.DelayUpdate();
		if (this.ItemsList.Count <= this.ItemPerPage)
		{
			this._PageRadio.gameObject.SetActive(false);
			return;
		}
		this.PageCount = (this.ItemsList.Count - 1) / this.ItemPerPage + 1;
		for (int i = this.ItemsList.Count; i < this.PageCount * this.ItemPerPage; i++)
		{
			PaddingObject paddingObject = U3DUtils.NEW<PaddingObject>();
			paddingObject.name = this.ItemCollection.Count.ToString("0000");
			paddingObject.Size = new Vector3(190f, 375f, 0f);
			this.ItemCollection.AddNoUpdate(paddingObject);
			paddingObject.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			paddingObject.Init();
		}
		this.ItemCollection.DelayUpdate();
		this._PageRadio.ItemCount = this.ItemCollection.Count;
		this._PageRadio.GotoPage(0);
	}

	public void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	public void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	public void ShowPage(int pageIndex)
	{
		this._PageRadio.GotoPage(pageIndex);
	}

	public void ShowHuoDongDetailWindow(int index)
	{
		GameObject childAt = this._TaskList.getChildAt(index);
		if (null == childAt)
		{
			return;
		}
		RiChangHuoDongItem riChangHuoDongItem = U3DUtils.AS<RiChangHuoDongItem>(childAt);
		if (null == riChangHuoDongItem)
		{
			return;
		}
		this.ShowHuoDongDetailWindow(riChangHuoDongItem);
	}

	private void ShowHuoDongDetailWindow(RiChangHuoDongItem item)
	{
		this.RiChangHuoDongType = item.HuoDongType;
		if (null == this.RiChangVIPHuoDongDetailWindow)
		{
			this.RiChangVIPHuoDongDetailWindow = U3DUtils.NEW<GChildWindow>();
			this.RiChangVIPHuoDongDetailWindow.Modal = true;
			base.Children.Add(this.RiChangVIPHuoDongDetailWindow);
		}
		this.RiChangVIPHuoDongDetailWindow.Visibility = true;
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.BossZhiJia || this.RiChangHuoDongType == RiChangHuoDongTypes.HuangJinShengDian)
		{
			if (null == this._RiChangVIPHuoDongDetailPart)
			{
				this._RiChangVIPHuoDongDetailPart = U3DUtils.NEW<RiChangVIPHuoDongDetailPartPart>();
				this.RiChangVIPHuoDongDetailWindow.Children.Add(this._RiChangVIPHuoDongDetailPart);
				this._RiChangVIPHuoDongDetailPart.transform.localPosition = new Vector3(0f, 0f, -2f);
				this._RiChangVIPHuoDongDetailPart._Close.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
				{
					this.RiChangVIPHuoDongDetailWindow.Visibility = false;
					Object.Destroy(this._RiChangVIPHuoDongDetailPart.gameObject);
					this._RiChangVIPHuoDongDetailPart = null;
				};
			}
			this._RiChangVIPHuoDongDetailPart.InitData(item.HuoDongType, item.ItemName);
		}
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		RiChangHuoDongItem riChangHuoDongItem = U3DUtils.AS<RiChangHuoDongItem>(this._TaskList.SelectedItem);
		if (null == riChangHuoDongItem)
		{
			return;
		}
		if (!riChangHuoDongItem.IsEnabled)
		{
			return;
		}
		this.ShowHuoDongDetailWindow(riChangHuoDongItem);
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		this.RefreshFuBenDataList();
		this.ShowPage(0);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void RefreshFuBenDataList()
	{
		for (int i = 0; i < this.CanShowItemsList.Count; i++)
		{
			FuBenData fuBenData = Global.GetFuBenData(this.CanShowItemsList[i].CopyID);
			if (fuBenData != null)
			{
				this.CanShowItemsList[i].EnterNum = fuBenData.EnterNum;
				this.CanShowItemsList[i].FinishNum = fuBenData.FinishNum;
				if (null != this._RiChangVIPHuoDongDetailPart && this._RiChangVIPHuoDongDetailPart.CopyID == fuBenData.FuBenID)
				{
					this._RiChangVIPHuoDongDetailPart.RefreshDetail(null);
				}
			}
		}
	}

	public virtual void OnEnable()
	{
		this.ResetGetNewData();
		this.GetNewData();
		if (null != this.RiChangVIPHuoDongDetailWindow)
		{
			this.RiChangVIPHuoDongDetailWindow.Visibility = false;
		}
	}

	protected virtual void OnDisable()
	{
		if (null != this.RiChangVIPHuoDongDetailWindow)
		{
			this.RiChangVIPHuoDongDetailWindow.Visibility = false;
		}
		if (null != this._RiChangVIPHuoDongDetailPart)
		{
			Object.Destroy(this._RiChangVIPHuoDongDetailPart.gameObject);
			this._RiChangVIPHuoDongDetailPart = null;
		}
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangVIPHuoDongPart = null;
		}
		base.OnDestroy();
	}

	public ShowNetImage _Bak;

	public ListBox _TaskList;

	public GScrollBarPageList _PageRadio;

	public UIDraggablePanel _TaskPanel;

	public GChildWindow RiChangVIPHuoDongDetailWindow;

	public RiChangVIPHuoDongDetailPartPart _RiChangVIPHuoDongDetailPart;

	private int ItemPerPage = 4;

	private int PageCount = 1;

	public int NpcID = -1;

	private int MaxPageCount;

	private List<RiChangHuoDongItem> ItemsList = new List<RiChangHuoDongItem>();

	private List<RiChangHuoDongItem> CanShowItemsList = new List<RiChangHuoDongItem>();

	private int CurrentSelectedPage;

	private RiChangHuoDongTypes RiChangHuoDongType;

	private bool FirstGetNewData;
}
