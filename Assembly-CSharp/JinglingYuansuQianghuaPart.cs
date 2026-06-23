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

public class JinglingYuansuQianghuaPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("升级");
		this.checkName[0].text = Global.GetLang("白色");
		this.checkName[1].text = Global.GetLang("绿色");
		this.checkName[2].text = Global.GetLang("蓝色");
		for (int i = 0; i < this.TextNextProp.Length; i++)
		{
			this.TextCurrentProp[i].Text = string.Empty;
			this.TextNextProp[i].Text = string.Empty;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitYuansuUpDataDict();
		this.InitAllValue();
		this.CheckBoxWhite.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = Convert.ToInt32(this.CheckBoxWhite.Check),
					IDType = 0
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
					IDType = 1
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
					IDType = 2
				});
			}
		};
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartQianghua();
		};
	}

	protected override void OnDestroy()
	{
	}

	public int EquipDbID
	{
		get
		{
			this._EquipDbID = -1;
			if (this.EquipGoodsData != null)
			{
				this._EquipDbID = this.EquipGoodsData.Id;
			}
			return this._EquipDbID;
		}
	}

	public void InitAllValue()
	{
		this.TextCurrentLevel.Text = string.Empty;
		this.TextNextLevel.Text = string.Empty;
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

	private void SetTextLineShowNum(int shownum)
	{
		if (this.Arrows == null)
		{
			return;
		}
		if (shownum < 3)
		{
			this.TextCurrentProp[1].gameObject.SetActive(false);
			this.TextNextProp[1].gameObject.SetActive(false);
		}
		else
		{
			this.TextCurrentProp[1].gameObject.SetActive(true);
			this.TextNextProp[1].gameObject.SetActive(true);
		}
		for (int i = 0; i < this.Arrows.Length; i++)
		{
			if (i < shownum)
			{
				this.Arrows[i].gameObject.SetActive(true);
			}
			else
			{
				this.Arrows[i].gameObject.SetActive(false);
			}
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false, 28);
			this.EquipGoodsData = gd;
			this.TextCurrentLevel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"D6AA69",
				Global.GetLang("等       级: "),
				"FFFFFF",
				Global.GetYuansuGoodsDataLevel(gd).ToString()
			});
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(gd.GoodsID);
			this.GetBaseAttributeStr(gd, goodsEquipPropsDoubleList);
			this.TextNextLevel.Text = Global.GetYuansuGoodsDataLevel(gd).ToString();
			if (this.EquipPropsValue[1] > 0)
			{
				this.SetTextLineShowNum(3);
			}
			else
			{
				this.SetTextLineShowNum(2);
			}
			this.RefreshPropsAndExp2();
		}
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 28)
	{
		this.EquipIcon.Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.TipType = 12;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.YuansuBagTip, GoodsOwnerTypes.None, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool flag = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitYuansuGoodsGIcon(icon, gd);
			this.EquipIcon.Add(icon);
		}
	}

	private void GetBaseAttributeStr(GoodsData gd, double[] equipFields_1)
	{
		int num = 0;
		this.EquipPropsValue[0] = 0;
		this.EquipPropsValue[1] = 0;
		string text = string.Empty;
		int yuansuGoodsDataLevel = Global.GetYuansuGoodsDataLevel(gd);
		for (int i = 1; i <= 10; i += 2)
		{
			if (equipFields_1[i] != 0.0)
			{
				text = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num2 = equipFields_1[i];
				if (i == 1)
				{
					if (equipFields_1[i] != 0.0)
					{
						string text2 = Global.GetColorStringForNGUIText(new object[]
						{
							"D6AA69",
							string.Format("{0}: ", text),
							"FFFFFF",
							string.Format("{0}%", (int)equipFields_1[i])
						});
						text2 += "\n";
						this.TextCurrentProp[num].Text = text2;
						this.EquipPropsValue[num] = (int)equipFields_1[i];
						this.TextNextProp[num].Text = this.EquipPropsValue[num].ToString();
						num++;
					}
				}
				else
				{
					int num3 = i;
					int num4 = i + 1;
					if (equipFields_1[num3] != 0.0 || equipFields_1[num4] != 0.0)
					{
						double num5 = equipFields_1[num3];
						double num6 = equipFields_1[num4];
						string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
						{
							"D6AA69",
							string.Format("{0}: ", text),
							"FFFFFF",
							string.Format("{0}", (int)(num6 * (double)yuansuGoodsDataLevel))
						});
						this.TextCurrentProp[num].Text = colorStringForNGUIText;
						this.EquipPropsValue[num] = (int)equipFields_1[i];
						this.TextNextProp[num].Text = this.EquipPropsValue[num].ToString();
						num++;
					}
				}
			}
		}
		for (int i = 11; i < 177; i++)
		{
			if (equipFields_1[i] != 0.0)
			{
				text = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num7 = equipFields_1[i];
				if (ExtPropIndexes.ExtPropIndexPercents[i] != 1)
				{
					if (ExtPropIndexes.ExtPropIndexPercents[i] == 0)
					{
						string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
						{
							"D6AA69",
							string.Format("{0}: ", text),
							"FFFFFF",
							string.Format("{0}", (int)(num7 * (double)yuansuGoodsDataLevel))
						});
						this.TextCurrentProp[num].Text = colorStringForNGUIText2;
						this.EquipPropsValue[num] = (int)equipFields_1[i];
						this.TextNextProp[num].Text = this.EquipPropsValue[num].ToString();
						num++;
					}
				}
			}
		}
	}

	private XElement GetYuansuUpXmlNode(int suitID)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ElementsHeart.xml");
		if (gameResXml == null)
		{
			return null;
		}
		XElement xelement = Global.GetXElement(gameResXml, "XingZuo", "ID", suitID.ToString());
		if (xelement == null)
		{
			return null;
		}
		return xelement;
	}

	private int GetSumExpByGoodsDatas(Dictionary<int, GoodsData> dict)
	{
		int result = 0;
		if (dict == null || dict.Count <= 0)
		{
			return result;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (GoodsData goodsData in dict.Values)
		{
			num2 = 0;
			num3 = 0;
			Global.GetYuansuGoodsDataLevelAndExp(goodsData, out num2, out num3);
			num += num3;
		}
		return num;
	}

	private int GetUpLevelByExpAndXmlNode(int totalExp, XElement xmlNode, int level)
	{
		int num = 0;
		if (totalExp == 0 || xmlNode == null)
		{
			return num;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xmlNode, "XingWei"), "*");
		for (int i = level; i < xelementList.Count; i++)
		{
			num = i;
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NeedExp");
			totalExp -= xelementAttributeInt;
			if (totalExp < 0)
			{
				return num;
			}
		}
		if (totalExp > 0 && num >= xelementList.Count - 1)
		{
			XElement xelement2 = xelementList[num];
			num = Global.GetXElementAttributeInt(xelement2, "ID");
		}
		return num;
	}

	public void RefreshPropsAndExp2()
	{
		if (JinglingYuansuPart.SelectedGoodsDataDict == null)
		{
			return;
		}
		if (this.EquipGoodsData == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		Global.GetYuansuGoodsDataLevelAndExp(this.EquipGoodsData, out num, out num2);
		if (JinglingYuansuPart.SelectedGoodsDataDict.Count <= 0)
		{
			num3 = this.GetExpByLevel(Global.GetEquipGoodsSuitID(this.EquipGoodsData.GoodsID), num + 1, false);
			this.TextNextLevel.Text = string.Empty;
			this.TextTotalExp.Text = string.Empty;
			this.SetTextLineShowNum(0);
			for (int i = 0; i < this.TextNextProp.Length; i++)
			{
				this.TextNextProp[i].Text = string.Empty;
			}
			this.ProgressBar.ProgessText = string.Format("{0}/{1}", num2, num3);
			this.ProgressBar.PercentByStopTween = (double)num2 / (double)num3;
			this.LastLevel = (double)num;
			return;
		}
		if (this.EquipPropsValue[1] > 0)
		{
			this.SetTextLineShowNum(3);
		}
		else
		{
			this.SetTextLineShowNum(2);
		}
		num = Global.GetYuansuGoodsDataLevel(this.EquipGoodsData);
		int num4 = this.GetSumExpByGoodsDatas2(JinglingYuansuPart.SelectedGoodsDataDict);
		this.TextTotalExp.Text = string.Format(Global.GetLang("(经验 +{0})"), num4);
		num4 += num2;
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(this.EquipGoodsData.GoodsID);
		if (equipGoodsSuitID == -1)
		{
			return;
		}
		int num5 = 0;
		num3 = 0;
		int upLevelByExp = this.GetUpLevelByExp(num4, equipGoodsSuitID, num, out num5, out num3);
		this.TextNextLevel.Text = upLevelByExp.ToString();
		for (int j = 0; j < this.EquipPropsValue.Length; j++)
		{
			if (this.EquipPropsValue[j] > 0)
			{
				this.TextNextProp[j].Text = (this.EquipPropsValue[j] * upLevelByExp).ToString();
			}
		}
		if (num3 > 0)
		{
			this.ProgressBar.ProgessText = string.Format("{0}/{1}", num4 - num5, num3);
			double num6 = (double)(num4 - num5) / (double)num3;
			this.ProgressBar.TweenPercent(this.ProgressBar.Percent + this.LastLevel, num6 + (double)upLevelByExp, 1.0);
			this.LastLevel = (double)upLevelByExp;
		}
	}

	private void InitYuansuUpDataDict()
	{
		if (this.YuansuUpDataDict == null)
		{
			this.YuansuUpDataDict = new Dictionary<int, Dictionary<int, ElementsHeartXmlData>>();
		}
		XElement gameResXml = Global.GetGameResXml("Config/ElementsHeart.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "XingZuo");
		if (xelementList == null)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = Global.GetXElement(xelementList[i], "XingZuo");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			List<XElement> xelementList2 = Global.GetXElementList(xelement, "*");
			if (xelementList2 != null)
			{
				Dictionary<int, ElementsHeartXmlData> dictionary = new Dictionary<int, ElementsHeartXmlData>();
				int num = 0;
				for (int j = 0; j < xelementList2.Count; j++)
				{
					ElementsHeartXmlData elementsHeartXmlData = new ElementsHeartXmlData();
					elementsHeartXmlData.ID = Global.GetXElementAttributeInt(xelementList2[j], "ID");
					elementsHeartXmlData.NeedExp = Global.GetXElementAttributeInt(xelementList2[j], "NeedExp");
					num += elementsHeartXmlData.NeedExp;
					elementsHeartXmlData.TotalExp = num;
					if (!dictionary.ContainsKey(elementsHeartXmlData.ID))
					{
						dictionary.Add(elementsHeartXmlData.ID, elementsHeartXmlData);
					}
				}
				if (!this.YuansuUpDataDict.ContainsKey(xelementAttributeInt))
				{
					this.YuansuUpDataDict.Add(xelementAttributeInt, dictionary);
				}
			}
		}
	}

	private int GetSumExpByGoodsDatas2(Dictionary<int, GoodsData> dict)
	{
		int result = 0;
		if (dict == null || dict.Count <= 0)
		{
			return result;
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
		int num = 0;
		if (this.YuansuUpDataDict == null)
		{
			return num;
		}
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(gd.GoodsID);
		if (equipGoodsSuitID == -1)
		{
			return num;
		}
		if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 810)
		{
			return this.GetSpecialGoodsDataExp(gd.GoodsID);
		}
		int num2 = 0;
		int num3 = 0;
		Global.GetYuansuGoodsDataLevelAndExp(gd, out num2, out num3);
		num += num3;
		Dictionary<int, ElementsHeartXmlData> dictionary = null;
		if (this.YuansuUpDataDict.TryGetValue(equipGoodsSuitID, ref dictionary))
		{
			ElementsHeartXmlData elementsHeartXmlData = null;
			if (dictionary.TryGetValue(num2, ref elementsHeartXmlData))
			{
				num += elementsHeartXmlData.TotalExp;
			}
		}
		return num;
	}

	private int GetSpecialGoodsDataExp(int goodsID)
	{
		if (this.SpecialGoodsDataExpDict == null)
		{
			this.SpecialGoodsDataExpDict = new Dictionary<int, int>();
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("SpecialElementsHeart", '|');
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
		Dictionary<int, ElementsHeartXmlData> dictionary = null;
		if (this.YuansuUpDataDict.TryGetValue(suitID, ref dictionary))
		{
			int num = 0;
			for (int i = level; i < dictionary.Count; i++)
			{
				ElementsHeartXmlData elementsHeartXmlData = null;
				if (dictionary.TryGetValue(i, ref elementsHeartXmlData))
				{
					if (elementsHeartXmlData.ID == level)
					{
						num = elementsHeartXmlData.TotalExp;
					}
					if (totalExp < elementsHeartXmlData.TotalExp - num)
					{
						nextExp = elementsHeartXmlData.NeedExp;
						return result;
					}
					useExp = elementsHeartXmlData.TotalExp - num;
					result = elementsHeartXmlData.ID;
				}
			}
		}
		return result;
	}

	private int GetExpByLevel(int suitID, int level, bool isTotal = false)
	{
		if (this.YuansuUpDataDict == null)
		{
			return 0;
		}
		suitID = Math.Min(this.YuansuUpDataDict.Count, suitID);
		int result = 0;
		Dictionary<int, ElementsHeartXmlData> dictionary = null;
		if (this.YuansuUpDataDict.TryGetValue(suitID, ref dictionary))
		{
			level = Math.Min(dictionary.Count, level);
			ElementsHeartXmlData elementsHeartXmlData = null;
			if (dictionary.TryGetValue(level, ref elementsHeartXmlData))
			{
				if (isTotal)
				{
					result = elementsHeartXmlData.TotalExp;
				}
				else
				{
					result = elementsHeartXmlData.NeedExp;
				}
			}
		}
		return result;
	}

	private int GetMaxLevelByGoodsData(GoodsData gd)
	{
		int result = 0;
		if (this.YuansuUpDataDict == null || this.YuansuUpDataDict.Count <= 0)
		{
			return result;
		}
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(gd.GoodsID);
		Dictionary<int, ElementsHeartXmlData> dictionary = null;
		if (this.YuansuUpDataDict.TryGetValue(equipGoodsSuitID, ref dictionary))
		{
			result = dictionary.Count;
		}
		return result;
	}

	private void StartQianghua()
	{
		if (this.EquipGoodsData == null)
		{
			return;
		}
		if (this.YuansuUpDataDict == null)
		{
			return;
		}
		if (Global.GetYuansuGoodsDataLevel(this.EquipGoodsData) >= this.GetMaxLevelByGoodsData(this.EquipGoodsData))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经达到最大级别"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int id = this.EquipGoodsData.Id;
		int site = this.EquipGoodsData.Site;
		if (JinglingYuansuPart.SelectedGoodsDataDict == null || JinglingYuansuPart.SelectedGoodsDataDict.Count <= 0)
		{
			return;
		}
		string text = string.Empty;
		foreach (GoodsData goodsData in JinglingYuansuPart.SelectedGoodsDataDict.Values)
		{
			text += string.Format("{0}|", goodsData.Id);
		}
		this.ShowModalDialog();
		Super.PlayAnim(this.Anim[0]);
		GameInstance.Game.SpriteStartQianghua(this.EquipDbID, site, text);
	}

	public void NotifyResult(int result, GoodsData gd)
	{
		this.CloseModalDialog();
		if (result == 0)
		{
			this.AddEquip(gd);
			Super.PlayAnim(this.Anim[1]);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升级成功"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升级时发生错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
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

	private const string GAME_CONFIG_ELEMENTSHEART_FILE_MU = "Config/ElementsHeart.xml";

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL EquipIcon;

	public GameObject[] Anim;

	public GameObject Texts;

	public TextBlock TextCurrentLevel;

	public TextBlock TextNextLevel;

	public TextBlock[] TextCurrentProp;

	public TextBlock[] TextNextProp;

	public GameObject[] Arrows;

	public GImgProgressBar ProgressBar;

	public TextBlock TextTotalExp;

	public GCheckBox CheckBoxWhite;

	public GCheckBox CheckBoxGreen;

	public GCheckBox CheckBoxBlue;

	public GButton SubmitBtn;

	public TextBlock[] checkName;

	private GoodsData EquipGoodsData;

	private int[] EquipPropsValue = new int[2];

	private int _EquipDbID = -1;

	private double LastLevel;

	private Dictionary<int, Dictionary<int, ElementsHeartXmlData>> YuansuUpDataDict;

	private Dictionary<int, int> SpecialGoodsDataExpDict;

	public enum AnimTypes
	{
		None = -1,
		Submit,
		NotifyOk
	}
}
