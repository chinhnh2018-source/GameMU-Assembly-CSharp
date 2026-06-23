using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class Team_Part_Request_Item : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnAgree.Text = Global.GetLang("同意");
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.InitTextInPrefabs();
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

	public TeamUIItem CurrentTeamUIItem
	{
		get
		{
			return this._CurrentTeamUIItem;
		}
		set
		{
			this._CurrentTeamUIItem = value;
			this.texPersonImg.URL = StringUtil.substitute("NetImages/Face/{0}0_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(value.Occupation)
			});
			this.lblPersonName.text = value.OtherRoleName;
			this.lblPersonLevel.text = string.Concat(new object[]
			{
				"LV",
				value.Level.ToString(),
				"[",
				value.ChangeLifeLevel,
				Global.GetLang("转]")
			});
		}
	}

	public int ID
	{
		get
		{
			return this._ID;
		}
		set
		{
			this._ID = value;
		}
	}

	public void InitButtons()
	{
		this.btnAgree.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteTeam((int)this.CmdType, this.CurrentTeamUIItem.OtherRoleID, 0);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.CurrentTeamUIItem.OtherRoleID
				});
			}
		};
	}

	public TeamCmds CmdType
	{
		get
		{
			return this._cmdType;
		}
		set
		{
			this._cmdType = value;
			this.InitButtons();
		}
	}

	public UILabel lblPersonName;

	public UILabel lblPersonLevel;

	public GButton btnAgree;

	public ShowNetImage texPersonImg;

	private Canvas Root;

	public GTextBlockOutLine gtbPlayerName;

	public GTextBlockOutLine gtbLevel;

	public SpriteSL thisCtrl;

	private TeamUIItem _CurrentTeamUIItem;

	private int _ID;

	public DPSelectedItemEventHandler DPSelectedItem;

	private TeamCmds _cmdType;
}
