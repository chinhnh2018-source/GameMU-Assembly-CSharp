using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiMainPart : UserControl
{
	private ConfigZhanMengLianSaiLeagueNewAward ConfigZhanMengLianSaiLeagueNewAward
	{
		get
		{
			if (this.mConfigZhanMengLianSaiLeagueNewAward == null)
			{
				this.mConfigZhanMengLianSaiLeagueNewAward = new ConfigZhanMengLianSaiLeagueNewAward();
			}
			return this.mConfigZhanMengLianSaiLeagueNewAward;
		}
	}

	private ConfigZhanMengLianSaiLeagueSuperAward ConfigZhanMengLianSaiLeagueSuperAward
	{
		get
		{
			if (this.mConfigZhanMengLianSaiLeagueSuperAward == null)
			{
				this.mConfigZhanMengLianSaiLeagueSuperAward = new ConfigZhanMengLianSaiLeagueSuperAward();
			}
			return this.mConfigZhanMengLianSaiLeagueSuperAward;
		}
	}

	private ConfigZhanMengLianSaiLeagueOpen ConfigZhanMengLianSaiLeagueOpen
	{
		get
		{
			if (this.mConfigZhanMengLianSaiLeagueOpen == null)
			{
				this.mConfigZhanMengLianSaiLeagueOpen = new ConfigZhanMengLianSaiLeagueOpen();
			}
			return this.mConfigZhanMengLianSaiLeagueOpen;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	public override void Destroy()
	{
		base.Destroy();
		this.StopTimeTicks();
	}

	protected override void OnDestroy()
	{
		this.StopTimeTicks();
		base.OnDestroy();
	}

	private bool IsBangHuiLeader()
	{
		return Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction);
	}

	private void InitPrefabText()
	{
		try
		{
			this.mBtnAwardPreview_.Label.text = Global.GetLang("赛季奖励");
			this.mBtnPreviewRule.Label.text = Global.GetLang("赛事规则");
			if (this.IsBangHuiLeader())
			{
				this.mBtnJion.Label.text = Global.GetLang("立即报名");
			}
			else
			{
				this.mBtnJion.Label.text = Global.GetLang("尚未报名");
			}
			this.mBtnRank.Label.text = Global.GetLang("赛季排名");
			this.mOpenTimeLabel.text = string.Empty;
			this.mSignNameLabel.text = string.Empty;
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
		try
		{
			this.mIamgeBGTexture.URL = "NetImages/GameRes/Images/Plate/SuperLianSaiBG.jpg";
		}
		catch (Exception ex)
		{
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mBtnAwardPreview_.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mOldBangHuiMatchType == 1)
				{
					if (this.ConfigZhanMengLianSaiLeagueSuperAward.GetLianSaiAwardByID(this.mRank) != null)
					{
						this.ShowLianSaiGeiAwardPart();
					}
				}
				else if (this.mOldBangHuiMatchType == 2)
				{
					if (this.ConfigZhanMengLianSaiLeagueNewAward.GetLianSaiAwardByID(this.mRank) != null)
					{
						this.ShowLianSaiGeiAwardPart();
					}
				}
				else
				{
					this.ShowShaiJiAwardPart();
				}
			};
			this.mBtnPreviewRule.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.ShowPreviewRulePart();
			};
			this.mBtnJion.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mBangHuiMatchGameStates == 3)
				{
					GameInstance.Game.SendZhanMengLianSaiCompetitionEnter(0);
				}
				else if (this.mBangHuiMatchGameStates == 1)
				{
					if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
					{
						Super.ShowNetWaiting(null);
						GameInstance.Game.SendZhanMengLianSaiJionMes();
					}
					else
					{
						Super.HintMainText(Global.GetLang("只有战盟盟主才有权力报名"), 10, 3);
					}
				}
				else if (this.mBangHuiMatchGameStates == 4)
				{
					GameInstance.Game.SendZhanMengLianSaiCompetitionAward();
				}
				else if (this.mBangHuiMatchGameStates == 5)
				{
				}
			};
			this.mBtnRank.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.ShowRanKPart();
			};
			this.mBtnWingTips.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.ShowBaZhuZhiYiPart();
			};
			this.mBtnGloryHall.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1499
				});
			};
			this.mBtnShiPin.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 0
				});
			};
		}
		catch (Exception ex)
		{
		}
	}

	private void ShowLianSaiGeiAwardPart()
	{
		if (null != this.mZhanMengLianSaiGetAwardParttWind)
		{
			this.mZhanMengLianSaiGetAwardParttWind.Visibility = true;
		}
		else
		{
			this.mZhanMengLianSaiGetAwardParttWind = U3DUtils.NEW<GChildWindow>();
			this.mZhanMengLianSaiGetAwardParttWind.transform.SetParent(base.transform, false);
			this.mZhanMengLianSaiGetAwardParttWind.ModalType = ChildWindowModalType.Translucent;
			this.mZhanMengLianSaiGetAwardParttWind.IsShowModal = false;
			WindowManage.AddWindows(this.mZhanMengLianSaiGetAwardParttWind, false, null);
		}
		this.mZhanMengLianSaiGetAwardPart = U3DUtils.NEW<ZhanMengLianSaiGetAwardPart>();
		this.mZhanMengLianSaiGetAwardParttWind.Body.Add(this.mZhanMengLianSaiGetAwardPart);
		this.mZhanMengLianSaiGetAwardParttWind.ChildWindowModalBakClick = delegate(object e, EventArgs s)
		{
			this.CloseLianSaiGetAwardPart();
			return true;
		};
		string text = string.Empty;
		List<GoodsData> goodsList = null;
		if (this.mOldBangHuiMatchType == 1)
		{
			text += Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("超级黄金联赛排名：")
			});
			goodsList = this.ConfigZhanMengLianSaiLeagueSuperAward.GetLianSaiAwardByID(this.mRank);
		}
		else if (this.mOldBangHuiMatchType == 2)
		{
			text += Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("潜力新星联赛排名：")
			});
			goodsList = this.ConfigZhanMengLianSaiLeagueNewAward.GetLianSaiAwardByID(this.mRank);
		}
		text += Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("第") + this.mRank + Global.GetLang("名")
		});
		this.mZhanMengLianSaiGetAwardPart.RefreshInf(text, goodsList);
		this.mZhanMengLianSaiGetAwardPart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e != null)
			{
				if (e.Type == 0)
				{
					this.CloseLianSaiGetAwardPart();
				}
				else if (e.IDType == 1)
				{
					this.CloseLianSaiGetAwardPart();
				}
			}
		};
	}

	private void CloseLianSaiGetAwardPart()
	{
		Object.Destroy(this.mZhanMengLianSaiGetAwardPart.gameObject);
		Object.Destroy(this.mZhanMengLianSaiGetAwardParttWind.gameObject);
		this.mZhanMengLianSaiGetAwardPart = null;
		this.mZhanMengLianSaiGetAwardParttWind = null;
	}

	private void ShowBaZhuZhiYiPart()
	{
		if (null != this.mZhanMengLianSaiBaZhuZhiYiPartWind)
		{
			this.mZhanMengLianSaiBaZhuZhiYiPartWind.Visibility = true;
		}
		else
		{
			this.mZhanMengLianSaiBaZhuZhiYiPartWind = U3DUtils.NEW<GChildWindow>();
			this.mZhanMengLianSaiBaZhuZhiYiPartWind.transform.SetParent(base.transform, false);
			this.mZhanMengLianSaiBaZhuZhiYiPartWind.ModalType = ChildWindowModalType.Translucent;
			this.mZhanMengLianSaiBaZhuZhiYiPartWind.IsShowModal = false;
			WindowManage.AddWindows(this.mZhanMengLianSaiBaZhuZhiYiPartWind, false, null);
		}
		this.mZhanMengLianSaiBaZhuZhiYiPart = U3DUtils.NEW<ZhanMengLianSaiBaZhuZhiYiPart>();
		this.mZhanMengLianSaiBaZhuZhiYiPartWind.Body.Add(this.mZhanMengLianSaiBaZhuZhiYiPart);
		this.mZhanMengLianSaiBaZhuZhiYiPartWind.ChildWindowModalBakClick = delegate(object e, EventArgs s)
		{
			this.CloseLianSaiBaZhuZhiYiPart();
			return true;
		};
		this.mZhanMengLianSaiBaZhuZhiYiPart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e != null)
			{
				if (e.Type == 0)
				{
					this.CloseLianSaiBaZhuZhiYiPart();
				}
				else if (e.Type == 1)
				{
					this.CloseLianSaiBaZhuZhiYiPart();
				}
			}
		};
	}

	private void CloseLianSaiBaZhuZhiYiPart()
	{
		Object.Destroy(this.mZhanMengLianSaiBaZhuZhiYiPart.gameObject);
		Object.Destroy(this.mZhanMengLianSaiBaZhuZhiYiPartWind.gameObject);
		this.mZhanMengLianSaiBaZhuZhiYiPart = null;
		this.mZhanMengLianSaiBaZhuZhiYiPartWind = null;
	}

	private void ShowRanKPart()
	{
		if (null != this.mZhanMengLianSaiMianRankPartWind)
		{
			this.mZhanMengLianSaiMianRankPartWind.Visibility = true;
		}
		else
		{
			this.mZhanMengLianSaiMianRankPartWind = U3DUtils.NEW<GChildWindow>();
			this.mZhanMengLianSaiMianRankPartWind.transform.SetParent(base.transform, false);
			this.mZhanMengLianSaiMianRankPartWind.Modal = true;
			this.mZhanMengLianSaiMianRankPartWind.IsShowModal = false;
			WindowManage.AddWindows(this.mZhanMengLianSaiMianRankPartWind, false, null);
		}
		this.mZhanMengLianSaiMianRankPart = U3DUtils.NEW<ZhanMengLianSaiMianRankPart>();
		this.mZhanMengLianSaiMianRankPartWind.Body.Add(this.mZhanMengLianSaiMianRankPart);
		this.mZhanMengLianSaiMianRankPart.ShowData(this.mBangHuiMatchType);
		this.mZhanMengLianSaiMianRankPart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e != null)
			{
				if (e.Type == 0 && e.ID == 0)
				{
					Object.Destroy(this.mZhanMengLianSaiMianRankPart.gameObject);
					Object.Destroy(this.mZhanMengLianSaiMianRankPartWind.gameObject);
					this.mZhanMengLianSaiMianRankPart = null;
					this.mZhanMengLianSaiMianRankPartWind = null;
				}
				else if (e.Type == 1)
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.GetZhanMengLianSaiRankInfo(e.ID);
				}
			}
		};
	}

	private void ShowShaiJiAwardPart()
	{
		if (null != this.mZhanMengLianSaiAwardPreviewPartWind)
		{
			this.mZhanMengLianSaiAwardPreviewPartWind.Visibility = true;
		}
		else
		{
			this.mZhanMengLianSaiAwardPreviewPartWind = U3DUtils.NEW<GChildWindow>();
			this.mZhanMengLianSaiAwardPreviewPartWind.transform.SetParent(base.transform, false);
			this.mZhanMengLianSaiAwardPreviewPartWind.Modal = true;
			this.mZhanMengLianSaiAwardPreviewPartWind.IsShowModal = false;
			WindowManage.AddWindows(this.mZhanMengLianSaiAwardPreviewPartWind, false, null);
		}
		this.mZhanMengLianSaiAwardPreviewPart = U3DUtils.NEW<ZhanMengLianSaiAwardPreviewPart>();
		this.mZhanMengLianSaiAwardPreviewPartWind.Body.Add(this.mZhanMengLianSaiAwardPreviewPart);
		this.mZhanMengLianSaiAwardPreviewPart.RefreshView(this.ConfigZhanMengLianSaiLeagueNewAward, this.ConfigZhanMengLianSaiLeagueSuperAward);
		this.mZhanMengLianSaiAwardPreviewPart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e != null && e.Type == 0)
			{
				Object.Destroy(this.mZhanMengLianSaiAwardPreviewPart.gameObject);
				Object.Destroy(this.mZhanMengLianSaiAwardPreviewPartWind.gameObject);
				this.mZhanMengLianSaiAwardPreviewPart = null;
				this.mZhanMengLianSaiAwardPreviewPartWind = null;
			}
		};
	}

	private void ShowPreviewRulePart()
	{
		if (null != this.ZhanMengLianSaiHelpPartWind)
		{
			this.ZhanMengLianSaiHelpPartWind.Visibility = true;
		}
		else
		{
			this.ZhanMengLianSaiHelpPartWind = U3DUtils.NEW<GChildWindow>();
			this.ZhanMengLianSaiHelpPartWind.transform.SetParent(base.transform, false);
			this.ZhanMengLianSaiHelpPartWind.ModalType = ChildWindowModalType.Translucent;
			this.ZhanMengLianSaiHelpPartWind.IsShowModal = false;
			WindowManage.AddWindows(this.ZhanMengLianSaiHelpPartWind, false, null);
		}
		this.mZhanMengLianSaiHelpPart = U3DUtils.NEW<ZhanMengLianSaiHelpPart>();
		this.ZhanMengLianSaiHelpPartWind.Body.Add(this.mZhanMengLianSaiHelpPart);
		this.ZhanMengLianSaiHelpPartWind.ChildWindowModalBakClick = delegate(object e, EventArgs s)
		{
			this.CloseLianSaiRulePart();
			return true;
		};
		this.mZhanMengLianSaiHelpPart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == 0)
			{
				this.CloseLianSaiRulePart();
			}
		};
	}

	private void CloseLianSaiRulePart()
	{
		Object.Destroy(this.mZhanMengLianSaiHelpPart.gameObject);
		Object.Destroy(this.ZhanMengLianSaiHelpPartWind.gameObject);
		this.mZhanMengLianSaiHelpPart = null;
		this.ZhanMengLianSaiHelpPartWind = null;
	}

	private int GetMatchIDByType(BangHuiMatchType type)
	{
		return type;
	}

	private string GetWeekNums(int index)
	{
		string[] array = new string[]
		{
			Global.GetLang("星期日"),
			Global.GetLang("星期一"),
			Global.GetLang("星期二"),
			Global.GetLang("星期三"),
			Global.GetLang("星期四"),
			Global.GetLang("星期五"),
			Global.GetLang("星期六")
		};
		return array[index];
	}

	private void SetActivityTimeInf()
	{
		string text = Global.GetLang("活动时间：") + "{fdf7dd}";
		List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
		if (lianSaiOpenAndCloseDateTimeByID != null && 0 < lianSaiOpenAndCloseDateTimeByID.Count)
		{
			int i = 0;
			while (i < lianSaiOpenAndCloseDateTimeByID.Count)
			{
				LianSaiLeagueMatchVO.TimePoint timePoint = lianSaiOpenAndCloseDateTimeByID[i];
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					Global.GetLang(this.GetWeekNums(timePoint.Week)),
					timePoint.Time1.ToString("HH:mm"),
					"-",
					timePoint.Time2.ToString("HH:mm")
				});
				i++;
				if (i < lianSaiOpenAndCloseDateTimeByID.Count)
				{
					text += Global.GetLang("、");
				}
			}
		}
		text += "{-}";
		this.mOpenTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			text
		});
	}

	private void RefreshBtnsInf()
	{
		if (this.mBangHuiMatchType == 1)
		{
			this.mIamgeBGTexture.URL = "NetImages/GameRes/Images/Plate/SuperLianSaiBG.jpg";
		}
		else
		{
			this.mIamgeBGTexture.URL = "NetImages/GameRes/Images/Plate/NewLianSaiBg.jpg";
		}
		if (this.mBangHuiMatchGameStates == null)
		{
			this.ChangeBtnState(this.mBtnJion, false, Global.GetLang("结算中"));
		}
		else if (this.mBangHuiMatchGameStates == 5)
		{
			this.ChangeBtnState(this.mBtnJion, false, Global.GetLang("未报名"));
		}
		else if (this.mBangHuiMatchGameStates == 1)
		{
			if (this.IsBangHuiLeader())
			{
				this.ChangeBtnState(this.mBtnJion, true, Global.GetLang("立即报名"));
			}
			else
			{
				this.ChangeBtnState(this.mBtnJion, false, Global.GetLang("未报名"));
			}
		}
		else if (this.mBangHuiMatchGameStates == 4)
		{
			this.ChangeBtnState(this.mBtnJion, true, Global.GetLang("领取奖励"));
		}
		else if (this.mBangHuiMatchGameStates == 3)
		{
			this.ChangeBtnState(this.mBtnJion, true, Global.GetLang("立即进入"));
		}
		else if (this.mBangHuiMatchGameStates == 2)
		{
			this.ChangeBtnState(this.mBtnJion, false, Global.GetLang("已报名"));
		}
		else if (this.mBangHuiMatchGameStates == 6)
		{
			this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("赛季已结束")
			});
			this.ChangeBtnState(this.mBtnJion, false, Global.GetLang("已结束"));
		}
		else if (this.mBangHuiMatchGameStates == 7)
		{
			this.ChangeBtnState(this.mBtnJion, false, Global.GetLang("轮空"));
		}
		if (this.mOldBangHuiMatchType == 1)
		{
			if (this.ConfigZhanMengLianSaiLeagueSuperAward.GetLianSaiAwardByID(this.mRank) != null)
			{
				this.ChangeBtnState(this.mBtnAwardPreview_, true, Global.GetLang("领取奖励"));
			}
		}
		else if (this.mOldBangHuiMatchType == 2 && this.ConfigZhanMengLianSaiLeagueNewAward.GetLianSaiAwardByID(this.mRank) != null)
		{
			this.ChangeBtnState(this.mBtnAwardPreview_, true, Global.GetLang("领取奖励"));
		}
	}

	private void ChangeBtnState(GButton btn, bool isEnabled, string btnStr)
	{
		if (null != btn)
		{
			if (btnStr != null && null != btn.Label && !string.IsNullOrEmpty(btnStr))
			{
				btn.Label.text = btnStr;
			}
			btn.isEnabled = isEnabled;
			string text = string.Empty;
			string text2 = string.Empty;
			if (isEnabled)
			{
				text = "btn_green";
				text2 = "btn_green_selected";
			}
			else
			{
				text = "btn_green_disable";
				text2 = "btn_green_disable";
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				btn.normalSprite = text;
				btn.hoverSprite = text2;
				btn.pressedSprite = text2;
				btn.disabledSprite = text2;
			}
			btn.Refresh();
		}
	}

	private long SignStartTimeLift()
	{
		List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
		if (this.mBangHuiMatchGameStates == 4 || this.mBangHuiMatchGameStates == 6 || this.mBangHuiMatchGameStates == 5)
		{
			DateTime lianSaiOpenDateTime = this.ConfigZhanMengLianSaiLeagueOpen.GetLianSaiOpenDateTime();
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime.Ticks < lianSaiOpenDateTime.Ticks)
			{
				return lianSaiOpenDateTime.Ticks - correctDateTime.Ticks;
			}
		}
		return 0L;
	}

	private int[] SignStartTimeLiftEX()
	{
		int[] array = new int[4];
		if (this.mBangHuiMatchGameStates == null)
		{
			List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
			for (int i = 0; i < lianSaiOpenAndCloseDateTimeByID.Count; i++)
			{
				LianSaiLeagueMatchVO.TimePoint timePoint = lianSaiOpenAndCloseDateTimeByID[i];
				DateTime dateTime = timePoint.Time2.AddSeconds((double)this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiLeagueMatchVOByID(this.GetMatchIDByType(this.mBangHuiMatchType)).ApplyStartTime);
				DateTime correctDateTime = Global.GetCorrectDateTime();
				if (timePoint.Week == correctDateTime.DayOfWeek)
				{
					int num = correctDateTime.DayOfWeek - timePoint.Week;
					int num2 = dateTime.Hour - correctDateTime.Hour;
					int num3 = dateTime.Minute - correctDateTime.Minute;
					int num4 = dateTime.Second - correctDateTime.Second;
					this.CheckTime(ref num, ref num2, ref num3, ref num4);
					array[0] = num;
					array[1] = num2;
					array[2] = num3;
					array[3] = num4;
					break;
				}
			}
		}
		return array;
	}

	private long GetOneDayTicks()
	{
		long num = 86400000L;
		return num * 10000L;
	}

	private int[] SignTimeLift()
	{
		int[] array = new int[4];
		if (this.mBangHuiMatchGameStates == 1 || this.mBangHuiMatchGameStates == 4 || this.mBangHuiMatchGameStates == 2)
		{
			List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
			DateTime correctDateTime = Global.GetCorrectDateTime();
			for (int i = 0; i < lianSaiOpenAndCloseDateTimeByID.Count; i++)
			{
				LianSaiLeagueMatchVO.TimePoint timePoint = lianSaiOpenAndCloseDateTimeByID[i];
				DateTime dateTime = this.SubSeconds(timePoint.Time1, this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiLeagueMatchVOByID(this.GetMatchIDByType(this.mBangHuiMatchType)).ApplyOverTime);
				int num = this.WeekToIntValue(timePoint.Week);
				if (num >= this.WeekToIntValue(correctDateTime.DayOfWeek))
				{
					int num2 = num - this.WeekToIntValue(correctDateTime.DayOfWeek);
					int num3 = dateTime.Hour - correctDateTime.Hour;
					int num4 = dateTime.Minute - correctDateTime.Minute;
					int num5 = dateTime.Second - correctDateTime.Second;
					long num6 = (long)num3 * 3600L + (long)num4 * 60L + (long)num5;
					long num7 = num6 * 10000000L + this.GetOneDayTicks() * (long)num2;
					this.CheckTime(ref num2, ref num3, ref num4, ref num5);
					if (0L <= num7)
					{
						array[0] = num2;
						array[1] = num3;
						array[2] = num4;
						array[3] = num5;
						return array;
					}
				}
			}
			LianSaiLeagueMatchVO.TimePoint timePoint2 = lianSaiOpenAndCloseDateTimeByID[0];
			DateTime correctDateTime2 = Global.GetCorrectDateTime();
			int num8 = this.WeekToIntValue(timePoint2.Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek);
			DateTime dateTime2 = this.SubSeconds(timePoint2.Time1, this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiLeagueMatchVOByID(this.GetMatchIDByType(this.mBangHuiMatchType)).ApplyOverTime);
			int num9 = dateTime2.Hour - correctDateTime2.Hour;
			int num10 = dateTime2.Minute - correctDateTime2.Minute;
			int num11 = dateTime2.Second - correctDateTime2.Second;
			this.CheckTime(ref num8, ref num9, ref num10, ref num11);
			array[0] = num8;
			array[1] = num9;
			array[2] = num10;
			array[3] = num11;
		}
		return array;
	}

	private int WeekToIntValue(int week)
	{
		if (week == 0)
		{
			return 7;
		}
		return week;
	}

	private int WeekToIntValue(DayOfWeek week)
	{
		if (week == null)
		{
			return 7;
		}
		return week;
	}

	private long GetMatchStartSignWithMatchEndSginTime()
	{
		List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
		int num = 0;
		for (int i = 0; i < lianSaiOpenAndCloseDateTimeByID.Count - 1; i++)
		{
			int num2 = (lianSaiOpenAndCloseDateTimeByID[i].Week != 0) ? lianSaiOpenAndCloseDateTimeByID[i].Week : 7;
			int num3 = (lianSaiOpenAndCloseDateTimeByID[i + 1].Week != 0) ? lianSaiOpenAndCloseDateTimeByID[i + 1].Week : 7;
			if (num2 > num3)
			{
				int num4 = num3;
				num3 = num2;
				num2 = num4;
			}
			int num5 = Mathf.Abs(num2 - num3);
			int num6 = Mathf.Abs(7 + num2 - num3);
			num = ((num5 <= num6) ? num6 : num5);
		}
		return ((long)lianSaiOpenAndCloseDateTimeByID[0].Time1.Hour * 3600L + (long)lianSaiOpenAndCloseDateTimeByID[0].Time1.Minute * 60L + (long)lianSaiOpenAndCloseDateTimeByID[0].Time1.Second) * 10000000L + this.GetOneDayTicks() * (long)num;
	}

	private int[] MatchstarttimeLiftLeap()
	{
		int num = 0;
		int[] array = new int[4];
		if (this.mBangHuiMatchGameStates == 2)
		{
			List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
			DateTime correctDateTime = Global.GetCorrectDateTime();
			int[] leapMatchTime = this.GetLeapMatchTime();
			if (leapMatchTime != null)
			{
				try
				{
					DateTime dateTime;
					dateTime..ctor(leapMatchTime[0], leapMatchTime[1], leapMatchTime[2]);
					if (correctDateTime < dateTime)
					{
						int num2 = 0;
						int num3 = 0;
						int num4 = 0;
						for (int i = 0; i < lianSaiOpenAndCloseDateTimeByID.Count; i++)
						{
							LianSaiLeagueMatchVO.TimePoint timePoint = lianSaiOpenAndCloseDateTimeByID[i];
							int num5 = this.WeekToIntValue(timePoint.Week);
							if (num5 >= this.WeekToIntValue(dateTime.DayOfWeek))
							{
								num = num5 - this.WeekToIntValue(dateTime.DayOfWeek);
								num2 = timePoint.Time1.Hour - dateTime.Hour;
								num3 = timePoint.Time1.Minute - dateTime.Minute;
								num4 = timePoint.Time1.Second - dateTime.Second;
								break;
							}
						}
						if (num == 0 && 0 >= num2 && 0 >= num3 && 0 >= num4)
						{
							num = this.WeekToIntValue(lianSaiOpenAndCloseDateTimeByID[0].Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek);
						}
						dateTime = dateTime.AddDays((double)num);
						dateTime = dateTime.AddHours((double)num2);
						dateTime = dateTime.AddMinutes((double)num3);
						dateTime = dateTime.AddSeconds((double)num4);
						array[0] = dateTime.Day - correctDateTime.Day;
						array[1] = dateTime.Hour - correctDateTime.Hour;
						array[2] = dateTime.Minute - correctDateTime.Minute;
						array[3] = dateTime.Second - correctDateTime.Second;
						this.CheckTime(ref array[0], ref array[1], ref array[2], ref array[3]);
						return array;
					}
				}
				catch (Exception ex)
				{
				}
				return array;
			}
		}
		return array;
	}

	private int[] MatchStartTimeLift()
	{
		int[] array = new int[4];
		if (this.mBangHuiMatchGameStates == 2 || this.mBangHuiMatchGameStates == 5)
		{
			List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.mConfigZhanMengLianSaiLeagueMatch.GetLianSaiOpenAndCloseDateTimeByID(this.GetMatchIDByType(this.mBangHuiMatchType));
			DateTime correctDateTime = Global.GetCorrectDateTime();
			for (int i = 0; i < lianSaiOpenAndCloseDateTimeByID.Count; i++)
			{
				LianSaiLeagueMatchVO.TimePoint timePoint = lianSaiOpenAndCloseDateTimeByID[i];
				DateTime time = timePoint.Time1;
				int num = this.WeekToIntValue(timePoint.Week);
				if (num >= this.WeekToIntValue(correctDateTime.DayOfWeek))
				{
					int num2 = num - this.WeekToIntValue(correctDateTime.DayOfWeek);
					int num3 = time.Hour - correctDateTime.Hour;
					int num4 = time.Minute - correctDateTime.Minute;
					int num5 = time.Second - correctDateTime.Second;
					long num6 = (long)num3 * 3600L + (long)num4 * 60L + (long)num5;
					long num7 = num6 * 10000000L + this.GetOneDayTicks() * (long)num2;
					this.CheckTime(ref num2, ref num3, ref num4, ref num5);
					if (0L <= num7)
					{
						array[0] = num2;
						array[1] = num3;
						array[2] = num4;
						array[3] = num5;
						return array;
					}
					long ticks = (timePoint.Time2 - timePoint.Time1).Ticks;
					if (ticks + num7 > 0L)
					{
						return array;
					}
				}
			}
			LianSaiLeagueMatchVO.TimePoint timePoint2 = lianSaiOpenAndCloseDateTimeByID[0];
			int num8 = this.WeekToIntValue(timePoint2.Week) + 7 - this.WeekToIntValue(correctDateTime.DayOfWeek);
			DateTime time2 = timePoint2.Time1;
			int num9 = time2.Hour - correctDateTime.Hour;
			int num10 = time2.Minute - correctDateTime.Minute;
			int num11 = time2.Second - correctDateTime.Second;
			this.CheckTime(ref num8, ref num9, ref num10, ref num11);
			array[0] = num8;
			array[1] = num9;
			array[2] = num10;
			array[3] = num11;
		}
		return array;
	}

	private void CheckTime(ref int day, ref int Hours, ref int Minute, ref int Second)
	{
		long num = (long)Hours * 3600L + (long)Minute * 60L + (long)Second;
		if (0 > Second)
		{
			Minute--;
			Second += 60;
		}
		if (0 > Minute)
		{
			Hours--;
			Minute += 60;
		}
		if (0 > Hours)
		{
			day--;
			Hours += 24;
		}
	}

	private void StartTimeTicks()
	{
		this.mDispatcherTimer1 = null;
		this.mDispatcherTimer1 = new DispatcherTimer("ZhanMengLianSaiTicks1");
		this.mDispatcherTimer1.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer1.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer1.Start();
		this.UITimeTicks(null, null);
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		if (this.mSynchronousData != null && this.mSynchronousData.UpDate())
		{
			GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(false);
			this.mSynchronousData = null;
		}
		string text = string.Empty;
		if (this.mBangHuiMatchGameStates == 4)
		{
			this.mSignNameLabel.text = string.Empty;
			return;
		}
		if (this.mBangHuiMatchGameStates == 1)
		{
			int[] array = this.SignTimeLift();
			if (0L <= this.PressTimeToTicks(array) && 7 > array[0])
			{
				text = Global.GetLang("报名截止：");
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.ParseDateTimeToChanese(array)
				});
				this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					text
				});
				if (array[3] == 0 && this.mSynchronousData == null)
				{
					this.mSynchronousData = new ZhanMengLianSaiMainPart.SynchronousData();
				}
				return;
			}
			this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("报名截止：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("0小时0分0秒")
			});
			if (this.mSynchronousData == null)
			{
				this.mSynchronousData = new ZhanMengLianSaiMainPart.SynchronousData();
			}
		}
		else if (this.mBangHuiMatchGameStates == 5)
		{
			int[] array2 = this.MatchStartTimeLift();
			if (0L < this.PressTimeToTicks(array2) && 7 > array2[0])
			{
				text = Global.GetLang("距比赛开始还有：") + Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.ParseDateTimeToChanese(array2)
				});
				this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					text
				});
				return;
			}
			if (this.PressTimeToTicks(array2) == 0L)
			{
				this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("联赛已开始")
				});
				return;
			}
			this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("距比赛开始还有：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("0小时0分0秒")
			});
			if (this.mSynchronousData == null)
			{
				this.mSynchronousData = new ZhanMengLianSaiMainPart.SynchronousData();
			}
		}
		else if (this.mBangHuiMatchGameStates == null)
		{
			int[] array3 = this.SignStartTimeLiftEX();
			if (0L <= this.PressTimeToTicks(array3) && 7 > array3[0])
			{
				this.mSignNameLabel.text = string.Empty;
				return;
			}
			this.mSignNameLabel.text = string.Empty;
			if (this.mSynchronousData == null)
			{
				this.mSynchronousData = new ZhanMengLianSaiMainPart.SynchronousData();
			}
		}
		else if (this.mBangHuiMatchGameStates == 2)
		{
			int[] times = this.MatchstarttimeLiftLeap();
			if (0L < this.PressTimeToTicks(times))
			{
				text = Global.GetLang("距比赛开始还有：") + Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.ParseDateTimeToChanese(times)
				});
				this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					text
				});
				return;
			}
			int[] times2 = this.MatchStartTimeLift();
			if (0L < this.PressTimeToTicks(times2))
			{
				text = Global.GetLang("距比赛开始还有：") + Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.ParseDateTimeToChanese(times2)
				});
				this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					text
				});
				return;
			}
			this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("距比赛开始还有：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("0小时0分0秒")
			});
			if (this.mSynchronousData == null)
			{
				this.mSynchronousData = new ZhanMengLianSaiMainPart.SynchronousData();
			}
		}
		else if (this.mBangHuiMatchGameStates == 3)
		{
			this.mSignNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("比赛已开始")
			});
		}
	}

	private int[] GetLeapMatchTime()
	{
		if (this.mBangHuiMatchType == 1 && 0 < this.mNextBeginTime)
		{
			return new int[]
			{
				this.mNextBeginTime / 10000,
				this.mNextBeginTime % (this.mNextBeginTime / 10000) / 100,
				this.mNextBeginTime % (this.mNextBeginTime / 10000) % 100
			};
		}
		return null;
	}

	private long PressTimeToTicks(int[] times)
	{
		return ((long)times[1] * 3600L + (long)times[3] * 60L + (long)times[2]) * 10000000L + this.GetOneDayTicks() * (long)times[0];
	}

	private string ParseDateTimeToChanese(int[] times)
	{
		string text = string.Empty;
		byte b = 0;
		string[] array = new string[]
		{
			Global.GetLang("天"),
			Global.GetLang("小时"),
			Global.GetLang("分钟"),
			Global.GetLang("秒")
		};
		byte b2 = 0;
		while ((int)b2 < times.Length)
		{
			if (times[(int)b2] > 0)
			{
				text = text + times[(int)b2].ToString() + array[(int)b2];
				b = 1;
			}
			else if (b == 1)
			{
				text = text + times[(int)b2].ToString() + array[(int)b2];
			}
			b2 += 1;
		}
		return text;
	}

	private string ParseDateTimeToChanese(DateTime time)
	{
		string text = string.Empty;
		string text2 = (TimeSpan.FromTicks(time.Ticks).TotalHours <= 24.0) ? time.ToString("HH:mm:ss") : time.ToString("d H:m:s");
		if (!string.IsNullOrEmpty(text2))
		{
			string[] array = text2.Split(new char[]
			{
				' '
			});
			if (array.Length == 2)
			{
				string[] array2 = array[0].Split(new char[]
				{
					':'
				});
				string[] array3 = array[1].Split(new char[]
				{
					':'
				});
				if (array2.Length == 1)
				{
					text = text + array2[0] + Global.GetLang("天");
				}
				else if (array2.Length == 2)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						array2[0],
						Global.GetLang("月"),
						array2[1],
						Global.GetLang("天")
					});
				}
				else if (array2.Length == 3)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						array2[0],
						Global.GetLang("年"),
						array2[1],
						Global.GetLang("月"),
						array2[2],
						Global.GetLang("天")
					});
				}
				if (array3.Length == 1)
				{
					text = text + array3[0] + Global.GetLang("秒");
				}
				else if (array3.Length == 2)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						array3[0],
						Global.GetLang("分钟"),
						array3[1],
						Global.GetLang("秒")
					});
				}
				else if (array3.Length == 3)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						array3[0],
						Global.GetLang("小时"),
						array3[1],
						Global.GetLang("分钟"),
						array3[2],
						Global.GetLang("秒")
					});
				}
			}
			else if (array.Length == 1)
			{
				string[] array4 = array[0].Split(new char[]
				{
					':'
				});
				if (array4.Length == 1)
				{
					text = text + array4[0] + Global.GetLang("秒");
				}
				else if (array4.Length == 2)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						array4[0],
						Global.GetLang("分钟"),
						array4[1],
						Global.GetLang("秒")
					});
				}
				else if (array4.Length == 3)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						array4[0],
						Global.GetLang("小时"),
						array4[1],
						Global.GetLang("分钟"),
						array4[2],
						Global.GetLang("秒")
					});
				}
			}
		}
		return text;
	}

	private DateTime SubSeconds(DateTime time, long value)
	{
		return time.AddSeconds((double)(-(double)value));
	}

	private void StopTimeTicks()
	{
		if (this.mDispatcherTimer1 != null)
		{
			this.mDispatcherTimer1.Stop();
			this.mDispatcherTimer1.Dispose();
			this.mDispatcherTimer1 = null;
		}
	}

	public void NoticeZhanMengLianSaiRankInfoCallBack(List<BangHuiMatchRankInfo> data)
	{
	}

	internal void NoticeGetSaiJiAwardCallBack(int ret)
	{
		Super.HideNetWaiting();
		if (ret == 0)
		{
			GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(true);
			this.mBtnAwardPreview_.Label.text = Global.GetLang("赛季奖励");
			Transform transform = this.mBtnAwardPreview_.transform.FindChild("TipIconTop");
			if (null != transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
		}
	}

	public void NoticeJionCallBack(string ret)
	{
		if ("0".Equals(ret))
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(true);
			Super.HintMainText(Global.GetLang("报名成功"), 10, 3);
		}
		else
		{
			Super.HintMainText(StdErrorCode.GetErrMsg(ret.SafeToInt32(0), true, false), 10, 3);
		}
	}

	public void RefreshInf(BangHuiMatchType MatchType, BangHuiMatchType OldBangHuiMatchType, int Rank, BangHuiMatchGameStates state, ConfigZhanMengLianSaiLeagueMatch MathXml, int NextBeginTime)
	{
		this.mConfigZhanMengLianSaiLeagueMatch = MathXml;
		this.mBangHuiMatchType = MatchType;
		this.mOldBangHuiMatchType = OldBangHuiMatchType;
		this.mRank = Rank;
		this.mBangHuiMatchGameStates = state;
		this.mNextBeginTime = NextBeginTime;
		if (OldBangHuiMatchType == 1)
		{
			if (this.ConfigZhanMengLianSaiLeagueSuperAward.GetLianSaiAwardByID(Rank) != null)
			{
				this.mBtnAwardPreview_.Label.text = Global.GetLang("领取奖励");
				Transform transform = this.mBtnAwardPreview_.transform.FindChild("TipIconTop");
				if (null != transform)
				{
					transform.gameObject.SetActive(true);
				}
			}
		}
		else if (OldBangHuiMatchType == 2 && this.ConfigZhanMengLianSaiLeagueNewAward.GetLianSaiAwardByID(Rank) != null)
		{
			this.mBtnAwardPreview_.Label.text = Global.GetLang("领取奖励");
			Transform transform2 = this.mBtnAwardPreview_.transform.FindChild("TipIconTop");
			if (null != transform2)
			{
				transform2.gameObject.SetActive(true);
			}
		}
		this.SetActivityTimeInf();
		this.RefreshBtnsInf();
		this.StopTimeTicks();
		this.StartTimeTicks();
	}

	public void NoticeGetSaiJiGetRankInfMiniCallBack(List<BangHuiMatchRankInfo> Datalist)
	{
	}

	public void NotifyGetLianSaiANALYSISCallBack(List<int> dataList)
	{
	}

	public void NotifyLianSaiRankCallBack(List<BangHuiMatchRankInfo> data)
	{
		Super.HideNetWaiting();
		if (null != this.mZhanMengLianSaiMianRankPart)
		{
			this.mZhanMengLianSaiMianRankPart.NoticeRankDataCallBack(data);
		}
	}

	public void NoticeGetSaiJiGetBHMatch_AwardCallBack(BangHuiMatchAwardsData data)
	{
	}

	public void NoticeGetSaiJiGetBHMatch_GetAwardCallBack()
	{
		GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(true);
	}

	public void NoticeGetShiPinDataCallBack(Dictionary<int, OrnamentData> data)
	{
	}

	private const long mOneHourEqualSecond = 3600L;

	private const long mOneMinteEqualSecond = 60L;

	private const long mOneSecondEqualTicks = 10000000L;

	[SerializeField]
	private GButton mBtnAwardPreview_;

	[SerializeField]
	private GButton mBtnPreviewRule;

	[SerializeField]
	private GButton mBtnJion;

	[SerializeField]
	private GButton mBtnRank;

	[SerializeField]
	private GButton mBtnWingTips;

	[SerializeField]
	private GButton mBtnGloryHall;

	[SerializeField]
	private GButton mBtnShiPin;

	[SerializeField]
	private ShowNetImage mIamgeBGTexture;

	[SerializeField]
	private UILabel mSignNameLabel;

	[SerializeField]
	private UILabel mOpenTimeLabel;

	private ConfigZhanMengLianSaiLeagueNewAward mConfigZhanMengLianSaiLeagueNewAward;

	private ConfigZhanMengLianSaiLeagueSuperAward mConfigZhanMengLianSaiLeagueSuperAward;

	private ConfigZhanMengLianSaiLeagueMatch mConfigZhanMengLianSaiLeagueMatch;

	private ConfigZhanMengLianSaiLeagueOpen mConfigZhanMengLianSaiLeagueOpen;

	private DispatcherTimer mDispatcherTimer1;

	private ZhanMengLianSaiAwardPreviewPart mZhanMengLianSaiAwardPreviewPart;

	private GChildWindow mZhanMengLianSaiAwardPreviewPartWind;

	private ZhanMengLianSaiMianRankPart mZhanMengLianSaiMianRankPart;

	private GChildWindow mZhanMengLianSaiMianRankPartWind;

	private ZhanMengLianSaiBaZhuZhiYiPart mZhanMengLianSaiBaZhuZhiYiPart;

	private GChildWindow mZhanMengLianSaiBaZhuZhiYiPartWind;

	private ZhanMengLianSaiGetAwardPart mZhanMengLianSaiGetAwardPart;

	private GChildWindow mZhanMengLianSaiGetAwardParttWind;

	private ZhanMengLianSaiHelpPart mZhanMengLianSaiHelpPart;

	private GChildWindow ZhanMengLianSaiHelpPartWind;

	private BangHuiMatchGameStates mBangHuiMatchGameStates;

	private BangHuiMatchType mBangHuiMatchType = 2;

	private int mRank;

	private BangHuiMatchType mOldBangHuiMatchType = 2;

	private ZhanMengLianSaiMainPart.SynchronousData mSynchronousData;

	private int mNextBeginTime;

	private Dictionary<BangHuiMatchRankType, List<BangHuiMatchRankInfo>> Textdatadic = new Dictionary<BangHuiMatchRankType, List<BangHuiMatchRankInfo>>();

	public DPSelectedItemEventHandler Hander;

	private class SynchronousData
	{
		public SynchronousData()
		{
			GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(false);
			this.Synchronous = true;
		}

		public bool Synchronous
		{
			set
			{
				if (!this.mSynchronous)
				{
					this.mSynchronous = value;
					this.mSynchronousTime = 5;
				}
				if (!value)
				{
					this.mSynchronous = value;
				}
			}
		}

		public bool UpDate()
		{
			if (!this.mSynchronous)
			{
				return false;
			}
			this.mSynchronousTime--;
			if (0 > this.mSynchronousTime)
			{
				this.Synchronous = false;
				return true;
			}
			return false;
		}

		private int mSynchronousTime;

		private bool mSynchronous;
	}
}
