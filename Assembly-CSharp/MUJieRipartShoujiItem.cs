using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUJieRipartShoujiItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodlist.ItemsSource;
		this.btnExchange.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.btnExchange.isEnabled)
			{
				return;
			}
			GameInstance.Game.SpriteFetchActivityAward(14, this.ItemID);
		};
	}

	private void InitTextInPrefabs()
	{
		this.btnExchange.Text = Global.GetLang("兑换");
	}

	public void RefreshGoods()
	{
		this.loadGoodsList(this.GoodsIDs);
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
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

	public int INeedNub
	{
		get
		{
			return this._iNeedNub;
		}
		set
		{
			this._iNeedNub = value;
		}
	}

	public string GoodsIDs
	{
		get
		{
			return this._GoodsIDs;
		}
		set
		{
			this._GoodsIDs = value;
			this.loadGoodsList(this.GoodsIDs);
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

	public int TotalNum
	{
		get
		{
			return this._totalNum;
		}
		set
		{
			this._totalNum = value;
			this.totalNum.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"00fe30",
				string.Format(Global.GetLang("今日可兑:{0}"), this.TotalNum)
			});
			if (this.TotalNum <= 0)
			{
				this.totalNum.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"00fe30",
					string.Format(Global.GetLang("今日可兑:{0}"), 0)
				});
				this.btnExchange.isEnabled = false;
			}
		}
	}

	private void loadGoodsList(string goodsID)
	{
		this.ItemCollection.Clear();
		string text = StringUtil.trim(goodsID);
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
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			this.AddGoodsIcon(int.Parse(array2[0]), int.Parse(array2[1]));
		}
	}

	public void AddGoodsIcon(int goodsID, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			ggoodIcon.ItemNum = iNeedNub;
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = dummyGoodsData;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.Text = iNeedNub.ToString();
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = this.GetTotalGoodsCountByID(goodsID);
			ggoodIcon.Text = string.Format("{0}/{1}", totalGoodsCountByID, iNeedNub);
			if (totalGoodsCountByID >= iNeedNub)
			{
				ggoodIcon.EnableIcon = true;
				ggoodIcon.TextColor = 16777215U;
			}
			else
			{
				ggoodIcon.EnableIcon = false;
				ggoodIcon.TextColor = 16711680U;
			}
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			this.ItemCollection.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			MUJierihuodongPart.SetGoodsIconBoxCollider(ggoodIcon);
		}
	}

	public void RefreshGoodsNum()
	{
		int count = this.ItemCollection.Count;
		for (int i = 0; i < count; i++)
		{
			GGoodIcon component = this.ItemCollection.GetAt(i).GetComponent<GGoodIcon>();
			if (component != null)
			{
				int itemCode = component.ItemCode;
				int itemNum = component.ItemNum;
				int totalGoodsCountByID = this.GetTotalGoodsCountByID(itemCode);
				component.Text = string.Format("{0}/{1}", totalGoodsCountByID, itemNum);
				if (totalGoodsCountByID >= itemNum)
				{
					component.EnableIcon = true;
					component.TextColor = 16777215U;
				}
				else
				{
					component.EnableIcon = false;
					component.TextColor = 16711680U;
				}
			}
		}
	}

	public int GetTotalGoodsCountByID(int goodsID)
	{
		if (Global.Data.roleData.GoodsDataList == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsData.GoodsID == goodsID)
			{
				if (!Global.IsGoodsTimeOver(goodsData))
				{
					if (goodsXmlNodeByID != null)
					{
						int categoriy = goodsXmlNodeByID.Categoriy;
						if (categoriy < 0 || categoriy >= 25 || goodsData.Using != 1)
						{
							num += goodsData.GCount;
						}
					}
				}
			}
		}
		return num;
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
			this.goodIcon = U3DUtils.NEW<GGoodIcon>();
			this.goodIcon.Width = 78.0;
			this.goodIcon.Height = 78.0;
			this.goodIcon.BackSpriteName0 = backSpriteName;
			this.goodIcon.TipType = 1;
			this.goodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			this.goodIcon.ItemCode = gd.GoodsID;
			this.goodIcon.ItemObject = gd;
			this.goodIcon.BoxTypes = -1;
			if (!grayShow)
			{
				this.goodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				this.goodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(this.goodIcon, gd, canUse, IconTextTypes.Qianghua);
			this.spritesl.Add(this.goodIcon);
			this.goodIcon.gameObject.AddComponent<UIDragPanelContents>();
			this.goodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			MUJierihuodongPart.SetGoodsIconBoxCollider(this.goodIcon);
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

	public ListBox goodlist;

	public GGoodIcon goodIcon;

	public GButton btnExchange;

	public TextBlock totalNum;

	public SpriteSL spritesl;

	private ObservableCollection _ItemCollection;

	private int _itemID;

	private int _iNeedNub;

	private string _GoodsIDs = string.Empty;

	private string _GoodsIDNew = string.Empty;

	private int _totalNum;
}
