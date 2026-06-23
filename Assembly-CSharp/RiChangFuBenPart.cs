using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RiChangFuBenPart : UserControl
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
		this.thisCtrl = this;
		this._TaskList.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this._PageRadio.ItemPerPage = this.ItemPerPage;
		this._PageRadio.ItemCount = 0;
		this.InitPartSize(800, 450);
		this._TaskList.ItemPerPage = 4;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	private void FilterItemList()
	{
		this.CanShowItemsList = this.ItemsList;
	}

	private bool HideItem(int DisplayID, int MinLevel, int MaxLevel, int minZhuanSheng, int maxZhuanSheng)
	{
		if (this.Category == ActivityCategorys.RiChangFuBen)
		{
			if (DisplayID == 2)
			{
				return false;
			}
		}
		else if (this.Category == ActivityCategorys.RiChangHuoDong && DisplayID != 2)
		{
			return false;
		}
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return true;
		}
		MaxLevel = ((MaxLevel != -1) ? MaxLevel : 4095);
		maxZhuanSheng = ((maxZhuanSheng != -1) ? maxZhuanSheng : 4095);
		int num = MinLevel + minZhuanSheng * 65536;
		int num2 = MinLevel + maxZhuanSheng * 65536;
		int num3 = Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 65536;
		return num > num3 || num3 > num2;
	}

	public void InitPartData(ActivityCategorys category)
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBenTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		Dictionary<int, int[]> systemParamIntDict1ByName = ConfigSystemParam.GetSystemParamIntDict1ByName("FuBenNeed", '|', ',');
		Dictionary<int, int[]> systemParamIntDict1ByName2 = ConfigSystemParam.GetSystemParamIntDict1ByName("RiChangFuBenNeed", '|', ',');
		List<RiChangFuBenItem> list = new List<RiChangFuBenItem>();
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "FuBenType");
			if (xelementAttributeInt2 == (int)category)
			{
				RiChangFuBenItem riChangFuBenItem = U3DUtils.NEW<RiChangFuBenItem>();
				riChangFuBenItem.TabID = xelementAttributeInt;
				riChangFuBenItem.FuBenType = xelementAttributeInt2;
				riChangFuBenItem.name = this.ItemCollection.Count.ToString("0000");
				this.ItemCollection.AddNoUpdate(riChangFuBenItem);
				riChangFuBenItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
				riChangFuBenItem.Init();
				riChangFuBenItem.ItemName = Global.GetXElementAttributeStr(xelement, "Name");
				riChangFuBenItem._Preview.URL = Super.GetFuBenPreviewImageString(Global.GetXElementAttributeStr(xelement, "Preview"));
				string[] array = Global.GetXElementAttributeStr(xelement, "RewardExplain").Split(new char[]
				{
					'|'
				});
				int num = 0;
				while (num < riChangFuBenItem._RewardType.Length && num < array.Length)
				{
					string[] array2 = array[num].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						riChangFuBenItem._RewardType[num].text = Global.GetFuBenRewardTypeStr(array2[0].SafeToInt32(0)) + ":";
						riChangFuBenItem._RewardType[num].gameObject.SetActive(true);
						riChangFuBenItem._RewardLevel[num].Percent = (double)array2[1].SafeToInt32(0) / 5.0;
						riChangFuBenItem._RewardLevel[num].gameObject.SetActive(true);
					}
					num++;
				}
				list.Add(riChangFuBenItem);
				bool flag = false;
				if (systemParamIntDict1ByName != null)
				{
					int num2 = -1;
					int[] array3;
					if (systemParamIntDict1ByName.TryGetValue(xelementAttributeInt, ref array3))
					{
						num2 = Convert.ToInt32(array3[1]);
					}
					if (Global.Data.roleData.CompletedMainTaskID < num2)
					{
						riChangFuBenItem.WeiKaiQi.gameObject.SetActive(true);
						riChangFuBenItem.NeedText.Text = Global.GetLang("需要完成主线任务");
						riChangFuBenItem.RenWuTitle.Text = string.Format(Global.GetLang("【{0}】"), Global.GetTaskTitleByID(num2));
						flag = true;
					}
				}
				if (!flag && systemParamIntDict1ByName2 != null)
				{
					int num3 = -1;
					int[] array4;
					if (systemParamIntDict1ByName2.TryGetValue(xelementAttributeInt, ref array4))
					{
						num3 = Convert.ToInt32(array4[1]);
					}
					if (Global.GetUnionLevel(-1, -1) < num3)
					{
						riChangFuBenItem.WeiKaiQi.gameObject.SetActive(true);
						riChangFuBenItem.NeedText.Text = string.Format(Global.GetLang("【需要达到0转{0}级】"), num3);
						riChangFuBenItem.RenWuTitle.Text = null;
						flag = true;
					}
				}
				if (!flag)
				{
					riChangFuBenItem.WeiKaiQi.gameObject.SetActive(false);
				}
				riChangFuBenItem.IsEnabeld = !flag;
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
		for (int j = this.ItemsList.Count; j < this.PageCount * this.ItemPerPage; j++)
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
		if (0 < this.DetailTabID)
		{
			this.ShowRiChangFuBenDetailPart(this.mActivityCategorys, this.DetailTabID);
		}
	}

	public string GetFuBenName(int copyID)
	{
		if (this.ItemsList == null)
		{
			return string.Empty;
		}
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			if (this.ItemsList[i].CopyID == copyID)
			{
				return this.ItemsList[i].ItemName;
			}
		}
		return string.Empty;
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

	public void ShowFuBenDetailWindow(int index)
	{
		if (index < 0 || index >= this._TaskList.Count())
		{
			return;
		}
		this._TaskList.SelectedIndex = index;
		this.listBox_MouseLeftButtonUp(this._TaskList, MouseEvent.Empty);
	}

	public void ShowRiChangFuBenDetailPart(ActivityCategorys activityCategorys, int TabID)
	{
		TabID -= 99999;
		this.DetailTabID = TabID;
		this.mActivityCategorys = activityCategorys;
		if (0 < this._TaskList.Items.Count)
		{
			for (int i = 0; i < this._TaskList.Items.Count; i++)
			{
				RiChangFuBenItem riChangFuBenItem = this._TaskList.Items[i].SafeGetComponent<RiChangFuBenItem>();
				if (null != riChangFuBenItem && riChangFuBenItem.FuBenType == (int)activityCategorys && riChangFuBenItem.TabID == TabID)
				{
					this._TaskList.SelectedIndex = i;
					this.listBox_MouseLeftButtonUp(null, null);
					SpringPanel.Begin(this._TaskList.transform.parent.gameObject, new Vector3(-239f - this._TaskList.cellWidth * (float)this._TaskList.SelectedIndex, 16f, -0.01f), 10f);
					break;
				}
			}
		}
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		RiChangFuBenItem riChangFuBenItem = this._TaskList.SelectedItem.SafeGetComponent<RiChangFuBenItem>();
		if (null == riChangFuBenItem)
		{
			return;
		}
		if (!riChangFuBenItem.IsEnabeld)
		{
			return;
		}
		if (riChangFuBenItem.FuBenType == 0 || riChangFuBenItem.FuBenType == 1)
		{
			if (null == this.RichangFuBenDetailWindow)
			{
				this.RichangFuBenDetailWindow = U3DUtils.NEW<GChildWindow>();
				base.Children.Add(this.RichangFuBenDetailWindow);
			}
			this.RichangFuBenDetailWindow.Visibility = true;
			if (null == this._RiChangFuBenDetailPart)
			{
				this._RiChangFuBenDetailPart = U3DUtils.NEW<RiChangFuBenDetailPart>();
				this.RichangFuBenDetailWindow.Children.Add(this._RiChangFuBenDetailPart);
				this._RiChangFuBenDetailPart.transform.localPosition = new Vector3(0f, 0f, -2f);
				this._RiChangFuBenDetailPart._Close.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
				{
					this.RichangFuBenDetailWindow.Visibility = false;
					this.DetailTabID = 0;
				};
			}
			this._RiChangFuBenDetailPart.FuBenType = riChangFuBenItem.FuBenType;
			this._RiChangFuBenDetailPart.InitData(riChangFuBenItem.TabID, riChangFuBenItem.ItemName);
		}
		else if (riChangFuBenItem.FuBenType == 2)
		{
			if (null == this.JingYanFuBenDetailWindow)
			{
				this.JingYanFuBenDetailWindow = U3DUtils.NEW<GChildWindow>();
				base.Children.Add(this.JingYanFuBenDetailWindow);
			}
			this.JingYanFuBenDetailWindow.Visibility = true;
			if (null == this._JingYanFuBenDetailPart)
			{
				this._JingYanFuBenDetailPart = U3DUtils.NEW<JingYanFuBenDetailPart>();
				this.JingYanFuBenDetailWindow.Children.Add(this._JingYanFuBenDetailPart);
				this._JingYanFuBenDetailPart.transform.localPosition = new Vector3(0f, 0f, -2f);
				this._JingYanFuBenDetailPart._Close.MouseLeftButtonUp = delegate(object s1, MouseEvent e1)
				{
					this.JingYanFuBenDetailWindow.Visibility = false;
				};
			}
			this._JingYanFuBenDetailPart.FuBenType = 2;
			this._JingYanFuBenDetailPart.InitData(riChangFuBenItem.TabID, riChangFuBenItem.ItemName);
			SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenDetailpartEnter, HelpStateEvents.Inactived, 1);
		}
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
		this.FilterItemList();
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
				if (null != this._RiChangFuBenDetailPart && this._RiChangFuBenDetailPart.CopyID == fuBenData.FuBenID)
				{
					this._RiChangFuBenDetailPart.InitPartData();
				}
			}
		}
	}

	public virtual void OnEnable()
	{
		this.ResetGetNewData();
		this.GetNewData();
		if (null != this.RichangFuBenDetailWindow && 0 >= this.DetailTabID)
		{
			this.RichangFuBenDetailWindow.Visibility = false;
		}
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.riChangFuBenDetailPart = null;
		}
		base.OnDestroy();
	}

	public ShowNetImage _Bak;

	public ListBox _TaskList;

	public GScrollBarPageList _PageRadio;

	public UIDraggablePanel _TaskPanel;

	public GChildWindow RichangFuBenDetailWindow;

	public RiChangFuBenDetailPart _RiChangFuBenDetailPart;

	public GChildWindow JingYanFuBenDetailWindow;

	public JingYanFuBenDetailPart _JingYanFuBenDetailPart;

	private SpriteSL thisCtrl;

	public ActivityCategorys Category;

	private int ItemPerPage = 4;

	private int PageCount = 1;

	public int NpcID = -1;

	private int MaxPageCount;

	private List<RiChangFuBenItem> ItemsList = new List<RiChangFuBenItem>();

	private List<RiChangFuBenItem> CanShowItemsList = new List<RiChangFuBenItem>();

	private int CurrentSelectedPage;

	private int DetailTabID;

	private ActivityCategorys mActivityCategorys;

	private bool FirstGetNewData;
}
