using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class TeamCompeteInviteFriendPart : UserControl, IMUEventManagerHandler
{
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.mListBox.ItemsSource;
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Label.text = Global.GetLang("好友邀请");
		this.LblTitleName.Label.text = Global.GetLang("名称");
		this.LblTitleLevel.Label.text = Global.GetLang("等级");
		this.LblTitleBattle.Label.text = Global.GetLang("战力");
		this.LblTitleTeamInfo.Label.text = Global.GetLang("战队信息");
		this.BtnAll.Label.text = Global.GetLang("全选");
		this.BtnInvite.Label.text = Global.GetLang("邀请");
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
		this.BtnAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsSelectAll)
			{
				this.CancelAll();
			}
			else
			{
				this.SelectAll();
			}
		};
		this.BtnInvite.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendInviteFriendsID();
		};
	}

	private void InitValue()
	{
		this.RequestFriendsMsg();
	}

	private void SelectAll()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			TeamCompeteInviteFriendItem component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteInviteFriendItem>();
			component.IsClick = true;
		}
		this.IsSelectAll = true;
	}

	private void CancelAll()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			TeamCompeteInviteFriendItem component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteInviteFriendItem>();
			component.IsClick = false;
		}
		this.IsSelectAll = false;
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_GETFRIENDS", new Action<MUSocketConnectEventArgs>(this.RespondFriends));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_GETFRIENDS", new Action<MUSocketConnectEventArgs>(this.RespondFriends));
	}

	public void RequestFriendsMsg()
	{
		GameInstance.Game.SpriteGetFriends();
	}

	public void RespondFriends(MUSocketConnectEventArgs e)
	{
		List<FriendData> list = DataHelper.BytesToObject<List<FriendData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null || list.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("您还没有好友"), 10, 3);
			return;
		}
		this.LoadItems(list);
	}

	private void SendInviteFriendsID()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			TeamCompeteInviteFriendItem component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteInviteFriendItem>();
			if (component.IsClick)
			{
				stringBuilder.Append(component.ID);
				stringBuilder.Append("|");
			}
		}
		string text = stringBuilder.ToString();
		if (!string.IsNullOrEmpty(text))
		{
			text = stringBuilder.ToString().TrimEnd(new char[]
			{
				'|'
			});
			GameInstance.Game.SendInviteTeamMemberMsg(text, null);
		}
	}

	private void LoadItems(List<FriendData> datas)
	{
		for (int i = 0; i < datas.Count; i++)
		{
			if (datas[i].ZhanDuiID <= 0)
			{
				TeamCompeteInviteFriendItem teamCompeteInviteFriendItem = U3DUtils.NEW<TeamCompeteInviteFriendItem>();
				NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteInviteFriendItem);
				teamCompeteInviteFriendItem.InitValue(datas[i]);
				teamCompeteInviteFriendItem.IsClick = false;
				this.ItemCollection.Add(teamCompeteInviteFriendItem);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblTitleName;

	public TextBlock LblTitleLevel;

	public TextBlock LblTitleBattle;

	public TextBlock LblTitleTeamInfo;

	public GButton BtnClose;

	public GButton BtnAll;

	public GButton BtnInvite;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private bool IsSelectAll;
}
