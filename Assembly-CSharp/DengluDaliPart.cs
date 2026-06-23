using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class DengluDaliPart : UserControl
{
	public DengluDaliPart()
	{
		this.ItemCollection = this.DengluDaliJiangLiList.ItemsSource;
		this.thisCtrl = this;
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.HeadBak);
		this.HeadBak.Width = 330.0;
		this.HeadBak.Height = 100.0;
		Canvas.SetLeft(this.HeadBak, 9);
		Canvas.SetTop(this.HeadBak, 9);
		this.Container.Children.Add(this.txtHuoDongShiJian);
		Canvas.SetLeft(this.txtHuoDongShiJian, 78);
		Canvas.SetTop(this.txtHuoDongShiJian, 354);
		this.txtHuoDongShiJian.TextColor = new SolidColorBrush(64770U);
		this.Container.Children.Add(this.DengluDaliJiangLiList);
		this.DengluDaliJiangLiList.Background = new SolidColorBrush(16777215U);
		this.DengluDaliJiangLiList.Width = 326.0;
		this.DengluDaliJiangLiList.Height = 227.0;
		this.DengluDaliJiangLiList.ItemMargin = new Thickness(0.0, 0.0, 0.0, 5.0);
		this.DengluDaliJiangLiList.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.DengluDaliJiangLiList, 11);
		Canvas.SetTop(this.DengluDaliJiangLiList, 116);
	}

	public void InitPartData()
	{
		this.HeadBak.URL = Global.GetGameResImageURL(StringUtil.substitute("Images/Plate/LeijiDengluHead_bak.png", new object[0]));
		this.LoadDengluDaliConfigFromXML();
	}

	private void LoadDengluDaliConfigFromXML()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/HuoDongLoginNumGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		if (Global.DengluDaliStartTime != string.Empty && Global.DengluDaliEndTime != string.Empty)
		{
			this.txtHuoDongShiJian.Text = StringUtil.substitute("{0} {1} {2}", new object[]
			{
				Global.DengluDaliStartTime,
				Global.GetLang("至"),
				Global.DengluDaliEndTime
			});
		}
		XElement xelement = Global.GetXElement(isolateResXml, "GoodsList");
		List<XElement> xelementList = Global.GetXElementList(xelement, "Goods");
		this.LoadDengluDaliJiangLi(xelementList);
	}

	private void LoadDengluDaliJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_normal.png"), 50.0, 29.0, 3.0, 2.0));
		ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_hover.png"), 50.0, 29.0, 3.0, 2.0));
		ImageBrush disableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_nouse.png"), 50.0, 29.0, 3.0, 2.0));
		int num = (Global.Data.MyHuoDongData != null) ? Global.Data.MyHuoDongData.LimitTimeLoginNum : 0;
		uint textColor = 16777080U;
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TimeOl");
			GIcon iconBtn = U3DUtils.NEW<GIcon>();
			iconBtn.Width = 50.0;
			iconBtn.Height = 29.0;
			iconBtn.BodySource = bodySource;
			iconBtn.NewSource = newSource;
			iconBtn.DisableBodySource = disableBodySource;
			iconBtn.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			iconBtn.Name = "LingquBtn_" + xelementAttributeInt;
			iconBtn.ItemCode = xelementAttributeInt;
			iconBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!this.EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteGetLimitTimeLoginGift(iconBtn.ItemCode);
			};
			this.BtnArr[xelementAttributeInt] = iconBtn;
			if (Global.IsLingquDengluDali(i))
			{
				iconBtn.EnableIcon = false;
				iconBtn.Text = Global.GetLang("已领取");
			}
			else if (i < num)
			{
				iconBtn.Text = Global.GetLang("领取");
				iconBtn.EnableIcon = true;
			}
			else
			{
				textColor = 8421504U;
				iconBtn.Text = Global.GetLang("未领取");
				iconBtn.EnableIcon = false;
			}
			DengluDaliJiangliListItem dengluDaliJiangliListItem = U3DUtils.NEW<DengluDaliJiangliListItem>();
			dengluDaliJiangliListItem.TextColor = textColor;
			dengluDaliJiangliListItem.DayNum = StringUtil.substitute(Global.GetLang("累计登陆{0}天"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "TimeOl")
			});
			dengluDaliJiangliListItem.BtnLingQu = iconBtn;
			dengluDaliJiangliListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			this.ItemCollection.Add(dengluDaliJiangliListItem);
		}
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

	private void SetBtnState(int id, bool enable, bool lingqu = true)
	{
		if (id >= 0 && null != this.BtnArr[id])
		{
			this.BtnArr[id].EnableIcon = enable;
		}
		if (lingqu)
		{
			this.BtnArr[id].Text = Global.GetLang("已领取");
		}
		else
		{
			this.BtnArr[id].Text = Global.GetLang("领取");
		}
	}

	public void OnLingquCompleted(int result, int day)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result < 0)
		{
			if (result == -200)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已经满，无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，失败原因{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功领取了累计登陆第{0}天的奖励"), new object[]
			{
				day
			}), 0, -1, -1, 0);
			this.SetBtnState(day, false, true);
		}
	}

	private UserControl thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private URLImage HeadBak = new URLImage();

	private ListBox DengluDaliJiangLiList = new ListBox();

	private GTextBlockOutLine txtHuoDongShiJian = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private List<GIcon> BtnArr = new List<GIcon>();

	private ObservableCollection _ItemCollection;
}
