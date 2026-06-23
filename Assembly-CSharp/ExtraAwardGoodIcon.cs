using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class ExtraAwardGoodIcon : UserControl
{
	public GGoodIcon goodsIcon
	{
		get
		{
			return this.icon;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public GoodsData goodsData
	{
		set
		{
			this._goodsData = value;
			if (this._goodsData != null)
			{
				this.AddExtraAwardGoodsIcon(this._goodsData);
			}
		}
	}

	public bool gray
	{
		set
		{
			this._gray = value;
		}
	}

	public bool dragable
	{
		set
		{
			this._dragable = value;
		}
	}

	private void AddExtraAwardGoodsIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			this.icon = U3DUtils.NEW<GGoodIcon>();
			this.icon.Width = 78.0;
			this.icon.Height = 78.0;
			this.icon.BackSpriteName0 = backSpriteName;
			this.icon.TipType = 1;
			this.icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			this.icon.ItemCode = gd.GoodsID;
			this.icon.ItemObject = gd;
			this.icon.BoxTypes = -1;
			if (!this._gray)
			{
				this.icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				this.icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(this.icon, gd, canUse, IconTextTypes.Qianghua);
			if (this._dragable)
			{
				this.icon.gameObject.AddComponent<UIDragPanelContents>();
			}
			U3DUtils.AddChild(base.gameObject, this.icon.gameObject, false);
		}
	}

	private GGoodIcon icon;

	private GoodsData _goodsData;

	private bool _gray;

	private bool _dragable;
}
