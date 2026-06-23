using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TianFuXiDian : UserControl
{
	protected override void InitializeComponent()
	{
		this.XiDian_Btn.Text = Global.GetLang("洗点");
		this.DuiGou.gameObject.SetActive(false);
		UIEventListener.Get(this.DuiGouBack.gameObject).onClick = delegate(GameObject s)
		{
			if (!this.DuiGouIs)
			{
				this.DuiGouIs = true;
			}
			else
			{
				this.DuiGouIs = false;
			}
			this.DuiGou.gameObject.SetActive(this.DuiGouIs);
		};
		this.XiDian_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClickXiDian();
		};
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs());
			}
		};
		this.InitStr();
		base.transform.localPosition = Vector3.zero;
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "TianFuXiDian", this.Cost_Stone, string.Empty);
	}

	public void InitStr()
	{
		this.TitleStr.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("洗点消耗")
		});
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingTianFuCostZuanShi", ',');
		this.Cost_Stone = systemParamIntArrayByName[0] * (Global.Data.roleData.MyTalentData.TotalCount - TianFuSystemPart.getShengYuDianShu());
		if (this.Cost_Stone > systemParamIntArrayByName[1])
		{
			this.Cost_Stone = systemParamIntArrayByName[1];
		}
		this.XiDian_Juan.text = Global.GetLang("{e3b36c}没有{b266ff}【洗点卷】{-}，自动消耗     {-}") + Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("{0}"), this.Cost_Stone)
		});
		this.XiDian_Num.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("可洗点数:0")
		});
		this.XiDian_Btn.Text = Global.GetLang("洗点");
	}

	private new void Start()
	{
	}

	public void initIcon()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingTianFuCostGoods", ',');
		if (this.icon == null)
		{
			this.ADDGoodsICON(systemParamIntArrayByName[0]);
		}
		else
		{
			this.Cost_Juan = systemParamIntArrayByName[1];
			this.Juan_Num = Global.GetTotalGoodsCountByID(systemParamIntArrayByName[0]);
			if (this.icon != null)
			{
				this.icon.Text = string.Format(Global.GetLang("{0}/{1}"), this.Juan_Num, this.Cost_Juan);
				if (this.Juan_Num >= this.Cost_Juan)
				{
					this.icon.TextColor = 65280U;
				}
				else
				{
					this.icon.TextColor = 16777215U;
				}
			}
		}
	}

	public void setItemLable()
	{
	}

	public int KeXiDianShu
	{
		set
		{
			this.XiDian_Num.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("可洗点数:{0}"), value)
			});
		}
	}

	public void ClickXiDian()
	{
		if (Global.Data.roleData.MyTalentData.CountList[1] + Global.Data.roleData.MyTalentData.CountList[2] + Global.Data.roleData.MyTalentData.CountList[3] == 0)
		{
			Super.HintMainText(Global.GetLang("没有可洗点数"), 10, 3);
			return;
		}
		if (!this.DuiGouIs)
		{
			if (this.Juan_Num >= this.Cost_Juan)
			{
				if (this.ShenJiDianChongZhi)
				{
					GameInstance.Game.ShenJiDianChongZhi(0);
				}
				else
				{
					GameInstance.Game.TianFuXiDian(1);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("道具不足"), 10, 3);
			}
			return;
		}
		if (Global.Data.roleData.UserMoney < this.Cost_Stone && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("TianFuXiDian", this.Cost_Stone, false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		if (this.ShenJiDianChongZhi)
		{
			GameInstance.Game.ShenJiDianChongZhi(1);
			return;
		}
		string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "TianFuXiDian", this.Cost_Stone);
		GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.Cost_Stone, text)
		}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.TianFuXiDian(0);
			}
			return true;
		};
	}

	private void ADDGoodsICON(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			this.icon = U3DUtils.NEW<GGoodIcon>();
			this.icon.Width = 64.0;
			this.icon.Height = 64.0;
			this.icon.isAutoSize = true;
			this.icon.BackSpriteName0 = backSpriteName;
			this.icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			this.icon.TipType = 1;
			this.icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			this.icon.ItemCode = goodsID;
			this.icon.ItemObject = dummyGoodsData;
			this.icon.BoxTypes = 5;
			this.icon.TextShadowColor = 4278190080U;
			this.icon.TextColor = 16777215U;
			this.icon.DisableTextColor = 8421504U;
			this.icon.TextHorizontalAlignment = global::Layout.Right;
			this.icon.TextVerticalAlignment = global::Layout.Bottom;
			this.icon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(this.icon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingTianFuCostGoods", ',');
			this.Cost_Juan = systemParamIntArrayByName[1];
			this.Juan_Num = Global.GetTotalGoodsCountByID(systemParamIntArrayByName[0]);
			this.icon.Text = string.Format(Global.GetLang("{0}/{1}"), this.Juan_Num, this.Cost_Juan);
			if (this.Juan_Num >= this.Cost_Juan)
			{
				this.icon.TextColor = 65280U;
			}
			this.icon.TextShadowColor = 4278190080U;
			this.icon.TextHorizontalAlignment = global::Layout.Right;
			this.icon.TextVerticalAlignment = global::Layout.Bottom;
			this.GiftGood.Add(this.icon);
			UIPanel component = this.icon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			this.icon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData = this.icon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(this.icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			});
		}
	}

	public UISprite ItemIcon_Sprite;

	public UILabel ItemLable;

	public UILabel XiDian_Num;

	public UILabel XiDian_Juan;

	public UISprite DuiGou;

	public UISprite DuiGouBack;

	public bool DuiGouIs;

	public DPSelectedItemEventHandler CloseHandler;

	public int _ID;

	public GButton XiDian_Btn;

	public UILabel TitleStr;

	public ShowNetImage Image;

	public GButton Close;

	public List<UISprite> listDaiBi = new List<UISprite>();

	public bool ShenJiDianChongZhi;

	public SpriteSL GiftGood;

	private int Juan_Num;

	private int Cost_Juan;

	private int Cost_Stone;

	private GGoodIcon icon;
}
