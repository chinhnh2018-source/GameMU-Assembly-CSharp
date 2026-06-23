using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MUJieriPartDuihuanItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnExchange.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.btnExchange.isEnabled)
			{
				return;
			}
			GameInstance.Game.SpriteFetchActivityAward(14, this.ItemID);
		};
		this.labName.X = -220.0;
	}

	private void InitTextInPrefabs()
	{
		this.btnExchange.Text = Global.GetLang("兑换");
	}

	public int Num
	{
		get
		{
			return this._num;
		}
		set
		{
			this._num = value;
			this.labNum.text = this.Num.ToString();
		}
	}

	public int Times
	{
		get
		{
			return this._times;
		}
		set
		{
			this._times = value;
			this.labTimes.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("兑换次数:{0}"), this.Times)
			});
			if (this.Times <= 0)
			{
				this.labTimes.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("兑换次数:{0}"), 0)
				});
				this.btnExchange.isEnabled = false;
			}
		}
	}

	public int ItemID
	{
		get
		{
			return this._itemID;
		}
		set
		{
			this._itemID = value;
		}
	}

	public string Goodname
	{
		get
		{
			return this._goodname;
		}
		set
		{
			this._goodname = value;
			this.labName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.Goodname
			});
		}
	}

	public void setPicfromInt(int mojing, int qifu, int jingling)
	{
		if (mojing != 0)
		{
			this.sprMojing.gameObject.SetActive(true);
			this.sprQifu.gameObject.SetActive(false);
			this.sprJingling.gameObject.SetActive(false);
			this.Num = mojing;
		}
		if (qifu != 0)
		{
			this.sprMojing.gameObject.SetActive(false);
			this.sprQifu.gameObject.SetActive(true);
			this.sprJingling.gameObject.SetActive(false);
			this.Num = qifu;
		}
		if (jingling != 0)
		{
			this.sprMojing.gameObject.SetActive(false);
			this.sprQifu.gameObject.SetActive(false);
			this.sprJingling.gameObject.SetActive(true);
			this.Num = jingling;
		}
	}

	public string GoodsIDNew
	{
		get
		{
			return this._GoodsIDNew;
		}
		set
		{
			this._GoodsIDNew = value;
			this.loadOtherJiangLiGoodsList(this.GoodsIDNew, false);
		}
	}

	private void loadOtherJiangLiGoodsList(string goodsStr, bool isOcc = false)
	{
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				if (!isOcc || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					this.addGoodsIcon(dummyGoodsDataMu, false);
				}
			}
		}
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			this.Goodname = Global.GetColorStringForNGUIText(new object[]
			{
				goodsXmlNodeByID.GoodsColor,
				goodsXmlNodeByID.Title
			});
			this.spritesl.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			MUJierihuodongPart.SetGoodsIconBoxCollider(ggoodIcon);
		}
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

	public UISprite sprJingling;

	public UISprite sprQifu;

	public UISprite sprMojing;

	public GButton btnExchange;

	public TextBlock labTimes;

	public TextBlock labNum;

	public TextBlock labName;

	public SpriteSL spritesl;

	private int _num;

	private int _times;

	private int _itemID;

	private string _goodname = string.Empty;

	private string _GoodsIDNew = string.Empty;
}
