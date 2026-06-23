using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class JunQiManager
	{
		public static void AddKillJunQiItem(int mapCode, int npcID, int bhid)
		{
			string key = string.Format("{0}_{1}", mapCode, npcID);
			lock (JunQiManager.KillJunQiDict)
			{
				JunQiManager.KillJunQiDict[key] = new KillJunQiItem
				{
					BHID = bhid,
					KillJunQiTicks = TimeUtil.NOW()
				};
			}
		}

		public static bool CanInstallJunQiNow(int mapCode, int npcExtentionID, int bhid)
		{
			KillJunQiItem killJunQiItem = null;
			long num = TimeUtil.NOW();
			string key = string.Format("{0}_{1}", mapCode, npcExtentionID);
			lock (JunQiManager.KillJunQiDict)
			{
				if (!JunQiManager.KillJunQiDict.TryGetValue(key, out killJunQiItem))
				{
					return true;
				}
				if (killJunQiItem.BHID == bhid)
				{
					return true;
				}
				if (num - killJunQiItem.KillJunQiTicks >= 10000L)
				{
					return true;
				}
			}
			return false;
		}

		public static void LoadBangHuiJunQiItemsDictFromDBServer()
		{
			byte[] array = null;
			if (TCPProcessCmdResults.RESULT_FAILED != Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10073, string.Format("{0}", GameManager.ServerLineID), out array, 0))
			{
				if (array != null && array.Length > 6)
				{
					int num = BitConverter.ToInt32(array, 0);
					Dictionary<int, BangHuiJunQiItemData> bangHuiJunQiItemsDict = JunQiManager._BangHuiJunQiItemsDict;
					Dictionary<int, BangHuiJunQiItemData> dictionary = DataHelper.BytesToObject<Dictionary<int, BangHuiJunQiItemData>>(array, 6, num - 2);
					if (null != dictionary)
					{
						foreach (int key in dictionary.Keys)
						{
							if (bangHuiJunQiItemsDict == null || !bangHuiJunQiItemsDict.ContainsKey(key))
							{
								BangHuiJunQiItemData bangHuiJunQiItemData = dictionary[key];
							}
							else
							{
								BangHuiJunQiItemData bangHuiJunQiItemData = dictionary[key];
								BangHuiJunQiItemData bangHuiJunQiItemData2 = bangHuiJunQiItemsDict[key];
								if (bangHuiJunQiItemData.QiLevel != bangHuiJunQiItemData2.QiLevel)
								{
								}
							}
						}
					}
					JunQiManager._BangHuiJunQiItemsDict = dictionary;
				}
			}
		}

		public static void NotifySyncBangHuiJunQiItemsDict(GameClient client)
		{
			string text = string.Format("-syncjunqi", new object[0]);
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				(client != null) ? client.ClientData.RoleID : -1,
				"",
				0,
				"",
				0,
				text,
				0,
				0,
				-1
			}), null, 0);
		}

		public static int GetJunQiLevelByBHID(int bhid)
		{
			int result;
			if (null == JunQiManager._BangHuiJunQiItemsDict)
			{
				result = 0;
			}
			else
			{
				BangHuiJunQiItemData bangHuiJunQiItemData = null;
				if (!JunQiManager._BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
				{
					result = 0;
				}
				else
				{
					result = bangHuiJunQiItemData.QiLevel;
				}
			}
			return result;
		}

		public static string GetJunQiNameByBHID(int bhid)
		{
			string result;
			if (null == JunQiManager._BangHuiJunQiItemsDict)
			{
				result = GLang.GetLang(393, new object[0]);
			}
			else
			{
				BangHuiJunQiItemData bangHuiJunQiItemData = null;
				if (!JunQiManager._BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
				{
					result = GLang.GetLang(393, new object[0]);
				}
				else
				{
					result = bangHuiJunQiItemData.QiName;
				}
			}
			return result;
		}

		public static Dictionary<int, BangHuiLingDiItemData> LoadBangHuiLingDiItemsDictFromDBServer(int serverId)
		{
			byte[] array = null;
			Dictionary<int, BangHuiLingDiItemData> result;
			if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10074, string.Format("{0}", GameManager.ServerLineID), out array, serverId))
			{
				result = null;
			}
			else if (array == null || array.Length <= 6)
			{
				result = null;
			}
			else
			{
				int num = BitConverter.ToInt32(array, 0);
				Dictionary<int, BangHuiLingDiItemData> dictionary = DataHelper.BytesToObject<Dictionary<int, BangHuiLingDiItemData>>(array, 6, num - 2);
				result = dictionary;
			}
			return result;
		}

		public static void LoadBangHuiLingDiItemsDictFromDBServer()
		{
			Dictionary<int, BangHuiLingDiItemData> bangHuiLingDiItemsDict = JunQiManager._BangHuiLingDiItemsDict;
			Dictionary<int, BangHuiLingDiItemData> dictionary = JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer(0);
			bool flag = false;
			if (null != dictionary)
			{
				foreach (int num in dictionary.Keys)
				{
					if (bangHuiLingDiItemsDict == null || !bangHuiLingDiItemsDict.ContainsKey(num))
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = dictionary[num];
						GameManager.ClientMgr.NotifyAllLingDiForBHMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bangHuiLingDiItemData.LingDiID, bangHuiLingDiItemData.BHID, bangHuiLingDiItemData.ZoneID, bangHuiLingDiItemData.BHName, bangHuiLingDiItemData.LingDiTax);
						if (num == 7)
						{
							flag = true;
						}
					}
					else
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = dictionary[num];
						BangHuiLingDiItemData bangHuiLingDiItemData2 = bangHuiLingDiItemsDict[num];
						if (bangHuiLingDiItemData.BHID != bangHuiLingDiItemData2.BHID || bangHuiLingDiItemData.LingDiTax != bangHuiLingDiItemData2.LingDiTax)
						{
							GameManager.ClientMgr.NotifyAllLingDiForBHMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bangHuiLingDiItemData.LingDiID, bangHuiLingDiItemData.BHID, bangHuiLingDiItemData.ZoneID, bangHuiLingDiItemData.BHName, bangHuiLingDiItemData.LingDiTax);
							if (num == 7)
							{
								flag = true;
							}
						}
					}
				}
			}
			JunQiManager._BangHuiLingDiItemsDict = dictionary;
			if (flag)
			{
				LuoLanChengZhanManager.getInstance().BangHuiLingDiItemsDictFromDBServer();
			}
			Global.UpdateWangChengZhanWeekDays(true);
		}

		public static Dictionary<int, BangHuiLingDiItemData> GetBangHuiLingDiItemsDict()
		{
			return JunQiManager._BangHuiLingDiItemsDict;
		}

		public static void NotifySyncBangHuiLingDiItemsDict()
		{
			JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
			string text = string.Format("-synclingdi", new object[0]);
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				text,
				0,
				0,
				-1
			}), null, 0);
		}

		public static BangHuiLingDiItemData GetItemByLingDiID(int lingDiID)
		{
			BangHuiLingDiItemData result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = null;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				if (!JunQiManager._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiItemData))
				{
					result = null;
				}
				else
				{
					result = bangHuiLingDiItemData;
				}
			}
			return result;
		}

		public static int GetBHIDByLingDiID(int lingDiID)
		{
			int result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = 0;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				if (!JunQiManager._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiItemData))
				{
					result = 0;
				}
				else
				{
					result = bangHuiLingDiItemData.BHID;
				}
			}
			return result;
		}

		public static int GetTaxByLingDiID(int lingDiID)
		{
			int result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = 0;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				if (!JunQiManager._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiItemData))
				{
					result = 0;
				}
				else
				{
					result = bangHuiLingDiItemData.LingDiTax;
				}
			}
			return result;
		}

		public static BangHuiLingDiItemData GetFirstLingDiItemDataByBHID(int bhid)
		{
			BangHuiLingDiItemData result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = null;
			}
			else
			{
				int num = 10000;
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				foreach (BangHuiLingDiItemData bangHuiLingDiItemData2 in JunQiManager._BangHuiLingDiItemsDict.Values)
				{
					if (bangHuiLingDiItemData2.LingDiID != 1)
					{
						if (bangHuiLingDiItemData2.BHID == bhid)
						{
							if (bangHuiLingDiItemData2.LingDiID < num)
							{
								num = bangHuiLingDiItemData2.LingDiID;
								bangHuiLingDiItemData = bangHuiLingDiItemData2;
							}
						}
					}
				}
				result = bangHuiLingDiItemData;
			}
			return result;
		}

		public static BangHuiLingDiItemData GetAnyLingDiItemDataByBHID(int bhid)
		{
			BangHuiLingDiItemData result;
			if (null == JunQiManager._BangHuiLingDiItemsDict)
			{
				result = null;
			}
			else
			{
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				foreach (BangHuiLingDiItemData bangHuiLingDiItemData2 in JunQiManager._BangHuiLingDiItemsDict.Values)
				{
					if (bangHuiLingDiItemData2.LingDiID != 1)
					{
						if (bangHuiLingDiItemData2.BHID == bhid)
						{
							bangHuiLingDiItemData = bangHuiLingDiItemData2;
							break;
						}
					}
				}
				result = bangHuiLingDiItemData;
			}
			return result;
		}

		private static Point GetQiZuoNPCPosition(int mapCode, int npcID)
		{
			SystemXmlItem systemXmlItem = null;
			Point result;
			if (!GameManager.systemQiZuoMgr.SystemXmlItemDict.TryGetValue(mapCode, out systemXmlItem))
			{
				result = new Point(0.0, 0.0);
			}
			else
			{
				for (int i = 1; i <= JunQiManager.MaxInstallQiNum; i++)
				{
					if (npcID == systemXmlItem.GetIntValue(string.Format("NPC{0}", i), -1))
					{
						return new Point((double)systemXmlItem.GetIntValue(string.Format("NPC{0}PosX", i), -1), (double)systemXmlItem.GetIntValue(string.Format("NPC{0}PosY", i), -1));
					}
				}
				result = new Point(0.0, 0.0);
			}
			return result;
		}

		private static Point GetNormalMapJunQiPosition(int mapCode)
		{
			SystemXmlItem systemXmlItem = null;
			Point result;
			if (!GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict.TryGetValue(mapCode, out systemXmlItem))
			{
				result = new Point(0.0, 0.0);
			}
			else
			{
				result = new Point((double)systemXmlItem.GetIntValue("PosX", -1), (double)systemXmlItem.GetIntValue("PosY", -1));
			}
			return result;
		}

		private static JunQiItem AddJunQi(int mapCode, int bhid, int zoneID, string bhName, int npcID, string junQiName, int junQiLevel, SceneUIClasses sceneType = 0)
		{
			SystemXmlItem systemXmlItem = null;
			JunQiItem result;
			if (!GameManager.systemJunQiMgr.SystemXmlItemDict.TryGetValue(junQiLevel, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				Point point = new Point(0.0, 0.0);
				if (sceneType == 24)
				{
					if (-1 != npcID)
					{
						point = JunQiManager.GetQiZuoNPCPosition(mapCode, npcID);
					}
				}
				else
				{
					point = JunQiManager.GetNormalMapJunQiPosition(mapCode);
				}
				if (0.0 == point.X && 0.0 == point.Y)
				{
					result = null;
				}
				else
				{
					JunQiItem junQiItem = new JunQiItem
					{
						JunQiID = (int)GameManager.JunQiIDMgr.GetNewID(),
						QiName = junQiName,
						ZoneID = zoneID,
						BHID = bhid,
						BHName = bhName,
						JunQiLevel = junQiLevel,
						QiZuoNPC = npcID,
						MapCode = mapCode,
						PosX = (int)point.X,
						PosY = (int)point.Y,
						Direction = 0,
						LifeV = systemXmlItem.GetIntValue("Lifev", -1),
						StartTime = TimeUtil.NOW(),
						CurrentLifeV = systemXmlItem.GetIntValue("Lifev", -1),
						CutLifeV = systemXmlItem.GetIntValue("CutLifeV", -1),
						BodyCode = systemXmlItem.GetIntValue("BodyCode", -1),
						PicCode = systemXmlItem.GetIntValue("PicCode", -1),
						ManagerType = sceneType
					};
					if (-1 != npcID)
					{
						lock (JunQiManager._NPCID2JunQiDict)
						{
							JunQiManager._NPCID2JunQiDict[npcID] = junQiItem;
						}
					}
					lock (JunQiManager._ID2JunQiDict)
					{
						JunQiManager._ID2JunQiDict[junQiItem.JunQiID] = junQiItem;
					}
					result = junQiItem;
				}
			}
			return result;
		}

		public static JunQiItem FindJunQiByNpcID(int npcID)
		{
			JunQiItem result = null;
			lock (JunQiManager._NPCID2JunQiDict)
			{
				JunQiManager._NPCID2JunQiDict.TryGetValue(npcID, out result);
			}
			return result;
		}

		public static JunQiItem FindJunQiByID(int JunQiID)
		{
			JunQiItem result = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				JunQiManager._ID2JunQiDict.TryGetValue(JunQiID, out result);
			}
			return result;
		}

		public static JunQiItem FindJunQiByBHID(int bhid)
		{
			JunQiItem result = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem junQiItem in JunQiManager._ID2JunQiDict.Values)
				{
					if (junQiItem.BHID == bhid)
					{
						result = junQiItem;
						break;
					}
				}
			}
			return result;
		}

		private static void RemoveJunQi(int JunQiID)
		{
			JunQiItem junQiItem = null;
			lock (JunQiManager._ID2JunQiDict)
			{
				JunQiManager._ID2JunQiDict.TryGetValue(JunQiID, out junQiItem);
				if (null != junQiItem)
				{
					JunQiManager._ID2JunQiDict.Remove(junQiItem.JunQiID);
				}
			}
			if (null != junQiItem)
			{
				if (-1 != junQiItem.QiZuoNPC)
				{
					lock (JunQiManager._NPCID2JunQiDict)
					{
						JunQiManager._NPCID2JunQiDict.Remove(junQiItem.QiZuoNPC);
					}
				}
			}
		}

		private static int CalcJunQiNumByMapCode(int mapCode, out int bhid)
		{
			bhid = 0;
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem junQiItem in JunQiManager._ID2JunQiDict.Values)
				{
					if (junQiItem.MapCode == mapCode)
					{
						if (dictionary.ContainsKey(junQiItem.BHID))
						{
							dictionary[junQiItem.BHID] = dictionary[junQiItem.BHID] + 1;
						}
						else
						{
							dictionary[junQiItem.BHID] = 1;
						}
					}
				}
			}
			int num = 0;
			foreach (int num2 in dictionary.Keys)
			{
				if (dictionary[num2] > num)
				{
					num = dictionary[num2];
					bhid = num2;
				}
			}
			return num;
		}

		private static string GetBHNameByNPCID(int npcID)
		{
			JunQiItem junQiItem = JunQiManager.FindJunQiByNpcID(npcID);
			string result;
			if (null == junQiItem)
			{
				result = "";
			}
			else
			{
				result = Global.FormatBangHuiName(junQiItem.ZoneID, junQiItem.BHName);
			}
			return result;
		}

		public static bool ProcessNewJunQi(SocketListener sl, TCPOutPacketPool pool, int mapCode, int bhid, int zoneID, string bhName, int npcID, string junQiName, int junQiLevel, SceneUIClasses sceneType = 0)
		{
			JunQiItem junQiItem = JunQiManager.AddJunQi(mapCode, bhid, zoneID, bhName, npcID, junQiName, junQiLevel, sceneType);
			bool result;
			if (null == junQiItem)
			{
				LogManager.WriteLog(2, string.Format("为RoleID生成帮旗对象时失败, MapCode={0}, BHID={1}, BHName={2}, NPCID={3}, QiName={4}, QiLevel={5}", new object[]
				{
					mapCode,
					bhid,
					bhName,
					npcID,
					junQiName,
					junQiLevel
				}), null, true);
				result = false;
			}
			else
			{
				GameManager.MapGridMgr.DictGrids[junQiItem.MapCode].MoveObject(-1, -1, junQiItem.PosX, junQiItem.PosY, junQiItem);
				result = true;
			}
			return result;
		}

		public static void ProcessDelJunQi(SocketListener sl, TCPOutPacketPool pool, int JunQiID)
		{
			JunQiItem junQiItem = JunQiManager.FindJunQiByID(JunQiID);
			if (null != junQiItem)
			{
				JunQiManager.RemoveJunQi(JunQiID);
				GameManager.MapGridMgr.DictGrids[junQiItem.MapCode].RemoveObject(junQiItem);
			}
		}

		public static void ProcessDelAllJunQiByMapCode(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<JunQiItem> list = new List<JunQiItem>();
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem junQiItem in JunQiManager._ID2JunQiDict.Values)
				{
					if (junQiItem.MapCode == mapCode)
					{
						list.Add(junQiItem);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				JunQiManager.RemoveJunQi(list[i].JunQiID);
				GameManager.MapGridMgr.DictGrids[list[i].MapCode].RemoveObject(list[i]);
			}
		}

		public static void ProcessDelAllJunQiByBHID(SocketListener sl, TCPOutPacketPool pool, int bhid)
		{
			List<JunQiItem> list = new List<JunQiItem>();
			lock (JunQiManager._ID2JunQiDict)
			{
				foreach (JunQiItem junQiItem in JunQiManager._ID2JunQiDict.Values)
				{
					if (junQiItem.BHID == bhid)
					{
						list.Add(junQiItem);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				JunQiManager.RemoveJunQi(list[i].JunQiID);
				GameManager.MapGridMgr.DictGrids[list[i].MapCode].RemoveObject(list[i]);
			}
		}

		private static List<int> GetQiZuoNPCIDList(int mapCode)
		{
			SystemXmlItem systemXmlItem = null;
			List<int> result;
			if (!GameManager.systemQiZuoMgr.SystemXmlItemDict.TryGetValue(mapCode, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 1; i <= JunQiManager.MaxInstallQiNum; i++)
				{
					list.Add(systemXmlItem.GetIntValue(string.Format("NPC{0}", i), -1));
				}
				result = list;
			}
			return result;
		}

		public static void ProcessAllNewJunQiByMapCode(int mapCode, int bhid, int zoneID, string bhName)
		{
			List<int> qiZuoNPCIDList = JunQiManager.GetQiZuoNPCIDList(mapCode);
			if (null != qiZuoNPCIDList)
			{
				string junQiNameByBHID = JunQiManager.GetJunQiNameByBHID(bhid);
				int junQiLevelByBHID = JunQiManager.GetJunQiLevelByBHID(bhid);
				for (int i = 0; i < qiZuoNPCIDList.Count; i++)
				{
					JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCode, bhid, zoneID, bhName, qiZuoNPCIDList[i], junQiNameByBHID, junQiLevelByBHID, 0);
				}
			}
		}

		public static void NotifyOthersShowJunQi(SocketListener sl, TCPOutPacketPool pool, JunQiItem JunQiItem)
		{
			if (null != JunQiItem)
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].MoveObject(-1, -1, JunQiItem.PosX, JunQiItem.PosY, JunQiItem);
			}
		}

		public static void NotifyOthersHideJunQi(SocketListener sl, TCPOutPacketPool pool, JunQiItem JunQiItem)
		{
			if (null != JunQiItem)
			{
				GameManager.MapGridMgr.DictGrids[JunQiItem.MapCode].RemoveObject(JunQiItem);
			}
		}

		private static bool ProcessJunQiDead(SocketListener sl, TCPOutPacketPool pool, long nowTicks, JunQiItem JunQiItem)
		{
			bool result;
			if (JunQiItem.CurrentLifeV > 0)
			{
				result = false;
			}
			else
			{
				long num = nowTicks - JunQiItem.JunQiDeadTicks;
				if (num < 2000L)
				{
					result = false;
				}
				else
				{
					JunQiManager.ProcessDelJunQi(sl, pool, JunQiItem.JunQiID);
					JunQiManager.NotifyAllLingDiMapInfoData(JunQiItem.MapCode);
					result = true;
				}
			}
			return result;
		}

		public static void ProcessAllJunQiItems(SocketListener sl, TCPOutPacketPool pool)
		{
			if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
			{
				List<JunQiItem> list = new List<JunQiItem>();
				lock (JunQiManager._ID2JunQiDict)
				{
					foreach (JunQiItem item in JunQiManager._ID2JunQiDict.Values)
					{
						list.Add(item);
					}
				}
				long nowTicks = TimeUtil.NOW();
				for (int i = 0; i < list.Count; i++)
				{
					JunQiItem junQiItem = list[i];
					if (JunQiManager.ProcessJunQiDead(sl, pool, nowTicks, junQiItem))
					{
					}
				}
			}
		}

		public static void SendMySelfJunQiItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is JunQiItem)
					{
						if ((objsList[i] as JunQiItem).CurrentLifeV > 0)
						{
							JunQiItem junQiItem = objsList[i] as JunQiItem;
							GameManager.ClientMgr.NotifyMySelfNewJunQi(sl, pool, client, junQiItem);
						}
					}
				}
			}
		}

		public static void DelMySelfJunQiItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is JunQiItem)
					{
						JunQiItem junQiItem = objsList[i] as JunQiItem;
						GameManager.ClientMgr.NotifyMySelfDelJunQi(sl, pool, client, junQiItem.JunQiID);
					}
				}
			}
		}

		public static void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is JunQiItem)
					{
						if (client == null || Global.IsOpposition(client, list[i] as JunQiItem))
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as JunQiItem).CopyMapID)
							{
								Point target = new Point((double)(list[i] as JunQiItem).PosX, (double)(list[i] as JunQiItem).PosY);
								if (Global.InCircle(target, center, (double)radius))
								{
									enemiesList.Add(list[i]);
								}
							}
						}
					}
				}
			}
		}

		public static void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<JunQiItem> enemiesList, double angle)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is JunQiItem)
					{
						if (client == null || Global.IsOpposition(client, list[i] as JunQiItem))
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as JunQiItem).CopyMapID)
							{
								Point target = new Point((double)(list[i] as JunQiItem).PosX, (double)(list[i] as JunQiItem).PosY);
								if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
								{
									enemiesList.Add(list[i] as JunQiItem);
								}
							}
						}
					}
				}
			}
		}

		public static void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			int currentMapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
			List<object> list = mapGrid.FindObjects(gridX, gridY);
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is JunQiItem)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (list[i] as JunQiItem).CopyMapID)
						{
							enemiesList.Add(list[i]);
						}
					}
				}
			}
		}

		public static void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			int currentMapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
			Point currentGrid = attacker.CurrentGrid;
			int gridX = (int)currentGrid.X;
			int gridY = (int)currentGrid.Y;
			Point gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY);
			JunQiManager.LookupEnemiesAtGridXY(attacker, (int)gridPointByDirection.X, (int)gridPointByDirection.Y, enemiesList);
		}

		public static void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> list = new List<object>();
			JunQiManager.LookupAttackEnemies(attacker, direction, list);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as JunQiItem).JunQiID);
			}
		}

		public static void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridPointByDirection.Count > 0)
			{
				for (int i = 0; i < gridPointByDirection.Count; i++)
				{
					JunQiManager.LookupEnemiesAtGridXY(obj, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y, enemiesList);
				}
			}
		}

		public static bool CanAttack(JunQiItem enemy)
		{
			bool result;
			if (null == enemy)
			{
				result = false;
			}
			else if (JunQiManager.JugeLingDiZhanEndByMapCode(enemy.MapCode))
			{
				result = false;
			}
			else
			{
				int lingDiIDBy2MapCode = JunQiManager.GetLingDiIDBy2MapCode(enemy.MapCode);
				result = (lingDiIDBy2MapCode == 3 && LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState);
			}
			return result;
		}

		public static int Monster_NotifyInjured(SocketListener sl, TCPOutPacketPool pool, Monster monster, JunQiItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int result = 0;
			if ((enemy as JunQiItem).CurrentLifeV > 0 && null != monster.OwnerClient)
			{
				GameClient ownerClient = monster.OwnerClient;
				injure = (enemy as JunQiItem).CutLifeV;
				injure = (int)((double)injure * baseRate + (double)addVlue);
				injure = (int)((double)injure * injurePercent);
				result = injure;
				(enemy as JunQiItem).CurrentLifeV -= injure;
				(enemy as JunQiItem).CurrentLifeV = Global.GMax((enemy as JunQiItem).CurrentLifeV, 0);
				int currentLifeV = (enemy as JunQiItem).CurrentLifeV;
				(enemy as JunQiItem).AttackedRoleID = ownerClient.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, ownerClient, injure);
				(enemy as JunQiItem).AddAttacker(ownerClient.ClientData.RoleID, Global.GMax(0, injure), monster);
				GameManager.SystemServerEvents.AddEvent(string.Format("帮旗减血, Injure={0}, Life={1}", injure, currentLifeV), EventLevels.Debug);
				if ((enemy as JunQiItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("帮旗死亡, roleID={0}", (enemy as JunQiItem).JunQiID), EventLevels.Debug);
					JunQiManager.ProcessJunQiDead(sl, pool, ownerClient, enemy as JunQiItem);
				}
				if ((enemy as JunQiItem).AttackedRoleID >= 0 && (enemy as JunQiItem).AttackedRoleID != ownerClient.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient((enemy as JunQiItem).AttackedRoleID);
					if (null != gameClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (enemy as JunQiItem).JunQiID, 0, 0, (double)currentLifeV, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, enemy.JunQiID, 0, 0, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, ownerClient, ownerClient.ClientData.MapCode, ownerClient.ClientData.RoleID, (enemy as JunQiItem).JunQiID, burst, injure, (double)currentLifeV, ownerClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.RoleID, (enemy as JunQiItem).JunQiID, burst, injure, (double)currentLifeV, monster.MonsterInfo.VLevel, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, ownerClient, ownerClient.ClientData.RoleID, enemy.JunQiID, burst, injure, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!ownerClient.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, ownerClient);
				}
			}
			return result;
		}

		public static int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, JunQiItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int result = 0;
			if ((enemy as JunQiItem).CurrentLifeV > 0)
			{
				injure = (enemy as JunQiItem).CutLifeV;
				injure = (int)((double)injure * baseRate + (double)addVlue);
				injure = (int)((double)injure * injurePercent);
				result = injure;
				(enemy as JunQiItem).CurrentLifeV -= injure;
				(enemy as JunQiItem).CurrentLifeV = Global.GMax((enemy as JunQiItem).CurrentLifeV, 0);
				int currentLifeV = (enemy as JunQiItem).CurrentLifeV;
				(enemy as JunQiItem).AttackedRoleID = client.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				(enemy as JunQiItem).AddAttacker(client.ClientData.RoleID, Global.GMax(0, injure), null);
				client.ClientData.RoleIDAttackebByMyself = enemy.GetObjectID();
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(client, enemy);
				GameManager.SystemServerEvents.AddEvent(string.Format("帮旗减血, Injure={0}, Life={1}", injure, currentLifeV), EventLevels.Debug);
				if ((enemy as JunQiItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("帮旗死亡, roleID={0}", (enemy as JunQiItem).JunQiID), EventLevels.Debug);
					JunQiManager.ProcessJunQiDead(sl, pool, client, enemy as JunQiItem);
				}
				if ((enemy as JunQiItem).AttackedRoleID >= 0 && (enemy as JunQiItem).AttackedRoleID != client.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient((enemy as JunQiItem).AttackedRoleID);
					if (null != gameClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (enemy as JunQiItem).JunQiID, 0, 0, (double)currentLifeV, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, enemy.JunQiID, 0, 0, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as JunQiItem).JunQiID, burst, injure, (double)currentLifeV, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.JunQiID, burst, injure, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!client.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
				}
			}
			return result;
		}

		public static void NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int enemyX, int enemyY, int burst, int injure, double attackPercent, int addAttack, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			object obj = JunQiManager.FindJunQiByID(enemy);
			if (null != obj)
			{
				if ((obj as JunQiItem).CurrentLifeV > 0)
				{
					injure = (obj as JunQiItem).CutLifeV;
					(obj as JunQiItem).CurrentLifeV -= injure;
					(obj as JunQiItem).CurrentLifeV = Global.GMax((obj as JunQiItem).CurrentLifeV, 0);
					int currentLifeV = (obj as JunQiItem).CurrentLifeV;
					(obj as JunQiItem).AttackedRoleID = client.ClientData.RoleID;
					GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
					GameManager.SystemServerEvents.AddEvent(string.Format("帮旗减血, Injure={0}, Life={1}", injure, currentLifeV), EventLevels.Debug);
					if ((obj as JunQiItem).CurrentLifeV <= 0)
					{
						GameManager.SystemServerEvents.AddEvent(string.Format("帮旗死亡, roleID={0}", (obj as JunQiItem).JunQiID), EventLevels.Debug);
						JunQiManager.ProcessJunQiDead(sl, pool, client, obj as JunQiItem);
					}
					int attackerFromList = (obj as JunQiItem).GetAttackerFromList();
					if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
						if (null != gameClient)
						{
							GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (obj as JunQiItem).JunQiID, 0, 0, (double)currentLifeV, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
							ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, (obj as JunQiItem).JunQiID, 0, 0, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (obj as JunQiItem).JunQiID, burst, injure, (double)currentLifeV, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
					ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, (obj as JunQiItem).JunQiID, burst, injure, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					if (!client.ClientData.DisableChangeRolePurpleName)
					{
						GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
					}
				}
			}
		}

		private static void ProcessJunQiDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, JunQiItem junQiItem)
		{
			if (!junQiItem.HandledDead)
			{
				junQiItem.HandledDead = true;
				junQiItem.JunQiDeadTicks = TimeUtil.NOW();
				int attackerFromList = junQiItem.GetAttackerFromList();
				if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
					if (null != gameClient)
					{
						client = gameClient;
					}
				}
				JunQiManager.AddKillJunQiItem(client.ClientData.MapCode, junQiItem.QiZuoNPC, client.ClientData.Faction);
				if (junQiItem.ManagerType == 24)
				{
					LuoLanChengZhanManager.getInstance().OnProcessJunQiDead(junQiItem.QiZuoNPC, junQiItem.BHID);
				}
				if (client.ClientData.Faction > 0)
				{
					Global.BroadcastBangHuiMsg(-1, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(394, new object[0]), new object[]
					{
						Global.FormatRoleName(client, client.ClientData.RoleName),
						Global.GetServerLineName2(),
						Global.GetMapName(client.ClientData.MapCode),
						junQiItem.BHName
					}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
				}
				Global.BroadcastBangHuiMsg(-1, junQiItem.BHID, StringUtil.substitute(GLang.GetLang(395, new object[0]), new object[]
				{
					Global.GetServerLineName2(),
					Global.GetMapName(client.ClientData.MapCode),
					string.IsNullOrEmpty(client.ClientData.BHName) ? "" : StringUtil.substitute(GLang.GetLang(396, new object[0]), new object[]
					{
						client.ClientData.BHName
					}),
					Global.FormatRoleName(client, client.ClientData.RoleName)
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
			}
		}

		public static void ParseWeekDaysTimes()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("LingDiZhanWeekDays");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					','
				});
				int[] array2 = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = Global.SafeConvertToInt32(array[i]);
				}
				JunQiManager.LingDiZhanWeekDays = array2;
			}
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("LingDiIDs2MapCodes");
			if (!string.IsNullOrEmpty(paramValueByName2))
			{
				string[] array3 = paramValueByName2.Split(new char[]
				{
					','
				});
				int[] array4 = new int[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					array4[i] = Global.SafeConvertToInt32(array3[i]);
				}
				JunQiManager.LingDiIDs2MapCodes = array4;
			}
			string paramValueByName3 = GameManager.systemParamsList.GetParamValueByName("LingDiZhanFightingDayTimes");
			JunQiManager.LingDiZhanFightingDayTimes = Global.ParseDateTimeRangeStr(paramValueByName3);
			string paramValueByName4 = GameManager.systemParamsList.GetParamValueByName("LingDiZhanEndDayTimes");
			JunQiManager.LingDiZhanEndDayTimes = Global.ParseDateTimeRangeStr(paramValueByName4);
		}

		private static bool IsDayOfWeek(int weekDayID)
		{
			bool result;
			if (null == JunQiManager.LingDiZhanWeekDays)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < JunQiManager.LingDiZhanWeekDays.Length; i++)
				{
					if (JunQiManager.LingDiZhanWeekDays[i] == weekDayID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public static bool IsInLingDiZhanFightingTime()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int dayOfWeek = (int)dateTime.DayOfWeek;
			bool result;
			if (!JunQiManager.IsDayOfWeek(dayOfWeek))
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, JunQiManager.LingDiZhanFightingDayTimes, out num, false);
			}
			return result;
		}

		private static bool IsInLingDiZhanEndTime()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int dayOfWeek = (int)dateTime.DayOfWeek;
			bool result;
			if (!JunQiManager.IsDayOfWeek(dayOfWeek))
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, JunQiManager.LingDiZhanEndDayTimes, out num, false);
			}
			return result;
		}

		public static int GetLingDiIDBy2MapCode(int mapCode)
		{
			int result;
			if (null == JunQiManager.LingDiIDs2MapCodes)
			{
				result = 0;
			}
			else
			{
				for (int i = 0; i < JunQiManager.LingDiIDs2MapCodes.Length; i++)
				{
					if (JunQiManager.LingDiIDs2MapCodes[i] == mapCode)
					{
						return i + 1;
					}
				}
				if (Global.GetWangChengMapCode() == mapCode)
				{
					result = 2;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public static int GetMapCodeByLingDiID(int lingDiID)
		{
			int result;
			if (null == JunQiManager.LingDiIDs2MapCodes)
			{
				result = 0;
			}
			else
			{
				result = JunQiManager.LingDiIDs2MapCodes[lingDiID];
			}
			return result;
		}

		public static bool CanInstallJunQi(GameClient client)
		{
			return !JunQiManager.JugeLingDiZhanEndByMapCode(client.ClientData.MapCode) && LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState;
		}

		private static bool JugeLingDiZhanEndByMapCode(int mapCode)
		{
			bool result = false;
			lock (JunQiManager.LingDiZhanResultsDict)
			{
				if (!JunQiManager.LingDiZhanResultsDict.TryGetValue(mapCode, out result))
				{
					return false;
				}
			}
			return result;
		}

		private static void AddLingDiZhanEndResultByMapCode(int mapCode, bool result)
		{
			lock (JunQiManager.LingDiZhanResultsDict)
			{
				JunQiManager.LingDiZhanResultsDict[mapCode] = result;
			}
		}

		public static void ProcessLingDiZhanResult()
		{
			if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
			{
				if (JunQiManager.LingDiIDs2MapCodes != null && JunQiManager.LingDiIDs2MapCodes.Length == 7)
				{
					if (LingDiZhanStates.None == JunQiManager.LingDiZhanState)
					{
						if (JunQiManager.IsInLingDiZhanFightingTime())
						{
							JunQiManager.LingDiZhanResultsDict.Clear();
							JunQiManager.LingDiZhanState = LingDiZhanStates.Fighting;
							for (int i = 3; i <= 3; i++)
							{
								JunQiManager.NotifyAllLingDiMapInfoData(JunQiManager.LingDiIDs2MapCodes[i - 1]);
							}
						}
					}
					else if (JunQiManager.IsInLingDiZhanFightingTime())
					{
						if (JunQiManager.IsInLingDiZhanEndTime())
						{
							for (int i = 3; i <= 3; i++)
							{
								if (!JunQiManager.JugeLingDiZhanEndByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1]))
								{
									int bhid = 0;
									int num = JunQiManager.CalcJunQiNumByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], out bhid);
									if (num >= JunQiManager.MaxInstallQiNum)
									{
										JunQiManager.AddLingDiZhanEndResultByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], true);
										JunQiManager.HandleLingDiZhanResultByMapCode(i, JunQiManager.LingDiIDs2MapCodes[i - 1], bhid, true, true);
										JunQiManager.ProcessHuangChengFightingEndAwards();
									}
								}
							}
						}
						else
						{
							JunQiManager.ProcessTimeAddRoleExp();
						}
					}
					else
					{
						JunQiManager.LingDiZhanState = LingDiZhanStates.None;
						for (int i = 3; i <= 3; i++)
						{
							if (!JunQiManager.JugeLingDiZhanEndByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1]))
							{
								int bhid = 0;
								int num = JunQiManager.CalcJunQiNumByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], out bhid);
								JunQiManager.AddLingDiZhanEndResultByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], true);
								JunQiManager.HandleLingDiZhanResultByMapCode(i, JunQiManager.LingDiIDs2MapCodes[i - 1], bhid, true, true);
							}
						}
						JunQiManager.ProcessHuangChengFightingEndAwards();
					}
				}
			}
		}

		public static void NotifySyncBangHuiLingDiZhanResult(int lingDiID, int mapCode, int bhid, int zoneID, string bhName)
		{
			string text = string.Format("-syncldzresult {0} {1} {2} {3} {4}", new object[]
			{
				lingDiID,
				mapCode,
				bhid,
				zoneID,
				bhName
			});
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				text,
				0,
				0,
				GameManager.ServerLineID
			}), null, 0);
		}

		public static void HandleLingDiZhanResultByMapCode(int lingDiID, int mapCode, int bhid, bool sendToOtherLine, bool lingDiOkHint = true)
		{
			JunQiItem junQiItem = null;
			if (bhid > 0)
			{
				junQiItem = JunQiManager.FindJunQiByBHID(bhid);
			}
			if (sendToOtherLine)
			{
				Global.UpdateLingDiForBH(lingDiID, bhid);
				JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapCode);
				if (null != junQiItem)
				{
					JunQiManager.ProcessAllNewJunQiByMapCode(mapCode, bhid, junQiItem.ZoneID, junQiItem.BHName);
				}
			}
			if (null != junQiItem)
			{
				BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(lingDiID);
				if (null != itemByLingDiID)
				{
					JunQiManager.InstallJunQiOnNormalMap(lingDiID, bhid, junQiItem.ZoneID, junQiItem.BHName, true);
				}
			}
			else
			{
				JunQiManager.ClearJunQiOnNormalMap(lingDiID);
			}
			if (sendToOtherLine)
			{
				if (null != junQiItem)
				{
					JunQiManager.NotifySyncBangHuiLingDiZhanResult(lingDiID, mapCode, bhid, junQiItem.ZoneID, junQiItem.BHName);
				}
			}
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
			if (lingDiOkHint)
			{
				if (null != junQiItem)
				{
					Global.BroadcastLingDiOkHint(junQiItem.BHName, mapCode);
				}
			}
		}

		public static void HandleLuoLanChengZhanResult(int lingDiID, int mapCode, int bhid, string bhName, bool sendToOtherLine, bool lingDiOkHint = true)
		{
			Global.UpdateLingDiForBH(lingDiID, bhid);
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
			if (lingDiOkHint)
			{
				Global.BroadcastLingDiOkHint(bhName, mapCode);
			}
		}

		public static void HandleLingDiZhanResultByMapCode2(int lingDiID, int mapCode, int bhid, int zoneID, string bhName)
		{
			if (bhid > 0)
			{
				BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(lingDiID);
				if (null != itemByLingDiID)
				{
					JunQiManager.InstallJunQiOnNormalMap(lingDiID, bhid, zoneID, bhName, true);
				}
			}
			else
			{
				JunQiManager.ClearJunQiOnNormalMap(lingDiID);
			}
			JunQiManager.NotifyAllLingDiMapInfoData(mapCode);
		}

		public static void InitLingDiJunQi()
		{
			if (JunQiManager.LingDiIDs2MapCodes != null && JunQiManager.LingDiIDs2MapCodes.Length == 7)
			{
				if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
				{
					for (int i = 3; i <= 3; i++)
					{
						BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(i);
						if (null != itemByLingDiID)
						{
							if (itemByLingDiID.BHID > 0)
							{
								JunQiManager.ProcessAllNewJunQiByMapCode(JunQiManager.LingDiIDs2MapCodes[i - 1], itemByLingDiID.BHID, itemByLingDiID.ZoneID, itemByLingDiID.BHName);
							}
						}
					}
				}
			}
		}

		public static void InstallJunQiOnNormalMap(int lingDiID, int bhid, int zoneID, string bhName, bool forceClean = true)
		{
			List<int> list = new List<int>();
			foreach (int num in GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict[num];
				if (lingDiID == systemXmlItem.GetIntValue("LingDiID", -1))
				{
					list.Add(num);
				}
			}
			string junQiNameByBHID = JunQiManager.GetJunQiNameByBHID(bhid);
			int junQiLevelByBHID = JunQiManager.GetJunQiLevelByBHID(bhid);
			for (int i = 0; i < list.Count; i++)
			{
				if (forceClean)
				{
					JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, list[i]);
				}
				JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, list[i], bhid, zoneID, bhName, -1, junQiNameByBHID, junQiLevelByBHID, 0);
			}
		}

		public static void ClearJunQiOnNormalMap(int lingDiID)
		{
			List<int> list = new List<int>();
			foreach (int num in GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = GameManager.systemLingQiMapQiZhiMgr.SystemXmlItemDict[num];
				if (lingDiID == systemXmlItem.GetIntValue("LingDiID", -1))
				{
					list.Add(num);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, list[i]);
			}
		}

		public static void SendClearJunQiCmd(int bhid)
		{
			if (JunQiManager.LingDiIDs2MapCodes != null && JunQiManager.LingDiIDs2MapCodes.Length == 7)
			{
				string text = string.Format("-clearmap {0}", bhid);
				GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					-1,
					"",
					0,
					"",
					0,
					text,
					0,
					0,
					-1
				}), null, 0);
			}
		}

		public static LingDiMapInfoData GetLingDiMapData(GameClient client)
		{
			int lingDiIDBy2MapCode = JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode);
			LingDiMapInfoData result;
			if (lingDiIDBy2MapCode != 3)
			{
				result = null;
			}
			else
			{
				result = JunQiManager.FormatLingDiMapData(client.ClientData.MapCode);
			}
			return result;
		}

		private static LingDiMapInfoData FormatLingDiMapData(int mapCode)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long fightingEndTime = 0L;
			long fightingStartTime = 0L;
			if (JunQiManager.LingDiZhanFightingDayTimes != null && JunQiManager.LingDiZhanFightingDayTimes.Length > 0)
			{
				if (!JunQiManager.JugeLingDiZhanEndByMapCode(mapCode))
				{
					if (LingDiZhanStates.Fighting == JunQiManager.LingDiZhanState)
					{
						DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, JunQiManager.LingDiZhanFightingDayTimes[0].EndHour, JunQiManager.LingDiZhanFightingDayTimes[0].EndMinute, 0);
						fightingEndTime = dateTime2.Ticks / 10000L;
						DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, JunQiManager.LingDiZhanFightingDayTimes[0].FromHour, JunQiManager.LingDiZhanFightingDayTimes[0].FromMinute, 0);
						fightingStartTime = dateTime3.Ticks / 10000L;
					}
				}
			}
			LingDiMapInfoData lingDiMapInfoData = new LingDiMapInfoData
			{
				FightingEndTime = fightingEndTime,
				FightingStartTime = fightingStartTime,
				BHNameDict = new Dictionary<int, string>()
			};
			List<int> qiZuoNPCIDList = JunQiManager.GetQiZuoNPCIDList(mapCode);
			if (null != qiZuoNPCIDList)
			{
				for (int i = 0; i < qiZuoNPCIDList.Count; i++)
				{
					lingDiMapInfoData.BHNameDict[qiZuoNPCIDList[i]] = JunQiManager.GetBHNameByNPCID(qiZuoNPCIDList[i]);
				}
			}
			return lingDiMapInfoData;
		}

		public static void NotifyAllLingDiMapInfoData(int mapCode)
		{
			LingDiMapInfoData lingDiMapInfoData = JunQiManager.FormatLingDiMapData(mapCode);
			GameManager.ClientMgr.NotifyAllLingDiMapInfoData(mapCode, lingDiMapInfoData);
		}

		private static void ProcessTimeAddRoleExp()
		{
			long num = TimeUtil.NOW();
			if (num - JunQiManager.LastAddBangZhanAwardsTicks >= 10000L)
			{
				JunQiManager.LastAddBangZhanAwardsTicks = num;
				List<object> mapClients = GameManager.ClientMgr.GetMapClients(JunQiManager.LingDiIDs2MapCodes[2]);
				if (null != mapClients)
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						GameClient gameClient = mapClients[i] as GameClient;
						if (gameClient != null)
						{
							BangZhanAwardsMgr.ProcessBangZhanAwards(gameClient);
						}
					}
				}
			}
		}

		private static int GetExperienceAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 5000000;
			}
			else
			{
				result = 2500000;
			}
			return result;
		}

		private static int GetRongYuAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 5000;
			}
			else
			{
				result = 2500;
			}
			return result;
		}

		private static void ProcessRoleExperienceAwards(GameClient client, bool success)
		{
			int experienceAwards = JunQiManager.GetExperienceAwards(client, success);
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)experienceAwards, true, false, false, "none");
		}

		private static void ProcessRoleBangGongAwards(GameClient client, bool success)
		{
			int rongYuAwards = JunQiManager.GetRongYuAwards(client, success);
			if (rongYuAwards > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYuAwards, true, true);
				GameManager.SystemServerEvents.AddEvent(string.Format("角色获取荣誉, roleID={0}({1}), BangGong={2}, newBangGong={3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					client.ClientData.BangGong,
					rongYuAwards
				}), EventLevels.Record);
			}
		}

		private static bool CanGetAWards(GameClient client, long nowTicks)
		{
			return nowTicks - client.ClientData.EnterMapTicks >= (long)JunQiManager.MaxHavingAwardsSecs;
		}

		private static void ProcessHuangChengFightingEndAwards()
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(JunQiManager.LingDiIDs2MapCodes[2]);
			if (null != mapClients)
			{
				int num = -1;
				int num2 = 3;
				if (num2 > 0)
				{
					BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(num2);
					if (itemByLingDiID != null && itemByLingDiID.BHID > 0)
					{
						num = itemByLingDiID.BHID;
					}
				}
				long nowTicks = TimeUtil.NOW();
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.CurrentLifeV > 0)
						{
							if (JunQiManager.CanGetAWards(gameClient, nowTicks))
							{
								JunQiManager.ProcessRoleExperienceAwards(gameClient, num == gameClient.ClientData.Faction);
								JunQiManager.ProcessRoleBangGongAwards(gameClient, num == gameClient.ClientData.Faction);
							}
						}
					}
				}
			}
		}

		public static object JunQiMutex = new object();

		private static Dictionary<string, KillJunQiItem> KillJunQiDict = new Dictionary<string, KillJunQiItem>();

		private static Dictionary<int, BangHuiJunQiItemData> _BangHuiJunQiItemsDict = null;

		private static Dictionary<int, BangHuiLingDiItemData> _BangHuiLingDiItemsDict = null;

		private static Dictionary<int, JunQiItem> _NPCID2JunQiDict = new Dictionary<int, JunQiItem>();

		private static Dictionary<int, JunQiItem> _ID2JunQiDict = new Dictionary<int, JunQiItem>();

		private static int MaxInstallQiNum = 3;

		private static int[] LingDiZhanWeekDays = null;

		private static DateTimeRange[] LingDiZhanFightingDayTimes = null;

		private static DateTimeRange[] LingDiZhanEndDayTimes = null;

		private static int[] LingDiIDs2MapCodes = null;

		public static LingDiZhanStates LingDiZhanState = LingDiZhanStates.None;

		private static Dictionary<int, bool> LingDiZhanResultsDict = new Dictionary<int, bool>();

		private static long LastAddBangZhanAwardsTicks = 0L;

		private static int MaxHavingAwardsSecs = 1200000;
	}
}
