using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JieriHuodongPart : UserControl
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
		this.HuodongTitle = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongTitle.TextFontColor = new SolidColorBrush(16777080U);
		this.HuodongTitle.TextSize = 14.0;
		this.HuodongTitle.fontBold = true;
		this.HuodongTitle.Width = 125.0;
		this.HuodongTitle.Height = 25.0;
		Canvas.SetLeft(this.HuodongTitle, 295);
		Canvas.SetTop(this.HuodongTitle, 19);
		this.Container.Children.Add(this.HuodongTitle);
		this.HuodongTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.HuodongTime.TextFontColor = new SolidColorBrush(16777215U);
		this.HuodongTime.TextSize = 12.0;
		Canvas.SetLeft(this.HuodongTime, 215);
		Canvas.SetTop(this.HuodongTime, 61);
		this.Container.Children.Add(this.HuodongTime);
		this.LingquTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.LingquTime.TextFontColor = new SolidColorBrush(16777215U);
		this.LingquTime.TextSize = 12.0;
		Canvas.SetLeft(this.LingquTime, 215);
		Canvas.SetTop(this.LingquTime, 85);
		this.Container.Children.Add(this.LingquTime);
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
		this.SetBossTimes();
		GameInstance.Game.SpriteGetJieriXmlData();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void OnXmlDataResult(JieriXmlData jieriXmlData)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (jieriXmlData == null)
		{
			return;
		}
		List<string> xmlList = jieriXmlData.XmlList;
		Global.Data.JieriData = jieriXmlData;
		this.LoadItemByConfig();
	}

	public void InitPartData(int pageID = 9)
	{
		if (this.ItemsDict.ContainsKey(pageID))
		{
			string iconName = null;
			this.ItemsDict.TryGetValue(pageID, ref iconName);
			this.SelectIcon(iconName);
			this.StartUITimer();
		}
	}

	public void SetBossTimes()
	{
		this.bossTime = new List<DateTime>();
		this.bossTime[0] = Global.GetJieriTimeDateTime(1, 14, 30, 0);
		this.bossTime[1] = Global.GetJieriTimeDateTime(3, 14, 30, 0);
		this.bossTime[2] = Global.GetJieriTimeDateTime(5, 14, 30, 0);
	}

	public void LoadItemByConfig()
	{
		if (Global.Data.JieriData == null)
		{
			return;
		}
		XElement xelement = new XElement(Global.Data.JieriData.XmlList[0]);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Type");
		if (xelementList == null)
		{
			return;
		}
		int num = 10;
		string text = string.Empty;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			if (xelement2 != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ID");
				text = Global.GetXElementAttributeStr(xelement2, "PeiZhi");
				this.AddIcon(text, xelementAttributeInt, Global.GetXElementAttributeStr(xelement2, "Name"), num);
				this.ItemsDict[xelementAttributeInt] = text;
				num += 41;
			}
		}
		this.InitPartData(9);
	}

	public void AddIcon(string namex, int flag, string text, int topY)
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = namex;
		gicon.Width = 104.0;
		gicon.Height = 35.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btnHefu_hover.png"));
		gicon.Text = Global.GetLang(text);
		gicon.TextColor = new SolidColorBrush(8418620U);
		gicon.Cursor = Cursors.Hand;
		gicon.ItemCode = flag;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectIcon(namex);
		};
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, topY);
		this.Container.Children.Add(gicon);
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
		showPage -= 8;
		XElement xelement = new XElement(Global.Data.JieriData.XmlList[showPage]);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		if (xelementList == null)
		{
			return;
		}
		if (showPage == 2 || showPage == 5)
		{
			this.HuodongEndTime.visible = false;
		}
		else
		{
			this.HuodongEndTime.visible = true;
		}
		this.HuodongEndTime.Text = string.Empty;
		XElement xelement2 = xelementList[0];
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		text = Global.GetXElementAttributeStr(xelement2, "FromDate");
		text2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		text3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		text4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.HuodongTime.Text = StringUtil.substitute(Global.GetLang("{0} 至 {1}"), new object[]
		{
			text,
			text2
		});
		this.LingquTime.Text = StringUtil.substitute(Global.GetLang("{0} 至 {1}"), new object[]
		{
			text3,
			text4
		});
		bool isInLingquTime;
		if (Global.InLimitTimeRange(text3, text4))
		{
			this.LingquTime.TextColor = new SolidColorBrush(65280U);
			isInLingquTime = true;
		}
		else
		{
			this.LingquTime.TextColor = new SolidColorBrush(16725815U);
			isInLingquTime = false;
		}
		this.endTime = Global.SafeConvertDateTime(text2);
		xelementList = Global.GetXElementList(xelement, "GiftList");
		if (xelementList == null)
		{
			return;
		}
		xelement2 = xelementList[0];
		if (xelement2 != null)
		{
			this.HuodongIntro.Text = Global.GetXElementAttributeStr(xelement2, "Description");
		}
		xelementList = Global.GetXElementList(xelement, "Award");
		this.partCanvas.Clear();
		switch (showPage)
		{
		case 1:
			if (null == this.jieriLibaoPart)
			{
				this.jieriLibaoPart = U3DUtils.NEW<JieriLibaoPart>();
				this.jieriLibaoPart.InitPartSize(414, 174);
				this.jieriLibaoPart.InitPartData(xelementList);
			}
			this.jieriLibaoPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriLibaoPart);
			Canvas.SetLeft(this.jieriLibaoPart, 0);
			Canvas.SetTop(this.jieriLibaoPart, 0);
			break;
		case 2:
			if (null == this.jieriDengluPart)
			{
				this.jieriDengluPart = U3DUtils.NEW<JieriDengluPart>();
				this.jieriDengluPart.InitPartSize(414, 174);
				this.jieriDengluPart.InitPartData(xelementList);
			}
			this.jieriDengluPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriDengluPart);
			Canvas.SetLeft(this.jieriDengluPart, 0);
			Canvas.SetTop(this.jieriDengluPart, 0);
			break;
		case 3:
			if (null == this.jieriVIPPart)
			{
				this.jieriVIPPart = U3DUtils.NEW<JieriVIPPart>();
				this.jieriVIPPart.InitPartSize(414, 174);
				this.jieriVIPPart.InitPartData(xelementList);
			}
			this.jieriVIPPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriVIPPart);
			Canvas.SetLeft(this.jieriVIPPart, 0);
			Canvas.SetTop(this.jieriVIPPart, 0);
			break;
		case 4:
			if (null == this.jieriChongzhisongPart)
			{
				this.jieriChongzhisongPart = U3DUtils.NEW<JieriChongzhisongPart>();
				this.jieriChongzhisongPart.InitPartSize(414, 174);
				this.jieriChongzhisongPart.InitPartData(xelementList);
			}
			this.jieriChongzhisongPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriChongzhisongPart);
			Canvas.SetLeft(this.jieriChongzhisongPart, 0);
			Canvas.SetTop(this.jieriChongzhisongPart, 0);
			break;
		case 5:
			if (null == this.jieriLeijiPart)
			{
				this.jieriLeijiPart = U3DUtils.NEW<JieriLeijiPart>();
				this.jieriLeijiPart.InitPartSize(414, 174);
				this.jieriLeijiPart.InitPartData(xelementList);
			}
			this.jieriLeijiPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriLeijiPart);
			Canvas.SetLeft(this.jieriLeijiPart, 0);
			Canvas.SetTop(this.jieriLeijiPart, 0);
			break;
		case 6:
			if (null == this.jieriBaoxiangPart)
			{
				this.jieriBaoxiangPart = U3DUtils.NEW<JieriBaoxiangPart>();
				this.jieriBaoxiangPart.InitPartSize(414, 174);
				this.jieriBaoxiangPart.InitPartData(xelementList);
			}
			this.jieriBaoxiangPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriBaoxiangPart);
			Canvas.SetLeft(this.jieriBaoxiangPart, 0);
			Canvas.SetTop(this.jieriBaoxiangPart, 0);
			break;
		case 7:
			if (null == this.jieriXiaofeikingPart)
			{
				this.jieriXiaofeikingPart = U3DUtils.NEW<JieriXiaofeikingPart>();
				this.jieriXiaofeikingPart.InitPartSize(414, 174);
				this.jieriXiaofeikingPart.InitPartData(xelementList);
			}
			this.jieriXiaofeikingPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriXiaofeikingPart);
			Canvas.SetLeft(this.jieriXiaofeikingPart, 0);
			Canvas.SetTop(this.jieriXiaofeikingPart, 0);
			break;
		case 8:
			if (null == this.jieriChongzhikingPart)
			{
				this.jieriChongzhikingPart = U3DUtils.NEW<JieriChongzhikingPart>();
				this.jieriChongzhikingPart.InitPartSize(414, 174);
				this.jieriChongzhikingPart.InitPartData(xelementList);
			}
			this.jieriChongzhikingPart.GetData(isInLingquTime);
			this.partCanvas.Add(this.jieriChongzhikingPart);
			Canvas.SetLeft(this.jieriChongzhikingPart, 0);
			Canvas.SetTop(this.jieriChongzhikingPart, 0);
			break;
		case 9:
			if (null == this.jieriBossPart)
			{
				this.jieriBossPart = U3DUtils.NEW<JieriBossPart>();
				this.jieriBossPart.InitPartSize(414, 174);
				this.jieriBossPart.InitPartData(xelementList);
			}
			this.partCanvas.Add(this.jieriBossPart);
			Canvas.SetLeft(this.jieriBossPart, 0);
			Canvas.SetTop(this.jieriBossPart, 0);
			break;
		}
	}

	private void SetBackground(int showPage)
	{
		showPage -= 8;
		if (showPage == 1 || showPage == 3 || showPage == 4)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/HefuHuikuiDali_bak.png"), false, 0);
		}
		else if (showPage == 2 || showPage == 5)
		{
			this.partCanvas.BackgroundURL = null;
		}
		else if (showPage == 6)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/JieriLihe_bak.png"), false, 0);
		}
		else if (showPage == 7 || showPage == 8)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/JieriWang_bak.png"), false, 0);
		}
		else if (showPage == 9)
		{
			this.partCanvas.BackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/JieriBoss_bak.png"), false, 0);
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
			int num2 = this.SelectedIcon.ItemCode - 8;
			if (num2 == 2 || num2 == 5)
			{
				this.HuodongEndTime.Text = string.Empty;
				return;
			}
			if (num2 == 9)
			{
				if (this.bossTime == null)
				{
					return;
				}
				if (this.bossTime.Count <= 0)
				{
					this.HuodongEndTime.Text = Global.GetLang("BOSS已刷新");
					this.HuodongEndTime.TextColor = new SolidColorBrush(16777215U);
					return;
				}
				num = (double)Math.Max(0L, (this.bossTime[0].Ticks - correctDateTime.Ticks) / 10000000L);
				if (num <= 0.0)
				{
					this.bossTime.RemoveAt(0);
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

	public void OnLingquCompleted(int activityType, int result)
	{
		switch (activityType)
		{
		case 9:
			this.jieriLibaoPart.OnLingquCompleted(result);
			break;
		case 10:
			this.jieriDengluPart.OnLingquCompleted(result);
			break;
		case 11:
			this.jieriVIPPart.OnLingquCompleted(result);
			break;
		case 12:
			this.jieriChongzhisongPart.OnLingquCompleted(result);
			break;
		case 13:
			this.jieriLeijiPart.OnLingquCompleted(result);
			break;
		case 14:
			this.jieriBaoxiangPart.OnLingquCompleted(result);
			break;
		case 15:
			this.jieriXiaofeikingPart.OnLingquCompleted(result);
			break;
		case 16:
			this.jieriChongzhikingPart.OnLingquCompleted(result);
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
			else if (result == -10077)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前的隔天登陆次数尚未达到, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10088)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前充值额度不足, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10099)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前还不是VIP, 无法领取VIP奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -20000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成节日礼盒今日的次数已经为0，请明日再进行合成操作"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -20003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成节日礼盒时需要的材料不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前不在排行榜内，无法领取奖励"), new object[0]), 0, -1, -1, 0);
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

	private GTextBlockOutLine HuodongTime;

	private GTextBlockOutLine LingquTime;

	private GTextBlockOutLine HuodongEndTime;

	private GTextBlockOutLine HuodongIntro;

	private LoadingWindow LoadingWin;

	private DateTime endTime;

	private List<DateTime> bossTime;

	private DispatcherTimer UITimer;

	public JieriLibaoPart jieriLibaoPart;

	public JieriDengluPart jieriDengluPart;

	public JieriVIPPart jieriVIPPart;

	public JieriChongzhisongPart jieriChongzhisongPart;

	public JieriLeijiPart jieriLeijiPart;

	public JieriBaoxiangPart jieriBaoxiangPart;

	public JieriXiaofeikingPart jieriXiaofeikingPart;

	public JieriChongzhikingPart jieriChongzhikingPart;

	public JieriBossPart jieriBossPart;

	private Dictionary<int, string> ItemsDict = new Dictionary<int, string>();
}
