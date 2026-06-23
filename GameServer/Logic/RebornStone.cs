using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Logic.Reborn;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class RebornStone
	{
		public static RebornStone getInstance()
		{
			return RebornStone.instance;
		}

		public static bool ParseRebornStoneConfig()
		{
			string text = Global.GameResPath(RebornStoneConst.RebornEquipDaKong);
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				Dictionary<int, RebornHoleStruct> dictionary = new Dictionary<int, RebornHoleStruct>();
				Dictionary<Dictionary<int, int>, int> dictionary2 = new Dictionary<Dictionary<int, int>, int>();
				Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					RebornHoleStruct rebornHoleStruct = new RebornHoleStruct();
					Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
					Dictionary<int, int> dictionary5 = new Dictionary<int, int>();
					Dictionary<double, int> dictionary6 = new Dictionary<double, int>();
					rebornHoleStruct.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					rebornHoleStruct.RebornEquipDengJi = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ZhuangBeiDengJie"));
					rebornHoleStruct.RebornEquipPinZhi = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ZhuangBeiPinZhi"));
					rebornHoleStruct.Count = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "DaKongShuLiang"));
					string[] array = Global.GetSafeAttributeStr(xml, "XiaoHaoDaoJu").Split(new char[]
					{
						','
					});
					dictionary4.Add(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
					rebornHoleStruct.UseGoods = dictionary4;
					double num = 0.0;
					string[] array2 = Global.GetSafeAttributeStr(xml, "GaiLv").Split(new char[]
					{
						'|'
					});
					int num2 = 0;
					foreach (string text2 in array2)
					{
						string[] array4 = text2.Split(new char[]
						{
							','
						});
						double num3 = Convert.ToDouble(array4[1]);
						if (num3 != 0.0)
						{
							int num4 = Convert.ToInt32(array4[0]);
							if (dictionary3.TryGetValue(rebornHoleStruct.RebornEquipPinZhi, out num2))
							{
								if (num2 < num4)
								{
									dictionary3[rebornHoleStruct.RebornEquipPinZhi] = num4;
								}
							}
							else
							{
								dictionary3.Add(rebornHoleStruct.RebornEquipPinZhi, num4);
							}
							num += num3;
							dictionary6.Add(num, num4);
						}
					}
					rebornHoleStruct.GaiLv = dictionary6;
					dictionary5.Add(rebornHoleStruct.RebornEquipPinZhi, rebornHoleStruct.RebornEquipDengJi);
					dictionary2.Add(dictionary5, rebornHoleStruct.ID);
					dictionary.Add(rebornHoleStruct.ID, rebornHoleStruct);
				}
				RebornStone.RebornHoleStr = dictionary;
				RebornStone.ItemIDMap = dictionary2;
				RebornStone.RebornEquipHoleMap = dictionary3;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (RebornStone.RebornHoleStr == null || RebornStone.ItemIDMap == null || RebornStone.RebornEquipHoleMap == null)
			{
				result = false;
			}
			else
			{
				Dictionary<int, double> dictionary7 = new Dictionary<int, double>();
				List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("DaKongShuXing", '|');
				try
				{
					foreach (string text2 in paramValueStringListByName)
					{
						string[] array5 = text2.Split(new char[]
						{
							','
						});
						dictionary7.Add(Convert.ToInt32(array5[0]), Convert.ToDouble(array5[1]));
					}
					RebornStone.RebornHoleExpend = dictionary7;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				if (RebornStone.RebornHoleExpend == null)
				{
					result = false;
				}
				else
				{
					text = Global.GameResPath(RebornStoneConst.RebornStorn);
					xelement = XElement.Load(text);
					if (null == xelement)
					{
						LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
					}
					try
					{
						Dictionary<int, RebornStornStruct> dictionary8 = new Dictionary<int, RebornStornStruct>();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							RebornStornStruct rebornStornStruct = new RebornStornStruct();
							Dictionary<int, double> dictionary9 = new Dictionary<int, double>();
							rebornStornStruct.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
							rebornStornStruct.StornID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "BaoShiID"));
							rebornStornStruct.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Type"));
							rebornStornStruct.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Level"));
							rebornStornStruct.FengYinJingShi = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "FengYinJingShi"));
							rebornStornStruct.ChongShengJingShi = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ChongShengJingShi"));
							rebornStornStruct.XuanCaiJingShi = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "XuanCaiJingShi"));
							string[] array6 = Global.GetSafeAttributeStr(xml, "ShuXing").Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array6)
							{
								string[] array5 = text2.Split(new char[]
								{
									','
								});
								dictionary9.Add((int)ConfigParser.GetPropIndexByPropName(array5[0]), Convert.ToDouble(array5[1]));
							}
							rebornStornStruct.Attr = dictionary9;
							dictionary8.Add(rebornStornStruct.StornID, rebornStornStruct);
						}
						RebornStone.RebornStoneXml = dictionary8;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					if (RebornStone.RebornStoneXml == null)
					{
						result = false;
					}
					else
					{
						text = Global.GameResPath(RebornStoneConst.RebornStornXuanCaiCompound);
						xelement = XElement.Load(text);
						if (null == xelement)
						{
							LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
						}
						try
						{
							Dictionary<int, RebornStornComp> dictionary10 = new Dictionary<int, RebornStornComp>();
							IEnumerable<XElement> enumerable = xelement.Elements();
							foreach (XElement xml in enumerable)
							{
								RebornStornComp rebornStornComp = new RebornStornComp();
								List<int> list = new List<int>();
								rebornStornComp.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
								rebornStornComp.StornID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "BaoShiID"));
								rebornStornComp.StornAID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HeChengBaoShiId"));
								string[] array7 = Global.GetSafeAttributeStr(xml, "HeChengXiaoHao").Split(new char[]
								{
									','
								});
								foreach (string text2 in array7)
								{
									list.Add(Convert.ToInt32(text2));
								}
								rebornStornComp.CompNeed = list;
								dictionary10.Add(rebornStornComp.StornID, rebornStornComp);
							}
							RebornStone.RebornStoneComplex = dictionary10;
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
						if (RebornStone.RebornStoneComplex == null)
						{
							result = false;
						}
						else
						{
							text = Global.GameResPath(RebornStoneConst.RebornStornXuanCaiAttr);
							xelement = XElement.Load(text);
							if (null == xelement)
							{
								LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
							}
							try
							{
								Dictionary<int, Dictionary<int, RebornXuanCaiStorn>> dictionary11 = new Dictionary<int, Dictionary<int, RebornXuanCaiStorn>>();
								IEnumerable<XElement> enumerable = xelement.Elements();
								foreach (XElement xml in enumerable)
								{
									Dictionary<int, Dictionary<int, RebornXuanCaiStorn>> dictionary12 = new Dictionary<int, Dictionary<int, RebornXuanCaiStorn>>();
									Dictionary<int, double> dictionary13 = new Dictionary<int, double>();
									Dictionary<int, int> dictionary14 = new Dictionary<int, int>();
									RebornXuanCaiStorn rebornXuanCaiStorn = new RebornXuanCaiStorn();
									rebornXuanCaiStorn.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
									rebornXuanCaiStorn.StornID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "DaoJuID"));
									rebornXuanCaiStorn.StornLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Level"));
									string[] array8 = Global.GetSafeAttributeStr(xml, "JiHuoShuXing").Split(new char[]
									{
										'|'
									});
									foreach (string text2 in array8)
									{
										string[] array5 = text2.Split(new char[]
										{
											','
										});
										dictionary13.Add((int)ConfigParser.GetPropIndexByPropName(array5[0]), Convert.ToDouble(array5[1]));
									}
									rebornXuanCaiStorn.StornAttr = dictionary13;
									string[] array9 = Global.GetSafeAttributeStr(xml, "JiHuoTiaoJian").Split(new char[]
									{
										'|'
									});
									foreach (string text2 in array9)
									{
										string[] array5 = text2.Split(new char[]
										{
											','
										});
										dictionary14.Add(Convert.ToInt32(array5[0]), Convert.ToInt32(array5[1]));
									}
									rebornXuanCaiStorn.StornActivity = dictionary14;
									if (dictionary11.ContainsKey(rebornXuanCaiStorn.StornID))
									{
										dictionary11[rebornXuanCaiStorn.StornID].Add(rebornXuanCaiStorn.ID, rebornXuanCaiStorn);
									}
									else
									{
										Dictionary<int, RebornXuanCaiStorn> dictionary15 = new Dictionary<int, RebornXuanCaiStorn>();
										dictionary15.Add(rebornXuanCaiStorn.ID, rebornXuanCaiStorn);
										dictionary11.Add(rebornXuanCaiStorn.StornID, dictionary15);
									}
								}
								RebornStone.RebornStoneActiveAttr = dictionary11;
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
							result = (RebornStone.RebornStoneActiveAttr != null);
						}
					}
				}
			}
			return result;
		}

		public static double GetExpandValue(int HoleSuit)
		{
			double num;
			double result;
			if (RebornStone.RebornHoleExpend.TryGetValue(HoleSuit, out num))
			{
				result = num;
			}
			else
			{
				result = 1.0;
			}
			return result;
		}

		public static int MakeHoleQualityOne(int Suit, int Quality)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			dictionary.Add(Quality, Suit);
			foreach (KeyValuePair<Dictionary<int, int>, int> keyValuePair in RebornStone.ItemIDMap)
			{
				foreach (KeyValuePair<int, int> keyValuePair2 in keyValuePair.Key)
				{
					if (keyValuePair2.Key == Quality && keyValuePair2.Value == Suit)
					{
						RebornHoleStruct rebornHoleStruct;
						if (RebornStone.RebornHoleStr.TryGetValue(keyValuePair.Value, out rebornHoleStruct))
						{
							double random = Global.GetRandom();
							foreach (KeyValuePair<double, int> keyValuePair3 in rebornHoleStruct.GaiLv)
							{
								if (random < keyValuePair3.Key)
								{
									return keyValuePair3.Value;
								}
							}
						}
					}
				}
			}
			return 0;
		}

		public Dictionary<int, Dictionary<int, int>> MakeHole(Dictionary<double, int> GaiLv, int Site)
		{
			Dictionary<int, Dictionary<int, int>> result;
			if (GaiLv == null)
			{
				result = null;
			}
			else
			{
				Dictionary<int, Dictionary<int, int>> dictionary = new Dictionary<int, Dictionary<int, int>>();
				double random = Global.GetRandom();
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
				foreach (KeyValuePair<double, int> keyValuePair in GaiLv)
				{
					if (random < keyValuePair.Key)
					{
						dictionary2.Add(keyValuePair.Value, 0);
						dictionary.Add(Site, dictionary2);
						break;
					}
				}
				result = dictionary;
			}
			return result;
		}

		public static int GetGoodsHoleInfo(GoodsData gd, int Site, out Dictionary<int, Dictionary<int, int>> HoleInfo, SystemXmlItem systemGoods)
		{
			HoleInfo = null;
			int intValue = systemGoods.GetIntValue("SuitID", -1);
			int currGoodsQuality = RebornEquip.GetCurrGoodsQuality(gd);
			lock (RebornStone.RebornHoleStr)
			{
				foreach (KeyValuePair<int, RebornHoleStruct> keyValuePair in RebornStone.RebornHoleStr)
				{
					if (keyValuePair.Value.RebornEquipDengJi == intValue && currGoodsQuality == keyValuePair.Value.RebornEquipPinZhi)
					{
						HoleInfo = RebornStone.getInstance().MakeHole(keyValuePair.Value.GaiLv, Site);
						if (HoleInfo == null)
						{
							return 0;
						}
						return keyValuePair.Key;
					}
				}
			}
			return 0;
		}

		public static string MakeHoleInfo(Dictionary<int, Dictionary<int, int>> HoleInfo, int XuanCaiID)
		{
			string text = "";
			lock (HoleInfo)
			{
				Dictionary<int, int> dictionary;
				if (HoleInfo.TryGetValue(-1, out dictionary))
				{
					int key;
					if (dictionary.TryGetValue(-1, out key))
					{
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(key, out systemXmlItem))
						{
							HoleInfo.Remove(-1);
						}
					}
				}
				if (XuanCaiID != 0)
				{
					text = "-1_-1_" + XuanCaiID.ToString() + "|";
				}
				foreach (KeyValuePair<int, Dictionary<int, int>> keyValuePair in HoleInfo)
				{
					foreach (KeyValuePair<int, int> keyValuePair2 in keyValuePair.Value)
					{
						text = string.Concat(new string[]
						{
							text,
							keyValuePair.Key.ToString(),
							"_",
							keyValuePair2.Key.ToString(),
							"_",
							keyValuePair2.Value.ToString(),
							"|"
						});
					}
				}
			}
			return text;
		}

		public static Dictionary<int, Dictionary<int, int>> ParessMakeHoleInfo(string HoleInfoStr)
		{
			Dictionary<int, Dictionary<int, int>> dictionary = new Dictionary<int, Dictionary<int, int>>();
			string[] array = HoleInfoStr.Split(new char[]
			{
				'|'
			});
			foreach (string text in array)
			{
				if (text.Length >= 3)
				{
					Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
					string[] array3 = text.Split(new char[]
					{
						'_'
					});
					if (array3.Length >= 3)
					{
						dictionary2.Add(Convert.ToInt32(array3[1]), Convert.ToInt32(array3[2]));
						int key = Convert.ToInt32(array3[0]);
						if (!dictionary.ContainsKey(key))
						{
							dictionary.Add(key, dictionary2);
						}
					}
				}
			}
			return dictionary;
		}

		public static bool UpdateGoodsPropsToDB(GameClient client, GoodsData goodsData)
		{
			string[] array = null;
			string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
			{
				client.ClientData.RoleID,
				goodsData.Id,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				goodsData.Props,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				goodsData.Binding,
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
				GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, goodsData.Id, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, 1);
				result = true;
			}
			return result;
		}

		public static Dictionary<int, int> GetUseGoodsByEquipSuitAndQuality(int Suit, int Quality)
		{
			foreach (KeyValuePair<int, RebornHoleStruct> keyValuePair in RebornStone.RebornHoleStr)
			{
				if (keyValuePair.Value.RebornEquipDengJi == Suit && keyValuePair.Value.RebornEquipPinZhi == Quality)
				{
					return keyValuePair.Value.UseGoods;
				}
			}
			return null;
		}

		public static bool RebornHoleRemoveUseGoods(GameClient client, GoodsData UseGood, int Count, out bool bUsedBinding)
		{
			bUsedBinding = false;
			bool result;
			if (UseGood != null && UseGood.GCount >= Count)
			{
				bool flag = false;
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, UseGood, Count, false, out bUsedBinding, out flag, false))
				{
					result = false;
				}
				else
				{
					GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 4, UseGood.Id, UseGood.Using, UseGood.Site, UseGood.GCount, UseGood.BagIndex, 1);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool RebornUseCaleMothed(Dictionary<int, Dictionary<bool, GoodsData>> GoodsDict, Dictionary<int, Dictionary<int, GoodsData>> UseGoodInfo, int Count)
		{
			int num = 0;
			bool flag = false;
			foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> keyValuePair in GoodsDict)
			{
				using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						KeyValuePair<bool, GoodsData> keyValuePair2 = enumerator2.Current;
						if (keyValuePair2.Key)
						{
							Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
							dictionary.Add(Count, keyValuePair2.Value);
							UseGoodInfo.Add(keyValuePair2.Value.Id, dictionary);
							flag = true;
						}
						else
						{
							num += keyValuePair2.Value.GCount;
							if (!flag && num > Count)
							{
								Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
								dictionary.Add(keyValuePair2.Value.GCount - num + Count, keyValuePair2.Value);
								UseGoodInfo.Add(keyValuePair2.Value.Id, dictionary);
								flag = true;
							}
							else
							{
								Dictionary<int, GoodsData> dictionary2 = new Dictionary<int, GoodsData>();
								dictionary2.Add(keyValuePair2.Value.GCount, keyValuePair2.Value);
								UseGoodInfo.Add(keyValuePair2.Value.Id, dictionary2);
								if (!flag && num == Count)
								{
									flag = true;
								}
							}
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		public static bool RebornUseGoodHasBinding(GameClient client, int UseGoodID, int Count, int Binding, out bool EquipBindUse)
		{
			EquipBindUse = false;
			bool result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = false;
			}
			else if (Count == 0)
			{
				result = true;
			}
			else
			{
				Dictionary<int, Dictionary<bool, GoodsData>> dictionary = new Dictionary<int, Dictionary<bool, GoodsData>>();
				Dictionary<int, Dictionary<bool, GoodsData>> dictionary2 = new Dictionary<int, Dictionary<bool, GoodsData>>();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
				{
					if (client.ClientData.RebornGoodsDataList[i].Using < 1)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == UseGoodID)
						{
							if (client.ClientData.RebornGoodsDataList[i].Binding == 1)
							{
								Dictionary<bool, GoodsData> dictionary3 = new Dictionary<bool, GoodsData>();
								if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
								{
									dictionary3.Add(true, client.ClientData.RebornGoodsDataList[i]);
								}
								else
								{
									dictionary3.Add(false, client.ClientData.RebornGoodsDataList[i]);
								}
								dictionary.Add(client.ClientData.RebornGoodsDataList[i].Id, dictionary3);
								num += client.ClientData.RebornGoodsDataList[i].GCount;
							}
							else
							{
								Dictionary<bool, GoodsData> dictionary4 = new Dictionary<bool, GoodsData>();
								if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
								{
									dictionary4.Add(true, client.ClientData.RebornGoodsDataList[i]);
								}
								else
								{
									dictionary4.Add(false, client.ClientData.RebornGoodsDataList[i]);
								}
								dictionary2.Add(client.ClientData.RebornGoodsDataList[i].Id, dictionary4);
								num2 += client.ClientData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
				if (num + num2 < Count)
				{
					result = false;
				}
				else
				{
					Dictionary<int, Dictionary<int, GoodsData>> dictionary5 = new Dictionary<int, Dictionary<int, GoodsData>>();
					lock (dictionary5)
					{
						if (Binding == 1)
						{
							if (num >= Count)
							{
								if (dictionary != null && dictionary.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(dictionary, dictionary5, Count))
									{
										return false;
									}
								}
							}
							else
							{
								int num3 = 0;
								if (dictionary != null && dictionary.Count > 0)
								{
									foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> keyValuePair in dictionary)
									{
										using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
										{
											if (enumerator2.MoveNext())
											{
												KeyValuePair<bool, GoodsData> keyValuePair2 = enumerator2.Current;
												Dictionary<int, GoodsData> dictionary6 = new Dictionary<int, GoodsData>();
												dictionary6.Add(keyValuePair2.Value.GCount, keyValuePair2.Value);
												dictionary5.Add(keyValuePair2.Value.Id, dictionary6);
												num3 += keyValuePair2.Value.GCount;
											}
										}
									}
								}
								int num4 = Count - num3;
								if (num4 <= 0)
								{
									return false;
								}
								if (dictionary2 != null && dictionary2.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(dictionary2, dictionary5, num4))
									{
										return false;
									}
								}
							}
						}
						else
						{
							if (dictionary2 == null)
							{
								return false;
							}
							if (num2 >= Count)
							{
								if (dictionary2 != null && dictionary2.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(dictionary2, dictionary5, Count))
									{
										return false;
									}
								}
							}
							else
							{
								int num4 = 0;
								if (dictionary2 != null && dictionary2.Count > 0)
								{
									foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> keyValuePair in dictionary2)
									{
										using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
										{
											if (enumerator2.MoveNext())
											{
												KeyValuePair<bool, GoodsData> keyValuePair2 = enumerator2.Current;
												Dictionary<int, GoodsData> dictionary6 = new Dictionary<int, GoodsData>();
												dictionary6.Add(keyValuePair2.Value.GCount, keyValuePair2.Value);
												dictionary5.Add(keyValuePair2.Value.Id, dictionary6);
												num4 += keyValuePair2.Value.GCount;
											}
										}
									}
								}
								int num5 = Count - num4;
								if (num5 <= 0)
								{
									return false;
								}
								if (dictionary != null && dictionary.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(dictionary, dictionary5, num5))
									{
										return false;
									}
								}
							}
						}
						foreach (KeyValuePair<int, Dictionary<int, GoodsData>> keyValuePair3 in dictionary5)
						{
							using (Dictionary<int, GoodsData>.Enumerator enumerator4 = keyValuePair3.Value.GetEnumerator())
							{
								if (enumerator4.MoveNext())
								{
									KeyValuePair<int, GoodsData> keyValuePair4 = enumerator4.Current;
									if (!RebornStone.RebornHoleRemoveUseGoods(client, keyValuePair4.Value, keyValuePair4.Key, out EquipBindUse))
									{
										return false;
									}
								}
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static Dictionary<int, Dictionary<int, GoodsData>> GetDictRebornUseGoodHasBinding(GameClient client, int UseGoodID, int Count, int Binding, out bool EquipBindUse)
		{
			EquipBindUse = false;
			Dictionary<int, Dictionary<int, GoodsData>> result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = null;
			}
			else if (Count == 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, Dictionary<bool, GoodsData>> dictionary = new Dictionary<int, Dictionary<bool, GoodsData>>();
				Dictionary<int, Dictionary<bool, GoodsData>> dictionary2 = new Dictionary<int, Dictionary<bool, GoodsData>>();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < client.ClientData.RebornBagNum; i++)
				{
					if (i >= client.ClientData.RebornGoodsDataList.Count)
					{
						break;
					}
					if (client.ClientData.RebornGoodsDataList[i].GoodsID == UseGoodID)
					{
						if (client.ClientData.RebornGoodsDataList[i].Binding == 1)
						{
							Dictionary<bool, GoodsData> dictionary3 = new Dictionary<bool, GoodsData>();
							if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
							{
								dictionary3.Add(true, client.ClientData.RebornGoodsDataList[i]);
							}
							else
							{
								dictionary3.Add(false, client.ClientData.RebornGoodsDataList[i]);
							}
							dictionary.Add(client.ClientData.RebornGoodsDataList[i].Id, dictionary3);
							num += client.ClientData.RebornGoodsDataList[i].GCount;
						}
						else
						{
							Dictionary<bool, GoodsData> dictionary4 = new Dictionary<bool, GoodsData>();
							if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
							{
								dictionary4.Add(true, client.ClientData.RebornGoodsDataList[i]);
							}
							else
							{
								dictionary4.Add(false, client.ClientData.RebornGoodsDataList[i]);
							}
							dictionary2.Add(client.ClientData.RebornGoodsDataList[i].Id, dictionary4);
							num2 += client.ClientData.RebornGoodsDataList[i].GCount;
						}
					}
				}
				if (num + num2 < Count)
				{
					result = null;
				}
				else
				{
					Dictionary<int, Dictionary<int, GoodsData>> dictionary5 = new Dictionary<int, Dictionary<int, GoodsData>>();
					lock (dictionary5)
					{
						if (Binding == 0)
						{
							if (num >= Count)
							{
								if (dictionary != null && dictionary.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(dictionary, dictionary5, Count))
									{
										return null;
									}
								}
							}
							else
							{
								int num3 = 0;
								if (dictionary != null && dictionary.Count > 0)
								{
									foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> keyValuePair in dictionary)
									{
										using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
										{
											if (enumerator2.MoveNext())
											{
												KeyValuePair<bool, GoodsData> keyValuePair2 = enumerator2.Current;
												Dictionary<int, GoodsData> dictionary6 = new Dictionary<int, GoodsData>();
												dictionary6.Add(keyValuePair2.Value.GCount, keyValuePair2.Value);
												dictionary5.Add(keyValuePair2.Value.Id, dictionary6);
												num3 += keyValuePair2.Value.GCount;
											}
										}
									}
								}
								int num4 = Count - num3;
								if (num4 <= 0)
								{
									return null;
								}
								if (dictionary2 != null && dictionary2.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(dictionary2, dictionary5, num4))
									{
										return null;
									}
								}
							}
						}
						else
						{
							if (dictionary == null)
							{
								return null;
							}
							if (num < Count)
							{
								return null;
							}
							if (dictionary != null && dictionary.Count > 0)
							{
								if (!RebornStone.RebornUseCaleMothed(dictionary, dictionary5, Count))
								{
									return null;
								}
							}
						}
					}
					result = dictionary5;
				}
			}
			return result;
		}

		public static void ActiveXuanCaiAttr(GameClient client, GoodsData goodsData, Dictionary<int, int> Active, int StoneID)
		{
			double[] array = new double[177];
			Dictionary<int, RebornXuanCaiStorn> dictionary;
			if (RebornStone.RebornStoneActiveAttr.TryGetValue(StoneID, out dictionary))
			{
				lock (dictionary)
				{
					Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
					Dictionary<int, double> dictionary3 = new Dictionary<int, double>();
					int num = 0;
					int num2 = 0;
					foreach (RebornXuanCaiStorn rebornXuanCaiStorn in dictionary.Values)
					{
						foreach (KeyValuePair<int, int> keyValuePair in rebornXuanCaiStorn.StornActivity)
						{
							int num3;
							if (Active.TryGetValue(keyValuePair.Key, out num3))
							{
								if (num3 >= keyValuePair.Value)
								{
									bool flag2;
									if (rebornXuanCaiStorn.StornActivity.Count >= 2)
									{
										num2++;
										flag2 = true;
									}
									else
									{
										flag2 = false;
									}
									int num4 = 0;
									foreach (KeyValuePair<int, double> keyValuePair2 in rebornXuanCaiStorn.StornAttr)
									{
										if (!flag2)
										{
											if (!dictionary2.ContainsKey(keyValuePair2.Key))
											{
												dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value);
											}
											else
											{
												Dictionary<int, double> dictionary4;
												int key;
												(dictionary4 = dictionary2)[key = keyValuePair2.Key] = dictionary4[key] + keyValuePair2.Value;
											}
										}
										else
										{
											num4++;
											if (num2 == num4)
											{
												if (!dictionary3.ContainsKey(keyValuePair2.Key))
												{
													dictionary3.Add(keyValuePair2.Key, keyValuePair2.Value);
													num++;
												}
												else
												{
													Dictionary<int, double> dictionary4;
													int key;
													(dictionary4 = dictionary3)[key = keyValuePair2.Key] = dictionary4[key] + keyValuePair2.Value;
													num++;
												}
											}
										}
									}
								}
							}
						}
					}
					if (dictionary2 != null)
					{
						foreach (KeyValuePair<int, double> keyValuePair3 in dictionary2)
						{
							array[keyValuePair3.Key] = keyValuePair3.Value;
						}
					}
					if (dictionary3 != null && num == 2)
					{
						foreach (KeyValuePair<int, double> keyValuePair3 in dictionary3)
						{
							array[keyValuePair3.Key] = keyValuePair3.Value;
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.RebornXuanCaiStone,
					array
				});
			}
		}

		public static bool IsXuanCaiStone(GoodsData goodsData, out GoodsData OutGoodsData, out int XuanCaiStone)
		{
			XuanCaiStone = 0;
			OutGoodsData = null;
			SystemXmlItem systemXmlItem;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
			{
				result = false;
			}
			else if (37 != systemXmlItem.GetIntValue("Categoriy", -1))
			{
				result = false;
			}
			else
			{
				Dictionary<int, Dictionary<int, int>> dictionary = RebornStone.ParessMakeHoleInfo(goodsData.Props);
				if (dictionary == null)
				{
					result = false;
				}
				else
				{
					int num = 0;
					lock (dictionary)
					{
						Dictionary<int, int> dictionary2;
						if (!dictionary.TryGetValue(-1, out dictionary2))
						{
							return false;
						}
						if (!dictionary2.TryGetValue(-1, out num))
						{
							return false;
						}
					}
					SystemXmlItem systemXmlItem2;
					if (num == 0)
					{
						result = false;
					}
					else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num, out systemXmlItem2))
					{
						result = false;
					}
					else if (960 != systemXmlItem2.GetIntValue("Categoriy", -1))
					{
						result = false;
					}
					else
					{
						XuanCaiStone = num;
						OutGoodsData = goodsData;
						result = true;
					}
				}
			}
			return result;
		}

		public static void VoidXuanCaiProps(GameClient client)
		{
			double[] array = new double[177];
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.RebornXuanCaiStone,
				array
			});
		}

		public static void RefreshProps(GameClient client, Dictionary<int, double> StoneAllAttr)
		{
			double[] array = new double[177];
			if (StoneAllAttr != null && StoneAllAttr.Count > 0)
			{
				foreach (KeyValuePair<int, double> keyValuePair in StoneAllAttr)
				{
					array[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.RebornStone,
				array
			});
		}

		public static void GetRefreshProps(GameClient client, GoodsData goodsData, Dictionary<int, int> Active, Dictionary<int, double> StoneAllAttr)
		{
			if (!string.IsNullOrEmpty(goodsData.Props))
			{
				Dictionary<int, Dictionary<int, int>> dictionary = RebornStone.ParessMakeHoleInfo(goodsData.Props);
				if (dictionary != null)
				{
					lock (dictionary)
					{
						foreach (Dictionary<int, int> dictionary2 in dictionary.Values)
						{
							foreach (KeyValuePair<int, int> keyValuePair in dictionary2)
							{
								RebornStornStruct rebornStornStruct;
								if (keyValuePair.Value != 0 && RebornStone.RebornStoneXml.TryGetValue(keyValuePair.Value, out rebornStornStruct))
								{
									double num;
									if (!RebornStone.RebornHoleExpend.TryGetValue(keyValuePair.Key, out num))
									{
										num = 0.0;
									}
									int num2;
									if (RebornEquip.IsRebornEquipAttackOrDefense(goodsData.GoodsID, out num2))
									{
										int num3 = 0;
										foreach (KeyValuePair<int, double> keyValuePair2 in rebornStornStruct.Attr)
										{
											if (num3 == 3)
											{
												break;
											}
											if (num3 == num2)
											{
												if (StoneAllAttr.ContainsKey(keyValuePair2.Key))
												{
													int key;
													StoneAllAttr[key = keyValuePair2.Key] = StoneAllAttr[key] + keyValuePair2.Value * num;
												}
												else
												{
													StoneAllAttr.Add(keyValuePair2.Key, keyValuePair2.Value * num);
												}
											}
											num3++;
										}
										if (Active.ContainsKey(rebornStornStruct.Type))
										{
											int key;
											Active[key = rebornStornStruct.Type] = Active[key] + rebornStornStruct.Level;
										}
										else if (rebornStornStruct.Type >= 1 && rebornStornStruct.Type <= 5)
										{
											Active.Add(rebornStornStruct.Type, rebornStornStruct.Level);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static RebornStornOpcode ProessMakeRebornEquipHold(GameClient client, int DBID, int Bind, int IsReset, int Number, out string prop, out int bind)
		{
			prop = "";
			bind = 0;
			RebornStornOpcode result;
			if (Bind != 0 && Bind != 1)
			{
				result = RebornStornOpcode.RebornUseBind;
			}
			else
			{
				GoodsData rebornGoodsByDbID = RebornEquip.GetRebornGoodsByDbID(client, DBID);
				SystemXmlItem systemXmlItem;
				if (rebornGoodsByDbID == null)
				{
					result = RebornStornOpcode.RebornNotExist;
				}
				else if (!RebornEquip.IsRebornEquip(rebornGoodsByDbID.GoodsID))
				{
					result = RebornStornOpcode.RebornNotEquip;
				}
				else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rebornGoodsByDbID.GoodsID, out systemXmlItem))
				{
					result = RebornStornOpcode.RebornNotExistGoods;
				}
				else if (IsReset == 1)
				{
					if (rebornGoodsByDbID.Props != null && rebornGoodsByDbID.Props == "")
					{
						result = RebornStornOpcode.RebornNotInfo;
					}
					else
					{
						int currGoodsQuality = RebornEquip.GetCurrGoodsQuality(rebornGoodsByDbID);
						int num;
						if (!RebornStone.RebornEquipHoleMap.TryGetValue(currGoodsQuality, out num))
						{
							result = RebornStornOpcode.RebornNotFindMaxQuality;
						}
						else
						{
							int intValue = systemXmlItem.GetIntValue("SuitID", -1);
							Dictionary<int, int> useGoodsByEquipSuitAndQuality = RebornStone.GetUseGoodsByEquipSuitAndQuality(intValue, currGoodsQuality);
							if (useGoodsByEquipSuitAndQuality == null)
							{
								result = RebornStornOpcode.RebornUseMatterrislNull;
							}
							else
							{
								bool flag = false;
								foreach (KeyValuePair<int, int> keyValuePair in useGoodsByEquipSuitAndQuality)
								{
									if (!RebornStone.RebornUseGoodHasBinding(client, keyValuePair.Key, keyValuePair.Value, Bind, out flag))
									{
										return RebornStornOpcode.RebornUseMaterrislErr;
									}
								}
								if (flag)
								{
									rebornGoodsByDbID.Binding = 1;
								}
								Dictionary<int, Dictionary<int, int>> dictionary = RebornStone.ParessMakeHoleInfo(rebornGoodsByDbID.Props);
								lock (dictionary)
								{
									if (Number <= 0 || dictionary == null || !dictionary.ContainsKey(Number))
									{
										return RebornStornOpcode.RebornHoleSiteErr;
									}
									int num2 = RebornStone.MakeHoleQualityOne(intValue, currGoodsQuality);
									if (num2 == 0)
									{
										return RebornStornOpcode.RebornRandomHoleErr;
									}
									foreach (KeyValuePair<int, int> keyValuePair in dictionary[Number])
									{
										if (keyValuePair.Key == num)
										{
											return RebornStornOpcode.RebornMaxQuality;
										}
										dictionary[Number] = new Dictionary<int, int>
										{
											{
												num2,
												keyValuePair.Value
											}
										};
									}
									rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary, 0);
								}
								if (!RebornStone.UpdateGoodsPropsToDB(client, rebornGoodsByDbID))
								{
									result = RebornStornOpcode.RebornUpdateInfoErr;
								}
								else
								{
									prop = rebornGoodsByDbID.Props;
									bind = rebornGoodsByDbID.Binding;
									result = RebornStornOpcode.RebornHoleSucc;
								}
							}
						}
					}
				}
				else
				{
					bool flag3 = false;
					Dictionary<int, Dictionary<int, int>> dictionary = null;
					if (rebornGoodsByDbID.Props != null && rebornGoodsByDbID.Props != "")
					{
						dictionary = RebornStone.ParessMakeHoleInfo(rebornGoodsByDbID.Props);
						lock (dictionary)
						{
							Dictionary<int, int> dictionary2;
							if (dictionary.TryGetValue(Number, out dictionary2))
							{
								return RebornStornOpcode.RebornHasHole;
							}
							flag3 = true;
						}
					}
					Dictionary<int, Dictionary<int, int>> dictionary3;
					int goodsHoleInfo = RebornStone.GetGoodsHoleInfo(rebornGoodsByDbID, Number, out dictionary3, systemXmlItem);
					if (goodsHoleInfo == 0)
					{
						result = RebornStornOpcode.RebornMakeHoleErr;
					}
					else
					{
						bool flag = false;
						RebornHoleStruct rebornHoleStruct;
						if (RebornStone.RebornHoleStr.TryGetValue(goodsHoleInfo, out rebornHoleStruct))
						{
							lock (rebornHoleStruct)
							{
								foreach (KeyValuePair<int, int> keyValuePair in rebornHoleStruct.UseGoods)
								{
									if (!RebornStone.RebornUseGoodHasBinding(client, keyValuePair.Key, keyValuePair.Value, Bind, out flag))
									{
										return RebornStornOpcode.RebornUseMaterrislErr;
									}
								}
							}
						}
						if (flag)
						{
							rebornGoodsByDbID.Binding = 1;
						}
						if (dictionary3 != null)
						{
							lock (dictionary3)
							{
								if (flag3 && dictionary != null)
								{
									using (Dictionary<int, Dictionary<int, int>>.Enumerator enumerator2 = dictionary3.GetEnumerator())
									{
										if (enumerator2.MoveNext())
										{
											KeyValuePair<int, Dictionary<int, int>> keyValuePair2 = enumerator2.Current;
											dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
										}
									}
									rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary, 0);
								}
								else if (!flag3 && dictionary == null)
								{
									rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary3, 0);
								}
								if (!RebornStone.UpdateGoodsPropsToDB(client, rebornGoodsByDbID))
								{
									return RebornStornOpcode.RebornUpdateInfoErr;
								}
								prop = rebornGoodsByDbID.Props;
								bind = rebornGoodsByDbID.Binding;
							}
						}
						result = RebornStornOpcode.RebornHoleSucc;
					}
				}
			}
			return result;
		}

		public static RebornStornOpcode ProessRebornStoneInlayHold(GameClient client, int EquipDBID, int StoneDBID, int Number, out string prop, out int bind)
		{
			prop = "";
			bind = 0;
			GoodsData rebornGoodsByDbID = RebornEquip.GetRebornGoodsByDbID(client, EquipDBID);
			RebornStornOpcode result;
			if (rebornGoodsByDbID == null)
			{
				result = RebornStornOpcode.RebornInlayNotExistEquip;
			}
			else
			{
				GoodsData rebornGoodsByDbID2 = RebornEquip.GetRebornGoodsByDbID(client, StoneDBID);
				if (rebornGoodsByDbID2 == null)
				{
					result = RebornStornOpcode.RebornInlayNotExistStone;
				}
				else if (!RebornEquip.IsRebornEquip(rebornGoodsByDbID.GoodsID))
				{
					result = RebornStornOpcode.RebornInlayNotEquip;
				}
				else
				{
					if (Number == -1)
					{
						Dictionary<int, Dictionary<int, int>> dictionary = null;
						if (rebornGoodsByDbID.Props == null || rebornGoodsByDbID.Props == "")
						{
							dictionary = new Dictionary<int, Dictionary<int, int>>();
						}
						else
						{
							dictionary = RebornStone.ParessMakeHoleInfo(rebornGoodsByDbID.Props);
						}
						if (!RebornEquip.IsRebornEquipShengWu(rebornGoodsByDbID.GoodsID))
						{
							return RebornStornOpcode.RebornInlayNotEquip;
						}
						if (!RebornStone.RebornStoneActiveAttr.ContainsKey(rebornGoodsByDbID2.GoodsID))
						{
							return RebornStornOpcode.RebornInlayNotXuanCai;
						}
						Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
						if (dictionary.TryGetValue(Number, out dictionary2))
						{
							int num = 0;
							if (dictionary2.TryGetValue(Number, out num))
							{
								if (num != 0)
								{
									return RebornStornOpcode.RebornInlayCurrSiteHasStone;
								}
							}
						}
						rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary, rebornGoodsByDbID2.GoodsID);
						if (!RebornStone.UpdateGoodsPropsToDB(client, rebornGoodsByDbID))
						{
							return RebornStornOpcode.RebornInlayUpdateInfoErr;
						}
					}
					else
					{
						if (rebornGoodsByDbID.Props == null || rebornGoodsByDbID.Props == "")
						{
							return RebornStornOpcode.RebornInlayNotMakeHole;
						}
						Dictionary<int, Dictionary<int, int>> dictionary = RebornStone.ParessMakeHoleInfo(rebornGoodsByDbID.Props);
						if (dictionary == null)
						{
							return RebornStornOpcode.RebornInlayGetInfoErr;
						}
						lock (dictionary)
						{
							Dictionary<int, int> dictionary3;
							if (!dictionary.TryGetValue(Number, out dictionary3))
							{
								return RebornStornOpcode.RebornInlayNotHoleSite;
							}
							if (!RebornStone.RebornStoneXml.ContainsKey(rebornGoodsByDbID2.GoodsID))
							{
								return RebornStornOpcode.RebornInlayNotChongSheng;
							}
							Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
							using (Dictionary<int, int>.Enumerator enumerator = dictionary[Number].GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									KeyValuePair<int, int> keyValuePair = enumerator.Current;
									if (keyValuePair.Value != 0)
									{
										return RebornStornOpcode.RebornInlayCurrSiteHasStone;
									}
									dictionary2.Add(keyValuePair.Key, rebornGoodsByDbID2.GoodsID);
								}
							}
							dictionary[Number] = dictionary2;
							int xuanCaiID = 0;
							Dictionary<int, int> dictionary4;
							if (dictionary.TryGetValue(-1, out dictionary4))
							{
								int num2;
								if (dictionary4.TryGetValue(-1, out num2))
								{
									xuanCaiID = num2;
								}
							}
							rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary, xuanCaiID);
							if (!RebornStone.UpdateGoodsPropsToDB(client, rebornGoodsByDbID))
							{
								return RebornStornOpcode.RebornInlayUpdateInfoErr;
							}
						}
					}
					prop = rebornGoodsByDbID.Props;
					bool flag2;
					if (!RebornStone.RebornHoleRemoveUseGoods(client, rebornGoodsByDbID2, 1, out flag2))
					{
						result = RebornStornOpcode.RebornInlayUpdateStoneInfoErr;
					}
					else
					{
						if (flag2)
						{
							rebornGoodsByDbID.Binding = 1;
						}
						bind = rebornGoodsByDbID.Binding;
						if (rebornGoodsByDbID.Using == 1)
						{
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
						GameManager.logDBCmdMgr.AddDBLogInfo(rebornGoodsByDbID.Id, "重生宝石镶嵌", "重生宝石镶嵌", client.ClientData.RoleName, "系统", "添加宝石", 0, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, rebornGoodsByDbID);
						result = RebornStornOpcode.RebornInlaySucc;
					}
				}
			}
			return result;
		}

		public static RebornStornOpcode ProessRebornStoneDisInlayHold(GameClient client, int EquipDBID, int Number, out string prop)
		{
			prop = "";
			GoodsData rebornGoodsByDbID = RebornEquip.GetRebornGoodsByDbID(client, EquipDBID);
			RebornStornOpcode result;
			if (rebornGoodsByDbID == null)
			{
				result = RebornStornOpcode.RebornInlayNotExistEquip;
			}
			else if (!RebornEquip.IsRebornEquip(rebornGoodsByDbID.GoodsID))
			{
				result = RebornStornOpcode.RebornInlayNotEquip;
			}
			else if (rebornGoodsByDbID.Props == null || rebornGoodsByDbID.Props == "")
			{
				result = RebornStornOpcode.RebornInlayNotMakeHole;
			}
			else
			{
				Dictionary<int, Dictionary<int, int>> dictionary = RebornStone.ParessMakeHoleInfo(rebornGoodsByDbID.Props);
				if (dictionary == null)
				{
					result = RebornStornOpcode.RebornInlayGetInfoErr;
				}
				else
				{
					lock (dictionary)
					{
						if (Number == -1)
						{
							if (!RebornEquip.IsRebornEquipShengWu(rebornGoodsByDbID.GoodsID))
							{
								return RebornStornOpcode.RebornInlayNotEquip;
							}
							Dictionary<int, int> dictionary2;
							if (!dictionary.TryGetValue(Number, out dictionary2))
							{
								return RebornStornOpcode.RebornDisInlayCurrSiteNotHasXStone;
							}
							int num;
							if (!dictionary2.TryGetValue(Number, out num))
							{
								return RebornStornOpcode.RebornDisInlayCurrSiteNotHasXStone;
							}
							SystemXmlItem systemXmlItem;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num, out systemXmlItem))
							{
								LogManager.WriteLog(2, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, num), null, true);
								GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num, 0);
								return RebornStornOpcode.RebornDisInlayCurrUserNotHasXStone;
							}
							int binding = 0;
							if (rebornGoodsByDbID.Binding == 1)
							{
								binding = 1;
							}
							Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, num, 1, 0, "", 0, binding, 15000, "", true, 1, string.Format("炫彩宝石卸下", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
							dictionary.Remove(Number);
							rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary, 0);
							if (!RebornStone.UpdateGoodsPropsToDB(client, rebornGoodsByDbID))
							{
								return RebornStornOpcode.RebornDisInlayUpdateInfoErr;
							}
						}
						else
						{
							Dictionary<int, int> dictionary3;
							if (!dictionary.TryGetValue(Number, out dictionary3))
							{
								return RebornStornOpcode.RebornDisInlayCurrSiteNotHasStone;
							}
							Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
							using (Dictionary<int, int>.Enumerator enumerator = dictionary[Number].GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									KeyValuePair<int, int> keyValuePair = enumerator.Current;
									if (keyValuePair.Value == 0)
									{
										return RebornStornOpcode.RebornDisInlayCurrSiteNotHasStone;
									}
									SystemXmlItem systemXmlItem;
									if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(keyValuePair.Value, out systemXmlItem))
									{
										LogManager.WriteLog(2, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, keyValuePair.Value), null, true);
										GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, keyValuePair.Value, 0);
										return RebornStornOpcode.RebornDisInlayCurrUserNotHasStone;
									}
									int binding = 0;
									if (rebornGoodsByDbID.Binding == 1)
									{
										binding = 1;
									}
									Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, keyValuePair.Value, 1, 0, "", 0, binding, 15000, "", true, 1, string.Format("重生宝石卸下", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
									dictionary4.Add(keyValuePair.Key, 0);
								}
							}
							dictionary[Number] = dictionary4;
							int xuanCaiID = 0;
							Dictionary<int, int> dictionary5;
							if (dictionary.TryGetValue(-1, out dictionary5))
							{
								int num2;
								if (dictionary5.TryGetValue(-1, out num2))
								{
									xuanCaiID = num2;
								}
							}
							rebornGoodsByDbID.Props = RebornStone.MakeHoleInfo(dictionary, xuanCaiID);
						}
						if (!RebornStone.UpdateGoodsPropsToDB(client, rebornGoodsByDbID))
						{
							return RebornStornOpcode.RebornDisInlayUpdateInfoErr;
						}
					}
					prop = rebornGoodsByDbID.Props;
					if (rebornGoodsByDbID.Using == 1)
					{
						Global.RefreshEquipProp(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
					result = RebornStornOpcode.RebornInlaySucc;
				}
			}
			return result;
		}

		public static RebornStornOpcode ProessRebornStoneComplex(GameClient client, int GoodID, int Count)
		{
			SystemXmlItem systemXmlItem;
			RebornStornOpcode result;
			RebornStornStruct rebornStornStruct;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodID, out systemXmlItem))
			{
				LogManager.WriteLog(2, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, GoodID), null, true);
				GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GoodID, 0);
				result = RebornStornOpcode.RebornComplexStoneNotGood;
			}
			else if (Count <= 0 || Count > 999)
			{
				result = RebornStornOpcode.RebornComplexCountErr;
			}
			else if (!RebornStone.RebornStoneXml.TryGetValue(GoodID, out rebornStornStruct))
			{
				result = RebornStornOpcode.RebornComplexStoneNotFind;
			}
			else
			{
				lock (rebornStornStruct)
				{
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10252");
					int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "10253");
					int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "10254");
					int num = rebornStornStruct.FengYinJingShi * Count;
					int num2 = rebornStornStruct.ChongShengJingShi * Count;
					int num3 = rebornStornStruct.XuanCaiJingShi * Count;
					if (roleParamsInt32FromDB < num)
					{
						return RebornStornOpcode.RebornComplexFengYinNotEnough;
					}
					if (roleParamsInt32FromDB2 < num2)
					{
						return RebornStornOpcode.RebornComplexChongShengNotEnough;
					}
					if (roleParamsInt32FromDB3 < num3)
					{
						return RebornStornOpcode.RebornComplexXuanCaiNotEnough;
					}
					if (!GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(client, -num, "重生宝石合成消耗封印晶石", true, true, false))
					{
						return RebornStornOpcode.RebornComplexNeedFengYinErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(client, -num2, "重生宝石合成消耗重生晶石", true, true, false))
					{
						return RebornStornOpcode.RebornComplexNeedChongShengErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(client, -num3, "重生宝石合成消耗炫彩晶石", true, true, false))
					{
						return RebornStornOpcode.RebornComplexNeedXuanCaiErr;
					}
				}
				int num4 = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, GoodID, Count, 0, "", 0, 0, 15000, "", true, 1, string.Format("重生宝石合成", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
				result = RebornStornOpcode.RebornComplexNewStoneSucc;
			}
			return result;
		}

		public static RebornStornOpcode RebornStoneResolve(GameClient client, int GoodsID, int Count, int bind)
		{
			RebornStornOpcode result;
			if (bind < 0 || bind > 1)
			{
				result = RebornStornOpcode.RebornNotFindBind;
			}
			else if (Count <= 0 || Count > 9999)
			{
				result = RebornStornOpcode.RebornResolveCountErr;
			}
			else
			{
				bool flag;
				Dictionary<int, Dictionary<int, GoodsData>> dictRebornUseGoodHasBinding = RebornStone.GetDictRebornUseGoodHasBinding(client, GoodsID, Count, bind, out flag);
				if (dictRebornUseGoodHasBinding == null)
				{
					result = RebornStornOpcode.RebornResolveNotFind;
				}
				else
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					lock (dictRebornUseGoodHasBinding)
					{
						foreach (Dictionary<int, GoodsData> dictionary in dictRebornUseGoodHasBinding.Values)
						{
							foreach (KeyValuePair<int, GoodsData> keyValuePair in dictionary)
							{
								GoodsData value = keyValuePair.Value;
								if (value == null)
								{
									return RebornStornOpcode.RebornResolveNotFind;
								}
								if (value.Using == 1)
								{
									return RebornStornOpcode.RebornResolveIsUsing;
								}
								SystemXmlItem systemXmlItem;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(value.GoodsID, out systemXmlItem))
								{
									LogManager.WriteLog(2, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, value.GoodsID), null, true);
									GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, value.GoodsID, 0);
									return RebornStornOpcode.RebornResolveStoneNotGood;
								}
								if (keyValuePair.Key < 0)
								{
									return RebornStornOpcode.RebornResolveGoodNotEnoughErr;
								}
								bool flag3 = false;
								if (!RebornStone.RebornHoleRemoveUseGoods(client, value, keyValuePair.Key, out flag3))
								{
									return RebornStornOpcode.RebornResolveDeleteErr;
								}
								int intValue = systemXmlItem.GetIntValue("ChangeFengYingJingShi", -1);
								int intValue2 = systemXmlItem.GetIntValue("ChangeChongShengJingShi", -1);
								int intValue3 = systemXmlItem.GetIntValue("ChangeXuanCaiJingShi", -1);
								if (intValue < 0 || intValue2 < 0 || intValue3 < 0)
								{
									return RebornStornOpcode.RebornResolveGoodXmlErr;
								}
								num += intValue * keyValuePair.Key;
								num2 += intValue2 * keyValuePair.Key;
								num3 += intValue3 * keyValuePair.Key;
							}
						}
					}
					if (!GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(client, num, "重生宝石分解增加封印晶石", true, true, false))
					{
						result = RebornStornOpcode.RebornResolveAddFengYinErr;
					}
					else if (!GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(client, num2, "重生宝石分解增加重生晶石", true, true, false))
					{
						result = RebornStornOpcode.RebornResolveAddChongShengErr;
					}
					else if (!GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(client, num3, "重生宝石分解增加炫彩晶石", true, true, false))
					{
						result = RebornStornOpcode.RebornResolveAddXuanCaiErr;
					}
					else
					{
						result = RebornStornOpcode.RebornResolveStoneSucc;
					}
				}
			}
			return result;
		}

		public static RebornStornOpcode RebornXuanCaiComplexStone(GameClient client, int DBID1, int num1, int DBID2, int num2, int DBID3, int num3)
		{
			GoodsData rebornGoodsByDbID = RebornEquip.GetRebornGoodsByDbID(client, DBID1);
			RebornStornOpcode result;
			if (rebornGoodsByDbID == null)
			{
				result = RebornStornOpcode.RebornXuanCaiNotFind;
			}
			else
			{
				int num4 = 0;
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				lock (dictionary)
				{
					if (num1 > 0)
					{
						if (dictionary.ContainsKey(DBID1))
						{
							return RebornStornOpcode.RebornXuanGoodInfoErr;
						}
						dictionary.Add(DBID1, num1);
						num4 += num1;
					}
					if (num2 > 0)
					{
						if (dictionary.ContainsKey(DBID2))
						{
							return RebornStornOpcode.RebornXuanGoodInfoErr;
						}
						dictionary.Add(DBID2, num2);
						num4 += num2;
					}
					if (num3 > 0)
					{
						if (dictionary.ContainsKey(DBID3))
						{
							return RebornStornOpcode.RebornXuanGoodInfoErr;
						}
						dictionary.Add(DBID3, num3);
						num4 += num3;
					}
				}
				if (num4 != 3)
				{
					result = RebornStornOpcode.RebornXuanCaiNotFind;
				}
				else
				{
					Dictionary<int, Dictionary<int, int>> dictionary2 = null;
					List<GoodsData> rebornGoodsByDbIDDict = RebornEquip.GetRebornGoodsByDbIDDict(client, dictionary, out dictionary2);
					if (rebornGoodsByDbIDDict == null)
					{
						result = RebornStornOpcode.RebornXuanCaiNotFind;
					}
					else
					{
						List<int> list = new List<int>();
						lock (rebornGoodsByDbIDDict)
						{
							foreach (GoodsData goodsData in rebornGoodsByDbIDDict)
							{
								SystemXmlItem systemXmlItem;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
								{
									LogManager.WriteLog(2, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, goodsData.GoodsID), null, true);
									GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, 0);
									return RebornStornOpcode.RebornXuanCaiGoodXmlErr;
								}
								list.Add(systemXmlItem.GetIntValue("SuitID", -1));
							}
						}
						if (list.Count == 0)
						{
							result = RebornStornOpcode.RebornXuanCaiSuitErr;
						}
						else
						{
							lock (list)
							{
								if (list[0] >= RebornStone.XuanCaiMaxLevel)
								{
									return RebornStornOpcode.RebornXuanCaiMaxLevel;
								}
								for (int i = 0; i < list.Count; i++)
								{
									if (i == list.Count - 1)
									{
										break;
									}
									if (list[i] != list[i + 1])
									{
										return RebornStornOpcode.RebornXuanCaiNotSameLevel;
									}
								}
							}
							RebornStornComp rebornStornComp;
							if (!RebornStone.RebornStoneComplex.TryGetValue(rebornGoodsByDbID.GoodsID, out rebornStornComp))
							{
								result = RebornStornOpcode.RebornXuanCaiNotFindComplex;
							}
							else
							{
								int stornAID = rebornStornComp.StornAID;
								if (dictionary2 == null)
								{
									result = RebornStornOpcode.RebornXuanGoodInfoErr;
								}
								else
								{
									int binding = 0;
									lock (dictionary2)
									{
										List<int> list2 = new List<int>();
										List<bool> list3 = new List<bool>();
										foreach (int num5 in rebornStornComp.CompNeed)
										{
											foreach (Dictionary<int, int> dictionary3 in dictionary2.Values)
											{
												using (Dictionary<int, int>.Enumerator enumerator4 = dictionary3.GetEnumerator())
												{
													if (enumerator4.MoveNext())
													{
														KeyValuePair<int, int> keyValuePair = enumerator4.Current;
														if (keyValuePair.Key == num5)
														{
															list3.Add(true);
														}
														list2.Add(keyValuePair.Value);
													}
												}
											}
										}
										if (list3.Count != dictionary2.Count)
										{
											return RebornStornOpcode.RebornXuanCaiNotUseGoodComplex;
										}
										int num6 = 0;
										foreach (GoodsData useGood in rebornGoodsByDbIDDict)
										{
											bool flag5 = false;
											if (!RebornStone.RebornHoleRemoveUseGoods(client, useGood, list2[num6], out flag5))
											{
												return RebornStornOpcode.RebornXuanCaiUseGoodErr;
											}
											num6++;
											if (flag5)
											{
												binding = 1;
											}
										}
									}
									Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, stornAID, 1, 0, "", 0, binding, 15000, "", true, 1, string.Format("炫彩宝石合成", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
									result = RebornStornOpcode.RebornXuanCaiComplexSucc;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static RebornStornOpcode SaleRebornStoneProcess(GameClient client, string strGoodsID)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
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
						if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "重生宝石回收", null))
						{
							if (client.ClientData.RebornCount > 0)
							{
								int intValue = systemXmlItem.GetIntValue("ChangeFengYingJingShi", -1);
								int intValue2 = systemXmlItem.GetIntValue("ChangeChongShengJingShi", -1);
								int intValue3 = systemXmlItem.GetIntValue("ChangeXuanCaiJingShi", -1);
								if (intValue > 0)
								{
									num += intValue * rebornGoodsByDbID.GCount;
								}
								if (intValue2 > 0)
								{
									num2 += intValue2 * rebornGoodsByDbID.GCount;
								}
								if (intValue3 > 0)
								{
									num3 += intValue3 * rebornGoodsByDbID.GCount;
								}
							}
						}
					}
				}
				IL_1FE:
				i++;
				continue;
				goto IL_1FE;
			}
			if (num > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(client, num, "重生宝石一键分解增加封印晶石", true, true, false))
				{
					return RebornStornOpcode.RebornBatchResolveAddFengYinErr;
				}
			}
			if (num2 > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(client, num2, "重生宝石一键分解增加重生晶石", true, true, false))
				{
					return RebornStornOpcode.RebornBatchResolveAddChongShengErr;
				}
			}
			if (num3 > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(client, num3, "重生宝石一键分解增加炫彩晶石", true, true, false))
				{
					return RebornStornOpcode.RebornBatchResolveAddXuanCaiErr;
				}
			}
			return RebornStornOpcode.RebornBatchResolveStoneSucc;
		}

		public static Dictionary<int, RebornHoleStruct> RebornHoleStr = new Dictionary<int, RebornHoleStruct>();

		public static Dictionary<int, int> RebornEquipHoleMap = new Dictionary<int, int>();

		public static Dictionary<Dictionary<int, int>, int> ItemIDMap = new Dictionary<Dictionary<int, int>, int>();

		public static Dictionary<int, double> RebornHoleExpend = new Dictionary<int, double>();

		public static Dictionary<int, RebornStornStruct> RebornStoneXml = new Dictionary<int, RebornStornStruct>();

		public static Dictionary<int, RebornStornComp> RebornStoneComplex = new Dictionary<int, RebornStornComp>();

		public static Dictionary<int, Dictionary<int, RebornXuanCaiStorn>> RebornStoneActiveAttr = new Dictionary<int, Dictionary<int, RebornXuanCaiStorn>>();

		public static int XuanCaiMaxLevel = 10;

		private static RebornStone instance = new RebornStone();
	}
}
