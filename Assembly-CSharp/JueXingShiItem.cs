using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class JueXingShiItem : MonoBehaviour
{
	public int TaoZhuangId
	{
		get
		{
			return this.m_taoZhuangId;
		}
	}

	public MUAwakenActivationDetail JueXingShi
	{
		get
		{
			return this.m_JueXingShi;
		}
	}

	public void SetJueXingShiId(int id, int taoZhuangID)
	{
		this.m_JueXingShi = JueXingData.GetJueXingShiInfoById(id);
		this.m_taoZhuangId = taoZhuangID;
		this.SetEquipInfo();
		this.imgShiIcon.URL = JueXingData.GetJueXingShiIconURL(this.m_JueXingShi);
		if (JueXingData.IsSelfJiHuo(id, this.m_taoZhuangId))
		{
			this.imgShiIcon.ToGrayBitmap = false;
		}
		else
		{
			this.imgShiIcon.ToGrayBitmap = true;
		}
		if (this.beCanEffect())
		{
			this.imgShiBgSelect.gameObject.SetActive(true);
		}
		else
		{
			this.imgShiBgSelect.gameObject.SetActive(false);
		}
	}

	public void Refersh()
	{
		this.SetJueXingShiId(this.m_JueXingShi.ID, this.m_taoZhuangId);
	}

	private bool beCanEffect()
	{
		return JueXingData.IsCanEffect(this.m_equip);
	}

	private void Awake()
	{
		if (this.BtnBg != null)
		{
			this.SetSelectState(false);
			this.BtnBg.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.OnSelectAction != null)
				{
					this.OnSelectAction.Invoke(this);
				}
			};
		}
	}

	private void SetEquipInfo()
	{
		this.m_equip = JueXingData.GetEquipInfo(JueXingData.GetSelfJueXingEquips(), this.positionType);
		if (this.imgEquipIcon != null)
		{
			if (this.m_equip == null)
			{
				this.imgEquipIcon.gameObject.SetActive(false);
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.m_equip.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				this.imgEquipIcon.gameObject.SetActive(false);
				return;
			}
			this.imgEquipIcon.gameObject.SetActive(true);
			this.imgEquipIcon.StopAllCoroutines();
			this.imgEquipIcon.Width = 80.0;
			this.imgEquipIcon.Height = 80.0;
			this.imgEquipIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				Super.GetIconCode(goodsXmlNodeByID)
			}), false, 0);
			this.imgEquipIcon.TipType = 1;
			this.imgEquipIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			this.imgEquipIcon.ItemCode = goodsXmlNodeByID.ID;
			this.imgEquipIcon.ItemObject = goodsXmlNodeByID;
			this.imgEquipIcon.BoxTypes = -1;
			this.imgEquipIcon.BindingSprite.gameObject.SetActive(false);
			this.imgEquipIcon.BackgroundSprite2.enabled = false;
			bool canUse = Global.CanUseGoods(goodsXmlNodeByID.ID, false, true);
			Super.InitGoodsGIcon(this.imgEquipIcon, this.m_equip, canUse, IconTextTypes.Qianghua);
		}
	}

	public void SetNotVisable()
	{
		this.imgShiBgSelect.gameObject.SetActive(false);
		this.imgShiIcon.URL = string.Empty;
	}

	public void SetSelectState(bool beSelected)
	{
		if (this.BtnBg == null)
		{
			return;
		}
		if (beSelected)
		{
			this.BtnBg.target.enabled = true;
		}
		else
		{
			this.BtnBg.target.enabled = false;
		}
	}

	private const string BgSelectIcon = "xuanzhongkuang";

	private const string BgNormalIcon = "1jikuang";

	private const string NotVisableIcon = "closehover";

	private const string EffectIcon = "JueXing_Kuang";

	private const string NotEffectIcon = "JueXing_KuangZhiHui";

	public ShowNetImage imgShiIcon;

	public ShowNetImage imgShiBg;

	public ShowNetImage imgShiBgSelect;

	public GGoodIcon imgEquipIcon;

	public GButton BtnBg;

	public JueXingPositionType positionType;

	public Action<JueXingShiItem> OnSelectAction;

	private MUAwakenActivationDetail m_JueXingShi;

	private int m_taoZhuangId;

	private GoodsData m_equip;
}
