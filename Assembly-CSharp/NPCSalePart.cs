using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class NPCSalePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.ReturnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
			SystemHelpMgr.OnAction(UIObjIDs.NPCSalePart, HelpStateEvents.Inactived, 1);
		};
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

	public void UpdateMoneyText()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	private void BuyInGoodsClick(object sender, MouseEvent e)
	{
		if (null == this.CurrentSelectedItem)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请先选中要购买的物品!"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GameInstance.Game.SpriteBuyGoods(this.CurrentSelectedItem.GoodsID, 1, this.SaleType);
	}

	private string GetGoodsDataName(int goodsID, int num)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID == null)
		{
			return string.Empty;
		}
		return StringUtil.substitute(Global.GetLang("{0}(个数:{1})"), new object[]
		{
			goodsXmlNodeByID.Title,
			num
		});
	}

	private void BuyOutGoodsClick(object sender, MouseEvent e)
	{
	}

	public void RecalculateSaleType()
	{
	}

	public void InitPartData(int npcID)
	{
		if (Super._ParcelPart == null)
		{
			ParcelPart parcelPart = U3DUtils.NEW<ParcelPart>();
			parcelPart.iBaoGuoMode = 4;
			parcelPart.InitPartData();
			Super._ParcelPart = parcelPart;
			this.BaoGuoCanvas.Children.Add(Super._ParcelPart);
		}
		else
		{
			Super._ParcelPart.iBaoGuoMode = 4;
			Super._ParcelPart.Visibility = true;
			this.BaoGuoCanvas.Children.Add(Super._ParcelPart);
		}
		if (Convert.ToString(this.SaleID) != string.Empty)
		{
			XElement xelement = Global.GetXElement(Global.GetGameResXml("Config/NPCSaleList.Xml"), "Sale", "ID", Convert.ToString(this.SaleID));
			if (xelement != null)
			{
				this.SaleType = Global.GetXElementAttributeInt(xelement, "SaleType");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Items");
				this.NPCSaleItemFields = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
			}
			if (this.SaleID == 1 || this.SaleID == 11)
			{
				this.Title.spriteName = "wuqidian";
			}
			else if (this.SaleID == 2 || this.SaleID == 21)
			{
				this.Title.spriteName = "fangjudian";
			}
			else
			{
				this.Title.spriteName = "yaodian";
			}
		}
		this.CurrentSelectedPage = 0;
		this.ShowPage(this.CurrentSelectedPage);
		SystemHelpMgr.OnAction(UIObjIDs.NPCSalePart, HelpStateEvents.Actived, 1);
	}

	private NPCSaleItem GetNPCSaleItem(string item)
	{
		string[] array = item.Split(new char[]
		{
			','
		});
		if (array.Length <= 0)
		{
			return null;
		}
		int num = Convert.ToInt32(array[0]);
		int forgeLevel = Convert.ToInt32(array[1]);
		int zhuijiaLevel = Convert.ToInt32(array[2]);
		int zhuoyueIndex = Convert.ToInt32(array[4]);
		int lucky = Convert.ToInt32(array[3]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			NPCSaleItem npcsaleItem = U3DUtils.NEW<NPCSaleItem>();
			npcsaleItem.ItemName.textColor = Global.GetColorByGoodsData(dummyGoodsDataMu);
			npcsaleItem.ItemName.Text = string.Format("{0}", goodsXmlNodeByID.Title);
			npcsaleItem.GoodsID = Convert.ToInt32(num);
			npcsaleItem.goodsIcon = this.GetIcon(goodsXmlNodeByID, dummyGoodsDataMu);
			npcsaleItem.MoneyText.Text = Global.GetGoodsPriceByMoneyType(num, this.SaleType).ToString();
			return npcsaleItem;
		}
		return null;
	}

	private void ClearPage()
	{
		this.ItemCollection.Clear();
	}

	private void ShowPage(int pageIndex)
	{
		this.ItemCollection.Clear();
		this.CurrentSelectedItem = null;
		for (int i = 0; i < this.NPCSaleItemFields.Length; i++)
		{
			NPCSaleItem npcsaleItem = this.GetNPCSaleItem(this.NPCSaleItemFields[i]);
			if (null != npcsaleItem)
			{
				this.ItemCollection.Add(npcsaleItem);
				if (this.SaleType == 13)
				{
					npcsaleItem.MoneyTeypImg.spriteName = "moneyJifen";
				}
				else if (this.SaleType == 1)
				{
					npcsaleItem.MoneyTeypImg.spriteName = "moneyBindJinbi";
				}
				else if (this.SaleType == 40)
				{
					npcsaleItem.MoneyTeypImg.spriteName = "moneyZhuanshi";
				}
				else if (this.SaleType == 8)
				{
					npcsaleItem.MoneyTeypImg.spriteName = "moneyJinbi";
				}
			}
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		NPCSaleItem npcsaleItem = U3DUtils.AS<NPCSaleItem>(this.listBox.SelectedItem);
		if (null == npcsaleItem)
		{
			return;
		}
		if (this.tempItem != null && this.tempItem != npcsaleItem)
		{
			this.tempItem.SetSelectStat = false;
		}
		this.tempItem = npcsaleItem;
		npcsaleItem.SetSelectStat = true;
		GTipServiceEx.SelfBagOnly = false;
		GoodsPriceUnitTypes goodsPriceUnit = GoodsPriceUnitTypes.Jinbi;
		MoneyTypes saleType = (MoneyTypes)this.SaleType;
		if (saleType != MoneyTypes.TongQian)
		{
			if (saleType != MoneyTypes.YinLiang)
			{
				if (saleType != MoneyTypes.JingYuanZhi)
				{
					if (saleType == MoneyTypes.YuanBao)
					{
						goodsPriceUnit = GoodsPriceUnitTypes.Zhuanshi;
					}
				}
				else
				{
					goodsPriceUnit = GoodsPriceUnitTypes.Jifen;
				}
			}
			else
			{
				goodsPriceUnit = GoodsPriceUnitTypes.Jinbi;
			}
		}
		else
		{
			goodsPriceUnit = GoodsPriceUnitTypes.BindJinBi;
		}
		GTipServiceEx.ShowTip(npcsaleItem.goodsIcon, TipTypes.GoodsText, GoodsOwnerTypes.NPCSale, goodsPriceUnit, Convert.ToInt32(npcsaleItem.MoneyText.Text), npcsaleItem.goodsIcon.ItemObject as GoodsData);
	}

	private GGoodIcon GetIcon(GoodVO goodVO, GoodsData gd)
	{
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodVO.IconCode, string.Empty);
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 64.0;
		icon.Height = 64.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCategory = goodVO.Categoriy;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = -1;
		bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
		Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
		icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
		{
			if (ev.IDType == 8 && ev.ID > 0)
			{
				GameInstance.Game.SpriteBuyGoods(icon.ItemCode, ev.ID, this.SaleType);
			}
		};
		return icon;
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id < 1000)
			{
				GameObject at = this.listBox.ItemsSource.GetAt(id);
				if (null != at)
				{
					SystemHelpPart.SetMask(at.transform, default(Vector4));
				}
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.ReturnBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private NPCSaleItem CurrentSelectedItem;

	private int CurrentSelectedPage;

	private string[] NPCSaleItemFields;

	public Canvas BaoGuoCanvas;

	public ListBox listBox;

	public GButton ReturnBtn;

	public UISprite Title;

	public int SaleID = -1;

	private ObservableCollection ItemCollection;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	private int SaleType;

	private NPCSaleItem tempItem;
}
