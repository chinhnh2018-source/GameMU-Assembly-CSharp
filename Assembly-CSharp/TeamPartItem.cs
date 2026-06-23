using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class TeamPartItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public string PersonImg
	{
		get
		{
			return this.texPersonImg.ImageURL;
		}
		set
		{
			this.texPersonImg.ImageURL = value;
			this.texPersonImg.ForceShow();
		}
	}

	public bool IsCaptaion
	{
		get
		{
			return this.texCaptaionImg.gameObject.activeInHierarchy;
		}
		set
		{
			this.texCaptaionImg.gameObject.SetActive(value);
		}
	}

	public string PersonName
	{
		get
		{
			return this.lblPersonName.text;
		}
		set
		{
			this.lblPersonName.text = value;
		}
	}

	public string PersonLevel
	{
		get
		{
			return this.lblLevel.text;
		}
		set
		{
			this.lblLevel.text = value;
		}
	}

	public string PersonForce
	{
		get
		{
			return this.lblForce.text;
		}
		set
		{
			this.lblForce.text = value;
		}
	}

	public string TeamName
	{
		get
		{
			return this.lblTeamName.text;
		}
		set
		{
			this.lblTeamName.text = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._roleID;
		}
		set
		{
			this._roleID = value;
		}
	}

	public bool SelectedState
	{
		get
		{
			return this._SelectedState;
		}
		set
		{
			this._SelectedState = value;
			if (null != this.texSelected)
			{
				NGUITools.SetActiveSelf(this.texSelected.gameObject, this._SelectedState);
			}
			if (!this._SelectedState)
			{
				if (this.menuPart != null)
				{
					this.menuPart.Visibility = false;
				}
			}
			else
			{
				this.CreateMenuWindow();
			}
		}
	}

	public bool IsTeamLeader
	{
		get
		{
			return this._isTeamLeader;
		}
		set
		{
			this._isTeamLeader = value;
		}
	}

	public int PageType
	{
		get
		{
			return this._pageType;
		}
		set
		{
			this._pageType = value;
		}
	}

	public void OnClick()
	{
		NGUITools.SetActiveSelf(this.texSelected.gameObject, true);
		if (this.RoleID != Global.Data.roleData.RoleID)
		{
			this.CreateMenuWindow();
		}
	}

	public void CreateMenuWindow()
	{
		if (null != this.menuPart)
		{
			if (this.menuPart.Visibility)
			{
				this.menuPart.Visibility = false;
			}
			else
			{
				this.ResetMenuPartPos();
				this.menuPart.Visibility = true;
			}
			return;
		}
		this.menuPart = U3DUtils.NEW<GTxtMenuPart>();
		this.menuPart.Width = 150.0;
		this.menuPart.ItemHeight = 35;
		this.menuPart.AddMenuItem(0, Global.GetLang("查 看"));
		this.menuPart.AddMenuItem(1, Global.GetLang("私 聊"));
		this.menuPart.AddMenuItem(2, Global.GetLang("加为好友"));
		switch (this.PageType)
		{
		case 0:
			if (this.IsTeamLeader)
			{
				this.menuPart.AddMenuItem(3, Global.GetLang("任命队长"));
				this.menuPart.AddMenuItem(4, Global.GetLang("踢出队伍"));
			}
			break;
		case 1:
			this.menuPart.AddMenuItem(5, Global.GetLang("申请入队"));
			break;
		case 2:
			this.menuPart.AddMenuItem(6, Global.GetLang("邀请组队"));
			break;
		}
		this.menuPart.RenderMenu();
		this.menuPart.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != this.menuPart)
			{
				NGUITools.Destroy(this.menuPart.gameObject);
			}
			this.menuPart = null;
		};
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GTxtMenuItem gtxtMenuItem = s as GTxtMenuItem;
			if (null == gtxtMenuItem)
			{
				return;
			}
			if (gtxtMenuItem.MenuItemID >= 0 && gtxtMenuItem.MenuItemID <= 6)
			{
				this.ItemFunction(gtxtMenuItem.MenuItemID);
			}
			this.menuPart.Visibility = false;
		};
		U3DUtils.AddChild(base.transform.gameObject, this.menuPart.gameObject, true);
		this.ResetMenuPartPos();
		this.menuPart.SelectIndex = -1;
	}

	private void ResetMenuPartPos()
	{
		Vector3 zero = Vector3.zero;
		if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 754f)
		{
			zero..ctor(-190f, 97f, -1f);
		}
		else
		{
			zero..ctor(39f, 97f, -1f);
		}
		if (this.menuPart != null)
		{
			this.menuPart.transform.localPosition = zero;
		}
	}

	private void ItemFunction(int idx)
	{
		switch (idx)
		{
		case 0:
			if (this.RoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.RoleID);
			}
			break;
		case 1:
			if (this.RoleID != Global.Data.roleData.RoleID && this.ShowChatBoxCallback != null)
			{
				this.ShowChatBoxCallback(this, new DPSelectedItemEventArgs
				{
					IDType = 1,
					Title = this.PersonName
				});
			}
			break;
		case 2:
			if (this.RoleID != Global.Data.roleData.RoleID)
			{
				if (Global.FindFriendData(this.PersonName) == null)
				{
					GameInstance.Game.SpriteAddFriend(-1, this.PersonName, 0);
				}
				else
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("【{0}】已经在好友列表中"), new object[]
					{
						this.PersonName
					}), 10, 3);
				}
			}
			break;
		case 3:
			if (Global.Data.CurrentTeamData != null && Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteTeam(10, this.RoleID, 0);
			}
			break;
		case 4:
			if (Global.Data.CurrentTeamData != null && Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteTeam(8, this.RoleID, 0);
			}
			break;
		case 5:
			if (Global.Data.CurrentTeamData != null)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("抱歉，自已已有队伍，无法再申请其它队伍。"), new object[0]), 0, -1, -1, 0);
				Super.HintMainText(Global.GetLang("抱歉，自已已有队伍，无法再申请其它队伍。"), 10, 3);
				return;
			}
			GameInstance.Game.SpriteTeam(4, this.RoleID, 0);
			Super.HintMainText(StringUtil.substitute(Global.GetLang("申请入队请求已发出，请等待队长的回复。"), new object[0]), 10, 3);
			break;
		case 6:
			if (this.RoleID != Global.Data.roleData.RoleID)
			{
				if (Global.Data.CurrentTeamData != null && Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
				{
					GameInstance.Game.SpriteTeam(3, this.RoleID, 0);
					Super.HintMainText(StringUtil.substitute(Global.GetLang("邀请组队请求已发出，请等待对方的回复。"), new object[0]), 10, 3);
				}
				else
				{
					Super.HintMainText(Global.GetLang("先创建自己的队伍后，才能邀请他人"), 10, 3);
				}
			}
			break;
		}
	}

	public ShowNetImage texPersonImg;

	public UISprite texCaptaionImg;

	public UISprite texSelected;

	public UILabel lblPersonName;

	public UILabel lblLevel;

	public UILabel lblForce;

	public UILabel lblTeamName;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	private int _roleID = -1;

	private bool _SelectedState;

	private bool _isTeamLeader = true;

	private int _pageType;

	public GTxtMenuPart menuPart;
}
