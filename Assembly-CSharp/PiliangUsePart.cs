using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class PiliangUsePart : UserControl
{
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

	public int DBID
	{
		get
		{
			return this._DBID;
		}
		set
		{
			this._DBID = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("使用");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
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
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(this.DBID, null);
			if (goodsDataByDbID != null)
			{
				if (Global.GetCategoriyByGoodsID(goodsDataByDbID.GoodsID) == 704)
				{
					GameInstance.Game.SendTOUseTaLuopaiSuiPian(goodsDataByDbID.Id, goodsDataByDbID.GoodsID, this.InputNum);
				}
				else
				{
					GameInstance.Game.SpriteUseGoods(goodsDataByDbID.Id, goodsDataByDbID.GoodsID, this.InputNum);
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 0
				});
			}
			else
			{
				string goodsNameByID = Global.GetGoodsNameByID(goodsDataByDbID.GoodsID, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}无法使用"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
		};
		this.XScrollBar.onChange = delegate(UIScrollBar sb)
		{
			this.InputNum = Mathf.Min(Mathf.RoundToInt(sb.scrollValue * (float)this.MaxNum), this.MaxNum);
			if (this.InputNum <= 0)
			{
				this.InputNum = 1;
			}
			this.Input.Text = this.InputNum.ToString();
		};
		UIEventListener.Get(this.AddIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.InputNum = Mathf.Min(++this.InputNum, this.MaxNum);
			this.Input.Text = this.InputNum.ToString();
			this.XScrollBar.scrollValue = (float)this.InputNum / (float)this.MaxNum;
		};
		UIEventListener.Get(this.SubIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.InputNum = Mathf.Max(--this.InputNum, 1);
			this.Input.Text = this.InputNum.ToString();
			if (this.InputNum <= 1)
			{
				this.XScrollBar.scrollValue = 0f;
			}
			else
			{
				this.XScrollBar.scrollValue = (float)this.InputNum / (float)this.MaxNum;
			}
		};
	}

	public void InitPartSize()
	{
		this.AddGoodsIcon();
		this.Input.Text = this.MaxNum.ToString();
		this.XScrollBar.scrollValue = 1f;
	}

	public void InitPartData()
	{
	}

	private void AddGoodsIcon()
	{
		this.GoodsCanvas.Clear();
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(this.DBID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		int goodsID = goodsDataByDbID.GoodsID;
		this.MaxNum = goodsDataByDbID.GCount;
		if (this.MaxNum == 0)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			this.GoodsName.Text = Global.GetColorStringForNGUIText(new object[]
			{
				goodsXmlNodeByID.GoodsColor,
				goodsXmlNodeByID.Title
			});
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = goodsDataByDbID;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.Text = goodsDataByDbID.GCount.ToString();
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(goodsDataByDbID.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsDataByDbID, canUse, IconTextTypes.Qianghua);
			this.GoodsCanvas.Add(ggoodIcon);
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton CloseBtn;

	public GButton SubmitBtn;

	public UIButton AddIcon;

	public UIButton SubIcon;

	public TextBox Input;

	public UIScrollBar XScrollBar;

	public SpriteSL GoodsCanvas;

	public TextBlock GoodsName;

	private int MaxNum;

	private int InputNum = 1;

	private int _GoodsID;

	private int _DBID;
}
