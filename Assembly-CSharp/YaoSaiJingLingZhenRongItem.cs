using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YaoSaiJingLingZhenRongItem : UserControl
{
	public int mBossId { get; set; }

	public string mJingLingZhenRongStr { get; set; }

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
		if (this.mClick != null)
		{
			UIEventListener.Get(this.mClick).onClick = delegate(GameObject s)
			{
				this.OpenPaiZhuJingLingPart(this.JingLingDBId, this.mBossId, this.mJingLingZhenRongStr);
			};
		}
	}

	private void InitValue()
	{
	}

	public int JingLingIconById
	{
		set
		{
			if (value == 0)
			{
				this.mJingLingIcon.URL = "NetImages/GameRes/Images/YaoSaiBossTexture/defalut_jinglingKuang.png";
			}
			else
			{
				this.AddJingLingIcon(value);
			}
		}
	}

	public int JingLingIndex
	{
		get
		{
			return this.mjingLingIndex;
		}
		set
		{
			this.mjingLingIndex = value;
		}
	}

	public int JingLingDBId
	{
		get
		{
			return this.mJingLingDBId;
		}
		set
		{
			this.mJingLingDBId = value;
		}
	}

	public void AddJingLingIcon(int DBId)
	{
		GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData s) => s.Id == DBId);
		if (goodsData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsData.GoodsID,
				0,
				-1,
				-1
			});
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.STextVisibility = false;
			ggoodIcon.SecondText.gameObject.SetActive(false);
			ggoodIcon.GoodImg.transform.localPosition = new Vector3(0f, 0f, -1.5f);
			ggoodIcon.BindingSprite.transform.localPosition = new Vector3(-24f, -24f, -4f);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			ggoodIcon.gameObject.transform.parent = base.transform;
			ggoodIcon.gameObject.transform.localPosition = this.mJingLingIcon.transform.localPosition;
			ggoodIcon.gameObject.transform.localScale = Vector3.one;
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		this.OpenPaiZhuJingLingPart(this.JingLingDBId, this.mBossId, this.mJingLingZhenRongStr);
	}

	private void OpenPaiZhuJingLingPart(int DBId, int bossId, string jingLingZhenRongStr)
	{
		if (null == this.mYaoSaiPaiZhuJingLingPartWindow)
		{
			this.mYaoSaiPaiZhuJingLingPartWindow = U3DUtils.NEW<GChildWindow>();
			this.mYaoSaiPaiZhuJingLingPartWindow.ModalType = ChildWindowModalType.Translucent;
			this.mYaoSaiPaiZhuJingLingPartWindow.IsShowModal = true;
			Super.InitChildWindow(this.mYaoSaiPaiZhuJingLingPartWindow, Global.GetLang("YaoSaiPaiZhuJingLingPartWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.mYaoSaiPaiZhuJingLingPartWindow);
		}
		if (null == this.mYaoSaiPaiZhuJingLingPart)
		{
			this.mYaoSaiPaiZhuJingLingPart = U3DUtils.NEW<YaoSaiPaiZhuJingLingPart>();
			this.mYaoSaiPaiZhuJingLingPartWindow.Body.Add(this.mYaoSaiPaiZhuJingLingPart);
			this.mYaoSaiPaiZhuJingLingPart.InitJingLingList(DBId, bossId, jingLingZhenRongStr, this.JingLingIndex);
			this.mYaoSaiPaiZhuJingLingPart.SelectJingLingHandler = delegate(object s1, DPSelectedItemEventArgs e1)
			{
				string title = e1.Title;
				int flag = e1.Flag;
				if (this.RefreshMyJingLingHandler != null)
				{
					this.RefreshMyJingLingHandler(null, new DPSelectedItemEventArgs
					{
						Title = title,
						Index = this.JingLingIndex
					});
				}
				if (flag == 1)
				{
					this.ClosePaiZhuJingLingPart();
				}
			};
			this.mYaoSaiPaiZhuJingLingPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.ClosePaiZhuJingLingPart();
				}
			};
		}
	}

	private void ClosePaiZhuJingLingPart()
	{
		if (null != this.mYaoSaiPaiZhuJingLingPartWindow)
		{
			Object.Destroy(this.mYaoSaiPaiZhuJingLingPart);
			this.mYaoSaiPaiZhuJingLingPart = null;
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.mYaoSaiPaiZhuJingLingPartWindow);
			this.mYaoSaiPaiZhuJingLingPartWindow = null;
		}
	}

	protected override void OnDestroy()
	{
	}

	public DPSelectedItemEventHandler SetJingLingHandler;

	public DPSelectedItemEventHandler RefreshMyJingLingHandler;

	public GameObject mClick;

	public ShowNetImage mJingLingIcon;

	private int mjingLingIndex;

	private int mJingLingDBId;

	private GChildWindow mYaoSaiPaiZhuJingLingPartWindow;

	private YaoSaiPaiZhuJingLingPart mYaoSaiPaiZhuJingLingPart;
}
