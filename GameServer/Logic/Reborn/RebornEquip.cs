using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.Reborn
{
	public class RebornEquip
	{
		public static RebornEquip getInstance()
		{
			return RebornEquip.instance;
		}

		public static bool ParseRebornEquipConfig()
		{
			Dictionary<int, RebornEquipXmlStruct> dictionary = new Dictionary<int, RebornEquipXmlStruct>();
			string text = Global.GameResPath(RebornEquipConst.RebornEquip);
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "GoodsID"));
					if (RebornEquip.IsRebornEquipShengWu(num) || RebornEquip.IsRebornEquipShengQi(num))
					{
						dictionary.Add(num, new RebornEquipXmlStruct
						{
							ZSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ZSMod")),
							FSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "FSMod")),
							GJSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "GSMOd")),
							LMJSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "LMJSMod")),
							FMJSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "FMJSMod")),
							ZHSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ZHSMod"))
						});
					}
				}
				RebornEquip.EquipSQSW = dictionary;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (RebornEquip.EquipSQSW == null)
			{
				result = false;
			}
			else
			{
				text = Global.GameResPath(RebornEquipConst.RebornEquipEvolution);
				xelement = XElement.Load(text);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
				}
				try
				{
					Dictionary<int, RebornEquipEvolution> dictionary2 = new Dictionary<int, RebornEquipEvolution>();
					Dictionary<int, RebornEquipEvolution> dictionary3 = new Dictionary<int, RebornEquipEvolution>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						RebornEquipEvolution rebornEquipEvolution = new RebornEquipEvolution();
						rebornEquipEvolution.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
						rebornEquipEvolution.NewEquitID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "NewEquitID"));
						rebornEquipEvolution.NeedEquitID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "NeedEquitID"));
						rebornEquipEvolution.NeedCuiLian = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "NeedCuiLian"));
						rebornEquipEvolution.NeedDuanZao = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "NeedDuanZao"));
						rebornEquipEvolution.NeedNiePan = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "NeedNiePan"));
						rebornEquipEvolution.SuccessRate = Convert.ToDouble(Global.GetSafeAttributeStr(xml, "SuccessRate"));
						dictionary2.Add(rebornEquipEvolution.ID, rebornEquipEvolution);
						dictionary3.Add(rebornEquipEvolution.NeedEquitID, rebornEquipEvolution);
					}
					RebornEquip.Evolution = dictionary2;
					RebornEquip.RebornEquipUp = dictionary3;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				if (RebornEquip.Evolution == null && RebornEquip.RebornEquipUp == null)
				{
					result = false;
				}
				else
				{
					text = Global.GameResPath(RebornEquipConst.RebornSuperiorDrop);
					xelement = XElement.Load(text);
					if (null == xelement)
					{
						LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
					}
					try
					{
						Dictionary<int, RebornSuperiorDrop> dictionary4 = new Dictionary<int, RebornSuperiorDrop>();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							RebornSuperiorDrop rebornSuperiorDrop = new RebornSuperiorDrop();
							List<int> list = new List<int>();
							List<int> list2 = new List<int>();
							Dictionary<double, int> dictionary5 = new Dictionary<double, int>();
							Dictionary<double, int> dictionary6 = new Dictionary<double, int>();
							Dictionary<double, int> dictionary7 = new Dictionary<double, int>();
							List<int> list3 = new List<int>();
							List<int> list4 = new List<int>();
							List<int> list5 = new List<int>();
							rebornSuperiorDrop.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xml, "RebornSuperiorInherentNum");
							string[] array = safeAttributeStr.Split(new char[]
							{
								','
							});
							foreach (string text2 in array)
							{
								list.Add(Convert.ToInt32(text2));
								rebornSuperiorDrop.RebornSuperiorInherentNum = list;
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "RebornSuperiorInherentBank");
							string[] array3 = safeAttributeStr2.Split(new char[]
							{
								','
							});
							foreach (string text2 in array3)
							{
								list2.Add(Convert.ToInt32(text2));
								rebornSuperiorDrop.RebornSuperiorInherentBank = list2;
							}
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "RebornSuperiorRate1");
							double num2 = 0.0;
							string[] array4 = safeAttributeStr3.Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array4)
							{
								num2 += Convert.ToDouble(text2.Split(new char[]
								{
									','
								})[1]);
								if (num2 != 0.0)
								{
									dictionary5.Add(num2, Convert.ToInt32(text2.Split(new char[]
									{
										','
									})[0]));
									rebornSuperiorDrop.RebornSuperiorRate1 = dictionary5;
								}
							}
							string safeAttributeStr4 = Global.GetSafeAttributeStr(xml, "RebornSuperiorBank1");
							string[] array5 = safeAttributeStr4.Split(new char[]
							{
								','
							});
							foreach (string text2 in array5)
							{
								list3.Add(Convert.ToInt32(text2));
								rebornSuperiorDrop.RebornSuperiorBank1 = list3;
							}
							string safeAttributeStr5 = Global.GetSafeAttributeStr(xml, "RebornSuperiorRate2");
							double num3 = 0.0;
							string[] array6 = safeAttributeStr5.Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array6)
							{
								num3 += Convert.ToDouble(text2.Split(new char[]
								{
									','
								})[1]);
								if (num3 != 0.0)
								{
									dictionary6.Add(num3, Convert.ToInt32(text2.Split(new char[]
									{
										','
									})[0]));
									rebornSuperiorDrop.RebornSuperiorRate2 = dictionary6;
								}
							}
							string safeAttributeStr6 = Global.GetSafeAttributeStr(xml, "RebornSuperiorBank2");
							string[] array7 = safeAttributeStr6.Split(new char[]
							{
								','
							});
							foreach (string text2 in array7)
							{
								list4.Add(Convert.ToInt32(text2));
								rebornSuperiorDrop.RebornSuperiorBank2 = list4;
							}
							string safeAttributeStr7 = Global.GetSafeAttributeStr(xml, "RebornSuperiorRate3");
							double num4 = 0.0;
							string[] array8 = safeAttributeStr7.Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array8)
							{
								num4 += Convert.ToDouble(text2.Split(new char[]
								{
									','
								})[1]);
								if (num4 != 0.0)
								{
									dictionary7.Add(num4, Convert.ToInt32(text2.Split(new char[]
									{
										','
									})[0]));
									rebornSuperiorDrop.RebornSuperiorRate3 = dictionary7;
								}
							}
							string safeAttributeStr8 = Global.GetSafeAttributeStr(xml, "RebornSuperiorBank3");
							string[] array9 = safeAttributeStr8.Split(new char[]
							{
								','
							});
							foreach (string text2 in array9)
							{
								list5.Add(Convert.ToInt32(text2));
								rebornSuperiorDrop.RebornSuperiorBank3 = list5;
							}
							rebornSuperiorDrop.ShowColor = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ShowColor"));
							dictionary4.Add(rebornSuperiorDrop.ID, rebornSuperiorDrop);
						}
						RebornEquip.SuperiorDrop = dictionary4;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					if (RebornEquip.SuperiorDrop == null)
					{
						result = false;
					}
					else
					{
						Dictionary<int, RebornSuperiorType> dictionary8 = new Dictionary<int, RebornSuperiorType>();
						text = Global.GameResPath(RebornEquipConst.RebornSuperiorType);
						xelement = XElement.Load(text);
						if (null == xelement)
						{
							LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
						}
						try
						{
							IEnumerable<XElement> enumerable = xelement.Elements();
							foreach (XElement xml in enumerable)
							{
								RebornSuperiorType rebornSuperiorType = new RebornSuperiorType();
								Dictionary<double, double> dictionary9 = new Dictionary<double, double>();
								rebornSuperiorType.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
								rebornSuperiorType.Type = (int)ConfigParser.GetPropIndexByPropName(Global.GetSafeAttributeStr(xml, "Type"));
								string[] array10 = Global.GetSafeAttributeStr(xml, "Parameter").Split(new char[]
								{
									'|'
								});
								double num5 = 0.0;
								foreach (string text2 in array10)
								{
									num5 += Convert.ToDouble(text2.Split(new char[]
									{
										','
									})[1]);
									dictionary9.Add(Convert.ToDouble(text2.Split(new char[]
									{
										','
									})[0]), num5);
									rebornSuperiorType.Parameter = dictionary9;
								}
								dictionary8.Add(rebornSuperiorType.ID, rebornSuperiorType);
							}
							RebornEquip.SuperiorType = dictionary8;
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
						if (RebornEquip.SuperiorType == null)
						{
							result = false;
						}
						else
						{
							Dictionary<int, Dictionary<int, RebornQuenching>> dictionary10 = new Dictionary<int, Dictionary<int, RebornQuenching>>();
							Dictionary<int, int> dictionary11 = new Dictionary<int, int>();
							text = Global.GameResPath(RebornEquipConst.RebornEquipQuenching);
							xelement = XElement.Load(text);
							if (null == xelement)
							{
								LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
							}
							try
							{
								IEnumerable<XElement> enumerable = xelement.Elements();
								foreach (XElement xml in enumerable)
								{
									RebornQuenching rebornQuenching = new RebornQuenching();
									rebornQuenching.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
									rebornQuenching.Perfusion = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "PerfusionName"));
									rebornQuenching.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "PerfusionLevel"));
									Dictionary<int, double> dictionary12 = new Dictionary<int, double>();
									string safeAttributeStr9 = Global.GetSafeAttributeStr(xml, "EverLelAttribute");
									if (safeAttributeStr9.Equals("-1"))
									{
										dictionary12.Add(0, 0.0);
									}
									else
									{
										string[] array11 = Global.GetSafeAttributeStr(xml, "EverLelAttribute").Split(new char[]
										{
											'|'
										});
										foreach (string text2 in array11)
										{
											string[] array12 = text2.Split(new char[]
											{
												','
											});
											int propIndexByPropName = (int)ConfigParser.GetPropIndexByPropName(array12[0]);
											if (!dictionary12.ContainsKey(propIndexByPropName))
											{
												dictionary12.Add(propIndexByPropName, Convert.ToDouble(array12[1]));
											}
										}
									}
									rebornQuenching.Attr = dictionary12;
									Dictionary<int, int> dictionary13 = new Dictionary<int, int>();
									string[] array13 = Global.GetSafeAttributeStr(xml, "LossItem").Split(new char[]
									{
										','
									});
									if (array13.Length != 1)
									{
										int key = Convert.ToInt32(array13[0]);
										if (!dictionary13.ContainsKey(key))
										{
											dictionary13.Add(key, Convert.ToInt32(array13[1]));
										}
									}
									rebornQuenching.UseGoods = dictionary13;
									string[] array14 = Global.GetSafeAttributeStr(xml, "IncreaseProbability").Split(new char[]
									{
										'|'
									});
									if (array14.Length == 1)
									{
										rebornQuenching.AddStart = 0.0;
										rebornQuenching.AddEnd = 0.0;
									}
									else
									{
										rebornQuenching.AddStart = Convert.ToDouble(array14[0]);
										rebornQuenching.AddEnd = Convert.ToDouble(array14[1]);
									}
									string[] array15 = Global.GetSafeAttributeStr(xml, "ReduceProbability").Split(new char[]
									{
										'|'
									});
									if (array15.Length == 1)
									{
										rebornQuenching.SubStart = 0.0;
										rebornQuenching.SubEnd = 0.0;
									}
									else
									{
										rebornQuenching.SubStart = Convert.ToDouble(array15[0]);
										rebornQuenching.SubEnd = Convert.ToDouble(array15[1]);
									}
									string[] array16 = Global.GetSafeAttributeStr(xml, "UpProbability").Split(new char[]
									{
										'|'
									});
									if (array16.Length == 1)
									{
										rebornQuenching.SturmGaiLv = 0.0;
										rebornQuenching.SturmLevel = 0;
									}
									else
									{
										rebornQuenching.SturmGaiLv = Convert.ToDouble(array16[0]);
										rebornQuenching.SturmLevel = Convert.ToInt32(array16[1]);
									}
									rebornQuenching.AbschreckenUnterwerfen = Global.GetSafeAttributeDouble(xml, "LelInterval");
									if (dictionary10.ContainsKey(rebornQuenching.Perfusion))
									{
										if (dictionary10[rebornQuenching.Perfusion].ContainsKey(rebornQuenching.Level))
										{
											dictionary10[rebornQuenching.Perfusion][rebornQuenching.Level] = rebornQuenching;
										}
										else
										{
											dictionary10[rebornQuenching.Perfusion].Add(rebornQuenching.Level, rebornQuenching);
										}
									}
									else
									{
										Dictionary<int, RebornQuenching> dictionary14 = new Dictionary<int, RebornQuenching>();
										dictionary14.Add(rebornQuenching.Level, rebornQuenching);
										dictionary10.Add(rebornQuenching.Perfusion, dictionary14);
									}
									if (dictionary11.ContainsKey(rebornQuenching.Perfusion))
									{
										if (dictionary11[rebornQuenching.Perfusion] < rebornQuenching.Level)
										{
											dictionary11[rebornQuenching.Perfusion] = rebornQuenching.Level;
										}
									}
									else
									{
										dictionary11.Add(rebornQuenching.Perfusion, rebornQuenching.Level);
									}
								}
								RebornEquip.RebornEquipHole = dictionary10;
								RebornEquip.RebornEquipHoleLevelMap = dictionary11;
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
							if (RebornEquip.RebornEquipHole == null || RebornEquip.RebornEquipHoleLevelMap == null)
							{
								result = false;
							}
							else
							{
								int num6 = 0;
								for (int j = 122; j <= 150; j += 7)
								{
									int num7 = 165 + num6;
									List<int> list6 = new List<int>();
									list6.Add(num7);
									list6.Add(num7 + 1);
									if (!RebornEquip.ExtPropIndexMap.ContainsKey(j))
									{
										RebornEquip.ExtPropIndexMap.Add(j, list6);
									}
									num6 += 2;
								}
								result = (RebornEquip.ExtPropIndexMap != null);
							}
						}
					}
				}
			}
			return result;
		}

		public void InitRoleRebornGoodsData(GameClient client)
		{
			if (GlobalNew.IsGongNengOpened(client, 105, false))
			{
				if (null == client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 15001), client.ServerId);
					if (null == client.ClientData.RebornGoodsStoreList)
					{
						client.ClientData.RebornGoodsStoreList = new List<GoodsData>();
					}
				}
				if (null == client.ClientData.RebornGoodsDataList)
				{
					client.ClientData.RebornGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 15000), client.ServerId);
					if (null == client.ClientData.RebornGoodsDataList)
					{
						client.ClientData.RebornGoodsDataList = new List<GoodsData>();
					}
				}
				RebornEquip.ResetBagAllGoods(client, true);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
		}

		public static void RefreshOneEquipProp(GameClient client, GoodsData goodsData, ref AllThingsCalcItem allThingsCalcItem)
		{
			SystemXmlItem systemGoods = null;
			systemGoods = null;
			if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
			{
				if (RebornEquip.IsRebornEquip(goodsData.GoodsID))
				{
					int num = client.UsingEquipMgr.EquipFirstPropCondition(client, systemGoods);
					EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
					if (null != equipPropItem)
					{
						if (num == 1)
						{
							if (goodsData.Quality >= 4)
							{
								allThingsCalcItem.TotalGoldQualityNum++;
							}
							else if (goodsData.Quality >= 3)
							{
								allThingsCalcItem.TotalPurpleQualityNum++;
							}
						}
						if (num == 1)
						{
							allThingsCalcItem.ChangeTotalForgeLevel(goodsData.Forge_level, true);
						}
						Global.CalcExcellenceEquipNum(allThingsCalcItem, goodsData, num, true);
						if (!string.IsNullOrEmpty(goodsData.Jewellist) && num == 1)
						{
							AllThingsCalcItem allThingsCalcItem2 = new AllThingsCalcItem();
							string[] array = goodsData.Jewellist.Split(new char[]
							{
								','
							});
							for (int i = 0; i < array.Length; i++)
							{
								int num2 = Convert.ToInt32(array[i]);
								EquipPropItem equipPropItem2 = GameManager.EquipPropsMgr.FindEquipPropItem(num2);
								if (null != equipPropItem2)
								{
									int jewelLevel = Global.GetJewelLevel(num2);
									if (jewelLevel >= 8)
									{
										allThingsCalcItem.TotalJewel8LevelNum++;
										allThingsCalcItem2.TotalJewel8LevelNum++;
									}
									else if (jewelLevel >= 7)
									{
										allThingsCalcItem.TotalJewel7LevelNum++;
										allThingsCalcItem2.TotalJewel7LevelNum++;
									}
									else if (jewelLevel >= 6)
									{
										allThingsCalcItem.TotalJewel6LevelNum++;
										allThingsCalcItem2.TotalJewel6LevelNum++;
									}
									else if (jewelLevel >= 5)
									{
										allThingsCalcItem.TotalJewel5LevelNum++;
										allThingsCalcItem2.TotalJewel5LevelNum++;
									}
									else if (jewelLevel >= 4)
									{
										allThingsCalcItem.TotalJewel4LevelNum++;
										allThingsCalcItem2.TotalJewel4LevelNum++;
									}
								}
							}
						}
						int num3 = (int)equipPropItem.ExtProps[0];
						if (goodsData.Strong < num3)
						{
							bool flag = true;
							if (num != 1)
							{
								flag = false;
							}
							if (goodsData.ExcellenceInfo != 0)
							{
								Global.ProcessEquipExcellenceProp(client, goodsData, flag, systemGoods);
							}
							if (flag && goodsData.Lucky > 0)
							{
								Global.ProcessEquipLuckProp(client, goodsData, flag, systemGoods);
							}
							if (num == 1)
							{
								for (int j = 0; j < 5; j++)
								{
									client.ClientData.EquipProp.BaseProps[j] += Global.GetEquipBasePropsItemVal(goodsData, equipPropItem, j);
								}
								for (int j = 0; j < 177; j++)
								{
									client.ClientData.EquipProp.ExtProps[j] += Global.GetEquipExtPropsItemVal(client, goodsData, equipPropItem, j, systemGoods);
								}
							}
							int occupation = Global.CalcOriginalOccupationID(client);
							ChuanQiQianHua.ApplayEquipQianHuaProps(client.ClientData.EquipProp.ExtProps, occupation, goodsData, systemGoods, true);
							if (num > 0)
							{
								if (goodsData.WashProps != null && goodsData.WashProps.Count >= 2)
								{
									for (int j = 0; j < goodsData.WashProps.Count; j += 2)
									{
										int num4 = goodsData.WashProps[j];
										if (0 < num4 && num4 < 177)
										{
											client.ClientData.EquipProp.ExtProps[num4] += (double)goodsData.WashProps[j + 1] * 0.001;
										}
									}
								}
								if (goodsData.ElementhrtsProps != null && goodsData.ElementhrtsProps.Count >= 2 && GoodsUtil.IsEquip(goodsData.GoodsID))
								{
									double num5 = 0.001;
									for (int j = 0; j <= goodsData.ElementhrtsProps.Count - 2; j += 2)
									{
										int num4 = goodsData.ElementhrtsProps[j];
										if (0 < num4 && num4 < 177)
										{
											client.ClientData.EquipProp.ExtProps[num4] += (double)goodsData.ElementhrtsProps[j + 1] * num5;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static List<GoodsData> GetUsingGoodsList(SafeClientData clientData)
		{
			List<GoodsData> result;
			if (null == clientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (clientData.RebornGoodsDataList)
				{
					for (int i = 0; i < clientData.RebornGoodsDataList.Count; i++)
					{
						if (clientData.RebornGoodsDataList[i].Using > 0 && GoodsUtil.IsVisiableEquip(clientData.RebornGoodsDataList[i].GoodsID))
						{
							list.Add(clientData.RebornGoodsDataList[i]);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static int GetCurrGoodsQuality(GoodsData gd)
		{
			int result;
			if (!RebornEquip.IsRebornEquip(gd.GoodsID))
			{
				result = 0;
			}
			else if (gd.WashProps == null)
			{
				result = 0;
			}
			else
			{
				int num = gd.WashProps.Count / 2;
				if (num >= 6)
				{
					result = 5;
				}
				else if (num > 4)
				{
					result = 4;
				}
				else if (num >= 3)
				{
					result = 3;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public static int CalcFixAllEquipsStrongMoney(GameClient client)
		{
			int result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = 0;
			}
			else if (client.ClientData.RebornGoodsDataList.Count <= 0)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				List<GoodsData> list = null;
				lock (client.ClientData.RebornGoodsDataList)
				{
					list = client.ClientData.RebornGoodsDataList.GetRange(0, client.ClientData.RebornGoodsDataList.Count);
				}
				for (int i = 0; i < list.Count; i++)
				{
					GoodsData goodsData = list[i];
					if (goodsData.Using > 0)
					{
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (goodsCatetoriy >= 30 && goodsCatetoriy <= 38)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem) && null != systemXmlItem)
							{
								int intValue = systemXmlItem.GetIntValue("PriceTwo", -1);
								int[] intArrayValue = systemXmlItem.GetIntArrayValue("EquipProps", ',');
								if (intArrayValue != null && intArrayValue.Length >= 2 && intValue > 0)
								{
									double num2 = (double)intArrayValue[0];
									if (num2 > 0.0 && (double)goodsData.Strong > 0.0)
									{
										int num3 = (int)((double)intValue / 3.0 * (double)goodsData.Strong / num2);
										num3 = Global.RecalcNeedYinLiang(num3);
										num += num3;
									}
								}
							}
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static int VerifyWeaponCanEquip(Dictionary<int, List<GoodsData>> EquipDict)
		{
			int num = 0;
			List<GoodsData> list = null;
			int i = 37;
			while (i <= 38)
			{
				list = null;
				lock (EquipDict)
				{
					if (!EquipDict.TryGetValue(i, out list))
					{
						goto IL_CB;
					}
					if (list != null && list.Count > 0)
					{
						num += list.Count;
						for (int j = 0; j < list.Count; j++)
						{
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(list[j].GoodsID, out systemXmlItem))
							{
								return -1;
							}
						}
					}
				}
				goto IL_B5;
				IL_CB:
				i++;
				continue;
				IL_B5:
				if (num <= 1)
				{
					goto IL_CB;
				}
				return -3;
			}
			return 0;
		}

		public static bool isWarnRebornEquip(GameClient client, SystemXmlItem systemGoods)
		{
			int intValue = systemGoods.GetIntValue("ToReborn", -1);
			int intValue2 = systemGoods.GetIntValue("ToRebornLevel", -1);
			return client.ClientData.RebornCount >= intValue && client.ClientData.RebornLevel >= intValue2;
		}

		public static bool IsRebornEquip(int goodsID)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
			return goodsCatetoriy >= 30 && goodsCatetoriy <= 38;
		}

		public static bool IsRebornEquipShengWu(int goodsID)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
			return goodsCatetoriy == 37;
		}

		public static bool IsRebornEquipShengQi(int goodsID)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
			return goodsCatetoriy == 38;
		}

		public static bool IsRebornEquipAttackOrDefense(int goodsID, out int Index)
		{
			Index = -1;
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
			bool result;
			if (goodsCatetoriy == 37 || goodsCatetoriy == 38 || goodsCatetoriy == 36 || goodsCatetoriy == 35)
			{
				Index = 0;
				result = true;
			}
			else if (goodsCatetoriy == 34 || goodsCatetoriy == 32 || goodsCatetoriy == 33 || goodsCatetoriy == 31 || goodsCatetoriy == 30)
			{
				Index = 1;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool IsRebornType(int goodsID)
		{
			return RebornEquip.GetGoodsRebornEquip(goodsID) == 1;
		}

		public static bool OneIsCanIntoRebornOrBaseBag(GameClient client, GoodsData goodsData, out int BagInt)
		{
			BagInt = 0;
			bool result;
			if (Global.GetGoodsRebornEquip(goodsData.GoodsID) == 1)
			{
				if (!RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true, false))
				{
					BagInt = 1;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (!Global.CanAddGoods2(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true))
			{
				BagInt = 2;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static bool MoreIsCanIntoRebornOrBaseBag(GameClient client, List<GoodsData> goodsData, out int BagInt)
		{
			BagInt = 0;
			int num = 0;
			foreach (GoodsData goodsData2 in goodsData)
			{
				if (Global.GetGoodsRebornEquip(goodsData2.GoodsID) == 1)
				{
					num++;
					if (!RebornEquip.CanAddGoodsToReborn(client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Binding, "1900-01-01 12:00:00", true, false) || !RebornEquip.CanAddGoodsNum(client, num))
					{
						BagInt = 1;
						return false;
					}
				}
				else if (!Global.CanAddGoods2(client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Binding, "1900-01-01 12:00:00", true))
				{
					BagInt = 2;
					return false;
				}
			}
			return true;
		}

		public static bool MoreIsCanIntoRebornOrBaseBagAward(GameClient client, List<AwardsItemData> goodsData, out int BagInt)
		{
			BagInt = 0;
			int num = 0;
			int num2 = 0;
			foreach (AwardsItemData awardsItemData in goodsData)
			{
				if (Global.GetGoodsRebornEquip(awardsItemData.GoodsID) == 1)
				{
					num2++;
					if (!RebornEquip.CanAddGoodsToReborn(client, awardsItemData.GoodsID, awardsItemData.GoodsNum, awardsItemData.Binding, "1900-01-01 12:00:00", true, false) || !RebornEquip.CanAddGoodsNum(client, num2))
					{
						BagInt = 1;
						return false;
					}
				}
				else
				{
					num++;
					if (!Global.CanAddGoods2(client, awardsItemData.GoodsID, awardsItemData.GoodsNum, awardsItemData.Binding, "1900-01-01 12:00:00", true) || !Global.CanAddGoodsNum(client, num))
					{
						BagInt = 2;
						return false;
					}
				}
			}
			return true;
		}

		public static bool MoreIsCanIntoRebornOrBaseBagAward(GameClient client, List<GoodsData> goodsData, out int BagInt)
		{
			BagInt = 0;
			int num = 0;
			int num2 = 0;
			foreach (GoodsData goodsData2 in goodsData)
			{
				if (Global.GetGoodsRebornEquip(goodsData2.GoodsID) == 1)
				{
					num2++;
					if (!RebornEquip.CanAddGoodsToReborn(client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Binding, "1900-01-01 12:00:00", true, false) || !RebornEquip.CanAddGoodsNum(client, num2))
					{
						BagInt = 1;
						return false;
					}
				}
				else
				{
					num++;
					if (!Global.CanAddGoods2(client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Binding, "1900-01-01 12:00:00", true) || !Global.CanAddGoodsNum(client, num))
					{
						BagInt = 2;
						return false;
					}
				}
			}
			return true;
		}

		public static int GetGoodsRebornEquip(int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
			{
				result = 0;
			}
			else
			{
				result = systemXmlItem.GetIntValue("RebornEquip", -1);
			}
			return result;
		}

		public static bool CanAddGoodsDataList(GameClient client, List<GoodsData> goodsDataList)
		{
			bool result;
			if (null == goodsDataList)
			{
				result = true;
			}
			else
			{
				int num = 0;
				foreach (GoodsData goodsData in goodsDataList)
				{
					num += RebornEquip.CalGoodsGridNum(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, goodsData.Endtime, true);
				}
				result = RebornEquip.CanAddGoodsNum(client, num);
			}
			return result;
		}

		public static bool CanAddGoodsDataList2(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int num = RebornEquip.CalGoodsGridNum(client, goodsID, newGoodsNum, binding, endTime, canUseOld);
				int goodsUsedGrid = RebornEquip.GetGoodsUsedGrid(client);
				int selfBagCapacity = RebornEquip.GetSelfBagCapacity(client);
				result = (goodsUsedGrid + num <= selfBagCapacity);
			}
			return result;
		}

		private static int CalGoodsGridNum(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			int num = Global.GetGoodsGridNumByID(goodsID);
			num = Global.GMax(num, 1);
			int result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = (newGoodsNum - 1) / num + 1;
			}
			else
			{
				int num2 = 0;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							num2++;
							if (canUseOld && num > 1)
							{
								if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.RebornGoodsDataList[i].Endtime, endTime))
								{
									if (client.ClientData.RebornGoodsDataList[i].GCount < num)
									{
										newGoodsNum -= Global.GMin(newGoodsNum, num - client.ClientData.RebornGoodsDataList[i].GCount);
									}
								}
							}
						}
					}
				}
				if (newGoodsNum <= 0)
				{
					result = 0;
				}
				else
				{
					result = (newGoodsNum - 1) / num + 1;
				}
			}
			return result;
		}

		public static int GetTotalGoodsCountByID(GameClient client, int goodsID)
		{
			int result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID)
						{
							if (!Global.IsGoodsTimeOver(client.ClientData.RebornGoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.RebornGoodsDataList[i]))
							{
								num += client.ClientData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static bool CanAddGoodsNum(GameClient client, int newGoodsCount)
		{
			bool result;
			if (newGoodsCount <= 0)
			{
				result = false;
			}
			else
			{
				int goodsUsedGrid = RebornEquip.GetGoodsUsedGrid(client);
				result = (newGoodsCount + goodsUsedGrid <= client.ClientData.RebornBagNum);
			}
			return result;
		}

		public static int GetGoodsUsedGrid(GameClient client)
		{
			int num = 0;
			int result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = num;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							num++;
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static bool RemoveGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.RebornGoodsDataList == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				lock (client.ClientData.RebornGoodsDataList)
				{
					flag = client.ClientData.RebornGoodsDataList.Remove(gd);
				}
				result = flag;
			}
			return result;
		}

		public static bool RemoveGoodsDataToDb(GameClient client, GoodsData goodsData)
		{
			string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				4,
				goodsData.Id,
				goodsData.GoodsID,
				0,
				goodsData.Site,
				goodsData.GCount,
				goodsData.BagIndex,
				""
			});
			return TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
		}

		public static bool RemoveGoodsDataToDb(GameClient client, GoodsData goodsData, int Num)
		{
			bool result;
			if (goodsData.GCount < Num)
			{
				result = false;
			}
			else
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					client.ClientData.RoleID,
					4,
					goodsData.Id,
					goodsData.GoodsID,
					0,
					goodsData.Site,
					Num,
					goodsData.BagIndex,
					""
				});
				result = (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null));
			}
			return result;
		}

		public static void AddPortableGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 15001)
			{
				RebornEquip.UpdatePortableGoodsNum(client, 1);
				if (null == client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList = new List<GoodsData>();
				}
				lock (client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList.Add(goodsData);
				}
			}
		}

		public static void AddGoodsData(GameClient client, GoodsData gd)
		{
			if (null != gd)
			{
				if (client.ClientData.RebornGoodsDataList == null)
				{
					client.ClientData.RebornGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.RebornGoodsDataList)
				{
					client.ClientData.RebornGoodsDataList.Add(gd);
				}
			}
		}

		public static int GetMaxMountCount()
		{
			return 240;
		}

		public static int GetIdleSlotOfRebornGoods(GameClient client)
		{
			int num = 0;
			int result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = num;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
				{
					if (client.ClientData.RebornGoodsDataList[i].Site == 15000 && client.ClientData.RebornGoodsDataList[i].Using <= 0)
					{
						if (list.IndexOf(client.ClientData.RebornGoodsDataList[i].BagIndex) < 0)
						{
							list.Add(client.ClientData.RebornGoodsDataList[i].BagIndex);
						}
					}
				}
				for (int j = 0; j < client.ClientData.RebornBagNum; j++)
				{
					if (list.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetIdleSlotOfPortableGoods(GameClient client)
		{
			int num = -1;
			int result;
			if (null == client.ClientData.RebornGoodsStoreList)
			{
				result = 0;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
				{
					if (client.ClientData.RebornGoodsStoreList[i].Site == 15001 && client.ClientData.RebornGoodsStoreList[i].Using <= 0)
					{
						if (list.IndexOf(client.ClientData.RebornGoodsStoreList[i].BagIndex) < 0)
						{
							list.Add(client.ClientData.RebornGoodsStoreList[i].BagIndex);
						}
					}
				}
				for (int j = 0; j < RebornEquip.GetRebornStoreCapacity(client); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public static void UpdatePortableGoodsNum(GameClient client, int addNum)
		{
			client.ClientData.RebornGirdData.GoodsUsedGridNum += addNum;
		}

		public static void RemovePortableGoodsData(GameClient client, GoodsData goodsData)
		{
			RebornEquip.UpdatePortableGoodsNum(client, -1);
			if (null != client.ClientData.RebornGoodsStoreList)
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList.Remove(goodsData);
				}
			}
		}

		public static bool CanOpenPortableBag(GameClient client)
		{
			VIPDataInfo vipdataInfo;
			if (Data.VIPDataInfoList.TryGetValue(5001, out vipdataInfo))
			{
				if (client.ClientData.VipLevel < vipdataInfo.VIPlev && Global.GetTwoPointDistanceSquare(client.CurrentPos, client.ClientData.OpenPortableBagPoint) > 810000.0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool CanPortableAddGoods(GameClient client)
		{
			return client.ClientData.RebornGirdData.GoodsUsedGridNum < client.ClientData.RebornGirdData.ExtGridNum;
		}

		public static bool CanAddGoodsToReborn(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true, bool bLeftGrid = false)
		{
			bool result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int num = Global.GetGoodsGridNumByID(goodsID);
				num = Global.GMax(num, 1);
				bool flag = false;
				int num2 = 0;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							num2++;
							if (canUseOld && num > 1)
							{
								if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.RebornGoodsDataList[i].Endtime, endTime))
								{
									if (client.ClientData.RebornGoodsDataList[i].GCount + newGoodsNum <= num)
									{
										flag = true;
										break;
									}
								}
							}
						}
					}
				}
				result = ((flag && !bLeftGrid) || num2 < client.ClientData.RebornBagNum);
			}
			return result;
		}

		public static GoodsData AddRebornGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			if (null == client.ClientData.RebornGoodsDataList)
			{
				client.ClientData.RebornGoodsDataList = new List<GoodsData>();
			}
			lock (client.ClientData.RebornGoodsDataList)
			{
				client.ClientData.RebornGoodsDataList.Add(goodsData);
			}
			return goodsData;
		}

		public static List<int> GetRandomSuperior(List<int> bankListSource, int num)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			list2.AddRange(bankListSource);
			List<int> result;
			if (num <= 0)
			{
				result = list;
			}
			else
			{
				lock (RebornEquip.Mutex)
				{
					for (int i = 0; i < num; i++)
					{
						if (list2.Count < 1)
						{
							break;
						}
						int randomNumber = Global.GetRandomNumber(0, list2.Count);
						RebornSuperiorType rebornSuperiorType;
						if (!RebornEquip.SuperiorType.TryGetValue(list2[randomNumber], out rebornSuperiorType))
						{
							break;
						}
						double random = Global.GetRandom();
						foreach (KeyValuePair<double, double> keyValuePair in rebornSuperiorType.Parameter)
						{
							if (random <= keyValuePair.Value)
							{
								list.Add(rebornSuperiorType.Type);
								list.Add(Convert.ToInt32(keyValuePair.Key * 1000.0));
								break;
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static List<int> GetFixationSuperior(RebornSuperiorDrop superiorDrop)
		{
			List<int> list = new List<int>();
			lock (list)
			{
				foreach (int num in superiorDrop.RebornSuperiorInherentNum)
				{
					if (num < superiorDrop.RebornSuperiorInherentBank.Count)
					{
						RebornSuperiorType rebornSuperiorType;
						if (RebornEquip.SuperiorType.TryGetValue(superiorDrop.RebornSuperiorInherentBank[num], out rebornSuperiorType))
						{
							double random = Global.GetRandom();
							foreach (KeyValuePair<double, double> keyValuePair in rebornSuperiorType.Parameter)
							{
								if (random <= keyValuePair.Value)
								{
									list.Add(rebornSuperiorType.Type);
									list.Add(Convert.ToInt32(keyValuePair.Key * 1000.0));
									break;
								}
							}
						}
					}
				}
			}
			return list;
		}

		public static List<int> CalZhuoYueAttrByID(int code)
		{
			List<int> list = new List<int>();
			try
			{
				RebornSuperiorDrop rebornSuperiorDrop;
				if (!RebornEquip.SuperiorDrop.TryGetValue(code, out rebornSuperiorDrop))
				{
					return list;
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				lock (rebornSuperiorDrop)
				{
					double random = Global.GetRandom();
					foreach (KeyValuePair<double, int> keyValuePair in rebornSuperiorDrop.RebornSuperiorRate1)
					{
						if (random < keyValuePair.Key)
						{
							num = Convert.ToInt32(keyValuePair.Value);
							break;
						}
					}
					double random2 = Global.GetRandom();
					foreach (KeyValuePair<double, int> keyValuePair in rebornSuperiorDrop.RebornSuperiorRate2)
					{
						if (random2 < keyValuePair.Key)
						{
							num2 = Convert.ToInt32(keyValuePair.Value);
							break;
						}
					}
					double random3 = Global.GetRandom();
					foreach (KeyValuePair<double, int> keyValuePair in rebornSuperiorDrop.RebornSuperiorRate3)
					{
						if (random3 < keyValuePair.Key)
						{
							num3 = Convert.ToInt32(keyValuePair.Value);
							break;
						}
					}
				}
				lock (list)
				{
					list.AddRange(RebornEquip.GetFixationSuperior(rebornSuperiorDrop));
					list.AddRange(RebornEquip.GetRandomSuperior(rebornSuperiorDrop.RebornSuperiorBank1, num));
					list.AddRange(RebornEquip.GetRandomSuperior(rebornSuperiorDrop.RebornSuperiorBank2, num2));
					list.AddRange(RebornEquip.GetRandomSuperior(rebornSuperiorDrop.RebornSuperiorBank3, num3));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("重生装备 :: 根据卓越ID计算随机卓越属性，code={0}。", code), ex, true);
			}
			return list;
		}

		public static GoodsData GetRebornGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Id == dbID)
						{
							return client.ClientData.RebornGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static List<GoodsData> GetRebornGoodsByDbIDDict(GameClient client, Dictionary<int, int> DBidList, out Dictionary<int, Dictionary<int, int>> IDNumMap)
		{
			IDNumMap = new Dictionary<int, Dictionary<int, int>>();
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> result;
			if (null == DBidList)
			{
				result = null;
			}
			else if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						foreach (KeyValuePair<int, int> keyValuePair in DBidList)
						{
							if (client.ClientData.RebornGoodsDataList[i].Id == keyValuePair.Key && client.ClientData.RebornGoodsDataList[i].GCount >= keyValuePair.Value)
							{
								Dictionary<int, int> dictionary = new Dictionary<int, int>();
								list.Add(client.ClientData.RebornGoodsDataList[i]);
								dictionary.Add(client.ClientData.RebornGoodsDataList[i].GoodsID, keyValuePair.Value);
								IDNumMap.Add(i, dictionary);
							}
						}
					}
				}
				if (list.Count != DBidList.Count)
				{
					result = null;
				}
				else if (list.Count > 0)
				{
					result = list;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static GoodsData GetRebornStoreGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsStoreList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
					{
						if (client.ClientData.RebornGoodsStoreList[i].Id == dbID)
						{
							return client.ClientData.RebornGoodsStoreList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static TCPProcessCmdResults SaleRebornEquipProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			string[] array = strGoodsID.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < array.Length)
			{
				int dbID = Global.SafeConvertToInt32(array[i]);
				GoodsData rebornGoodsByDbID = RebornEquip.GetRebornGoodsByDbID(client, dbID);
				if (rebornGoodsByDbID != null && rebornGoodsByDbID.Site == 15000 && rebornGoodsByDbID.Using <= 0)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(rebornGoodsByDbID.GoodsID);
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rebornGoodsByDbID.GoodsID, out systemXmlItem) && null != systemXmlItem)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							4,
							rebornGoodsByDbID.Id,
							rebornGoodsByDbID.GoodsID,
							0,
							rebornGoodsByDbID.Site,
							rebornGoodsByDbID.GCount,
							rebornGoodsByDbID.BagIndex,
							""
						});
						if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "重生装备回收", null))
						{
							if (client.ClientData.RebornCount > 0)
							{
								SystemXmlItem systemXmlItem2 = null;
								GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rebornGoodsByDbID.GoodsID, out systemXmlItem2);
								if (null != systemXmlItem2)
								{
									int num7 = Global.GetGoodsDataPrice(rebornGoodsByDbID);
									if (num7 < 0)
									{
										num7 = 0;
									}
									if (RebornEquip.IsRebornEquip(rebornGoodsByDbID.GoodsID))
									{
										if (rebornGoodsByDbID.Props != null || rebornGoodsByDbID.Props != "")
										{
											Dictionary<int, Dictionary<int, int>> dictionary = RebornStone.ParessMakeHoleInfo(rebornGoodsByDbID.Props);
											if (dictionary != null || dictionary.Count != 0)
											{
												foreach (Dictionary<int, int> dictionary2 in dictionary.Values)
												{
													foreach (int num8 in dictionary2.Values)
													{
														SystemXmlItem systemXmlItem3 = null;
														if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num8, out systemXmlItem3))
														{
															break;
														}
														int binding = 0;
														if (rebornGoodsByDbID.Binding == 1)
														{
															binding = 1;
														}
														Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, num8, 1, 0, "", 0, binding, 15000, "", true, 1, string.Format("装备回收宝石卸下", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
													}
												}
											}
										}
										if (systemXmlItem.GetIntValue("SuitID", -1) >= 2)
										{
											int num9 = 0;
											int num10 = 0;
											int num11 = 0;
											if (RebornEquip.GetRebornEquipUseGoods(rebornGoodsByDbID.GoodsID, out num9, out num10, out num11))
											{
												num6 += num9;
												num5 += num10;
												num4 += num11;
											}
										}
										int intValue = systemXmlItem2.GetIntValue("ChangeRebornExp", -1);
										if (intValue > 0)
										{
											num += intValue;
										}
									}
									else if (rebornGoodsByDbID.Binding > 0)
									{
										num3 += num7;
									}
									else
									{
										num2 += num7;
									}
								}
							}
						}
					}
				}
				IL_3E5:
				i++;
				continue;
				goto IL_3E5;
			}
			if (num > 0)
			{
				RebornManager.getInstance().ProcessRoleExperience(client, (long)num, MoneyTypes.RebornExpSale, true, true, false, "none");
			}
			GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "重生背包出售材料", false);
			GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num3, "重生背包出售材料", true);
			RebornEquip.SetRebornEquipUseGoods(client, num6, num5, num4);
			if (num2 > 0 && num3 > 0)
			{
				GameManager.LuaMgr.Hot(client, StringUtil.substitute(GLang.GetLang(655, new object[0]), new object[]
				{
					num2,
					num3
				}), 0);
			}
			else if (num2 > 0)
			{
				GameManager.LuaMgr.Hot(client, StringUtil.substitute(GLang.GetLang(656, new object[0]), new object[]
				{
					num2
				}), 0);
			}
			else if (num3 > 0)
			{
				GameManager.LuaMgr.Hot(client, StringUtil.substitute(GLang.GetLang(657, new object[0]), new object[]
				{
					num3
				}), 0);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		public static TCPProcessCmdResults SaleStoreRebornEquipProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			return TCPProcessCmdResults.RESULT_OK;
		}

		public static void ResetBagAllGoods(GameClient client, bool notifyClient = true)
		{
			byte[] array = null;
			if (client.ClientData.RebornGoodsDataList != null)
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
					List<GoodsData> list = new List<GoodsData>();
					List<GoodsData> list2 = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							if (!RebornEquip.IsRebornType(client.ClientData.RebornGoodsDataList[i].GoodsID))
							{
								list2.Add(client.ClientData.RebornGoodsDataList[i]);
							}
							client.ClientData.RebornGoodsDataList[i].BagIndex = 0;
							int goodsGridNumByID = Global.GetGoodsGridNumByID(client.ClientData.RebornGoodsDataList[i].GoodsID);
							if (goodsGridNumByID > 1)
							{
								GoodsData goodsData = null;
								string key = string.Format("{0}_{1}_{2}_{3}", new object[]
								{
									client.ClientData.RebornGoodsDataList[i].GoodsID,
									client.ClientData.RebornGoodsDataList[i].Binding,
									Global.DateTimeTicks(client.ClientData.RebornGoodsDataList[i].Starttime),
									Global.DateTimeTicks(client.ClientData.RebornGoodsDataList[i].Endtime)
								});
								if (dictionary.TryGetValue(key, out goodsData))
								{
									int num = Global.GMin(goodsGridNumByID - goodsData.GCount, client.ClientData.RebornGoodsDataList[i].GCount);
									goodsData.GCount += num;
									client.ClientData.RebornGoodsDataList[i].GCount -= num;
									client.ClientData.RebornGoodsDataList[i].BagIndex = 1;
									goodsData.BagIndex = 1;
									if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsDataList[i]))
									{
										break;
									}
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, num, goodsData.GCount, "整理背包");
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, client.ClientData.RebornGoodsDataList[i].GoodsID, (long)client.ClientData.RebornGoodsDataList[i].Id, -num, client.ClientData.RebornGoodsDataList[i].GCount, "整理背包");
									if (goodsData.GCount >= goodsGridNumByID)
									{
										if (client.ClientData.RebornGoodsDataList[i].GCount > 0)
										{
											dictionary[key] = client.ClientData.RebornGoodsDataList[i];
										}
										else
										{
											dictionary.Remove(key);
											list.Add(client.ClientData.RebornGoodsDataList[i]);
										}
									}
									else if (client.ClientData.RebornGoodsDataList[i].GCount <= 0)
									{
										list.Add(client.ClientData.RebornGoodsDataList[i]);
									}
								}
								else
								{
									dictionary[key] = client.ClientData.RebornGoodsDataList[i];
								}
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						client.ClientData.RebornGoodsDataList.Remove(list[i]);
					}
					for (int i = 0; i < list2.Count; i++)
					{
						client.ClientData.RebornGoodsDataList.Remove(list2[i]);
						int idleSlotOfBagGoods = Global.GetIdleSlotOfBagGoods(client);
						list2[i].BagIndex = idleSlotOfBagGoods;
						list2[i].Site = 0;
						if (client.ClientData.GoodsDataList == null)
						{
							List<GoodsData> list3 = new List<GoodsData>();
							list3.Add(list2[i]);
							client.ClientData.GoodsDataList = list3;
						}
						else
						{
							client.ClientData.GoodsDataList.Add(list2[i]);
						}
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							3,
							list2[i].Id,
							list2[i].GoodsID,
							list2[i].Using,
							list2[i].Site,
							list2[i].GCount,
							list2[i].BagIndex,
							""
						});
						if (TCPProcessCmdResults.RESULT_OK != Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null))
						{
						}
					}
					client.ClientData.RebornGoodsDataList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int num2 = 0;
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							if (GameManager.Flag_OptimizationBagReset)
							{
								bool flag2 = client.ClientData.RebornGoodsDataList[i].BagIndex > 0;
								client.ClientData.RebornGoodsDataList[i].BagIndex = num2++;
								if (flag2)
								{
									if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsDataList[i]))
									{
										break;
									}
								}
							}
							else
							{
								client.ClientData.RebornGoodsDataList[i].BagIndex = num2++;
								if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsDataList[i]))
								{
									break;
								}
							}
						}
					}
					array = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.RebornGoodsDataList);
				}
				if (notifyClient)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 2041);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		public static void ResetStoreRebormGoods(GameClient client)
		{
			if (null != client.ClientData.RebornGoodsStoreList)
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
					{
						if (client.ClientData.RebornGoodsStoreList[i].Using <= 0)
						{
							client.ClientData.RebornGoodsStoreList[i].BagIndex = 1;
							int goodsGridNumByID = Global.GetGoodsGridNumByID(client.ClientData.RebornGoodsStoreList[i].GoodsID);
							if (goodsGridNumByID > 1)
							{
								GoodsData goodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.RebornGoodsStoreList[i].GoodsID, client.ClientData.RebornGoodsStoreList[i].Binding, Global.DateTimeTicks(client.ClientData.RebornGoodsStoreList[i].Endtime));
								if (dictionary.TryGetValue(key, out goodsData))
								{
									int num = Global.GMin(goodsGridNumByID - goodsData.GCount, client.ClientData.RebornGoodsStoreList[i].GCount);
									goodsData.GCount += num;
									client.ClientData.RebornGoodsStoreList[i].GCount -= num;
									client.ClientData.RebornGoodsStoreList[i].BagIndex = 1;
									goodsData.BagIndex = 1;
									if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsStoreList[i]))
									{
										break;
									}
									if (goodsData.GCount >= goodsGridNumByID)
									{
										if (client.ClientData.RebornGoodsStoreList[i].GCount > 0)
										{
											dictionary[key] = client.ClientData.RebornGoodsStoreList[i];
										}
										else
										{
											dictionary.Remove(key);
											list.Add(client.ClientData.RebornGoodsStoreList[i]);
										}
									}
									else if (client.ClientData.RebornGoodsStoreList[i].GCount <= 0)
									{
										list.Add(client.ClientData.RebornGoodsStoreList[i]);
									}
								}
								else
								{
									dictionary[key] = client.ClientData.RebornGoodsStoreList[i];
								}
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						client.ClientData.RebornGoodsStoreList.Remove(list[i]);
					}
					client.ClientData.RebornGoodsStoreList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int num2 = 0;
					for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
					{
						if (client.ClientData.RebornGoodsStoreList[i].Using <= 0)
						{
							bool flag2 = 0 == 0;
							client.ClientData.RebornGoodsStoreList[i].BagIndex = num2++;
							if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsStoreList[i]))
							{
								break;
							}
						}
					}
				}
			}
			TCPOutPacket tcpOutPacket = null;
			if (null != client.ClientData.RebornGoodsStoreList)
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.RebornGoodsStoreList, Global._TCPManager.TcpOutPacketPool, 2042);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.RebornGoodsStoreList, Global._TCPManager.TcpOutPacketPool, 2042);
			}
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		public static int GetRebornStoreCapacity(GameClient client)
		{
			return client.ClientData.RebornGirdData.ExtGridNum;
		}

		public static int GetSelfBagCapacity(GameClient client)
		{
			return client.ClientData.RebornBagNum;
		}

		private static void GetNormalAndExNum(GameClient client, int addGridNum, out int normalNum, out int exNum)
		{
			normalNum = 0;
			exNum = 0;
			addGridNum = Global.GMax(0, addGridNum);
			int rebornStoreCapacity = RebornEquip.GetRebornStoreCapacity(client);
			if (rebornStoreCapacity >= Global.MaxPortableGridNum)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
				{
					Global.MaxPortableGridNum
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
			else
			{
				addGridNum = Global.GMin(addGridNum, Global.MaxPortableGridNum - rebornStoreCapacity);
				normalNum = ((rebornStoreCapacity + addGridNum < Global.ExtraBagGridPriceStartPos) ? addGridNum : Global.GMax(0, Global.ExtraBagGridPriceStartPos - rebornStoreCapacity - 1));
				exNum = Global.GMax(0, addGridNum - normalNum);
			}
		}

		public static int GetOneRebronBagExtendNeedYuanBaoForStorage(int extendPos)
		{
			int num = (extendPos - Global.DefaultPortableGridNum) * Global.OnePortableGridYuanBao;
			if (num > 10 * Global.OnePortableGridYuanBao)
			{
				num = 10 * Global.OnePortableGridYuanBao;
			}
			return num;
		}

		public static int GetExtRebornNeedYuanBaoForStorage(GameClient client, int addNum, int hasTime)
		{
			int rebornStoreCapacity = RebornEquip.GetRebornStoreCapacity(client);
			int num = rebornStoreCapacity + 1;
			int num2 = rebornStoreCapacity + addNum;
			int num3 = 0;
			for (int i = num; i <= num2; i++)
			{
				num3 += RebornEquip.GetOneRebronBagExtendNeedYuanBaoForStorage(i);
				if (i == num)
				{
					double num4 = (double)hasTime / (double)((num - Global.DefaultPortableGridNum) * 1500);
					num3 = (int)((double)num3 * Math.Max(0.0, 1.0 - num4));
				}
			}
			return num3;
		}

		public static int GetOneBagGridExtendNeedYuanBao(int extendPos)
		{
			int num = (extendPos - Global.DefaultBagGridNum) * Global.OneBagGridYuanBao;
			if (num > 10 * Global.OneBagGridYuanBao)
			{
				num = 10 * Global.OneBagGridYuanBao;
			}
			return num;
		}

		public static int GetExtBagGridNeedYuanBao(GameClient client, int addNum, int hasTime)
		{
			int selfBagCapacity = RebornEquip.GetSelfBagCapacity(client);
			int num = selfBagCapacity + 1;
			int num2 = selfBagCapacity + addNum;
			int num3 = 0;
			for (int i = num; i <= num2; i++)
			{
				num3 += RebornEquip.GetOneBagGridExtendNeedYuanBao(i);
				if (i == num)
				{
					double num4 = (double)hasTime / (double)((selfBagCapacity + 1 - Global.DefaultBagGridNum) * 3000);
					num3 = (int)((double)num3 * Math.Max(0.0, 1.0 - num4));
				}
			}
			return num3;
		}

		public static int ExtRoleRebornBagNumWithYuanBao(TCPOutPacketPool pool, GameClient client, int addGridNum)
		{
			int selfBagCapacity = RebornEquip.GetSelfBagCapacity(client);
			int result;
			if (selfBagCapacity >= Global.MaxBagGridNum)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
				{
					Global.MaxBagGridNum
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = -1;
			}
			else
			{
				addGridNum = Global.Clamp(addGridNum, 0, Global.MaxBagGridNum - selfBagCapacity);
				if (addGridNum <= 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(159, new object[0]), new object[]
					{
						addGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -2;
				}
				else
				{
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornBagGridParams", ',');
					if (paramValueIntArrayByName == null || paramValueIntArrayByName.Length != 4 || paramValueIntArrayByName[1] == 0)
					{
						result = -2;
					}
					else
					{
						int num = selfBagCapacity - Global.DefaultBagGridNum + 1;
						if (num == 0)
						{
							result = -2;
						}
						else
						{
							int num2 = 0;
							for (int i = 0; i < addGridNum; i++)
							{
								int num3 = paramValueIntArrayByName[1] * (num + i);
								if (num3 > paramValueIntArrayByName[3])
								{
									num3 = paramValueIntArrayByName[3];
								}
								num2 += num3;
							}
							if (num2 > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "扩充重生背包", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(161, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
									return -170;
								}
							}
							client.ClientData.RebornBagNum += addGridNum;
							GameManager.DBCmdMgr.AddDBCmd(14118, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornBagNum), null, client.ServerId);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		public static int ExtRebornStoreWithYuanBao(TCPOutPacketPool pool, GameClient client, int addGridNum)
		{
			int rebornStoreCapacity = RebornEquip.GetRebornStoreCapacity(client);
			int result;
			if (rebornStoreCapacity >= Global.MaxPortableGridNum)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
				{
					Global.MaxPortableGridNum
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = -1;
			}
			else
			{
				addGridNum = Global.Clamp(addGridNum, 0, Global.MaxPortableGridNum - rebornStoreCapacity);
				if (addGridNum <= 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(159, new object[0]), new object[]
					{
						addGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -2;
				}
				else
				{
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornBagGridParams", ',');
					if (paramValueIntArrayByName == null || paramValueIntArrayByName.Length != 4 || paramValueIntArrayByName[0] == 0)
					{
						result = -2;
					}
					else
					{
						int num = rebornStoreCapacity - Global.DefaultPortableGridNum + 1;
						if (num == 0)
						{
							result = -2;
						}
						else
						{
							int num2 = 0;
							for (int i = 0; i < addGridNum; i++)
							{
								int num3 = paramValueIntArrayByName[0] * (num + i);
								if (num3 > paramValueIntArrayByName[2])
								{
									num3 = paramValueIntArrayByName[2];
								}
								num2 += num3;
							}
							if (num2 > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "扩充移动重生仓库", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(160, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
									return -170;
								}
							}
							client.ClientData.RebornGirdData.ExtGridNum += addGridNum;
							GameManager.DBCmdMgr.AddDBCmd(14119, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornGirdData.ExtGridNum), null, client.ServerId);
							RebornEquip.NotifyRebornBagData(client);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		public static bool ResetRebornBagGoodsData(GameClient client, GoodsData goodsData)
		{
			string[] array = null;
			string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
			{
				client.ClientData.RoleID,
				goodsData.Id,
				goodsData.Using,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				goodsData.GCount,
				"*",
				goodsData.BagIndex,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*"
			});
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
			bool result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = false;
			}
			else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
			{
				result = false;
			}
			else
			{
				Global.NoDBLogModRoleGoodsEvent(client, goodsData, 0, "重置重生背包索引", false);
				EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "重置重生背包索引");
				result = true;
			}
			return result;
		}

		public static void NotifyRebornBagData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RebornPortableBagData>(client.ClientData.RebornGirdData, Global._TCPManager.TcpOutPacketPool, 2045);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public static void NotifyRebornShowEquipData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<int>(client.ClientData.RebornShowEquip, Global._TCPManager.TcpOutPacketPool, 2051);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public static TCPProcessCmdResults ProcessSpriteGetRebornGoodsListCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num || num2 != 15000)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (null == gameClient.ClientData.RebornGoodsDataList)
				{
					TCPProcessCmdResults tcpprocessCmdResults = Global.TransferRequestToDBServer(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 204, data, count, out tcpOutPacket, gameClient.ServerId);
					if (TCPProcessCmdResults.RESULT_FAILED != tcpprocessCmdResults && null != tcpOutPacket)
					{
						List<GoodsData> rebornGoodsDataList = DataHelper.BytesToObject<List<GoodsData>>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
						gameClient.ClientData.RebornGoodsDataList = rebornGoodsDataList;
						Global.PushBackTcpOutPacket(tcpOutPacket);
					}
					if (null == gameClient.ClientData.RebornGoodsDataList)
					{
						gameClient.ClientData.RebornGoodsDataList = new List<GoodsData>();
					}
				}
				byte[] buffer = DataHelper.ObjectToBytes<List<GoodsData>>(gameClient.ClientData.RebornGoodsDataList);
				GameManager.ClientMgr.SendToClient(gameClient, buffer, nID);
				Global.RefreshEquipProp(gameClient);
				GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, gameClient);
				GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, gameClient, true, false, 7);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessSpriteRebornResetBagCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (SingletonTemplate<CreateRoleLimitManager>.Instance().ResetBagSlotTicks > 0 && TimeUtil.NOW() - gameClient.ClientData._ResetBagTicks < (long)SingletonTemplate<CreateRoleLimitManager>.Instance().ResetBagSlotTicks)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(129, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return TCPProcessCmdResults.RESULT_OK;
				}
				gameClient.ClientData._ResetBagTicks = TimeUtil.NOW();
				RebornEquip.ResetBagAllGoods(gameClient, true);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static GoodsData GetGoodsByID(GameClient client, int goodsID)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID)
						{
							return client.ClientData.RebornGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static GoodsData GetBindingGoodsByID(GameClient client, int goodsID, int Binding)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				GoodsData goodsData = null;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == Binding)
						{
							if (goodsData == null)
							{
								goodsData = client.ClientData.RebornGoodsDataList[i];
							}
							else
							{
								goodsData.GCount += client.ClientData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
				result = goodsData;
			}
			return result;
		}

		public static List<GoodsData> GetBindingNotCountGoodsByID(GameClient client, int goodsID, int Binding)
		{
			List<GoodsData> result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == Binding)
						{
							list.Add(client.ClientData.RebornGoodsDataList[i]);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static GoodsData GetGoodsByID(GameClient client, int goodsID, int bingding, string endTime, ref int startIndex)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					if (startIndex >= client.ClientData.RebornGoodsDataList.Count)
					{
						return null;
					}
					for (int i = startIndex; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == bingding && Global.DateTimeEqual(client.ClientData.RebornGoodsDataList[i].Endtime, endTime))
						{
							startIndex = i + 1;
							return client.ClientData.RebornGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessSpriteResetRebornStoreCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				RebornEquip.ResetStoreRebormGoods(gameClient);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessExtRebornStoreByYuanBaoCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int addGridNum = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int rebornStoreCapacity = RebornEquip.GetRebornStoreCapacity(gameClient);
				if (rebornStoreCapacity >= Global.MaxPortableGridNum)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, gameClient, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
					{
						Global.MaxPortableGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return TCPProcessCmdResults.RESULT_OK;
				}
				RebornEquip.ExtRebornStoreWithYuanBao(pool, gameClient, addGridNum);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessExtRebornBagNumByYuanBaoCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int addGridNum = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int selfBagCapacity = RebornEquip.GetSelfBagCapacity(gameClient);
				if (selfBagCapacity >= Global.MaxBagGridNum)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, gameClient, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
					{
						Global.MaxBagGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int num2 = RebornEquip.ExtRoleRebornBagNumWithYuanBao(pool, gameClient, addGridNum);
				string data2 = string.Format("{0}:{1}:{2}", gameClient.ClientData.RoleID, gameClient.ClientData.RebornBagNum, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessQueryRebornOpenGridCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1 && array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string data2 = string.Format("{0}", gameClient.ClientData.OpenRebornBagTime);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessQueryOpenRebornPortableGridCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string data2 = string.Format("{0}", gameClient.ClientData.OpenRebornGridTime);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static RebornEquipOpcode RebornEquipShowProcess(GameClient client, int nRoleID)
		{
			RebornEquipOpcode result;
			if (client.ClientData.RoleID != nRoleID)
			{
				result = RebornEquipOpcode.RebornShowErr;
			}
			else
			{
				if (client.ClientData.RebornShowEquip == 0)
				{
					client.ClientData.RebornShowEquip = 1;
				}
				else
				{
					client.ClientData.RebornShowEquip = 0;
				}
				GameManager.DBCmdMgr.AddDBCmd(14120, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowEquip), null, client.ServerId);
				GameManager.ClientMgr.NotifyOthersRebornEquipChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = RebornEquipOpcode.RebornShowSucc;
			}
			return result;
		}

		public static RebornEquipOpcode RebornEquipShowModelProcess(GameClient client, int nRoleID)
		{
			RebornEquipOpcode result;
			if (client.ClientData.RoleID != nRoleID)
			{
				result = RebornEquipOpcode.RebornShowErr;
			}
			else
			{
				if (client.ClientData.RebornShowModel == 0)
				{
					client.ClientData.RebornShowModel = 1;
				}
				else
				{
					client.ClientData.RebornShowModel = 0;
				}
				GameManager.DBCmdMgr.AddDBCmd(14122, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowModel), null, client.ServerId);
				GameManager.ClientMgr.NotifyOthersRebornModelChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = RebornEquipOpcode.RebornShowSucc;
			}
			return result;
		}

		public static void NotifyRebornEquipUp(GameClient client, GoodsData gd)
		{
			GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 0, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
		}

		public static List<GoodsData> GetRebornUsingAttackWeaponGoods(GameClient client)
		{
			List<GoodsData> result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using > 0)
						{
							int goodsCatetoriy = Global.GetGoodsCatetoriy(client.ClientData.RebornGoodsDataList[i].GoodsID);
							if (38 == goodsCatetoriy || 37 == goodsCatetoriy)
							{
								list.Add(client.ClientData.RebornGoodsDataList[i]);
							}
						}
					}
				}
				if (list.Count<GoodsData>() > 0)
				{
					result = list;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static GoodsData GetMagicWeaponGoods(List<GoodsData> BaseWeap, bool IsBase)
		{
			GoodsData goodsData = null;
			GoodsData result;
			if (BaseWeap == null || BaseWeap.Count <= 0)
			{
				result = null;
			}
			else
			{
				if (BaseWeap.Count == 1)
				{
					goodsData = BaseWeap[0];
				}
				else if (BaseWeap.Count > 1)
				{
					for (int i = 0; i < BaseWeap.Count; i++)
					{
						if (BaseWeap[i].BagIndex == 0 && IsBase)
						{
							goodsData = BaseWeap[i];
							break;
						}
						if (!IsBase && (RebornEquip.IsRebornEquipShengQi(BaseWeap[i].GoodsID) || RebornEquip.IsRebornEquipShengWu(BaseWeap[i].GoodsID)))
						{
							goodsData = BaseWeap[i];
							break;
						}
					}
					if (null == goodsData)
					{
						for (int i = 0; i < BaseWeap.Count; i++)
						{
							if (BaseWeap[i].BagIndex == 1 && IsBase)
							{
								goodsData = BaseWeap[i];
								break;
							}
							if (!IsBase && (RebornEquip.IsRebornEquipShengQi(BaseWeap[i].GoodsID) || RebornEquip.IsRebornEquipShengWu(BaseWeap[i].GoodsID)))
							{
								goodsData = BaseWeap[i];
								break;
							}
						}
					}
				}
				if (null == goodsData)
				{
					result = null;
				}
				else
				{
					result = goodsData;
				}
			}
			return result;
		}

		public static bool IsWeaponCanAttackOrActtion(GameClient client, out int Code)
		{
			Code = 0;
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			List<GoodsData> usingAttackWeaponGoods = Global.GetUsingAttackWeaponGoods(client);
			List<GoodsData> rebornUsingAttackWeaponGoods = RebornEquip.GetRebornUsingAttackWeaponGoods(client);
			bool result;
			if (54 != mapSceneType && client.ClientData.RebornShowEquip == 0 && (usingAttackWeaponGoods == null || usingAttackWeaponGoods.Count<GoodsData>() < 0))
			{
				Code = 1;
				result = false;
			}
			else if (54 != mapSceneType && client.ClientData.RebornShowEquip == 1 && (rebornUsingAttackWeaponGoods == null || rebornUsingAttackWeaponGoods.Count<GoodsData>() < 0))
			{
				Code = 2;
				result = false;
			}
			else if (54 == mapSceneType && (rebornUsingAttackWeaponGoods == null || rebornUsingAttackWeaponGoods.Count<GoodsData>() < 0))
			{
				Code = 3;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static RebornEquipOpcode RebornEquipAdvanceProcess(GameClient client, int nRoleID, int DBID)
		{
			GoodsData rebornGoodsByDbID = RebornEquip.GetRebornGoodsByDbID(client, DBID);
			RebornEquipOpcode result;
			RebornEquipEvolution rebornEquipEvolution;
			if (rebornGoodsByDbID == null)
			{
				result = RebornEquipOpcode.NotExistRebornEquip;
			}
			else if (rebornGoodsByDbID.Site != 15000)
			{
				result = RebornEquipOpcode.NotInRebornBag;
			}
			else if (!RebornEquip.RebornEquipUp.TryGetValue(rebornGoodsByDbID.GoodsID, out rebornEquipEvolution))
			{
				result = RebornEquipOpcode.NotFindRebornLow;
			}
			else
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10251");
				int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "10250");
				int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "10249");
				lock (rebornEquipEvolution)
				{
					if (roleParamsInt32FromDB < rebornEquipEvolution.NeedNiePan)
					{
						return RebornEquipOpcode.NeedNiePanNotEnough;
					}
					if (roleParamsInt32FromDB2 < rebornEquipEvolution.NeedDuanZao)
					{
						return RebornEquipOpcode.NeedDuanZaoNotEnough;
					}
					if (roleParamsInt32FromDB3 < rebornEquipEvolution.NeedCuiLian)
					{
						return RebornEquipOpcode.NeedCuiLianNotEnough;
					}
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rebornEquipEvolution.NeedEquitID, out systemXmlItem))
					{
						return RebornEquipOpcode.NotHaveUpEquip;
					}
					if (!GameManager.ClientMgr.ModifyRebornNiePanPointValue(client, -rebornEquipEvolution.NeedNiePan, "装备进阶消耗涅槃点", true, true, false))
					{
						return RebornEquipOpcode.NeedNiePanErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornDuanZaoPointValue(client, -rebornEquipEvolution.NeedDuanZao, "装备进阶消耗锻造点", true, true, false))
					{
						return RebornEquipOpcode.NeedDuanZaoErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornCuiLianPointValue(client, -rebornEquipEvolution.NeedCuiLian, "装备进阶消耗淬炼点", true, true, false))
					{
						return RebornEquipOpcode.NeedCuiLianErr;
					}
					double random = Global.GetRandom();
					if (random > rebornEquipEvolution.SuccessRate)
					{
						return RebornEquipOpcode.NotEnoughProb;
					}
					int num = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, rebornEquipEvolution.NewEquitID, 1, rebornGoodsByDbID.Quality, rebornGoodsByDbID.Props, rebornGoodsByDbID.Forge_level, rebornGoodsByDbID.Binding, 15000, rebornGoodsByDbID.Jewellist, true, 1, string.Format("重生装备晋升", new object[0]), false, rebornGoodsByDbID.Endtime, rebornGoodsByDbID.AddPropIndex, rebornGoodsByDbID.BornIndex, rebornGoodsByDbID.Lucky, rebornGoodsByDbID.Strong, rebornGoodsByDbID.ExcellenceInfo, rebornGoodsByDbID.AppendPropLev, rebornGoodsByDbID.ChangeLifeLevForEquip, true, rebornGoodsByDbID.WashProps, rebornGoodsByDbID.ElementhrtsProps, rebornGoodsByDbID.Starttime, rebornGoodsByDbID.JuHunID, true);
					if (num == DBID)
					{
						return RebornEquipOpcode.RebornNewEquipErr;
					}
					string props = rebornGoodsByDbID.Props;
					if (!RebornEquip.RemoveGoodsDataToDb(client, rebornGoodsByDbID))
					{
						return RebornEquipOpcode.RebornLowError;
					}
					GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 4, rebornGoodsByDbID.Id, rebornGoodsByDbID.Using, rebornGoodsByDbID.Site, rebornGoodsByDbID.GCount, rebornGoodsByDbID.BagIndex, 1);
					GoodsData rebornGoodsByDbID2 = RebornEquip.GetRebornGoodsByDbID(client, num);
					rebornGoodsByDbID2.Props = props;
					if (rebornGoodsByDbID2.Using > 0)
					{
						Global.RefreshEquipProp(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
				result = RebornEquipOpcode.RebornUpSucc;
			}
			return result;
		}

		public static int GetGoodDataCategoriyByRebornPerfusion(int Item, int JieZhiHand = 0)
		{
			int result;
			switch (Item)
			{
			case 30:
				result = 1;
				break;
			case 31:
				result = 2;
				break;
			case 32:
				result = 3;
				break;
			case 33:
				result = 4;
				break;
			case 34:
				result = 5;
				break;
			case 35:
				result = 8;
				break;
			case 36:
				if (JieZhiHand == 0)
				{
					result = 6;
				}
				else
				{
					result = 7;
				}
				break;
			case 37:
				result = 9;
				break;
			case 38:
				result = 10;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public static long GetFreeTime()
		{
			return GameManager.systemParamsList.GetParamValueIntByName("EquipQuenchingFreeNum", 0);
		}

		public static int UseGoodAblePerfusion(GameClient client, int HoleSite, int Level, out int Able, out bool isFree)
		{
			Able = 0;
			isFree = false;
			int result;
			if (!RebornEquip.RebornEquipHole.ContainsKey(HoleSite))
			{
				result = -1;
			}
			else if (!RebornEquip.RebornEquipHole[HoleSite].ContainsKey(Level))
			{
				result = -2;
			}
			else
			{
				int num = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][Level].AddStart * 100.0);
				int num2 = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][Level].AddEnd * 100.0);
				if (num < 0 || num2 < 0)
				{
					result = -3;
				}
				else
				{
					if ((long)Global.GetRoleParamsInt32FromDB(client, "10255") < RebornEquip.GetFreeTime())
					{
						isFree = true;
					}
					else
					{
						Dictionary<int, Dictionary<int, GoodsData>> dictionary = new Dictionary<int, Dictionary<int, GoodsData>>();
						lock (dictionary)
						{
							int num3 = 0;
							foreach (KeyValuePair<int, int> keyValuePair in RebornEquip.RebornEquipHole[HoleSite][Level].UseGoods)
							{
								SystemXmlItem systemXmlItem = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(keyValuePair.Key, out systemXmlItem))
								{
									return -4;
								}
								GoodsData goodsByID = RebornEquip.GetGoodsByID(client, keyValuePair.Key);
								if (goodsByID == null)
								{
									return -5;
								}
								Dictionary<int, GoodsData> dictionary2 = new Dictionary<int, GoodsData>();
								dictionary2.Add(keyValuePair.Value, goodsByID);
								num3++;
								dictionary.Add(num3, dictionary2);
							}
							foreach (Dictionary<int, GoodsData> dictionary3 in dictionary.Values)
							{
								foreach (KeyValuePair<int, GoodsData> keyValuePair2 in dictionary3)
								{
									bool flag2;
									if (!RebornStone.RebornUseGoodHasBinding(client, keyValuePair2.Value.GoodsID, keyValuePair2.Key, 1, out flag2))
									{
										return -6;
									}
								}
							}
						}
					}
					Able = Global.GetRandomNumber(num, num2);
					result = 0;
				}
			}
			return result;
		}

		public static RebornEquipData GetNewInfo(GameClient client, int HoleSite)
		{
			return new RebornEquipData
			{
				RoleID = client.ClientData.RebornEquipHoleInfo[HoleSite].RoleID,
				HoleID = client.ClientData.RebornEquipHoleInfo[HoleSite].HoleID,
				Able = client.ClientData.RebornEquipHoleInfo[HoleSite].Able,
				Level = client.ClientData.RebornEquipHoleInfo[HoleSite].Level
			};
		}

		public static void RefreshProps(GameClient client)
		{
			if (client.ClientData.RebornEquipHoleInfo != null && client.ClientData.RebornEquipHoleInfo.Count != 0)
			{
				double[] array = new double[177];
				try
				{
					List<int> list = new List<int>();
					foreach (GoodsData goodsData in client.ClientData.RebornGoodsDataList)
					{
						if (goodsData.Using != 0)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
							{
								int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
								if (intValue >= 30 && intValue <= 38)
								{
									if (intValue == 36)
									{
										list.Add(RebornEquip.GetGoodDataCategoriyByRebornPerfusion(intValue, goodsData.BagIndex));
									}
									else
									{
										list.Add(RebornEquip.GetGoodDataCategoriyByRebornPerfusion(intValue, 0));
									}
								}
							}
						}
					}
					if (list != null && list.Count != 0)
					{
						foreach (KeyValuePair<int, RebornEquipData> keyValuePair in client.ClientData.RebornEquipHoleInfo)
						{
							if (keyValuePair.Value.Level != 0)
							{
								if (list.IndexOf(keyValuePair.Value.HoleID) != -1)
								{
									if (RebornEquip.RebornEquipHole.ContainsKey(keyValuePair.Value.HoleID))
									{
										if (RebornEquip.RebornEquipHole[keyValuePair.Value.HoleID].ContainsKey(keyValuePair.Value.Level))
										{
											foreach (KeyValuePair<int, double> keyValuePair2 in RebornEquip.RebornEquipHole[keyValuePair.Value.HoleID][keyValuePair.Value.Level].Attr)
											{
												array[keyValuePair2.Key] += keyValuePair2.Value;
											}
										}
									}
								}
							}
						}
					}
				}
				finally
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.RebornEquipHole,
						array
					});
				}
			}
		}

		public static RebornPerfusionOpcode RebornEquipHolePerfusionProcess(GameClient client, int HoleSite, out int ClientAble)
		{
			ClientAble = 0;
			RebornPerfusionOpcode result;
			if (!GlobalNew.IsGongNengOpened(client, 116, false))
			{
				LogManager.WriteLog(2, string.Format("灌注功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				result = RebornPerfusionOpcode.NotStart;
			}
			else if (!RebornEquip.RebornEquipHole.ContainsKey(HoleSite))
			{
				result = RebornPerfusionOpcode.NotExsit;
			}
			else
			{
				if (client.ClientData.RebornEquipHoleInfo == null || !client.ClientData.RebornEquipHoleInfo.ContainsKey(HoleSite))
				{
					if (client.ClientData.RebornEquipHoleInfo == null)
					{
						client.ClientData.RebornEquipHoleInfo = new Dictionary<int, RebornEquipData>();
					}
					int num = 0;
					bool flag = false;
					if (0 != RebornEquip.UseGoodAblePerfusion(client, HoleSite, 0, out num, out flag))
					{
						return RebornPerfusionOpcode.MakeAbleErr;
					}
					if (flag)
					{
						if (!GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, 1, "重生装备槽灌注免费次数减少", true, true, false))
						{
							return RebornPerfusionOpcode.PerfusionNumErr;
						}
						client.ClientData.RebornEquipHole += 1L;
					}
					RebornEquipData rebornEquipData = new RebornEquipData();
					rebornEquipData.RoleID = client.ClientData.RoleID;
					rebornEquipData.HoleID = HoleSite;
					rebornEquipData.Level = 0;
					rebornEquipData.Able = num;
					int num2 = Global.sendToDB<int, RebornEquipData>(14123, rebornEquipData, client.ServerId);
					if (num2 < 0)
					{
						LogManager.WriteLog(2, string.Format("灌注插入数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
						return RebornPerfusionOpcode.InsertDataErr;
					}
					client.ClientData.RebornEquipHoleInfo.Add(rebornEquipData.HoleID, rebornEquipData);
					ClientAble = client.ClientData.RebornEquipHoleInfo[rebornEquipData.HoleID].Able;
				}
				else
				{
					if (client.ClientData.RebornEquipHoleInfo[HoleSite].RoleID != client.ClientData.RoleID)
					{
						LogManager.WriteLog(2, string.Format("灌注ID校验出错, 玩家id RoleID={0}, 灌注结构 RoleID={1} 灌注部位 HoleID={2}", client.ClientData.RoleID, client.ClientData.RebornEquipHoleInfo[HoleSite].RoleID, client.ClientData.RebornEquipHoleInfo[HoleSite].HoleID), null, true);
						return RebornPerfusionOpcode.PerfusionInfoErr;
					}
					RebornEquipData rebornEquipData = RebornEquip.GetNewInfo(client, HoleSite);
					if (rebornEquipData == null)
					{
						return RebornPerfusionOpcode.InsertDataErr;
					}
					if (rebornEquipData.Able >= 100)
					{
						return RebornPerfusionOpcode.MaxAble;
					}
					int num = 0;
					bool flag = false;
					if (0 != RebornEquip.UseGoodAblePerfusion(client, HoleSite, client.ClientData.RebornEquipHoleInfo[HoleSite].Level, out num, out flag))
					{
						return RebornPerfusionOpcode.MakeAbleErr;
					}
					rebornEquipData.Able += num;
					if (rebornEquipData.Able > 100)
					{
						rebornEquipData.Able = 100;
					}
					if (flag)
					{
						if (!GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, 1, "重生装备槽灌注免费次数减少", true, true, false))
						{
							return RebornPerfusionOpcode.PerfusionNumErr;
						}
						client.ClientData.RebornEquipHole += 1L;
					}
					int num2 = Global.sendToDB<int, RebornEquipData>(14124, rebornEquipData, client.ServerId);
					if (num2 <= 0)
					{
						LogManager.WriteLog(2, string.Format("灌注更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
						return RebornPerfusionOpcode.UpdateDataErr;
					}
					client.ClientData.RebornEquipHoleInfo[rebornEquipData.HoleID].Able = rebornEquipData.Able;
					ClientAble = client.ClientData.RebornEquipHoleInfo[rebornEquipData.HoleID].Able;
				}
				result = RebornPerfusionOpcode.Succ;
			}
			return result;
		}

		public static RebornPerfusionOpcode RebornEquipHoleAbschreckenProcess(GameClient client, int HoleSite, out int ClientLevel, out int ClientAble)
		{
			ClientLevel = 0;
			ClientAble = 0;
			RebornPerfusionOpcode result;
			if (!GlobalNew.IsGongNengOpened(client, 116, false))
			{
				LogManager.WriteLog(2, string.Format("灌注功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				result = RebornPerfusionOpcode.NotStart;
			}
			else if (client.ClientData.RebornEquipHoleInfo == null || !client.ClientData.RebornEquipHoleInfo.ContainsKey(HoleSite) || RebornEquip.RebornEquipHole == null || !RebornEquip.RebornEquipHole.ContainsKey(HoleSite) || !RebornEquip.RebornEquipHole[HoleSite].ContainsKey(client.ClientData.RebornEquipHoleInfo[HoleSite].Level))
			{
				result = RebornPerfusionOpcode.NotExsit;
			}
			else if (!RebornEquip.RebornEquipHoleLevelMap.ContainsKey(HoleSite))
			{
				result = RebornPerfusionOpcode.NotExsit;
			}
			else if (client.ClientData.RebornEquipHoleInfo[HoleSite].Level >= RebornEquip.RebornEquipHoleLevelMap[HoleSite])
			{
				result = RebornPerfusionOpcode.MaxLevel;
			}
			else
			{
				int num = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][client.ClientData.RebornEquipHoleInfo[HoleSite].Level].AbschreckenUnterwerfen * 100.0);
				if (num < 0 || client.ClientData.RebornEquipHoleInfo[HoleSite].Able < num)
				{
					result = RebornPerfusionOpcode.SuccNotVoll;
				}
				else
				{
					RebornEquipData newInfo = RebornEquip.GetNewInfo(client, HoleSite);
					if (newInfo == null)
					{
						result = RebornPerfusionOpcode.InsertDataErr;
					}
					else
					{
						int randomNumber = Global.GetRandomNumber(1, 101);
						if (randomNumber > client.ClientData.RebornEquipHoleInfo[HoleSite].Able)
						{
							int num2 = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][client.ClientData.RebornEquipHoleInfo[HoleSite].Level].SubStart * 100.0);
							int num3 = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][client.ClientData.RebornEquipHoleInfo[HoleSite].Level].SubEnd * 100.0);
							if (num2 < 0 || num3 < 0)
							{
								result = RebornPerfusionOpcode.AbschreckenXmlErr;
							}
							else
							{
								int randomNumber2 = Global.GetRandomNumber(num2, num3);
								if (randomNumber2 < 0)
								{
									result = RebornPerfusionOpcode.AbschreckenXmlErr;
								}
								else
								{
									if (newInfo.Able == 0 || newInfo.Able <= randomNumber2)
									{
										newInfo.Able = 0;
									}
									else
									{
										newInfo.Able -= randomNumber2;
									}
									int num4 = Global.sendToDB<int, RebornEquipData>(14124, newInfo, client.ServerId);
									if (num4 <= 0)
									{
										LogManager.WriteLog(2, string.Format("淬炼失败更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
										result = RebornPerfusionOpcode.UpdateDataErr;
									}
									else
									{
										client.ClientData.RebornEquipHoleInfo[HoleSite].Able = newInfo.Able;
										ClientAble = client.ClientData.RebornEquipHoleInfo[HoleSite].Able;
										ClientLevel = client.ClientData.RebornEquipHoleInfo[HoleSite].Level;
										result = RebornPerfusionOpcode.AbschreckenFail;
									}
								}
							}
						}
						else
						{
							double random = Global.GetRandom();
							if (random <= RebornEquip.RebornEquipHole[HoleSite][newInfo.Level].SturmGaiLv)
							{
								newInfo.Level += RebornEquip.RebornEquipHole[HoleSite][newInfo.Level].SturmLevel;
								if (newInfo.Level > RebornEquip.RebornEquipHoleLevelMap[HoleSite])
								{
									newInfo.Level = RebornEquip.RebornEquipHoleLevelMap[HoleSite];
								}
							}
							else
							{
								newInfo.Level++;
							}
							newInfo.Able = 0;
							int num4 = Global.sendToDB<int, RebornEquipData>(14124, newInfo, client.ServerId);
							if (num4 <= 0)
							{
								LogManager.WriteLog(2, string.Format("淬炼成功更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
								result = RebornPerfusionOpcode.UpdateDataErr;
							}
							else
							{
								client.ClientData.RebornEquipHoleInfo[HoleSite].Level = newInfo.Level;
								client.ClientData.RebornEquipHoleInfo[HoleSite].Able = 0;
								ClientAble = client.ClientData.RebornEquipHoleInfo[HoleSite].Able;
								ClientLevel = client.ClientData.RebornEquipHoleInfo[HoleSite].Level;
								Global.RefreshEquipProp(client);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								result = RebornPerfusionOpcode.Succ;
							}
						}
					}
				}
			}
			return result;
		}

		public static double GetRebornEquipRate()
		{
			return Math.Min(GameManager.systemParamsList.GetParamValueDoubleByName("ChongShengReturnNum", 0.0), 1.0);
		}

		public static bool GetRebornEquipUseGoods(int goodsId, out int NiePan, out int DuanZao, out int CuiLian)
		{
			NiePan = 0;
			DuanZao = 0;
			CuiLian = 0;
			bool result;
			if (RebornEquip.Evolution == null)
			{
				result = false;
			}
			else
			{
				foreach (KeyValuePair<int, RebornEquipEvolution> keyValuePair in RebornEquip.Evolution)
				{
					if (keyValuePair.Value.NewEquitID == goodsId)
					{
						double rebornEquipRate = RebornEquip.GetRebornEquipRate();
						NiePan = (int)Math.Floor((double)keyValuePair.Value.NeedNiePan * rebornEquipRate);
						DuanZao = (int)Math.Floor((double)keyValuePair.Value.NeedDuanZao * rebornEquipRate);
						CuiLian = (int)Math.Floor((double)keyValuePair.Value.NeedCuiLian * rebornEquipRate);
					}
				}
				result = true;
			}
			return result;
		}

		public static bool SetRebornEquipUseGoods(GameClient client, int NiePan, int DuanZao, int Cuiian)
		{
			if (NiePan > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornNiePanPointValue(client, NiePan, "装备回收获得涅槃点", true, true, false))
				{
					return false;
				}
			}
			if (DuanZao > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornDuanZaoPointValue(client, DuanZao, "装备回收获得锻造点", true, true, false))
				{
					return false;
				}
			}
			if (Cuiian > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornCuiLianPointValue(client, Cuiian, "装备回收获得淬炼点", true, true, false))
				{
					return false;
				}
			}
			return true;
		}

		public static Dictionary<int, RebornEquipEvolution> Evolution = new Dictionary<int, RebornEquipEvolution>();

		public static Dictionary<int, RebornSuperiorDrop> SuperiorDrop = new Dictionary<int, RebornSuperiorDrop>();

		public static Dictionary<int, RebornSuperiorType> SuperiorType = new Dictionary<int, RebornSuperiorType>();

		public static Dictionary<int, RebornEquipXmlStruct> EquipSQSW = new Dictionary<int, RebornEquipXmlStruct>();

		public static Dictionary<int, RebornEquipEvolution> RebornEquipUp = new Dictionary<int, RebornEquipEvolution>();

		public static Dictionary<int, Dictionary<int, RebornQuenching>> RebornEquipHole = new Dictionary<int, Dictionary<int, RebornQuenching>>();

		public static Dictionary<int, int> RebornEquipHoleLevelMap = new Dictionary<int, int>();

		public static Dictionary<int, List<int>> ExtPropIndexMap = new Dictionary<int, List<int>>();

		public static object Mutex = new object();

		private static RebornEquip instance = new RebornEquip();
	}
}
