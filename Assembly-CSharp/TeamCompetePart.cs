using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TeamCompetePart : UserControl, IMUEventManagerHandler
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblDuanWei.Label.text = Global.GetLang("暂无段位");
		this.LblShengLv.Label.text = Global.GetLang("胜率：");
		this.LblLianSheng.Label.text = Global.GetLang("连胜：");
		this.LblRongYaoCount.Label.text = Global.GetLang("获取荣耀场次：");
		this.LblRole1Name.Label.text = Global.GetLang(string.Empty);
		this.LblRole1Rank.Label.text = Global.GetLang("1");
		this.LblRole1DuanWei.Label.text = Global.GetLang(string.Empty);
		this.LblRole2Name.Label.text = Global.GetLang(string.Empty);
		this.LblRole2Rank.Label.text = Global.GetLang("2");
		this.LblRole2DuanWei.Label.text = Global.GetLang(string.Empty);
		this.LblRole3Name.Label.text = Global.GetLang(string.Empty);
		this.LblRole3Rank.Label.text = Global.GetLang("3");
		this.LblRole3DuanWei.Label.text = Global.GetLang(string.Empty);
		this.BtnBattle.Label.text = Global.GetLang("参与战斗");
		this.LblRole1Rank.Y = 65.0;
		this.LblRole2Rank.Y = 65.0;
		this.LblRole3Rank.Y = 65.0;
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
		this.BtnShop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenMUDuiHuanPart();
		};
		this.BtnZhanBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenTeamCompeteZhanBaoPart();
		};
		this.BtnDuanWeiRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenTeamCompeteRankPart();
		};
		this.BtnAwardPreview.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenJiangLiYuLanPart(0);
		};
		this.BtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenCommonHelpWindow();
		};
		this.IsShowSearchEmenyPanel = false;
		this.BtnBattle.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnClickBattleBtn);
		UIEventListener.Get(this.mBtnCancel.gameObject).onClick = delegate(GameObject s)
		{
			if (this.CurrentSearchType == TeamCompetePart.SearchType.NotFound)
			{
				this.IsShowSearchEmenyPanel = false;
			}
			else
			{
				this.RequestCancelSearchEnemy();
			}
		};
		this.OnCreateTeamCallBack = delegate(bool result)
		{
			this.HaveTeam = result;
			if (result)
			{
				this.IsCreatTeamSuccess = true;
				this.CloseTeamCompeteCreateTeamPart();
			}
		};
	}

	private void InitValue()
	{
		this.RequestMainData();
		this.mTeamConfirmTime = (int)ConfigSystemParam.GetSystemParamIntByName("TeamConfirmTime");
	}

	private bool IsTeamLeader
	{
		get
		{
			return this.mIsTeamLeader;
		}
		set
		{
			this.mIsTeamLeader = value;
			NGUITools.SetActive(this.BtnBattle.gameObject, this.mIsTeamLeader);
		}
	}

	private float ProgressBarValue
	{
		set
		{
			if (value > 0f && this.ProgressBarSumValue > 0f)
			{
				this.mLblProgressBarValue.Text = Global.GetString(new object[]
				{
					value,
					"/",
					this.ProgressBarSumValue
				});
				this.mSprProgressBar.transform.localScale = new Vector3(value / this.ProgressBarSumValue * 408f, this.mSprProgressBar.transform.localScale.y, this.mSprProgressBar.transform.localScale.z);
			}
			else
			{
				this.mLblProgressBarValue.Text = Global.GetString(new object[]
				{
					"0/",
					this.ProgressBarSumValue
				});
				this.mSprProgressBar.transform.localScale = new Vector3(0f, this.mSprProgressBar.transform.localScale.y, this.mSprProgressBar.transform.localScale.z);
			}
		}
	}

	private void OnClickBattleBtn(object sender, MouseEvent e)
	{
		if (!this.IsTeamLeader)
		{
			return;
		}
		this.RequestConfirmBattle();
	}

	private bool IsShowSearchEmenyPanel
	{
		set
		{
			NGUITools.SetActive(this.mFindSomeoneObj, value);
			if (value)
			{
				this.CurrentSearchType = TeamCompetePart.SearchType.Searching;
				this.CountDownTimer();
			}
			else
			{
				this.CurrentSearchType = TeamCompetePart.SearchType.None;
				this.StopSearchEnemyCountDown();
				base.StopAllCoroutines();
			}
		}
	}

	private void CountDownTimer()
	{
		base.StartCoroutine(this.CountDown(this.mTeamConfirmTime, delegate
		{
			this.CurrentSearchType = TeamCompetePart.SearchType.NotFound;
			this.StopSearchEnemyCountDown();
			MUDebug.LogError<string>(new string[]
			{
				"倒计时结束"
			});
		}));
	}

	private IEnumerator CountDown(int time, Action callBack = null)
	{
		for (int i = time; i >= 0; i--)
		{
			this.mLblCountDown.Text = i.ToString();
			yield return new WaitForSeconds(1f);
		}
		if (callBack != null)
		{
			callBack.Invoke();
		}
		yield break;
	}

	private void RefreshRoleInfo()
	{
	}

	private void StopSearchEnemyCountDown()
	{
		base.StopCoroutine(this.CountDown(0, null));
	}

	private int GetRandomSecondsNum
	{
		get
		{
			return Random.Range(1, 11);
		}
	}

	private TeamCompetePart.SearchType CurrentSearchType
	{
		get
		{
			return this.mSearchType;
		}
		set
		{
			this.mSearchType = value;
			switch (this.mSearchType)
			{
			case TeamCompetePart.SearchType.Searching:
				NGUITools.SetActive(this.mSprCountDownDes.gameObject, true);
				NGUITools.SetActive(this.mSprNotFoundDes.gameObject, false);
				NGUITools.SetActive(this.mSprHaveFoundDes.gameObject, false);
				break;
			case TeamCompetePart.SearchType.Searched:
				NGUITools.SetActive(this.mSprCountDownDes.gameObject, false);
				NGUITools.SetActive(this.mSprNotFoundDes.gameObject, false);
				NGUITools.SetActive(this.mSprHaveFoundDes.gameObject, true);
				break;
			case TeamCompetePart.SearchType.NotFound:
				NGUITools.SetActive(this.mSprCountDownDes.gameObject, false);
				NGUITools.SetActive(this.mSprNotFoundDes.gameObject, true);
				NGUITools.SetActive(this.mSprHaveFoundDes.gameObject, false);
				break;
			}
		}
	}

	public void OpenTeamCompeteConfirmPartFromExternal(bool isOpen)
	{
		if (isOpen)
		{
			this.OpenTeamCompeteConfirmPart(null);
		}
	}

	public void OpenTeamCompeteCreateTeamPart()
	{
		if (this.mTeamCompeteCreateTeamPartWind != null || this.mTeamCompeteCreateTeamPart != null)
		{
			this.CloseTeamCompeteCreateTeamPart();
		}
		this.mTeamCompeteCreateTeamPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteCreateTeamPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteCreateTeamPartWind.Modal = true;
		this.mTeamCompeteCreateTeamPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mTeamCompeteCreateTeamPartWind, "mTeamCompeteCreateTeamPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteCreateTeamPartWind);
		this.mTeamCompeteCreateTeamPart = U3DUtils.NEW<TeamCompeteCreateTeamPart>();
		this.mTeamCompeteCreateTeamPart.OnCreateTeamCallBack = this.OnCreateTeamCallBack;
		this.mTeamCompeteCreateTeamPartWind.Body.Add(this.mTeamCompeteCreateTeamPart);
		this.mTeamCompeteCreateTeamPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteCreateTeamPart();
		};
	}

	private void CloseTeamCompeteCreateTeamPart()
	{
		if (null != this.mTeamCompeteCreateTeamPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteCreateTeamPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteCreateTeamPartWind, true);
			this.mTeamCompeteCreateTeamPartWind = null;
		}
		if (null != this.mTeamCompeteCreateTeamPart)
		{
			this.mTeamCompeteCreateTeamPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteCreateTeamPart.gameObject);
			this.mTeamCompeteCreateTeamPart = null;
		}
	}

	public void OpenMUDuiHuanPart()
	{
		if (this.mMUDuiHuanPartWind != null || this.mMUDuiHuanPart != null)
		{
			this.CloseMUDuiHuanPart();
		}
		this.mMUDuiHuanPartWind = U3DUtils.NEW<GChildWindow>();
		this.mMUDuiHuanPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mMUDuiHuanPartWind.Modal = true;
		this.mMUDuiHuanPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mMUDuiHuanPartWind, "mMUDuiHuanPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mMUDuiHuanPartWind);
		this.mMUDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
		this.mMUDuiHuanPartWind.Body.Add(this.mMUDuiHuanPart);
		this.mMUDuiHuanPart.InitPartData(10, 0);
		this.mMUDuiHuanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseMUDuiHuanPart();
			return true;
		};
	}

	private void CloseMUDuiHuanPart()
	{
		if (null != this.mMUDuiHuanPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mMUDuiHuanPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mMUDuiHuanPartWind, true);
			this.mMUDuiHuanPartWind = null;
		}
		if (null != this.mMUDuiHuanPart)
		{
			this.mMUDuiHuanPart.transform.parent = null;
			Object.Destroy(this.mMUDuiHuanPart.gameObject);
			this.mMUDuiHuanPart = null;
		}
	}

	public void OpenTeamCompeteZhanBaoPart()
	{
		if (this.mTeamCompeteZhanBaoPartWind != null || this.mTeamCompeteZhanBaoPart != null)
		{
			this.CloseTeamCompeteZhanBaoPart();
		}
		this.mTeamCompeteZhanBaoPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteZhanBaoPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteZhanBaoPartWind.Modal = true;
		this.mTeamCompeteZhanBaoPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mTeamCompeteZhanBaoPartWind, "mTeamCompeteZhanBaoPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteZhanBaoPartWind);
		this.mTeamCompeteZhanBaoPart = U3DUtils.NEW<TeamCompeteZhanBaoPart>();
		this.mTeamCompeteZhanBaoPartWind.Body.Add(this.mTeamCompeteZhanBaoPart);
		this.mTeamCompeteZhanBaoPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteZhanBaoPart();
		};
	}

	private void CloseTeamCompeteZhanBaoPart()
	{
		if (null != this.mTeamCompeteZhanBaoPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteZhanBaoPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteZhanBaoPartWind, true);
			this.mTeamCompeteZhanBaoPartWind = null;
		}
		if (null != this.mTeamCompeteZhanBaoPart)
		{
			this.mTeamCompeteZhanBaoPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteZhanBaoPart.gameObject);
			this.mTeamCompeteZhanBaoPart = null;
		}
	}

	public void OpenTeamCompeteRankPart()
	{
		if (this.mTeamCompeteRankPartWind != null || this.mTeamCompeteRankPart != null)
		{
			this.CloseTeamCompeteRankPart();
		}
		this.mTeamCompeteRankPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteRankPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteRankPartWind.Modal = true;
		this.mTeamCompeteRankPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mTeamCompeteRankPartWind, "mTeamCompeteRankPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteRankPartWind);
		this.mTeamCompeteRankPart = U3DUtils.NEW<TeamCompeteRankPart>();
		this.mTeamCompeteRankPartWind.Body.Add(this.mTeamCompeteRankPart);
		this.mTeamCompeteRankPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteRankPart();
		};
	}

	private void CloseTeamCompeteRankPart()
	{
		if (null != this.mTeamCompeteRankPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteRankPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteRankPartWind, true);
			this.mTeamCompeteRankPartWind = null;
		}
		if (null != this.mTeamCompeteRankPart)
		{
			this.mTeamCompeteRankPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteRankPart.gameObject);
			this.mTeamCompeteRankPart = null;
		}
	}

	public void OpenCommonHelpWindow()
	{
		if (this.mCommonHelpWindowWind != null || this.mCommonHelpWindow != null)
		{
			this.CloseCommonHelpWindow();
		}
		this.mCommonHelpWindowWind = U3DUtils.NEW<GChildWindow>();
		this.mCommonHelpWindowWind.ModalType = ChildWindowModalType.Translucent;
		this.mCommonHelpWindowWind.Modal = true;
		this.mCommonHelpWindowWind.IsShowModal = true;
		Super.InitChildWindow(this.mCommonHelpWindowWind, "mCommonHelpWindowWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mCommonHelpWindowWind);
		this.mCommonHelpWindow = U3DUtils.NEW<CommonHelpWindow>();
		this.mCommonHelpWindowWind.Body.Add(this.mCommonHelpWindow);
		this.mCommonHelpWindow.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCommonHelpWindow();
		};
		this.mCommonHelpWindow.SetHelpInfo(IConfigbase<ConfigTeamCompete>.Instance.GetTeamCompeteHelpInfo(0).list);
	}

	private void CloseCommonHelpWindow()
	{
		if (null != this.mCommonHelpWindowWind)
		{
			Super.CloseChildWindow(base.Children, this.mCommonHelpWindowWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mCommonHelpWindowWind, true);
			this.mCommonHelpWindowWind = null;
		}
		if (null != this.mCommonHelpWindow)
		{
			this.mCommonHelpWindow.transform.parent = null;
			Object.Destroy(this.mCommonHelpWindow.gameObject);
			this.mCommonHelpWindow = null;
		}
	}

	public void OpenJiangLiYuLanPart(int rankId = 0)
	{
		if (this.mJiangLiYuLanPartWind != null || this.mJiangLiYuLanPart != null)
		{
			this.CloseJiangLiYuLanPart();
		}
		this.initXmlData();
		this.mJiangLiYuLanPartWind = U3DUtils.NEW<GChildWindow>();
		this.mJiangLiYuLanPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mJiangLiYuLanPartWind.Modal = true;
		this.mJiangLiYuLanPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mJiangLiYuLanPartWind, "mJiangLiYuLanPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mJiangLiYuLanPartWind);
		this.mJiangLiYuLanPart = U3DUtils.NEW<JiangLiYuLanPart>();
		this.mJiangLiYuLanPartWind.Body.Add(this.mJiangLiYuLanPart);
		this.mJiangLiYuLanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseJiangLiYuLanPart();
		};
		if (TeamCompeteDataManager.MainZhanDuiData != null && TeamCompeteDataManager.MainZhanDuiData.MonthDuanWeiRank > 0)
		{
			rankId = TeamCompeteDataManager.MainZhanDuiData.MonthDuanWeiRank;
		}
		base.StartCoroutine<bool>(this.mJiangLiYuLanPart.init(this.awardXmlList, rankId, null));
	}

	private void CloseJiangLiYuLanPart()
	{
		if (null != this.mJiangLiYuLanPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mJiangLiYuLanPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mJiangLiYuLanPartWind, true);
			this.mJiangLiYuLanPartWind = null;
		}
		if (null != this.mJiangLiYuLanPart)
		{
			this.mJiangLiYuLanPart.transform.parent = null;
			Object.Destroy(this.mJiangLiYuLanPart.gameObject);
			this.mJiangLiYuLanPart = null;
		}
	}

	private void initXmlData()
	{
		if (this.awardXmlList != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/TeamDuanWeiAward.xml");
		if (gameResXml != null)
		{
			this.awardXmlList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		}
	}

	public void OpenTeamCompeteConfirmPart(Action<int> ConfirmCallBack)
	{
		if (this.mTeamCompeteConfirmPartWind != null || this.mTeamCompeteConfirmPart != null)
		{
			if (ConfirmCallBack == null)
			{
				this.mTeamCompeteConfirmPart.RefreshData();
				return;
			}
			this.CloseTeamCompeteConfirmPart();
		}
		this.mTeamCompeteConfirmPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteConfirmPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteConfirmPartWind.Modal = true;
		this.mTeamCompeteConfirmPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mTeamCompeteConfirmPartWind, "mTeamCompeteConfirmPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteConfirmPartWind);
		this.mTeamCompeteConfirmPart = U3DUtils.NEW<TeamCompeteConfirmPart>();
		TeamCompeteDataManager.SecondConfirmWindowCallBack = new Action(this.CloseExistConfirmWindow);
		this.mTeamCompeteConfirmPart.ResultCallBack = ConfirmCallBack;
		this.mTeamCompeteConfirmPartWind.Body.Add(this.mTeamCompeteConfirmPart);
		this.mTeamCompeteConfirmPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteConfirmPart();
		};
	}

	private void CloseExistConfirmWindow()
	{
		this.CloseTeamCompeteConfirmPart();
		TeamCompeteDataManager.SecondConfirmWindowCallBack = null;
	}

	private void CloseTeamCompeteConfirmPart()
	{
		if (null != this.mTeamCompeteConfirmPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteConfirmPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteConfirmPartWind, true);
			this.mTeamCompeteConfirmPartWind = null;
		}
		if (null != this.mTeamCompeteConfirmPart)
		{
			this.mTeamCompeteConfirmPart.ResultCallBack = null;
			this.mTeamCompeteConfirmPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteConfirmPart.gameObject);
			this.mTeamCompeteConfirmPart = null;
		}
	}

	public void OpenCommonCompeteMonthAwardPart(int rankId = 0)
	{
		if (this.mCommonCompeteMonthAwardPartWind != null || this.mCommonCompeteMonthAwardPart != null)
		{
			this.CloseCommonCompeteMonthAwardPart();
		}
		this.mCommonCompeteMonthAwardPartWind = U3DUtils.NEW<GChildWindow>();
		this.mCommonCompeteMonthAwardPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mCommonCompeteMonthAwardPartWind.Modal = true;
		this.mCommonCompeteMonthAwardPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mCommonCompeteMonthAwardPartWind, "mCommonCompeteMonthAwardPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mCommonCompeteMonthAwardPartWind);
		this.mCommonCompeteMonthAwardPart = U3DUtils.NEW<CommonCompeteMonthAwardPart>();
		this.mCommonCompeteMonthAwardPartWind.Body.Add(this.mCommonCompeteMonthAwardPart);
		this.mCommonCompeteMonthAwardPart.InitTeamCompeteAward(rankId);
		this.mCommonCompeteMonthAwardPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCommonCompeteMonthAwardPart();
		};
	}

	private void CloseCommonCompeteMonthAwardPart()
	{
		if (null != this.mCommonCompeteMonthAwardPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mCommonCompeteMonthAwardPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mCommonCompeteMonthAwardPartWind, true);
			this.mCommonCompeteMonthAwardPartWind = null;
		}
		if (null != this.mCommonCompeteMonthAwardPart)
		{
			this.mCommonCompeteMonthAwardPart.transform.parent = null;
			Object.Destroy(this.mCommonCompeteMonthAwardPart.gameObject);
			this.mCommonCompeteMonthAwardPart = null;
		}
	}

	public void RespondRankAward(MUSocketConnectEventArgs e)
	{
		if (this.mCommonCompeteMonthAwardPart != null)
		{
			this.mCommonCompeteMonthAwardPart.RespondAcceptAward(e);
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_SPR_GET_KF5V5_INFO", new Action<MUSocketConnectEventArgs>(this.RespondMainData));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ROLE_SIGN", new Action<MUSocketConnectEventArgs>(this.RespondSearchEnemy));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ROLE_CANCLE_SIGN", new Action<MUSocketConnectEventArgs>(this.RespondCancelSearchEnemy));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_NOTIFY_PIPEI_SUCCESS", new Action<MUSocketConnectEventArgs>(this.RespondSuccessSearchEnemy));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ENTER_KF5V5_SCENE", new Action<MUSocketConnectEventArgs>(this.RespondEnterGame));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CONFIRM_BATTLE", new Action<MUSocketConnectEventArgs>(this.RespondConfirmBattle));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_SPR_GET_KF5V5_INFO", new Action<MUSocketConnectEventArgs>(this.RespondMainData));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ROLE_SIGN", new Action<MUSocketConnectEventArgs>(this.RespondSearchEnemy));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ROLE_CANCLE_SIGN", new Action<MUSocketConnectEventArgs>(this.RespondCancelSearchEnemy));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_NOTIFY_PIPEI_SUCCESS", new Action<MUSocketConnectEventArgs>(this.RespondSuccessSearchEnemy));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ENTER_KF5V5_SCENE", new Action<MUSocketConnectEventArgs>(this.RespondEnterGame));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CONFIRM_BATTLE", new Action<MUSocketConnectEventArgs>(this.RespondConfirmBattle));
	}

	public void RequestMainData()
	{
		GameInstance.Game.RequestTeamCompeteMianInfoMsg();
	}

	public void RespondMainData(MUSocketConnectEventArgs e)
	{
		TianTi5v5DataAndDayPaiHang tianTi5v5DataAndDayPaiHang = DataHelper.BytesToObject<TianTi5v5DataAndDayPaiHang>(e.bytesData, 0, e.bytesData.Length);
		if (tianTi5v5DataAndDayPaiHang != null)
		{
			TeamCompeteDataManager.HaveMonthPaiHangAwards = tianTi5v5DataAndDayPaiHang.HaveMonthPaiHangAwards;
			TeamCompeteDataManager.TodayFightCount = tianTi5v5DataAndDayPaiHang.TodayFightCount;
			this.InitSelfInfo(tianTi5v5DataAndDayPaiHang.TianTi5v5Data);
			this.InitRankInfo(tianTi5v5DataAndDayPaiHang.PaiHangRoleDataList);
			if (TeamCompeteDataManager.HaveMonthPaiHangAwards > 0 && tianTi5v5DataAndDayPaiHang.TianTi5v5Data != null)
			{
				this.OpenCommonCompeteMonthAwardPart(tianTi5v5DataAndDayPaiHang.TianTi5v5Data.MonthDuanWeiRank);
			}
		}
		else
		{
			this.NoTeam();
		}
	}

	public void RequestSearchEnemy()
	{
		this.IsShowSearchEmenyPanel = true;
		GameInstance.Game.SendSearchEnemyMsg();
	}

	public void RespondSearchEnemy(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		this.CloseTeamCompeteConfirmPart();
		if (num > 0)
		{
			if (num == -12)
			{
				return;
			}
			this.IsShowSearchEmenyPanel = true;
		}
		else
		{
			TeamCompeteDataManager.ErrorTips(num);
		}
	}

	public void RequestCancelSearchEnemy()
	{
	}

	public void RespondCancelSearchEnemy(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num >= 0)
		{
			if (Global.Data.roleData != null && Global.Data.roleData.ZhanDuiZhiWu == 0)
			{
				Super.HintMainText(Global.GetLang("队长取消匹配"), 10, 3);
				this.CloseTeamCompeteConfirmPart();
				TeamCompeteDataManager.CloseAllTipWindow();
			}
			return;
		}
		if (num == -12)
		{
			return;
		}
		TeamCompeteDataManager.ErrorTips(num);
	}

	public void RespondSuccessSearchEnemy(MUSocketConnectEventArgs e)
	{
		int gameId = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		this.CurrentSearchType = TeamCompetePart.SearchType.Searched;
		base.StopAllCoroutines();
		base.StartCoroutine(this.CountDown(3, delegate
		{
			GameInstance.Game.RequestEnterTeamCompeteScene(gameId);
		}));
	}

	public void RespondEnterGame(MUSocketConnectEventArgs e)
	{
	}

	public void RequestConfirmBattle()
	{
		GameInstance.Game.SendTeamLeaderConfirmBattleMsg();
	}

	public void RespondConfirmBattle(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			TeamCompeteDataManager.ErrorTips(num);
		}
	}

	private void InitSelfInfo(TianTi5v5ZhanDuiData data)
	{
		if (data != null)
		{
			if (data.ZhanDuiID <= 0)
			{
				this.NoTeam();
			}
			else
			{
				this.HaveTeam = true;
				this.IsTeamLeader = (data.LeaderRoleID == Global.Data.roleData.RoleID);
				TeamCompeteDataManager.MainZhanDuiData = data;
				this.LblDuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
				this.LblShengLv.Text = Global.GetString(new object[]
				{
					Global.GetLang("胜率："),
					this.ShengLv(data.SuccessCount, data.FightCount)
				});
				this.LblLianSheng.Text = Global.GetString(new object[]
				{
					Global.GetLang("连胜："),
					data.LianSheng
				});
				this.LblRongYaoCount.Text = Global.GetString(new object[]
				{
					Global.GetLang("获取荣耀场次："),
					TeamCompeteDataManager.TodayFightCount,
					"/",
					TeamCompeteDataManager.GetDuanWeiRongYaoNumByID(data.DuanWeiId)
				});
				this.ProgressBarSumValue = (float)TeamCompeteDataManager.GetDuanWeiNeedJiFenByID(data.DuanWeiId);
				this.ProgressBarValue = (float)data.DuanWeiJiFen;
			}
		}
		else
		{
			this.NoTeam();
		}
	}

	private void NoTeam()
	{
		this.HaveTeam = false;
		this.IsTeamLeader = false;
		TeamCompeteDataManager.MainZhanDuiData = null;
		this.LblDuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID(0);
		this.LblShengLv.Text = Global.GetString(new object[]
		{
			Global.GetLang("胜率："),
			"0%"
		});
		this.LblLianSheng.Text = Global.GetString(new object[]
		{
			Global.GetLang("连胜："),
			0
		});
		this.LblRongYaoCount.Text = Global.GetString(new object[]
		{
			Global.GetLang("获取荣耀场次："),
			0
		});
		this.ProgressBarValue = 0f;
		TeamCompeteDataManager.HaveMonthPaiHangAwards = 0;
		TeamCompeteDataManager.TodayFightCount = 0;
	}

	private string ShengLv(int lianSheng, int fightCount)
	{
		if (fightCount <= 0)
		{
			return "0%";
		}
		return ((float)lianSheng / (float)fightCount).ToString("p1");
	}

	private void InitRankInfo(List<TianTi5v5ZhanDuiData> datas)
	{
		if (datas != null)
		{
			if (datas.Count == 1)
			{
				this.Rank1Info(datas[0]);
			}
			else if (datas.Count == 2)
			{
				base.StartCoroutine(this.LoadRoleInfoByCoroutine(datas));
			}
			else if (datas.Count == 3)
			{
				base.StartCoroutine(this.LoadRoleInfoByCoroutine(datas));
			}
		}
	}

	private IEnumerator LoadRoleInfoByCoroutine(List<TianTi5v5ZhanDuiData> datas)
	{
		for (int i = 0; i < datas.Count; i++)
		{
			if (i == 0)
			{
				this.Rank1Info(datas[0]);
			}
			else if (i == 1)
			{
				this.Rank2Info(datas[1]);
			}
			else
			{
				this.Rank3Info(datas[2]);
			}
			yield return null;
		}
		yield break;
	}

	private void Rank1Info(TianTi5v5ZhanDuiData data)
	{
		this.LblRole1DuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		this.LblRole1Name.Text = data.ZhanDuiName;
		this.LblRole1Rank.Text = "1";
		if (data.teamerList.Count > 0)
		{
			this.LoadRoleInfo(TeamCompeteDataManager.GetRoleData4Selector(data.teamerList[0].ModelData), 0);
		}
	}

	private void Rank2Info(TianTi5v5ZhanDuiData data)
	{
		this.LblRole2DuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		this.LblRole2Name.Text = data.ZhanDuiName;
		this.LblRole2Rank.Text = "2";
		if (data.teamerList.Count > 0)
		{
			this.LoadRoleInfo(TeamCompeteDataManager.GetRoleData4Selector(data.teamerList[0].ModelData), 1);
		}
	}

	private void Rank3Info(TianTi5v5ZhanDuiData data)
	{
		this.LblRole3DuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		this.LblRole3Name.Text = data.ZhanDuiName;
		this.LblRole3Rank.Text = "3";
		if (data.teamerList.Count > 0)
		{
			this.LoadRoleInfo(TeamCompeteDataManager.GetRoleData4Selector(data.teamerList[0].ModelData), 2);
		}
	}

	private void LoadRoleInfo(RoleData4Selector _RoleData4Selector, int index)
	{
		Modal3DShow modal3DShow = this.modals[index];
		int fashionGoodsID = Global.GetFashionGoodsID(_RoleData4Selector.FashionWingsID);
		this.roleResLoader = UIHelper.LoadRoleRes(modal3DShow, _RoleData4Selector.SettingBitFlags, _RoleData4Selector.Occupation, _RoleData4Selector.SubOccupation, _RoleData4Selector.OtherName, _RoleData4Selector.GoodsDataList, null, _RoleData4Selector.MyWingData, 1f, fashionGoodsID, null, false);
		UIHelper.SetModalPosZ(modal3DShow.transform);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		TeamCompeteDataManager.Clear();
		if (this.awardXmlList != null && this.awardXmlList.Count > 0)
		{
			this.awardXmlList.Clear();
		}
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		this.StopSearchEnemyCountDown();
		base.StopAllCoroutines();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public TextBlock LblDuanWei;

	public TextBlock LblShengLv;

	public TextBlock LblLianSheng;

	public TextBlock LblRongYaoCount;

	public TextBlock LblRole1Name;

	public TextBlock LblRole1Rank;

	public TextBlock LblRole1DuanWei;

	public TextBlock LblRole2Name;

	public TextBlock LblRole2Rank;

	public TextBlock LblRole2DuanWei;

	public TextBlock LblRole3Name;

	public TextBlock LblRole3Rank;

	public TextBlock LblRole3DuanWei;

	public GButton BtnClose;

	public GButton BtnZhanDui;

	public GButton BtnShop;

	public GButton BtnZhanBao;

	public GButton BtnDuanWeiRank;

	public GButton BtnAwardPreview;

	public GButton BtnHelp;

	public GButton BtnBattle;

	public UISprite mSprProgressBar;

	public TextBlock mLblProgressBarValue;

	public Modal3DShow[] modals;

	public UIButton mBtnCancel;

	public GameObject mFindSomeoneObj;

	public TextBlock mLblCountDown;

	public UISprite mSprCountDownDes;

	public UISprite mSprNotFoundDes;

	public UISprite mSprHaveFoundDes;

	private bool HaveTeam;

	private int mTeamConfirmTime;

	private Action<bool> OnCreateTeamCallBack;

	private bool IsCreatTeamSuccess;

	private bool mIsTeamLeader;

	private float ProgressBarSumValue;

	private TeamCompetePart.SearchType mSearchType;

	protected GChildWindow mTeamCompeteCreateTeamPartWind;

	protected TeamCompeteCreateTeamPart mTeamCompeteCreateTeamPart;

	protected GChildWindow mMUDuiHuanPartWind;

	protected MUDuiHuanPart mMUDuiHuanPart;

	protected GChildWindow mTeamCompeteZhanBaoPartWind;

	protected TeamCompeteZhanBaoPart mTeamCompeteZhanBaoPart;

	protected GChildWindow mTeamCompeteRankPartWind;

	protected TeamCompeteRankPart mTeamCompeteRankPart;

	protected GChildWindow mCommonHelpWindowWind;

	protected CommonHelpWindow mCommonHelpWindow;

	protected GChildWindow mJiangLiYuLanPartWind;

	protected JiangLiYuLanPart mJiangLiYuLanPart;

	private List<XElement> awardXmlList;

	protected GChildWindow mTeamCompeteConfirmPartWind;

	protected TeamCompeteConfirmPart mTeamCompeteConfirmPart;

	protected GChildWindow mCommonCompeteMonthAwardPartWind;

	protected CommonCompeteMonthAwardPart mCommonCompeteMonthAwardPart;

	private RoleResLoader roleResLoader;

	private enum SearchType
	{
		None,
		Searching,
		Searched,
		NotFound
	}
}
