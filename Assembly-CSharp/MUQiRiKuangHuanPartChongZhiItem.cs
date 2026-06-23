using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUQiRiKuangHuanPartChongZhiItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodList.ItemsSource;
		this.btnChongzhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		};
		this.btnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int baoGuoSpaceCount = Global.GetBaoGuoSpaceCount();
			if (baoGuoSpaceCount < this.goodsCount)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包空间不足，至少需要{0}个空格子"), new object[]
				{
					this.goodsCount
				}), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.GetActivityAward(2, (int)this.DayMark);
		};
	}

	private void InitTextInPrefabs()
	{
		this.btnLingqu.Label.text = Global.GetLang("领 取");
		this.btnChongzhi.Label.text = Global.GetLang("充 值");
		this.daysLabel.text = Global.GetLang("第1天");
		this.daysLabelOver.text = Global.GetLang("第1天");
		this.daysLabel.transform.localPosition = new Vector3(-191f, 61.5f, this.daysLabel.transform.localPosition.z);
		this.daysLabel.transform.localScale = new Vector3(30f, 30f, 1f);
		this.daysLabelOver.transform.localPosition = new Vector3(-191f, 61.5f, this.daysLabelOver.transform.localPosition.z);
		this.daysLabelOver.transform.localScale = new Vector3(30f, 30f, 1f);
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
				this.btnLingqu.gameObject.SetActive(true);
				this.btnChongzhi.gameObject.SetActive(false);
				this.StateSpriteGained.gameObject.SetActive(false);
				this.StateSpriteWeiKaiQi.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				this.daysLabel.gameObject.SetActive(true);
				this.daysLabelOver.gameObject.SetActive(false);
				break;
			case JieriAwardGiftGainState.Gained:
				this.btnLingqu.gameObject.SetActive(false);
				this.btnChongzhi.gameObject.SetActive(false);
				this.StateSpriteGained.gameObject.SetActive(true);
				this.StateSpriteWeiKaiQi.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				this.daysLabel.gameObject.SetActive(false);
				this.daysLabelOver.gameObject.SetActive(true);
				this.StateSpriteGained.GetComponent<UISprite>().spriteName = "lingqu";
				break;
			case JieriAwardGiftGainState.CanNotGain:
				this.btnLingqu.gameObject.SetActive(false);
				this.btnChongzhi.gameObject.SetActive(true);
				this.StateSpriteGained.gameObject.SetActive(false);
				this.StateSpriteWeiKaiQi.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				this.daysLabel.gameObject.SetActive(true);
				this.daysLabelOver.gameObject.SetActive(false);
				break;
			case JieriAwardGiftGainState.OverTime:
				this.btnLingqu.gameObject.SetActive(false);
				this.btnChongzhi.gameObject.SetActive(false);
				this.StateSpriteGained.gameObject.SetActive(true);
				this.StateSpriteWeiKaiQi.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				this.daysLabel.gameObject.SetActive(false);
				this.daysLabelOver.gameObject.SetActive(true);
				this.StateSpriteGained.GetComponent<UISprite>().spriteName = "guoqi";
				break;
			case JieriAwardGiftGainState.NotNeedGain:
				this.btnLingqu.gameObject.SetActive(false);
				this.btnChongzhi.gameObject.SetActive(false);
				this.StateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				this.daysLabel.gameObject.SetActive(false);
				this.daysLabelOver.gameObject.SetActive(true);
				this.StateSpriteWeiKaiQi.gameObject.SetActive(true);
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

	public int Need
	{
		get
		{
			return this._need;
		}
		set
		{
			this._need = value;
		}
	}

	public int yichongzhi
	{
		get
		{
			return this._yichongzhi;
		}
		set
		{
			this._yichongzhi = value;
			this.labNeed.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2e1bd",
				string.Format(Global.GetLang("充值额度{0}钻石({1}/{0})"), this.Need, this.yichongzhi)
			});
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
				this.loadOtherJiangLiGoodsList(array[0], false);
				this.loadOtherJiangLiGoodsList(array[1], true);
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
		this.goodsCount = array.Length;
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

	public static bool IsTongGuo(string goodStr, int roleOcc)
	{
		if (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && ConfigSystemParam.GetSystemParamStringArrayByName("LiMJSDaTianShi", ',').IndexOf(goodStr) != -1)
		{
			return false;
		}
		if (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && ConfigSystemParam.GetSystemParamStringArrayByName("ZhiMJSDaTianShi", ',').IndexOf(goodStr) != -1)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(goodStr));
		return goodsXmlNodeByID == null || goodsXmlNodeByID.MainOccupation != roleOcc || (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && goodsXmlNodeByID.Intelligence < goodsXmlNodeByID.Strength) || (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && goodsXmlNodeByID.Intelligence > goodsXmlNodeByID.Strength);
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = backSpriteName;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.ItemCollection.Add(icon);
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
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

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public TextBlock daysLabel;

	public TextBlock daysLabelOver;

	public ListBox goodList;

	public GButton btnLingqu;

	public GButton btnChongzhi;

	public TextBlock labNeed;

	public UISprite StateSpriteGained;

	public UISprite StateSpriteWeiKaiQi;

	private int goodsCount;

	private DayMark m_dayMark = DayMark.One;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.NotNeedGain;

	private ObservableCollection _ItemCollection;

	private int _need;

	private int _yichongzhi;

	private string _GoodsIDs = string.Empty;
}
