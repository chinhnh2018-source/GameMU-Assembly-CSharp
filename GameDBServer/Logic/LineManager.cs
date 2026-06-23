using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class LineManager
	{
		public static void LoadConfig()
		{
			bool flag = false;
			Dictionary<int, LineItem> dictionary = new Dictionary<int, LineItem>();
			try
			{
				XElement xelement = XElement.Load("GameServer.xml");
				IEnumerable<XElement> enumerable = xelement.Element("GameServer").Elements();
				foreach (XElement xml in enumerable)
				{
					LineItem lineItem = new LineItem
					{
						LineID = (int)Global.GetSafeAttributeLong(xml, "ID"),
						GameServerIP = Global.GetSafeAttributeStr(xml, "ip"),
						GameServerPort = (int)Global.GetSafeAttributeLong(xml, "port"),
						OnlineCount = 0
					};
					dictionary[lineItem.LineID] = lineItem;
					flag = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				flag = false;
			}
			if (flag)
			{
				lock (LineManager.Mutex)
				{
					foreach (LineItem lineItem2 in dictionary.Values)
					{
						if (!LineManager._LinesDict.ContainsKey(lineItem2.LineID))
						{
							LineManager._LinesDict.Add(lineItem2.LineID, lineItem2);
						}
					}
					LineManager._LinesList = LineManager._LinesDict.Values.ToList<LineItem>();
				}
			}
		}

		public static void UpdateLineHeart(GameServerClient client, int lineID, int onlineNum, string strMapOnlineNum = "")
		{
			lock (LineManager.Mutex)
			{
				LineItem lineItem = null;
				if (LineManager._LinesDict.TryGetValue(lineID, out lineItem))
				{
					lineItem.OnlineCount = onlineNum;
					lineItem.OnlineTicks = DateTime.Now.Ticks / 10000L;
					lineItem.MapOnlineNum = strMapOnlineNum;
					lineItem.ServerClient = client;
					client.LineId = lineID;
				}
				else if (lineID >= 9000)
				{
					lineItem = new LineItem();
					LineManager._LinesDict[lineID] = lineItem;
					lineItem.LineID = lineID;
					lineItem.OnlineCount = onlineNum;
					lineItem.OnlineTicks = DateTime.Now.Ticks / 10000L;
					lineItem.MapOnlineNum = strMapOnlineNum;
					lineItem.ServerClient = client;
					client.LineId = lineID;
					if (!LineManager._LinesList.Contains(lineItem))
					{
						LineManager._LinesList.Add(lineItem);
					}
				}
			}
		}

		public static GameServerClient GetGameServerClient(int lineId)
		{
			lock (LineManager.Mutex)
			{
				LineItem lineItem = null;
				if (LineManager._LinesDict.TryGetValue(lineId, out lineItem))
				{
					return lineItem.ServerClient;
				}
			}
			return null;
		}

		public static int GetLineHeartState(int lineID)
		{
			long num = DateTime.Now.Ticks / 10000L;
			int result = 0;
			lock (LineManager.Mutex)
			{
				LineItem lineItem = null;
				if (LineManager._LinesDict.TryGetValue(lineID, out lineItem))
				{
					if (num - lineItem.OnlineTicks < 60000L)
					{
						result = 1;
					}
				}
			}
			return result;
		}

		public static List<LineItem> GetLineItemList()
		{
			List<LineItem> list = new List<LineItem>();
			long num = DateTime.Now.Ticks / 10000L;
			lock (LineManager.Mutex)
			{
				for (int i = 0; i < LineManager._LinesList.Count; i++)
				{
					if (num - LineManager._LinesList[i].OnlineTicks < 180000L)
					{
						list.Add(LineManager._LinesList[i]);
					}
				}
			}
			return list;
		}

		public static int GetTotalOnlineNum()
		{
			int num = 0;
			long num2 = DateTime.Now.Ticks / 10000L;
			lock (LineManager.Mutex)
			{
				for (int i = 0; i < LineManager._LinesList.Count; i++)
				{
					if (num2 - LineManager._LinesList[i].OnlineTicks < 60000L)
					{
						num += LineManager._LinesList[i].OnlineCount;
					}
				}
			}
			return num;
		}

		public static string GetMapOnlineNum()
		{
			string result = "";
			long num = DateTime.Now.Ticks / 10000L;
			lock (LineManager.Mutex)
			{
				for (int i = 0; i < LineManager._LinesList.Count; i++)
				{
					if (num - LineManager._LinesList[i].OnlineTicks < 60000L)
					{
						return LineManager._LinesList[i].MapOnlineNum;
					}
				}
			}
			return result;
		}

		private static object Mutex = new object();

		private static Dictionary<int, LineItem> _LinesDict = new Dictionary<int, LineItem>();

		private static List<LineItem> _LinesList = new List<LineItem>();
	}
}
