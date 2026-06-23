using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RideFashionItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
			this._UsedImage.URL = "NetImages/GameRes/Images/Fashionwardrobe/IsUsing.png";
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void itemClick(GameObject e)
	{
		if (this.Hander != null)
		{
			this.Hander(e, new DPSelectedItemEventArgs
			{
				Type = 1,
				ID = this.mGoodsData.Id
			});
		}
	}

	public void SetData(GoodsData Goods)
	{
		if (Goods != null)
		{
			this.mGoodsData = Goods;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.mGoodsData.GoodsID);
			if (null == this.mIcon)
			{
				this.mIcon = Global.GetNewGoodIcon();
			}
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			this.mIcon.GoodImg.URL = goodsImageURLFromIconCode;
			Super.InitGoodsGIcon(this.mIcon, Goods, true, IconTextTypes.Qianghua);
			this.mIcon.ItemObject = Goods;
			this.mIcon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			U3DUtils.AddChild(this._GoodsRoot, this.mIcon.gameObject, true);
			this.mIcon.GetComponent<BoxCollider>().enabled = false;
			this.mIcon.STextVisibility = false;
			this.Level = Goods.Forge_level;
			this._UsedImage.gameObject.SetActive(1 == Goods.Using);
			UIPanel component = this.mIcon.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public UIDraggablePanel DragPane
	{
		set
		{
			UIDragPanelContents uidragPanelContents = base.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
			BoxCollider component2 = base.gameObject.GetComponent<BoxCollider>();
			if (null != component2)
			{
				component2.size = new Vector3(84f, 84f, 0f);
			}
			UIEventListener.Get(base.gameObject).onClick = delegate(GameObject g)
			{
				this.itemClick(g);
			};
		}
	}

	public int Level
	{
		set
		{
			if (null != this.mIcon)
			{
				if (value == 0)
				{
					this.mIcon.ContentText.text = string.Empty;
				}
				else
				{
					this.mIcon.ContentText.text = "+" + value;
				}
			}
		}
	}

	public bool bSelect
	{
		get
		{
			return NGUITools.GetActive(this._SelectSp.gameObject);
		}
		set
		{
			this._SelectSp.gameObject.SetActive(value);
		}
	}

	public int ID
	{
		get
		{
			if (this.mGoodsData != null)
			{
				return this.mGoodsData.Id;
			}
			return 0;
		}
	}

	public GoodsData GoodsData
	{
		get
		{
			return this.mGoodsData;
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GameObject _GoodsRoot;

	[SerializeField]
	private UISprite _SelectSp;

	[SerializeField]
	private ShowNetImage _UsedImage;

	private GGoodIcon mIcon;

	private GoodsData mGoodsData;
}
