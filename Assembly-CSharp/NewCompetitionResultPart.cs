using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class NewCompetitionResultPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblWinValue.Text = Global.GetLang("战队当前排名");
		this.LblFirstWin.Text = Global.GetLang("首胜奖励：");
		this.LblShiBaiValue.Text = Global.GetLang(string.Empty);
		this.LblShiBaiDes.Text = Global.GetLang(string.Empty);
		this.BtnWinLeave.Label.text = Global.GetLang("离开战场");
		this.BtnShiBaiGuanZhan.Label.text = Global.GetLang("观 战");
		this.BtnShiBaiLeave.Label.text = Global.GetLang("离开战场");
	}

	private void InitEvent()
	{
		this.BtnWinLeave.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
			this.Leave();
		};
		this.BtnShiBaiGuanZhan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
			GameInstance.Game.GetGuanZhanRoleMiniDatalist();
			DaTaoShaDataManager.IsGuanZhan = true;
			if (PlayZone.GlobalPlayZone != null)
			{
				if (PlayZone.GlobalPlayZone.GetGameCtrlBar != null)
				{
					PlayZone.GlobalPlayZone.GetGameCtrlBar.IsShowSkill = false;
				}
				if (PlayZone.GlobalPlayZone.GetRadarMap() != null)
				{
					PlayZone.GlobalPlayZone.GetRadarMap().DaTaoShaGuanZhanJinZhiHideOthers();
				}
			}
			if (DaTaoShaDataManager.HideBuyAbutton != null)
			{
				DaTaoShaDataManager.HideBuyAbutton.Invoke();
			}
			if (DaTaoShaDataManager.ShowGuanZhanBtnCallBack != null)
			{
				DaTaoShaDataManager.ShowGuanZhanBtnCallBack.Invoke();
			}
		};
		this.BtnShiBaiLeave.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
			this.Leave();
		};
	}

	private void Leave()
	{
		this.CloseUI();
		Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
		{
			NpcID = 1000000,
			ScriptID = 10,
			Hint = 0
		}));
	}

	public void InitValue(EscapeBattleAwardsData data)
	{
		if (data == null)
		{
			NGUITools.SetActive(this.mWinPart, false);
			NGUITools.SetActive(this.mLosePart, true);
			this.LblShiBaiValue.Text = string.Empty;
			this.LblShiBaiDes.Text = Global.GetLang("战斗结束后，奖励将以邮件方式发送给您，请注意查收邮箱。");
		}
		else
		{
			bool flag = data.Success == 1;
			NGUITools.SetActive(this.mWinPart, data.Success == 1);
			NGUITools.SetActive(this.mLosePart, data.Success == 0);
			if (flag)
			{
				this.LblFirstWin.Text = ((data.WinToDay != 1) ? Global.GetLang("获胜奖励：") : Global.GetLang("首胜奖励："));
				this.LblWinValue.Text = Global.GetString(new object[]
				{
					Global.GetLang("战队当前排名："),
					data.RankNum,
					Global.GetLang("  战队杀人数："),
					data.ZhanDuiKillNum,
					Global.GetLang("  积分："),
					data.ModJiFen
				});
				string[] array = (data.WinToDay != 1) ? IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(data.AwardID).WinRankReward.Split(new char[]
				{
					'|'
				}) : IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(data.AwardID).FirstWinRankReward.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(array[i], false, true, true);
					if (!(ggoodIcon == null))
					{
						ggoodIcon.transform.SetParent(this.AwardGoodsRoot.transform);
						ggoodIcon.isAutoSize = false;
						ggoodIcon.transform.localPosition = new Vector3((float)(i * 90), 0f, 0f);
						ggoodIcon.transform.localScale = Vector3.one;
					}
				}
			}
			else
			{
				NGUITools.SetActive(this.BtnShiBaiGuanZhan.gameObject, false);
				this.BtnShiBaiLeave.transform.localPosition = new Vector3(0f, this.BtnShiBaiLeave.transform.localPosition.y, this.BtnShiBaiLeave.transform.localPosition.z);
				this.LblShiBaiValue.Text = Global.GetString(new object[]
				{
					Global.GetLang("战队当前排名："),
					(data.RankNum != 0) ? data.RankNum.ToString() : Global.GetLang("无"),
					Global.GetLang("  战队杀人数："),
					data.ZhanDuiKillNum,
					Global.GetLang("  积分："),
					data.ModJiFen
				});
				string[] array2 = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(data.AwardID).LoseRankReward.Split(new char[]
				{
					'|'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					GGoodIcon ggoodIcon2 = Global.LoadRewardItemGoodsIcon(array2[j], false, true, true);
					if (!(ggoodIcon2 == null))
					{
						ggoodIcon2.transform.SetParent(this.FailureAwardGoodsRoot.transform);
						ggoodIcon2.isAutoSize = false;
						ggoodIcon2.transform.localPosition = new Vector3((float)(j * 90), 0f, 0f);
						ggoodIcon2.transform.localScale = Vector3.one;
					}
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
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblWinTitle;

	public TextBlock LblWinValue;

	public TextBlock LblFirstWin;

	public TextBlock LblShiBaiTitle;

	public TextBlock LblShiBaiValue;

	public TextBlock LblShiBaiDes;

	public GButton BtnWinLeave;

	public GButton BtnShiBaiGuanZhan;

	public GButton BtnShiBaiLeave;

	public GameObject AwardGoodsRoot;

	public GameObject FailureAwardGoodsRoot;

	public GameObject mWinPart;

	public GameObject mLosePart;
}
