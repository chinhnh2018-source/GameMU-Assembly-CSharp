using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class LianLuJuHunShuXingPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_ZhanDouLiName.Text = Global.GetLang("战斗力：");
		this.m_QiangHuaName.Text = Global.GetLang("【强化属性】");
		this.m_ZhuiJiaName.Text = Global.GetLang("【追加属性】");
		this.m_PeiYangName.Text = Global.GetLang("【培养属性】");
		this.WULI = Global.GetLang("物理防御：");
		this.MOFA = Global.GetLang("魔法防御：");
		this.LIFE = Global.GetLang("生命上限：");
		this.FUJIA = Global.GetLang("附加伤害：");
		this.DIDANG = Global.GetLang("抵挡伤害：");
		this.GONGJI = Global.GetLang("攻 击 力：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.originalClipRang = this.bodyPanel.clipRange;
		this.originalLocalPosition = this.bodyPanel.transform.localPosition;
	}

	public void InitValue(int juHunID, GoodsData goodsData, string title)
	{
		this.bodyPanel.clipRange = this.originalClipRang;
		this.bodyPanel.transform.localPosition = this.originalLocalPosition;
		this.m_TitleValue.Text = title;
		JuHunData juHunDataById = ParseJuHunConfig.GetJuHunDataById(juHunID);
		this.juHunGrowPercent = juHunDataById.GrowProportion;
		this.m_ZhanDouLiValue.Text = Global.GetGoodsDataZhanLi(goodsData).ToString();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
		int categoriy = goodsXmlNodeByID.Categoriy;
		this.m_QiangHuaValue.Text = this.GetBaseAttributeStr(goodsData, goodsEquipPropsDoubleList, categoriy);
		this.m_ZhuiJiaValue.Text = this.GetZhuijiaAttributeStr(goodsData, goodsEquipPropsDoubleList);
		this.m_PeiYangValue.Text = this.GetXilianAttributeStr(goodsData, goodsEquipPropsDoubleList);
		this.table.repositionNow = true;
	}

	private string GetBaseAttributeStr(GoodsData gd, double[] equipFields_1, int categoriy = -1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if ((categoriy <= 6 || categoriy >= 10) && (categoriy <= 19 || categoriy == 21) && categoriy != 9 && categoriy != 7)
		{
			if (Global.MaxForgeLevel != 20)
			{
				if (gd.Forge_level <= 15)
				{
				}
			}
		}
		if (gd.Forge_level < 0 || gd.Forge_level <= 15)
		{
		}
		double num = (gd.ExcellenceInfo <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd.ExcellenceInfo);
		double num2 = (gd.ExcellenceInfo <= 0) ? 0.0 : Global.GetZhuoYueAddDefenseRates(gd.ExcellenceInfo);
		for (int i = 1; i <= 10; i += 2)
		{
			text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
			if (i == 1)
			{
				if (equipFields_1[i] != 0.0)
				{
					text += string.Format("{0}: {1}%", text2, (int)equipFields_1[i]);
					text += "\n";
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
					if (i == 3 || i == 5)
					{
						num5 += num2 * num5;
						num6 += num2 * num6;
					}
					else if (i == 7 || i == 9)
					{
						num5 += num * num5;
						num6 += num * num6;
					}
					double equipForgeAddBaseValue = Global.GetEquipForgeAddBaseValue(gd, num4);
					double num7;
					double num8;
					if (equipForgeAddBaseValue == 0.0)
					{
						num7 = 0.0;
						num8 = 0.0;
					}
					else
					{
						num7 = Math.Max(num5 * equipForgeAddBaseValue, 3.0);
						num8 = Math.Max(num6 * equipForgeAddBaseValue, 3.0);
					}
					string text3 = string.Empty;
					string text4 = string.Empty;
					if (equipForgeAddBaseValue > 0.0)
					{
						if (categoriy != 7 && categoriy != 23 && categoriy != 24)
						{
							text3 = Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_lv,
								string.Format("(+{0})", (int)(num7 * (double)this.juHunGrowPercent))
							});
							text4 = Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_lv,
								string.Format("(+{0})", (int)(num8 * (double)this.juHunGrowPercent))
							});
							text += string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
							{
								Global.GetColorStringForNGUIText(new object[]
								{
									(num7 <= 0.0) ? this.color_hui : this.color_huang,
									text2
								}),
								Global.GetColorStringForNGUIText(new object[]
								{
									(num7 <= 0.0) ? this.color_hui : this.color_huang,
									": "
								}),
								Global.GetColorStringForNGUIText(new object[]
								{
									this.color_bai,
									(int)num7
								}),
								text3,
								Global.GetColorStringForNGUIText(new object[]
								{
									(num7 <= 0.0) ? this.color_hui : this.color_huang,
									" - "
								}),
								Global.GetColorStringForNGUIText(new object[]
								{
									this.color_bai,
									(int)num8
								}),
								text4
							});
						}
						else
						{
							text += string.Format("{0}{1}{2}{3}{4}", new object[]
							{
								text2,
								": ",
								Global.GetColorStringForNGUIText(new object[]
								{
									this.color_bai,
									(int)num7
								}),
								" - ",
								Global.GetColorStringForNGUIText(new object[]
								{
									this.color_bai,
									(int)num8
								})
							});
						}
					}
					else
					{
						text += string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_huang,
								text2
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_huang,
								": "
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_bai,
								(int)num7
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								"(+0)"
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_huang,
								" - "
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num7 <= 0.0) ? this.color_hui : this.color_bai,
								(int)num8
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								"(+0)"
							})
						});
					}
					text += "\n";
				}
			}
		}
		string text5 = string.Empty;
		for (int i = 11; i < 177; i++)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num9 = equipFields_1[i];
				double equipForgeAddBaseValue2 = Global.GetEquipForgeAddBaseValue(gd, i);
				if (ExtPropIndexes.ExtPropIndexPercents[i] == 1)
				{
					if (categoriy != 8 || i != 24)
					{
						if (categoriy != 24)
						{
							if (equipForgeAddBaseValue2 > 0.0 && categoriy != 7 && categoriy != 23 && categoriy != 24)
							{
								text5 = Global.GetColorStringForNGUIText(new object[]
								{
									"00FF00",
									string.Format("(+{0}%)", (int)equipForgeAddBaseValue2 * 100)
								});
								text += string.Format("{0}{1}{2}{3}{4}", new object[]
								{
									text2,
									": ",
									Global.GetColorStringForNGUIText(new object[]
									{
										this.color_bai,
										(int)(num9 * 100.0)
									}),
									Global.GetColorStringForNGUIText(new object[]
									{
										this.color_bai,
										"%"
									}),
									text5
								});
							}
							else
							{
								text += string.Format("{0}{1}{2}{3}", new object[]
								{
									text2,
									": ",
									Global.GetColorStringForNGUIText(new object[]
									{
										this.color_bai,
										(int)(num9 * 100.0)
									}),
									Global.GetColorStringForNGUIText(new object[]
									{
										this.color_bai,
										"%"
									})
								});
							}
						}
					}
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[i] == 0)
				{
					if (equipForgeAddBaseValue2 > 0.0 && categoriy != 7 && categoriy != 23 && categoriy != 24)
					{
						int num10 = (int)(num9 * equipForgeAddBaseValue2);
						string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
						{
							(num10 <= 0) ? this.color_hui : this.color_lv,
							string.Format("(+{0})", (int)((float)num10 * this.juHunGrowPercent))
						});
						text += string.Format("{0}{1}{2}{3}", new object[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								(num10 <= 0) ? this.color_hui : this.color_huang,
								text2
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num10 <= 0) ? this.color_hui : this.color_huang,
								": "
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num10 <= 0) ? this.color_hui : this.color_huang,
								num10
							}),
							colorStringForNGUIText
						});
					}
					else
					{
						int num11 = (int)(num9 * equipForgeAddBaseValue2);
						string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
						{
							(num11 <= 0) ? this.color_hui : this.color_lv,
							string.Format("(+{0})", (int)((float)num11 * this.juHunGrowPercent))
						});
						text += string.Format("{0}{1}{2}{3}", new object[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								(num11 <= 0) ? this.color_hui : this.color_huang,
								text2
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num11 <= 0) ? this.color_hui : this.color_huang,
								": "
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								(num11 <= 0) ? this.color_hui : this.color_huang,
								num11
							}),
							colorStringForNGUIText2
						});
					}
				}
				text += "\n";
			}
		}
		return this.ProcessStr(text);
	}

	private string GetZhuijiaAttributeStr(GoodsData gd, double[] equipFields_1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (gd.AppendPropLev <= 0)
		{
			for (int i = 8; i <= 13; i++)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num = equipFields_1[i];
				if (num != 0.0)
				{
					string text3 = string.Format("(+{0})", 0);
					text = Global.GetColorStringForNGUIText(new object[]
					{
						this.color_hui,
						string.Format("{0}: {1}{2}", text2, 0, text3)
					});
					text += "\n";
				}
			}
			return this.ProcessStr(text);
		}
		int maxZhuijiaLevelByGoodsData = Global.GetMaxZhuijiaLevelByGoodsData(gd);
		double num2 = (gd.ExcellenceInfo <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd.ExcellenceInfo);
		double num3 = (gd.ExcellenceInfo <= 0) ? 0.0 : Global.GetZhuoYueAddDefenseRates(gd.ExcellenceInfo);
		for (int i = 8; i <= 13; i++)
		{
			text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
			double num4 = equipFields_1[i];
			if (num4 != 0.0)
			{
				double equipZhuijiaAddBaseValue = Global.GetEquipZhuijiaAddBaseValue(gd, i);
				if (i == 8 || i == 10)
				{
					num4 += num2 * num4;
				}
				int num5 = (int)Math.Ceiling(num4 * equipZhuijiaAddBaseValue);
				if (num5 > 0)
				{
					string text4 = string.Format("(+{0})", (int)((float)num5 * this.juHunGrowPercent));
					text += string.Format("{0}: {1}{2}", text2, Global.GetColorStringForNGUIText(new object[]
					{
						this.color_bai,
						num5
					}), Global.GetColorStringForNGUIText(new object[]
					{
						this.color_lv,
						text4
					}));
					text += "\n";
				}
			}
		}
		return this.ProcessStr(text);
	}

	private string GetXilianAttributeStr(GoodsData gd, double[] equipFields_1)
	{
		string text = string.Empty;
		int num = 0;
		int num2 = 5;
		if (gd.ExcellenceInfo <= 0)
		{
			return text;
		}
		Dictionary<int, int> xilianPropsUpLimitDict = Global.GetXilianPropsUpLimitDict(gd);
		float xilianPropsUpFactor = Global.GetXilianPropsUpFactor(gd);
		if (xilianPropsUpLimitDict == null)
		{
			return text;
		}
		if (gd.WashProps == null)
		{
			foreach (int num3 in xilianPropsUpLimitDict.Keys)
			{
				if (xilianPropsUpLimitDict[num3] > 0)
				{
					if (ExtPropIndexes.ExtPropIndexPercents[num3] == 1)
					{
						text += string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num3]), 0);
					}
					else if (ExtPropIndexes.ExtPropIndexPercents[num3] == 0)
					{
						text += string.Format("{0} {1}{2}{3}{4}", new object[]
						{
							Global.GetLang(Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								ExtPropIndexes.ChineseNames[num3]
							})),
							Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								0
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								"(+"
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								0
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								this.color_hui,
								")"
							})
						});
					}
					text += this.GetSpaceString(Mathf.Max(num2 - num.ToString().Length, 0));
					text += "\n";
				}
			}
		}
		else
		{
			for (int i = 0; i < gd.WashProps.Count; i += 2)
			{
				int num4 = gd.WashProps[i];
				num = gd.WashProps[i + 1];
				if (ExtPropIndexes.ExtPropIndexPercents[num4] == 1)
				{
					text += string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num);
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[num4] == 0)
				{
					text += string.Format("{0} {1}{2}{3}{4}", new object[]
					{
						Global.GetLang(ExtPropIndexes.ChineseNames[num4]),
						Global.GetColorStringForNGUIText(new object[]
						{
							this.color_bai,
							num
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							this.color_lv,
							"(+"
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							this.color_lv,
							(int)((float)num * this.juHunGrowPercent)
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							this.color_lv,
							")"
						})
					});
				}
				text += this.GetSpaceString(Mathf.Max(num2 - num.ToString().Length, 0));
				text += "\n";
			}
		}
		return this.ProcessStr(text);
	}

	private string GetSpaceString(int num)
	{
		string text = string.Empty;
		while (num > 0)
		{
			text += "  ";
			num--;
		}
		return text;
	}

	private string GetUpLimitValueStr(Dictionary<int, int> dict, float factor, int key, int currentValue)
	{
		int num = 0;
		string text = string.Empty;
		if (dict != null && dict.TryGetValue(key, ref num))
		{
			num = (int)((float)num * factor);
			text += "     ";
			if (currentValue >= num)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"808080",
					Global.GetLang("已达上限")
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"808080",
					Global.GetLang(string.Format(Global.GetLang("最高上限 +{0}"), num))
				});
			}
		}
		return text;
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	public SpriteSL m_Title;

	public TextBlock m_TitleValue;

	public SpriteSL m_ZhanDouLi;

	public TextBlock m_ZhanDouLiName;

	public TextBlock m_ZhanDouLiValue;

	public SpriteSL m_QiangHua;

	public TextBlock m_QiangHuaName;

	public TextBlock m_QiangHuaValue;

	public SpriteSL m_ZhuiJia;

	public TextBlock m_ZhuiJiaName;

	public TextBlock m_ZhuiJiaValue;

	public SpriteSL m_PeiYang;

	public TextBlock m_PeiYangName;

	public TextBlock m_PeiYangValue;

	private string WULI;

	private string MOFA;

	private string LIFE;

	private string FUJIA;

	private string DIDANG;

	private string GONGJI;

	public UITable table;

	private float juHunGrowPercent;

	private string color_hui = "808081";

	private string color_huang = "e3b36c";

	private string color_lv = "17e43e";

	private string color_bai = "f0f0f0";

	public UIPanel bodyPanel;

	private Vector4 originalClipRang;

	private Vector3 originalLocalPosition;
}
