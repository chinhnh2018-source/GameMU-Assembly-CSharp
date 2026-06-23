using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class DigTreasurePart : UserControl
{
	public DigTreasurePart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 200.0;
		this.listBox.Height = 90.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.listBox, 35);
		Canvas.SetTop(this.listBox, 37);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 0.0);
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 32.0;
		this.Equip.Height = 32.0;
		this.Equip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Equip, 110);
		Canvas.SetTop(this.Equip, 152);
		this.Equip.BorderThickness = 0;
		this.Container.Children.Add(this.EquipImage);
		this.EquipImage.Width = 32.0;
		this.EquipImage.Height = 32.0;
		Canvas.SetLeft(this.EquipImage, 110);
		Canvas.SetTop(this.EquipImage, 153);
		this.Container.Children.Add(this.CangBaoTuText);
		this.CangBaoTuText.Width = 32.0;
		this.CangBaoTuText.Height = 32.0;
		this.CangBaoTuText.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.CangBaoTuText, 39);
		Canvas.SetTop(this.CangBaoTuText, 255);
		this.CangBaoTuText.BorderThickness = 0;
	}

	public ObservableCollection[] equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
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
		this.EndDig();
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.CanContinueDig = false;
		this.EndDig();
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "StartDig";
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("开始挖宝");
		Canvas.SetLeft(gicon, 48);
		Canvas.SetTop(gicon, 322);
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
		gicon.Name = "GetGoods";
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取宝物");
		Canvas.SetLeft(gicon, 135);
		Canvas.SetTop(gicon, 322);
		this.Container.Children.Add(gicon);
		gicon.EnableIcon = false;
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.StartGetGoods();
		};
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "AotoGet";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.Text = Global.GetLang("自动领取宝物");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 90);
		Canvas.SetTop(gcheckBox, 252);
		this.Container.Children.Add(gcheckBox);
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "AoToStart";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.Text = Global.GetLang("领取宝物后自动重新开始");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 90);
		Canvas.SetTop(gcheckBox, 277);
		this.Container.Children.Add(gcheckBox);
		this.progressBar = U3DUtils.NEW<GProgressBar>();
		this.progressBar.BodyWidth = 184.0;
		this.progressBar.BodyHeight = 5.0;
		this.progressBar.ForeBrush = new ImageBrush(Global.GetGameResImage("Images/Plate/wb_progress.png"));
		this.progressBar.Stroke = 4289309097U;
		this.progressBar.RadiusX = 0.0;
		this.progressBar.RadiusY = 0.0;
		this.progressBar.Visibility = false;
		Canvas.SetLeft(this.progressBar, 35);
		Canvas.SetTop(this.progressBar, 211);
		this.Container.Children.Add(this.progressBar);
		this.EquipImage.Visibility = false;
	}

	public void InitPartData()
	{
		this.equipIcon = new ObservableCollection[2];
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[1] = this.CangBaoTuText.ItemsSource;
		this.InitGoodsIconIDList();
		this.GetGoodsList();
	}

	public void GetNewData()
	{
		this.CanContinueDig = true;
		this.RefurbishIcon();
	}

	private void RefurbishIcon()
	{
		if (Global.Data.WaBaoGoodsData != null)
		{
			this.AddGoodsIcon(Global.Data.WaBaoGoodsData.GoodsID, 0, Global.Data.WaBaoGoodsData);
			GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
			gicon.EnableIcon = false;
			gicon = U3DUtils.AS<GIcon>(this.Container.FindName("GetGoods"));
			gicon.EnableIcon = true;
		}
		else
		{
			GIcon gicon2 = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
			gicon2.EnableIcon = true;
			gicon2 = U3DUtils.AS<GIcon>(this.Container.FindName("GetGoods"));
			gicon2.EnableIcon = false;
		}
		int goodsID = (int)ConfigSystemParam.GetSystemParamIntByName("WaBaoGoodsID");
		this.AddGoodsIcon(goodsID, 1, null);
	}

	private void StartDig()
	{
		if (Global.Data.WaBaoGoodsData != null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("请先领走已经挖出的宝物后，才能再次挖宝"), 0, -1, -1, 0);
			return;
		}
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
		gicon.EnableIcon = false;
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		this.EquipImage.Visibility = true;
		this.Equip.Visibility = false;
		this.StartHeart();
	}

	private void EndDig()
	{
		this.StopHeart();
		this.IsSendWaBaoCmd = false;
		this.EquipImage.Destroy();
		this.EquipImage.Visibility = false;
		this.Equip.Visibility = true;
		if (Global.Data.WaBaoGoodsData != null)
		{
			this.AddGoodsIcon(Global.Data.WaBaoGoodsData.GoodsID, 0, Global.Data.WaBaoGoodsData);
			GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("GetGoods"));
			gicon.EnableIcon = true;
		}
		else
		{
			GIcon gicon2 = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
			gicon2.EnableIcon = true;
		}
	}

	public bool CanGetwaBaoGoods()
	{
		return null != Global.Data.WaBaoGoodsData;
	}

	private void StartGetGoods()
	{
		if (Global.Data.WaBaoGoodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前没有可以领取挖的宝物"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteGetWaBaoGoods();
	}

	private void InitGoodsIconIDList()
	{
		string xmlName = StringUtil.substitute("Config/Dig.Xml", new object[0]);
		XElement gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "*");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GoodsID");
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(xelementAttributeInt);
			string iconCode = Super.GetIconCode(goodsXmlNodeByID);
			this.GoodsIconIDList.Add(iconCode);
		}
	}

	public void RefreshGoodsCount()
	{
		this.GetNewData();
		this.ResetGoodsIconCount();
	}

	private void ResetGoodsIconCount()
	{
		if (this.equipIcon[1].Length <= 0)
		{
			return;
		}
		GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[1][0]);
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(gicon.ItemCode);
		gicon.Text = totalGoodsCountByID.ToString();
		if (totalGoodsCountByID <= 0)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
		}
	}

	private void AddGoodsIcon(int goodsID, int index, GoodsData goodsData = null)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				(goodsData == null) ? -1 : goodsData.Id,
				(goodsData == null) ? -1 : 11
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = ((goodsData == null) ? Global.GetTotalGoodsCountByID(goodsID).ToString() : string.Empty);
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			if (index <= 0)
			{
				bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			}
			else if (Global.GetTotalGoodsCountByID(goodsID) <= 0)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(ggoodIcon);
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

	public void StartHeart()
	{
		this.StopHeart();
		this.progressBar.Visibility = true;
		this._Timer = new DispatcherTimer("DigTreasurePart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(50.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this.progressBar.Percent = 0.0;
		this._TimerCount = 0;
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this.progressBar.Visibility = false;
		this._Timer.Stop();
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._Timer = null;
		this._TimerCount = 0;
	}

	private void ForgeTimer_Tick(object sender, object e)
	{
		this._TimerCount++;
		this.progressBar.Percent = Math.Min(1.0, (double)this._TimerCount / 20.0);
		if (this._TimerCount >= 15 && !this.IsSendWaBaoCmd)
		{
			this.IsSendWaBaoCmd = true;
			GameInstance.Game.SpriteExecWaBao();
		}
		this.EquipImage.URL = this.GetRandomGoodsImage();
	}

	public void NotifyExecWaBaoResult(GoodsData goodsData)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		GCheckBox gcheckBox;
		if (goodsData.Id < 0)
		{
			if (goodsData.Id == -1)
			{
				int toBuyGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("WaBaoGoodsID");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您背包中已经没有无量密藏，请刷新万宝阁购买后再开始挖宝"), new object[0]), 20, -1, -1, toBuyGoodsID);
			}
			else if (goodsData.Id == -20)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("非常不幸, 本次没有挖到任何宝物"), 0, -1, -1, 0);
			}
			else if (goodsData.Id == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("请先领走已经挖出的宝物后，才能再次挖宝"), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("执行挖宝时错误:{0}"), new object[]
				{
					goodsData.Id
				}), 0, -1, -1, 0);
			}
			Global.Data.WaBaoGoodsData = null;
			this.EndDig();
			this.ResetGoodsIconCount();
			if (goodsData.Id == -20)
			{
				this.equipIcon[0].Clear();
				GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
				gicon.EnableIcon = true;
				gicon = U3DUtils.AS<GIcon>(this.Container.FindName("GetGoods"));
				gicon.EnableIcon = false;
				if (this.CanContinueDig)
				{
					gcheckBox = U3DUtils.AS<GCheckBox>(this.Container.FindName("AoToStart"));
					if (null != gcheckBox && gcheckBox.Check)
					{
						this.StartDig();
					}
				}
			}
			return;
		}
		Global.Data.WaBaoGoodsData = goodsData;
		this.EndDig();
		this.ResetGoodsIconCount();
		if (!this.CanContinueDig)
		{
			return;
		}
		gcheckBox = U3DUtils.AS<GCheckBox>(this.Container.FindName("AotoGet"));
		if (null != gcheckBox && gcheckBox.Check)
		{
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteGetWaBaoGoods();
		}
	}

	public void NotifyGetWaBaoGoodsResult(int result, int roleID)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result < 0)
		{
			if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前没有可以领取挖的宝物"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请清理背包，整理出空格后，再领取宝物"), new object[0]), 1, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取挖宝物品时错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		Global.Data.WaBaoGoodsData = null;
		this.equipIcon[0].Clear();
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("StartDig"));
		gicon.EnableIcon = true;
		gicon = U3DUtils.AS<GIcon>(this.Container.FindName("GetGoods"));
		gicon.EnableIcon = false;
		if (!this.CanContinueDig)
		{
			return;
		}
		GCheckBox gcheckBox = U3DUtils.AS<GCheckBox>(this.Container.FindName("AoToStart"));
		if (null != gcheckBox && gcheckBox.Check)
		{
			this.StartDig();
		}
	}

	public void GetGoodsList()
	{
		if (DigTreasurePart.WuLiangGoodsIDs == null)
		{
			DigTreasurePart.WuLiangGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("WuLiangGoodsIDs", ',');
		}
		for (int i = 0; i < DigTreasurePart.WuLiangGoodsIDs.Length; i++)
		{
			int num = Global.SafeConvertToInt32(DigTreasurePart.WuLiangGoodsIDs[i].ToString());
			if (num > 0)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
				if (goodsXmlNodeByID != null)
				{
					string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
					GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
					ggoodIcon.Width = 32.0;
					ggoodIcon.Height = 32.0;
					ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
					ggoodIcon.TipType = 1;
					ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
					{
						num,
						0,
						-1,
						-1
					});
					ggoodIcon.ItemCode = num;
					ggoodIcon.ItemObject = null;
					ggoodIcon.BoxTypes = -1;
					GoodsPackItem goodsPackItem = U3DUtils.NEW<GoodsPackItem>();
					goodsPackItem.GoodsImgs = ggoodIcon;
					goodsPackItem.GoodsImgBacks = Global.GetGameResImage("Images/Plate/rm_listItem.png");
					this.ItemCollection.AddNoUpdate(goodsPackItem);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private GProgressBar progressBar;

	private LoadingWindow LoadingWin;

	private bool IsSendWaBaoCmd;

	private bool CanContinueDig;

	private List<string> GoodsIconIDList = new List<string>();

	private RandomAS rand = new RandomAS(0);

	private int _TimerCount;

	private DispatcherTimer _Timer;

	private ListBox Equip = new ListBox();

	private URLImage EquipImage = new URLImage();

	private ListBox CangBaoTuText = new ListBox();

	private ListBox listBox = new ListBox();

	public ObservableCollection ItemCollection;

	private ObservableCollection[] _equipIcon;

	public static int[] WuLiangGoodsIDs;
}
