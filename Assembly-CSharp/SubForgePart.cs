using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SubForgePart : UserControl
{
	public SubForgePart()
	{
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.MoneyText);
		this.MoneyText.FontSize = HSTextField.defaultFontSize;
		this.MoneyText.Text = string.Empty;
		this.MoneyText.Width = 200.0;
		this.MoneyText.TextWrapping = true;
		this.MoneyText.Foreground = new SolidColorBrush(9010791U);
		Canvas.SetLeft(this.MoneyText, 133);
		Canvas.SetTop(this.MoneyText, 33);
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 35.0;
		this.Equip.Height = 35.0;
		Canvas.SetLeft(this.Equip, 40);
		Canvas.SetTop(this.Equip, 55);
		this.Equip.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Rock);
		this.Rock.Width = 35.0;
		this.Rock.Height = 35.0;
		Canvas.SetLeft(this.Rock, 21);
		Canvas.SetTop(this.Rock, 282);
		this.Rock.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Fu);
		this.Fu.Width = 35.0;
		this.Fu.Height = 35.0;
		Canvas.SetLeft(this.Fu, 68);
		Canvas.SetTop(this.Fu, 282);
		this.Fu.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.XYfu);
		this.XYfu.Width = 35.0;
		this.XYfu.Height = 35.0;
		Canvas.SetLeft(this.XYfu, 115);
		Canvas.SetTop(this.XYfu, 282);
		this.XYfu.Background = new SolidColorBrush(16777215U);
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

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public bool Forging
	{
		get
		{
			return this._Forging;
		}
		set
		{
			this._Forging = value;
		}
	}

	public override void Destroy()
	{
		ObjectClickGetingMgr.CancelClickGetThing(11);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("精锻");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		Canvas.SetLeft(gicon, 217);
		Canvas.SetTop(gicon, 375);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartForgeEquip();
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入装备");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Tip = "PutEquipBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 23);
		Canvas.SetTop(gicon, 14);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 30.0;
		Canvas.SetLeft(gtextBlockOutLine, 258);
		Canvas.SetTop(gtextBlockOutLine, 278);
		this.Container.Children.Add(gtextBlockOutLine);
		this.QDSNubText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 30.0;
		Canvas.SetLeft(gtextBlockOutLine, 258);
		Canvas.SetTop(gtextBlockOutLine, 302);
		this.Container.Children.Add(gtextBlockOutLine);
		this.YinLiangText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 30.0;
		Canvas.SetLeft(gtextBlockOutLine, 125);
		Canvas.SetTop(gtextBlockOutLine, 326);
		this.Container.Children.Add(gtextBlockOutLine);
		this.OddsText = gtextBlockOutLine;
		this.progressBar = U3DUtils.NEW<GProgressBar>();
		this.progressBar.BodyWidth = 117.0;
		this.progressBar.BodyHeight = 15.0;
		this.progressBar.BackColor = 4286611584U;
		this.progressBar.StrokeThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
		this.progressBar.Stroke = 4289309097U;
		this.progressBar.RadiusX = 8.0;
		this.progressBar.RadiusY = 8.0;
		this.levelImg.SingleStarWidth = 22.0;
		this.levelImg.Img0_Source = Global.GetGameResImage("Images/Plate/zbdz_level_0.png");
		this.levelImg.Img1_Source = Global.GetGameResImage("Images/Plate/zbdz_level_1.png");
		this.levelImg.Img0_Size = new SizeSL(220.0, 21.0);
		this.levelImg.Img1_Size = new SizeSL(220.0, 21.0);
		this.levelImg.Level = 0;
		Canvas.SetLeft(this.levelImg, 96);
		Canvas.SetTop(this.levelImg, 49);
		this.Container.Children.Add(this.levelImg);
		this.progressBar.Visibility = false;
		Canvas.SetLeft(this.progressBar, 182);
		Canvas.SetTop(this.progressBar, 329);
		this.Container.Children.Add(this.progressBar);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ObjectClickEvent objectClickEvent = evt.Tag as ObjectClickEvent;
		if (objectClickEvent.ClickGetThingType != ClickGetThingTypes.SubForge)
		{
			return;
		}
		if (this.Forging)
		{
			return;
		}
		int clickGetThingDbID = objectClickEvent.ClickGetThingDbID;
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(clickGetThingDbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		if (goodsDataByDbID.AddPropIndex >= 10)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("装备强化只能精锻10星以下的装备!"), 0, -1, -1, 0);
			return;
		}
		this.levelImg.Level = goodsDataByDbID.AddPropIndex;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				if (goodsDataByDbID.Quality == 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("白色装备不能精锻"), 0, -1, -1, 0);
					return;
				}
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					goodsDataByDbID.Id,
					0
				});
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemCode = goodsDataByDbID.GoodsID;
				ggoodIcon.ItemObject = goodsDataByDbID;
				ggoodIcon.BoxTypes = 5;
				ggoodIcon.Text = ((goodsDataByDbID.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
				{
					goodsDataByDbID.Forge_level.ToString()
				}));
				ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.TextVerticalAlignment = global::Layout.Top;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = 4294901760U;
				this.ZBGicon = ggoodIcon;
				Super.InitEquipGIcon(ggoodIcon, goodsDataByDbID, false, IconTextTypes.Qianghua);
				this.equipIcon[0].Clear();
				this.equipIcon[0].Add(ggoodIcon);
				this.InitSubForgeNeedGoodsInfo(goodsDataByDbID);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有装备才能精锻"), new object[0]), 0, -1, -1, 0);
			}
		}
		if (this.ToHintStartForce)
		{
		}
	}

	private void StartForgeEquip()
	{
		if (this.Forging)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经在精锻中..."), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.ForgeDbID = -1;
		this.RockGoodsID = -1;
		this.NeedNum = -1;
		if (this.equipIcon[0].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将要精锻的装备拖放到装备位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精锻的装备不在背包中，无法精锻"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		string title = goodsXmlNodeByID.Title;
		if (goodsData.AddPropIndex >= 10)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别，无法精锻"), new object[]
			{
				title
			}), 0, -1, -1, 0);
			return;
		}
		if (goodsData.Quality == 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】是白色装备，不能精锻"), new object[]
			{
				title
			}), 0, -1, -1, 0);
			return;
		}
		bool flag = goodsData.Binding > 0;
		bool flag2 = false;
		string[] subForgeNextLevelParams = Global.GetSubForgeNextLevelParams(goodsData);
		if (subForgeNextLevelParams == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("千锻石配置信息错误"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.NeedYinLiang = subForgeNextLevelParams[2].SafeToInt32(0);
		if (Global.Data.roleData.YinLiang < this.NeedYinLiang)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		}
		this.ForgeDbID = goodsData.Id;
		this.NeedNum = subForgeNextLevelParams[1].SafeToInt32(0);
		if (this.equipIcon[1].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将精锻需要的千锻石拖放到千锻石位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int itemCode = U3DUtils.AS<GIcon>(this.equipIcon[1][0]).ItemCode;
		goodsData = Global.GetGoodsDataByID(itemCode);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有需要的千锻石，无法精锻"), new object[0]), 19, -1, -1, itemCode);
			return;
		}
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsData.GoodsID);
		if (totalGoodsCountByID < this.NeedNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的千锻石数量不足, 需要{0}块增千锻石"), new object[]
			{
				this.NeedNum
			}), 0, -1, -1, 0);
			return;
		}
		flag2 = (flag2 || Global.GetTotalBindingGoodsCountByID(goodsData.GoodsID) > 0);
		this.RockGoodsID = goodsData.GoodsID;
		if (!flag && flag2)
		{
			string message = StringUtil.substitute(Global.GetLang("精锻使用的材料中有绑定的材料, 精锻后您的【{0}】可能被绑定，为避免此情况，请将背包中绑定的材料转移到随身仓库中，继续精锻吗？"), new object[]
			{
				title
			});
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), message, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.ExecuteForgeEquip();
				}
				return true;
			};
		}
		else
		{
			this.ExecuteForgeEquip();
		}
	}

	private void ExecuteForgeEquip()
	{
		this.Forging = true;
		Global.Data.GameScene.AddForgeDecoration();
		this.progressBar.Visibility = true;
		this.StartHeart();
	}

	public void SetNeedGoodsIcon(int goodsID)
	{
		this.AddGoodsIcon(goodsID, 1, 1);
	}

	protected void InitSubForgeNeedGoodsInfo(GoodsData goodsData)
	{
		string[] subForgeNextLevelParams = Global.GetSubForgeNextLevelParams(goodsData);
		if (subForgeNextLevelParams == null)
		{
			return;
		}
		int num = subForgeNextLevelParams[1].SafeToInt32(0);
		this.AddGoodsIcon(subForgeNextLevelParams[0].SafeToInt32(0), 1, num);
		this.QDSNubText.Text = num.ToString();
		if (Global.GetTotalGoodsCountByID(subForgeNextLevelParams[0].SafeToInt32(0)) < num)
		{
			this.QDSNubText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
		}
		else
		{
			this.QDSNubText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
		}
		int num2 = subForgeNextLevelParams[2].SafeToInt32(0);
		num2 = Global.RecalcNeedYinLiang(num2);
		this.YinLiangText.Text = num2.ToString();
		if (Global.Data.roleData.YinLiang < num2)
		{
			this.YinLiangText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
		}
		else
		{
			this.YinLiangText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
		}
		this.OddsText.Text = StringUtil.substitute("100%", new object[0]);
	}

	public void NotifyForgeResult(GoodsData goodsData, int result, int dbID, int addPropIndex, bool down, int binding)
	{
		this.Forging = false;
		this.progressBar.Visibility = false;
		this.progressBar.Percent = 0.0;
		Global.Data.GameScene.RemoveForgeDecoration();
		this.RefreshGoodsCount();
		this.levelImg.Level = goodsData.AddPropIndex;
		this.InitSubForgeNeedGoodsInfo(goodsData);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		string title = goodsXmlNodeByID.Title;
		if (result < 1)
		{
			if (result != 0)
			{
				if (result == -1)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("要精锻的【{0}】不在背包中"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else if (result == -2)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的千锻石数量不足, 需要{0}块千锻石"), new object[]
					{
						this.NeedNum
					}), 0, -1, -1, 0);
				}
				else if (result == -3)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				}
				else if (result == -4)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别，无法精锻"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else if (result == -5)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang(" 精锻【{0}】到{1}级，扣除{2}个金币失败"), new object[]
					{
						title,
						goodsData.Forge_level + 1,
						this.NeedYinLiang
					}), 0, -1, -1, 0);
				}
				else if (result == -10)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精锻【{0}】时更新数据失败"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else if (result == -9998)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经不在背包中，无法精锻"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else if (result == -9999)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】佩戴在身上时无法精锻"), new object[]
					{
						title
					}), 0, -1, -1, 0);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精锻【{0}】时发生错误:{1}"), new object[]
					{
						title,
						result
					}), 0, -1, -1, 0);
				}
			}
			return;
		}
		Super.InitEquipGIcon(this.ZBGicon, goodsData, false, IconTextTypes.Qianghua);
		Global.Data.GameScene.ExternalPlayDeco(60004, 0, 0);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功地将【{0}】精锻到了{1}级"), new object[]
		{
			title,
			goodsData.AddPropIndex
		}), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 1
			});
		}
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("SubForgePart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(100.0);
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
		this._Timer.Tick = null;
		this._Timer = null;
		this._TimerCount = 0;
	}

	private void ForgeTimer_Tick(object sender, object e)
	{
		this._TimerCount++;
		this.progressBar.Percent = (double)this._TimerCount / 12.0;
		if ((double)this._TimerCount >= 12.0)
		{
			this.StopHeart();
			GameInstance.Game.SpriteSubForgeGoods(this.ForgeDbID, this.RockGoodsID);
		}
	}

	public void InitPartData()
	{
		this.equipIcon = new ObservableCollection[2];
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[1] = this.Rock.ItemsSource;
	}

	private void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = totalGoodsCountByID.ToString();
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableHandCursor = true;
			if (totalGoodsCountByID > 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(gicon);
		}
	}

	public void ClearAllValues()
	{
		this.equipIcon[0].Clear();
		this.RefreshGoodsCount();
	}

	public void RefreshGoodsCount()
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[1][0]);
		if (null != gicon)
		{
			int itemCode = gicon.ItemCode;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(itemCode);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(itemCode);
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			gicon.Text = totalGoodsCountByID.ToString();
			if (totalGoodsCountByID > 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
		}
	}

	private void XYMouseLeftButtonUp(object sender, MouseEvent e)
	{
		Point position = new global::MousePosition(e).GetPosition(this.XYfu);
		this.ShowMenuWindow(position.X, position.Y + 200, this.tpMenuItemIDs, this.tiMenuItemNames);
	}

	public void ShowMenuWindow(int px, int py, int[] ids, string[] names)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)px;
		this.MenuWindow.Top = (double)py;
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 120.0;
		this.MenuWindow.BodyHeight = (double)((ids.Length + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Container.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
		}
		this.menuPart.RenderMenu(21);
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GMenuItem gmenuItem = s as GMenuItem;
			if (null == gmenuItem)
			{
				return;
			}
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.ProcessMenuClick(gmenuItem.MenuItemID);
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void ProcessMenuClick(int id)
	{
		this.SelectedMenuItemID = id;
		if (id == 1)
		{
			this.XinyunNubText.Text = "1";
			this.AddGoodsIcon(this.GlobalForgeLuckyGoodsID, 3, 1);
		}
		else if (id == 2)
		{
			this.XinyunNubText.Text = "2";
			this.AddGoodsIcon(this.GlobalForgeLuckyGoodsID, 3, 2);
		}
		if (this.equipIcon[0].Length <= 0)
		{
			this.OddsText.Text = StringUtil.substitute("0%", new object[0]);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		this.OddsText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Global.GetForgePercent(goodsData, this.GetLuckyNum()).ToString()
		});
	}

	private bool CanValClick()
	{
		if (this.equipIcon[0] == null || this.equipIcon[0].Length <= 0)
		{
			return false;
		}
		int luckyID = Convert.ToInt32(this.XinyunNubText.Text);
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		return Global.GetForgePercent(goodsData, luckyID) < 100;
	}

	private void ProcessValClick()
	{
		int iNeedNub = Convert.ToInt32(this.XinyunNubText.Text);
		this.AddGoodsIcon(this.GlobalForgeLuckyGoodsID, 3, iNeedNub);
		if (this.equipIcon[0].Length <= 0)
		{
			this.OddsText.Text = StringUtil.substitute("0%", new object[0]);
			return;
		}
		if (!this.LuckyCheckBox.Check)
		{
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		this.OddsText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Global.GetForgePercent(goodsData, this.GetLuckyNum()).ToString()
		});
	}

	private void InputEquipMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (Super.RemoveSystemNaviBox(this.Container, Global.GetLang("装备强化UI"), null))
		{
			this.ToHintStartForce = true;
			string taskPropNameByID = Global.GetTaskPropNameByID(170402);
			string title = null;
			int num = 0;
			int num2 = 0;
			Global.ParsePropNameInfo(taskPropNameByID, out title, out num, out num2);
			int num3 = ConfigGoods.FindGoodsIDByName(title);
		}
		(sender as GIcon).Cursor = Cursors.Auto;
		ObjectClickGetingMgr.StartClickGetThing(11, e);
	}

	private int GetLuckyNum()
	{
		if (!this.LuckyCheckBox.Check)
		{
			return 0;
		}
		return Convert.ToInt32(this.XinyunNubText.Text);
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private TextBlock MoneyText = new TextBlock();

	private ListBox Equip = new ListBox();

	private ListBox Rock = new ListBox();

	private ListBox Fu = new ListBox();

	private ListBox XYfu = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine XinyunNubText;

	private GTextBlockOutLine YinLiangText;

	private GTextBlockOutLine QDSNubText;

	private GTextBlockOutLine OddsText;

	private GCheckBox LuckyCheckBox;

	private int GlobalForgeLuckyGoodsID = -1;

	private GGoodIcon ZBGicon;

	private int SelectedMenuItemID = 1;

	private GImgLevel levelImg = U3DUtils.NEW<GImgLevel>();

	private GProgressBar progressBar;

	private int ForgeDbID = -1;

	private int RockGoodsID = -1;

	private int NeedNum = -1;

	private int NeedYinLiang = -1;

	private int _TimerCount;

	private DispatcherTimer _Timer;

	private int[] tpMenuItemIDs = new int[]
	{
		1,
		2
	};

	private string[] tiMenuItemNames = new string[]
	{
		Global.GetLang("1个"),
		Global.GetLang("2个")
	};

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private ObservableCollection[] _equipIcon;

	private bool _Forging;

	private bool ToHintStartForce;
}
