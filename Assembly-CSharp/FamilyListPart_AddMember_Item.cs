using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class FamilyListPart_AddMember_Item : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this._RoleName;
		}
		set
		{
			this._RoleName = value;
		}
	}

	public int RoleLevel
	{
		get
		{
			return this._RoleLevel;
		}
		set
		{
			this._RoleLevel = value;
		}
	}

	public int Faction
	{
		get
		{
			return this._Faction;
		}
		set
		{
			this._Faction = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
			base.BackgroundAlpha = 0.4;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
		base.BackgroundAlpha = 0.01;
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
			}
		}
	}

	public UILabel lblName;

	public UILabel lblOcupation;

	public UILabel lblLevel;

	public UILabel lblDuty;

	public UILabel lblForce;

	public UISprite texSelected;

	public GButton btnDetail;

	public GButton btnChat;

	public GButton btnAddFriend;

	public Image imgAvatar;

	public GTextBlockOutLine gtbPlayerName;

	public GTextBlockOutLine gtbLevel;

	public GTextBlockOutLine gtbWork;

	private int _RoleID;

	private string _RoleName;

	private int _RoleLevel;

	private int _Faction;

	private bool _SelectedState;
}
