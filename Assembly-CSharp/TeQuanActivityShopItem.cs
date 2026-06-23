using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeQuanActivityShopItem : UserControl
{
	public int ID
	{
		get
		{
			return this.mID;
		}
	}

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
			UIEventListener.Get(base.gameObject).onClick = delegate(GameObject g)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						MyID = this.mID,
						ID = this.mGoodsData.GoodsID,
						CanUse = this.mCanBuyNum,
						buyFrom = this.mPrice
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public GGoodIcon AddGoodIcon(GoodsData gd)
	{
		if (null != this.mIcon)
		{
			return this.mIcon;
		}
		return U3DUtils.NEW<GGoodIcon>();
	}

	private void RefreshGoodsIcon(GGoodIcon icon, GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.SecondText.Text = gd.GCount.ToString();
		icon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		icon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2.5f);
		icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		if (Global.GetZhuoyueAttributeCount(gd) >= 5)
		{
			icon.TeXiao.gameObject.SetActive(true);
			GameObject gameObject = Resources.Load("UITeXiao/Qifu/GaoJiXuanZhuan") as GameObject;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.15f);
			U3DUtils.AddPrefab(icon.TeXiao.gameObject, gameObject, true);
			gameObject = (Resources.Load("UITeXiao/Qifu/GaoJiShanGuang") as GameObject);
			U3DUtils.AddPrefab(icon.TeXiao.gameObject, gameObject, true);
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null != ggoodIcon && ggoodIcon.ItemObject != null)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData != null)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			}
		}
	}

	public int Price
	{
		set
		{
			this.mPrice = value;
			this._GoodsPrice.text = this.mPrice.ToString();
		}
	}

	public void SetData(TeQuanShangChengVO vo)
	{
		if (vo != null)
		{
			this.mID = vo.ID;
			GoodsData goodsDataByStr = Global.GetGoodsDataByStr(vo.WuPinID, 0);
			if (goodsDataByStr != null)
			{
				if (null == this.mIcon)
				{
					this.mIcon = this.AddGoodIcon(goodsDataByStr);
				}
				this.mIcon.transform.SetParent(this._GoodsRoot, false);
				if (this.mGoodsData == null || !this.mGoodsData.Equals(goodsDataByStr))
				{
					this.mGoodsData = goodsDataByStr;
					this.RefreshGoodsIcon(this.mIcon, goodsDataByStr);
				}
				this._GoodsName.text = Global.GetGoodsNameByID(goodsDataByStr.GoodsID, true);
				this.Price = vo.JiaGe;
				this.CanBuyNum = vo.GouMaiCiShu;
			}
		}
	}

	public int CanBuyNum
	{
		set
		{
			this.mCanBuyNum = value;
			this._GoodsCanBuyNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e42e",
				Global.GetLang("购买数量："),
				(0 < this.mCanBuyNum) ? "17e42e" : "ff0000",
				value.ToString()
			});
		}
	}

	public UIDraggablePanel DragPanel
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
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private Transform _GoodsRoot;

	[SerializeField]
	private UILabel _GoodsName;

	[SerializeField]
	private UILabel _GoodsPrice;

	[SerializeField]
	private UILabel _GoodsCanBuyNum;

	private int mID;

	private GGoodIcon mIcon;

	private GoodsData mGoodsData;

	private int mCanBuyNum;

	private int mPrice;
}
