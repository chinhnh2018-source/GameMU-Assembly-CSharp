using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class TeamCompeteListPart : UserControl, IMUEventManagerHandler
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
		this.dragPanel.onDragFinished = delegate()
		{
			if ((double)this.mScrollBar.scrollValue >= 0.9)
			{
				this.LoadItems(this.cacheDatas);
			}
		};
	}

	private void InitTextInPrefabs()
	{
		this.LblTeamName.Text = Global.GetLang("战队名称");
		this.LblTeamLeader.Text = Global.GetLang("战队队长");
		this.LblTeamCount.Text = Global.GetLang("战队人数");
		this.LblTeamBattleValue.Text = Global.GetLang("战队战力");
		this.LblTeamDuanWei.Text = Global.GetLang("竞技段位");
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
	}

	private void InitValue()
	{
		this.RequestOtherTeamInfoMsg();
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GETZHANDUI_LIST", new Action<MUSocketConnectEventArgs>(this.RespondOtherTeamInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GETZHANDUI_LIST", new Action<MUSocketConnectEventArgs>(this.RespondOtherTeamInfo));
	}

	public void RequestOtherTeamInfoMsg()
	{
		GameInstance.Game.RequestOtherTeamInfoMsg();
	}

	public void RespondOtherTeamInfo(MUSocketConnectEventArgs e)
	{
		List<TianTi5v5ZhanDuiMiniData> list = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiMiniData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null || list.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		this.cacheDatas = list;
		this.LoadItems(list);
	}

	private void LoadItems(List<TianTi5v5ZhanDuiMiniData> data)
	{
		this.start = this.ItemCollection.Count;
		this.end = ((data.Count - (this.loadCount + this.ItemCollection.Count) <= 0) ? data.Count : (this.loadCount + this.ItemCollection.Count));
		for (int i = this.start; i < this.end; i++)
		{
			TeamCompeteListItem teamCompeteListItem = U3DUtils.NEW<TeamCompeteListItem>();
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteListItem);
			teamCompeteListItem.InitValue(data[i]);
			this.ItemCollection.Add(teamCompeteListItem);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTeamName;

	public TextBlock LblTeamLeader;

	public TextBlock LblTeamCount;

	public TextBlock LblTeamBattleValue;

	public TextBlock LblTeamDuanWei;

	public GButton BtnClose;

	public UIDraggablePanel dragPanel;

	public UIScrollBar mScrollBar;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private int loadCount = 10;

	private int start;

	private int end;

	private List<TianTi5v5ZhanDuiMiniData> cacheDatas;
}
