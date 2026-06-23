using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class DiamondInlay : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.dic_diamond = Global.GetDiamondsConfig();
		this.InitEquipAttribuesWindow();
		this.InitInlayItemList();
		this.RefreshInlayItemList();
		this.RefreshTipsIcons();
		this.SetDiamondProperties();
		this.GetGoodsIcon();
	}

	private void RefreshInlayItemList()
	{
		if (this.list_inlayItem == null || this.list_inlayItem.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.list_inlayItem.Count; i++)
		{
			this.RefreshInlayItemAtIndex(i);
		}
	}

	private void RefreshInlayItemAtIndex(int index)
	{
		if (this.list_inlayItem == null || this.list_inlayItem.Count <= 0)
		{
			return;
		}
		if (index < 0 || index >= this.list_inlayItem.Count)
		{
			return;
		}
		DiamondInlayItem inlayItem = this.list_inlayItem[index];
		Dictionary<int, GoodsData> dictionary = Global.GetEquipedDiamondsBySlotID(index + 1);
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, GoodsData>();
		}
		for (int i = 1; i <= 3; i++)
		{
			GoodsData value = dictionary.GetValue(i);
			this.RefreshDiamondItem(inlayItem, i, (value == null) ? -1 : value.GoodsID);
		}
	}

	private void RefreshDiamondItemWithShape(int index, int shapeID)
	{
		if (this.list_inlayItem == null || this.list_inlayItem.Count <= 0)
		{
			return;
		}
		if (index < 1 || index > this.list_inlayItem.Count)
		{
			return;
		}
		DiamondInlayItem inlayItem = this.list_inlayItem[index - 1];
		Dictionary<int, GoodsData> dictionary = Global.GetEquipedDiamondsBySlotID(index);
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, GoodsData>();
		}
		GoodsData value = dictionary.GetValue(shapeID);
		this.RefreshDiamondItem(inlayItem, shapeID, (value == null) ? -1 : value.GoodsID);
	}

	private void RefreshDiamondItem(DiamondInlayItem inlayItem, int shapeID, int goodsID)
	{
		if (null == inlayItem)
		{
			return;
		}
		inlayItem.SetDiamondIconByType(shapeID, goodsID);
	}

	private void RefreshDiamondItemWithGoods(int goodsID)
	{
		if (this.list_inlayItem == null || this.list_inlayItem.Count <= 0)
		{
			return;
		}
		if (this.selectSlot < 1 || this.selectSlot > this.list_inlayItem.Count)
		{
			return;
		}
		DiamondInlayItem diamondInlayItem = this.list_inlayItem[this.selectSlot - 1];
		if (null != diamondInlayItem)
		{
			this.RefreshDiamondItem(diamondInlayItem, this.selectShapeID, goodsID);
		}
	}

	private void InitInlayItemList()
	{
		if (this.list_inlayItem == null || this.list_inlayItem.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.list_inlayItem.Count; i++)
		{
			DiamondInlayItem diamondInlayItem = this.list_inlayItem[i];
			diamondInlayItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				int id = e.ID;
				int shapeID = (int)e.Data;
				this.OnInlayItemComponentSelected(id, shapeID);
			};
		}
	}

	private void OnInlayItemComponentSelected(int index, int shapeID)
	{
		if (index < 1 || index > 10)
		{
			return;
		}
		this.selectSlot = index;
		if (shapeID == -1)
		{
			this.HideEquipAttributesWindow(false);
			this.SetEquipAttributes(index);
			return;
		}
		if (shapeID < 1 || shapeID > 3)
		{
			return;
		}
		this.selectShapeID = shapeID;
		Dictionary<int, GoodsData> equipedDiamondsBySlotID = Global.GetEquipedDiamondsBySlotID(index);
		GoodsData goodsData = null;
		bool flag = Global.IsAvailableDiamondInBag(shapeID);
		if (equipedDiamondsBySlotID != null)
		{
			goodsData = equipedDiamondsBySlotID.GetValue(shapeID);
		}
		if (!flag && (equipedDiamondsBySlotID == null || goodsData == null))
		{
			Super.HintMainText(Global.GetLang("无可用宝石，宝石可通过挖掘获得"), 10, 3);
			return;
		}
		if (goodsData != null)
		{
			this.selectGoodsData = goodsData;
			string text = UIHelper.FormatGoodsName(this.selectGoodsData, false, false, false);
			GTipServiceEx.ShowTip(this.goodsIconInstance, TipTypes.FluorescentDiamondBagTip, GoodsOwnerTypes.FluorescentDiamondBag, goodsData);
		}
		else if (flag)
		{
			this.EquipDiamondByShape(shapeID);
		}
	}

	private void GetGoodsIcon()
	{
		if (null == this.goodsIconInstance)
		{
			this.goodsIconInstance = U3DUtils.NEW<GGoodIcon>();
			U3DUtils.AddChild(base.gameObject, this.goodsIconInstance.gameObject, false);
			this.goodsIconInstance.transform.localPosition = new Vector3(-21500f, -21500f, 0f);
			this.goodsIconInstance.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 3)
				{
					this.UplevelDiamond();
				}
				else if (ev.IDType == 2)
				{
					this.UnloadDiamondAtSlot(this.selectSlot, this.selectShapeID, 0);
				}
			};
		}
	}

	public void RefreshTipsIcons()
	{
		if (this.list_inlayItem == null || this.list_inlayItem.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.list_inlayItem.Count; i++)
		{
			DiamondInlayItem diamondInlayItem = this.list_inlayItem[i];
			diamondInlayItem.isDiamondUpgradable = Global.IsUpgradableDiamondInEquipPart(i + 1);
		}
	}

	private void SetDiamondProperties()
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
			this.propertyLabel.Text = DiamondInlay.GetBaseAttributeStrFromPropertyList(totalBaseProperties, 3);
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
		List<int> equipedDiamondGoodsIDList = this.GetEquipedDiamondGoodsIDList();
		if (equipedDiamondGoodsIDList == null)
		{
			return null;
		}
		double[] array = new double[177];
		for (int i = 0; i < equipedDiamondGoodsIDList.Count; i++)
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(equipedDiamondGoodsIDList[i]);
			for (int j = 0; j < array.Length; j++)
			{
				array[j] += goodsEquipPropsDoubleList[j];
			}
		}
		return array;
	}

	private List<int> GetEquipedDiamondGoodsIDList()
	{
		Dictionary<int, Dictionary<int, GoodsData>> equipedDiamondList = Global.GetEquipedDiamondList();
		if (equipedDiamondList == null || equipedDiamondList.Count <= 0)
		{
			return null;
		}
		List<int> list = new List<int>();
		foreach (Dictionary<int, GoodsData> dictionary in equipedDiamondList.Values)
		{
			if (dictionary != null)
			{
				foreach (GoodsData goodsData in dictionary.Values)
				{
					if (goodsData != null)
					{
						list.Add(goodsData.GoodsID);
					}
				}
			}
		}
		return list;
	}

	public static string GetBaseAttributeStrFromPropertyList(double[] equipFields, int fillCount = 0)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = "e3b36c";
		string lang = Global.GetLang("            元素伤害");
		string lang2 = Global.GetLang("            元素抵抗");
		string lang3 = Global.GetLang("            基础属性");
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
				text3 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
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
		for (int j = 81; j <= 86; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
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
		for (int j = 1; j <= 10; j += 2)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num4 = equipFields[j];
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
					int num5 = j;
					int num6 = j + 1;
					if (equipFields[num5] != 0.0 || equipFields[num6] != 0.0)
					{
						double num7 = equipFields[num5];
						double num8 = equipFields[num6];
						text2 += string.Format("{0}+{1}", text3, (int)num8);
					}
				}
				text2 += "\n";
			}
		}
		for (int j = 11; j <= 68; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					text4,
					text3 + ": " + text6
				});
				double num9 = equipFields[j];
				if (ExtPropIndexes.ExtPropIndexPercents[j] == 1)
				{
					double num10 = num9 * 100.0;
					text2 += string.Format("{0}+{1}%", text3, Math.Round(num10, 2));
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[j] == 0)
				{
					text2 += string.Format("{0}+{1}", text3, (int)num9);
				}
				text2 += "\n";
			}
		}
		for (int j = 87; j < 177; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text3 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
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
		return DiamondInlay.ProcessStr(text);
	}

	public static string ProcessStr(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private void InitEquipAttribuesWindow()
	{
		if (null != this.equipAttributes)
		{
			this.equipAttributes.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.HideEquipAttributesWindow(true);
				}
				else if (e.ID == 101)
				{
					int slotID = (int)e.Data;
					this.HideEquipAttributesWindow(true);
					this.UnloadDiamondAtSlot(slotID, -1, 1);
				}
				else
				{
					int id = e.ID;
					int shapeID = (int)e.Data;
					this.OnInlayItemComponentSelected(id, shapeID);
				}
			};
		}
	}

	private void HideEquipAttributesWindow(bool hide = true)
	{
		if (null == this.equipAttributes)
		{
			return;
		}
		this.equipAttributes.gameObject.SetActive(!hide);
	}

	private bool IsEquipAttributesWindowActive(int slotID)
	{
		return !(null == this.equipAttributes) && (this.equipAttributes.gameObject.activeSelf && slotID == this.equipAttributes.slotID);
	}

	private void SetEquipAttributes(int slotID)
	{
		if (null == this.equipAttributes)
		{
			return;
		}
		Dictionary<int, GoodsData> equipedDiamondsBySlotID = Global.GetEquipedDiamondsBySlotID(slotID);
		this.equipAttributes.slotID = slotID;
		this.equipAttributes.dic_equipedDiamond = equipedDiamondsBySlotID;
	}

	private void EquipDiamondByShape(int shapeID)
	{
		if (shapeID < 1 || shapeID > 3)
		{
			return;
		}
		List<GoodsData> availableDiamondByShape = Global.GetAvailableDiamondByShape(shapeID);
		if (availableDiamondByShape != null)
		{
			availableDiamondByShape.Sort(new Comparison<GoodsData>(this.SortGoodsDataList));
		}
		DPSelectedItemEventHandler equipGoodsCallBack = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null)
			{
				GoodsData goodsData = args.Data as GoodsData;
				this.selectGoodsData = goodsData;
				string text = UIHelper.FormatGoodsName(this.selectGoodsData, false, false, false);
				this.InlayDiamondRequst(this.selectSlot, this.selectShapeID, goodsData.BagIndex);
			}
		};
		PlayZone.GlobalPlayZone.ShowAvailableGoodsWindow(availableDiamondByShape, GoodsType.GoodsType_FluorescentDiamond, equipGoodsCallBack);
	}

	private void UnloadDiamondAtSlot(int slotID, int shapeType, int operationType = 0)
	{
		if (slotID < 1 || slotID > 10)
		{
			return;
		}
		this.UnloadDiamondRequest(slotID, shapeType, operationType);
	}

	private void UplevelDiamond()
	{
		if (this.selectGoodsData == null)
		{
			return;
		}
		DPSelectedItemEventHandler uplevelCallBack = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null && args.ID == 101)
			{
				Dictionary<int, GoodsData> equipedDiamondsBySlotID = Global.GetEquipedDiamondsBySlotID(this.selectSlot);
				if (equipedDiamondsBySlotID != null)
				{
					this.selectGoodsData = equipedDiamondsBySlotID.GetValue(this.selectShapeID);
				}
				else
				{
					this.selectGoodsData = null;
				}
				string text = UIHelper.FormatGoodsName(this.selectGoodsData, false, false, false);
			}
		};
		GoodsData goodsData = Global.CloneGoodsData(this.selectGoodsData, false);
		goodsData.Id = this.GenerateSpecailPartID();
		PlayZone.GlobalPlayZone.ShowDiamondUplevelWindow(goodsData, 1, uplevelCallBack);
	}

	private int GenerateSpecailPartID()
	{
		return this.selectShapeID * 100 + this.selectSlot;
	}

	private int SortGoodsDataList(GoodsData x, GoodsData y)
	{
		int goodsPrice = this.GetGoodsPrice(x.GoodsID);
		int goodsPrice2 = this.GetGoodsPrice(y.GoodsID);
		if (goodsPrice2 == goodsPrice)
		{
			return y.GCount - x.GCount;
		}
		return goodsPrice - goodsPrice2;
	}

	private int GetGoodsPrice(int goodsID)
	{
		if (goodsID <= 0)
		{
			return 0;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			return goodsXmlNodeByID.PriceTwo;
		}
		return 0;
	}

	public void OnEquipedDiamondChanged(int slotID, int shapeID, GoodsData gd, EquipedDiamondModifyType type)
	{
		Global.OnEquipedDiamondDataChanged(slotID, shapeID, gd, type);
		this.RefreshDiamondItemWithShape(slotID, shapeID);
		this.SetDiamondProperties();
		if (this.IsEquipAttributesWindowActive(slotID))
		{
			this.SetEquipAttributes(slotID);
		}
	}

	public void SetDiamondInlayResult(int status)
	{
		Super.HideNetWaiting();
		string textMsg = string.Empty;
		switch (status + 2)
		{
		case 0:
			textMsg = Global.GetLang("功能未开启");
			break;
		case 1:
			textMsg = Global.GetLang("异常");
			break;
		case 3:
			textMsg = Global.GetLang("物品不存在");
			break;
		case 4:
			textMsg = Global.GetLang("部位索引错误");
			break;
		case 5:
			textMsg = Global.GetLang("宝石类型错误");
			break;
		case 6:
			textMsg = Global.GetLang("装备失败");
			break;
		case 7:
			textMsg = Global.GetLang("扣除物品失败");
			break;
		case 8:
			textMsg = Global.GetLang("卸下失败");
			break;
		case 9:
			textMsg = Global.GetLang("不是荧光宝石");
			break;
		case 10:
			textMsg = Global.GetLang("宝石数据错误");
			break;
		}
		if (status != 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	public void SetDiamondUnloadResult(int status)
	{
		Super.HideNetWaiting();
		string textMsg = string.Empty;
		switch (status + 2)
		{
		case 0:
			textMsg = Global.GetLang("功能未开启");
			break;
		case 1:
			textMsg = Global.GetLang("异常");
			break;
		case 3:
			textMsg = Global.GetLang("物品不存在");
			break;
		case 4:
			textMsg = Global.GetLang("部位索引错误");
			break;
		case 5:
			textMsg = Global.GetLang("宝石类型错误");
			break;
		case 6:
			textMsg = Global.GetLang("卸下失败");
			break;
		case 7:
			textMsg = Global.GetLang("背包空间不足1格");
			break;
		case 8:
			textMsg = Global.GetLang("背包空间不足3格");
			break;
		}
		if (status != 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	private void InlayDiamondRequst(int slotID, int shapeID, int bagIndex)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.InlayDiamond(slotID, shapeID, bagIndex);
	}

	private void UnloadDiamondRequest(int slotID, int shapeID, int operationType)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.UnloadDiamond(slotID, operationType, shapeID);
	}

	public EquipDiamondAttributes equipAttributes;

	public List<DiamondInlayItem> list_inlayItem;

	public TextBlock propertyLabel;

	private Dictionary<int, DiamondAttribute> dic_diamond;

	private GGoodIcon goodsIconInstance;

	private GoodsData selectGoodsData;

	private int selectSlot;

	private int selectShapeID;
}
