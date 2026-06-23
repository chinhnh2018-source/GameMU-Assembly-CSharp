using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class HintIconBar : UserControl
{
	public HintIconBar()
	{
		this.Width = (double)this.HINTICONBAR_WIDTH;
		this.Height = (double)this.HINTICONBAR_HEIGHT;
	}

	public void AddButton(int systemWizardType)
	{
	}

	private int FindTypeInArray(int type)
	{
		int result = -1;
		for (int i = 0; i < this.arrayItemCollection.Count; i++)
		{
			if (this.arrayItemCollection[i].ItemCode == type)
			{
				result = i;
			}
		}
		return result;
	}

	private void RemoveButton(int type)
	{
		if (this.arrayItemCollection != null)
		{
			int num = this.FindTypeInArray(type);
			if (num != -1)
			{
				this.arrayItemCollection.RemoveRange(num, 1);
				this.RefreshHintIconBar(true);
			}
		}
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		GIcon gicon = sender as GIcon;
		if (null != gicon)
		{
			int itemCode = gicon.ItemCode;
			switch (itemCode)
			{
			case 6:
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 3
					});
				}
				break;
			default:
				if (itemCode == 49)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 0,
							IDType = 1
						});
					}
				}
				break;
			case 9:
				this.ToUseGoods(this.GoodsDbID);
				Super.RemoveSystemNaviBox(this.Container, Global.GetLang("图标向导UI"), null);
				break;
			case 10:
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 8
					});
				}
				break;
			case 11:
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 9
					});
				}
				break;
			case 12:
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 5
					});
				}
				break;
			}
			this.RemoveButton(gicon.ItemCode);
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
	}

	private void RefreshHintIconBar(bool del = false)
	{
		if (this.arrayItemCollection != null)
		{
			int count = this.arrayItemCollection.Count;
			if (del)
			{
				this.Container.Clear();
				for (int i = 0; i < count; i++)
				{
					this.Container.Children.Add(this.arrayItemCollection[i]);
					Canvas.SetLeft(this.arrayItemCollection[i], this.HINTICONBAR_WIDTH - this.HINTICONBAR_HEIGHT * (i + 1) - i * this.HINTICONBAR_MARGIN);
					Canvas.SetTop(this.arrayItemCollection[i], 0);
				}
			}
			else
			{
				this.Container.Children.Add(this.arrayItemCollection[count - 1]);
				Canvas.SetLeft(this.arrayItemCollection[count - 1], this.HINTICONBAR_WIDTH - this.HINTICONBAR_HEIGHT * count - (count - 1) * this.HINTICONBAR_MARGIN);
				Canvas.SetTop(this.arrayItemCollection[count - 1], 0);
			}
		}
	}

	public int GoodsDbID
	{
		get
		{
			return this._GoodsDbID;
		}
		set
		{
			this._GoodsDbID = value;
		}
	}

	private void ToUseGoods(int goodsDbID)
	{
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(goodsDbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		if (!Global.CanUseGoods(goodsDataByDbID.GoodsID, true, true))
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		int num = goodsXmlNodeByID.Categoriy;
		if (num >= 0 && num < 25)
		{
			int actionType = goodsXmlNodeByID.ActionType;
			int handType = goodsXmlNodeByID.HandType;
			List<GoodsData> list;
			if (num >= 11 && num <= 21)
			{
				list = Super.FindWuQi(num, actionType, handType);
			}
			else
			{
				list = Super.FindEquip(num);
			}
			for (int i = 0; i < Enumerable.Count<GoodsData>(list); i++)
			{
				if (list[i] != null && list[i].Using == 1)
				{
					list[i].Using = 0;
					int handType2 = ConfigGoods.GetGoodsXmlNodeByID(list[i].GoodsID).HandType;
					if (num != 6 && handType2 != 2)
					{
						list[i].BagIndex = goodsDataByDbID.BagIndex;
					}
					GameInstance.Game.SpriteModGoods(2, list[i].Id, list[i].GoodsID, list[i].Using, list[i].Site, list[i].GCount, list[i].BagIndex, string.Empty);
				}
			}
			if (goodsDataByDbID.Using == 0)
			{
				goodsDataByDbID.Using = 1;
				if (num == 6 || handType == 2)
				{
					if (num >= 11 && num <= 21)
					{
						num = 11;
					}
					goodsDataByDbID.BagIndex = Super.FindEquipBagIndex(num);
				}
				GameInstance.Game.SpriteModGoods(1, goodsDataByDbID.Id, goodsDataByDbID.GoodsID, goodsDataByDbID.Using, goodsDataByDbID.Site, goodsDataByDbID.GCount, goodsDataByDbID.BagIndex, string.Empty);
			}
		}
		else if (!Super.IsDisableUsingGoods() && goodsDataByDbID != null)
		{
			if (!Global.GoodsCoolDown(goodsDataByDbID.GoodsID))
			{
				if (Global.GetCategoriyByGoodsID(goodsDataByDbID.GoodsID) == 704)
				{
					GameInstance.Game.SendTOUseTaLuopaiSuiPian(goodsDataByDbID.Id, goodsDataByDbID.GoodsID, 1);
				}
				else
				{
					GameInstance.Game.SpriteUseGoods(goodsDataByDbID.Id, goodsDataByDbID.GoodsID, 1);
				}
			}
			else
			{
				string goodsNameByID = Global.GetGoodsNameByID(goodsDataByDbID.GoodsID, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】在冷却中, 无法使用"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
		}
	}

	private int HINTICONBAR_WIDTH = 240;

	private int HINTICONBAR_HEIGHT = 40;

	private int HINTICONBAR_MARGIN;

	private List<GIcon> arrayItemCollection = new List<GIcon>();

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _GoodsDbID;
}
