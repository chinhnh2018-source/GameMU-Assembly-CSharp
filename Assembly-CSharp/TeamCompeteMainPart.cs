using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteMainPart : UserControl, IMUEventManagerHandler
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
		this.ItemCollection = this.mListBox.Items;
		this.mListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			GameObject selectedItem = this.mListBox.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			GButton component = selectedItem.GetComponent<GButton>();
			this.OpenUIByIndex(component.TagIndex);
		};
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblGongGaoTitle.Text = Global.GetLang("战队公告");
		this.LblInfoTitle.Text = Global.GetLang("战队信息");
		this.LblUpdateGongGao.Text = Global.GetLang("点击修改公告！");
		this.LblTeamName.Text = Global.GetLang("战队名称：");
		this.LblTeamValue.Text = Global.GetLang(string.Empty);
		this.LblTeamLeader.Text = Global.GetLang("战队队长：");
		this.LblTeamLeaderValue.Text = Global.GetLang(string.Empty);
		this.LblTeamMember.Text = Global.GetLang("战队人数：");
		this.LblTeamMemberValue.Text = Global.GetLang(string.Empty);
		this.LblTeamZhanLi.Text = Global.GetLang("战队战力：");
		this.LblTeamZhanLiValue.Text = Global.GetLang(string.Empty);
		this.LblTeamDuanWei.Text = Global.GetLang("竞技段位：");
		this.LblTeamDuanWeiValue.Text = Global.GetLang(string.Empty);
		this.BtnInputConfirm.Text = Global.GetLang("提交");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		UIEventListener.Get(this.mGongGaoSprt.gameObject).onClick = delegate(GameObject s)
		{
			if (this.IsTeamLeader)
			{
				this.IsShowInputPanel = true;
				this.mInput.Text = this.LblUpdateGongGao.Label.text;
			}
		};
		this.BtnInputConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string xuanYan = this.mInput.Text;
			if (!string.IsNullOrEmpty(xuanYan))
			{
				if (xuanYan.Length > 100)
				{
					Super.HintMainText(Global.GetLang("战队宣言字数不能超过100个"), 10, 3);
					return;
				}
				WordsFilterMgr.ExecWordsFilter(xuanYan, delegate(object content, ExecWordsFilterEventArgs result)
				{
					if (result.ret > 0)
					{
						Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
						{
							result.ret,
							result.msg
						}), 10, 3);
						return;
					}
					if (result.is_dirty > 0)
					{
						Super.HintMainText(Global.GetLang("战队宣言不能包含国家规定禁止的词汇!"), 10, 3);
						return;
					}
					this.mCurrentXuanYan = xuanYan;
					this.RequestUpdateTeamXuanYan(xuanYan);
				});
			}
		};
		this.BtnInputCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			NGUITools.SetActive(this.mInputObj, false);
		};
	}

	private bool IsShowInputPanel
	{
		set
		{
			this.mInputObj.SetActive(value);
		}
	}

	private void InitValue()
	{
		this.RequestTeamInfo();
	}

	private void InitInfo()
	{
		if (this.ItemDict.Count <= 0)
		{
			this.ItemDict.Add(1, Global.GetLang("战队成员"));
			this.ItemDict.Add(2, Global.GetLang("战队活动"));
			this.ItemDict.Add(3, Global.GetLang("战队列表"));
			this.ItemDict.Add(4, Global.GetLang("退出战队"));
			this.ItemDict.Add(5, Global.GetLang("好友邀请"));
			this.ItemDict.Add(6, Global.GetLang("搜索玩家"));
			Dictionary<int, string>.Enumerator enumerator = this.ItemDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!this.IsTeamLeader)
				{
					KeyValuePair<int, string> keyValuePair = enumerator.Current;
					if (keyValuePair.Key != 5)
					{
						KeyValuePair<int, string> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Key != 6)
						{
							goto IL_E1;
						}
					}
					continue;
				}
				IL_E1:
				GameObject gameObject = Object.Instantiate<GameObject>(this.mBtnObj);
				gameObject.SetActive(true);
				GButton component = gameObject.GetComponent<GButton>();
				GButton gbutton = component;
				KeyValuePair<int, string> keyValuePair3 = enumerator.Current;
				gbutton.TagIndex = keyValuePair3.Key;
				GButton gbutton2 = component;
				KeyValuePair<int, string> keyValuePair4 = enumerator.Current;
				gbutton2.Text = keyValuePair4.Value;
				this.ItemCollection.Add(component);
			}
		}
	}

	private void OpenUIByIndex(int tagIndex)
	{
		switch (tagIndex)
		{
		case 1:
			this.OpenTeamCompeteTeamPart();
			break;
		case 2:
			this.OpenActivityWindow();
			this.CloseUI();
			break;
		case 3:
			this.OpenTeamCompeteListPart();
			break;
		case 4:
			this.ExitTeam();
			break;
		case 5:
			this.OpenTeamCompeteInviteFriendPart();
			break;
		case 6:
			this.OpenTeamCompeteSearchFriend();
			break;
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	public void OpenTeamCompeteTeamPart()
	{
		if (this.mTeamCompeteTeamPartWind != null || this.mTeamCompeteTeamPart != null)
		{
			this.CloseTeamCompeteTeamPart();
		}
		this.mTeamCompeteTeamPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteTeamPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteTeamPartWind.Modal = true;
		this.mTeamCompeteTeamPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteTeamPartWind, "mTeamCompeteTeamPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteTeamPartWind);
		this.mTeamCompeteTeamPart = U3DUtils.NEW<TeamCompeteTeamPart>();
		this.mTeamCompeteTeamPartWind.Body.Add(this.mTeamCompeteTeamPart);
		this.mTeamCompeteTeamPart.CallBackDeleteTeam = delegate()
		{
			this.CloseUI();
		};
		this.mTeamCompeteTeamPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteTeamPart();
		};
		this.mTeamCompeteTeamPart.ChangeZhanDuiLeaderCallBack = delegate()
		{
			this.InitValue();
		};
	}

	private void CloseTeamCompeteTeamPart()
	{
		if (null != this.mTeamCompeteTeamPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteTeamPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteTeamPartWind, true);
			this.mTeamCompeteTeamPartWind = null;
		}
		if (null != this.mTeamCompeteTeamPart)
		{
			this.mTeamCompeteTeamPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteTeamPart.gameObject);
			this.mTeamCompeteTeamPart = null;
		}
	}

	private void OpenActivityWindow()
	{
		PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
		{
			ID = 1535
		});
	}

	public void OpenTeamCompeteListPart()
	{
		if (this.mTeamCompeteListPartWind != null || this.mTeamCompeteListPart != null)
		{
			this.CloseTeamCompeteListPart();
		}
		this.mTeamCompeteListPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteListPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteListPartWind.Modal = true;
		this.mTeamCompeteListPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteListPartWind, "mTeamCompeteListPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteListPartWind);
		this.mTeamCompeteListPart = U3DUtils.NEW<TeamCompeteListPart>();
		this.mTeamCompeteListPartWind.Body.Add(this.mTeamCompeteListPart);
		this.mTeamCompeteListPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteListPart();
		};
	}

	private void CloseTeamCompeteListPart()
	{
		if (null != this.mTeamCompeteListPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteListPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteListPartWind, true);
			this.mTeamCompeteListPartWind = null;
		}
		if (null != this.mTeamCompeteListPart)
		{
			this.mTeamCompeteListPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteListPart.gameObject);
			this.mTeamCompeteListPart = null;
		}
	}

	private void ExitTeam()
	{
		if (this.IsTeamLeader)
		{
			Super.HintMainText(Global.GetLang("战队队长不能退出战队"), 10, 3);
			return;
		}
		if (TeamCompeteDataManager.IsInTeamZhengBaActivityOpen())
		{
			return;
		}
		TeamCompeteDataManager.PopupLeaveTeamWindow();
	}

	public void OpenTeamCompeteInviteFriendPart()
	{
		if (this.mTeamCompeteInviteFriendPartWind != null || this.mTeamCompeteInviteFriendPart != null)
		{
			this.CloseTeamCompeteInviteFriendPart();
		}
		this.mTeamCompeteInviteFriendPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteInviteFriendPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteInviteFriendPartWind.Modal = true;
		this.mTeamCompeteInviteFriendPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteInviteFriendPartWind, "mTeamCompeteInviteFriendPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteInviteFriendPartWind);
		this.mTeamCompeteInviteFriendPart = U3DUtils.NEW<TeamCompeteInviteFriendPart>();
		this.mTeamCompeteInviteFriendPartWind.Body.Add(this.mTeamCompeteInviteFriendPart);
		this.mTeamCompeteInviteFriendPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteInviteFriendPart();
		};
	}

	private void CloseTeamCompeteInviteFriendPart()
	{
		if (null != this.mTeamCompeteInviteFriendPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteInviteFriendPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteInviteFriendPartWind, true);
			this.mTeamCompeteInviteFriendPartWind = null;
		}
		if (null != this.mTeamCompeteInviteFriendPart)
		{
			this.mTeamCompeteInviteFriendPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteInviteFriendPart.gameObject);
			this.mTeamCompeteInviteFriendPart = null;
		}
	}

	public void OpenTeamCompeteSearchFriend()
	{
		if (this.mTeamCompeteSearchFriendWind != null || this.mTeamCompeteSearchFriend != null)
		{
			this.CloseTeamCompeteSearchFriend();
		}
		this.mTeamCompeteSearchFriendWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteSearchFriendWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteSearchFriendWind.Modal = true;
		this.mTeamCompeteSearchFriendWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteSearchFriendWind, "mTeamCompeteSearchFriendWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteSearchFriendWind);
		this.mTeamCompeteSearchFriend = U3DUtils.NEW<TeamCompeteSearchFriend>();
		this.mTeamCompeteSearchFriendWind.Body.Add(this.mTeamCompeteSearchFriend);
		this.mTeamCompeteSearchFriend.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteSearchFriend();
		};
	}

	private void CloseTeamCompeteSearchFriend()
	{
		if (null != this.mTeamCompeteSearchFriendWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteSearchFriendWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteSearchFriendWind, true);
			this.mTeamCompeteSearchFriendWind = null;
		}
		if (null != this.mTeamCompeteSearchFriend)
		{
			this.mTeamCompeteSearchFriend.transform.parent = null;
			Object.Destroy(this.mTeamCompeteSearchFriend.gameObject);
			this.mTeamCompeteSearchFriend = null;
		}
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_UPDATE_ZHANDUI_XUYAN", new Action<MUSocketConnectEventArgs>(this.RespondUpdateTeamXuanYan));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_QUIT_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondExitTeam));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO", new Action<MUSocketConnectEventArgs>(this.RespondTeamInfo));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_UPDATE_ZHANDUI_XUYAN", new Action<MUSocketConnectEventArgs>(this.RespondUpdateTeamXuanYan));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_QUIT_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondExitTeam));
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
			this.IsTeamLeader = (tianTi5v5ZhanDuiData.LeaderRoleID == Global.Data.roleData.RoleID);
			if (this.IsTeamLeader && this.ItemDict != null && this.ItemDict.Count > 0)
			{
				this.ItemDict.Clear();
				this.ItemCollection.Clear();
			}
			this.LblTeamValue.Text = tianTi5v5ZhanDuiData.ZhanDuiName;
			this.LblTeamLeaderValue.Text = tianTi5v5ZhanDuiData.LeaderRoleName;
			this.LblTeamMemberValue.Text = tianTi5v5ZhanDuiData.teamerList.Count + "/5";
			this.LblTeamZhanLiValue.Text = tianTi5v5ZhanDuiData.ZhanDouLi.ToString();
			this.LblTeamDuanWeiValue.Text = TeamCompeteDataManager.GetDuanWeiNameByID(tianTi5v5ZhanDuiData.DuanWeiId);
			if (!string.IsNullOrEmpty(tianTi5v5ZhanDuiData.XuanYan))
			{
				this.LblUpdateGongGao.Text = tianTi5v5ZhanDuiData.XuanYan;
			}
		}
		this.InitInfo();
	}

	public void RequestUpdateTeamXuanYan(string content)
	{
		GameInstance.Game.SendUpdateZhanDuiXuanYanMsg(content);
	}

	public void RespondUpdateTeamXuanYan(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			this.mInput.label.text = Global.GetLang("点击修改公告！");
			if (num == -4027 || num == -40)
			{
				Super.HintMainText(Global.GetLang("战队宣言包含特殊字符，请重新输入!"), 10, 3);
			}
			else
			{
				TeamCompeteDataManager.ErrorTips(num);
			}
		}
		else
		{
			this.LblUpdateGongGao.Label.text = this.mCurrentXuanYan;
			NGUITools.SetActive(this.mInputObj, false);
			Super.HintMainText(Global.GetLang("战队宣言修改成功"), 10, 3);
		}
	}

	public void RespondExitTeam(MUSocketConnectEventArgs e)
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
			this.CloseUI();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblGongGaoTitle;

	public TextBlock LblInfoTitle;

	public TextBlock LblUpdateGongGao;

	public TextBlock LblTeamName;

	public TextBlock LblTeamValue;

	public TextBlock LblTeamLeader;

	public TextBlock LblTeamLeaderValue;

	public TextBlock LblTeamMember;

	public TextBlock LblTeamMemberValue;

	public TextBlock LblTeamZhanLi;

	public TextBlock LblTeamZhanLiValue;

	public TextBlock LblTeamDuanWei;

	public TextBlock LblTeamDuanWeiValue;

	public GButton BtnClose;

	public UISprite mGongGaoSprt;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public Dictionary<int, string> ItemDict = new Dictionary<int, string>();

	public GameObject mBtnObj;

	public GameObject mInputObj;

	public TextBox mInput;

	public GButton BtnInputConfirm;

	public GButton BtnInputCancel;

	private bool IsTeamLeader;

	private string mCurrentXuanYan;

	protected GChildWindow mTeamCompeteTeamPartWind;

	protected TeamCompeteTeamPart mTeamCompeteTeamPart;

	protected GChildWindow mTeamCompeteListPartWind;

	protected TeamCompeteListPart mTeamCompeteListPart;

	protected GChildWindow mTeamCompeteInviteFriendPartWind;

	protected TeamCompeteInviteFriendPart mTeamCompeteInviteFriendPart;

	protected GChildWindow mTeamCompeteSearchFriendWind;

	protected TeamCompeteSearchFriend mTeamCompeteSearchFriend;
}
