using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RideZhuangBeiPart : UserControl
{
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (0 < this.mHorseResLoaderList.Count)
		{
			for (int i = 0; i < this.mHorseResLoaderList.Count; i++)
			{
				if (this.mHorseResLoaderList[i] != null)
				{
					this.mHorseResLoaderList[i].Stop();
				}
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mBakImage.URL = "NetImages/GameRes/Images/RidePet/RideEquipBak.jpg";
		this.mBakImage.ImageDownloaded = delegate(object g)
		{
			this.mBakImage.transform.localScale = new Vector3((float)this.mBakImage.ItsSizeWidth, (float)this.mBakImage.ItsSizeHeight, 0f);
		};
		this.InitVlaue();
		this.InitPage();
		this.RefreshionData();
		this.m_ListBox.MouseLeftButtonDownEx = delegate(object e, MouseEvent s)
		{
			this.MouseLeftButtonUp(s);
		};
		this.mGoodsObc = this.m_ListBox.ItemsSource;
		this._BtnAtt.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = 1521
			});
		};
		this.mHorseResLoaderList.Clear();
		if ((double)Global.VersionCode > 7.0)
		{
			this._YinJiBtn.gameObject.SetActive(true);
		}
		else
		{
			this._YinJiBtn.gameObject.SetActive(false);
		}
		this._YinJiBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 11
				});
			}
		};
	}

	public void RefreshionData()
	{
		this.InitModal();
	}

	private ItemCategories GetItemCategoriesHorseRquipByRidePetPosition(RideZhuangBeiPart.RidePetPosition pos)
	{
		switch (pos)
		{
		case RideZhuangBeiPart.RidePetPosition.MaBian:
			return ItemCategories.MaBian;
		case RideZhuangBeiPart.RidePetPosition.HuJu:
			return ItemCategories.HuJu;
		case RideZhuangBeiPart.RidePetPosition.JiangSheng:
			return ItemCategories.JiangSheng;
		case RideZhuangBeiPart.RidePetPosition.MaAn:
			return ItemCategories.MaAn;
		case RideZhuangBeiPart.RidePetPosition.MaCi:
			return ItemCategories.MaCi;
		case RideZhuangBeiPart.RidePetPosition.MaZhang:
			return ItemCategories.MaZhang;
		default:
			return ItemCategories.MaBian;
		}
	}

	private void InitVlaue()
	{
		this.m_DicZhuangBei.Add(ItemCategories.MaBian, new RideZhuangBeiPart.RideEquipData
		{
			RidePos = RideZhuangBeiPart.RidePetPosition.MaBian,
			IsOpen = false,
			IconObject = this.Equips[0],
			Tip = this.Tips[0],
			icon = this.Icons[0]
		});
		this.m_DicZhuangBei.Add(ItemCategories.HuJu, new RideZhuangBeiPart.RideEquipData
		{
			RidePos = RideZhuangBeiPart.RidePetPosition.HuJu,
			IsOpen = false,
			IconObject = this.Equips[1],
			Tip = this.Tips[1],
			icon = this.Icons[1]
		});
		this.m_DicZhuangBei.Add(ItemCategories.JiangSheng, new RideZhuangBeiPart.RideEquipData
		{
			RidePos = RideZhuangBeiPart.RidePetPosition.JiangSheng,
			IsOpen = false,
			IconObject = this.Equips[2],
			Tip = this.Tips[2],
			icon = this.Icons[2]
		});
		this.m_DicZhuangBei.Add(ItemCategories.MaAn, new RideZhuangBeiPart.RideEquipData
		{
			RidePos = RideZhuangBeiPart.RidePetPosition.MaAn,
			IsOpen = false,
			IconObject = this.Equips[3],
			Tip = this.Tips[3],
			icon = this.Icons[3]
		});
		this.m_DicZhuangBei.Add(ItemCategories.MaCi, new RideZhuangBeiPart.RideEquipData
		{
			RidePos = RideZhuangBeiPart.RidePetPosition.MaCi,
			IsOpen = false,
			IconObject = this.Equips[4],
			Tip = this.Tips[4],
			icon = this.Icons[4]
		});
		this.m_DicZhuangBei.Add(ItemCategories.MaZhang, new RideZhuangBeiPart.RideEquipData
		{
			RidePos = RideZhuangBeiPart.RidePetPosition.MaZhang,
			IsOpen = false,
			IconObject = this.Equips[5],
			Tip = this.Tips[5],
			icon = this.Icons[5]
		});
		int num = 0;
		Dictionary<int, bool>.Enumerator enumerator = Global.DicHorseEquipOpen.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (this.m_DicZhuangBei.ContainsKey(this.GetItemCategoriesHorseRquipByRidePetPosition((RideZhuangBeiPart.RidePetPosition)num)))
			{
				RideZhuangBeiPart.RideEquipData rideEquipData = this.m_DicZhuangBei[this.GetItemCategoriesHorseRquipByRidePetPosition((RideZhuangBeiPart.RidePetPosition)num)];
				KeyValuePair<int, bool> keyValuePair = enumerator.Current;
				rideEquipData.IsOpen = keyValuePair.Value;
			}
			num++;
		}
		List<GoodsData> roleHorseEquipGoodsDataList = Global.GetRoleHorseEquipGoodsDataList(Global.Data.RoleID, -1);
		if (0 < roleHorseEquipGoodsDataList.Count)
		{
			for (int i = 0; i < roleHorseEquipGoodsDataList.Count; i++)
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(roleHorseEquipGoodsDataList[i].GoodsID);
				if (roleHorseEquipGoodsDataList[i].Using == 1)
				{
					this.m_DicZhuangBei[(ItemCategories)goodsCatetoriy].SetEquipGoodsData(roleHorseEquipGoodsDataList[i], new MouseEventHandler(this.MouseLeftButtonUp));
					this.m_DicZhuangBei[(ItemCategories)goodsCatetoriy].ShowTips = false;
				}
			}
			for (int j = 0; j < roleHorseEquipGoodsDataList.Count; j++)
			{
				int goodsCatetoriy2 = Global.GetGoodsCatetoriy(roleHorseEquipGoodsDataList[j].GoodsID);
				if (this.m_DicZhuangBei[(ItemCategories)goodsCatetoriy2].EquipGoodsData == null)
				{
					this.m_DicZhuangBei[(ItemCategories)goodsCatetoriy2].ShowTips = true;
				}
			}
		}
		this.m_Draggable.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
	}

	private void InitModal()
	{
		GoodsData roleFightHorseData = Global.GetRoleFightHorseData(Global.Data.RoleID);
		if (roleFightHorseData != null)
		{
			if (this.mModal.ChildGameObjectList != null && 0 < this.mModal.ChildGameObjectList.Count)
			{
				for (int i = this.mModal.ChildGameObjectList.Count - 1; i >= 0; i--)
				{
					if (null != this.mModal.ChildGameObjectList[i])
					{
						Object.Destroy(this.mModal.ChildGameObjectList[i]);
						this.mModal.ChildGameObjectList.RemoveAt(i);
					}
				}
			}
			HorseResLoader horseResLoader = UIHelper.LoadHorseRes(this.mModal, roleFightHorseData.GoodsID, roleFightHorseData.Forge_level + 1, Quaternion.Euler(new Vector3(0f, 135f, 0f)), new Vector3(120f, 120f, 120f), delegate(GameObject g)
			{
				if (this.mModal.ChildGameObjectList != null && 1 < this.mModal.ChildGameObjectList.Count)
				{
					for (int j = this.mModal.ChildGameObjectList.Count - 1; j > 0; j--)
					{
						if (null != this.mModal.ChildGameObjectList[j])
						{
							Object.Destroy(this.mModal.ChildGameObjectList[j]);
							this.mModal.ChildGameObjectList.RemoveAt(this.mModal.ChildGameObjectList.Count - 1);
						}
					}
					this.mModal._Target = this.mModal.ChildGameObjectList[0];
				}
			});
			this.mHorseResLoaderList.Add(horseResLoader);
		}
	}

	private void InitIocn(GGoodIcon icon, GoodsData gd)
	{
		bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.GoodImg.URL = "NetImages/GameRes/Images/Goods/" + Super.GetIconCode(gd.GoodsID) + ".png";
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.ItemCategory = Global.GetCategoriyByGoodsID(gd.GoodsID);
		Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
		icon.BackSpriteName0 = "bagGrid4_bak";
		icon.BackgroundSprite0.gameObject.SetActive(true);
		icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		if (null != icon.GetComponent<BoxCollider>())
		{
			icon.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -0.2f);
		}
		if (gd.GCount > 0 && gd.Using == 0 && gd.Site == 0)
		{
			Global.SetEquipGoodsZhanLiStat(icon, gd);
		}
		icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
		{
			if (ev.IDType == 1)
			{
				Global.ToUseGoods(icon.ItemObject as GoodsData, true, false);
			}
			else if (ev.IDType != 2)
			{
				if (ev.IDType == 4)
				{
					if ((icon.ItemObject as GoodsData).ExcellenceInfo > 0)
					{
						Super.HintMainText(Global.GetLang("卓越及以上装备可回收获得魔晶!"), 10, 3);
						return;
					}
					string text = (icon.ItemObject as GoodsData).Id.ToString();
					if (string.Empty != text)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID((icon.ItemObject as GoodsData).GoodsID);
						if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao) && goodsXmlNodeByID.SuitID >= Global.ShenqiZaizaoSuit)
						{
							Super.HintMainText(string.Format(Global.GetLang("需要开启【{0}】系统才能回收"), GongnengYugaoMgr.GetGongNengName(GongNengIDs.ZaiZao)), 10, 3);
							return;
						}
						if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
						{
							GameInstance.Game.SpriteOneKeyQuickSaleOut(3, text);
						}
						else
						{
							GameInstance.Game.SpriteOneKeyQuickSaleOut(1, text);
						}
					}
				}
				else if (ev.IDType == 7)
				{
					Global.ToUseGoods(icon.ItemObject as GoodsData, true, false);
				}
				else if (ev.IDType == 10)
				{
					if (ev.ID > 0)
					{
						GoodsData goodsData = icon.ItemObject as GoodsData;
						if (Global.CanAddGoods(goodsData.GoodsID, ev.ID, goodsData.Binding, goodsData.Endtime, false))
						{
							GameInstance.Game.SpriteSplitGoods(goodsData.Id, goodsData.Site, goodsData.GoodsID, ev.ID);
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再拆分物品..."), new object[0]), 1, -1, -1, 0);
						}
					}
				}
				else if (ev.IDType != 5)
				{
					if (ev.IDType == 15)
					{
						GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, 5000, gd.GCount, gd.BagIndex, string.Empty);
					}
					else if (ev.IDType == 17)
					{
						GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, 10000, gd.GCount, gd.BagIndex, string.Empty);
					}
					else if (ev.IDType == 22)
					{
						GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, 13000, gd.GCount, gd.BagIndex, string.Empty);
					}
				}
			}
		};
	}

	private GGoodIcon GetGoodsIcon(GoodsData gd)
	{
		GGoodIcon ggoodIcon = this.FindGGoodIcon(gd);
		if (null == ggoodIcon)
		{
			ggoodIcon = Global.GetNewGoodIcon();
		}
		this.InitIocn(ggoodIcon, gd);
		return ggoodIcon;
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
		if (!GTipServiceEx.EquipTipWindowVisiable)
		{
			GTipServiceEx.SelfBagOnly = true;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
		}
	}

	private void InitDrag(GameObject obj)
	{
		UIDragPanelContents uidragPanelContents = obj.GetComponent<UIDragPanelContents>();
		if (null == uidragPanelContents)
		{
			uidragPanelContents = obj.AddComponent<UIDragPanelContents>();
		}
		uidragPanelContents.draggablePanel = this.m_Draggable;
		UIPanel component = obj.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
	}

	private IEnumerator InitPage(List<GoodsData> equipList)
	{
		yield return null;
		for (int i = 0; i < equipList.Count; i++)
		{
			if (equipList[i] != null)
			{
				GGoodIcon bakGoodsImage = this.GetGoodsIcon(equipList[i]);
				if (null == bakGoodsImage)
				{
					bakGoodsImage = this.ShowEmptyGGoodIcon();
				}
				this.mGoodsObc.Add(bakGoodsImage);
				this.InitDrag(bakGoodsImage.gameObject);
			}
			if (i != 0 && i % 5 == 0)
			{
				yield return null;
			}
		}
		for (int j = equipList.Count; j < this.mGoodsCount; j++)
		{
			GGoodIcon bakGoodsImage2 = this.ShowEmptyGGoodIcon();
			this.mGoodsObc.Add(bakGoodsImage2);
			this.InitDrag(bakGoodsImage2.gameObject);
			if (j != 0 && j % 5 == 0)
			{
				yield return null;
			}
		}
		yield break;
	}

	private void InitPage()
	{
		List<GoodsData> roleHorseEquipGoodsDataList = Global.GetRoleHorseEquipGoodsDataList(Global.Data.RoleID, 0);
		int num = roleHorseEquipGoodsDataList.Count;
		if (20 > num)
		{
			num = 20;
		}
		this.mGoodsCount = num - num % 5 + ((0 >= num % 5) ? 0 : 5);
		roleHorseEquipGoodsDataList.Sort(delegate(GoodsData a, GoodsData b)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(a.GoodsID);
			GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(b.GoodsID);
			if (goodsXmlNodeByID.Categoriy != goodsXmlNodeByID2.Categoriy)
			{
				return goodsXmlNodeByID.Categoriy - goodsXmlNodeByID2.Categoriy;
			}
			if (goodsXmlNodeByID.SuitID == goodsXmlNodeByID2.SuitID)
			{
				return Global.GetZhuoyueAttributeCount(b) - Global.GetZhuoyueAttributeCount(a);
			}
			return goodsXmlNodeByID2.SuitID - goodsXmlNodeByID.SuitID;
		});
		base.StartCoroutine<bool>(this.InitPage(roleHorseEquipGoodsDataList));
	}

	public GGoodIcon ShowEmptyGGoodIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private void onDragFinished()
	{
	}

	private GGoodIcon FindGGoodIcon(GoodsData gd)
	{
		for (int i = 0; i < this.mGoodsObc.Count; i++)
		{
			GameObject at = this.mGoodsObc.GetAt(i);
			if (null != at)
			{
				GGoodIcon component = at.GetComponent<GGoodIcon>();
				if (null != component)
				{
					GoodsData goodsData = component.ItemObject as GoodsData;
					if (goodsData != null && gd.Id == goodsData.Id)
					{
						return component;
					}
				}
			}
		}
		return null;
	}

	private GGoodIcon FindEmpyGoodsIocn()
	{
		for (int i = 0; i < this.mGoodsObc.Count; i++)
		{
			GameObject at = this.mGoodsObc.GetAt(i);
			if (null != at)
			{
				GGoodIcon component = at.GetComponent<GGoodIcon>();
				if (null != component)
				{
					if (!(component.ItemObject is GoodsData))
					{
						return component;
					}
					if (!NGUITools.GetActive(component.GoodImg.gameObject))
					{
						return component;
					}
				}
			}
		}
		return null;
	}

	private void RefreshIcon(GoodsData gd)
	{
		GGoodIcon ggoodIcon = this.FindGGoodIcon(gd);
		if (null != ggoodIcon)
		{
			if (0 >= gd.GCount)
			{
				if (ggoodIcon.TeXiao != null)
				{
					ggoodIcon.TeXiao.gameObject.SetActive(false);
				}
				ggoodIcon.BackgroundSprite1.gameObject.SetActive(false);
				ggoodIcon.BackgroundSprite15.gameObject.SetActive(false);
				ggoodIcon.BindingSprite.gameObject.SetActive(false);
				ggoodIcon.NoUseSprite.gameObject.SetActive(false);
				ggoodIcon.ZhanLiSprite.gameObject.SetActive(false);
				ggoodIcon.EndTimeSprite.gameObject.SetActive(false);
				ggoodIcon.GoodImg.SetTexture(null);
				ggoodIcon.GoodImg.ImageURL = string.Empty;
				ggoodIcon.ItemObject = null;
				NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			}
			else if (gd.Using == 1)
			{
				if (ggoodIcon.TeXiao != null)
				{
					ggoodIcon.TeXiao.gameObject.SetActive(false);
				}
				ggoodIcon.BackgroundSprite1.gameObject.SetActive(false);
				ggoodIcon.BackgroundSprite15.gameObject.SetActive(false);
				ggoodIcon.BindingSprite.gameObject.SetActive(false);
				ggoodIcon.NoUseSprite.gameObject.SetActive(false);
				ggoodIcon.ZhanLiSprite.gameObject.SetActive(false);
				ggoodIcon.EndTimeSprite.gameObject.SetActive(false);
				ggoodIcon.GoodImg.SetTexture(null);
				ggoodIcon.GoodImg.ImageURL = string.Empty;
				ggoodIcon.ItemObject = null;
				NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			}
			else if (null != ggoodIcon)
			{
				ggoodIcon.GoodImg.gameObject.SetActive(true);
				this.InitIocn(ggoodIcon, gd);
			}
		}
		else
		{
			ggoodIcon = this.FindEmpyGoodsIocn();
			if (null != ggoodIcon)
			{
				ggoodIcon.GoodImg.gameObject.SetActive(true);
				this.InitIocn(ggoodIcon, gd);
			}
			else
			{
				this.mGoodsCount += 5;
				for (int i = this.mGoodsObc.Count; i < this.mGoodsCount; i++)
				{
					GGoodIcon ggoodIcon2 = this.ShowEmptyGGoodIcon();
					this.mGoodsObc.AddNoUpdate(ggoodIcon2);
					this.InitDrag(ggoodIcon2.gameObject);
				}
				ggoodIcon = this.FindEmpyGoodsIocn();
				if (null != ggoodIcon)
				{
					ggoodIcon.GoodImg.gameObject.SetActive(true);
					this.InitIocn(ggoodIcon, gd);
				}
			}
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		for (int j = 0; j < this.mGoodsObc.Count; j++)
		{
			GameObject at = this.mGoodsObc.GetAt(j);
			if (null != at)
			{
				GGoodIcon component = at.GetComponent<GGoodIcon>();
				if (null != component)
				{
					GoodsData goodsData = component.ItemObject as GoodsData;
					if (goodsData != null)
					{
						GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
						if (goodsXmlNodeByID.Categoriy == goodsXmlNodeByID2.Categoriy && goodsData.GCount > 0 && goodsData.Using == 0 && goodsData.Site == 0)
						{
							Global.SetEquipGoodsZhanLiStat(component, goodsData);
						}
					}
				}
			}
		}
	}

	internal void NoticRefreshEquip(GoodsData data)
	{
		if (data.GCount <= 0)
		{
			if (this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].EquipGoodsData != null && this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].EquipGoodsData.Id == data.Id)
			{
				this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].SetEquipGoodsData(null, new MouseEventHandler(this.MouseLeftButtonUp));
				this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].ShowTips = false;
			}
		}
		else if (data.Using == 1)
		{
			this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].ShowTips = false;
			this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].SetEquipGoodsData(data, new MouseEventHandler(this.MouseLeftButtonUp));
		}
		else if (this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].EquipGoodsData != null && this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].EquipGoodsData.Id == data.Id)
		{
			this.m_DicZhuangBei[(ItemCategories)Global.GetCategoriyByGoodsID(data.GoodsID)].SetEquipGoodsData(null, new MouseEventHandler(this.MouseLeftButtonUp));
		}
		this.RefreshIcon(data);
	}

	public DPSelectedItemEventHandler Hander;

	public GameObject[] Equips;

	public GGoodIcon[] Icons;

	public GameObject[] Tips;

	[SerializeField]
	public GButton _BtnAtt;

	public ListBox m_ListBox;

	public UISprite m_SpListBox;

	[SerializeField]
	public UIDraggablePanel m_Draggable;

	[SerializeField]
	private ShowNetImage mBakImage;

	[SerializeField]
	private Modal3DShow mModal;

	[SerializeField]
	private GButton _YinJiBtn;

	private Dictionary<ItemCategories, RideZhuangBeiPart.RideEquipData> m_DicZhuangBei = new Dictionary<ItemCategories, RideZhuangBeiPart.RideEquipData>();

	private ObservableCollection mGoodsObc;

	private List<HorseResLoader> mHorseResLoaderList = new List<HorseResLoader>();

	private int mGoodsCount;

	private class RideEquipData
	{
		public bool IsOpen
		{
			get
			{
				return this.mIsOpen;
			}
			set
			{
				this.mIsOpen = value;
				if (null != this.IconObject)
				{
					this.IconObject.SetActive(this.mIsOpen);
				}
			}
		}

		public bool ShowTips
		{
			set
			{
				this.Tip.SetActive(value);
			}
		}

		public void SetEquipGoodsData(GoodsData data, MouseEventHandler handler)
		{
			if (data != null)
			{
				this.mIocnGoodsData = data.Clone();
			}
			else
			{
				this.mIocnGoodsData = null;
			}
			if (this.mIocnGoodsData == null)
			{
				this.icon.gameObject.SetActive(false);
			}
			else
			{
				if (null != this.icon)
				{
					GameObject gameObject = this.icon.transform.parent.gameObject;
					Object.Destroy(this.icon.gameObject);
					this.icon = Global.GetNewGoodIcon();
					U3DUtils.AddChild(gameObject, this.icon.gameObject, true);
					this.icon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
				}
				this.icon.Width = 78.0;
				this.icon.Height = 78.0;
				this.icon.gameObject.SetActive(true);
				this.icon.GoodImg.URL = "NetImages/GameRes/Images/Goods/" + Super.GetIconCode(this.mIocnGoodsData.GoodsID) + ".png";
				this.icon.ItemCode = this.mIocnGoodsData.GoodsID;
				this.icon.ItemObject = this.mIocnGoodsData;
				this.icon.ItemCategory = Global.GetCategoriyByGoodsID(this.mIocnGoodsData.GoodsID);
				Super.InitGoodsGIcon(this.icon, this.mIocnGoodsData, true, IconTextTypes.Qianghua);
				this.icon.BackSpriteName0 = "bagGrid4_bak";
				this.icon.BackgroundSprite0.gameObject.SetActive(false);
				this.icon.addEventListener("click", handler);
				this.icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
				{
					if (ev.IDType == 1)
					{
						Global.ToUseGoods(this.icon.ItemObject as GoodsData, true, false);
					}
					else if (ev.IDType == 2)
					{
						this.mIocnGoodsData.Using = 0;
						GameInstance.Game.SpriteModGoods(2, this.mIocnGoodsData.Id, this.mIocnGoodsData.GoodsID, this.mIocnGoodsData.Using, 0, this.mIocnGoodsData.GCount, this.mIocnGoodsData.BagIndex, string.Empty);
					}
					else if (ev.IDType == 4)
					{
						if ((this.icon.ItemObject as GoodsData).ExcellenceInfo > 0)
						{
							Super.HintMainText(Global.GetLang("卓越及以上装备可回收获得魔晶!"), 10, 3);
							return;
						}
						string text = (this.icon.ItemObject as GoodsData).Id.ToString();
						if (string.Empty != text)
						{
							GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID((this.icon.ItemObject as GoodsData).GoodsID);
							if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao) && goodsXmlNodeByID.SuitID >= Global.ShenqiZaizaoSuit)
							{
								Super.HintMainText(string.Format(Global.GetLang("需要开启【{0}】系统才能回收"), GongnengYugaoMgr.GetGongNengName(GongNengIDs.ZaiZao)), 10, 3);
								return;
							}
							if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
							{
								GameInstance.Game.SpriteOneKeyQuickSaleOut(3, text);
							}
							else
							{
								GameInstance.Game.SpriteOneKeyQuickSaleOut(1, text);
							}
						}
					}
					else if (ev.IDType == 7)
					{
						Global.ToUseGoods(this.icon.ItemObject as GoodsData, true, false);
					}
					else if (ev.IDType == 10)
					{
						if (ev.ID > 0)
						{
							GoodsData goodsData = this.icon.ItemObject as GoodsData;
							if (Global.CanAddGoods(goodsData.GoodsID, ev.ID, goodsData.Binding, goodsData.Endtime, false))
							{
								GameInstance.Game.SpriteSplitGoods(goodsData.Id, goodsData.Site, goodsData.GoodsID, ev.ID);
							}
							else
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再拆分物品..."), new object[0]), 1, -1, -1, 0);
							}
						}
					}
					else if (ev.IDType != 5)
					{
						if (ev.IDType == 15)
						{
							GameInstance.Game.SpriteModGoods(3, this.mIocnGoodsData.Id, this.mIocnGoodsData.GoodsID, this.mIocnGoodsData.Using, 5000, this.mIocnGoodsData.GCount, this.mIocnGoodsData.BagIndex, string.Empty);
						}
						else if (ev.IDType == 17)
						{
							GameInstance.Game.SpriteModGoods(3, this.mIocnGoodsData.Id, this.mIocnGoodsData.GoodsID, this.mIocnGoodsData.Using, 10000, this.mIocnGoodsData.GCount, this.mIocnGoodsData.BagIndex, string.Empty);
						}
						else if (ev.IDType == 22)
						{
							GameInstance.Game.SpriteModGoods(3, this.mIocnGoodsData.Id, this.mIocnGoodsData.GoodsID, this.mIocnGoodsData.Using, 13000, this.mIocnGoodsData.GCount, this.mIocnGoodsData.BagIndex, string.Empty);
						}
					}
				};
			}
		}

		public GoodsData EquipGoodsData
		{
			get
			{
				return this.mIocnGoodsData;
			}
		}

		public RideZhuangBeiPart.RidePetPosition RidePos = RideZhuangBeiPart.RidePetPosition.HuJu;

		public GameObject IconObject;

		public GameObject Tip;

		public GGoodIcon icon;

		private bool mIsOpen;

		private GoodsData mIocnGoodsData;
	}

	public enum RidePetPosition
	{
		MaBian,
		JiangSheng = 2,
		MaCi = 4,
		HuJu = 1,
		MaAn = 3,
		MaZhang = 5
	}
}
