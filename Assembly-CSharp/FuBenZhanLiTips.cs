using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class FuBenZhanLiTips : UserControl
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

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mLblTitle.Text = Global.GetLang("副本提示");
		this.mLblTipsContent.Text = Global.GetLang("当前战斗力，挑战该副本难度较大\n建议提升战斗力后再来挑战");
		this.mLblTipsRecommandTitle.Text = Global.GetLang("推荐提升战斗力方式");
		this.mLblTipsRecommandZhanDouLi.Text = Global.GetLang("建议战斗力：");
		this.mBtnOk.Text = Global.GetLang("强行进入");
		this.mBtnCancel.Text = Global.GetLang("稍后再来");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnOk.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 1
				});
			}
		};
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
		this.mListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			GameObject selectedItem = this.mListBox.SelectedItem;
			if (null != selectedItem)
			{
				FuBenZhanLiTipsItem component = selectedItem.GetComponent<FuBenZhanLiTipsItem>();
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = component.ItemVO.LinkID
				});
				if (this.CloseHandler != null)
				{
					this.CloseHandler(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			}
		};
		this.BianQiangXML = Global.GetGameResXml("BianQiang");
		this.BianQiangList = Global.GetXElementList(this.BianQiangXML, "BianQiang");
		this.BianQiangStandardXML = Global.GetGameResXml("Standard");
		this.BianQiangStandardList = Global.GetXElementList(this.BianQiangStandardXML, "BianQiang");
	}

	public void InitZhanLiValue(int zhanLi)
	{
		this.mLblTipsRecommandZhanDouLiValue.Text = zhanLi.ToString();
		this.StartRequest();
	}

	public void InitRecommandWay()
	{
	}

	private List<TiShengZhanLiItemVO> GetDataList(int index)
	{
		List<TiShengZhanLiItemVO> list = null;
		if (index == 0)
		{
			if (this.JiXuListData != null)
			{
				return this.JiXuListData;
			}
			this.JiXuListData = new List<TiShengZhanLiItemVO>();
			list = this.JiXuListData;
		}
		else if (index == 1)
		{
			if (this.YouDaiListData != null)
			{
				return this.YouDaiListData;
			}
			this.YouDaiListData = new List<TiShengZhanLiItemVO>();
			list = this.YouDaiListData;
		}
		else if (index == 2)
		{
			if (this.WanMeiListData != null)
			{
				return this.WanMeiListData;
			}
			this.WanMeiListData = new List<TiShengZhanLiItemVO>();
			list = this.WanMeiListData;
		}
		int count = this.BianQiangList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.BianQiangList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "PlaceType");
			if (xelementAttributeInt2 == 1)
			{
				int level = Global.Data.roleData.Level;
				int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Parameter");
				string[] array = xelementAttributeStr.Split(new char[]
				{
					','
				});
				int num = int.Parse(array[0]);
				int num2 = int.Parse(array[1]);
				if (changeLifeCount * 100 + level >= num * 100 + num2)
				{
					TiShengZhanLiItemVO itemVO = this.GetItemVO(xelementAttributeInt, xelement);
					if (itemVO != null)
					{
						if (index == 0)
						{
							if (itemVO.percent <= 0.5f)
							{
								list.Add(itemVO);
							}
						}
						else if (index == 1)
						{
							if (itemVO.percent > 0.5f && itemVO.percent < 1f)
							{
								list.Add(itemVO);
							}
						}
						else if (index == 2 && itemVO.percent == 1f)
						{
							list.Add(itemVO);
						}
					}
				}
			}
			else if (xelementAttributeInt2 == 2)
			{
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Parameter");
				int completedMainTaskID = Global.Data.roleData.CompletedMainTaskID;
				if (completedMainTaskID >= xelementAttributeInt3)
				{
					TiShengZhanLiItemVO itemVO2 = this.GetItemVO(xelementAttributeInt, xelement);
					if (itemVO2 != null)
					{
						if (index == 0)
						{
							if (itemVO2.percent <= 0.5f)
							{
								list.Add(itemVO2);
							}
						}
						else if (index == 1)
						{
							if (itemVO2.percent > 0.5f && itemVO2.percent < 1f)
							{
								list.Add(itemVO2);
							}
						}
						else if (index == 2 && itemVO2.percent == 1f)
						{
							list.Add(itemVO2);
						}
					}
				}
			}
			else if (xelementAttributeInt2 == 3)
			{
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Parameter");
				string[] array2 = xelementAttributeStr2.Split(new char[]
				{
					','
				});
				if ((long)Global.Data.roleData.MyWingData.WingID >= (long)((ulong)Convert.ToUInt32(array2[0])))
				{
					TiShengZhanLiItemVO itemVO3 = this.GetItemVO(xelementAttributeInt, xelement);
					if (itemVO3 != null)
					{
						if (index == 0)
						{
							if (itemVO3.percent <= 0.5f)
							{
								list.Add(itemVO3);
							}
						}
						else if (index == 1)
						{
							if (itemVO3.percent > 0.5f && itemVO3.percent < 1f)
							{
								list.Add(itemVO3);
							}
						}
						else if (index == 2 && itemVO3.percent == 1f)
						{
							list.Add(itemVO3);
						}
					}
				}
			}
			else if (xelementAttributeInt2 == 4 && Global.GetChengJiuLevel(0) >= 4)
			{
				TiShengZhanLiItemVO itemVO4 = this.GetItemVO(xelementAttributeInt, xelement);
				if (itemVO4 != null)
				{
					if (index == 0)
					{
						if (itemVO4.percent <= 0.5f)
						{
							list.Add(itemVO4);
						}
					}
					else if (index == 1)
					{
						if (itemVO4.percent > 0.5f && itemVO4.percent < 1f)
						{
							list.Add(itemVO4);
						}
					}
					else if (index == 2 && itemVO4.percent == 1f)
					{
						list.Add(itemVO4);
					}
				}
			}
			else if (xelementAttributeInt2 == 5 && Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel) >= 4)
			{
				TiShengZhanLiItemVO itemVO5 = this.GetItemVO(xelementAttributeInt, xelement);
				if (itemVO5 != null)
				{
					if (index == 0)
					{
						if (itemVO5.percent <= 0.5f)
						{
							list.Add(itemVO5);
						}
					}
					else if (index == 1)
					{
						if (itemVO5.percent > 0.5f && itemVO5.percent < 1f)
						{
							list.Add(itemVO5);
						}
					}
					else if (index == 2 && itemVO5.percent == 1f)
					{
						list.Add(itemVO5);
					}
				}
			}
		}
		return list;
	}

	private TiShengZhanLiItemVO GetItemVO(int ID, XElement item)
	{
		TiShengZhanLiItemVO tiShengZhanLiItemVO = new TiShengZhanLiItemVO();
		XElement bianQiangStandardItemXML = this.GetBianQiangStandardItemXML(ID);
		tiShengZhanLiItemVO.ID = ID;
		tiShengZhanLiItemVO.ImageName = Global.GetXElementAttributeStr(item, "Image");
		tiShengZhanLiItemVO.LinkID = Global.GetXElementAttributeInt(item, "Link");
		if (bianQiangStandardItemXML == null)
		{
			return null;
		}
		tiShengZhanLiItemVO.Name = Global.GetXElementAttributeStr(bianQiangStandardItemXML, "Name");
		tiShengZhanLiItemVO.StandardValue = Global.GetXElementAttributeInt(bianQiangStandardItemXML, "Value");
		tiShengZhanLiItemVO.ItemType = Global.GetXElementAttributeInt(bianQiangStandardItemXML, "Type");
		float num = 0f;
		if (tiShengZhanLiItemVO.ItemType == 1)
		{
			WingData myWingData = Global.Data.roleData.MyWingData;
			float num2 = (float)myWingData.ForgeLevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num2;
		}
		else if (tiShengZhanLiItemVO.ItemType == 2)
		{
			int wingID = Global.Data.roleData.MyWingData.WingID;
			float num3 = (float)wingID / (float)tiShengZhanLiItemVO.StandardValue;
			num = num3;
		}
		else if (tiShengZhanLiItemVO.ItemType == 3)
		{
			float averageSkillLevel = this.GetAverageSkillLevel();
			num = averageSkillLevel / (float)tiShengZhanLiItemVO.StandardValue;
		}
		else if (tiShengZhanLiItemVO.ItemType == 4)
		{
			float avergeEquipJieShu = this.GetAvergeEquipJieShu();
			num = avergeEquipJieShu / (float)tiShengZhanLiItemVO.StandardValue;
		}
		else if (tiShengZhanLiItemVO.ItemType == 5)
		{
			float avergeEquipQiangHuaLevel = this.GetAvergeEquipQiangHuaLevel();
			num = avergeEquipQiangHuaLevel / (float)tiShengZhanLiItemVO.StandardValue;
		}
		else if (tiShengZhanLiItemVO.ItemType == 6)
		{
			float avergeEquipZhuiJia = this.GetAvergeEquipZhuiJia();
			num = avergeEquipZhuiJia / (float)tiShengZhanLiItemVO.StandardValue;
		}
		else if (tiShengZhanLiItemVO.ItemType == 7)
		{
			float num4 = (float)Global.GetHuShengFuLevel();
			num = num4 / (float)tiShengZhanLiItemVO.StandardValue;
		}
		else if (tiShengZhanLiItemVO.ItemType == 8)
		{
			int viplevel = Global.Data.roleData.VIPLevel;
			float num5 = (float)viplevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num5;
		}
		else if (tiShengZhanLiItemVO.ItemType == 9)
		{
			int chengJiuLevel = Global.GetChengJiuLevel(0);
			float num6 = (float)chengJiuLevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num6;
		}
		else if (tiShengZhanLiItemVO.ItemType == 10)
		{
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
			float num7 = (float)roleCommonUseParamsValue / (float)tiShengZhanLiItemVO.StandardValue;
			num = num7;
		}
		else if (tiShengZhanLiItemVO.ItemType == 11)
		{
			int opne_XIN_HUN_COUNT = TiShengZhanLiPart.OPNE_XIN_HUN_COUNT;
			float num8 = (float)opne_XIN_HUN_COUNT / (float)tiShengZhanLiItemVO.StandardValue;
			num = num8;
		}
		else if (tiShengZhanLiItemVO.ItemType == 12)
		{
			int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
			float num9 = (float)changeLifeCount / (float)tiShengZhanLiItemVO.StandardValue;
			num = num9;
		}
		else if (tiShengZhanLiItemVO.ItemType == 13)
		{
			int sumOfAllEquipXilianValue = Global.GetSumOfAllEquipXilianValue();
			float num10 = (float)sumOfAllEquipXilianValue / (float)tiShengZhanLiItemVO.StandardValue;
			num = num10;
		}
		else if (tiShengZhanLiItemVO.ItemType == 14)
		{
			int petBattlingLevel = this.GetPetBattlingLevel();
			float num11 = (float)petBattlingLevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num11;
		}
		else if (tiShengZhanLiItemVO.ItemType == 15)
		{
			float num12 = (float)Global.GetSumOfAllUsedYuansuLevel();
			float num13 = num12 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num13;
		}
		else if (tiShengZhanLiItemVO.ItemType == 16)
		{
			float num14 = (float)this.lingyuTotalLevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num14;
		}
		else if (tiShengZhanLiItemVO.ItemType == 17)
		{
			float num15 = (float)Global.Data.roleData.MyWingData.ZhuLingNum;
			float num16 = num15 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num16 + 0.0001f;
		}
		else if (tiShengZhanLiItemVO.ItemType == 18)
		{
			float num17 = (float)Global.Data.roleData.MyWingData.ZhuHunNum;
			float num18 = num17 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num18 + 0.0001f;
		}
		else if (tiShengZhanLiItemVO.ItemType == 19)
		{
			float fuWenLv = Global.GetFuWenLv();
			float num19 = fuWenLv / (float)tiShengZhanLiItemVO.StandardValue;
			num = num19;
		}
		else if (tiShengZhanLiItemVO.ItemType == 20)
		{
			float shengWangXunZhangLv = Global.GetShengWangXunZhangLv();
			float num20 = shengWangXunZhangLv / (float)tiShengZhanLiItemVO.StandardValue;
			num = num20;
		}
		else if (tiShengZhanLiItemVO.ItemType == 21)
		{
			float num21 = (float)this.DataBagMerlinGrowthSaveDBData._Level;
			float num22 = num21 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num22;
		}
		else if (tiShengZhanLiItemVO.ItemType == 22)
		{
			float num23 = (float)this.GetGuardStatueGrade();
			float num24 = num23 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num24;
		}
		else if (tiShengZhanLiItemVO.ItemType == 23)
		{
			float num25 = (float)Global.GetFluorescentDiamondTotalLevel();
			float num26 = num25 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num26;
		}
		else if (tiShengZhanLiItemVO.ItemType == 24)
		{
			float shengWuTotalLevel = Global.GetShengWuTotalLevel();
			float num27 = shengWuTotalLevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num27;
		}
		else if (tiShengZhanLiItemVO.ItemType == 25)
		{
			float shenDianLevel = this.GetShenDianLevel();
			float num28 = shenDianLevel / (float)tiShengZhanLiItemVO.StandardValue;
			num = num28;
		}
		else if (tiShengZhanLiItemVO.ItemType == 26)
		{
			float num29 = (float)this.GetRoleUsingJInglingSkillAllLev();
			float num30 = num29 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num30;
		}
		else if (tiShengZhanLiItemVO.ItemType == 27)
		{
			float num31 = (float)Global.Data.taLuoPaiLevel;
			float num32 = num31 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num32;
		}
		else if (tiShengZhanLiItemVO.ItemType == 28)
		{
			if (this.shenQiData != null)
			{
				float num33 = (float)(this.shenQiData.LifeAdd + this.shenQiData.DefenseAdd + this.shenQiData.ToughnessAdd + this.shenQiData.AttackAdd);
				ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(this.shenQiData.ShenQiID);
				float num34 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
				bool flag = num34 == num33;
				float num35 = (float)((!flag) ? (this.shenQiData.ShenQiID - 1) : this.shenQiData.ShenQiID);
				float num36 = num35 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num36;
			}
		}
		else if (tiShengZhanLiItemVO.ItemType == 29)
		{
			float num37 = 0f;
			if (Global.Data.roleData.ShenJiDict != null && Global.Data.roleData.ShenJiDict.Count != 0)
			{
				Dictionary<int, ShenJiFuWenData>.Enumerator enumerator = Global.Data.roleData.ShenJiDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Dictionary<int, ShenJiFuWen> dicShenJiFuWen = SpiritTrackPart.GetDicShenJiFuWen();
					KeyValuePair<int, ShenJiFuWenData> keyValuePair = enumerator.Current;
					if (dicShenJiFuWen.ContainsKey(keyValuePair.Value.ID))
					{
						float num38 = num37;
						Dictionary<int, ShenJiFuWen> dicShenJiFuWen2 = SpiritTrackPart.GetDicShenJiFuWen();
						KeyValuePair<int, ShenJiFuWenData> keyValuePair2 = enumerator.Current;
						float upNeed = (float)dicShenJiFuWen2[keyValuePair2.Value.ID].UpNeed;
						KeyValuePair<int, ShenJiFuWenData> keyValuePair3 = enumerator.Current;
						num37 = num38 + upNeed * (float)keyValuePair3.Value.Level;
					}
				}
			}
			num37 += (float)Global.Data.roleData.RoleCommonUseIntPamams[46];
			float num39 = num37 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num39;
		}
		else if (tiShengZhanLiItemVO.ItemType == 30)
		{
			float num40 = (float)Global.SetEmbelemUpLevel;
			float num41 = num40 / (float)tiShengZhanLiItemVO.StandardValue;
			num = num41;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		else if (num > 1f)
		{
			num = 1f;
		}
		tiShengZhanLiItemVO.percent = num;
		return tiShengZhanLiItemVO;
	}

	private float GetAvergeEquipJieShu()
	{
		int num = 0;
		int num2 = 0;
		if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
		{
			Global.GetUsingGoodsDataList();
		}
		Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
		foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
		{
			GoodsData value = keyValuePair.Value;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
			if (goodsXmlNodeByID != null && (goodsXmlNodeByID.Categoriy == 0 || goodsXmlNodeByID.Categoriy == 1 || goodsXmlNodeByID.Categoriy == 2 || goodsXmlNodeByID.Categoriy == 3 || goodsXmlNodeByID.Categoriy == 4 || goodsXmlNodeByID.Categoriy == 5 || goodsXmlNodeByID.Categoriy == 6 || goodsXmlNodeByID.Categoriy == 11 || goodsXmlNodeByID.Categoriy == 12 || goodsXmlNodeByID.Categoriy == 13 || goodsXmlNodeByID.Categoriy == 14 || goodsXmlNodeByID.Categoriy == 15 || goodsXmlNodeByID.Categoriy == 16 || goodsXmlNodeByID.Categoriy == 17 || goodsXmlNodeByID.Categoriy == 18 || goodsXmlNodeByID.Categoriy == 19 || goodsXmlNodeByID.Categoriy == 21))
			{
				num += goodsXmlNodeByID.SuitID;
				num2++;
			}
		}
		float result = 0f;
		if (num2 > 0)
		{
			result = (float)(num / num2);
		}
		return result;
	}

	private float GetAvergeEquipQiangHuaLevel()
	{
		int num = 0;
		int num2 = 0;
		if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
		{
			Global.GetUsingGoodsDataList();
		}
		Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
		foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
		{
			GoodsData value = keyValuePair.Value;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
			if (goodsXmlNodeByID != null && (goodsXmlNodeByID.Categoriy == 0 || goodsXmlNodeByID.Categoriy == 1 || goodsXmlNodeByID.Categoriy == 2 || goodsXmlNodeByID.Categoriy == 3 || goodsXmlNodeByID.Categoriy == 4 || goodsXmlNodeByID.Categoriy == 5 || goodsXmlNodeByID.Categoriy == 6 || goodsXmlNodeByID.Categoriy == 11 || goodsXmlNodeByID.Categoriy == 12 || goodsXmlNodeByID.Categoriy == 13 || goodsXmlNodeByID.Categoriy == 14 || goodsXmlNodeByID.Categoriy == 15 || goodsXmlNodeByID.Categoriy == 16 || goodsXmlNodeByID.Categoriy == 17 || goodsXmlNodeByID.Categoriy == 18 || goodsXmlNodeByID.Categoriy == 19 || goodsXmlNodeByID.Categoriy == 21))
			{
				num += value.Forge_level;
				num2++;
			}
		}
		float result = 0f;
		if (num2 > 0)
		{
			result = (float)(num / num2);
		}
		return result;
	}

	private float GetAvergeEquipZhuiJia()
	{
		int num = 0;
		int num2 = 0;
		if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
		{
			Global.GetUsingGoodsDataList();
		}
		Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
		foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
		{
			GoodsData value = keyValuePair.Value;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
			if (goodsXmlNodeByID != null && (goodsXmlNodeByID.Categoriy == 0 || goodsXmlNodeByID.Categoriy == 1 || goodsXmlNodeByID.Categoriy == 2 || goodsXmlNodeByID.Categoriy == 3 || goodsXmlNodeByID.Categoriy == 4 || goodsXmlNodeByID.Categoriy == 5 || goodsXmlNodeByID.Categoriy == 6 || goodsXmlNodeByID.Categoriy == 11 || goodsXmlNodeByID.Categoriy == 12 || goodsXmlNodeByID.Categoriy == 13 || goodsXmlNodeByID.Categoriy == 14 || goodsXmlNodeByID.Categoriy == 15 || goodsXmlNodeByID.Categoriy == 16 || goodsXmlNodeByID.Categoriy == 17 || goodsXmlNodeByID.Categoriy == 18 || goodsXmlNodeByID.Categoriy == 19 || goodsXmlNodeByID.Categoriy == 21))
			{
				num += value.AppendPropLev;
				num2++;
			}
		}
		float result = 0f;
		if (num2 > 0)
		{
			result = (float)(num / num2);
		}
		return result;
	}

	private float GetAverageSkillLevel()
	{
		if (Global.Data.roleData.SkillDataList == null)
		{
			return 0f;
		}
		int num = 0;
		int num2 = 0;
		List<MagicInfoVO> skillListByOccupation = Global.GetSkillListByOccupation(Global.Data.roleData.Occupation);
		for (int i = 0; i < skillListByOccupation.Count; i++)
		{
			MagicInfoVO magicInfoVO = skillListByOccupation[i];
			int id = magicInfoVO.ID;
			int magicIcon = magicInfoVO.MagicIcon;
			int actionIndex = magicInfoVO.ActionIndex;
			if (actionIndex < 1000)
			{
				SkillData skillDataByID = Global.GetSkillDataByID(id);
				if (skillDataByID != null && skillDataByID.SkillLevel > 0)
				{
					num += skillDataByID.SkillLevel;
					num2++;
				}
			}
		}
		float result = 0f;
		if (num2 > 0)
		{
			result = (float)(num / num2);
		}
		return result;
	}

	private int GetGuardStatueGrade()
	{
		if (this.guardStatue == null)
		{
			return 1;
		}
		return this.guardStatue.grade;
	}

	private int GetRoleUsingJInglingSkillAllLev()
	{
		if (Global.Data.equipPet != null)
		{
			int count = Global.Data.equipPet.Count;
			int i = 0;
			while (i < count)
			{
				GoodsData goodsData = Global.Data.equipPet[i];
				if (goodsData.Using == 1)
				{
					if (goodsData.ElementhrtsProps != null)
					{
						int num = 0;
						int num2 = 0;
						for (int j = 0; j < goodsData.ElementhrtsProps.Count; j++)
						{
							if (j % 3 == 1)
							{
								int num3 = goodsData.ElementhrtsProps[j];
								if (1 > num3)
								{
									num3 = 1;
								}
								num += num3;
								num2 = num3;
							}
							if (j % 3 == 2)
							{
								if (0 >= goodsData.ElementhrtsProps[j])
								{
									num -= num2;
								}
								num2 = 0;
							}
						}
						return num;
					}
					return 0;
				}
				else
				{
					i++;
				}
			}
		}
		return 0;
	}

	private int GetPetBattlingLevel()
	{
		int result = 0;
		if (Global.Data.equipPet != null)
		{
			int count = Global.Data.equipPet.Count;
			for (int i = 0; i < count; i++)
			{
				GoodsData goodsData = Global.Data.equipPet[i];
				if (goodsData != null && goodsData.Using == 1)
				{
					result = goodsData.Forge_level + 1;
					break;
				}
			}
		}
		return result;
	}

	private XElement GetBianQiangStandardItemXML(int id)
	{
		XElement result = null;
		int count = this.BianQiangStandardList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.BianQiangStandardList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			if (xelementAttributeInt == id)
			{
				int level = Global.Data.roleData.Level;
				int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinZhuanShengLevel");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinLevel");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxZhuanShengLevel");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
				if (xelementAttributeInt2 * 100 + xelementAttributeInt3 <= changeLifeCount * 100 + level && changeLifeCount * 100 + level <= xelementAttributeInt4 * 100 + xelementAttributeInt5)
				{
					result = xelement;
					break;
				}
			}
		}
		return result;
	}

	private void GetLingyuTotalLevel()
	{
		GameInstance.Game.GetLingyuList();
	}

	public void CountLingyuTotalLevel(List<LingYuData> lingyuList)
	{
		for (int i = 0; i < lingyuList.Count; i++)
		{
			this.lingyuTotalLevel += lingyuList[i].Suit;
		}
	}

	private float GetShenDianLevel()
	{
		if (this.CurrentData == null)
		{
			return 0f;
		}
		return (float)this.CurrentData.StatueLevel;
	}

	private void StartRequest()
	{
		Super.ShowNetWaiting(null);
		this.GetLingyuTotalLevel();
		GameInstance.Game.SpriteQueryStarConstelltionInfoCmd();
		this.requstRetainCount++;
		GameInstance.Game.FuWenChengJiuInfo();
		this.requstRetainCount++;
		GameInstance.Game.GetAdendaInfo();
		this.requstRetainCount++;
		GameInstance.Game.GetGuardStatueInfo();
		this.requstRetainCount++;
		GameInstance.Game.GetGetTaLuopaiData();
		GameInstance.Game.SpritQueryYuansuBagByUsed(3001);
		if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MeiLanZhiShu))
		{
			TCPGameServerCmds.CMD_SPR_MERLIN_QUERY.SendDataUseRoleID();
			this.requstRetainCount++;
		}
		GameInstance.Game.SendShenDianInfo();
		this.requstRetainCount++;
		GameInstance.Game.SendGetShenQiDataRequest();
		this.requstRetainCount++;
	}

	public void RecvShenDianData(UnionPalaceData Data)
	{
		this.CurrentData = Data;
		this.OnRequestCallBack_CheckPending();
	}

	public void RecvShenQiData(ShenQiData Data)
	{
		this.shenQiData = Data;
		this.OnRequestCallBack_CheckPending();
	}

	public void OnRequestCallBack_QueryStar(Dictionary<int, int> dic)
	{
		if (dic != null)
		{
			this.m_DicActiveStar = dic;
			FuBenZhanLiTips.OPNE_XIN_HUN_COUNT = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.m_DicActiveStar)
			{
				FuBenZhanLiTips.OPNE_XIN_HUN_COUNT += keyValuePair.Value;
			}
		}
		this.OnRequestCallBack_CheckPending();
	}

	public void OnRequestCallBack_QueryPrestige()
	{
		this.OnRequestCallBack_CheckPending();
	}

	public void OnRequestCallBack_QueryChengJiu()
	{
		this.OnRequestCallBack_CheckPending();
	}

	public void OnRequestCallBack_GuardStatue(GuardStatueData statueData)
	{
		this.guardStatue = statueData;
		this.OnRequestCallBack_CheckPending();
	}

	public void OnRequestCallBack_MerLin(MerlinGrowthSaveDBData DataBag)
	{
		this.DataBagMerlinGrowthSaveDBData = DataBag;
		this.OnRequestCallBack_CheckPending();
	}

	public void OnRequestCallBack_CheckPending()
	{
		this.requstRetainCount--;
		if (this.requstRetainCount > 0)
		{
			return;
		}
		Super.HideNetWaiting();
		this.sumList.Clear();
		List<TiShengZhanLiItemVO> dataList = this.GetDataList(0);
		List<TiShengZhanLiItemVO> dataList2 = this.GetDataList(1);
		this.sumList.AddRange(dataList);
		this.sumList.AddRange(dataList2);
		for (int i = 0; i < this.sumList.Count; i++)
		{
			if (i >= 4)
			{
				break;
			}
			GameObject gameObject = NGUITools.AddChild(this.mListBox.gameObject, this.Item);
			gameObject.SetActive(true);
			FuBenZhanLiTipsItem component = gameObject.GetComponent<FuBenZhanLiTipsItem>();
			component.Img = this.sumList[i].ImageName;
			component.ItemVO = this.sumList[i];
			this.ItemCollection.Add(gameObject);
		}
	}

	protected override void OnDestroy()
	{
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblTitle;

	public TextBlock mLblTipsContent;

	public TextBlock mLblTipsRecommandTitle;

	public TextBlock mLblTipsRecommandZhanDouLi;

	public TextBlock mLblTipsRecommandZhanDouLiValue;

	public GButton mBtnOk;

	public GButton mBtnCancel;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public GameObject Item;

	private ShenQiData shenQiData;

	private List<TiShengZhanLiItemVO> JiXuListData;

	private List<TiShengZhanLiItemVO> YouDaiListData;

	private List<TiShengZhanLiItemVO> WanMeiListData;

	private XElement BianQiangXML;

	private List<XElement> BianQiangList;

	private List<XElement> BianQiangStandardList;

	private XElement BianQiangStandardXML;

	private MerlinGrowthSaveDBData DataBagMerlinGrowthSaveDBData = new MerlinGrowthSaveDBData();

	private GuardStatueData guardStatue;

	private UnionPalaceData CurrentData;

	private int lingyuTotalLevel;

	private int requstRetainCount;

	private Dictionary<int, int> m_DicActiveStar = new Dictionary<int, int>();

	public static int OPNE_XIN_HUN_COUNT;

	private List<TiShengZhanLiItemVO> sumList = new List<TiShengZhanLiItemVO>();
}
