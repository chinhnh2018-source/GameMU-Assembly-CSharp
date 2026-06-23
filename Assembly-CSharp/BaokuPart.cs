using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class BaokuPart : UserControl
{
	public BaokuPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.ItemCollection2 = this.listBoxDest.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.listBoxDest);
		this.listBoxDest.Width = 480.0;
		this.listBoxDest.Height = 32.0;
		Canvas.SetLeft(this.listBoxDest, 153);
		Canvas.SetTop(this.listBoxDest, 42);
		this.listBoxDest.Background = new SolidColorBrush(16777215U);
		this.listBoxDest.BorderThickness = 0;
		this.listBoxDest.ItemMargin = new Thickness(0.0, 0.0, 65.0, 0.0);
		this.Container.Children.Add(this.txtRefreshNum);
		this.txtRefreshNum.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtRefreshNum, 429);
		Canvas.SetTop(this.txtRefreshNum, 268);
		this.Container.Children.Add(this.txtYGMDNum);
		this.txtYGMDNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtYGMDNum, 561);
		Canvas.SetTop(this.txtYGMDNum, 268);
		this.Container.Children.Add(this.txtHJZQNum);
		this.txtHJZQNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtHJZQNum, 561);
		Canvas.SetTop(this.txtHJZQNum, 290);
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 580.0;
		this.listBox.Height = 111.0;
		Canvas.SetLeft(this.listBox, 29);
		Canvas.SetTop(this.listBox, 128);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.Container.Children.Add(this.Mask);
		this.Mask.Width = 580.0;
		this.Mask.Height = 111.0;
		Canvas.SetLeft(this.Mask, 29);
		Canvas.SetTop(this.Mask, 128);
		Canvas.SetZIndex(this.Mask, 100.0);
		this.Mask.Background = new SolidColorBrush(4278190080U);
		this.Mask.Opacity = 0.01;
		this.Mask.Visibility = false;
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "AotoBuy";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.Text = Global.GetLang("自动购买昆仑镜");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 475);
		Canvas.SetTop(gcheckBox, 338);
		this.Container.Children.Add(gcheckBox);
		this.AutoBuyCheckBox = gcheckBox;
		this.progressBar = U3DUtils.NEW<GProgressBar>();
		this.progressBar.BodyWidth = 321.0;
		this.progressBar.BodyHeight = 5.0;
		this.progressBar.ForeBrush = new ImageBrush(Global.GetGameResImage("Images/Plate/zuoji_jinjie_progressbar_bak.png"));
		Canvas.SetLeft(this.progressBar, 74);
		Canvas.SetTop(this.progressBar, 282);
		this.Container.Children.Add(this.progressBar);
		this.progressBar.Percent = 0.0;
		this.Container.Children.Add(this.progressText);
		this.progressText.TextColor = new SolidColorBrush(46850U);
		Canvas.SetLeft(this.progressText, 209);
		Canvas.SetTop(this.progressText, 262);
		this.progressText.Text = "100/200";
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
		this.DuiHuanIcon = U3DUtils.NEW<GIcon>();
		this.DuiHuanIcon.Width = 66.0;
		this.DuiHuanIcon.Height = 21.0;
		this.DuiHuanIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.DuiHuanIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.DuiHuanIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.DuiHuanIcon.Text = Global.GetLang("兑换");
		this.DuiHuanIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.DuiHuanIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 1
				});
			}
		};
		Canvas.SetLeft(this.DuiHuanIcon, 405);
		Canvas.SetTop(this.DuiHuanIcon, 272);
		this.Container.Children.Add(this.DuiHuanIcon);
		this.StartIcon = U3DUtils.NEW<GIcon>();
		this.StartIcon.Width = 80.0;
		this.StartIcon.Height = 21.0;
		this.StartIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.StartIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.StartIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.StartIcon.Text = Global.GetLang("开始选宝");
		this.StartIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.StartIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSelectBaoku();
		};
		Canvas.SetLeft(this.StartIcon, 261);
		Canvas.SetTop(this.StartIcon, 333);
		this.Container.Children.Add(this.StartIcon);
		this.RefreshIcon = U3DUtils.NEW<GIcon>();
		this.RefreshIcon.Width = 112.0;
		this.RefreshIcon.Height = 21.0;
		this.RefreshIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		this.RefreshIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		this.RefreshIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 112.0, 21.0, 3.0, 2.0));
		this.RefreshIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.RefreshIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.FreeRefreshAllGoods();
		};
		Canvas.SetLeft(this.RefreshIcon, 130);
		Canvas.SetTop(this.RefreshIcon, 333);
		this.Container.Children.Add(this.RefreshIcon);
		this.ReStartIcon = U3DUtils.NEW<GIcon>();
		this.ReStartIcon.Width = 80.0;
		this.ReStartIcon.Height = 21.0;
		this.ReStartIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.ReStartIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.ReStartIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.ReStartIcon.Text = Global.GetLang("再来一局");
		this.ReStartIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.ReStartIcon.Visibility = false;
		this.ReStartIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NextOpenYangGongBK();
		};
		Canvas.SetLeft(this.ReStartIcon, 168);
		Canvas.SetTop(this.ReStartIcon, 333);
		this.Container.Children.Add(this.ReStartIcon);
		GameInstance.Game.SpriteQueryYangGongBKDailyData();
	}

	private void FreeRefreshAllGoods()
	{
		double num = (double)DateTime.Now.Ticks;
		if (num - this.FreeRefreshAllGoodsTicks <= 3000000.0)
		{
			return;
		}
		this.FreeRefreshAllGoodsTicks = num;
		if (this._FreeRefreshNum >= Global.MaxFreeRefreshNum)
		{
			return;
		}
		this._FreeRefreshNum++;
		this.RefreshIcon.Text = StringUtil.substitute(Global.GetLang("免费刷新({0})"), new object[]
		{
			Global.MaxFreeRefreshNum - this._FreeRefreshNum
		});
		if (this._FreeRefreshNum >= Global.MaxFreeRefreshNum)
		{
			this.RefreshIcon.Visibility = false;
		}
		GameInstance.Game.SpriteFreeRefreshYangGongBK();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void NextOpenYangGongBK()
	{
		double num = (double)DateTime.Now.Ticks;
		if (num - this.NextOpenYangGongBKTicks <= 3000000.0)
		{
			return;
		}
		this.NextOpenYangGongBKTicks = num;
		GameInstance.Game.SpriteOpenYangGongBK(this.AutoBuyCheckBox.Check);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		this.ItemCollection2.Clear();
	}

	public void InitPartData()
	{
		this.yangGongMiDianGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("YangGongMiDianGoodsID");
		this.kunLunJingGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("KunLunJingGoodsID");
		this.RefreshGoodNum();
	}

	public void RefreshGoodNum()
	{
		this.txtHJZQNum.Text = Global.GetTotalGoodsCountByID(this.kunLunJingGoodsID).ToString();
	}

	public void StartFinishUI()
	{
		this._FreeRefreshNum = Global.MaxFreeRefreshNum;
		this.RefreshIcon.Text = StringUtil.substitute(Global.GetLang("免费刷新({0})"), new object[]
		{
			Global.MaxFreeRefreshNum - this._FreeRefreshNum
		});
		this.RefreshIcon.Visibility = false;
		this.StartIcon.Visibility = false;
		this.ReStartIcon.Visibility = true;
	}

	public void ReStartFinishUI()
	{
		this.ItemCollection2.Clear();
		this.ItemCollection.Clear();
		this._ClickBKNum = 0;
		this.PickUpDict.Clear();
		this.PositionDict.Clear();
		this._FreeRefreshNum = 0;
		this.RefreshIcon.Text = StringUtil.substitute(Global.GetLang("免费刷新({0})"), new object[]
		{
			Global.MaxFreeRefreshNum - this._FreeRefreshNum
		});
		this.ReStartIcon.Visibility = false;
		this.StartIcon.Visibility = true;
		this.RefreshIcon.Visibility = true;
		this.Mask.Visibility = false;
	}

	private void UnSelectItem()
	{
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	public void StartSelectBaoku()
	{
		if (this.Timer != null)
		{
			this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitOpenBaokuTimer_Tick);
			this.Timer.Stop();
			this.Timer = null;
		}
		this.StartFinishUI();
		this.ItemCollection.Clear();
		this.Mask.Visibility = true;
		for (int i = 0; i < 8; i++)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 68.0;
			gicon.Height = 111.0;
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int num = -1;
				if (num >= 0)
				{
					this.SelectListBox(num);
				}
				this.OpenBaoku();
			};
			BaokuListItem baokuListItem = U3DUtils.NEW<BaokuListItem>();
			gicon.Tag = baokuListItem;
			baokuListItem.BodyWidth = 68.0;
			baokuListItem.BodyHeight = 111.0;
			baokuListItem.BaokuBtn = gicon;
			this.ItemCollection.AddNoUpdate(baokuListItem);
		}
		this.ItemCollection.DelayUpdate();
		this.DecoMoveBaoku = Global.GetDecoration(502, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.DecoMoveBaoku.Coordinate = new Point(63, 191);
		this.WaitOpenBaoku();
	}

	private void WaitOpenBaoku()
	{
		this.Timer = new DispatcherTimer("WaitOpenBaoku_Timer");
		this.Timer.Interval = TimeSpan.FromMilliseconds(600.0);
		this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitOpenBaokuTimer_Tick);
		this.Timer.Start();
	}

	private void WaitOpenBaokuTimer_Tick(object sender, object e)
	{
		if (this.Timer != null)
		{
			this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitOpenBaokuTimer_Tick);
			this.Timer.Stop();
			this.Timer = null;
		}
		if (this.DecoMoveBaoku != null)
		{
			Global.RemoveObject(this.DecoMoveBaoku, true);
		}
		this.Mask.Visibility = false;
	}

	private void OpenBaoku()
	{
		if ((double)Global.GetCorrectLocalTime() - this.LastClickTicks <= 300.0)
		{
			return;
		}
		this.LastClickTicks = (double)Global.GetCorrectLocalTime();
		BaokuListItem baokuListItem = U3DUtils.AS<BaokuListItem>(this.listBox.SelectedItem);
		if (null == baokuListItem)
		{
			return;
		}
		if (this._ClickBKNum >= Global.MaxClickYangGongBKNum)
		{
			return;
		}
		int num = Global.CalcNeedKunLunJingNum(this._ClickBKNum);
		if (num <= 0)
		{
			this.OpenBaokuOk(this.listBox.SelectedIndex);
		}
		else
		{
			string goodsNameByID = Global.GetGoodsNameByID(this.kunLunJingGoodsID, false);
			string message = StringUtil.substitute(Global.GetLang("第{0}次杨公宝库选宝需要使用{1}个{2}，是否选宝？"), new object[]
			{
				this._ClickBKNum + 1,
				num,
				goodsNameByID
			});
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Root, 1, Global.GetLang("提示"), message, Super.GetChildLeft(630, 316), Super.GetChildTop(321, 128), 630, 321, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Root, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.OpenBaokuOk(this.listBox.SelectedIndex);
				}
				return true;
			};
		}
	}

	private void OpenBaokuOk(int bkIndex)
	{
		GameInstance.Game.SpriteClickYangGongBK(bkIndex, this.AutoBuyCheckBox.Check);
	}

	public void OnQueryYangGongBKDailyDataCompleted(YangGongBKDailyJiFenData dailyData)
	{
		if (dailyData != null)
		{
			this.progressText.Text = dailyData.JiFen + "/" + Global.GetYangGongBKMaxLuckyValue();
			this.progressBar.Percent = (double)dailyData.JiFen / Global.GetYangGongBKMaxLuckyValue();
		}
		else
		{
			this.progressText.Text = "0";
		}
	}

	public void WaitShowBaokuFinish()
	{
		if (this.Timer != null)
		{
			this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitOpenBaokuTimer_Tick);
			this.Timer.Stop();
			this.Timer = null;
		}
		this.Timer = new DispatcherTimer("WaitShowBaokuFinish_Timer");
		this.Timer.Interval = TimeSpan.FromMilliseconds(600.0);
		this.Mask.Visibility = true;
		this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitTimer_Tick);
		this.Timer.Start();
	}

	private void WaitTimer_Tick(object sender, object e)
	{
		if (this.Timer != null)
		{
			this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitOpenBaokuTimer_Tick);
			this.Timer.Stop();
			this.Timer = null;
		}
		this.ShowBaokuFinish();
	}

	private void ShowBaokuFinish()
	{
		this.Mask.Visibility = false;
		this.ItemCollection.Clear();
		List<GoodsData> list = Global.RandomSortList<GoodsData>(Super.GData.YangGongGoodsDataList);
		foreach (int num in this.PickUpDict.Keys)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Id == num)
				{
					list.RemoveRange(i, 1);
					break;
				}
			}
		}
		int num2 = 0;
		for (int j = 0; j < 8; j++)
		{
			BaokuListItem baokuListItem;
			if (this.PositionDict.ContainsKey(j))
			{
				baokuListItem = U3DUtils.NEW<BaokuListItem>();
				baokuListItem.BodyWidth = 68.0;
				baokuListItem.BodyHeight = 111.0;
				baokuListItem.Width = 68.0;
				baokuListItem.Height = 111.0;
			}
			else
			{
				baokuListItem = U3DUtils.NEW<BaokuListItem>();
				baokuListItem.BodyWidth = 68.0;
				baokuListItem.BodyHeight = 111.0;
				baokuListItem.Width = 68.0;
				baokuListItem.Height = 111.0;
				baokuListItem.GoodImg = this.GetGoodsIcon(list[num2++]);
				int num3 = 68 * j + 29 + 18;
				int num4 = 160;
				GDecoration decoration = Global.GetDecoration(32, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, -1, true, false);
				decoration.Coordinate = new Point(num3 + 20, num4 + 60);
			}
			this.ItemCollection.AddNoUpdate(baokuListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	private GGoodIcon GetGoodsIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
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
				goodsData.GoodsID,
				0,
				goodsData.Id,
				16
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			return ggoodIcon;
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
		if (this.Timer != null)
		{
			this.Timer.Tick = new DispatcherTimerEventHandler(this.WaitOpenBaokuTimer_Tick);
			this.Timer.Stop();
			this.Timer = null;
		}
		if (this.DecoMoveBaoku != null)
		{
			Global.RemoveObject(this.DecoMoveBaoku, true);
		}
		this.Mask.Visibility = false;
		Super.CleanUpAllChildWindows(this.Root);
	}

	public void NotifyOpenYangGongBKResult(List<GoodsData> goodsDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.GData.YangGongGoodsDataList = goodsDataList;
		this.RefreshGoodNum();
		this.ReStartFinishUI();
		if (goodsDataList == null || goodsDataList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < goodsDataList.Count; i++)
		{
			BaokuListItem baokuListItem = U3DUtils.NEW<BaokuListItem>();
			baokuListItem.BodyWidth = 68.0;
			baokuListItem.BodyHeight = 111.0;
			baokuListItem.BodyBackground = new ImageBrush(Global.GetGameResImage("Images/Plate/bp_empty.png"));
			baokuListItem.GoodImg = this.GetGoodsIcon(goodsDataList[i]);
			this.ItemCollection.AddNoUpdate(baokuListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	public void NotifyFreeRefreshYangGongBKResult(List<GoodsData> goodsDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (goodsDataList == null)
		{
			return;
		}
		Super.GData.YangGongGoodsDataList = goodsDataList;
		this.ItemCollection.Clear();
		if (goodsDataList == null || goodsDataList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < goodsDataList.Count; i++)
		{
			BaokuListItem baokuListItem = U3DUtils.NEW<BaokuListItem>();
			baokuListItem.BodyWidth = 68.0;
			baokuListItem.BodyHeight = 111.0;
			baokuListItem.BodyBackground = new ImageBrush(Global.GetGameResImage("Images/Plate/bp_empty.png"));
			baokuListItem.GoodImg = this.GetGoodsIcon(goodsDataList[i]);
			this.ItemCollection.AddNoUpdate(baokuListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	public void NotifyclickYangGongBKResult(int roleID, int goodsDbID, int bkIndex)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (goodsDbID < 0)
		{
			if (goodsDbID == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("杨公宝库中选宝超过了最大次数【{0}】"), new object[]
				{
					Global.MaxClickYangGongBKNum
				}), 0, -1, -1, 0);
			}
			else if (goodsDbID == -500)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请清理出空格后，再重新选宝"), new object[0]), 0, -1, -1, 0);
			}
			else if (goodsDbID == -700 || goodsDbID == -701)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("从杨公宝库中选宝时，背包中没有找到【{0}】"), new object[]
				{
					Global.GetGoodsNameByID(this.kunLunJingGoodsID, false)
				}), 19, -1, -1, this.kunLunJingGoodsID);
			}
			else if (goodsDbID == -2300)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("从杨公宝库中选宝自动购买【{0}】时，你的钻石不够"), new object[]
				{
					Global.GetGoodsNameByID(this.kunLunJingGoodsID, false)
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("从杨公宝库中选宝时发生错误: {0}"), new object[]
				{
					goodsDbID
				}), 0, -1, -1, 0);
			}
			return;
		}
		this.RefreshGoodNum();
		this._ClickBKNum++;
		this.PickUpDict[goodsDbID] = true;
		this.PositionDict[bkIndex] = true;
		int num = 68 * bkIndex + 29 + 18;
		int num2 = 160;
		GDecoration decoration = Global.GetDecoration(32, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, -1, true, false);
		decoration.Coordinate = new Point(num + 20, num2 + 60);
		foreach (int num3 in this.PositionDict.Keys)
		{
		}
		GoodsData yangGongBKGoodsDataByID = Super.GetYangGongBKGoodsDataByID(goodsDbID);
		this.ItemCollection2.Add(this.GetGoodsIcon(yangGongBKGoodsDataByID));
		if (this._ClickBKNum >= Global.MaxClickYangGongBKNum)
		{
			this.WaitShowBaokuFinish();
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private GDecoration DecoMoveBaoku;

	private GIcon StartIcon;

	private GIcon RefreshIcon;

	private GIcon ReStartIcon;

	private GIcon DuiHuanIcon;

	private DispatcherTimer Timer;

	private int _FreeRefreshNum;

	private int _ClickBKNum;

	private Dictionary<int, bool> PickUpDict = new Dictionary<int, bool>();

	private Dictionary<int, bool> PositionDict = new Dictionary<int, bool>();

	private double FreeRefreshAllGoodsTicks;

	private double NextOpenYangGongBKTicks;

	private int yangGongMiDianGoodsID;

	private int kunLunJingGoodsID;

	private double LastClickTicks;

	private Canvas Root;

	private ListBox listBoxDest = new ListBox();

	private GTextBlockOutLine txtRefreshNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYGMDNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtHJZQNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private Canvas Mask = new Canvas();

	private GCheckBox AutoBuyCheckBox;

	private GProgressBar progressBar;

	private GTextBlockOutLine progressText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection2;
}
