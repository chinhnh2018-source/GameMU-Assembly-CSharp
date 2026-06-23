using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class FuBenManager
	{
		public static int FindFuBenSeqIDByRoleID(int roleID)
		{
			int num = 0;
			int result;
			lock (FuBenManager._FuBenSeqIDDict)
			{
				if (FuBenManager._FuBenSeqIDDict.TryGetValue(roleID, out num))
				{
					result = num;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public static FuBenInfoItem FindFuBenInfoBySeqID(int fuBenSeqID)
		{
			FuBenInfoItem fuBenInfoItem = null;
			FuBenInfoItem result;
			lock (FuBenManager._FuBenSeqID2InfoDict)
			{
				if (!FuBenManager._FuBenSeqID2InfoDict.TryGetValue(fuBenSeqID, out fuBenInfoItem))
				{
					result = null;
				}
				else
				{
					result = fuBenInfoItem;
				}
			}
			return result;
		}

		public static void AddFuBenSeqID(int roleID, int fuBenSeqID, int goodsBinding, int fuBenID)
		{
			lock (FuBenManager._FuBenSeqIDDict)
			{
				FuBenManager._FuBenSeqIDDict[roleID] = fuBenSeqID;
			}
			lock (FuBenManager._FuBenSeqID2InfoDict)
			{
				if (!FuBenManager._FuBenSeqID2InfoDict.ContainsKey(fuBenSeqID))
				{
					FuBenManager._FuBenSeqID2InfoDict[fuBenSeqID] = new FuBenInfoItem
					{
						FuBenSeqID = fuBenSeqID,
						StartTicks = TimeUtil.NOW(),
						EndTicks = 0L,
						GoodsBinding = goodsBinding,
						FuBenID = fuBenID
					};
				}
			}
		}

		public static void RemoveFuBenSeqID(int roleID)
		{
			int num = -1;
			lock (FuBenManager._FuBenSeqIDDict)
			{
				if (FuBenManager._FuBenSeqIDDict.TryGetValue(roleID, out num))
				{
					FuBenManager._FuBenSeqIDDict.Remove(roleID);
				}
				else
				{
					num = -1;
				}
			}
		}

		public static void RemoveFuBenInfoBySeqID(int fuBenSeqID)
		{
			if (fuBenSeqID != -1)
			{
				lock (FuBenManager._FuBenSeqID2InfoDict)
				{
					FuBenManager._FuBenSeqID2InfoDict.Remove(fuBenSeqID);
				}
			}
		}

		public static int GetFuBenSeqId(int tag = 0)
		{
			string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", tag), 0);
			if (array != null && array.Length >= 2)
			{
				int num = Global.SafeConvertToInt32(array[1]);
				if (num > 0)
				{
					return num;
				}
			}
			return 0;
		}

		public static FuBenMapItem FindMapCodeByFuBenID(int fuBenID, int mapCode)
		{
			FuBenMapItem fuBenMapItem = null;
			string key = string.Format("{0}_{1}", fuBenID, mapCode);
			FuBenMapItem result;
			if (!FuBenManager._FuBenMapCode2MapItemDict.TryGetValue(key, out fuBenMapItem))
			{
				result = null;
			}
			else
			{
				result = fuBenMapItem;
			}
			return result;
		}

		public static List<FuBenMapItem> GetAllFubenMapItem()
		{
			List<FuBenMapItem> list = new List<FuBenMapItem>();
			lock (FuBenManager._FuBenMapCode2MapItemDict)
			{
				list.AddRange(FuBenManager._FuBenMapCode2MapItemDict.Values);
			}
			return list;
		}

		public static List<int> FindMapCodeListByFuBenID(int fuBenID)
		{
			List<int> list = null;
			List<int> result;
			if (!FuBenManager._FuBen2MapCodeListDict.TryGetValue(fuBenID, out list))
			{
				result = null;
			}
			else
			{
				result = list;
			}
			return result;
		}

		public static int FindFuBenIDByMapCode(int mapCode)
		{
			int num = -1;
			int result;
			if (!FuBenManager._MapCode2FuBenDict.TryGetValue(mapCode, out num))
			{
				result = -1;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public static bool IsFuBenMap(int mapCode)
		{
			bool result = FuBenManager.FindFuBenIDByMapCode(mapCode) > 0;
			if (Global.GetMapType(mapCode) == MapTypes.HuanYingSiYuan)
			{
				result = true;
			}
			return result;
		}

		public static int FindNextMapCodeByFuBenID(int mapCode)
		{
			int num = FuBenManager.FindFuBenIDByMapCode(mapCode);
			int result;
			if (num <= 0)
			{
				result = -1;
			}
			else
			{
				List<int> list = FuBenManager.FindMapCodeListByFuBenID(num);
				if (null == list)
				{
					result = -1;
				}
				else
				{
					int num2 = list.IndexOf(mapCode);
					if (-1 == num2)
					{
						result = -1;
					}
					else if (num2 >= list.Count - 1)
					{
						result = -1;
					}
					else
					{
						result = list[num2 + 1];
					}
				}
			}
			return result;
		}

		public static int FindMapCodeIndexByFuBenID(int mapCode)
		{
			int num = FuBenManager.FindFuBenIDByMapCode(mapCode);
			int result;
			if (num <= 0)
			{
				result = 0;
			}
			else
			{
				List<int> list = FuBenManager.FindMapCodeListByFuBenID(num);
				if (null == list)
				{
					result = 0;
				}
				else
				{
					int num2 = list.IndexOf(mapCode);
					if (-1 == num2)
					{
						result = 0;
					}
					else
					{
						result = num2 + 1;
					}
				}
			}
			return result;
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
				if (!(fields[i] == "1") && !(fields[i] == ""))
				{
					if (array.Length != 7)
					{
						LogManager.WriteLog(2, string.Format("解析FuBenMap.xml文件中的奖励项时失败, 物品配置项个数错误", new object[0]), null, true);
					}
					else
					{
						int[] array2 = Global.StringArray2IntArray(array);
						GoodsData newGoodsData = Global.GetNewGoodsData(array2[0], array2[1], 0, array2[3], array2[2], 0, array2[5], 0, array2[6], array2[4], 0);
						list.Add(newGoodsData);
					}
				}
			}
			return list;
		}

		private static void ParseXmlItem(SystemXmlItem systemXmlItem)
		{
			int intValue = systemXmlItem.GetIntValue("MapCode", -1);
			int intValue2 = systemXmlItem.GetIntValue("CopyID", -1);
			int intValue3 = systemXmlItem.GetIntValue("MaxTime", -1);
			int intValue4 = systemXmlItem.GetIntValue("Moneyaward", -1);
			int intValue5 = systemXmlItem.GetIntValue("Experienceaward", -1);
			int intValue6 = systemXmlItem.GetIntValue("FirstGold", -1);
			int intValue7 = systemXmlItem.GetIntValue("FirstExp", -1);
			int intValue8 = systemXmlItem.GetIntValue("MinSaoDangTime", -1);
			int intValue9 = systemXmlItem.GetIntValue("XingHunaward", -1);
			int intValue10 = systemXmlItem.GetIntValue("FirstXingHun", -1);
			int intValue11 = systemXmlItem.GetIntValue("ZhanGongaward", -1);
			int intValue12 = systemXmlItem.GetIntValue("YuanSuFenMoaward", -1);
			int intValue13 = systemXmlItem.GetIntValue("YingGuangaward", -1);
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
					goodsDataList = FuBenManager.ParseGoodsDataList(array);
				}
			}
			List<GoodsData> firstGoodsDataList = null;
			string stringValue2 = systemXmlItem.GetStringValue("FirstGoodsID");
			if (!string.IsNullOrEmpty(stringValue2))
			{
				string[] array = stringValue2.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					firstGoodsDataList = FuBenManager.ParseGoodsDataList(array);
				}
			}
			FuBenMapItem value = new FuBenMapItem
			{
				FuBenID = intValue2,
				MapCode = intValue,
				MaxTime = intValue3,
				Money1 = intValue4,
				Experience = intValue5,
				GoodsDataList = goodsDataList,
				FirstGoodsDataList = firstGoodsDataList,
				MinSaoDangTimer = intValue8,
				nFirstExp = intValue7,
				nFirstGold = intValue6,
				nXingHunAward = intValue9,
				nFirstXingHunAward = intValue10,
				nZhanGongaward = intValue11,
				YuanSuFenMoaward = intValue12,
				LightAward = intValue13
			};
			string key = string.Format("{0}_{1}", intValue2, intValue);
			lock (FuBenManager._FuBenMapCode2MapItemDict)
			{
				FuBenManager._FuBenMapCode2MapItemDict[key] = value;
			}
			List<int> list = null;
			if (!FuBenManager._FuBen2MapCodeListDict.TryGetValue(intValue2, out list))
			{
				list = new List<int>();
				FuBenManager._FuBen2MapCodeListDict[intValue2] = list;
			}
			list.Add(intValue);
			FuBenManager._MapCode2FuBenDict[intValue] = intValue2;
		}

		public static void LoadFuBenMap()
		{
			XElement xelement = null;
			string text = "Config/FuBenMap.xml";
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
				FuBenManager.ParseXmlItem(systemXmlItem);
			}
		}

		public static int GetFuBenMaxTimeSecs(int fuBenId)
		{
			SystemXmlItem systemXmlItem = null;
			FuBenMapItem fuBenMapItem = null;
			int result;
			if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenId, out systemXmlItem) || (fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenId, systemXmlItem.GetIntValue("MapCode", -1))) == null)
			{
				result = 15;
			}
			else
			{
				result = fuBenMapItem.MaxTime * 60;
			}
			return result;
		}

		public static bool CanFuBenMapFallGoodsAutoGet(GameClient client)
		{
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
			bool result;
			if (null == fuBenInfoItem)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(fuBenInfoItem.FuBenID, out systemXmlItem))
				{
					result = false;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("CopyType", -1);
					result = (0 == intValue);
				}
			}
			return result;
		}

		public static int GetFuBenMapAwardsGoodsBinding(GameClient client)
		{
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			return FuBenManager.GetFuBenMapAwardsGoodsBinding(fuBenSeqID);
		}

		public static int GetFuBenMapAwardsGoodsBinding(int fuBenSeqID)
		{
			FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
			int result;
			if (null == fuBenInfoItem)
			{
				result = 0;
			}
			else
			{
				result = fuBenInfoItem.GoodsBinding;
			}
			return result;
		}

		public static bool CanGetFuBenMapAwards(GameClient client)
		{
			int num = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
			bool result;
			if (num <= 0)
			{
				result = false;
			}
			else if (client.ClientData.FuBenSeqID <= 0)
			{
				result = false;
			}
			else
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
				if (null == fuBenInfoItem)
				{
					result = false;
				}
				else if (num != fuBenInfoItem.FuBenID)
				{
					result = false;
				}
				else
				{
					FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num, client.ClientData.MapCode);
					if (null == fuBenMapItem)
					{
						result = false;
					}
					else
					{
						if (fuBenMapItem.GoodsDataList == null || fuBenMapItem.GoodsDataList.Count <= 0)
						{
							if (fuBenMapItem.Experience <= 0)
							{
								if (fuBenMapItem.Money1 <= 0)
								{
									return false;
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		public static bool CanAutoGetFuBenMapAwards(GameClient client)
		{
			int num = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
			bool result;
			if (num <= 0)
			{
				result = false;
			}
			else if (client.ClientData.FuBenSeqID <= 0)
			{
				result = false;
			}
			else
			{
				FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
				if (null == fuBenInfoItem)
				{
					result = false;
				}
				else if (num != fuBenInfoItem.FuBenID)
				{
					result = false;
				}
				else
				{
					FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num, client.ClientData.MapCode);
					if (null == fuBenMapItem)
					{
						result = false;
					}
					else
					{
						if (fuBenMapItem.GoodsDataList == null || fuBenMapItem.GoodsDataList.Count <= 0)
						{
							if (fuBenMapItem.Experience <= 0)
							{
								if (fuBenMapItem.Money1 <= 0)
								{
									return false;
								}
							}
						}
						result = (fuBenMapItem.GoodsDataList == null || fuBenMapItem.GoodsDataList.Count <= 0);
					}
				}
			}
			return result;
		}

		public static bool ProcessFuBenMapAwards(GameClient client, bool notifyClient = false)
		{
			bool result;
			if (client.ClientData.FuBenSeqID < 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(113, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				int num = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, client.ClientData.FuBenSeqID, client.ClientData.MapCode);
				if (num > 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(21, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = false;
				}
				else
				{
					int num2 = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
					if (num2 <= 0)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(114, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
						if (null == fuBenInfoItem)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(115, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = false;
						}
						else if (num2 != fuBenInfoItem.FuBenID)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(116, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = false;
						}
						else
						{
							FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num2, client.ClientData.MapCode);
							if (null == fuBenMapItem)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(117, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								result = false;
							}
							else
							{
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode);
								if (copyMap == null)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(118, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									result = false;
								}
								else
								{
									GameManager.CopyMapMgr.AddAwardState(client.ClientData.RoleID, client.ClientData.FuBenSeqID, client.ClientData.MapCode, 1);
									int nMaxTime = fuBenMapItem.MaxTime * 60;
									long startTicks = fuBenInfoItem.StartTicks;
									long endTicks = fuBenInfoItem.EndTicks;
									int nFinishTimer = (int)(endTicks - startTicks) / 1000;
									int killedNormalNum = 0;
									int nDieCount = fuBenInfoItem.nDieCount;
									FuBenTongGuanData fuBenTongGuanData = Global.GiveCopyMapGiftForScore(client, num2, client.ClientData.MapCode, nMaxTime, nFinishTimer, killedNormalNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, null);
									if (fuBenTongGuanData != null)
									{
										TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenTongGuanData>(fuBenTongGuanData, Global._TCPManager.TcpOutPacketPool, 521);
										if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
										{
										}
									}
									Global.AddFuBenAwardEvent(client, num2);
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static Dictionary<int, int> _FuBenSeqIDDict = new Dictionary<int, int>();

		private static Dictionary<int, FuBenInfoItem> _FuBenSeqID2InfoDict = new Dictionary<int, FuBenInfoItem>();

		private static Dictionary<string, FuBenMapItem> _FuBenMapCode2MapItemDict = new Dictionary<string, FuBenMapItem>();

		private static Dictionary<int, List<int>> _FuBen2MapCodeListDict = new Dictionary<int, List<int>>();

		private static Dictionary<int, int> _MapCode2FuBenDict = new Dictionary<int, int>();
	}
}
