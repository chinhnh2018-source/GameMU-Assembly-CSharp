using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZhuangBeiChongShengPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Bak.URL = "NetImages/GameRes/Images/Plate/beibaokuangdi.jpg";
		this.zhandouli.Pivot = 3;
		this.zhandouli.X = 19.0;
		this.zhandouli.Y = -180.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.equipIcon[37] = this.wuqizuoIcon;
		this.equipIcon[38] = this.wuqiyouIcon;
		this.equipIcon[30] = this.toukuiIcon;
		this.equipIcon[31] = this.kaijiaIcon;
		this.equipIcon[32] = this.hushouIcon;
		this.equipIcon[33] = this.hutuiIcon;
		this.equipIcon[34] = this.xueziIcon;
		this.equipIcon[35] = this.xianglianIcon;
		this.equipIcon[36] = this.jiezhizuoIcon;
		this.equipIcon[136] = this.jiezhiyouIcon;
	}

	public void InitPartData()
	{
		this.zhandouli.Text = Global.Data.roleData.RebornCombatForce.ToString();
		this.StartSetEquipIcon();
		if (SceneUIClasses.RebornMap.IsTheScene())
		{
			this.checkBoxShow._Lable.text = Global.GetLang("显示时装");
			this.checkBoxShow.isChecked = (Global.Data.roleData.RebornShowModel == 1);
		}
		else
		{
			this.checkBoxShow._Lable.text = Global.GetLang("显示重生装备");
			this.checkBoxShow.isChecked = (Global.Data.roleData.RebornShowEquip == 1);
		}
		this.checkBoxShow.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				GameInstance.Game.RebornModelShow();
			}
			else
			{
				GameInstance.Game.RebornShow();
			}
		};
	}

	public void RefershBeRebornShow()
	{
		if (SceneUIClasses.RebornMap.IsTheScene())
		{
			this.checkBoxShow.isChecked = (Global.Data.roleData.RebornShowModel == 1);
		}
		else
		{
			this.checkBoxShow.isChecked = (Global.Data.roleData.RebornShowEquip == 1);
		}
	}

	private void StartSetEquipIcon()
	{
		ChongShengData.GetUsingChongShengGoodsDataList();
		this.usingGoodsList = Super.GData.RoleUsingChongShengGoodsDataList;
		if (this.usingGoodsList == null)
		{
			return;
		}
		foreach (KeyValuePair<int, GoodsData> keyValuePair in this.usingGoodsList)
		{
			GoodsData value = keyValuePair.Value;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy >= 30 && categoriy <= 38)
				{
					this.SetEquipIcon(categoriy, value);
				}
			}
		}
	}

	public void SetEquipIcon(int equipCategory, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		int categoriy = goodsXmlNodeByID.Categoriy;
		if (equipCategory == categoriy)
		{
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.isAutoSize = true;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Equip/{0}.png", new object[]
			{
				Super.GetIconCode(goodsXmlNodeByID)
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = goodsData.GoodsID;
			icon.ItemObject = goodsData;
			icon.BoxTypes = 0;
			icon.TextSize = 20;
			icon.Tag = goodsData.ExcellenceInfo;
			this.InitGoodIconSize(icon, icon.ItemCategory);
			if (Global.GetZhuoyueAttributeCount(goodsData) > 0)
			{
				this.SetExcellenceStat(icon, categoriy);
			}
			this.SetEquipBorderBySuitID(icon, goodsData);
			icon.BindingSprite.gameObject.SetActive(goodsData.Binding > 0);
			Vector3 localScale = icon.BackgroundSprite1.transform.localScale;
			icon.BindingSprite.transform.localPosition = this.Pos(localScale, -(localScale.x / 2f - 12f), -(localScale.y / 2f - 12f), -0.03f);
			icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 2)
				{
					if (Global.Data.GameScene.IsDead())
					{
						return;
					}
					GoodsData goodsData2 = icon.ItemObject as GoodsData;
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData2.GoodsID);
					if (Global.CanAddGoods(goodsData2.GoodsID, goodsData2.GCount, goodsData2.Binding, goodsData2.Endtime, false))
					{
						if (goodsData2.Using == 1)
						{
							goodsData2.Using = 0;
							GameInstance.Game.SpriteModGoods(2, goodsData2.Id, goodsData2.GoodsID, goodsData2.Using, goodsData2.Site, goodsData2.GCount, goodsData2.BagIndex, string.Empty);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再卸载装备..."), new object[0]), 1, -1, -1, 0);
					}
				}
				else if (ev.IDType == 16)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -10
					});
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1330
					});
				}
			};
			icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				this.SetBoxCollider(icon);
			};
			int handType = goodsXmlNodeByID.HandType;
			icon.transform.name = string.Format("{0}::{1}", (ItemCategories)goodsXmlNodeByID.Categoriy, goodsData.BagIndex);
			this.SetZhuangBeiPeiDai(icon, equipCategory, handType, goodsData.BagIndex);
		}
	}

	public void RemoveEquipIcon(GoodsData gd)
	{
		foreach (KeyValuePair<int, SpriteSL> keyValuePair in this.equipIcon)
		{
			if (keyValuePair.Value != null && keyValuePair.Value.transform.childCount > 0)
			{
				GoodsData goodsData = keyValuePair.Value.getChildAt(0).GetComponent<GGoodIcon>().ItemObject as GoodsData;
				if (goodsData != null && goodsData.Id == gd.Id && this.equipIcon[keyValuePair.Key].Count() > 0)
				{
					this.equipIcon[keyValuePair.Key].Remove(0);
				}
			}
		}
	}

	public void SetZhuangBeiPeiDai(GGoodIcon icon, int categoriy, int handType, int iBagIndex)
	{
		if (categoriy == 36)
		{
			if (iBagIndex == 0)
			{
				if (this.equipIcon[categoriy].Count() > 0)
				{
					this.equipIcon[categoriy].RemoveAt(0, true, true);
				}
				this.equipIcon[categoriy].Add(icon);
			}
			else if (iBagIndex == 1)
			{
				if (this.equipIcon[136].Count() > 0)
				{
					this.equipIcon[136].RemoveAt(0, true, true);
				}
				this.equipIcon[136].Add(icon);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"戒子的数据出错  位置为非左手、右手"
				});
				if (this.equipIcon[136].Count() == 0)
				{
					this.equipIcon[136].Add(icon);
				}
				else if (this.equipIcon[categoriy].Count() == 0)
				{
					this.equipIcon[categoriy].Add(icon);
				}
				else
				{
					if (this.equipIcon[categoriy].Count() > 0)
					{
						this.equipIcon[categoriy].RemoveAt(0, true, true);
					}
					this.equipIcon[categoriy].Add(icon);
				}
			}
		}
		else
		{
			if (this.equipIcon[categoriy].Count() > 0)
			{
				this.equipIcon[categoriy].RemoveAt(0, true, true);
			}
			this.equipIcon[categoriy].Add(icon);
		}
	}

	public void RefreshEquipGoods(GoodsData gd)
	{
		this.StartSetEquipIcon();
	}

	public void SetBoxCollider(GGoodIcon icon)
	{
		bool flag = Global.CanUseGoodsAttr(icon.ItemCode, false);
		if (icon.ItemCategory == 37 || icon.ItemCategory == 38 || icon.ItemCategory == 31)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(83f, 129f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(83f, 129f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 30 || icon.ItemCategory == 32 || icon.ItemCategory == 34 || icon.ItemCategory == 33)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(80f, 80f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(80f, 80f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 36 || icon.ItemCategory == 35)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(53f, 53f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(53f, 53f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		Super.InitEquipGIcon(icon, icon.ItemObject as GoodsData, true, IconTextTypes.Qianghua);
		icon.ContentText.Pivot = 2;
		icon.ContentText.X = (double)(icon.GetComponent<BoxCollider>().size.x / 2f);
		icon.ContentText.Y = (double)(icon.GetComponent<BoxCollider>().size.y / 2f);
	}

	public void MouseLeftButtonUp(MouseEvent evt)
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
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
	}

	public void InitGoodIconSize(GGoodIcon icon, int iCategoriy)
	{
		int num = 64;
		int num2 = 64;
		if (iCategoriy == 37 || iCategoriy == 38 || iCategoriy == 31)
		{
			num = 83;
			num2 = 129;
		}
		else if (iCategoriy == 36 || iCategoriy == 35)
		{
			num = 53;
			num2 = 53;
		}
		else if (iCategoriy == 30 || iCategoriy == 32 || iCategoriy == 34 || iCategoriy == 33)
		{
			num = 80;
			num2 = 80;
		}
		icon.Width = (double)num;
		icon.Height = (double)num2;
	}

	public void SetExcellenceStat(GGoodIcon icon, int iCategoriy)
	{
		if (iCategoriy == 37 || iCategoriy == 38 || iCategoriy == 31)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(83f, 129f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xiongjia");
		}
		else if (iCategoriy == 36 || iCategoriy == 35)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(53f, 53f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xianglian");
		}
		else if (iCategoriy == 30 || iCategoriy == 32 || iCategoriy == 34 || iCategoriy == 33)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(80f, 80f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_toukui");
		}
		icon.BackgroundSprite1Visible = true;
	}

	private void SetExcellence(GGoodIcon icon, string zhuoyueTeXiaoPrefab)
	{
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (zhuoyueAttributeCount > 0)
		{
			if (zhuoyueAttributeCount >= 6)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (zhuoyueAttributeCount < 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount < 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (zhuoyueAttributeCount == 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
		}
	}

	public void SetEquipBorderBySuitID(GGoodIcon icon, GoodsData goodsData)
	{
		if (null == icon)
		{
			return;
		}
		if (goodsData == null)
		{
			return;
		}
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		Transform transform = icon.BackgroundSprite15.transform;
		if (categoriyByGoodsID == 37 || categoriyByGoodsID == 38 || categoriyByGoodsID == 31)
		{
			transform.localScale = new Vector3(93f, 139f);
			transform.localPosition = new Vector3(0.5f, 0.5f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 36 || categoriyByGoodsID == 35)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(63f, 63f);
			transform.localPosition = new Vector3(0.5f, 0.5f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 30 || categoriyByGoodsID == 32 || categoriyByGoodsID == 34 || categoriyByGoodsID == 33)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(90f, 90f);
		}
		Vector3 vector = icon.BackgroundSprite15.transform.localScale;
		vector += new Vector3(2f, 2f, 0f);
		icon.BackSpriteName15 = "iconStateGold";
		icon.BackgroundSprite15.transform.localScale = vector;
	}

	private Vector3 Pos(Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
		return v;
	}

	private const int JieZhiYou = 136;

	public SpriteSL toukuiIcon;

	public SpriteSL wuqizuoIcon;

	public SpriteSL wuqiyouIcon;

	public SpriteSL xianglianIcon;

	public SpriteSL kaijiaIcon;

	public SpriteSL hushouIcon;

	public SpriteSL jiezhizuoIcon;

	public SpriteSL jiezhiyouIcon;

	public SpriteSL hutuiIcon;

	public SpriteSL xueziIcon;

	public TextBlock zhandouli;

	public GCheckBox checkBoxShow;

	public Dictionary<int, SpriteSL> equipIcon = new Dictionary<int, SpriteSL>();

	private Dictionary<int, GoodsData> usingGoodsList = new Dictionary<int, GoodsData>();

	private bool FirstGetNewData = true;

	private int CurrentTaoZhuangIndex = -1;

	private int CurrentZhuoYueIndex = -1;

	private int CurrentChengJiuLevel = -1;

	private int CurrentJunXianLevel = -1;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public ShowNetImage Bak;
}
