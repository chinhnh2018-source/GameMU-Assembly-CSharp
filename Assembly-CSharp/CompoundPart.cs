using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class CompoundPart : UserControl
{
	public CompoundPart()
	{
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
		this.InfoText.TextFontWrapping = TextWrapping.Wrap;
		this.InfoText.TextHeight = 16.0;
		this.strText1.Text = string.Empty;
		this.sInfoNub.Text = Global.GetLang("合成数量：");
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.InfoText);
		Canvas.SetLeft(this.InfoText, 23);
		Canvas.SetTop(this.InfoText, 15);
		this.InfoText.TextLineHeight = 3.0;
		this.InfoText.FontSize = HSTextField.defaultFontSize;
		this.InfoText.Width = 205.0;
		this.InfoText.TextColor = new SolidColorBrush(11394222U);
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 2);
		Canvas.SetTop(this.listBox, 71);
		this.listBox.Width = 232.0;
		this.listBox.Height = 60.0;
		this.Container.Children.Add(this.strText1);
		Canvas.SetLeft(this.strText1, 19);
		Canvas.SetTop(this.strText1, 132);
		this.strText1.FontSize = HSTextField.defaultFontSize;
		this.strText1.TextFontColor = new SolidColorBrush(4289584302U);
		this.Container.Children.Add(this.SuccessRateText);
		Canvas.SetLeft(this.SuccessRateText, 130);
		Canvas.SetTop(this.SuccessRateText, 160);
		this.SuccessRateText.FontSize = HSTextField.defaultFontSize;
		this.SuccessRateText.TextFontColor = new SolidColorBrush(3669815U);
		this.Container.Children.Add(this.sInfoNub);
		Canvas.SetLeft(this.sInfoNub, 10);
		Canvas.SetTop(this.sInfoNub, 207);
		this.sInfoNub.FontSize = HSTextField.defaultFontSize;
		this.sInfoNub.TextFontColor = new SolidColorBrush(11394222U);
		this.Container.Children.Add(this.img);
		Canvas.SetLeft(this.img, 110);
		Canvas.SetTop(this.img, 86);
		this.img.Source = new ImageBrush(this.backImg2);
		this.img.Visibility = false;
		this.Container.Children.Add(this.dragDropListTarget);
		this.dragDropListTarget.AllowDrop = true;
		Canvas.SetLeft(this.dragDropListTarget, 113);
		Canvas.SetTop(this.dragDropListTarget, 89);
		Canvas.SetZIndex(this.dragDropListTarget, 1.0);
		this.dragDropListTarget.Children.Add(this.list);
		this.dragDropListTarget.Visibility = false;
		this.list.Width = 32.0;
		this.list.Height = 32.0;
		this.list.Background = new SolidColorBrush(540994U);
		this.list.BackgroundAlpha = 0.01;
		this.dragDropListTarget.AllowDrop = true;
		this.dragDropListTarget.Drop = new EventHandler(this.DragDrop);
		this.dragDropListTarget.ItemDroppedOnTarget = new EventHandler(this.ItemDroppedOnTarget);
		this.dragDropListTarget.ItemDragCompleted = new EventHandler(this.ItemDragCompleted);
		this.ItemCollectionList = this.list.ItemsSource;
	}

	public override void Destroy()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("合成子窗口UI"), null);
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "StartSynthesis";
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("批量合成");
		Canvas.SetLeft(gicon, 154);
		Canvas.SetTop(gicon, 202);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartHeCheng();
		};
		this.GoodsNum = U3DUtils.NEW<GTextBlock>();
		this.GoodsNum.BodyWidth = 35.0;
		this.GoodsNum.BodyHeight = 21.0;
		this.GoodsNum.EditText = "1";
		this.GoodsNum.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 35.0, 21.0, 3.0, 2.0));
		this.GoodsNum.Onlydouble = true;
		this.GoodsNum.Text.border = false;
		this.GoodsNum.Text.TextBoxChanged = new EventHandler(this.GoodsNum_TextChanged);
		Canvas.SetLeft(this.GoodsNum, 80);
		Canvas.SetTop(this.GoodsNum, 205);
		this.Container.Children.Add(this.GoodsNum);
		this.iconsub = U3DUtils.NEW<GIcon>();
		this.iconsub.Width = 17.0;
		this.iconsub.Height = 16.0;
		this.iconsub.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/SubNum_Normal.png"));
		this.iconsub.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/sub.png"));
		this.iconsub.DisableBodySource = this.iconsub.BodySource;
		Canvas.SetLeft(this.iconsub, 120);
		Canvas.SetTop(this.iconsub, 215);
		this.Container.Children.Add(this.iconsub);
		this.iconsub.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			int num = 1;
			try
			{
				num = Convert.ToInt32(this.GoodsNum.Text.Text);
				if (num > 1)
				{
					num--;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				num = 1;
			}
			this.GoodsNum.Text.Text = num.ToString();
		};
		this.iconAdd = U3DUtils.NEW<GIcon>();
		this.iconAdd.Width = 17.0;
		this.iconAdd.Height = 16.0;
		this.iconAdd.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/AddNum_Normal.png"));
		this.iconAdd.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/add.png"));
		this.iconAdd.DisableBodySource = this.iconAdd.BodySource;
		Canvas.SetLeft(this.iconAdd, 120);
		Canvas.SetTop(this.iconAdd, 199);
		this.Container.Children.Add(this.iconAdd);
		this.iconAdd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			int num = 1;
			try
			{
				num = Convert.ToInt32(this.GoodsNum.Text.Text);
				if (num < 99)
				{
					num++;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				num = 1;
			}
			this.GoodsNum.Text.Text = num.ToString();
		};
	}

	public void InitPartData(string sItemID)
	{
		this.InitItemID = string.Empty;
		this.InitItemID = sItemID;
		this.ItemCollection.Clear();
		XElement isolateResXml = Global.GetIsolateResXml("Config/GoodsMergeItems.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Item", "ID", sItemID);
		if (xelement == null)
		{
			return;
		}
		if (Global.GetXElementAttributeInt(xelement, "MergeType") == 101)
		{
			this.dragDropListTarget.Visibility = true;
			this.img.Visibility = true;
		}
		this.MergeItemID = Convert.ToInt32(sItemID);
		string text = string.Empty;
		string text2 = string.Empty;
		this.InfoText.Text = Global.GetXElementAttributeStr(xelement, "Description");
		this.iMoney = Global.GetXElementAttributeInt(xelement, "Money");
		this.iJiFen = Global.GetXElementAttributeInt(xelement, "JiFen");
		this.iJingYuan = Global.GetXElementAttributeInt(xelement, "JingYuan");
		this.iUccessRate = Global.GetXElementAttributeInt(xelement, "SuccessRate");
		this.strText1.Text = StringUtil.substitute(Global.GetLang("需要消耗：{0} {1} {2}"), new object[]
		{
			(this.iMoney <= 0) ? string.Empty : (Global.GetLang("金币：") + this.iMoney),
			(this.iJingYuan <= 0) ? string.Empty : (Global.GetLang("精元：") + this.iJingYuan),
			(this.iJiFen <= 0) ? string.Empty : (Global.GetLang("神器之魂：") + this.iJiFen)
		});
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.iMoney || Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < this.iJingYuan || Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhuangBeiJiFen) < this.iJiFen)
		{
			this.strText1.TextColor = new SolidColorBrush(4294901760U);
		}
		else
		{
			this.strText1.TextColor = new SolidColorBrush(uint.MaxValue);
		}
		this.SuccessRateText.Text = StringUtil.substitute("{0}%", new object[]
		{
			this.iUccessRate
		});
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "OrigGoodsIDs");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			string[] array = Global.StringTrim(xelementAttributeStr).Split(new char[]
			{
				'|'
			});
			if (array.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				text = string.Empty;
				text = array[i];
				text2 = text.Substring(0, text.IndexOf(","));
				string text3 = text.Substring(text.IndexOf(",") + 1, text.Length - text.IndexOf(",") - 1);
				CompoundItem compoundItem = U3DUtils.NEW<CompoundItem>();
				compoundItem.Width = 45.0;
				compoundItem.Height = 60.0;
				compoundItem.GoodsImgBacks = this.backImg;
				compoundItem.GoodsImgs = this.GetIcon(Convert.ToInt32(text2), text3);
				compoundItem.NeedGoodsNum = Convert.ToInt32(text3);
				Canvas.SetZIndex(compoundItem.GoodsImg, 100.0);
				this.ItemCollection.AddNoUpdate(compoundItem);
			}
			this.ItemCollection.DelayUpdate();
		}
		Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("合成子窗口UI"), 170401, Super.GetTaskStateByID(170401), 1);
	}

	private GGoodIcon GetIcon(int goodsID, string sCout)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty);
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsID,
			0,
			-1,
			-1
		});
		ggoodIcon.ItemCode = goodsID;
		ggoodIcon.ItemObject = null;
		ggoodIcon.BoxTypes = 5;
		ggoodIcon.Text = sCout;
		ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
		ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.TextColor = ColorSL.FromArgb(255, 0, 255, 0);
		if (Global.GetTotalGoodsCountByID(goodsID) < Convert.ToInt32(sCout))
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 255, 0, 0);
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		return ggoodIcon;
	}

	public bool FindBindingGoods()
	{
		bool result = false;
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			GGoodIcon goodsImgs = U3DUtils.AS<CompoundItem>(this.ItemCollection[i]).GoodsImgs;
			if (Global.GetTotalBindingGoodsCountByID(goodsImgs.ItemCode) > 0)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void RefreshGoodsCount()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			GGoodIcon goodsImgs = U3DUtils.AS<CompoundItem>(this.ItemCollection[i]).GoodsImgs;
			if (Global.GetTotalGoodsCountByID(goodsImgs.ItemCode) < U3DUtils.AS<CompoundItem>(this.ItemCollection[i]).NeedGoodsNum)
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsImgs.ItemCode), string.Empty);
				goodsImgs.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
				goodsImgs.TextColor = ColorSL.FromArgb(255, 255, 0, 0);
			}
			else
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsImgs.ItemCode), string.Empty);
				goodsImgs.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				goodsImgs.TextColor = ColorSL.FromArgb(255, 0, 255, 0);
			}
		}
	}

	private string GetMergeItemTitle()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/GoodsMergeItems.Xml");
		if (isolateResXml == null)
		{
			return string.Empty;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Item", "ID", this.MergeItemID.ToString());
		if (xelement == null)
		{
			return string.Empty;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NewGoodsID");
		return Global.GetGoodsNameByID(xelementAttributeInt, false);
	}

	public void RefreshData(int retCode)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.RefreshGoodsCount();
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.iMoney || Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < this.iJingYuan || Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhuangBeiJiFen) < this.iJiFen)
		{
			this.strText1.TextColor = new SolidColorBrush(4294901760U);
		}
		else
		{
			this.strText1.TextColor = new SolidColorBrush(uint.MaxValue);
		}
		if (retCode >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜您, 合成【{0}】成功!"), new object[]
			{
				this.GetMergeItemTitle()
			}), 0, -1, -1, 0);
		}
		else
		{
			if (retCode == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成【{0}】失败，请检查您的背包是否已满"), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
			}
			else if (retCode == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成【{0}】失败，请检查您的材料是否足够"), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
			}
			else if (retCode == -3)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
			else if (retCode == -4)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			}
			else if (retCode == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成【{0}】失败，请检查您的真气是否足够"), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
			}
			else if (retCode == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成【{0}】失败，请检查您的神器之魂是否足够"), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
			}
			else if (retCode == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成【{0}】失败，请检查您的天地精元是否足够"), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成【{0}】失败..."), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
			}
			if (retCode != -1000)
			{
				this.EndHeCheng();
			}
		}
	}

	private void GoodsNum_TextChanged(object sender, object e)
	{
		int num = 1;
		try
		{
			num = Convert.ToInt32(this.GoodsNum.EditText);
			if (num >= 99)
			{
				num = 99;
			}
			if (num == 0)
			{
				num = 1;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 1;
		}
		this.GoodsNum.EditText = num.ToString();
	}

	private void StartHeCheng()
	{
		GIcon gicon = this.Container.FindName("StartSynthesis").SafeGetComponent<GIcon>();
		gicon.EnableIcon = false;
		this.GoodsNum.mouseEnabled = false;
		this.iconsub.EnableIcon = false;
		this.iconAdd.EnableIcon = false;
		this.StartHeart();
	}

	public void EndHeCheng()
	{
		this.StopHeart();
		this.Destroy();
		this.IsSendHeChengCmd = false;
		GIcon gicon = this.Container.FindName("StartSynthesis").SafeGetComponent<GIcon>();
		gicon.EnableIcon = true;
		this.iCiShu = 0;
		this.GoodsNum.mouseEnabled = true;
		this.iconsub.EnableIcon = true;
		this.iconAdd.EnableIcon = true;
	}

	private void BatchSynthesis()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("合成子窗口UI"), null);
		if (this.MergeItemID >= 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.iMoney)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("无法合成【{0}】, 您的金币不足！"), new object[]
				{
					this.GetMergeItemTitle()
				}), 0, -1, -1, 0);
				this.EndHeCheng();
				return;
			}
			if (this.LuckyGoodsData != null && Global.GetTotalGoodsCountByID(this.LuckyGoodsData.GoodsID) <= 0)
			{
				this.ItemCollectionList.Clear();
				this.SuccessRateText.Text = StringUtil.substitute("{0}%", new object[]
				{
					this.iUccessRate
				});
			}
			GameInstance.Game.SpriteGoodsMergeMsg(this.MergeItemID, (this.LuckyGoodsData == null) ? 0 : this.LuckyGoodsData.GoodsID, -1, -1, 1);
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
		}
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("SynthesisPart_Timer");
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
		if (this._TimerCount >= 15)
		{
			this.iCiShu++;
			if (this.iCiShu > Convert.ToInt32(this.GoodsNum.Text.Text))
			{
				this.EndHeCheng();
				return;
			}
			if (!this.IsSendHeChengCmd)
			{
				this.Destroy();
				this.BatchSynthesis();
			}
		}
	}

	public void DragDrop(object sender, object e)
	{
		if (this.list != Super.GData.DragingListBox && null != Super.GData.DragingItem && Super.GData.DragingItem.BoxTypes == 1)
		{
			this.LuckyGoodsData = (Super.GData.DragingItem.ItemObject as GoodsData);
			if (this.LuckyGoodsData.Using <= 0)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeLuckyGoodsIDs", ',');
				int num = Array.IndexOf<int>(systemParamIntArrayByName, this.LuckyGoodsData.GoodsID);
				if (num == -1)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只能放置幸运符！"), new object[0]), 0, -1, -1, 0);
					return;
				}
				this.AddGoodsIcon(this.LuckyGoodsData.GoodsID, this.LuckyGoodsData.GCount, this.LuckyGoodsData.Quality, this.LuckyGoodsData.Forge_level, this.LuckyGoodsData.AddPropIndex, this.LuckyGoodsData.BornIndex, this.LuckyGoodsData.Binding, true, this.LuckyGoodsData.Id);
				int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeLuckyGoodsRate", ',');
				this.SuccessRateText.Text = StringUtil.substitute("{0}%", new object[]
				{
					(this.iUccessRate + Convert.ToInt32(systemParamIntArrayByName2[num]) <= 100) ? (this.iUccessRate + systemParamIntArrayByName2[num]) : 100
				});
			}
		}
	}

	private void ItemDroppedOnTarget(object sender, object e)
	{
	}

	private void ItemDragCompleted(object sender, object e)
	{
		(e as DragDropEventArgs).Cancel = true;
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int addPropIndex, int bornIndex, int binding, bool clear = false, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(Id, null);
			goodsDataByDbID.Id = Id;
			this.ItemCollectionList.Clear();
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
				goodsDataByDbID.Id,
				20
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = goodsDataByDbID;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = gcount.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			Super.InitGoodsGIcon(ggoodIcon, goodsDataByDbID, true, IconTextTypes.Qianghua);
			this.ItemCollectionList.Add(ggoodIcon);
		}
	}

	private UserControl thisCtrl;

	private LoadingWindow LoadingWin;

	private BitmapData backImg = Global.GetGameResImage("Images/Plate/rm_listItem.png");

	private BitmapData backImg2 = Global.GetGameResImage("Images/Plate/rm_listItem2.png");

	private Image img = new Image();

	private int MergeItemID = -1;

	private string InitItemID = string.Empty;

	private int iMoney;

	private int iJiFen;

	private int iJingYuan;

	private int _TimerCount;

	private bool IsSendHeChengCmd;

	private GoodsData LuckyGoodsData;

	private GTextBlockOutLine InfoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine strText1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine SuccessRateText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine sInfoNub = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlock GoodsNum;

	private DispatcherTimer _Timer;

	public ObservableCollection ItemCollectionList;

	public ObservableCollection ItemCollection;

	private GIcon iconAdd;

	private GIcon iconsub;

	private FixedListBoxDragDropTarget dragDropListTarget = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox list = new ListBox();

	private int iUccessRate;

	private int iCiShu;
}
