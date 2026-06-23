using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUJieriZengsongPiliangPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.initPrefabText();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.BtnZengsong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = Global.SafeConvertToInt32(this.LabNum.text)
			});
		};
		this.ScrollBarNum.scrollValue = 0f;
		this.ScrollBarNum.onChange = delegate(UIScrollBar sb)
		{
			this.LabNum.text = Mathf.RoundToInt(sb.scrollValue * (float)this.totalNum).ToString();
		};
		UIEventListener.Get(this.BtnJia.gameObject).onClick = delegate(GameObject s)
		{
			if (this.totalNum == 0)
			{
				return;
			}
			this.changeNum = Global.SafeConvertToInt32(this.LabNum.text);
			this.changeNum = ((this.changeNum >= this.totalNum) ? this.totalNum : (++this.changeNum));
			this.LabNum.text = this.changeNum.ToString();
			this.ScrollBarNum.scrollValue = (float)this.changeNum / (float)this.totalNum;
		};
		UIEventListener.Get(this.BtnJian.gameObject).onClick = delegate(GameObject s)
		{
			if (this.totalNum == 0)
			{
				return;
			}
			this.changeNum = Global.SafeConvertToInt32(this.LabNum.text);
			this.changeNum = ((this.changeNum <= 0) ? 0 : (--this.changeNum));
			this.LabNum.text = this.changeNum.ToString();
			this.ScrollBarNum.scrollValue = (float)this.changeNum / (float)this.totalNum;
		};
	}

	private void initPrefabText()
	{
		this.LabNum.text = string.Empty;
		this.GoodsName.text = string.Empty;
		this.BtnZengsong.Text = Global.GetLang("赠送");
		this.staticText.text = Global.GetLang("赠送数量:");
	}

	public int Num
	{
		get
		{
			return this._Num;
		}
		set
		{
			this._Num = value;
			this.LabNum.text = this._Num.ToString();
		}
	}

	public string GoodName
	{
		get
		{
			return this._GoodName;
		}
		set
		{
			this._GoodName = value;
			this.GoodsName.text = this._GoodName;
		}
	}

	public string GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
			this.ADDGoodsICON(Global.SafeConvertToInt32(this._GoodsID));
			this.totalNum = Global.GetTotalGoodsCountByID(Global.SafeConvertToInt32(this._GoodsID));
			this.LabNum.text = string.Empty + ((this.totalNum <= 0) ? 0 : 1);
		}
	}

	private void ADDGoodsICON(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			this.GoodsName.text = goodsXmlNodeByID.Title;
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.isAutoSize = true;
			icon.BackSpriteName0 = backSpriteName;
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			icon.TipType = 1;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			icon.ItemCode = goodsID;
			icon.ItemObject = dummyGoodsData;
			icon.BoxTypes = 5;
			icon.TextShadowColor = 4278190080U;
			icon.TextColor = 16777215U;
			icon.DisableTextColor = 8421504U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			icon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			icon.SecondText.gameObject.SetActive(true);
			icon.SecondText.text = totalGoodsCountByID.ToString();
			icon.TextShadowColor = 4278190080U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			this.GiftIcon.Add(icon);
			icon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = icon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			icon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData = icon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			});
		}
	}

	public SpriteSL GiftIcon;

	public GButton BtnClose;

	public TextBlock LabNum;

	public UISprite Jian;

	public UISprite Jia;

	public UIScrollBar ScrollBarNum;

	public GButton BtnZengsong;

	public TextBlock GoodsName;

	public UIButton BtnJian;

	public UIButton BtnJia;

	public TextBlock staticText;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int totalNum;

	private int changeNum;

	private int _Num;

	private string _GoodName = string.Empty;

	private string _GoodsID = string.Empty;
}
