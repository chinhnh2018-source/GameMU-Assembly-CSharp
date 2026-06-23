using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class FubenBoxMini : UserControl
{
	public long StartTicks
	{
		get
		{
			return this._StartTicks;
		}
		set
		{
			this._StartTicks = value;
		}
	}

	public long EndTicks
	{
		get
		{
			return this._EndTicks;
		}
		set
		{
			this._EndTicks = value;
		}
	}

	public int KillEnemiesNum
	{
		get
		{
			return this._KillEnemiesNum;
		}
		set
		{
			this._KillEnemiesNum = value;
		}
	}

	public int TopScore
	{
		get
		{
			return this._TopScore;
		}
		set
		{
			this._TopScore = value;
		}
	}

	public int SuiEnemiesNum
	{
		get
		{
			return this._SuiEnemiesNum;
		}
		set
		{
			this._SuiEnemiesNum = value;
		}
	}

	public int TangEnemiesNum
	{
		get
		{
			return this._TangEnemiesNum;
		}
		set
		{
			this._TangEnemiesNum = value;
		}
	}

	public int RemainPlayer
	{
		get
		{
			return this._RemainPlayer;
		}
		set
		{
			this._RemainPlayer = value;
		}
	}

	public int SelfScore
	{
		get
		{
			return this._SelfScore;
		}
		set
		{
			this._SelfScore = value;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.CancelInvoke("JunTuanCaiJiCountDown");
		this.StopTimer();
		base.StopCoroutine("CheckTimeOutHaveDispose");
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("FuBenBoxMini_Timer");
			this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
			this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		}
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
		this.EndTicks = 0L;
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (PlayZone.GlobalPlayZone == null || PlayZone.GlobalPlayZone.GameFubenBoxMini == null)
		{
			return;
		}
		FubenBoxMini gameFubenBoxMini = PlayZone.GlobalPlayZone.GameFubenBoxMini;
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (Global.IsInDaTaoShaPrepare() || Global.IsInDaTaoSha())
		{
			this.ProcessDaTaoShaCountDown(gameFubenBoxMini, correctLocalTime);
			return;
		}
		if (gameFubenBoxMini.Visibility)
		{
			if (string.IsNullOrEmpty(this.TimeString))
			{
				this.TimeString = Global.GetLang(" 战斗时间 {0} ");
			}
			if (correctLocalTime < this.EndTicks)
			{
				long num = (this.EndTicks - correctLocalTime) / 1000L;
				string text;
				if (num < 3600L)
				{
					text = UIHelper.FormatSecsMS(num, "00:00");
				}
				else
				{
					text = UIHelper.FormatSecs2(num, "01:00:00");
				}
				this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), text);
			}
			else if (this.EndTicks > this.StartTicks)
			{
				this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), "00:00");
			}
		}
		if (!this.TimerEnabled)
		{
			return;
		}
		if (correctLocalTime < this.EndTicks)
		{
			long num2 = (this.EndTicks - correctLocalTime) / 1000L;
			if (this._SceneUIClass == SceneUIClasses.PKKing)
			{
				if (this._SceneState == SceneStates.Task && num2 <= 3L)
				{
					SystemHelpPart.Countdown(3, null, false);
				}
			}
			else if (this._SceneUIClass == SceneUIClasses.AngelTemple && (this._SceneState == SceneStates.Task || this._SceneState == SceneStates.Enter) && num2 <= 3L)
			{
				SystemHelpPart.Countdown(3, null, false);
			}
		}
		else if (this.EndTicks > this.StartTicks)
		{
			this.OnTimeOut();
			if (Global.GetIsOnePieceFuBen(this.FuBenID))
			{
				base.StartCoroutine(this.CheckTimeOutHaveDispose());
			}
		}
		switch (this._SceneUIClass)
		{
		case SceneUIClasses.BloodCastle:
			if (this._SceneState == SceneStates.Task && this.StartTicks > 0L && correctLocalTime < this.StartTicks + 10000L)
			{
				Global.Data.GameScene.SetGuangMuState(1, 0, false, -1);
			}
			break;
		case SceneUIClasses.AngelTemple:
			if (this._SceneState == SceneStates.Task && this.StartTicks > 0L && correctLocalTime < this.StartTicks + 10000L)
			{
				Global.Data.GameScene.ClearTianShiShenDianBaoZuDang0();
			}
			break;
		case SceneUIClasses.ShiLian:
			if (this._SceneState != SceneStates.Enter && this._SceneState != SceneStates.None)
			{
				Global.Data.GameScene.ClearZhuTiFuShiLianZuDang();
			}
			break;
		}
	}

	private void ProcessDaTaoShaCountDown(FubenBoxMini tthis, long ticks)
	{
		if (tthis.Visibility)
		{
			if (ticks < this.EndTicks)
			{
				long num = (this.EndTicks - ticks) / 1000L;
				string text;
				if (num < 3600L)
				{
					text = UIHelper.FormatSecsMS(num, "00:00");
				}
				else
				{
					text = UIHelper.FormatSecs2(num, "01:00:00");
				}
				this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), text);
			}
			else if (this.EndTicks > this.StartTicks)
			{
				this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), "00:00");
			}
		}
	}

	public SceneStates SceneState
	{
		get
		{
			return this._SceneState;
		}
	}

	protected override void InitializeComponent()
	{
		this.LeaveFubenIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnLeaveFuBenIconClick);
		this.StartUITimer();
	}

	public void ShowAnim(int id, int param)
	{
		if (param != 0)
		{
			this.ChangeSceneState(SceneStates.Clear);
		}
		this.Anim.SetActive(param != 0);
	}

	public void ProcessPataEnd()
	{
		if (this._SceneUIClass == SceneUIClasses.PaTa)
		{
			if (this.IsHavePataNextIndex())
			{
				this.ChangeSceneState(SceneStates.Next);
			}
			else
			{
				this.ChangeSceneState(SceneStates.Clear);
			}
			this.SetSceneTaskBtn(true);
		}
	}

	public void OnLeaveFuBenIconClick(object sender, MouseEvent e)
	{
		this.m_OnTimeOutHaveDispose = true;
		this.ShowAnim(0, 0);
		if (Global.IsInDaTaoShaPrepare())
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 5
			}));
			return;
		}
		if (Global.IsInDaTaoSha())
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 2
			}));
			return;
		}
		if (null != this._FuBenTongGuanPart && this._FuBenTongGuanPart && this._FuBenTongGuanPart.IsActive)
		{
			this._FuBenTongGuanPart.ShowMainPart(true);
			return;
		}
		if (this._SceneUIClass == SceneUIClasses.Battle)
		{
			if (this.SceneState < SceneStates.Clear)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 3
				}));
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}
		}
		else if (this._SceneUIClass == SceneUIClasses.BloodCastle)
		{
			if (this.SceneState < SceneStates.Clear)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 1
				}));
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}
		}
		else if (this._SceneUIClass == SceneUIClasses.Demon)
		{
			if (this.SceneState < SceneStates.Clear)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 1
				}));
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}
		}
		else if (this._SceneUIClass == SceneUIClasses.TaskCopy)
		{
			if (this.SceneState < SceneStates.Clear)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 1
				}));
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}
		}
		else if (this._SceneUIClass == SceneUIClasses.YongZheZhanChang)
		{
			if (this.SceneState < SceneStates.Clear)
			{
				if (0 < Global.YongzhezhanchangLianShaInf)
				{
					Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("当前离开会清空连杀状态，确定要离开？"), delegate(object s2, DPSelectedItemEventArgs e2)
					{
						if (e2.ID == 0)
						{
							Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
							{
								NpcID = 1000000,
								ScriptID = 10,
								Hint = 0
							}));
						}
					}, new string[]
					{
						Global.GetLang("离开"),
						Global.GetLang("取消")
					});
				}
				else
				{
					Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
					{
						NpcID = 1000000,
						ScriptID = 10,
						Hint = 1
					}));
				}
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}
		}
		else if (this._SceneUIClass == SceneUIClasses.LingDiCaiJi)
		{
			if (this.SceneState < SceneStates.Clear)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 4
				}));
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}
		}
		else if (this.SceneState < SceneStates.Clear)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 1
			}));
		}
		else
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
		}
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			IDType = 0,
			ID = this.FuBenID,
			Type = (int)this._SceneUIClass
		});
	}

	public bool IsHavePataNextIndex()
	{
		int num = Global.Data.roleData.MapCode;
		num++;
		return num >= Global.GetPataIndexRange()[0] && num <= Global.GetPataIndexRange()[1];
	}

	public bool BodyVisible
	{
		get
		{
			return this.Body.Visibility;
		}
		set
		{
			this.Body.Visibility = value;
			this.m_OnTimeOutHaveDispose = false;
			base.StopCoroutine("CheckTimeOutHaveDispose");
		}
	}

	public string FubenEndTime
	{
		get
		{
			return this.TxtFubenEndTime.Text;
		}
		set
		{
			this.TxtFubenEndTime.Text = value;
		}
	}

	public void EnterMapScene()
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (this.CurrentMapCode == Global.Data.roleData.MapCode && this._SceneUIClass == mapSceneUIClass)
		{
			return;
		}
		this.CurrentMapCode = Global.Data.roleData.MapCode;
		this._SceneUIClass = mapSceneUIClass;
		if (this._SceneUIClass == SceneUIClasses.TaskCopy)
		{
			this.ChangeSceneState(SceneStates.Task);
			this.LeaveFubenIcon.isEnabled = true;
		}
		else if (this._SceneUIClass == SceneUIClasses.NormalCopy || this._SceneUIClass == SceneUIClasses.FamilyBoss || this._SceneUIClass == SceneUIClasses.JingYanFuBen || this._SceneUIClass == SceneUIClasses.KaLiMaTemple || this._SceneUIClass == SceneUIClasses.PaTa || this._SceneUIClass == SceneUIClasses.JinBiFuBen || this._SceneUIClass == SceneUIClasses.EMoLaiXiCopy || this._SceneUIClass == SceneUIClasses.LuolanFazhen || Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass) || this._SceneUIClass == SceneUIClasses.LoveFuBen || this._SceneUIClass == SceneUIClasses.OnePiece)
		{
			if (this._SceneUIClass == SceneUIClasses.DaTaoSha)
			{
				this.TimeString = Global.GetLang("战场入场倒计时 {0}");
				this.LeaveFubenIcon.isEnabled = true;
			}
			else
			{
				this.ChangeSceneState(SceneStates.FuBen);
				this.LeaveFubenIcon.isEnabled = true;
			}
		}
		else if (this._SceneUIClass == SceneUIClasses.BloodCastle || this._SceneUIClass == SceneUIClasses.AngelTemple || this._SceneUIClass == SceneUIClasses.Demon || this._SceneUIClass == SceneUIClasses.FamilyBoss)
		{
			this.LeaveFubenIcon.isEnabled = true;
		}
		else if (this._SceneUIClass == SceneUIClasses.Battle)
		{
			this.LeaveFubenIcon.isEnabled = true;
		}
		else if (this._SceneUIClass == SceneUIClasses.PKKing)
		{
			this.LeaveFubenIcon.isEnabled = true;
			this.NotifySceneMsg(this.PKKingTimeType, this.PKKingLeftSecs);
		}
		else if (this._SceneUIClass == SceneUIClasses.NewPlayerMap)
		{
			this.ChangeSceneState(SceneStates.Task);
			this.LeaveFubenIcon.isEnabled = false;
		}
		else if (this._SceneUIClass == SceneUIClasses.JingJiChang)
		{
			this.ChangeSceneState(SceneStates.Task);
			this.LeaveFubenIcon.isEnabled = true;
		}
		else if (this._SceneUIClass == SceneUIClasses.GuZhanChang || this._SceneUIClass == SceneUIClasses.ShuiJingHuanJing || this._SceneUIClass == SceneUIClasses.MoYu)
		{
			this.ChangeSceneState(SceneStates.Remain);
			this.RefreshLeftSecs(-1);
			this.LeaveFubenIcon.isEnabled = true;
		}
		else if (this._SceneUIClass == SceneUIClasses.LuoLanChengZhan)
		{
			this.NotifySceneTimeInfo(SceneStates.Task, (double)Global.GetCorrectLocalTime(), (double)(Global.GetCorrectLocalTime() + (long)(Global.GetLuolanchengzhanTime() * 1000)));
		}
		else if (this._SceneUIClass == SceneUIClasses.LangHunLingYu)
		{
			this.NotifySceneTimeInfo(SceneStates.Task, (double)Global.GetCorrectLocalTime(), (double)(Global.GetCorrectLocalTime() + (long)(Global.GetLangHunLingYuTime() * 1000)));
		}
		else if (this._SceneUIClass == SceneUIClasses.DaTaoSha)
		{
			this.TimeString = Global.GetLang("战场入场倒计时 {0}");
			this.LeaveFubenIcon.isEnabled = true;
		}
		else
		{
			this.LeaveFubenIcon.isEnabled = true;
		}
		if (Global.Data.roleData.MapCode >= 80000 && Global.Data.roleData.MapCode <= 80020)
		{
			this.ChangeSceneState(SceneStates.FuBen);
		}
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime > this.EndTicks && correctLocalTime > this.StartTicks + 1000L)
		{
			this.StartTicks = 0L;
			this.EndTicks = 0L;
		}
	}

	public void InitPartData()
	{
		this.TxtFubenEndTime.text = null;
		this.EnterMapScene();
		if (this._SceneUIClass == SceneUIClasses.TaskCopy || this._SceneUIClass == SceneUIClasses.NewPlayerMap || this._SceneUIClass == SceneUIClasses.NormalCopy || this._SceneUIClass == SceneUIClasses.PaTa || this._SceneUIClass == SceneUIClasses.JinBiFuBen || this._SceneUIClass == SceneUIClasses.EMoLaiXiCopy)
		{
			GameInstance.Game.SpriteGetFuBenBeginInfo();
		}
		else if (this._SceneUIClass == SceneUIClasses.KaLiMaTemple || this._SceneUIClass == SceneUIClasses.LoveFuBen)
		{
			GameInstance.Game.SpriteGetFuBenBeginInfo();
			GameInstance.Game.SpriteGetFuBenMonstersNum();
		}
		else if (this._SceneUIClass == SceneUIClasses.BloodCastle)
		{
			GameInstance.Game.QueryActivitySomeInfo(1);
		}
		else if (this._SceneUIClass == SceneUIClasses.Demon)
		{
			GameInstance.Game.QueryActivitySomeInfo(2);
		}
		else if (this._SceneUIClass == SceneUIClasses.JingYanFuBen || this._SceneUIClass == SceneUIClasses.OnePiece)
		{
			GameInstance.Game.SpriteGetFuBenBeginInfo();
			GameInstance.Game.SpriteQueryExperienceCopyMapInfoCmd();
		}
		else if (this._SceneUIClass == SceneUIClasses.ShuiJingHuanJing)
		{
			GameInstance.Game.SpriteQueryCaijiLastNum(0);
		}
		else if (this._SceneUIClass == SceneUIClasses.FamilyBoss)
		{
			GameInstance.Game.SpriteGetCopyTeamDamageInfo();
			this.ChangeSceneState(SceneStates.None);
		}
		else if (this._SceneUIClass != SceneUIClasses.LuolanFazhen)
		{
			if (Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass))
			{
				GameInstance.Game.GetHuanYingTime();
				GameInstance.Game.GetHuanYingScore();
			}
			else if (this._SceneUIClass == SceneUIClasses.DaTaoSha)
			{
				MUDebug.LogError<string>(new string[]
				{
					"大逃杀战斗场景请求状态数据 CMD_SPR_NOTIFY_TIME_STATE"
				});
				GameInstance.Game.GetHuanYingTime();
			}
		}
		this.RefreshSceneScoreInfo(false);
		if (this._SceneUIClass == SceneUIClasses.DaTaoSha || this._SceneUIClass == SceneUIClasses.DaTaoShaPrepare)
		{
			this.Anim.SetActive(false);
		}
		else
		{
			this.ShowAnim(0, 0);
		}
	}

	public bool ChangeSceneState(SceneStates value)
	{
		if (this._SceneUIClass == SceneUIClasses.JinBiFuBen && value == SceneStates.Clear)
		{
			return false;
		}
		this._SceneState = value;
		this.TimerEnabled = (this._SceneState != SceneStates.None);
		SceneStates sceneState = this._SceneState;
		switch (sceneState)
		{
		case SceneStates.Clear:
			this.TimeString = Global.GetLang("清场倒计时 {0}");
			break;
		case SceneStates.Next:
			this.TimeString = Global.GetLang("下层倒计时 {0}");
			break;
		case SceneStates.Remain:
			this.TimeString = Global.GetLang(" 剩余时间 {0} ");
			break;
		default:
			if (sceneState != SceneStates.None)
			{
				if (sceneState != SceneStates.Enter)
				{
					if (sceneState != SceneStates.Task)
					{
						if (sceneState != SceneStates.FuBen)
						{
							this.TimeString = Global.GetLang(string.Empty);
						}
						else if (Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass))
						{
							this.TimeString = Global.GetLang("结束倒计时 {0}");
						}
						else
						{
							this.TimeString = Global.GetLang("副本倒计时 {0}");
						}
					}
					else
					{
						this.TimeString = Global.GetLang("战斗倒计时 {0}");
					}
				}
				else
				{
					this.TimeString = Global.GetLang("入场倒计时 {0}");
				}
			}
			break;
		}
		return true;
	}

	public static void TrySendLuoLanFaZhenQuest()
	{
		if (Global.GetMapSceneUIClass() == SceneUIClasses.LuolanFazhen)
		{
			GameInstance.Game.SpriteGetFuBenBeginInfo();
			GameInstance.Game.GetLuolanFazhenBossCmd();
			GameInstance.Game.GetLuolanFazhenDoorCmd(Global.Data.roleData.MapCode);
		}
	}

	public void SetSceneTaskInfos(int index, string info, params object[] datas)
	{
		if (null != this.GameTaskBoxMini)
		{
			this.GameTaskBoxMini.SetSceneTaskInfos(index, info, datas);
		}
	}

	public void SetHuanyingTaskScene(bool isWhich)
	{
		if (this.GameTaskBoxMini != null)
		{
			this.GameTaskBoxMini.SetHuanyingScene(isWhich);
		}
	}

	public void SetSceneTaskInfos2(int index, string info, params object[] datas)
	{
		if (null != this.GameHuoDongInfoBox)
		{
			this.GameHuoDongInfoBox.SetSceneTaskInfos(index, info, datas);
		}
	}

	public void SetSceneTaskBtn(bool isVisible)
	{
		if (null != this.GameTaskBoxMini)
		{
			this.GameTaskBoxMini.SetSceneTaskBtn(isVisible, true);
		}
	}

	private IEnumerator CheckTimeOutHaveDispose()
	{
		yield return new WaitForSeconds(5f);
		if (!this.m_OnTimeOutHaveDispose)
		{
			this.OnLeaveFuBenIconClick(this, null);
		}
		this.m_OnTimeOutHaveDispose = false;
		base.StopCoroutine("CheckTimeOutHaveDispose");
		yield break;
	}

	private void OnTimeOut()
	{
		if (this._SceneState == SceneStates.Clear)
		{
			if (this._SceneUIClass != SceneUIClasses.Normal)
			{
				this.OnLeaveFuBenIconClick(this, null);
				this.TimerEnabled = false;
			}
		}
		else if (this._SceneState == SceneStates.FuBen && this.StartTicks > 0L && this.EndTicks > this.StartTicks + 90000L)
		{
			if (this._SceneUIClass == SceneUIClasses.TaskCopy || this._SceneUIClass == SceneUIClasses.NormalCopy || this._SceneUIClass == SceneUIClasses.FamilyBoss || this._SceneUIClass == SceneUIClasses.JingYanFuBen || this._SceneUIClass == SceneUIClasses.OnePiece || this._SceneUIClass == SceneUIClasses.JinBiFuBen || this._SceneUIClass == SceneUIClasses.EMoLaiXiCopy || this._SceneUIClass == SceneUIClasses.LuolanFazhen || Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass))
			{
				this._SceneState = SceneStates.Clear;
				this.OnLeaveFuBenIconClick(this, null);
				this.TimerEnabled = false;
			}
		}
		else if (this._SceneState == SceneStates.Next && this._SceneUIClass == SceneUIClasses.PaTa)
		{
			if (this.IsHavePataNextIndex())
			{
				(Super.GData.PlayZoneRoot as PlayZone).OnPataNextIndexEvent();
			}
			else
			{
				this._SceneState = SceneStates.Clear;
				this.OnLeaveFuBenIconClick(this, null);
			}
			this.TimerEnabled = false;
		}
	}

	public void RefreshFuBenMapMonsterNum(int remainPlayer, int killedNormalNum, int totalNormalNum, int killedBossNum, int totalBossNum, int nInfo = 0)
	{
		if (this._SceneUIClass == SceneUIClasses.BloodCastle || this._SceneUIClass == SceneUIClasses.Demon || this._SceneUIClass == SceneUIClasses.LuolanFazhen || this._SceneUIClass == SceneUIClasses.ShiLian || Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass))
		{
			return;
		}
		if (remainPlayer >= 0)
		{
			this.RemainPlayer = remainPlayer;
		}
		this.KilledNormalNum = killedNormalNum;
		this.TotalNormalNum = totalNormalNum;
		this.KilledBossNum = killedBossNum;
		this.TotalBossNun = totalBossNum;
		if (this._SceneUIClass == SceneUIClasses.KaLiMaTemple || this._SceneUIClass == SceneUIClasses.LoveFuBen)
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("剩余怪物: "), "00ff00"), new object[]
			{
				this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
			});
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("剩余玩家: "), "00ff00"), new object[]
			{
				this.RemainPlayer
			});
		}
		else if (this._SceneUIClass == SceneUIClasses.PaTa)
		{
			this.SetSceneTaskInfos(7, ColorCode.EncodingText(Global.GetLang("剩余怪物: "), "00ff00"), new object[]
			{
				this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
			});
		}
		else if (this._SceneUIClass == SceneUIClasses.JinBiFuBen || this._SceneUIClass == SceneUIClasses.EMoLaiXiCopy)
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("等待怪物刷新"), "c39550"), new object[0]);
		}
		else if (this._SceneUIClass != SceneUIClasses.NormalCopy)
		{
			if (this._SceneUIClass == SceneUIClasses.FamilyBoss || this._SceneUIClass == SceneUIClasses.OnePiece)
			{
				if (nInfo == 0)
				{
					return;
				}
				string chineseText = string.Empty;
				if (nInfo == 1)
				{
					chineseText = Global.GetLang("副本Boss未刷新");
				}
				else if (nInfo == 2)
				{
					chineseText = Global.GetLang("副本Boss未击杀");
				}
				else if (nInfo == 3)
				{
					chineseText = Global.GetLang("副本Boss已击杀");
				}
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang(chineseText), "c39550"), new object[0]);
			}
			else
			{
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("剩余怪物: "), "00ff00"), new object[]
				{
					this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
				});
			}
		}
	}

	public void ReUpdateFuBenStepData(SceneUIClasses sceneUIClass)
	{
		if (sceneUIClass != SceneUIClasses.NormalCopy)
		{
			return;
		}
		if (this.CacheDatas.Count <= 0)
		{
			return;
		}
		this._SceneUIClass = sceneUIClass;
		Dictionary<int, FubenBoxMini.FuBenCacheData>.Enumerator enumerator = this.CacheDatas.GetEnumerator();
		if (enumerator.MoveNext())
		{
			KeyValuePair<int, FubenBoxMini.FuBenCacheData> keyValuePair = enumerator.Current;
			FubenBoxMini.FuBenCacheData value = keyValuePair.Value;
			this.UpdateFuBenMapMonsterWithNum(value.CurrLeftNum, this.CurrFubenId, this.CurrBoShu, 0, value.MonsterId, 4);
		}
	}

	public void UpdateFuBenMapMonsterWithNum(int leftNum, int fubenId, int boShu, int killedBossNum, int killedMonsterId, int type)
	{
		if (type < 4)
		{
			return;
		}
		string text = string.Empty;
		if (type == 4)
		{
			if (!this.BeExistFuBen(fubenId))
			{
				return;
			}
			if (this.CurrFubenId != fubenId && fubenId != 0)
			{
				this.CurrFubenId = fubenId;
				this.CacheDatas.Clear();
			}
			if (this.CurrBoShu < boShu)
			{
				this.CurrBoShu = boShu;
				this.CacheDatas.Clear();
			}
			if (this.CacheDatas.Count == 0)
			{
				MUFuBenMuBiao fuBenMuBiao = FubenBoxMini.GetFuBenMuBiao(fubenId, this.CurrBoShu);
				if (fuBenMuBiao != null)
				{
					int monsterId = fuBenMuBiao.MonsterId;
					int monsterNum = fuBenMuBiao.MonsterNum;
					if (monsterId == killedMonsterId || killedMonsterId == -1)
					{
						if (!this.CacheDatas.ContainsKey(monsterId))
						{
							MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterId);
							FubenBoxMini.FuBenCacheData fuBenCacheData = new FubenBoxMini.FuBenCacheData();
							fuBenCacheData.FuBenId = this.CurrFubenId;
							if (leftNum == -1)
							{
								fuBenCacheData.CurrLeftNum = monsterNum;
							}
							else
							{
								fuBenCacheData.CurrLeftNum = leftNum;
							}
							fuBenCacheData.StepCount = this.CurrBoShu;
							fuBenCacheData.TotalNum = monsterNum;
							fuBenCacheData.MonsterName = monsterXmlNodeByID.SName;
							fuBenCacheData.MonsterId = monsterId;
							fuBenCacheData.MonsterId = monsterId;
							fuBenCacheData.TargetPoint = fuBenMuBiao.MuBiaoPoint;
							this.CacheDatas.Add(monsterId, fuBenCacheData);
						}
						else if (leftNum == -1)
						{
							this.CacheDatas[monsterId].CurrLeftNum = monsterNum;
						}
						else
						{
							this.CacheDatas[monsterId].CurrLeftNum = leftNum;
						}
					}
				}
			}
			else if (this.CacheDatas.ContainsKey(killedMonsterId))
			{
				this.CacheDatas[killedMonsterId].CurrLeftNum = leftNum;
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					Global.GetLang("副本中没有缓存该怪物信息")
				});
			}
			if (this.CacheDatas.Count > 0)
			{
				text = string.Empty;
				bool flag = true;
				foreach (KeyValuePair<int, FubenBoxMini.FuBenCacheData> keyValuePair in this.CacheDatas)
				{
					FubenBoxMini.FuBenCacheData value = keyValuePair.Value;
					flag = (value.CurrLeftNum == 0 && flag);
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
					{
						FubenBoxMini.FuBenShowColor,
						Global.GetLang("击杀")
					});
					text = text + colorStringForNGUIText + Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}{1}/{2}", value.MonsterName, value.TotalNum - value.CurrLeftNum, value.TotalNum)
					}) + "\n";
				}
				string lang = Global.GetLang("副本目标");
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(lang, FubenBoxMini.TaskA), new object[0]);
				this.SetSceneTaskInfos(1, text, new object[0]);
				if (flag)
				{
					this.UpdateFuBenMapMonsterWithNum(-1, this.FuBenID, this.CurrBoShu + 1, -1, -1, 4);
					return;
				}
				if (!Global.IsAutoFighting() && (Global.Data.AutoRoadItemsList == null || Global.Data.AutoRoadItemsList.Count <= 0))
				{
					this.ShowFubenArrowAnim(true);
				}
			}
			else if (this.FuBenID > 0)
			{
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("恭喜您，副本已通关！"), FubenBoxMini.TaskA), new object[0]);
				this.SetSceneTaskInfos(1, string.Empty, new object[0]);
				this.ShowFubenArrowAnim(false);
			}
			else
			{
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("等待怪物刷新"), "dac7ae"), new object[0]);
				this.SetSceneTaskInfos(1, string.Empty, new object[0]);
				this.ShowFubenArrowAnim(false);
			}
		}
		else
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("剩余怪物："), FubenBoxMini.INum), new object[]
			{
				this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
			});
		}
	}

	public void OnClickTxt()
	{
		if (Global.IsAutoFighting())
		{
			return;
		}
		if (this.CacheDatas.Count > 0)
		{
			foreach (KeyValuePair<int, FubenBoxMini.FuBenCacheData> keyValuePair in this.CacheDatas)
			{
				FubenBoxMini.FuBenCacheData value = keyValuePair.Value;
				if (value.CurrLeftNum > 0 && (value.TargetPoint.X != -1 || value.TargetPoint.Y != -1))
				{
					Global.Data.GameScene.AutoFindRoad(Global.Data.roleData.MapCode, value.TargetPoint, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
					break;
				}
				if (value.TargetPoint.X == -1 || value.TargetPoint.Y == -1)
				{
					PlayZone playZone = Super.GData.PlayZoneRoot as PlayZone;
					playZone.ProcessQuickFightEvent();
				}
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (this.CacheDatas != null)
		{
			this.CacheDatas.Clear();
			this.CurrFubenId = 0;
			this.CurrBoShu = 0;
		}
		this.ShowFubenArrowAnim(false);
	}

	public void RefreshSceneScoreInfo(bool showScore = true)
	{
		if (this._SceneUIClass == SceneUIClasses.Battle)
		{
			this.SetSceneTaskInfos(4, ColorCode.EncodingText(Global.GetLang("教团 "), "fd010c"), new object[]
			{
				this._SuiEnemiesNum
			});
			this.SetSceneTaskInfos(5, ColorCode.EncodingText(Global.GetLang(" 盟军"), "4997bc"), new object[]
			{
				this._TangEnemiesNum
			});
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("本场最高: "), "00ff00"), new object[]
			{
				this._TopScore
			});
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("个人得分: "), "00ff00"), new object[]
			{
				this._KillEnemiesNum
			});
		}
		else if (this._SceneUIClass == SceneUIClasses.PKKing)
		{
			this.SetSceneTaskInfos2(0, ColorCode.EncodingText(Global.GetLang("个人积分: "), "00ff00"), new object[]
			{
				this._SelfScore
			});
			this.SetSceneTaskInfos2(1, ColorCode.EncodingText(Global.GetLang("剩余玩家: "), "00ff00"), new object[]
			{
				this._RemainPlayer
			});
		}
		else if (this._SceneUIClass == SceneUIClasses.KaLiMaTemple || this._SceneUIClass == SceneUIClasses.LoveFuBen)
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("剩余怪物: "), "00ff00"), new object[]
			{
				this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
			});
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("剩余玩家: "), "00ff00"), new object[]
			{
				this.TotalBossNun - this.KilledBossNum
			});
		}
	}

	public void NotifySceneTimeInfo(SceneStates sceneState, double startTicks, double endTicks)
	{
		bool flag = this.ChangeSceneState(sceneState);
		if (flag)
		{
			if (startTicks > 0.0)
			{
				this.StartTicks = (long)startTicks;
			}
			if (endTicks > 0.0)
			{
				this.EndTicks = (long)endTicks;
			}
		}
	}

	public void NotifyPKKingTime(int type, int leftSecs)
	{
		this.PKKingTimeType = type;
		this.PKKingLeftSecs = leftSecs;
	}

	public void NotifySceneMsg(int type, int leftSecs)
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (mapSceneUIClass != SceneUIClasses.PKKing && mapSceneUIClass != SceneUIClasses.Battle && mapSceneUIClass != SceneUIClasses.Demon && mapSceneUIClass != SceneUIClasses.ShiLian)
		{
			return;
		}
		bool flag = false;
		switch (type)
		{
		case 2:
			flag = this.ChangeSceneState(SceneStates.Enter);
			break;
		case 3:
			flag = this.ChangeSceneState(SceneStates.Task);
			break;
		case 4:
			flag = this.ChangeSceneState(SceneStates.Clear);
			break;
		case 5:
			flag = this.ChangeSceneState(SceneStates.Clear);
			break;
		}
		if (flag)
		{
			this.StartTicks = Global.GetCorrectLocalTime();
			this.EndTicks = this.StartTicks + (long)(leftSecs * 1000);
		}
		if (this.CurrentMapCode != Global.Data.roleData.MapCode)
		{
			this.EnterMapScene();
		}
		if (Global.IsBattleMap() || this._SceneUIClass == SceneUIClasses.PKKing)
		{
			this.BattleSceneState(type);
		}
	}

	private void BattleSceneState(int type)
	{
		if (type != 1)
		{
			if (type == 2)
			{
				this.ChangeSceneState(SceneStates.Enter);
			}
			else if (type == 3)
			{
				if (Global.IsInArenaBattleMap())
				{
					Global.Data.IsArenaBattling = true;
				}
				else if (Global.IsBattleMap())
				{
					Global.Data.IsYanHuangBattling = true;
				}
				this.ChangeSceneState(SceneStates.Task);
			}
			else if (type == 4 || type == 5)
			{
				Global.Data.IsArenaBattling = false;
				Global.Data.IsYanHuangBattling = false;
				this.ChangeSceneState(SceneStates.Clear);
			}
		}
		this.RefreshSceneScoreInfo(true);
	}

	public void RefreshFuBenMapBeginInfo(int roleID, int fuBenID, double startTicks, double endTicks, int killedNormalNum, int totalNormalNum, int killedBossNum, int totalBossNum, int nStroyCopyMapInfo = 0)
	{
		if (this._SceneUIClass == SceneUIClasses.BloodCastle || this._SceneUIClass == SceneUIClasses.Demon)
		{
			return;
		}
		this.FuBenID = fuBenID;
		this.StartTicks = (long)startTicks;
		this.EndTicks = (long)endTicks;
		this.KilledNormalNum = killedNormalNum;
		this.TotalNormalNum = totalNormalNum;
		this.KilledBossNum = killedBossNum;
		this.TotalBossNun = totalBossNum;
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				startTicks,
				":::",
				endTicks,
				":::: "
			})
		});
		if (this._SceneUIClass != SceneUIClasses.JingYanFuBen)
		{
			if (this._SceneUIClass == SceneUIClasses.PaTa)
			{
				this.SetSceneTaskInfos(6, ConfigSettings.GetMapNameByCode(fuBenID, false), new object[0]);
				this.SetSceneTaskInfos(7, ColorCode.EncodingText(Global.GetLang("剩余怪物: "), "00ff00"), new object[]
				{
					this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
				});
			}
			else if (this._SceneUIClass != SceneUIClasses.JinBiFuBen)
			{
				if (this._SceneUIClass != SceneUIClasses.NormalCopy && this._SceneUIClass != SceneUIClasses.FamilyBoss && this._SceneUIClass != SceneUIClasses.LuolanFazhen && this._SceneUIClass != SceneUIClasses.EMoLaiXiCopy && !Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass))
				{
					this.SetSceneTaskInfos(0, Global.GetLang("剩余怪物:"), new object[]
					{
						this.TotalNormalNum + this.TotalBossNun - this.KilledNormalNum - this.KilledBossNum
					});
				}
			}
		}
		long num = (long)(Global.GetCurrentFuBenMapLimitTimeMinutes() * 60);
		if (num <= 0L)
		{
			this.FubenEndTime = StringUtil.substitute(Global.GetLang("时间不限"), new object[0]);
		}
		MUDebug.Log<string>(new string[]
		{
			num + ":::: "
		});
		if (this.EndTicks >= this.StartTicks)
		{
			if (this._SceneUIClass == SceneUIClasses.TaskCopy)
			{
				this.GameTaskBoxMini.ShowHelpAnim(1, 1);
			}
			else if (this._SceneUIClass != SceneUIClasses.PaTa)
			{
				this.ShowAnim(0, 1);
			}
			else if (this._SceneUIClass == SceneUIClasses.PaTa && !this.IsHavePataNextIndex())
			{
				this.ShowAnim(0, 1);
			}
			this.ChangeSceneState(SceneStates.Clear);
			this.StartTicks = this.EndTicks;
			this.EndTicks = this.StartTicks + 60000L;
		}
		else
		{
			this.EndTicks = this.StartTicks + num * 1000L;
		}
		if (this.TotalNormalNum - this.KilledNormalNum <= 0 && this.TotalBossNun - this.KilledBossNum <= 0)
		{
			this.SetFubenBtn(fuBenID, 1);
		}
		else
		{
			this.SetFubenBtn(fuBenID, 0);
		}
	}

	public void SetFubenBtn(int fuBenID = -1, int flag = 0)
	{
		if (fuBenID != -1)
		{
			this.FuBenID = fuBenID;
		}
	}

	public void RefreshLeftSecs(int mapCode = -1)
	{
		long mapLimitTime = Global.GetMapLimitTime(mapCode);
		bool flag = this.ChangeSceneState(SceneStates.Remain);
		if (flag)
		{
			this.StartTicks = Global.GetCorrectLocalTime();
			this.EndTicks = this.StartTicks + mapLimitTime * 1000L;
		}
	}

	public void RefreshLuolanFazhenInfo(int boss1ID, int boss1Num, int boss2ID, int boss2Num)
	{
		if (this._SceneUIClass == SceneUIClasses.LuolanFazhen)
		{
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(boss1ID);
			int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
			string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
			string sname = monsterXmlNodeByID.SName;
			MonsterVO monsterXmlNodeByID2 = ConfigMonsters.GetMonsterXmlNodeByID(boss2ID);
			int npcorMonsterMapCodeByID2 = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID2.MapCode);
			string mapNameByCode2 = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID2, false);
			string sname2 = monsterXmlNodeByID2.SName;
			string text = (1 - boss1Num != 0) ? "00ff00" : "c39550";
			string text2 = (1 - boss2Num != 0) ? "00ff00" : "c39550";
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("通关BOSS ") + sname + "  ", text), new object[]
			{
				1 - boss1Num,
				1
			});
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("隐藏BOSS ") + sname2 + "  ", text2), new object[]
			{
				1 - boss2Num,
				1
			});
			this.SetSceneTaskInfos(2, (1 - boss2Num != 0) ? ColorCode.EncodingText(Global.GetLang("通关奖励翻倍(已激活)"), "00ff00") : ColorCode.EncodingText(Global.GetLang("通关奖励翻倍(未激活)"), "c39550"), new object[0]);
			this.GameTaskBoxMini.SetLuolanFazhenInfosColor(text, text2);
		}
	}

	public void DaTaoShaInfo(GameSceneStateTimeData result)
	{
		this.Anim.SetActive(false);
		GameTypes gameType = (GameTypes)result.GameType;
		int state = result.State;
		long endTicks = result.EndTicks;
		if (gameType == GameTypes.Escape)
		{
			if (endTicks > 0L)
			{
				this.EndTicks = endTicks;
			}
			switch (state)
			{
			case 1:
				this.TimeString = Global.GetLang("战场入场倒计时 {0}");
				break;
			case 2:
				this.TimeString = Global.GetLang("安全阶段倒计时 {0}");
				break;
			case 3:
				this.TimeString = Global.GetLang("杀戮阶段倒计时 {0}");
				break;
			case 4:
				this.TimeString = Global.GetLang("狂热阶段倒计时 {0}");
				break;
			case 5:
				this.TimeString = Global.GetLang("战斗结束倒计时 {0}");
				break;
			}
		}
	}

	public void InitHuanYingInfo(GameSceneStateTimeData result)
	{
		GameTypes gameType = (GameTypes)result.GameType;
		int state = result.State;
		long endTicks = result.EndTicks;
		bool flag = false;
		if (Global.IsKuaFuHuoDongMapSceneUIClass(this._SceneUIClass))
		{
			GameTypes gameTypes = gameType;
			switch (gameTypes)
			{
			case GameTypes.HuanYingSiYuan:
			case GameTypes.TianTiJingSai:
			case GameTypes.YongZheZhanChang:
			case GameTypes.KuafuBoss:
			case GameTypes.LangHunLingYu:
			case GameTypes.CoupleArena:
			case GameTypes.KingOfBattle:
			case GameTypes.zhengduozhidi:
			case GameTypes.ZhanMengLianSaiBiSai:
				break;
			default:
				switch (gameTypes)
				{
				case GameTypes.KuaFuTeamCompete:
				case GameTypes.Zork5v5:
					break;
				case (GameTypes)35:
					goto IL_9D;
				default:
					goto IL_9D;
				}
				break;
			}
			flag = true;
			IL_9D:
			if (endTicks > 0L)
			{
				this.EndTicks = endTicks;
			}
			switch (state)
			{
			case 1:
				this.ChangeSceneState(SceneStates.Enter);
				break;
			case 2:
				if (flag)
				{
					this.ChangeSceneState(SceneStates.Task);
				}
				else
				{
					this.ChangeSceneState(SceneStates.FuBen);
				}
				break;
			case 3:
				this.ChangeSceneState(SceneStates.Clear);
				break;
			case 5:
				this.ChangeSceneState(SceneStates.Clear);
				break;
			}
		}
	}

	public void InitHuanYingScoreInfo(HuanYingSiYuanScoreInfoData result)
	{
		if (Global.Data.roleData.BattleWhichSide == 2)
		{
			this.SetHuanyingTaskScene(true);
		}
		else
		{
			this.SetHuanyingTaskScene(false);
		}
		this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang(Global.GetLang("教团       {0}         {1} ")), result.Count1, result.Score1), "ff0000"), new object[0]);
		this.SetSceneTaskInfos(2, ColorCode.EncodingText(string.Format(Global.GetLang(Global.GetLang("盟军       {0}         {1} ")), result.Count2, result.Score2), "4997bc"), new object[0]);
	}

	public void InitJunTuanCaiJiInfo(long endTicks)
	{
		this.TimeString = Global.GetLang(string.Empty);
		this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), string.Empty);
		base.CancelInvoke("JunTuanCaiJiCountDown");
		long num = endTicks / 10000L;
		if (Global.GetCorrectLocalTime() < num)
		{
			this.m_EndTicks = num;
			base.InvokeRepeating("JunTuanCaiJiCountDown", 0f, 1f);
		}
	}

	private void JunTuanCaiJiCountDown()
	{
		this.m_JunTuanTicks = Global.GetCorrectLocalTime();
		this.TimeString = Global.GetLang("双倍时间 {0}");
		if (this.m_JunTuanTicks < this.m_EndTicks)
		{
			long num = (this.m_EndTicks - this.m_JunTuanTicks) / 1000L;
			string text;
			if (num < 3600L)
			{
				text = UIHelper.FormatSecsMS(num, "00:00");
			}
			else
			{
				text = UIHelper.FormatSecs2(num, "01:00:00");
			}
			this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), text);
		}
		else
		{
			base.CancelInvoke("JunTuanCaiJiCountDown");
			this.TimeString = Global.GetLang(string.Empty);
			this.FubenEndTime = string.Format(Global.GetLang(this.TimeString), string.Empty);
		}
	}

	public void ShowMessageText()
	{
		string lang = Global.GetLang("此功能需退出当前活动，确定要退出？");
		string[] buttons = new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		};
		Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s2, DPSelectedItemEventArgs e2)
		{
			if (e2.ID == 0)
			{
				this.OnLeaveFuBenIconClick(this, null);
			}
		}, buttons);
	}

	public void ShowSuccessScore(HuanYingSiYuanAddScore result)
	{
		if (result.Side == 2)
		{
			Super.HintMainText(string.Format(Global.GetLang("【{0}区{1}】成功夺取圣杯，{2}获得{3}积分！"), new object[]
			{
				result.ZoneID,
				result.Name,
				Global.GetLang("盟军"),
				result.Score
			}), 10, 3);
		}
		else
		{
			Super.HintMainText(string.Format(Global.GetLang("【{0}区{1}】成功夺取圣杯，{2}获得{3}积分！"), new object[]
			{
				result.ZoneID,
				result.Name,
				Global.GetLang("教团"),
				result.Score
			}), 10, 3);
		}
	}

	public void ShowFubenArrowAnim(bool show)
	{
		if (this.GameTaskBoxMini != null)
		{
			if (show && this.CacheDatas.Count > 0)
			{
				foreach (KeyValuePair<int, FubenBoxMini.FuBenCacheData> keyValuePair in this.CacheDatas)
				{
					FubenBoxMini.FuBenCacheData value = keyValuePair.Value;
					if (value.CurrLeftNum > 0 && (value.TargetPoint.X != -1 || value.TargetPoint.Y != -1))
					{
						this.GameTaskBoxMini.SetFubenArrow(true);
						break;
					}
				}
			}
			else
			{
				this.GameTaskBoxMini.SetFubenArrow(false);
			}
		}
	}

	public static MUAllFuBenMuBiao GetAllFuBenMuBiao()
	{
		if (FubenBoxMini.m_allMuBiao == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBenMuBiao.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/FuBenMuBiao.xml 出现错误"
				});
				return null;
			}
			FubenBoxMini.m_allMuBiao = new MUAllFuBenMuBiao(gameResXml);
		}
		return FubenBoxMini.m_allMuBiao;
	}

	public static MUFuBenMuBiao GetFuBenMuBiao(int fubenId, int muBiaoId)
	{
		return FubenBoxMini.GetAllFuBenMuBiao().GetFuBenMuBiao(fubenId, muBiaoId);
	}

	public bool BeExistFuBen(int fubenId)
	{
		return FubenBoxMini.GetAllFuBenMuBiao().BeExistFuBen(fubenId);
	}

	public FubenBoxMini _Instance;

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL Body;

	public TextBlock TxtFubenEndTime;

	public GButton LeaveFubenIcon;

	public GameObject Anim;

	public int FuBenID;

	public int KilledNormalNum;

	public int TotalNormalNum;

	public int KilledBossNum;

	public int TotalBossNun;

	public long _StartTicks;

	public long _EndTicks;

	private int _KillEnemiesNum;

	private int _TopScore;

	private int _SuiEnemiesNum;

	private int _TangEnemiesNum;

	private int _RemainPlayer;

	private int _SelfScore;

	public TaskBoxMini GameTaskBoxMini;

	public FuBenTongGuanPart _FuBenTongGuanPart;

	public HuoDongInfoBox GameHuoDongInfoBox;

	public bool TimerEnabled;

	private DispatcherTimer UITimer;

	private string TimeString = string.Empty;

	private SceneStates _SceneState = SceneStates.None;

	private SceneUIClasses _SceneUIClass;

	private int CurrentMapCode = -1;

	private bool m_OnTimeOutHaveDispose;

	private int CurrFubenId;

	private int CurrBoShu;

	private static string FuBenShowColor = "ecf7f9";

	public static string INum = "68c773";

	public static string TaskA = "facb0d";

	private Dictionary<int, FubenBoxMini.FuBenCacheData> CacheDatas = new Dictionary<int, FubenBoxMini.FuBenCacheData>();

	private int PKKingTimeType;

	private int PKKingLeftSecs;

	private long m_JunTuanTicks;

	private long m_EndTicks;

	private static MUAllFuBenMuBiao m_allMuBiao;

	private class FuBenCacheData
	{
		public string MonsterName;

		public int MonsterId;

		public int TotalNum;

		public int CurrLeftNum;

		public int FuBenId;

		public int StepCount;

		public Point TargetPoint = new Point(1, 1);
	}
}
