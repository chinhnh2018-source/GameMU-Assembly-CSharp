using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LibaoPart : UserControl
{
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

	public int GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.GoodsList.ItemsSource;
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.SubmitBtn.isEnabled)
			{
				return;
			}
			GoodsData goodsDataByID = Global.GetGoodsDataByID(this.GoodsID);
			if (goodsDataByID != null)
			{
				if (Global.GetCategoriyByGoodsID(goodsDataByID.GoodsID) == 704)
				{
					GameInstance.Game.SendTOUseTaLuopaiSuiPian(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
				}
				else
				{
					GameInstance.Game.SpriteUseGoods(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
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
			else
			{
				string goodsNameByID = Global.GetGoodsNameByID(goodsDataByID.GoodsID, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}无法使用"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
		};
	}

	private bool GetRoleLevelVsGoodsLevel(int goodsID, out string str)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsID);
		int toZhuanSheng = goodsXmlNodeByID.ToZhuanSheng;
		int toLevel = goodsXmlNodeByID.ToLevel;
		if (toZhuanSheng == 0)
		{
			str = string.Format(Global.GetLang("{0}"), toLevel);
		}
		else
		{
			str = string.Format(Global.GetLang("{0}[{1}]转"), toLevel, toZhuanSheng);
		}
		if (Global.Data.roleData.ChangeLifeCount == toZhuanSheng)
		{
			if (Global.Data.roleData.Level < toLevel)
			{
				return false;
			}
		}
		else if (Global.Data.roleData.ChangeLifeCount < toZhuanSheng)
		{
			return false;
		}
		return true;
	}

	public void InitPartSize()
	{
		string empty = string.Empty;
		string text = "FFFFFF";
		if (this.GetRoleLevelVsGoodsLevel(this.GoodsID, out empty))
		{
			this.SubmitBtn.isEnabled = true;
		}
		else
		{
			text = "FF0000";
			this.SubmitBtn.isEnabled = false;
		}
		this.HintText.Text = string.Format(Global.GetLang("等级达到{0}可开启"), Global.GetColorStringForNGUIText(new object[]
		{
			text,
			empty
		}));
	}

	public void InitPartData()
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsID);
		string backSpriteName = "bagGrid2_bak";
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 301 && categoriy <= 302)
			{
				int baoguoID = goodsXmlNodeByID.BaoguoID;
				if (baoguoID > 0)
				{
					Global.Data.ViewGoodsPackDataList = Global.UnPackGoodsID(baoguoID);
					if (Global.Data.ViewGoodsPackDataList != null)
					{
						for (int i = 0; i < Global.Data.ViewGoodsPackDataList.Count; i++)
						{
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.ViewGoodsPackDataList[i].GoodsID);
							if (goodsXmlNodeByID2 != null)
							{
								string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID2), "NetImages/GameRes/");
								GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
								icon.Width = 78.0;
								icon.Height = 78.0;
								icon.BackgroundSprite0.transform.localScale = new Vector3(92f, 92f, 0f);
								icon.BackSpriteName0 = backSpriteName;
								icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
								icon.ItemCategory = goodsXmlNodeByID2.Categoriy;
								icon.ItemCode = Global.Data.ViewGoodsPackDataList[i].GoodsID;
								icon.ItemObject = Global.Data.ViewGoodsPackDataList[i];
								icon.BoxTypes = 1;
								icon.TextShadowColor = 4278190080U;
								icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
								{
									GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, icon.ItemObject as GoodsData);
								};
								bool canUse = Global.CanUseGoods(icon.ItemCode, false, true);
								Super.InitGoodsGIcon(icon, icon.ItemObject as GoodsData, canUse, IconTextTypes.Qianghua);
								this.ItemCollection.AddNoUpdate(icon);
							}
						}
						this.ItemCollection.DelayUpdate();
					}
				}
			}
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton CloseBtn;

	public GButton SubmitBtn;

	public TextBlock HintText;

	public ListBox GoodsList;

	private ObservableCollection _ItemCollection;

	private int _GoodsID;
}
