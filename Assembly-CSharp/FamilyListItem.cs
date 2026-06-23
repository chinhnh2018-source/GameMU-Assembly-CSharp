using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class FamilyListItem : UserControl
{
	protected override void InitializeComponent()
	{
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
			if (!this._SelectedState && this.menuPart != null)
			{
				this.menuPart.Visibility = false;
			}
		}
	}

	public int BHID
	{
		get
		{
			return this._BHID;
		}
		set
		{
			this._BHID = value;
		}
	}

	public string BHName
	{
		get
		{
			return this.lblLeagueName.text;
		}
		set
		{
			this.lblLeagueName.text = value;
		}
	}

	public int BangQiLevel
	{
		get
		{
			return this._BangQiLevel;
		}
		set
		{
			this._BangQiLevel = value;
			this.lblLeagueLevel.text = string.Empty + value;
		}
	}

	public int BZRoleID
	{
		get
		{
			return this._BZRoleID;
		}
		set
		{
			this._BZRoleID = value;
		}
	}

	public string BZRoleName
	{
		get
		{
			return this.lblLeaderName.text;
		}
		set
		{
			this._BZRoleName = value;
			this.lblLeaderName.text = value;
			float num = this.lblLeaderName.transform.localScale.x * this.lblLeaderName.relativeSize.x;
			float num2 = this.lblLeaderName.transform.localScale.y * this.lblLeaderName.relativeSize.y;
			if (null != this.texUnderLine)
			{
				this.texUnderLine.transform.localScale = new Vector3(num, num2, 0f);
			}
		}
	}

	public string BHForce
	{
		get
		{
			return this.lblLeagueForce.text;
		}
		set
		{
			this.lblLeagueForce.text = value;
		}
	}

	public string BHFamilyCount
	{
		get
		{
			string[] array = this.lblMemberCount.text.Split(new char[]
			{
				'/'
			});
			if (array.Length > 0)
			{
				return array[0];
			}
			return "0";
		}
		set
		{
			this.lblMemberCount.text = value + "/50";
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		this.SelectedRect.Visibility = true;
		base.buttonMode = true;
		base.BackgroundAlpha = 0.2;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (!this.SelectedState)
		{
			this.SelectedRect.Visibility = false;
		}
		base.buttonMode = false;
		base.BackgroundAlpha = 0.01;
	}

	public void OnClick()
	{
		if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 222f && UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX < 390f && this.BZRoleID != Global.Data.roleData.RoleID)
		{
			this.CreateMenuWindow();
		}
		NGUITools.SetActiveSelf(this.texSelected.gameObject, true);
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
		this.menuPart.SelectIndex = -1;
		U3DUtils.AddChild(base.transform.gameObject, this.menuPart.gameObject, true);
		this.ResetMenuPartPos();
	}

	private void ResetMenuPartPos()
	{
		if (this.menuPart != null)
		{
			Vector3 localPosition;
			localPosition..ctor(-88f, 24f, -1f);
			this.menuPart.transform.localPosition = localPosition;
		}
	}

	private void ItemFunction(int idx)
	{
		switch (idx)
		{
		case 0:
			if (this.BZRoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.BZRoleID);
			}
			break;
		case 1:
			if (this.ShowChatBoxCallback != null)
			{
				this.ShowChatBoxCallback(this, new DPSelectedItemEventArgs
				{
					IDType = 1,
					Title = this.lblLeaderName.text
				});
			}
			break;
		case 2:
			if (Global.FindFriendData(this.lblLeaderName.text) == null)
			{
				GameInstance.Game.SpriteAddFriend(-1, this.lblLeaderName.text, 0);
			}
			break;
		}
	}

	public UILabel lblLeagueName;

	public UILabel lblLeaderName;

	public UILabel lblMemberCount;

	public UILabel lblLeagueForce;

	public UILabel lblLeagueLevel;

	public UISprite texUnderLine;

	public ShowNetImage huangGuan;

	public UISprite texBak;

	public UISprite texSelected;

	private bool _SelectedState;

	public GTextBlockOutLine FamilNameText;

	public GTextBlockOutLine FamilMainNameText;

	public GTextBlockOutLine OccupationText;

	public GTextBlockOutLine FamilCount;

	public GTextBlockOutLine SumLeveText;

	public GTextBlockOutLine BangQiLeveText;

	public RectangleSL SelectedRect;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	private int _BHID;

	private int _BangQiLevel;

	private int _BZRoleID;

	private string _BZRoleName;

	public GTxtMenuPart menuPart;
}
