using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class TiLianPart : UserControl
{
	protected uint GetColor(int index)
	{
		uint result = ColorSL.FromArgb(255, 113, 167, 180);
		switch (index)
		{
		case 0:
			result = ColorSL.FromArgb(255, 0, 255, 255);
			break;
		case 1:
			result = ColorSL.FromArgb(255, 0, 255, 0);
			break;
		case 2:
			result = ColorSL.FromArgb(255, 0, 124, 255);
			break;
		case 3:
			result = ColorSL.FromArgb(255, 180, 0, 255);
			break;
		}
		return result;
	}

	protected void InitRadioArr()
	{
		for (int i = 0; i < this.RadioArr.Count; i++)
		{
			this.Container.Children.Remove(this.RadioArr[i], true);
		}
		List<int> list = new List<int>();
		Dictionary<string, string> yaoShiDiaoLuoForXiangZhi = Global.GetYaoShiDiaoLuoForXiangZhi(this.XiangZiGoodID, list);
		for (int j = 0; j < list.Count; j++)
		{
			string text = Global.GetLang("直接打开");
			string name = list[j].ToString();
			string tag = list[j].ToString();
			if (list[j] != 0)
			{
				text = Global.GetLang("使用 ") + Global.GetGoodsNameByID(list[j], false);
			}
			GCheckBox gcheckBox = new GCheckBox();
			gcheckBox.Name = name;
			gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/radio_normal.png"));
			gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/radio_active.png"));
			gcheckBox.Check = false;
			gcheckBox.Tag = tag;
			gcheckBox.Text = text;
			gcheckBox.TextColor = new SolidColorBrush(this.GetColor(j + 1));
			int value = 0;
			int value2 = 0;
			switch (j)
			{
			case 0:
				value = 30;
				value2 = 189;
				break;
			case 1:
				value = 143;
				value2 = 189;
				break;
			case 2:
				value = 30;
				value2 = 210;
				break;
			case 3:
				value = 143;
				value2 = 210;
				break;
			}
			Canvas.SetLeft(gcheckBox, value);
			Canvas.SetTop(gcheckBox, value2);
			this.Container.Children.Add(gcheckBox);
			this.RadioArr.Add(gcheckBox);
			gcheckBox.CheckChanged = delegate(object r, BaseEventArgs o)
			{
				if ((r as GCheckBox).Check)
				{
					this.SelectedYaoShiID = (int)(r as GCheckBox).Tag;
				}
				bool flag = false;
				for (int k = 0; k < this.RadioArr.Count; k++)
				{
					if (this.RadioArr[k] != r as GCheckBox && this.RadioArr[k].Check)
					{
						flag = true;
						if ((r as GCheckBox).Check)
						{
							this.RadioArr[k].Check = false;
						}
						break;
					}
				}
				if (!flag)
				{
					(r as GCheckBox).Check = true;
				}
			};
			if (j == 0)
			{
				gcheckBox.Check = true;
				this.SelectedYaoShiID = (int)gcheckBox.Tag;
			}
		}
	}

	protected void InitTextGoodsNameArray()
	{
		this.TextGoodsNameArr.Clear();
		for (int i = 0; i < 4; i++)
		{
			GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
			this.Container.Children.Add(gtextBlockOutLine);
			gtextBlockOutLine.TextColor = new SolidColorBrush(this.GetColor(i));
			Canvas.SetTop(gtextBlockOutLine, 111);
			int value = 0;
			switch (i)
			{
			case 0:
				value = 22;
				break;
			case 1:
				value = 85;
				break;
			case 2:
				value = 134;
				break;
			case 3:
				value = 184;
				break;
			}
			Canvas.SetLeft(gtextBlockOutLine, value);
			this.TextGoodsNameArr.Add(gtextBlockOutLine);
		}
	}

	protected void AddGoodsName(int index, string goodsName)
	{
		if (index >= this.TextGoodsNameArr.Count || index < 0)
		{
			return;
		}
		GTextBlockOutLine gtextBlockOutLine = this.TextGoodsNameArr[index];
		gtextBlockOutLine.Text = goodsName;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public ObservableCollection goodIcon
	{
		get
		{
			return this._goodIcon;
		}
		set
		{
			this._goodIcon = value;
		}
	}

	public int XiangZiGoodID
	{
		get
		{
			return this._XiangZiGoodID;
		}
		set
		{
			this._XiangZiGoodID = value;
		}
	}

	protected override void InitializeComponent()
	{
	}

	public void ReInitPart()
	{
		this.InitRadioArr();
		this.RefurbishIcon();
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.InitRadioArr();
		this.InitTextGoodsNameArray();
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "StartDig";
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("开启");
		Canvas.SetLeft(gicon, 30);
		Canvas.SetTop(gicon, 317);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.StartDig();
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "StopDig";
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("停止");
		Canvas.SetLeft(gicon, 140);
		Canvas.SetTop(gicon, 317);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon || this._Timer == null)
			{
				return;
			}
			if (!this.IsSendWaBaoCmd)
			{
				this.StopHeart();
				this.EndDig();
			}
			this.needStopDig = true;
		};
		gicon.EnableIcon = false;
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "AotoBuy";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.Text = Global.GetLang("道具数量不足时自动购买");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 34);
		Canvas.SetTop(gcheckBox, 250);
		this.Container.Children.Add(gcheckBox);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "AoToStart";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.Text = Global.GetLang("领取物品后自动重新开始");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 34);
		Canvas.SetTop(gcheckBox, 280);
		this.Container.Children.Add(gcheckBox);
		this.Container.Children.Add(this.listBoxIcon);
		this.listBoxIcon.Width = 200.0;
		this.listBoxIcon.Height = 32.0;
		this.listBoxIcon.ItemMargin = new Thickness(0.0, 0.0, 18.0, 0.0);
		Canvas.SetLeft(this.listBoxIcon, 36);
		Canvas.SetTop(this.listBoxIcon, 133);
		this.Container.Children.Add(this.imgJiangPing);
		this.imgJiangPing.Width = 32.0;
		this.imgJiangPing.Height = 32.0;
		Canvas.SetLeft(this.imgJiangPing, 110);
		Canvas.SetTop(this.imgJiangPing, 40);
		this.IconDiged = U3DUtils.NEW<GIcon>();
		gicon.Width = 32.0;
		gicon.Height = 32.0;
		this.Container.Children.Add(this.IconDiged);
		Canvas.SetLeft(this.IconDiged, 110);
		Canvas.SetTop(this.IconDiged, 40);
		this.IconDiged.Visibility = false;
	}

	public void InitPartData()
	{
		this.InitGoodsIconIDList();
		this.goodIcon = this.listBoxIcon.ItemsSource;
	}

	public void GetNewData()
	{
		this.RefurbishIcon();
	}

	private void RefurbishIcon()
	{
		this.goodIcon.Clear();
		this.AddGoodsIcon(this.XiangZiGoodID, 0);
		List<int> list = new List<int>();
		Global.GetYaoShiDiaoLuoForXiangZhi(this.XiangZiGoodID, list);
		int num = 0;
		while (num < list.Count && num < 3)
		{
			this.AddGoodsIcon(list[num], num + 1);
			num++;
		}
	}

	public bool CanGetwaBaoGoods()
	{
		return null != Global.Data.WaBaoGoodsData;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.EndDig();
	}

	private void ResetGoodsIconCount()
	{
		if (this.goodIcon.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < this.goodIcon.Length; i++)
		{
			GIcon gicon = U3DUtils.AS<GIcon>(this.goodIcon.GetAt(i));
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(gicon.ItemCode);
			gicon.Text = totalGoodsCountByID.ToString();
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
			if (totalGoodsCountByID <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
		}
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("DigTreasurePart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(50.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._TimerCount = 0;
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._Timer = null;
		this._TimerCount = 0;
	}

	private void ForgeTimer_Tick(object sender, object e)
	{
		this._TimerCount++;
		if (this._TimerCount >= 15 && !this.IsSendWaBaoCmd)
		{
			this.IsSendWaBaoCmd = true;
			GameInstance.Game.SpriteExecWaBaoByYaoShi(this.XiangZiGoodID, this.SelectedYaoShiID, this.GetAutoBuy());
		}
		this.imgJiangPing.URL = this.GetRandomGoodsImage();
	}

	private int GetAutoBuy()
	{
		GCheckBox gcheckBox = U3DUtils.AS<GCheckBox>(this.Container.FindName("AotoBuy"));
		if (null != gcheckBox && gcheckBox.Check)
		{
			return 1;
		}
		return 0;
	}

	private void InitGoodsIconIDList()
	{
		if (this.PreXiangZiID == this.XiangZiGoodID)
		{
			return;
		}
		this.PreXiangZiID = this.XiangZiGoodID;
		this.GoodsIconIDList.Clear();
		string xmlName = StringUtil.substitute("Config/YuanShiDig.Xml", new object[0]);
		XElement gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "*");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "YuanShiID");
			if (xelementAttributeInt == this.XiangZiGoodID)
			{
				string[] array = Global.GetXElementAttributeStr(xelement, "GoodsID").Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(array[i]));
					string iconCode = Super.GetIconCode(goodsXmlNodeByID);
					this.GoodsIconIDList.Add(iconCode);
				}
				break;
			}
		}
	}

	private string GetRandomGoodsImage()
	{
		if (this.GoodsIconIDList.Count <= 0)
		{
			return null;
		}
		int num = this.rand.Next(0, this.GoodsIconIDList.Count);
		return Super.GetGoodsImageURLFromIconCode(this.GoodsIconIDList[num], "NetImages/GameRes/");
	}

	private void StartDig()
	{
		this.IsSendWaBaoCmd = false;
		this.IconDiged.Visibility = false;
		this.imgJiangPing.Visibility = true;
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
		gicon.EnableIcon = false;
		gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StopDig"));
		gicon.EnableIcon = true;
		this.needStopDig = false;
		this.InitGoodsIconIDList();
		this.StartHeart();
	}

	private void EndDig()
	{
		this.IsSendWaBaoCmd = false;
		this.imgJiangPing.Destroy();
		this.imgJiangPing.Visibility = false;
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
		gicon.EnableIcon = true;
		gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StopDig"));
		gicon.EnableIcon = false;
	}

	private void AddGoodsIcon(int goodsID, int titleIndex = -1)
	{
		GoodsData goodsData = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string title = goodsXmlNodeByID.Title;
			this.AddGoodsName(titleIndex, title);
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = goodsData;
			gicon.BoxTypes = -1;
			gicon.Text = ((goodsData == null) ? Global.GetTotalGoodsCountByID(goodsID).ToString() : string.Empty);
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableMovingEnd = true;
			if (Global.GetTotalGoodsCountByID(goodsID) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.goodIcon.Add(gicon);
		}
	}

	public void RefreshGoodsCount()
	{
		for (int i = 0; i < this.goodIcon.Length; i++)
		{
			GIcon gicon = U3DUtils.AS<GIcon>(this.goodIcon[i]);
			if (!(null == gicon))
			{
				int itemCode = gicon.ItemCode;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(itemCode);
				if (goodsXmlNodeByID != null)
				{
					string title = goodsXmlNodeByID.Title;
					string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
					gicon.Text = Global.GetTotalGoodsCountByID(itemCode).ToString();
					if (Global.GetTotalGoodsCountByID(itemCode) <= 0)
					{
						gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
					}
					else
					{
						gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
					}
				}
			}
		}
	}

	private void ModifyGetGoodsIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		int goodsID = goodsData.GoodsID;
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			this.IconDiged.Width = 32.0;
			this.IconDiged.Height = 32.0;
			this.IconDiged.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			this.IconDiged.TipType = 1;
			this.IconDiged.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			this.IconDiged.ItemCode = goodsID;
			this.IconDiged.ItemObject = goodsData;
			this.IconDiged.BoxTypes = -1;
			this.IconDiged.Text = ((goodsData == null) ? Global.GetTotalGoodsCountByID(goodsID).ToString() : string.Empty);
			this.IconDiged.TextHorizontalAlignment = global::Layout.Right;
			this.IconDiged.TextVerticalAlignment = global::Layout.Bottom;
			this.IconDiged.TextShadowColor = 4278190080U;
			this.IconDiged.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			this.IconDiged.DisableMovingEnd = true;
			if (Global.GetTotalGoodsCountByID(goodsData.GoodsID) <= 0)
			{
				this.IconDiged.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.IconDiged.Visibility = true;
		}
	}

	public void NotifyExecWaBaoByYaoShiResult(GoodsData goodsData)
	{
		this.StopHeart();
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (goodsData.Id < 0)
		{
			if (goodsData.Id == -20)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("本挖宝功能已经关闭"), 0, -1, -1, 0);
			}
			else if (goodsData.Id == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包内没有{0}"), new object[]
				{
					Global.GetGoodsNameByID(this.XiangZiGoodID, false)
				}), 0, -1, -1, 0);
			}
			else if (goodsData.Id == -200)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包内没有{0}"), new object[]
				{
					Global.GetGoodsNameByID(this.SelectedYaoShiID, false)
				}), 0, -1, -1, 0);
			}
			else if (goodsData.Id == -300)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("你的背包内没有位置了"), 0, -1, -1, 0);
			}
			else if (goodsData.Id == -2300)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("自动购买时钻石不足"), 0, -1, -1, 0);
			}
			else if (goodsData.Id == -2000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包内没有{0}了,商城不出售该物品，没法自动购买"), new object[]
				{
					Global.GetGoodsNameByID(this.XiangZiGoodID, false)
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("开启礼盒时错误:{0}"), new object[]
				{
					goodsData.Id
				}), 0, -1, -1, 0);
			}
			this.EndDig();
			this.ResetGoodsIconCount();
			return;
		}
		this.ModifyGetGoodsIcon(goodsData);
		this.EndDig();
		this.ResetGoodsIconCount();
		GCheckBox gcheckBox = U3DUtils.AS<GCheckBox>(this.Container.FindName("AoToStart"));
		if (null != gcheckBox && gcheckBox.Check && !this.needStopDig)
		{
			this.StartDig();
		}
	}

	public void NotifyHide()
	{
		GCheckBox gcheckBox = U3DUtils.AS<GCheckBox>(this.Container.FindName("AoToStart"));
		if (null != gcheckBox && gcheckBox.Check)
		{
			gcheckBox.Check = false;
		}
	}

	private RandomAS rand = new RandomAS(0);

	private int _TimerCount;

	private DispatcherTimer _Timer;

	private List<string> GoodsIconIDList = new List<string>();

	private LoadingWindow LoadingWin;

	private bool IsSendWaBaoCmd;

	private GIcon IconDiged;

	private URLImage imgJiangPing = new URLImage();

	private ListBox listBoxIcon = new ListBox();

	private int SelectedYaoShiID;

	private bool needStopDig;

	private List<GCheckBox> RadioArr = new List<GCheckBox>();

	private List<GTextBlockOutLine> TextGoodsNameArr = new List<GTextBlockOutLine>();

	private ObservableCollection _goodIcon;

	private int _XiangZiGoodID;

	private int PreXiangZiID = -100;
}
