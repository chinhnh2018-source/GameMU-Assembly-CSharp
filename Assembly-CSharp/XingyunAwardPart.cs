using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class XingyunAwardPart : UserControl
{
	public string GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
			this.mInitGoodsIcon(value);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPerfabText();
		this.mBtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.GetBaoGuoSpaceCount() < 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包空间不足，至少需要{0}个空格子"), new object[]
				{
					1
				}), 0, -1, -1, 0);
				PlayZone.GlobalPlayZone.CloseXingyunChoujiangAwardWindow();
				PlayZone.GlobalPlayZone.CloseXingyunChoujiangWindow();
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
	}

	private new void Update()
	{
	}

	private void InitPerfabText()
	{
		if (this.mBtnConfirm.Label != null)
		{
			this.mBtnConfirm.Text = Global.GetLang("领   取");
		}
	}

	public ZhuanPanAwardType ZhuanPanAwardType
	{
		get
		{
			return this._ZhuanPanAwardType;
		}
		set
		{
			this._ZhuanPanAwardType = value;
			switch (value)
			{
			case ZhuanPanAwardType.Low:
				NGUITools.SetActive(this.mLow.gameObject, true);
				NGUITools.SetActive(this.mMiddle.gameObject, false);
				NGUITools.SetActive(this.mHigh.gameObject, false);
				break;
			case ZhuanPanAwardType.Middle:
				NGUITools.SetActive(this.mLow.gameObject, false);
				NGUITools.SetActive(this.mMiddle.gameObject, true);
				NGUITools.SetActive(this.mHigh.gameObject, false);
				break;
			case ZhuanPanAwardType.High:
				NGUITools.SetActive(this.mLow.gameObject, false);
				NGUITools.SetActive(this.mMiddle.gameObject, false);
				NGUITools.SetActive(this.mHigh.gameObject, true);
				break;
			}
		}
	}

	private void mInitGoodsIcon(string value)
	{
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(value);
		if (dummyGoodsData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = dummyGoodsData.GoodsID;
		ggoodIcon.ItemObject = dummyGoodsData;
		ggoodIcon.BoxTypes = -1;
		ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		Super.InitGoodsGIcon(ggoodIcon, dummyGoodsData, Global.CanUseGoods(dummyGoodsData.GoodsID, false, true), IconTextTypes.Qianghua);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		this.mAwardObj.Add(ggoodIcon);
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public void mLingquResult(int result)
	{
		if (result != 1)
		{
			if (result == -101)
			{
				Super.HintMainText(Global.GetLang("配置出错"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("其他错误") + result, 10, 3);
			}
		}
		PlayZone.GlobalPlayZone.CloseXingyunChoujiangAwardWindow();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject mLow;

	public GameObject mMiddle;

	public GameObject mHigh;

	public SpriteSL mAwardObj;

	public GButton mBtnConfirm;

	private string _GoodsID = string.Empty;

	private ZhuanPanAwardType _ZhuanPanAwardType;
}
