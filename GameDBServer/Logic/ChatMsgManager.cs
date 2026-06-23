using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameServer.Core.AssemblyPatch;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class ChatMsgManager
	{
		private static Queue<string> GetChatMsgQueue(int serverLineID)
		{
			Queue<string> queue = null;
			lock (ChatMsgManager.ChatMsgDict)
			{
				if (!ChatMsgManager.ChatMsgDict.TryGetValue(serverLineID, out queue))
				{
					queue = new Queue<string>();
					ChatMsgManager.ChatMsgDict[serverLineID] = queue;
				}
			}
			return queue;
		}

		public static void AddGMCmdChatMsg(int serverLineID, string gmCmd)
		{
			string chatMsg = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				gmCmd,
				0,
				0,
				serverLineID
			});
			List<LineItem> lineItemList = LineManager.GetLineItemList();
			if (null != lineItemList)
			{
				for (int i = 0; i < lineItemList.Count; i++)
				{
					if (lineItemList[i].LineID != serverLineID)
					{
						if (lineItemList[i].LineID < 9000 || lineItemList[i].LineID == GameDBManager.ZoneID)
						{
							ChatMsgManager.AddChatMsg(lineItemList[i].LineID, chatMsg);
						}
					}
				}
			}
		}

		public static void AddGMCmdChatMsgToOneClient(string gmCmd)
		{
			string chatMsg = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				gmCmd,
				0,
				0,
				-1
			});
			List<LineItem> lineItemList = LineManager.GetLineItemList();
			if (null != lineItemList)
			{
				for (int i = 0; i < lineItemList.Count; i++)
				{
					if (lineItemList[i].LineID < 9000 || lineItemList[i].LineID == GameDBManager.ZoneID)
					{
						ChatMsgManager.AddChatMsg(lineItemList[i].LineID, chatMsg);
						break;
					}
				}
			}
		}

		public static void AddChatMsg(int serverLineID, string chatMsg)
		{
			LogManager.WriteLog(LogTypes.SQL, string.Format("AddChatMsg:LineID={0},Msg={1}", serverLineID, chatMsg), null, true);
			Queue<string> chatMsgQueue = ChatMsgManager.GetChatMsgQueue(serverLineID);
			lock (chatMsgQueue)
			{
				if (chatMsgQueue.Count > 30000)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("线路{0}的转发消息太多，被丢弃，一共丢弃{1}条，请检查GameServer是否正常", serverLineID, chatMsgQueue.Count), null, true);
					List<string> list = chatMsgQueue.ToList<string>();
					chatMsgQueue.Clear();
					Dictionary<string, int> dictionary = new Dictionary<string, int>();
					foreach (string text in list)
					{
						string text2 = string.Empty;
						try
						{
							text2 = text.Split(new char[]
							{
								':'
							})[5].Split(new char[]
							{
								' '
							})[0];
						}
						catch
						{
						}
						if (!string.IsNullOrEmpty(text2))
						{
							if (dictionary.ContainsKey(text2))
							{
								Dictionary<string, int> dictionary2;
								string key;
								(dictionary2 = dictionary)[key = text2] = dictionary2[key] + 1;
							}
							else
							{
								dictionary[text2] = 1;
							}
							if (text2.StartsWith("-buyyueka") || text2.StartsWith("-updateyb") || text2.StartsWith("-updateBindgold") || text2.StartsWith("-config"))
							{
								chatMsgQueue.Enqueue(text);
							}
						}
					}
					if (chatMsgQueue.Count<string>() >= 15000)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("线路{0}丢失重要命令{1}条", serverLineID, chatMsgQueue.Count<string>()), null, true);
						chatMsgQueue.Clear();
					}
					List<KeyValuePair<string, int>> list2 = dictionary.ToList<KeyValuePair<string, int>>();
					list2.Sort((KeyValuePair<string, int> _left, KeyValuePair<string, int> _right) => _right.Value - _left.Value);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("转发消息统计,").AppendFormat("共有{0}类消息:    ", list2.Count<KeyValuePair<string, int>>()).AppendLine();
					for (int i = 0; i < list2.Count<KeyValuePair<string, int>>(); i++)
					{
						string key2 = list2[i].Key;
						int value = list2[i].Value;
						if (value <= 10)
						{
							break;
						}
						stringBuilder.AppendFormat("   cmd={0}, cnt={1}", key2, value).AppendLine();
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("线路{0}的转发消息太多，丢弃日志分析如下{1}", serverLineID, stringBuilder.ToString()), null, true);
				}
				chatMsgQueue.Enqueue(chatMsg);
			}
		}

		public static TCPOutPacket GetWaitingChatMsg(TCPOutPacketPool pool, int cmdID, int serverLineID)
		{
			List<string> list = new List<string>();
			Queue<string> chatMsgQueue = ChatMsgManager.GetChatMsgQueue(serverLineID);
			lock (chatMsgQueue)
			{
				while (chatMsgQueue.Count > 0 && list.Count < 250)
				{
					list.Add(chatMsgQueue.Dequeue());
				}
			}
			return DataHelper.ObjectToTCPOutPacket<List<string>>(list, pool, cmdID);
		}

		public static void ScanGMMsgToGameServer(DBManager dbMgr)
		{
			try
			{
				long num = DateTime.Now.Ticks / 10000L;
				if (num - ChatMsgManager.LastScanInputGMMsgTicks >= 10000L)
				{
					ChatMsgManager.LastScanInputGMMsgTicks = num;
					List<string> list = new List<string>();
					DBQuery.ScanGMMsgFromTable(dbMgr, list);
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					for (int i = 0; i < list.Count; i++)
					{
						string text = list[i].Replace(":", "：");
						if (text.IndexOf("-config ") >= 0)
						{
							flag = true;
							string[] array = text.Trim().Split(new char[]
							{
								' '
							});
							if (array.Count<string>() == 3)
							{
								string paramName = array[1];
								string paramValue = array[2];
								DBWriter.UpdateGameConfig(dbMgr, paramName, paramValue);
							}
						}
						else if (text == "-reload kuafu")
						{
							flag2 = true;
						}
						else if (text == "-reloadall")
						{
							try
							{
								AssemblyPatchManager.getInstance().InitConfig();
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
						if (text.IndexOf("-resetgmail") >= 0)
						{
							flag3 = true;
						}
						if (text.IndexOf("-outrank") >= 0)
						{
							GameDBManager.RankCacheMgr.PrintfRankData();
						}
						ChatMsgManager.AddGMCmdChatMsg(-1, text);
					}
					if (flag)
					{
						GameDBManager.GameConfigMgr.LoadGameConfigFromDB(dbMgr);
					}
					if (flag2)
					{
						LineManager.LoadConfig();
					}
					if (flag3)
					{
						GroupMailManager.ResetData();
					}
				}
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("扫描GM命令表时发生了错误", new object[0]), null, true);
			}
		}

		private static Dictionary<int, Queue<string>> ChatMsgDict = new Dictionary<int, Queue<string>>();

		private static long LastScanInputGMMsgTicks = DateTime.Now.Ticks / 10000L;
	}
}
