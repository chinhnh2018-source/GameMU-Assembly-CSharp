using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class Team_Part_Request : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.lbPlayers.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.lbPlayers.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbPlayers_SelectionChanged);
		this.ItemCollection = this.lbPlayers.ItemsSource;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.ItemCollection.Clear();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 10
				});
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
		this.ProcessTeamUIItemQueue();
	}

	private void ShowRequest(TeamUIItem teamUIItem, int teamCmd)
	{
		this.CmdType = teamCmd;
		Team_Part_Request_Item team_Part_Request_Item = U3DUtils.NEW<Team_Part_Request_Item>();
		this.ItemCollection.AddNoUpdate(team_Part_Request_Item);
		if (teamCmd == 4)
		{
			this.lblTitle.text = Global.GetLang("申请入队");
			this.texTitle.spriteName = "ruduiShengqin";
		}
		else
		{
			this.lblTitle.text = Global.GetLang("邀请入队");
			this.texTitle.spriteName = "ruduiYaoqing";
		}
		team_Part_Request_Item.ID = this.RequestID++;
		team_Part_Request_Item.CurrentTeamUIItem = teamUIItem;
		team_Part_Request_Item.InitButtons();
		team_Part_Request_Item.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.CmdType == 4)
			{
				GameInstance.Game.SpriteTeam(7, e.ID, 0);
				this.RemoveItemByID(e.ID);
			}
			else if (this.CmdType == 3)
			{
				GameInstance.Game.SpriteTeam(6, e.ID, 0);
				this.RemoveItemByID(e.ID);
				this.RefuseAllRequest();
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 10,
						Tag = null
					});
				}
			}
		};
	}

	private void RemoveItemByID(int id)
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			if (U3DUtils.AS<Team_Part_Request_Item>(this.ItemCollection[i]).CurrentTeamUIItem.OtherRoleID == id)
			{
				this.ItemCollection.RemoveAt(i);
				break;
			}
		}
	}

	public int GetRequestCount()
	{
		return this.ItemCollection.Count;
	}

	public void RefuseAllRequest()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			TeamUIItem currentTeamUIItem = U3DUtils.AS<Team_Part_Request_Item>(this.ItemCollection[i]).CurrentTeamUIItem;
			GameInstance.Game.SpriteTeam(5, currentTeamUIItem.OtherRoleID, 0);
		}
		this.ItemCollection.Clear();
	}

	private void ProcessTeamUIItemQueue()
	{
		if (Super.GData.TeamUIItemQueue == null)
		{
			return;
		}
		while (Super.GData.TeamUIItemQueue.Count > 0)
		{
			TeamUIItem teamUIItem = Super.GData.TeamUIItemQueue[0];
			Super.GData.TeamUIItemQueue.RemoveAt(0);
			if (teamUIItem.TeamCmd == 3)
			{
				this.ShowRequest(teamUIItem, teamUIItem.TeamCmd);
			}
			else if (teamUIItem.TeamCmd == 4)
			{
				this.ShowRequest(teamUIItem, teamUIItem.TeamCmd);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void lbPlayers_SelectionChanged(object sender, MouseEvent e)
	{
		if (null != this.CurrentSelectedItem)
		{
			this.CurrentSelectedItem.BodyBackground = null;
		}
		Team_Part_Request_Item team_Part_Request_Item = U3DUtils.AS<Team_Part_Request_Item>(this.lbPlayers.SelectedItem);
		if (null == team_Part_Request_Item)
		{
			return;
		}
		team_Part_Request_Item.BodyHeight = 20.0;
		team_Part_Request_Item.BodyWidth = 278.0;
		team_Part_Request_Item.BodyBackground = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 278.0, 20.0, 5.0, 5.0));
		this.CurrentSelectedItem = team_Part_Request_Item;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private Team_Part_Request_Item CurrentSelectedItem;

	private int RequestID;

	private Canvas Root;

	public ListBox lbPlayers;

	public UILabel lblTitle;

	public GButton btnClose;

	public UISprite texTitle;

	private int CmdType = -1;

	private ObservableCollection _ItemCollection;
}
