using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RidePetItem : UserControl
{
	public int ItemCout
	{
		get
		{
			return this.mItemCout;
		}
		set
		{
			this.mItemCout = value;
		}
	}

	public bool ChuZhan
	{
		get
		{
			return NGUITools.GetActive(this.m_SpZhan.gameObject);
		}
		set
		{
			this.m_SpZhan.gameObject.SetActive(value);
		}
	}

	public GoodsData GoodsData
	{
		get
		{
			return this.mGoodsdata;
		}
	}

	public int Id
	{
		get
		{
			if (this.mGoodsdata != null)
			{
				return this.mGoodsdata.Id;
			}
			return base.GetInstanceID();
		}
	}

	public int HorseLevel
	{
		get
		{
			if (this.mGoodsdata != null)
			{
				return this.mGoodsdata.Forge_level + 1;
			}
			return 0;
		}
	}

	public bool IsGray
	{
		get
		{
			return this.mIsGray;
		}
		set
		{
			this.mIsGray = value;
			if (null != this.mICon)
			{
				this.mICon.GoodImg.ToGrayBitmap = this.mIsGray;
				UIWidget[] componentsInChildren = this.mICon.TeXiao.GetComponentsInChildren<UIWidget>();
				if (0 < componentsInChildren.Length)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (null != componentsInChildren[i])
						{
							componentsInChildren[i].enabled = !this.mIsGray;
						}
					}
				}
			}
		}
	}

	public bool IsNew
	{
		set
		{
			this.m_SpNew.gameObject.SetActive(value);
		}
	}

	public UIDraggablePanel DragPanel
	{
		set
		{
			this.mUIDraggablePanel = value;
			if (null != this.mICon)
			{
				UIDragPanelContents uidragPanelContents = this.mICon.GetComponent<UIDragPanelContents>();
				if (null == uidragPanelContents)
				{
					uidragPanelContents = this.mICon.gameObject.AddComponent<UIDragPanelContents>();
				}
				uidragPanelContents.draggablePanel = value;
			}
			UIPanel[] componentsInChildren = base.GetComponentsInChildren<UIPanel>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (null != componentsInChildren[i])
				{
					Object.Destroy(componentsInChildren[i]);
				}
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ValueInit();
	}

	private void ValueInit()
	{
		this.m_SpBackOnClick.gameObject.SetActive(false);
		this.m_SpNew.gameObject.SetActive(false);
		this.m_SpZhan.gameObject.SetActive(false);
		this.m_BtnColse.gameObject.SetActive(false);
		this.m_LabLV.gameObject.SetActive(false);
	}

	public bool ItemIsSelect
	{
		get
		{
			return NGUITools.GetActive(this.m_SpBackOnClick.gameObject);
		}
		set
		{
			this.m_SpBackOnClick.gameObject.SetActive(value);
		}
	}

	public void SetData(GoodsData data, RidePetUIType uiType = RidePetUIType.None)
	{
		this.type = uiType;
		this.mGoodsdata = data;
		if (data == null)
		{
			if (null != this.mICon)
			{
				this.mICon.gameObject.SetActive(false);
			}
		}
		else
		{
			if (null != this.mICon)
			{
				this.mICon.gameObject.SetActive(true);
			}
			this.mICon = this.AddGoodsIcon(data, this.m_GameParent, this.mIsGray);
		}
		if (this.type == RidePetUIType.TuJian)
		{
			this.m_SpBack.gameObject.SetActive(false);
		}
		else if (this.type == RidePetUIType.ZuoQi)
		{
			this.m_LabLV.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				"LV" + this.HorseLevel
			});
			if (this.mGoodsdata != null)
			{
				this.m_LabZhuoYue.gameObject.SetActive(true);
				if (this.mGoodsdata.WashProps != null && this.mGoodsdata.WashProps.Count > 0)
				{
					this.m_LabZhuoYue.text = Global.GetColorStringForNGUIText(new object[]
					{
						"b266ff",
						this.mGoodsdata.WashProps.Count / 2 + Global.GetLang("卓越")
					});
				}
				else
				{
					this.m_LabZhuoYue.text = Global.GetColorStringForNGUIText(new object[]
					{
						"b266ff",
						"0" + Global.GetLang("卓越")
					});
				}
				this.m_BtnColse.gameObject.SetActive(true);
				this.m_BtnColse.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2,
						IDType = (int)this.type,
						MyID = ((this.mGoodsdata != null) ? this.mGoodsdata.Id : 0),
						ZhuZhuangBei = this.mGoodsdata
					});
				};
				this.m_SpZhan.gameObject.SetActive(1 == this.mGoodsdata.Using);
				if (null != this.mICon)
				{
					this.mICon.BindingSprite.gameObject.SetActive(1 == data.Binding);
				}
			}
			else
			{
				this.m_LabZhuoYue.text = string.Empty;
				this.m_BtnColse.gameObject.SetActive(false);
				this.m_SpZhan.gameObject.SetActive(false);
			}
		}
	}

	public GGoodIcon AddGoodsIcon(GoodsData gd, GameObject parent, bool grayShow = false)
	{
		if (gd != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
				string backSpriteName = "bagGrid4_bak";
				GGoodIcon ggoodIcon;
				if (null == this.mICon)
				{
					ggoodIcon = U3DUtils.NEW<GGoodIcon>();
					ggoodIcon.Width = 78.0;
					ggoodIcon.Height = 78.0;
					ggoodIcon.BackSpriteName0 = backSpriteName;
					ggoodIcon.TipType = 1;
					ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
					ggoodIcon.ItemCode = gd.GoodsID;
					ggoodIcon.ItemObject = gd;
					ggoodIcon.BoxTypes = -1;
				}
				else
				{
					ggoodIcon = this.mICon;
				}
				if (!grayShow)
				{
					ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				}
				else
				{
					ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
				}
				ggoodIcon.GoodImg.ToGrayBitmap = grayShow;
				bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
				ggoodIcon.gameObject.AddComponent<UIPanel>();
				ggoodIcon.transform.SetParent(parent.transform, false);
				if (this.type == RidePetUIType.TuJian)
				{
					ggoodIcon.BackgroundSprite0.gameObject.SetActive(true);
				}
				else
				{
					ggoodIcon.BackgroundSprite0.gameObject.SetActive(false);
				}
				if (grayShow && null != ggoodIcon.TeXiao)
				{
					ggoodIcon.TeXiao.gameObject.SetActive(false);
				}
				ggoodIcon.addEventListener("click", delegate(MouseEvent s)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = (int)this.type,
						MyID = ((this.mGoodsdata != null) ? this.mGoodsdata.Id : 0),
						ZhuZhuangBei = gd
					});
				});
				if (null != this.mUIDraggablePanel)
				{
					UIDragPanelContents uidragPanelContents = ggoodIcon.GetComponent<UIDragPanelContents>();
					if (null == uidragPanelContents)
					{
						uidragPanelContents = ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
					}
					uidragPanelContents.draggablePanel = this.mUIDraggablePanel;
				}
				UIPanel component = ggoodIcon.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
				return ggoodIcon;
			}
		}
		return null;
	}

	public GameObject m_GameParent;

	public UISprite m_SpBackOnClick;

	public UISprite m_SpNew;

	public UISprite m_SpZhan;

	public GButton m_BtnColse;

	public UISprite m_SpBack;

	public ShowNetImage m_Img;

	public UILabel m_LabZhuoYue;

	public UILabel m_LabLV;

	private RidePetUIType type;

	private GoodsData mGoodsdata;

	private GGoodIcon mICon;

	private int mItemCout = -1;

	private bool mIsGray;

	private UIDraggablePanel mUIDraggablePanel;

	public DPSelectedItemEventHandler DPSelectedItem;
}
