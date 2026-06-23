using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class FamilyListPart : UserControl
{
	public bool HadToJionTheLeague { get; set; }

	private void InitTextInPrefabs()
	{
		this.btnShowCreate.Text = Global.GetLang("创建战盟");
		this.btnJoin.Text = Global.GetLang("申请加入");
		this.chkShowFilter.Text = Global.GetLang("仅显示自动收人的战盟");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.listBox.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonDown);
		UIDraggablePanel dragablepnl = this.listBox.Parent.GetComponent<UIDraggablePanel>();
		if (dragablepnl != null)
		{
			dragablepnl.onDragFinished = delegate()
			{
				if ((double)dragablepnl.verticalScrollBar.scrollValue > 0.9)
				{
					this.AddListItem();
				}
			};
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

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.btnShowCreate.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.IsHavingBangHui())
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, null);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("您已经加入战盟，无法再创建新的战盟!"), 10, 3);
			}
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.transform.parent = null;
			Object.Destroy(base.transform.gameObject);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, null);
			}
		};
		this.btnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.Level < Global.JoinBangHuiNeedLevel)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("角色等级达到【{0}】后，才能申请加入战盟"), new object[]
				{
					Global.JoinBangHuiNeedLevel
				}), 10, 3);
				return;
			}
			if (!Global.IsHavingBangHui())
			{
				if (null != this.SelectedListItem)
				{
					if (int.Parse(this.SelectedListItem.BHFamilyCount) >= 50)
					{
						string message = StringUtil.substitute(Global.GetLang("战盟成员人数已经达到上限，无法再申请加入!"), new object[0]);
						string[] buttons = new string[]
						{
							Global.GetLang("确定")
						};
						Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
						{
						}, buttons);
					}
					else
					{
						Global.SendEvent("701", Global.GetLang("申请战盟次数"));
						GameInstance.Game.SpriteApplyToBHMember(this.SelectedListItem.BHID, this.SelectedListItem.BHName);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("请先选中要加入的战盟!"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("你已经在战盟中，无法再加入新的战盟!"), 10, 3);
			}
		};
		this.chkShowFilter.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.SelectedMenuID = ((!this.chkShowFilter.Check) ? -1 : 0);
			this.listBox.Parent.GetComponent<UIDraggablePanel>().ResetPosition();
			this.GetNewData();
		};
	}

	public void InitPartData()
	{
	}

	private void ShowPage(int pageIndex)
	{
	}

	public void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.GetNewData();
		}
	}

	public void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.GetNewData();
		}
	}

	private void AddListItem()
	{
		int num = (this.listBox.Count() + 16 <= this.BangHuiItemDataList.Count) ? (this.listBox.Count() + 16) : this.BangHuiItemDataList.Count;
		for (int i = this.listBox.Count(); i < num; i++)
		{
			FamilyListItem familyListItem = U3DUtils.NEW<FamilyListItem>();
			familyListItem.BHID = this.BangHuiItemDataList[i].BHID;
			familyListItem.BZRoleID = this.BangHuiItemDataList[i].BZRoleID;
			familyListItem.BHName = Global.FormatBangHuiName(this.BangHuiItemDataList[i].ZoneID, this.BangHuiItemDataList[i].BHName);
			familyListItem.BZRoleName = Global.FormatRoleName(this.BangHuiItemDataList[i].ZoneID, this.BangHuiItemDataList[i].BZRoleName);
			familyListItem.BangQiLevel = this.BangHuiItemDataList[i].QiLevel;
			familyListItem.BHForce = string.Empty + this.BangHuiItemDataList[i].TotalCombatForce;
			familyListItem.BHFamilyCount = this.BangHuiItemDataList[i].TotalNum.ToString();
			if (this.luolanChengZhuID == familyListItem.BZRoleID)
			{
				familyListItem.huangGuan.URL = "NetImages/GameRes/Images/Plate/huangguan.png";
			}
			else
			{
				familyListItem.huangGuan.URL = string.Empty;
			}
			familyListItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(s, e);
				}
			};
			UIPanel component = familyListItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			this.ItemCollection.AddNoUpdate(familyListItem);
		}
	}

	private void FilterItemsList(int Type)
	{
		this.ItemCollection.Clear();
		if (this.BangHuiItemDataList != null)
		{
			for (int i = 0; i < this.BangHuiItemDataList.Count; i++)
			{
				FamilyListItem familyListItem = U3DUtils.NEW<FamilyListItem>();
				familyListItem.BHID = this.BangHuiItemDataList[i].BHID;
				familyListItem.BZRoleID = this.BangHuiItemDataList[i].BZRoleID;
				familyListItem.BHName = Global.FormatBangHuiName(this.BangHuiItemDataList[i].ZoneID, this.BangHuiItemDataList[i].BHName);
				familyListItem.BZRoleName = Global.FormatRoleName(this.BangHuiItemDataList[i].ZoneID, this.BangHuiItemDataList[i].BZRoleName);
				familyListItem.BangQiLevel = this.BangHuiItemDataList[i].QiLevel;
				familyListItem.BHForce = string.Empty + this.BangHuiItemDataList[i].TotalCombatForce;
				familyListItem.BHFamilyCount = this.BangHuiItemDataList[i].TotalNum.ToString();
				familyListItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.ShowChatBoxCallback != null)
					{
						this.ShowChatBoxCallback(s, e);
					}
				};
				UIPanel component = familyListItem.transform.GetComponent<UIPanel>();
				if (component != null)
				{
					Object.Destroy(component);
				}
				this.ItemCollection.AddNoUpdate(familyListItem);
			}
		}
	}

	private int ItemsList_Sort(FamilyListItem a, FamilyListItem b)
	{
		return b.BangQiLevel - a.BangQiLevel;
	}

	public void RefreshItemsList(List<BangHuiItemData> bangHuiItemDataList, int count)
	{
		this.listBox.Items.Clear();
		this.BangHuiItemDataList = bangHuiItemDataList;
		this.AddListItem();
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (null != this.listBox.LastSelectedItem && null != this.listBox.SelectedItem && this.listBox.LastSelectedItem != this.listBox.SelectedItem)
		{
			FamilyListItem familyListItem = U3DUtils.AS<FamilyListItem>(this.listBox.LastSelectedItem);
			if (null != familyListItem)
			{
				familyListItem.SelectedState = false;
			}
		}
		this.SelectedListItem = U3DUtils.AS<FamilyListItem>(this.listBox.SelectedItem);
	}

	public void SetBtnState(bool state)
	{
		this.FirstPage.EnableIcon = state;
		this.EndPage.EnableIcon = state;
		this.NextPageIcon.EnableIcon = state;
		this.PrevPageIcon.EnableIcon = state;
	}

	public void GetNewData()
	{
		int num = this.CurrentSelectedPage * this.NumOfPage;
		int endIndex = num + this.NumOfPage;
		GameInstance.Game.SpriteGetBangHuiListData(this.SelectedMenuID, num, endIndex);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void listBox_MouseLeftButtonDown(object sender, MouseEvent e)
	{
	}

	private void HideWindow()
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		this.HideWindow();
	}

	public void ShowMenuWindow(int px, int py, int[] ids, string[] names, int menus_id)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)px;
		this.MenuWindow.Top = (double)py;
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 120.0;
		this.MenuWindow.BodyHeight = (double)((ids.Length + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Container.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
		}
		this.menuPart.RenderMenu(21);
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GMenuItem gmenuItem = s as GMenuItem;
			if (null == gmenuItem)
			{
				return;
			}
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			if (menus_id == 0)
			{
				this.ProcessDropDownMenuClick(gmenuItem.MenuItemID);
			}
			else if (menus_id == 1)
			{
				this.ProcessPropMenuClick(gmenuItem.MenuItemID);
			}
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void ProcessDropDownMenuClick(int id)
	{
		this.SelectedMenuID = id;
		this.gtbDDL.EditText = this.tiMenuItemNames[this.SelectedMenuID];
		this.CurrentSelectedPage = 0;
		this.GetNewData();
	}

	private void ProcessPropMenuClick(int id)
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		if (id == 0)
		{
			GameInstance.Game.SpriteGetOtherAttrib(this.SelectedListItem.BZRoleID);
		}
		else if (id == 1)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = this.SelectedListItem.BHID
				});
			}
		}
		else if (id == 2)
		{
			if (Global.Data.roleData.RoleID != this.SelectedListItem.BZRoleID)
			{
				GameInstance.Game.SpriteAddFriend(this.SelectedListItem.BZRoleID, this.SelectedListItem.BZRoleName, 0);
			}
		}
		else if (id == 3)
		{
			if (Global.Data.roleData.Level < Global.JoinBangHuiNeedLevel)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("角色等级达到【{0}】后，才能申请加入战盟"), new object[]
				{
					Global.JoinBangHuiNeedLevel
				}), 10, 3);
				return;
			}
			if (!Global.IsHavingBangHui())
			{
				if (null != this.SelectedListItem)
				{
					string actionName = StringUtil.substitute("applytobh_{0}", new object[]
					{
						this.SelectedListItem.BHID
					});
					if (ActionCoolDownMgr.FindAction(actionName, 180000L))
					{
						Super.HintMainText(StringUtil.substitute(Global.GetLang("你已经申请过了加入【{0}】战盟， 正在等待管理员批准"), new object[]
						{
							this.SelectedListItem.BHName
						}), 10, 3);
					}
					else
					{
						ActionCoolDownMgr.AddAction(actionName);
						GameInstance.Game.SpriteApplyToBHMember(this.SelectedListItem.BHID, this.SelectedListItem.BHName);
						Super.HintMainText(StringUtil.substitute(Global.GetLang("你已经申请了加入【{0}】战盟中..."), new object[]
						{
							this.SelectedListItem.BHName
						}), 10, 3);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("请先选中要加入的战盟!"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("你已经在战盟中，无法再加入新的战盟!"), 10, 3);
			}
		}
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.alpha = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void CloseCreatFamilyWindow()
	{
		if (null != this.CreatFamilyPartWindow)
		{
			this.CloseModalDialog();
			this.creatfamilyPart.CleanUpChildWindows();
			this.creatfamilyPart = null;
			Super.CloseChildWindow(this.Container, this.CreatFamilyPartWindow);
			this.CreatFamilyPartWindow = null;
		}
	}

	private void ShowCreatFamilyWindow()
	{
		if (null != this.CreatFamilyPartWindow)
		{
			this.CloseCreatFamilyWindow();
			return;
		}
		this.CreatFamilyPartWindow = U3DUtils.NEW<GChildWindow>();
		Super.InitChildWindow1(this.CreatFamilyPartWindow, Global.GetLang("创建战盟"));
		this.Container.Children.Add(this.CreatFamilyPartWindow);
		this.creatfamilyPart = U3DUtils.NEW<CreatFamilyPart>();
		this.creatfamilyPart.InitPartSize((int)this.CreatFamilyPartWindow.BodyWidth - 18, (int)this.CreatFamilyPartWindow.BodyHeight - 9);
		this.creatfamilyPart.InitPartData();
		this.CreatFamilyPartWindow.SetContent(this.CreatFamilyPartWindow.BodyPresenter, this.creatfamilyPart, 9.0, 0.0, true);
		this.creatfamilyPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCreatFamilyWindow();
		};
	}

	public void NotifyBangHuiListData(BangHuiListData bangHuiListData)
	{
		if (bangHuiListData != null)
		{
			int totalBangHuiItemNum = bangHuiListData.TotalBangHuiItemNum;
			bangHuiListData.BangHuiItemDataList.Sort((BangHuiItemData a, BangHuiItemData b) => b.TotalCombatForce - a.TotalCombatForce);
			List<BangHuiItemData> bangHuiItemDataList = bangHuiListData.BangHuiItemDataList;
			this.RefreshItemsList(bangHuiItemDataList, totalBangHuiItemNum);
		}
	}

	public void NotifyCreateBangHuiResult(int retCode, int roleID, int bangHuiID)
	{
		if (null != this.creatfamilyPart)
		{
			this.creatfamilyPart.CleanUpLoadingWindow();
		}
		if (retCode < 0)
		{
			if (retCode == -1031)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("要创建的战盟名称已经存在，请输入其他名称重新创建"), new object[0]), 10, 3);
			}
			else
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("创建战盟时发生未知错误: {0}"), new object[]
				{
					retCode
				}), 10, 3);
			}
			return;
		}
		this.CloseCreatFamilyWindow();
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
	}

	public void ClearRequestedData()
	{
		this.RequestedBHDataDict.Clear();
	}

	public void OnClick()
	{
		FamilyListItem familyListItem = U3DUtils.AS<FamilyListItem>(this.listBox.SelectedItem);
		if (null != familyListItem)
		{
			familyListItem.SelectedState = false;
		}
	}

	private const int countPerPage = 16;

	public GButton btnShowCreate;

	public GButton btnJoin;

	public GButton btnClose;

	public GCheckBox chkShowFilter;

	public int luolanChengZhuID;

	public DPSelectedItemEventHandler DPSelectedItem;

	private GIcon NextPageIcon;

	private GIcon PrevPageIcon;

	private GIcon FirstPage;

	private GIcon EndPage;

	private int MaxPageCount;

	private int CurrentSelectedPage;

	private int NumOfPage = 11;

	private GTextBlock gtbDDL;

	private List<BangHuiItemData> BangHuiItemDataList;

	private FamilyListItem SelectedListItem;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	private Dictionary<int, string> RequestedBHDataDict = new Dictionary<int, string>();

	public ListBox listBox;

	private string[] tiMenuItemNames;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private int SelectedMenuID = -1;

	private Canvas PlaceHolder;

	private GChildWindow CreatFamilyPartWindow;

	private CreatFamilyPart creatfamilyPart;

	private ObservableCollection _ItemCollection;
}
