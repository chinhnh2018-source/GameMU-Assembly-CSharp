using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteMatchStatusPart : UserControl, IMUEventManagerHandler
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
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTeamName.Text = Global.GetLang("服务器");
		this.LblTeamLeader.Text = Global.GetLang("名字");
		this.LblTeamCount.Text = Global.GetLang("成绩");
		this.LblTeamBattleValue.Text = Global.GetLang("战力");
		this.LblTeamDuanWei.Text = Global.GetLang("当前状态");
		this.BtnZhanBao.Label.text = Global.GetLang("战  报");
		this.BtnGuess.Label.text = Global.GetLang("我的竞猜");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnZhanBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RequestLog();
		};
		this.BtnGuess.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			TeamCompeteDataManager.ZhangBaGuessCallBack = delegate(List<ZhanDuiZhengBaZhanDuiData> list)
			{
				if (list == null)
				{
					Super.HintMainText(Global.GetLang("您还没有竞猜战队，请选择您要投注的战队"), 10, 3);
					return;
				}
				this.OpenTeamCompeteGuessPart(list);
			};
			GameInstance.Game.RequestTeamMatchYaZhuList();
		};
	}

	private void InitValue()
	{
	}

	public void OpenCommonZhanBaoPart(List<ZhanDuiZhengBaPkLogData> dataList)
	{
		if (this.mCommonZhanBaoPartWind != null || this.mCommonZhanBaoPart != null)
		{
			this.CloseCommonZhanBaoPart();
		}
		this.mCommonZhanBaoPartWind = U3DUtils.NEW<GChildWindow>();
		this.mCommonZhanBaoPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mCommonZhanBaoPartWind.Modal = true;
		this.mCommonZhanBaoPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mCommonZhanBaoPartWind, "mCommonZhanBaoPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mCommonZhanBaoPartWind);
		this.mCommonZhanBaoPart = U3DUtils.NEW<CommonZhanBaoPart>();
		this.mCommonZhanBaoPart.InitValue(dataList);
		this.mCommonZhanBaoPartWind.Body.Add(this.mCommonZhanBaoPart);
		this.mCommonZhanBaoPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCommonZhanBaoPart();
		};
	}

	private void CloseCommonZhanBaoPart()
	{
		if (null != this.mCommonZhanBaoPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mCommonZhanBaoPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mCommonZhanBaoPartWind, true);
			this.mCommonZhanBaoPartWind = null;
		}
		if (null != this.mCommonZhanBaoPart)
		{
			this.mCommonZhanBaoPart.transform.parent = null;
			Object.Destroy(this.mCommonZhanBaoPart.gameObject);
			this.mCommonZhanBaoPart = null;
		}
	}

	public void OpenTeamCompeteGuessPart(List<ZhanDuiZhengBaZhanDuiData> dataList)
	{
		if (this.mTeamCompeteGuessPartWind != null || this.mTeamCompeteGuessPart != null)
		{
			this.CloseTeamCompeteGuessPart();
		}
		this.mTeamCompeteGuessPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteGuessPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteGuessPartWind.Modal = true;
		this.mTeamCompeteGuessPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteGuessPartWind, "mTeamCompeteGuessPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteGuessPartWind);
		this.mTeamCompeteGuessPart = U3DUtils.NEW<TeamCompeteGuessPart>();
		this.mTeamCompeteGuessPart.InitValue(dataList);
		this.mTeamCompeteGuessPartWind.Body.Add(this.mTeamCompeteGuessPart);
		this.mTeamCompeteGuessPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteGuessPart();
		};
		this.mTeamCompeteGuessPart.ClickHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteGuessPart();
		};
	}

	private void CloseTeamCompeteGuessPart()
	{
		if (null != this.mTeamCompeteGuessPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteGuessPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteGuessPartWind, true);
			this.mTeamCompeteGuessPartWind = null;
		}
		if (null != this.mTeamCompeteGuessPart)
		{
			this.mTeamCompeteGuessPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteGuessPart.gameObject);
			this.mTeamCompeteGuessPart = null;
		}
	}

	public void InitData(List<ZhanDuiZhengBaZhanDuiData> dataList)
	{
		if (dataList != null && dataList.Count > 0)
		{
			dataList.Sort((ZhanDuiZhengBaZhanDuiData data1, ZhanDuiZhengBaZhanDuiData data2) => data1.Grade - data2.Grade);
		}
		for (int i = 0; i < dataList.Count; i++)
		{
			TeamCompeteMatchStatusItem teamCompeteMatchStatusItem = U3DUtils.NEW<TeamCompeteMatchStatusItem>();
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteMatchStatusItem);
			teamCompeteMatchStatusItem.InitValue(dataList[i]);
			this.ItemCollection.Add(teamCompeteMatchStatusItem);
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_LOG", new Action<MUSocketConnectEventArgs>(this.RespondLog));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_LOG", new Action<MUSocketConnectEventArgs>(this.RespondLog));
	}

	private void RequestLog()
	{
		GameInstance.Game.RequestTeamZhengBaZhanBao();
	}

	public void RespondLog(MUSocketConnectEventArgs e)
	{
		List<ZhanDuiZhengBaPkLogData> list = DataHelper.BytesToObject<List<ZhanDuiZhengBaPkLogData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		this.OpenCommonZhanBaoPart(list);
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
		TeamCompeteDataManager.ZhangBaGuessCallBack = null;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTeamName;

	public TextBlock LblTeamLeader;

	public TextBlock LblTeamCount;

	public TextBlock LblTeamBattleValue;

	public TextBlock LblTeamDuanWei;

	public GButton BtnClose;

	public GButton BtnZhanBao;

	public GButton BtnGuess;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	protected GChildWindow mCommonZhanBaoPartWind;

	protected CommonZhanBaoPart mCommonZhanBaoPart;

	protected GChildWindow mTeamCompeteGuessPartWind;

	protected TeamCompeteGuessPart mTeamCompeteGuessPart;
}
