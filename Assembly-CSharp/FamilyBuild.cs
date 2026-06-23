using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class FamilyBuild : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (null != this.listBox.LastSelectedItem && this.listBox.LastSelectedItem != this.listBox.SelectedItem)
		{
			FamilyBuildItem familyBuildItem = U3DUtils.AS<FamilyBuildItem>(this.listBox.LastSelectedItem);
			familyBuildItem.SelectedState = false;
		}
		if (null != this.listBox.SelectedItem && this.listBox.LastSelectedItem != this.listBox.SelectedItem)
		{
			FamilyBuildItem familyBuildItem = U3DUtils.AS<FamilyBuildItem>(this.listBox.SelectedItem);
			familyBuildItem.SelectedState = true;
			this.RefreshPromptBoard(familyBuildItem);
			this.InitUpgradeButton(familyBuildItem.BuildingType, familyBuildItem.CurrentLevel);
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnUpgrade.Text = Global.GetLang("升级");
		this.ConstLblBuffEffect.text = Global.GetLang("技能效果");
		this.ConstLblBuffPersistTime.text = Global.GetLang("持续时间");
		this.ConstLblBuffEffectNextTitle.text = Global.GetLang("下一级效果");
		this.ConstLblBuffEffectNext.text = Global.GetLang("技能效果");
		this.ConstLblBuffPersistTimeNext.text = Global.GetLang("持续时间");
		this.ConstLblUpgrade.text = Global.GetLang("升级条件");
		this.lblBuffEffect.transform.localPosition = new Vector3(-180f, 85f, 0f);
		this.lblBuffPersistTime.transform.localPosition = new Vector3(-125f, 54f, 0f);
		this.lblBuffEffectNext.transform.localPosition = new Vector3(-180f, -32f, 0f);
		this.lblBuffPersistTimeNext.transform.localPosition = new Vector3(-125f, -63f, 0f);
		this.lblUpgrade.transform.localPosition = new Vector3(-180f, -95f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.InitBuildParams();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, null);
			}
		};
		this.btnUpgrade.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.m_btnUpgradeCanClick)
			{
				return;
			}
			this.m_btnUpgradeCanClick = false;
			base.StartCoroutine<bool>(this.ChangeValue_Time(0.75f));
			if (this._ZwId != 0)
			{
				FamilyBuildItem familyBuildItem = U3DUtils.AS<FamilyBuildItem>(this.listBox.SelectedItem);
				if (familyBuildItem == null)
				{
					familyBuildItem = U3DUtils.AS<FamilyBuildItem>(this.ItemCollection[0]);
				}
				if (familyBuildItem.CurrentLevel < 10)
				{
					if (familyBuildItem.BuildingType == 1)
					{
						Global.SendEvent("709", Global.GetLang("战盟军旗升级次数"));
					}
					if (familyBuildItem.BuildingType == 2)
					{
						Global.SendEvent("711", Global.GetLang("战盟祭坛升级次数"));
					}
					if (familyBuildItem.BuildingType == 3)
					{
						Global.SendEvent("713", Global.GetLang("战盟军械升级次数"));
					}
					if (familyBuildItem.BuildingType == 4)
					{
						Global.SendEvent("715", Global.GetLang("战盟光环升级次数"));
					}
					GameInstance.Game.SpriteZhanMengBuildLevelUpCmd(Global.Data.roleData.Faction, familyBuildItem.BuildingType, familyBuildItem.CurrentLevel + 1);
				}
				else
				{
					Super.HintMainText(Global.GetLang("该建筑已经达到最大等级！"), 10, 3);
				}
			}
		};
		this.lblZhangong.text = string.Empty + Global.Data.roleData.BangGong;
	}

	private IEnumerator ChangeValue_Time(float time_)
	{
		yield return new WaitForSeconds(time_);
		this.m_btnUpgradeCanClick = true;
		yield break;
	}

	public void SetRoleInfo(int ZwId)
	{
		this._ZwId = ZwId;
	}

	public void SetBuildingInfo(int zhanqiLevel, int jitanLevel, int junjieLevel, int guanghuanLevel, int familyMoney)
	{
		this.lblMaintanceCost.text = string.Empty + Global.ZhanMengWeiHuXiaoHao;
		this.currentBuildInfo.Add(1, zhanqiLevel);
		this.currentBuildInfo.Add(2, jitanLevel);
		this.currentBuildInfo.Add(3, junjieLevel);
		this.currentBuildInfo.Add(4, guanghuanLevel);
		this.lblLeagueMoney.text = string.Empty + familyMoney;
		if (this.currentBuildInfo.Count == this.buildConfDict.Count)
		{
			this.InitListBox();
		}
	}

	private void InitBuildParams()
	{
		if (this.buildConfDict == null)
		{
			this.buildConfDict = new Dictionary<int, Dictionary<int, XElement>>();
		}
		else
		{
			this.buildConfDict.Clear();
		}
		XElement gameResXml = Global.GetGameResXml("Config/ZhanMengBuild.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Build");
		Dictionary<int, XElement> dictionary = null;
		for (int i = 0; i < xelementList.Count; i++)
		{
			dictionary = null;
			FamilyBuild.BuildType xelementAttributeInt = (FamilyBuild.BuildType)Global.GetXElementAttributeInt(xelementList[i], "Type");
			if (!this.buildConfDict.TryGetValue((int)xelementAttributeInt, ref dictionary))
			{
				dictionary = new Dictionary<int, XElement>();
				this.buildConfDict.Add((int)xelementAttributeInt, dictionary);
			}
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "Level");
			if (!dictionary.ContainsKey(xelementAttributeInt2))
			{
				dictionary.Add(xelementAttributeInt2, xelementList[i]);
			}
		}
		this.buffIdConfDict.Add(1, this.ParseBuffID("ZhanMengZhanQiBUFF"));
		this.buffIdConfDict.Add(2, this.ParseBuffID("ZhanMengJiTanBUFF"));
		this.buffIdConfDict.Add(3, this.ParseBuffID("ZhanMengJunXieBUFF"));
		this.buffIdConfDict.Add(4, this.ParseBuffID("ZhanMengGuangHuanBUFF"));
	}

	private Dictionary<int, string> ParseBuffID(string BuildName)
	{
		Dictionary<int, string> dictionary = null;
		string systemParamByName = ConfigSystemParam.GetSystemParamByName(BuildName, true);
		if (systemParamByName != null)
		{
			string[] array = systemParamByName.Split(new char[]
			{
				','
			});
			dictionary = new Dictionary<int, string>();
			for (int i = 0; i < array.Length; i++)
			{
				dictionary.Add(i + 1, array[i]);
			}
		}
		return dictionary;
	}

	private void InitListBox()
	{
		XElement xelement = null;
		Dictionary<int, XElement> dictionary = null;
		foreach (KeyValuePair<int, int> keyValuePair in this.currentBuildInfo)
		{
			if (this.buildConfDict.TryGetValue(keyValuePair.Key, ref dictionary) && dictionary.TryGetValue(keyValuePair.Value, ref xelement))
			{
				FamilyBuildItem familyBuildItem = U3DUtils.NEW<FamilyBuildItem>();
				this.ItemCollection.AddNoUpdate(familyBuildItem);
				familyBuildItem.BuildingType = keyValuePair.Key;
				familyBuildItem.CurrentLevel = keyValuePair.Value;
				familyBuildItem.NeedZhangong = int.Parse(Global.GetXElementAttributeStr(xelement, "ConvertCost"));
				int num = int.Parse(this.buffIdConfDict.GetValue(keyValuePair.Key).GetValue(keyValuePair.Value));
				int goodsIconCodeByID = Global.GetGoodsIconCodeByID(num);
				familyBuildItem.SetIcon("NetImages/GameRes/Images/Goods/" + goodsIconCodeByID + ".png");
				familyBuildItem.BuffID = num;
				UIPanel component = familyBuildItem.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
		}
		if (this.ItemCollection.Count > 0)
		{
			this.listBox.SelectedIndex = 0;
			FamilyBuildItem familyBuildItem2 = U3DUtils.AS<FamilyBuildItem>(this.ItemCollection[0]);
			this.RefreshPromptBoard(familyBuildItem2);
			this.InitUpgradeButton(familyBuildItem2.BuildingType, familyBuildItem2.CurrentLevel);
		}
	}

	private void RefreshPromptBoard(FamilyBuildItem item)
	{
		if (null != item)
		{
			int buildingType = item.BuildingType;
			int currentLevel = item.CurrentLevel;
			int num = currentLevel + 1;
			int num2 = 14;
			this.lblBuffPersistTime.text = string.Empty;
			switch (buildingType)
			{
			case 1:
			{
				this.lblPromptBoardTitle.text = Global.GetLang("战盟旗帜");
				UILabel uilabel = this.lblBuffEffect;
				string text = Global.GetLang("生命上限提高:");
				this.lblBuffEffectNext.text = text;
				uilabel.text = text;
				this.lblBuffPersistTime.text = Global.GetLang("{3CA723}30分钟{-}");
				this.lblBuffPersistTimeNext.text = ((num >= 11) ? string.Empty : Global.GetColorStringForNGUIText(new object[]
				{
					"3CA723",
					this.GetBuffTimeConfDict()[1][num] / 60 + Global.GetLang("分钟")
				}));
				this.lblUpgrade.text = ((currentLevel >= 10) ? Global.GetLang("已达到最大等级") : (Global.GetLang("所有建筑均达到") + currentLevel + Global.GetLang("级")));
				num2 = 14;
				this.texBuildingicon.ImageURL = "NetImages/GameRes/Images/Plate/FamilyBuildZhanqi.png";
				this.texBuildingicon.ForceShow();
				break;
			}
			case 2:
			{
				this.lblPromptBoardTitle.text = Global.GetLang("战盟祭坛");
				UILabel uilabel2 = this.lblBuffEffect;
				string text = Global.GetLang("经验加成提高:");
				this.lblBuffEffectNext.text = text;
				uilabel2.text = text;
				this.lblUpgrade.text = ((currentLevel >= 10) ? Global.GetLang("已达到最大等级") : Global.GetLang("祭坛等级低于战旗等级"));
				num2 = -1;
				this.texBuildingicon.ImageURL = "NetImages/GameRes/Images/Plate/FamilyBuildJitan.png";
				this.texBuildingicon.ForceShow();
				break;
			}
			case 3:
			{
				this.lblPromptBoardTitle.text = Global.GetLang("战盟军械");
				UILabel uilabel3 = this.lblBuffEffect;
				string text = Global.GetLang("伤害提高:");
				this.lblBuffEffectNext.text = text;
				uilabel3.text = text;
				this.lblBuffPersistTime.text = Global.GetLang("{3CA723}30分钟{-}");
				this.lblBuffPersistTimeNext.text = ((num >= 11) ? string.Empty : Global.GetColorStringForNGUIText(new object[]
				{
					"3CA723",
					this.GetBuffTimeConfDict()[3][num] / 60 + Global.GetLang("分钟")
				}));
				this.lblUpgrade.text = ((currentLevel >= 5) ? Global.GetLang("已达到最大等级") : Global.GetLang("军械等级低于战旗等级"));
				num2 = 11;
				this.texBuildingicon.ImageURL = "NetImages/GameRes/Images/Plate/FamilyBuildJunjie.png";
				this.texBuildingicon.ForceShow();
				break;
			}
			case 4:
			{
				this.lblPromptBoardTitle.text = Global.GetLang("战盟光环");
				UILabel uilabel4 = this.lblBuffEffect;
				string text = Global.GetLang("防御提升:");
				this.lblBuffEffectNext.text = text;
				uilabel4.text = text;
				this.lblBuffPersistTime.text = Global.GetLang("{3CA723}30分钟{-}");
				this.lblBuffPersistTimeNext.text = ((num >= 11) ? string.Empty : Global.GetColorStringForNGUIText(new object[]
				{
					"3CA723",
					this.GetBuffTimeConfDict()[4][num] / 60 + Global.GetLang("分钟")
				}));
				this.lblUpgrade.text = ((currentLevel >= 10) ? Global.GetLang("已达到最大等级") : Global.GetLang("光环等级低于战旗等级"));
				num2 = 42;
				this.texBuildingicon.ImageURL = "NetImages/GameRes/Images/Plate/FamilyBuildGuanghuan.png";
				this.texBuildingicon.ForceShow();
				break;
			}
			}
			if (num2 != -1)
			{
				string text2 = null;
				Dictionary<int, string> dictionary = null;
				if (this.buffIdConfDict.TryGetValue(buildingType, ref dictionary))
				{
					if (dictionary.TryGetValue(currentLevel, ref text2))
					{
						double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(int.Parse(text2));
						UILabel uilabel5 = this.lblBuffEffect;
						string text = uilabel5.text;
						uilabel5.text = string.Concat(new object[]
						{
							text,
							"{3CA723}",
							goodsEquipPropsDoubleList[num2] * 100.0,
							"%{-}"
						});
					}
					if (dictionary.TryGetValue(num, ref text2))
					{
						double[] goodsEquipPropsDoubleList2 = Global.GetGoodsEquipPropsDoubleList(int.Parse(text2));
						UILabel uilabel6 = this.lblBuffEffectNext;
						string text = uilabel6.text;
						uilabel6.text = string.Concat(new object[]
						{
							text,
							"{3CA723}",
							goodsEquipPropsDoubleList2[num2] * 100.0,
							"%{-}"
						});
					}
				}
			}
			else
			{
				string text3 = null;
				Dictionary<int, string> dictionary2 = null;
				if (this.buffIdConfDict.TryGetValue(buildingType, ref dictionary2))
				{
					if (dictionary2.TryGetValue(currentLevel, ref text3))
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(int.Parse(text3));
						string text4 = goodsXmlNodeByID.ExecMagic;
						if (text4 != null && text4.Length > 0)
						{
							text4 = text4.Substring("DB_ADD_MULTIEXP(".Length);
							text4 = text4.TrimEnd(new char[]
							{
								')'
							});
							string[] array = text4.Split(new char[]
							{
								','
							});
							this.lblBuffPersistTime.text = "{3CA723}" + int.Parse(array[1]) / 60 + Global.GetLang("分钟{-}");
							UILabel uilabel7 = this.lblBuffEffect;
							uilabel7.text = uilabel7.text + "{3CA723}" + array[0] + Global.GetLang("倍{-}");
						}
					}
					if (dictionary2.TryGetValue(num, ref text3))
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(int.Parse(text3));
						string text4 = goodsXmlNodeByID.ExecMagic;
						if (text4 != null && text4.Length > 0)
						{
							text4 = text4.Substring("DB_ADD_MULTIEXP(".Length);
							text4 = text4.TrimEnd(new char[]
							{
								')'
							});
							string[] array = text4.Split(new char[]
							{
								','
							});
							this.lblBuffPersistTimeNext.text = "{3CA723}" + int.Parse(array[1]) / 60 + Global.GetLang("分钟{-}");
							UILabel uilabel8 = this.lblBuffEffectNext;
							uilabel8.text = uilabel8.text + "{3CA723}" + array[0] + Global.GetLang("倍{-}");
						}
					}
				}
			}
			XElement xelement = null;
			Dictionary<int, XElement> dictionary3 = null;
			this.ClearGoodsIcon();
			if (this.buildConfDict.TryGetValue(buildingType, ref dictionary3) && dictionary3.TryGetValue(currentLevel + 1, ref xelement))
			{
				int num3 = int.Parse(this.lblLeagueMoney.text);
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "LevelupCost");
				string text5 = (num3 >= xelementAttributeInt) ? "{FFFFFF}" : "{FF0000}";
				this.lblNeedMoney.text = text5 + Global.GetXElementAttributeStr(xelement, "LevelupCost") + "{-}";
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				this.InitGoods(xelementAttributeStr);
			}
		}
	}

	private void ClearGoodsIcon()
	{
		for (int i = 0; i < this.goodsGroup.transform.childCount; i++)
		{
			GameObject gameObject = this.goodsGroup.transform.FindChild(string.Empty + i).gameObject;
			if (gameObject.transform.childCount > 0)
			{
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					if (!(gameObject.transform.GetChild(j).name == "count"))
					{
						Object.Destroy(gameObject.transform.GetChild(j).gameObject);
					}
				}
			}
		}
	}

	private void InitGoods(string goodsIdStr)
	{
		if (string.IsNullOrEmpty(goodsIdStr))
		{
			return;
		}
		this.ClearGoodsIcon();
		string text = string.Empty;
		string[] array = goodsIdStr.Split(new char[]
		{
			'|'
		});
		this.goodsList.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = this.goodsGroup.transform.FindChild(string.Empty + i).gameObject;
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			text = array2[0] + "," + array2[1] + ",0,0,0,0,0";
			this.initGood(gameObject, text.Split(new char[]
			{
				','
			}), i);
		}
		this.SetGoodsSecText();
	}

	private void initGood(GameObject parent, string[] goods, int idx)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			U3DUtils.AddChild(parent, ggoodIcon.gameObject, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.addEventListener("click", delegate(MouseEvent e)
			{
			});
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			component.center = new Vector3(0f, 0f, -1f);
			this.goodsList.Add(ggoodIcon);
		}
	}

	private void InitUpgradeButton(int buildingType, int buildingLevel)
	{
		bool flag = true;
		XElement xelement = null;
		Dictionary<int, XElement> dictionary = null;
		if (this.buildConfDict.TryGetValue(buildingType, ref dictionary) && dictionary.TryGetValue(buildingLevel, ref xelement))
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "LevelupNeed");
			if (buildingType == 1)
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					flag &= (this.currentBuildInfo[int.Parse(array2[0])] == int.Parse(array2[1]));
				}
			}
			else
			{
				string[] array3 = xelementAttributeStr.Split(new char[]
				{
					','
				});
				flag = (this.currentBuildInfo[int.Parse(array3[0])] == int.Parse(array3[1]));
			}
		}
		this.btnUpgrade.isEnabled = (flag && this._ZwId == 1 && buildingLevel < 10);
	}

	public void NotifyBuildingLevelUpSucess(int buildType, int newLevel, int subMoney)
	{
		this.lblLeagueMoney.text = string.Empty + subMoney;
		this.currentBuildInfo[buildType] = newLevel;
		if (null != this.listBox.SelectedItem)
		{
			FamilyBuildItem familyBuildItem = U3DUtils.AS<FamilyBuildItem>(this.listBox.SelectedItem);
			familyBuildItem.CurrentLevel = newLevel;
			Dictionary<int, XElement> dictionary = null;
			XElement xelement = null;
			if (this.buildConfDict.TryGetValue(familyBuildItem.BuildingType, ref dictionary) && dictionary.TryGetValue(familyBuildItem.CurrentLevel, ref xelement))
			{
				familyBuildItem.NeedZhangong = int.Parse(Global.GetXElementAttributeStr(xelement, "ConvertCost"));
			}
			this.RefreshPromptBoard(familyBuildItem);
			this.InitUpgradeButton(familyBuildItem.BuildingType, familyBuildItem.CurrentLevel);
		}
	}

	public void NotifyGetBuffSucess()
	{
		Super.HintMainText(Global.GetLang("领取BUFF成功！"), 10, 3);
		this.lblZhangong.text = string.Empty + Global.Data.roleData.BangGong;
	}

	public void NotifyGoodsData(BangHuiBagData bangHuiBagData)
	{
		this.m_bangHuiBagData = bangHuiBagData;
		this.SetGoodsSecText();
	}

	private void SetGoodsSecText()
	{
		if (this.m_bangHuiBagData != null)
		{
			for (int i = 0; i < this.goodsList.Count; i++)
			{
				if (i == 0)
				{
					this.goodsList[i].SecondText.text = string.Concat(new object[]
					{
						string.Empty,
						this.m_bangHuiBagData.Goods1Num,
						"/",
						(this.goodsList[i].ItemObject as GoodsData).GCount
					});
				}
				if (i == 1)
				{
					this.goodsList[i].SecondText.text = string.Concat(new object[]
					{
						string.Empty,
						this.m_bangHuiBagData.Goods2Num,
						"/",
						(this.goodsList[i].ItemObject as GoodsData).GCount
					});
				}
				if (i == 2)
				{
					this.goodsList[i].SecondText.text = string.Concat(new object[]
					{
						string.Empty,
						this.m_bangHuiBagData.Goods3Num,
						"/",
						(this.goodsList[i].ItemObject as GoodsData).GCount
					});
				}
				if (i == 3)
				{
					this.goodsList[i].SecondText.text = string.Concat(new object[]
					{
						string.Empty,
						this.m_bangHuiBagData.Goods4Num,
						"/",
						(this.goodsList[i].ItemObject as GoodsData).GCount
					});
				}
			}
		}
	}

	private Dictionary<int, Dictionary<int, int>> GetBuffTimeConfDict()
	{
		if (this.buffTimeConfDict.Count > 0)
		{
			return this.buffTimeConfDict;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ZhanMengBuild.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Build");
		for (int i = 0; i < xelementList.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Type");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "Level");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList[i], "BuffTime");
			if (!this.buffTimeConfDict.ContainsKey(xelementAttributeInt))
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				dictionary.Add(xelementAttributeInt2, xelementAttributeInt3);
				this.buffTimeConfDict.Add(xelementAttributeInt, dictionary);
			}
			else
			{
				this.buffTimeConfDict[xelementAttributeInt].Add(xelementAttributeInt2, xelementAttributeInt3);
			}
		}
		return this.buffTimeConfDict;
	}

	public GButton btnClose;

	public GButton btnUpgrade;

	public UILabel lblBuffEffect;

	public UILabel lblBuffPersistTime;

	public UILabel lblBuffEffectNext;

	public UILabel lblBuffPersistTimeNext;

	public UILabel lblUpgrade;

	public UILabel lblPromptBoardTitle;

	public UILabel lblNeedMoney;

	public UILabel lblLeagueMoney;

	public UILabel lblZhangong;

	public UILabel lblMaintanceCost;

	public UILabel ConstLblBuffEffect;

	public UILabel ConstLblBuffPersistTime;

	public UILabel ConstLblBuffEffectNextTitle;

	public UILabel ConstLblBuffEffectNext;

	public UILabel ConstLblBuffPersistTimeNext;

	public UILabel ConstLblUpgrade;

	public ShowNetImage texBuildingicon;

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	private Dictionary<int, Dictionary<int, XElement>> buildConfDict;

	private Dictionary<int, Dictionary<int, string>> buffIdConfDict = new Dictionary<int, Dictionary<int, string>>();

	private Dictionary<int, int> currentBuildInfo = new Dictionary<int, int>();

	private Dictionary<int, Dictionary<int, int>> buffTimeConfDict = new Dictionary<int, Dictionary<int, int>>();

	public GameObject goodsGroup;

	private BangHuiBagData m_bangHuiBagData;

	private bool m_btnUpgradeCanClick = true;

	private int _ZwId;

	private List<GGoodIcon> goodsList = new List<GGoodIcon>();

	private enum BuildType
	{
		BuildType_Zhanqi = 1,
		BuildType_Jitan,
		BuildType_Junjie,
		BuildType_Guanghuan
	}
}
