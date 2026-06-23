using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class OpenServerActiveLevelPartGiftItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnLingJiang.Text = Global.GetLang("领取");
		this.m_Rank.Text = Global.GetLang("第      名");
		this.m_NameFirst.X = -260.0;
		this.m_ListWuPin.X = -180.0;
		this.m_TiaoJian.X = 253.0;
		this.m_BtnLingJiang.transform.localPosition = new Vector3(300f, this.m_BtnLingJiang.transform.localPosition.y, this.m_BtnLingJiang.transform.localPosition.z);
		this.m_SpriteState.transform.localPosition = new Vector3(300f, this.m_SpriteState.transform.localPosition.y, this.m_SpriteState.transform.localPosition.z);
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
				MUDebug.Log<string>(new string[]
				{
					"m_BtnLingJiang:" + this.m_nEventID
				});
				if (!this.m_BtnLingJiang.isEnabled)
				{
					return;
				}
				switch (this.m_ActiveType)
				{
				case OpenServerActiveType.LEVEL:
					GameInstance.Game.SpriteFetchNewZoneActivityAward(33, this.m_nEventID);
					break;
				case OpenServerActiveType.BossKing:
					GameInstance.Game.SpriteFetchNewZoneActivityAward(36, 0);
					break;
				case OpenServerActiveType.ChongZhiKing:
					GameInstance.Game.SpriteFetchNewZoneActivityAward(34, 0);
					break;
				case OpenServerActiveType.XiaoFeiKing:
					GameInstance.Game.SpriteFetchNewZoneActivityAward(35, 0);
					break;
				}
			};
		}
		this.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
	}

	public OpenServerActiveType ActiveType
	{
		get
		{
			return this.m_ActiveType;
		}
		set
		{
			this.m_ActiveType = value;
			if (this.m_ActiveType == OpenServerActiveType.LEVEL)
			{
				this.m_LevelContainer.SetActive(true);
				this.m_ActiveContainer.SetActive(false);
			}
			else
			{
				this.m_LevelContainer.SetActive(false);
				this.m_ActiveContainer.SetActive(true);
			}
		}
	}

	public OpenServerActiveGiftGainState GiftGainState
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
			case OpenServerActiveGiftGainState.CanGain:
				this.m_BtnLingJiang.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				this.m_BtnLingJiang.isEnabled = true;
				break;
			case OpenServerActiveGiftGainState.Gained:
				this.m_BtnLingJiang.gameObject.SetActive(false);
				this.m_SpriteState.gameObject.SetActive(true);
				break;
			case OpenServerActiveGiftGainState.CanNotGain:
				this.m_BtnLingJiang.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				this.m_BtnLingJiang.isEnabled = false;
				break;
			}
		}
	}

	public string NameFrist
	{
		set
		{
			this.m_NameFirst.text = value;
		}
	}

	public string NameSecond
	{
		set
		{
			if (value == null || value == string.Empty)
			{
				this.m_NameSecond.Visibility = false;
				this.m_NameSecond.text = string.Empty;
			}
			else
			{
				this.m_NameSecond.Visibility = true;
				this.m_NameSecond.text = value;
			}
		}
	}

	public string LevelNameFrist
	{
		set
		{
			this.m_LevelNameFirst.text = value;
		}
	}

	public string LevelNameSecond
	{
		set
		{
			if (value == null || value == string.Empty)
			{
				this.m_LevelNameSecond.Visibility = false;
				this.m_LevelNameSecond.text = string.Empty;
			}
			else
			{
				this.m_LevelNameSecond.Visibility = true;
				this.m_LevelNameSecond.text = value;
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
			this.LoadGoodsList(this.GoodsList);
		}
	}

	private void LoadGoodsList(string goodsList)
	{
		this.ItemCollection.Clear();
		this.LoadOtherJiangLiGoodsList();
	}

	private void LoadOtherJiangLiGoodsList()
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
			GGoodIcon newGoodIcon;
			if (dummyGoodsDataMu != null)
			{
				newGoodIcon = Global.GetNewGoodIcon();
				newGoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
				newGoodIcon.Width = 70.0;
				newGoodIcon.Height = 70.0;
				newGoodIcon.ItemCategory = categoriy;
				newGoodIcon.ItemObject = dummyGoodsDataMu;
				newGoodIcon.isAutoSize = true;
				newGoodIcon.BackSpriteName0 = "bagGrid4_bak";
				newGoodIcon.BackgroundSprite0.gameObject.transform.localScale = new Vector3(80f, 80f, 0f);
				newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				newGoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
				bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
				Super.InitGoodsGIcon(newGoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			}
			else
			{
				newGoodIcon = Global.GetNewGoodIcon();
				newGoodIcon.Width = 70.0;
				newGoodIcon.Height = 70.0;
				newGoodIcon.BackSpriteName0 = "bagGrid_bak";
			}
			this.ItemCollection.AddNoUpdate(newGoodIcon);
			newGoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			newGoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
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
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	public GTextBlockOutLine m_NameFirst;

	public GTextBlockOutLine m_NameSecond;

	public GTextBlockOutLine m_LevelNameFirst;

	public GTextBlockOutLine m_LevelNameSecond;

	public TextBlock m_Rank;

	public GameObject m_ActiveContainer;

	public GameObject m_LevelContainer;

	public GTextBlockOutLine m_TiaoJian;

	public GButton m_BtnLingJiang;

	public UISprite m_SpriteState;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	private OpenServerActiveType m_ActiveType;

	public int m_nEventID;

	public int ID = -1;

	private int TiaoJianValueInstance;

	private OpenServerActiveGiftGainState m_GiftGainState = OpenServerActiveGiftGainState.CanNotGain;

	private string _GoodsList = string.Empty;
}
