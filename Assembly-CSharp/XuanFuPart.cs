using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class XuanFuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_EnterServerLabel.text = Global.GetLang("进入游戏");
		this.m_inputDefaultLabel.text = Global.GetLang(string.Empty);
		this.m_BtnRecordServerLabel.text = Global.GetLang("推荐服务器");
		if (this.staticText.Length > 2 && Context.IsHaiwai)
		{
			this.staticText[0].text = Global.GetLang("选择主题服");
			this.staticText[1].text = Global.GetLang("选择星座服");
			this.staticText[2].text = Global.GetLang("服");
			this.staticText[2].Pivot = 5;
			this.staticText[2].X = 300.0;
			this.staticText[2].Y = -225.0;
			this.InputObj.gameObject.transform.localPosition = new Vector3(180f, -225f, this.InputObj.gameObject.transform.localPosition.z);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.mOBCFirstTabList = this.ListboxFirst.ItemsSource;
		this.ListboxFirst.SelectionChanged = new MouseLeftButtonUpEventHandler(this.FirstTabListSelectChange);
		this.mOBCSecondTabList = this.ListboxSecond.ItemsSource;
		this.ListboxSecond.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SecondTabListSelectChange);
		UIEventListener.Get(this.FirstBottomBtn.gameObject).onClick = delegate(GameObject s1)
		{
			NGUITools.SetActive(this.PanelFirstLevel, !this.PanelFirstLevel.gameObject.activeInHierarchy);
			if (this.PanelFirstLevel.gameObject.activeInHierarchy)
			{
				this.FirstBottomBtnBackground.spriteName = "arrow01";
			}
			else
			{
				this.FirstBottomBtnBackground.spriteName = "arrow02";
			}
		};
		UIEventListener.Get(this.SecondBottomBtn.gameObject).onClick = delegate(GameObject s1)
		{
			NGUITools.SetActive(this.PanelSecondLevel, !this.PanelSecondLevel.gameObject.activeInHierarchy);
			if (this.PanelSecondLevel.gameObject.activeInHierarchy)
			{
				this.SecondBottomBtnBackground.spriteName = "arrow01";
			}
			else
			{
				this.SecondBottomBtnBackground.spriteName = "arrow02";
			}
		};
	}

	private new void Start()
	{
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		if (this.m_BtnRecordServer != null)
		{
			this.m_BtnRecordServer.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3
				});
			};
		}
		if (this.m_EnterServer != null)
		{
			this.m_EnterServer.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (string.IsNullOrEmpty(this.ServerTextBox.Text))
				{
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("请输入服务器号"), -1, -1, -1, -1, false);
					return;
				}
				if (this.bottomSelectedFirstLevelData == null)
				{
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("请选择主题服"), -1, -1, -1, -1, false);
					return;
				}
				if (this.bottomSelectedFirstLevelData.nFirstLevelServerID != 10 && this.bottomSelectedSecondLevelData == null)
				{
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("请选择二级主题服"), -1, -1, -1, -1, false);
					return;
				}
				int zoneID = ConvertExt.SafeConvertToInt32(this.ServerTextBox.Text);
				ZtBuffServerInfo ztBuffServerInfo = this.FindServerInfo(zoneID);
				if (ztBuffServerInfo == null)
				{
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("输入的服务器号不正确"), -1, -1, -1, -1, false);
					return;
				}
				if (ztBuffServerInfo.nStatus == 1)
				{
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("选择的服务器处于维护中"), -1, -1, -1, -1, false);
					return;
				}
				if (ztBuffServerInfo != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2,
						Data = ztBuffServerInfo
					});
				}
			};
		}
	}

	private ZtBuffServerInfo FindServerInfo(int zoneID)
	{
		ZtBuffServerInfo result = null;
		if (this.bottomSelectedFirstLevelData.nFirstLevelServerID == 10)
		{
			int count = this.bottomSelectedFirstLevelData.listServerData.Count;
			for (int i = 0; i < count; i++)
			{
				SecondLevelServerListData secondLevelServerListData = this.bottomSelectedFirstLevelData.listServerData[i];
				int count2 = secondLevelServerListData.listServerData.Count;
				for (int j = 0; j < count2; j++)
				{
					ZtBuffServerInfo ztBuffServerInfo = secondLevelServerListData.listServerData[j];
					if (ztBuffServerInfo.nZoneID == zoneID)
					{
						return ztBuffServerInfo;
					}
				}
			}
		}
		else
		{
			int count = this.bottomSelectedSecondLevelData.listServerData.Count;
			for (int k = 0; k < count; k++)
			{
				ZtBuffServerInfo ztBuffServerInfo = this.bottomSelectedSecondLevelData.listServerData[k];
				if (ztBuffServerInfo.nZoneID == zoneID)
				{
					return ztBuffServerInfo;
				}
			}
		}
		return result;
	}

	public void initDataFromServer()
	{
		this.initDataFromServerEx();
	}

	private new void Update()
	{
		if (this.IsFirstRender)
		{
			if (this.m_ListTotalServerItem != null && this.m_ListEachServerItem != null)
			{
				this.m_ListTotalServerItem.repositionNow = true;
				this.m_ListEachServerItem.repositionNow = true;
			}
			this.IsFirstRender = false;
		}
		if (this.ResetPanelTotalServerItemPos)
		{
			this.ResetPosCounter++;
			if (this.ResetPosCounter > 1)
			{
				this.ResetPosCounter = 0;
				this.ResetPanelTotalServerItemPos = false;
				Vector3 localPosition = this.PanelTotalServerItem.transform.localPosition;
				this.PanelTotalServerItem.transform.localPosition = new Vector3(localPosition.x, -20f, localPosition.z);
				Vector4 clipRange = this.PanelTotalServerItem.clipRange;
				this.PanelTotalServerItem.clipRange = new Vector4(clipRange.x, 20f, clipRange.z, clipRange.w);
			}
		}
	}

	public void SetBtnActieve(XuanFuTotalServerItem btn)
	{
		if (null != btn)
		{
			if (btn == this.m_currentSelectedItem)
			{
				this.m_currentSelectedItem.ToggleState = true;
				return;
			}
			if (null != this.m_currentSelectedItem)
			{
				this.m_lastSelectedItem = this.m_currentSelectedItem;
				this.m_currentSelectedItem = btn;
			}
			this.m_currentSelectedItem = btn;
			if (null != this.m_currentSelectedItem)
			{
				this.m_currentSelectedItem.ToggleState = true;
			}
			if (null != this.m_lastSelectedItem)
			{
				this.m_lastSelectedItem.ToggleState = false;
			}
		}
	}

	private void RefreshEachServerItems(SecondLevelServerListData secondLevelListData)
	{
		this.PnlItemServers.transform.localPosition = new Vector3(120f, 0f, 0f);
		UIPanel component = this.PnlItemServers.GetComponent<UIPanel>();
		if (component != null)
		{
			component.clipRange = new Vector4(0f, 10f, 640f, 370f);
		}
		this.m_ListEachServerItem.Clear();
		int count = secondLevelListData.listServerData.Count;
		for (int i = 0; i < count; i++)
		{
			XuanFuServerItem xuanFuServerItem = U3DUtils.NEW<XuanFuServerItem>();
			ZtBuffServerInfo serverInfoVO = secondLevelListData.listServerData[i];
			xuanFuServerItem.RefreshUI(serverInfoVO);
			xuanFuServerItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				ZtBuffServerInfo ztBuffServerInfo = (ZtBuffServerInfo)e.Data;
				if (ztBuffServerInfo != null && ztBuffServerInfo.nStatus == 1)
				{
					return;
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					Data = e.Data
				});
			};
			U3DUtils.AddChild(this.m_ListEachServerItem.gameObject, xuanFuServerItem.gameObject, true);
		}
		this.m_ListEachServerItem.repositionNow = true;
	}

	private void InitFirstLevelData()
	{
		this.mOBCFirstTabList.Clear();
		int count = this.ServerData.ServerListData.listServerData.Count;
		for (int i = 0; i < count; i++)
		{
			FistLevelServerListData fistLevelServerListData = this.ServerData.ServerListData.listServerData[i];
			XuanFuLabItem xuanFuLabItem = U3DUtils.NEW<XuanFuLabItem>();
			xuanFuLabItem.labText.text = fistLevelServerListData.strFirstLevelServerName;
			xuanFuLabItem.firstLevelData = fistLevelServerListData;
			this.mOBCFirstTabList.AddNoUpdate(xuanFuLabItem);
		}
		Vector3 localPosition = this.PanelFirstLevel.transform.localPosition;
		Vector3 localPosition2;
		localPosition2..ctor(localPosition.x, (float)(count * 37), localPosition.z);
		this.PanelFirstLevel.transform.localPosition = localPosition2;
		NGUITools.SetActive(this.PanelFirstLevel, false);
		if (this.PanelFirstLevel.gameObject.activeInHierarchy)
		{
			this.FirstBottomBtnBackground.spriteName = "arrow01";
		}
		else
		{
			this.FirstBottomBtnBackground.spriteName = "arrow02";
		}
		if (count > 0)
		{
			this.SetBottomSelectedFirstLevelData(this.ServerData.ServerListData.listServerData[0]);
		}
	}

	private void InitSecondLevelData(FistLevelServerListData firstLevel)
	{
		this.mOBCSecondTabList.Clear();
		int count = firstLevel.listServerData.Count;
		for (int i = 0; i < count; i++)
		{
			SecondLevelServerListData secondLevelServerListData = firstLevel.listServerData[i];
			XuanFuLabItem xuanFuLabItem = U3DUtils.NEW<XuanFuLabItem>();
			xuanFuLabItem.labText.text = secondLevelServerListData.strSecondtLevelServerName;
			xuanFuLabItem.secondLevelData = secondLevelServerListData;
			this.mOBCSecondTabList.AddNoUpdate(xuanFuLabItem);
		}
		Vector3 localPosition = this.PanelSecondLevel.transform.localPosition;
		Vector3 localPosition2;
		localPosition2..ctor(localPosition.x, (float)(count * 37), localPosition.z);
		this.PanelSecondLevel.transform.localPosition = localPosition2;
		NGUITools.SetActive(this.PanelSecondLevel, false);
		if (this.PanelSecondLevel.gameObject.activeInHierarchy)
		{
			this.SecondBottomBtnBackground.spriteName = "arrow01";
		}
		else
		{
			this.SecondBottomBtnBackground.spriteName = "arrow02";
		}
		if (count > 0)
		{
			this.SetBottomSelectedSecondLevelData(firstLevel.listServerData[0]);
		}
	}

	private void SetBottomSelectedFirstLevelData(FistLevelServerListData firstLevel)
	{
		this.bottomSelectedFirstLevelData = firstLevel;
		this.FirstLevelSelectLabel.text = firstLevel.strFirstLevelServerName;
		if (firstLevel.nFirstLevelServerID == 10)
		{
			NGUITools.SetActive(this.SecondLevelObj, false);
		}
		else
		{
			NGUITools.SetActive(this.SecondLevelObj, true);
			this.InitSecondLevelData(firstLevel);
			this.LabelSecond.text = string.Format(Global.GetLang("选择{0}"), firstLevel.strFirstLevelServerName);
		}
	}

	private void SetBottomSelectedSecondLevelData(SecondLevelServerListData secondLevel)
	{
		this.bottomSelectedSecondLevelData = secondLevel;
		this.SecondLevelSelectLabel.text = secondLevel.strSecondtLevelServerName;
	}

	public void initDataFromServerEx()
	{
		this.InitFirstLevelData();
		this.m_ListTotalServerItem.IsPosYFixed = true;
		this.m_ListTotalServerItem.Clear();
		if (this.ServerData == null)
		{
			return;
		}
		List<FistLevelServerListData> listServerData = this.ServerData.ServerListData.listServerData;
		int count = listServerData.Count;
		if (this.items == null)
		{
			this.items = new XuanFuTotalServerOneLevelItem[count];
		}
		for (int i = 0; i < count; i++)
		{
			if (this.items[i] == null)
			{
				this.items[i] = U3DUtils.NEW<XuanFuTotalServerOneLevelItem>();
				this.items[i].ListIndex = i;
			}
			this.items[i].FirstLevelListData = listServerData[i];
			this.items[i].RefreshUI();
			this.items[i].DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				XuanFuTotalServerOneLevelItem xuanFuTotalServerOneLevelItem = s as XuanFuTotalServerOneLevelItem;
				if (e.ID == 1)
				{
					this.setTab(xuanFuTotalServerOneLevelItem.ListIndex);
				}
				else if (e.ID == 2)
				{
					SecondLevelServerListData secondLevelServerListData = e.Data as SecondLevelServerListData;
					if (secondLevelServerListData != null)
					{
						this.RefreshEachServerItems(secondLevelServerListData);
					}
				}
				else if (e.ID == 3)
				{
					this.ResetPanelTotalServerItemPos = true;
					this.ResetPosCounter = 0;
				}
			};
			U3DUtils.AddChild(this.m_ListTotalServerItem.gameObject, this.items[i].gameObject, false);
		}
		this.IsFirstRender = true;
		this.m_ListTotalServerItem.UpdataNow();
		if (this.items.Length > 0)
		{
			UIEventListener component = this.items[0].BtnItem.GetComponent<UIEventListener>();
			try
			{
				if (component.onClick != null)
				{
					component.onClick.Invoke(this.items[0].BtnItem);
				}
				if (this.items[0].m_ListTabBtn != null && this.items[0].m_ListTabBtn.Count() > 0)
				{
					this.items[0].SetUIState(0);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}
	}

	private void setTab(int index = -1)
	{
		if (index >= 0)
		{
			if (this.selectedIndex >= 0)
			{
				if (index == this.selectedIndex)
				{
					this.items[this.selectedIndex].ToggleState = !this.items[this.selectedIndex].ToggleState;
					return;
				}
				if (this.items[this.selectedIndex].ToggleState)
				{
					this.items[this.selectedIndex].ToggleState = false;
				}
			}
			this.selectedIndex = index;
			this.items[this.selectedIndex].ToggleState = true;
		}
	}

	private void FirstTabListSelectChange(object sender, MouseEvent e)
	{
		if (this.ListboxFirst.SelectedIndex == -1)
		{
			return;
		}
		GameObject at = this.mOBCFirstTabList.GetAt(this.ListboxFirst.SelectedIndex);
		XuanFuLabItem component = at.GetComponent<XuanFuLabItem>();
		if (component != null)
		{
			this.SetBottomSelectedFirstLevelData(component.firstLevelData);
		}
		NGUITools.SetActive(this.PanelFirstLevel, !this.PanelFirstLevel.gameObject.activeInHierarchy);
		if (this.PanelFirstLevel.gameObject.activeInHierarchy)
		{
			this.FirstBottomBtnBackground.spriteName = "arrow01";
		}
		else
		{
			this.FirstBottomBtnBackground.spriteName = "arrow02";
		}
	}

	private void SecondTabListSelectChange(object sender, MouseEvent e)
	{
		if (this.ListboxSecond.SelectedIndex == -1)
		{
			return;
		}
		GameObject at = this.mOBCSecondTabList.GetAt(this.ListboxSecond.SelectedIndex);
		XuanFuLabItem component = at.GetComponent<XuanFuLabItem>();
		if (component != null)
		{
			this.SetBottomSelectedSecondLevelData(component.secondLevelData);
		}
		NGUITools.SetActive(this.PanelSecondLevel, !this.PanelSecondLevel.gameObject.activeInHierarchy);
		if (this.PanelSecondLevel.gameObject.activeInHierarchy)
		{
			this.SecondBottomBtnBackground.spriteName = "arrow01";
		}
		else
		{
			this.SecondBottomBtnBackground.spriteName = "arrow02";
		}
	}

	public void TestCreateDate()
	{
	}

	public const int ONLINE_SERVER_FIRST_ID = 10;

	public const int ONLINE_SERVER_SECOND_ID = 100;

	public TextBlock[] staticText;

	public GButton m_EnterServer;

	public GButton m_BtnClose;

	public UILabel m_LabelInput;

	public UILabel m_EnterServerLabel;

	public UILabel m_inputDefaultLabel;

	public GButton m_BtnRecordServer;

	public UILabel m_BtnRecordServerLabel;

	public TextBox ServerTextBox;

	private XuanFuTotalServerItem m_currentSelectedItem;

	private XuanFuTotalServerItem m_lastSelectedItem;

	public UIPanel PanelTotalServerItem;

	public UITable m_ListTotalServerItem;

	public UITable m_ListEachServerItem;

	public XuanFuServerData ServerData;

	private bool IsFirstRender;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject PnlItemServers;

	public GameObject FirstLevelObj;

	public GameObject PanelFirstLevel;

	public ListBox ListboxFirst;

	public UILabel FirstLevelSelectLabel;

	private ObservableCollection mOBCFirstTabList;

	public UIButton FirstBottomBtn;

	public UISprite FirstBottomBtnBackground;

	public GameObject PanelSecondLevel;

	public GameObject SecondLevelObj;

	public ListBox ListboxSecond;

	public UILabel SecondLevelSelectLabel;

	public UILabel LabelSecond;

	private ObservableCollection mOBCSecondTabList;

	public UIButton SecondBottomBtn;

	public UISprite SecondBottomBtnBackground;

	public GameObject InputObj;

	private FistLevelServerListData bottomSelectedFirstLevelData;

	private SecondLevelServerListData bottomSelectedSecondLevelData;

	private XuanFuTotalServerOneLevelItem[] items;

	private int selectedIndex = -1;

	private bool ResetPanelTotalServerItemPos;

	private int ResetPosCounter;
}
