using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class HefuLeijiDengluRewardListItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.daysLabel.text = Global.GetLang("第1天");
		this.daysLabelOver.text = Global.GetLang("第1天");
		this.lingquBtn.Text = Global.GetLang("领 取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodList.ItemsSource;
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.lingquBtn.isEnabled)
			{
				return;
			}
			GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 21, (int)this.m_dayMark);
		};
		this.lingquBtn.gameObject.SetActive(false);
	}

	public AwardGiftGainState AwardGiftGainState
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
			case AwardGiftGainState.CanGain:
				this.lingquBtn.gameObject.SetActive(true);
				this.stateSpriteCanNot.gameObject.SetActive(false);
				this.stateSpriteGained.gameObject.SetActive(false);
				this.daysLabel.gameObject.SetActive(true);
				this.daysLabelOver.gameObject.SetActive(false);
				break;
			case AwardGiftGainState.Gained:
				this.lingquBtn.gameObject.SetActive(false);
				this.stateSpriteCanNot.gameObject.SetActive(false);
				this.stateSpriteGained.gameObject.SetActive(true);
				this.daysLabel.gameObject.SetActive(false);
				this.daysLabelOver.gameObject.SetActive(true);
				break;
			case AwardGiftGainState.CanNotGain:
				this.lingquBtn.gameObject.SetActive(false);
				this.stateSpriteCanNot.gameObject.SetActive(true);
				this.stateSpriteGained.gameObject.SetActive(false);
				this.daysLabel.gameObject.SetActive(true);
				this.daysLabelOver.gameObject.SetActive(false);
				break;
			}
		}
	}

	public DayMark DayMark
	{
		get
		{
			return this.m_dayMark;
		}
		set
		{
			this.m_dayMark = value;
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

	private void loadGoodsList(string goodsIDs)
	{
		this.ItemCollection.Clear();
		if (!(string.Empty == goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.loadOtherJiangLiGoodsList(goodsIDs, false);
			}
			else
			{
				this.loadOtherJiangLiGoodsList(array[0], true);
				this.loadOtherJiangLiGoodsList(array[1], false);
			}
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
		this.ItemCollection.DelayUpdate();
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
			this.ItemCollection.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
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
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public TextBlock daysLabel;

	public TextBlock daysLabelOver;

	public ListBox goodList;

	public GButton lingquBtn;

	public UISprite stateSpriteCanNot;

	public UISprite stateSpriteGained;

	public DPSelectedItemEventHandler DPSelectedItem;

	private AwardGiftGainState m_AwardGiftGainState = AwardGiftGainState.CanNotGain;

	private DayMark m_dayMark = DayMark.One;

	private ObservableCollection _ItemCollection;

	private string _GoodsIDs = string.Empty;
}
