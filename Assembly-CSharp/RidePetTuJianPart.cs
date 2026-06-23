using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RidePetTuJianPart : UserControl
{
	protected override void OnDestroy()
	{
		base.OnDestroy();
		IConfigbase<ConfigRidePet>.Instance.ClearHorsePokedexXml();
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
		this.m_Collection = this.m_ListBox.ItemsSource;
		this.InitText();
		this.InitOnClick();
		this.OnTab(1);
		this.mHorseResLoaderList.Clear();
		GameInstance.Game.GetRidePetMainData();
		GameInstance.Game.SendGetRoleRideTuJian();
		Super.ShowNetWaiting(null);
	}

	private void OnEnable()
	{
		GameInstance.Game.GetRidePetMainData();
		GameInstance.Game.SendGetRoleRideTuJian();
		Super.ShowNetWaiting(null);
	}

	private void InitText()
	{
		this.m_BtnLeftShuXing.Label.lineWidth = 110;
		this.m_BtnRightShuXing.Label.lineWidth = 110;
		this.m_LabTuJianTitle.lineWidth = 258;
		this.m_LabTuJianTitle.pivot = 0;
		this.m_LabTuJian.transform.localPosition = new Vector3(this.m_LabTuJian.transform.localPosition.x, -10f, this.m_LabTuJian.transform.localPosition.z);
		this.m_BtnHuoQu.Text = Global.GetLang("获取");
		this.m_LabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑技能")
		});
		this.m_LabJiChuTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑基础属性")
		});
		this.m_LabZhuoYueTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("卓越属性")
		});
		this.m_LabZhuoYueContent.text = string.Empty;
		this.m_LabTuJianTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑图鉴属性")
		});
		if (Global.Data.roleData.MountStoreList != null && 0 < Global.Data.roleData.MountStoreList.Count)
		{
			this.m_BtnBeiBaoHongDian.gameObject.SetActive(true);
			this.m_BtnBeiBao.Text = Global.Data.roleData.MountStoreList.Count.ToString();
		}
		else
		{
			this.m_BtnBeiBaoHongDian.gameObject.SetActive(false);
		}
	}

	private void InitOnClick()
	{
		this.m_BtnLeftShuXing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnTab(1);
		};
		this.m_BtnRightShuXing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnTab(2);
		};
		this.m_BtnHuoQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.HorseLieQu, null, string.Empty, string.Empty);
		};
		this.m_BtnHuoQuTwo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenRidePetLieQuPartWindow(this.mHaveAvtiviteList);
		};
		this.m_BtnZongLan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenZongLan();
		};
		this.m_BtnBeiBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_Handler != null)
			{
				this.m_Handler(this, new DPSelectedItemEventArgs
				{
					Type = 1,
					ID = 0
				});
			}
		};
		this.m_BtnXiangXi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.AddZhuoYue(this.m_Goodid);
		};
	}

	private void OnTab(int key)
	{
		if (key == 1)
		{
			this.m_BtnLeftShuXing.normalSprite = "teamTab_hover";
			this.m_BtnLeftShuXing.target.spriteName = "teamTab_hover";
			this.m_BtnLeftShuXing.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑属性")
			});
			this.m_BtnLeftShuXing.transform.localPosition = new Vector3(this.m_BtnLeftShuXing.transform.localPosition.x, this.m_BtnLeftShuXing.transform.localPosition.y, -0.8f);
			this.m_GameZuoQiPanel.SetActive(true);
			this.m_GameTuJianPanel.SetActive(false);
			this.m_BtnRightShuXing.normalSprite = "teamTab_normal";
			this.m_BtnRightShuXing.target.spriteName = "teamTab_normal";
			this.m_BtnRightShuXing.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("图鉴属性")
			});
			this.m_BtnRightShuXing.transform.localPosition = new Vector3(this.m_BtnRightShuXing.transform.localPosition.x, this.m_BtnRightShuXing.transform.localPosition.y, -0.1f);
		}
		else if (key == 2)
		{
			this.m_BtnLeftShuXing.normalSprite = "teamTab_normal";
			this.m_BtnLeftShuXing.target.spriteName = "teamTab_normal";
			this.m_BtnLeftShuXing.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("坐骑属性")
			});
			this.m_BtnLeftShuXing.transform.localPosition = new Vector3(this.m_BtnLeftShuXing.transform.localPosition.x, this.m_BtnLeftShuXing.transform.localPosition.y, -0.1f);
			this.m_GameZuoQiPanel.SetActive(false);
			this.m_GameTuJianPanel.SetActive(true);
			this.m_BtnRightShuXing.normalSprite = "teamTab_hover";
			this.m_BtnRightShuXing.target.spriteName = "teamTab_hover";
			this.m_BtnRightShuXing.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("图鉴属性")
			});
			this.m_BtnRightShuXing.transform.localPosition = new Vector3(this.m_BtnRightShuXing.transform.localPosition.x, this.m_BtnRightShuXing.transform.localPosition.y, -0.8f);
		}
	}

	private void RerfeshModal(int goodsID, int level)
	{
		this.m_Game3DModel.Clear();
		HorseResLoader horseResLoader = UIHelper.LoadHorseRes(this.m_Game3DModel, goodsID, level, Quaternion.Euler(0f, 135f, 0f), Vector3.one * 110f, delegate(GameObject g)
		{
			if (this.m_Game3DModel.ChildGameObjectList != null && 1 < this.m_Game3DModel.ChildGameObjectList.Count)
			{
				for (int i = this.m_Game3DModel.ChildGameObjectList.Count - 1; i > 0; i--)
				{
					if (null != this.m_Game3DModel.ChildGameObjectList[i])
					{
						Object.Destroy(this.m_Game3DModel.ChildGameObjectList[i]);
						this.m_Game3DModel.ChildGameObjectList.RemoveAt(this.m_Game3DModel.ChildGameObjectList.Count - 1);
					}
				}
				this.m_Game3DModel._Target = this.m_Game3DModel.ChildGameObjectList[0];
			}
		});
		this.mHorseResLoaderList.Add(horseResLoader);
	}

	private void OpenZongLan()
	{
		if (this.mMountData == null || this.mMountData.Count <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < this.mMountData.Count; i++)
		{
			list.Add(this.mMountData[i].GoodsID);
		}
		string text = string.Empty;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int j = 0; j < list.Count; j++)
		{
			HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(list[j]);
			if (horsePokedexByHorseID != null)
			{
				string[] array = horsePokedexByHorseID.PokedexAttribute.Split(new char[]
				{
					'|'
				});
				for (int k = 0; k < array.Length; k++)
				{
					string text2 = array[k].Split(new char[]
					{
						','
					})[0];
					int num = array[k].Split(new char[]
					{
						','
					})[1].SafeToInt32(0);
					if (dictionary.ContainsKey(text2))
					{
						dictionary[text2] += num;
					}
					else
					{
						dictionary.Add(text2, num);
					}
				}
			}
		}
		foreach (KeyValuePair<string, int> keyValuePair in dictionary)
		{
			if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair.Key))
			{
				string text3 = text;
				object[] array2 = new object[2];
				array2[0] = "e3b36c";
				int num2 = 1;
				Dictionary<string, int>.Enumerator enumerator;
				KeyValuePair<string, int> keyValuePair2 = enumerator.Current;
				array2[num2] = Global.GetLang(ConfigExtPropIndexes.GetExtPropIndexesVOByWord(keyValuePair2.Key).Description);
				string colorStringForNGUIText = Global.GetColorStringForNGUIText(array2);
				object[] array3 = new object[2];
				array3[0] = "dac7ae";
				int num3 = 1;
				string text4 = "      ";
				KeyValuePair<string, int> keyValuePair3 = enumerator.Current;
				array3[num3] = text4 + ((float)keyValuePair3.Value / 1000f * 100f).ToString("f0") + "%";
				text = text3 + colorStringForNGUIText + Global.GetColorStringForNGUIText(array3) + Environment.NewLine;
			}
			else
			{
				string text5 = text;
				string[] array4 = new string[5];
				array4[0] = text5;
				int num4 = 1;
				object[] array5 = new object[2];
				array5[0] = "e3b36c";
				int num5 = 1;
				Dictionary<string, int>.Enumerator enumerator;
				KeyValuePair<string, int> keyValuePair4 = enumerator.Current;
				array5[num5] = Global.GetLang(ConfigExtPropIndexes.GetExtPropIndexesVOByWord(keyValuePair4.Key).Description);
				array4[num4] = Global.GetColorStringForNGUIText(array5);
				array4[2] = "      ";
				int num6 = 3;
				object[] array6 = new object[2];
				array6[0] = "dac7ae";
				int num7 = 1;
				KeyValuePair<string, int> keyValuePair5 = enumerator.Current;
				array6[num7] = (float)keyValuePair5.Value;
				array4[num6] = Global.GetColorStringForNGUIText(array6);
				array4[4] = Environment.NewLine;
				text = string.Concat(array4);
			}
		}
		PlayZone.GlobalPlayZone.m_RidePetPart.OpenWindow(Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑图鉴总属性")
		}), text);
		this.m_Handler(this, new DPSelectedItemEventArgs
		{
			Type = 2,
			Title = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑图鉴总属性")
			}),
			CountdownInfo = text
		});
	}

	private void AddRidePetList(List<MountData> listData)
	{
		this.mHaveAvtiviteList.Clear();
		if (listData != null)
		{
			for (int i = 0; i < listData.Count; i++)
			{
				if (listData[i] != null)
				{
					this.mHaveAvtiviteList.Add(listData[i].GoodsID);
				}
			}
		}
		this.m_Collection.Clear();
		List<RidePetTuJianPart.HorseData> list = new List<RidePetTuJianPart.HorseData>();
		List<RidePetTuJianPart.HorseData> list2 = new List<RidePetTuJianPart.HorseData>();
		List<RidePetTuJianPart.HorseData> list3 = new List<RidePetTuJianPart.HorseData>();
		Dictionary<int, HorsePokedexVO>.Enumerator horsePokedexEnumerator = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexEnumerator();
		while (horsePokedexEnumerator.MoveNext())
		{
			RidePetTuJianPart.HorseData horseData = default(RidePetTuJianPart.HorseData);
			horseData.HasAvtive = false;
			KeyValuePair<int, HorsePokedexVO> keyValuePair = horsePokedexEnumerator.Current;
			horseData.vo = keyValuePair.Value;
			horseData.IsNew = false;
			if (listData != null)
			{
				for (int j = 0; j < listData.Count; j++)
				{
					int goodsID = listData[j].GoodsID;
					KeyValuePair<int, HorsePokedexVO> keyValuePair2 = horsePokedexEnumerator.Current;
					if (goodsID == keyValuePair2.Value.HorseGoods)
					{
						horseData.IsNew = listData[j].IsNew;
						horseData.HasAvtive = true;
						break;
					}
				}
			}
			if (horseData.IsNew)
			{
				list.Add(horseData);
			}
			else if (horseData.HasAvtive)
			{
				list2.Add(horseData);
			}
			else
			{
				list3.Add(horseData);
			}
		}
		list2.Sort((RidePetTuJianPart.HorseData a, RidePetTuJianPart.HorseData b) => a.vo.ID - b.vo.ID);
		list3.Sort((RidePetTuJianPart.HorseData a, RidePetTuJianPart.HorseData b) => a.vo.ID - b.vo.ID);
		List<RidePetTuJianPart.HorseData> list4 = new List<RidePetTuJianPart.HorseData>();
		if (0 < list.Count)
		{
			list4.AddRange(list);
		}
		if (0 < list2.Count)
		{
			list4.AddRange(list2);
		}
		if (0 < list3.Count)
		{
			list4.AddRange(list3);
		}
		for (int k = 0; k < list4.Count; k++)
		{
			RidePetItem ridePetItem = U3DUtils.NEW<RidePetItem>();
			this.m_Collection.AddNoUpdate(ridePetItem);
			ridePetItem.DPSelectedItem = new DPSelectedItemEventHandler(this.ItemOnClick);
			if (ridePetItem.GetComponent<BoxCollider>() != null)
			{
				Object.Destroy(ridePetItem.GetComponent<BoxCollider>());
			}
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(list4[k].vo.HorseGoods, IConfigbase<ConfigRidePet>.Instance.MaxStar - 1, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			ridePetItem.SetData(dummyGoodsDataMu, RidePetUIType.TuJian);
			ridePetItem.IsNew = list4[k].IsNew;
			ridePetItem.IsGray = !list4[k].HasAvtive;
			ridePetItem.DragPanel = this.m_DraggablePanel;
		}
		GameObject at = this.m_Collection.GetAt(0);
		if (null != at)
		{
			RidePetItem component = at.GetComponent<RidePetItem>();
			if (null != component && component.GoodsData != null)
			{
				this.AddZuoQi(component.GoodsData);
				this.m_Goodid = component.GoodsData.GoodsID;
				this.AddTuJian(component.GoodsData.GoodsID);
				this.RerfeshModal(component.GoodsData.GoodsID, component.GoodsData.Forge_level);
				component.ItemIsSelect = true;
			}
		}
	}

	private void ItemOnClick(object s, DPSelectedItemEventArgs e)
	{
		if (e.ID == 1 && e.IDType == 3)
		{
			for (int i = 0; i < this.m_Collection.Count; i++)
			{
				GameObject at = this.m_Collection.GetAt(i);
				if (null != at)
				{
					RidePetItem component = at.GetComponent<RidePetItem>();
					if (component.GoodsData != null && e.ZhuZhuangBei != null)
					{
						if (component.GoodsData.GoodsID == e.ZhuZhuangBei.GoodsID)
						{
							component.m_SpBackOnClick.gameObject.SetActive(true);
							this.AddZuoQi(component.GoodsData);
							this.m_Goodid = component.GoodsData.GoodsID;
							this.AddTuJian(component.GoodsData.GoodsID);
							this.RerfeshModal(component.GoodsData.GoodsID, component.GoodsData.Forge_level);
						}
						else
						{
							component.m_SpBackOnClick.gameObject.SetActive(false);
						}
					}
					else
					{
						component.m_SpBackOnClick.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	private void AddTuJian(int goodsID)
	{
		HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(goodsID);
		if (horsePokedexByHorseID == null)
		{
			return;
		}
		string pokedexAttribute = horsePokedexByHorseID.PokedexAttribute;
		string empty = string.Empty;
		string text = string.Empty;
		List<int> list = new List<int>();
		string[] array = pokedexAttribute.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[0]).Description
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"      " + ((float)array2[1].SafeToInt32(0) / 1000f * 100f).ToString("f0") + "%"
				}) + Environment.NewLine;
			}
			else
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[0]).Description
					}),
					"      ",
					array2[1].SafeToInt32(0),
					Environment.NewLine
				});
			}
		}
		this.m_LabTuJian.text = text;
	}

	private void AddZuoQi(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		this.m_LabTitleName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetGoodsNameByID(gd.GoodsID, false)
		});
		this.m_LabSkillContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang(string.Empty)
		});
		this.m_LabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑技能")
		});
		HorseSkillData horseSkillData = new HorseSkillData(gd);
		if (0 < horseSkillData.SkillID)
		{
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(horseSkillData.SkillID);
			if (skillXmlNode != null)
			{
				if (string.IsNullOrEmpty(this.m_ShowSkill.URL) || !this.m_ShowSkill.URL.Equals("NetImages/GameRes/Images/Skill/" + skillXmlNode.MagicIcon + ".png"))
				{
					this.m_ShowSkill.URL = "NetImages/GameRes/Images/Skill/" + skillXmlNode.MagicIcon + ".png";
				}
				this.m_LabSkillContent.text = skillXmlNode.Description;
				this.m_LabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang(skillXmlNode.Name)
				});
			}
		}
		this.y = 98f;
		if (goodsXmlNodeByID != null)
		{
			this.m_LabJiChuTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑基础属性")
			});
			this.y -= this.m_LabJiChuTitle.relativeSize.y * this.m_LabJiChuTitle.transform.localScale.y;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(1))
			{
				num += IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[1].AdvancedEffect;
			}
			if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(goodsXmlNodeByID.ID, 1) != null)
			{
				num2 += IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(goodsXmlNodeByID.ID, 1).AdvancedEffectFloat;
			}
			if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(goodsXmlNodeByID.ID, 10) != null)
			{
				num3 += IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(goodsXmlNodeByID.ID, 10).AdvancedEffectFloat;
			}
			if (0f >= num)
			{
				num = 0f;
			}
			if (0f >= num2)
			{
				num2 = 0f;
			}
			double[] equipProps = goodsXmlNodeByID.EquipProps;
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
			string text = string.Empty;
			string empty = string.Empty;
			for (int i = 0; i < equipProps.Length; i++)
			{
				if (equipProps[i] > 0.0)
				{
					if (dictionary.ContainsKey(i))
					{
						Dictionary<int, double> dictionary4;
						Dictionary<int, double> dictionary3 = dictionary4 = dictionary;
						int num5;
						int num4 = num5 = i;
						double num6 = dictionary4[num5];
						dictionary3[num4] = num6 + (equipProps[i] + equipProps[i] * (double)num2 + equipProps[i] * (double)num);
					}
					else
					{
						dictionary.Add(i, equipProps[i] + equipProps[i] * (double)num2 + equipProps[i] * (double)num);
					}
					if (dictionary2.ContainsKey(i))
					{
						Dictionary<int, double> dictionary6;
						Dictionary<int, double> dictionary5 = dictionary6 = dictionary2;
						int num5;
						int num7 = num5 = i;
						double num6 = dictionary6[num5];
						dictionary5[num7] = num6 + (equipProps[i] + equipProps[i] * (double)num3 + equipProps[i] * (double)num);
					}
					else
					{
						dictionary2.Add(i, equipProps[i] + equipProps[i] * (double)num3 + equipProps[i] * (double)num);
					}
				}
			}
			foreach (KeyValuePair<int, double> keyValuePair in dictionary)
			{
				int num8 = keyValuePair.Key;
				Dictionary<int, double>.Enumerator enumerator;
				KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
				double value = keyValuePair2.Value;
				if (dictionary2.ContainsKey(num8))
				{
					double num9 = dictionary2[num8];
				}
				if (num8 != 0)
				{
					if (num8 != 8 && num8 != 9 && num8 != 10 && num8 != 4 && num8 != 5 && num8 != 6)
					{
						if (num8 == 7)
						{
							num8 = 45;
						}
						if (num8 == 3)
						{
							num8 = 46;
						}
						if (ConfigExtPropIndexes.GetPercentByID(num8))
						{
							double num10 = value * 100.0;
							text = text + Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num8, true) + Global.GetLang("："),
								"fdf7dd",
								num10.ToString("f0") + "%"
							}) + Environment.NewLine;
						}
						else
						{
							double num11 = value;
							text = text + Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num8, true) + Global.GetLang("："),
								"fdf7ff",
								num11.ToString("f0")
							}) + Environment.NewLine;
						}
					}
				}
			}
			HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(gd.GoodsID);
			if (horsePokedexByHorseID != null)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("移动速度：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7ff",
					(int)(horsePokedexByHorseID.HorseSpeed * 100f) + "%"
				}) + Environment.NewLine;
			}
			this.m_LabJiChuContent1.text = text;
			this.m_LabJiChuContent1.transform.localPosition = new Vector3(this.m_LabJiChuContent1.transform.localPosition.x, this.y, this.m_LabJiChuContent1.transform.localPosition.z);
			this.y -= this.m_LabJiChuContent1.relativeSize.y * this.m_LabJiChuContent1.transform.localScale.y;
			this.m_BtnXiangXi.transform.localPosition = new Vector3(this.m_BtnXiangXi.transform.localPosition.x, this.y, this.m_BtnXiangXi.transform.localPosition.z);
			this.m_LabZhuoYueTitle.transform.localPosition = new Vector3(this.m_LabZhuoYueTitle.transform.localPosition.x, this.y, this.m_LabZhuoYueTitle.transform.localPosition.z);
			this.y -= this.m_LabZhuoYueTitle.relativeSize.y * this.m_LabZhuoYueTitle.transform.localScale.y;
			this.m_LabZhuoYueContent.text = string.Empty;
			this.m_LabZhuoYueContent.transform.localPosition = new Vector3(this.m_LabZhuoYueContent.transform.localPosition.x, this.y, this.m_LabZhuoYueContent.transform.localPosition.z);
			HorsePokedexVO horsePokedexByHorseID2 = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(gd.GoodsID);
			if (horsePokedexByHorseID2 != null && IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop.ContainsKey(horsePokedexByHorseID2.SuperiorAttributeID))
			{
				string[] array = IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop[horsePokedexByHorseID2.SuperiorAttributeID].CommonSuperiorRate.Split(new char[]
				{
					'|'
				});
				string[] array2 = IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop[horsePokedexByHorseID2.SuperiorAttributeID].SeniorSuperiorRate.Split(new char[]
				{
					'|'
				});
				this.m_LabZhuoYueContent.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("获得坐骑时，随机获得卓越属性，最多可以获得"),
					"e3b36c",
					(array.Length - 1 + array2.Length - 1).ToString() + Global.GetLang("条")
				});
			}
		}
	}

	private void AddZhuoYue(int goodsID)
	{
		HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(goodsID);
		if (horsePokedexByHorseID == null)
		{
			return;
		}
		int superiorAttributeID = horsePokedexByHorseID.SuperiorAttributeID;
		if (!IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop.ContainsKey(superiorAttributeID))
		{
			return;
		}
		string empty = string.Empty;
		string text = string.Empty;
		List<int> list = new List<int>();
		string[] array = string.Format("{0},{1}", IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop[superiorAttributeID].CommonSuperiorBank, IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop[superiorAttributeID].SeniorSuperiorBank).Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			foreach (KeyValuePair<int, HorseSuperiorType> keyValuePair in IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorType)
			{
				if (keyValuePair.Value.ID == array[i].SafeToInt32(0))
				{
					Dictionary<int, HorseSuperiorType>.Enumerator enumerator;
					KeyValuePair<int, HorseSuperiorType> keyValuePair2 = enumerator.Current;
					string[] array2 = keyValuePair2.Value.Parameter.Split(new char[]
					{
						'|'
					});
					int[] array3 = new int[array2.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						array3[j] = array2[j].Split(new char[]
						{
							','
						})[0].SafeToInt32(0);
					}
					KeyValuePair<int, HorseSuperiorType> keyValuePair3 = enumerator.Current;
					if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair3.Value.Type))
					{
						string text2 = text;
						string[] array4 = new string[5];
						array4[0] = text2;
						int num = 1;
						object[] array5 = new object[2];
						array5[0] = "e3b36c";
						int num2 = 1;
						KeyValuePair<int, HorseSuperiorType> keyValuePair4 = enumerator.Current;
						array5[num2] = Global.GetLang(keyValuePair4.Value.Name) + ":";
						array4[num] = Global.GetColorStringForNGUIText(array5);
						array4[2] = "      ";
						array4[3] = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Concat(new object[]
							{
								(float)Mathf.Min(array3) / 1000f * 100f,
								"%-",
								(float)Mathf.Max(array3) * 100f / 1000f,
								"%"
							})
						});
						array4[4] = Environment.NewLine;
						text = string.Concat(array4);
					}
					else
					{
						string text2 = text;
						string[] array6 = new string[5];
						array6[0] = text2;
						int num3 = 1;
						object[] array7 = new object[2];
						array7[0] = "e3b36c";
						int num4 = 1;
						KeyValuePair<int, HorseSuperiorType> keyValuePair5 = enumerator.Current;
						array7[num4] = Global.GetLang(keyValuePair5.Value.Name) + ":";
						array6[num3] = Global.GetColorStringForNGUIText(array7);
						array6[2] = "      ";
						array6[3] = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							((float)Mathf.Min(array3) / 1000f).ToString("f0") + "-" + ((float)Mathf.Max(array3) / 1000f).ToString("f0")
						});
						array6[4] = Environment.NewLine;
						text = string.Concat(array6);
					}
				}
			}
		}
		if (this.m_Handler != null)
		{
			this.m_Handler(this, new DPSelectedItemEventArgs
			{
				Type = 2,
				Title = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("坐骑卓越属性")
				}),
				CountdownInfo = text
			});
		}
	}

	internal void NoticeRideGetTuJianDataCallBack(List<MountData> data)
	{
		this.AddRidePetList(data);
		if (data != null)
		{
			this.mMountData = data;
		}
	}

	public ListBox m_ListBox;

	public UIDraggablePanel m_DraggablePanel;

	public UILabel m_LabTitleName;

	public GButton m_BtnHuoQu;

	public GButton m_BtnHuoQuTwo;

	public GButton m_BtnZongLan;

	public GButton m_BtnBeiBao;

	public UISprite m_BtnBeiBaoHongDian;

	public Modal3DShow m_Game3DModel;

	public GButton m_BtnLeftShuXing;

	public GButton m_BtnRightShuXing;

	public UILabel m_LabSkillTitle;

	public UILabel m_LabSkillContent;

	public UILabel m_LabJiChuTitle;

	public UILabel m_LabJiChuContent1;

	public UILabel m_LabJiChuContent2;

	public UILabel m_LabZhuoYueTitle;

	public UILabel m_LabZhuoYueContent;

	public GButton m_BtnXiangXi;

	public UILabel m_LabTuJianTitle;

	public UILabel m_LabTuJian;

	public ShowNetImage m_ShowSkill;

	public GameObject m_GameZuoQiPanel;

	public GameObject m_GameTuJianPanel;

	private ObservableCollection m_Collection;

	public DPSelectedItemEventHandler m_Handler;

	private int m_Goodid;

	private List<int> mHaveAvtiviteList = new List<int>();

	private List<MountData> mMountData = new List<MountData>();

	private List<HorseResLoader> mHorseResLoaderList = new List<HorseResLoader>();

	private float y = 98f;

	private struct HorseData
	{
		public bool HasAvtive;

		public HorsePokedexVO vo;

		public bool IsNew;
	}
}
