using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUQiRiGoalItemList : UserControl
{
	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public int goodscount
	{
		get
		{
			return this._goodscount;
		}
		set
		{
			this._goodscount = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.m_BtnLingJiang.Text = Global.GetLang("领取");
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
				if (!this.m_BtnLingJiang.isEnabled)
				{
					return;
				}
				if (Global.GetBaoGuoSpaceCount() < this.goodscount)
				{
					Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
					return;
				}
				GameInstance.Game.GetActivityAward(3, this.id);
			};
		}
		this.m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
	}

	public JieriAwardGiftGainState AwardGiftGainState
	{
		get
		{
			return this.m_AwardGiftGainState;
		}
		set
		{
			this.m_AwardGiftGainState = value;
			switch (this.m_AwardGiftGainState)
			{
			case JieriAwardGiftGainState.CanGain:
				this.m_BtnLingJiang.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				this.m_BtnLingJiang.isEnabled = true;
				break;
			case JieriAwardGiftGainState.Gained:
				this.m_BtnLingJiang.gameObject.SetActive(false);
				this.m_SpriteState.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.CanNotGain:
				this.m_BtnLingJiang.gameObject.SetActive(true);
				this.m_SpriteState.gameObject.SetActive(false);
				this.m_BtnLingJiang.isEnabled = false;
				break;
			}
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

	public string TypeGoal
	{
		get
		{
			return this._typeGoal;
		}
		set
		{
			this._typeGoal = value;
		}
	}

	public int HadNum
	{
		get
		{
			return this.hadNum;
		}
		set
		{
			this.hadNum = value;
			string[] array = this.TypeGoal.Split(new char[]
			{
				','
			});
			if (this.showNum == -1)
			{
				this.miaoshu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}", this._describe)
				});
				return;
			}
			if (this.showNum == 1)
			{
				this.miaoshu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}({1}/{2})", this._describe, this.hadNum, array[0])
				});
				return;
			}
			if (this.showNum == 2)
			{
				this.miaoshu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}({1}/{2})", this._describe, this.hadNum, array[1])
				});
				return;
			}
			if (this.showNum == 3)
			{
				this.miaoshu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}({1}/{2})", this._describe, this.hadNum, array[2])
				});
				return;
			}
		}
	}

	public int ShowNum
	{
		get
		{
			return this.showNum;
		}
		set
		{
			this.showNum = value;
		}
	}

	public string describe
	{
		get
		{
			return this._describe;
		}
		set
		{
			this._describe = value;
			this.miaoshu.text = string.Format("{0}", this._describe);
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

	public UILabel miaoshu;

	public GButton m_BtnLingJiang;

	public UISprite m_SpriteState;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int id = -1;

	private int _goodscount;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;

	private string _typeGoal = string.Empty;

	private int hadNum;

	private int showNum = -1;

	private string _describe;

	private string _GoodsList = string.Empty;
}
