using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ZhanDouLiAwardPartGiftItem : UserControl
{
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

	public int ZhanDouLi
	{
		get
		{
			return this._ZhanDouLi;
		}
		set
		{
			this._ZhanDouLi = value;
			this.m_TiaoJian.Text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("战斗力：")
			}), this._ZhanDouLi);
		}
	}

	private void InitTextInPrefabs()
	{
		this.m_LingQuBtn.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListWuPin)
		{
			this.ItemCollection = this.m_ListWuPin.ItemsSource;
		}
		this.m_LingQuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_nEventID = this.ID;
			if (!this.m_LingQuBtn.isEnabled)
			{
				return;
			}
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteGetZhanDouLiGiftFlagList(this.ID);
		};
		this.GiftGainState = ZhanDouLiAwardGiftGainState.CanNotGain;
	}

	public ZhanDouLiAwardGiftGainState GiftGainState
	{
		get
		{
			return this.m_State;
		}
		set
		{
			this.m_State = value;
			switch (this.m_State)
			{
			case ZhanDouLiAwardGiftGainState.CanGain:
				this.m_LingQuBtn.isEnabled = true;
				this.m_LingQuBtn.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				break;
			case ZhanDouLiAwardGiftGainState.Gained:
				this.m_LingQuBtn.isEnabled = false;
				this.m_LingQuBtn.gameObject.SetActive(false);
				this.m_SpriteState.gameObject.SetActive(true);
				break;
			case ZhanDouLiAwardGiftGainState.CanNotGain:
				this.m_LingQuBtn.isEnabled = false;
				this.m_LingQuBtn.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				break;
			}
		}
	}

	public void ParseGoodsList(string goodsOne, string goodsTwo, string goodsThr)
	{
		this.ItemCollection.Clear();
		if (!string.IsNullOrEmpty(goodsOne))
		{
			this.LoadGoodsList(goodsOne, false);
		}
		if (!string.IsNullOrEmpty(goodsTwo))
		{
			this.LoadGoodsList(goodsTwo, true);
		}
		if (!string.IsNullOrEmpty(goodsThr))
		{
			this.LoadGoodsList(goodsThr, false);
		}
	}

	private void LoadGoodsList(string goodsList, bool isOccu = false)
	{
		if (string.IsNullOrEmpty(goodsList))
		{
			return;
		}
		string[] array = goodsList.Split(new char[]
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
				if (!isOccu || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
				{
					this.AddGoodsIcon(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[6]));
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(int goodsID, int gcount, int binding, int forgeLevel, int zhuijiaLevel = 0, int lucky = 0, int zhuoyueIndex = 0)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon;
			if (dummyGoodsDataMu != null)
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
				ggoodIcon.Width = 78.0;
				ggoodIcon.Height = 78.0;
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemObject = dummyGoodsDataMu;
				ggoodIcon.isAutoSize = true;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
				ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
				bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			}
			else
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 78.0;
				ggoodIcon.Height = 78.0;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			}
			this.ItemCollection.AddNoUpdate(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			this.m_LstGoodsData.Add(dummyGoodsDataMu);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private GoodsData GetGoodsDataByID(int nID)
	{
		if (0 < nID && this.m_LstGoodsData != null)
		{
			for (int i = 0; i < this.m_LstGoodsData.Count; i++)
			{
				if (nID == this.m_LstGoodsData[i].GoodsID)
				{
					return this.m_LstGoodsData[i];
				}
			}
		}
		return null;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		if (goodsData != null)
		{
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
		}
	}

	public int m_nEventID = -1;

	public int ID = -1;

	public GButton m_LingQuBtn;

	public GTextBlockOutLine m_TiaoJian;

	public ListBox m_ListWuPin = new ListBox();

	public UISprite m_SpriteState;

	public ObservableCollection _ItemCollection;

	private int _ZhanDouLi = -1;

	private List<GoodsData> m_LstGoodsData = new List<GoodsData>();

	private ZhanDouLiAwardGiftGainState m_State = ZhanDouLiAwardGiftGainState.CanNotGain;
}
