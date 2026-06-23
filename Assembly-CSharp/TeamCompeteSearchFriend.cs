using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class TeamCompeteSearchFriend : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Text = Global.GetLang("搜索玩家");
		this.LblName.Text = Global.GetLang("玩家名称");
		this.BtnConfirm.Label.text = Global.GetLang("添加");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, null);
			}
		};
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = this.mInputName.Text;
			MUDebug.LogError<string>(new string[]
			{
				"搜索玩家Name：" + text
			});
			GameInstance.Game.SendInviteTeamMemberMsg(string.Empty, text);
		};
	}

	private void InitValue()
	{
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblName;

	public GButton BtnClose;

	public GButton BtnConfirm;

	public TextBox mInputName;
}
