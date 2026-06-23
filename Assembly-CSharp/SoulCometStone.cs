using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SoulCometStone : UserControl
{
	private void InitTextInPrefabs()
	{
		this.descriptionLabel.Text = Global.GetLang("同一魂阵不能镶嵌相同的魂石");
		this.descriptionLabel.X = -370.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.gatheringBtn.gameObject).onClick = delegate(GameObject s)
		{
			this.GatheringSoulCometStone();
		};
		UIEventListener.Get(this.groupBtn.gameObject).onClick = delegate(GameObject s)
		{
			this.GroupSoulComet();
		};
		this.GetSlotConfig();
		this.InitSoulCometStoneSlots();
		this.InitTabbars();
		this.SetTabbarHighlight(0);
	}

	private new void OnDestroy()
	{
		Super.HideNetWaiting();
	}

	private void InitSoulCometStoneSlots()
	{
		if (this.stones == null || this.stones.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.stones.Count; i++)
		{
			UIEventListener.Get(this.stones[i].gameObject).onClick = delegate(GameObject s)
			{
				int num = this.circleID * 100 + Convert.ToInt32(s.name);
				GoodsData equipedSoulCometStoneBySlotID = Global.GetEquipedSoulCometStoneBySlotID(num);
				if (equipedSoulCometStoneBySlotID != null)
				{
					return;
				}
				this.selectSlot = num;
				this.ShowAvailableStoneWindow(num / 100);
			};
		}
	}

	private void InitTabbars()
	{
		if (this.circleItems == null || this.circleItems.Length <= 0)
		{
			return;
		}
		string[] array = new string[]
		{
			Global.GetLang("星魂阵"),
			Global.GetLang("月魂阵"),
			Global.GetLang("日魂阵")
		};
		for (int i = 0; i < this.circleItems.Length; i++)
		{
			TabbarItem tabbarItem = this.circleItems[i];
			tabbarItem.barName = array[i];
			tabbarItem.tabbarItemState = TabbarItemState.Disabled;
			tabbarItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.TabbarSelectChanged(e.ID);
			};
		}
	}

	private void SetTabbarHighlight(int hoverIndex = 0)
	{
		if (this.circleItems == null || this.circleItems.Length <= 0 || hoverIndex >= this.activeCount)
		{
			return;
		}
		for (int i = 0; i < this.activeCount; i++)
		{
			TabbarItem tabbarItem = this.circleItems[i];
			tabbarItem.tabbarItemState = ((i != hoverIndex) ? TabbarItemState.Normal : TabbarItemState.Hover);
		}
		for (int j = this.activeCount; j < 3; j++)
		{
			TabbarItem tabbarItem2 = this.circleItems[j];
			tabbarItem2.tabbarItemState = TabbarItemState.Disabled;
		}
	}

	public void InitSoulCometStone()
	{
		this.RefreshSoulCometStoneAllSlots();
		this.SetSoulCometStoneProperties();
	}

	private void TabbarSelectChanged(int select_index)
	{
		if (!this.IsSlotActived(select_index))
		{
			this.ShowUnactiveHintMsg(select_index);
			base.Invoke("HideUnactiveHintMsg", 0.5f);
		}
		if (this.selectedIndex == select_index)
		{
			return;
		}
		if (select_index >= 0 && select_index < this.activeCount)
		{
			this.circleID = select_index + 1;
			this.RefreshSoulCometStoneAllSlots();
		}
		this.selectedIndex = select_index;
		this.SetTabbarHighlight(select_index);
	}

	private void ShowUnactiveHintMsg(int slotID)
	{
		string msg = this.UnActiveMsg(slotID);
		PlayZone.GlobalPlayZone.ShowUnactiveHintMsgWindow(msg);
	}

	private void HideUnactiveHintMsg()
	{
		PlayZone.GlobalPlayZone.CloseUnactiveHintMsgWindow();
	}

	private void GetSlotConfig()
	{
		if (this.dic_slot == null)
		{
			this.dic_slot = new Dictionary<int, LevelAttribute>();
		}
		this.dic_slot.Clear();
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("HunShiOpen", '|');
		if (systemParamStringArrayByName.Length <= 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				LevelAttribute levelAttribute = new LevelAttribute(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
				this.dic_slot.Add(i, levelAttribute);
				int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
				int level = Global.Data.roleData.Level;
				if (changeLifeCount >= levelAttribute.grade && level >= levelAttribute.level)
				{
					num++;
				}
			}
		}
		this.activeCount = num;
	}

	private bool IsSlotActived(int slotID)
	{
		return slotID < this.activeCount;
	}

	private string UnActiveMsg(int slotID)
	{
		if (this.dic_slot == null || this.dic_slot.Count <= slotID)
		{
			return string.Empty;
		}
		LevelAttribute value = this.dic_slot.GetValue(slotID);
		if (value == null)
		{
			return string.Empty;
		}
		return string.Format(Global.GetLang("等级达到{0}转{1}级开启"), value.grade, value.level);
	}

	private void StrengtheningSoulCometStone(GoodsData selectGoodsData, SoulCometStoneStrengtheningType strengtheningType, SoulCometStoneStrengtheningDataChangeType dataChangeType = SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_Reload)
	{
		if (selectGoodsData == null)
		{
			return;
		}
		DPSelectedItemEventHandler strengtheningCallBack = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null && args.ID == 101)
			{
				GoodsData goodsData = args.Data as GoodsData;
				selectGoodsData = goodsData;
				if (args.Type == 0)
				{
					Global.OnEquipedSoulCometStoneDataChanged(goodsData.Id, goodsData, EquipedDiamondModifyType.EquipedDiamondModifyType_Replace);
					this.OnEquipedSoulCometStoneChanged(goodsData, EquipedDiamondModifyType.EquipedDiamondModifyType_Replace);
				}
				else if (args.Type == 1)
				{
					Global.OnGoodsChangedInSoulCometStoneBag(goodsData, BagModifyType.BagModifyType_Replace);
					this.OnSoulCometStoneBagChanged(goodsData);
					if (null != this.soulCometStoneBag)
					{
						this.soulCometStoneBag.SelectGoodsIconInStrengthening(goodsData.Id);
					}
				}
			}
		};
		this.ShowSoulCometStoneStrengtheningView(selectGoodsData, strengtheningType, dataChangeType, strengtheningCallBack);
		this.HideSoulCometStoneStrengtheningView(false);
		this.ShowSoulCometStoneBagView(true);
		this.SetSoulCometStoneBagMode((strengtheningType != SoulCometStoneStrengtheningType.StrengtheningType_Bag) ? SoulCometStoneBagTypes.Strengthening_Body : SoulCometStoneBagTypes.Strengthening_Bag);
	}

	private void EquipSoulCometStone(bool equipOrNot = true)
	{
		if (this.selectGoodsData == null || !Global.IsSoulCometStoneSlotID(this.selectSlot))
		{
			return;
		}
		this.EquipSoulCometStoneRequst(this.selectSlot, (!equipOrNot) ? -1 : this.selectGoodsData.Id);
	}

	private void GatheringSoulCometStone()
	{
		this.HideStoneEquipingView(true);
		this.ShowSoulCometStoneBagView(true);
		this.SetSoulCometStoneBagMode(SoulCometStoneBagTypes.Gathering);
		this.ShowSoulCometStoneGatheringView(true);
	}

	private void GroupSoulComet()
	{
		Dictionary<int, GoodsData> equipedSoulCometStoneListByCircleID = SoulCometStone.GetEquipedSoulCometStoneListByCircleID(this.circleID);
		this.ShowSoulCometStoneGroupView(equipedSoulCometStoneListByCircleID);
	}

	private void SetSoulCometStoneProperties()
	{
		if (null == this.propertyLabel)
		{
			return;
		}
		double[] totalBaseProperties = this.GetTotalBaseProperties();
		if (totalBaseProperties == null)
		{
			this.propertyLabel.Text = string.Empty;
		}
		else
		{
			this.propertyLabel.Text = SoulCometStone.GetBaseAttributeStrFromPropertyList(totalBaseProperties, 3);
			BoxCollider component = this.propertyLabel.GetComponent<BoxCollider>();
			float num = (float)this.propertyLabel.ActualWidth;
			float num2 = (float)this.propertyLabel.ActualHeight;
			num2 /= this.propertyLabel.transform.localScale.y;
			num /= this.propertyLabel.transform.localScale.x;
			component.size = new Vector3(num, num2, 1f);
			Vector3 center = component.center;
			center.y = -num2 / 2f;
			component.center = center;
		}
	}

	private double[] GetTotalBaseProperties()
	{
		List<GoodsData> equipedSoulCometStoneList = Global.GetEquipedSoulCometStoneList();
		if (equipedSoulCometStoneList == null || equipedSoulCometStoneList.Count <= 0)
		{
			return null;
		}
		double[] array = new double[177];
		for (int i = 0; i < equipedSoulCometStoneList.Count; i++)
		{
			GoodsData goodsData = equipedSoulCometStoneList[i];
			if (goodsData != null)
			{
				int num = 0;
				int num2 = Mathf.Max(0, Global.GetSoulCometStoneLevel(goodsData, out num));
				double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
				for (int j = 0; j < array.Length; j++)
				{
					array[j] += goodsEquipPropsDoubleList[j] * (double)num2;
				}
			}
		}
		return array;
	}

	public static string GetBaseAttributeStrFromPropertyList(double[] equipFields, int fillCount = 0)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = "e3b36c";
		string lang = Global.GetLang("            元素伤害");
		string lang2 = Global.GetLang("            元素穿透");
		string lang3 = Global.GetLang("            元素抵抗");
		string lang4 = Global.GetLang("            基础属性");
		string text5 = "fac60d";
		string text6 = string.Empty;
		string text7 = " ";
		for (int i = 0; i < fillCount; i++)
		{
			text6 += text7;
		}
		for (int j = 69; j <= 74; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num = equipFields[j];
				text2 += string.Format("{0}+{1}", text3, (int)num);
				text2 += "\n";
			}
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text3 = Global.GetColorStringForNGUIText(new object[]
			{
				text5,
				lang
			});
			text3 += "\n";
			text = text + text3 + text2;
		}
		text2 = string.Empty;
		for (int j = 75; j <= 80; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num2 = equipFields[j];
				double num3 = num2 * 100.0;
				text2 += string.Format("{0}+{1}%", text3, Math.Round(num3, 2));
				text2 += "\n";
			}
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text3 = Global.GetColorStringForNGUIText(new object[]
			{
				text5,
				lang2
			});
			text3 += "\n";
			text = text + text3 + text2;
		}
		text2 = string.Empty;
		for (int j = 81; j <= 86; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num4 = equipFields[j];
				double num5 = num4 * 100.0;
				text2 += string.Format("{0}+{1}%", text3, Math.Round(num5, 2));
				text2 += "\n";
			}
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text3 = Global.GetColorStringForNGUIText(new object[]
			{
				text5,
				lang3
			});
			text3 += "\n";
			text = text + text3 + text2;
		}
		text2 = string.Empty;
		for (int j = 1; j <= 10; j += 2)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num6 = equipFields[j];
				if (j == 1)
				{
					if (equipFields[j] != 0.0)
					{
						text2 += string.Format("{0}+{1}%", text3, (int)equipFields[j]);
						text2 += "\n";
					}
				}
				else
				{
					int num7 = j;
					int num8 = j + 1;
					if (equipFields[num7] != 0.0 || equipFields[num8] != 0.0)
					{
						double num9 = equipFields[num7];
						double num10 = equipFields[num8];
						text2 += string.Format("{0}+{1}", text3, (int)num10);
					}
				}
				text2 += "\n";
			}
		}
		for (int j = 11; j <= 68; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num11 = equipFields[j];
				if (ExtPropIndexes.ExtPropIndexPercents[j] == 1)
				{
					double num12 = num11 * 100.0;
					text2 += string.Format("{0}+{1}%", text3, Math.Round(num12, 2));
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[j] == 0)
				{
					text2 += string.Format("{0}+{1}", text3, (int)num11);
				}
				text2 += "\n";
			}
		}
		for (int j = 87; j < 177; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num13 = equipFields[j];
				if (ExtPropIndexes.ExtPropIndexPercents[j] == 1)
				{
					double num14 = num13 * 100.0;
					text2 += string.Format("{0}+{1}%", text3, Math.Round(num14, 2));
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[j] == 0)
				{
					text2 += string.Format("{0}+{1}", text3, (int)num13);
				}
				text2 += "\n";
			}
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text3 = Global.GetColorStringForNGUIText(new object[]
			{
				text5,
				lang4
			});
			text3 += "\n";
			text = text + text3 + text2;
		}
		return Global.ProcessStr(text);
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.TextColor = 15793920U;
			int num = 0;
			int soulCometStoneLevel = Global.GetSoulCometStoneLevel(goodsData, out num);
			ggoodIcon.ContentText.Text = "Lv" + soulCometStoneLevel;
			int equipGoodsSuitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
			if (equipGoodsSuitID == 1)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue";
			}
			if (equipGoodsSuitID == 2)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
			}
			if (equipGoodsSuitID == 3)
			{
				ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
			}
			if (equipGoodsSuitID >= 4 && equipGoodsSuitID <= 10)
			{
				ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
				ggoodIcon.TeXiao.gameObject.SetActive(true);
			}
			ggoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 3)
				{
					this.HideStoneEquipingView(true);
					GoodsData goodsData2 = Global.CloneGoodsData(this.selectGoodsData, false);
					goodsData2.Id = this.selectGoodsData.Id;
					this.StrengtheningSoulCometStone(goodsData2, SoulCometStoneStrengtheningType.StrengtheningType_Equiped, SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_Reload);
				}
				else if (ev.IDType == 2)
				{
					this.EquipSoulCometStone(false);
				}
			};
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			return ggoodIcon;
		}
		return null;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null != ggoodIcon)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			this.selectGoodsData = goodsData;
			this.selectSlot = ((goodsData == null) ? -1 : goodsData.BagIndex);
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.SoulCometStoneBagTip, GoodsOwnerTypes.SoulCometStoneBag, goodsData);
		}
	}

	private int SortGoodsDataList(GoodsData x, GoodsData y)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(x.GoodsID);
		GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(y.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return 1;
		}
		if (goodsXmlNodeByID2 == null)
		{
			return -1;
		}
		int num = 0;
		int num2 = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(x, out num);
		int soulCometStoneLevel2 = Global.GetSoulCometStoneLevel(y, out num2);
		if (goodsXmlNodeByID.SuitID != goodsXmlNodeByID2.SuitID)
		{
			return goodsXmlNodeByID2.SuitID - goodsXmlNodeByID.SuitID;
		}
		if (soulCometStoneLevel == soulCometStoneLevel2)
		{
			return goodsXmlNodeByID2.ID - goodsXmlNodeByID.ID;
		}
		return soulCometStoneLevel2 - soulCometStoneLevel;
	}

	private void ShowAvailableStoneWindow(int selectCircleID)
	{
		List<GoodsData> availableSoulCometStoneByCircleID = SoulCometStone.GetAvailableSoulCometStoneByCircleID(selectCircleID);
		if (availableSoulCometStoneByCircleID == null || availableSoulCometStoneByCircleID.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("无可用魂石，魂石可通过聚魂获得"), 10, 3);
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			"显示选择魂石窗口"
		});
		availableSoulCometStoneByCircleID.Sort(new Comparison<GoodsData>(this.SortGoodsDataList));
		DPSelectedItemEventHandler equipGoodsCallBack = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null)
			{
				GoodsData goodsData = args.Data as GoodsData;
				this.selectGoodsData = goodsData;
				this.EquipSoulCometStoneRequst(this.selectSlot, goodsData.Id);
			}
		};
		PlayZone.GlobalPlayZone.ShowAvailableGoodsWindow(availableSoulCometStoneByCircleID, GoodsType.GoodsType_FluorescentDiamond, equipGoodsCallBack);
	}

	private void ShowSoulCometStoneBagView(bool visible = true)
	{
		if (!visible && null == this.soulCometStoneBag)
		{
			return;
		}
		if (null == this.soulCometStoneBag)
		{
			this.soulCometStoneBag = U3DUtils.NEW<SoulCometStoneBag>();
			this.soulCometStoneBag.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 1000)
				{
					GoodsData goodsData = e.Data as GoodsData;
					SoulCometStoneStrengtheningDataChangeType idtype = (SoulCometStoneStrengtheningDataChangeType)e.IDType;
					this.StrengtheningSoulCometStone(goodsData, SoulCometStoneStrengtheningType.StrengtheningType_Bag, idtype);
				}
				else if (e.ID == 1100)
				{
					Dictionary<int, GoodsData> selectedGoodsDataDict = e.Data as Dictionary<int, GoodsData>;
					this.RefreshStrengtheningProgress(selectedGoodsDataDict);
				}
				else if (e.ID == 1200)
				{
					if (this.soulCometStoneBag.soulCometStoneBagTypes == SoulCometStoneBagTypes.Gathering && Global.isSoulCometStoneGathering)
					{
						return;
					}
					this.Back();
				}
			};
			this.soulCometStoneBag.InitBag();
			U3DUtils.AddChild(this.modules.gameObject, this.soulCometStoneBag.gameObject, true);
		}
		this.soulCometStoneBag.gameObject.SetActive(visible);
	}

	private void ResetSoulCometStoneBag()
	{
		if (null != this.soulCometStoneBag)
		{
			this.soulCometStoneBag.ResetBag();
		}
	}

	private void CloseSoulCometStoneBagView()
	{
		NGUITools.Destroy(this.soulCometStoneBag.gameObject);
	}

	private void SetSoulCometStoneBagMode(SoulCometStoneBagTypes bagType)
	{
		if (null != this.soulCometStoneBag)
		{
			this.soulCometStoneBag.soulCometStoneBagTypes = bagType;
		}
		switch (bagType)
		{
		case SoulCometStoneBagTypes.Gathering:
			this.HideSoulCometStoneStrengtheningView(true);
			break;
		case SoulCometStoneBagTypes.Strengthening_Body:
		case SoulCometStoneBagTypes.Strengthening_Bag:
			this.ShowSoulCometStoneGatheringView(false);
			break;
		}
	}

	private void ShowSoulCometStoneGatheringView(bool visible = true)
	{
		if (!visible && null == this.soulCometStoneGathering)
		{
			return;
		}
		if (null == this.soulCometStoneGathering)
		{
			this.soulCometStoneGathering = U3DUtils.NEW<SoulCometStoneGathering>();
			this.soulCometStoneGathering.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					GoodsData goodsData = Global.PopSoulCometStoneFromQueue();
					if (goodsData != null && null != this.soulCometStoneBag)
					{
						this.soulCometStoneBag.ReplaceGoodsIcon(goodsData);
					}
				}
			};
			this.soulCometStoneGathering.callback = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseMainWindow();
			};
			U3DUtils.AddChild(this.modules.gameObject, this.soulCometStoneGathering.gameObject, true);
		}
		this.soulCometStoneGathering.gameObject.SetActive(visible);
	}

	private void CloseSoulCometStoneGatheringView()
	{
		NGUITools.Destroy(this.soulCometStoneGathering.gameObject);
	}

	private void ShowSoulCometStoneGroupView(Dictionary<int, GoodsData> dic_goods)
	{
		PlayZone.GlobalPlayZone.ShowSoulCometStoneGroupWindow(dic_goods);
	}

	private void ShowSoulCometStoneStrengtheningView(GoodsData targetGoodsData, SoulCometStoneStrengtheningType strengtheningType, SoulCometStoneStrengtheningDataChangeType dataChangeType = SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_Reload, DPSelectedItemEventHandler strengtheningCallBack = null)
	{
		if (null == this.soulCometStoneStrengthening)
		{
			this.soulCometStoneStrengthening = U3DUtils.NEW<SoulCometStoneStrengthening>();
			this.soulCometStoneStrengthening.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 101)
				{
					if (strengtheningCallBack != null)
					{
						strengtheningCallBack(this.soulCometStoneStrengthening.gameObject, new DPSelectedItemEventArgs
						{
							ID = 101,
							Data = e.Data,
							Type = e.Type
						});
					}
				}
				else
				{
					GoodsQuality idtype = (GoodsQuality)e.IDType;
					bool state = Convert.ToBoolean(e.ID);
					GoodsData goodsData = e.Data as GoodsData;
					if (null != this.soulCometStoneBag)
					{
						this.soulCometStoneBag.SelectIconByColor(idtype, state, (goodsData == null) ? -1 : goodsData.Id);
					}
				}
			};
			this.soulCometStoneStrengthening.callback = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseMainWindow();
			};
			U3DUtils.AddChild(this.modules.gameObject, this.soulCometStoneStrengthening.gameObject, true);
		}
		this.soulCometStoneStrengthening.strengtheningType = strengtheningType;
		this.soulCometStoneStrengthening.dataChangeType = dataChangeType;
		this.soulCometStoneStrengthening.targetGoodsData = targetGoodsData;
	}

	private void HideSoulCometStoneStrengtheningView(bool hide = true)
	{
		if (null != this.soulCometStoneStrengthening)
		{
			this.soulCometStoneStrengthening.gameObject.SetActive(!hide);
		}
	}

	private void CloseSoulCometStoneStrengtheningView()
	{
		NGUITools.Destroy(this.soulCometStoneStrengthening.gameObject);
	}

	private void RefreshStrengtheningProgress(Dictionary<int, GoodsData> selectedGoodsDataDict)
	{
		if (null != this.soulCometStoneStrengthening)
		{
			this.soulCometStoneStrengthening.RefreshPropsAndExp(selectedGoodsDataDict);
		}
	}

	private void HideStoneEquipingView(bool hide = true)
	{
		if (null != this.stoneEquiping)
		{
			this.stoneEquiping.SetActive(!hide);
		}
	}

	private void Back()
	{
		if (null == this.soulCometStoneBag)
		{
			return;
		}
		switch (this.soulCometStoneBag.soulCometStoneBagTypes)
		{
		case SoulCometStoneBagTypes.Gathering:
		case SoulCometStoneBagTypes.Strengthening_Body:
		case SoulCometStoneBagTypes.Strengthening_Bag__FromBody:
			this.BackToHomePage();
			break;
		case SoulCometStoneBagTypes.Strengthening_Bag__FromGathering:
			this.BackToGatheringView();
			break;
		}
	}

	private void BackToHomePage()
	{
		this.ResetSoulCometStoneBag();
		this.ShowSoulCometStoneBagView(false);
		this.ShowSoulCometStoneGatheringView(false);
		this.HideSoulCometStoneStrengtheningView(true);
		this.HideStoneEquipingView(false);
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = 1500,
			Type = 5
		});
	}

	private void BackToGatheringView()
	{
		this.GatheringSoulCometStone();
	}

	private void CloseMainWindow()
	{
		PlayZone.GlobalPlayZone.CloseFluorescentDiamondWindow();
	}

	public void OnEquipedSoulCometStoneChanged(GoodsData gd, EquipedDiamondModifyType modType)
	{
		this.RefreshSoulCometStoneAtSlot(this.selectSlot, (modType != EquipedDiamondModifyType.EquipedDiamondModifyType_Replace) ? null : gd);
		this.SetSoulCometStoneProperties();
	}

	private void RefreshSoulCometStoneAtSlot(int slotID, GoodsData gd)
	{
		if (this.stones == null || this.stones.Count <= 0)
		{
			return;
		}
		int num = slotID % 100;
		if (num > this.stones.Count)
		{
			return;
		}
		this.ClearAtSlot(num);
		if (gd == null)
		{
			return;
		}
		GameObject parent = this.stones[num - 1];
		GGoodIcon ggoodIcon = this.AddIcon(gd);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
		U3DUtils.AddChild(parent, ggoodIcon.gameObject, true);
		ggoodIcon.transform.localPosition = new Vector3(0f, 0f, -1f);
	}

	private void RefreshSoulCometStoneAllSlots()
	{
		if (this.stones == null || this.stones.Count <= 0)
		{
			return;
		}
		this.ClearAllSlots();
		List<GoodsData> equipedSoulCometStoneList = Global.GetEquipedSoulCometStoneList();
		if (equipedSoulCometStoneList == null || equipedSoulCometStoneList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < equipedSoulCometStoneList.Count; i++)
		{
			GoodsData goodsData = equipedSoulCometStoneList[i];
			int num = goodsData.BagIndex / 100;
			if (num == this.circleID)
			{
				this.RefreshSoulCometStoneAtSlot(goodsData.BagIndex, goodsData);
			}
		}
	}

	private void ClearAllSlots()
	{
		if (this.stones == null || this.stones.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.stones.Count; i++)
		{
			GameObject gameObject = this.stones[i];
			if (null != gameObject)
			{
				int childCount = gameObject.transform.childCount;
				if (childCount > 0)
				{
					for (int j = 0; j < childCount; j++)
					{
						Transform child = gameObject.transform.GetChild(j);
						NGUITools.Destroy(child.gameObject);
					}
				}
			}
		}
	}

	private void ClearAtSlot(int slot)
	{
		if (slot < 0 || slot > 6)
		{
			return;
		}
		if (this.stones == null || this.stones.Count <= 0)
		{
			return;
		}
		GameObject gameObject = this.stones[slot - 1];
		if (null != gameObject)
		{
			int childCount = gameObject.transform.childCount;
			if (childCount <= 0)
			{
				return;
			}
			for (int i = 0; i < childCount; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				NGUITools.Destroy(child.gameObject);
			}
		}
	}

	public static List<GoodsData> GetAvailableSoulCometStoneByCircleID(int circleID)
	{
		SoulCometStone.<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF = new SoulCometStone.<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF();
		<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.circleID = circleID;
		if (<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.circleID <= 0 || <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.circleID > 3)
		{
			return null;
		}
		List<GoodsData> bagSoulCometStoneList = Global.GetBagSoulCometStoneList();
		if (bagSoulCometStoneList == null || bagSoulCometStoneList.Count <= 0)
		{
			return null;
		}
		<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList = new List<GoodsData>();
		for (int j = 0; j < bagSoulCometStoneList.Count; j++)
		{
			GoodsData goodsData = bagSoulCometStoneList[j];
			if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) != 910)
			{
				<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList.Add(goodsData);
			}
		}
		if (<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList == null || <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList.Count <= 0)
		{
			return null;
		}
		List<GoodsData> equipedSoulCometStoneList = Global.GetEquipedSoulCometStoneList();
		if (equipedSoulCometStoneList == null || equipedSoulCometStoneList.Count <= 0)
		{
			return <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList;
		}
		List<GoodsData> list = equipedSoulCometStoneList.FindAll((GoodsData gd) => gd.BagIndex / 100 == <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.circleID);
		if (list == null || list.Count <= 0)
		{
			return <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList;
		}
		List<GoodsData> list2 = new List<GoodsData>();
		int i;
		for (i = 0; i < <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList.Count; i++)
		{
			bool flag = list.Exists((GoodsData gd) => gd.GoodsID == <GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList[i].GoodsID);
			if (!flag)
			{
				list2.Add(<GetAvailableSoulCometStoneByCircleID>c__AnonStorey3BF.soulCometStoneStoreList[i]);
			}
		}
		return list2;
	}

	public static Dictionary<int, GoodsData> GetEquipedSoulCometStoneListByCircleID(int circleID)
	{
		if (circleID <= 0 || circleID > 3)
		{
			return null;
		}
		List<GoodsData> equipedSoulCometStoneList = Global.GetEquipedSoulCometStoneList();
		if (equipedSoulCometStoneList == null || equipedSoulCometStoneList.Count <= 0)
		{
			return null;
		}
		List<GoodsData> list = equipedSoulCometStoneList.FindAll((GoodsData gd) => gd.BagIndex / 100 == circleID);
		if (list == null || list.Count <= 0)
		{
			return null;
		}
		Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
		for (int i = 0; i < list.Count; i++)
		{
			GoodsData goodsData = list[i];
			if (goodsData != null)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
				int categoriy = goodsXmlNodeByID.Categoriy;
				GoodsData goodsData2 = Global.CloneGoodsData(goodsData, false);
				goodsData2.Site = 0;
				if (dictionary.ContainsKey(categoriy))
				{
					GoodsData goodsData3 = dictionary[categoriy];
					GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData3.GoodsID);
					if (goodsXmlNodeByID.SuitID > goodsXmlNodeByID2.SuitID)
					{
						dictionary[categoriy] = goodsData2;
					}
				}
				else
				{
					dictionary.Add(categoriy, goodsData2);
				}
			}
		}
		return dictionary;
	}

	public void OnSoulCometStoneBagSorted(List<GoodsData> list_goods)
	{
		if (null != this.soulCometStoneBag)
		{
			this.soulCometStoneBag.SetBagSortResult(list_goods);
		}
		Global.ClearSoulCometStoneQueue();
	}

	public void OnSoulCometStoneBagChanged(GoodsData gd)
	{
		if (null == this.soulCometStoneBag)
		{
			return;
		}
		this.soulCometStoneBag.ReplaceGoodsIcon(gd);
	}

	public void SetSoulCometStoneGroupInfo(SoulStoneQueryGetData randGroupInfo)
	{
		if (null != this.soulCometStoneGathering)
		{
			this.soulCometStoneGathering.SetSoulCometStoneGroupInfo(randGroupInfo);
		}
		if (randGroupInfo != null && randGroupInfo.ExtFuncList != null && randGroupInfo.ExtFuncList.Count >= 1)
		{
			SoulStoneExtFuncItem soulStoneExtFuncItem = randGroupInfo.ExtFuncList[0];
			if (soulStoneExtFuncItem != null && soulStoneExtFuncItem.CostType > 0)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1500,
					Type = soulStoneExtFuncItem.CostType
				});
			}
		}
	}

	public void SetGatheringResult(SoulStoneGetData soulStoneData)
	{
		if (null != this.soulCometStoneGathering)
		{
			this.soulCometStoneGathering.SetGatheringResult(soulStoneData);
			if (soulStoneData != null && soulStoneData.Error != 0)
			{
				this.OnErrorOccurs(soulStoneData.Error);
			}
		}
	}

	public void SetStrengtheningResult(int result, int dbid, int site, int level, int exp)
	{
		if (result != 0)
		{
			this.OnErrorOccurs(result);
		}
		if (null != this.soulCometStoneStrengthening)
		{
			this.soulCometStoneStrengthening.SetStrengtheningResult(result, dbid, site, level, exp);
		}
		if (null != this.soulCometStoneBag && result == 0)
		{
			this.soulCometStoneBag.RemoveSelectIcon();
		}
	}

	public void SetEquipStoneResult(int result, int slotID, int dbid)
	{
		Super.HideNetWaiting();
		if (result != 0)
		{
			this.OnErrorOccurs(result);
		}
	}

	private void OnErrorOccurs(int errorCode)
	{
		string text = string.Empty;
		switch (errorCode)
		{
		case 1:
			text = Global.GetLang("服务器异常");
			break;
		case 2:
			text = Global.GetLang("参数错误");
			break;
		case 3:
			text = Global.GetLang("选中的额外功能未开启");
			break;
		case 4:
			text = Global.GetLang("服务器异常，请稍后再试");
			break;
		case 7:
			text = Global.GetLang("背包空间不足");
			break;
		case 8:
			text = Global.GetLang("已是最高级");
			break;
		case 9:
			text = Global.GetLang("选中的魂石不可装备");
			break;
		case 10:
			text = Global.GetLang("服务器异常，请稍后再试");
			break;
		case 11:
			text = Global.GetLang("功能未开启");
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, text, 0, -1, -1, 0);
		}
	}

	private void EquipSoulCometStoneRequst(int slotID, int dbid)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.EquipSoulCometStone(slotID, dbid);
	}

	private const int maxCircle = 3;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton gatheringBtn;

	public UIButton groupBtn;

	public TextBlock propertyLabel;

	public TextBlock descriptionLabel;

	public List<GameObject> stones;

	public TabbarItem[] circleItems;

	private int activeCount;

	private GoodsData selectGoodsData;

	private int circleID = 1;

	private int selectSlot;

	public GameObject stoneEquiping;

	public GameObject modules;

	private SoulCometStoneBag soulCometStoneBag;

	private SoulCometStoneStrengthening soulCometStoneStrengthening;

	private SoulCometStoneGathering soulCometStoneGathering;

	private Dictionary<int, LevelAttribute> dic_slot;

	private int selectedIndex;
}
