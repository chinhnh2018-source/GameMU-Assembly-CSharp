using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class WuXingMapMgr
	{
		private static List<int> RandomIntList(List<int> list)
		{
			List<int> result;
			if (null == list)
			{
				result = null;
			}
			else
			{
				List<int> list2 = new List<int>();
				foreach (int item in list)
				{
					int randomNumber = Global.GetRandomNumber(0, list2.Count);
					list2.Insert(randomNumber, item);
				}
				result = list2;
			}
			return result;
		}

		public static int GetNextMapCodeByNPCID(int mapCode, int npcID)
		{
			WuXingNPCItem wuXingNPCItem = null;
			string key = string.Format("{0}_{1}", mapCode, npcID);
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			lock (WuXingMapMgr.WuXingNPCDict)
			{
				if (!WuXingMapMgr.WuXingNPCDict.TryGetValue(key, out wuXingNPCItem))
				{
					return -1;
				}
				if (null == wuXingNPCItem.MapItem)
				{
					return -1;
				}
				if (dayOfYear != wuXingNPCItem.MapItem.DayID)
				{
					wuXingNPCItem.MapItem.DayID = dayOfYear;
					wuXingNPCItem.MapItem.GoToMapCodeList = WuXingMapMgr.RandomIntList(wuXingNPCItem.MapItem.GoToMapCodeList);
				}
				if (wuXingNPCItem.MapItem.GoToMapCodeList == null || wuXingNPCItem.MapItem.OtherNPCIDList == null || wuXingNPCItem.MapItem.GoToMapCodeList.Count != wuXingNPCItem.MapItem.OtherNPCIDList.Count)
				{
					return -1;
				}
				for (int i = 0; i < wuXingNPCItem.MapItem.OtherNPCIDList.Count; i++)
				{
					if (npcID == wuXingNPCItem.MapItem.OtherNPCIDList[i])
					{
						return wuXingNPCItem.MapItem.GoToMapCodeList[i];
					}
				}
			}
			return -1;
		}

		public static int GetNeedGoodsIDByNPCID(int mapCode, int npcID)
		{
			WuXingNPCItem wuXingNPCItem = null;
			string key = string.Format("{0}_{1}", mapCode, npcID);
			int result;
			lock (WuXingMapMgr.WuXingNPCDict)
			{
				if (!WuXingMapMgr.WuXingNPCDict.TryGetValue(key, out wuXingNPCItem))
				{
					result = -1;
				}
				else
				{
					result = wuXingNPCItem.NeedGoodsID;
				}
			}
			return result;
		}

		private static List<int> Str2IntArray(string str)
		{
			List<int> result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				List<int> list = new List<int>();
				string[] array = str.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						list.Add(Convert.ToInt32(array[i]));
					}
					catch (Exception)
					{
					}
				}
				result = list;
			}
			return result;
		}

		private static WuXingMapItem ParseGlobalConfigItem(int globalID, string otherNPCIDs, string goToMaps)
		{
			WuXingMapItem wuXingMapItem = null;
			WuXingMapItem result;
			if (WuXingMapMgr.WuXingMapDict.TryGetValue(globalID, out wuXingMapItem))
			{
				result = wuXingMapItem;
			}
			else
			{
				wuXingMapItem = new WuXingMapItem
				{
					GlobalID = globalID,
					OtherNPCIDList = WuXingMapMgr.Str2IntArray(otherNPCIDs),
					GoToMapCodeList = WuXingMapMgr.Str2IntArray(goToMaps)
				};
				if (wuXingMapItem.OtherNPCIDList == null || wuXingMapItem.GoToMapCodeList == null || wuXingMapItem.OtherNPCIDList.Count != wuXingMapItem.GoToMapCodeList.Count)
				{
					throw new Exception(string.Format("解析五行奇阵配置文件时，解析NPC列表或者地图列表失败, GlobalID={0}", globalID));
				}
				WuXingMapMgr.WuXingMapDict[globalID] = wuXingMapItem;
				result = wuXingMapItem;
			}
			return result;
		}

		private static void ParseWuXingXmlItem(SystemXmlItem systemXmlItem)
		{
			int intValue = systemXmlItem.GetIntValue("NPCID", -1);
			int intValue2 = systemXmlItem.GetIntValue("MapCode", -1);
			int intValue3 = systemXmlItem.GetIntValue("NeedGoodsID", -1);
			int intValue4 = systemXmlItem.GetIntValue("GlobalID", -1);
			string stringValue = systemXmlItem.GetStringValue("OtherNPCIDs");
			string stringValue2 = systemXmlItem.GetStringValue("GoToMaps");
			WuXingMapItem mapItem = WuXingMapMgr.ParseGlobalConfigItem(intValue4, stringValue, stringValue2);
			WuXingNPCItem value = new WuXingNPCItem
			{
				NPCID = intValue,
				MapCode = intValue2,
				NeedGoodsID = intValue3,
				MapItem = mapItem
			};
			string key = string.Format("{0}_{1}", intValue2, intValue);
			WuXingMapMgr.WuXingNPCDict[key] = value;
		}

		public static void LoadXuXingConfig()
		{
			XElement xelement = null;
			string text = "Config/WuXing.xml";
			try
			{
				xelement = XElement.Load(Global.GameResPath(text));
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements();
			foreach (XElement xmlnode in enumerable)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = xmlnode
				};
				WuXingMapMgr.ParseWuXingXmlItem(systemXmlItem);
			}
		}

		private static List<GoodsData> ParseGoodsDataList(string[] fields)
		{
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] array = fields[i].Split(new char[]
				{
					','
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(2, string.Format("解析WuXingAwards.xml文件中的奖励项时失败, 物品配置项个数错误", new object[0]), null, true);
				}
				else
				{
					int[] array2 = Global.StringArray2IntArray(array);
					GoodsData newGoodsData = Global.GetNewGoodsData(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5], 0, 0, 0, 0, 0);
					list.Add(newGoodsData);
				}
			}
			return list;
		}

		public static void ParseWuXingAwardItem(SystemXmlItem systemXmlItem)
		{
			List<GoodsData> goodsDataList = null;
			string stringValue = systemXmlItem.GetStringValue("GoodsIDs");
			if (!string.IsNullOrEmpty(stringValue))
			{
				string[] array = stringValue.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					goodsDataList = WuXingMapMgr.ParseGoodsDataList(array);
				}
				else
				{
					LogManager.WriteLog(2, string.Format("解析WuXingAwards.xml配置项中的物品奖励失败, MapCode={0}", systemXmlItem.GetIntValue("MapCode", -1)), null, true);
				}
			}
			else
			{
				LogManager.WriteLog(2, string.Format("解析WuXingAwards.xml配置项中的物品奖励失败, MapCode={0}", systemXmlItem.GetIntValue("MapCode", -1)), null, true);
			}
			WuXingMapMgr.TheWuXingMapAwardItem = new WuXingMapAwardItem
			{
				MapCode = systemXmlItem.GetIntValue("MapCode", -1),
				Money1 = systemXmlItem.GetIntValue("Moneyaward", -1),
				ExpXiShu = systemXmlItem.GetDoubleValue("ExpXiShu"),
				GoodsDataList = goodsDataList,
				MinBlessPoint = systemXmlItem.GetIntValue("MinBlessPoint", -1),
				MaxBlessPoint = systemXmlItem.GetIntValue("MaxBlessPoint", -1)
			};
		}

		public static void LoadWuXingAward()
		{
			XElement xelement = null;
			string text = "Config/WuXingAwards.xml";
			try
			{
				xelement = XElement.Load(Global.GameResPath(text));
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements();
			using (IEnumerator<XElement> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					XElement xmlnode = enumerator.Current;
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = xmlnode
					};
					WuXingMapMgr.ParseWuXingAwardItem(systemXmlItem);
				}
			}
			if (null == WuXingMapMgr.TheWuXingMapAwardItem)
			{
				throw new Exception(string.Format("加载五行奇阵的最顶层奖励项失败!", new object[0]));
			}
		}

		public static bool CanGetWuXingAward(GameClient client)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			int num = -1;
			int num2 = 0;
			if (null != client.ClientData.MyRoleDailyData)
			{
				num = client.ClientData.MyRoleDailyData.WuXingDayID;
				num2 = client.ClientData.MyRoleDailyData.WuXingNum;
			}
			return dayOfYear != num || num2 <= 0;
		}

		public static void ProcessWuXingAward(GameClient client)
		{
			if (WuXingMapMgr.CanGetWuXingAward(client))
			{
				if (null != WuXingMapMgr.TheWuXingMapAwardItem)
				{
					if (null != WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList)
					{
						if (!Global.CanAddGoodsDataList(client, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList))
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(574, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
							return;
						}
					}
					int num = 0;
					if (WuXingMapMgr.TheWuXingMapAwardItem.MinBlessPoint >= 0 && WuXingMapMgr.TheWuXingMapAwardItem.MaxBlessPoint >= 0)
					{
						num = Global.GetRandomNumber(WuXingMapMgr.TheWuXingMapAwardItem.MinBlessPoint, WuXingMapMgr.TheWuXingMapAwardItem.MaxBlessPoint);
						if (num > 0)
						{
							if (client.ClientData.HorseDbID <= 0)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(575, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								return;
							}
						}
					}
					if (Global.FilterFallGoods(client))
					{
						if (null != WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList)
						{
							for (int i = 0; i < WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].GoodsID, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].GCount, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Quality, "", WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Forge_level, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Binding, 0, "", true, 1, "五行奇阵奖励物品", "1900-01-01 12:00:00", WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].AddPropIndex, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].BornIndex, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Lucky, WuXingMapMgr.TheWuXingMapAwardItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
							}
						}
					}
					GameManager.ClientMgr.UpdateRoleDailyData_WuXingNum(client, 1);
					double expXiShu = WuXingMapMgr.TheWuXingMapAwardItem.ExpXiShu;
					int num2 = (int)Math.Pow((double)client.ClientData.Level, expXiShu);
					if (DBRoleBufferManager.ProcessMonthVIP(client) > 0.0)
					{
						num2 = (int)((double)num2 * 1.5);
					}
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)num2, true, false, false, "none");
					Global.BroadcastWuXingExperience(client, num2);
					int num3 = WuXingMapMgr.TheWuXingMapAwardItem.Money1;
					if (-1 != num3)
					{
						num3 = Global.FilterValue(client, num3);
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num3, "五行奇阵", false);
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.Money1,
							num3
						}), EventLevels.Record);
					}
					int currentHorseBlessPoint = ProcessHorse.GetCurrentHorseBlessPoint(client);
					if (currentHorseBlessPoint > 0 && num > 0)
					{
						double num4 = (double)num / 100.0;
						num = (int)(num4 * (double)currentHorseBlessPoint);
						num = Global.FilterValue(client, num);
						ProcessHorse.ProcessAddHorseAwardLucky(client, num, true, "五行奇阵奖励");
					}
					Global.AddWuXingAwardEvent(client, num2, num);
				}
			}
		}

		private static Dictionary<int, WuXingMapItem> WuXingMapDict = new Dictionary<int, WuXingMapItem>();

		private static Dictionary<string, WuXingNPCItem> WuXingNPCDict = new Dictionary<string, WuXingNPCItem>();

		private static Dictionary<int, int> ClientsAwardsDict = new Dictionary<int, int>();

		private static WuXingMapAwardItem TheWuXingMapAwardItem = null;
	}
}
