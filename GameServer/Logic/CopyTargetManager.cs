using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class CopyTargetManager
	{
		public static void LoadConfig()
		{
			lock (CopyTargetManager._Mutex)
			{
				CopyTargetManager.CopyTargetInfoDict.Clear();
				string uri = "Config/FuBenMuBiao.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements("FuBenMuBiao");
					foreach (XElement xelement2 in enumerable)
					{
						IEnumerable<XElement> enumerable2 = xelement2.Elements("MuBiao");
						foreach (XElement xml in enumerable2)
						{
							int num = (int)Global.GetSafeAttributeLong(xml, "FuBenID");
							int num2 = (int)Global.GetSafeAttributeLong(xml, "MuBiaoID");
							string[] array = Global.GetSafeAttributeStr(xml, "MuBiaoCanShu").Split(new char[]
							{
								'|'
							});
							foreach (string text in array)
							{
								CopyTargetInfo copyTargetInfo = new CopyTargetInfo();
								string[] array3 = text.Split(new char[]
								{
									','
								});
								if (2 != array3.Length)
								{
									LogManager.WriteLog(2, string.Format("Config/FuBenMuBiao.xml 解析失败 fubenID={0} targetID={1} field={2}", num, num2, text), null, true);
								}
								else
								{
									copyTargetInfo.MonsterID = Global.SafeConvertToInt32(array3[0]);
									copyTargetInfo.Count = Global.SafeConvertToInt32(array3[1]);
									if (!CopyTargetManager.CopyTargetInfoDict.ContainsKey(num))
									{
										Dictionary<int, List<CopyTargetInfo>> dictionary = new Dictionary<int, List<CopyTargetInfo>>();
										dictionary.Add(num2, new List<CopyTargetInfo>
										{
											copyTargetInfo
										});
										CopyTargetManager.CopyTargetInfoDict[num] = dictionary;
									}
									else if (!CopyTargetManager.CopyTargetInfoDict[num].ContainsKey(num2))
									{
										List<CopyTargetInfo> list = new List<CopyTargetInfo>();
										list.Add(copyTargetInfo);
										CopyTargetManager.CopyTargetInfoDict[num][num2] = list;
									}
									else
									{
										CopyTargetManager.CopyTargetInfoDict[num][num2].Add(copyTargetInfo);
									}
									CopyTargetKey copyTargetKey = new CopyTargetKey();
									copyTargetKey.fubenID = num;
									copyTargetKey.targetIdx = num2;
									if (!CopyTargetManager.CopyTargetKeyDict.ContainsKey(copyTargetInfo.MonsterID))
									{
										List<CopyTargetKey> list2 = new List<CopyTargetKey>();
										list2.Add(copyTargetKey);
										CopyTargetManager.CopyTargetKeyDict[copyTargetInfo.MonsterID] = list2;
									}
									else
									{
										CopyTargetManager.CopyTargetKeyDict[copyTargetInfo.MonsterID].Add(copyTargetKey);
									}
								}
							}
						}
					}
				}
			}
		}

		private static List<CopyTargetInfo> GetConfig(int fubenid, int targetIdx)
		{
			List<CopyTargetInfo> result = null;
			lock (CopyTargetManager._Mutex)
			{
				if (CopyTargetManager.CopyTargetInfoDict.ContainsKey(fubenid))
				{
					if (CopyTargetManager.CopyTargetInfoDict[fubenid].ContainsKey(targetIdx))
					{
						result = CopyTargetManager.CopyTargetInfoDict[fubenid][targetIdx];
					}
				}
			}
			return result;
		}

		private static int GetMonsterIdx(int monsterID, int fubenid)
		{
			lock (CopyTargetManager._Mutex)
			{
				if (CopyTargetManager.CopyTargetKeyDict.ContainsKey(monsterID))
				{
					foreach (CopyTargetKey copyTargetKey in CopyTargetManager.CopyTargetKeyDict[monsterID])
					{
						if (copyTargetKey.fubenID == fubenid)
						{
							return copyTargetKey.targetIdx;
						}
					}
				}
			}
			return 0;
		}

		public static void ProcessInitGame(GameClient client)
		{
			if (null != client)
			{
				int num = 0;
				List<CopyTargetInfo> config;
				for (;;)
				{
					num++;
					config = CopyTargetManager.GetConfig(client.ClientData.FuBenID, num);
					if (null == config)
					{
						break;
					}
					bool flag = false;
					foreach (CopyTargetInfo copyTargetInfo in config)
					{
						List<object> list = GameManager.MonsterMgr.FindMonsterByExtensionID(client.CurrentCopyMapID, copyTargetInfo.MonsterID);
						flag = (list.Count > 0);
						if (flag)
						{
							break;
						}
					}
					if (flag)
					{
						goto Block_4;
					}
				}
				return;
				Block_4:
				foreach (CopyTargetInfo copyTargetInfo in config)
				{
					List<object> list = GameManager.MonsterMgr.FindMonsterByExtensionID(client.CurrentCopyMapID, copyTargetInfo.MonsterID);
					string data = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						client.ClientData.FuBenID,
						num,
						copyTargetInfo.MonsterID,
						list.Count
					});
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 877);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		public static void ProcessMonsterDead(GameClient client, Monster monster)
		{
			if (null != client)
			{
				if (null != monster)
				{
					int monsterIdx = CopyTargetManager.GetMonsterIdx(monster.MonsterInfo.ExtensionID, client.ClientData.FuBenID);
					if (monsterIdx > 0)
					{
						List<object> list = GameManager.MonsterMgr.FindMonsterByExtensionID(monster.CurrentCopyMapID, monster.MonsterInfo.ExtensionID);
						string text = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.FuBenID,
							monsterIdx,
							monster.MonsterInfo.ExtensionID,
							list.Count
						});
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, text, 877);
						if (MapTypes.MarriageCopy == Global.GetMapType(client.ClientData.MapCode))
						{
							if (client.ClientData.MyMarriageData.byMarrytype > 0 && client.ClientData.MyMarriageData.nSpouseID > 0)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
								if (gameClient != null && MapTypes.MarriageCopy == Global.GetMapType(gameClient.ClientData.MapCode))
								{
									gameClient.sendCmd(877, text, false);
								}
							}
							client.sendCmd(877, text, false);
						}
						else
						{
							TeamData teamData = null;
							if (client.ClientData.TeamID > 0)
							{
								teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
							}
							if (null == teamData)
							{
								Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
							}
							else
							{
								lock (teamData)
								{
									for (int i = 0; i < teamData.TeamRoles.Count; i++)
									{
										GameClient gameClient2 = GameManager.ClientMgr.FindClient(teamData.TeamRoles[i].RoleID);
										if (null != gameClient2)
										{
											if (gameClient2.ClientData.MapCode == client.ClientData.MapCode)
											{
												if (gameClient2.ClientData.CopyMapID == client.ClientData.CopyMapID)
												{
													Global._TCPManager.MySocketListener.SendData(gameClient2.ClientSocket, tcpOutPacket, true);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static object _Mutex = new object();

		private static Dictionary<int, Dictionary<int, List<CopyTargetInfo>>> CopyTargetInfoDict = new Dictionary<int, Dictionary<int, List<CopyTargetInfo>>>();

		private static Dictionary<int, List<CopyTargetKey>> CopyTargetKeyDict = new Dictionary<int, List<CopyTargetKey>>();
	}
}
