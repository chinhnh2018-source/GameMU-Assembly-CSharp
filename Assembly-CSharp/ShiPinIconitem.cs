using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiPinIconitem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.IsGoods = false;
		this.IsLock = false;
		this.BShowWarning = false;
		this.BSelect = false;
		this.bShowUnLoadBtn = this.m_bShowUnLoadBtn;
		this.LockColliderEnable = this.m_LockColliderEnable;
		this.ShowLock = this.m_ShowLock;
		this._Level.supportEncoding = true;
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		if (null != this._LockSp)
		{
			UIEventListener.Get(this._LockSp.gameObject).onClick = delegate(GameObject e)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = this.m_GoodsId,
						Index = this.m_Index,
						IDType = 10
					});
				}
				this.BSelect = true;
			};
		}
		if (null != this._BtnUnLoad)
		{
			this._BtnUnLoad.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						ID = -1,
						Index = this.m_Index
					});
				}
			};
		}
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			if (BHaveTips)
			{
				ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.ShowGoodsTip(e);
				};
			}
			BoxCollider component = ggoodIcon.GetComponent<BoxCollider>();
			if (null != component)
			{
				Vector3 center = component.center;
				center.z -= 2f;
				component.center = center;
			}
		}
		return ggoodIcon;
	}

	private void ShowGoodsTip(object icon)
	{
		if (this.m_CanShowTips)
		{
			if (this.m_IsGoods)
			{
				if (this.m_Tips)
				{
					GGoodIcon ggoodIcon = icon as GGoodIcon;
					if (null != ggoodIcon)
					{
						GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
						GTipServiceEx.SelfBagOnly = false;
						GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
					}
				}
				else if (this.Hander != null)
				{
					this.Hander(icon, new DPSelectedItemEventArgs
					{
						ID = this.m_GoodsId,
						IDType = 10,
						Index = this.m_Index
					});
				}
			}
			else if (this.Hander != null)
			{
				this.Hander(icon, new DPSelectedItemEventArgs
				{
					ID = this.m_GoodsId,
					IDType = 10,
					Index = this.m_Index
				});
			}
		}
	}

	public void ShowTeXiao(bool bShow = true)
	{
		for (int i = 0; i < this._TeXiaoRoot.childCount; i++)
		{
			Transform child = this._TeXiaoRoot.GetChild(i);
			if (null != child)
			{
				Object.Destroy(child.gameObject);
			}
		}
		if (bShow)
		{
			GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/chongwujineng/chongwu_lingwujineng", this._TeXiaoRoot);
			gameObject.transform.localPosition = Vector3.one;
			NGUITools.SetActive(this._TeXiaoRoot, true);
			DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
			delayDestroy.delayTime = 3f;
		}
	}

	public void SetTrInf(Vector3 pos, Transform parent)
	{
		base.transform.SetParent(parent, false);
		base.transform.localPosition = pos;
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == this.m_UIDragPanelContents && null != this.m_icon)
			{
				this.m_UIDragPanelContents = this.m_icon.GetComponent<UIDragPanelContents>();
				if (null == this.m_UIDragPanelContents)
				{
					this.m_UIDragPanelContents = this.m_icon.gameObject.AddComponent<UIDragPanelContents>();
				}
				this.m_UIDragPanelContents.draggablePanel = value;
			}
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public int Level
	{
		get
		{
			return this.m_Level;
		}
		set
		{
			this.m_Level = value;
			Transform parent = this._Level.transform.parent;
			if (null != parent)
			{
				Vector3 localPosition = parent.localPosition;
				localPosition.z = -1f;
				parent.localPosition = localPosition;
			}
			if (this.m_ShowLock && !this.m_IsGoods)
			{
				this._Level.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdc000",
					string.Format("Lv{0}", this.m_Level)
				});
				NGUITools.SetActive(this._Level, true);
			}
			else
			{
				if (null != this.m_icon)
				{
					this.m_icon.ContentText.Label.supportEncoding = true;
					this.m_icon.ContentText.text = string.Format("Lv{0}", this.m_Level);
				}
				NGUITools.SetActive(this._Level, false);
			}
		}
	}

	public bool IsEmptyGoods
	{
		get
		{
			return this.m_IsEmptyGoods;
		}
		set
		{
			this.m_IsEmptyGoods = value;
			NGUITools.SetActive(this._GoodsRoot, !this.m_IsEmptyGoods);
			NGUITools.SetActive(this._LockSp, this.m_IsEmptyGoods);
			if (this.m_IsEmptyGoods)
			{
				this._LockSp.spriteName = "GoodBg";
				UIDragPanelContents uidragPanelContents = this._LockSp.GetComponent<UIDragPanelContents>();
				if (null == uidragPanelContents)
				{
					uidragPanelContents = this._LockSp.gameObject.AddComponent<UIDragPanelContents>();
				}
				uidragPanelContents.draggablePanel = base.transform.GetComponentInParent<UIDraggablePanel>();
			}
			else
			{
				this._LockSp.spriteName = ((!this.m_IsLock) ? "SeatBg" : "Lock");
				UIDragPanelContents component = this._LockSp.GetComponent<UIDragPanelContents>();
				if (null == component)
				{
					Object.Destroy(component);
				}
			}
			BoxCollider component2 = this._LockSp.GetComponent<BoxCollider>();
			if (null != component2)
			{
				Vector3 center = component2.center;
				center.z = (float)(this.m_IsEmptyGoods ? -1 : 1);
				component2.center = center;
			}
		}
	}

	public int _Type { get; set; }

	public bool Tips
	{
		get
		{
			return this.m_Tips;
		}
		set
		{
			this.m_Tips = value;
			if (this.m_Tips)
			{
				this.HaveTips = true;
			}
		}
	}

	public bool CanShowTips
	{
		get
		{
			return this.m_CanShowTips;
		}
		set
		{
			this.m_CanShowTips = value;
		}
	}

	public bool ShowLock
	{
		get
		{
			return this.m_ShowLock;
		}
		set
		{
			this.m_ShowLock = value;
			NGUITools.SetActive(this._LockSp, this.m_ShowLock);
		}
	}

	public bool LockColliderEnable
	{
		get
		{
			return this.m_LockColliderEnable;
		}
		set
		{
			this.m_LockColliderEnable = value;
			BoxCollider component = this._LockSp.GetComponent<BoxCollider>();
			if (component)
			{
				component.enabled = this.m_LockColliderEnable;
			}
		}
	}

	public bool HaveTips
	{
		get
		{
			return this.m_HaveTips;
		}
		set
		{
			this.m_HaveTips = value;
			if (null != this.m_icon && this.m_icon.MouseLeftButtonUp == null)
			{
				this.m_icon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.ShowGoodsTip(e);
				};
			}
		}
	}

	public int GoodsId
	{
		get
		{
			return this.m_GoodsId;
		}
		set
		{
			this.m_GoodsId = value;
			if (0 < this.m_GoodsId)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.m_GoodsId);
				if (goodsXmlNodeByID != null)
				{
					GoodsData goodsData = Global.GetRoleDecorationList().Find((GoodsData e) => e.GoodsID == this.m_GoodsId);
					if (goodsData == null)
					{
						goodsData = Global.GetEmptyGoodsData(this.m_GoodsId, 0, 1, 0, 1, 1, 1, 1, 1);
					}
					goodsData.Binding = 0;
					this.m_icon = this.initGood(goodsData, this.m_HaveTips);
					if (0 < this._GoodsRoot.childCount)
					{
						for (int i = 0; i < this._GoodsRoot.childCount; i++)
						{
							Transform child = this._GoodsRoot.GetChild(i);
							if (null != child)
							{
								Object.Destroy(child.gameObject);
							}
						}
					}
					this.m_icon.transform.SetParent(this._GoodsRoot, false);
					UIPanel component = this.m_icon.GetComponent<UIPanel>();
					if (null != component)
					{
						Object.Destroy(component);
					}
					UIEventListener.Get(this.m_icon.gameObject).onClick = delegate(GameObject e)
					{
						this.ShowGoodsTip(e);
					};
				}
			}
		}
	}

	public int Index
	{
		get
		{
			return this.m_Index;
		}
		set
		{
			this.m_Index = value;
		}
	}

	public bool BSelect
	{
		get
		{
			return this.m_BSelect;
		}
		set
		{
			this.m_BSelect = value;
			NGUITools.SetActive(this._SelecetObj, this.m_BSelect);
			Vector3 localPosition = this._SelecetObj.transform.localPosition;
			localPosition.z = (float)((!this.m_BSelect) ? 1 : -1);
			this._SelecetObj.transform.localPosition = localPosition;
		}
	}

	public bool IsLock
	{
		get
		{
			return this.m_IsLock;
		}
		set
		{
			this.m_IsLock = value;
			if (null != this._LockSp)
			{
				this._LockSp.spriteName = ((!this.m_IsLock) ? "SeatBg" : "Lock");
			}
		}
	}

	public bool IsGoods
	{
		get
		{
			return this.m_IsGoods;
		}
		set
		{
			this.m_IsGoods = value;
			if (!this.m_IsGoods)
			{
				if (!NGUITools.GetActive(this._LockSp.gameObject))
				{
					NGUITools.SetActive(this._LockSp, true);
				}
			}
			else
			{
				NGUITools.SetActive(this._LockSp, false);
			}
			NGUITools.SetActive(this._GoodsRoot, this.m_IsGoods);
		}
	}

	public bool BShowWarning
	{
		get
		{
			return this.m_BShowWarning;
		}
		set
		{
			this.m_BShowWarning = value;
			NGUITools.SetActive(this._Warning, this.m_BShowWarning);
			if (this.m_BShowWarning)
			{
				this.bShowUnLoadBtn = false;
			}
		}
	}

	public bool bShowUnLoadBtn
	{
		get
		{
			return this.m_bShowUnLoadBtn;
		}
		set
		{
			this.m_bShowUnLoadBtn = value;
			NGUITools.SetActive(this._BtnUnLoad, this.m_bShowUnLoadBtn);
			if (this.m_bShowUnLoadBtn)
			{
				this.BShowWarning = false;
			}
		}
	}

	public GGoodIcon Icon
	{
		get
		{
			return this.m_icon;
		}
		set
		{
			this.m_icon = null;
		}
	}

	public UISprite _LockSp;

	public GameObject _Warning;

	public GameObject _SelecetObj;

	public Transform _GoodsRoot;

	public Transform _TeXiaoRoot;

	public GButton _BtnUnLoad;

	public UILabel _Level;

	[HideInInspector]
	public bool haveShiPin;

	private bool m_BSelect;

	private bool m_IsLock = true;

	private bool m_ShowLock = true;

	private bool m_IsEmptyGoods;

	private int m_Index;

	private int m_GoodsId;

	private bool m_IsGoods;

	private bool m_BShowWarning;

	private bool m_HaveTips;

	private bool m_CanShowTips = true;

	private bool m_Tips = true;

	private int m_Level = 1;

	private bool m_LockColliderEnable = true;

	private GGoodIcon m_icon;

	private bool m_bShowUnLoadBtn;

	private UIDragPanelContents m_UIDragPanelContents;

	public DPSelectedItemEventHandler Hander;
}
