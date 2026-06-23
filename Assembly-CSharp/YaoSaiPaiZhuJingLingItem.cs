using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YaoSaiPaiZhuJingLingItem : UserControl
{
	public bool mIsRenWuIng { get; set; }

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		UIEventListener.Get(this.mClick).onClick = delegate(GameObject s)
		{
		};
	}

	private void InitValue()
	{
	}

	public void InitItemData(GoodsData data)
	{
		this.AddJingLingIcon(data.GoodsID, data.Id);
		this.OrigionalStatus();
		this.mJingLingData = data;
		this.Level = data.Forge_level + 1;
		int site = data.Site;
		this.IsRenWuIng(site);
	}

	public void AddJingLingIcon(int goodsID, int DBId)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData s) => s.Id == DBId);
			if (goodsData == null)
			{
				return;
			}
			this.icon = U3DUtils.NEW<GGoodIcon>();
			this.icon.Width = 64.0;
			this.icon.Height = 64.0;
			this.icon.isAutoSize = true;
			this.icon.BackSpriteName0 = backSpriteName;
			this.icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			this.icon.TipType = 1;
			this.icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			this.icon.ItemCode = goodsID;
			this.icon.ItemObject = goodsData;
			this.icon.BoxTypes = 5;
			this.icon.TextShadowColor = 4278190080U;
			this.icon.TextColor = 16777215U;
			this.icon.DisableTextColor = 8421504U;
			this.icon.STextVisibility = false;
			this.icon.SecondText.gameObject.SetActive(false);
			this.icon.GoodImg.transform.localPosition = new Vector3(0f, 0f, -1.5f);
			this.icon.BindingSprite.transform.localPosition = new Vector3(-24f, -24f, -4f);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(this.icon, goodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			this.icon.TextShadowColor = 4278190080U;
			this.icon.TextHorizontalAlignment = global::Layout.Right;
			this.icon.TextVerticalAlignment = global::Layout.Top;
			this.icon.gameObject.transform.parent = base.transform;
			this.icon.gameObject.transform.localPosition = this.mJingLingIcon.transform.localPosition;
			this.icon.gameObject.transform.localScale = Vector3.one;
			this.icon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = this.icon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			this.icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		if (this.JingLingDataCallBack != null)
		{
			this.JingLingDataCallBack(this, new DPSelectedItemEventArgs
			{
				Data = this.mJingLingData
			});
		}
	}

	public void OrigionalStatus()
	{
		this.IsShowFullHask = false;
		this.IsShowHalfHask = false;
		this.Describe = string.Empty;
	}

	public void IsRenWuIng(int status)
	{
		if (status == 10001)
		{
			this.mIsRenWuIng = true;
			this.IsShowFullHask = true;
			this.IsShowHalfHask = false;
			this.Describe = Global.GetLang("任务中");
		}
		else
		{
			this.mIsRenWuIng = false;
		}
	}

	public void ShangZhen()
	{
		this.IsShowFullHask = false;
		this.IsShowHalfHask = true;
		this.IsShangZhen = true;
		this.Describe = Global.GetLang("已上阵");
	}

	public void CheXiao()
	{
		this.IsShowFullHask = false;
		this.IsShowHalfHask = false;
		this.IsShangZhen = false;
		this.Describe = Global.GetLang(string.Empty);
	}

	public bool IsShangZhen
	{
		get
		{
			return this.mIsShangZhen;
		}
		set
		{
			this.mIsShangZhen = value;
		}
	}

	public bool IsSelected
	{
		set
		{
			NGUITools.SetActive(this.mSelected, value);
		}
	}

	public int JingLingIconById
	{
		set
		{
			this.mJingLingIcon.URL = string.Format("{0}{1}{2}", "NetImages/GameRes/Images/Goods/", value, ".png");
		}
	}

	public bool IsShowHalfHask
	{
		set
		{
			NGUITools.SetActive(this.mHalfMask, value);
		}
	}

	public bool IsShowFullHask
	{
		set
		{
			NGUITools.SetActive(this.mMaskAll, value);
		}
	}

	public string Describe
	{
		set
		{
			this.mLblMask.Text = value;
		}
	}

	public int Level
	{
		set
		{
			this.mLblLevel.Text = string.Format("{0}{1}", "Lv", value);
		}
	}

	protected override void OnDestroy()
	{
	}

	public DPSelectedItemEventHandler JingLingDataCallBack;

	public ShowNetImage mJingLingIcon;

	public TextBlock mLblLevel;

	public TextBlock mLblMask;

	public GameObject mClick;

	public GameObject mHalfMask;

	public GameObject mMaskAll;

	public GoodsData mJingLingData;

	public GameObject mSelected;

	private GGoodIcon icon;

	private bool mIsShangZhen;
}
