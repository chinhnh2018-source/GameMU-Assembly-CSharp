using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class XingYunZhiDuiHuanPart : UserControl
{
	public XingYunZhiDuiHuanPart()
	{
		this.arrayXingYunZhi[0] = this.txtFirst;
		this.arrayXingYunZhi[1] = this.txtSecond;
		this.arrayXingYunZhi[2] = this.txtThird;
		this.arrayXingYunZhi[3] = this.txtFour;
		this.ItemCollection = this.listbox.ItemsSource;
	}

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

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.listbox);
		this.listbox.Background = new SolidColorBrush(16777215U);
		this.listbox.Width = 590.0;
		this.listbox.Height = 139.0;
		this.listbox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.listbox, 19);
		Canvas.SetTop(this.listbox, 176);
		this.Container.Children.Add(this.txtFirst);
		Canvas.SetLeft(this.txtFirst, 104);
		Canvas.SetTop(this.txtFirst, 57);
		this.txtFirst.TextColor = new SolidColorBrush(65535U);
		this.Container.Children.Add(this.txtSecond);
		Canvas.SetLeft(this.txtSecond, 242);
		Canvas.SetTop(this.txtSecond, 57);
		this.txtSecond.TextColor = new SolidColorBrush(65535U);
		this.Container.Children.Add(this.txtThird);
		Canvas.SetLeft(this.txtThird, 381);
		Canvas.SetTop(this.txtThird, 57);
		this.txtThird.TextColor = new SolidColorBrush(65535U);
		this.Container.Children.Add(this.txtFour);
		Canvas.SetLeft(this.txtFour, 517);
		Canvas.SetTop(this.txtFour, 57);
		this.txtFour.TextColor = new SolidColorBrush(65535U);
		this.Container.Children.Add(this.txtXingYunZhi);
		Canvas.SetLeft(this.txtXingYunZhi, 495);
		Canvas.SetTop(this.txtXingYunZhi, 107);
		this.txtXingYunZhi.TextColor = new SolidColorBrush(46850U);
		this.txtXingYunZhi.Text = "100";
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteQueryYangGongBKDailyData();
		this.GetXingYunZhi();
		this.LoadList();
	}

	private void GetXingYunZhi()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Lucky.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			this.arrayXingYunZhi[i].Text = Global.GetXElementAttributeStr(xelement, "Lucky");
		}
	}

	private void LoadList()
	{
		this.ItemCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/LuckyAward.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 80.0;
			gicon.Height = 21.0;
			gicon.Text = Global.GetLang("点击领取");
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(10551295U);
			gicon.ItemCode = Global.GetXElementAttributeInt(xelement, "ID");
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (base.EnableIcon)
				{
					GameInstance.Game.SpriteFetchYangGongBKJiFenAward((s as GIcon).ItemCode);
				}
			};
			gicon.EnableIcon = false;
			XingYunZhiJiangLiListItem xingYunZhiJiangLiListItem = U3DUtils.NEW<XingYunZhiJiangLiListItem>();
			xingYunZhiJiangLiListItem.BodyWidth = 582.0;
			xingYunZhiJiangLiListItem.BodyHeight = 23.0;
			xingYunZhiJiangLiListItem.XingYunZhiText = Global.GetXElementAttributeStr(xelement, "MinLucky");
			xingYunZhiJiangLiListItem.JiangLiDaoJuNameText = this.GetGoodsNameByStr(Global.GetXElementAttributeStr(xelement, "GoodsIDs"));
			xingYunZhiJiangLiListItem.ShuoMingText = Global.GetXElementAttributeStr(xelement, "Description");
			xingYunZhiJiangLiListItem.BtnLingQu = gicon;
			this.ItemCollection.AddNoUpdate(xingYunZhiJiangLiListItem);
			this.BtnArr.Add(gicon);
		}
		this.ItemCollection.DelayUpdate();
	}

	private string GetGoodsNameByStr(string str)
	{
		str = StringUtil.trim(str);
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		string[] array = str.Split(new char[]
		{
			','
		});
		if (array.Length <= 0)
		{
			return string.Empty;
		}
		return Global.GetGoodsNameByID(Convert.ToInt32(array[0]), false);
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void OnFetchYangGongBKJiFenAwardCompleted(int result, int roleID, int awardNo)
	{
		if (result >= 0)
		{
			this.EnableFetchButton(awardNo, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取杨公宝库积分奖励成功"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10006)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取杨公宝库积分奖励失败，积分不够"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10007)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取杨公宝库积分奖励失败，积分不够"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10008)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取杨公宝库积分奖励失败，已经领取过了"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -125)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取杨公宝库积分奖励失败，你的背包空格不够"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取杨公宝库积分奖励时错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void OnQueryYangGongBKDailyDataCompleted(YangGongBKDailyJiFenData dailyData)
	{
		if (dailyData == null)
		{
			this.txtXingYunZhi.Text = "0";
			return;
		}
		this.txtXingYunZhi.Text = dailyData.JiFen.ToString();
		for (int i = 0; i < this.BtnArr.Count; i++)
		{
			this.BtnArr[i].EnableIcon = Global.IsYangGongBKAwardCanBeenGot(this.BtnArr[i].ItemCode, dailyData);
			if (Global.IsYangGongBKAwardHasBeenGot(this.BtnArr[i].ItemCode, dailyData))
			{
				this.BtnArr[i].EnableIcon = false;
				this.BtnArr[i].Text = Global.GetLang("已经领取");
			}
		}
	}

	public void EnableFetchButton(int awardNo, bool enable = false)
	{
		for (int i = 0; i < this.BtnArr.Count; i++)
		{
			if (awardNo == this.BtnArr[i].ItemCode)
			{
				this.BtnArr[i].EnableIcon = enable;
				break;
			}
		}
	}

	private LoadingWindow LoadingWin;

	private GTextBlockOutLine txtFirst = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtSecond = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtThird = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtFour = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtXingYunZhi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine[] arrayXingYunZhi = new GTextBlockOutLine[4];

	private ListBox listbox = new ListBox();

	private List<GIcon> BtnArr = new List<GIcon>();

	private ObservableCollection _ItemCollection;
}
