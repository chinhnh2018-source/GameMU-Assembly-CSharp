using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Core.Executor;
using Server.Data;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	internal class FileBanLogic
	{
		private static void LoadBanFile()
		{
			long num = TimeUtil.NOW();
			if (num - FileBanLogic.m_UpdateTicks >= 10000L)
			{
				FileBanLogic.m_UpdateTicks = num;
				IEnumerable<string> enumerable = from file in Directory.EnumerateFiles(DataHelper.CurrentDirectory, "Ban*.txt", SearchOption.AllDirectories)
				select file;
				foreach (string text in enumerable)
				{
					FileStream fileStream = null;
					try
					{
						fileStream = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.None);
					}
					catch
					{
						fileStream = null;
					}
					if (null != fileStream)
					{
						StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
						string text2;
						while (null != (text2 = streamReader.ReadLine()))
						{
							string[] array = text2.Split(new char[]
							{
								','
							});
							if (array.Length > 0)
							{
								FileBanLogic.m_BanList.Add(array[0]);
							}
						}
						streamReader.Close();
						fileStream.Close();
						FileInfo fileInfo = new FileInfo(text);
						if (fileInfo.Attributes.ToString().IndexOf("ReadOnly") != -1)
						{
							fileInfo.Attributes = FileAttributes.Normal;
						}
						File.Delete(text);
					}
				}
			}
		}

		public static void Tick()
		{
			FileBanLogic.LoadBanFile();
			if (null != FileBanLogic.m_BanList)
			{
				if (FileBanLogic.m_IsNeedClear > 0)
				{
					FileBanLogic.m_BanList.Clear();
					FileBanLogic.m_IsNeedClear = 0;
				}
				bool flag = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("CrashUnityForce");
				int num = 20;
				while (num > 0 && FileBanLogic.m_BanList.Count > 0)
				{
					num--;
					string text = FileBanLogic.m_BanList[FileBanLogic.m_BanList.Count - 1];
					FileBanLogic.m_BanList.RemoveAt(FileBanLogic.m_BanList.Count - 1);
					BanManager.BanUserID2Memory(text);
					TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(text);
					if (null != tmsksocket)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
						if (null != gameClient)
						{
							RoleData cmdData = new RoleData
							{
								RoleID = -70
							};
							gameClient.sendCmd<RoleData>(104, cmdData, false);
							if (flag)
							{
								FileBanLogic.SendMagicCrashMsg(gameClient, MagicCrashUnityType.CrashTimeOut);
							}
							LogManager.WriteLog(8, string.Format("FileBanLogic ban2 userID={0} roleID={1}", text, gameClient.ClientData.RoleID), null, true);
						}
						else
						{
							Global.ForceCloseSocket(tmsksocket, "被禁止登陆", true);
							LogManager.WriteLog(8, string.Format("FileBanLogic ForceCloseSocket userID={0}", text), null, true);
						}
					}
				}
			}
		}

		public static void ClearBanList()
		{
			FileBanLogic.m_IsNeedClear = 1;
		}

		public static void BroadCastDetectHook()
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, true)) != null)
			{
				if (nextClient != null)
				{
					FileBanLogic.SendMagicCrashMsg(nextClient, MagicCrashUnityType.DetectHook);
				}
			}
		}

		public static void BroadCastDetectHook(List<string> uidList)
		{
			if (uidList != null)
			{
				foreach (string userID in uidList)
				{
					TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
					if (null != tmsksocket)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
						if (gameClient != null)
						{
							FileBanLogic.SendMagicCrashMsg(gameClient, MagicCrashUnityType.DetectHook);
						}
					}
				}
			}
		}

		private static void SendMagicCrashMsg(GameClient client, MagicCrashUnityType crashType)
		{
			if (client != null)
			{
				int randomNumber = Global.GetRandomNumber(5, 15);
				string cmdData = string.Format("{0}:{1}", (int)crashType, randomNumber);
				client.sendCmd(14010, cmdData, false);
			}
		}

		private static List<string> m_BanList = new List<string>();

		private static long m_UpdateTicks = 0L;

		private static int m_IsNeedClear = 0;
	}
}
