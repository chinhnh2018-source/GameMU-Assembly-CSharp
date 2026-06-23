using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FriendListItem : UserControl
{
	public bool SetFuMo
	{
		set
		{
			if (value)
			{
				this.btnFuMoZengSong.isEnabled = true;
			}
			else
			{
				this.btnFuMoZengSong.isEnabled = false;
			}
		}
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

	public string OtherRoleName
	{
		get
		{
			return this.lblUserName.text;
		}
		set
		{
			this.lblUserName.text = value;
		}
	}

	public string OtherRoleLevel
	{
		get
		{
			return this.lblUserLevel.text;
		}
		set
		{
			this.lblUserLevel.text = value;
		}
	}

	public string UserPosition
	{
		get
		{
			return this.lblPosition.text;
		}
		set
		{
			this.lblPosition.text = value;
		}
	}

	public string OtherRoleForce
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

	public int DbID
	{
		get
		{
			return this._DbId;
		}
		set
		{
			this._DbId = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleId;
		}
		set
		{
			this._RoleId = value;
		}
	}

	public int Occupation
	{
		get
		{
			return this._Occupation;
		}
		set
		{
			this._Occupation = value;
		}
	}

	public int OnlineState
	{
		get
		{
			return this._OnlineState;
		}
		set
		{
			this._OnlineState = value;
			if (value == 1)
			{
				this.texPersonImg.ToGrayBitmap = false;
			}
			else if (value == 0)
			{
				this.texPersonImg.ToGrayBitmap = true;
				this.UserPosition = Global.GetLang("离线");
			}
		}
	}

	public int FriendType
	{
		get
		{
			return this._FriendType;
		}
		set
		{
			this._FriendType = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.btnFuMoZengSong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SenUserFuMoGive(this.RoleID, this.OtherRoleName);
		};
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
			else if (this.RoleID != Global.Data.roleData.RoleID)
			{
				this.CreateMenuWindow();
			}
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
		this.menuPart.AddMenuItem(1, Global.GetLang("删 除"));
		switch (this.PageType)
		{
		case 0:
		{
			this.menuPart.AddMenuItem(2, Global.GetLang("私 聊"));
			this.menuPart.AddMenuItem(7, Global.GetLang("邀请战队"));
			this.menuPart.AddMenuItem(3, Global.GetLang("加为仇人"));
			this.menuPart.AddMenuItem(4, Global.GetLang("加为屏蔽"));
			FriendData friendData = Global.FindFriendData(this.OtherRoleName);
			if (friendData != null && friendData.SpouseId > 0)
			{
				this.menuPart.AddMenuItem(6, Global.GetLang("送TA祝福"));
			}
			break;
		}
		case 1:
			this.menuPart.AddMenuItem(5, Global.GetLang("加为好友"));
			this.menuPart.AddMenuItem(3, Global.GetLang("加为仇人"));
			break;
		case 2:
			this.menuPart.AddMenuItem(5, Global.GetLang("加为好友"));
			this.menuPart.AddMenuItem(4, Global.GetLang("加为屏蔽"));
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
			if (gtxtMenuItem.MenuItemID >= 0 && gtxtMenuItem.MenuItemID <= 7)
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
			if (this.RoleID != -1 && this.ShowConfirmWinCallback != null)
			{
				this.ShowConfirmWinCallback(this, new DPSelectedItemEventArgs
				{
					ID = this.DbID,
					Flag = 0
				});
			}
			break;
		case 2:
			if (this.ShowChatBoxCallback != null)
			{
				this.ShowChatBoxCallback(this, new DPSelectedItemEventArgs
				{
					ID = this.DbID,
					IDType = 1,
					Title = this.OtherRoleName
				});
			}
			break;
		case 3:
			if (this.RoleID != -1 && this.ShowConfirmWinCallback != null)
			{
				this.ShowConfirmWinCallback(this, new DPSelectedItemEventArgs
				{
					ID = this.DbID,
					IDType = 2,
					Title = this.OtherRoleName,
					Flag = 1
				});
			}
			break;
		case 4:
			if (this.RoleID != -1 && this.ShowConfirmWinCallback != null)
			{
				this.ShowConfirmWinCallback(this, new DPSelectedItemEventArgs
				{
					ID = this.DbID,
					IDType = 1,
					Title = this.OtherRoleName,
					Flag = 1
				});
			}
			break;
		case 5:
			if (this.RoleID != -1 && this.ShowConfirmWinCallback != null)
			{
				this.ShowConfirmWinCallback(this, new DPSelectedItemEventArgs
				{
					ID = this.DbID,
					IDType = 0,
					Title = this.OtherRoleName,
					Flag = 1
				});
			}
			break;
		case 6:
			if (this.OtherRoleName != null)
			{
				PlayZone.GlobalPlayZone.OpenLoversWishPartSendWishWindow(false, this.OtherRoleName, null, 0);
			}
			break;
		case 7:
			if (this.OtherRoleName != null && this.RoleID != Global.Data.roleData.RoleID)
			{
				TeamCompeteDataManager.SendInviteTeanMemberMsg(this.RoleID);
			}
			break;
		}
	}

	public void ChangeImage(string path)
	{
		if (null != this.SelectedBak)
		{
			this.SelectedBak.URL = path;
		}
	}

	public void ShowSelectedBak()
	{
		NGUITools.SetActiveSelf(this.SelectedBak.gameObject, this._SelectedState);
	}

	public double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
		}
	}

	public Brush BodyBackgournd
	{
		set
		{
			this.Root.Background = value;
		}
	}

	public int indexI
	{
		get
		{
			return this._indexI;
		}
		set
		{
			this._indexI = value;
		}
	}

	private bool _SelectedState;

	private Canvas Root;

	public ShowNetImage SelectedBak;

	public ShowNetImage texPersonImg;

	public UISprite texSelected;

	public UILabel lblUserName;

	public UILabel lblUserLevel;

	public UILabel lblPosition;

	public UILabel lblForce;

	public GButton btnFuMoZengSong;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	public DPSelectedItemEventHandler ShowConfirmWinCallback;

	private int _DbId = -1;

	private int _RoleId = -1;

	private int _Occupation = -1;

	private int _OnlineState = -1;

	private int _FriendType = -1;

	private int _pageType;

	public GTxtMenuPart menuPart;

	private int _indexI;
}
