using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamGroupBox : UserControl
{
	public string RoleName
	{
		get
		{
			return this._roleName;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.thisCtrl = this;
	}

	public void InitPartSize(int width, int height)
	{
	}

	private FacePlate AddObjectRoleFace(TeamMemberData tmd)
	{
		return null;
	}

	public void ShowContent(bool beShow)
	{
		this.m_panel.enabled = beShow;
	}

	public void HideMenu()
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private void ShowObjectRoleFace(FacePlate objectRoleFace, TeamMemberData tmd, double roleLifeV, int roleMaxLifeV, int roleMagicV, int roleMaxMagicV)
	{
	}

	public void UpdateTeamRoleData(int injuredRoleID, double injuredRoleLifeV, int injuredRoleMaxLifeV, int injuredRoleMagicV, int injuredRoleMaxMagicV)
	{
		if (!this.Visibility)
		{
			return;
		}
		if (Global.Data.CurrentTeamData == null)
		{
			return;
		}
		if (null == this.ContainPanel)
		{
			return;
		}
		for (int i = 0; i < this.ContainPanel.Children.Length(); i++)
		{
			FacePlate facePlate = U3DUtils.AS<FacePlate>(this.ContainPanel.getChildAt(i));
			if (!(null == facePlate))
			{
				if (injuredRoleID == facePlate.RoleID)
				{
					TeamMemberData teamMemberData = facePlate.ItemObject as TeamMemberData;
					if (teamMemberData != null)
					{
						this.ShowObjectRoleFace(facePlate, teamMemberData, injuredRoleLifeV, injuredRoleMaxLifeV, injuredRoleMagicV, injuredRoleMaxMagicV);
						break;
					}
				}
			}
		}
	}

	public void UpdateTeamRoleDataLevel(int roleID, int level, int zhuanShengLevel)
	{
		if (!this.Visibility)
		{
			return;
		}
		if (Global.Data.CurrentTeamData == null)
		{
			return;
		}
		if (null == this.ContainPanel)
		{
			return;
		}
		for (int i = 0; i < this.ContainPanel.Children.Length(); i++)
		{
			FacePlate facePlate = U3DUtils.AS<FacePlate>(this.ContainPanel.getChildAt(i));
			if (!(null == facePlate))
			{
				if (roleID == facePlate.RoleID)
				{
					if (SceneUIClasses.RebornMap.IsTheScene())
					{
						facePlate.VLevel = StringUtil.substitute(Global.GetLang("重生{0}"), new object[]
						{
							Global.Data.roleData.RebornLevel.ToString()
						});
					}
					else
					{
						facePlate.VLevel = StringUtil.substitute(Global.GetLang("{0} [{1}转]"), new object[]
						{
							level,
							zhuanShengLevel
						});
					}
					break;
				}
			}
		}
	}

	public void UpdateTeamRoleFaces()
	{
		if (Global.Data.CurrentTeamData == null)
		{
			this.MemberNum = 0;
			return;
		}
		if (Global.Data.CurrentTeamData.TeamRoles.Count > 0)
		{
			this.MemberNum = Global.Data.CurrentTeamData.TeamRoles.Count;
		}
		if (!Global.Data.SysSetting.HideTeamMembersFace)
		{
		}
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Root, noBorderWindow);
	}

	public void ShowMenuWindow(int px, int py, int[] ids, string[] names)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			if (ids[i] != 5)
			{
				this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
			}
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)px;
		this.MenuWindow.Top = (double)py;
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 120.0;
		this.MenuWindow.BodyHeight = (double)((this.menuPart.ItemCount + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Root.Children.Add(this.MenuWindow);
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		this.menuPart.RenderMenu(21);
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GMenuItem gmenuItem = s as GMenuItem;
			if (null == gmenuItem)
			{
				return;
			}
			this.ProcessMenuClick(gmenuItem.MenuItemID);
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void ProcessMenuClick(int id)
	{
		if (null == this.SelectedObjectRoleFace)
		{
			return;
		}
		if (id == 0)
		{
			if (Global.Data.CurrentTeamData != null && Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteTeam(8, this.SelectedObjectRoleFace.RoleID, 0);
			}
		}
		else if (id == 1)
		{
			GameInstance.Game.SpriteGetOtherAttrib(this.SelectedObjectRoleFace.RoleID);
		}
		else if (id == 2)
		{
			if (this.DPSelectedItem != null)
			{
				TeamMemberData teamMemberData = (TeamMemberData)this.SelectedObjectRoleFace.ItemObject;
				if (teamMemberData != null)
				{
					this._roleName = teamMemberData.RoleName;
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = id,
						ID = this.SelectedObjectRoleFace.RoleID
					});
				}
			}
		}
		else if (id == 3)
		{
			try
			{
				TeamMemberData teamMemberData2 = (TeamMemberData)this.SelectedObjectRoleFace.ItemObject;
				if (teamMemberData2 != null)
				{
				}
			}
			catch (Exception ex)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("调用系统剪贴板功能出错!"), new object[0]), 0, -1, -1, 0);
				MUDebug.LogException(ex);
			}
		}
		else if (id == 4)
		{
			TeamMemberData teamMemberData3 = (TeamMemberData)this.SelectedObjectRoleFace.ItemObject;
			if (teamMemberData3 != null)
			{
				FriendData friendData = Global.FindFriendData(teamMemberData3.RoleName);
				if (friendData == null)
				{
					GameInstance.Game.SpriteAddFriend(-1, teamMemberData3.RoleName, 0);
				}
				else if (friendData.FriendType != 0)
				{
					GameInstance.Game.SpriteAddFriend(friendData.DbID, friendData.OtherRoleName, 0);
				}
				else
				{
					string lang = Global.GetLang("好友");
					if (friendData.FriendType == 1)
					{
						lang = Global.GetLang("屏蔽");
					}
					else if (friendData.FriendType == 2)
					{
						lang = Global.GetLang("仇人");
					}
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经在{1}列表中"), new object[]
					{
						teamMemberData3.RoleName,
						lang
					}), 0, -1, -1, 0);
				}
			}
		}
		else if (id == 5)
		{
			GameInstance.Game.SpriteQueryIDByName(this.SelectedObjectRoleFace.VSName, 4);
		}
	}

	protected override void InitializeComponent()
	{
		UIEventListener.Get(this.TeamIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (!this.m_panel.enabled)
			{
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 10
			});
		};
	}

	public void ShowTeam()
	{
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			IDType = 10
		});
	}

	public int MemberNum
	{
		set
		{
			if (value == 0)
			{
				this.TeamMemberNum.Text = string.Empty;
			}
			else
			{
				this.TeamMemberNum.Text = string.Format("X{0}", value);
			}
		}
	}

	public void PlayAnimation()
	{
		Animation component = this.TeamIcon.gameObject.transform.GetComponent<Animation>();
		if (component != null)
		{
			component.Play();
		}
	}

	public void StopAnimation()
	{
		Animation component = this.TeamIcon.gameObject.transform.GetComponent<Animation>();
		if (component != null)
		{
			component.Stop();
		}
		this.TeamIcon.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public UIButton TeamIcon;

	public TextBlock TeamMemberNum;

	public UIPanel m_panel;

	private UserControl thisCtrl;

	private string _roleName = string.Empty;

	private FacePlate SelectedObjectRoleFace;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private int[] LeaderMenuItemIDs = new int[]
	{
		0,
		1,
		2,
		4,
		5
	};

	private string[] LeaderMenuItemNames = new string[]
	{
		Global.GetLang("踢出队伍"),
		Global.GetLang("查看装备"),
		Global.GetLang("私聊"),
		Global.GetLang("加为好友"),
		Global.GetLang("查看摊位")
	};

	private int[] MemeberMenuItemIDs = new int[]
	{
		1,
		2,
		4,
		5
	};

	private string[] MemeberMenuItemNames = new string[]
	{
		Global.GetLang("查看装备"),
		Global.GetLang("私聊"),
		Global.GetLang("加为好友"),
		Global.GetLang("查看摊位")
	};

	private Canvas Root;

	private StackPanel ContainPanel = new StackPanel();

	public DPSelectedItemEventHandler DPSelectedItem;
}
