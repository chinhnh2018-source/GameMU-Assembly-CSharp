using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class RelifePart : UserControl
{
	public RelifePart(int type, int seconds = 0)
	{
		this._RelifeType = (RoleReliveTypes)type;
		this._TimeToCountdown = seconds * 1000;
		this.thisCtrl = this;
	}

	private void InitTextInPrefabs()
	{
		this.m_BtnYuanDiFuHuo.Text = Global.GetLang("原地满血复活");
		this.m_BtnHuiChengFuHuo.Text = Global.GetLang("回城安全复活");
		this.m_BtnAnQuanFuhuo2.Text = Global.GetLang("立即复活");
		this.mBtnKuaFuPlunderLeave.Text = Global.GetLang("离开战场");
		this.mBtnKuaFuPlunderRelife.Text = Global.GetLang("立刻复活");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.m_FuHuoBak1)
		{
			this.m_FuHuoBak1.localScale = Super.GetScreenSize();
		}
		if (this.m_FuHuoBak2)
		{
			this.m_FuHuoBak2.localScale = Super.GetScreenSize();
		}
		if (this.m_FuHuoBak3)
		{
			this.m_FuHuoBak3.localScale = Super.GetScreenSize();
		}
		if (null != Global.MainCamera)
		{
			this.m_CanveGraySet = Global.MainCamera.GetComponent<ColorCorrectionCurves>();
			if (null != this.m_CanveGraySet)
			{
				this.m_nGrayColorValue = this.m_CanveGraySet.saturation;
				this.m_CanveGraySet.saturation = 0f;
				this.m_bGrayColor = this.m_CanveGraySet.enabled;
				this.m_CanveGraySet.enabled = true;
			}
		}
		if (null != this.m_BtnAnQuanFuhuo2)
		{
			this.m_BtnAnQuanFuhuo2.Text = Global.GetLang("立即复活");
		}
	}

	public void InitContrl(int nType = 0, int nSec = 0, int nVipReLifeNum = 0)
	{
		this.m_TimeCount = nSec;
		this.PlunderFuHuoTimeSec = nSec;
		this.GetAttackName();
		this.SetBtnProc(nType);
		this.ShowFuHuoWindowByType(nType, nVipReLifeNum);
		if (null != this.m_LblShowTitle)
		{
			if (nSec == 0)
			{
				nSec = 1;
			}
			this.m_TimeCount = nSec;
			this.GetShowTitle();
			this.RelifeType = nType;
			if (nType == 0 || nType == 2 || nType == 3 || nType == 4)
			{
				this.StartHeart();
			}
			else
			{
				if (nType == 6)
				{
					this.ShowInfoBySec(false, -1);
					this.StartTicks = Global.GetCorrectLocalTime();
					base.InvokeRepeating("TickProc", 0f, 0.5f);
					return;
				}
				if (nType == 7)
				{
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CrusadeEnterTime", ',');
					this.mLblKuaFuPlunderRelifeCount.Text = Global.GetLang(string.Format("{0}{1}/{2}{3}", new object[]
					{
						Global.GetLang("剩余参与次数："),
						Global.Data.roleData.MoneyData[134],
						systemParamIntArrayByName[1],
						Global.GetLang("次")
					}));
					this.StartTicks1 = Global.GetCorrectLocalTime();
					base.InvokeRepeating("KuaFuTickProc", 0f, 1f);
					return;
				}
			}
		}
		if (null != this.m_LblBottom)
		{
			this.m_LblBottom.text = string.Format(Global.GetLang("{{ff0000}}{0}{{-}}秒后自动回安全区复活"), nSec);
		}
	}

	private void KuaFuTickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime > this.StartTicks1)
		{
			int num = (int)((correctLocalTime - this.StartTicks1) / 1000L);
			if (num >= this.PlunderFuHuoTimeSec)
			{
				base.CancelInvoke("KuaFuTickProc");
				this.ShowInfoBySec1(true, 0);
			}
			else
			{
				this.ShowInfoBySec1(false, this.PlunderFuHuoTimeSec - num);
			}
		}
	}

	private void ShowInfoBySec1(bool isEnable, int sec)
	{
		if (isEnable)
		{
			if (Global.Data.roleData.MoneyData[134] <= 0L)
			{
				Global.Data.GameScene.RelifeOnOriginalPoint();
			}
			else
			{
				Global.Data.GameScene.RelifeOnLeaderPoint();
			}
		}
		else
		{
			this.m_LblInfo1.text = string.Format(Global.GetLang("{0}秒后自动回归属阵营出生点复活"), Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Convert.ToString(sec)
			}));
		}
	}

	private void GetAttackName()
	{
		if (Global.Data.nAttackRoleID == Global.Data.roleData.RoleID && Global.Data.roleData.MapCode == 85001)
		{
			this.m_strAttackName = Global.GetLang("神罚闪电");
			this.m_nAttackNameColor = ColorSL.FromArgb(255, 255, 0, 0);
		}
		else
		{
			GSprite gsprite = Global.FindSprite(StringUtil.substitute("Role_{0}", new object[]
			{
				Global.Data.nAttackRoleID
			}));
			if (gsprite != null)
			{
				this.m_strAttackName = gsprite.VSName;
				SolidColorBrush snameBrush = gsprite.SNameBrush;
				this.m_nAttackNameColor = ColorSL.FromArgb(255, 255, 0, 0);
			}
			else
			{
				this.m_strAttackName = Global.Data.strAttackName;
			}
		}
	}

	private void SetBtnProc(int nType)
	{
		if (null != this.m_BtnYuanDiFuHuo)
		{
			this.m_BtnYuanDiFuHuo.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.YuDiFuhuo();
			};
		}
		if (null != this.m_BtnHuiChengFuHuo)
		{
			this.m_BtnHuiChengFuHuo.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.HuiChengFuhuo();
			};
		}
		if (null != this.m_BtnAnQuanFuhuo2)
		{
			this.m_BtnAnQuanFuhuo2.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.LiJiFuhuo();
			};
		}
		this.mBtnKuaFuPlunderLeave.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
		};
		this.mBtnKuaFuPlunderRelife.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CrusadeEnterTime", ',');
			if (Global.Data.roleData.MoneyData[134] <= 0L)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1513,
					ShowFlagUpdate = true
				});
			}
			else
			{
				Global.Data.GameScene.RelifeOnLeaderPoint();
			}
		};
	}

	private void GetShowTitle()
	{
		if (null != this.m_LblShowTitle)
		{
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				NGUIMath.DecimalToHex((int)this.m_nAttackNameColor),
				this.m_strAttackName
			});
			this.m_LblShowTitle.text = string.Format("{0}", colorStringForNGUIText);
		}
	}

	private void YuDiFuhuo()
	{
		if (Global.Data.roleData.UserMoney >= 10 || Global.GetGoodsDataByID(Global.Data.AliveGoodsID) != null || this.m_nUseVipReLifeNum < this.m_nVipReLifeNum)
		{
			Global.Data.GameScene.RelifeOnLeaderPoint();
			this.m_DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 4
			});
		}
		else
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.AliveGoodsID);
			if (goodsXmlNodeByID != null)
			{
				string title = goodsXmlNodeByID.Title;
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("原地复活需要背包中有【{0}】或10钻石"), new object[]
				{
					title
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有配置原地复活需要的道具"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	private void HuiChengFuhuo()
	{
		Global.Data.GameScene.RelifeOnOriginalPoint();
	}

	private void LiJiFuhuo()
	{
		Global.Data.GameScene.RelifeOnOriginalPoint();
	}

	private void ShowFuHuoWindowByType(int nType, int nVipReLifeNum)
	{
		this.m_BtnYuanDiFuHuo.isEnabled = true;
		if (nType == 0 || nType == 6)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("VIPResurrectionAdd", ',');
			int num = 0;
			if (systemParamIntArrayByName != null)
			{
				num = systemParamIntArrayByName[Global.Data.roleData.VIPLevel];
			}
			if (0 < Global.Data.roleData.VIPLevel && nVipReLifeNum < num)
			{
				this.m_nVipReLifeNum = num;
				this.m_nUseVipReLifeNum = nVipReLifeNum;
				if (null != this.m_LblYuanDi)
				{
					this.m_LblYuanDi.text = string.Format(Global.GetLang("VIP免费次数（{0}/{1}）"), nVipReLifeNum, num);
				}
			}
			else if (Global.GetGoodsDataByID(Global.Data.AliveGoodsID) != null)
			{
				if (null != this.m_LblYuanDi)
				{
					SolidColorBrush goodsColor = Global.GetGoodsColor(Global.Data.AliveGoodsID);
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
					{
						NGUIMath.DecimalToHex((int)goodsColor.Color),
						Global.GetLang("【复活石】")
					});
					this.m_LblYuanDi.text = Global.GetLang("消耗") + colorStringForNGUIText + "x 1";
				}
			}
			else if (null != this.m_LblYuanDi)
			{
				if (Global.Data.roleData.UserMoney >= 10)
				{
					this.m_LblYuanDi.text = string.Format(Global.GetLang("消耗【钻石】x 10"), new object[0]);
				}
				else
				{
					this.m_BtnYuanDiFuHuo.isEnabled = false;
					this.m_LblYuanDi.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("消耗【钻石】x 10")
					});
				}
			}
		}
		if (nType == 0)
		{
			this.ShowWindow(0);
		}
		else if (nType == 1 || nType == 2)
		{
			this.ShowWindow(1);
			if (nType == 2)
			{
				this.m_LblInfo1.gameObject.SetActive(true);
				this.m_LblInfo1.text = string.Format(Global.GetLang("{0}秒后自动回归属阵营出生点复活"), Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Convert.ToString(this.m_TimeCount)
				}));
			}
		}
		else if (nType == 4)
		{
			this.ShowWindow(2);
			this.m_LblInfo2_1.text = string.Format(Global.GetLang("复活倒计时: {0}秒"), ColorCode.EncodingText(this.m_TimeCount, "fd010c"));
			this.AngelTempleZuanshiRevive = ConfigSystemParam.GetSystemParamIntByName("AngelTempleZuanshiRevive");
			this.m_LblInfo2_2.text = string.Format(Global.GetLang("可使用{0}钻石立即复活"), ColorCode.EncodingText(this.AngelTempleZuanshiRevive.ToString(), "00ff00"));
		}
		else if (nType == 7)
		{
			this.ShowWindow(1);
			this.mKuaFuPlunderObj.SetActive(true);
			this.m_LblInfo1.gameObject.SetActive(true);
			this.m_LblInfo2.text = this.m_strAttackName;
			this.m_LblInfo1.text = string.Format(Global.GetLang("{0}秒后自动回归属阵营出生点复活"), Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				this.PlunderFuHuoTimeSec
			}));
		}
	}

	private void ShowWindow(int index)
	{
		this.m_FuHuoWindow.gameObject.SetActive(index == 0);
		this.m_FuHuoWindow1.gameObject.SetActive(index == 1);
		this.m_FuHuoWindow2.gameObject.SetActive(index == 2);
	}

	public void CloseWindow()
	{
		this.StopHeart();
		base.CancelInvoke("KuaFuTickProc");
		this.m_DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = 10
		});
	}

	public void StartHeart()
	{
		this._Timer = new DispatcherTimer("RelifePart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(1000.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.RelifePartTimer_Tick);
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this.m_CanveGraySet != null)
		{
			this.m_CanveGraySet.saturation = this.m_nGrayColorValue;
			this.m_CanveGraySet.enabled = this.m_bGrayColor;
		}
		this.m_strAttackName = string.Empty;
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
		this.CloseWindow();
	}

	public void RelifePartTimer_Tick(object sender, EventArgs e)
	{
		this.m_TimeCount--;
		this.m_LblInfo2_1.text = string.Format(Global.GetLang("复活倒计时: {0}秒"), ColorCode.EncodingText(this.m_TimeCount, "fd010c"));
		if (null != this.m_LblShowTitle)
		{
			this.GetShowTitle();
		}
		int num = this.m_TimeCount;
		if (num < 0)
		{
			num = 0;
		}
		if (null != this.m_LblBottom)
		{
			this.m_LblBottom.text = string.Format(Global.GetLang("{{ff0000}}{0}{{-}}秒后自动回安全区复活"), num);
		}
		if (null != this.m_LblInfo1)
		{
			this.m_LblInfo1.text = string.Format(Global.GetLang("{0}秒后自动回归属阵营出生点复活"), Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Convert.ToString(num)
			}));
		}
		if (null != this.m_LblInfo2)
		{
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				NGUIMath.DecimalToHex((int)this.m_nAttackNameColor),
				this.m_strAttackName
			});
			this.m_LblInfo2.text = string.Format("{0}", colorStringForNGUIText);
		}
		if (0 >= this.m_TimeCount)
		{
			if (this.m_TimeCount == 0)
			{
				Global.Data.GameScene.RelifeOnOriginalPoint();
			}
			else if (Math.Abs(this.m_TimeCount) % 5 == 0)
			{
				Global.Data.GameScene.RelifeOnOriginalPoint();
			}
		}
	}

	private void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime > this.StartTicks)
		{
			int num = (int)((correctLocalTime - this.StartTicks) / 1000L);
			if (num >= 15)
			{
				base.CancelInvoke("TickProc");
				this.ShowInfoBySec(true, 0);
			}
			else
			{
				this.ShowInfoBySec(false, 15 - num);
			}
		}
	}

	private void ShowInfoBySec(bool isEnable, int sec)
	{
		if (isEnable)
		{
			this.m_BtnYuanDiFuHuo.isEnabled = true;
			this.m_BtnHuiChengFuHuo.isEnabled = true;
			if (null != this.m_LblBottom)
			{
				this.m_LblBottom.text = string.Empty;
			}
			if (this.RelifeType == 6)
			{
				this.StartHeart();
			}
		}
		else
		{
			this.m_BtnYuanDiFuHuo.isEnabled = false;
			this.m_BtnHuiChengFuHuo.isEnabled = false;
			if (null != this.m_LblBottom && sec >= 0)
			{
				this.m_LblBottom.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					sec.ToString(),
					"ffffff",
					Global.GetLang("秒后才可执行复活操作")
				});
			}
		}
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public ImageBrush BodyBground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	private void InitControls()
	{
		if (this._RelifeType == RoleReliveTypes.HomeOrHere)
		{
			this.InitControlsForHomeOrHere();
		}
		else if (this._RelifeType == RoleReliveTypes.Home)
		{
			this.InitControlsForHome();
		}
		else if (this._RelifeType == RoleReliveTypes.TimeWaiting)
		{
			this.InitControlsForTimeWaiting();
		}
		else if (this._RelifeType == RoleReliveTypes.TimeWaitingRandomAlive)
		{
			this.InitControlsForTimeWaitingRandomAlive();
		}
		else if (this._RelifeType == RoleReliveTypes.TimeWaitingOrRelifeNow)
		{
			this.AngelTempleZuanshiRevive = ConfigSystemParam.GetSystemParamIntByName("AngelTempleZuanshiRevive");
			this.m_LblInfo2_2.text = string.Format(Global.GetLang("可使用{0}钻石立即复活"), ColorCode.EncodingText(this.AngelTempleZuanshiRevive.ToString(), "00ff00"));
		}
	}

	private void InitControlsForHomeOrHere()
	{
		if (this._RelifeType != RoleReliveTypes.HomeOrHere)
		{
			return;
		}
		double num = 0.0;
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("请选择复活方式？");
		Canvas.SetLeft(gtextBlockOutLine, 93);
		Canvas.SetTop(gtextBlockOutLine, num);
		this.Container.Children.Add(gtextBlockOutLine);
		num += 20.0;
		GIcon gicon = this.CreateHomeReliveButton();
		this.Container.Children.Add(gicon);
		Canvas.SetLeft(gicon, 100);
		Canvas.SetTop(gicon, num);
		num += 24.0;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("回城复活：立即传送到安全区复活");
		Canvas.SetLeft(gtextBlockOutLine, 46);
		Canvas.SetTop(gtextBlockOutLine, num);
		this.Container.Children.Add(gtextBlockOutLine);
		num += 20.0;
		gicon = this.CreateHereReliveButton();
		this.Container.Children.Add(gicon);
		Canvas.SetLeft(gicon, 100);
		Canvas.SetTop(gicon, num);
		num += 24.0;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("原地复活：在当死亡点立即满血复活");
		Canvas.SetLeft(gtextBlockOutLine, 46);
		Canvas.SetTop(gtextBlockOutLine, num);
		this.Container.Children.Add(gtextBlockOutLine);
		num += gtextBlockOutLine.RealSize.Height + 1.0;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.htmlText = Global.GetColorStringForHtmlText(new object[]
		{
			"#FFFFFFFF",
			Global.GetLang("( 消耗"),
			"#FFFFFF00",
			Global.GetLang("100钻石"),
			"#FFFFFFFF",
			Global.GetLang("或1个"),
			"#FF00ff00",
			Global.GetLang("【九转还魂丹】"),
			"#FFFFFFFF",
			Global.GetLang(")")
		});
		Canvas.SetLeft(gtextBlockOutLine, 46);
		Canvas.SetTop(gtextBlockOutLine, num);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private void InitControlsForHome()
	{
		if (this._RelifeType != RoleReliveTypes.Home)
		{
			return;
		}
		GIcon gicon = this.CreateHomeReliveButton();
		Canvas.SetLeft(gicon, 100);
		Canvas.SetTop(gicon, 20);
		this.Container.Children.Add(gicon);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("回城复活：立即传送到安全区复活");
		Canvas.SetLeft(gtextBlockOutLine, 46);
		Canvas.SetTop(gtextBlockOutLine, 55);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private void InitControlsForTimeWaiting()
	{
		if (this._RelifeType != RoleReliveTypes.TimeWaiting)
		{
			return;
		}
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		Canvas.SetLeft(gtextBlockOutLine, 81);
		Canvas.SetTop(gtextBlockOutLine, 20);
		this.Container.Children.Add(gtextBlockOutLine);
		this.AutoGoBackTextBlock = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		if (this._RelifeType == RoleReliveTypes.TimeWaiting)
		{
			gtextBlockOutLine.Text = Global.GetLang("倒计时结束后在所属阵营安全区复活!");
		}
		else if (this._RelifeType == RoleReliveTypes.TimeWaitingRandomAlive)
		{
			gtextBlockOutLine.Text = Global.GetLang("倒计时结束后随机复活!");
		}
		Canvas.SetLeft(gtextBlockOutLine, 41);
		Canvas.SetTop(gtextBlockOutLine, 45);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private void InitControlsForTimeWaitingRandomAlive()
	{
		if (this._RelifeType != RoleReliveTypes.TimeWaitingRandomAlive)
		{
			return;
		}
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		Canvas.SetLeft(gtextBlockOutLine, 81);
		Canvas.SetTop(gtextBlockOutLine, 20);
		this.Container.Children.Add(gtextBlockOutLine);
		this.AutoGoBackTextBlock = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(uint.MaxValue);
		if (this._RelifeType == RoleReliveTypes.TimeWaitingRandomAlive)
		{
			gtextBlockOutLine.Text = Global.GetLang("倒计时结束后随机复活!");
		}
		Canvas.SetLeft(gtextBlockOutLine, 41);
		Canvas.SetTop(gtextBlockOutLine, 45);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private GIcon CreateHomeReliveButton()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("回城复活");
		gicon.MouseLeftButtonDown = delegate(object sender, MouseEvent e)
		{
		};
		gicon.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Global.Data.GameScene.RelifeOnOriginalPoint();
		};
		Canvas.SetLeft(gicon, 167);
		Canvas.SetTop(gicon, 70);
		return gicon;
	}

	private GIcon CreateHereReliveButton()
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("原地复活");
		gicon.MouseLeftButtonDown = delegate(object sender, MouseEvent e)
		{
		};
		gicon.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (Global.Data.roleData.UserMoney >= 10 || Global.GetGoodsDataByID(Global.Data.AliveGoodsID) != null)
			{
				Global.Data.GameScene.RelifeOnLeaderPoint();
				if (this.ToClose != null)
				{
					this.ToClose.Invoke(this.thisCtrl, EventArgs.Empty);
				}
			}
			else
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.AliveGoodsID);
				if (goodsXmlNodeByID != null)
				{
					string title = goodsXmlNodeByID.Title;
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("原地复活需要背包中有【{0}】或10钻石"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有配置原地复活需要的道具"), new object[0]), 0, -1, -1, 0);
				}
			}
		};
		Canvas.SetLeft(gicon, 75);
		Canvas.SetTop(gicon, 70);
		return gicon;
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("KuaFuTickProc");
		base.OnDestroy();
	}

	private new void Update()
	{
		if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.LifeV > 0 && this.m_FuHuoWindow != null)
		{
			this.StopHeart();
			Object.Destroy(base.gameObject);
			PlayZone.GlobalPlayZone.m_FuhuoPanle = null;
		}
	}

	private const int TimeSec = 15;

	public GButton m_BtnYuanDiFuHuo;

	public GButton m_BtnHuiChengFuHuo;

	public UILabel m_LblInfo1;

	public UILabel m_LblInfo2;

	public UILabel m_LblShowTitle;

	public UILabel m_LblBottom;

	public UILabel m_LblYuanDi;

	public GButton m_BtnAnQuanFuhuo2;

	public UILabel m_LblInfo2_1;

	public UILabel m_LblInfo2_2;

	public DPSelectedItemBoolEventHandler m_DPSelectedItem;

	public int m_TimeCount;

	private string m_strAttackName = string.Empty;

	private uint m_nAttackNameColor;

	public Transform m_FuHuoWindow;

	public Transform m_FuHuoWindow1;

	public Transform m_FuHuoWindow2;

	public ColorCorrectionCurves m_CanveGraySet;

	public int m_nUseVipReLifeNum;

	public int m_nVipReLifeNum;

	private float m_nGrayColorValue;

	private bool m_bGrayColor;

	private long AngelTempleZuanshiRevive = 20L;

	private int RelifeType;

	public GameObject mKuaFuPlunderObj;

	public TextBlock mLblKuaFuPlunderRelifeCount;

	public GButton mBtnKuaFuPlunderLeave;

	public GButton mBtnKuaFuPlunderRelife;

	public Transform m_FuHuoBak1;

	public Transform m_FuHuoBak2;

	public Transform m_FuHuoBak3;

	private long StartTicks1;

	private int PlunderFuHuoTimeSec;

	private long StartTicks;

	private SpriteSL thisCtrl;

	public EventHandler ToClose;

	private GTextBlockOutLine AutoGoBackTextBlock;

	private DispatcherTimer _Timer;

	private RoleReliveTypes _RelifeType;

	private int _TimeToCountdown = 30000;
}
