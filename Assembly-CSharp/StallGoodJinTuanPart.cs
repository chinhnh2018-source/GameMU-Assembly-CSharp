using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class StallGoodJinTuanPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.MaxBtn.Text = Global.GetLang("一口价");
		this.SubmitBtn.Text = Global.GetLang("确定");
		this.m_LabelDiamond.text = Global.GetLang("拥有钻石");
		this.m_LabelJingJia.text = Global.GetLang("竞        价");
		this.m_LabelTitle.text = Global.GetLang("竞    价");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.TxtJingJiaColor = this.m_TxtJingJia.color;
		UIEventListener.Get(this.m_BakJingJia).onClick = new UIEventListener.VoidDelegate(this.InputJingJiaOnClick);
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
		this.XScrollBar.onChange = delegate(UIScrollBar sb)
		{
			this.InputNum = (float)Mathf.RoundToInt(sb.scrollValue * (float)(this.MaxNum - this.MinNum));
		};
		this.XScrollBar.onDragFinished = delegate()
		{
			int num = (int)(this.InputNum % (float)this.UnitPrice);
			if ((double)num < (double)this.UnitPrice * 0.5)
			{
				this.InputNum -= (float)num;
			}
			else
			{
				this.InputNum = this.InputNum - (float)num + (float)this.UnitPrice;
			}
			this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
		};
		UIEventListener.Get(this.AddIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.InputNum = Mathf.Min(this.InputNum += (float)this.UnitPrice, (float)(this.MaxNum - this.MinNum));
			this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
		};
		UIEventListener.Get(this.SubIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.InputNum = Mathf.Max(this.InputNum -= (float)this.UnitPrice, 0f);
			if (this.InputNum <= 0f)
			{
				this.XScrollBar.scrollValue = 0f;
			}
			else
			{
				this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
			}
		};
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if ((int)(this.InputNum + (float)this.MinNum) > Global.Data.roleData.UserMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				Title = this.m_AuctionItem.AuctionItemKey,
				NeedYuanBao = (int)(this.InputNum + (float)this.MinNum)
			});
		};
		this.MaxBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.XScrollBar.scrollValue = 1f;
		};
	}

	public float InputNum
	{
		get
		{
			return this.m_InputNum;
		}
		set
		{
			this.m_InputNum = value;
			if (this.m_TxtJingJia)
			{
				this.SetTextJingJia((long)(this.m_InputNum + (float)this.MinNum));
			}
		}
	}

	public void RefreshByAuctionItemS2C(AuctionItemS2C auctionItem)
	{
		this.m_AuctionItem = auctionItem;
		this.m_TxtGoodName.text = BuyGoodsJinTuanPartGoodItem.GetGoodsColor(auctionItem.Goods);
		if (auctionItem.Goods.Binding == 1)
		{
			this.m_TxtBind.color = NGUIMath.HexToColorEx(16711680U);
			this.m_TxtBind.text = Global.GetLang("绑定");
		}
		else
		{
			this.m_TxtBind.color = NGUIMath.HexToColorEx(65280U);
			this.m_TxtBind.text = Global.GetLang("不绑定");
		}
		this.InputNum = 0f;
		this.XScrollBar.scrollValue = 0f;
		this.MaxNum = this.m_AuctionItem.MaxPrice;
		this.MinNum = this.m_AuctionItem.Price;
		this.UnitPrice = this.m_AuctionItem.UnitPrice;
		this.m_TxtDiamond.text = Global.Data.roleData.UserMoney.ToString();
		this.SetTextJingJia(this.m_AuctionItem.Price);
		this.m_LabelJingJiaEach.text = string.Format(Global.GetLang("每次竞价以{0}为单位"), this.m_AuctionItem.UnitPrice);
		this.AddGoodIcon(auctionItem.Goods);
	}

	public void AddGoodIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCode = gd.GoodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.BoxTypes = 0;
		ggoodIcon.TextSize = 16;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.Tag = gd.ExcellenceInfo;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(gd.GoodsID)
		});
		ggoodIcon.ItemCode = gd.GoodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(gd.GoodsID);
		bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
		U3DUtils.AddChild(this.m_GoodIconContainer, ggoodIcon.gameObject, true);
		Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
		ggoodIcon.NoUseSprite.gameObject.SetActive(false);
		Global.SetEquipGoodsZhanLiStat(ggoodIcon, gd);
	}

	private void SetTextJingJia(long jingJia)
	{
		if (jingJia > (long)Global.Data.roleData.UserMoney)
		{
			this.m_TxtJingJia.color = NGUIMath.HexToColorEx(16711680U);
		}
		else
		{
			this.m_TxtJingJia.color = this.TxtJingJiaColor;
		}
		this.m_TxtJingJia.text = jingJia.ToString();
	}

	private void InputJingJiaOnClick(GameObject obj)
	{
		this.JingJiaDPS = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			if ((long)id < this.m_AuctionItem.Price)
			{
				this.InputNum = 0f;
				this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
			}
			else if ((long)id > this.m_AuctionItem.MaxPrice)
			{
				this.InputNum = (float)(this.m_AuctionItem.MaxPrice - this.m_AuctionItem.Price);
				this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
			}
			else if (((long)id - this.m_AuctionItem.Price) % this.m_AuctionItem.UnitPrice != 0L)
			{
				this.InputNum = 0f;
				this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
				Super.HintMainText(Global.GetLang("输入的值不符合单位竞价"), 10, 3);
			}
			else
			{
				this.InputNum = (float)((long)id - this.m_AuctionItem.Price);
				this.XScrollBar.scrollValue = this.InputNum / (float)(this.MaxNum - this.MinNum);
			}
		};
		PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.JingJiaDPS, null, 0, -100);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton CloseBtn;

	public GButton SubmitBtn;

	public GButton MaxBtn;

	public UIButton AddIcon;

	public UIButton SubIcon;

	public UIScrollBar XScrollBar;

	public UILabel m_TxtGoodName;

	public UILabel m_TxtBind;

	public UILabel m_TxtDiamond;

	public UILabel m_TxtJingJia;

	public UILabel m_LabelJingJiaEach;

	public GameObject m_BakJingJia;

	public UILabel m_LabelDiamond;

	public UILabel m_LabelJingJia;

	public UILabel m_LabelTitle;

	public GameObject m_GoodIconContainer;

	public DPSelectedItemEventHandler JingJiaDPS;

	private long MinNum;

	private long MaxNum;

	private long UnitPrice;

	private float m_InputNum;

	private AuctionItemS2C m_AuctionItem;

	private Color TxtJingJiaColor;
}
