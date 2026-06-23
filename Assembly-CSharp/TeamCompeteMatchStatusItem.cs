using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteMatchStatusItem : UserControl
{
	public ZhanDuiZhengBaZhanDuiData mData { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblServerName.Text = Global.GetLang(string.Empty);
		this.LblLeaderName.Text = Global.GetLang(string.Empty);
		this.LblChengJi.Text = Global.GetLang(string.Empty);
		this.LblLeaderBattleValue.Text = Global.GetLang(string.Empty);
		this.LblStatus.Text = Global.GetLang("晋级");
	}

	private void InitEvent()
	{
		this.BtnChaKan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenTeamCompeteSingleInfoPart();
		};
		this.BtnGuess.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PopupGuessWindow();
		};
	}

	public void InitValue(ZhanDuiZhengBaZhanDuiData data)
	{
		this.mData = data;
		this.LblServerName.Text = Global.FormatRoleNameZoneid(data.ZoneId, null, 0, 1);
		this.LblLeaderName.Text = data.ZhanDuiName;
		this.LblChengJi.Text = this.GetJinJiName(data.Grade);
		this.LblLeaderBattleValue.Text = data.ZhanLi.ToString();
		this.LblStatus.Text = this.GetStatus(data.State);
	}

	private string GetStatus(int state)
	{
		if (state == 1)
		{
			return Global.GetLang("晋级");
		}
		if (state == 2)
		{
			return Global.GetLang("淘汰");
		}
		return Global.GetLang("暂无");
	}

	private string GetJinJiName(int grade)
	{
		if (grade == 1)
		{
			return Global.GetLang("冠军");
		}
		if (grade == 2)
		{
			return Global.GetLang("亚军");
		}
		return grade + Global.GetLang("强");
	}

	public bool ForbidGuess
	{
		set
		{
			this.BtnGuess.isEnabled = value;
		}
	}

	public void OpenTeamCompeteSingleInfoPart()
	{
		if (this.mTeamCompeteSingleInfoPartWind != null || this.mTeamCompeteSingleInfoPart != null)
		{
			this.CloseTeamCompeteSingleInfoPart();
		}
		this.mTeamCompeteSingleInfoPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteSingleInfoPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteSingleInfoPartWind.Modal = true;
		this.mTeamCompeteSingleInfoPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mTeamCompeteSingleInfoPartWind, "mTeamCompeteSingleInfoPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteSingleInfoPartWind);
		this.mTeamCompeteSingleInfoPart = U3DUtils.NEW<TeamCompeteSingleInfoPart>();
		this.mTeamCompeteSingleInfoPartWind.Body.Add(this.mTeamCompeteSingleInfoPart);
		this.mTeamCompeteSingleInfoPart.InitValue(this.mData);
		this.mTeamCompeteSingleInfoPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteSingleInfoPart();
		};
	}

	private void CloseTeamCompeteSingleInfoPart()
	{
		if (null != this.mTeamCompeteSingleInfoPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteSingleInfoPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteSingleInfoPartWind, true);
			this.mTeamCompeteSingleInfoPartWind = null;
		}
		if (null != this.mTeamCompeteSingleInfoPart)
		{
			this.mTeamCompeteSingleInfoPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteSingleInfoPart.gameObject);
			this.mTeamCompeteSingleInfoPart = null;
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

	public TextBlock LblServerName;

	public TextBlock LblLeaderName;

	public TextBlock LblChengJi;

	public TextBlock LblLeaderBattleValue;

	public TextBlock LblStatus;

	public GButton BtnChaKan;

	public GButton BtnGuess;

	protected GChildWindow mTeamCompeteSingleInfoPartWind;

	protected TeamCompeteSingleInfoPart mTeamCompeteSingleInfoPart;

	private string timePoint = string.Empty;

	private string[] timePoints;

	private string[] begin;

	private string[] end;
}
