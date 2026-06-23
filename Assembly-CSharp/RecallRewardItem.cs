using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RecallRewardItem : UserControl
{
	public RecallGoodsEx mySun
	{
		get
		{
			return this._mySun;
		}
		set
		{
			this._mySun = value;
			this.Resize();
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._itemCollection;
		}
		set
		{
			this._itemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.signBtn.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this._itemCollection = this.goodsList.ItemsSource;
	}

	public string GoodsIDs
	{
		get
		{
			return this._goodsIDs;
		}
		set
		{
			this._goodsIDs = value;
			this.LoadGoodsList(this.GoodsIDs);
		}
	}

	public EReturnAwardOperateState PickupStatusNew
	{
		set
		{
			this._PickupStatusNew = value;
		}
	}

	public void RefreshUI()
	{
		if (this._PickupStatusNew == 1)
		{
			this.signStatus.gameObject.SetActive(false);
			this.signBtn.gameObject.SetActive(true);
		}
		else if (this._PickupStatusNew == null)
		{
			this.signStatus.spriteName = "weidacheng";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
		}
		else if (this._PickupStatusNew == -1)
		{
			this.signStatus.spriteName = "yilingqu";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
		}
		else if (this._PickupStatusNew == 2)
		{
			this.signStatus.spriteName = "weihuigui";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
		}
	}

	public static Vector3 GetSize(Type tp)
	{
		Vector3 result;
		result..ctor(500f, 105f, 0f);
		if (tp == typeof(RecallRewards))
		{
			result..ctor(500f, 105f, 0f);
		}
		else if (tp == typeof(RecallGiftSet))
		{
			result..ctor(500f, 130f, 0f);
		}
		return result;
	}

	private Vector3 GetSize()
	{
		if (this.mySun == null)
		{
			return new Vector3(500f, 105f);
		}
		return RecallRewardItem.GetSize(this.mySun.GetType());
	}

	private void Resize()
	{
		Vector3 size = this.GetSize();
		if (this.backSpr)
		{
			this.backSpr.transform.localScale = new Vector3(size.x, size.y, 1f);
		}
		if (this.boxCollider)
		{
			this.boxCollider.size = size;
		}
	}

	private void LoadGoodsList(string goodsIDs)
	{
		this._itemCollection.Clear();
		if (!string.IsNullOrEmpty(goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.LoadOtherRewardGoodsList(goodsIDs, false);
			}
			else
			{
				this.LoadOtherRewardGoodsList(array[0], false);
				this.LoadOtherRewardGoodsList(array[1], true);
			}
		}
	}

	private void LoadOtherRewardGoodsList(string goodsStr, bool isOccupation = false)
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
		foreach (string goodsStr2 in array)
		{
			GGoodIcon ggoodIcon = RecallRewardItem.LoadRewardItemGoodsIcon(goodsStr2, isOccupation, false, true);
			if (!(null == ggoodIcon))
			{
				this.ItemCollection.Add(ggoodIcon);
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
				ggoodIcon.addEventListener("click", new MouseEventHandler(RecallRewardItem.MouseLeftButtonUp));
			}
		}
		this._itemCollection.DelayUpdate();
	}

	public static GGoodIcon LoadRewardItemGoodsIcon(string goodsStr, bool isOccupation = false, bool autoListen = true, bool activeBackground = true)
	{
		if (string.IsNullOrEmpty(goodsStr))
		{
			return null;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		string[] array = goodsStr.Split(new char[]
		{
			','
		});
		if (array.Length != 7)
		{
			return null;
		}
		if (isOccupation && MUJieripartChongzhiKingItem.IsTongGuo(array[0], roleOcc))
		{
			return null;
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array[0]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[6]), Convert.ToInt32(array[5]), Convert.ToInt32(array[2]), Convert.ToInt32(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon = RecallRewardItem.CreateGoodsIcon(dummyGoodsDataMu, false, true);
		if (autoListen && null != ggoodIcon)
		{
			ggoodIcon.addEventListener("click", new MouseEventHandler(RecallRewardItem.MouseLeftButtonUp));
		}
		return ggoodIcon;
	}

	private GGoodIcon AddGoodsIcon(GoodsData goodData, bool grayShow = false)
	{
		GGoodIcon ggoodIcon = RecallRewardItem.CreateGoodsIcon(goodData, grayShow, true);
		if (null != ggoodIcon)
		{
			this.ItemCollection.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(RecallRewardItem.MouseLeftButtonUp));
		}
		return ggoodIcon;
	}

	public static GGoodIcon CreateGoodsIcon(GoodsData goodData, bool grayShow = false, bool activeBackground = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
		string text = "bagGrid4_bak";
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = ((!activeBackground) ? null : text);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodData.GoodsID;
		ggoodIcon.ItemObject = goodData;
		ggoodIcon.BoxTypes = -1;
		if (!grayShow)
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
		}
		bool canUse = Global.CanUseGoods(goodData.GoodsID, false, true);
		Super.InitGoodsGIcon(ggoodIcon, goodData, canUse, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	private static void MouseLeftButtonUp(MouseEvent evt)
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

	public UISprite backSpr;

	public BoxCollider boxCollider;

	public UILabel label;

	public ListBox goodsList;

	public UISprite signStatus;

	public GButton signBtn;

	public UILabel minimumRecallPlayers;

	public int index;

	public int xmlID = 1;

	public int minRecruitNum;

	public int minLevel;

	public int minVip;

	public int minQianDaoDay;

	public string Description = string.Empty;

	public RecallGoodsEx _mySun;

	private ObservableCollection _itemCollection;

	private string _goodsIDs = string.Empty;

	private EReturnAwardOperateState _PickupStatusNew;
}
