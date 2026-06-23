using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	internal class MountHolyStampManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		public static MountHolyStampManager getInstance()
		{
			return MountHolyStampManager.instance;
		}

		public bool InitConfig()
		{
			string text = Global.GameResPath(MountHolyStampConst.ShengYinShengJi);
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				Dictionary<int, List<HolyStampUpLeve>> dictionary = new Dictionary<int, List<HolyStampUpLeve>>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				int num = 0;
				foreach (XElement xml in enumerable)
				{
					HolyStampUpLeve holyStampUpLeve = new HolyStampUpLeve();
					Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
					holyStampUpLeve.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					holyStampUpLeve.GoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "DaoJuID"));
					holyStampUpLeve.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "LeiXing"));
					holyStampUpLeve.Site = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "BuWei"));
					holyStampUpLeve.Quality = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "PinZhi"));
					holyStampUpLeve.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "DengJi"));
					holyStampUpLeve.UpExp = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ShengJiJingYan"));
					holyStampUpLeve.PhagocytosisExp = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "TunShiJingYan"));
					holyStampUpLeve.AttrNum = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ZhuoYueShuXingTiaoShu"));
					if (holyStampUpLeve.AttrNum < 0)
					{
						holyStampUpLeve.AttrNum = 0;
					}
					string[] array = Global.GetSafeAttributeStr(xml, "JiChuShuXing").Split(new char[]
					{
						'|'
					});
					if (array != null && array.Length == 1)
					{
						dictionary2.Add(0, 0.0);
					}
					else
					{
						for (int i = 0; i < array.Length; i++)
						{
							dictionary2.Add((int)ConfigParser.GetPropIndexByPropName(array[i].Split(new char[]
							{
								','
							})[0]), Convert.ToDouble(array[i].Split(new char[]
							{
								','
							})[1]));
						}
					}
					holyStampUpLeve.AttrList = dictionary2;
					if (dictionary.ContainsKey(holyStampUpLeve.GoodID))
					{
						dictionary[holyStampUpLeve.GoodID].Add(holyStampUpLeve);
					}
					else
					{
						List<HolyStampUpLeve> list = new List<HolyStampUpLeve>();
						list.Add(holyStampUpLeve);
						dictionary.Add(holyStampUpLeve.GoodID, list);
					}
					if (this.GoodsLvDict.ContainsKey(holyStampUpLeve.GoodID))
					{
						if (this.GoodsLvDict[holyStampUpLeve.GoodID] < holyStampUpLeve.Level)
						{
							this.GoodsLvDict[holyStampUpLeve.GoodID] = holyStampUpLeve.Level;
						}
					}
					else
					{
						this.GoodsLvDict.Add(holyStampUpLeve.GoodID, holyStampUpLeve.Level);
					}
					num += holyStampUpLeve.AttrNum;
					if (this.GoodsLvAttrCount.ContainsKey(holyStampUpLeve.GoodID))
					{
						if (this.GoodsLvAttrCount[holyStampUpLeve.GoodID].ContainsKey(holyStampUpLeve.Level))
						{
							this.GoodsLvAttrCount[holyStampUpLeve.GoodID][holyStampUpLeve.Level] = num;
						}
						else
						{
							this.GoodsLvAttrCount[holyStampUpLeve.GoodID].Add(holyStampUpLeve.Level, num);
						}
					}
					else
					{
						num = holyStampUpLeve.AttrNum;
						Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
						dictionary3.Add(holyStampUpLeve.Level, num);
						this.GoodsLvAttrCount.Add(holyStampUpLeve.GoodID, dictionary3);
					}
				}
				this.holyStampUpLeveL = dictionary;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (this.holyStampUpLeveL == null || this.GoodsLvDict == null || this.GoodsLvAttrCount == null)
			{
				result = false;
			}
			else
			{
				text = Global.GameResPath(MountHolyStampConst.ShengYinZhuoYue);
				xelement = XElement.Load(text);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
				}
				try
				{
					Dictionary<int, Dictionary<int, List<HolyStampAttr>>> dictionary4 = new Dictionary<int, Dictionary<int, List<HolyStampAttr>>>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						HolyStampAttr holyStampAttr = new HolyStampAttr();
						Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
						holyStampAttr.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
						holyStampAttr.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ShengYinLeiXing"));
						holyStampAttr.Quality = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "PinZhi"));
						string[] array = Global.GetSafeAttributeStr(xml, "ShuXingLeiXing").Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < array.Length; i++)
						{
							dictionary2.Add((int)ConfigParser.GetPropIndexByPropName(array[i].Split(new char[]
							{
								','
							})[0]), Convert.ToDouble(array[i].Split(new char[]
							{
								','
							})[1]));
						}
						holyStampAttr.AttrList = dictionary2;
						if (dictionary4.ContainsKey(holyStampAttr.Type))
						{
							if (dictionary4[holyStampAttr.Type].ContainsKey(holyStampAttr.Quality))
							{
								dictionary4[holyStampAttr.Type][holyStampAttr.Quality].Add(holyStampAttr);
							}
							else
							{
								List<HolyStampAttr> list2 = new List<HolyStampAttr>();
								list2.Add(holyStampAttr);
								dictionary4[holyStampAttr.Type].Add(holyStampAttr.Quality, list2);
							}
						}
						else
						{
							Dictionary<int, List<HolyStampAttr>> dictionary5 = new Dictionary<int, List<HolyStampAttr>>();
							List<HolyStampAttr> list2 = new List<HolyStampAttr>();
							list2.Add(holyStampAttr);
							dictionary5.Add(holyStampAttr.Quality, list2);
							dictionary4.Add(holyStampAttr.Type, dictionary5);
						}
					}
					this.holyStampAttr = dictionary4;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				if (this.holyStampAttr == null)
				{
					result = false;
				}
				else
				{
					text = Global.GameResPath(MountHolyStampConst.ShengYinTaoZhuang);
					xelement = XElement.Load(text);
					if (null == xelement)
					{
						LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
					}
					try
					{
						Dictionary<int, HolyStampSuit> dictionary6 = new Dictionary<int, HolyStampSuit>();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							HolyStampSuit holyStampSuit = new HolyStampSuit();
							Dictionary<int, double> dictionary7 = new Dictionary<int, double>();
							Dictionary<int, double> dictionary8 = new Dictionary<int, double>();
							Dictionary<int, double> dictionary9 = new Dictionary<int, double>();
							holyStampSuit.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
							holyStampSuit.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "LeiXing"));
							string[] array2 = Global.GetSafeAttributeStr(xml, "TaoZhuangShuXingTwo").Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array2.Length; i++)
							{
								dictionary7.Add((int)ConfigParser.GetPropIndexByPropName(array2[i].Split(new char[]
								{
									','
								})[0]), Convert.ToDouble(array2[i].Split(new char[]
								{
									','
								})[1]));
							}
							holyStampSuit.AttrListTwo = dictionary7;
							string[] array3 = Global.GetSafeAttributeStr(xml, "TaoZhuangShuXingFour").Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array3.Length; i++)
							{
								dictionary8.Add((int)ConfigParser.GetPropIndexByPropName(array3[i].Split(new char[]
								{
									','
								})[0]), Convert.ToDouble(array3[i].Split(new char[]
								{
									','
								})[1]));
							}
							holyStampSuit.AttrListFour = dictionary8;
							string[] array4 = Global.GetSafeAttributeStr(xml, "TaoZhuangShuXingSix").Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array4.Length; i++)
							{
								dictionary9.Add((int)ConfigParser.GetPropIndexByPropName(array4[i].Split(new char[]
								{
									','
								})[0]), Convert.ToDouble(array4[i].Split(new char[]
								{
									','
								})[1]));
							}
							holyStampSuit.AttrListSix = dictionary9;
							dictionary6.Add(holyStampSuit.Type, holyStampSuit);
						}
						this.holyStampSuit = dictionary6;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					if (this.holyStampSuit == null)
					{
						result = false;
					}
					else
					{
						List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("ShengYinJieSuo", '|');
						try
						{
							Dictionary<int, int> dictionary10 = new Dictionary<int, int>();
							foreach (string text2 in paramValueStringListByName)
							{
								string[] array5 = text2.Split(new char[]
								{
									','
								});
								dictionary10.Add(Convert.ToInt32(array5[0]), Convert.ToInt32(array5[1]));
							}
							this.holyStampDesbloquear = dictionary10;
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
						result = (this.holyStampDesbloquear != null);
					}
				}
			}
			return result;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2090, 2, 2, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2091, 1, 1, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2092, 1, 1, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2093, 1, 1, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public int GetGolaNum(GameClient client)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else if (client.ClientData.ZuoQiMainData == null)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				lock (this.holyStampDesbloquear)
				{
					foreach (KeyValuePair<int, int> keyValuePair in this.holyStampDesbloquear)
					{
						if (client.ClientData.ZuoQiMainData.MountLevel + 1 < keyValuePair.Value)
						{
							break;
						}
						num = keyValuePair.Key;
					}
				}
				result = num;
			}
			return result;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 2090:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int dbid = Convert.ToInt32(cmdParams[0]);
					string phagocytosisDBids = cmdParams[1];
					this.ProcessHolyStampUpGrade(client, dbid, phagocytosisDBids);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_HOLYSTAMP_UPGRADE", false, false);
				}
				break;
			case 2091:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int dbid = Convert.ToInt32(cmdParams[0]);
					int num = Convert.ToInt32(this.ProcessHolyStampUsing(client, dbid));
					client.sendCmd(nID, string.Format("{0}", num), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_HOLYSTAMP_INLAY", false, false);
				}
				break;
			case 2092:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int dbid = Convert.ToInt32(cmdParams[0]);
					int num = Convert.ToInt32(this.ProcessHolyStampDisUsing(client, dbid));
					client.sendCmd(nID, string.Format("{0}", num), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_HOLYSTAMP_DEMOUNT", false, false);
				}
				break;
			case 2093:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num2 = Convert.ToInt32(cmdParams[0]);
					this.ResetHolyStampBag(client, true);
					client.sendCmd<List<GoodsData>>(nID, client.ClientData.HolyGoodsDataList, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_HOLYSTAMP_BAGRESET", false, false);
				}
				break;
			}
			return true;
		}

		public static int GetIdleSlotOfGoods(GameClient client)
		{
			int num = 0;
			int result;
			if (null == client.ClientData.HolyGoodsDataList)
			{
				result = num;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
				{
					if (client.ClientData.HolyGoodsDataList[i].Site == 16000 && client.ClientData.HolyGoodsDataList[i].Using <= 0)
					{
						if (list.IndexOf(client.ClientData.HolyGoodsDataList[i].BagIndex) < 0)
						{
							list.Add(client.ClientData.HolyGoodsDataList[i].BagIndex);
						}
					}
				}
				for (int j = 0; j < MountHolyStampManager.HolyBagNum; j++)
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

		public GoodsData GetGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.HolyGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					result = client.ClientData.HolyGoodsDataList.Find((GoodsData _g) => _g.Id == dbID);
				}
			}
			return result;
		}

		public static bool CheckIsMountBagByGoodsID(int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem) && (systemXmlItem.GetIntValue("Categoriy", -1) == 980 || systemXmlItem.GetIntValue("Categoriy", -1) == 981);
		}

		public static GoodsData AddGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
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
			if (null == client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList = new List<GoodsData>();
			}
			lock (client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList.Add(goodsData);
			}
			return goodsData;
		}

		public HolyStampUpLeve GetHolyUpGradeInfo(GoodsData goodData)
		{
			HolyStampUpLeve result;
			if (goodData == null || this.holyStampUpLeveL == null || !this.holyStampUpLeveL.ContainsKey(goodData.GoodsID) || goodData.ElementhrtsProps == null || goodData.ElementhrtsProps.Count != 2)
			{
				result = null;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodData.GoodsID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					lock (this.holyStampUpLeveL[goodData.GoodsID])
					{
						foreach (HolyStampUpLeve holyStampUpLeve in this.holyStampUpLeveL[goodData.GoodsID])
						{
							if (980 == systemXmlItem.GetIntValue("Categoriy", -1) && holyStampUpLeve.Level == goodData.ElementhrtsProps[0])
							{
								return holyStampUpLeve;
							}
							if (981 == systemXmlItem.GetIntValue("Categoriy", -1) && holyStampUpLeve.Level == -1)
							{
								return holyStampUpLeve;
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		public HolyStampUpLeve GetLevelUpCount(GoodsData goodData, HolyStampUpLeve CurrInfo, int TunExp, out int TotleExp)
		{
			TotleExp = 0;
			HolyStampUpLeve result;
			if (TunExp < 0 || TunExp > 2147483647)
			{
				result = null;
			}
			else if (CurrInfo == null || goodData == null || goodData.ElementhrtsProps == null || goodData.ElementhrtsProps.Count != 2)
			{
				result = null;
			}
			else if (!this.holyStampUpLeveL.ContainsKey(goodData.GoodsID))
			{
				result = null;
			}
			else if (CurrInfo.GoodID != goodData.GoodsID)
			{
				result = null;
			}
			else
			{
				TotleExp = goodData.ElementhrtsProps[1] + TunExp;
				if (CurrInfo.UpExp > TotleExp)
				{
					result = CurrInfo;
				}
				else
				{
					lock (this.holyStampUpLeveL)
					{
						foreach (HolyStampUpLeve holyStampUpLeve in this.holyStampUpLeveL[goodData.GoodsID])
						{
							if (holyStampUpLeve.UpExp > TotleExp)
							{
								return holyStampUpLeve;
							}
							if (this.GoodsLvDict[holyStampUpLeve.GoodID] <= holyStampUpLeve.Level)
							{
								TotleExp = holyStampUpLeve.UpExp;
								return holyStampUpLeve;
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		public HolyStampAttr GetAttrByQuality(HolyStampUpLeve StampInfo, GoodsData goodsData, List<int> newList = null)
		{
			if (this.holyStampAttr.ContainsKey(StampInfo.Type))
			{
				if (this.holyStampAttr[StampInfo.Type].ContainsKey(StampInfo.Quality))
				{
					if (newList == null)
					{
						if (goodsData.WashProps == null || goodsData.WashProps.Count == 0)
						{
							int count = this.holyStampAttr[StampInfo.Type][StampInfo.Quality].Count;
							if (count < 1)
							{
								return null;
							}
							int randomNumber = Global.GetRandomNumber(1, count);
							return this.holyStampAttr[StampInfo.Type][StampInfo.Quality][randomNumber];
						}
						else
						{
							List<HolyStampAttr> list = new List<HolyStampAttr>();
							bool flag = false;
							foreach (HolyStampAttr holyStampAttr in this.holyStampAttr[StampInfo.Type][StampInfo.Quality])
							{
								for (int i = 0; i < goodsData.WashProps.Count; i += 3)
								{
									if (holyStampAttr.ID == goodsData.WashProps[i])
									{
										flag = true;
										break;
									}
								}
								if (!flag && list.IndexOf(holyStampAttr) == -1)
								{
									list.Add(holyStampAttr);
								}
								flag = false;
							}
							int count = list.Count;
							if (count < 1)
							{
								return null;
							}
							int randomNumber = Global.GetRandomNumber(0, count);
							return list[randomNumber];
						}
					}
					else
					{
						List<HolyStampAttr> list = new List<HolyStampAttr>();
						bool flag = false;
						foreach (HolyStampAttr holyStampAttr in this.holyStampAttr[StampInfo.Type][StampInfo.Quality])
						{
							for (int i = 0; i < newList.Count; i += 3)
							{
								if (holyStampAttr.ID == newList[i])
								{
									flag = true;
									break;
								}
							}
							if (!flag && list.IndexOf(holyStampAttr) == -1)
							{
								list.Add(holyStampAttr);
							}
							flag = false;
						}
						int count = list.Count;
						if (count < 1)
						{
							return null;
						}
						int randomNumber = Global.GetRandomNumber(0, count);
						return list[randomNumber];
					}
				}
			}
			return null;
		}

		public void UpdateProps(GameClient client)
		{
			double[] array = new double[177];
			try
			{
				if (client != null && client.ClientData.HolyGoodsDataList != null)
				{
					double num = 0.001;
					int num2 = 0;
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					foreach (GoodsData goodsData in client.ClientData.HolyGoodsDataList)
					{
						if (goodsData.Using == 1)
						{
							HolyStampUpLeve holyUpGradeInfo = this.GetHolyUpGradeInfo(goodsData);
							if (holyUpGradeInfo != null)
							{
								if (dictionary.ContainsKey(holyUpGradeInfo.Type))
								{
									Dictionary<int, int> dictionary2;
									int key;
									(dictionary2 = dictionary)[key = holyUpGradeInfo.Type] = dictionary2[key] + 1;
								}
								else
								{
									dictionary.Add(holyUpGradeInfo.Type, 1);
								}
								foreach (KeyValuePair<int, double> keyValuePair in holyUpGradeInfo.AttrList)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								if (goodsData.WashProps != null && goodsData.WashProps.Count >= 3)
								{
									for (int i = 1; i < goodsData.WashProps.Count; i += 3)
									{
										if (i + 1 < goodsData.WashProps.Count)
										{
											array[goodsData.WashProps[i]] += (double)goodsData.WashProps[i + 1] * num;
										}
									}
								}
								num2++;
							}
						}
					}
					if (num2 <= 6)
					{
						Dictionary<int, double> dictionary3 = new Dictionary<int, double>();
						lock (dictionary3)
						{
							if (dictionary != null)
							{
								foreach (KeyValuePair<int, int> keyValuePair2 in dictionary)
								{
									if (this.holyStampSuit.ContainsKey(keyValuePair2.Key))
									{
										if (keyValuePair2.Value >= 2)
										{
											foreach (KeyValuePair<int, double> keyValuePair in this.holyStampSuit[keyValuePair2.Key].AttrListTwo)
											{
												if (dictionary3.ContainsKey(keyValuePair.Key))
												{
													int key;
													Dictionary<int, double> dictionary4;
													(dictionary4 = dictionary3)[key = keyValuePair.Key] = dictionary4[key] + keyValuePair.Value;
												}
												else
												{
													dictionary3.Add(keyValuePair.Key, keyValuePair.Value);
												}
											}
											if (keyValuePair2.Value >= 4)
											{
												foreach (KeyValuePair<int, double> keyValuePair in this.holyStampSuit[keyValuePair2.Key].AttrListFour)
												{
													if (dictionary3.ContainsKey(keyValuePair.Key))
													{
														int key;
														Dictionary<int, double> dictionary4;
														(dictionary4 = dictionary3)[key = keyValuePair.Key] = dictionary4[key] + keyValuePair.Value;
													}
													else
													{
														dictionary3.Add(keyValuePair.Key, keyValuePair.Value);
													}
												}
												if (keyValuePair2.Value >= 6)
												{
													foreach (KeyValuePair<int, double> keyValuePair in this.holyStampSuit[keyValuePair2.Key].AttrListSix)
													{
														if (dictionary3.ContainsKey(keyValuePair.Key))
														{
															int key;
															Dictionary<int, double> dictionary4;
															(dictionary4 = dictionary3)[key = keyValuePair.Key] = dictionary4[key] + keyValuePair.Value;
														}
														else
														{
															dictionary3.Add(keyValuePair.Key, keyValuePair.Value);
														}
													}
												}
											}
										}
									}
								}
							}
							foreach (KeyValuePair<int, double> keyValuePair3 in dictionary3)
							{
								array[keyValuePair3.Key] += keyValuePair3.Value;
							}
						}
					}
				}
			}
			finally
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.HolyStamp,
					array
				});
			}
		}

		public GoodsData GetUsingGoodDataByBagindex(GameClient client, int HolySite)
		{
			GoodsData result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = null;
			}
			else
			{
				foreach (GoodsData goodsData in client.ClientData.HolyGoodsDataList)
				{
					if (goodsData.Using == 1 && goodsData.BagIndex == HolySite)
					{
						return goodsData;
					}
				}
				result = null;
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
			else if (client.ClientData.HolyGoodsDataList == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				lock (client.ClientData.HolyGoodsDataList)
				{
					flag = client.ClientData.HolyGoodsDataList.Remove(gd);
				}
				result = flag;
			}
			return result;
		}

		public static int GetGoodsGridNumByID(int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
			{
				result = 1;
			}
			else
			{
				result = systemXmlItem.GetIntValue("GridNum", -1);
			}
			return result;
		}

		private static int CalGoodsGridNum(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			int num = MountHolyStampManager.GetGoodsGridNumByID(goodsID);
			num = Global.GMax(num, 1);
			int result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = (newGoodsNum - 1) / num + 1;
			}
			else
			{
				int num2 = 0;
				lock (client.ClientData.HolyGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							num2++;
							if (canUseOld && num > 1)
							{
								if (client.ClientData.HolyGoodsDataList[i].GoodsID == goodsID && client.ClientData.HolyGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.HolyGoodsDataList[i].Endtime, endTime))
								{
									if (client.ClientData.HolyGoodsDataList[i].GCount < num)
									{
										newGoodsNum -= Global.GMin(newGoodsNum, num - client.ClientData.HolyGoodsDataList[i].GCount);
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

		public static int GetGoodsUsedGrid(GameClient client)
		{
			int num = 0;
			int result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = num;
			}
			else
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							num++;
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static bool CanAddGoods(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int num = MountHolyStampManager.CalGoodsGridNum(client, goodsID, newGoodsNum, binding, endTime, canUseOld);
				int goodsUsedGrid = MountHolyStampManager.GetGoodsUsedGrid(client);
				result = (goodsUsedGrid + num <= MountHolyStampManager.HolyBagNum);
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
				int goodsUsedGrid = MountHolyStampManager.GetGoodsUsedGrid(client);
				result = (newGoodsCount + goodsUsedGrid <= MountHolyStampManager.HolyBagNum);
			}
			return result;
		}

		public List<int> SetHolyStampAttr(GoodsData goodsData)
		{
			List<int> result;
			if (goodsData == null || this.holyStampUpLeveL == null || !this.holyStampUpLeveL.ContainsKey(goodsData.GoodsID) || goodsData.ElementhrtsProps == null || goodsData.ElementhrtsProps.Count != 2)
			{
				result = null;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					lock (this.holyStampUpLeveL[goodsData.GoodsID])
					{
						foreach (HolyStampUpLeve holyStampUpLeve in this.holyStampUpLeveL[goodsData.GoodsID])
						{
							if (980 == systemXmlItem.GetIntValue("Categoriy", -1) && holyStampUpLeve.Level == goodsData.ElementhrtsProps[0])
							{
								if (this.GoodsLvAttrCount.ContainsKey(goodsData.GoodsID) && this.GoodsLvAttrCount[goodsData.GoodsID].ContainsKey(holyStampUpLeve.Level))
								{
									int num = this.GoodsLvAttrCount[goodsData.GoodsID][holyStampUpLeve.Level];
									if (goodsData.WashProps == null)
									{
										if (this.holyStampAttr.ContainsKey(holyStampUpLeve.Type) && this.holyStampAttr[holyStampUpLeve.Type].ContainsKey(holyStampUpLeve.Quality))
										{
											goodsData.WashProps = new List<int>();
											for (int i = 0; i < num; i++)
											{
												int randomNumber = Global.GetRandomNumber(0, this.holyStampAttr[holyStampUpLeve.Type][holyStampUpLeve.Quality].Count);
												if (this.holyStampAttr[holyStampUpLeve.Type][holyStampUpLeve.Quality][randomNumber] != null)
												{
													HolyStampAttr attrByQuality = this.GetAttrByQuality(holyStampUpLeve, goodsData, null);
													if (attrByQuality != null)
													{
														using (Dictionary<int, double>.Enumerator enumerator2 = attrByQuality.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> keyValuePair = enumerator2.Current;
																goodsData.WashProps.Add(attrByQuality.ID);
																goodsData.WashProps.Add(keyValuePair.Key);
																goodsData.WashProps.Add(Convert.ToInt32(keyValuePair.Value * 1000.0));
															}
														}
													}
												}
											}
											return goodsData.WashProps;
										}
									}
									else if (goodsData.WashProps.Count > 0)
									{
										int num2 = goodsData.WashProps.Count / 3;
										int num3 = num - num2;
										if (num3 > 0)
										{
											for (int i = 0; i < num3; i++)
											{
												int randomNumber = Global.GetRandomNumber(0, this.holyStampAttr[holyStampUpLeve.Type][holyStampUpLeve.Quality].Count);
												if (this.holyStampAttr[holyStampUpLeve.Type][holyStampUpLeve.Quality][randomNumber] != null)
												{
													HolyStampAttr attrByQuality = this.GetAttrByQuality(holyStampUpLeve, goodsData, null);
													if (attrByQuality != null)
													{
														using (Dictionary<int, double>.Enumerator enumerator2 = attrByQuality.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> keyValuePair = enumerator2.Current;
																goodsData.WashProps.Add(attrByQuality.ID);
																goodsData.WashProps.Add(keyValuePair.Key);
																goodsData.WashProps.Add(Convert.ToInt32(keyValuePair.Value * 1000.0));
															}
														}
													}
												}
											}
											return goodsData.WashProps;
										}
									}
								}
							}
							else if (981 == systemXmlItem.GetIntValue("Categoriy", -1) && holyStampUpLeve.Level == -1)
							{
								return null;
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		public int GetCurrHolyMaxLevelExp(HolyStampUpLeve data, int currLevel)
		{
			int result;
			lock (this.Mutex)
			{
				if (data == null)
				{
					result = -1;
				}
				else if (!this.GoodsLvDict.ContainsKey(data.GoodID))
				{
					result = -1;
				}
				else if (!this.holyStampUpLeveL.ContainsKey(data.GoodID))
				{
					result = -1;
				}
				else
				{
					int num = this.GoodsLvDict[data.GoodID] - 1;
					foreach (HolyStampUpLeve holyStampUpLeve in this.holyStampUpLeveL[data.GoodID])
					{
						if (holyStampUpLeve.Level == num)
						{
							return holyStampUpLeve.UpExp;
						}
					}
					result = -1;
				}
			}
			return result;
		}

		public MountHolyOpcode ProcessHolyStampUpGrade(GameClient client, int Dbid, string PhagocytosisDBids)
		{
			int num = 1;
			bool flag = false;
			GoodsData goodsData = null;
			goodsData = this.GetGoodsByDbID(client, Dbid);
			if (goodsData == null)
			{
				num = 5;
			}
			else
			{
				HolyStampUpLeve holyUpGradeInfo = this.GetHolyUpGradeInfo(goodsData);
				if (holyUpGradeInfo == null)
				{
					num = 9;
				}
				else if (!this.GoodsLvDict.ContainsKey(goodsData.GoodsID))
				{
					num = 9;
				}
				else if (holyUpGradeInfo.Level == this.GoodsLvDict[goodsData.GoodsID])
				{
					num = 10;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
					{
						LogManager.WriteLog(2, string.Format("圣印升级类型错误{0}", goodsData.GoodsID), null, true);
						num = 5;
					}
					else if (980 != systemXmlItem.GetIntValue("Categoriy", -1))
					{
						num = 4;
					}
					else if (string.IsNullOrEmpty(PhagocytosisDBids))
					{
						num = 3;
					}
					else
					{
						Dictionary<int, int> dictionary = new Dictionary<int, int>();
						string[] array = PhagocytosisDBids.Split(new char[]
						{
							'|'
						});
						foreach (string text in array)
						{
							if (!string.IsNullOrEmpty(text))
							{
								string[] array3 = text.Split(new char[]
								{
									','
								});
								if (array3 != null && array3.Length == 2)
								{
									int num2 = Convert.ToInt32(array3[0]);
									int num3 = Convert.ToInt32(array3[1]);
									if (num2 == Dbid)
									{
										num = 11;
										break;
									}
									if (dictionary.ContainsKey(num2))
									{
										Dictionary<int, int> dictionary2;
										int key;
										(dictionary2 = dictionary)[key = num2] = dictionary2[key] + num3;
									}
									else
									{
										dictionary.Add(num2, num3);
									}
								}
							}
						}
						if (num != 11)
						{
							int num4 = this.GetCurrHolyMaxLevelExp(holyUpGradeInfo, goodsData.ElementhrtsProps[0]);
							if (num4 <= 0)
							{
								num = 9;
							}
							else
							{
								num4 -= goodsData.ElementhrtsProps[1];
								num4 = Math.Max(0, num4);
								int num5 = 0;
								int j = 0;
								Dictionary<int, Dictionary<int, GoodsData>> dictionary3 = new Dictionary<int, Dictionary<int, GoodsData>>();
								lock (dictionary)
								{
									foreach (KeyValuePair<int, int> keyValuePair in dictionary)
									{
										GoodsData goodsByDbID = this.GetGoodsByDbID(client, keyValuePair.Key);
										if (goodsByDbID == null)
										{
											num = 6;
											flag = true;
											break;
										}
										if (goodsByDbID.GCount < keyValuePair.Value || keyValuePair.Value <= 0)
										{
											num = 7;
											flag = true;
											break;
										}
										SystemXmlItem systemXmlItem2 = null;
										if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem2))
										{
											LogManager.WriteLog(2, string.Format("系统中不存在{0}", goodsByDbID.GoodsID), null, true);
											num = 8;
											flag = true;
											break;
										}
										HolyStampUpLeve holyUpGradeInfo2 = this.GetHolyUpGradeInfo(goodsByDbID);
										if (holyUpGradeInfo2 == null)
										{
											num = 8;
											flag = true;
											break;
										}
										bool flag3 = false;
										int num6 = 0;
										if (981 == systemXmlItem2.GetIntValue("Categoriy", -1))
										{
											for (int k = 0; k < keyValuePair.Value; k++)
											{
												num5 += holyUpGradeInfo2.PhagocytosisExp;
												num6++;
												if (num5 > num4)
												{
													flag3 = true;
													break;
												}
											}
										}
										else
										{
											if (980 != systemXmlItem2.GetIntValue("Categoriy", -1))
											{
												num = 11;
												flag = true;
												break;
											}
											num5 += holyUpGradeInfo2.PhagocytosisExp;
										}
										j++;
										Dictionary<int, GoodsData> dictionary4 = new Dictionary<int, GoodsData>();
										if (981 == systemXmlItem2.GetIntValue("Categoriy", -1))
										{
											dictionary4.Add(num6, goodsByDbID);
										}
										else
										{
											dictionary4.Add(keyValuePair.Value, goodsByDbID);
										}
										if (dictionary3.ContainsKey(j))
										{
											dictionary3[j] = dictionary4;
										}
										else
										{
											dictionary3.Add(j, dictionary4);
										}
										if (num5 > num4 || flag3)
										{
											break;
										}
									}
								}
								if (!flag)
								{
									int value = 0;
									HolyStampUpLeve levelUpCount = this.GetLevelUpCount(goodsData, holyUpGradeInfo, num5, out value);
									if (levelUpCount == null)
									{
										num = 12;
									}
									else if (!this.GoodsLvAttrCount.ContainsKey(goodsData.GoodsID))
									{
										num = 13;
									}
									else if (!this.GoodsLvAttrCount[goodsData.GoodsID].ContainsKey(levelUpCount.Level))
									{
										num = 13;
									}
									else
									{
										List<int> list = new List<int>();
										if (goodsData.WashProps == null)
										{
											if (this.GoodsLvAttrCount[goodsData.GoodsID][levelUpCount.Level] > 0)
											{
												int l = 0;
												while (j < this.GoodsLvAttrCount[goodsData.GoodsID][levelUpCount.Level])
												{
													HolyStampAttr attrByQuality = this.GetAttrByQuality(levelUpCount, goodsData, list);
													if (attrByQuality != null)
													{
														list.Add(attrByQuality.ID);
														using (Dictionary<int, double>.Enumerator enumerator2 = attrByQuality.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> keyValuePair2 = enumerator2.Current;
																list.Add(keyValuePair2.Key);
																int item = (int)(keyValuePair2.Value * 1000.0);
																list.Add(item);
															}
														}
													}
													l++;
												}
											}
										}
										else if (goodsData.WashProps.Count >= 0)
										{
											int num6 = goodsData.WashProps.Count / 3;
											num6 = this.GoodsLvAttrCount[goodsData.GoodsID][levelUpCount.Level] - num6;
											if (num6 > 0)
											{
												list.AddRange(goodsData.WashProps);
												for (int l = 0; l < num6; l++)
												{
													HolyStampAttr attrByQuality = this.GetAttrByQuality(levelUpCount, goodsData, list);
													if (attrByQuality != null)
													{
														list.Add(attrByQuality.ID);
														using (Dictionary<int, double>.Enumerator enumerator2 = attrByQuality.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> keyValuePair2 = enumerator2.Current;
																list.Add(keyValuePair2.Key);
																int item = (int)(keyValuePair2.Value * 1000.0);
																list.Add(item);
															}
														}
													}
												}
											}
										}
										flag = false;
										foreach (KeyValuePair<int, Dictionary<int, GoodsData>> keyValuePair3 in dictionary3)
										{
											foreach (KeyValuePair<int, GoodsData> keyValuePair4 in keyValuePair3.Value)
											{
												bool flag4;
												if (!RebornStone.RebornHoleRemoveUseGoods(client, keyValuePair4.Value, keyValuePair4.Key, out flag4))
												{
													num = 15;
													flag = true;
													break;
												}
											}
										}
										if (!flag)
										{
											goodsData.ElementhrtsProps[0] = levelUpCount.Level;
											if (levelUpCount.Level >= this.GoodsLvDict[goodsData.GoodsID])
											{
												goodsData.ElementhrtsProps[1] = 0;
											}
											else
											{
												goodsData.ElementhrtsProps[1] = value;
											}
											UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
											{
												RoleID = client.ClientData.RoleID,
												DbID = goodsData.Id
											};
											updateGoodsArgs.ElementhrtsProps = new List<int>();
											updateGoodsArgs.ElementhrtsProps.Add(goodsData.ElementhrtsProps[0]);
											updateGoodsArgs.ElementhrtsProps.Add(goodsData.ElementhrtsProps[1]);
											int num7 = 3;
											if (goodsData.WashProps != null)
											{
												num7 = goodsData.WashProps.Count;
											}
											if (list.Count >= num7)
											{
												updateGoodsArgs.WashProps = new List<int>();
												updateGoodsArgs.WashProps.AddRange(list);
											}
											if (goodsData.Using == 1)
											{
												Global.RefreshEquipProp(client);
												GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
											}
											if (Global.UpdateGoodsProp(client, goodsData, updateGoodsArgs, true) < 0)
											{
												num = 12;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			byte[] array4;
			if (num == 1)
			{
				array4 = DataHelper.ObjectToBytes<GoodsData>(goodsData);
			}
			else
			{
				array4 = DataHelper.ObjectToBytes<GoodsData>(new GoodsData
				{
					Id = -1,
					GoodsID = num
				});
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array4, 0, array4.Length, 2090);
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
			return MountHolyOpcode.Succ;
		}

		public bool ModifyHolyStampState(GameClient client, int DBid, int IsUsing, int Site, int Bagindex)
		{
			string[] array = null;
			string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
			{
				client.ClientData.RoleID,
				DBid,
				IsUsing,
				"*",
				"*",
				"*",
				Site,
				"*",
				"*",
				"*",
				"*",
				Bagindex,
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
			return tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED && array.Length > 0 && Convert.ToInt32(array[1]) >= 0;
		}

		public MountHolyOpcode ProcessHolyStampUsing(GameClient client, int Dbid)
		{
			GoodsData goodsByDbID = this.GetGoodsByDbID(client, Dbid);
			MountHolyOpcode result;
			if (goodsByDbID == null)
			{
				result = MountHolyOpcode.NotExsitGood;
			}
			else if (goodsByDbID.Using == 1)
			{
				result = MountHolyOpcode.GoodHasUsing;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem))
				{
					LogManager.WriteLog(2, string.Format("系统中不存在{0}", goodsByDbID.GoodsID), null, true);
					result = MountHolyOpcode.NotExsitGoodXml;
				}
				else if (980 != systemXmlItem.GetIntValue("Categoriy", -1))
				{
					result = MountHolyOpcode.NotUsingType;
				}
				else
				{
					int golaNum = this.GetGolaNum(client);
					if (golaNum <= 0)
					{
						result = MountHolyOpcode.GetHoleNumErr;
					}
					else
					{
						HolyStampUpLeve holyUpGradeInfo = this.GetHolyUpGradeInfo(goodsByDbID);
						if (holyUpGradeInfo == null)
						{
							result = MountHolyOpcode.NotExsitInfo;
						}
						else if (holyUpGradeInfo.Site > golaNum)
						{
							result = MountHolyOpcode.CurrHoleLock;
						}
						else
						{
							GoodsData usingGoodDataByBagindex = this.GetUsingGoodDataByBagindex(client, holyUpGradeInfo.Site);
							if (usingGoodDataByBagindex != null)
							{
								int num;
								if (MountHolyStampManager.CanAddGoods(client, usingGoodDataByBagindex.GoodsID, 1, usingGoodDataByBagindex.Binding, "1900-01-01 12:00:00", true))
								{
									num = MountHolyStampManager.GetIdleSlotOfGoods(client);
								}
								else
								{
									if (client.ClientData.HolyGoodsDataList == null || client.ClientData.HolyGoodsDataList.Count != MountHolyStampManager.HolyBagNum)
									{
										return MountHolyOpcode.HolyGoodListNotFree;
									}
									num = goodsByDbID.BagIndex;
								}
								if (!this.ModifyHolyStampState(client, usingGoodDataByBagindex.Id, 0, 16000, num))
								{
									LogManager.WriteLog(2, string.Format("卸下圣印数据出错 GoodsID={0} DBID={1}", goodsByDbID.GoodsID, usingGoodDataByBagindex.Id), null, true);
									return MountHolyOpcode.DataModifyErr;
								}
								usingGoodDataByBagindex.Using = 0;
								usingGoodDataByBagindex.BagIndex = num;
								GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, usingGoodDataByBagindex.Id, usingGoodDataByBagindex.Using, usingGoodDataByBagindex.Site, usingGoodDataByBagindex.GCount, usingGoodDataByBagindex.BagIndex, 1);
							}
							if (!this.ModifyHolyStampState(client, goodsByDbID.Id, 1, 16000, holyUpGradeInfo.Site))
							{
								LogManager.WriteLog(2, string.Format("佩戴圣印数据出错 GoodsID={0} DBID={1}", goodsByDbID.GoodsID, usingGoodDataByBagindex.Id), null, true);
								result = MountHolyOpcode.DataModifyErr;
							}
							else
							{
								goodsByDbID.Using = 1;
								goodsByDbID.BagIndex = holyUpGradeInfo.Site;
								GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, goodsByDbID.Id, goodsByDbID.Using, goodsByDbID.Site, goodsByDbID.GCount, goodsByDbID.BagIndex, 1);
								if (goodsByDbID.Using == 1)
								{
									Global.RefreshEquipProp(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
								}
								result = MountHolyOpcode.Succ;
							}
						}
					}
				}
			}
			return result;
		}

		public MountHolyOpcode ProcessHolyStampDisUsing(GameClient client, int Dbid)
		{
			GoodsData goodsByDbID = this.GetGoodsByDbID(client, Dbid);
			MountHolyOpcode result;
			if (goodsByDbID == null)
			{
				result = MountHolyOpcode.NotExsitGood;
			}
			else if (goodsByDbID.Using <= 0)
			{
				result = MountHolyOpcode.GoodHasNot;
			}
			else
			{
				HolyStampUpLeve holyUpGradeInfo = this.GetHolyUpGradeInfo(goodsByDbID);
				if (holyUpGradeInfo == null)
				{
					result = MountHolyOpcode.NotExsitInfo;
				}
				else
				{
					GoodsData usingGoodDataByBagindex = this.GetUsingGoodDataByBagindex(client, holyUpGradeInfo.Site);
					if (usingGoodDataByBagindex == null)
					{
						result = MountHolyOpcode.HoleInfoIsNull;
					}
					else if (!MountHolyStampManager.CanAddGoods(client, usingGoodDataByBagindex.GoodsID, 1, usingGoodDataByBagindex.Binding, "1900-01-01 12:00:00", true))
					{
						result = MountHolyOpcode.HolyGoodListNotFree;
					}
					else
					{
						int idleSlotOfGoods = MountHolyStampManager.GetIdleSlotOfGoods(client);
						if (!this.ModifyHolyStampState(client, usingGoodDataByBagindex.Id, 0, 16000, idleSlotOfGoods))
						{
							LogManager.WriteLog(2, string.Format("卸下圣印数据出错 GoodsID={0} DBID={1}", goodsByDbID.GoodsID, usingGoodDataByBagindex.Id), null, true);
							result = MountHolyOpcode.DataModifyErr;
						}
						else
						{
							usingGoodDataByBagindex.Using = 0;
							usingGoodDataByBagindex.BagIndex = idleSlotOfGoods;
							GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, usingGoodDataByBagindex.Id, usingGoodDataByBagindex.Using, usingGoodDataByBagindex.Site, usingGoodDataByBagindex.GCount, usingGoodDataByBagindex.BagIndex, 1);
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							result = MountHolyOpcode.Succ;
						}
					}
				}
			}
			return result;
		}

		public static GoodsData GetMountHolyGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.HolyGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Id == dbID)
						{
							return client.ClientData.HolyGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		private void ResetHolyStampBag(GameClient client, bool notifyClient = true)
		{
			byte[] array = null;
			if (client.ClientData.HolyGoodsDataList != null)
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							client.ClientData.HolyGoodsDataList[i].BagIndex = 0;
							int goodsGridNumByID = Global.GetGoodsGridNumByID(client.ClientData.HolyGoodsDataList[i].GoodsID);
							if (goodsGridNumByID > 1)
							{
								GoodsData goodsData = null;
								string key = string.Format("{0}_{1}_{2}_{3}", new object[]
								{
									client.ClientData.HolyGoodsDataList[i].GoodsID,
									client.ClientData.HolyGoodsDataList[i].Binding,
									Global.DateTimeTicks(client.ClientData.HolyGoodsDataList[i].Starttime),
									Global.DateTimeTicks(client.ClientData.HolyGoodsDataList[i].Endtime)
								});
								if (dictionary.TryGetValue(key, out goodsData))
								{
									int num = Global.GMin(goodsGridNumByID - goodsData.GCount, client.ClientData.HolyGoodsDataList[i].GCount);
									goodsData.GCount += num;
									client.ClientData.HolyGoodsDataList[i].GCount -= num;
									client.ClientData.HolyGoodsDataList[i].BagIndex = 1;
									goodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.HolyGoodsDataList[i]))
									{
										break;
									}
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, num, goodsData.GCount, "整理圣印背包");
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, client.ClientData.HolyGoodsDataList[i].GoodsID, (long)client.ClientData.HolyGoodsDataList[i].Id, -num, client.ClientData.HolyGoodsDataList[i].GCount, "整理圣印背包");
									if (goodsData.GCount >= goodsGridNumByID)
									{
										if (client.ClientData.HolyGoodsDataList[i].GCount > 0)
										{
											dictionary[key] = client.ClientData.HolyGoodsDataList[i];
										}
										else
										{
											dictionary.Remove(key);
											list.Add(client.ClientData.HolyGoodsDataList[i]);
										}
									}
									else if (client.ClientData.HolyGoodsDataList[i].GCount <= 0)
									{
										list.Add(client.ClientData.HolyGoodsDataList[i]);
									}
								}
								else
								{
									dictionary[key] = client.ClientData.HolyGoodsDataList[i];
								}
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						client.ClientData.HolyGoodsDataList.Remove(list[i]);
					}
					client.ClientData.HolyGoodsDataList.Sort(delegate(GoodsData _left, GoodsData _right)
					{
						int num3 = 0;
						if (_left.ElementhrtsProps != null && _right.ElementhrtsProps != null)
						{
							num3 = _left.ElementhrtsProps[0] - _right.ElementhrtsProps[0];
						}
						int result;
						if (num3 > 0)
						{
							result = -1;
						}
						else if (num3 < 0)
						{
							result = 1;
						}
						else
						{
							HolyStampUpLeve holyUpGradeInfo = this.GetHolyUpGradeInfo(_left);
							if (holyUpGradeInfo == null)
							{
								result = -1;
							}
							else
							{
								HolyStampUpLeve holyUpGradeInfo2 = this.GetHolyUpGradeInfo(_right);
								if (holyUpGradeInfo2 == null)
								{
									result = -1;
								}
								else
								{
									int quality = holyUpGradeInfo.Quality;
									int quality2 = holyUpGradeInfo2.Quality;
									if (quality > quality2)
									{
										result = -1;
									}
									else if (quality < quality2)
									{
										result = 1;
									}
									else
									{
										result = _right.GoodsID - _left.GoodsID;
									}
								}
							}
						}
						return result;
					});
					int num2 = 0;
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							if (GameManager.Flag_OptimizationBagReset)
							{
								bool flag2 = client.ClientData.HolyGoodsDataList[i].BagIndex > 0;
								client.ClientData.HolyGoodsDataList[i].BagIndex = num2++;
								if (flag2)
								{
									if (!Global.ResetBagGoodsData(client, client.ClientData.HolyGoodsDataList[i]))
									{
										break;
									}
								}
							}
							else
							{
								client.ClientData.HolyGoodsDataList[i].BagIndex = num2++;
								if (!Global.ResetBagGoodsData(client, client.ClientData.HolyGoodsDataList[i]))
								{
									break;
								}
							}
						}
					}
					array = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.HolyGoodsDataList);
				}
				if (notifyClient)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 2093);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		public void InitRoleHolyStampGoodsData(GameClient client)
		{
			if (null == client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 16000), client.ServerId);
				if (null == client.ClientData.HolyGoodsDataList)
				{
					client.ClientData.HolyGoodsDataList = new List<GoodsData>();
				}
			}
			if (null == client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 16000), client.ServerId);
				if (null == client.ClientData.HolyGoodsDataList)
				{
					client.ClientData.HolyGoodsDataList = new List<GoodsData>();
				}
			}
			this.ResetHolyStampBag(client, true);
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				default(DelayExecProcIds),
				2
			});
		}

		public static bool IsHolyStamp(int goodID)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodID);
			return goodsCatetoriy == 980;
		}

		public const int DefaultLevel = 1;

		public Dictionary<int, HolyStampSuit> holyStampSuit = new Dictionary<int, HolyStampSuit>();

		public Dictionary<int, Dictionary<int, List<HolyStampAttr>>> holyStampAttr = new Dictionary<int, Dictionary<int, List<HolyStampAttr>>>();

		public Dictionary<int, List<HolyStampUpLeve>> holyStampUpLeveL = new Dictionary<int, List<HolyStampUpLeve>>();

		public Dictionary<int, int> holyStampDesbloquear = new Dictionary<int, int>();

		public Dictionary<int, int> GoodsLvDict = new Dictionary<int, int>();

		public Dictionary<int, Dictionary<int, int>> GoodsLvAttrCount = new Dictionary<int, Dictionary<int, int>>();

		protected object Mutex = new object();

		public static int HolyBagNum = 200;

		private static MountHolyStampManager instance = new MountHolyStampManager();
	}
}
