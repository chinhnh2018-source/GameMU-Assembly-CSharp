using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using UnityEngine;

public class ShiLiSelectItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this._BakImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/weikaiqi.png";
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
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void InitNull(int type)
	{
		this._BakImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/M_mihukuangdongzhengba_weikaiqi.png";
		this._TitleLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("跨服活动")
		});
		this._StateLabel.text = string.Empty;
		this._ActivityTimeLabel.text = string.Empty;
		this._TitleSp.gameObject.SetActive(false);
	}

	public void SetActivityInf(int id, int gameStates)
	{
		this.mId = id;
		this.mGameStates = gameStates;
		this._BakImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/miwuzhengba_bg_1.png";
		if (this.mId == 1)
		{
			if (ConfigVersionSystemOpen.VersionSystemOpenHaveTheId(120402, 1) && !ConfigVersionSystemOpen.IsVersionSystemOpen(120402))
			{
				this._BakImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/weikaiqi.png";
				this._BakBgImage.ToGrayBitmap = true;
				return;
			}
			this._TitleSp.spriteName = "ItemTitle0";
			this._TitleSp.transform.localScale = new Vector3(94f, 20f, 0f);
			this._TitleLabel.text = Global.GetLang("跨服活动");
			this._BakBgImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/Image" + (this.mId - 1).ToString() + ".png";
		}
		else if (this.mId == 2)
		{
			this._TitleSp.spriteName = "ItemTitle1";
			this._TitleSp.transform.localScale = new Vector3(75f, 20f, 0f);
			this._TitleLabel.text = Global.GetLang("跨服活动");
			if (ConfigVersionSystemOpen.VersionSystemOpenHaveTheId(120403, 1) && !ConfigVersionSystemOpen.IsVersionSystemOpen(120403))
			{
				this._BakImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/weikaiqi.png";
				this._BakBgImage.ToGrayBitmap = true;
				return;
			}
			this._BakBgImage.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/Image" + (this.mId - 1).ToString() + ".png";
		}
	}

	public void RefreshTime()
	{
		if (this.mActivityTime != null)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime dateTime = this.mActivityTime.ToDateTime1();
			DateTime dateTime2 = this.mActivityTime.ToDateTime2();
			if (this.mGameStates == 1)
			{
				if (correctDateTime >= dateTime && correctDateTime < dateTime2)
				{
					int sec = (int)(dateTime2 - correctDateTime).TotalSeconds;
					this._StateLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("距活动结束")
					});
					this._ActivityTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
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
					this._StateLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("活动已开始")
					});
					this._ActivityTimeLabel.text = string.Empty;
				}
			}
			else if (this.mGameStates == 4)
			{
				this._StateLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ddd2bd",
					Global.GetLang("结算中")
				});
				this._ActivityTimeLabel.text = string.Empty;
			}
			else if (correctDateTime >= dateTime && correctDateTime < dateTime2)
			{
				int sec2 = (int)(dateTime2 - correctDateTime).TotalSeconds;
				this._StateLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("距活动结束")
				});
				this._ActivityTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec2, true, 2)
				});
			}
			else
			{
				if (correctDateTime >= dateTime2)
				{
					dateTime = this.mActivityTime.ToDateTime1(7.0);
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
				this._StateLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("距活动开始")
				});
				this._ActivityTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("剩余：") + Global.GetTimeStrBySecFilterZero(sec3, true, 2)
				});
			}
		}
		if (this.CheckInterval == 1)
		{
			if (this.mId == 2)
			{
				GameInstance.Game.SendShiLiGetCompMiDongStates();
			}
			else if (this.mId == 1)
			{
				GameInstance.Game.ShiLiGetCompBattleState();
			}
		}
	}

	public void SetActivityTime(CompMineWarVO.TimePoint time)
	{
		this.mActivityTime = time;
		this.RefreshTime();
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			UIDragPanelContents uidragPanelContents = base.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	[SerializeField]
	private ShowNetImage _BakImage;

	[SerializeField]
	private ShowNetImage _BakBgImage;

	[SerializeField]
	private UISprite _TitleSp;

	[SerializeField]
	private UILabel _TitleLabel;

	[SerializeField]
	private UILabel _StateLabel;

	[SerializeField]
	private UILabel _ActivityTimeLabel;

	private CompMineWarVO.TimePoint mActivityTime;

	private int mId = -1;

	private int mGameStates = 1;

	private int CheckInterval;
}
