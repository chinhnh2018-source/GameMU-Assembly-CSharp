using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ChongzhiFanliPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 270.0;
		this.listBox.Height = 108.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 2.0);
		Canvas.SetLeft(this.listBox, 61);
		Canvas.SetTop(this.listBox, 77);
		this.ItemCollection = this.listBox.ItemsSource;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 87.0;
		gicon.Height = 40.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/cz_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/cz_hover.png"));
		Canvas.SetLeft(gicon, 220);
		Canvas.SetTop(gicon, 0);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.OpenChongZhiHtmlWindow();
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/scbtn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/scbtn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取");
		gicon.Tip = Global.GetLang("领取昨天的充值返利");
		gicon.EnableIcon = false;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteFetchActivityAward(30, 0);
		};
		Canvas.SetLeft(gicon, 256);
		Canvas.SetTop(gicon, 190);
		this.Container.Children.Add(gicon);
		this.BtnArr[0] = gicon;
		this.Container.Children.Add(this.YuanbaoText);
		this.YuanbaoText.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.YuanbaoText, 156);
		Canvas.SetTop(this.YuanbaoText, 193);
		this.Container.Children.Add(this.TimeText);
		this.TimeText.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.TimeText, 156);
		Canvas.SetTop(this.TimeText, 219);
		this.StartUITimer();
	}

	public void InitPartData()
	{
		this.GetData();
		GameInstance.Game.SpriteQueryXinFanLi();
	}

	public void GetData()
	{
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void OnGetDataCompleted(JieriCZKingData result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result != null)
		{
			this.state = result;
			this.SetBtnState(0, this.state.State);
			this.LoadList();
		}
	}

	private void SetBtnState(int id, int flag)
	{
		if (id >= 0 && null != this.BtnArr[id])
		{
			this.BtnArr[id].Visibility = true;
			if (flag > 0)
			{
				this.BtnArr[id].Text = Global.GetLang("已领取");
				this.BtnArr[id].EnableIcon = false;
			}
			else
			{
				this.BtnArr[id].Text = Global.GetLang("领取");
				this.BtnArr[id].EnableIcon = true;
			}
		}
	}

	private void LoadList()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/XinFanLi.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Award");
		if (xelementList == null)
		{
			return;
		}
		this.YuanbaoText.Text = string.Empty + this.state.YuanBao;
		this.ItemCollection.Clear();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			ChongzhiFanliListItem chongzhiFanliListItem = U3DUtils.NEW<ChongzhiFanliListItem>();
			chongzhiFanliListItem.txtRoleName.text = this.GetNameByPaiHang(i + 1);
			chongzhiFanliListItem.txtCondition.text = StringUtil.substitute(Global.GetLang("{0}%"), new object[]
			{
				int.Parse(Global.GetXElementAttributeStr(xelement, "FanLi")) * 100
			});
			chongzhiFanliListItem.Width = 265.0;
			chongzhiFanliListItem.Height = 19.0;
			this.ItemCollection.Add(chongzhiFanliListItem);
		}
	}

	private bool IsMeByPaiHang()
	{
		bool result = false;
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> listPaiHang = this.state.ListPaiHang;
		if (listPaiHang == null)
		{
			return result;
		}
		for (int i = 0; i < listPaiHang.Count; i++)
		{
			if (listPaiHang[i] != null && Global.Data.UserID == listPaiHang[i].UserID)
			{
				result = true;
			}
		}
		return result;
	}

	private string GetNameByPaiHang(int paiHang)
	{
		string result = Global.GetLang("无");
		if (this.state == null)
		{
			return result;
		}
		List<InputKingPaiHangData> listPaiHang = this.state.ListPaiHang;
		if (listPaiHang == null)
		{
			return result;
		}
		for (int i = 0; i < listPaiHang.Count; i++)
		{
			if (listPaiHang[i] != null && paiHang == listPaiHang[i].PaiHang)
			{
				result = Global.FormatRoleName(listPaiHang[i].MaxLevelRoleZoneID, listPaiHang[i].MaxLevelRoleName);
				break;
			}
		}
		return result;
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("ChongzhiFanliPart_Timer");
		this.UITimer.Interval = TimeSpan.FromMilliseconds(0.0);
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

	protected void UITimer_Tick(object sender, object e)
	{
		if (this.UITimer.Interval.Ticks <= 0L)
		{
			this.UITimer.Interval = TimeSpan.FromMilliseconds(1000.0);
		}
		DateTime huodongTimeDateTime = Global.GetHuodongTimeDateTime(4, 23, 59, 59);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		double num = Math.Max(0.0, Math.Floor((double)(huodongTimeDateTime.Ticks - correctDateTime.Ticks) / 10000000.0));
		if (num == 0.0)
		{
			this.UITimer.Stop();
			this.BtnArr[0].EnableIcon = true;
		}
		else
		{
			this.TimeText.Text = Global.GetTimeStrBySec(num, true);
		}
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		if (!(e.Tag is SpecialTextItem))
		{
			return true;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		string text2 = (e.Tag as SpecialTextItem).Tag as string;
		if (text2 == null || string.Empty == text2)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
			return true;
		}
		return true;
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

	public void OnQueryInputFanLiCompleted(int result, int roleID, int fanliYuanBao)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result <= 0)
		{
			return;
		}
		this.YuanbaoText.Text = fanliYuanBao.ToString();
	}

	public void OnFetchActivityAwardCompleted(int result, int roleID, int activityType)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result <= 0)
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
			else if (result == -1003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("昨日充值额不在排行榜内，不能领取"), new object[0]), 0, -1, -1, 0);
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
			return;
		}
		Global.IsLingquChongzhiFanli = true;
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			IDType = 0
		});
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励成功"), new object[0]), 0, -1, -1, 0);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private GTextBlockOutLine YuanbaoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine TimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private DispatcherTimer UITimer;

	private ListBox listBox = new ListBox();

	private JieriCZKingData state;

	private GIcon[] BtnArr = new GIcon[0];

	private ObservableCollection _ItemCollection;
}
