using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class NPCGeneralManager
	{
		public static bool ReloadMapNPCRoles(int mapCode)
		{
			string uri = string.Format("Map/{0}/npcs.xml", mapCode);
			GeneralCachingXmlMgr.Reload(Global.ResPath(uri));
			GameManager.SystemNPCsMgr.ReloadLoadFromXMlFile();
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			return NPCGeneralManager.LoadMapNPCRoles(mapCode, gameMap);
		}

		public static bool LoadMapNPCRoles(int mapCode, GameMap gameMap)
		{
			string uri = string.Format("Map/{0}/npcs.xml", mapCode);
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.ResPath(uri));
			bool result;
			if (null == xelement)
			{
				result = false;
			}
			else
			{
				IEnumerable<XElement> enumerable = xelement.Elements("NPCs").Elements<XElement>();
				foreach (XElement xelement2 in enumerable)
				{
					NPC npc = new NPC();
					npc.NpcID = Convert.ToInt32((string)xelement2.Attribute("Code"));
					npc.MapCode = mapCode;
					npc.CurrentPos = new Point((double)Convert.ToInt32((string)xelement2.Attribute("X")), (double)Convert.ToInt32((string)xelement2.Attribute("Y")));
					if (xelement2.Attribute("Dir") != null)
					{
						npc.CurrentDir = (Dircetions)Global.GetSafeAttributeLong(xelement2, "Dir");
					}
					else
					{
						npc.CurrentDir = Dircetions.DR_DOWN;
					}
					npc.RoleBufferData = NPCGeneralManager.GenerateNpcRoleBufferData(npc);
					if (null != npc.RoleBufferData)
					{
						NPCGeneralManager.AddNpcToMap(npc);
						int num = 2;
						SystemXmlItem systemXmlItem;
						if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(npc.NpcID, out systemXmlItem))
						{
							num = systemXmlItem.GetIntValue("IsSafe", -1);
						}
						if (num > 0)
						{
							gameMap.SetPartialSafeRegion(npc.GridPoint, num);
						}
					}
				}
				result = true;
			}
			return result;
		}

		public static NPC GetNPCFromConfig(int mapCode, int npcID, int toX, int toY, int dir)
		{
			SystemXmlItem systemXmlItem = null;
			NPC result;
			if (!GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(npcID, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
				{
					result = null;
				}
				else
				{
					NPC npc = new NPC();
					npc.NpcID = npcID;
					npc.MapCode = mapCode;
					npc.CurrentPos = new Point((double)toX, (double)toY);
					npc.CurrentDir = (Dircetions)dir;
					npc.RoleBufferData = NPCGeneralManager.GenerateNpcRoleBufferData(npc);
					if (null == npc.RoleBufferData)
					{
						result = null;
					}
					else
					{
						result = npc;
					}
				}
			}
			return result;
		}

		public static bool AddNpcToMap(NPC myNpc)
		{
			MapGrid mapGrid;
			lock (GameManager.MapGridMgr.DictGrids)
			{
				mapGrid = GameManager.MapGridMgr.GetMapGrid(myNpc.MapCode);
			}
			bool result;
			if (null == mapGrid)
			{
				result = false;
			}
			else
			{
				lock (NPCGeneralManager.mutexAddNPC)
				{
					string key = string.Format("{0}_{1}_{2}", myNpc.MapCode, myNpc.GridPoint.X, myNpc.GridPoint.Y);
					NPC obj2 = null;
					if (NPCGeneralManager.ListNpc.TryGetValue(key, out obj2))
					{
						NPCGeneralManager.ListNpc.Remove(key);
						mapGrid.RemoveObject(obj2);
						LogManager.WriteLog(2, string.Format("地图{0}的({1}, {2})处旧的NPC被替换", myNpc.MapCode, myNpc.GridPoint.X, myNpc.GridPoint.Y), null, true);
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[myNpc.MapCode];
					if (mapGrid.MoveObject(-1, -1, (int)((double)gameMap.MapGridWidth * myNpc.GridPoint.X), (int)((double)gameMap.MapGridHeight * myNpc.GridPoint.Y), myNpc))
					{
						NPCGeneralManager.ListNpc.Add(key, myNpc);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public static void RemoveMapNpcs(int mapCode)
		{
			if (mapCode > 0)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
				if (null != mapGrid)
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, NPC> keyValuePair in NPCGeneralManager.ListNpc)
					{
						if (keyValuePair.Value.MapCode == mapCode)
						{
							mapGrid.RemoveObject(keyValuePair.Value);
							list.Add(keyValuePair.Key);
						}
					}
					foreach (string key in list)
					{
						NPCGeneralManager.ListNpc.Remove(key);
					}
				}
			}
		}

		public static void RemoveMapNpc(int mapCode, int npcID)
		{
			if (mapCode > 0)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
				if (null != mapGrid)
				{
					foreach (KeyValuePair<string, NPC> keyValuePair in NPCGeneralManager.ListNpc)
					{
						if (keyValuePair.Value.MapCode == mapCode && keyValuePair.Value.NpcID == npcID)
						{
							mapGrid.RemoveObject(keyValuePair.Value);
							NPCGeneralManager.ListNpc.Remove(keyValuePair.Key);
							break;
						}
					}
				}
			}
		}

		public static NPC FindNPC(int mapCode, int npcID)
		{
			foreach (KeyValuePair<string, NPC> keyValuePair in NPCGeneralManager.ListNpc)
			{
				if (keyValuePair.Value.MapCode == mapCode && keyValuePair.Value.NpcID == npcID)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public static byte[] GenerateNpcRoleBufferData(NPC myNpc)
		{
			SystemXmlItem systemXmlItem = null;
			byte[] result;
			if (!GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(myNpc.NpcID, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = DataHelper.ObjectToBytes<NPCRole>(new NPCRole
				{
					NpcID = myNpc.NpcID,
					PosX = (int)myNpc.CurrentPos.X,
					PosY = (int)myNpc.CurrentPos.Y,
					MapCode = myNpc.MapCode,
					Dir = (int)myNpc.CurrentDir,
					RoleString = systemXmlItem.XMLNode.ToString()
				});
			}
			return result;
		}

		public static void SendMySelfNPCs(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					NPC npc = objsList[i] as NPC;
					if (null != npc)
					{
						if (npc.ShowNpc)
						{
							GameManager.ClientMgr.NotifyMySelfNewNPC(sl, pool, client, npc);
						}
					}
				}
			}
		}

		public static void DelMySelfNpcs(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					NPC npc = objsList[i] as NPC;
					if (null != npc)
					{
						GameManager.ClientMgr.NotifyMySelfDelNPC(sl, pool, client, npc);
					}
				}
			}
		}

		public static Dictionary<string, NPC> ListNpc = new Dictionary<string, NPC>();

		public static object mutexAddNPC = new object();
	}
}
