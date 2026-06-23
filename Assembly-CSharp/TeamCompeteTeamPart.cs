using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteTeamPart : UserControl
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

	private void ResetTeamItem()
	{
		if (this.mTeamItem != null)
		{
			this.mTeamItem.IsSelect = false;
			this.mTeamItem = null;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.mListBox.Items;
		this.mListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			if (!this.isTeamLeader)
			{
				return;
			}
			GameObject selectedItem = this.mListBox.SelectedItem;
			if (this.mTeamItem != null)
			{
				this.mTeamItem.IsSelect = false;
			}
			this.mTeamItem = selectedItem.GetComponent<TeamCompeteTeamItemPart>();
			if (this.mTeamItem.IsTeamLeader)
			{
				this.mTeamItem = null;
				return;
			}
			if (this.mTeamItem.IsYourSelf)
			{
				return;
			}
			if (this.mTeamItem.ID == -1)
			{
				this.mTeamItem = null;
				return;
			}
			this.mTeamItem.IsSelect = true;
		};
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Label.text = Global.GetLang("队伍");
		this.LblTeamName.Label.text = Global.GetLang("战队名称");
		this.LblDuanWei.Label.text = Global.GetLang("战队段位：");
		this.LblBattleValue.Label.text = Global.GetLang("战队战力：");
		this.LblTeamFlag.Label.text = Global.GetLang("战队宣言：");
		this.LblContent.Label.text = Global.GetLang("请输入战队宣言");
		this.BtnCancel.Label.text = Global.GetLang("解散战队");
		this.BtnInviteFriend.Label.text = Global.GetLang("邀请好友");
		this.BtnInviteTeamer.Label.text = Global.GetLang("邀请队员");
		this.BtnYiChu.Label.text = Global.GetLang("移出战队");
		this.BtnTeamLeader.Label.text = Global.GetLang("提升队长");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUIWinodw();
		};
		this.BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (TeamCompeteDataManager.IsInTeamZhengBaActivityOpen())
			{
				return;
			}
			this.RequestDeleteTeam();
		};
		this.BtnYiChu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (TeamCompeteDataManager.IsInTeamZhengBaActivityOpen())
			{
				return;
			}
			if (this.mTeamItem != null)
			{
				string message = string.Format(Global.GetLang("确认从战队中移除玩家") + this.mTeamItem.Name + Global.GetLang("？"), new object[0]);
				GChildWindow messageBoxWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.DeleteMemberCallBack(this.mTeamItem.ID);
					}
					return true;
				};
			}
			else
			{
				Super.HintMainText(Global.GetLang("请选择队员"), 10, 3);
			}
		};
		this.BtnTeamLeader.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (TeamCompeteDataManager.IsInTeamZhengBaActivityOpen())
			{
				return;
			}
			if (this.mTeamItem != null)
			{
				string message = string.Format(Global.GetLang("确认将玩家") + this.mTeamItem.Name + Global.GetLang("提升为战队队长？"), new object[0]);
				GChildWindow messageBoxWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.ChangeLeaderCallBack(this.mTeamItem.ID);
					}
					return true;
				};
			}
			else
			{
				Super.HintMainText(Global.GetLang("请选择队员"), 10, 3);
			}
		};
	}

	private void InitValue()
	{
		for (int i = 0; i < 5; i++)
		{
			TeamCompeteTeamItemPart teamCompeteTeamItemPart = U3DUtils.NEW<TeamCompeteTeamItemPart>();
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteTeamItemPart);
			teamCompeteTeamItemPart.InitValue(null, 0);
			this.ItemCollection.Add(teamCompeteTeamItemPart);
		}
		this.RequestTeamInfo();
	}

	private bool IsTeamLeader
	{
		get
		{
			return this.isTeamLeader;
		}
		set
		{
			this.isTeamLeader = value;
			NGUITools.SetActive(this.BtnCancel.gameObject, this.isTeamLeader);
			NGUITools.SetActive(this.BtnYiChu.gameObject, this.isTeamLeader);
			NGUITools.SetActive(this.BtnTeamLeader.gameObject, this.isTeamLeader);
		}
	}

	private void ChangeLeaderCallBack(int id)
	{
		this.RequestChangeTeamLeader(id);
	}

	private void DeleteMemberCallBack(int id)
	{
		this.RequestDeleteMember(id);
	}

	private void CloseUIWinodw()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	private void UpdateTeamMembersInfo(int id = 0, TeamCompeteTeamPart.MemberState state = TeamCompeteTeamPart.MemberState.None)
	{
		switch (state)
		{
		case TeamCompeteTeamPart.MemberState.Add:
			this.AddNewMember(id);
			break;
		case TeamCompeteTeamPart.MemberState.Delete:
			this.DeleteMember(id);
			break;
		case TeamCompeteTeamPart.MemberState.ChangeLeader:
			this.ChangerLeader(id);
			break;
		}
	}

	private void ChangerLeader(int id)
	{
		int count = this.ItemCollection.Count;
		for (int i = 0; i < count; i++)
		{
			TeamCompeteTeamItemPart component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteTeamItemPart>();
			if (component.ID == id)
			{
				component.IsTeamLeader = true;
				Transform transform = this.ItemCollection.GetAt(i).gameObject.transform;
			}
			else
			{
				component.IsTeamLeader = false;
			}
		}
		this.RefreshListBox();
	}

	private void AddNewMember(int id)
	{
		int count = this.ItemCollection.Count;
		TeamCompeteTeamItemPart teamCompeteTeamItemPart = null;
		int num = 0;
		if (num < count)
		{
			TeamCompeteTeamItemPart component = this.ItemCollection.GetAt(num).GetComponent<TeamCompeteTeamItemPart>();
			if (component.ID == id && !component.gameObject.activeSelf)
			{
				component.gameObject.SetActive(true);
				teamCompeteTeamItemPart = component;
			}
		}
		if (teamCompeteTeamItemPart == null)
		{
			TeamCompeteTeamItemPart teamCompeteTeamItemPart2 = U3DUtils.NEW<TeamCompeteTeamItemPart>();
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteTeamItemPart2);
			this.ItemCollection.Add(teamCompeteTeamItemPart2);
		}
		this.RefreshListBox();
	}

	private void DeleteMember(int id)
	{
		int count = this.ItemCollection.Count;
		for (int i = 0; i < count; i++)
		{
			TeamCompeteTeamItemPart component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteTeamItemPart>();
			if (component.ID == id)
			{
				component.gameObject.SetActive(false);
				break;
			}
		}
		this.RefreshListBox();
	}

	private void RefreshListBox()
	{
		this.mListBox.repositionNow = true;
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO", new Action<MUSocketConnectEventArgs>(this.RespondTeamInfo));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_DELETE_MEMBER", new Action<MUSocketConnectEventArgs>(this.RespondDeleteMember));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CHANGE_ZHANDUI_LEADER", new Action<MUSocketConnectEventArgs>(this.RespondChangeTeamLeader));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_JIESAN_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondDeleteTeam));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_AGREE_ZHANDUI_INVITE", new Action<MUSocketConnectEventArgs>(this.RespondAddNewMember));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_Notify_ZHANDUI_DATA_CHANGED", new Action<MUSocketConnectEventArgs>(this.RespondRefreshChangeZhanDuiData));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO", new Action<MUSocketConnectEventArgs>(this.RespondTeamInfo));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_DELETE_MEMBER", new Action<MUSocketConnectEventArgs>(this.RespondDeleteMember));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CHANGE_ZHANDUI_LEADER", new Action<MUSocketConnectEventArgs>(this.RespondChangeTeamLeader));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_JIESAN_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondDeleteTeam));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_AGREE_ZHANDUI_INVITE", new Action<MUSocketConnectEventArgs>(this.RespondAddNewMember));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_Notify_ZHANDUI_DATA_CHANGED", new Action<MUSocketConnectEventArgs>(this.RespondRefreshChangeZhanDuiData));
	}

	public void RespondRefreshChangeZhanDuiData(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		this.RequestTeamInfo();
	}

	public void RequestTeamInfo()
	{
		GameInstance.Game.RequestSingleTeamInfoMsg();
	}

	public void RespondTeamInfo(MUSocketConnectEventArgs e)
	{
		TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = DataHelper.BytesToObject<TianTi5v5ZhanDuiData>(e.bytesData, 0, e.bytesData.Length);
		if (tianTi5v5ZhanDuiData == null)
		{
			this.IsTeamLeader = false;
		}
		else
		{
			TeamCompeteDataManager.MainZhanDuiData = tianTi5v5ZhanDuiData;
			this.IsTeamLeader = (tianTi5v5ZhanDuiData.LeaderRoleID == Global.Data.roleData.RoleID);
			this.LoadTeamItems(tianTi5v5ZhanDuiData.LeaderRoleID, tianTi5v5ZhanDuiData.teamerList);
		}
	}

	public void RequestDeleteMember(int otherId)
	{
		GameInstance.Game.RequestDeleteTeamMemberMsg(otherId);
	}

	public void RespondDeleteMember(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			if (num == -12)
			{
				return;
			}
			TeamCompeteDataManager.ErrorTips(num);
		}
		else
		{
			this.RequestTeamInfo();
			this.ResetTeamItem();
		}
	}

	public void RequestChangeTeamLeader(int otherId)
	{
		GameInstance.Game.SendChangeTeamLeaderMsg(otherId);
	}

	public void RespondChangeTeamLeader(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			if (num == -12)
			{
				return;
			}
			TeamCompeteDataManager.ErrorTips(num);
		}
		else
		{
			this.RequestTeamInfo();
			this.ResetTeamItem();
			if (this.ChangeZhanDuiLeaderCallBack != null)
			{
				this.ChangeZhanDuiLeaderCallBack.Invoke();
			}
		}
	}

	public void RequestDeleteTeam()
	{
		string message = string.Format(Global.GetLang("是否确定要解散战队？"), new object[0]);
		GChildWindow messageBoxWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SendDeleteTeamMsg();
			}
			return true;
		};
	}

	public void RespondDeleteTeam(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			if (num == -12)
			{
				return;
			}
			TeamCompeteDataManager.ErrorTips(num);
		}
		else
		{
			this.CloseUIWinodw();
			if (this.CallBackDeleteTeam != null)
			{
				this.CallBackDeleteTeam.Invoke();
			}
		}
	}

	public void RespondAddNewMember(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			if (num == -12)
			{
				return;
			}
			TeamCompeteDataManager.ErrorTips(num);
		}
		else
		{
			Super.HintMainText(Global.GetLang("成功加入战队"), 10, 3);
			this.RequestTeamInfo();
		}
	}

	private void LoadTeamItems(int teamLeaderId, List<TianTi5v5ZhanDuiRoleData> teamList = null)
	{
		if (teamList.Count <= 0)
		{
			return;
		}
		this.SortByTeamleader(teamLeaderId, teamList);
		if (this.ItemCollection.Count > 0)
		{
			int num = this.ItemCollection.Count - teamList.Count;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					TeamCompeteTeamItemPart component = this.ItemCollection.GetAt(this.ItemCollection.Count - (i + 1)).GetComponent<TeamCompeteTeamItemPart>();
					if (component != null)
					{
						component.InitValue(null, 0);
					}
				}
			}
			for (int j = 0; j < teamList.Count; j++)
			{
				TeamCompeteTeamItemPart component2 = this.ItemCollection.GetAt(j).GetComponent<TeamCompeteTeamItemPart>();
				if (component2 == null)
				{
					TeamCompeteTeamItemPart teamCompeteTeamItemPart = U3DUtils.NEW<TeamCompeteTeamItemPart>();
					NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteTeamItemPart);
					teamCompeteTeamItemPart.InitValue(teamList[j], teamLeaderId);
					this.ItemCollection.Add(teamCompeteTeamItemPart);
				}
				else
				{
					component2.InitValue(teamList[j], teamLeaderId);
				}
			}
		}
		else
		{
			for (int k = 0; k < teamList.Count; k++)
			{
				TeamCompeteTeamItemPart teamCompeteTeamItemPart2 = U3DUtils.NEW<TeamCompeteTeamItemPart>();
				NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteTeamItemPart2);
				teamCompeteTeamItemPart2.InitValue(teamList[k], teamLeaderId);
				this.ItemCollection.Add(teamCompeteTeamItemPart2);
			}
		}
	}

	private void SortByTeamleader(int teamLeaderId, List<TianTi5v5ZhanDuiRoleData> teamList)
	{
		if (teamList[0].RoleID != teamLeaderId)
		{
			int num = 0;
			for (int i = 0; i < teamList.Count; i++)
			{
				if (teamLeaderId == teamList[i].RoleID)
				{
					num = i;
					break;
				}
			}
			if (num > 0)
			{
				TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = teamList[num];
				teamList.Remove(tianTi5v5ZhanDuiRoleData);
				teamList.Insert(0, tianTi5v5ZhanDuiRoleData);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public Action ChangeZhanDuiLeaderCallBack;

	public DPSelectedItemEventHandler CloseHandler;

	public Action CallBackDeleteTeam;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblTeamName;

	public TextBlock LblDuanWei;

	public TextBlock LblBattleValue;

	public TextBlock LblTeamFlag;

	public TextBlock LblContent;

	public GButton BtnClose;

	public GButton BtnCancel;

	public GButton BtnInviteFriend;

	public GButton BtnInviteTeamer;

	public GButton BtnYiChu;

	public GButton BtnTeamLeader;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public Action<string> mInputCallBack;

	public GameObject mInputObj;

	public TextBox mInput;

	public GButton mBtnInputCancel;

	public GButton mBtnInputConfirm;

	private string mLastXuanYan = string.Empty;

	private string mCurrentXuanYan = string.Empty;

	private TeamCompeteTeamItemPart mTeamItem;

	private bool isTeamLeader;

	private enum MemberState
	{
		None,
		Add,
		Delete,
		ChangeLeader
	}
}
