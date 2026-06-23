using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class VipPart : UserControl
{
	public VipPart()
	{
		this.ItemCollection = this.VIPJiangLiList.ItemsSource;
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
		this.Container.Children.Add(this.txtHint);
		Canvas.SetLeft(this.txtHint, 88);
		Canvas.SetTop(this.txtHint, 32);
		this.txtHint.TextColor = new SolidColorBrush(16774144U);
		this.txtHint.fontBold = true;
		this.txtHint.TextWrapping = TextWrapping.Wrap;
		this.Container.Children.Add(this.VIPJiangLiList);
		this.VIPJiangLiList.Background = new SolidColorBrush(16777215U);
		this.VIPJiangLiList.Width = 320.0;
		this.VIPJiangLiList.Height = 283.0;
		this.VIPJiangLiList.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.VIPJiangLiList, 14);
		Canvas.SetTop(this.VIPJiangLiList, 114);
		this.iconBtn = U3DUtils.NEW<GIcon>();
		this.iconBtn.Width = 81.0;
		this.iconBtn.Height = 25.0;
		this.iconBtn.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.iconBtn.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.iconBtn.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.iconBtn.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.iconBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 1
				});
			}
		};
		this.Container.Children.Add(this.iconBtn);
		Canvas.SetLeft(this.iconBtn, 245);
		Canvas.SetTop(this.iconBtn, 38);
		this.iconGood = this.GetGicon((int)ConfigSystemParam.GetSystemParamIntByName("VIPGoodsID"));
		if (null != this.iconGood)
		{
			this.Container.Children.Add(this.iconGood);
			Canvas.SetLeft(this.iconGood, 27);
			Canvas.SetTop(this.iconGood, 27);
		}
	}

	public void InitPartData()
	{
		if (this._vip)
		{
			GameInstance.Game.SpriteQueryVipDailyDataList();
		}
		this.LoadVIPJiangLiList();
	}

	public void TryHintVIP()
	{
		if (!this._vip)
		{
			this.iconBtn.HintDecoType = 1;
		}
	}

	public override void Destroy()
	{
		this.iconBtn.HintDecoType = -1;
	}

	private void LoadVIPJiangLiList()
	{
		this.ItemCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/VIP.Xml");
		if (gameResXml == null)
		{
			return;
		}
		GIcon iconBtn = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 39.0, 16.0, 3.0, 2.0));
		ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 39.0, 16.0, 3.0, 2.0));
		ImageBrush disableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 39.0, 16.0, 3.0, 2.0));
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GetID");
			if (xelementAttributeInt > 0)
			{
				iconBtn = U3DUtils.NEW<GIcon>();
				iconBtn.Width = 39.0;
				iconBtn.Height = 16.0;
				iconBtn.Text = Global.GetLang("领取");
				iconBtn.BodySource = bodySource;
				iconBtn.NewSource = newSource;
				iconBtn.DisableBodySource = disableBodySource;
				iconBtn.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
				iconBtn.Name = "FetchBtn_" + xelementAttributeInt;
				iconBtn.ItemCode = xelementAttributeInt;
				iconBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (this.EnableIcon)
					{
						GameInstance.Game.SpriteFetchVipDailyAward(iconBtn.ItemCode);
					}
				};
				this.BtnArr[xelementAttributeInt] = iconBtn;
			}
			else
			{
				iconBtn = null;
			}
			VIPJiangLiListItem vipjiangLiListItem = U3DUtils.NEW<VIPJiangLiListItem>();
			vipjiangLiListItem.BodyWidth = 318.0;
			vipjiangLiListItem.BodyHeight = 21.0;
			vipjiangLiListItem.JiangLiText = Global.GetXElementAttributeStr(xelement, "Description");
			if (null != iconBtn)
			{
				vipjiangLiListItem.BtnLingQu = iconBtn;
			}
			this.ItemCollection.AddNoUpdate(vipjiangLiListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void SetFetchButtonStateForPriority(int priority, bool enable)
	{
		if (priority >= 0 && null != this.BtnArr[priority])
		{
			this.BtnArr[priority].EnableIcon = false;
		}
	}

	public void IsVip(bool vip)
	{
		if (!this._vip && vip)
		{
			foreach (GIcon gicon in this.BtnArr)
			{
				if (null != gicon)
				{
					gicon.Visibility = true;
				}
			}
		}
		int num = 20;
		this._vip = vip;
		if (this._vip)
		{
			this.txtHint.BodyWidth = 219.0;
			this.txtHint.Text = StringUtil.substitute(Global.GetLang("尊贵的VIP用户，您可以享受系统为您精心准备的{0}项特权"), new object[]
			{
				num
			});
			this.txtHint.TextColor = new SolidColorBrush(16774144U);
			this.iconBtn.Visibility = false;
		}
		else
		{
			this.txtHint.BodyWidth = 140.0;
			this.txtHint.Text = StringUtil.substitute(Global.GetLang("立即成为VIP，即可尊享VIP多达{0}项特权豪礼"), new object[]
			{
				num
			});
			this.txtHint.TextColor = new SolidColorBrush(65280U);
			this.iconBtn.Visibility = true;
		}
	}

	private GIcon GetGicon(int iGoodsid)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(iGoodsid);
		if (goodsXmlNodeByID != null)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 48.0;
			gicon.Height = 48.0;
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/64_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				iGoodsid,
				0,
				-1,
				-1
			});
			gicon.ItemCode = iGoodsid;
			gicon.ItemCategory = goodsXmlNodeByID.Categoriy;
			Super.GetGoods64x64ImageFromFile(goodsXmlNodeByID.IconCode, gicon);
			return gicon;
		}
		return null;
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

	public void OnUseVipDailyPriorityCompleted(int result, int roleID, int priority)
	{
		if (result >= 0)
		{
			this.SetFetchButtonStateForPriority(priority, false);
		}
		else if (result == -10000)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，需要vip权限"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10001)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，今天你已经领取过了"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -125)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，你的背包空格不够"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励时错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void OnUpdateVipDailyData(List<VipDailyData> vipDailyDataList)
	{
		if (vipDailyDataList == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("vip日数据为空"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int dayOfYear = Global.GetCorrectDateTime().DayOfYear;
		for (int i = 0; i < vipDailyDataList.Count; i++)
		{
			VipDailyData vipDailyData = vipDailyDataList[i];
			if (vipDailyData != null && dayOfYear == vipDailyData.DayID && vipDailyData.UsedTimes >= 1)
			{
				this.SetFetchButtonStateForPriority(vipDailyData.PriorityType, false);
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private ListBox VIPJiangLiList = new ListBox();

	private GTextBlockOutLine txtHint = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GIcon iconBtn;

	private GIcon iconGood;

	private List<GIcon> BtnArr = new List<GIcon>();

	public bool ShowVipHint;

	private ObservableCollection _ItemCollection;

	private bool _vip;
}
