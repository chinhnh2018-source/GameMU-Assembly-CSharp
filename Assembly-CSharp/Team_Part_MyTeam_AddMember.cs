using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class Team_Part_MyTeam_AddMember : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.lbPlayers);
		this.thisCtrl = this;
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

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public new void Awake()
	{
		this.ItemCollection = this.lbPlayers.ItemsSource;
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.InitCtr();
	}

	private void InitCtr()
	{
		this.btnSearch.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string key = Global.StringTrim(this.inputUserName.text);
			this.SearchPlayers(key);
		};
		this.btnDetailInfo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrentSelectedItem.RoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.CurrentSelectedItem.RoleID);
			}
		};
		this.btnInvite.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			if (this.CurrentSelectedItem == null)
			{
				return;
			}
			if (this.CurrentSelectedItem.RoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteTeam(3, this.CurrentSelectedItem.RoleID, 0);
			}
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
	}

	private void SearchPlayers(string key)
	{
		if (this.inputUserName.text.Length < 2)
		{
			return;
		}
		if (this.inputUserName.text.Length > 10)
		{
			return;
		}
		this.ItemCollection.Clear();
		this.CurrentSelectedItem = null;
		string text = Global.StringReplaceAll(key, "'", string.Empty);
		text = Global.StringReplaceAll(text, "|", string.Empty);
		text = Global.StringReplaceAll(text, "$", string.Empty);
		text = Global.StringReplaceAll(text, ":", string.Empty);
		GameInstance.Game.SpriteSearchRoles(text, 0);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void lbPlayers_SelectionChanged(object sender, MouseEvent e)
	{
		Team_Part_MyTeam_AddMember_Item team_Part_MyTeam_AddMember_Item = null;
		if (null != this.CurrentSelectedItem)
		{
			team_Part_MyTeam_AddMember_Item = U3DUtils.AS<Team_Part_MyTeam_AddMember_Item>(this.lbPlayers.LastSelectedItem);
			if (null != team_Part_MyTeam_AddMember_Item)
			{
				team_Part_MyTeam_AddMember_Item.SelectedState = false;
			}
		}
		Team_Part_MyTeam_AddMember_Item team_Part_MyTeam_AddMember_Item2 = U3DUtils.AS<Team_Part_MyTeam_AddMember_Item>(this.lbPlayers.SelectedItem);
		if (null == team_Part_MyTeam_AddMember_Item2)
		{
			return;
		}
		this.CurrentSelectedItem = team_Part_MyTeam_AddMember_Item2;
		team_Part_MyTeam_AddMember_Item.SelectedState = true;
	}

	private void SetButtonLight()
	{
		if (this.CurrentSelectedItem != null)
		{
			this.iconShowInfo.EnableIcon = true;
			this.iconToMember.EnableIcon = true;
		}
		else
		{
			this.iconShowInfo.EnableIcon = false;
			this.iconToMember.EnableIcon = false;
		}
	}

	public void RefreshSearchData(List<SearchRoleData> searchRoleDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (searchRoleDataList == null)
		{
			return;
		}
		for (int i = 0; i < searchRoleDataList.Count; i++)
		{
			Team_Part_MyTeam_AddMember_Item team_Part_MyTeam_AddMember_Item = U3DUtils.NEW<Team_Part_MyTeam_AddMember_Item>();
			team_Part_MyTeam_AddMember_Item.gtbPlayerName.Text = searchRoleDataList[i].RoleName;
			team_Part_MyTeam_AddMember_Item.gtbLevel.Text = searchRoleDataList[i].Level.ToString();
			team_Part_MyTeam_AddMember_Item.gtbWork.Text = Global.GetOccupationStr(searchRoleDataList[i].Occupation);
			team_Part_MyTeam_AddMember_Item.RoleID = searchRoleDataList[i].RoleID;
			team_Part_MyTeam_AddMember_Item.BodyHeight = 20.0;
			team_Part_MyTeam_AddMember_Item.BodyWidth = 230.0;
			this.ItemCollection.AddNoUpdate(team_Part_MyTeam_AddMember_Item);
		}
		this.ItemCollection.DelayUpdate();
	}

	public GButton btnSearch;

	public GButton btnDetailInfo;

	public GButton btnInvite;

	public GButton btnClose;

	public UILabel inputUserName;

	public ListBox lbPlayers;

	private LoadingWindow LoadingWin;

	private Team_Part_MyTeam_AddMember_Item CurrentSelectedItem;

	private GIcon iconShowInfo;

	private GIcon iconToMember;

	private SpriteSL thisCtrl = new SpriteSL();

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;
}
