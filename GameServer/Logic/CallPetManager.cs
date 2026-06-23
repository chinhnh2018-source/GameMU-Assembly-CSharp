using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	internal class CallPetManager
	{
		public static void LoadCallPetType()
		{
			try
			{
				lock (CallPetManager._CallPetMutex)
				{
					CallPetManager.CallPetTypeDict.Clear();
					string uri = "Config/CallPetType.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
					if (null == xelement)
					{
						LogManager.WriteLog(1000, "加载Config/CallPetType.xml时出错!!!文件不存在", null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							CallPetType callPetType = new CallPetType();
							callPetType.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
							callPetType.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
							callPetType.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
							callPetType.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MaxZhuanSheng");
							callPetType.MaxLevel = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
							CallPetManager.CallPetTypeDict[callPetType.ID] = callPetType;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "加载Config/CallPetType.xml时文件出错", ex, true);
			}
		}

		public static void LoadCallPetConfig()
		{
			try
			{
				lock (CallPetManager._CallPetMutex)
				{
					CallPetManager.CallPetConfigList.Clear();
					CallPetManager.FreeCallPetConfigList.Clear();
					CallPetManager.HuoDongCallPetConfigList.Clear();
					CallPetManager.TeQuanCallPetConfigList.Clear();
					string uri = "Config/CallPet.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
					if (null == xelement)
					{
						LogManager.WriteLog(1000, "加载Config/CallPet.xml时出错!!!文件不存在", null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							CallPetConfig callPetConfig = new CallPetConfig();
							callPetConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
							callPetConfig.GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
							callPetConfig.Num = (int)Global.GetSafeAttributeLong(xml, "Num");
							callPetConfig.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xml, "QiangHuaFallID");
							callPetConfig.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuiJiaFallID");
							callPetConfig.LckyProbability = (int)Global.GetSafeAttributeLong(xml, "LckyProbability");
							callPetConfig.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuoYueFallID");
							callPetConfig.MinMoney = (int)Global.GetSafeAttributeLong(xml, "MinMoney");
							callPetConfig.MaxMoney = (int)Global.GetSafeAttributeLong(xml, "MaxMoney");
							callPetConfig.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MinBindYuanBao");
							callPetConfig.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MaxBindYuanBao");
							callPetConfig.MinExp = (int)Global.GetSafeAttributeLong(xml, "MinExp");
							callPetConfig.MaxExp = (int)Global.GetSafeAttributeLong(xml, "MaxExp");
							callPetConfig.StartValues = (int)Global.GetSafeAttributeLong(xml, "StartValues");
							callPetConfig.EndValues = (int)Global.GetSafeAttributeLong(xml, "EndValues");
							CallPetManager.CallPetConfigList.Add(callPetConfig);
						}
						uri = "Config/FreeCallPet.xml";
						GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
						xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
						if (null == xelement)
						{
							LogManager.WriteLog(1000, "加载Config/FreeCallPet.xml时出错!!!文件不存在", null, true);
						}
						else
						{
							enumerable = xelement.Elements();
							foreach (XElement xml in enumerable)
							{
								CallPetConfig callPetConfig = new CallPetConfig();
								callPetConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
								callPetConfig.GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
								callPetConfig.Num = (int)Global.GetSafeAttributeLong(xml, "Num");
								callPetConfig.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xml, "QiangHuaFallID");
								callPetConfig.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuiJiaFallID");
								callPetConfig.LckyProbability = (int)Global.GetSafeAttributeLong(xml, "LckyProbability");
								callPetConfig.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuoYueFallID");
								callPetConfig.MinMoney = (int)Global.GetSafeAttributeLong(xml, "MinMoney");
								callPetConfig.MaxMoney = (int)Global.GetSafeAttributeLong(xml, "MaxMoney");
								callPetConfig.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MinBindYuanBao");
								callPetConfig.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MaxBindYuanBao");
								callPetConfig.MinExp = (int)Global.GetSafeAttributeLong(xml, "MinExp");
								callPetConfig.MaxExp = (int)Global.GetSafeAttributeLong(xml, "MaxExp");
								callPetConfig.StartValues = (int)Global.GetSafeAttributeLong(xml, "StartValues");
								callPetConfig.EndValues = (int)Global.GetSafeAttributeLong(xml, "EndValues");
								CallPetManager.FreeCallPetConfigList.Add(callPetConfig);
							}
							uri = "Config/HuoDongCallPet.xml";
							GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
							xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
							if (null == xelement)
							{
								LogManager.WriteLog(1000, "加载Config/HuoDongCallPet.xml时出错!!!文件不存在", null, true);
							}
							else
							{
								enumerable = xelement.Elements();
								foreach (XElement xml in enumerable)
								{
									CallPetConfig callPetConfig = new CallPetConfig();
									callPetConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
									callPetConfig.GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
									callPetConfig.Num = (int)Global.GetSafeAttributeLong(xml, "Num");
									callPetConfig.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xml, "QiangHuaFallID");
									callPetConfig.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuiJiaFallID");
									callPetConfig.LckyProbability = (int)Global.GetSafeAttributeLong(xml, "LckyProbability");
									callPetConfig.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuoYueFallID");
									callPetConfig.MinMoney = (int)Global.GetSafeAttributeLong(xml, "MinMoney");
									callPetConfig.MaxMoney = (int)Global.GetSafeAttributeLong(xml, "MaxMoney");
									callPetConfig.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MinBindYuanBao");
									callPetConfig.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MaxBindYuanBao");
									callPetConfig.MinExp = (int)Global.GetSafeAttributeLong(xml, "MinExp");
									callPetConfig.MaxExp = (int)Global.GetSafeAttributeLong(xml, "MaxExp");
									callPetConfig.StartValues = (int)Global.GetSafeAttributeLong(xml, "StartValues");
									callPetConfig.EndValues = (int)Global.GetSafeAttributeLong(xml, "EndValues");
									CallPetManager.HuoDongCallPetConfigList.Add(callPetConfig);
								}
								uri = "Config/TeQuanCallPet.xml";
								GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
								xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
								if (null == xelement)
								{
									LogManager.WriteLog(1000, "加载Config/TeQuanBuHuo.xml时出错!!!文件不存在", null, true);
								}
								else
								{
									enumerable = xelement.Elements();
									foreach (XElement xml in enumerable)
									{
										CallPetConfig callPetConfig = new CallPetConfig();
										callPetConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
										callPetConfig.GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
										callPetConfig.Num = (int)Global.GetSafeAttributeLong(xml, "Num");
										callPetConfig.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xml, "QiangHuaFallID");
										callPetConfig.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuiJiaFallID");
										callPetConfig.LckyProbability = (int)Global.GetSafeAttributeLong(xml, "LckyProbability");
										callPetConfig.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xml, "ZhuoYueFallID");
										callPetConfig.MinMoney = (int)Global.GetSafeAttributeLong(xml, "MinMoney");
										callPetConfig.MaxMoney = (int)Global.GetSafeAttributeLong(xml, "MaxMoney");
										callPetConfig.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MinBindYuanBao");
										callPetConfig.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xml, "MaxBindYuanBao");
										callPetConfig.MinExp = (int)Global.GetSafeAttributeLong(xml, "MinExp");
										callPetConfig.MaxExp = (int)Global.GetSafeAttributeLong(xml, "MaxExp");
										callPetConfig.StartValues = (int)Global.GetSafeAttributeLong(xml, "StartValues");
										callPetConfig.EndValues = (int)Global.GetSafeAttributeLong(xml, "EndValues");
										CallPetManager.TeQuanCallPetConfigList.Add(callPetConfig);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "加载Config/CallPet.xml或FreeCallPet.xml时文件出错", ex, true);
			}
		}

		public static void LoadCallPetSystem()
		{
			lock (CallPetManager._CallPetMutex)
			{
				CallPetManager.CallPetPriceDict.Clear();
				string[] array = GameManager.systemParamsList.GetParamValueByName("CallPet").Split(new char[]
				{
					','
				});
				if (array == null || array.Length != 2)
				{
					SysConOut.WriteLine("        加载SystemParams.xml时出错!!!CallPet不存在");
				}
				else
				{
					CallPetManager.CallPetPriceDict[1] = Convert.ToInt32(array[0]);
					CallPetManager.CallPetPriceDict[10] = Convert.ToInt32(array[1]);
					double paramValueDoubleByName = GameManager.systemParamsList.GetParamValueDoubleByName("FreeCallPet", 0.0);
					if (paramValueDoubleByName <= 0.0)
					{
						SysConOut.WriteLine("        加载SystemParams.xml时出错!!!FreeCallPet不存在");
					}
					else
					{
						CallPetManager.CallPetFreeHour = paramValueDoubleByName;
						double paramValueDoubleByName2 = GameManager.systemParamsList.GetParamValueDoubleByName("ConsumeCallPetJiFen", 0.0);
						if (paramValueDoubleByName2 < 0.0)
						{
							SysConOut.WriteLine("        加载SystemParams.xml时出错!!!ConsumeCallPetJiFen小于0");
						}
						else
						{
							CallPetManager.ConsumeCallPetJiFen = paramValueDoubleByName2;
							paramValueDoubleByName2 = GameManager.systemParamsList.GetParamValueDoubleByName("ZhaoHuan", 0.0);
							if (paramValueDoubleByName2 < 0.0)
							{
							}
							CallPetManager.CallPetGoodsID = (int)paramValueDoubleByName2;
						}
					}
				}
			}
		}

		public static CallPetType GetCallPetType(int type = 1)
		{
			CallPetType result = null;
			lock (CallPetManager._CallPetMutex)
			{
				if (CallPetManager.CallPetTypeDict.ContainsKey(type))
				{
					result = CallPetManager.CallPetTypeDict[type];
				}
			}
			return result;
		}

		public static List<CallPetConfig> GetCallPetConfigList(bool freeCall)
		{
			List<CallPetConfig> result;
			lock (CallPetManager._CallPetMutex)
			{
				if (freeCall)
				{
					result = CallPetManager.FreeCallPetConfigList;
				}
				else
				{
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPriorityActivity != null && specPriorityActivity.IsChouJiangOpen(SpecPActivityChouJiangType.TeQuanBuHuo))
					{
						result = CallPetManager.TeQuanCallPetConfigList;
					}
					else
					{
						JieRiFuLiActivity jieriFuLiActivity = HuodongCachingMgr.GetJieriFuLiActivity();
						object obj = null;
						if (jieriFuLiActivity != null && jieriFuLiActivity.IsOpened(EJieRiFuLiType.CallPetReplace, out obj))
						{
							result = CallPetManager.HuoDongCallPetConfigList;
						}
						else
						{
							result = CallPetManager.CallPetConfigList;
						}
					}
				}
			}
			return result;
		}

		public static int GetCallPetPrice(int times)
		{
			int result = -1;
			lock (CallPetManager._CallPetMutex)
			{
				if (CallPetManager.CallPetPriceDict.ContainsKey(times))
				{
					result = CallPetManager.CallPetPriceDict[times];
				}
			}
			return result;
		}

		public static GoodsData GetPetByDbID(GameClient client, int id)
		{
			if (null != client.ClientData.PetList)
			{
				for (int i = 0; i < client.ClientData.PetList.Count; i++)
				{
					if (client.ClientData.PetList[i].Id == id)
					{
						return client.ClientData.PetList[i];
					}
				}
			}
			return null;
		}

		public static void AddPetData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 4000)
			{
				if (null == client.ClientData.PetList)
				{
					client.ClientData.PetList = new List<GoodsData>();
				}
				lock (client.ClientData.PetList)
				{
					client.ClientData.PetList.Add(goodsData);
				}
			}
		}

		public static GoodsData AddPetData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, int idelBagIndex, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = idelBagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			CallPetManager.AddPetData(client, goodsData);
			return goodsData;
		}

		public static void RemovePetGoodsData(GameClient client, GoodsData goodsData)
		{
			lock (client.ClientData.PetList)
			{
				if (null != client.ClientData.PetList)
				{
					client.ClientData.PetList.Remove(goodsData);
				}
			}
		}

		public static int GetIdleSlotOfBag(GameClient client)
		{
			int num = -1;
			int result;
			if (null == client.ClientData.PetList)
			{
				result = 0;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.PetList.Count; i++)
				{
					if (list.IndexOf(client.ClientData.PetList[i].BagIndex) < 0)
					{
						list.Add(client.ClientData.PetList[i].BagIndex);
					}
				}
				for (int j = 0; j < CallPetManager.GetMaxPetCount(); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						return j;
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetPetListCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.PetList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.PetList.Count;
			}
			return result;
		}

		public static int GetMaxPetCount()
		{
			return CallPetManager.MaxPetGridNum;
		}

		public static long getFreeSec(GameClient client)
		{
			double offsetSecond = Global.GetOffsetSecond(TimeUtil.NowDateTime());
			double num = Convert.ToDouble(Global.GetRoleParamByName(client, "CallPetFreeTime"));
			double num2 = CallPetManager.CallPetFreeHour * 60.0 * 60.0;
			return (long)Global.GMax(0.0, num + num2 - offsetSecond);
		}

		public static void ResetPetBagAllGoods(GameClient client)
		{
			if (null != client.ClientData.PetList)
			{
				lock (client.ClientData.PetList)
				{
					Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.PetList.Count; i++)
					{
						if (client.ClientData.PetList[i].Using <= 0)
						{
							client.ClientData.PetList[i].BagIndex = 1;
							int goodsGridNumByID = Global.GetGoodsGridNumByID(client.ClientData.PetList[i].GoodsID);
							if (goodsGridNumByID > 1)
							{
								GoodsData goodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.PetList[i].GoodsID, client.ClientData.PetList[i].Binding, Global.DateTimeTicks(client.ClientData.PetList[i].Endtime));
								if (dictionary.TryGetValue(key, out goodsData))
								{
									int num = Global.GMin(goodsGridNumByID - goodsData.GCount, client.ClientData.PetList[i].GCount);
									goodsData.GCount += num;
									client.ClientData.PetList[i].GCount -= num;
									client.ClientData.PetList[i].BagIndex = 1;
									goodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.PetList[i]))
									{
										break;
									}
									if (goodsData.GCount >= goodsGridNumByID)
									{
										if (client.ClientData.PetList[i].GCount > 0)
										{
											dictionary[key] = client.ClientData.PetList[i];
										}
										else
										{
											dictionary.Remove(key);
											list.Add(client.ClientData.PetList[i]);
										}
									}
									else if (client.ClientData.PetList[i].GCount <= 0)
									{
										list.Add(client.ClientData.PetList[i]);
									}
								}
								else
								{
									dictionary[key] = client.ClientData.PetList[i];
								}
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						client.ClientData.PetList.Remove(list[i]);
					}
					client.ClientData.PetList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int num2 = 0;
					for (int i = 0; i < client.ClientData.PetList.Count; i++)
					{
						if (client.ClientData.PetList[i].Using <= 0)
						{
							bool flag2 = 0 == 0;
							client.ClientData.PetList[i].BagIndex = num2++;
							if (!Global.ResetBagGoodsData(client, client.ClientData.PetList[i]))
							{
								break;
							}
						}
					}
				}
			}
			TCPOutPacket tcpOutPacket = null;
			if (null != client.ClientData.PetList)
			{
				lock (client.ClientData.PetList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.PetList, Global._TCPManager.TcpOutPacketPool, 754);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.PetList, Global._TCPManager.TcpOutPacketPool, 754);
			}
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		public static CallSpriteResult CallPet(GameClient client, int times, out string strGetGoods)
		{
			strGetGoods = "";
			CallSpriteResult result;
			if (times != 1 && times != 10)
			{
				result = CallSpriteResult.ErrorParams;
			}
			else
			{
				CallPetType callPetType = CallPetManager.GetCallPetType(1);
				if (null == callPetType)
				{
					result = CallSpriteResult.ErrorConfig;
				}
				else if (client.ClientData.Level < callPetType.MinLevel)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else if (client.ClientData.Level > callPetType.MaxLevel)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else if (client.ClientData.ChangeLifeCount < callPetType.MinZhuanSheng)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else if (client.ClientData.ChangeLifeCount > callPetType.MaxZhuanSheng)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else
				{
					bool flag = false;
					bool flag2 = false;
					int num = 0;
					if (1 == times)
					{
						if (CallPetManager.getFreeSec(client) <= 0L)
						{
							flag = true;
							num = 1;
						}
					}
					if (!flag && CallPetManager.CallPetGoodsID > 0)
					{
						if (1 == times)
						{
							if (null != Global.GetGoodsByID(client, CallPetManager.CallPetGoodsID))
							{
								flag2 = true;
								num = 1;
							}
						}
					}
					int callPetPrice = CallPetManager.GetCallPetPrice(times);
					if (callPetPrice < 0)
					{
						result = CallSpriteResult.ErrorConfig;
					}
					else
					{
						if (!flag && !flag2)
						{
							if (Global.IsRoleHasEnoughMoney(client, callPetPrice, 163) < 0 && !HuanLeDaiBiManager.GetInstance().HuanledaibiReplaceEnough(client, callPetPrice, DaiBiSySType.JingLingLieQu))
							{
								return CallSpriteResult.ZuanShiNotEnough;
							}
						}
						if (CallPetManager.GetMaxPetCount() - CallPetManager.GetPetListCount(client) < times)
						{
							result = CallSpriteResult.SpriteBagIsFull;
						}
						else
						{
							if (!flag)
							{
								if (flag2)
								{
									bool flag3 = false;
									bool flag4 = false;
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, CallPetManager.CallPetGoodsID, 1, false, out flag3, out flag4, false))
									{
										flag2 = false;
									}
								}
							}
							if (!flag && !flag2)
							{
								if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -callPetPrice, "精灵召唤", false, DaiBiSySType.JingLingLieQu))
								{
									return CallSpriteResult.ZuanShiNotEnough;
								}
								num = 0;
							}
							for (int i = 0; i < times; i++)
							{
								CallPetConfig callPetConfig = null;
								List<CallPetConfig> callPetConfigList = CallPetManager.GetCallPetConfigList(flag || flag2);
								if (callPetConfigList == null || callPetConfigList.Count <= 0)
								{
									return CallSpriteResult.ErrorConfig;
								}
								int randomNumber = Global.GetRandomNumber(1, 100001);
								foreach (CallPetConfig callPetConfig2 in callPetConfigList)
								{
									if (randomNumber >= callPetConfig2.StartValues && randomNumber <= callPetConfig2.EndValues)
									{
										callPetConfig = callPetConfig2;
										break;
									}
								}
								LogManager.WriteLog(0, string.Format("获取精灵随机数: random = {0}, GoodsID = {1}", randomNumber, callPetConfig.GoodsID), null, true);
								if (null != callPetConfig)
								{
									int num2 = 0;
									if (callPetConfig.ZhuoYueFallID != -1)
									{
										num2 = GameManager.GoodsPackMgr.GetExcellencePropertysID(callPetConfig.GoodsID, callPetConfig.ZhuoYueFallID);
									}
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, callPetConfig.GoodsID, callPetConfig.Num, 0, "", 0, num, 4000, "", false, 1, "精灵召唤", "1900-01-01 12:00:00", 0, 0, 0, 0, num2, 0, 0, null, null, 0, true);
									strGetGoods += string.Format("{0},{1},{2},{3},{4},{5},{6}|", new object[]
									{
										callPetConfig.GoodsID,
										callPetConfig.Num,
										num,
										0,
										0,
										0,
										num2
									});
								}
							}
							if (flag)
							{
								Global.UpdateRoleParamByName(client, "CallPetFreeTime", Global.GetOffsetSecond(TimeUtil.NowDateTime()).ToString(), true);
								if (client._IconStateMgr.CheckPetIcon(client))
								{
									client._IconStateMgr.SendIconStateToClient(client);
								}
							}
							else if (!flag2)
							{
								int addValue = (int)((double)callPetPrice * CallPetManager.ConsumeCallPetJiFen);
								GameManager.ClientMgr.ModifyPetJiFenValue(client, addValue, "精灵召唤", false, true);
							}
							result = CallSpriteResult.Success;
						}
					}
				}
			}
			return result;
		}

		public static CallSpriteResult MovePet(GameClient client, int dbid)
		{
			GoodsData petByDbID = CallPetManager.GetPetByDbID(client, dbid);
			CallSpriteResult result;
			if (null == petByDbID)
			{
				result = CallSpriteResult.GoodsNotExist;
			}
			else if (!Global.CanAddGoods(client, petByDbID.GoodsID, petByDbID.GCount, petByDbID.Binding, "1900-01-01 12:00:00", true, false))
			{
				result = CallSpriteResult.BagIsFull;
			}
			else
			{
				string[] array = null;
				string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					dbid,
					"*",
					"*",
					"*",
					"*",
					0,
					"*",
					"*",
					1,
					"*",
					0,
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
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					result = CallSpriteResult.DBSERVERERROR;
				}
				else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
				{
					result = CallSpriteResult.DBSERVERERROR;
				}
				else
				{
					CallPetManager.RemovePetGoodsData(client, petByDbID);
					petByDbID.Site = 0;
					Global.AddGoodsData(client, petByDbID);
					result = CallSpriteResult.Success;
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessGetPetList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (1 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				byte[] buffer = DataHelper.ObjectToBytes<List<GoodsData>>(gameClient.ClientData.PetList);
				GameManager.ClientMgr.SendToClient(gameClient, buffer, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetPetList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessGetPetUIInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (1 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string data2 = string.Format("{0}:{1}", num, CallPetManager.getFreeSec(client));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetPetUIInfo", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessCallPetCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string text2 = "";
				CallSpriteResult callSpriteResult = CallPetManager.CallPet(client, num2, out text2);
				string data2;
				if (callSpriteResult != CallSpriteResult.Success)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						(int)callSpriteResult,
						num,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num2,
					text2,
					CallPetManager.getFreeSec(client)
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessCallPetCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessMovePetCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_OK;
		}

		public static TCPProcessCmdResults ProcessResetPetBagCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				CallPetManager.ResetPetBagAllGoods(client);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessResetPetBagCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private static object _CallPetMutex = new object();

		private static Dictionary<int, CallPetType> CallPetTypeDict = new Dictionary<int, CallPetType>();

		private static List<CallPetConfig> CallPetConfigList = new List<CallPetConfig>();

		private static List<CallPetConfig> FreeCallPetConfigList = new List<CallPetConfig>();

		private static List<CallPetConfig> HuoDongCallPetConfigList = new List<CallPetConfig>();

		private static List<CallPetConfig> TeQuanCallPetConfigList = new List<CallPetConfig>();

		private static double CallPetFreeHour = 60.0;

		private static Dictionary<int, int> CallPetPriceDict = new Dictionary<int, int>();

		private static double ConsumeCallPetJiFen = 0.1;

		private static int CallPetGoodsID = 0;

		public static int MaxPetGridNum = 240;
	}
}
