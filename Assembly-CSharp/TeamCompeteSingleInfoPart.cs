using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteSingleInfoPart : UserControl
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

	public ZhanDuiZhengBaZhanDuiData mData { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.mListBox.Items;
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Text = Global.GetLang("战队信息");
		this.LblTeamName.Text = Global.GetLang("战队信息");
		this.LblZhanLiValue.Text = Global.GetLang("战力：");
		this.LblDuanWei.Text = Global.GetLang("段位：");
		this.BtnGuess.Label.text = Global.GetLang("竞猜");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnGuess.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PopupGuessWindow();
		};
	}

	public void InitValue(ZhanDuiZhengBaZhanDuiData data)
	{
		this.mData = data;
		this.LblTeamName.Text = TeamCompeteDataManager.ServerTeamName(data.ZoneId, data.ZhanDuiName);
		this.LblZhanLiValue.Text = Global.GetLang("战力：") + data.ZhanLi;
		this.LblDuanWei.Text = Global.GetLang("段位：") + TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		if (data.MemberList != null && data.MemberList.Count > 0)
		{
			this.LoadItem(data.MemberList);
		}
	}

	public void LoadItem(List<RoleOccuNameZhanLi> MemberList)
	{
		for (int i = 0; i < MemberList.Count; i++)
		{
			TeamCompeteSingleInfoItem teamCompeteSingleInfoItem = U3DUtils.NEW<TeamCompeteSingleInfoItem>();
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteSingleInfoItem);
			teamCompeteSingleInfoItem.InitValue(MemberList[i]);
			this.ItemCollection.Add(teamCompeteSingleInfoItem);
		}
	}

	private DateTime ParseStringTimePoint(int day, string timePoint)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime result = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			day,
			timePoint
		});
		DateTime.TryParse(text, ref result);
		return result;
	}

	private bool CanGuess()
	{
		if (string.IsNullOrEmpty(this.timePoint))
		{
			this.timePoint = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchGuessTimePoint();
			this.timePoints = this.timePoint.Split(new char[]
			{
				'|'
			});
			this.begin = this.timePoints[0].Split(new char[]
			{
				','
			});
			this.end = this.timePoints[1].Split(new char[]
			{
				','
			});
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = this.ParseStringTimePoint(this.begin[0].SafeToInt32(0), this.begin[1]);
		DateTime dateTime2 = this.ParseStringTimePoint(this.end[0].SafeToInt32(0), this.end[1]);
		return dateTime <= correctDateTime && correctDateTime <= dateTime2;
	}

	public void PopupGuessWindow()
	{
		if (!this.CanGuess())
		{
			Super.HintMainText(Global.GetLang("当前时间段不能竞猜"), 10, 3);
			return;
		}
		string message = string.Format(Global.GetLang("确认在战队争霸竞猜中向【{0}】战队下注{1}金币？"), this.mData.ZhanDuiName, IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchGuessCostJinBi());
		GChildWindow popupGuessWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-158f, 17f, -0.01f), new Vector3(-72f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		popupGuessWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = popupGuessWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, popupGuessWindow);
			if (messageBoxReturn == 0)
			{
				this.SendYaZhu();
			}
			return true;
		};
	}

	public void SendYaZhu()
	{
		GameInstance.Game.SendTeamMatchYaZhu(this.mData.ZhanDuiID);
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

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblTeamName;

	public TextBlock LblZhanLiValue;

	public TextBlock LblDuanWei;

	public GButton BtnClose;

	public GButton BtnGuess;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private string timePoint = string.Empty;

	private string[] timePoints;

	private string[] begin;

	private string[] end;
}
