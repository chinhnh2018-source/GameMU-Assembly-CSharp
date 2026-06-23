using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RideQiYuanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		IConfigbase<ConfigRidePet>.Instance.ClearHorseArrayAdditionData();
	}

	private void InitPrefabText()
	{
		try
		{
			this._Title.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑奇缘")
			});
			for (int i = 0; i < this._ItemContent.Length; i++)
			{
				this._ItemContent[i].Margin = new Vector2(0f, 8f);
			}
			for (int j = 0; j < this._ItemContentValue.Length; j++)
			{
				this._ItemContentValue[j].Margin = new Vector2(0f, 8f);
			}
			for (int k = 0; k < this._ItemNextLecelContent.Length; k++)
			{
				this._ItemNextLecelContent[k].Margin = new Vector2(0f, 8f);
			}
			for (int l = 0; l < this._ItemNextLecelContentValue.Length; l++)
			{
				this._ItemNextLecelContentValue[l].Margin = new Vector2(0f, 8f);
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
			this._ItemBakImage[0].URL = "NetImages/GameRes/Images/RidePet/RongLianJC_DiKuang02.png";
			this._ItemBakImage[1].URL = "NetImages/GameRes/Images/RidePet/RongLianJC_DiKuang02.png";
			this._ItemBakImage[2].URL = "NetImages/GameRes/Images/RidePet/RongLianJC_DiKuang02.png";
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void RefreshDengJi(ZuoQiMainData data)
	{
		this._ItemTitle[0].spriteName = "DengJiQiYuan";
		HorseArrayAdditionVO horseArrayAdditionVO = null;
		HorseArrayAdditionVO horseArrayAdditionVO2 = null;
		Dictionary<int, HorseArrayAdditionVO>.Enumerator horseArrayAdditionEnumerator = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionEnumerator();
		while (horseArrayAdditionEnumerator.MoveNext())
		{
			KeyValuePair<int, HorseArrayAdditionVO> keyValuePair = horseArrayAdditionEnumerator.Current;
			if (keyValuePair.Value.Type == 1)
			{
				int num = data.MountLevel + 1;
				KeyValuePair<int, HorseArrayAdditionVO> keyValuePair2 = horseArrayAdditionEnumerator.Current;
				if (num >= keyValuePair2.Value.NeedLevel)
				{
					KeyValuePair<int, HorseArrayAdditionVO> keyValuePair3 = horseArrayAdditionEnumerator.Current;
					horseArrayAdditionVO = keyValuePair3.Value;
					if (horseArrayAdditionVO.NextID != -1)
					{
						horseArrayAdditionVO2 = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionVOByID(horseArrayAdditionVO.NextID);
					}
					else
					{
						horseArrayAdditionVO2 = null;
					}
				}
			}
		}
		if (horseArrayAdditionVO == null)
		{
			Dictionary<int, HorseArrayAdditionVO>.Enumerator horseArrayAdditionEnumerator2 = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionEnumerator();
			while (horseArrayAdditionEnumerator2.MoveNext())
			{
				KeyValuePair<int, HorseArrayAdditionVO> keyValuePair4 = horseArrayAdditionEnumerator2.Current;
				if (keyValuePair4.Value.Type == 1)
				{
					if (horseArrayAdditionVO2 != null)
					{
						int needLevel = horseArrayAdditionVO2.NeedLevel;
						KeyValuePair<int, HorseArrayAdditionVO> keyValuePair5 = horseArrayAdditionEnumerator2.Current;
						if (needLevel > keyValuePair5.Value.NeedLevel)
						{
							KeyValuePair<int, HorseArrayAdditionVO> keyValuePair6 = horseArrayAdditionEnumerator2.Current;
							horseArrayAdditionVO2 = keyValuePair6.Value;
						}
					}
					else
					{
						KeyValuePair<int, HorseArrayAdditionVO> keyValuePair7 = horseArrayAdditionEnumerator2.Current;
						horseArrayAdditionVO2 = keyValuePair7.Value;
					}
				}
			}
		}
		if (horseArrayAdditionVO == null)
		{
			this._ItemContent[0].text = Global.GetLang("\n \n                  暂无");
			this._ItemContentValue[0].text = string.Empty;
		}
		if (horseArrayAdditionVO != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("总等级达到：")
			}) + "\n";
			text2 = text2 + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				horseArrayAdditionVO.NeedLevel + Global.GetLang("级")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator = horseArrayAdditionVO.AdditionPropsDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text3 = text;
				KeyValuePair<string, double> keyValuePair8 = enumerator.Current;
				text = text3 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair8.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair9 = enumerator.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
				{
					string text4 = text2;
					KeyValuePair<string, double> keyValuePair10 = enumerator.Current;
					text2 = text4 + (keyValuePair10.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text5 = text2;
					KeyValuePair<string, double> keyValuePair11 = enumerator.Current;
					text2 = text5 + keyValuePair11.Value.ToString("f0") + "\n";
				}
			}
			this._ItemContent[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			this._ItemContentValue[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				text2
			});
		}
		if (horseArrayAdditionVO2 != null)
		{
			string text6 = string.Empty;
			string text7 = string.Empty;
			text6 = text6 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("总等级达到：")
			}) + "\n";
			text7 = text7 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				horseArrayAdditionVO2.NeedLevel + Global.GetLang("级")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator2 = horseArrayAdditionVO2.AdditionPropsDic.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				string text8 = text6;
				KeyValuePair<string, double> keyValuePair12 = enumerator2.Current;
				text6 = text8 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair13 = enumerator2.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair13.Key))
				{
					string text9 = text7;
					KeyValuePair<string, double> keyValuePair14 = enumerator2.Current;
					text7 = text9 + (keyValuePair14.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text10 = text7;
					KeyValuePair<string, double> keyValuePair15 = enumerator2.Current;
					text7 = text10 + keyValuePair15.Value.ToString("f0") + "\n";
				}
			}
			this._ItemNextLevelTitle[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下一级")
			});
			this._ItemNextLecelContent[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text6
			});
			this._ItemNextLecelContentValue[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text7
			});
		}
		else
		{
			this._NextLevelRoot[0].SetActive(false);
			this._NextMaxLevelRoot[0].SetActive(true);
		}
	}

	private void InitTianFu()
	{
		int num = 0;
		if (Global.Data.roleData.MountEquipList != null)
		{
			for (int i = 0; i < Global.Data.roleData.MountEquipList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.MountEquipList[i];
				if (goodsData != null && Global.Data.roleData.MountEquipList[i].WashProps != null)
				{
					num += Global.Data.roleData.MountEquipList[i].WashProps.Count / 2;
				}
			}
		}
		this._ItemTitle[1].spriteName = "TianFuQiYuan";
		HorseArrayAdditionVO horseArrayAdditionVO = null;
		HorseArrayAdditionVO horseArrayAdditionVO2 = null;
		Dictionary<int, HorseArrayAdditionVO>.Enumerator horseArrayAdditionEnumerator = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionEnumerator();
		while (horseArrayAdditionEnumerator.MoveNext())
		{
			KeyValuePair<int, HorseArrayAdditionVO> keyValuePair = horseArrayAdditionEnumerator.Current;
			if (keyValuePair.Value.Type == 2)
			{
				int num2 = num;
				KeyValuePair<int, HorseArrayAdditionVO> keyValuePair2 = horseArrayAdditionEnumerator.Current;
				if (num2 >= keyValuePair2.Value.NeedSuperiorNum)
				{
					KeyValuePair<int, HorseArrayAdditionVO> keyValuePair3 = horseArrayAdditionEnumerator.Current;
					horseArrayAdditionVO = keyValuePair3.Value;
					if (horseArrayAdditionVO.NextID != -1)
					{
						horseArrayAdditionVO2 = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionVOByID(horseArrayAdditionVO.NextID);
					}
					else
					{
						horseArrayAdditionVO2 = null;
					}
				}
			}
		}
		if (horseArrayAdditionVO == null)
		{
			Dictionary<int, HorseArrayAdditionVO>.Enumerator horseArrayAdditionEnumerator2 = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionEnumerator();
			while (horseArrayAdditionEnumerator2.MoveNext())
			{
				KeyValuePair<int, HorseArrayAdditionVO> keyValuePair4 = horseArrayAdditionEnumerator2.Current;
				if (keyValuePair4.Value.Type == 2)
				{
					if (horseArrayAdditionVO2 != null)
					{
						int needSuperiorNum = horseArrayAdditionVO2.NeedSuperiorNum;
						KeyValuePair<int, HorseArrayAdditionVO> keyValuePair5 = horseArrayAdditionEnumerator2.Current;
						if (needSuperiorNum > keyValuePair5.Value.NeedSuperiorNum)
						{
							KeyValuePair<int, HorseArrayAdditionVO> keyValuePair6 = horseArrayAdditionEnumerator2.Current;
							horseArrayAdditionVO2 = keyValuePair6.Value;
						}
					}
					else
					{
						KeyValuePair<int, HorseArrayAdditionVO> keyValuePair7 = horseArrayAdditionEnumerator2.Current;
						horseArrayAdditionVO2 = keyValuePair7.Value;
					}
				}
			}
		}
		if (horseArrayAdditionVO == null)
		{
			this._ItemContent[1].text = Global.GetLang("\n \n                  暂无");
			this._ItemContentValue[1].text = string.Empty;
		}
		if (horseArrayAdditionVO != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("总天赋达到：")
			}) + "\n";
			text2 = text2 + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				horseArrayAdditionVO.NeedSuperiorNum + Global.GetLang("条")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator = horseArrayAdditionVO.AdditionPropsDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text3 = text;
				KeyValuePair<string, double> keyValuePair8 = enumerator.Current;
				text = text3 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair8.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair9 = enumerator.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
				{
					string text4 = text2;
					KeyValuePair<string, double> keyValuePair10 = enumerator.Current;
					text2 = text4 + (keyValuePair10.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text5 = text2;
					KeyValuePair<string, double> keyValuePair11 = enumerator.Current;
					text2 = text5 + keyValuePair11.Value.ToString("f0") + "\n";
				}
			}
			this._ItemContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			this._ItemContentValue[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				text2
			});
		}
		if (horseArrayAdditionVO2 != null)
		{
			string text6 = string.Empty;
			string text7 = string.Empty;
			text6 = text6 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("总天赋达到：")
			}) + "\n";
			text7 = text7 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				horseArrayAdditionVO2.NeedSuperiorNum + Global.GetLang("条")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator2 = horseArrayAdditionVO2.AdditionPropsDic.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				string text8 = text6;
				KeyValuePair<string, double> keyValuePair12 = enumerator2.Current;
				text6 = text8 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair13 = enumerator2.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair13.Key))
				{
					string text9 = text7;
					KeyValuePair<string, double> keyValuePair14 = enumerator2.Current;
					text7 = text9 + (keyValuePair14.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text10 = text7;
					KeyValuePair<string, double> keyValuePair15 = enumerator2.Current;
					text7 = text10 + keyValuePair15.Value.ToString("f0") + "\n";
				}
			}
			this._ItemNextLevelTitle[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下一级")
			});
			this._ItemNextLecelContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text6
			});
			this._ItemNextLecelContentValue[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text7
			});
		}
		else
		{
			this._NextLevelRoot[1].SetActive(false);
			this._NextMaxLevelRoot[1].SetActive(true);
		}
	}

	private void InitStar()
	{
		int num = 0;
		if (Global.Data.roleData.MountEquipList != null)
		{
			for (int i = 0; i < Global.Data.roleData.MountEquipList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.MountEquipList[i];
				if (goodsData != null)
				{
					num += goodsData.Forge_level + 1;
				}
			}
		}
		this._ItemTitle[2].spriteName = "JinJieQiYan";
		HorseArrayAdditionVO horseArrayAdditionVO = null;
		HorseArrayAdditionVO horseArrayAdditionVO2 = null;
		Dictionary<int, HorseArrayAdditionVO>.Enumerator horseArrayAdditionEnumerator = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionEnumerator();
		while (horseArrayAdditionEnumerator.MoveNext())
		{
			KeyValuePair<int, HorseArrayAdditionVO> keyValuePair = horseArrayAdditionEnumerator.Current;
			if (keyValuePair.Value.Type == 3)
			{
				int num2 = num;
				KeyValuePair<int, HorseArrayAdditionVO> keyValuePair2 = horseArrayAdditionEnumerator.Current;
				if (num2 >= keyValuePair2.Value.NeedOrderNum)
				{
					KeyValuePair<int, HorseArrayAdditionVO> keyValuePair3 = horseArrayAdditionEnumerator.Current;
					horseArrayAdditionVO = keyValuePair3.Value;
					if (horseArrayAdditionVO.NextID != -1)
					{
						horseArrayAdditionVO2 = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionVOByID(horseArrayAdditionVO.NextID);
					}
					else
					{
						horseArrayAdditionVO2 = null;
					}
				}
			}
		}
		if (horseArrayAdditionVO == null)
		{
			Dictionary<int, HorseArrayAdditionVO>.Enumerator horseArrayAdditionEnumerator2 = IConfigbase<ConfigRidePet>.Instance.GetHorseArrayAdditionEnumerator();
			while (horseArrayAdditionEnumerator2.MoveNext())
			{
				KeyValuePair<int, HorseArrayAdditionVO> keyValuePair4 = horseArrayAdditionEnumerator2.Current;
				if (keyValuePair4.Value.Type == 3)
				{
					if (horseArrayAdditionVO2 != null)
					{
						int needOrderNum = horseArrayAdditionVO2.NeedOrderNum;
						KeyValuePair<int, HorseArrayAdditionVO> keyValuePair5 = horseArrayAdditionEnumerator2.Current;
						if (needOrderNum > keyValuePair5.Value.NeedOrderNum)
						{
							KeyValuePair<int, HorseArrayAdditionVO> keyValuePair6 = horseArrayAdditionEnumerator2.Current;
							horseArrayAdditionVO2 = keyValuePair6.Value;
						}
					}
					else
					{
						KeyValuePair<int, HorseArrayAdditionVO> keyValuePair7 = horseArrayAdditionEnumerator2.Current;
						horseArrayAdditionVO2 = keyValuePair7.Value;
					}
				}
			}
		}
		if (horseArrayAdditionVO == null)
		{
			this._ItemContent[2].text = Global.GetLang("\n \n                  暂无");
			this._ItemContentValue[2].text = string.Empty;
		}
		if (horseArrayAdditionVO != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("总进阶达到：")
			}) + "\n";
			text2 = text2 + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				horseArrayAdditionVO.NeedOrderNum + Global.GetLang("阶 ")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator = horseArrayAdditionVO.AdditionPropsDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text3 = text;
				KeyValuePair<string, double> keyValuePair8 = enumerator.Current;
				text = text3 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair8.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair9 = enumerator.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
				{
					string text4 = text2;
					KeyValuePair<string, double> keyValuePair10 = enumerator.Current;
					text2 = text4 + (keyValuePair10.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text5 = text2;
					KeyValuePair<string, double> keyValuePair11 = enumerator.Current;
					text2 = text5 + keyValuePair11.Value.ToString("f0") + "\n";
				}
			}
			this._ItemContent[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			this._ItemContentValue[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				text2
			});
		}
		if (horseArrayAdditionVO2 != null)
		{
			string text6 = string.Empty;
			string text7 = string.Empty;
			text6 += Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("总进阶达到：") + "\n"
			});
			text7 += Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				horseArrayAdditionVO2.NeedOrderNum + Global.GetLang("阶 ") + "\n"
			});
			Dictionary<string, double>.Enumerator enumerator2 = horseArrayAdditionVO2.AdditionPropsDic.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				string text8 = text6;
				KeyValuePair<string, double> keyValuePair12 = enumerator2.Current;
				text6 = text8 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair13 = enumerator2.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair13.Key))
				{
					string text9 = text7;
					KeyValuePair<string, double> keyValuePair14 = enumerator2.Current;
					text7 = text9 + (keyValuePair14.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text10 = text7;
					KeyValuePair<string, double> keyValuePair15 = enumerator2.Current;
					text7 = text10 + keyValuePair15.Value.ToString("f0") + "\n";
				}
			}
			this._ItemNextLevelTitle[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下一级")
			});
			this._ItemNextLecelContent[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text6
			});
			this._ItemNextLecelContentValue[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text7
			});
		}
		else
		{
			this._NextLevelRoot[2].SetActive(false);
			this._NextMaxLevelRoot[2].SetActive(true);
		}
	}

	public void RefreshDengJiZhuangBei()
	{
		this._ItemTitle[0].spriteName = "QiangHuaQIYuan";
		int num = 0;
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
				if (goodsData != null && goodsData.Using == 1)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
					{
						num += goodsData.Forge_level;
					}
				}
			}
		}
		HorseEquipAddition horseEquipAddition = null;
		HorseEquipAddition horseEquipAddition2 = null;
		foreach (KeyValuePair<int, HorseEquipAddition> keyValuePair in IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition)
		{
			if (keyValuePair.Value.Type == 1)
			{
				int num2 = num;
				Dictionary<int, HorseEquipAddition>.Enumerator enumerator;
				KeyValuePair<int, HorseEquipAddition> keyValuePair2 = enumerator.Current;
				if (num2 >= keyValuePair2.Value.NeedStrengthenLevel)
				{
					KeyValuePair<int, HorseEquipAddition> keyValuePair3 = enumerator.Current;
					horseEquipAddition = keyValuePair3.Value;
					if (horseEquipAddition.NextID != -1)
					{
						if (IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition.ContainsKey(horseEquipAddition.NextID))
						{
							horseEquipAddition2 = IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition[horseEquipAddition.NextID];
						}
					}
					else
					{
						horseEquipAddition2 = null;
					}
				}
			}
		}
		if (horseEquipAddition == null)
		{
			foreach (KeyValuePair<int, HorseEquipAddition> keyValuePair4 in IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition)
			{
				if (keyValuePair4.Value.Type == 1)
				{
					if (horseEquipAddition2 != null)
					{
						int needStrengthenLevel = horseEquipAddition2.NeedStrengthenLevel;
						Dictionary<int, HorseEquipAddition>.Enumerator enumerator2;
						KeyValuePair<int, HorseEquipAddition> keyValuePair5 = enumerator2.Current;
						if (needStrengthenLevel > keyValuePair5.Value.NeedStrengthenLevel)
						{
							KeyValuePair<int, HorseEquipAddition> keyValuePair6 = enumerator2.Current;
							horseEquipAddition2 = keyValuePair6.Value;
						}
					}
					else
					{
						Dictionary<int, HorseEquipAddition>.Enumerator enumerator2;
						KeyValuePair<int, HorseEquipAddition> keyValuePair7 = enumerator2.Current;
						horseEquipAddition2 = keyValuePair7.Value;
					}
				}
			}
		}
		if (horseEquipAddition == null)
		{
			this._ItemContent[0].text = Global.GetLang("\n \n                  暂无");
			this._ItemContentValue[0].text = string.Empty;
		}
		if (horseEquipAddition != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("强化总等级达到：")
			}) + "\n";
			text2 = text2 + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				horseEquipAddition.NeedStrengthenLevel + Global.GetLang("级")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator3 = horseEquipAddition.AdditionPropsDic.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				string text3 = text;
				KeyValuePair<string, double> keyValuePair8 = enumerator3.Current;
				text = text3 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair8.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair9 = enumerator3.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
				{
					string text4 = text2;
					KeyValuePair<string, double> keyValuePair10 = enumerator3.Current;
					text2 = text4 + (keyValuePair10.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text5 = text2;
					KeyValuePair<string, double> keyValuePair11 = enumerator3.Current;
					text2 = text5 + keyValuePair11.Value.ToString("f0") + "\n";
				}
			}
			this._ItemContent[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			this._ItemContentValue[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				text2
			});
		}
		if (horseEquipAddition2 != null)
		{
			string text6 = string.Empty;
			string text7 = string.Empty;
			text6 = text6 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("总等级达到：")
			}) + "\n";
			text7 = text7 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				horseEquipAddition2.NeedStrengthenLevel + Global.GetLang("级")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator4 = horseEquipAddition2.AdditionPropsDic.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				string text8 = text6;
				KeyValuePair<string, double> keyValuePair12 = enumerator4.Current;
				text6 = text8 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair13 = enumerator4.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair13.Key))
				{
					string text9 = text7;
					KeyValuePair<string, double> keyValuePair14 = enumerator4.Current;
					text7 = text9 + (keyValuePair14.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text10 = text7;
					KeyValuePair<string, double> keyValuePair15 = enumerator4.Current;
					text7 = text10 + keyValuePair15.Value.ToString("f0") + "\n";
				}
			}
			this._ItemNextLevelTitle[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下一级")
			});
			this._ItemNextLecelContent[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text6
			});
			this._ItemNextLecelContentValue[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text7
			});
		}
		else
		{
			this._NextLevelRoot[0].SetActive(false);
			this._NextMaxLevelRoot[0].SetActive(true);
		}
	}

	private void InitZhuoYueZuoQiZhuangBei()
	{
		this._ItemTitle[1].spriteName = "ZhuiJiaQiYuan";
		int num = 0;
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
				if (goodsData != null && goodsData.Using == 1)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
					{
						num += goodsData.AppendPropLev;
					}
				}
			}
		}
		HorseEquipAddition horseEquipAddition = null;
		HorseEquipAddition horseEquipAddition2 = null;
		foreach (KeyValuePair<int, HorseEquipAddition> keyValuePair in IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition)
		{
			if (keyValuePair.Value.Type == 2)
			{
				int num2 = num;
				Dictionary<int, HorseEquipAddition>.Enumerator enumerator;
				KeyValuePair<int, HorseEquipAddition> keyValuePair2 = enumerator.Current;
				if (num2 >= keyValuePair2.Value.NeedAdditionLevel)
				{
					KeyValuePair<int, HorseEquipAddition> keyValuePair3 = enumerator.Current;
					horseEquipAddition = keyValuePair3.Value;
					if (horseEquipAddition.NextID != -1 && IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition.ContainsKey(horseEquipAddition.NextID))
					{
						horseEquipAddition2 = IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition[horseEquipAddition.NextID];
					}
					else
					{
						horseEquipAddition2 = null;
					}
				}
			}
		}
		if (horseEquipAddition == null)
		{
			foreach (KeyValuePair<int, HorseEquipAddition> keyValuePair4 in IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition)
			{
				if (keyValuePair4.Value.Type == 2)
				{
					if (horseEquipAddition2 != null)
					{
						int needAdditionLevel = horseEquipAddition2.NeedAdditionLevel;
						Dictionary<int, HorseEquipAddition>.Enumerator enumerator2;
						KeyValuePair<int, HorseEquipAddition> keyValuePair5 = enumerator2.Current;
						if (needAdditionLevel > keyValuePair5.Value.NeedAdditionLevel)
						{
							KeyValuePair<int, HorseEquipAddition> keyValuePair6 = enumerator2.Current;
							horseEquipAddition2 = keyValuePair6.Value;
						}
					}
					else
					{
						Dictionary<int, HorseEquipAddition>.Enumerator enumerator2;
						KeyValuePair<int, HorseEquipAddition> keyValuePair7 = enumerator2.Current;
						horseEquipAddition2 = keyValuePair7.Value;
					}
				}
			}
		}
		if (horseEquipAddition == null)
		{
			this._ItemContent[1].text = Global.GetLang("\n \n                  暂无");
			this._ItemContentValue[1].text = string.Empty;
		}
		if (horseEquipAddition != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("追加总等级达到：")
			}) + "\n";
			text2 = text2 + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				horseEquipAddition.NeedAdditionLevel + Global.GetLang("级")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator3 = horseEquipAddition.AdditionPropsDic.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				string text3 = text;
				KeyValuePair<string, double> keyValuePair8 = enumerator3.Current;
				text = text3 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair8.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair9 = enumerator3.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
				{
					string text4 = text2;
					KeyValuePair<string, double> keyValuePair10 = enumerator3.Current;
					text2 = text4 + (keyValuePair10.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text5 = text2;
					KeyValuePair<string, double> keyValuePair11 = enumerator3.Current;
					text2 = text5 + keyValuePair11.Value.ToString("f0") + "\n";
				}
			}
			this._ItemContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			this._ItemContentValue[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				text2
			});
		}
		if (horseEquipAddition2 != null)
		{
			string text6 = string.Empty;
			string text7 = string.Empty;
			text6 = text6 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("追加总等级达到：")
			}) + "\n";
			text7 = text7 + Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				horseEquipAddition2.NeedAdditionLevel + Global.GetLang("级")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator4 = horseEquipAddition2.AdditionPropsDic.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				string text8 = text6;
				KeyValuePair<string, double> keyValuePair12 = enumerator4.Current;
				text6 = text8 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair13 = enumerator4.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair13.Key))
				{
					string text9 = text7;
					KeyValuePair<string, double> keyValuePair14 = enumerator4.Current;
					text7 = text9 + (keyValuePair14.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text10 = text7;
					KeyValuePair<string, double> keyValuePair15 = enumerator4.Current;
					text7 = text10 + keyValuePair15.Value.ToString("f0") + "\n";
				}
			}
			this._ItemNextLevelTitle[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下一级")
			});
			this._ItemNextLecelContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text6
			});
			this._ItemNextLecelContentValue[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text7
			});
		}
		else
		{
			this._NextLevelRoot[1].SetActive(false);
			this._NextMaxLevelRoot[1].SetActive(true);
		}
	}

	private void InitJieShuZuoQiZhuangBei()
	{
		this._ItemTitle[2].spriteName = "JinJieQiYan";
		int num = 0;
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
				if (goodsData != null && goodsData.Using == 1)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
					{
						num += goodsXmlNodeByID.SuitID;
					}
				}
			}
		}
		HorseEquipAddition horseEquipAddition = null;
		HorseEquipAddition horseEquipAddition2 = null;
		foreach (KeyValuePair<int, HorseEquipAddition> keyValuePair in IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition)
		{
			if (keyValuePair.Value.Type == 3)
			{
				int num2 = num;
				Dictionary<int, HorseEquipAddition>.Enumerator enumerator;
				KeyValuePair<int, HorseEquipAddition> keyValuePair2 = enumerator.Current;
				if (num2 >= keyValuePair2.Value.NeedOrderNum)
				{
					KeyValuePair<int, HorseEquipAddition> keyValuePair3 = enumerator.Current;
					horseEquipAddition = keyValuePair3.Value;
					if (horseEquipAddition.NextID != -1)
					{
						if (IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition.ContainsKey(horseEquipAddition.NextID))
						{
							horseEquipAddition2 = IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition[horseEquipAddition.NextID];
						}
					}
					else
					{
						horseEquipAddition2 = null;
					}
				}
			}
		}
		if (horseEquipAddition == null)
		{
			foreach (KeyValuePair<int, HorseEquipAddition> keyValuePair4 in IConfigbase<ConfigRidePet>.Instance.DicHorseEquipAddition)
			{
				if (keyValuePair4.Value.Type == 3)
				{
					if (horseEquipAddition2 != null)
					{
						int needOrderNum = horseEquipAddition2.NeedOrderNum;
						Dictionary<int, HorseEquipAddition>.Enumerator enumerator2;
						KeyValuePair<int, HorseEquipAddition> keyValuePair5 = enumerator2.Current;
						if (needOrderNum > keyValuePair5.Value.NeedOrderNum)
						{
							KeyValuePair<int, HorseEquipAddition> keyValuePair6 = enumerator2.Current;
							horseEquipAddition2 = keyValuePair6.Value;
						}
					}
					else
					{
						Dictionary<int, HorseEquipAddition>.Enumerator enumerator2;
						KeyValuePair<int, HorseEquipAddition> keyValuePair7 = enumerator2.Current;
						horseEquipAddition2 = keyValuePair7.Value;
					}
				}
			}
		}
		if (horseEquipAddition == null)
		{
			this._ItemContent[2].text = Global.GetLang("\n \n                  暂无");
			this._ItemContentValue[2].text = string.Empty;
		}
		if (horseEquipAddition != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("阶数总等级达到：")
			}) + "\n";
			text2 = text2 + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				horseEquipAddition.NeedOrderNum + Global.GetLang("级 ")
			}) + "\n";
			Dictionary<string, double>.Enumerator enumerator3 = horseEquipAddition.AdditionPropsDic.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				string text3 = text;
				KeyValuePair<string, double> keyValuePair8 = enumerator3.Current;
				text = text3 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair8.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair9 = enumerator3.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
				{
					string text4 = text2;
					KeyValuePair<string, double> keyValuePair10 = enumerator3.Current;
					text2 = text4 + (keyValuePair10.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text5 = text2;
					KeyValuePair<string, double> keyValuePair11 = enumerator3.Current;
					text2 = text5 + keyValuePair11.Value.ToString("f0") + "\n";
				}
			}
			this._ItemContent[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			this._ItemContentValue[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				text2
			});
		}
		if (horseEquipAddition2 != null)
		{
			string text6 = string.Empty;
			string text7 = string.Empty;
			text6 += Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("总进阶达到：") + "\n"
			});
			text7 += Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				horseEquipAddition2.NeedOrderNum + Global.GetLang("阶 ") + "\n"
			});
			Dictionary<string, double>.Enumerator enumerator4 = horseEquipAddition2.AdditionPropsDic.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				string text8 = text6;
				KeyValuePair<string, double> keyValuePair12 = enumerator4.Current;
				text6 = text8 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true) + ":\n";
				KeyValuePair<string, double> keyValuePair13 = enumerator4.Current;
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair13.Key))
				{
					string text9 = text7;
					KeyValuePair<string, double> keyValuePair14 = enumerator4.Current;
					text7 = text9 + (keyValuePair14.Value * 100.0).ToString("f1") + "%\n";
				}
				else
				{
					string text10 = text7;
					KeyValuePair<string, double> keyValuePair15 = enumerator4.Current;
					text7 = text10 + keyValuePair15.Value.ToString("f0") + "\n";
				}
			}
			this._ItemNextLevelTitle[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("下一级")
			});
			this._ItemNextLecelContent[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text6
			});
			this._ItemNextLecelContentValue[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text7
			});
		}
		else
		{
			this._NextLevelRoot[2].SetActive(false);
			this._NextMaxLevelRoot[2].SetActive(true);
		}
	}

	public void RefreshPanel(RidePetUIType type)
	{
		this._NextMaxLevelRoot[0].SetActive(false);
		this._NextMaxLevelRoot[1].SetActive(false);
		this._NextMaxLevelRoot[2].SetActive(false);
		if (type == RidePetUIType.ZuoQi)
		{
			this.InitPrefabText();
			this.InitTexture();
			GameInstance.Game.GetRidePetMainData();
			this.InitHandler();
			this.InitTianFu();
			this.InitStar();
		}
		else if (type == RidePetUIType.ZhuangBei)
		{
			this.InitPrefabText();
			this._Title.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("熔炼加成")
			});
			this.InitHandler();
			this.InitTexture();
			this.RefreshDengJiZhuangBei();
			this.InitZhuoYueZuoQiZhuangBei();
			this.InitJieShuZuoQiZhuangBei();
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UILabel _Title;

	[SerializeField]
	private UISprite[] _ItemTitle;

	[SerializeField]
	private UILabel[] _ItemContent;

	[SerializeField]
	private UILabel[] _ItemContentValue;

	[SerializeField]
	private UILabel[] _ItemNextLevelTitle;

	[SerializeField]
	private UILabel[] _ItemNextLecelContent;

	[SerializeField]
	private UILabel[] _ItemNextLecelContentValue;

	[SerializeField]
	private ShowNetImage[] _ItemBakImage;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GameObject[] _NextLevelRoot;

	[SerializeField]
	private GameObject[] _NextMaxLevelRoot;
}
