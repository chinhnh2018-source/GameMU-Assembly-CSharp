using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class XinQuHuoDongPart : UserControl
{
	public XinQuHuoDongPart()
	{
		this.ItemCollection = this.XinQuHuoDongList.ItemsSource;
		this.ItemCollection2 = this.XinQuHuoDongJiangLiList.ItemsSource;
		for (int i = 0; i < this.AwardInfoArr.Length; i++)
		{
			this.AwardInfoArr[i] = null;
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static XinQuHuoDongPart()
	{
		string[,] array = new string[8, 2];
		array[0, 0] = Global.GetLang("首充大礼");
		array[0, 1] = "Config/Gifts/FirstCharge.Xml";
		array[1, 0] = Global.GetLang("充值返利");
		array[1, 1] = "Config/Gifts/XinFanLi.Xml";
		array[2, 0] = Global.GetLang("充值加送");
		array[2, 1] = "Config/Gifts/ChongZhiSong.Xml";
		array[3, 0] = Global.GetLang("充值王");
		array[3, 1] = "Config/Gifts/ChongZhiKing.Xml";
		array[4, 0] = Global.GetLang("冲级王");
		array[4, 1] = "Config/Gifts/LevelKing.Xml";
		array[5, 0] = Global.GetLang("Boss王");
		array[5, 1] = "Config/Gifts/BossKing.Xml";
		array[6, 0] = Global.GetLang("武学王");
		array[6, 1] = "Config/Gifts/WuXueKing.Xml";
		array[7, 0] = Global.GetLang("经脉王");
		array[7, 1] = "Config/Gifts/JingMaiKing.Xml";
		XinQuHuoDongPart.XinQuHuoDongItemNames = array;
		XinQuHuoDongPart.XinQuHuoDongItemTime = new int[,]
		{
			{
				-1,
				-1,
				-1,
				-1
			},
			{
				4,
				23,
				59,
				59
			},
			{
				6,
				23,
				59,
				59
			},
			{
				6,
				23,
				59,
				59
			},
			{
				7,
				7,
				10,
				0
			},
			{
				7,
				7,
				10,
				0
			},
			{
				7,
				7,
				10,
				0
			},
			{
				7,
				7,
				10,
				0
			}
		};
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

	public ObservableCollection ItemCollection2
	{
		get
		{
			return this._ItemCollection2;
		}
		set
		{
			this._ItemCollection2 = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.XinQuHuoDongList);
		this.XinQuHuoDongList.Background = new SolidColorBrush(16777215U);
		this.XinQuHuoDongList.Width = 565.0;
		this.XinQuHuoDongList.Height = 100.0;
		this.XinQuHuoDongList.ItemMargin = new Thickness(0.0, 0.0, -5.0, 0.0);
		Canvas.SetLeft(this.XinQuHuoDongList, 29);
		Canvas.SetTop(this.XinQuHuoDongList, 16);
		this.XinQuHuoDongList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.XinQuHuoDongList_SelectionChanged);
		this.Container.Children.Add(this.XinQuHuoDongJiangLiList);
		this.XinQuHuoDongJiangLiList.Background = new SolidColorBrush(16777215U);
		this.XinQuHuoDongJiangLiList.Width = 567.0;
		this.XinQuHuoDongJiangLiList.Height = 190.0;
		this.XinQuHuoDongJiangLiList.ItemMargin = new Thickness(0.0, 0.0, 0.0, 5.0);
		this.XinQuHuoDongJiangLiList.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.XinQuHuoDongJiangLiList, 20);
		Canvas.SetTop(this.XinQuHuoDongJiangLiList, 267);
		this.Container.Children.Add(this.txtHuoDongMingCheng);
		Canvas.SetLeft(this.txtHuoDongMingCheng, 24);
		Canvas.SetTop(this.txtHuoDongMingCheng, 124);
		this.txtHuoDongMingCheng.TextColor = new SolidColorBrush(16774144U);
		this.txtHuoDongMingCheng.fontBold = true;
		this.Container.Children.Add(this.txtHuoDongShuoMing);
		Canvas.SetLeft(this.txtHuoDongShuoMing, 27);
		Canvas.SetTop(this.txtHuoDongShuoMing, 155);
		this.txtHuoDongShuoMing.Text = Global.GetLang("活动说明");
		this.txtHuoDongShuoMing.TextColor = new SolidColorBrush(11394222U);
		this.Container.Children.Add(this.txtHuoDongShuoMingNeiRong);
		Canvas.SetLeft(this.txtHuoDongShuoMingNeiRong, 56);
		Canvas.SetTop(this.txtHuoDongShuoMingNeiRong, 173);
		this.txtHuoDongShuoMingNeiRong.TextWrapping = TextWrapping.Wrap;
		this.txtHuoDongShuoMingNeiRong.BodyWidth = 510.0;
		this.txtHuoDongShuoMingNeiRong.TextColor = new SolidColorBrush(11394222U);
		this.Container.Children.Add(this.txtHuoDongShiJian);
		Canvas.SetLeft(this.txtHuoDongShiJian, 255);
		Canvas.SetTop(this.txtHuoDongShiJian, 124);
		this.txtHuoDongShiJian.TextColor = new SolidColorBrush(64770U);
		this.txtHuoDongShiJian.fontBold = true;
	}

	public void InitPartData()
	{
		this.LoadXinQuHuoDongList();
	}

	private void LoadXinQuHuoDongList()
	{
		this.ItemCollection.Clear();
		if (XinQuHuoDongPart.XinQuHuoDongItemNames == null)
		{
			return;
		}
		for (int i = 0; i < XinQuHuoDongPart.XinQuHuoDongItemNames.Length; i++)
		{
			XinQuHuoDongListItem xinQuHuoDongListItem = U3DUtils.NEW<XinQuHuoDongListItem>();
			xinQuHuoDongListItem.Width = 73.0;
			xinQuHuoDongListItem.Height = 93.0;
			xinQuHuoDongListItem.Title = XinQuHuoDongPart.XinQuHuoDongItemNames[i, 0];
			xinQuHuoDongListItem.TitleColor = new SolidColorBrush(16711901U);
			xinQuHuoDongListItem.ItemBackground = new ImageURL(Global.GetGameResImageURL("Images/Plate/huoDongSongLi_XinQuHuoDongItem_bak.png"), false, 0);
			xinQuHuoDongListItem.ItemImgURL = Global.GetGameResImageURL(StringUtil.substitute("Images/Plate/liBao_0{0}.png", new object[]
			{
				i + 1
			}));
			this.ItemCollection.AddNoUpdate(xinQuHuoDongListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void SelectistBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.XinQuHuoDongList.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	public void SetXinQuHuoDongListPage(int showPage = 0)
	{
		this.SelectistBox(showPage);
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void XinQuHuoDongList_SelectionChanged(object sender, object e)
	{
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.BodyBackground = null;
			this.SelectedListItem.TitleColor = new SolidColorBrush(16711901U);
			this.SelectedListItem.TitleBold = false;
		}
		if (this.XinQuHuoDongList.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem = U3DUtils.AS<XinQuHuoDongListItem>(this.XinQuHuoDongList.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.TitleColor = new SolidColorBrush(16774144U);
		this.SelectedListItem.TitleBold = true;
		if (this.currentPageIndex == this.XinQuHuoDongList.SelectedIndex)
		{
			return;
		}
		this.currentPageIndex = this.XinQuHuoDongList.SelectedIndex;
		if (this.currentPageIndex == 0)
		{
			this.LoadXinQuHuoDongConfigFromXML(this.currentPageIndex);
		}
		else if (this.AwardInfoArr[this.currentPageIndex] != null)
		{
			this.LoadXinQuHuoDongConfigFromXML(this.currentPageIndex);
			this.SendActivityQueryRequest(this.currentPageIndex);
		}
		else
		{
			this.SendActivityQueryRequest(this.currentPageIndex);
		}
	}

	private bool SendActivityQueryRequest(int index)
	{
		bool result = true;
		switch (index)
		{
		case 0:
			result = false;
			break;
		case 1:
			GameInstance.Game.SpriteQueryXinFanLi();
			break;
		case 2:
			GameInstance.Game.SpriteQueryInputSongLi();
			break;
		case 3:
			GameInstance.Game.SpriteQueryInputKing();
			break;
		case 4:
			GameInstance.Game.SpriteQueryLevelKing();
			break;
		case 5:
			GameInstance.Game.SpriteQueryEquipKing();
			break;
		case 6:
			GameInstance.Game.SpriteQueryHorseKing();
			break;
		case 7:
			GameInstance.Game.SpriteQueryJingMaiKing();
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	private void LoadXinQuHuoDongConfigFromXML(int index)
	{
		XElement isolateResXml = Global.GetIsolateResXml(XinQuHuoDongPart.XinQuHuoDongItemNames[index, 1]);
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Activities");
		if (xelementList == null)
		{
			return;
		}
		XElement xelement = xelementList[0];
		if (xelement != null)
		{
			this.txtHuoDongMingCheng.Text = Global.GetXElementAttributeStr(xelement, "Title");
			string huodongTimeStr = Global.GetHuodongTimeStr(0, 0, 0, 0);
			string huodongTimeStr2 = Global.GetHuodongTimeStr(XinQuHuoDongPart.XinQuHuoDongItemTime[index, 0], XinQuHuoDongPart.XinQuHuoDongItemTime[index, 1], XinQuHuoDongPart.XinQuHuoDongItemTime[index, 2], XinQuHuoDongPart.XinQuHuoDongItemTime[index, 3]);
			if ("-1" == huodongTimeStr || "-1" == huodongTimeStr2)
			{
				this.txtHuoDongShiJian.Text = Global.GetLang("【永久】");
			}
			else
			{
				this.txtHuoDongShiJian.Text = StringUtil.substitute(Global.GetLang("【{0} {1} {2}】"), new object[]
				{
					huodongTimeStr,
					Global.GetLang("至"),
					huodongTimeStr2
				});
			}
		}
		xelement = Global.GetXElement(isolateResXml, "GiftList");
		if (xelement == null)
		{
			return;
		}
		this.txtHuoDongShuoMingNeiRong.Text = Global.GetXElementAttributeStr(xelement, "Description");
		xelementList = Global.GetXElementList(isolateResXml, "Award");
		switch (index)
		{
		case 0:
			this.LoadShouChongDaJiangLi();
			this.GetChongZhiInfo();
			break;
		case 1:
			this.LoadChongZhiFanLiJiangLi(xelementList);
			break;
		case 2:
			this.LoadChongZhiJiaShongJiangLi(xelementList);
			break;
		case 3:
			this.LoadChongZhiWangJiangLi(xelementList);
			break;
		case 4:
			this.LoadChongJiWangJiangLi(xelementList);
			break;
		case 5:
			this.LoadZhuangBeiWangJiangLi(xelementList);
			break;
		case 6:
			this.LoadZuoJiWangJiangLi(xelementList);
			break;
		case 7:
			this.LoadJingMaiWangJiangLi(xelementList);
			break;
		}
	}

	private void GetChongZhiInfo()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteQueryChongZhiMoney(Global.Data.roleData.RoleID);
	}

	public void RefreshUI(bool isChongZhi, int ret)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (ret < 0)
		{
			return;
		}
		if (Global.Data.roleData.CZTaskID > 0)
		{
			this.iconBtn.Text = Global.GetLang("已经领取");
			this.iconBtn.EnableIcon = false;
		}
		else
		{
			this.iconBtn.EnableIcon = true;
			if (isChongZhi)
			{
				this.iconBtn.Text = Global.GetLang("立即领取");
				this.iconBtn.ItemCode = 1;
			}
			else
			{
				this.iconBtn.Text = Global.GetLang("立即充值");
				this.iconBtn.ItemCode = 0;
			}
		}
	}

	private void LoadShouChongDaJiangLi()
	{
		this.ItemCollection2.Clear();
		this.iconBtn = U3DUtils.NEW<GIcon>();
		this.iconBtn.Width = 81.0;
		this.iconBtn.Height = 21.0;
		this.iconBtn.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.iconBtn.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.iconBtn.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.iconBtn.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.iconBtn.ItemCode = -1;
		this.iconBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.iconBtn.ItemCode == 0)
			{
				Super.OpenChongZhiHtmlWindow();
			}
			else if (this.iconBtn.ItemCode == 1)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				if (this.LoadingWin != null)
				{
					this.Container.Children.Remove(this.LoadingWin, true);
					this.LoadingWin.Destroy();
					this.LoadingWin = null;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteGetFirstChongZhiDaLi(Global.Data.roleData.RoleID);
			}
		};
		XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
		xinQuHuoDongJiangLiListItem.TiaoJian = Global.GetLang("充值1钻石");
		xinQuHuoDongJiangLiListItem.ShuoMing = Global.GetLang("充值1钻石，即可领取");
		xinQuHuoDongJiangLiListItem.BtnLingQu = this.iconBtn;
		xinQuHuoDongJiangLiListItem.JiangLiLeiXing = true;
		xinQuHuoDongJiangLiListItem.GoodsList = string.Empty;
		xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
		xinQuHuoDongJiangLiListItem.BodyHeight = 0.0;
		this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
	}

	private void LoadChongZhiFanLiJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
			xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("第{0}名\n{1}"), new object[]
			{
				i + 1,
				this.GetNameByPaiHang(i + 1)
			});
			xinQuHuoDongJiangLiListItem.ShuoMing = string.Empty;
			xinQuHuoDongJiangLiListItem.JiangLi = StringUtil.substitute(Global.GetLang("返还当天充值比例的 {0}%"), new object[]
			{
				Global.GetXElementAttributeInt(xelement, "FanLi") * 100
			});
			xinQuHuoDongJiangLiListItem.JiangLiLeiXing = false;
			xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
			xinQuHuoDongJiangLiListItem.BodyHeight = 50.0;
			this.ItemCollection2.AddNoUpdate(xinQuHuoDongJiangLiListItem);
		}
		this.ItemCollection2.DelayUpdate();
	}

	private void LoadChongZhiJiaShongJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		XElement xelement = xml[0];
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinYuanBao");
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("立即领取");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteFetchActivityAward(3, 0);
		};
		XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
		xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("充值满{0}钻石"), new object[]
		{
			xelementAttributeInt
		});
		xinQuHuoDongJiangLiListItem.ShuoMing = StringUtil.substitute(Global.GetLang("充值累计满{0}钻石以上"), new object[]
		{
			xelementAttributeInt
		});
		xinQuHuoDongJiangLiListItem.BtnLingQu = gicon;
		xinQuHuoDongJiangLiListItem.JiangLiLeiXing = true;
		xinQuHuoDongJiangLiListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
		xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
		xinQuHuoDongJiangLiListItem.BodyHeight = 50.0;
		this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
	}

	private void LoadChongZhiWangJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = Global.GetLang("领取奖励");
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteFetchActivityAward(4, 0);
			};
			if (!this.IsMeByInputKingPaiHang(i + 1))
			{
				gicon.EnableIcon = false;
			}
			XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
			xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("第{0}名\n{1}"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "Ranking"),
				this.GetInputKingNameByPaiHang(i + 1)
			});
			xinQuHuoDongJiangLiListItem.ShuoMing = StringUtil.substitute(Global.GetLang("充值累计满{0}钻石以上"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			xinQuHuoDongJiangLiListItem.BtnLingQu = gicon;
			xinQuHuoDongJiangLiListItem.JiangLiLeiXing = true;
			xinQuHuoDongJiangLiListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
			xinQuHuoDongJiangLiListItem.BodyHeight = 0.0;
			this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
		}
	}

	private string GetInputKingNameByPaiHang(int paiHang)
	{
		string result = Global.GetLang("无");
		List<InputKingPaiHangData> paihangData = this.AwardInfoArr[3].paihangData;
		if (paihangData == null)
		{
			return result;
		}
		for (int i = 0; i < paihangData.Count; i++)
		{
			InputKingPaiHangData inputKingPaiHangData = paihangData[i];
			if (inputKingPaiHangData != null && paiHang == inputKingPaiHangData.PaiHang)
			{
				result = Global.FormatRoleName(inputKingPaiHangData.MaxLevelRoleZoneID, inputKingPaiHangData.MaxLevelRoleName);
			}
		}
		return result;
	}

	private string GetKingNameByPaiHang(int paiHang, int cacheIndex)
	{
		string result = Global.GetLang("无");
		if (cacheIndex < 0 || cacheIndex >= this.AwardInfoArr.Length)
		{
			return result;
		}
		List<HuoDongPaiHangData> huodongData = this.AwardInfoArr[cacheIndex].huodongData;
		if (huodongData == null)
		{
			return result;
		}
		for (int i = 0; i < huodongData.Count; i++)
		{
			HuoDongPaiHangData huoDongPaiHangData = huodongData[i];
			if (huoDongPaiHangData != null && paiHang == huoDongPaiHangData.PaiHang)
			{
				result = Global.FormatRoleName(huoDongPaiHangData.ZoneID, huoDongPaiHangData.RoleName);
			}
		}
		return result;
	}

	private bool IsMeByInputKingPaiHang(int paiHang)
	{
		bool result = false;
		List<InputKingPaiHangData> paihangData = this.AwardInfoArr[3].paihangData;
		if (paihangData == null)
		{
			return result;
		}
		for (int i = 0; i < paihangData.Count; i++)
		{
			InputKingPaiHangData inputKingPaiHangData = paihangData[i];
			if (inputKingPaiHangData != null && paiHang == inputKingPaiHangData.PaiHang && Global.Data.UserID == inputKingPaiHangData.UserID)
			{
				result = true;
			}
		}
		return result;
	}

	private bool IsMeByKingPaiHang(int paiHang, int cacheIndex)
	{
		bool result = false;
		if (cacheIndex < 0 || cacheIndex >= this.AwardInfoArr.Length)
		{
			return result;
		}
		List<HuoDongPaiHangData> huodongData = this.AwardInfoArr[cacheIndex].huodongData;
		if (huodongData == null)
		{
			return result;
		}
		for (int i = 0; i < huodongData.Count; i++)
		{
			HuoDongPaiHangData huoDongPaiHangData = huodongData[i];
			if (huoDongPaiHangData != null && paiHang == huoDongPaiHangData.PaiHang && Global.Data.roleData.RoleID == huoDongPaiHangData.RoleID)
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

	private bool IsMeByPaiHang(int index)
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
		if (listPaiHang[index] != null && Global.Data.UserID == listPaiHang[index].UserID)
		{
			result = true;
		}
		return result;
	}

	private void LoadChongJiWangJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = Global.GetLang("领取奖励");
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteFetchActivityAward(5, 0);
			};
			if (!this.IsMeByKingPaiHang(i + 1, 4))
			{
				gicon.EnableIcon = false;
			}
			XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
			xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("第{0}名\n{1}"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "Ranking"),
				this.GetKingNameByPaiHang(i + 1, 4)
			});
			xinQuHuoDongJiangLiListItem.ShuoMing = StringUtil.substitute(Global.GetLang("等级不低于{0}级的玩家"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinLevel")
			});
			xinQuHuoDongJiangLiListItem.JiangLi = StringUtil.substitute(Global.GetLang("{0}钻石"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "YuanBao")
			});
			xinQuHuoDongJiangLiListItem.BtnLingQu = gicon;
			xinQuHuoDongJiangLiListItem.JiangLiLeiXing = false;
			xinQuHuoDongJiangLiListItem.GoodsList = string.Empty;
			xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
			xinQuHuoDongJiangLiListItem.BodyHeight = 50.0;
			this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
		}
	}

	private void LoadZhuangBeiWangJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = Global.GetLang("领取奖励");
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteFetchActivityAward(6, 0);
			};
			if (!this.IsMeByKingPaiHang(i + 1, 5))
			{
				gicon.EnableIcon = false;
			}
			XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
			xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("第{0}名\n{1}"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "Ranking"),
				this.GetKingNameByPaiHang(i + 1, 5)
			});
			xinQuHuoDongJiangLiListItem.ShuoMing = StringUtil.substitute(Global.GetLang("至少击杀{0}个以上Boss"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinBoss")
			});
			xinQuHuoDongJiangLiListItem.BtnLingQu = gicon;
			xinQuHuoDongJiangLiListItem.JiangLiLeiXing = true;
			xinQuHuoDongJiangLiListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
			xinQuHuoDongJiangLiListItem.BodyHeight = 0.0;
			this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
		}
	}

	private void LoadZuoJiWangJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = Global.GetLang("领取奖励");
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteFetchActivityAward(7, 0);
			};
			if (!this.IsMeByKingPaiHang(i + 1, 6))
			{
				gicon.EnableIcon = false;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinWuXue");
			XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
			xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("第{0}名\n{1}"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "Ranking"),
				this.GetKingNameByPaiHang(i + 1, 6)
			});
			xinQuHuoDongJiangLiListItem.ShuoMing = StringUtil.substitute(Global.GetLang("武学境界需达到第{0}重:{1}"), new object[]
			{
				xelementAttributeInt,
				Global.GetWuXueName(xelementAttributeInt)
			});
			xinQuHuoDongJiangLiListItem.BtnLingQu = gicon;
			xinQuHuoDongJiangLiListItem.JiangLiLeiXing = true;
			xinQuHuoDongJiangLiListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
			xinQuHuoDongJiangLiListItem.BodyHeight = 0.0;
			this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
		}
	}

	private void LoadJingMaiWangJiangLi(List<XElement> xml)
	{
		if (xml == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = Global.GetLang("领取奖励");
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!base.EnableIcon)
				{
					return;
				}
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteFetchActivityAward(8, 0);
			};
			if (!this.IsMeByKingPaiHang(i + 1, 7))
			{
				gicon.EnableIcon = false;
			}
			XinQuHuoDongJiangLiListItem xinQuHuoDongJiangLiListItem = U3DUtils.NEW<XinQuHuoDongJiangLiListItem>();
			xinQuHuoDongJiangLiListItem.TiaoJian = StringUtil.substitute(Global.GetLang("第{0}名\n{1}"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "Ranking"),
				this.GetKingNameByPaiHang(i + 1, 7)
			});
			xinQuHuoDongJiangLiListItem.ShuoMing = StringUtil.substitute(Global.GetLang("经脉境界需达到{0}"), new object[]
			{
				Global.GetJingMaiNameNew(Global.GetXElementAttributeInt(xelement, "MinJingMai"))
			});
			xinQuHuoDongJiangLiListItem.BtnLingQu = gicon;
			xinQuHuoDongJiangLiListItem.JiangLiLeiXing = true;
			xinQuHuoDongJiangLiListItem.GoodsList = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			xinQuHuoDongJiangLiListItem.BodyWidth = 567.0;
			xinQuHuoDongJiangLiListItem.BodyHeight = 0.0;
			this.ItemCollection2.Add(xinQuHuoDongJiangLiListItem);
		}
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
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.roleID = roleID;
		oldHuodongXinquHuodongData.fanliYuanBao = fanliYuanBao;
		this.AwardInfoArr[1] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(1);
	}

	public void OnQueryInputFanLiExCompleted(JieriCZKingData dataList)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (dataList != null)
		{
			this.state = dataList;
			this.LoadXinQuHuoDongConfigFromXML(1);
		}
	}

	public void OnQueryInputSongLiCompleted(int result, int roleID, int roleYuanBaoInPeriod)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.roleID = roleID;
		oldHuodongXinquHuodongData.roleYuanBaoInPeriod = roleYuanBaoInPeriod;
		if (result <= 0)
		{
		}
		this.AwardInfoArr[2] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(2);
	}

	public void OnQueryInputKingActivityCompleted(List<InputKingPaiHangData> dataList)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.paihangData = dataList;
		if (dataList == null)
		{
		}
		this.AwardInfoArr[3] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(3);
	}

	public void OnQueryLevelKingActivityCompleted(List<HuoDongPaiHangData> dataList)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.huodongData = dataList;
		if (dataList == null)
		{
		}
		this.AwardInfoArr[4] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(4);
	}

	public void OnQueryEquipKingActivityCompleted(List<HuoDongPaiHangData> dataList)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.huodongData = dataList;
		if (dataList == null)
		{
		}
		this.AwardInfoArr[5] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(5);
	}

	public void OnQueryHorseKingActivityCompleted(List<HuoDongPaiHangData> dataList)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.huodongData = dataList;
		if (dataList == null)
		{
		}
		this.AwardInfoArr[6] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(6);
	}

	public void OnQueryJingMaiKingActivityCompleted(List<HuoDongPaiHangData> dataList)
	{
		if (null != this.LoadingWin)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		OldHuodongXinquHuodongData oldHuodongXinquHuodongData = new OldHuodongXinquHuodongData();
		oldHuodongXinquHuodongData.huodongData = dataList;
		if (dataList == null)
		{
		}
		this.AwardInfoArr[7] = oldHuodongXinquHuodongData;
		this.LoadXinQuHuoDongConfigFromXML(7);
	}

	public void OnQueryAwardHistoryCompleted(int result, int roleID, int activityType, int hasgettimes)
	{
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
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励成功"), new object[0]), 0, -1, -1, 0);
	}

	private LoadingWindow LoadingWin;

	private ListBox XinQuHuoDongList = new ListBox();

	private ListBox XinQuHuoDongJiangLiList = new ListBox();

	private int currentPageIndex = -1;

	private XinQuHuoDongListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Global.GetGameResImage("Images/Plate/huoDongSongLi_XinQuHuoDongItemSelect.png"));

	private static string[,] XinQuHuoDongItemNames;

	private static int[,] XinQuHuoDongItemTime;

	private GTextBlockOutLine txtHuoDongMingCheng = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtHuoDongShuoMing = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtHuoDongShuoMingNeiRong = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtHuoDongShiJian = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GIcon iconBtn;

	private OldHuodongXinquHuodongData[] AwardInfoArr = new OldHuodongXinquHuodongData[8];

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection2;

	private JieriCZKingData state;
}
