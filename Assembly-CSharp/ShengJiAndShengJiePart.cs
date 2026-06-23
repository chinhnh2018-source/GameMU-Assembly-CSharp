using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ShengJiAndShengJiePart : UserControl
{
	public string AddLevelStr
	{
		set
		{
			if (null != this.m_LabAddLevel)
			{
				this.m_LabAddLevel.text = value;
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (0f < this.BtnCD)
		{
			this.BtnCD -= Time.deltaTime;
		}
	}

	public void ClearData()
	{
		base.Destroy();
		for (int i = 0; i < this.mTeXiaoList.Count; i++)
		{
			Object.Destroy(this.mTeXiaoList[i]);
		}
		this.mTeXiaoList.Clear();
		for (int j = this.mUpSpList.Count - 1; j >= 0; j--)
		{
			if (null != this.mUpSpList[j])
			{
				Object.Destroy(this.mUpSpList[j]);
			}
		}
		this.mObservableCollection.Clear();
	}

	private void DisActioveUpSp()
	{
		for (int i = this.mUpSpList.Count - 1; i >= 0; i--)
		{
			if (null != this.mUpSpList[i])
			{
				Object.Destroy(this.mUpSpList[i]);
			}
			this.mUpSpList.RemoveAt(i);
		}
	}

	private GameObject GetUpSp()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.mUpSp.gameObject);
		gameObject.transform.SetParent((this.m_Type != RidePetUpType.UpStar) ? this.mUpStarRoot[1] : this.mUpStarRoot[0], false);
		this.mUpSpList.Add(gameObject);
		gameObject.SetActive(true);
		return gameObject;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_LabTitle.text = Global.GetLang("坐骑升级");
		this.mVector = this.m_ListBoxStarGoods.transform.localPosition;
		int[] keyValue = new int[3];
		int FindKey = 0;
		this.mObservableCollection = this.m_ListBoxStarGoods.ItemsSource;
		this.m_BtnUpLevel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BtnCD > 0f)
			{
				return;
			}
			this.BtnCD = 0.6f;
			int num = 0;
			foreach (KeyValuePair<int, int[]> keyValuePair in ConfigSystemParam.GetSystemParamIntDictByName("HorseLevelMax", '|', ','))
			{
				int num2 = keyValuePair.Value[0] * 1000;
				Dictionary<int, int[]>.Enumerator enumerator;
				KeyValuePair<int, int[]> keyValuePair2 = enumerator.Current;
				if (num2 + keyValuePair2.Value[1] >= Global.Data.roleData.ChangeLifeCount * 1000 + Global.Data.roleData.Level)
				{
					if (FindKey == 1)
					{
						KeyValuePair<int, int[]> keyValuePair3 = enumerator.Current;
						keyValue = keyValuePair3.Value;
						FindKey = 2;
					}
				}
				else
				{
					FindKey = 1;
					KeyValuePair<int, int[]> keyValuePair4 = enumerator.Current;
					num = keyValuePair4.Value[2];
				}
			}
			if (num > this.MountLevel)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendRoleRidePetUp();
			}
			else
			{
				Super.HintMainText(string.Concat(new object[]
				{
					Global.GetLang("人物"),
					keyValue[0],
					Global.GetLang("转"),
					keyValue[1],
					Global.GetLang("级后"),
					Global.GetLang("坐骑等级上限提升至"),
					keyValue[2],
					Global.GetLang("级")
				}), 10, 3);
			}
		};
		this.m_BtnUpStar.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BtnCD > 0f)
			{
				return;
			}
			this.BtnCD = 0.6f;
			GoodsData data = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, this.mSelectID, 1);
			if (data != null)
			{
				bool flag = true;
				HorseAdvancedVO horseAdvancedVOByID = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, data.Forge_level + 1 + 1);
				if (horseAdvancedVOByID != null)
				{
					List<GoodsData> needGoodsLst = horseAdvancedVOByID.NeedGoodsLst;
					if (0 < needGoodsLst.Count)
					{
						for (int i = 0; i < needGoodsLst.Count; i++)
						{
							if (Global.GetRoleGoodsNumberCountByGoodsID(needGoodsLst[i].GoodsID) < needGoodsLst[i].GCount)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (flag)
				{
					if (data.Binding == 0)
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("升阶后您的坐骑将变为绑定，确认要执行该操作吗? "), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								Super.ShowNetWaiting(null);
								GameInstance.Game.SendRoleRidePetUp(data.Id);
							}
							return true;
						};
						return;
					}
					Super.ShowNetWaiting(null);
					GameInstance.Game.SendRoleRidePetUp(data.Id);
				}
				else
				{
					Super.HintMainText(Global.GetLang("所需物品不足"), 10, 3);
				}
			}
		};
		this.mUpSp.gameObject.SetActive(false);
	}

	public void Refresh(RidePetUpType type, GoodsData goodsData, int level)
	{
		this.MountLevel = level;
		this.DisActioveUpSp();
		int num = goodsData.Forge_level + 1;
		this.m_GamePanelAddLevel.SetActive(false);
		this.m_GamePanelMaxStar.SetActive(false);
		this.m_LabAddLevel.gameObject.SetActive(false);
		this.mStarLevelAndNextLevel[0].text = string.Empty;
		this.mStarLevelAndNextLevel[1].text = string.Empty;
		this.m_Type = type;
		if (this.m_Type == RidePetUpType.UpLevel)
		{
			this.m_GamePanelUpStar.SetActive(false);
			if (!this.m_GamePanelUpLevel.gameObject.activeSelf)
			{
				this.m_GamePanelUpLevel.SetActive(true);
			}
			this.SetUpLevel(goodsData, level);
		}
		else if (this.m_Type == RidePetUpType.AddLevel)
		{
			this.m_GamePanelUpStar.SetActive(false);
			this.m_GamePanelUpLevel.SetActive(false);
			this.m_GamePanelAddLevel.SetActive(false);
			this.m_LabAddLevel.gameObject.SetActive(true);
			this.m_GamePanelMaxStar.SetActive(false);
		}
		else if (this.m_Type == RidePetUpType.UpStar)
		{
			this.m_GamePanelUpLevel.SetActive(false);
			this.m_LabStarTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑进阶")
			});
			this.mStarLevelAndNextLevel[0].transform.parent.gameObject.SetActive(true);
			this.m_GamePanelUpStar.SetActive(true);
			this.SetUpOrder(goodsData, level);
		}
		else if (this.m_Type == RidePetUpType.MaxStar)
		{
			this.m_GamePanelUpStar.SetActive(false);
			this.m_GamePanelUpLevel.SetActive(false);
			this.m_GamePanelMaxStar.SetActive(true);
			this.mStarLevelAndNextLevel[0].transform.parent.gameObject.SetActive(false);
		}
		else if (this.m_Type == RidePetUpType.MaxLevel)
		{
			this.m_GamePanelUpStar.SetActive(false);
			this.m_GamePanelUpLevel.SetActive(false);
			this.m_LabStarTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑升级")
			});
			this.m_GamePanelMaxStar.SetActive(true);
			this.mStarLevelAndNextLevel[0].transform.parent.gameObject.SetActive(false);
		}
	}

	public void AddTeXiaoLevel()
	{
		for (int i = 0; i < this.mUpSpList.Count; i++)
		{
			if (null != this.mUpSpList[i])
			{
				GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_shengji_shuzishuaxin", this.mUpSpList[i].transform.parent);
				Vector3 localPosition = this.mUpSpList[i].transform.localPosition;
				localPosition.x = 0f;
				gameObject.transform.localPosition = localPosition;
				if (gameObject.GetComponent<DelayDestroy>() == null)
				{
					DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
					delayDestroy.delayTime = 0.8f;
				}
				this.mTeXiaoList.Add(gameObject);
			}
		}
		GameObject gameObject2 = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_shengji_shuzishuaxin", base.transform);
		Vector3 localPosition2;
		localPosition2..ctor(-90f, 100f, -1f);
		localPosition2.z -= 1f;
		gameObject2.transform.localPosition = localPosition2;
		if (gameObject2.GetComponent<DelayDestroy>() == null)
		{
			DelayDestroy delayDestroy2 = gameObject2.AddComponent<DelayDestroy>();
			delayDestroy2.delayTime = 1f;
		}
		this.mTeXiaoList.Add(gameObject2);
	}

	public void AddTeXiao()
	{
		GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_jinjie_jineng", this.mTeXiaoParent.transform);
		gameObject.transform.localPosition = Vector3.back;
		if (gameObject.GetComponent<DelayDestroy>() == null)
		{
			DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
			delayDestroy.delayTime = 1f;
		}
		this.mTeXiaoList.Add(gameObject);
		GameObject gameObject2 = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_jinjie_jieshu", this.mLabLv.transform.parent.transform);
		Vector3 localPosition = this.mLabLv.transform.localPosition;
		localPosition.z -= 1f;
		gameObject2.transform.localPosition = localPosition;
		if (gameObject2.GetComponent<DelayDestroy>() == null)
		{
			DelayDestroy delayDestroy2 = gameObject2.AddComponent<DelayDestroy>();
			delayDestroy2.delayTime = 1f;
		}
		this.mTeXiaoList.Add(gameObject2);
		for (int i = 0; i < this.mUpSpList.Count; i++)
		{
			if (null != this.mUpSpList[i])
			{
				GameObject gameObject3 = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_shengji_shuzishuaxin", this.mUpSpList[i].transform.parent);
				Vector3 localPosition2 = this.mUpSpList[i].transform.localPosition;
				localPosition2.x = 0f;
				gameObject3.transform.localPosition = localPosition2;
				if (gameObject3.GetComponent<DelayDestroy>() == null)
				{
					DelayDestroy delayDestroy3 = gameObject3.AddComponent<DelayDestroy>();
					delayDestroy3.delayTime = 1f;
				}
				this.mTeXiaoList.Add(gameObject3);
			}
		}
	}

	private void SetUpOrder(GoodsData data, int Level)
	{
		this.mObservableCollection.Clear();
		this.m_ListBoxStarGoods.transform.localPosition = new Vector3(144f, -48f, -0.4f);
		if (1 >= Level)
		{
			Level = 1;
		}
		int num = data.Forge_level + 1;
		this.mSelectID = data.Id;
		this.m_LabStarTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑进阶")
		});
		if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, num) != null)
		{
			this.mLabLv.text = "Bậc " + num.ToString();
			this.mLabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑技能")
			});
			this.m_LabJiChuStarTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑基础属性")
			});
			this.m_BtnUpStar.Text = Global.GetLang("进阶");
			if (num < IConfigbase<ConfigRidePet>.Instance.MaxStar)
			{
				HorseAdvancedVO horseAdvancedVOByID = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, data.Forge_level + 1 + 1);
				if (horseAdvancedVOByID != null)
				{
					List<GoodsData> needGoodsLst = horseAdvancedVOByID.NeedGoodsLst;
					for (int i = 0; i < needGoodsLst.Count; i++)
					{
						GGoodIcon ggoodIcon = Super.AddGoodsIcon(needGoodsLst[i], this.mObservableCollection, false, true);
						ggoodIcon.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
						int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(needGoodsLst[i].GoodsID);
						ggoodIcon.SecondText.Label.supportEncoding = true;
						ggoodIcon.SecondText.text = ((roleGoodsNumberCountByGoodsID < needGoodsLst[i].GCount) ? Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							roleGoodsNumberCountByGoodsID
						}) : Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							needGoodsLst[i].GCount
						})) + "/" + needGoodsLst[i].GCount;
					}
					if (this.mObservableCollection.Count > 1)
					{
						Vector3 localPosition = this.mVector;
						localPosition.x -= (float)(this.mObservableCollection.Count - 1) * this.m_ListBoxStarGoods.cellWidth / 2f;
						this.m_ListBoxStarGoods.transform.localPosition = localPosition;
					}
				}
			}
			HorseAdvancedVO horseAdvancedVOByID2 = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, data.Forge_level + 1);
			if (horseAdvancedVOByID2 != null)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(horseAdvancedVOByID2.SkillID + 1);
				if (skillXmlNode != null)
				{
					this.mShowSkill.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
					{
						skillXmlNode.MagicIcon
					});
					this.mLabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						Global.GetLang(skillXmlNode.Name)
					});
					SkillData skillDataByID = Global.GetSkillDataByID(horseAdvancedVOByID2.SkillID);
					if (skillDataByID != null)
					{
						this.mLabSkillContent.text = skillXmlNode.Description;
					}
				}
				this.mLabLv.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"Bậc " + horseAdvancedVOByID2.Level.ToString()
				});
				this.m_LabLvLeft.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"Bậc" + horseAdvancedVOByID2.Level
				});
				this.m_LabLvRight.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					"Bậc" + (horseAdvancedVOByID2.Level + 1)
				});
				this.m_LabLvLeft.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				this.m_LabLvLeft.transform.parent.gameObject.SetActive(false);
			}
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(Level))
			{
				num3 += IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[Level].AdvancedEffect;
				if (0f > num3)
				{
					num3 = 0f;
				}
			}
			if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, num) != null)
			{
				num2 += IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, num).AdvancedEffectFloat;
				if (0f > num2)
				{
					num2 = 0f;
				}
			}
			if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, num + 1) != null)
			{
				num4 += IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, num + 1).AdvancedEffectFloat;
				if (0f > num4)
				{
					num4 = 0f;
				}
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				double[] equipProps = goodsXmlNodeByID.EquipProps;
				Dictionary<int, double> dictionary = new Dictionary<int, double>();
				Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
				string text = string.Empty;
				string text2 = string.Empty;
				for (int j = 0; j < equipProps.Length; j++)
				{
					if (equipProps[j] > 0.0)
					{
						if (dictionary.ContainsKey(j))
						{
							dictionary[j] = equipProps[j] + equipProps[j] * (double)num2 + equipProps[j] * (double)num3;
						}
						else
						{
							dictionary.Add(j, equipProps[j] + equipProps[j] * (double)num2 + equipProps[j] * (double)num3);
						}
						if (dictionary2.ContainsKey(j))
						{
							Dictionary<int, double> dictionary4;
							Dictionary<int, double> dictionary3 = dictionary4 = dictionary2;
							int num6;
							int num5 = num6 = j;
							double num7 = dictionary4[num6];
							dictionary3[num5] = num7 + (equipProps[j] + equipProps[j] * (double)num4 + equipProps[j] * (double)num3 - dictionary[j]);
						}
						else
						{
							dictionary2.Add(j, equipProps[j] + equipProps[j] * (double)num4 + equipProps[j] * (double)num3 - dictionary[j]);
						}
					}
				}
				float num8 = -44f;
				float num9 = -24f;
				float num10 = 63f;
				foreach (KeyValuePair<int, double> keyValuePair in dictionary)
				{
					int num11 = keyValuePair.Key;
					Dictionary<int, double>.Enumerator enumerator;
					KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
					double value = keyValuePair2.Value;
					if (num11 != 8 && num11 != 9 && num11 != 10 && num11 != 4 && num11 != 5 && num11 != 6)
					{
						if (num11 == 7)
						{
							num11 = 45;
							if (!dictionary2.ContainsKey(num11) && dictionary2.ContainsKey(7))
							{
								dictionary2.Add(num11, dictionary2[7]);
							}
						}
						if (num11 == 3)
						{
							num11 = 46;
							if (!dictionary2.ContainsKey(num11) && dictionary2.ContainsKey(3))
							{
								dictionary2.Add(num11, dictionary2[3]);
							}
						}
						if (num11 != 0)
						{
							if (ConfigExtPropIndexes.GetPercentByID(num11))
							{
								double num12 = value * 100.0;
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num11, true) + Global.GetLang("： "),
									"fdf7dd",
									num12.ToString("f0")
								}) + "%" + Environment.NewLine;
							}
							else
							{
								double num13 = value;
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num11, true) + Global.GetLang("： "),
									"fdf7dd",
									num13.ToString("f0")
								}) + Environment.NewLine;
							}
							num8 += num9;
							if (dictionary2.ContainsKey(num11) && 0.0 < dictionary2[num11])
							{
								if (ConfigExtPropIndexes.GetPercentByID(num11))
								{
									text2 = text2 + Global.GetColorStringForNGUIText(new object[]
									{
										"fdf7dd",
										(dictionary2[num11] * 100.0).ToString("f0")
									}) + "%" + Environment.NewLine;
								}
								else
								{
									text2 += Global.GetColorStringForNGUIText(new object[]
									{
										"fdf7dd",
										dictionary2[num11].ToString("f0") + Environment.NewLine
									});
								}
								GameObject upSp = this.GetUpSp();
								if (null != upSp)
								{
									upSp.transform.localPosition = new Vector3(num10, num8, upSp.transform.localPosition.z);
								}
							}
						}
					}
				}
				this.m_LabJiChuStarContent.text = text;
				this.mLabJiChuAddContent.text = text2;
			}
			else
			{
				this.m_LabJiChuStarContent.text = string.Empty;
				this.mLabJiChuAddContent.text = string.Empty;
			}
		}
	}

	private void SetUpLevel(GoodsData data, int level)
	{
		this.m_LabZuanShi.text = "0";
		this.mSelectID = data.Id;
		int level2 = data.Forge_level + 1;
		if (1 >= level)
		{
			level = 1;
		}
		this.m_LabStarTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑升级")
		});
		if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(level))
		{
			this.m_LabXiaoHaoTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("消耗：")
			});
			this.m_LabJiChuTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑基础属性")
			});
			this.m_BtnUpLevel.Text = Global.GetLang("升级");
			if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(level + 1))
			{
				HorseLevelUpVO horseLevelUpVO = IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[level + 1];
				if (horseLevelUpVO != null)
				{
					this.m_LabLevelLeft.text = (horseLevelUpVO.Level - 1).ToString();
					this.m_LabLevelRight.text = horseLevelUpVO.Level.ToString();
					this.m_LabZuanShi.text = ((Global.GetRoleOwnNumByMoneyType(139) < horseLevelUpVO.Exp) ? Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						horseLevelUpVO.Exp
					}) : horseLevelUpVO.Exp.ToString());
				}
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(level))
			{
				num2 = IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[level].AdvancedEffect;
				if (0f > num2)
				{
					num2 = 0f;
				}
			}
			if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, level2) != null)
			{
				num = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, level2).AdvancedEffectFloat;
				if (0f > num)
				{
					num = 0f;
				}
			}
			if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(level + 1))
			{
				num3 = IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[level + 1].AdvancedEffect;
				if (0f > num3)
				{
					num3 = 0f;
				}
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				double[] equipProps = goodsXmlNodeByID.EquipProps;
				Dictionary<int, double> dictionary = new Dictionary<int, double>();
				Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
				string text = string.Empty;
				string text2 = string.Empty;
				for (int i = 0; i < equipProps.Length; i++)
				{
					if (equipProps[i] > 0.0)
					{
						if (dictionary.ContainsKey(i))
						{
							dictionary[i] = equipProps[i] + equipProps[i] * (double)num + equipProps[i] * (double)num2;
						}
						else
						{
							dictionary.Add(i, equipProps[i] + equipProps[i] * (double)num + equipProps[i] * (double)num2);
						}
						if (dictionary2.ContainsKey(i))
						{
							dictionary2[i] = equipProps[i] + equipProps[i] * (double)num + equipProps[i] * (double)num3 - dictionary[i];
						}
						else
						{
							dictionary2.Add(i, equipProps[i] + equipProps[i] * (double)num + equipProps[i] * (double)num3 - dictionary[i]);
						}
					}
				}
				float num4 = 26f;
				float num5 = -24f;
				float num6 = 84f;
				foreach (KeyValuePair<int, double> keyValuePair in dictionary)
				{
					int num7 = keyValuePair.Key;
					Dictionary<int, double>.Enumerator enumerator;
					KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
					double value = keyValuePair2.Value;
					if (num7 != 8 && num7 != 9 && num7 != 10 && num7 != 4 && num7 != 5 && num7 != 6)
					{
						if (num7 == 7)
						{
							num7 = 45;
							if (!dictionary2.ContainsKey(num7) && dictionary2.ContainsKey(7))
							{
								dictionary2.Add(num7, dictionary2[7]);
							}
						}
						if (num7 == 3)
						{
							num7 = 46;
							if (!dictionary2.ContainsKey(num7) && dictionary2.ContainsKey(3))
							{
								dictionary2.Add(num7, dictionary2[3]);
							}
						}
						if (num7 != 0)
						{
							if (ConfigExtPropIndexes.GetPercentByID(num7))
							{
								double num8 = value * 100.0;
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num7, true) + Global.GetLang("：    "),
									"fdf7dd",
									num8.ToString("f0")
								}) + "%" + Environment.NewLine;
							}
							else
							{
								double num9 = value;
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num7, true) + Global.GetLang("：    "),
									"fdf7dd",
									num9.ToString("f0")
								}) + Environment.NewLine;
							}
							num4 += num5;
							if (dictionary2.ContainsKey(num7) && 0.0 < dictionary2[num7])
							{
								if (ConfigExtPropIndexes.GetPercentByID(num7))
								{
									text2 = text2 + Global.GetColorStringForNGUIText(new object[]
									{
										"fdf7dd",
										(dictionary2[num7] * 100.0).ToString("f0")
									}) + "%" + Environment.NewLine;
								}
								else
								{
									text2 += Global.GetColorStringForNGUIText(new object[]
									{
										"fdf7dd",
										dictionary2[num7].ToString("f0") + Environment.NewLine
									});
								}
								GameObject upSp = this.GetUpSp();
								if (null != upSp)
								{
									upSp.transform.localPosition = new Vector3(num6, num4, upSp.transform.localPosition.z);
								}
							}
						}
					}
				}
				this.m_LabJiChuContent.text = text;
				this.mLabJiChuLevelAddContent.text = text2;
			}
			else
			{
				this.m_LabJiChuContent.text = string.Empty;
				this.mLabJiChuLevelAddContent.text = string.Empty;
			}
		}
	}

	public GameObject m_GamePanelAddLevel;

	public GameObject m_GamePanelMaxStar;

	public GameObject m_GamePanelUpLevel;

	public GameObject m_GamePanelUpStar;

	public UILabel m_LabTitle;

	public UILabel m_LabJiChuTitle;

	public UILabel m_LabJiChuContent;

	[SerializeField]
	private UILabel mLabJiChuLevelAddContent;

	public UILabel m_LabXiaoHaoTitle;

	public UILabel m_LabAddLevel;

	public UILabel m_LabLevelLeft;

	public UILabel m_LabLevelRight;

	public UILabel m_LabZuanShi;

	public GButton m_BtnUpLevel;

	public UILabel m_LabStarTitle;

	public UILabel m_LabSkillTitle;

	public UILabel m_LabLvLeft;

	public UILabel m_LabLvRight;

	public UILabel m_LabSkillContent;

	public UILabel m_LabJiChuStarTitle;

	public UILabel m_LabJiChuStarContent;

	public GButton m_BtnUpStar;

	public ListBox m_ListBoxStarGoods;

	[SerializeField]
	private ShowNetImage mShowSkill;

	[SerializeField]
	private UILabel mLabSkillContent;

	[SerializeField]
	private UILabel mLabSkillTitle;

	[SerializeField]
	private UISprite mUpSp;

	[SerializeField]
	private Transform[] mUpStarRoot;

	[SerializeField]
	private UILabel mLabJiChuAddContent;

	[SerializeField]
	private UILabel[] mStarLevelAndNextLevel;

	[SerializeField]
	private GameObject mTeXiaoParent;

	[SerializeField]
	private UILabel mLabLv;

	private ObservableCollection mObservableCollection;

	private RidePetUpType m_Type;

	private List<GameObject> mUpSpList = new List<GameObject>();

	private List<GameObject> mTeXiaoList = new List<GameObject>();

	private int mSelectID;

	private Vector3 mVector = default(Vector3);

	private float BtnCD;

	private int MountLevel;
}
