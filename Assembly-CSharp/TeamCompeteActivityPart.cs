using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class TeamCompeteActivityPart : UserControl
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
			TeamCompeteActivityItem component = selectedItem.GetComponent<TeamCompeteActivityItem>();
			if (component == null)
			{
				return;
			}
			this.OpenActivityByID(component);
		};
		this.mBtnZhanDui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.CloseActivityWindow();
			}
			if (Global.Data.roleData != null && Global.Data.roleData.ZhanDuiID <= 0)
			{
				if (PlayZone.GlobalPlayZone != null)
				{
					PlayZone.GlobalPlayZone.OpenTeamCompeteCreatePart();
				}
			}
			else if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OpenTeamCompeteMainPart();
			}
		};
		this.InitTextInPrefabs();
		this.InitXML();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.mBtnZhanDui.Text = Global.GetLang("战队");
	}

	private void InitEvent()
	{
		GameInstance.Game.RequestDaTaoShaMainData();
		GameInstance.Game.SendDuoBaoGetActivityState();
		DaTaoShaDataManager.DaTaoShaSwitchCallBak = delegate(EscapeBattleGameStates s)
		{
			MUDebug.Log<string>(new string[]
			{
				"大逃杀服务器活动状态：" + s
			});
			this.LoadDaTaoSha(s);
			DaTaoShaDataManager.DaTaoShaSwitchCallBak = null;
		};
	}

	private void InitXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ZhanDuiHuoDongTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (Global.GetXElementAttributeInt(xelement, "ID") != 200000 || (ConfigVersionSystemOpen.IsVersionSystemOpen(100108) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompete)))
				{
					if (Global.GetXElementAttributeInt(xelement, "ID") != 200001 || (ConfigVersionSystemOpen.IsVersionSystemOpen(100113) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompeteZhengBa)))
					{
						if (Global.GetXElementAttributeInt(xelement, "ID") != 200003)
						{
							if (Global.GetXElementAttributeInt(xelement, "ID") != 200002)
							{
								TeamCompeteActivityItem teamCompeteActivityItem = U3DUtils.NEW<TeamCompeteActivityItem>();
								this.ItemCollection.Add(teamCompeteActivityItem);
								teamCompeteActivityItem.InitValue(xelement);
							}
						}
					}
				}
			}
		}
	}

	private void LoadMoYuDuoBaoActivity()
	{
	}

	public void LoadDaTaoSha(EscapeBattleGameStates status)
	{
		this.mEscapeBattleGameStates = status;
		XElement gameResXml = Global.GetGameResXml("Config/ZhanDuiHuoDongTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (Global.GetXElementAttributeInt(xelement, "ID") == 200003)
				{
					if (Global.GetXElementAttributeInt(xelement, "ID") != 200003 || (ConfigVersionSystemOpen.IsVersionSystemOpen(100115) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.DaTaoSha) && this.mEscapeBattleGameStates != EscapeBattleGameStates.NotOpen))
					{
						TeamCompeteActivityItem teamCompeteActivityItem = U3DUtils.NEW<TeamCompeteActivityItem>();
						this.ItemCollection.Add(teamCompeteActivityItem);
						teamCompeteActivityItem.InitValue(xelement);
					}
				}
			}
		}
		this.mListBox.repositionNow = true;
	}

	public void LoadMoYuDuoBao()
	{
		if (this.m_moYuItem != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ZhanDuiHuoDongTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (Global.GetXElementAttributeInt(xelement, "ID") == 200002)
				{
					if (Global.GetXElementAttributeInt(xelement, "ID") != 200002 || (ConfigVersionSystemOpen.IsVersionSystemOpen(100114) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MoYuDuoBao) && MoYuDuoBaoData.DuoBaoState != 4))
					{
						TeamCompeteActivityItem teamCompeteActivityItem = U3DUtils.NEW<TeamCompeteActivityItem>();
						this.ItemCollection.Add(teamCompeteActivityItem);
						teamCompeteActivityItem.InitValue(xelement);
						this.m_moYuItem = teamCompeteActivityItem;
					}
				}
			}
		}
		this.mListBox.repositionNow = true;
	}

	private void OpenActivityByID(TeamCompeteActivityItem item)
	{
		ZhanDuiHuoDongTabVo vo = item.vo;
		if (vo == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置解析有误 ZhanDuiHuoDongTabVo"
			});
			return;
		}
		switch (vo.ID)
		{
		case 200000:
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OpenTeamCompetePart(false);
			}
			break;
		case 200001:
			this.OpenTeamCompeteZhengBaPart();
			break;
		case 200002:
			PlayZone.GlobalPlayZone.OpenMoYuDuoBaoWindow();
			break;
		case 200003:
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OpenDaTaoShaMainPart();
			}
			break;
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
		MUEventManager.AddEventListener<ZorkBattleGameStates>("CMD_SPR_ZORK_STATE", new Action<ZorkBattleGameStates>(this.RespondDuoBaostate));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<ZorkBattleGameStates>("CMD_SPR_ZORK_STATE", new Action<ZorkBattleGameStates>(this.RespondDuoBaostate));
	}

	public void RespondDuoBaostate(ZorkBattleGameStates state)
	{
		if (state != 4)
		{
			this.LoadMoYuDuoBao();
		}
	}

	public void OpenTeamCompeteZhengBaPart()
	{
		if (this.mTeamCompeteZhengBaPartWind != null || this.mTeamCompeteZhengBaPart != null)
		{
			this.CloseTeamCompeteZhengBaPart();
		}
		this.mTeamCompeteZhengBaPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteZhengBaPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteZhengBaPartWind.Modal = true;
		this.mTeamCompeteZhengBaPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteZhengBaPartWind, "mTeamCompeteZhengBaPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteZhengBaPartWind);
		this.mTeamCompeteZhengBaPart = U3DUtils.NEW<TeamCompeteZhengBaPart>();
		this.mTeamCompeteZhengBaPartWind.Body.Add(this.mTeamCompeteZhengBaPart);
		this.mTeamCompeteZhengBaPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteZhengBaPart();
		};
	}

	private void CloseTeamCompeteZhengBaPart()
	{
		if (null != this.mTeamCompeteZhengBaPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteZhengBaPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteZhengBaPartWind, true);
			this.mTeamCompeteZhengBaPartWind = null;
		}
		if (null != this.mTeamCompeteZhengBaPart)
		{
			this.mTeamCompeteZhengBaPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteZhengBaPart.gameObject);
			this.mTeamCompeteZhengBaPart = null;
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public const int ZhanDuiJingJi = 200000;

	public const int ZhanDuiZhengBa = 200001;

	public const int MoYuDuoBao = 200002;

	public const int DaTaoSha = 200003;

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public GButton mBtnZhanDui;

	private EscapeBattleGameStates mEscapeBattleGameStates;

	private TeamCompeteActivityItem m_moYuItem;

	protected GChildWindow mTeamCompeteZhengBaPartWind;

	protected TeamCompeteZhengBaPart mTeamCompeteZhengBaPart;
}
