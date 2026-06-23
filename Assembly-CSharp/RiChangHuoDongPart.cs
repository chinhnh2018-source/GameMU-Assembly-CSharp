using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RiChangHuoDongPart : UserControl
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
		XElement gameResXml = Global.GetGameResXml("Config/HuoDongTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		Dictionary<int, int[]> dictionary = null;
		if (category != ActivityCategorys.JuQingFuBen)
		{
			dictionary = ConfigSystemParam.GetSystemParamIntDict1ByName("HuoDongNeed", '|', ',');
		}
		List<RiChangHuoDongItem> list = new List<RiChangHuoDongItem>();
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			if (xelementAttributeInt != 7 && xelementAttributeInt != 8)
			{
				RiChangHuoDongItem si = U3DUtils.NEW<RiChangHuoDongItem>();
				si.Width = 205.0;
				si.Height = 273.0;
				si.name = this.ItemCollection.Count.ToString("0000");
				this.ItemCollection.AddNoUpdate(si);
				si.transform.localPosition = new Vector3(0f, 0f, -0.01f);
				si.m_strConfigPath = Global.GetXElementAttributeStr(xelement, "GLXml");
				si.ItemName = Global.GetXElementAttributeStr(xelement, "Name");
				si.ActivityCategory = category;
				si.Init();
				si.TabID = xelementAttributeInt;
				si.HuoDongType = (RiChangHuoDongTypes)xelementAttributeInt;
				si._Preview.URL = Super.GetTaskImageString(Global.GetXElementAttributeStr(xelement, "Preview"));
				string[] array = Global.GetXElementAttributeStr(xelement, "RewardExplain").Split(new char[]
				{
					'|'
				});
				int num = 0;
				while (num < si._RewardType.Length && num < array.Length)
				{
					string[] array2 = array[num].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						si._RewardType[num].text = Global.GetFuBenRewardTypeStr(array2[0].SafeToInt32(0)) + ":";
						si._RewardType[num].gameObject.SetActive(true);
						si._RewardLevel[num].Percent = (double)array2[1].SafeToInt32(0) / 5.0;
						si._RewardLevel[num].gameObject.SetActive(true);
					}
					num++;
				}
				if (xelementAttributeInt == 3)
				{
					ActivityTipManager.RegActivityTipItem(1005, delegate(int s, ActivityTipItem e)
					{
						si.IsGrayImage = !e.IsActive;
					});
				}
				if (dictionary != null)
				{
					int num2 = -1;
					int num3 = -1;
					int[] array3;
					if (dictionary.TryGetValue(xelementAttributeInt, ref array3))
					{
						num2 = Convert.ToInt32(array3[1]);
						num3 = Convert.ToInt32(array3[2]);
					}
					if (!UIHelper.AvalidLevel(num3, num2, false))
					{
						si.IsEnabled = false;
						si.obj.gameObject.SetActive(true);
						si.NeedLeve.Text = string.Format(Global.GetLang("【需要{0}】"), Global.FormatLevel(num2, num3));
					}
					else
					{
						si.obj.gameObject.SetActive(false);
					}
				}
				else
				{
					si.obj.gameObject.SetActive(false);
				}
				list.Add(si);
			}
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
		if (null == this.RichangHuoDongDetailWindow)
		{
			this.RichangHuoDongDetailWindow = U3DUtils.NEW<GChildWindow>();
			this.RichangHuoDongDetailWindow.Modal = true;
			base.Children.Add(this.RichangHuoDongDetailWindow);
		}
		this.RichangHuoDongDetailWindow.Visibility = true;
		if (null != this._RiChangHuoDongDetailPart && this.RiChangHuoDongType != RiChangHuoDongTypes.Demon && this.RiChangHuoDongType != RiChangHuoDongTypes.BloodCastle)
		{
			Object.Destroy(this._RiChangHuoDongDetailPart.gameObject);
			this._RiChangHuoDongDetailPart = null;
		}
		if (null != this._RiChangHuoDongBattlePart && this.RiChangHuoDongType != RiChangHuoDongTypes.Battle && this.RiChangHuoDongType != RiChangHuoDongTypes.PKKing && this.RiChangHuoDongType != RiChangHuoDongTypes.AngelTemple)
		{
			Object.Destroy(this._RiChangHuoDongBattlePart.gameObject);
			this._RiChangHuoDongBattlePart = null;
		}
		if (null != this._RiChangHuoDongHuangJinPart && this.RiChangHuoDongType != RiChangHuoDongTypes.HuangJin)
		{
			Object.Destroy(this._RiChangHuoDongHuangJinPart.gameObject);
			this._RiChangHuoDongHuangJinPart = null;
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.Demon || this.RiChangHuoDongType == RiChangHuoDongTypes.BloodCastle)
		{
			if (null == this._RiChangHuoDongDetailPart)
			{
				this._RiChangHuoDongDetailPart = U3DUtils.NEW<RiChangHuoDongDetailPart>();
				this.RichangHuoDongDetailWindow.Children.Add(this._RiChangHuoDongDetailPart);
				this._RiChangHuoDongDetailPart.transform.localPosition = new Vector3(0f, 0f, -2f);
				this._RiChangHuoDongDetailPart._Close.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
				{
					this.RichangHuoDongDetailWindow.Visibility = false;
					Object.Destroy(this._RiChangHuoDongDetailPart.gameObject);
					this._RiChangHuoDongDetailPart = null;
				};
			}
			this._RiChangHuoDongDetailPart.InitData(item.HuoDongType, item.ItemName);
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.Battle || this.RiChangHuoDongType == RiChangHuoDongTypes.PKKing || this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			if (null == this._RiChangHuoDongBattlePart)
			{
				this._RiChangHuoDongBattlePart = U3DUtils.NEW<RiChangHuoDongBattlePart>();
				this.RichangHuoDongDetailWindow.Children.Add(this._RiChangHuoDongBattlePart);
				this._RiChangHuoDongBattlePart.transform.localPosition = new Vector3(0f, 0f, -2f);
				this._RiChangHuoDongBattlePart._Close.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
				{
					this.RichangHuoDongDetailWindow.Visibility = false;
					Object.Destroy(this._RiChangHuoDongBattlePart.gameObject);
					this._RiChangHuoDongBattlePart = null;
				};
			}
			this._RiChangHuoDongBattlePart.InitData(item.HuoDongType, item.ItemName);
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.HuangJin)
		{
			if (null == this._RiChangHuoDongHuangJinPart)
			{
				this._RiChangHuoDongHuangJinPart = U3DUtils.NEW<RiChangHuoDongHuangJinPart>();
				this.RichangHuoDongDetailWindow.Children.Add(this._RiChangHuoDongHuangJinPart);
				this._RiChangHuoDongHuangJinPart.transform.localPosition = new Vector3(0f, 0f, -2f);
				this._RiChangHuoDongHuangJinPart._Close.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
				{
					this.RichangHuoDongDetailWindow.Visibility = false;
					Object.Destroy(this._RiChangHuoDongHuangJinPart.gameObject);
					this._RiChangHuoDongHuangJinPart = null;
				};
			}
			this._RiChangHuoDongHuangJinPart.InitData(item.HuoDongType, item.ItemName);
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
				if (null != this._RiChangHuoDongDetailPart && this._RiChangHuoDongDetailPart.CopyID == fuBenData.FuBenID)
				{
					this._RiChangHuoDongDetailPart.RefreshDetail(null);
				}
			}
		}
	}

	public virtual void OnEnable()
	{
		this.ResetGetNewData();
		this.GetNewData();
		if (null != this.RichangHuoDongDetailWindow)
		{
			this.RichangHuoDongDetailWindow.Visibility = false;
		}
	}

	protected virtual void OnDisable()
	{
		if (null != this.RichangHuoDongDetailWindow)
		{
			this.RichangHuoDongDetailWindow.Visibility = false;
		}
		if (null != this._RiChangHuoDongHuangJinPart)
		{
			Object.Destroy(this._RiChangHuoDongHuangJinPart.gameObject);
			this._RiChangHuoDongHuangJinPart = null;
		}
		if (this._RiChangHuoDongBattlePart)
		{
			Object.Destroy(this._RiChangHuoDongBattlePart.gameObject);
			this._RiChangHuoDongBattlePart = null;
		}
		if (this._RiChangHuoDongDetailPart)
		{
			Object.Destroy(this._RiChangHuoDongDetailPart.gameObject);
			this._RiChangHuoDongDetailPart = null;
		}
	}

	protected override void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(1005, null);
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangHuoDongPart = null;
		}
		base.OnDestroy();
	}

	public ShowNetImage _Bak;

	public ListBox _TaskList;

	public GScrollBarPageList _PageRadio;

	public UIDraggablePanel _TaskPanel;

	[NonSerialized]
	public GChildWindow RichangHuoDongDetailWindow;

	[NonSerialized]
	public RiChangHuoDongDetailPart _RiChangHuoDongDetailPart;

	[NonSerialized]
	public RiChangHuoDongBattlePart _RiChangHuoDongBattlePart;

	[NonSerialized]
	public RiChangHuoDongHuangJinPart _RiChangHuoDongHuangJinPart;

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
