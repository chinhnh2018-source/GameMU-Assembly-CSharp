using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class SoulCometStoneStrengthening : UserControl
{
	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("升级");
		this.checkName[0].text = Global.GetLang("白色");
		this.checkName[1].text = Global.GetLang("绿色");
		this.checkName[2].text = Global.GetLang("蓝色");
		this.TextNextLevel.X = 180.0;
		this.TextCurrentLevel.X = -100.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitStrengtheningDataDict();
		this.InitAllValue();
		this.CheckBoxWhite.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = Convert.ToInt32(this.CheckBoxWhite.Check),
					IDType = 0,
					Data = this.currentGoodData
				});
			}
		};
		this.CheckBoxGreen.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = Convert.ToInt32(this.CheckBoxGreen.Check),
					IDType = 1,
					Data = this.currentGoodData
				});
			}
		};
		this.CheckBoxBlue.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = Convert.ToInt32(this.CheckBoxBlue.Check),
					IDType = 2,
					Data = this.currentGoodData
				});
			}
		};
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartStrengthening();
		};
	}

	protected override void OnDestroy()
	{
	}

	public SoulCometStoneStrengtheningType strengtheningType
	{
		get
		{
			return this._strengtheningType;
		}
		set
		{
			this._strengtheningType = value;
		}
	}

	public SoulCometStoneStrengtheningDataChangeType dataChangeType
	{
		get
		{
			return this._dataChangeType;
		}
		set
		{
			this._dataChangeType = value;
		}
	}

	public GoodsData targetGoodsData
	{
		set
		{
			this.currentGoodData = value;
			if (this.currentGoodData != null)
			{
				this.currentGoodData.Site = 0;
			}
			if (this.dataChangeType == SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_Reload)
			{
				this.AddEquip(value);
			}
		}
	}

	public void InitAllValue()
	{
		this.TextCurrentLevel.Text = string.Empty;
		this.TextNextLevel.Text = string.Empty;
		this.ClearCurrentLevelProperties();
		this.ClearNextLevelProperties();
		this.ProgressBar.Percent = 0.0;
		this.TextTotalExp.Text = string.Empty;
		this.CheckBoxWhite.isChecked = false;
		this.CheckBoxGreen.isChecked = false;
		this.CheckBoxBlue.isChecked = false;
		this.InitAnim();
	}

	private void InitAnim()
	{
		if (this.Anim == null)
		{
			return;
		}
		for (int i = 0; i < this.Anim.Length; i++)
		{
			this.Anim[i].gameObject.SetActive(false);
		}
	}

	private void SetArrows(int lightCount)
	{
		if (this.Arrows == null || lightCount < 0)
		{
			return;
		}
		int num = Math.Min(lightCount, 3) + 1;
		int i;
		for (i = 0; i < num; i++)
		{
			this.Arrows[i].gameObject.SetActive(true);
			this.Arrows[i].transform.localPosition = new Vector3(146f, this.Arrows[i].transform.localPosition.y, this.Arrows[i].transform.localPosition.z);
		}
		while (i <= 3)
		{
			this.Arrows[i].gameObject.SetActive(false);
			i++;
		}
	}

	private void HideArrows()
	{
		if (this.Arrows == null)
		{
			return;
		}
		for (int i = 0; i < this.Arrows.Length; i++)
		{
			this.Arrows[i].gameObject.SetActive(false);
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false);
			this.currentGoodData = gd;
			int num = 0;
			int soulCometStoneLevel = Global.GetSoulCometStoneLevel(gd, out num);
			this.TextCurrentLevel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"D6AA69",
				Global.GetLang("等       级: "),
				"FFFFFF",
				soulCometStoneLevel.ToString()
			});
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(gd.GoodsID);
			StrengtheningBaseLevelAttribute[] array = null;
			string[] props = null;
			this.propertyCount = this.GetBaseAttributeStr(gd, goodsEquipPropsDoubleList, out props, out array);
			this.baseAttributes = array;
			this.SetCurrentProperties(props, this.propertyCount);
			this.TextNextLevel.Text = soulCometStoneLevel.ToString();
			this.SetNextLevelProperties(1);
			this.RefreshPropsAndExp(null);
		}
	}

	private void SetCurrentProperties(string[] props, int propCount)
	{
		if (this.TextCurrentProps == null || this.TextCurrentProps.Length <= 0)
		{
			return;
		}
		if (props == null || propCount <= 0)
		{
			this.ClearCurrentLevelProperties();
			return;
		}
		int num = Math.Min(propCount, 3);
		int i;
		for (i = 0; i < num; i++)
		{
			this.TextCurrentProps[i].Text = props[i];
			this.TextCurrentProps[i].X = -100.0;
		}
		while (i < 3)
		{
			this.TextCurrentProps[i].Text = string.Empty;
			i++;
		}
	}

	private void SetNextLevelProperties(int toLevel = 1)
	{
		if (this.baseAttributes == null || this.propertyCount <= 0)
		{
			this.ClearNextLevelProperties();
			return;
		}
		int num = Math.Min(this.propertyCount, 3);
		int i;
		for (i = 0; i < num; i++)
		{
			StrengtheningBaseLevelAttribute strengtheningBaseLevelAttribute = this.baseAttributes[i];
			if (strengtheningBaseLevelAttribute != null)
			{
				double num2 = strengtheningBaseLevelAttribute.value * (double)toLevel;
				this.TextNextProps[i].Text = ((!strengtheningBaseLevelAttribute.isNumber) ? string.Format("{0}%", Math.Round(num2 * 100.0, 2)) : num2.ToString());
				this.TextNextProps[i].X = 180.0;
			}
		}
		while (i < 3)
		{
			this.TextNextProps[i].Text = string.Empty;
			i++;
		}
	}

	private void ClearCurrentLevelProperties()
	{
		if (this.TextCurrentProps == null || this.TextCurrentProps.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			this.TextCurrentProps[i].Text = string.Empty;
		}
	}

	private void ClearNextLevelProperties()
	{
		if (this.TextNextProps == null || this.TextNextProps.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			this.TextNextProps[i].Text = string.Empty;
		}
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false)
	{
		this.EquipIcon.Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.SoulCometStoneBagTip, GoodsOwnerTypes.SoulCometStoneBag, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			icon.TextColor = 15793920U;
			int num = 0;
			int soulCometStoneLevel = Global.GetSoulCometStoneLevel(gd, out num);
			icon.ContentText.Text = "Lv" + soulCometStoneLevel;
			int equipGoodsSuitID = Global.GetEquipGoodsSuitID(gd.GoodsID);
			if (equipGoodsSuitID == 1)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			if (equipGoodsSuitID == 2)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			if (equipGoodsSuitID == 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
			if (equipGoodsSuitID >= 4 && equipGoodsSuitID <= 10)
			{
				icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
				icon.TeXiao.gameObject.SetActive(true);
			}
			bool flag = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
			this.EquipIcon.Add(icon);
		}
	}

	private int GetBaseAttributeStr(GoodsData gd, double[] equipFields_1, out string[] propertiesStr, out StrengtheningBaseLevelAttribute[] propsValue)
	{
		propertiesStr = null;
		propsValue = null;
		string text = string.Empty;
		string text2 = string.Empty;
		propertiesStr = new string[3];
		propsValue = new StrengtheningBaseLevelAttribute[3];
		int num = 0;
		int num2 = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(gd, out num2);
		for (int i = 1; i <= 10; i += 2)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = ExtPropIndexes.ExtPropIndexChineseNames[i];
				double num3 = equipFields_1[i];
				if (i == 1)
				{
					if (equipFields_1[i] != 0.0)
					{
						text = Global.GetColorStringForNGUIText(new object[]
						{
							"D6AA69",
							string.Format("{0}: ", text2),
							"FFFFFF",
							string.Format("{0}%", (int)num3)
						});
						propertiesStr[num] = text;
						propsValue[num] = new StrengtheningBaseLevelAttribute(true, num3);
						num++;
						if (num >= 3)
						{
							return num;
						}
					}
				}
				else
				{
					int num4 = i;
					int num5 = i + 1;
					if (equipFields_1[num4] != 0.0 || equipFields_1[num5] != 0.0)
					{
						double num6 = equipFields_1[num4];
						double num7 = equipFields_1[num5];
						text = Global.GetColorStringForNGUIText(new object[]
						{
							"D6AA69",
							string.Format("{0}: ", text2),
							"FFFFFF",
							string.Format("{0}", (int)(num7 * (double)soulCometStoneLevel))
						});
						propertiesStr[num] = text;
						propsValue[num] = new StrengtheningBaseLevelAttribute(true, num3);
						num++;
						if (num >= 3)
						{
							return num;
						}
					}
				}
			}
		}
		for (int i = 11; i < 177; i++)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = ExtPropIndexes.ExtPropIndexChineseNames[i];
				double num8 = equipFields_1[i];
				if (ExtPropIndexes.ExtPropIndexPercents[i] == 1)
				{
					double num9 = num8 * (double)soulCometStoneLevel * 100.0;
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"D6AA69",
						string.Format("{0}: ", text2),
						"FFFFFF",
						string.Format("{0}%", Math.Round(num9, 2))
					});
					propertiesStr[num] = text;
					propsValue[num] = new StrengtheningBaseLevelAttribute(false, num8);
					num++;
					if (num >= 3)
					{
						return num;
					}
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[i] == 0)
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"D6AA69",
						string.Format("{0}: ", text2),
						"FFFFFF",
						string.Format("{0}", (int)(num8 * (double)soulCometStoneLevel))
					});
					propertiesStr[num] = text;
					propsValue[num] = new StrengtheningBaseLevelAttribute(true, num8);
					num++;
					if (num >= 3)
					{
						return num;
					}
				}
			}
		}
		return num;
	}

	public void RefreshPropsAndExp(Dictionary<int, GoodsData> selectedGoodsDataDict = null)
	{
		this.selectedGoodsDataDict = selectedGoodsDataDict;
		if (this.currentGoodData == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(this.currentGoodData, out num);
		if (selectedGoodsDataDict == null || selectedGoodsDataDict.Count <= 0)
		{
			num2 = this.GetExpByLevel(Global.GetEquipGoodsSuitID(this.currentGoodData.GoodsID), soulCometStoneLevel + 1, false);
			this.TextNextLevel.Text = string.Empty;
			this.ClearNextLevelProperties();
			this.TextTotalExp.Text = string.Empty;
			this.HideArrows();
			bool flag = soulCometStoneLevel >= this.GetMaxLevelByGoodsData(this.currentGoodData);
			this.ProgressBar.ProgessText = ((!flag) ? string.Format("{0}/{1}", num, num2) : "100%");
			this.ProgressBar.PercentByStopTween = ((!flag) ? ((double)num / (double)num2) : 1.0);
			this.LastLevel = (double)soulCometStoneLevel;
			return;
		}
		int num3 = this.GetSumExpByGoodsDatas(selectedGoodsDataDict);
		this.TextTotalExp.Text = string.Format(Global.GetLang("(经验 +{0})"), num3);
		num3 += num;
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(this.currentGoodData.GoodsID);
		if (equipGoodsSuitID == -1)
		{
			return;
		}
		int num4 = 0;
		num2 = 0;
		int upLevelByExp = this.GetUpLevelByExp(num3, equipGoodsSuitID, soulCometStoneLevel, out num4, out num2);
		this.TextNextLevel.Text = upLevelByExp.ToString();
		this.SetNextLevelProperties(upLevelByExp);
		this.SetArrows(this.propertyCount);
		if (num2 > 0)
		{
			this.ProgressBar.ProgessText = string.Format("{0}/{1}", num3 - num4, num2);
			double num5 = (double)(num3 - num4) / (double)num2;
			this.ProgressBar.TweenPercent(this.ProgressBar.Percent + this.LastLevel, num5 + (double)upLevelByExp, 1.0);
			this.LastLevel = (double)upLevelByExp;
		}
		else if (upLevelByExp >= this.GetMaxLevelByGoodsData(this.currentGoodData))
		{
			this.ProgressBar.ProgessText = "100%";
			this.ProgressBar.PercentByStopTween = 1.0;
			this.LastLevel = (double)upLevelByExp;
		}
	}

	private void InitStrengtheningDataDict()
	{
		if (this.stoneUpLevelDataDic == null)
		{
			this.stoneUpLevelDataDic = new Dictionary<int, Dictionary<int, StoneStrengtheningAttribute>>();
		}
		XElement gameResXml = Global.GetGameResXml("Config/HunShiExp.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HunShi");
		if (xelementList == null)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = Global.GetXElement(xelementList[i], "HunShi");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "SuitID");
			List<XElement> xelementList2 = Global.GetXElementList(xelement, "*");
			if (xelementList2 != null)
			{
				Dictionary<int, StoneStrengtheningAttribute> dictionary = new Dictionary<int, StoneStrengtheningAttribute>();
				int num = 0;
				for (int j = 0; j < xelementList2.Count; j++)
				{
					StoneStrengtheningAttribute stoneStrengtheningAttribute = new StoneStrengtheningAttribute();
					stoneStrengtheningAttribute.id = Global.GetXElementAttributeInt(xelementList2[j], "ID");
					stoneStrengtheningAttribute.needExp = Global.GetXElementAttributeInt(xelementList2[j], "Exp");
					num += stoneStrengtheningAttribute.needExp;
					stoneStrengtheningAttribute.totalExp = num;
					if (!dictionary.ContainsKey(stoneStrengtheningAttribute.id))
					{
						dictionary.Add(stoneStrengtheningAttribute.id, stoneStrengtheningAttribute);
					}
				}
				if (!this.stoneUpLevelDataDic.ContainsKey(xelementAttributeInt))
				{
					this.stoneUpLevelDataDic.Add(xelementAttributeInt, dictionary);
				}
			}
		}
	}

	private int GetSumExpByGoodsDatas(Dictionary<int, GoodsData> dict)
	{
		if (dict == null || dict.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		foreach (GoodsData gd in dict.Values)
		{
			num += this.GetGoodsDataExp(gd);
		}
		return num;
	}

	private int GetGoodsDataExp(GoodsData gd)
	{
		if (this.stoneUpLevelDataDic == null)
		{
			return 0;
		}
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(gd.GoodsID);
		if (equipGoodsSuitID == -1)
		{
			return 0;
		}
		if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 910)
		{
			return this.GetSpecialGoodsDataExp(gd.GoodsID);
		}
		int num = 0;
		int num2 = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(gd, out num2);
		num += num2;
		Dictionary<int, StoneStrengtheningAttribute> dictionary = null;
		if (this.stoneUpLevelDataDic.TryGetValue(equipGoodsSuitID, ref dictionary))
		{
			StoneStrengtheningAttribute stoneStrengtheningAttribute = null;
			if (dictionary.TryGetValue(soulCometStoneLevel, ref stoneStrengtheningAttribute))
			{
				num += stoneStrengtheningAttribute.totalExp;
			}
		}
		return num;
	}

	private int GetSpecialGoodsDataExp(int goodsID)
	{
		if (this.SpecialGoodsDataExpDict == null)
		{
			this.SpecialGoodsDataExpDict = new Dictionary<int, int>();
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("HunShiExp", '|');
			if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length <= 0)
			{
				return 0;
			}
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				if (array == null || array.Length < 2)
				{
					return 0;
				}
				int num = array[0].SafeToInt32(0);
				int num2 = array[1].SafeToInt32(0);
				if (!this.SpecialGoodsDataExpDict.ContainsKey(num))
				{
					this.SpecialGoodsDataExpDict.Add(num, num2);
				}
			}
		}
		int result = 0;
		if (this.SpecialGoodsDataExpDict.TryGetValue(goodsID, ref result))
		{
			return result;
		}
		return result;
	}

	private int GetUpLevelByExp(int totalExp, int suitID, int level, out int useExp, out int nextExp)
	{
		useExp = 0;
		nextExp = 0;
		int result = level;
		if (totalExp == 0)
		{
			return result;
		}
		Dictionary<int, StoneStrengtheningAttribute> dictionary = null;
		if (this.stoneUpLevelDataDic.TryGetValue(suitID, ref dictionary))
		{
			int num = 0;
			for (int i = level; i <= dictionary.Count; i++)
			{
				StoneStrengtheningAttribute stoneStrengtheningAttribute = null;
				if (dictionary.TryGetValue(i, ref stoneStrengtheningAttribute))
				{
					if (stoneStrengtheningAttribute.id == level)
					{
						num = stoneStrengtheningAttribute.totalExp;
					}
					if (totalExp < stoneStrengtheningAttribute.totalExp - num)
					{
						nextExp = stoneStrengtheningAttribute.needExp;
						return result;
					}
					useExp = stoneStrengtheningAttribute.totalExp - num;
					result = stoneStrengtheningAttribute.id;
				}
			}
		}
		return result;
	}

	private int GetExpByLevel(int suitID, int level, bool isTotal = false)
	{
		if (this.stoneUpLevelDataDic == null)
		{
			return 0;
		}
		suitID = Math.Min(this.stoneUpLevelDataDic.Count, suitID);
		int result = 0;
		Dictionary<int, StoneStrengtheningAttribute> dictionary = null;
		if (this.stoneUpLevelDataDic.TryGetValue(suitID, ref dictionary))
		{
			level = Math.Min(dictionary.Count, level);
			StoneStrengtheningAttribute stoneStrengtheningAttribute = null;
			if (dictionary.TryGetValue(level, ref stoneStrengtheningAttribute))
			{
				if (isTotal)
				{
					result = stoneStrengtheningAttribute.totalExp;
				}
				else
				{
					result = stoneStrengtheningAttribute.needExp;
				}
			}
		}
		return result;
	}

	private int GetMaxLevelByGoodsData(GoodsData gd)
	{
		if (this.stoneUpLevelDataDic == null || this.stoneUpLevelDataDic.Count <= 0)
		{
			return 0;
		}
		int result = 0;
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(gd.GoodsID);
		Dictionary<int, StoneStrengtheningAttribute> dictionary = null;
		if (this.stoneUpLevelDataDic.TryGetValue(equipGoodsSuitID, ref dictionary))
		{
			result = dictionary.Count;
		}
		return result;
	}

	private void StartStrengthening()
	{
		if (this.currentGoodData == null)
		{
			return;
		}
		if (this.stoneUpLevelDataDic == null)
		{
			return;
		}
		int num = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(this.currentGoodData, out num);
		if (soulCometStoneLevel >= this.GetMaxLevelByGoodsData(this.currentGoodData))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经达到最大级别"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Count <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未选择被吞噬的魂石"), new object[0]), 0, -1, -1, 0);
			return;
		}
		MessBoxIsHintTypes messBoxIsHintTypes = MessBoxIsHintTypes.None;
		bool flag = false;
		string[] array = new string[]
		{
			Global.GetLang("被吞噬的魂石有品质高于升级魂石，确认要升级吗？"),
			Global.GetLang("本次升级经验会溢出，确认要升级吗？")
		};
		int num2 = 0;
		if (this.IsSwallowHigherLevelStone() && Super.MessageBoxIsHint[9] == 0)
		{
			flag = true;
			messBoxIsHintTypes = MessBoxIsHintTypes.SoulCometStoneSwallowHigherLevelEventHint;
			num2 = 0;
		}
		else if (this.IsExperienceOverflow() && Super.MessageBoxIsHint[10] == 0)
		{
			flag = true;
			messBoxIsHintTypes = MessBoxIsHintTypes.SoulCometStoneUplevelOverflowEventHint;
			num2 = 1;
		}
		if (!flag)
		{
			this.StrengtheningRequest();
		}
		else
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), array[num2], 2, null, messBoxIsHintTypes);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.StrengtheningRequest();
				}
				return true;
			};
		}
	}

	private void StrengtheningRequest()
	{
		int id = this.currentGoodData.Id;
		int site = this.currentGoodData.Site;
		string text = string.Empty;
		foreach (GoodsData goodsData in this.selectedGoodsDataDict.Values)
		{
			text += string.Format("{0},", goodsData.Id);
		}
		this.ShowModalDialog();
		Super.PlayAnim(this.Anim[0]);
		GameInstance.Game.UplevelSoulCometStone(id, (this.strengtheningType != SoulCometStoneStrengtheningType.StrengtheningType_Bag) ? 8001 : 8000, text);
	}

	public void SetStrengtheningResult(int result, int dbid, int site, int level, int exp)
	{
		this.CloseModalDialog();
		if (result == 0)
		{
			if (this.currentGoodData != null)
			{
				this.currentGoodData.Id = dbid;
				Global.SetGoodsDataYuansuProps(this.currentGoodData, level, exp);
				GoodsData goodsData = Global.CloneGoodsData(this.currentGoodData, false);
				goodsData.Id = dbid;
				goodsData.Site = site;
				this.AddEquip(this.currentGoodData);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 101,
					Data = goodsData,
					Type = (int)this.strengtheningType
				});
			}
			Super.PlayAnim(this.Anim[1]);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升级成功"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	private bool IsSwallowHigherLevelStone()
	{
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Count <= 0 || this.currentGoodData == null)
		{
			return false;
		}
		foreach (GoodsData goodsData in this.selectedGoodsDataDict.Values)
		{
			if (goodsData != null)
			{
				int equipGoodsSuitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
				int equipGoodsSuitID2 = Global.GetEquipGoodsSuitID(this.currentGoodData.GoodsID);
				if (equipGoodsSuitID > equipGoodsSuitID2)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsExperienceOverflow()
	{
		if (this.currentGoodData == null)
		{
			return false;
		}
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(this.currentGoodData.GoodsID);
		if (equipGoodsSuitID == -1)
		{
			return false;
		}
		int num = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(this.currentGoodData, out num);
		int maxLevelByGoodsData = this.GetMaxLevelByGoodsData(this.currentGoodData);
		int sumExpByGoodsDatas = this.GetSumExpByGoodsDatas(this.selectedGoodsDataDict);
		int num2 = 0;
		int num3 = 0;
		int upLevelByExp = this.GetUpLevelByExp(sumExpByGoodsDatas, equipGoodsSuitID, soulCometStoneLevel, out num2, out num3);
		return num3 <= 0 || (upLevelByExp == maxLevelByGoodsData && sumExpByGoodsDatas > num2);
	}

	private const int maxProperty = 3;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public SpriteSL EquipIcon;

	public GameObject[] Anim;

	public TextBlock TextCurrentLevel;

	public TextBlock TextNextLevel;

	public TextBlock[] TextCurrentProps;

	public TextBlock[] TextNextProps;

	public GImgProgressBar ProgressBar;

	public TextBlock TextTotalExp;

	public GCheckBox CheckBoxWhite;

	public GCheckBox CheckBoxGreen;

	public GCheckBox CheckBoxBlue;

	public GButton SubmitBtn;

	public GameObject[] Arrows;

	public TextBlock[] checkName;

	private GoodsData currentGoodData;

	private StrengtheningBaseLevelAttribute[] baseAttributes;

	private int propertyCount;

	private Dictionary<int, GoodsData> selectedGoodsDataDict;

	private SoulCometStoneStrengtheningType _strengtheningType;

	private SoulCometStoneStrengtheningDataChangeType _dataChangeType;

	private double LastLevel;

	private Dictionary<int, Dictionary<int, StoneStrengtheningAttribute>> stoneUpLevelDataDic;

	private Dictionary<int, int> SpecialGoodsDataExpDict;

	public enum AnimTypes
	{
		None = -1,
		Submit,
		NotifyOk
	}
}
