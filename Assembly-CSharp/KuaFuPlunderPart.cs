using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class KuaFuPlunderPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitAwardGoodsList();
		GameInstance.Game.SendGetKuFuPlubderGameStateData((long)((this.mKuaFuLueDuoStateData != null) ? this.mKuaFuLueDuoStateData.GameState : -1));
	}

	protected override void OnDestroy()
	{
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
		base.OnDestroy();
		this.StopTimeTicks();
	}

	private bool IsBangHuiLeader()
	{
		return Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction);
	}

	private void InitPrefabText()
	{
		try
		{
			this.mGetAwardBtn.Text = Global.GetLang("领取奖励");
			this.mRuleBtn.Text = Global.GetLang("查看规则");
			if (this.IsBangHuiLeader())
			{
				this.mBiddingBtn.Text = Global.GetLang("参与竞价");
			}
			else
			{
				this.mBiddingBtn.Text = Global.GetLang("掠夺地图");
			}
			this.mRuleBtn.Label.lineWidth = 115;
			this.mBiddingBtn.Label.lineWidth = 115;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	private void InitAwardGoodsList()
	{
		List<GoodsData> showGoodsDataListByID = this.mCrusadeWarXml.GetShowGoodsDataListByID(1);
		if (showGoodsDataListByID != null)
		{
			for (int i = 0; i < showGoodsDataListByID.Count; i++)
			{
				GoodsData goodsData = showGoodsDataListByID[i];
				GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
				newGoodIcon.GoodsID = goodsData.GoodsID;
				newGoodIcon.Width = 70.0;
				newGoodIcon.Height = 70.0;
				newGoodIcon.ItemCategory = Global.GetGoodsCatetoriy(goodsData.GoodsID);
				newGoodIcon.ItemObject = goodsData;
				newGoodIcon.isAutoSize = true;
				newGoodIcon.BackSpriteName0 = "bagGrid4_bak";
				newGoodIcon.BackgroundSprite0.gameObject.transform.localScale = new Vector3(80f, 80f, 0f);
				newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsData.GoodsID), string.Empty)
				}), false, 0);
				newGoodIcon.Tip = Global.GetGoodsNameByID(goodsData.GoodsID, false);
				Super.InitGoodsGIcon(newGoodIcon, goodsData, true, IconTextTypes.Qianghua);
				newGoodIcon.transform.SetParent(this.mAwardGoodsListRoot, false);
				newGoodIcon.transform.localPosition = new Vector3((float)(-84 + 84 * i), 0f, 0f);
				newGoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			}
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	private void InitTexture()
	{
		try
		{
			this.mTimeLabel.text = string.Empty;
			this.mOpenTimeLabel.text = string.Empty;
			this.mIamgeBGTexture.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/MainPartBgImage.png";
			this.mIamgeBGTexture.ImageDownloaded = delegate(object g)
			{
				this.mIamgeBGTexture.transform.localScale = new Vector3((float)this.mIamgeBGTexture.ItsSizeWidth, (float)this.mIamgeBGTexture.ItsSizeHeight, 0f);
			};
			this.mIamgeBGTexture1.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_KuaFuLueDuoDi.jpg";
			this.mIamgeBGTexture1.ImageDownloaded = delegate(object g)
			{
				this.mIamgeBGTexture.transform.localScale = new Vector3((float)this.mIamgeBGTexture1.ItsSizeWidth, (float)this.mIamgeBGTexture1.ItsSizeHeight, 0f);
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mPlunderHallBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.ShowPlunderRankPart();
			};
			this.mAwakenShopBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1512
				});
			};
			this.mShiPinBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 1
				});
			};
			this.mGetAwardBtn.gameObject.SetActive(false);
			this.mGetAwardBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mKuaFuLueDuoStateData != null && this.mKuaFuLueDuoStateData.AwardsDataList != null && 0 <= this.mKuaFuLueDuoStateData.AwardsDataList.Count)
				{
					PlayZone.GlobalPlayZone.OpenKuaFuPlunderAwardPart(this.mKuaFuLueDuoStateData.AwardsDataList);
					return;
				}
				Super.HintMainText(Global.GetLang("当前无奖励可领"), 10, 3);
			};
			this.mRuleBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.ShowRulePart();
			};
			this.mBiddingBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mKuaFuLueDuoStateData == null)
				{
					Super.HintMainText(Global.GetLang("服务器尚在保护中，功能未开启"), 10, 3);
					return;
				}
				if (0 >= this.mKuaFuLueDuoStateData.ServerID)
				{
					Super.HintMainText(Global.GetLang("服务器尚在保护中，功能未开启"), 10, 3);
					return;
				}
				if (this.mKuaFuPlunderGameStateType == 4)
				{
					Super.HintMainText(Global.GetLang("正在结算"), 10, 3);
					return;
				}
				if (this.IsBangHuiLeader())
				{
					if (this.mKuaFuPlunderGameStateType != 1)
					{
						if (this.mKuaFuPlunderGameStateType == 3)
						{
							PlayZone.GlobalPlayZone.ShowKuaFuPlunderChoseFightPart();
						}
						else if (this.mKuaFuPlunderGameStateType == 2)
						{
							PlayZone.GlobalPlayZone.ShowKuaFuPlunderChoseFightPart();
						}
						else
						{
							this.GoToMap();
						}
					}
					else
					{
						this.GoToMap();
					}
				}
				else if (this.mKuaFuPlunderGameStateType == 3)
				{
					PlayZone.GlobalPlayZone.ShowKuaFuPlunderChoseFightPart();
				}
				else if (this.mKuaFuPlunderGameStateType == 2)
				{
					PlayZone.GlobalPlayZone.ShowKuaFuPlunderChoseFightPart();
				}
				else
				{
					this.GoToMap();
				}
			};
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this.mBtnClose, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	private void GoToMap()
	{
		if (Global.Data.CurrentCopyTeamData != null)
		{
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
					GameInstance.Game.SpriteGotToMap(91000);
					if (this.Hander != null)
					{
						this.Hander(this.mBtnClose, new DPSelectedItemEventArgs
						{
							ID = 0,
							Type = 0
						});
					}
					PlayZone.GlobalPlayZone.CloseActivityWindow();
				}
			}, -1);
		}
		else
		{
			GameInstance.Game.SpriteGotToMap(91000);
			if (this.Hander != null)
			{
				this.Hander(this.mBtnClose, new DPSelectedItemEventArgs
				{
					ID = 0,
					Type = 0
				});
			}
			PlayZone.GlobalPlayZone.CloseActivityWindow();
		}
	}

	private void ShowRulePart()
	{
		if (null != this.mKuafuPlunderRulePart)
		{
			this.mKuafuPlunderRulePartWind.Visibility = true;
		}
		else
		{
			this.mKuafuPlunderRulePartWind = U3DUtils.NEW<GChildWindow>();
			this.mKuafuPlunderRulePartWind.transform.SetParent(base.transform, false);
			this.mKuafuPlunderRulePartWind.ModalType = ChildWindowModalType.Translucent;
			this.mKuafuPlunderRulePartWind.IsShowModal = false;
			WindowManage.AddWindows(this.mKuafuPlunderRulePartWind, false, null);
		}
		this.mKuafuPlunderRulePart = U3DUtils.NEW<KuafuPlunderRulePart>();
		this.mKuafuPlunderRulePartWind.Body.Add(this.mKuafuPlunderRulePart);
		this.mKuafuPlunderRulePart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e != null)
			{
				if (0 < e.ID)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1508,
						Index = 2,
						MyID = e.ID - 1
					});
				}
				if (e.Type == 0)
				{
					Super.CloseChildWindow(this, this.mKuafuPlunderRulePartWind);
					this.mKuafuPlunderRulePartWind = null;
					if (null != this.mKuafuPlunderRulePart)
					{
						Object.Destroy(this.mKuafuPlunderRulePart.gameObject);
						this.mKuafuPlunderRulePart = null;
					}
				}
			}
		};
	}

	private void ShowPlunderRankPart()
	{
		if (null != this.mKuaFuPlunderRankPartWind)
		{
			this.mKuaFuPlunderRankPartWind.Visibility = true;
		}
		else
		{
			this.mKuaFuPlunderRankPartWind = U3DUtils.NEW<GChildWindow>();
			this.mKuaFuPlunderRankPartWind.transform.SetParent(base.transform, false);
			this.mKuaFuPlunderRankPartWind.ModalType = ChildWindowModalType.Translucent;
			this.mKuaFuPlunderRankPartWind.IsShowModal = false;
			WindowManage.AddWindows(this.mKuaFuPlunderRankPartWind, false, null);
		}
		this.mKuaFuPlunderRankPart = U3DUtils.NEW<KuaFuPlunderRankPart>();
		this.mKuaFuPlunderRankPartWind.Body.Add(this.mKuaFuPlunderRankPart);
		this.mKuaFuPlunderRankPart.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e != null)
			{
				if (e.Type == 0)
				{
					this.PlunderRankPart();
				}
				else if (e.Type == 1)
				{
					this.PlunderRankPart();
				}
			}
		};
	}

	private void PlunderRankPart()
	{
		if (null != this.mKuaFuPlunderRankPart)
		{
			Object.Destroy(this.mKuaFuPlunderRankPart.gameObject);
			this.mKuaFuPlunderRankPart = null;
		}
		if (null != this.mKuaFuPlunderRankPartWind)
		{
			WindowManage.RemoveWindows(this.mKuaFuPlunderRankPartWind);
			Object.Destroy(this.mKuaFuPlunderRankPartWind.gameObject);
			this.mKuaFuPlunderRankPartWind = null;
		}
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

	private void UITimeTicks(object sender, EventArgs args)
	{
		DateTime minValue = DateTime.MinValue;
		bool flag = false;
		int nextStateTimeData = this.mCrusadeWarXml.GetNextStateTimeData(out minValue, out flag, this.mKuaFuPlunderGameStateType);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (!flag)
		{
			if (this.mKuaFuPlunderGameStateType == 3)
			{
				int sec = (int)(minValue - correctDateTime).TotalSeconds;
				this.mTimeLabel.text = string.Empty;
				UILabel uilabel = this.mTimeLabel;
				uilabel.text += Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("距活动结束："),
					"17e43e",
					Global.GetTimeStrBySecFilterZero(sec, true, 2)
				});
			}
			else if (this.mKuaFuPlunderGameStateType == null)
			{
				int sec2 = (int)(minValue - correctDateTime).TotalSeconds;
				this.mTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("距竞价开始：")
				});
				UILabel uilabel2 = this.mTimeLabel;
				uilabel2.text += Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetTimeStrBySecFilterZero(sec2, true, 2)
				});
			}
			else if (this.mKuaFuPlunderGameStateType == 1)
			{
				if (nextStateTimeData == -1)
				{
					this.mTimeLabel.text = string.Empty;
					UILabel uilabel3 = this.mTimeLabel;
					uilabel3.text += Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("竞价已结束")
					});
				}
				else if (0 < nextStateTimeData && 5 > nextStateTimeData)
				{
					string text = string.Empty;
					if (nextStateTimeData == 1)
					{
						text = Global.GetLang("距第一轮竞价结束：");
					}
					else if (nextStateTimeData == 2)
					{
						text = Global.GetLang("距第二轮竞价结束：");
					}
					else if (nextStateTimeData == 3)
					{
						text = Global.GetLang("距第三轮竞价结束：");
					}
					else
					{
						text = Global.GetLang("距第四轮竞价结束：");
					}
					this.mTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						text
					});
					int sec3 = (int)(minValue - correctDateTime).TotalSeconds;
					UILabel uilabel4 = this.mTimeLabel;
					uilabel4.text += Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetTimeStrBySecFilterZero(sec3, true, 2)
					});
				}
				else
				{
					this.mTimeLabel.text = string.Empty;
					UILabel uilabel5 = this.mTimeLabel;
					uilabel5.text += Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("竞价已开始")
					});
				}
			}
			else if (this.mKuaFuPlunderGameStateType == 2)
			{
				int sec4 = (int)(minValue - correctDateTime).TotalSeconds;
				this.mTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("距掠夺开始：")
				});
				UILabel uilabel6 = this.mTimeLabel;
				uilabel6.text += Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetTimeStrBySecFilterZero(sec4, true, -1)
				});
			}
			else
			{
				this.mTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("结算中")
				});
				if (this.mSynchronousData == null)
				{
					this.mSynchronousData = new KuaFuPlunderPart.SynchronousData((this.mKuaFuLueDuoStateData != null) ? this.mKuaFuLueDuoStateData.GameState : -1);
					this.mSynchronousData.SenderGetData();
				}
			}
		}
		else
		{
			this.mTimeLabel.text = string.Empty;
			if (this.mSynchronousData == null)
			{
				this.mSynchronousData = new KuaFuPlunderPart.SynchronousData((this.mKuaFuLueDuoStateData != null) ? this.mKuaFuLueDuoStateData.GameState : -1);
				this.mSynchronousData.SenderGetData();
			}
		}
		if (this.mSynchronousData != null && this.mSynchronousData.UpDate())
		{
			this.mSynchronousData = null;
		}
	}

	private void StopTimeTicks()
	{
		if (this.mDispatcherTimer != null)
		{
			this.mDispatcherTimer.Stop();
			this.mDispatcherTimer.Dispose();
			this.mDispatcherTimer = null;
		}
	}

	private void StartTimeTicks()
	{
		this.StopTimeTicks();
		this.mDispatcherTimer = null;
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlubderPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
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

	private void RefreshTimeInf()
	{
		string text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("活动时间：")
		});
		CrusadeWarVO.TimePoint matchTime = this.mCrusadeWarXml.GetCrusadeWarVOByID(1).MatchTime;
		int week = matchTime.Week;
		text += Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetWeekNums(week) + Global.GetLang("：")
		});
		string text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			this.PrintfValue(matchTime.ToDateTime1().Hour),
			":",
			this.PrintfValue(matchTime.ToDateTime1().Minute),
			"-"
		});
		text = text + this.PrintfValue(matchTime.ToDateTime2().Hour) + ":" + this.PrintfValue(matchTime.ToDateTime2().Minute);
		this.mOpenTimeLabel.text = text;
	}

	public string NumToYNDday(int index)
	{
		if (index == 7)
		{
			index = 0;
		}
		string[] array = new string[]
		{
			Global.GetLang("越南星期日"),
			Global.GetLang("越南星期一"),
			Global.GetLang("越南星期二"),
			Global.GetLang("越南星期三"),
			Global.GetLang("越南星期四"),
			Global.GetLang("越南星期五"),
			Global.GetLang("越南星期六")
		};
		return array[index];
	}

	private string PrintfValue(int Value)
	{
		if (10 > Value)
		{
			return "0" + Value.ToString();
		}
		return Value.ToString();
	}

	private void RefreshBtnsText()
	{
		if (this.mKuaFuPlunderGameStateType == null)
		{
			this.ChangeBtnState(this.mBiddingBtn, true, Global.GetLang("掠夺地图"));
		}
		else if (this.mKuaFuPlunderGameStateType == 3)
		{
			this.ChangeBtnState(this.mBiddingBtn, true, Global.GetLang("参与战斗"));
		}
		else if (this.mKuaFuPlunderGameStateType == 1)
		{
			if (!this.IsBangHuiLeader())
			{
				this.ChangeBtnState(this.mBiddingBtn, true, Global.GetLang("掠夺地图"));
			}
			else
			{
				this.ChangeBtnState(this.mBiddingBtn, true, Global.GetLang("参与竞价"));
			}
		}
		else if (this.mKuaFuPlunderGameStateType == 2)
		{
			this.ChangeBtnState(this.mBiddingBtn, true, Global.GetLang("参与战斗"));
		}
		else
		{
			this.ChangeBtnState(this.mBiddingBtn, true, Global.GetLang("掠夺地图"));
		}
	}

	public void NoticePluderRankCallBackEX(KuaFuLueDuoRankListCmdData data)
	{
		if (null != this.mKuaFuPlunderRankPart)
		{
			if (this.mKuaFuLueDuoStateData != null)
			{
				this.mKuaFuPlunderRankPart.KuaFuLueDuoStateData = this.mKuaFuLueDuoStateData;
			}
			this.mKuaFuPlunderRankPart.NoticeGetRankDataCallback(data);
		}
	}

	public void NocticeKuaFuPlunderStateCallBack(KuaFuLueDuoStateData State)
	{
		bool flag = false;
		if (State != null)
		{
			if (this.mKuaFuLueDuoStateData != null)
			{
				if (State.Age != this.mKuaFuLueDuoStateData.Age)
				{
					flag = true;
					this.mKuaFuLueDuoStateData = State;
				}
			}
			else
			{
				flag = true;
				this.mKuaFuLueDuoStateData = State;
			}
		}
		this.mKuaFuPlunderGameStateType = State.GameState;
		this.RefreshBtnsText();
		if (flag)
		{
			this.StartTimeTicks();
			this.RefreshTimeInf();
			if (null != this.mKuaFuPlunderRankPart && this.mKuaFuLueDuoStateData != null)
			{
				this.mKuaFuPlunderRankPart.KuaFuLueDuoStateData = this.mKuaFuLueDuoStateData;
			}
		}
		if (this.mKuaFuLueDuoStateData.AwardsDataList != null && 0 <= this.mKuaFuLueDuoStateData.AwardsDataList.Count)
		{
			this.mGetAwardBtn.gameObject.SetActive(true);
		}
		else
		{
			this.mGetAwardBtn.gameObject.SetActive(false);
		}
	}

	public void NoticeGetAwardCallBack(int ret)
	{
		GameInstance.Game.SendGetKuFuPlubderGameStateData((long)((this.mKuaFuLueDuoStateData != null) ? this.mKuaFuLueDuoStateData.GameState : -1));
	}

	private const long mOneHourEqualSecond = 3600L;

	private const long mOneMinteEqualSecond = 60L;

	private const long mOneSecondEqualTicks = 10000000L;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UILabel mTimeLabel;

	[SerializeField]
	private UILabel mOpenTimeLabel;

	[SerializeField]
	private GButton mPlunderHallBtn;

	[SerializeField]
	private GButton mAwakenShopBtn;

	[SerializeField]
	private GButton mShiPinBtn;

	[SerializeField]
	private GButton mGetAwardBtn;

	[SerializeField]
	private GButton mRuleBtn;

	[SerializeField]
	private GButton mBiddingBtn;

	[SerializeField]
	private ShowNetImage mIamgeBGTexture;

	[SerializeField]
	private ShowNetImage mIamgeBGTexture1;

	[SerializeField]
	private Transform mAwardGoodsListRoot;

	private DispatcherTimer mDispatcherTimer;

	private CrusadeWarXml mCrusadeWarXml;

	private KuaFuLueDuoGameStates mKuaFuPlunderGameStateType;

	private KuaFuLueDuoStateData mKuaFuLueDuoStateData;

	private KuaFuPlunderPart.SynchronousData mSynchronousData;

	private KuaFuPlunderRankPart mKuaFuPlunderRankPart;

	private GChildWindow mKuaFuPlunderRankPartWind;

	private KuafuPlunderRulePart mKuafuPlunderRulePart;

	private GChildWindow mKuafuPlunderRulePartWind;

	public DPSelectedItemEventHandler Hander;

	private class SynchronousData
	{
		public SynchronousData(int age = -1)
		{
			this.Synchronous = true;
		}

		public bool Synchronous
		{
			set
			{
				if (!this.mSynchronous)
				{
					this.mSynchronous = value;
					this.mSynchronousTime = 3;
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

		public void SenderGetData()
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + Global.GetLang("SenderGetData   想读物器同步状态") + "</color>"
			});
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
		}

		private int mSynchronousTime;

		private bool mSynchronous;
	}
}
