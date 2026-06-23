using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	internal class GroupMailManager
	{
		public static void ResetData()
		{
			lock (GroupMailManager.GroupMailDataDict_Mutex)
			{
				GroupMailManager.LastMaxGroupMailID = 0;
				GroupMailManager.GroupMailDataDict.Clear();
			}
		}

		public static void RequestNewGroupMailList()
		{
			List<GroupMailData> list = Global.sendToDB<List<GroupMailData>, string>(10177, string.Format("{0}", GroupMailManager.LastMaxGroupMailID), 0);
			lock (GroupMailManager.GroupMailDataDict)
			{
				if (list != null && list.Count > 0)
				{
					foreach (GroupMailData groupMailData in list)
					{
						GroupMailManager.AddGroupMailData(groupMailData);
						if (groupMailData.GMailID > GroupMailManager.LastMaxGroupMailID)
						{
							GroupMailManager.LastMaxGroupMailID = groupMailData.GMailID;
						}
					}
				}
			}
		}

		private static void AddGroupMailData(GroupMailData gmailData)
		{
			lock (GroupMailManager.GroupMailDataDict_Mutex)
			{
				GroupMailManager.GroupMailDataDict[gmailData.GMailID] = gmailData;
			}
		}

		private static bool InConditions(GameClient client, GroupMailData gmailData)
		{
			bool result;
			if (string.IsNullOrEmpty(gmailData.Conditions))
			{
				result = true;
			}
			else
			{
				long num = TimeUtil.NOW() * 10000L;
				if (num < gmailData.InputTime || num > gmailData.EndTime)
				{
					result = false;
				}
				else
				{
					string[] array = gmailData.Conditions.Split(new char[]
					{
						'|'
					});
					bool flag = false;
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if ("level" == array2[0])
						{
							if (array2.Length != 2)
							{
								flag = true;
								break;
							}
							int num2 = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
							int num3 = Global.SafeConvertToInt32(array2[1]);
							if (num2 < num3)
							{
								return false;
							}
						}
						else if ("levelrange" == array2[0])
						{
							if (array2.Length != 3)
							{
								flag = true;
								break;
							}
							int num2 = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
							int num4 = Global.SafeConvertToInt32(array2[1]);
							int num5 = Global.SafeConvertToInt32(array2[2]);
							if (num2 < num4 || num2 > num5)
							{
								return false;
							}
						}
						else if ("loginrange" == array2[0])
						{
							if (array2.Length != 3)
							{
								flag = true;
								break;
							}
							string s = array2[1];
							string s2 = array2[2];
							DateTime beginTime = DateTime.Parse(s);
							DateTime endTime = DateTime.Parse(s2);
							if (!Global.CheckRoleIsLoginByTime(client, beginTime, endTime))
							{
								if (TimeUtil.NOW() * 10000L > endTime.Ticks)
								{
									GroupMailManager.SetGMailNeverSend(client, gmailData.GMailID, -1);
								}
								return false;
							}
						}
						else if ("vip" == array2[0])
						{
							if (array2.Length != 3)
							{
								flag = true;
								break;
							}
							int num2 = client.ClientData.VipLevel;
							int num4 = Global.SafeConvertToInt32(array2[1]);
							int num5 = Global.SafeConvertToInt32(array2[2]);
							if (num2 < num4 || num2 > num5)
							{
								return false;
							}
						}
						else
						{
							if (string.IsNullOrEmpty(array2[0]))
							{
								break;
							}
							LogManager.WriteLogUseCache(2, string.Format("GroupMailManager::InConditions Error Conditions={0}", gmailData.Conditions));
							return false;
						}
					}
					if (flag)
					{
						GroupMailManager.SetGMailIsSend(client, gmailData.GMailID);
						LogManager.WriteLog(2, string.Format("GroupMailManager::InConditions Error Conditions={0}", gmailData.Conditions), null, true);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		public static void CheckRoleGroupMail(GameClient client)
		{
			long num = TimeUtil.NOW();
			if (client.LastCheckGMailTick + 60000L <= num)
			{
				client.LastCheckGMailTick = num;
				lock (GroupMailManager.GroupMailDataDict)
				{
					foreach (KeyValuePair<int, GroupMailData> keyValuePair in GroupMailManager.GroupMailDataDict)
					{
						if (!GroupMailManager.IfGMailIsSend(client, keyValuePair.Value.GMailID))
						{
							if (GroupMailManager.InConditions(client, keyValuePair.Value))
							{
								GroupMailManager.SendGMail2Role(client, keyValuePair.Value);
							}
						}
					}
				}
			}
		}

		private static void SendGMail2Role(GameClient client, GroupMailData gmailData)
		{
			if (GroupMailManager.SetGMailNeverSend(client, gmailData.GMailID, 0))
			{
				int mailID = Global.UseMailGivePlayerAward2(client, gmailData.GoodsList, gmailData.Subject, gmailData.Content, gmailData.Yinliang, gmailData.Tongqian, gmailData.YuanBao);
				GroupMailManager.SetGMailNeverSend(client, gmailData.GMailID, mailID);
			}
		}

		public static bool IfGMailIsSend(GameClient client, int gmailID)
		{
			bool result;
			lock (client.ClientData.GroupMailRecordList)
			{
				result = (client.ClientData.GroupMailRecordList.IndexOf(gmailID) >= 0);
			}
			return result;
		}

		public static void SetGMailIsSend(GameClient client, int gmailID)
		{
			lock (client.ClientData.GroupMailRecordList)
			{
				if (client.ClientData.GroupMailRecordList.IndexOf(gmailID) < 0)
				{
					client.ClientData.GroupMailRecordList.Add(gmailID);
				}
			}
		}

		public static bool SetGMailNeverSend(GameClient client, int gmailID, int mailID)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, gmailID, mailID);
			string[] array = null;
			Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10178, strcmd, out array, client.ServerId);
			bool result;
			if (array == null || array.Length != 1)
			{
				result = false;
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				if (num <= 0)
				{
					result = false;
				}
				else
				{
					GroupMailManager.SetGMailIsSend(client, gmailID);
					result = true;
				}
			}
			return result;
		}

		private static int LastMaxGroupMailID = 0;

		private static object GroupMailDataDict_Mutex = new object();

		private static Dictionary<int, GroupMailData> GroupMailDataDict = new Dictionary<int, GroupMailData>();
	}
}
