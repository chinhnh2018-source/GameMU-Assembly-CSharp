using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class JingLingShuXinQiyuanPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitPrefabText();
		this.mItem.SetActive(false);
		this.mOBCView = this.mViewListBox.ItemsSource;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameObject parent = this.Parent;
			Object.Destroy(parent.gameObject, 0.01f);
		};
		XElement gameResXml = Global.GetGameResXml("Config/ExtPropIndexes.xml");
		if (gameResXml != null)
		{
			List<XElement> list = gameResXml.XElementList("ExtPropIndexes");
			for (int i = 0; i < list.Count; i++)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(list[i], "Word");
				if ("MaxLifePercent" == xelementAttributeStr)
				{
					this.str_MaxLifePercent = Global.GetXElementAttributeStr(list[i], "Description");
				}
				if ("ElementInjurePercent" == xelementAttributeStr)
				{
					this.str_ElementInjurePercent = Global.GetXElementAttributeStr(list[i], "Description");
				}
			}
		}
		this.InnitLevelAwardData();
		this.InnitPetTianFuAwardData();
		if (Global.GetJingLingSkillIsOpen())
		{
			this.m_Back.localScale = this.m_bakSize[1];
			this.btnClose.transform.localPosition = this.m_btnColsePos[1];
			this.InitPetSkillLevel();
			this.InitSkillAward();
			this.mSignL.SetActive(true);
			this.mSignR.SetActive(true);
			UIPanel component = this.mViewDragPanel.GetComponent<UIPanel>();
			Vector4 clipRange = component.clipRange;
			clipRange.z = this.m_bakSize[1].x;
			clipRange.w = this.m_bakSize[1].y;
			component.clipRange = clipRange;
			component.Refresh();
			this.mViewListBox.transform.localPosition = new Vector3(-234f, 0f, 0f);
		}
		else
		{
			this.m_Back.localScale = this.m_bakSize[0];
			this.btnClose.transform.localPosition = this.m_btnColsePos[0];
			this.mSignL.gameObject.SetActive(false);
			this.mSignR.SetActive(false);
			UIPanel component2 = this.mViewDragPanel.GetComponent<UIPanel>();
			Vector4 clipRange2 = component2.clipRange;
			clipRange2.z = this.m_bakSize[0].x;
			clipRange2.w = this.m_bakSize[0].y;
			component2.clipRange = clipRange2;
			component2.Refresh();
			this.mViewListBox.transform.localPosition = new Vector3(-118f, 0f, 0f);
		}
		this.mViewDragPanel.onDragFinished = delegate()
		{
			bool flag = false;
			bool flag2 = false;
			this.mViewDragPanel.IsOnLiftOrRight(ref flag, ref flag2);
			if (flag2)
			{
				this.mSignL.SetActive(true);
				this.mSignR.SetActive(false);
			}
			else if (flag)
			{
				this.mSignL.SetActive(false);
				this.mSignR.SetActive(true);
			}
			else if (!flag && !flag2)
			{
				this.mSignL.SetActive(true);
				this.mSignR.SetActive(true);
			}
		};
	}

	private void InitPrefabText()
	{
	}

	private bool[] CheckJingLingLev(int Forge_level)
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostLevel", '|');
		bool[] array = new bool[4];
		byte b = 0;
		while ((int)b < systemParamStringArrayByName.Length)
		{
			string[] array2 = systemParamStringArrayByName[(int)b].Split(new char[]
			{
				','
			});
			if (Forge_level >= Convert.ToInt32(array2[1]))
			{
				if ((int)b >= array.Length - 1)
				{
					break;
				}
				array[(int)b] = true;
			}
			b += 1;
		}
		return array;
	}

	private void InitPetSkillLevel()
	{
		int num = 0;
		List<GoodsData> equipPet = Global.Data.equipPet;
		if (equipPet != null)
		{
			for (int i = 0; i < equipPet.Count; i++)
			{
				if (equipPet[i] != null)
				{
					bool[] array = this.CheckJingLingLev(equipPet[i].Forge_level + 1);
					List<int> elementhrtsProps = equipPet[i].ElementhrtsProps;
					if (elementhrtsProps != null && 3 <= elementhrtsProps.Count)
					{
						int num2 = 0;
						int num3 = 1;
						int num4 = 2;
						int num5 = 0;
						while (4 >= num5)
						{
							if (num2 >= elementhrtsProps.Count)
							{
								break;
							}
							if (elementhrtsProps[num2] == 1)
							{
								if (0 < elementhrtsProps[num4])
								{
									num += elementhrtsProps[num3];
								}
								else if (num5 < array.Length && array[num5])
								{
									num++;
								}
							}
							num2 = num4 + 1;
							num3 = num2 + 1;
							num4 = num3 + 1;
							num5++;
						}
					}
					else
					{
						byte b = 0;
						while ((int)b < array.Length)
						{
							if (array[(int)b])
							{
								num++;
							}
							b += 1;
						}
					}
				}
			}
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>  AllLevel   = " + num.ToString() + "</color>"
			});
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		JingLingShuXinQiyuanPart.QiYuanItem qiYuanItem = new JingLingShuXinQiyuanPart.QiYuanItem(this.mItem.transform, this.mFont);
		XElement gameResXml = Global.GetGameResXml("Config/PetSkillLevelAward.xml");
		qiYuanItem.SetTitle(Global.GetLang("精灵技能等级奇缘"));
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "PelSkillLevelAward");
			if (xelementList != null)
			{
				XElement xelement = null;
				int num6 = -1;
				for (int j = 0; j < xelementList.Count; j++)
				{
					if (xelementList[j] != null)
					{
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[j], "Level");
						if (num >= xelementAttributeInt)
						{
							xelement = xelementList[j];
							num6 = j;
						}
						if (j == xelementList.Count - 1 && num6 != -1 && xelement == null)
						{
							xelement = xelementList[j];
							num6 = j;
						}
					}
				}
				if (xelement != null)
				{
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShuXing");
					list.Add(Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("精灵技能总等级达到：") + Global.GetXElementAttributeInt(xelement, "Level")
					}));
					string[] array2 = xelementAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array2 != null && 0 < array2.Length)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							string[] array3 = array2[k].Split(new char[]
							{
								','
							});
							if (array3 != null && array3.Length == 2)
							{
								list.Add(Global.GetColorStringForNGUIText(new object[]
								{
									"fdf7dd",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array3[0], true) + Global.GetLang("：") + ((!ConfigExtPropIndexes.GetPercentByWord(array3[0])) ? array3[1].ToString() : ((Convert.ToDouble(array3[1]) * 100.0).ToString() + "%"))
								}));
							}
						}
					}
					qiYuanItem.SetInf1(list);
				}
				else
				{
					list.Add(string.Empty);
					list.Add(Global.GetLang("无效果"));
					qiYuanItem.SetInf1(list);
				}
				if (num6 == xelementList.Count - 1)
				{
					list2.Add(" ");
					list2.Add(Global.GetLang("已达到最高级"));
					qiYuanItem.SetInf2(list2);
				}
				else
				{
					xelement = xelementList[num6 + 1];
					if (xelement != null)
					{
						list2.Add(Global.GetColorStringForNGUIText(new object[]
						{
							"888888",
							Global.GetLang("下一级")
						}));
						string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ShuXing");
						list2.Add(Global.GetColorStringForNGUIText(new object[]
						{
							"888888",
							Global.GetLang("精灵技能总等级达到：") + Global.GetXElementAttributeInt(xelement, "Level")
						}));
						string[] array4 = xelementAttributeStr2.Split(new char[]
						{
							'|'
						});
						if (array4 != null && 0 < array4.Length)
						{
							for (int l = 0; l < array4.Length; l++)
							{
								string[] array5 = array4[l].Split(new char[]
								{
									','
								});
								if (array5 != null && array5.Length == 2)
								{
									list2.Add(Global.GetColorStringForNGUIText(new object[]
									{
										"888888",
										ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array5[0], true) + Global.GetLang("：") + ((!ConfigExtPropIndexes.GetPercentByWord(array5[0])) ? array5[1].ToString() : ((Convert.ToDouble(array5[1]) * 100.0).ToString() + "%"))
									}));
								}
							}
						}
						qiYuanItem.SetInf2(list2);
					}
				}
			}
		}
		this.mOBCView.AddNoUpdate(qiYuanItem.Root);
		qiYuanItem.DragPanel = this.mViewDragPanel;
	}

	private void InitSkillAward()
	{
		List<GoodsData> equipPet = Global.Data.equipPet;
		List<int> list = new List<int>();
		if (equipPet != null)
		{
			for (int i = 0; i < equipPet.Count; i++)
			{
				if (equipPet[i].ElementhrtsProps != null && equipPet[i].ElementhrtsProps.Count != 0)
				{
					for (int j = 0; j < equipPet[i].ElementhrtsProps.Count; j++)
					{
						if (j % 3 == 2 && 0 < equipPet[i].ElementhrtsProps[j])
						{
							list.Add(equipPet[i].ElementhrtsProps[j]);
						}
					}
				}
			}
		}
		string text = string.Empty;
		int num = 0;
		XElement gameResXml = Global.GetGameResXml("Config/PetSkillGroupProperty.xml");
		if (gameResXml != null)
		{
			List<XElement> list2 = gameResXml.XElementList("PetSkillGroupProperty");
			string xelementAttributeStr = Global.GetXElementAttributeStr(list2[0], "SkillList");
			text = Global.GetXElementAttributeStr(list2[0], "Name");
			string[] array = xelementAttributeStr.Split(new char[]
			{
				'|'
			});
			for (int k = 0; k < list.Count; k++)
			{
				for (int l = 0; l < array.Length; l++)
				{
					if (array[l] == list[k].ToString())
					{
						num++;
					}
				}
			}
			for (int m = 0; m < list2.Count; m++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(list2[m], "SkillNum");
				if (num >= xelementAttributeInt)
				{
				}
			}
			this.m_DicXml.Clear();
			for (int n = 0; n < list2.Count; n++)
			{
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(list2[n], "SkillNum");
				this.m_DicXml.Add(xelementAttributeInt2, list2[n]);
			}
		}
		this.SetSkillAwardTxt(num);
		this.m_TopTitle.Text = text;
	}

	private void SetSkillAwardTxt(int skillNum)
	{
		JingLingShuXinQiyuanPart.QiYuanItem qiYuanItem = new JingLingShuXinQiyuanPart.QiYuanItem(this.mItem.transform, this.mFont);
		qiYuanItem.SetTitle(Global.GetLang("技能品质奇缘"));
		XElement xelement = null;
		int num = -1;
		List<string> list = new List<string>();
		if (this.m_DicXml.TryGetValue(skillNum, ref xelement))
		{
			num = Global.GetXElementAttributeInt(xelement, "NextID");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Property");
			string[] array = xelementAttributeStr.Split(new char[]
			{
				'|'
			});
			string[] array2 = array[0].Split(new char[]
			{
				','
			});
			string[] array3 = array[1].Split(new char[]
			{
				','
			});
			float num2 = (!(array2[0] == "ElementInjurePercent")) ? 0f : float.Parse(array2[1]);
			float num3 = (!(array3[0] == "MaxLifePercent")) ? 0f : float.Parse(array3[1]);
			list.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format(Global.GetLang("紫色品质技能个数：{0}"), skillNum)
			}));
			list.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format(Global.GetLang("{0}：{1}%"), this.str_ElementInjurePercent, num2 * 100f)
			}));
			list.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format(Global.GetLang("{0}：{1}%"), this.str_MaxLifePercent, num3 * 100f)
			}));
			qiYuanItem.SetInf1(list);
		}
		else
		{
			List<string> list2 = new List<string>();
			list2.Add(" ");
			list2.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("无效果")
			}));
			qiYuanItem.SetInf1(list2);
			if (3 > skillNum)
			{
				num = 1000;
			}
		}
		if (num != -1)
		{
			Dictionary<int, XElement>.Enumerator enumerator = this.m_DicXml.GetEnumerator();
			List<string> list3 = new List<string>();
			list3.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"888888",
				Global.GetLang("下一级")
			}));
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, XElement> keyValuePair = enumerator.Current;
				xelement = keyValuePair.Value;
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (xelementAttributeInt == num)
				{
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Property");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "SkillNum");
					string[] array4 = xelementAttributeStr2.Split(new char[]
					{
						'|'
					});
					string[] array5 = array4[0].Split(new char[]
					{
						','
					});
					string[] array6 = array4[1].Split(new char[]
					{
						','
					});
					float num4 = (!(array5[0] == "ElementInjurePercent")) ? 0f : float.Parse(array5[1]);
					float num5 = (!(array6[0] == "MaxLifePercent")) ? 0f : float.Parse(array6[1]);
					list3.Add(Global.GetColorStringForNGUIText(new object[]
					{
						"888888",
						string.Format(Global.GetLang("紫色品质技能个数：{0}"), xelementAttributeInt2)
					}));
					list3.Add(Global.GetColorStringForNGUIText(new object[]
					{
						"888888",
						string.Format(Global.GetLang("{0}：{1}%"), this.str_ElementInjurePercent, num4 * 100f)
					}));
					list3.Add(Global.GetColorStringForNGUIText(new object[]
					{
						"888888",
						string.Format(Global.GetLang("{0}：{1}%"), this.str_MaxLifePercent, num5 * 100f)
					}));
					break;
				}
			}
			qiYuanItem.SetInf2(list3);
		}
		else
		{
			List<string> list4 = new List<string>();
			list4.Add(" ");
			list4.Add(Global.GetLang("已达到最高级"));
			qiYuanItem.SetInf2(list4);
		}
		this.mOBCView.AddNoUpdate(qiYuanItem.Root);
		qiYuanItem.DragPanel = this.mViewDragPanel;
	}

	public void InnitLevelAwardData()
	{
		List<GoodsData> equipPet = Global.Data.equipPet;
		int num = 0;
		foreach (GoodsData goodsData in equipPet)
		{
			num += goodsData.Forge_level + 1;
		}
		XElement petLevelAwardXML = JingLingShuXinQiyuanPart.GetPetLevelAwardXML();
		List<XElement> list = petLevelAwardXML.XElementList("PelLevelAward");
		int num2 = int.Parse(list[0].GetXElementAttrStr("Level"));
		int num3 = int.Parse(list[list.Count - 1].GetXElementAttrStr("Level"));
		if (num < num2)
		{
			this.InnitPetLevelAwardInfo(null, list[0]);
		}
		else if (num >= num3)
		{
			this.InnitPetLevelAwardInfo(list[list.Count - 1], null);
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (num - int.Parse(list[i].GetXElementAttrStr("Level")) == 0)
				{
					this.InnitPetLevelAwardInfo(list[i], list[i + 1]);
					i = 99999;
				}
				else if (num - int.Parse(list[i].GetXElementAttrStr("Level")) < 0)
				{
					this.InnitPetLevelAwardInfo(list[i - 1], list[i]);
					i = 99999;
				}
			}
		}
	}

	public void InnitPetTianFuAwardData()
	{
		List<GoodsData> equipPet = Global.Data.equipPet;
		int num = 0;
		foreach (GoodsData goodData in equipPet)
		{
			List<string> zhuoYueAttribute = Global.GetZhuoYueAttribute(goodData);
			num += zhuoYueAttribute.Count;
		}
		XElement petTianFuAwardXML = JingLingShuXinQiyuanPart.GetPetTianFuAwardXML();
		List<XElement> list = petTianFuAwardXML.XElementList("PetTianFuAward");
		int num2 = int.Parse(list[0].Attribute("TianFuNum").Value);
		int num3 = int.Parse(list[list.Count - 1].GetXElementAttrStr("TianFuNum"));
		if (num < num2)
		{
			this.InnitPetTianFuAwardInfo(null, list[0]);
		}
		else if (num >= num3)
		{
			this.InnitPetTianFuAwardInfo(list[list.Count - 1], null);
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (num - int.Parse(list[i].GetXElementAttrStr("TianFuNum")) < 0)
				{
					this.InnitPetTianFuAwardInfo(list[i - 1], list[i]);
					i = 99999;
				}
				else if (num - int.Parse(list[i].GetXElementAttrStr("TianFuNum")) == 0)
				{
					this.InnitPetTianFuAwardInfo(list[i], list[i + 1]);
					i = 99999;
				}
			}
		}
	}

	private void InnitPetLevelAwardInfo(XElement XElement1, XElement XElement2)
	{
		JingLingShuXinQiyuanPart.QiYuanItem qiYuanItem = new JingLingShuXinQiyuanPart.QiYuanItem(this.mItem.transform, this.mFont);
		qiYuanItem.SetTitle(Global.GetLang("等级奇缘"));
		if (XElement1 != null)
		{
			List<string> list = new List<string>();
			list.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("精灵总等级达到") + ":" + XElement1.GetXElementAttrStr("Level")
			}));
			foreach (string text in XElement1.Attribute("ShuXing").Value.Split(new char[]
			{
				'|'
			}))
			{
				for (int j = 0; j < ExtPropIndexes.ExtPropIndexNames.Length; j++)
				{
					if (ExtPropIndexes.ExtPropIndexNames[j].ToUpper() == text.Split(new char[]
					{
						','
					})[0].ToUpper())
					{
						if (float.Parse(text.Split(new char[]
						{
							','
						})[1]) < 1f)
						{
							list.Add(Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								string.Concat(new object[]
								{
									ExtPropIndexes.ChineseNames[j],
									"  +  ",
									float.Parse(text.Split(new char[]
									{
										','
									})[1]) * 100f,
									"%"
								})
							}));
						}
						else
						{
							list.Add(Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								ExtPropIndexes.ChineseNames[j] + ":" + text.Split(new char[]
								{
									','
								})[1]
							}));
						}
					}
				}
			}
			qiYuanItem.SetInf1(list);
		}
		else
		{
			List<string> list2 = new List<string>();
			list2.Add(" ");
			list2.Add(Global.GetLang("无效果"));
			qiYuanItem.SetInf1(list2);
		}
		if (XElement2 != null)
		{
			List<string> list3 = new List<string>();
			list3.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"888888",
				Global.GetLang("下一级")
			}));
			list3.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"888888",
				Global.GetLang("精灵总等级达到") + ":" + XElement2.GetXElementAttrStr("Level")
			}));
			foreach (string text2 in XElement2.Attribute("ShuXing").Value.Split(new char[]
			{
				'|'
			}))
			{
				for (int l = 0; l < ExtPropIndexes.ExtPropIndexNames.Length; l++)
				{
					if (ExtPropIndexes.ExtPropIndexNames[l].ToUpper() == text2.Split(new char[]
					{
						','
					})[0].ToUpper())
					{
						if (float.Parse(text2.Split(new char[]
						{
							','
						})[1]) < 1f)
						{
							list3.Add(Global.GetColorStringForNGUIText(new object[]
							{
								"888888",
								string.Concat(new object[]
								{
									ExtPropIndexes.ChineseNames[l],
									"  +  ",
									float.Parse(text2.Split(new char[]
									{
										','
									})[1]) * 100f,
									"%"
								})
							}));
						}
						else
						{
							list3.Add(Global.GetColorStringForNGUIText(new object[]
							{
								"888888",
								ExtPropIndexes.ChineseNames[l] + ":" + text2.Split(new char[]
								{
									','
								})[1]
							}));
						}
					}
				}
			}
			qiYuanItem.SetInf2(list3);
		}
		this.mOBCView.AddNoUpdate(qiYuanItem.Root);
		qiYuanItem.DragPanel = this.mViewDragPanel;
	}

	private void InnitPetTianFuAwardInfo(XElement XElement1, XElement XElement2)
	{
		JingLingShuXinQiyuanPart.QiYuanItem qiYuanItem = new JingLingShuXinQiyuanPart.QiYuanItem(this.mItem.transform, this.mFont);
		qiYuanItem.SetTitle(Global.GetLang("天赋奇缘"));
		if (XElement1 != null)
		{
			List<string> list = new List<string>();
			list.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("精灵总天赋达到") + ":" + XElement1.GetXElementAttrStr("TianFuNum") + Global.GetLang("条")
			}));
			foreach (string text in XElement1.Attribute("ShuXing").Value.Split(new char[]
			{
				'|'
			}))
			{
				for (int j = 0; j < ExtPropIndexes.ExtPropIndexNames.Length; j++)
				{
					if (ExtPropIndexes.ExtPropIndexNames[j].ToUpper() == text.Split(new char[]
					{
						','
					})[0].ToUpper())
					{
						string text2 = string.Empty;
						if (float.Parse(text.Split(new char[]
						{
							','
						})[1]) < 1f)
						{
							text2 += Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								string.Concat(new object[]
								{
									ExtPropIndexes.ChineseNames[j],
									"  +  ",
									float.Parse(text.Split(new char[]
									{
										','
									})[1]) * 100f,
									"%"
								})
							});
						}
						else
						{
							text2 += Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								ExtPropIndexes.ChineseNames[j] + ":" + text.Split(new char[]
								{
									','
								})[1]
							});
						}
						list.Add(text2);
					}
				}
			}
			qiYuanItem.SetInf1(list);
		}
		else
		{
			List<string> list2 = new List<string>();
			list2.Add(" ");
			list2.Add(Global.GetLang("无效果"));
			qiYuanItem.SetInf1(list2);
		}
		if (XElement2 != null)
		{
			List<string> list3 = new List<string>();
			list3.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"888888",
				Global.GetLang("下一级")
			}));
			list3.Add(Global.GetColorStringForNGUIText(new object[]
			{
				"888888",
				Global.GetLang("精灵总天赋达到") + ":" + XElement2.GetXElementAttrStr("TianFuNum") + Global.GetLang("条")
			}));
			foreach (string text3 in XElement2.GetXElementAttrStr("ShuXing").Split(new char[]
			{
				'|'
			}))
			{
				for (int l = 0; l < ExtPropIndexes.ExtPropIndexNames.Length; l++)
				{
					if (ExtPropIndexes.ExtPropIndexNames[l].ToUpper() == text3.Split(new char[]
					{
						','
					})[0].ToUpper())
					{
						if (float.Parse(text3.Split(new char[]
						{
							','
						})[1]) < 1f)
						{
							list3.Add(Global.GetColorStringForNGUIText(new object[]
							{
								"888888",
								string.Concat(new object[]
								{
									ExtPropIndexes.ChineseNames[l],
									"  +  ",
									float.Parse(text3.Split(new char[]
									{
										','
									})[1]) * 100f,
									"%"
								})
							}));
						}
						else
						{
							list3.Add(Global.GetColorStringForNGUIText(new object[]
							{
								"888888",
								ExtPropIndexes.ChineseNames[l] + ":" + text3.Split(new char[]
								{
									','
								})[1]
							}));
						}
					}
				}
			}
			qiYuanItem.SetInf2(list3);
		}
		this.mOBCView.AddNoUpdate(qiYuanItem.Root);
		qiYuanItem.DragPanel = this.mViewDragPanel;
	}

	public static XElement GetPetLevelAwardXML()
	{
		return Global.GetGameResXml(string.Format("Config/PetLevelAward.xml", new object[0]));
	}

	public static XElement GetPetTianFuAwardXML()
	{
		return Global.GetGameResXml(string.Format("Config/PetTianFuAward.xml", new object[0]));
	}

	public Transform m_Back;

	public GButton btnClose;

	public TextBlock m_TopTitle;

	private Vector3[] m_btnColsePos = new Vector3[]
	{
		new Vector3(255.5674f, 207.3244f, -1.000004f),
		new Vector3(352.9f, 203.9f, -1.000004f)
	};

	private Vector3[] m_bakSize = new Vector3[]
	{
		new Vector3(520f, 420f, 1f),
		new Vector3(706f, 412f, 1f)
	};

	private Vector3[] m_OldInfPos = new Vector3[]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(-130f, 0f, 0f)
	};

	private Dictionary<int, XElement> m_DicXml = new Dictionary<int, XElement>();

	private string str_MaxLifePercent = string.Empty;

	private string str_ElementInjurePercent = string.Empty;

	[SerializeField]
	private GameObject mItem;

	[SerializeField]
	private UIFont mFont;

	[SerializeField]
	private ListBox mViewListBox;

	[SerializeField]
	private UIDraggablePanel mViewDragPanel;

	private ObservableCollection mOBCView;

	[SerializeField]
	private GameObject mSignL;

	[SerializeField]
	private GameObject mSignR;

	private class QiYuanItem
	{
		public QiYuanItem(Transform itemObj, UIFont font)
		{
			this.mRoot = Object.Instantiate<GameObject>(itemObj.gameObject).transform;
			this.mRoot.gameObject.SetActive(true);
			this.mFont = font;
			this.mRoot1 = this.mRoot.transform.FindChild("Root1");
			this.mRooe2 = this.mRoot.transform.FindChild("Root2");
			this.mTitleLabel = this.mRoot.transform.FindChild("Title/Label").GetComponent<UILabel>();
			if (null != this.Root.GetComponent<BoxCollider>())
			{
				this.Root.GetComponent<BoxCollider>().enabled = Global.GetJingLingSkillIsOpen();
			}
		}

		public UIDraggablePanel DragPanel
		{
			set
			{
				UIDragPanelContents uidragPanelContents = this.mRoot.GetComponent<UIDragPanelContents>();
				if (null == uidragPanelContents)
				{
					uidragPanelContents = this.mRoot.gameObject.AddComponent<UIDragPanelContents>();
				}
				uidragPanelContents.draggablePanel = value;
				if (null != this.mRoot.GetComponent<UIPanel>())
				{
					Object.Destroy(this.mRoot.GetComponent<UIPanel>());
				}
			}
		}

		public Transform Root
		{
			get
			{
				return this.mRoot;
			}
		}

		public void SetInf1(List<string> StrList)
		{
			try
			{
				for (int i = 0; i < StrList.Count; i++)
				{
					Vector3 scale;
					scale..ctor(18f, 18f, 0f);
					if (i == 0)
					{
						scale..ctor(20f, 20f, 0f);
					}
					UILabel label = this.GetLabel(this.mRoot1.gameObject, scale);
					label.text = StrList[i];
					label.transform.localPosition = new Vector3(0f, (float)(-24 * i), 0f);
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
		}

		public void SetInf2(List<string> StrList)
		{
			try
			{
				for (int i = 0; i < StrList.Count; i++)
				{
					UILabel label = this.GetLabel(this.mRooe2.gameObject, new Vector3(18f, 18f, 0f));
					label.text = StrList[i];
					label.transform.localPosition = new Vector3(0f, (float)(-24 * i), 0f);
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
		}

		public void SetTitle(string Str)
		{
			this.mTitleLabel.text = Str;
		}

		private UILabel GetLabel(GameObject parent, Vector3 scale)
		{
			UILabel uilabel = NGUITools.AddWidget<UILabel>(parent);
			uilabel.font = this.mFont;
			uilabel.transform.localScale = scale;
			uilabel.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			return uilabel;
		}

		private const int lineHeight = 24;

		private Transform mRoot;

		private Transform mRoot1;

		private Transform mRooe2;

		private UIFont mFont;

		private UILabel mTitleLabel;
	}
}
