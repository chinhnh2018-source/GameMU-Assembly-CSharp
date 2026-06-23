using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiMiDongMainPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.StartUITimer();
		this.mTimeActionList.Add(delegate()
		{
			this.TimeTicks();
		});
		this.InitAwardShowGoods();
		this.InitTime();
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendShiLiGetCompMiDongStates();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.StopTimer();
	}

	private void StartUITimer()
	{
		this.mUITimer = new DispatcherTimer("ShiLiMiDongMainPart");
		this.mUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mUITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.mUITimer.Start();
	}

	private void UITimer_Tick(object sender, EventArgs args)
	{
		if (0 < this.mTimeActionList.Count)
		{
			for (int i = 0; i < this.mTimeActionList.Count; i++)
			{
				if (this.mTimeActionList[i] != null)
				{
					this.mTimeActionList[i].Invoke();
				}
			}
		}
	}

	private void StopTimer()
	{
		if (this.mUITimer != null)
		{
			this.mUITimer.Tick = null;
			this.mUITimer.Stop();
			this.mUITimer.Dispose();
			this.mUITimer = null;
		}
	}

	private void TimeTicks()
	{
		CompMineGameStates compMineGameStates = CompMineGameStates.None;
		CompMineWarVO.TimePoint activityTimeEX = IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineWarVOByID(1).ActivityTimeEX;
		if (activityTimeEX != null)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime dateTime = activityTimeEX.ToDateTime1();
			DateTime dateTime2 = activityTimeEX.ToDateTime2();
			if (this.mCompBattleGameStates == CompMineGameStates.Start)
			{
				if (correctDateTime >= dateTime && correctDateTime < dateTime2)
				{
					int sec = (int)(dateTime2 - correctDateTime).TotalSeconds;
					this._TimeOfBeginLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("距活动结束")
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec, true, 2)
					});
				}
				else
				{
					if (8 > this.CheckInterval)
					{
						this.CheckInterval++;
					}
					else
					{
						this.CheckInterval = 0;
					}
					this._TimeOfBeginLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("活动已开始")
					});
				}
			}
			else if (this.mCompBattleGameStates == CompMineGameStates.Analysis)
			{
				this._TimeOfBeginLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffc705",
					Global.GetLang("结算中")
				});
			}
			else if (correctDateTime >= dateTime && correctDateTime < dateTime2)
			{
				compMineGameStates = CompMineGameStates.Start;
				int sec2 = (int)(dateTime2 - correctDateTime).TotalSeconds;
				this._TimeOfBeginLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("距活动结束")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec2, true, 2)
				});
			}
			else
			{
				if (correctDateTime >= dateTime2)
				{
					dateTime = activityTimeEX.ToDateTime1(7.0);
				}
				int sec3;
				if (correctDateTime > dateTime)
				{
					sec3 = (int)(correctDateTime - dateTime).TotalSeconds;
				}
				else
				{
					sec3 = (int)(dateTime - correctDateTime).TotalSeconds;
				}
				this._TimeOfBeginLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("距活动开始")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec3, true, 2)
				});
			}
		}
		byte b = 0;
		if (this.mCompBattleGameStates == CompMineGameStates.Start)
		{
			b = 1;
		}
		else if (compMineGameStates == CompMineGameStates.Start)
		{
			b = 1;
		}
		BoxCollider component = this._JoinFightBtn.GetComponent<BoxCollider>();
		if (null != component)
		{
			component.enabled = (1 == b);
		}
		UISprite componentInChildren = this._JoinFightBtn.GetComponentInChildren<UISprite>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = (1 == b);
		}
		UILabel componentInChildren2 = this._JoinFightBtn.GetComponentInChildren<UILabel>();
		if (null != componentInChildren2)
		{
			componentInChildren2.text = ((b != 1) ? Global.GetLang("活动未开始") : Global.GetLang("参与战斗"));
		}
		if (this.CheckInterval == 1)
		{
			GameInstance.Game.SendShiLiGetCompMiDongStates();
		}
	}

	private void InitPrefabText()
	{
		try
		{
			this._JoinFightBtn.Label.text = Global.GetLang("参与战斗");
			this._AwardBtn.Label.text = Global.GetLang("奖励预览");
			this._TimeOfBeginLabel.text = string.Empty;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this._HelpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._HelpBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._HelpBtn.GetInstanceID(), 1f);
				this.OpenHelpWindow();
			};
			this._JoinFightBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._HelpBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._HelpBtn.GetInstanceID(), 1f);
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 2,
						Type = 2
					});
				}
			};
			this._AwardBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._HelpBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._HelpBtn.GetInstanceID(), 1f);
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 2,
						ID = 1
					});
				}
			};
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitAwardShowGoods()
	{
		List<int> showGoodsByGoodsID = IConfigbase<ConfigShiLiMiDong>.Instance.GetShowGoodsByGoodsID(1);
		if (0 < showGoodsByGoodsID.Count)
		{
			for (int i = 0; i < showGoodsByGoodsID.Count; i++)
			{
				GoodsData dummyGoodsData = Global.GetDummyGoodsData(showGoodsByGoodsID[i]);
				dummyGoodsData.GCount = 1;
				GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(dummyGoodsData, true);
				if (null != ggoodIcon)
				{
					ggoodIcon.transform.SetParent(this._GoodsRoot);
					ggoodIcon.transform.localPosition = new Vector3(0f + (float)i * 88f, 0f, 0f);
				}
			}
		}
	}

	private void InitTime()
	{
		string text = Global.GetColorStringForNGUIText(new object[]
		{
			"9a7a50",
			Global.GetLang("活动时间：")
		});
		CompMineWarVO.TimePoint activityTimeEX = IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineWarVOByID(1).ActivityTimeEX;
		text += Global.GetColorStringForNGUIText(new object[]
		{
			"9a7a50",
			Global.GetWeekNums(activityTimeEX.Week) + Global.GetLang("：")
		});
		string text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			this.PrintfValue(activityTimeEX.ToDateTime1().Hour),
			":",
			this.PrintfValue(activityTimeEX.ToDateTime1().Minute),
			"-"
		});
		text = text + this.PrintfValue(activityTimeEX.ToDateTime2().Hour) + ":" + this.PrintfValue(activityTimeEX.ToDateTime2().Minute);
		this._TimeOfActivityOpenLabel.text = text;
	}

	private string PrintfValue(int Value)
	{
		if (10 > Value)
		{
			return "0" + Value.ToString();
		}
		return Value.ToString();
	}

	private void OpenHelpWindow()
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		this.m_helpPart.SetHelpInfo(IConfigbase<ConfigShiLiMiDong>.Instance.GetRule());
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	internal void NoticeGetStatesCallBask(int states)
	{
		this.mCompBattleGameStates = (CompMineGameStates)states;
		if (this.mCompBattleGameStates == CompMineGameStates.Awards)
		{
			Transform transform = this._AwardBtn.transform.FindChild("TipIconTop");
			if (null != transform)
			{
				transform.gameObject.SetActive(true);
			}
			this._AwardBtn.Label.text = Global.GetLang("领取奖励");
		}
		else
		{
			UISprite component = this._AwardBtn.transform.GetChild(1).GetComponent<UISprite>();
			BoxCollider component2 = this._AwardBtn.GetComponent<BoxCollider>();
			if (this.mCompBattleGameStates == CompMineGameStates.Analysis)
			{
				component.enabled = false;
				component2.enabled = false;
				this._AwardBtn.Text = Global.GetLang("结算中");
			}
			else
			{
				component.enabled = true;
				component2.enabled = true;
				Transform transform2 = this._AwardBtn.transform.FindChild("TipIconTop");
				if (null != transform2)
				{
					transform2.gameObject.SetActive(false);
				}
				this._AwardBtn.Label.text = Global.GetLang("奖励预览");
			}
		}
		if (0 < this.mTimeActionList.Count)
		{
			for (int i = 0; i < this.mTimeActionList.Count; i++)
			{
				if (this.mTimeActionList[i] != null)
				{
					this.mTimeActionList[i].Invoke();
				}
			}
		}
	}

	public void RefreshPartData()
	{
	}

	public int CrusadeWarID
	{
		get
		{
			return this.mCrusadeWarID;
		}
		private set
		{
			this.mCrusadeWarID = value;
		}
	}

	public const int UseCompMineWarID = 1;

	private const string TimeColor1 = "9a7a50";

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton _HelpBtn;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GButton _AwardBtn;

	[SerializeField]
	private GButton _JoinFightBtn;

	[SerializeField]
	private UILabel _TimeOfBeginLabel;

	[SerializeField]
	private UILabel _TimeOfActivityOpenLabel;

	[SerializeField]
	private ShowNetImage _BakOneImage;

	[SerializeField]
	private ShowNetImage _BgOneImage;

	[SerializeField]
	private Transform _GoodsRoot;

	private DispatcherTimer mUITimer;

	private List<Action> mTimeActionList = new List<Action>();

	private int mCrusadeWarID;

	private CompMineGameStates mCompBattleGameStates;

	private int CheckInterval;

	protected GChildWindow m_helpWindow;

	protected CommonHelpWindow m_helpPart;
}
