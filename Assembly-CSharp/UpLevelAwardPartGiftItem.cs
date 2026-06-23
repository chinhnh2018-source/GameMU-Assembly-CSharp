using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class UpLevelAwardPartGiftItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnLingJiang.Text = Global.GetLang("领取");
		this.m_TiaoJian.X = 435.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListWuPin)
		{
			this._ItemCollection = this.m_ListWuPin.ItemsSource;
		}
		if (null != this.m_BtnLingJiang)
		{
			this.m_BtnLingJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.m_nEventID = this.ID;
				if (!this.m_BtnLingJiang.isEnabled)
				{
					return;
				}
				GameInstance.Game.SpriteGetUpLevelGiftFlagList(this.ID);
			};
		}
		this.GiftGainState = UpLevelAwardGiftGainState.CanNotGain;
	}

	public UpLevelAwardGiftGainState GiftGainState
	{
		get
		{
			return this.m_GiftGainState;
		}
		set
		{
			this.m_GiftGainState = value;
			switch (this.m_GiftGainState)
			{
			case UpLevelAwardGiftGainState.CanGain:
				this.m_BtnLingJiang.isEnabled = true;
				this.m_BtnLingJiang.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				break;
			case UpLevelAwardGiftGainState.Gained:
				this.m_BtnLingJiang.gameObject.SetActive(false);
				this.m_SpriteState.gameObject.SetActive(true);
				break;
			case UpLevelAwardGiftGainState.CanNotGain:
				this.m_BtnLingJiang.isEnabled = false;
				this.m_BtnLingJiang.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				break;
			}
		}
	}

	public string TiaoJian
	{
		set
		{
			if (value == null || value == string.Empty)
			{
				this.m_TiaoJian.Visibility = false;
				this.m_TiaoJian.text = string.Empty;
			}
			else
			{
				this.m_TiaoJian.Visibility = true;
				this.m_TiaoJian.text = value;
			}
		}
	}

	public int TiaoJianValue
	{
		get
		{
			return this.TiaoJianValueInstance;
		}
		set
		{
			this.TiaoJianValueInstance = value;
		}
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

	public string GoodsList
	{
		get
		{
			return this._GoodsList;
		}
		set
		{
			this._GoodsList = value;
		}
	}

	public void LoadGoodsList(string goodsList, int moJing, int bindMoney)
	{
		this.ItemCollection.Clear();
		this.LoadOtherJiangLiGoodsList(moJing, bindMoney);
	}

	private void LoadOtherJiangLiGoodsList(int moJing, int bindMoney)
	{
		string text = StringUtil.trim(this._GoodsList);
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
		this.AddMoJingOrBindMoneyIcon(moJing, "5050");
		this.AddMoJingOrBindMoneyIcon(bindMoney, "8014");
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				this.AddGoodsIcon(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[6]));
			}
		}
		this.ItemCollection.DelayUpdate();
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

	private void AddGoodsIcon(int goodsID, int gcount, int binding, int forgeLevel, int zhuijiaLevel = 0, int lucky = 0, int zhuoyueIndex = 0)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			int goodsOccByID = Global.GetGoodsOccByID(goodsID);
			if (goodsOccByID != -1 && (goodsOccByID & 1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation)) == 0)
			{
				return;
			}
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

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodsDataByID = this.GetGoodsDataByID(num);
			GTipServiceEx.SelfBagOnly = false;
			if (goodsDataByID != null)
			{
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsDataByID);
			}
		}
	}

	private void AddMoJingOrBindMoneyIcon(int count, string iconCode)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.isAutoSize = true;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(iconCode, string.Empty);
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		ggoodIcon.ContentText.textColor = 13480843U;
		ggoodIcon.ContentText.FontSize = 14;
		ggoodIcon.Text = ggoodIcon.GetShowVal(count.ToString());
		Vector3 localPosition = ggoodIcon.SecondText.transform.localPosition;
		ggoodIcon.SecondText.transform.localPosition = new Vector3(localPosition.x, localPosition.y, -0.1f);
		this.ItemCollection.AddNoUpdate(ggoodIcon);
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
	}

	public GTextBlockOutLine m_TiaoJian;

	public GButton m_BtnLingJiang;

	public UISprite m_SpriteState;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int m_nEventID;

	public int ID = -1;

	private int TiaoJianValueInstance;

	private UpLevelAwardGiftGainState m_GiftGainState = UpLevelAwardGiftGainState.CanNotGain;

	private List<GoodsData> m_LstGoodsData = new List<GoodsData>();

	private string _GoodsList = string.Empty;
}
