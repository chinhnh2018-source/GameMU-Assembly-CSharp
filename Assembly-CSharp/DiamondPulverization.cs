using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class DiamondPulverization : UserControl
{
	private void InitTextInPrefabs()
	{
		this.title.Text = Global.GetLang("批量分解");
		this.pulverizeNumTxt.Text = Global.GetLang("分解获得:");
		this.pulverizeBtn.Text = Global.GetLang("分解");
		this.pulverizeNumTxt.GetComponent<UILabel>().lineWidth = 80;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
		this.pulverizeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.pulverizeBtn.isEnabled)
			{
				return;
			}
			if (this.currentGoodData != null)
			{
				this.PulverizeDiamondRequest();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 0
				});
			}
			else
			{
				string goodsNameByID = Global.GetGoodsNameByID(this.currentGoodData.GoodsID, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}无法分解"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
		};
		this.xScrollBar.onChange = delegate(UIScrollBar sb)
		{
			this.inputNum = Mathf.Min(Mathf.RoundToInt(sb.scrollValue * (float)this.maxNum), this.maxNum);
			if (this.inputNum <= 0)
			{
				this.inputNum = 1;
			}
			this.SetPulverizeNum(this.inputNum);
			this.SetPulverizedPowderNum(this.inputNum);
		};
		UIEventListener.Get(this.addIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.inputNum = Mathf.Min(++this.inputNum, this.maxNum);
			this.SetPulverizeNum(this.inputNum);
			this.SetPulverizedPowderNum(this.inputNum);
			this.xScrollBar.scrollValue = (float)this.inputNum / (float)this.maxNum;
		};
		UIEventListener.Get(this.subIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.inputNum = Mathf.Max(--this.inputNum, 1);
			this.SetPulverizeNum(this.inputNum);
			this.SetPulverizedPowderNum(this.inputNum);
			if (this.inputNum <= 1)
			{
				this.xScrollBar.scrollValue = 0f;
			}
			else
			{
				this.xScrollBar.scrollValue = (float)this.inputNum / (float)this.maxNum;
			}
		};
	}

	public GoodsData targetGoodsData
	{
		set
		{
			this.currentGoodData = value;
			this.AddGoodsIcon();
			this.SetPulverizeNum(this.maxNum);
			this.SetPulverizedPowderNum(this.maxNum);
			this.xScrollBar.scrollValue = 1f;
			this.xScrollBar.enabled = (this.currentGoodData.GCount > 1);
		}
	}

	private void AddGoodsIcon()
	{
		this.goodsIconCanvas.Clear();
		if (this.currentGoodData == null)
		{
			return;
		}
		int goodsID = this.currentGoodData.GoodsID;
		this.maxNum = this.currentGoodData.GCount;
		if (this.maxNum == 0)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			this.goodsName.Text = Global.GetColorStringForNGUIText(new object[]
			{
				goodsXmlNodeByID.GoodsColor,
				goodsXmlNodeByID.Title
			});
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid2_bak";
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = this.currentGoodData;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.Text = this.currentGoodData.GCount.ToString();
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 1565758U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.STextVisibility = false;
			int diamondLevelByGoodsID = Global.GetDiamondLevelByGoodsID(goodsID);
			if (diamondLevelByGoodsID >= 2 && diamondLevelByGoodsID <= 3)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue";
			}
			if (diamondLevelByGoodsID >= 4 && diamondLevelByGoodsID <= 5)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
			}
			if (diamondLevelByGoodsID >= 6 && diamondLevelByGoodsID <= 7)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
			}
			if (diamondLevelByGoodsID >= 8 && diamondLevelByGoodsID <= 12)
			{
				ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
				ggoodIcon.TeXiao.gameObject.SetActive(true);
			}
			bool canUse = Global.CanUseGoods(this.currentGoodData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, this.currentGoodData, canUse, IconTextTypes.Qianghua);
			this.goodsIconCanvas.Add(ggoodIcon);
		}
	}

	private void SetPulverizeNum(int num)
	{
		this.pulverizeNum.Text = num.ToString();
	}

	private void SetPulverizedPowderNum(int num)
	{
		if (this.currentGoodData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.currentGoodData.GoodsID);
			int num2 = (goodsXmlNodeByID == null) ? 1 : goodsXmlNodeByID.PulverizedFluorescentPowderNum;
			this.powderNum.Text = (num * num2).ToString();
		}
	}

	private void PulverizeDiamondRequest()
	{
		if (this.currentGoodData == null)
		{
			return;
		}
		GameInstance.Game.PulverizeDiamond(this.currentGoodData.BagIndex, this.inputNum);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton closeBtn;

	public GButton pulverizeBtn;

	public UIButton addIcon;

	public UIButton subIcon;

	public TextBlock powderNum;

	public TextBlock pulverizeNum;

	public TextBlock title;

	public TextBlock pulverizeNumTxt;

	public UIScrollBar xScrollBar;

	public SpriteSL goodsIconCanvas;

	public TextBlock goodsName;

	private int maxNum;

	private int inputNum = 1;

	private GoodsData currentGoodData;
}
