using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class DaTaoShaInvitePart : UserControl
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
		this.InitTextInPrefabs();
		this.InitEvent();
		this.PreLoadItem();
	}

	private void PreLoadItem()
	{
		for (int i = 0; i < 4; i++)
		{
			DaTaoShaInviteItem daTaoShaInviteItem = U3DUtils.NEW<DaTaoShaInviteItem>();
			NGUITools.AddChild2(this.mListBox.gameObject, daTaoShaInviteItem);
			daTaoShaInviteItem.InitValue(null);
			this.ItemCollection.Add(daTaoShaInviteItem);
		}
	}

	private void InitTextInPrefabs()
	{
		this.LblName.Text = Global.GetLang("名称");
		this.LblLevel.Text = Global.GetLang("等级");
		this.LblZhanLi.Text = Global.GetLang("战力");
		this.LblStatus.Text = Global.GetLang("信息");
		this.LblTitle.Text = Global.GetLang("参赛战队");
		this.BtnConfirm.Label.text = Global.GetLang("加入战场");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
			GameInstance.Game.RequestEnterDaTaoShaScene();
		};
	}

	public void InitValueByServer(List<EscapeBattleJoinRoleInfo> datas)
	{
		this.InitValue(datas);
	}

	private void InitValue(List<EscapeBattleJoinRoleInfo> datas)
	{
		if (datas == null || datas.Count <= 0)
		{
			return;
		}
		EscapeBattleJoinRoleInfo escapeBattleJoinRoleInfo = null;
		if (Global.Data != null && Global.Data.roleData != null)
		{
			for (int i = 0; i < datas.Count; i++)
			{
				if (datas[i].RoleID == Global.Data.roleData.RoleID)
				{
					escapeBattleJoinRoleInfo = datas[i];
					break;
				}
			}
		}
		if (escapeBattleJoinRoleInfo != null)
		{
			datas.Remove(escapeBattleJoinRoleInfo);
		}
		for (int j = 0; j < this.ItemCollection.Count; j++)
		{
			DaTaoShaInviteItem component = this.ItemCollection.GetAt(j).GetComponent<DaTaoShaInviteItem>();
			if (j < datas.Count)
			{
				if (Global.Data == null || Global.Data.roleData == null || datas[j].RoleID != Global.Data.roleData.RoleID)
				{
					component.InitValue(datas[j]);
				}
			}
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
		if (PlayZone.GlobalPlayZone.gamePayerRolePart != null)
		{
			PlayZone.GlobalPlayZone.CloseGamePayerRoleWindow();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblName;

	public TextBlock LblLevel;

	public TextBlock LblZhanLi;

	public TextBlock LblStatus;

	public TextBlock LblTitle;

	public GButton BtnClose;

	public GButton BtnConfirm;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;
}
