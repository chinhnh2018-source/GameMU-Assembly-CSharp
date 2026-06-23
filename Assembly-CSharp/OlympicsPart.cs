using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class OlympicsPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnMatch.Text = Global.GetLang(OlympicsDataManage.GetTitleName(0));
		this.btnGuess.Text = Global.GetLang(OlympicsDataManage.GetTitleName(1));
		this.btnRank.Text = Global.GetLang(OlympicsDataManage.GetTitleName(2));
		this.btnShop.Text = Global.GetLang(OlympicsDataManage.GetTitleName(3));
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitButton();
		GameInstance.Game.SendOlympicsScoreRequest();
		GameInstance.Game.SendOlympicsQueryAwardRequest(1);
		GameInstance.Game.SendOlympicsQueryAwardRequest(2);
		this.RegActivityTip();
	}

	private void InitButton()
	{
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.btnMatch.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_TabIndex == OlympicsTabTypes.Match)
			{
				return;
			}
			this.ShowTabByID(OlympicsTabTypes.Match);
		};
		this.btnGuess.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_TabIndex == OlympicsTabTypes.Guess)
			{
				return;
			}
			this.ShowTabByID(OlympicsTabTypes.Guess);
		};
		this.btnRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_TabIndex == OlympicsTabTypes.Rank)
			{
				return;
			}
			this.ShowTabByID(OlympicsTabTypes.Rank);
		};
		this.btnShop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_TabIndex == OlympicsTabTypes.Shop)
			{
				return;
			}
			this.ShowTabByID(OlympicsTabTypes.Shop);
		};
		this.btnMatch.Label.color = NGUIMath.HexToColorEx(12434877U);
		this.btnGuess.Label.color = NGUIMath.HexToColorEx(12434877U);
		this.btnRank.Label.color = NGUIMath.HexToColorEx(12434877U);
		this.btnShop.Label.color = NGUIMath.HexToColorEx(12434877U);
		if (Global.IsInOlympicsAwardActivity())
		{
			this.ShowTabByID(OlympicsTabTypes.Rank);
		}
		else
		{
			this.ShowTabByID(OlympicsTabTypes.Match);
		}
	}

	private void RegActivityTip()
	{
		ActivityTipManager.RegActivityTipItem(20001, delegate(int s1, ActivityTipItem e1)
		{
			this.matchTip.SetActive(e1.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(20002, delegate(int s2, ActivityTipItem e2)
		{
			this.guessTip.SetActive(e2.IsActive);
		});
	}

	public void ShowTabByID(OlympicsTabTypes tabType)
	{
		this.m_TabIndex = tabType;
		switch (tabType)
		{
		case OlympicsTabTypes.Match:
			this.SetBtnStat(this.btnMatch);
			this.OpenMatchTab();
			break;
		case OlympicsTabTypes.Guess:
			this.SetBtnStat(this.btnGuess);
			this.OpenGuessTab();
			break;
		case OlympicsTabTypes.Rank:
			this.SetBtnStat(this.btnRank);
			this.OpenRankTab();
			break;
		case OlympicsTabTypes.Shop:
			this.SetBtnStat(this.btnShop);
			this.OpenShopTab();
			break;
		}
	}

	private void SetBtnStat(GButton btn)
	{
		if (Global.IsInOlympicsAwardActivity() && this.m_TabIndex != OlympicsTabTypes.Rank)
		{
			btn.normalSprite = "Tab_normal";
			btn.hoverSprite = "Tab_normal";
			btn.pressedSprite = "Tab_normal";
			btn.Label.color = NGUIMath.HexToColorEx(12434877U);
			return;
		}
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(16434701U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16434701U);
			this.tempBtn.Pressed = false;
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(12434877U);
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16434701U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	public void OpenMatchTab()
	{
		if (Global.IsInOlympicsAwardActivity())
		{
			Super.HintMainText(Global.GetLang("活动已结束"), 10, 3);
			return;
		}
		if (null != this.matchPart)
		{
			this.matchPart.gameObject.SetActive(true);
		}
		else
		{
			this.matchPart = U3DUtils.NEW<OlympicsMatchPart>();
			U3DUtils.AddChild(this.panel, this.matchPart.gameObject, true);
			GameInstance.Game.SendOlympicsMatchTimesRequest();
		}
		this.SetGuessPanel(false);
		this.SetRankPanel(false);
		this.SetShopPanel(false);
	}

	public void OpenGuessTab()
	{
		if (Global.IsInOlympicsAwardActivity())
		{
			Super.HintMainText(Global.GetLang("竞猜已结束"), 10, 3);
			return;
		}
		GameInstance.Game.SendOlympicsGuessResultRequest(1);
	}

	public void OpenRankTab()
	{
		if (null != this.rankPart)
		{
			this.rankPart.gameObject.SetActive(true);
			this.rankPart.RefreshTotalScore();
		}
		else
		{
			this.rankPart = U3DUtils.NEW<OlympicsRankPart>();
			U3DUtils.AddChild(this.panel, this.rankPart.gameObject, true);
			GameInstance.Game.SendOlympicsRankResultRequest();
		}
		this.SetMatchPanel(false);
		this.SetGuessPanel(false);
		this.SetShopPanel(false);
	}

	public void OpenShopTab()
	{
		if (Global.IsInOlympicsAwardActivity())
		{
			Super.HintMainText(Global.GetLang("活动已结束"), 10, 3);
			return;
		}
		if (null != this.shopPart)
		{
			GameInstance.Game.SendOlympicsShopContentRequest();
		}
		else
		{
			this.shopPart = U3DUtils.NEW<OlympicsShopPart>();
			U3DUtils.AddChild(this.panel, this.shopPart.gameObject, true);
			GameInstance.Game.SendOlympicsShopContentRequest();
		}
		this.SetMatchPanel(false);
		this.SetGuessPanel(false);
		this.SetRankPanel(false);
	}

	public void SetScoreLabel()
	{
		this.scoreLabel.Text = OlympicsDataManage.ownedScore.ToString();
	}

	private void SetMatchPanel(bool isSow)
	{
		if (null != this.matchPart)
		{
			this.matchPart.gameObject.SetActive(isSow);
		}
	}

	private void SetGuessPanel(bool isSow)
	{
		if (null != this.guessPart)
		{
			this.guessPart.gameObject.SetActive(isSow);
		}
	}

	private void SetRankPanel(bool isSow)
	{
		if (null != this.rankPart)
		{
			this.rankPart.gameObject.SetActive(isSow);
		}
	}

	private void SetShopPanel(bool isSow)
	{
		if (null != this.shopPart)
		{
			this.shopPart.gameObject.SetActive(isSow);
		}
	}

	public void NotifyScoreResult(int tmpTotalScore, int tmpOwnedScore)
	{
		OlympicsDataManage.ownedScore = tmpOwnedScore;
		OlympicsDataManage.totalScore = tmpTotalScore;
		this.SetScoreLabel();
	}

	public void NotifyMatchResult(int tmpShootTimes, int tmpPlayBallTimes)
	{
		if (null != this.matchPart)
		{
			this.matchPart.RefreshData(tmpShootTimes, tmpPlayBallTimes);
		}
	}

	public void NotifySingleMatchResult(int operateType, int gameType, int currentScore)
	{
		if (OlympicsDataManage.MessageType(operateType))
		{
			if (gameType == 1)
			{
				this.matchPart.olympicsMatchShootPart.NotifyScoreLabel(currentScore);
			}
			else if (gameType == 2)
			{
				this.matchPart.olympicsMatchBallPart.NotifyScoreLabel(currentScore);
			}
		}
	}

	public void NotifyBeginMatch(int operateType, int gameType, int matchTimes, int timesInGame, int score)
	{
		if (OlympicsDataManage.MessageType(operateType))
		{
			if (gameType == 1)
			{
				if (timesInGame > 0)
				{
					if (null == this.olympicsMatchTipsWind)
					{
						this.olympicsMatchTipsWind = U3DUtils.NEW<GChildWindow>();
						this.olympicsMatchTipsWind.ModalType = ChildWindowModalType.Translucent;
						this.olympicsMatchTipsWind.IsShowModal = true;
						Super.InitChildWindow(this.olympicsMatchTipsWind, "OlympicsMatchTipsWind");
						Super.GData.GlobalPlayZone.Children.Add(this.olympicsMatchTipsWind);
					}
					this.olympicsMatchTips = U3DUtils.NEW<OlympicsMatchTips>();
					this.olympicsMatchTipsWind.Body.Add(this.olympicsMatchTips);
					this.olympicsMatchTips.Hander = delegate(object sender, DPSelectedItemEventArgs args)
					{
						if (args.ID == 0)
						{
							Object.Destroy(this.olympicsMatchTips.gameObject);
							this.olympicsMatchTips = null;
							Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchTipsWind);
						}
						else if (args.ID == 1)
						{
							Object.Destroy(this.olympicsMatchTips.gameObject);
							this.olympicsMatchTips = null;
							Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchTipsWind);
							this.matchPart.OpenShootGame(matchTimes, timesInGame, score);
						}
					};
				}
				else
				{
					this.matchPart.OpenShootGame(matchTimes, timesInGame, score);
				}
			}
			else if (gameType == 2)
			{
				if (timesInGame > 0)
				{
					if (null == this.olympicsMatchTipsWind)
					{
						this.olympicsMatchTipsWind = U3DUtils.NEW<GChildWindow>();
						this.olympicsMatchTipsWind.ModalType = ChildWindowModalType.Translucent;
						this.olympicsMatchTipsWind.IsShowModal = true;
						Super.InitChildWindow(this.olympicsMatchTipsWind, "OlympicsMatchTipsWind");
						Super.GData.GlobalPlayZone.Children.Add(this.olympicsMatchTipsWind);
					}
					this.olympicsMatchTips = U3DUtils.NEW<OlympicsMatchTips>();
					this.olympicsMatchTipsWind.Body.Add(this.olympicsMatchTips);
					this.olympicsMatchTips.Hander = delegate(object sender, DPSelectedItemEventArgs args)
					{
						if (args.ID == 0)
						{
							Object.Destroy(this.olympicsMatchTips.gameObject);
							this.olympicsMatchTips = null;
							Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchTipsWind);
						}
						else if (args.ID == 1)
						{
							Object.Destroy(this.olympicsMatchTips.gameObject);
							this.olympicsMatchTips = null;
							Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsMatchTipsWind);
							this.matchPart.OpenPlayBallGame(matchTimes, timesInGame, score);
						}
					};
				}
				else
				{
					this.matchPart.OpenPlayBallGame(matchTimes, timesInGame, score);
				}
			}
		}
	}

	public void NotifyFinallyMatchResult(int gameType, int isWin, int grade, int score)
	{
		if (gameType == 1)
		{
			this.matchPart.olympicsMatchShootPart.NotifyFinishGame(isWin, grade, score);
		}
		else if (gameType == 2)
		{
			this.matchPart.olympicsMatchBallPart.NotifyFinishGame(isWin, grade, score);
		}
	}

	public void NotifyGuessResult(OlympicsGuessDataResult tmpDataList)
	{
		if (tmpDataList == null || tmpDataList.List.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无竞猜数据"), 10, 3);
			return;
		}
		if (null != this.guessPart)
		{
			this.guessPart.gameObject.SetActive(true);
		}
		else
		{
			this.guessPart = U3DUtils.NEW<OlympicsGuessPart>();
			U3DUtils.AddChild(this.panel, this.guessPart.gameObject, true);
			GameInstance.Game.SendOlympicsGuessResultRequest(1);
		}
		this.SetMatchPanel(false);
		this.SetRankPanel(false);
		this.SetShopPanel(false);
		int type = tmpDataList.Type;
		if (tmpDataList.Type == 1)
		{
			OlympicsDataManage.SetGuessData(tmpDataList.List, tmpDataList.DayID);
		}
		else if (tmpDataList.Type == 2)
		{
			OlympicsDataManage.SetYesterdayGuessData(tmpDataList.List);
		}
		if (null != this.guessPart)
		{
			this.guessPart.RefreshData(type);
		}
	}

	public void NotifyGuessResultStatus(int result)
	{
		if (null != this.guessPart)
		{
			this.guessPart.ShowSubmitResult(result);
		}
	}

	public void NotifyRankResult(List<KFRankData> tmpDataList)
	{
		OlympicsDataManage.SetRankData(tmpDataList);
		if (null != this.rankPart)
		{
			this.rankPart.RefreshData();
		}
	}

	public void NotifyShopResult(List<OlympicsShopData> tmpDataList)
	{
		if (null != this.shopPart && !this.shopPart.gameObject.activeSelf)
		{
			this.shopPart.gameObject.SetActive(true);
		}
		if (tmpDataList == null || tmpDataList.Count <= 0)
		{
			return;
		}
		OlympicsDataManage.SetShopData(tmpDataList);
		this.shopPart.RefreshData();
	}

	public void NotifyShopBuyGoodsResult(int isSuccess, int id, int ownedBuyCount, int totalBuyCount)
	{
		if (null != this.shopPart && OlympicsDataManage.MessageType(isSuccess))
		{
			Super.HintMainText(Global.GetLang("兑换成功"), 10, 3);
			Super.ShowNetWaiting(null);
			this.shopPart.RefreshData(id, ownedBuyCount, totalBuyCount);
		}
	}

	public void NotifyQueryAwardResult(int type, int status)
	{
		if (type == 1 && status == 0)
		{
			GameInstance.Game.SendOlympicsGetAwardRequest(type);
		}
		if (type == 2)
		{
			if (status == 0)
			{
				GameInstance.Game.SendOlympicsGetAwardRequest(type);
			}
			else if (status == 1)
			{
				this.ShowTabByID(OlympicsTabTypes.Rank);
			}
			else if (status == -4 && Global.IsInOlympicsAwardActivity())
			{
				this.ShowTabByID(OlympicsTabTypes.Rank);
			}
		}
	}

	public void NotifyGetAwardResult(int type, int status, int id, int rankId)
	{
		if (type != 1 || status == 1)
		{
		}
		if (type == 2 && status == 1)
		{
			if (null == this.olympicsAwardWindow)
			{
				this.olympicsAwardWindow = U3DUtils.NEW<GChildWindow>();
				this.olympicsAwardWindow.ModalType = ChildWindowModalType.Translucent;
				this.olympicsAwardWindow.IsShowModal = true;
				Super.InitChildWindow(this.olympicsAwardWindow, "OlympicsAwardWindows");
				Super.GData.GlobalPlayZone.Children.Add(this.olympicsAwardWindow);
			}
			this.olympicsAwardTip = U3DUtils.NEW<OlympicsAwardWindow>();
			this.olympicsAwardTip.transform.localPosition = Vector3.zero;
			this.olympicsAwardWindow.Body.Add(this.olympicsAwardTip);
			this.olympicsAwardTip.InitData(id, rankId);
			this.olympicsAwardTip.Hander = delegate(object sender, DPSelectedItemEventArgs args)
			{
				if (args.ID == 0 && null != this.olympicsAwardTip)
				{
					Object.Destroy(this.olympicsAwardTip.gameObject);
					this.olympicsAwardTip = null;
					Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsAwardWindow);
				}
			};
			this.ShowTabByID(OlympicsTabTypes.Rank);
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(20001, null);
		ActivityTipManager.RegActivityTipItem(20002, null);
	}

	public override void Destroy()
	{
		base.Destroy();
		this.btnClose = null;
		this.btnMatch = null;
		this.btnGuess = null;
		this.btnRank = null;
		this.btnShop = null;
		this.tempBtn = null;
		this.panel = null;
		this.DPSelectedItem = null;
		this.scoreLabel = null;
		this.matchPart = null;
		this.guessPart = null;
		this.rankPart = null;
		this.shopPart = null;
		this.matchTip = null;
		this.guessTip = null;
		this.olympicsAwardTip = null;
		this.olympicsAwardWindow = null;
	}

	public GButton btnClose;

	public GButton btnMatch;

	public GButton btnGuess;

	public GButton btnRank;

	public GButton btnShop;

	private GButton tempBtn;

	private OlympicsTabTypes m_TabIndex;

	public GameObject panel;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock scoreLabel;

	[HideInInspector]
	public OlympicsMatchPart matchPart;

	[HideInInspector]
	public OlympicsGuessPart guessPart;

	[HideInInspector]
	public OlympicsRankPart rankPart;

	[HideInInspector]
	public OlympicsShopPart shopPart;

	public GameObject matchTip;

	public GameObject guessTip;

	private OlympicsAwardWindow olympicsAwardTip;

	private GChildWindow olympicsAwardWindow;

	private OlympicsMatchTips olympicsMatchTips;

	private GChildWindow olympicsMatchTipsWind;
}
