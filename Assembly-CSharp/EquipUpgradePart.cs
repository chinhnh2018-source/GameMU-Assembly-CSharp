using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class EquipUpgradePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 35.0;
		this.Equip.Height = 35.0;
		this.Equip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Equip, 48);
		Canvas.SetTop(this.Equip, 132);
		this.Container.Children.Add(this.JinJieS);
		this.JinJieS.Width = 35.0;
		this.JinJieS.Height = 35.0;
		this.JinJieS.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.JinJieS, 138);
		Canvas.SetTop(this.JinJieS, 132);
		this.Container.Children.Add(this.XuanTie);
		this.XuanTie.Width = 35.0;
		this.XuanTie.Height = 35.0;
		this.XuanTie.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.XuanTie, 184);
		Canvas.SetTop(this.XuanTie, 132);
		this.Container.Children.Add(this.JingJin);
		this.JingJin.Width = 35.0;
		this.JingJin.Height = 35.0;
		this.JingJin.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.JingJin, 230);
		Canvas.SetTop(this.JingJin, 132);
		this.Container.Children.Add(this.XingY);
		this.XingY.Width = 35.0;
		this.XingY.Height = 35.0;
		this.XingY.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.XingY, 276);
		Canvas.SetTop(this.XingY, 132);
		this.Container.Children.Add(this.NewEquip);
		this.NewEquip.Width = 35.0;
		this.NewEquip.Height = 35.0;
		this.NewEquip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.NewEquip, 48);
		Canvas.SetTop(this.NewEquip, 275);
	}

	public Dictionary<int, ObservableCollection> equipIcon
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

	public bool Upgrading
	{
		get
		{
			return this._Upgrading;
		}
		set
		{
			this._Upgrading = value;
		}
	}

	public ImageBrush BodyBackground
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
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("装备进阶");
		Canvas.SetLeft(gicon, 133);
		Canvas.SetTop(gicon, 346);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EquipUpgradeMouseLeftButtonUp);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("放入装备");
		gicon.Tip = "PutEquipBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 28);
		Canvas.SetTop(gicon, 92);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "IronCB";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = true;
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 194);
		Canvas.SetTop(gcheckBox, 173);
		this.Container.Children.Add(gcheckBox);
		this.IronCheckBox = gcheckBox;
		gcheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.equipIcon[0].Count > 0 && this.equipIcon[5].Count > 0)
			{
				if ((s as GCheckBox).Check)
				{
					(U3DUtils.AS<GIcon>(this.equipIcon[5][0]).ItemObject as GoodsData).Forge_level = (U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Forge_level;
				}
				else
				{
					(U3DUtils.AS<GGoodIcon>(this.equipIcon[5][0]).ItemObject as GoodsData).Forge_level = 0;
				}
				Super.InitEquipGIcon(U3DUtils.AS<GGoodIcon>(this.equipIcon[5][0]), U3DUtils.AS<GIcon>(this.equipIcon[5][0]).ItemObject as GoodsData, false, IconTextTypes.Qianghua);
			}
		};
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "GoldCB";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = true;
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 239);
		Canvas.SetTop(gcheckBox, 173);
		this.Container.Children.Add(gcheckBox);
		this.GoldCheckBox = gcheckBox;
		gcheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.equipIcon[0].Count > 0 && this.equipIcon[5].Count > 0)
			{
				if ((s as GCheckBox).Check)
				{
					(U3DUtils.AS<GIcon>(this.equipIcon[5][0]).ItemObject as GoodsData).Quality = (U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Quality;
				}
				else
				{
					(U3DUtils.AS<GIcon>(this.equipIcon[5][0]).ItemObject as GoodsData).Quality = 0;
				}
				Super.InitEquipGIcon(U3DUtils.AS<GGoodIcon>(this.equipIcon[5][0]), U3DUtils.AS<GGoodIcon>(this.equipIcon[5][0]).ItemObject as GoodsData, false, IconTextTypes.Qianghua);
			}
		};
		gcheckBox = new GCheckBox();
		gcheckBox.Name = "XYCB";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 287);
		Canvas.SetTop(gcheckBox, 173);
		this.Container.Children.Add(gcheckBox);
		this.XYCheckBox = gcheckBox;
		gcheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.equipIcon[0].Count <= 0)
			{
				this.JiLvText.Text = StringUtil.substitute("0%", new object[0]);
				return;
			}
			GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
			this.JiLvText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Global.GetJinjieNextPercent(Super.GData.UpgradeEquipGoodsData.GoodsID, this.GetLuckyNum()).ToString()
			});
		};
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.FontSize = 12;
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
		this.YinLiangText = gtextBlockOutLine;
		Canvas.SetLeft(gtextBlockOutLine, 243);
		Canvas.SetTop(gtextBlockOutLine, 274);
		this.Container.Children.Add(gtextBlockOutLine);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.FontSize = 12;
		gtextBlockOutLine.Text = "0%";
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
		this.JiLvText = gtextBlockOutLine;
		Canvas.SetLeft(gtextBlockOutLine, 243);
		Canvas.SetTop(gtextBlockOutLine, 295);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private void InputEquipMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (Super.RemoveSystemNaviBox(this.Container, Global.GetLang("装备进阶UI"), null))
		{
			this.ToHintStartUpgrade = true;
			string taskPropNameByID = Global.GetTaskPropNameByID(20411);
			string title;
			int num;
			int num2;
			Global.ParsePropNameInfo(taskPropNameByID, out title, out num, out num2);
			int id = ConfigGoods.FindGoodsIDByName(title);
			GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(id);
			goodVO = ConfigGoods.GetGoodsXmlNodeByCatSuitID(goodVO.Categoriy, goodVO.SuitID - 1, goodVO.ToSex, goodVO.ToOccupation);
			if (goodVO != null)
			{
			}
		}
		(sender as GIcon).Cursor = Cursors.Auto;
		ObjectClickGetingMgr.StartClickGetThing(3, e);
	}

	private object CopyUpgradeEquipGoodsData(GoodsData goodsData, GoodVO goodVO)
	{
		int suitID = goodVO.SuitID;
		if (suitID >= Global.MaxSuitID)
		{
			return null;
		}
		int categoriy = goodVO.Categoriy;
		int toOccupation = goodVO.ToOccupation;
		int toSex = goodVO.ToSex;
		GoodVO goodsXmlNodeByCatSuitID = ConfigGoods.GetGoodsXmlNodeByCatSuitID(categoriy, suitID + 1, toSex, toOccupation);
		int id = goodsXmlNodeByCatSuitID.ID;
		return new GoodsData
		{
			BagIndex = goodsData.BagIndex,
			Binding = goodsData.Binding,
			Endtime = goodsData.Endtime,
			Forge_level = ((!this.IronCheckBox.Check) ? 0 : goodsData.Forge_level),
			GCount = goodsData.GCount,
			GoodsID = id,
			Id = 10000,
			Jewellist = goodsData.Jewellist,
			Props = goodsData.Props,
			Quality = ((!this.GoldCheckBox.Check) ? 0 : goodsData.Quality),
			SaleMoney1 = goodsData.SaleMoney1,
			SaleYinPiao = goodsData.SaleYinPiao,
			SaleYuanBao = goodsData.SaleYuanBao,
			Site = goodsData.Site,
			Starttime = goodsData.Starttime,
			Using = goodsData.Using,
			AddPropIndex = goodsData.AddPropIndex,
			BornIndex = goodsData.BornIndex,
			Lucky = goodsData.Lucky,
			Strong = goodsData.Strong,
			ExcellenceInfo = goodsData.ExcellenceInfo,
			AppendPropLev = goodsData.AppendPropLev,
			ChangeLifeLevForEquip = goodsData.ChangeLifeLevForEquip
		};
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 3)
		{
			return;
		}
		if (this.Upgrading)
		{
			return;
		}
		int clickGetThingDbID = clickGetThingEventArgs.ClickGetThingDbID;
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(clickGetThingDbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				if (goodsXmlNodeByID.SuitID < Global.MaxSuitID)
				{
					this.AddEquipGoodsIcon(goodsDataByDbID, 0, goodsXmlNodeByID, false, 0);
					Super.GData.UpgradeEquipGoodsData = (this.CopyUpgradeEquipGoodsData(goodsDataByDbID, goodsXmlNodeByID) as GoodsData);
					this.AddEquipGoodsIcon(Super.GData.UpgradeEquipGoodsData, 5, null, true, 13);
					this.AddGoodsIcon(Global.GetJinjieNextRocksGoodsID(Super.GData.UpgradeEquipGoodsData.GoodsID), 1, 0);
					U3DUtils.AS<GIcon>(this.equipIcon[1][0]).Text = Global.GetJinjieNextRocks(Super.GData.UpgradeEquipGoodsData.GoodsID).ToString();
					int num = Global.GetJinjieNextLevelYinLiang(Super.GData.UpgradeEquipGoodsData.GoodsID);
					num = Global.RecalcNeedYinLiang(num);
					this.YinLiangText.Text = num.ToString();
					if (Global.Data.roleData.YinLiang < num)
					{
						this.YinLiangText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
					}
					else
					{
						this.YinLiangText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
					}
					this.JiLvText.Text = StringUtil.substitute("{0}%", new object[]
					{
						Global.GetJinjieNextPercent(Super.GData.UpgradeEquipGoodsData.GoodsID, this.GetLuckyNum()).ToString()
					});
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经到了最高阶， 不能再进阶"), new object[0]), 0, -1, -1, 0);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有装备才能进阶"), new object[0]), 0, -1, -1, 0);
			}
		}
		if (this.ToHintStartUpgrade)
		{
			Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("装备进阶UI"), 660000, 0, 1);
		}
	}

	private void EquipUpgradeMouseLeftButtonUp(object sender, MouseEvent e)
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("装备进阶UI"), null);
		if (this.equipIcon[0].Count <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将要进阶的装备放到装备位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进阶的装备不在背包中，无法进阶"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodVO == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进阶的装备在配置表中不存在，无法进阶"), new object[0]), 0, -1, -1, 0);
			return;
		}
		string title = goodVO.Title;
		int suitID = goodVO.SuitID;
		if (suitID >= Global.MaxSuitID)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高阶数，无法进阶"), new object[]
			{
				title
			}), 0, -1, -1, 0);
			return;
		}
		int categoriy = goodVO.Categoriy;
		int toOccupation = goodVO.ToOccupation;
		int toSex = goodVO.ToSex;
		GoodVO goodsXmlNodeByCatSuitID = ConfigGoods.GetGoodsXmlNodeByCatSuitID(categoriy, suitID + 1, toSex, toOccupation);
		if (goodsXmlNodeByCatSuitID == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("下阶装备在配置表中不存在，无法进阶"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int toLevel = goodsXmlNodeByCatSuitID.ToLevel;
		if (toLevel > Global.Data.roleData.Level)
		{
			string text = StringUtil.substitute(Global.GetLang("进阶后的装备使用要求的级别是{0}, 高于您目前的{1}级, 进阶成功后，您将暂时无法佩戴新装备到身上"), new object[]
			{
				toLevel,
				Global.Data.roleData.Level
			});
			text += StringUtil.substitute(Global.GetLang(", 继续进阶吗？"), new object[0]);
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), text, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.TryStartEquipUpgrade(goodsData, goodVO);
				}
				return true;
			};
		}
		else
		{
			this.TryStartEquipUpgrade(goodsData, goodVO);
		}
	}

	private void TryStartEquipUpgrade(GoodsData goodsData, GoodVO goodVO)
	{
		bool flag = goodsData.Binding > 0;
		bool flag2 = false;
		int num = Global.GetJinjieNextRocksGoodsID(Super.GData.UpgradeEquipGoodsData.GoodsID);
		if (Global.GetTotalGoodsCountByID(num) < Global.GetJinjieNextRocks(Super.GData.UpgradeEquipGoodsData.GoodsID))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中的进阶石数量不足，无法进阶"), new object[0]), 0, -1, -1, 0);
			return;
		}
		flag2 = (flag2 || Global.GetTotalBindingGoodsCountByID(num) > 0);
		string text = string.Empty;
		if (!this.IronCheckBox.Check)
		{
			if (goodsData.Forge_level > 0)
			{
				text = StringUtil.substitute(Global.GetLang("不使用镇星石，获取的新装备的等级将为0"), new object[0]);
			}
		}
		else
		{
			num = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieIronGoodsID");
			if (Global.GetTotalGoodsCountByID(num) < 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中的镇星石数量不足，无法进阶"), new object[0]), 19, -1, -1, num);
				return;
			}
			flag2 = (flag2 || Global.GetTotalBindingGoodsCountByID(num) > 0);
		}
		if (!this.GoldCheckBox.Check)
		{
			if (goodsData.Quality > 0)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text += StringUtil.substitute(Global.GetLang("不使用镇魂石，获取的新装备的品质将为白色"), new object[0]);
			}
		}
		else
		{
			num = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieGoldGoodsID");
			if (Global.GetTotalGoodsCountByID(num) < 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中的镇魂石数量不足，无法进阶"), new object[0]), 19, -1, -1, num);
				return;
			}
			flag2 = (flag2 || Global.GetTotalBindingGoodsCountByID(num) > 0);
		}
		if (text.Length > 0)
		{
			if (!flag && flag2)
			{
				text += StringUtil.substitute(Global.GetLang(", 进阶使用的材料中有绑定的材料, 进阶后的装备将被绑定, 为避免此情况，请将背包中绑定的材料转移到随身仓库"), new object[0]);
			}
			text += StringUtil.substitute(Global.GetLang(", 继续进阶吗？"), new object[0]);
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), text, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.StartEquipUpgrade(goodVO);
				}
				return true;
			};
		}
		else if (!flag && flag2)
		{
			text += StringUtil.substitute(Global.GetLang("进阶使用的材料中有绑定的材料, 进阶后的装备将被绑定, 为避免此情况，请将背包中绑定的材料转移到随身仓库"), new object[0]);
			text += StringUtil.substitute(Global.GetLang(", 继续进阶吗？"), new object[0]);
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), text, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.StartEquipUpgrade(goodVO);
				}
				return true;
			};
		}
		else
		{
			this.StartEquipUpgrade(goodVO);
		}
	}

	public void InitPartData()
	{
		this.YinLiangText.Text = "0";
		this.JiLvText.Text = "0%";
		this.InitIcon();
		Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("装备进阶UI"), 20411, Super.GetTaskStateByID(20411), 1);
	}

	private void InitIcon()
	{
		this.equipIcon = new Dictionary<int, ObservableCollection>();
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[1] = this.JinJieS.ItemsSource;
		this.equipIcon[2] = this.XuanTie.ItemsSource;
		this.AddGoodsIcon((int)ConfigSystemParam.GetSystemParamIntByName("JinjieIronGoodsID"), 2, 1);
		this.equipIcon[3] = this.JingJin.ItemsSource;
		this.AddGoodsIcon((int)ConfigSystemParam.GetSystemParamIntByName("JinjieGoldGoodsID"), 3, 1);
		this.equipIcon[4] = this.XingY.ItemsSource;
		this.AddGoodsIcon((int)ConfigSystemParam.GetSystemParamIntByName("JinjieLuckyGoodsID"), 4, 1);
		this.equipIcon[5] = this.NewEquip.ItemsSource;
	}

	public override void Destroy()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("装备进阶UI"), null);
		this.Container.Children.Clear();
		Super.GData.UpgradeEquipGoodsData = null;
	}

	private void AddGoodsIcon(int goodsID, int index, int needNum)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 12;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = needNum.ToString();
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableHandCursor = true;
			if (Global.GetTotalGoodsCountByID(goodsID) > 0)
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

	private void AddEquipGoodsIcon(GoodsData gd, int index, GoodVO goodVO = null, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.equipIcon[index].Clear();
		if (goodVO == null)
		{
			goodVO = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		}
		if (goodVO != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodVO), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodVO.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			ggoodIcon.ItemCategory = goodVO.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = 12;
			ggoodIcon.Text = ((gd.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
			{
				gd.Forge_level.ToString()
			}));
			ggoodIcon.TextSize = 12;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 4294901760U;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			Super.InitEquipGIcon(ggoodIcon, gd, false, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(ggoodIcon);
		}
	}

	public void ClearAllValues()
	{
		this.equipIcon[0].Clear();
		this.equipIcon[5].Clear();
		this.RefreshGoodsCount();
	}

	public void RefreshGoodsCount()
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[1][0]);
		if (null != gicon)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
			if (Global.GetTotalGoodsCountByID(gicon.ItemCode) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
		}
		gicon = U3DUtils.AS<GIcon>(this.equipIcon[2][0]);
		if (null != gicon)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
			if (Global.GetTotalGoodsCountByID(gicon.ItemCode) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
		}
		gicon = U3DUtils.AS<GIcon>(this.equipIcon[3][0]);
		if (null != gicon)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
			if (Global.GetTotalGoodsCountByID(gicon.ItemCode) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
		}
		gicon = U3DUtils.AS<GIcon>(this.equipIcon[4][0]);
		if (null != gicon)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
			if (Global.GetTotalGoodsCountByID(gicon.ItemCode) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
		}
	}

	private int GetLuckyNum()
	{
		if (!this.XYCheckBox.Check)
		{
			return 0;
		}
		return 1;
	}

	private void StartEquipUpgrade(GoodVO goodVO)
	{
		if (this.Upgrading)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经在进阶中..."), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.UpgradeDbID = -1;
		this.LuckyNum = 0;
		if (this.equipIcon[0].Count <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将要进阶的装备放到装备位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("目前将要进阶的装备不在背包中，无法进阶"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.NeedYinLiang = Global.GetJinjieNextLevelYinLiang(Super.GData.UpgradeEquipGoodsData.GoodsID);
		this.NeedYinLiang = Global.RecalcNeedYinLiang(this.NeedYinLiang);
		if (Global.Data.roleData.YinLiang < this.NeedYinLiang)
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("YinPiaoGoodsID");
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		}
		this.UpgradeDbID = goodsData.Id;
		this.LuckyNum = this.GetLuckyNum();
		if (this.LuckyNum > 0)
		{
			int num2 = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieLuckyGoodsID");
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(num2);
			if (totalGoodsCountByID < this.LuckyNum)
			{
				string goodsNameByID = Global.GetGoodsNameByID(num2, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的【{0}】数量不足, 需要{1}个【{2}】"), new object[]
				{
					goodsNameByID,
					this.LuckyNum,
					goodsNameByID
				}), 0, -1, -1, 0);
				return;
			}
		}
		int num3 = 0;
		if (this.IronCheckBox.Check)
		{
			num3 = 1;
			int num4 = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieIronGoodsID");
			int totalGoodsCountByID2 = Global.GetTotalGoodsCountByID(num4);
			if (totalGoodsCountByID2 < num3)
			{
				string goodsNameByID2 = Global.GetGoodsNameByID(num4, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的【{0}】数量不足, 需要{1}个【{2}】"), new object[]
				{
					goodsNameByID2,
					num3,
					goodsNameByID2
				}), 0, -1, -1, 0);
				return;
			}
		}
		int num5 = 0;
		if (this.GoldCheckBox.Check)
		{
			num5 = 1;
			int num6 = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieGoldGoodsID");
			int totalGoodsCountByID3 = Global.GetTotalGoodsCountByID(num6);
			if (totalGoodsCountByID3 < num5)
			{
				string goodsNameByID3 = Global.GetGoodsNameByID(num6, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的【{0}】数量不足, 需要{1}个【{2}】"), new object[]
				{
					goodsNameByID3,
					num5,
					goodsNameByID3
				}), 0, -1, -1, 0);
				return;
			}
		}
		this.Upgrading = true;
		GameInstance.Game.SpriteEquipUpgrade(this.UpgradeDbID, num3, num5, this.LuckyNum);
	}

	public void NotifyUpgradeResult(int dbID)
	{
		this.Upgrading = false;
		this.RefreshGoodsCount();
		if (dbID <= 0)
		{
			if (dbID == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("本次装备进阶失败！"), new object[0]), 0, -1, -1, 0);
			}
			else if (dbID == -9998)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经不在背包中，无法进阶"), new object[0]), 0, -1, -1, 0);
			}
			else if (dbID == -9999)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备佩戴在身上时无法进阶"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备进阶时发生错误:{0}"), new object[]
				{
					dbID
				}), 0, -1, -1, 0);
			}
			return;
		}
		this.equipIcon[0].Clear();
		U3DUtils.AS<GIcon>(this.equipIcon[1][0]).Text = "0";
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		this.AddEquipGoodsIcon(goodsDataByDbID, 5, null, false, 0);
		Global.Data.GameScene.ExternalPlayDeco(60005, 0, 0);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备进阶成功!"), new object[0]), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 1
			});
		}
	}

	private ListBox Equip = new ListBox();

	private ListBox JinJieS = new ListBox();

	private ListBox XuanTie = new ListBox();

	private ListBox JingJin = new ListBox();

	private ListBox XingY = new ListBox();

	private ListBox NewEquip = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine YinLiangText;

	private GTextBlockOutLine JiLvText;

	private GCheckBox IronCheckBox;

	private GCheckBox GoldCheckBox;

	private GCheckBox XYCheckBox;

	private int UpgradeDbID = -1;

	private int LuckyNum;

	private int NeedYinLiang = -1;

	private Dictionary<int, ObservableCollection> _equipIcon;

	private bool _Upgrading;

	private bool ToHintStartUpgrade;
}
