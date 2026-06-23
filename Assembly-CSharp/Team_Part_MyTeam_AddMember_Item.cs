using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Team_Part_MyTeam_AddMember_Item : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.SelectedBak);
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
			if (null != this.SelectedBak)
			{
				NGUITools.SetActiveSelf(this.SelectedBak.gameObject, this._SelectedState);
			}
		}
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

	public void ShowSelectedBak()
	{
		NGUITools.SetActiveSelf(this.SelectedBak.gameObject, this._SelectedState);
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

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
	}

	private Canvas Root;

	public GTextBlockOutLine gtbPlayerName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbWork = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public ShowNetImage SelectedBak;

	private bool _SelectedState;

	private int _RoleID;
}
