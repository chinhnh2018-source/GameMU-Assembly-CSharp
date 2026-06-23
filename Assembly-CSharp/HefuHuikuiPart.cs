using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class HefuHuikuiPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.partCanvas);
		this.partCanvas.Background = new SolidColorBrush(16777215U);
		this.partCanvas.Width = 414.0;
		this.partCanvas.Height = 174.0;
		Canvas.SetLeft(this.partCanvas, 132);
		Canvas.SetTop(this.partCanvas, 193);
		int num = 10;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "HefuDalibao";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("合服大礼包");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 0;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("HefuDalibao");
		};
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "VIPDahuikui";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("VIP大回馈");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 1;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("VIPDahuikui");
		};
		num += 41;
		Canvas.SetLeft(gicon, 1);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "ChongzhiDahuikui";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("充值大回馈");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 2;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("ChongzhiDahuikui");
		};
		num += 41;
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "ChongzhiDafanli";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("充值大返利");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 3;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("ChongzhiDafanli");
		};
		num += 41;
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "HefuWangchengZhengba";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("合服王城争霸");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 4;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("HefuWangchengZhengba");
		};
		num += 41;
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "SuperBossGongcheng";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("合服BOSS攻城");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 5;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("SuperBossGongcheng");
		};
		num += 41;
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "PKWangDali";
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang("合服PK王");
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = 6;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon("PKWangDali");
		};
		num += 41;
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, num);
		this.Container.Children.Add(gicon);
		this.HuodongTitle = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongTitle.TextFontColor = new SolidColorBrush(16777080U);
		this.HuodongTitle.TextSize = 14.0;
		this.HuodongTitle.fontBold = true;
		this.HuodongTitle.Width = 125.0;
		this.HuodongTitle.Height = 25.0;
		Canvas.SetLeft(this.HuodongTitle, 295);
		Canvas.SetTop(this.HuodongTitle, 19);
		this.Container.Children.Add(this.HuodongTitle);
		this.HefuTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HefuTime.TextFontColor = new SolidColorBrush(16777215U);
		this.HefuTime.TextSize = 12.0;
		Canvas.SetLeft(this.HefuTime, 215);
		Canvas.SetTop(this.HefuTime, 61);
		this.Container.Children.Add(this.HefuTime);
		this.HuodongTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongTime.TextFontColor = new SolidColorBrush(16777215U);
		this.HuodongTime.TextSize = 12.0;
		Canvas.SetLeft(this.HuodongTime, 215);
		Canvas.SetTop(this.HuodongTime, 85);
		this.Container.Children.Add(this.HuodongTime);
		this.HuodongEndTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongEndTime.TextFontColor = new SolidColorBrush(16777215U);
		this.HuodongEndTime.TextSize = 12.0;
		Canvas.SetLeft(this.HuodongEndTime, 299);
		Canvas.SetTop(this.HuodongEndTime, 344);
		this.Container.Children.Add(this.HuodongEndTime);
		this.HuodongIntro = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongIntro.TextFontColor = new SolidColorBrush(16777215U);
		this.HuodongIntro.TextSize = 12.0;
		this.HuodongIntro.mouseEnabled = false;
		this.HuodongIntro.TextWidth = 388.0;
		this.HuodongIntro.TextFontWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.HuodongIntro, 145);
		Canvas.SetTop(this.HuodongIntro, 125);
		this.Container.Children.Add(this.HuodongIntro);
	}

	public void InitPartData(int pageID = 0)
	{
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(4, 23, 59, 59);
		this.bossTime = Global.GetServerMergeHuodongTimeDateTime(1, 13, 0, 0);
		this.startTimeStr = this.startTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.endTimeStr = this.endTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.HefuTime.Text = this.startTimeStr;
		this.HuodongTime.Text = StringUtil.substitute(Global.GetLang("{0} 至 {1}"), new object[]
		{
			this.startTimeStr,
			this.endTimeStr
		});
		switch (pageID)
		{
		case 0:
			this.SelectIcon("HefuDalibao");
			break;
		case 1:
			this.SelectIcon("VIPDahuikui");
			break;
		case 2:
			this.SelectIcon("ChongzhiDahuikui");
			break;
		case 3:
			this.SelectIcon("ChongzhiDafanli");
			break;
		case 4:
			this.SelectIcon("HefuWangchengZhengba");
			break;
		case 5:
			this.SelectIcon("SuperBossGongcheng");
			break;
		case 6:
			this.SelectIcon("PKWangDali");
			break;
		}
		this.StartUITimer();
	}

	private void SelectIcon(string iconName)
	{
		this.SetBtnState(iconName);
		this.SetBackground(this.SelectedIcon.ItemCode);
		this.SetPart(this.SelectedIcon.ItemCode);
	}

	private void SetBtnState(string iconName)
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName(iconName));
		if (gicon == this.SelectedIcon)
		{
			return;
		}
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.TextColor = new SolidColorBrush(16777080U);
		this.HuodongTitle.Text = gicon.Text;
		if (null != this.SelectedIcon)
		{
			this.SelectedIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
			this.SelectedIcon.TextColor = new SolidColorBrush(8418620U);
		}
		this.SelectedIcon = gicon;
	}

	private void SetPart(int showPage)
	{
		XElement gameResXml = Global.GetGameResXml(this.HefuHuikuiItemNames[showPage]);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "GiftList");
		if (xelementList == null)
		{
			return;
		}
		XElement xelement = xelementList[0];
		if (xelement != null)
		{
			this.HuodongIntro.Text = Global.GetXElementAttributeStr(xelement, "Description");
		}
		xelementList = Global.GetXElementList(gameResXml, "Award");
		xelement = xelementList[0];
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
		bool isInLingquTime;
		if (Global.InLimitTimeRange(this.startTimeStr, this.endTimeStr))
		{
			this.HuodongTime.TextColor = new SolidColorBrush(65280U);
			isInLingquTime = true;
		}
		else
		{
			this.HuodongTime.TextColor = new SolidColorBrush(16725815U);
			isInLingquTime = false;
		}
		this.partCanvas.Clear();
		switch (showPage)
		{
		case 0:
			if (null == this.hefuHuikuiHefuDalibaoPart)
			{
				this.hefuHuikuiHefuDalibaoPart = U3DUtils.NEW<HefuHuikuiHefuDalibaoPart>();
				this.hefuHuikuiHefuDalibaoPart.InitPartSize(414, 174);
				this.hefuHuikuiHefuDalibaoPart.InitPartData(xelementAttributeStr);
			}
			this.hefuHuikuiHefuDalibaoPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.hefuHuikuiHefuDalibaoPart);
			Canvas.SetLeft(this.hefuHuikuiHefuDalibaoPart, 0);
			Canvas.SetTop(this.hefuHuikuiHefuDalibaoPart, 0);
			break;
		case 1:
			if (null == this.hefuHuikuiVIPDahuikuiPart)
			{
				this.hefuHuikuiVIPDahuikuiPart = U3DUtils.NEW<HefuHuikuiVIPDahuikuiPart>();
				this.hefuHuikuiVIPDahuikuiPart.InitPartSize(414, 174);
				this.hefuHuikuiVIPDahuikuiPart.InitPartData(xelementAttributeStr);
			}
			this.hefuHuikuiVIPDahuikuiPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.hefuHuikuiVIPDahuikuiPart);
			Canvas.SetLeft(this.hefuHuikuiVIPDahuikuiPart, 0);
			Canvas.SetTop(this.hefuHuikuiVIPDahuikuiPart, 0);
			break;
		case 2:
			if (null == this.hefuHuikuiChongzhiDahuikuiPart)
			{
				this.hefuHuikuiChongzhiDahuikuiPart = U3DUtils.NEW<HefuHuikuiChongzhiDahuikuiPart>();
				this.hefuHuikuiChongzhiDahuikuiPart.InitPartSize(414, 174);
				this.hefuHuikuiChongzhiDahuikuiPart.InitPartData(xelementAttributeStr);
			}
			this.hefuHuikuiChongzhiDahuikuiPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.hefuHuikuiChongzhiDahuikuiPart);
			Canvas.SetLeft(this.hefuHuikuiChongzhiDahuikuiPart, 0);
			Canvas.SetTop(this.hefuHuikuiChongzhiDahuikuiPart, 0);
			break;
		case 3:
			if (null == this.hefuHuikuiChongzhiDafanliPart)
			{
				this.hefuHuikuiChongzhiDafanliPart = U3DUtils.NEW<HefuHuikuiChongzhiDafanliPart>();
				this.hefuHuikuiChongzhiDafanliPart.InitPartSize(414, 174);
				this.hefuHuikuiChongzhiDafanliPart.InitPartData();
			}
			this.hefuHuikuiChongzhiDafanliPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.hefuHuikuiChongzhiDafanliPart);
			Canvas.SetLeft(this.hefuHuikuiChongzhiDafanliPart, 0);
			Canvas.SetTop(this.hefuHuikuiChongzhiDafanliPart, 0);
			break;
		case 4:
			if (null == this.hefuHuikuiHefuWangchengZhengbaPart)
			{
				this.hefuHuikuiHefuWangchengZhengbaPart = U3DUtils.NEW<HefuHuikuiHefuWangchengZhengbaPart>();
				this.hefuHuikuiHefuWangchengZhengbaPart.InitPartSize(414, 174);
				this.hefuHuikuiHefuWangchengZhengbaPart.InitPartData(xelementAttributeStr);
			}
			this.GetWangchengzhanData();
			this.hefuHuikuiHefuWangchengZhengbaPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.hefuHuikuiHefuWangchengZhengbaPart);
			Canvas.SetLeft(this.hefuHuikuiHefuWangchengZhengbaPart, 0);
			Canvas.SetTop(this.hefuHuikuiHefuWangchengZhengbaPart, 0);
			break;
		case 5:
			if (null == this.hefuHuikuiSuperBossGongchengPart)
			{
				this.hefuHuikuiSuperBossGongchengPart = U3DUtils.NEW<HefuHuikuiSuperBossGongchengPart>();
				this.hefuHuikuiSuperBossGongchengPart.InitPartSize(414, 174);
				this.hefuHuikuiSuperBossGongchengPart.InitPartData(xelementAttributeStr);
			}
			this.partCanvas.Add(this.hefuHuikuiSuperBossGongchengPart);
			Canvas.SetLeft(this.hefuHuikuiSuperBossGongchengPart, 0);
			Canvas.SetTop(this.hefuHuikuiSuperBossGongchengPart, 0);
			break;
		case 6:
			if (null == this.hefuHuikuiPKWangDaliPart)
			{
				this.hefuHuikuiPKWangDaliPart = U3DUtils.NEW<HefuHuikuiPKWangDaliPart>();
				this.hefuHuikuiPKWangDaliPart.InitPartSize(414, 174);
				this.hefuHuikuiPKWangDaliPart.InitPartData(xelementAttributeStr);
			}
			this.GetXuezhanDifuData();
			this.hefuHuikuiPKWangDaliPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.hefuHuikuiPKWangDaliPart);
			Canvas.SetLeft(this.hefuHuikuiPKWangDaliPart, 0);
			Canvas.SetTop(this.hefuHuikuiPKWangDaliPart, 0);
			break;
		}
	}

	private void SetBackground(int showPage = 0)
	{
		if (showPage == 0 || showPage == 1 || showPage == 2)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/HefuHuikuiDali_bak.png"), false, 0);
		}
		else if (showPage == 3)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/HefuHuikuiChongzhiDafanli_bak.png"), false, 0);
		}
		else if (showPage == 4)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/HefuHuikuiHefuWangchengZhengba_bak.png"), false, 0);
		}
		else if (showPage == 5)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/hefuHuikuiSuperBossGongcheng_bak.png"), false, 0);
		}
		else if (showPage == 6)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/HefuHuikuiPKWang_bak.png"), false, 0);
		}
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("HefuHuikuiPart_Timer");
		this.UITimer.Interval = TimeSpan.FromMilliseconds(1000.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
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
	}

	public void SetTimer(bool state)
	{
		if (this.UITimer != null)
		{
			if (state)
			{
				this.UITimer.Start();
			}
			else
			{
				this.UITimer.Stop();
			}
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		double num = (double)Math.Max(0L, (this.endTime.Ticks - correctDateTime.Ticks) / 10000000L);
		if (num <= 0.0)
		{
			this.HuodongEndTime.Text = Global.GetLang("活动已结束");
			this.HuodongEndTime.TextColor = new SolidColorBrush(16725815U);
		}
		else
		{
			if (this.SelectedIcon.ItemCode == 4)
			{
				num = (double)Math.Max(0L, (this.WangchengzhanTime.Ticks - correctDateTime.Ticks) / 10000000L);
				if (num <= 0.0)
				{
					this.WangchengzhanTime = this.JustForReset;
					this.GetWangchengzhanData();
					return;
				}
			}
			else if (this.SelectedIcon.ItemCode == 5)
			{
				num = (double)Math.Max(0L, (this.bossTime.Ticks - correctDateTime.Ticks) / 10000000L);
				if (num <= 0.0)
				{
					this.HuodongEndTime.Text = Global.GetLang("BOSS已刷新");
					this.HuodongEndTime.TextColor = new SolidColorBrush(16777215U);
					return;
				}
			}
			else if (this.SelectedIcon.ItemCode == 6)
			{
				num = (double)Math.Max(0L, (this.XuezhanDifuTime.Ticks - correctDateTime.Ticks) / 10000000L);
				if (num <= 0.0)
				{
					this.XuezhanDifuTime = this.JustForReset;
					this.GetXuezhanDifuData();
					return;
				}
			}
			this.HuodongEndTime.Text = Global.GetTimeStrBySec(num, true);
			this.HuodongEndTime.TextColor = new SolidColorBrush(65280U);
		}
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.StopTimer();
	}

	public void GetWangchengzhanData()
	{
		WangChengMapInfoData wangChengMapInfoData = new WangChengMapInfoData();
		string nextBattleTime = "2013-05-14 13:00:00";
		wangChengMapInfoData.NextBattleTime = nextBattleTime;
		this.OnGetWangchengzhanDataCompleted(wangChengMapInfoData);
	}

	public void OnGetWangchengzhanDataCompleted(WangChengMapInfoData result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result != null)
		{
			this.WangchengzhanTime = Global.SafeConvertDateTime(result.NextBattleTime);
		}
	}

	public void GetXuezhanDifuData()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		this.XuezhanDifuTime = Global.GetTodayDateTime(0, 13, 0, 0);
		if (correctDateTime.Ticks > this.XuezhanDifuTime.Ticks)
		{
			this.XuezhanDifuTime = Global.GetTodayDateTime(1, 13, 0, 0);
		}
		if (this.XuezhanDifuTime.Ticks >= this.endTime.Ticks)
		{
			this.XuezhanDifuTime = this.JustForReset;
		}
	}

	public void OnLingquCompleted(int activityType, int result)
	{
		switch (activityType)
		{
		case 20:
			this.hefuHuikuiHefuDalibaoPart.OnLingquCompleted(result);
			break;
		case 23:
			this.hefuHuikuiChongzhiDafanliPart.OnLingquCompleted(result);
			break;
		case 24:
			this.hefuHuikuiPKWangDaliPart.OnLingquCompleted(result);
			break;
		case 25:
			this.hefuHuikuiHefuWangchengZhengbaPart.OnLingquCompleted(result);
			break;
		}
		if (result < 0)
		{
			if (result == -10005)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你已经领取过了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10006)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("活动期间充值额度为0，不能领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10007)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足领取条件"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10066)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟不存在, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10067)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您不是战盟的首领, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10077)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前的隔天登陆次数尚未达到, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10088)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前充值额度不足, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10089)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足领取条件"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10099)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前还不是VIP, 无法领取VIP奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你昨日不在排行榜内，无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("现在不是领取时间"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包空格不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GIcon SelectedIcon;

	private Canvas partCanvas = new Canvas();

	private GTextBlockOutLine HuodongTitle;

	private GTextBlockOutLine HefuTime;

	private GTextBlockOutLine HuodongTime;

	private GTextBlockOutLine HuodongEndTime;

	private GTextBlockOutLine HuodongIntro;

	public HefuHuikuiHefuDalibaoPart hefuHuikuiHefuDalibaoPart;

	public HefuHuikuiVIPDahuikuiPart hefuHuikuiVIPDahuikuiPart;

	public HefuHuikuiChongzhiDahuikuiPart hefuHuikuiChongzhiDahuikuiPart;

	public HefuHuikuiChongzhiDafanliPart hefuHuikuiChongzhiDafanliPart;

	public HefuHuikuiHefuWangchengZhengbaPart hefuHuikuiHefuWangchengZhengbaPart;

	public HefuHuikuiSuperBossGongchengPart hefuHuikuiSuperBossGongchengPart;

	public HefuHuikuiPKWangDaliPart hefuHuikuiPKWangDaliPart;

	public string[] HefuHuikuiItemNames = new string[]
	{
		"Config/HeFuGifts/HeFuLiBao.Xml",
		"Config/HeFuGifts/VIPLiBao.Xml",
		"Config/HeFuGifts/ChongZhiSong.Xml",
		"Config/HeFuGifts/HeFuFanLi.Xml",
		"Config/HeFuGifts/WangChengJiangLi.Xml",
		"Config/HeFuGifts/HeFuBOSS.Xml",
		"Config/HeFuGifts/PKJiangLi.Xml"
	};

	private LoadingWindow LoadingWin;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime bossTime;

	private string startTimeStr;

	private string endTimeStr;

	private DispatcherTimer UITimer;

	private DateTime WangchengzhanTime;

	private DateTime XuezhanDifuTime;

	private DateTime JustForReset;
}
