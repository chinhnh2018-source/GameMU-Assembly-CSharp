using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class TeamPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.tabControl.TabBtns[0].Text = Global.GetLang("我的队伍");
		this.tabControl.TabBtns[1].Text = Global.GetLang("附近队伍");
		this.tabControl.TabBtns[2].Text = Global.GetLang("附近玩家");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.teamPartMyTeam.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(s, e);
			}
		};
		this.teamPartNearPlayer.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(s, e);
			}
		};
		this.teamPartNearTeam.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(s, e);
			}
		};
	}

	public void InitData()
	{
		this.teamPartMyTeam.InitPartData();
		this.teamPartMyTeam.GetNewData();
		this.teamPartNearTeam.InitPartData();
		this.teamPartNearTeam.GetNewData();
		this.teamPartNearPlayer.InitPartData();
		this.teamPartNearPlayer.GetNewData();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
		this.teamPartMyTeam.CleanUpChildWindows();
		this.teamPartNearTeam.CleanUpChildWindows();
		this.teamPartNearPlayer.CleanUpChildWindows();
	}

	public GButton btnClose;

	public Team_Part_MyTeam teamPartMyTeam;

	public Team_Part_NearTeam teamPartNearTeam;

	public Team_Part_NearPlayer teamPartNearPlayer;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GTabControl tabControl;
}
