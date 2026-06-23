using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class Team_Part_MyTeam : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnCreateTeam.Text = Global.GetLang("创建队伍");
		this.btnQuitTeam.Text = Global.GetLang("退出队伍");
		this.cbAllowGroup.Text = Global.GetLang("自动接受组队申请");
		this.cbAutoAcceptAsLeader.Text = Global.GetLang("自动接受入队申请");
		this.PickupTypetext.Add(Global.GetLang("伤害拾取分配"));
		this.PickupTypetext.Add(Global.GetLang("队伍随机分配"));
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.thisCtrl = this;
		this.ItemCollection = this.lbMyTeam.ItemsSource;
		this.lbMyTeam.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this.lbMyTeam.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.mOBCPickupTabList = this.ListboxPickup.ItemsSource;
		this.ListboxPickup.SelectionChanged = new MouseLeftButtonUpEventHandler(this.PickupTabListSelectChange);
		UIEventListener.Get(this.PickupBottomBtn.gameObject).onClick = delegate(GameObject s1)
		{
			NGUITools.SetActive(this.PanelPickupType, !this.PanelPickupType.gameObject.activeInHierarchy);
			if (this.PanelPickupType.gameObject.activeInHierarchy)
			{
				this.PickupBottomBtnBackground.spriteName = "arrow01";
			}
			else
			{
				this.PickupBottomBtnBackground.spriteName = "arrow02";
			}
		};
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

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.InitCheckBox();
		this.InitButton();
		this.InitPickupList();
	}

	public void GetNewData()
	{
		this.SetButtonState(this.IsHearoInTeam());
		this.SetButtonEnable(this.IsTeamLeader());
		this.lbMyTeam_Bind();
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (null != this.lbMyTeam.LastSelectedItem && null != this.lbMyTeam.SelectedItem && this.lbMyTeam.LastSelectedItem != this.lbMyTeam.SelectedItem)
		{
			TeamPartItem teamPartItem = U3DUtils.AS<TeamPartItem>(this.lbMyTeam.LastSelectedItem);
			if (null != teamPartItem)
			{
				teamPartItem.SelectedState = false;
			}
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitCheckBox()
	{
		this.cbAllowGroup.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			Global.Data.SysSetting.AutoAcceptTeamInvite = this.cbAllowGroup.Check;
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 10,
					IDType = 0
				});
			}
		};
		this.cbAllowGroup.Check = Global.Data.SysSetting.AutoAcceptTeamInvite;
		this.cbAutoAcceptAsLeader.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			Global.Data.SysSetting.AutoAcceptTeamApply = this.cbAutoAcceptAsLeader.Check;
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 10,
					IDType = 0
				});
			}
		};
		this.cbAutoAcceptAsLeader.Check = Global.Data.SysSetting.AutoAcceptTeamApply;
	}

	private void InitPickupList()
	{
		this.mOBCPickupTabList.Clear();
		int count = this.PickupTypetext.Count;
		for (int i = 0; i < count; i++)
		{
			PickupTypeLabItem pickupTypeLabItem = U3DUtils.NEW<PickupTypeLabItem>();
			pickupTypeLabItem.labText.text = this.PickupTypetext[i];
			this.mOBCPickupTabList.AddNoUpdate(pickupTypeLabItem);
		}
		NGUITools.SetActive(this.PanelPickupType, false);
		if (this.PanelPickupType.gameObject.activeInHierarchy)
		{
			this.PickupBottomBtnBackground.spriteName = "arrow01";
		}
		else
		{
			this.PickupBottomBtnBackground.spriteName = "arrow02";
		}
		this.PickTypeIndex = (int)ConfigSystemParam.GetSystemParamIntByName("TeamPickTypeIndex");
		if (this.PickTypeIndex == -1)
		{
			this.PickTypeIndex = 1;
		}
		if (count > 0 && Global.Data != null && Global.Data.SysSetting != null && Global.Data.SysSetting.TeamPartAutoPickUp < count)
		{
			this.PickTypeIndex = Global.Data.SysSetting.TeamPartAutoPickUp;
		}
		this.SetBottomSelectedPickupTypeData(this.PickTypeIndex);
	}

	private void InitButton()
	{
		this.btnCreateTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0,
					GetThingOpt = this.PickTypeIndex
				});
			}
		};
		this.btnQuitTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
		};
	}

	private void SetButtonState(bool alreadyInTeam)
	{
		this.btnCreateTeam.gameObject.SetActive(!alreadyInTeam);
		this.btnQuitTeam.gameObject.SetActive(alreadyInTeam);
	}

	private void SetButtonEnable(bool isTeamLeader)
	{
	}

	private void SetBottomSelectedPickupTypeData(int pickpTypeSelect)
	{
		if (pickpTypeSelect >= 0 && pickpTypeSelect < this.PickupTypetext.Count)
		{
			this.PickupTypeSelectLabel.text = this.PickupTypetext[pickpTypeSelect];
		}
	}

	private void lbMyTeam_SelectionChanged(object sender, MouseEvent e)
	{
		if (null != this.CurrentSelectedItem)
		{
			this.CurrentSelectedItem.BodyBackground = null;
		}
		Team_Part_MyTeam_Item team_Part_MyTeam_Item = U3DUtils.AS<Team_Part_MyTeam_Item>(this.lbMyTeam.SelectedItem);
		this.CurrentSelectedItem = team_Part_MyTeam_Item;
		if (null == team_Part_MyTeam_Item)
		{
			return;
		}
	}

	private void PickupTabListSelectChange(object sender, MouseEvent e)
	{
		if (this.ListboxPickup.SelectedIndex == -1)
		{
			return;
		}
		GameObject at = this.mOBCPickupTabList.GetAt(this.ListboxPickup.SelectedIndex);
		PickupTypeLabItem component = at.GetComponent<PickupTypeLabItem>();
		if (component != null)
		{
			if (Global.Data.CurrentTeamData != null)
			{
				if (Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
				{
					Global.Data.SysSetting.TeamPartAutoPickUp = this.ListboxPickup.SelectedIndex;
					this.PickTypeIndex = this.ListboxPickup.SelectedIndex;
					GameInstance.Game.SpriteTeam(11, Global.Data.roleData.RoleID, Global.Data.SysSetting.TeamPartAutoPickUp);
					this.SetBottomSelectedPickupTypeData(this.ListboxPickup.SelectedIndex);
				}
				else
				{
					Super.HintMainText(Global.GetLang("只有队伍的队长才能修改拾取选项"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("队伍信息不存在"), 10, 3);
			}
		}
		NGUITools.SetActive(this.PanelPickupType, !this.PanelPickupType.gameObject.activeInHierarchy);
		if (this.PanelPickupType.gameObject.activeInHierarchy)
		{
			this.PickupBottomBtnBackground.spriteName = "arrow01";
		}
		else
		{
			this.PickupBottomBtnBackground.spriteName = "arrow02";
		}
	}

	public bool IsHearoInTeam()
	{
		return Global.Data.CurrentTeamData != null;
	}

	public bool IsTeamLeader()
	{
		return Global.Data.CurrentTeamData != null && Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.roleData.RoleID;
	}

	public void lbMyTeam_Bind()
	{
		this.ItemCollection.Clear();
		if (Global.Data.CurrentTeamData == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.CurrentTeamData.TeamRoles.Count; i++)
		{
			if (Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.CurrentTeamData.TeamRoles[i].RoleID)
			{
				TeamMemberData teamMemberData = Global.Data.CurrentTeamData.TeamRoles[i];
				Global.Data.CurrentTeamData.TeamRoles[i] = Global.Data.CurrentTeamData.TeamRoles[0];
				Global.Data.CurrentTeamData.TeamRoles[0] = teamMemberData;
				break;
			}
		}
		for (int j = 0; j < Global.Data.CurrentTeamData.TeamRoles.Count; j++)
		{
			TeamPartItem teamPartItem = U3DUtils.NEW<TeamPartItem>();
			this.ItemCollection.AddNoUpdate(teamPartItem);
			if (Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.CurrentTeamData.TeamRoles[j].RoleID)
			{
				teamPartItem.IsCaptaion = true;
			}
			teamPartItem.PersonImg = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(Global.Data.CurrentTeamData.TeamRoles[j].Occupation),
				Global.Data.CurrentTeamData.TeamRoles[j].RoleSex
			});
			teamPartItem.RoleID = Global.Data.CurrentTeamData.TeamRoles[j].RoleID;
			teamPartItem.PersonName = Global.Data.CurrentTeamData.TeamRoles[j].RoleName;
			teamPartItem.PersonLevel = string.Concat(new object[]
			{
				"LV",
				Global.Data.CurrentTeamData.TeamRoles[j].Level,
				"[",
				Global.Data.CurrentTeamData.TeamRoles[j].ChangeLifeLev,
				Global.GetLang("转]")
			});
			teamPartItem.PersonForce = Global.GetLang("战力：") + Global.Data.CurrentTeamData.TeamRoles[j].CombatForce;
			teamPartItem.TeamName = ConfigSettings.GetMapNameByCode(Global.Data.CurrentTeamData.TeamRoles[j].MapCode, false);
			teamPartItem.PageType = 0;
			teamPartItem.IsTeamLeader = this.IsTeamLeader();
			teamPartItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(s, e);
				}
			};
			UIPanel component = teamPartItem.transform.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		this.ItemCollection.DelayUpdate();
		if (this.ItemCollection.Count > 0)
		{
			this.lbMyTeam.SelectedIndex = 0;
		}
	}

	private void ShowInfo()
	{
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
		Super.CloseChildWindow(this.Root, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.Root.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Root.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	public void RefreshData()
	{
		this.SetButtonState(this.IsHearoInTeam());
		this.SetButtonEnable(this.IsTeamLeader());
		this.lbMyTeam_Bind();
		this.InitCheckBox();
		this.InitPickupList();
	}

	public void RefreshSearchData(List<SearchRoleData> searchRoleDataList)
	{
		if (null != this.teamAddMember)
		{
			this.teamAddMember.RefreshSearchData(searchRoleDataList);
		}
	}

	private const float POPUP_BUTTON_WIDTH = 107f;

	private const float POPUP_BUTTON_HEIGHT = 48f;

	public GCheckBox cbAllowGroup;

	public GCheckBox cbAutoAcceptAsLeader;

	public GButton btnCreateTeam;

	public GButton btnQuitTeam;

	public GButton btnDetailInfo;

	public GameObject btnGroup;

	public ListBox lbMyTeam;

	public GameObject PickupTypeObj;

	public GameObject PanelPickupType;

	public ListBox ListboxPickup;

	public UILabel PickupTypeSelectLabel;

	private ObservableCollection mOBCPickupTabList;

	public UIButton PickupBottomBtn;

	public UISprite PickupBottomBtnBackground;

	private List<string> PickupTypetext = new List<string>();

	private bool FirstInitPartData = true;

	private Team_Part_MyTeam_Item CurrentSelectedItem;

	private Team_Part_MyTeam_AddMember teamAddMember;

	private List<GChildWindow> ChildWindowList;

	private Canvas PlaceHolder;

	private Canvas Root;

	private SpriteSL thisCtrl = new SpriteSL();

	private int PickTypeIndex = 1;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;
}
