using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class LianluShengzhuangLianhuaPart : UserControl
{
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 32.0;
		this.Equip.Height = 32.0;
		this.Equip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Equip, 126);
		Canvas.SetTop(this.Equip, 13);
		this.Equip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			this.ClearEquip(1);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
		};
		this.Container.Children.Add(this.NewEquip);
		this.NewEquip.Width = 32.0;
		this.NewEquip.Height = 32.0;
		this.NewEquip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.NewEquip, 126);
		Canvas.SetTop(this.NewEquip, 114);
		this.Container.Children.Add(this.Rock);
		this.Rock.Width = 32.0;
		this.Rock.Height = 32.0;
		this.Rock.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Rock, 126);
		Canvas.SetTop(this.Rock, 189);
		this.DangqianJifenText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.DangqianJifenText.TextFontColor = new SolidColorBrush(uint.MaxValue);
		this.DangqianJifenText.Text = "0";
		this.DangqianJifenText.Height = 14.0;
		this.DangqianJifenText.TextSize = 12.0;
		Canvas.SetLeft(this.DangqianJifenText, 16);
		Canvas.SetTop(this.DangqianJifenText, 28);
		this.Container.Children.Add(this.DangqianJifenText);
		this.XiaohaoJifenText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.XiaohaoJifenText.TextFontColor = new SolidColorBrush(3669815U);
		this.XiaohaoJifenText.Text = "0";
		this.XiaohaoJifenText.Height = 14.0;
		this.XiaohaoJifenText.TextSize = 12.0;
		Canvas.SetLeft(this.XiaohaoJifenText, 118);
		Canvas.SetTop(this.XiaohaoJifenText, 235);
		this.Container.Children.Add(this.XiaohaoJifenText);
		this.ChenggonglvText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.ChenggonglvText.TextFontColor = new SolidColorBrush(3669815U);
		this.ChenggonglvText.Text = "0%";
		this.ChenggonglvText.Height = 14.0;
		this.ChenggonglvText.TextSize = 12.0;
		Canvas.SetLeft(this.ChenggonglvText, 226);
		Canvas.SetTop(this.ChenggonglvText, 235);
		this.Container.Children.Add(this.ChenggonglvText);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("炼 化");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EquipUpgradeMouseLeftButtonUp);
		Canvas.SetLeft(gicon, 104);
		Canvas.SetTop(gicon, 254);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		this.equipIcon = new ObservableCollection[3];
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[1] = this.NewEquip.ItemsSource;
		this.equipIcon[2] = this.Rock.ItemsSource;
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.SetShenqiZhihun();
	}

	public void SetShenqiZhihun()
	{
		this.DangqianJifenText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhuangBeiJiFen).ToString();
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 3)
		{
			this.equipIcon[index].Clear();
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(gd.GoodsID);
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 0
			});
			GoodsData upgradeEquipGoodsData = this.GetUpgradeEquipGoodsData(gd, categoriyByGoodsID);
			if (upgradeEquipGoodsData != null)
			{
				this.AddEquipGoodsIcon(upgradeEquipGoodsData, 1, false, 0);
			}
		}
	}

	private GoodsData GetUpgradeEquipGoodsData(GoodsData goodsData, int type)
	{
		GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodVO == null)
		{
			return null;
		}
		int suitID = goodVO.SuitID;
		int toSex = goodVO.ToSex;
		int toOccupation = goodVO.ToOccupation;
		XElement nextEquipXML = Global.GetNextEquipXML(suitID, type);
		if (nextEquipXML == null)
		{
			return null;
		}
		this.SetJinjieXiaohao(nextEquipXML);
		goodVO = ConfigGoods.GetGoodsXmlNodeByCatSuitID(type, Global.GetXElementAttributeInt(nextEquipXML, "SuitID"), toSex, toOccupation);
		int id = goodVO.ID;
		return new GoodsData
		{
			BagIndex = goodsData.BagIndex,
			Binding = goodsData.Binding,
			Endtime = goodsData.Endtime,
			Forge_level = goodsData.Forge_level,
			GCount = goodsData.GCount,
			GoodsID = id,
			Id = 10000,
			Jewellist = goodsData.Jewellist,
			Props = goodsData.Props,
			Quality = goodsData.Quality,
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

	private void SetJinjieXiaohao(XElement xml)
	{
		this.NeedJifen = Global.GetXElementAttributeInt(xml, "JiFen");
		this.NeedNum = Global.GetXElementAttributeInt(xml, "GoodsNum");
		this.XiaohaoJifenText.Text = this.NeedJifen.ToString();
		this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
		{
			Global.GetXElementAttributeStr(xml, "Succeed")
		});
		this.AddGoodsIcon(Global.GetXElementAttributeInt(xml, "NeedGoodsID"), 2, this.NeedNum);
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.equipIcon[index].Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
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

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
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
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = iNeedNub.ToString();
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

	private void EquipUpgradeMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.equipIcon[0].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将要炼化的神装放到装备位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		if (Global.GetGoodsDataByDbID(goodsData.Id, null) == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("要炼化的装备不在背包中，无法炼化"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经炼化到了最高阶数，无法炼化"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.GetRoleOwnNumByMoneyType(30) < this.NeedJifen)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("器魂不够, 炼化到下一阶神装需要{0}器魂"), new object[]
			{
				this.NeedJifen
			}), 0, -1, -1, 0);
			return;
		}
		int itemCode = U3DUtils.AS<GIcon>(this.equipIcon[2][0]).ItemCode;
		goodsData = Global.GetGoodsDataByID(itemCode);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有需要的聚灵珠，无法炼化"), new object[0]), 19, -1, -1, itemCode);
			return;
		}
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsData.GoodsID);
		if (totalGoodsCountByID < this.NeedNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的聚灵珠数量不足, 需要{0}个聚灵珠"), new object[]
			{
				this.NeedNum
			}), 0, -1, -1, 0);
			return;
		}
		if (this.ToLevel > Global.Data.roleData.Level)
		{
			string text = StringUtil.substitute(Global.GetLang("进阶后的装备使用要求的级别是{0}, 高于您目前的{1}级, 进阶成功后，您将暂时无法佩戴新装备到身上"), new object[]
			{
				this.ToLevel,
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
					this.StartEquipUpgrade();
				}
				return true;
			};
		}
		else
		{
			this.StartEquipUpgrade();
		}
	}

	private void StartEquipUpgrade()
	{
		this.ShowModalDialog();
		GameInstance.Game.SpriteEquipUpgrade((U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, 0, 0, 0);
	}

	public void NotifyUpgradeResult(int dbID)
	{
		this.CloseModalDialog();
		if (dbID < 0)
		{
			if (dbID == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("本次神装炼化失败！"), new object[0]), 0, -1, -1, 0);
			}
			else if (dbID == -9998)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经不在背包中，无法炼化"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备炼化时发生错误:{0}"), new object[]
				{
					dbID
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio(this.equip_failed, false);
			if (this.equipIcon[2].Length > 0)
			{
				this.AddGoodsIcon(U3DUtils.AS<GIcon>(this.equipIcon[2][0]).ItemCode, 2, this.NeedNum);
			}
			return;
		}
		this.DPEffectItem(this, new NotifyLianluEffectEventArgs
		{
			EffectID = 1,
			PlayID = 1
		});
		this.equipIcon[0].Clear();
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		Global.PlaySoundAudio(this.equip_ok, false);
		this.AddEquipGoodsIcon(goodsDataByDbID, 0, false, 0);
		Global.Data.GameScene.ExternalPlayDeco(60005, 0, 0);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("神装炼化成功!"), new object[0]), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 1
			});
		}
		this.SetShenqiZhihun();
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	private Canvas PlaceHolder;

	private ListBox Equip = new ListBox();

	private ListBox NewEquip = new ListBox();

	private ListBox Rock = new ListBox();

	private GTextBlockOutLine DangqianJifenText;

	private GTextBlockOutLine XiaohaoJifenText;

	private GTextBlockOutLine ChenggonglvText;

	private int NeedJifen = -1;

	private int NeedNum = -1;

	private int ToLevel = -1;

	private ObservableCollection[] _equipIcon;

	private bool _Upgrading;

	private string equip_ok = StringUtil.substitute("Media/Music/UI/equip_ok.mp3", new object[0]);

	private string equip_failed = StringUtil.substitute("Media/Music/UI/equip_failed.mp3", new object[0]);
}
