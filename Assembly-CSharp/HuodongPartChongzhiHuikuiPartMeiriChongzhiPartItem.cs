using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem : UserControl
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

	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("领取");
		this.m_BtnChongZhi.Text = Global.GetLang("充值");
		this.m_SprZuanShi.transform.localPosition = new Vector3(35f, -40f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitControlProc();
		this.ItemCollection = this.GoodsList.ItemsSource;
	}

	private void InitControlProc()
	{
		if (null != this.SubmitBtn)
		{
			this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, (int)this.m_ActivityTypes, this.m_nJiangLiID);
			};
		}
		if (null != this.m_BtnChongZhi)
		{
			this.m_BtnChongZhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			};
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

	public int LingquFlag
	{
		get
		{
			return this._LingquFlag;
		}
		set
		{
			this._LingquFlag = value;
			if (this._LingquFlag == 0)
			{
				NGUITools.SetActive(this.IsLingqu.gameObject, false);
				NGUITools.SetActive(this.SubmitBtn.gameObject, false);
				NGUITools.SetActive(this.LingquCondition.gameObject, true);
				this.m_BtnChongZhi.gameObject.SetActive(true);
				this.m_LblShowText.gameObject.SetActive(true);
			}
			else if (this._LingquFlag == 1)
			{
				NGUITools.SetActive(this.IsLingqu.gameObject, false);
				NGUITools.SetActive(this.SubmitBtn.gameObject, true);
				NGUITools.SetActive(this.LingquCondition.gameObject, false);
			}
			else if (this._LingquFlag == 2)
			{
				NGUITools.SetActive(this.IsLingqu.gameObject, true);
				NGUITools.SetActive(this.SubmitBtn.gameObject, false);
				NGUITools.SetActive(this.LingquCondition.gameObject, false);
				this.m_LblShowText.gameObject.SetActive(false);
			}
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

	public ListBox GoodsList;

	public SpriteSL LingquCondition;

	public TextBlock TxtValue;

	public UILabel m_TextKey;

	public UISprite IsLingqu;

	public GButton SubmitBtn;

	public GButton m_BtnChongZhi;

	public GameObject m_Condition;

	public UILabel m_LblShowText;

	public UISprite m_SprZuanShi;

	private ActivityTypes m_ActivityTypes = ActivityTypes.MeiRiChongZhiHaoLi;

	public int m_nJiangLiID = -1;

	private ObservableCollection _ItemCollection;

	private string _GoodsIDs = string.Empty;

	private int _LingquFlag = -1;
}
