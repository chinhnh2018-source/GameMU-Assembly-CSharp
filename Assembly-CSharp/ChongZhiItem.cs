using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongZhiItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.texZengBak.MakePixelPerfect();
		this.texZengBak.transform.localPosition = new Vector3(0f, -81f, 0f);
		this.lblFreeDiamond.transform.localPosition = new Vector3(-15.5f, -76f, -0.1f);
		this.lblMoney.transform.localPosition = new Vector3(-25f, -120f, 0f);
		this.lblFreeDiamond.transform.localScale = new Vector3(10f, 10f, 1f);
	}

	public string Money
	{
		get
		{
			return this.lblMoney.text;
		}
		set
		{
			this.lblMoney.text = string.Format("{0}", value);
		}
	}

	public string DiamondNum
	{
		get
		{
			return this.lblDiamondNum.text;
		}
		set
		{
			if (this.lblDiamondNum.enabled)
			{
				this.lblDiamondNum.text = value;
			}
		}
	}

	public string DiamondLevel
	{
		get
		{
			return this.texDiamondLevel.spriteName;
		}
		set
		{
			this.texDiamondLevel.spriteName = value;
		}
	}

	public string FreeBingDiamond
	{
		get
		{
			return this.lblFreeDiamond.text;
		}
		set
		{
			this.lblFreeDiamond.text = value;
		}
	}

	public bool HasFreeDiamond
	{
		get
		{
			return this.hasFreeDiamond;
		}
		set
		{
			this.hasFreeDiamond = value;
			if (this.chongZhiType == ChongZhiPart.ChongzhiInfo.ChongZhiType.Normal)
			{
				this.texSong.gameObject.SetActive(value);
				this.texZeng.gameObject.SetActive(value);
				this.lblFreeDiamond.gameObject.SetActive(value);
				this.texZengBak.gameObject.SetActive(value);
			}
		}
	}

	public void SetYueKaUI()
	{
		this.texSong.gameObject.SetActive(true);
		this.texZeng.gameObject.SetActive(false);
		this.lblFreeDiamond.gameObject.SetActive(false);
		this.texZengBak.gameObject.SetActive(false);
		this.lblDiamondNum.enabled = false;
		this.diamondIcon.SetActive(false);
		this.SetNoteIcon("shop_new");
		this.yueKaName.SetActive(true);
		this.yueKaPrice.transform.localScale = new Vector3(190f, 26f, 1f);
		this.yueKaName.transform.localScale = new Vector3(85f, 24f, 1f);
		if (Global.Data.roleData.RoleCommonUseIntPamams.Count > 28 && Global.Data.roleData.RoleCommonUseIntPamams[28] > 0)
		{
			this.yueKaPrice.SetActive(false);
			this.textMonthDay.gameObject.SetActive(true);
			this.textMonthDay.text = Global.GetLang(string.Format(Global.GetLang("已激活，剩余{0}天"), Global.Data.roleData.RoleCommonUseIntPamams[28]));
		}
		else
		{
			this.textMonthDay.gameObject.SetActive(false);
			this.yueKaPrice.SetActive(true);
		}
	}

	public void SetNoteIcon(string name)
	{
		this.texSong.spriteName = name;
	}

	public string ItemTag
	{
		get
		{
			return this.itemTag;
		}
		set
		{
			this.itemTag = value;
		}
	}

	public string ProductId
	{
		get
		{
			return this.productId;
		}
		set
		{
			this.productId = value;
		}
	}

	public void OnClick()
	{
		if (this.DPSelectedItem != null)
		{
			if (!this.mIsFanLiHuoDong)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = int.Parse(this.ItemTag),
					Index = 1
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = int.Parse(this.ItemTag),
					Index = this.m_XianGouNumber,
					Flag = (int)this.chongZhiType
				});
			}
		}
	}

	public string RMB
	{
		get
		{
			return this.rmb;
		}
		set
		{
			this.rmb = value;
		}
	}

	public string MeiYuan
	{
		get
		{
			return this.meiYuan;
		}
		set
		{
			this.meiYuan = value;
		}
	}

	public void SetJieRiFanLi(ChongZhiFanLiData data, int purnum, ChongZhiFanLiState state = ChongZhiFanLiState.None)
	{
		this.mIsFanLiHuoDong = true;
		this.m_GouMaiNumber = purnum;
		NGUITools.SetActive(this.m_PuTongItem.gameObject, false);
		NGUITools.SetActive(this.m_FanLiItem.gameObject, true);
		this.FanLiData = data;
		this.m_LabShouJia.text = Global.GetLang(string.Format(this.lblMoney.text, new object[0]));
		this.m_LabZuanShiNumber.text = this.DiamondNum;
		this.m_ImgZuanShiImg.spriteName = this.texDiamondLevel.spriteName;
		this.mLblFanLiBeiShu.text = this.FanLiData.Num.ToString();
		if (purnum != -1 && this.FanLiData.SinglePurchase != -1)
		{
			this.m_XianGouNumber = this.FanLiData.SinglePurchase - purnum;
			if (this.m_XianGouNumber <= 0)
			{
				this.RefreshItem(purnum, ChongZhiFanLiState.None);
			}
			else
			{
				this.mLblXianGouCiShu.text = this.m_XianGouNumber.ToString();
			}
			if (this.m_XianGouNumber >= 99)
			{
			}
			if (state == ChongZhiFanLiState.End || state == ChongZhiFanLiState.WillBegin)
			{
				this.RefreshItem(purnum, state);
			}
		}
		else if (state == ChongZhiFanLiState.End || state == ChongZhiFanLiState.WillBegin)
		{
			this.RefreshItem(purnum, state);
		}
		if (this.chongZhiType == ChongZhiPart.ChongzhiInfo.ChongZhiType.YueKa)
		{
			NGUITools.SetActive(this.m_PuTongItem.gameObject, true);
			NGUITools.SetActive(this.m_FanLiItem.gameObject, false);
		}
	}

	public void RefreshItem(int purnum, ChongZhiFanLiState state = ChongZhiFanLiState.None)
	{
		if (state == ChongZhiFanLiState.End || state == ChongZhiFanLiState.WillBegin)
		{
			this.ChangeToPuTongItem();
			return;
		}
		if (purnum != -1)
		{
			this.m_XianGouNumber = this.FanLiData.SinglePurchase - purnum;
			if (this.FanLiData.SinglePurchase == -1)
			{
				return;
			}
			if (this.m_XianGouNumber <= 0)
			{
				this.ChangeToPuTongItem();
			}
			else if (this.m_XianGouNumber < 99)
			{
				this.mLblXianGouCiShu.text = this.m_XianGouNumber.ToString();
			}
		}
	}

	public void ChangeToPuTongItem()
	{
		NGUITools.SetActive(this.m_PuTongItem.gameObject, true);
		NGUITools.SetActive(this.m_FanLiItem.gameObject, false);
		this.mIsFanLiHuoDong = false;
	}

	public bool IDPiPei(string id)
	{
		return id.Equals(this.itemTag);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UILabel lblMoney;

	public UILabel lblDiamondNum;

	public UILabel lblFreeDiamond;

	public UISprite texDiamondLevel;

	public UISprite texSong;

	public UISprite texZeng;

	public UISprite texZengBak;

	public GameObject yueKaPrice;

	public GameObject yueKaName;

	public GameObject diamondIcon;

	public TextBlock textMonthDay;

	public GameObject m_PuTongItem;

	public GameObject m_FanLiItem;

	public UILabel m_LabXianGou;

	public UISprite m_ImgZuanShiImg;

	public UILabel m_LabZuanShiNumber;

	public UILabel m_LabShuangBeiZuanShi;

	public UILabel m_LabShouJia;

	public UISprite m_XianGouImg;

	public int m_XianGouNumber = -1;

	private int m_GouMaiNumber = -1;

	private ChongZhiFanLiData FanLiData;

	public GameObject ChongZhiFanLiObj;

	public UILabel mLblFanLiBeiShu;

	public UILabel mLblXianGouCiShu;

	public bool mIsFanLiHuoDong;

	public ChongZhiPart.ChongzhiInfo.ChongZhiType chongZhiType;

	private bool hasFreeDiamond;

	private string itemTag = string.Empty;

	private string productId = string.Empty;

	private string rmb = string.Empty;

	private string meiYuan = string.Empty;
}
