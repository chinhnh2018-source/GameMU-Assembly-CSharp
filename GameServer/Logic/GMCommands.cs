using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Building;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.LoginWaiting;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Logic.MUWings;
using GameServer.Logic.Name;
using GameServer.Logic.Olympics;
using GameServer.Logic.OnePiece;
using GameServer.Logic.ProtoCheck;
using GameServer.Logic.Reborn;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.Spread;
using GameServer.Logic.Talent;
using GameServer.Logic.Tarot;
using GameServer.Logic.Ten;
using GameServer.Logic.Today;
using GameServer.Logic.TuJian;
using GameServer.Logic.UnionPalace;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Logic.UserReturn;
using GameServer.Logic.WanMota;
using GameServer.Logic.YueKa;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class GMCommands
	{
		public void InitGMCommands(XElement xml)
		{
			if (null == xml)
			{
				try
				{
					xml = XElement.Load("GMConfig.xml");
				}
				catch (Exception)
				{
					return;
				}
			}
			string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GameManager", "SuperUserNames");
			string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "GameManager", "UserNames");
			string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "GameManager", "IPs");
			if (!string.IsNullOrEmpty(safeAttributeStr))
			{
				this.SuperGMUserNames = safeAttributeStr.Trim().Split(new char[]
				{
					','
				});
			}
			if (!string.IsNullOrEmpty(safeAttributeStr2))
			{
				this.GMUserNames = safeAttributeStr2.Trim().Split(new char[]
				{
					','
				});
			}
			if (!string.IsNullOrEmpty(safeAttributeStr3))
			{
				this.GMIPs = safeAttributeStr3.Trim().Split(new char[]
				{
					','
				});
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			XElement xelement = Global.GetXElement(xml, "OtherNames");
			if (null != xelement)
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml2 in enumerable)
				{
					dictionary[Global.GetSafeAttributeStr(xml2, "Name")] = (int)Global.GetSafeAttributeLong(xml2, "Priority");
				}
			}
			this.OtherUserNamesDict = dictionary;
			Dictionary<int, string[]> dictionary2 = new Dictionary<int, string[]>();
			XElement xelement2 = Global.GetXElement(xml, "Prioritys");
			if (null != xelement2)
			{
				IEnumerable<XElement> enumerable2 = xelement2.Elements();
				foreach (XElement xml2 in enumerable2)
				{
					dictionary2[(int)Global.GetSafeAttributeLong(xml2, "ID")] = Global.GetSafeAttributeStr(xml2, "Cmds").Split(new char[]
					{
						','
					});
				}
			}
			this.GMCmdsDict = dictionary2;
		}

		public int ReloadGMCommands()
		{
			XElement xml = null;
			try
			{
				xml = XElement.Load("GMConfig.xml");
			}
			catch (Exception)
			{
				return -1;
			}
			GameManager.systemGMCommands.InitGMCommands(xml);
			return 0;
		}

		public bool IsSuperGMUser(TMSKSocket socket)
		{
			string userName = GameManager.OnlineUserSession.FindUserName(socket);
			return this.IsSuperGMUser(userName);
		}

		public bool IsSuperGMUser(string userName)
		{
			bool result;
			if (string.IsNullOrEmpty(userName))
			{
				result = false;
			}
			else
			{
				string[] superGMUserNames = this.SuperGMUserNames;
				if (superGMUserNames == null || superGMUserNames.Length <= 0)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < superGMUserNames.Length; i++)
					{
						if (superGMUserNames[i] == userName)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public bool IsGMUser(TMSKSocket socket)
		{
			string userName = GameManager.OnlineUserSession.FindUserName(socket);
			return this.IsGMUser(userName);
		}

		public bool IsGMUser(string userName)
		{
			bool result;
			if (string.IsNullOrEmpty(userName))
			{
				result = false;
			}
			else
			{
				string[] gmuserNames = this.GMUserNames;
				if (gmuserNames == null || gmuserNames.Length <= 0)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < gmuserNames.Length; i++)
					{
						if (gmuserNames[i].IndexOf("*") >= 0)
						{
							return true;
						}
						if (gmuserNames[i] == userName)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public int IsPriorityUser(TMSKSocket socket)
		{
			string userName = GameManager.OnlineUserSession.FindUserName(socket);
			return this.IsPriorityUser(userName);
		}

		public int IsPriorityUser(string userName)
		{
			int result;
			if (string.IsNullOrEmpty(userName))
			{
				result = -1;
			}
			else
			{
				Dictionary<string, int> otherUserNamesDict = this.OtherUserNamesDict;
				if (otherUserNamesDict == null || otherUserNamesDict.Count <= 0)
				{
					result = -1;
				}
				else
				{
					int num = -1;
					if (!otherUserNamesDict.TryGetValue(userName, out num))
					{
						num = -1;
					}
					result = num;
				}
			}
			return result;
		}

		private bool CanExecCmd(int priority, string cmd)
		{
			bool result;
			if (priority < 0)
			{
				result = true;
			}
			else if (string.IsNullOrEmpty(cmd))
			{
				result = false;
			}
			else
			{
				Dictionary<int, string[]> gmcmdsDict = this.GMCmdsDict;
				if (gmcmdsDict == null || gmcmdsDict.Count <= 0)
				{
					result = false;
				}
				else
				{
					string[] array = null;
					if (!gmcmdsDict.TryGetValue(priority, out array))
					{
						result = false;
					}
					else if (array == null || array.Length <= 0)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] == cmd)
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		public bool IsValidIP(TMSKSocket socket)
		{
			string socketRemoteEndPoint = Global.GetSocketRemoteEndPoint(socket, true);
			bool result;
			if (string.IsNullOrEmpty(socketRemoteEndPoint))
			{
				result = false;
			}
			else
			{
				string[] array = socketRemoteEndPoint.Split(new char[]
				{
					':'
				});
				if (array.Length <= 0)
				{
					result = false;
				}
				else
				{
					string[] gmips = this.GMIPs;
					if (gmips == null || gmips.Length <= 0)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < gmips.Length; i++)
						{
							if (gmips[i].IndexOf("*") >= 0)
							{
								return true;
							}
							if (gmips[i] == array[0])
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		public bool RegisterGmCmdHandler(string gmCmd, GmCmdHandler handler)
		{
			bool result;
			if (string.IsNullOrEmpty(gmCmd))
			{
				result = false;
			}
			else if (null == handler)
			{
				result = false;
			}
			else
			{
				lock (this.GmCmdsHandlerDict)
				{
					if (this.GmCmdsHandlerDict.ContainsKey(gmCmd))
					{
						result = false;
					}
					else
					{
						this.GmCmdsHandlerDict[gmCmd] = handler;
						result = true;
					}
				}
			}
			return result;
		}

		public bool RemoveGmCmdHandler(GmCmdHandler handler)
		{
			bool result;
			if (null == handler)
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(handler.gmCmd))
			{
				result = false;
			}
			else
			{
				lock (this.GmCmdsHandlerDict)
				{
					GmCmdHandler gmCmdHandler;
					if (!this.GmCmdsHandlerDict.TryGetValue(handler.gmCmd, out gmCmdHandler) || gmCmdHandler != handler)
					{
						result = false;
					}
					else
					{
						this.GmCmdsHandlerDict.Remove(handler.gmCmd);
						result = true;
					}
				}
			}
			return result;
		}

		public GmCmdHandler GetHandler(string gmCmd)
		{
			GmCmdHandler result;
			if (string.IsNullOrEmpty(gmCmd))
			{
				result = null;
			}
			else
			{
				lock (this.GmCmdsHandlerDict)
				{
					GmCmdHandler result2;
					if (this.GmCmdsHandlerDict.TryGetValue(gmCmd, out result2))
					{
						return result2;
					}
				}
				result = null;
			}
			return result;
		}

		public void OnClientLogin(GameClient client)
		{
			if (this.IsPriorityUser(client.strUserName) == 2)
			{
				client.ClientData.HideGM = 1;
				client.ClientData.MapCode = 6090;
				client.ClientData.PosX = 1000;
				client.ClientData.PosY = 1000;
				lock (this.GMClientList)
				{
					if (!this.GMClientList.Contains(client))
					{
						this.GMClientList.Add(client);
					}
				}
			}
		}

		public void OnClientLogout(GameClient client)
		{
			if (this.IsPriorityUser(client.strUserName) == 2)
			{
				lock (this.GMClientList)
				{
					this.GMClientList.Remove(client);
				}
			}
		}

		public void BroadcastChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			List<GameClient> range;
			lock (this.GMClientList)
			{
				int count = this.GMClientList.Count;
				if (count <= 0)
				{
					return;
				}
				range = this.GMClientList.GetRange(0, count);
			}
			TCPOutPacket tcpoutPacket = null;
			try
			{
				for (int i = 0; i < range.Count; i++)
				{
					if (client != range[i])
					{
						if (!range[i].LogoutState)
						{
							if (null == tcpoutPacket)
							{
								tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdText, 157);
							}
							if (!sl.SendData(range[i].ClientSocket, tcpoutPacket, false))
							{
							}
						}
					}
				}
			}
			finally
			{
				Global.PushBackTcpOutPacket(tcpoutPacket);
			}
		}

		public bool ProcessChatMessage(TMSKSocket socket, GameClient client, string text, bool transmit)
		{
			bool result;
			if (text.Length <= 0)
			{
				result = false;
			}
			else if (text[0] != '-')
			{
				result = false;
			}
			else if (this.NormalGMMsg(socket, client, text, transmit))
			{
				result = false;
			}
			else
			{
				bool flag = false;
				int priority = -1;
				if (!transmit)
				{
					int gmPriority = socket.session.gmPriority;
					switch (gmPriority)
					{
					case 0:
						return true;
					case 1:
						break;
					default:
						if (gmPriority != 1000)
						{
							priority = socket.session.gmPriority;
						}
						else
						{
							flag = true;
						}
						break;
					}
				}
				string[] array = text.Trim().Split(new char[]
				{
					' '
				});
				if (array.Length <= 0)
				{
					result = true;
				}
				else if (!this.CanExecCmd(priority, array[0]))
				{
					result = true;
				}
				else
				{
					GmCmdHandler handler = this.GetHandler(array[0]);
					if (null != handler)
					{
						result = handler.Process(client, text, array, transmit, flag);
					}
					else
					{
						result = this.ProcessGMCommands(client, text, array, transmit, flag);
					}
				}
			}
			return result;
		}

		public bool NormalGMMsg(TMSKSocket socket, GameClient client, string text, bool transmit)
		{
			return text == "-guanzhan";
		}

		private int SafeConvertToInt32(string str)
		{
			int result = -1;
			try
			{
				result = Convert.ToInt32(str);
			}
			catch (Exception ex)
			{
				result = -1;
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		private string GetServerBaseInfo(string cmd = null)
		{
			string str = string.Format("在线数量 {0}/{1}", GameManager.ClientMgr.GetClientCount(), Global._TCPManager.MySocketListener.ConnectedSocketsCount);
			str += "\r\n";
			int num = 0;
			int num2 = 0;
			ThreadPool.GetMaxThreads(out num, out num2);
			str += string.Format("线程池信息 workerThreads={0}, completionPortThreads={1}", num, num2);
			str += "\r\n";
			str += string.Format("TCP事件读写缓存数量 readPool={0}/{2}, writePool={1}/{2}", Global._TCPManager.MySocketListener.ReadPoolCount, Global._TCPManager.MySocketListener.WritePoolCount, Global._TCPManager.MySocketListener.numConnections * 3);
			str += "\r\n";
			str += string.Format("数据库指令数量 {0}", GameManager.DBCmdMgr.GetDBCmdCount());
			str += "\r\n";
			str += string.Format("与DbServer的连接数量{0}/{1}", Global._TCPManager.tcpClientPool.GetPoolCount(), Global._TCPManager.tcpClientPool.InitCount);
			str += "\r\n";
			str += string.Format("TcpOutPacketPool个数 {0}, 实例 {1}, TcpInPacketPool个数{2}, 实例 {3}, TCPCmdWrapper个数 {4}, SendCmdWrapper {5}", new object[]
			{
				Global._TCPManager.TcpOutPacketPool.Count,
				TCPOutPacket.GetInstanceCount(),
				Global._TCPManager.TcpInPacketPool.Count,
				TCPInPacket.GetInstanceCount(),
				TCPCmdWrapper.GetTotalCount(),
				SendCmdWrapper.GetInstanceCount()
			});
			str += "\r\n";
			string str2 = Global._MemoryManager.GetCacheInfoStr();
			str += str2;
			str += "\r\n";
			str2 = Global._FullBufferManager.GetFullBufferInfoStr();
			str += str2;
			str += "\r\n";
			str2 = Global._TCPManager.GetAllCacheCmdPacketInfo();
			return str + str2;
		}

		private string GetServerTCPInfo(string cmd = null)
		{
			bool flag = cmd.Contains("/c");
			bool flag2 = cmd.Contains("/d");
			string text = "";
			DateTime dateTime = TimeUtil.NowDateTime();
			text += string.Format("当前时间:{0},统计时长:{1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss"), (dateTime - ProcessSessionTask.StartTime).ToString());
			text += "\r\n";
			if (flag)
			{
				flag2 = true;
				ProcessSessionTask.StartTime = dateTime;
			}
			text += string.Format("总接收字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesReadSize / 1048576.0);
			text += "\r\n";
			text += string.Format("总发送字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesWriteSize / 1048576.0);
			text += "\r\n";
			text += string.Format("总处理指令个数 {0}", TCPCmdHandler.TotalHandledCmdsNum);
			text += "\r\n";
			text += string.Format("当前正在处理指令的线程数 {0}", TCPCmdHandler.GetHandlingCmdCount());
			text += "\r\n";
			text += string.Format("单个指令消耗的最大时间 {0}", TCPCmdHandler.MaxUsedTicksByCmdID);
			text += "\r\n";
			text += string.Format("消耗的最大时间指令ID {0}", (TCPGameServerCmds)TCPCmdHandler.MaxUsedTicksCmdID);
			text += "\r\n";
			text += string.Format("发送调用总次数 {0}", Global._TCPManager.MySocketListener.GTotalSendCount);
			text += "\r\n";
			text += string.Format("发送的最大包的大小 {0}", Global._SendBufferManager.MaxOutPacketSize);
			text += "\r\n";
			text += string.Format("发送的最大包的指令ID {0}", (TCPGameServerCmds)Global._SendBufferManager.MaxOutPacketSizeCmdID);
			text += "\r\n";
			text += string.Format("指令处理平均耗时（毫秒）{0}", (ProcessSessionTask.processCmdNum != 0L) ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum, 2) : 0.0);
			text += "\r\n";
			text += string.Format("指令处理耗时详情", new object[0]);
			text += "\r\n";
			int num = 0;
			lock (ProcessSessionTask.cmdMoniter)
			{
				foreach (PorcessCmdMoniter porcessCmdMoniter in ProcessSessionTask.cmdMoniter.Values)
				{
					if (flag2)
					{
						if (num++ == 0)
						{
							text += string.Format("{0, -48}{1, 6}{2, 7}{3, 7} {4, 7} {5, 4} {6, 4} {7, 5}", new object[]
							{
								"消息",
								"已处理次数",
								"平均处理时长",
								"总计消耗时长",
								"总计字节数",
								"发送次数",
								"发送字节数",
								"失败/成功/数据"
							});
							text += "\r\n";
						}
						string str = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##} {4, 13:0.##} {5, 8} {6, 12} {7, 4}/{8}/{9}", new object[]
						{
							(TCPGameServerCmds)porcessCmdMoniter.cmd,
							porcessCmdMoniter.processNum,
							TimeUtil.TimeMS(porcessCmdMoniter.avgProcessTime(), 2),
							TimeUtil.TimeMS(porcessCmdMoniter.processTotalTime, 2),
							porcessCmdMoniter.GetTotalBytes(),
							porcessCmdMoniter.SendNum,
							porcessCmdMoniter.OutPutBytes,
							porcessCmdMoniter.Num_Faild,
							porcessCmdMoniter.Num_OK,
							porcessCmdMoniter.Num_WithData
						});
						text += str;
						text += "\r\n";
					}
					else
					{
						if (num++ == 0)
						{
							text += string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", new object[]
							{
								"消息",
								"已处理次数",
								"平均处理时长",
								"总计消耗时长"
							});
							text += "\r\n";
						}
						string str = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}", new object[]
						{
							(TCPGameServerCmds)porcessCmdMoniter.cmd,
							porcessCmdMoniter.processNum,
							TimeUtil.TimeMS(porcessCmdMoniter.avgProcessTime(), 2),
							TimeUtil.TimeMS(porcessCmdMoniter.processTotalTime, 2)
						});
						text += str;
						text += "\r\n";
					}
					if (flag)
					{
						porcessCmdMoniter.Reset();
					}
				}
			}
			text = text.Substring(0, text.Length - 2);
			return text;
		}

		private string GetCopyMapInfo(string cmd = null)
		{
			string copyMapStrInfo = GameManager.CopyMapMgr.GetCopyMapStrInfo();
			return copyMapStrInfo.Substring(0, copyMapStrInfo.Length - 2);
		}

		private static string GetGCInfo(string cmd = null)
		{
			Program.CalcGCInfo();
			string text = "";
			try
			{
				text += string.Format("GC计数类别    {0,-10} {1,-10} {2,-10}", "0 gen", "1 gen", "2 gen");
				text += "\r\n";
				text += string.Format("总计GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCounts[0], Program.GCCollectionCounts[1], Program.GCCollectionCounts[2]);
				text += "\r\n";
				text += string.Format("每秒GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCountsNow[0], Program.GCCollectionCountsNow[1], Program.GCCollectionCountsNow[2]);
				text += "\r\n";
				text += string.Format("1秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts1s[0], Program.MaxGCCollectionCounts1s[1], Program.MaxGCCollectionCounts1s[2]);
				text += "\r\n";
				text += string.Format("5秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts5s[0], Program.MaxGCCollectionCounts5s[1], Program.MaxGCCollectionCounts5s[2]);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ShowGCInfo()", false, false);
			}
			return text;
		}

		private bool ProcessGMCommands(GameClient client, string msgText, string[] cmdFields, bool transmit, bool isSuperGMUser)
		{
			if (!transmit)
			{
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText);
			}
			string text = "";
			if ("-info" == cmdFields[0])
			{
				if (!transmit)
				{
					text = string.Format("当前线路{0}在线人数{1}", GameManager.ServerLineID, GameManager.ClientMgr.GetClientCount());
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
				}
			}
			else if ("-info2" == cmdFields[0])
			{
				if (!transmit)
				{
					string[] array = Global.ExecuteDBCmd(10063, string.Format("{0}", client.ClientData.RoleID), 0);
					if (array == null || array.Length < 1)
					{
						text = string.Format("获取所有线路在线人数时连接数据库失败", new object[0]);
						return true;
					}
					int num = Global.SafeConvertToInt32(array[0]);
					text = string.Format("获取所有线路在线人数是{0}", num);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
				}
			}
			else if ("-version" == cmdFields[0])
			{
				if (!transmit)
				{
					string gameConifgItem = GameManager.GameConfigMgr.GetGameConifgItem("gameserver_version");
					string gameConifgItem2 = GameManager.GameConfigMgr.GetGameConifgItem("gamedb_version");
					text = string.Format("gameserver_version：{0},gamedb_version：{1},client：mainver-{2},resver-{3},codereversion-{4}", new object[]
					{
						gameConifgItem,
						gameConifgItem2,
						client.MainExeVer,
						client.ResVer,
						client.CodeRevision
					});
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
				}
			}
			else if (!("-patch" == cmdFields[0]))
			{
				if ("-serverinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						string gameConifgItem2 = GameManager.GameConfigMgr.GetGameConifgItem("gamedb_version");
						text = string.Format("{0}_{1}", Global.GetLocalAddressIPs(), TCPManager.ServerPort);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
					}
				}
				else if ("-baseinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							text = this.GetServerBaseInfo(null);
							client.sendCmd<ServerCommandResult>(30002, new ServerCommandResult
							{
								Cmd = cmdFields[0],
								ResultString = text
							}, false);
						}
					}
				}
				else if ("-tcpinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							ServerCommandResult serverCommandResult = new ServerCommandResult();
							if (cmdFields.Length > 1)
							{
								text = this.GetServerTCPInfo(cmdFields[0] + cmdFields[1]);
								serverCommandResult.Cmd = cmdFields[0] + " " + cmdFields[1];
							}
							else
							{
								text = this.GetServerTCPInfo(cmdFields[0]);
								serverCommandResult.Cmd = cmdFields[0];
							}
							serverCommandResult.ResultString = text;
							client.sendCmd<ServerCommandResult>(30002, serverCommandResult, false);
						}
					}
				}
				else if ("-copymapinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							text = this.GetCopyMapInfo(null);
							client.sendCmd<ServerCommandResult>(30002, new ServerCommandResult
							{
								Cmd = cmdFields[0],
								ResultString = text
							}, false);
						}
					}
				}
				else if ("-gcinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							text = GMCommands.GetGCInfo(null);
							client.sendCmd<ServerCommandResult>(30002, new ServerCommandResult
							{
								Cmd = cmdFields[0],
								ResultString = text
							}, false);
						}
					}
				}
				else if ("-hide" == cmdFields[0])
				{
					if (!transmit)
					{
						if (client.ClientData.HideGM < 1)
						{
							client.ClientData.HideGM = 1;
							List<object> all9Clients = Global.GetAll9Clients(client);
							GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, all9Clients);
							GameManager.ClientMgr.RemoveRolePet(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, all9Clients, false);
							text = string.Format("进入隐身模式", new object[0]);
						}
						else
						{
							text = string.Format("已经是隐身模式", new object[0]);
						}
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
					}
				}
				else if ("-show" == cmdFields[0])
				{
					if (!transmit)
					{
						if (client.ClientData.HideGM > 0)
						{
							client.ClientData.HideGM = 0;
							List<object> all9Clients = Global.GetAll9Clients(client);
							GameManager.ClientMgr.NotifyOthersIamComing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, all9Clients, 110);
							text = string.Format("退出隐身模式", new object[0]);
						}
						else
						{
							text = string.Format("已经退出隐身模式", new object[0]);
						}
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
					}
				}
				else if ("-moveto" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -moveto 地图编号 X坐标 Y坐标", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num2 = this.SafeConvertToInt32(cmdFields[1]);
							int num3 = 0;
							int num4 = 0;
							if (cmdFields.Length >= 4)
							{
								num3 = this.SafeConvertToInt32(cmdFields[2]);
								num4 = this.SafeConvertToInt32(cmdFields[3]);
							}
							GameMap gameMap = null;
							client.CheckCheatData.GmGotoShadowMapCode = num2;
							if (GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
							{
								Point agridPointIn4Direction = Global.GetAGridPointIn4Direction(ObjectTypes.OT_CLIENT, new Point((double)(num3 / gameMap.MapGridWidth), (double)(num4 / gameMap.MapGridHeight)), num2, 0, false);
								agridPointIn4Direction = new Point(agridPointIn4Direction.X * (double)gameMap.MapGridWidth, agridPointIn4Direction.Y * (double)gameMap.MapGridHeight);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, num2, (int)agridPointIn4Direction.X, (int)agridPointIn4Direction.Y, 0, 0))
								{
									if (num2 != client.ClientData.MapCode)
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num2, (int)agridPointIn4Direction.X, (int)agridPointIn4Direction.Y, -1, 0);
									}
									else
									{
										GameManager.LuaMgr.GotoMap(client, num2, (int)agridPointIn4Direction.X, (int)agridPointIn4Direction.Y, -1);
									}
									text = string.Format("执行移动到目标点({0},{1})的操作成功", agridPointIn4Direction.X, agridPointIn4Direction.Y);
								}
								else
								{
									text = string.Format("目标点({0},{1})可能是障碍物", num3, num4);
								}
							}
							else
							{
								text = string.Format("请输入正确的地图编号", new object[0]);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-moveto2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -moveto2 副本序号 地图编号 X坐标 Y坐标", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int fuBenSeqID = this.SafeConvertToInt32(cmdFields[1]);
							int num2 = this.SafeConvertToInt32(cmdFields[2]);
							int num3 = 0;
							int num4 = 0;
							if (cmdFields.Length >= 5)
							{
								num3 = this.SafeConvertToInt32(cmdFields[3]);
								num4 = this.SafeConvertToInt32(cmdFields[4]);
							}
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
							{
								if (gameMap.IsolatedMap == 1)
								{
									client.ClientData.FuBenID = num2;
								}
								client.ClientData.FuBenSeqID = fuBenSeqID;
								Point agridPointIn4Direction = Global.GetAGridPointIn4Direction(ObjectTypes.OT_CLIENT, new Point((double)(num3 / gameMap.MapGridWidth), (double)(num4 / gameMap.MapGridHeight)), num2, 0, false);
								agridPointIn4Direction = new Point(agridPointIn4Direction.X * (double)gameMap.MapGridWidth, agridPointIn4Direction.Y * (double)gameMap.MapGridHeight);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, num2, (int)agridPointIn4Direction.X, (int)agridPointIn4Direction.Y, 0, 0))
								{
									if (num2 != client.ClientData.MapCode)
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num2, (int)agridPointIn4Direction.X, (int)agridPointIn4Direction.Y, -1, 0);
									}
									else
									{
										GameManager.LuaMgr.GotoMap(client, num2, (int)agridPointIn4Direction.X, (int)agridPointIn4Direction.Y, -1);
									}
									text = string.Format("执行移动到目标点({0},{1})的操作成功", agridPointIn4Direction.X, agridPointIn4Direction.Y);
								}
								else
								{
									text = string.Format("目标点({0},{1})可能是障碍物", num3, num4);
								}
							}
							else
							{
								text = string.Format("请输入正确的地图编号", new object[0]);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-line" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -line 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							string[] array = Global.ExecuteDBCmd(195, string.Format("{0}:{1}:0", client.ClientData.RoleID, text2), 0);
							if (array == null || array.Length < 5)
							{
								text = string.Format("获取{0}的线路状态时连接数据库失败", text2);
							}
							else
							{
								int num5 = Global.SafeConvertToInt32(array[4]);
								text = string.Format("{0}所在的线路是{1}", text2, num5);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-viewum" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -viewum 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							string[] array = Global.ExecuteDBCmd(10069, string.Format("{0}:{1}", client.ClientData.RoleID, text2), 0);
							if (array == null || array.Length < 4)
							{
								text = string.Format("获取{0}的元宝数时连接数据库失败", text2);
							}
							else
							{
								string text3 = array[1];
								int num6 = Global.SafeConvertToInt32(array[2]);
								int num7 = Global.SafeConvertToInt32(array[3]);
								text = string.Format("{0}的平台ID是{1}，元宝是{2}，ＲＭＢ{3}", new object[]
								{
									text2,
									text3,
									num6,
									num7
								});
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-follow" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -follow 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num2 = gameClient.ClientData.MapCode;
							int num3 = gameClient.ClientData.PosX;
							int num4 = gameClient.ClientData.PosY;
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap) && MapTypes.Normal == Global.GetMapType(num2))
							{
								Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, num2, num3, num4, 60);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, num2, (int)mapPoint.X, (int)mapPoint.Y, 0, 0))
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num2, (int)mapPoint.X, (int)mapPoint.Y, -1, 0);
									text = string.Format("执行移动到{0}身边的操作成功", text2);
								}
								else
								{
									text = string.Format("执行移动到{0}身边的操作失败", text2);
								}
							}
							else
							{
								text = string.Format("{0}目前在副本地图, 无法移动到其旁边", text2);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-buff" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -buff buffid bufferParams...", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num9 = Global.SafeConvertToInt32(cmdFields[1]);
							double[] array2 = new double[cmdFields.Length - 2];
							for (int i = 2; i < cmdFields.Length; i++)
							{
								array2[i - 2] = Global.SafeConvertToDouble(cmdFields[i]);
							}
							Global.UpdateBufferData(client, (BufferItemTypes)num9, array2, 1, true);
						}
					}
				}
				else if ("-rolestatus" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							text = string.Format("请输入： -rolestatus type startTimeType(0|1) secs moveSpeed", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num10 = Global.SafeConvertToInt32(cmdFields[1]);
							int num11 = Global.SafeConvertToInt32(cmdFields[2]);
							int num12 = Global.SafeConvertToInt32(cmdFields[3]);
							double tag = (double)Global.SafeConvertToInt32(cmdFields[4]);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num10, (num11 == 0) ? 0L : TimeUtil.NOW(), num12, tag);
						}
					}
				}
				else if ("-ShenJiJiFen" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -ShenJiJiFen 神迹积分变化值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int addValue = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyShenJiJiFenValue(client, addValue, "GM命令", false, true);
						}
					}
				}
				else if ("-compboom" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -compboom compid modval", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int compType = Global.SafeConvertToInt32(cmdFields[1]);
							int num13 = Global.SafeConvertToInt32(cmdFields[2]);
							TianTiClient.getInstance().Comp_CompOpt(compType, 0, num13, 0);
						}
					}
				}
				else if ("-compjunxian" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -compjunxian modval", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num13 = Global.SafeConvertToInt32(cmdFields[1]);
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, num13);
							string msgText2 = string.Format(GLang.GetLang(4017, new object[0]), num13);
							GameManager.ClientMgr.NotifyImportantMsg(client, msgText2, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
				}
				else if ("-compdonate" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -compdonate modval", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num13 = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyCompDonateValue(client, num13, "GM指令-势力贡献", true, true, true);
						}
					}
				}
				else if ("-comptitle" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -comptitle buffid active", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num9 = Global.SafeConvertToInt32(cmdFields[1]);
							int num14 = Global.SafeConvertToInt32(cmdFields[2]);
							if (num9 < 9000 || num9 > 9011)
							{
								text = string.Format("请输入合法的职务类型9000~9011！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							if (num14 > 0)
							{
								double[] array2 = new double[]
								{
									1.0
								};
								Global.UpdateBufferData(client, (BufferItemTypes)num9, array2, 1, true);
							}
							else
							{
								double[] array3 = new double[1];
								double[] array2 = array3;
								Global.UpdateBufferData(client, (BufferItemTypes)num9, array2, 1, true);
							}
						}
					}
				}
				else if ("-compzhiwu" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -compzhiwu zhiwu", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num15 = Global.SafeConvertToInt32(cmdFields[1]);
							if (num15 < 1 || num15 > 5)
							{
								text = string.Format("请输入合法的职务类型！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							if (client.ClientData.CompType <= 0)
							{
								text = string.Format("请先加入势力！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							client.ClientData.CompZhiWu = (byte)num15;
							client.sendCmd(1135, string.Format("{0}:{1}", client.ClientData.RoleID, num15), false);
						}
					}
				}
				else if ("-compnotice" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							text = string.Format("请输入： -compnotice id toMapCode toPosX toPosY", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							CompScene compScene = client.SceneObject as CompScene;
							if (null == compScene)
							{
								text = string.Format("请在势力场景内执行该GM命令！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int noticeID = Global.SafeConvertToInt32(cmdFields[1]);
							int toMapCode = Global.SafeConvertToInt32(cmdFields[2]);
							int toPosX = Global.SafeConvertToInt32(cmdFields[3]);
							int toPosY = Global.SafeConvertToInt32(cmdFields[4]);
							KFCompNotice notice = new KFCompNotice
							{
								NoticeID = noticeID,
								CompType = compScene.CompSceneInfo.ID,
								Param1 = Global.FormatRoleNameWithZoneId2(client),
								Param2 = Global.FormatRoleNameWithZoneId2(client),
								toMapCode = toMapCode,
								toPosX = toPosX,
								toPosY = toPosY
							};
							CompManager.getInstance().CompNotice(compScene, notice);
						}
					}
				}
				else if ("-alchemy" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -alchemy 角色ID 货币类型 回退值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							int num16 = Global.SafeConvertToInt32(cmdFields[2]);
							int num17 = Global.SafeConvertToInt32(cmdFields[3]);
							string text4 = string.Format("{0},{1}", num16, num17);
							if (!AlchemyManager.getInstance().AlchemyRollBackCheck(num16, num17))
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", num8, text4), null, true);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								AlchemyManager.getInstance().AlchemyRollBackOffline(num8, text4);
								return true;
							}
							AlchemyManager.getInstance().AlchemyRollBack(gameClient, text4);
						}
					}
					else if (cmdFields.Length == 4)
					{
						int num8 = Global.SafeConvertToInt32(cmdFields[1]);
						int num16 = Global.SafeConvertToInt32(cmdFields[2]);
						int num17 = Global.SafeConvertToInt32(cmdFields[3]);
						string text4 = string.Format("{0},{1}", num16, num17);
						if (!AlchemyManager.getInstance().AlchemyRollBackCheck(num16, num17))
						{
							LogManager.WriteLog(3, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", num8, text4), null, true);
							return true;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
						if (null == gameClient)
						{
							AlchemyManager.getInstance().AlchemyRollBackOffline(num8, text4);
							return true;
						}
						AlchemyManager.getInstance().AlchemyRollBack(gameClient, text4);
					}
				}
				else if ("-OnePieceRoll" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -OnePieceRoll 骰子点数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int onePiece_FakeRollNum_GM = Global.SafeConvertToInt32(cmdFields[1]);
							OnePieceManager.getInstance().OnePiece_FakeRollNum_GM = onePiece_FakeRollNum_GM;
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-OnePieceDice" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -OnePieceDice 骰子类型 数量", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int diceType = Global.SafeConvertToInt32(cmdFields[1]);
							int newNum = Global.SafeConvertToInt32(cmdFields[2]);
							OnePieceManager.getInstance().GM_SetDice(client, diceType, newNum);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-OnePieceMove" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							text = string.Format("请输入： -OnePieceMove", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							OnePieceManager.getInstance().ProcessOnePieceMoveCmd(client, 1604, null, null);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-OnePiecePos" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -OnePiecePos 目标位置", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int posid = Global.SafeConvertToInt32(cmdFields[1]);
							OnePieceManager.getInstance().GM_SetPosID(client, posid);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-KarenEasy" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							text = string.Format("请输入： -KarenEasy 1 或 0(1打开gm模式，0关闭gm模式)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num18 = Global.SafeConvertToInt32(cmdFields[1]);
							KarenBattleManager.getInstance().GMTest = (num18 != 0);
						}
					}
				}
				else if ("-kfmap" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							text = string.Format("请输入： -kfmap mapcode lineid bossid teleportid", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string[] array4 = new string[cmdFields.Length - 1];
							Array.Copy(cmdFields, 1, array4, 0, cmdFields.Length - 1);
							KuaFuMapManager.getInstance().ProcessKuaFuMapEnterCmd(client, 0, null, array4);
						}
					}
				}
				else if ("-Karen" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							text = string.Format("请输入： -Karen 战场ID(区分东西战场)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int battleID = Global.SafeConvertToInt32(cmdFields[1]);
							KarenBattleSceneInfo karenBattleSceneInfo = KarenBattleManager.getInstance().TryGetKarenBattleSceneInfoByBattleID(battleID);
							if (null == karenBattleSceneInfo)
							{
								return true;
							}
							SceneUIClasses mapSceneType = Global.GetMapSceneType(karenBattleSceneInfo.MapCode);
							if (mapSceneType == 41)
							{
							}
							KuaFuServerInfo kuaFuServerInfo = null;
							KarenFuBenData karenKuaFuFuBenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(karenBattleSceneInfo.MapCode);
							if (karenKuaFuFuBenData == null || !KuaFuManager.getInstance().TryGetValue(karenKuaFuFuBenData.ServerId, out kuaFuServerInfo))
							{
								text = string.Format("请确阿卡伦线路畅通", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
								clientKuaFuServerLoginData.GameId = (long)karenKuaFuFuBenData.GameId;
								clientKuaFuServerLoginData.GameType = karenKuaFuFuBenData.GameType;
								clientKuaFuServerLoginData.EndTicks = karenKuaFuFuBenData.EndTime.Ticks;
								clientKuaFuServerLoginData.ServerId = client.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
								clientKuaFuServerLoginData.FuBenSeqId = 0;
							}
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
					}
				}
				else if ("-upKingOfBattlePoint" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -upKingOfBattlePoint 王者战场积分", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int addValue2 = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyKingOfBattlePointValue(client, addValue2, "GM命令", false, true);
						}
					}
				}
				else if ("-upCharmPoint" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -upCharmPoint 魅力积分", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int addValue3 = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, addValue3, "GM命令", false, true, false);
						}
					}
				}
				else if ("-upOnePieceJiFen" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -upTreasureJiFen 藏宝积分", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int addValue4 = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyTreasureJiFenValue(client, addValue4, "GM命令", true);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-upOnePieceXueZuan" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -upTreasureXueZuan 藏宝血钻", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int addValue5 = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyTreasureXueZuanValue(client, addValue5, false, true);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-upBuildLev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -upBuildLev 建筑物ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int buildID = Global.SafeConvertToInt32(cmdFields[1]);
							BuildingManager.getInstance().BuildingLevelUp_GM(client, buildID);
						}
					}
				}
				else if ("-upInputPoints" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -upInputPoints 角色名称 充值点(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num19 = Global.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							JieriIPointsExchgActivity jieriIPointsExchgActivity = HuodongCachingMgr.GetJieriIPointsExchgActivity();
							string text5 = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								num8,
								num19,
								jieriIPointsExchgActivity.FromDate.Replace(':', '$'),
								jieriIPointsExchgActivity.ToDate.Replace(':', '$')
							});
							Global.ExecuteDBCmd(13151, text5, gameClient.ServerId);
							jieriIPointsExchgActivity.NotifyInputPointsInfo(gameClient, false);
							text = string.Format("{0}充值点数变化({1})", text2, num19);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, text);
							gameClient._IconStateMgr.CheckJieRiActivity(gameClient, false);
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							UserReturnManager.getInstance().CheckActivityTip(gameClient);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string text6 = cmdFields[1];
						int num19 = Global.SafeConvertToInt32(cmdFields[2]);
						TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(text6);
						GameClient gameClient = null;
						if (null != tmsksocket)
						{
							gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
						}
						JieriIPointsExchgActivity jieriIPointsExchgActivity = HuodongCachingMgr.GetJieriIPointsExchgActivity();
						if (null != gameClient)
						{
							string text5 = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								gameClient.ClientData.RoleID,
								num19,
								jieriIPointsExchgActivity.FromDate.Replace(':', '$'),
								jieriIPointsExchgActivity.ToDate.Replace(':', '$')
							});
							string[] array5 = Global.ExecuteDBCmd(13151, text5, gameClient.ServerId);
							if (array5 != null && array5.Length == 2 && Convert.ToInt32(array5[1]) >= 0)
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求为账号：【{0}】添加充值积分【{1}】点！", text6, num19), null, true);
							}
							jieriIPointsExchgActivity.NotifyInputPointsInfo(gameClient, false);
							text = string.Format("{0}充值点数变化({1})", gameClient.ClientData.RoleName, num19);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, text);
							gameClient._IconStateMgr.CheckJieRiActivity(gameClient, false);
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							UserReturnManager.getInstance().CheckActivityTip(gameClient);
						}
						else
						{
							string text5 = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								text6,
								num19,
								jieriIPointsExchgActivity.FromDate.Replace(':', '$'),
								jieriIPointsExchgActivity.ToDate.Replace(':', '$')
							});
							string[] array5 = Global.ExecuteDBCmd(13152, text5, 0);
							if (array5 != null && array5.Length == 2 && Convert.ToInt32(array5[1]) >= 0)
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求为账号：【{0}】添加充值积分【{1}】点！", text6, num19), null, true);
							}
						}
					}
				}
				else if ("-substrong" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入：-substrong 角色名称 减少值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num20 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							gameClient.UsingEquipMgr.GMAddEquipStrong(gameClient, num20);
							text = string.Format("{0}佩戴的装备耐久减少({1})", text2, num20);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-addstorejinbi" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						long num21 = Convert.ToInt64(cmdFields[1]);
						GameManager.ClientMgr.AddUserStoreYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num21, "gm增加", true);
					}
				}
				else if ("-addstorebdjinbi" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						long num21 = Convert.ToInt64(cmdFields[1]);
						GameManager.ClientMgr.AddUserStoreMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num21, "gm增加", false);
					}
				}
				else if ("-qingkongyuansu" == cmdFields[0])
				{
					List<GoodsData> list = client.ClientData.ElementhrtsList;
					for (int i = list.Count - 1; i >= 0; i--)
					{
						GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, list[i], 1, false, false);
					}
				}
				else if ("-addyuansufenmo" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int num20 = this.SafeConvertToInt32(cmdFields[1]);
						GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, num20, "GM命令", true, false);
					}
				}
				else if ("-addlingjing" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int num20 = this.SafeConvertToInt32(cmdFields[1]);
						GameManager.ClientMgr.ModifyMUMoHeValue(client, num20, "GM命令", false, true, false);
					}
				}
				else if ("-updatebanggoods" == cmdFields[0])
				{
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
					if (null == bangHuiMiniData)
					{
						text = string.Format("你还没有帮会丫", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						return true;
					}
					if (cmdFields.Length == 8)
					{
						int num22 = this.SafeConvertToInt32(cmdFields[1]);
						int num23 = this.SafeConvertToInt32(cmdFields[2]);
						int num24 = this.SafeConvertToInt32(cmdFields[3]);
						int num25 = this.SafeConvertToInt32(cmdFields[4]);
						int num26 = this.SafeConvertToInt32(cmdFields[5]);
						int num27 = this.SafeConvertToInt32(cmdFields[6]);
						int num28 = this.SafeConvertToInt32(cmdFields[7]);
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.Faction,
							num22,
							num23,
							num24,
							num25,
							num26,
							num27,
							num28
						});
						string[] array6 = Global.ExecuteDBCmd(315, strcmd, client.ServerId);
						if (array6 == null || array6.Length != 4)
						{
						}
					}
				}
				else if ("-setbanglevel" == cmdFields[0])
				{
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
					if (null == bangHuiMiniData)
					{
						text = string.Format("你还没有帮会丫", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						return true;
					}
					if (cmdFields.Length == 2)
					{
						int num29 = this.SafeConvertToInt32(cmdFields[1]);
						string strcmd = string.Format("{0}:{1}", client.ClientData.Faction, num29);
						string[] array6 = Global.ExecuteDBCmd(10175, strcmd, 0);
						if (array6 == null || array6.Length != 1)
						{
							text = string.Format("GameDBServer返回失败了", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							return true;
						}
						int num30 = Global.SafeConvertToInt32(array6[0]);
						if (num30 >= 0)
						{
							JunQiManager.NotifySyncBangHuiJunQiItemsDict(client);
							Global.BroadcastJunQiUpLevelHint(client, num29);
						}
					}
				}
				else if ("-setwanmota" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int num29 = this.SafeConvertToInt32(cmdFields[1]);
						WanMoTaDBCommandManager.LayerChangeDBCommand(client, num29);
						GameManager.ClientMgr.SaveWanMoTaPassLayerValue(client, num29, true);
						client.ClientData.WanMoTaNextLayerOrder = num29;
					}
				}
				else if ("-addyuansu" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -addyuansu 元素道具id 元素等级1-99", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num31 = this.SafeConvertToInt32(cmdFields[1]);
							int num32 = 1;
							int num33 = 0;
							int num34 = this.SafeConvertToInt32(cmdFields[2]);
							int num35 = 0;
							int lucky = 0;
							int num36 = 0;
							num34 = Global.GMax(1, num34);
							num34 = Global.GMin(99, num34);
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
							{
								text = string.Format("系统中不存在{0}", num31);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num37 = 0;
							int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
							if (intValue >= 800 && intValue < 816)
							{
								num37 = 3000;
							}
							for (int i = 0; i < num32; i++)
							{
								if (!Global.CanAddGoods(client, num31, 1, num33, "1900-01-01 12:00:00", true, false))
								{
									text = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
									{
										client,
										i,
										num31,
										num32 - i
									});
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									client.ClientData.RoleName,
									num31,
									1,
									num34,
									0,
									num33
								}), null, true);
								List<int> list2 = new List<int>();
								list2.Add(num34);
								list2.Add(0);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num31, 1, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, list2, 0, true);
							}
							text = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
							{
								client.ClientData.RoleName,
								num31,
								num32,
								num34,
								0,
								num33
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-addzhangong" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int num20 = this.SafeConvertToInt32(cmdFields[1]);
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref num20, AddBangGongTypes.None, 0);
					}
				}
				else if ("-setviplev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入：-setviplev 角色名称 VIP等级", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num20 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							Global.GMSetVipLevel(gameClient, num20);
							text = string.Format("设置{0}的VIP等级为({1})", text2, num20);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-kick" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -kick 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							text = string.Format("将{0}踢出了当前线路", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								string text7 = string.Format("-kick {0}", cmdFields[1]);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									num8,
									"",
									0,
									"",
									0,
									text7,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								string text7 = string.Format("-kick {0}", cmdFields[1]);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									num8,
									"",
									0,
									"",
									0,
									text7,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器...", text2), null, true);
							Global.ForceCloseClient(gameClient, "被GM踢了", true);
						}
					}
					else if (cmdFields.Length >= 2)
					{
						string text2 = cmdFields[1];
						int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
						if (-1 == num8)
						{
							return true;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
						if (null == gameClient)
						{
							return true;
						}
						LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器...", text2), null, true);
						Global.ForceCloseClient(gameClient, "被GM踢了", true);
					}
				}
				else if ("-kick_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -kick_rid 角色ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1] + "$rid";
							text = string.Format("将{0}踢出了当前线路", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							if (-1 == num8)
							{
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器...", text2), null, true);
							Global.ForceCloseClient(gameClient, "被GM踢了", true);
						}
					}
					else if (cmdFields.Length >= 2)
					{
						string text2 = cmdFields[1] + "$rid";
						int num8 = Global.SafeConvertToInt32(cmdFields[1]);
						if (-1 == num8)
						{
							return true;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
						if (null == gameClient)
						{
							return true;
						}
						LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器...", text2), null, true);
						Global.ForceCloseClient(gameClient, "被GM踢了", true);
					}
				}
				else if ("-kicku" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -kicku 账户名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text8 = cmdFields[1];
							text = string.Format("将账户{0}的角色踢出服务器", text8);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserName(text8);
							if (null == tmsksocket)
							{
								string text7 = string.Format("-kicku {0}", cmdFields[1]);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									0,
									"",
									0,
									"",
									0,
									text7,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
							if (null == gameClient)
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求将账户{0}的连接踢出了服务器...", text8), null, true);
								Global.ForceCloseSocket(tmsksocket, "被GM踢了, 但是这个socket上没有对应的client", true);
							}
							else
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求将账户{0}的角色踢出了服务器...", text8), null, true);
								Global.ForceCloseClient(gameClient, "被GM踢了", true);
							}
						}
					}
					else if (cmdFields.Length >= 2)
					{
						string text8 = cmdFields[1];
						if (cmdFields.Length >= 3)
						{
							int num38;
							if (int.TryParse(cmdFields[2], out num38) && num38 == GameManager.ServerLineID)
							{
								return true;
							}
						}
						TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserName(text8);
						if (null == tmsksocket)
						{
							return true;
						}
						long num39;
						if (cmdFields.Length >= 4 && long.TryParse(cmdFields[3], out num39))
						{
							if (num39 < tmsksocket.session.SocketTime[0])
							{
								return true;
							}
						}
						LogManager.WriteLog(3, string.Format("根据GM的要求将账户{0}的角色踢出了服务器...", text8), null, true);
						GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
						if (null == gameClient)
						{
							Global.ForceCloseSocket(tmsksocket, "被GM踢了, 但是这个socket上没有对应的client", true);
						}
						else
						{
							Global.ForceCloseClient(gameClient, "被GM踢了", true);
						}
					}
				}
				else if ("-ban" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -ban 角色名称 禁止的分钟(例如1表示禁止1分钟内登陆)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num40 = Global.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 != num8)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null != gameClient)
								{
									LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", text2), null, true);
									Global.ForceCloseClient(gameClient, "被GM踢了", true);
								}
								else
								{
									string text7 = string.Format("-kick {0}", cmdFields[1]);
									GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										num8,
										"",
										0,
										"",
										0,
										text7,
										0,
										0,
										GameManager.ServerLineID
									}), null, 0);
								}
							}
							Global.BanRoleNameToDBServer(text2, num40);
							text = string.Format("将{0}踢出了当前线路, 并禁止从任何线路再登陆", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-unban" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -unban 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							Global.BanRoleNameToDBServer(text2, 0);
							text = string.Format("解除对于{0}的禁止登陆限制", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-banchat" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -banchat 角色名称 几个小时", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num41 = this.SafeConvertToInt32(cmdFields[2]);
							if (num41 > 0)
							{
								Global.BanRoleChatToDBServer(text2, num41);
								BanChatManager.AddBanRoleName(text2, num41);
							}
							text = string.Format("将{0}列入黑名单，禁止聊天发言", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-unbanchat" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -unbanchat 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							Global.BanRoleChatToDBServer(text2, 0);
							BanChatManager.AddBanRoleName(text2, 0);
							text = string.Format("解除对于{0}的禁止聊天发言限制", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-ban_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -ban_rid 角色名称 禁止的分钟(例如1表示禁止1分钟内登陆)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1] + "$rid";
							int num40 = Global.SafeConvertToInt32(cmdFields[2]);
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							if (-1 != num8)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null != gameClient)
								{
									LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", text2), null, true);
									Global.ForceCloseClient(gameClient, "被GM踢了", true);
								}
							}
							Global.BanRoleNameToDBServer(text2, num40);
							text = string.Format("将{0}踢出了当前线路, 并禁止从任何线路再登陆", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-banu" == cmdFields[0])
				{
					if (cmdFields.Length >= 2)
					{
						BanManager.BanUserID2Memory(cmdFields[1]);
					}
				}
				else if ("-unbanu" == cmdFields[0])
				{
					if (cmdFields.Length >= 2)
					{
						BanManager.UnBanUserID2Memory(cmdFields[1]);
					}
				}
				else if ("-unban_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -unban_rid 角色ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1] + "$rid";
							Global.BanRoleNameToDBServer(text2, 0);
							text = string.Format("解除对于{0}的禁止登陆限制", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-banchat_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -banchat_rid 角色ID 几个小时", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1] + "$rid";
							int num41 = this.SafeConvertToInt32(cmdFields[2]);
							if (num41 > 0)
							{
								Global.BanRoleChatToDBServer(text2, num41);
								BanChatManager.AddBanRoleName(text2, num41);
							}
							text = string.Format("将{0}列入黑名单，禁止聊天发言", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-unbanchat_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -unbanchat_rid 角色ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1] + "$rid";
							Global.BanRoleChatToDBServer(text2, 0);
							BanChatManager.AddBanRoleName(text2, 0);
							text = string.Format("解除对于{0}的禁止聊天发言限制", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-recover" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -recover 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}恢复血和蓝...", text2), null, true);
							RoleRelifeLog roleRelifeLog = new RoleRelifeLog(gameClient.ClientData.RoleID, gameClient.ClientData.RoleName, gameClient.ClientData.MapCode, "GM命令");
							if (gameClient.ClientData.CurrentLifeV > 0)
							{
								bool flag = false;
								if (gameClient.ClientData.CurrentLifeV < gameClient.ClientData.LifeV)
								{
									flag = true;
									roleRelifeLog.hpModify = true;
									roleRelifeLog.oldHp = gameClient.ClientData.CurrentLifeV;
									int num42 = gameClient.ClientData.LifeV - gameClient.ClientData.CurrentLifeV;
									gameClient.ClientData.CurrentLifeV = gameClient.ClientData.LifeV;
									roleRelifeLog.newHp = gameClient.ClientData.CurrentLifeV;
								}
								if (gameClient.ClientData.CurrentMagicV < gameClient.ClientData.MagicV)
								{
									flag = true;
									roleRelifeLog.mpModify = true;
									roleRelifeLog.oldMp = gameClient.ClientData.CurrentMagicV;
									int num43 = gameClient.ClientData.MagicV - gameClient.ClientData.CurrentMagicV;
									gameClient.ClientData.CurrentMagicV = gameClient.ClientData.MagicV;
									roleRelifeLog.newMp = gameClient.ClientData.CurrentMagicV;
								}
								SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
								if (flag)
								{
									List<object> all9Clients2 = Global.GetAll9Clients(gameClient);
									GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.CopyMapID, gameClient.ClientData.RoleID, gameClient.ClientData.PosX, gameClient.ClientData.PosY, gameClient.ClientData.RoleDirection, (double)gameClient.ClientData.CurrentLifeV, (double)gameClient.ClientData.CurrentMagicV, 120, all9Clients2, 0);
								}
							}
							text = string.Format("为{0}恢复了HP和MP", text2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setpetai" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -setpetai AI模式(1 自由攻击, 2 攻击主人的目标)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int petAiControlType = this.SafeConvertToInt32(cmdFields[1]);
							Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
							if (null != monster)
							{
								monster.PetAiControlType = petAiControlType;
							}
						}
					}
				}
				else if ("-showpet" == cmdFields[0])
				{
					if (!transmit)
					{
						Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
						if (null == monster)
						{
							text = string.Format("请先召唤出您的召唤兽", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							text = string.Format("Defense={0},MDefense={1},MinAttack={2},MaxAttack={3}", new object[]
							{
								monster.MonsterInfo.Defense,
								monster.MonsterInfo.MDefense,
								monster.MonsterInfo.MinAttack,
								monster.MonsterInfo.MaxAttack
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							text = string.Format("VLifeMax={0},HitV={1},Dodge={2},SubAttackInjurePercent={3}", new object[]
							{
								monster.MonsterInfo.VLifeMax,
								monster.MonsterInfo.HitV,
								monster.MonsterInfo.Dodge,
								monster.MonsterInfo.MonsterSubAttackInjurePercent
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							text = string.Format("HolyAttack={0},HolyDefense={1}", monster.MonsterInfo.ExtProps[122], monster.MonsterInfo.ExtProps[123]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							text = string.Format("ShadowAttack={0},ShadowDefense={1}", monster.MonsterInfo.ExtProps[129], monster.MonsterInfo.ExtProps[130]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							text = string.Format("NatureAttack={0},NatureDefense={1}", monster.MonsterInfo.ExtProps[136], monster.MonsterInfo.ExtProps[137]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							text = string.Format("ChaosAttack={0},ChaosDefense={1}", monster.MonsterInfo.ExtProps[143], monster.MonsterInfo.ExtProps[144]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							text = string.Format("IncubusAttack={0},IncubusDefense={1}", monster.MonsterInfo.ExtProps[150], monster.MonsterInfo.ExtProps[151]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-testmode" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -testmode 压测选项(1-15,1 锁生命值,2 不限测试号,4 全体PK) 地图模式(可选,0-3,0 指定地图, 1 新手场景, 2 多主线地图, 3 剧情副本地图) 指定地图编号(可选,默认1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							GameManager.TestGamePerformanceMode = (this.SafeConvertToInt32(cmdFields[1]) > 0);
							GameManager.TestGamePerformanceForAllUser = ((this.SafeConvertToInt32(cmdFields[1]) & 2) > 0);
							GameManager.TestGamePerformanceAllPK = ((this.SafeConvertToInt32(cmdFields[1]) & 4) > 0);
							GameManager.TestGamePerformanceLockLifeV = ((this.SafeConvertToInt32(cmdFields[1]) & 8) > 0);
							int pkmode = GameManager.TestGamePerformanceAllPK ? 1 : 0;
							int num44 = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < num44; i++)
							{
								GameClient gameClient2 = GameManager.ClientMgr.FindClientByNid(i);
								if (gameClient2 != null && (GameManager.TestGamePerformanceForAllUser || gameClient2.strUserID == null || gameClient2.strUserID.StartsWith("mu")))
								{
									gameClient2.ClientData.PKMode = pkmode;
								}
							}
							if (cmdFields.Length > 2)
							{
								GameManager.TestGamePerformanceMapMode = this.SafeConvertToInt32(cmdFields[2]);
								if (cmdFields.Length > 3)
								{
									GameManager.TestGamePerformanceMapCode = this.SafeConvertToInt32(cmdFields[3]);
								}
							}
							do
							{
								Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, 1, 10000, 10000, 8000);
								if (GameManager.TestBirthPointList1.FindIndex((Point x) => x.X / 100.0 == newPos.X / 100.0 && x.Y / 100.0 == newPos.Y / 100.0) < 0)
								{
									GameManager.TestBirthPointList1.Add(newPos);
								}
							}
							while (GameManager.TestBirthPointList1.Count < 1000);
							do
							{
								Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, 2, 5000, 5000, 4000);
								if (GameManager.TestBirthPointList2.FindIndex((Point x) => x.X / 100.0 == newPos.X / 100.0 && x.Y / 100.0 == newPos.Y / 100.0) < 0)
								{
									GameManager.TestBirthPointList2.Add(newPos);
								}
							}
							while (GameManager.TestBirthPointList2.Count < 1000);
							text = string.Format("设置了压测模式 {0} {1}", GameManager.TestGamePerformanceMode, GameManager.TestGamePerformanceMapMode);
							LogManager.WriteLog(3, string.Format("根据GM({0})的要求设置压测模式：{1}", Global.FormatRoleName4(client), msgText), null, true);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-useworkpool" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -useworkpool 是否使用线程池(0/1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							text = string.Format("设置了命令处理模式是否使用线程池 {0}", false);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-manyattack" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -manyattack 是否使用多段攻击(0/1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							text = string.Format("多段攻击开启状态： {0}", true);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setmaxthread" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -setmaxthread 后台线程数 完成端口线程数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num45 = Global.SafeConvertToInt32(cmdFields[1]);
							int num46 = Global.SafeConvertToInt32(cmdFields[2]);
							ThreadPool.SetMaxThreads(num45, num46);
							text = string.Format("线程池最大线程数设置为：后台线程{0},完成端口线程{1}", num45, num46);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setlev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -setlev 角色名称 级别 转生次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							this.GMSetLevel(client, cmdFields);
						}
					}
				}
				else if ("-specprioritykf" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -specprioritykf TeQuanTiaoJian.xml的ID 次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int groupid = Global.SafeConvertToInt32(cmdFields[1]);
							int num47 = Global.SafeConvertToInt32(cmdFields[2]);
							SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
							if (null == specPriorityActivity)
							{
								return true;
							}
							List<SpecPConditionConfig> list3 = specPriorityActivity.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
							if (list3 == null || list3.Count == 0)
							{
								return true;
							}
							SpecPConditionConfig specPConditionConfig = list3.Find((SpecPConditionConfig x) => x.GroupID == groupid);
							if (null == specPConditionConfig)
							{
								return true;
							}
							if (specPriorityActivity.IfKFConditonType(specPConditionConfig.ConditionType))
							{
								lock (SpecPriorityActivity.Mutex)
								{
									specPriorityActivity.OnConditionNumChangeBefore();
									KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(groupid, num47);
									specPriorityActivity.ModifySpecialPriorityActConitionInfo(groupid, num47);
									specPriorityActivity.OnConditionNumChangeAfter();
								}
								text = string.Format("特权活动条件计数{0}添加了{1}", specPConditionConfig.ConditionType, num47);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if (cmdFields.Length >= 3)
					{
						int groupid = Global.SafeConvertToInt32(cmdFields[1]);
						int num47 = Global.SafeConvertToInt32(cmdFields[2]);
						SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
						if (null == specPriorityActivity)
						{
							return true;
						}
						List<SpecPConditionConfig> list3 = specPriorityActivity.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
						if (list3 == null || list3.Count == 0)
						{
							return true;
						}
						SpecPConditionConfig specPConditionConfig = list3.Find((SpecPConditionConfig x) => x.GroupID == groupid);
						if (null == specPConditionConfig)
						{
							return true;
						}
						if (specPriorityActivity.IfKFConditonType(specPConditionConfig.ConditionType))
						{
							lock (SpecPriorityActivity.Mutex)
							{
								specPriorityActivity.OnConditionNumChangeBefore();
								KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(groupid, num47);
								specPriorityActivity.ModifySpecialPriorityActConitionInfo(groupid, num47);
								specPriorityActivity.OnConditionNumChangeAfter();
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求特权活动条件计数{0}添加了{1}", specPConditionConfig.ConditionType, num47), null, true);
						}
					}
				}
				else if ("-specpriority" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -specpriority TeQuanTiaoJian.xml的ID 次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int groupid = Global.SafeConvertToInt32(cmdFields[1]);
							int num47 = Global.SafeConvertToInt32(cmdFields[2]);
							SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
							if (null == specPriorityActivity)
							{
								return true;
							}
							List<SpecPConditionConfig> list3 = specPriorityActivity.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
							if (list3 == null || list3.Count == 0)
							{
								return true;
							}
							SpecPConditionConfig specPConditionConfig = list3.Find((SpecPConditionConfig x) => x.GroupID == groupid);
							if (null == specPConditionConfig)
							{
								return true;
							}
							if (!specPriorityActivity.IfKFConditonType(specPConditionConfig.ConditionType))
							{
								lock (SpecPriorityActivity.Mutex)
								{
									specPriorityActivity.OnConditionNumChangeBefore();
									specPriorityActivity.ModifySpecialPriorityActConitionInfo(groupid, num47);
									specPriorityActivity.OnConditionNumChangeAfter();
								}
								text = string.Format("特权活动条件计数{0}添加了{1}", specPConditionConfig.ConditionType, num47);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if (cmdFields.Length >= 3)
					{
						int groupid = Global.SafeConvertToInt32(cmdFields[1]);
						int num47 = Global.SafeConvertToInt32(cmdFields[2]);
						SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
						if (null == specPriorityActivity)
						{
							return true;
						}
						List<SpecPConditionConfig> list3 = specPriorityActivity.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
						if (list3 == null || list3.Count == 0)
						{
							return true;
						}
						SpecPConditionConfig specPConditionConfig = list3.Find((SpecPConditionConfig x) => x.GroupID == groupid);
						if (null == specPConditionConfig)
						{
							return true;
						}
						if (!specPriorityActivity.IfKFConditonType(specPConditionConfig.ConditionType))
						{
							lock (SpecPriorityActivity.Mutex)
							{
								specPriorityActivity.OnConditionNumChangeBefore();
								specPriorityActivity.ModifySpecialPriorityActConitionInfo(groupid, num47);
								specPriorityActivity.OnConditionNumChangeAfter();
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求特权活动条件计数{0}添加了{1}", specPConditionConfig.ConditionType, num47), null, true);
						}
					}
				}
				else if ("-printrebornboss" == cmdFields[0])
				{
					if (!transmit)
					{
						RebornBoss.getInstance().PrintBossInfoGM(client, int.MaxValue, null);
					}
				}
				else if ("-fakerebornboss" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -fakerebornboss 虚拟数据条数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num48 = Global.SafeConvertToInt32(cmdFields[1]);
							RebornBoss.getInstance().BuildFakeBossInfoGM(client, num48);
							text = string.Format("重生Boss排行榜添加了{0}条假数据", num48);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setrebornlev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -setrebornlev 角色名称 级别 重生次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							this.GMSetRebornLevel(client, cmdFields);
						}
					}
				}
				else if ("-addexpmax" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -addexpmax 角色名称 经验类型(1怪 2回收) 经验(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							MoneyTypes types = (Global.SafeConvertToInt32(cmdFields[2]) == 1) ? MoneyTypes.RebornExpMonster : MoneyTypes.RebornExpSale;
							long num49 = Global.SafeConvertToInt64(cmdFields[3]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加重生经验上限{1}", text2, num49), null, true);
							GameManager.ClientMgr.ModifyRebornExpMaxAddValue(gameClient, num49, "GM道具脚本", types, false, true, false);
							text = string.Format("为{0}添加了重生经验上限{1}", text2, num49);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-addrebornexp" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -addrebornexp 角色名称 经验类型(1怪 2回收) 经验(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							MoneyTypes types = (Global.SafeConvertToInt32(cmdFields[2]) == 1) ? MoneyTypes.RebornExpMonster : MoneyTypes.RebornExpSale;
							long num49 = Global.SafeConvertToInt64(cmdFields[3]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加重生经验{1}", text2, num49), null, true);
							RebornManager.getInstance().ProcessRoleExperience(gameClient, num49, types, false, true, false, "none");
							text = string.Format("为{0}添加了重生经验{1}", text2, num49);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-clearguildmap" == cmdFields[0])
				{
					if (client.ClientData.Faction > 0)
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardFlag", 0, true);
						GameManager.GuildCopyMapDBMgr.ResetGuildCopyMapDB(client.ClientData.Faction, 0);
					}
				}
				else if ("-showallicon" == cmdFields[0])
				{
					GameManager.ClientMgr.SendToClient(client, "", 688);
				}
				else if ("-addexp" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -addexp 角色名称 经验(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							long num49 = Global.SafeConvertToInt64(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加经验{1}", text2, num49), null, true);
							GameManager.ClientMgr.ProcessRoleExperience(gameClient, num49, false, true, false, "none");
							text = string.Format("为{0}添加了经验{1}", text2, num49);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-addexp2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -addexp2 当前等级需要经验的百分数(最大100)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num50 = this.SafeConvertToInt32(cmdFields[1]);
							num50 = Math.Max(0, num50);
							num50 = Math.Min(100, num50);
							LogManager.WriteLog(3, string.Format("根据GM的要求为所有在线用户添加经验百分比{0}", num50), null, true);
							GameManager.ClientMgr.AddAllOnlieRoleExperience(num50);
							text = string.Format("为所有在线用户添加经验百分比{0}", num50);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							string text7 = string.Format("-addexp2 {0}", cmdFields[1]);
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								text7,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 2)
					{
						int num50 = this.SafeConvertToInt32(cmdFields[1]);
						num50 = Math.Max(0, num50);
						num50 = Math.Min(100, num50);
						LogManager.WriteLog(3, string.Format("根据GM的要求为所有在线用户添加经验百分比{0}", num50), null, true);
						GameManager.ClientMgr.AddAllOnlieRoleExperience(num50);
					}
				}
				else if ("-addipower" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -addipower 角色名称 内力(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num51 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加灵力{1}", text2, num51), null, true);
							if (num51 > 0)
							{
								GameManager.ClientMgr.AddInterPower(gameClient, num51, false, true);
							}
							else
							{
								GameManager.ClientMgr.SubInterPower(gameClient, -num51);
							}
							text = string.Format("为{0}添加了灵力{1}", text2, num51);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-addmoney" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -addmoney 角色名称 游戏币(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num52 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加游戏币{1}", text2, num52), null, true);
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, num52, "GM指令添加绑金", true);
							GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
							{
								gameClient.ClientData.RoleID,
								gameClient.ClientData.RoleName,
								gameClient.ClientData.Money1,
								num52
							}), EventLevels.Record);
							text = string.Format("为{0}添加了游戏币{1}", text2, num52);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string text2 = cmdFields[1];
						int num52 = this.SafeConvertToInt32(cmdFields[2]);
						int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
						if (-1 == num8)
						{
							return true;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
						if (null == gameClient)
						{
							return true;
						}
						LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加游戏币{1}", text2, num52), null, true);
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, num52, "GM指令添加绑金", true);
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							gameClient.ClientData.RoleID,
							gameClient.ClientData.RoleName,
							gameClient.ClientData.Money1,
							num52
						}), EventLevels.Record);
					}
				}
				else if ("-addyl" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -addlj 角色名称 银两(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num53 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加银两{1}", text2, num53), null, true);
							if (num53 >= 0)
							{
								GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num53), "GM指令添加", false);
							}
							else
							{
								GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num53), "GM指令扣除", false);
							}
							GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), YinLiang={2}, newYinLiang={3}", new object[]
							{
								gameClient.ClientData.RoleID,
								gameClient.ClientData.RoleName,
								gameClient.ClientData.YinLiang,
								num53
							}), EventLevels.Record);
							text = string.Format("为{0}添加了银两{1}", text2, num53);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string text2 = cmdFields[1];
						int num53 = this.SafeConvertToInt32(cmdFields[2]);
						int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
						if (-1 == num8)
						{
							return true;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
						if (null == gameClient)
						{
							return true;
						}
						LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加银两{1}", text2, num53), null, true);
						if (num53 >= 0)
						{
							GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num53), "GM指令添加", false);
						}
						else
						{
							GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num53), "GM指令扣除", false);
						}
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), YinLiang={2}, newYinLiang={3}", new object[]
						{
							gameClient.ClientData.RoleID,
							gameClient.ClientData.RoleName,
							gameClient.ClientData.YinLiang,
							num53
						}), EventLevels.Record);
					}
				}
				else if ("-addgold" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -addlj 角色名称 金币(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num54 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加金币{1}", text2, num54), null, true);
							if (num54 >= 0)
							{
								GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num54), "GM指令");
							}
							else
							{
								GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num54), "GM指令", false);
							}
							GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金币, roleID={0}({1}), Gold={2}, newGold={3}", new object[]
							{
								gameClient.ClientData.RoleID,
								gameClient.ClientData.RoleName,
								gameClient.ClientData.Gold,
								num54
							}), EventLevels.Record);
							text = string.Format("为{0}添加了金币{1}", text2, num54);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-adddj" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -adddj 角色名称 元宝(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num55 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加元宝{1}", text2, num55), null, true);
							if (num55 >= 0)
							{
								GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num55), "GM要求添加", ActivityTypes.None, "");
							}
							else
							{
								GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num55), "GM要求扣除", true, true, false, DaiBiSySType.None);
							}
							text = string.Format("为{0}添加了钻石{1}", text2, num55);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-additem" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 9)
						{
							text = string.Format("请输入： -additem 角色名称 物品名称 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							string text9 = cmdFields[2];
							int num32 = this.SafeConvertToInt32(cmdFields[3]);
							num32 = Global.GMax(0, num32);
							num32 = Global.GMin(int.MaxValue, num32);
							int num33 = this.SafeConvertToInt32(cmdFields[4]);
							int num34 = this.SafeConvertToInt32(cmdFields[5]);
							int num35 = this.SafeConvertToInt32(cmdFields[6]);
							int lucky = this.SafeConvertToInt32(cmdFields[7]);
							int num36 = this.SafeConvertToInt32(cmdFields[8]);
							int num31 = Global.GetGoodsByName(text9);
							if (-1 == num31)
							{
								text = string.Format("系统中不存在{0}", text9);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							num34 = Global.GMax(0, num34);
							num34 = Global.GMin(15, num34);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
							{
								text = string.Format("系统中不存在{0}", text9);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num37 = 0;
							int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
							if (intValue >= 800 && intValue < 816)
							{
								num37 = 3000;
							}
							if (systemXmlItem.GetIntValue("GridNum", -1) <= 1)
							{
								for (int i = 0; i < num32; i++)
								{
									if (!Global.CanAddGoods(gameClient, num31, 1, num33, "1900-01-01 12:00:00", true, false))
									{
										text = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											text2,
											i,
											text9,
											num32 - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
										return true;
									}
									LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
									{
										text2,
										text9,
										1,
										num34,
										0,
										num33
									}), null, true);
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, 1, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, null, 0, true);
								}
							}
							else
							{
								if (!Global.CanAddGoods(gameClient, num31, num32, num33, "1900-01-01 12:00:00", true, false))
								{
									text = string.Format("{0}背包已经满，无法添加{1}", text2, text9);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									text2,
									text9,
									num32,
									num34,
									0,
									num33
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, num32, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, null, 0, true);
							}
							text = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
							{
								text2,
								text9,
								num32,
								num34,
								0,
								num33
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-additem2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 7)
						{
							text = string.Format("请输入： -additem2 角色名称 物品名称 个数 绑定(0/1) 限制日期(2011-01-01) 限制时间(00$00$00)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							string text9 = cmdFields[2];
							int num32 = this.SafeConvertToInt32(cmdFields[3]);
							num32 = Global.GMax(0, num32);
							num32 = Global.GMin(int.MaxValue, num32);
							int num33 = this.SafeConvertToInt32(cmdFields[4]);
							string text10 = cmdFields[5];
							string text11 = cmdFields[6];
							string text12 = string.Format("{0} {1}", text10, text11);
							text12 = text12.Replace("$", ":");
							if (Global.DateTimeTicks(text12) <= 0L)
							{
								text = string.Format("限时格式错误{0} {1}", text10, text11);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num31 = Global.GetGoodsByName(text9);
							if (-1 == num31)
							{
								text = string.Format("系统中不存在{0}", text9);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int goodsCatetoriy = Global.GetGoodsCatetoriy(num31);
							if (goodsCatetoriy < 49 || (goodsCatetoriy >= 800 && goodsCatetoriy < 816))
							{
								text = string.Format("不能添加限时的{0}", text9);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
							{
								text = string.Format("系统中不存在{0}", text9);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							if (systemXmlItem.GetIntValue("GridNum", -1) <= 1)
							{
								for (int i = 0; i < num32; i++)
								{
									if (!Global.CanAddGoods(gameClient, num31, 1, num33, text12, true, false))
									{
										text = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											text2,
											i,
											text9,
											num32 - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
										return true;
									}
									LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}, 结束时间:{6}", new object[]
									{
										text2,
										text9,
										1,
										0,
										"白色",
										num33,
										text12
									}), null, true);
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, 1, 0, "", 0, num33, 0, "", true, 1, "GM添加", text12, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
								}
							}
							else
							{
								if (!Global.CanAddGoods(gameClient, num31, num32, num33, text12, true, false))
								{
									text = string.Format("{0}背包已经满，无法添加{1}", text2, text9);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}, 结束时间:{6}", new object[]
								{
									text2,
									text9,
									num32,
									0,
									"白色",
									num33,
									text12
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, num32, 0, "", 0, num33, 0, "", true, 1, "GM添加", text12, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
							}
							text = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}, 结束时间:{6} {7}", new object[]
							{
								text2,
								text9,
								num32,
								0,
								"白色",
								num33,
								text10,
								text11
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-addgoodpro" == cmdFields[0])
				{
					if (cmdFields.Length < 9)
					{
						text = string.Format("请输入： -addgoodpro 角色id 物品ID 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越 [洗练属性] [元素属性] [聚魂属性]", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
					}
					int num8 = this.SafeConvertToInt32(cmdFields[1]);
					int num31 = this.SafeConvertToInt32(cmdFields[2]);
					int num32 = this.SafeConvertToInt32(cmdFields[3]);
					num32 = Global.GMax(0, num32);
					num32 = Global.GMin(int.MaxValue, num32);
					int num33 = this.SafeConvertToInt32(cmdFields[4]);
					int num34 = this.SafeConvertToInt32(cmdFields[5]);
					int num35 = this.SafeConvertToInt32(cmdFields[6]);
					int lucky = this.SafeConvertToInt32(cmdFields[7]);
					int num36 = this.SafeConvertToInt32(cmdFields[8]);
					bool onLine = true;
					List<int> washProps = null;
					string text13 = "";
					if (cmdFields.Length > 9 && cmdFields[9] != "*")
					{
						text13 = cmdFields[9];
						byte[] array7 = Convert.FromBase64String(cmdFields[9]);
						washProps = DataHelper.BytesToObject<List<int>>(array7, 0, array7.Length);
					}
					List<int> elementhrtsProps = null;
					string text14 = "";
					if (cmdFields.Length > 10 && cmdFields[10] != "*")
					{
						text14 = cmdFields[10];
						byte[] array8 = Convert.FromBase64String(cmdFields[10]);
						elementhrtsProps = DataHelper.BytesToObject<List<int>>(array8, 0, array8.Length);
					}
					int num56 = 0;
					if (cmdFields.Length > 11 && cmdFields[11] != "*")
					{
						num56 = this.SafeConvertToInt32(cmdFields[11]);
					}
					int num57 = 0;
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
					{
						text = string.Format("系统中不存在{0}", num31);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						return true;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
					if (null == gameClient)
					{
						RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, num8), 0);
						if (null == roleDataEx)
						{
							LogManager.WriteLog(2, "添加角色道具，但是查不到角色数据。", null, true);
							return true;
						}
						gameClient = new GameClient
						{
							ClientData = new SafeClientData
							{
								RoleData = roleDataEx
							}
						};
						onLine = false;
					}
					int num37 = 0;
					int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
					if (intValue >= 800 && intValue < 816)
					{
						num37 = 3000;
					}
					else if (intValue == 901)
					{
						num37 = 7000;
					}
					else if (intValue >= 910 && intValue <= 928)
					{
						num37 = 8000;
					}
					else if (intValue == 940)
					{
						num37 = 11000;
					}
					else if (intValue >= 980 && intValue <= 981)
					{
						num37 = 16000;
					}
					if (intValue != 9 && intValue != 10)
					{
						num34 = Global.GMax(0, num34);
						num34 = Global.GMin(20, num34);
					}
					string text9 = systemXmlItem.GetStringValue("Title");
					if (systemXmlItem.GetIntValue("GridNum", -1) <= 1)
					{
						for (int i = 0; i < num32; i++)
						{
							if (!Global.CanAddGoods(gameClient, num31, 1, num33, "1900-01-01 12:00:00", true, false))
							{
								text = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
								{
									num8,
									i,
									text9,
									num32 - i
								});
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5} 洗练:{6} 元素:{7} 聚魂:{8}", new object[]
							{
								num8,
								text9,
								1,
								num34,
								num57,
								num33,
								text13,
								text14,
								num56
							}), null, true);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, 1, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, washProps, elementhrtsProps, num56, onLine);
						}
					}
					else
					{
						if (!Global.CanAddGoods(gameClient, num31, num32, num33, "1900-01-01 12:00:00", true, false))
						{
							text = string.Format("{0}背包已经满，无法添加{1}", num8, text9);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							return true;
						}
						LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5} 洗练:{6} 元素:{7}", new object[]
						{
							num8,
							text9,
							num32,
							num34,
							num57,
							num33,
							text13,
							text14
						}), null, true);
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, num32, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, washProps, elementhrtsProps, num56, onLine);
					}
					text = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5} 洗练:{6} 元素:{7}", new object[]
					{
						num8,
						text9,
						num32,
						num34,
						num57,
						num33,
						text13,
						text14
					});
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
				}
				else if ("-addid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 9)
						{
							text = string.Format("请输入： -addid 角色名称 物品ID 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num31 = this.SafeConvertToInt32(cmdFields[2]);
							int num32 = this.SafeConvertToInt32(cmdFields[3]);
							num32 = Global.GMax(0, num32);
							num32 = Global.GMin(int.MaxValue, num32);
							int num33 = this.SafeConvertToInt32(cmdFields[4]);
							int num34 = this.SafeConvertToInt32(cmdFields[5]);
							int num35 = this.SafeConvertToInt32(cmdFields[6]);
							int lucky = this.SafeConvertToInt32(cmdFields[7]);
							int num36 = this.SafeConvertToInt32(cmdFields[8]);
							num34 = Global.GMax(0, num34);
							num34 = Global.GMin(20, num34);
							int num57 = 0;
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
							{
								text = string.Format("系统中不存在{0}", num31);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num37 = 0;
							int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
							if (intValue >= 800 && intValue < 816)
							{
								num37 = 3000;
							}
							else if (intValue == 901)
							{
								num37 = 7000;
							}
							else if (intValue >= 910 && intValue <= 928)
							{
								num37 = 8000;
							}
							else if (intValue == 940)
							{
								num37 = 11000;
							}
							else if (intValue >= 980 && intValue <= 981)
							{
								num37 = 16000;
							}
							string text9 = systemXmlItem.GetStringValue("Title");
							if (systemXmlItem.GetIntValue("GridNum", -1) <= 1)
							{
								for (int i = 0; i < num32; i++)
								{
									if (!Global.CanAddGoods(gameClient, num31, 1, num33, "1900-01-01 12:00:00", true, false) && !RebornEquip.IsRebornType(num31))
									{
										text = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											text2,
											i,
											text9,
											num32 - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
										return true;
									}
									if (!RebornEquip.CanAddGoodsDataList2(gameClient, num31, 1, num33, "1900-01-01 12:00:00", true) && RebornEquip.IsRebornType(num31))
									{
										text = string.Format("{0}重生背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											text2,
											i,
											text9,
											num32 - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
										return true;
									}
									LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
									{
										text2,
										text9,
										1,
										num34,
										num57,
										num33
									}), null, true);
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, 1, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, null, 0, true);
								}
							}
							else
							{
								if (!Global.CanAddGoods(gameClient, num31, num32, num33, "1900-01-01 12:00:00", true, false) && !RebornEquip.IsRebornType(num31))
								{
									text = string.Format("{0}背包已经满，无法添加{1}", text2, text9);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								if (!RebornEquip.CanAddGoodsDataList2(gameClient, num31, 1, num33, "1900-01-01 12:00:00", true) && RebornEquip.IsRebornType(num31))
								{
									text = string.Format("{0}重生背包已经满，无法添加{1}", text2, text9);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									text2,
									text9,
									num32,
									num34,
									num57,
									num33
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, num32, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, null, 0, true);
							}
							text = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
							{
								text2,
								text9,
								num32,
								num34,
								num57,
								num33
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if (cmdFields.Length >= 9)
					{
						string text2 = cmdFields[1];
						int num31 = this.SafeConvertToInt32(cmdFields[2]);
						int num32 = this.SafeConvertToInt32(cmdFields[3]);
						num32 = Global.GMax(0, num32);
						num32 = Global.GMin(int.MaxValue, num32);
						int num33 = this.SafeConvertToInt32(cmdFields[4]);
						int num34 = this.SafeConvertToInt32(cmdFields[5]);
						int num35 = this.SafeConvertToInt32(cmdFields[6]);
						int lucky = this.SafeConvertToInt32(cmdFields[7]);
						int num36 = this.SafeConvertToInt32(cmdFields[8]);
						num34 = Global.GMax(0, num34);
						num34 = Global.GMin(20, num34);
						int num57 = 0;
						int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
						if (-1 == num8)
						{
							text = string.Format("根据GM的要求为{0}添加物品,目标不在线", text2);
							LogManager.WriteLog(3, text, null, true);
							return true;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
						if (null == gameClient)
						{
							text = string.Format("根据GM的要求为{0}添加物品,目标不在线", text2);
							LogManager.WriteLog(3, text, null, true);
							return true;
						}
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
						{
							text = string.Format("根据GM的要求为{1}添加物品,但系统中不存在{0}", num31, text2);
							LogManager.WriteLog(3, text, null, true);
							return true;
						}
						int num37 = 0;
						int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
						if (intValue >= 800 && intValue < 816)
						{
							num37 = 3000;
						}
						string text9 = systemXmlItem.GetStringValue("Title");
						if (systemXmlItem.GetIntValue("GridNum", -1) <= 1)
						{
							for (int i = 0; i < num32; i++)
							{
								if (!Global.CanAddGoods(gameClient, num31, 1, num33, "1900-01-01 12:00:00", true, false))
								{
									text = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{3}个无法添加{1}", new object[]
									{
										text2,
										i,
										text9,
										num32 - i
									});
									LogManager.WriteLog(3, text, null, true);
									return true;
								}
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									text2,
									text9,
									1,
									num34,
									num57,
									num33
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, 1, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, null, 0, true);
							}
						}
						else
						{
							if (!Global.CanAddGoods(gameClient, num31, num32, num33, "1900-01-01 12:00:00", true, false))
							{
								text = string.Format("{0}背包已经满，无法添加{1}", text2, text9);
								LogManager.WriteLog(3, text, null, true);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
							{
								text2,
								text9,
								num32,
								num34,
								num57,
								num33
							}), null, true);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, num31, num32, 0, "", num34, num33, num37, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, num36, num35, 0, null, null, 0, true);
						}
					}
				}
				else if ("-setpkv" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -setpkv 角色名称 pk值(最小值0) pk点(最小值0)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num58 = this.SafeConvertToInt32(cmdFields[2]);
							int num59 = this.SafeConvertToInt32(cmdFields[3]);
							num58 = Global.GMax(0, num58);
							num59 = Global.GMax(0, num59);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置PK值{1}", text2, num58), null, true);
							GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num58, num59, true);
							Global.ProcessRedNamePunishForDebuff(gameClient);
							text = string.Format("为{0}设置了PK值{1}", text2, num58);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-adddjpoint" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -adddjpoint 角色名称 点将积分(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num60 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}添加点将积分{1}", text2, num60), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10023, string.Format("{0}:{1}", gameClient.ClientData.RoleID, num60), null, gameClient.ServerId);
							text = string.Format("为{0}添加点将积分{1}", text2, num60);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setmaintask" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -setmaintask RoleName TaskID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num61 = this.SafeConvertToInt32(cmdFields[2]);
							num61 = Global.GMax(0, num61);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								num8 = Global.SafeConvertToInt32(text2);
								if (num8 < 200000)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置主线任务ID为{1}", text2, num61), null, true);
							ProcessTask.GMSetMainTaskID(gameClient, num61);
							SingletonTemplate<GuardStatueManager>.Instance().OnTaskComplete(gameClient);
							text = string.Format("为{0}设置主线任务ID为{1}", text2, num61);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-settaskv" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							text = string.Format("请输入： -settaskv 角色名称 任务名称 目标类型(1/2) 数值(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							string text15 = cmdFields[2];
							int num62 = this.SafeConvertToInt32(cmdFields[3]);
							num62 = Global.GMax(1, num62);
							num62 = Global.GMin(2, num62);
							int num61 = this.SafeConvertToInt32(cmdFields[4]);
							num61 = Global.GMax(0, num61);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置任务{1}, 值类型:{2}, 任务值:{3}", new object[]
							{
								text2,
								text15,
								num62,
								num61
							}), null, true);
							ProcessTask.ProcessTaskValue(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, text15, num62, num61);
							text = string.Format("为{0}设置任务{1}, 值类型{2}, 任务值{3}", new object[]
							{
								text2,
								text15,
								num62,
								num61
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-shutdown" == cmdFields[0])
				{
					if (!transmit)
					{
						LogManager.WriteLog(3, string.Format("根据GM的要求为关闭服务器: {0}", TimeUtil.NowDateTime()), null, true);
						Program.Exit();
					}
				}
				else if ("-auth" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -auth 公告开关(0/1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num63 = this.SafeConvertToInt32(cmdFields[1]);
							LogManager.WriteLog(3, string.Format("根据GM的切换授权开关: {0}", num63), null, true);
							GameManager.ClientMgr.NotifyGMAuthCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num63);
						}
					}
				}
				else if ("-bull" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 6)
						{
							text = string.Format("请输入： -bull 公告ID(文字和数字都可以) 开始时间(yyyy-MM-dd_HH&mm&ss) 结束时间(yyyy-MM-dd_HH&mm&ss) 间隔(数字，单位秒) 公告文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text16 = cmdFields[1].Trim();
							string text17 = cmdFields[2].Trim().Replace('&', ':').Replace('_', ' ');
							string text18 = cmdFields[3].Trim().Replace('&', ':').Replace('_', ' ');
							int num64 = this.SafeConvertToInt32(cmdFields[4]);
							string text19 = cmdFields[5];
							if (string.IsNullOrEmpty(text16))
							{
								text = string.Format("公告ID不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求发布公告: {0} {1} {2} {3} {4}", new object[]
							{
								text16,
								text17,
								text18,
								num64,
								text19
							}), null, true);
							GameManager.BulletinMsgMgr.AddBulletinMsgBackground(text16, text17, text18, num64, text19);
							string text7 = string.Format("-bull {0} {1} {2} {3} {4}", new object[]
							{
								text16,
								cmdFields[2],
								cmdFields[3],
								num64,
								text19
							});
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								text7,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 6)
					{
						string text16 = cmdFields[1].Trim();
						string text17 = cmdFields[2].Trim().Replace('&', ':').Replace('_', ' ');
						string text18 = cmdFields[3].Trim().Replace('&', ':').Replace('_', ' ');
						int num64 = this.SafeConvertToInt32(cmdFields[4]);
						string text19 = cmdFields[5];
						GameManager.BulletinMsgMgr.AddBulletinMsgBackground(text16, text17, text18, num64, text19);
					}
				}
				else if ("-rmbull" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -rmbull 公告ID(文字和数字都可以)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text16 = cmdFields[1].Trim();
							if (string.IsNullOrEmpty(text16))
							{
								text = string.Format("公告ID不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求删除公告: {0}", text16), null, true);
							BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.RemoveBulletinMsg(text16);
							if (null != bulletinMsgData)
							{
								string text7 = string.Format("-rmbull {0}", text16);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									client.ClientData.RoleID,
									"",
									0,
									"",
									0,
									text7,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
							}
						}
					}
					else if (cmdFields.Length == 2)
					{
						string text16 = cmdFields[1].Trim();
						GameManager.BulletinMsgMgr.RemoveBulletinMsg(text16);
					}
				}
				else if ("-listbull" == cmdFields[0])
				{
					if (!transmit)
					{
						LogManager.WriteLog(3, string.Format("根据GM的要求列举公告", new object[0]), null, true);
						GameManager.BulletinMsgMgr.SendAllBulletinMsgToGM(client);
					}
				}
				else if ("-sysmsg" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -sysmsg 临时公告ID(文字和数字都可以) 临时公告文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text16 = cmdFields[1].Trim();
							int num40 = 0;
							int num65 = 1;
							string text19 = cmdFields[2];
							if (string.IsNullOrEmpty(text16))
							{
								text = string.Format("临时公告ID不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求发布临时公告: {0} {1} {2} {3}", new object[]
							{
								text16,
								num40,
								num65,
								text19
							}), null, true);
							BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(text16, num40, num65, text19, 1);
							GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData, 0, 0);
							string text7 = string.Format("-sysmsg {0} {1} {2} {3}", new object[]
							{
								text16,
								num40,
								num65,
								text19
							});
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								text7,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string text16 = cmdFields[1].Trim();
						int num40 = 0;
						int num65 = 1;
						string text19 = cmdFields[2];
						BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(text16, num40, num65, text19, 1);
						GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData, 0, 0);
					}
				}
				else if ("-hintmsg" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -hintmsg 消息类型 显示类型 提示文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num66 = this.SafeConvertToInt32(cmdFields[1]);
							int num67 = this.SafeConvertToInt32(cmdFields[2]);
							string text20 = cmdFields[3];
							LogManager.WriteLog(3, string.Format("根据GM的要求发布提示消息: {0} {1} {2}", num66, num67, text20), null, true);
							GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, text20, (GameInfoTypeIndexes)num66, (ShowGameInfoTypes)num67, 0, 0, 0, 100, 100);
							string text7 = string.Format("-hintmsg {0} {1} {2}", num66, num67, text20);
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								text7,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 4)
					{
						int num66 = this.SafeConvertToInt32(cmdFields[1]);
						int num67 = this.SafeConvertToInt32(cmdFields[2]);
						string text20 = cmdFields[3];
						GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, text20, (GameInfoTypeIndexes)num66, (ShowGameInfoTypes)num67, 0, 0, 0, 100, 100);
					}
				}
				else if ("-hintmsg2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							text = string.Format("请输入： -hintmsg2 帮会ID 消息类型 显示类型 提示文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							int num68 = this.SafeConvertToInt32(cmdFields[1]);
							int num66 = this.SafeConvertToInt32(cmdFields[2]);
							int num67 = this.SafeConvertToInt32(cmdFields[3]);
							string text20 = cmdFields[4];
							LogManager.WriteLog(3, string.Format("根据GM的要求发布帮会提示消息: {0} {1} {2} {3}", new object[]
							{
								num68,
								num66,
								num67,
								text20
							}), null, true);
							string text7 = string.Format("-hintmsg2 {0} {1} {2} {3}", new object[]
							{
								num68,
								num66,
								num67,
								text20
							});
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								text7,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 5)
					{
						int num68 = this.SafeConvertToInt32(cmdFields[1]);
						int num66 = this.SafeConvertToInt32(cmdFields[2]);
						int num67 = this.SafeConvertToInt32(cmdFields[3]);
						string text20 = cmdFields[4];
						GameManager.ClientMgr.NotifyBangHuiImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, num68, text20, (GameInfoTypeIndexes)num66, (ShowGameInfoTypes)num67, 0);
					}
				}
				else if ("-zp" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("if" == cmdFields[1])
					{
						ZhuanPanManager.getInstance().ProcessZhuanPanInfoCmd(client, 1810, null, cmdFields);
					}
					else if ("cj" == cmdFields[1])
					{
						string[] cmdParams = new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2]
						};
						ZhuanPanManager.getInstance().ProcessZhuanPanChouJiangCmd(client, 1811, null, cmdParams);
					}
				}
				else if ("-sq" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("1" == cmdFields[1])
					{
						string[] cmdParams = new string[]
						{
							client.ClientData.RoleID.ToString()
						};
						ShenQiManager.getInstance().ProcessShenQiInfoCmd(client, 1816, null, cmdParams);
					}
					else if ("2" == cmdFields[1])
					{
						string[] cmdParams = new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2]
						};
						ShenQiManager.getInstance().ProcessShenQiUpCmd(client, 1817, null, cmdParams);
					}
					else if ("3" == cmdFields[1])
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, Convert.ToInt32(cmdFields[2]), "GM添加", true, true);
					}
					else if ("4" == cmdFields[1])
					{
						ShenQiData shenQiData = new ShenQiData
						{
							ShenQiID = Convert.ToInt32(cmdFields[2])
						};
						client.ClientData.shenQiData = shenQiData;
						List<int> list4 = new List<int>();
						list4.AddRange(new int[]
						{
							shenQiData.ShenQiID,
							shenQiData.LifeAdd,
							shenQiData.AttackAdd,
							shenQiData.DefenseAdd,
							shenQiData.ToughnessAdd
						});
						Global.SaveRoleParamsIntListToDB(client, list4, "36", true);
						ShenQiManager.getInstance().UpdateRoleShenQiProps(client);
						ShenQiManager.getInstance().UpdateRoleTouhgnessProps(client);
						ShenQiManager.getInstance().UpdateRoleGodProps(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
				else if ("-caiji" == cmdFields[0])
				{
					if ("3" == cmdFields[1])
					{
						string[] cmdParams = new string[]
						{
							client.ClientData.RoleID.ToString()
						};
						LingDiCaiJiManager.getInstance().ProcessLingZhuSetDoubleOpenCmd(client, 1832, null, cmdParams);
					}
					else if ("4" == cmdFields[1])
					{
						string[] cmdParams = new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2]
						};
						LingDiCaiJiManager.getInstance().ProcessLingDiEnterCmd(client, 1829, null, cmdParams);
					}
					else if ("5" == cmdFields[1])
					{
						if (cmdFields.Length < 4)
						{
							return true;
						}
						int num69 = Convert.ToInt32(cmdFields[3]);
						if (num69 == 1 && client.ClientData.JunTuanId > 0)
						{
							LingDiCaiJiManager.getInstance().SetLingZhu(Convert.ToInt32(cmdFields[2]), client.ClientData.RoleID, client.ClientData.JunTuanId, client.ClientData.JunTuanName, null);
						}
						else
						{
							LingDiCaiJiManager.getInstance().SetLingZhu(Convert.ToInt32(cmdFields[2]), 0, 0, "", null);
						}
					}
					else if ("6" == cmdFields[1])
					{
						if (cmdFields.Length < 3)
						{
							return true;
						}
						int num70 = Convert.ToInt32(cmdFields[2]);
						if (num70 < 0 || num70 > 1)
						{
							return true;
						}
						int lingDiRoleNum = LingDiCaiJiManager.getInstance().GetLingDiRoleNum(num70);
						string arg = (num70 == 0) ? "地宫人数：" : "荒漠人数：";
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, arg + lingDiRoleNum);
					}
					else if ("sync" == cmdFields[1])
					{
						LingDiCaiJiManager.getInstance().SetSync();
					}
				}
				else if ("-nlhx" == cmdFields[0])
				{
					int addType = Convert.ToInt32(cmdFields[1]);
					int addValue6 = Convert.ToInt32(cmdFields[2]);
					BuildingManager.getInstance().ModifyNengLiangPointsValue(client, addType, addValue6, "GM修改", true, true);
				}
				else if ("-jx" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("mohua" == cmdFields[1])
					{
						if (cmdFields.Length < 4)
						{
							return true;
						}
						int num71 = Math.Max(Convert.ToInt32(cmdFields[2]) - 1, 0);
						int num72 = Convert.ToInt32(cmdFields[3]);
						int num73 = num71 * 11 + num72;
						AwakenLevelItem awakenLevelItem;
						if (!JueXingManager.getInstance().JueXingRunTimeData.AwakenLevelDict.TryGetValue(num73, out awakenLevelItem))
						{
							return true;
						}
						Global.SaveRoleParamsInt32ValueToDB(client, "10193", num73, true);
						client.ClientData.JueXingData.JueXingJie = awakenLevelItem.Order;
						client.ClientData.JueXingData.JueXingJi = awakenLevelItem.Star;
						JueXingManager.getInstance().UpdataPalyerJueXingAttr(client, true);
					}
				}
				else if ("-dacaiji" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("cl" == cmdFields[1])
					{
						if (cmdFields.Length < 3)
						{
							return true;
						}
						client.ClientData.CurrentLifeV = Convert.ToInt32((double)client.ClientData.LifeV * Convert.ToDouble(cmdFields[2]));
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					else if ("al" == cmdFields[1])
					{
						List<Monster> list5 = GameManager.MonsterMgr.FindMonsterAll(client.ClientData.MapCode);
						foreach (Monster monster2 in list5)
						{
							if (cmdFields.Length < 3)
							{
								return true;
							}
							monster2.VLife = (double)Convert.ToInt32(monster2.MonsterInfo.VLifeMax * Convert.ToDouble(cmdFields[2]));
							ClientManager.NotifySelfEnemyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, monster2.RoleID, 0, 0, monster2.VLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					else if ("ca" == cmdFields[1])
					{
						for (int i = 177; i > 10; i--)
						{
							if (i != 18 && i != 13 && i != 15)
							{
								client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
								{
									PropsSystemTypes.GM_Temp_Props,
									i,
									0.0 - RoleAlgorithm.GetExtProp(client, i)
								});
							}
						}
						for (int i = 10; i > 6; i--)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								PropsSystemTypes.GM_Temp_Props,
								i,
								1000.0 - RoleAlgorithm.GetExtProp(client, i)
							});
						}
						for (int i = 6; i > 2; i--)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								PropsSystemTypes.GM_Temp_Props,
								i,
								0.0 - RoleAlgorithm.GetExtProp(client, i)
							});
						}
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							PropsSystemTypes.GM_Temp_Props,
							13,
							1000000.0 - RoleAlgorithm.GetExtProp(client, 13)
						});
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					else if ("0" == cmdFields[1])
					{
						if (cmdFields.Length < 4)
						{
							return true;
						}
						ExtPropIndexes extPropIndexes = (ExtPropIndexes)Global.SafeConvertToInt32(cmdFields[2]);
						double num74 = Convert.ToDouble(cmdFields[3]);
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							PropsSystemTypes.GM_Temp_Props,
							(int)extPropIndexes,
							num74 - RoleAlgorithm.GetExtProp(client, (int)extPropIndexes)
						});
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
				else if ("-config" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -config 参数名称 参数值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text21 = cmdFields[1].Trim();
							string text22 = cmdFields[2].Trim();
							if (string.IsNullOrEmpty(text21))
							{
								text = string.Format("参数名称不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							if (string.IsNullOrEmpty(text22))
							{
								text = string.Format("参数值不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求更改游戏参数: {0}=>{1}", text21, text22), null, true);
							Global.UpdateDBGameConfigg(text21, text22);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string text21 = cmdFields[1].Trim();
						string text22 = cmdFields[2].Trim();
						if (string.IsNullOrEmpty(text21))
						{
							return true;
						}
						if (string.IsNullOrEmpty(text22))
						{
							return true;
						}
						if ("qinggongyan_joincount" == text21 || "qinggongyan_joinmoney" == text21 || "vip_fullpurchase" == text21 || "everydayact" == text21 || "bhmatch_goldjoin" == text21 || "era_rank_award" == text21 || "czfl_fullpurnum" == text21 || string.Compare("comp_monster_", 0, text21, 0, "comp_monster_".Length) == 0 || string.Compare("reborn_boss_", 0, text21, 0, "reborn_boss_".Length) == 0 || "specpact" == text21 || "ZorkAwardSeasonID" == text21)
						{
							return true;
						}
						GameManager.GameConfigMgr.SetGameConfigItem(text21, text22);
						GameManager.ServerMonitor.SetNeedReload();
						int index = 0;
						GameClient nextClient;
						while ((nextClient = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
						{
							nextClient.sendCmd(1842, text21 + ":" + text22, false);
						}
						if ("kaifutime" == text21)
						{
							ReloadXmlManager.ReloadAllXmlFile();
						}
						else if ("userbegintime" == text21)
						{
							UserReturnManager.getInstance().UpdateUserReturnState();
						}
						else if ("jieristartday" == text21)
						{
							ReloadXmlManager.ReloadAllXmlFile();
						}
						else if ("hefutime" == text21)
						{
							ReloadXmlManager.ReloadAllXmlFile();
						}
						else if ("yueduchoujiangstartday" == text21)
						{
							HuodongCachingMgr.ResetYueDuZhuanPanActivity();
						}
						else if ("whiteiplist" == text21)
						{
							Program.LoadIPList(text22);
						}
						else if ("nochecktime" == text21)
						{
							if (text22 == "1")
							{
								GameManager.GM_NoCheckTokenTimeRemainMS = 3600000L;
							}
							else
							{
								GameManager.GM_NoCheckTokenTimeRemainMS = 0L;
							}
						}
						else if ("lixianguaji" == text21)
						{
							GameManager.FlagLiXianGuaJi = Global.SafeConvertToInt32(text22);
						}
						else if ("optimization_bag_reset" == text21)
						{
							GameManager.Flag_OptimizationBagReset = (Global.SafeConvertToInt32(text22) > 0);
						}
						else if ("checkservertime" == text21)
						{
							GameManager.ConstCheckServerTimeDiffMinutes = Global.SafeConvertToInt32(text22);
						}
						else if ("hideflags" == text21)
						{
							if (cmdFields.Length >= 4)
							{
								bool enable = Global.SafeConvertToInt32(text22) > 0;
								int num10 = Global.SafeConvertToInt32(cmdFields[3].Trim());
								GameManager.ResetHideFlagsMaps(enable, num10);
							}
						}
						else if ("maxposcmdnum" == text21)
						{
							TCPSession.SetMaxPosCmdNumPer5Seconds(10);
						}
						else if ("maxsubticks" == text21)
						{
							TCPSession.MaxAntiProcessJiaSuSubTicks = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubticks", 1000);
						}
						else if ("maxsubnum" == text21)
						{
							TCPSession.MaxAntiProcessJiaSuSubNum = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubnum", 3);
						}
						else if ("loginwebkey" == text21)
						{
							if (!string.IsNullOrEmpty(text22) && text22.Length >= 5)
							{
								TCPCmdHandler.WebKey = text22;
							}
							else
							{
								TCPCmdHandler.WebKey = TCPCmdHandler.WebKeyLocal;
							}
						}
						else if ("userwaitconfig" == text21 || "vipwaitconfig" == text21)
						{
							GameManager.loginWaitLogic.LoadConfig();
						}
						GameManager.LoadGameConfigFlags();
					}
					if (cmdFields.Length >= 3)
					{
						string text21 = cmdFields[1].Trim();
						string text22 = cmdFields[2].Trim();
						if ("logflags" == text21)
						{
							if (cmdFields.Length >= 3)
							{
								GameManager.SetLogFlags(Global.SafeConvertToInt64(text22));
								GameManager.GameConfigMgr.UpdateGameConfigItem(text21, text22, true);
							}
						}
					}
				}
				else if ("-listconfig" == cmdFields[0])
				{
					if (!transmit)
					{
						LogManager.WriteLog(3, string.Format("根据GM的要求列举游戏参数", new object[0]), null, true);
						GameManager.GameConfigMgr.SendAllGameConfigItemsToGM(client);
					}
				}
				else if ("-holdqinggongyan" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						GameManager.QingGongYanMgr.HoldQingGongYan(client, Convert.ToInt32(cmdFields[1]), 0);
					}
				}
				else if ("-joinqinggongyan" == cmdFields[0])
				{
					GameManager.QingGongYanMgr.JoinQingGongYan(client);
				}
				else if ("-qingkongpet" == cmdFields[0])
				{
					List<GoodsData> list = client.ClientData.PetList;
					for (int i = list.Count - 1; i >= 0; i--)
					{
						GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, list[i], 1, false, false);
					}
				}
				else if ("-callpet" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						string text23 = "";
						CallPetManager.CallPet(client, Convert.ToInt32(cmdFields[1]), out text23);
					}
				}
				else if ("-movepet" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						CallPetManager.MovePet(client, Convert.ToInt32(cmdFields[1]));
					}
				}
				else if ("-setjmrate" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -setjmrate 角色名称 冲穴成功率倍数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num75 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置冲穴的成功率倍数{1}", text2, num75), null, true);
							gameClient.ClientData.TempJMChongXueRate = num75;
							text = string.Format("为{0}设置冲穴成功率倍数{1}", text2, num75);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-sethrate1" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -sethrate1 角色名称 坐骑强化成功率倍数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num76 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置坐骑强化的成功率倍数{1}", text2, num76), null, true);
							gameClient.ClientData.TempHorseEnchanceRate = num76;
							text = string.Format("为{0}设置坐骑强化成功率倍数{1}", text2, num76);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-sethrate2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -sethrate1 角色名称 坐骑进阶成功率倍数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num76 = this.SafeConvertToInt32(cmdFields[2]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置坐骑进阶的成功率倍数{1}", text2, num76), null, true);
							gameClient.ClientData.TempHorseEnchanceRate = num76;
							text = string.Format("为{0}设置坐骑进阶成功率倍数{1}", text2, num76);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setjmlev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -setjmlev 角色名称 经脉ID 穴位ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num77 = this.SafeConvertToInt32(cmdFields[2]);
							int num78 = this.SafeConvertToInt32(cmdFields[3]);
							if (num77 < 0 || num77 >= 8)
							{
								text = string.Format("经脉的ID{0}超过了范围限制, 应该是:{1}-{2}", num77, 0, 7);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							num78 = Global.GMax(0, num78);
							num78 = Global.GMin(Global.MaxJingMaiLevel, num78);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置经脉{1}的穴位为{2}", text2, Global.GetJingMaiName(num77), num78), null, true);
							Global.UpdateJingMaiListProps(gameClient, false);
							int jingMaiBodyLevel = gameClient.ClientData.JingMaiBodyLevel;
							int num79 = Global.ProcessUpJingmaiLevel(gameClient, jingMaiBodyLevel, num77, ref num78, 0);
							Global.UpdateJingMaiListProps(gameClient, true);
							if (num79 > 0)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
								GameManager.ClientMgr.NotifyJingMaiInfoCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
							}
							GameManager.ClientMgr.NotifyJingMaiResult(gameClient, num79, num77, num78);
							text = string.Format("为{0}设置经脉{1}的穴位为{2}", text2, Global.GetJingMaiName(num77), num78);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if ("-setskilllev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -setskilllev 角色名称 技能ID 级别", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							string text2 = cmdFields[1];
							int num80 = this.SafeConvertToInt32(cmdFields[2]);
							int num81 = this.SafeConvertToInt32(cmdFields[3]);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							SystemXmlItem systemXmlItem2 = null;
							if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(num80, out systemXmlItem2))
							{
								text = string.Format("技能ID{0}在系统中不存在", num80);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							num81 = Global.GMax(0, num81);
							num81 = Global.GMin(systemXmlItem2.GetIntValue("MaxLevel", -1), num81);
							string stringValue = systemXmlItem2.GetStringValue("Name");
							SkillData skillDataByID = Global.GetSkillDataByID(gameClient, num80);
							if (null == skillDataByID)
							{
								text = string.Format("{0}尚未学习{1}技能", text2, stringValue);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置技能{1}的级别为{2}", text2, stringValue, num81), null, true);
							skillDataByID.SkillLevel = num81;
							GameManager.ClientMgr.UpdateSkillInfo(client, skillDataByID, true);
							if (systemXmlItem2.GetIntValue("MagicType", -1) < 0)
							{
								Global.RefreshSkillForeverProps(gameClient);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
							}
							string text5 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								0,
								gameClient.ClientData.RoleID,
								skillDataByID.DbID,
								skillDataByID.SkillLevel,
								skillDataByID.UsedNum
							});
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, text5, 216);
							Global._TCPManager.MySocketListener.SendData(gameClient.ClientSocket, tcpOutPacket, true);
							text = string.Format("为{0}设置技能{1}的级别为{2}", text2, stringValue, num81);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
				}
				else if (!("-setskillum" == cmdFields[0]))
				{
					if ("-del" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -del 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求从数据库删除:{0}", text2), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10068, string.Format("{0}", text2), null, 0);
								text = string.Format("从数据库删除{0}", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-undel" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -undel 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求为恢复删除:{0}", text2), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10052, string.Format("{0}", text2), null, 0);
								text = string.Format("为{0}恢复删除", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-modlimit" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -modlimit 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num82 = this.SafeConvertToInt32(cmdFields[1]);
								Global._TCPManager.MaxConnectedClientLimit = num82;
								LogManager.WriteLog(3, string.Format("根据GM的要求为修改最大在线人数{0}", num82), null, true);
								text = string.Format("修改最大在线人数限制为{0}", num82);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							int num82 = this.SafeConvertToInt32(cmdFields[1]);
							Global._TCPManager.MaxConnectedClientLimit = num82;
							LogManager.WriteLog(3, string.Format("根据GM的要求为修改最大在线人数{0}", num82), null, true);
						}
					}
					else if ("-listhorse" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -listhorse 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求列举{0}的坐骑ID列表", text2), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								if (null == gameClient.ClientData.HorsesDataList)
								{
									text = string.Format("{0}的坐骑ID列表为空", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								}
								List<string> list6 = new List<string>();
								lock (gameClient.ClientData.HorsesDataList)
								{
									for (int i = 0; i < gameClient.ClientData.HorsesDataList.Count; i++)
									{
										text = string.Format("{0} {1} {2}/{3}/{4}", new object[]
										{
											gameClient.ClientData.HorsesDataList[i].DbID,
											Global.GetHorseNameByID(gameClient.ClientData.HorsesDataList[i].HorseID),
											Global.GetHorseFailedNum(gameClient.ClientData.HorsesDataList[i]),
											Global.GetHorseHorseBlessPoint(gameClient.ClientData.HorsesDataList[i].HorseID + 1)
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									}
								}
							}
						}
					}
					else if ("-sethl" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 4)
							{
								text = string.Format("请输入：-sethl 角色名称 坐骑ID(-listhorse得到的ID) 幸运值", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num83 = Global.SafeConvertToInt32(cmdFields[2]);
								int num84 = Global.SafeConvertToInt32(cmdFields[3]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置坐骑ID{1}的进阶幸运值", text2, num83), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								HorseData horseDataByDbID = Global.GetHorseDataByDbID(gameClient, num83);
								if (null == horseDataByDbID)
								{
									text = string.Format("在{0}的坐骑列表中没有找到ID为{1}的坐骑", text2, num83);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								}
								Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, horseDataByDbID.DbID, horseDataByDbID.HorseID, num84, Global.GetHorseStrTempTime(horseDataByDbID), horseDataByDbID.JinJieTempNum, horseDataByDbID.JinJieFailedDayID);
								text = string.Format("为{0}的坐骑ID为{1}的设置幸运值{2}", text2, num83, num84);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setwlogin" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -setwlogin 角色名称 天数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num85 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置本周的连续登录天数{1}", text2, num85), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								gameClient.ClientData.MyHuodongData.LoginNum = num85;
								text = string.Format("为{0}设置本周的连续登录天数{1}", text2, num85);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setmtime" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -setmtime 角色名称 在线时长(秒数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num86 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置本月的在线时长{1}", text2, num86), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								gameClient.ClientData.MyHuodongData.CurMTime = num86;
								text = string.Format("为{0}设置本周的连续登录天数{1}", text2, num86);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setnstep" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -setnstep 角色名称 当前步骤(1~5)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num87 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置新手见面礼物领取的步骤{1}", text2, num87), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								long stepTime = TimeUtil.NOW();
								gameClient.ClientData.MyHuodongData.NewStep++;
								gameClient.ClientData.MyHuodongData.StepTime = stepTime;
								GameManager.ClientMgr.NotifyHuodongData(gameClient);
								text = string.Format("为{0}设置新手见面礼物领取的步骤{1}", text2, num87);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-updateBindgold" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 5)
							{
								string text3 = cmdFields[1];
								int num88 = Global.SafeConvertToInt32(cmdFields[2]);
								int bindMoney = Global.SafeConvertToInt32(cmdFields[3]);
								string firstbindData = cmdFields[4];
								TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(text3);
								GameClient gameClient = null;
								if (null != tmsksocket)
								{
									gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
								}
								UserMoneyMgr.getInstance().ProcessSendBindGold(gameClient, bindMoney, text3, num88, firstbindData);
							}
						}
					}
					else if ("-removefriend" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 3)
							{
								int num88 = Global.SafeConvertToInt32(cmdFields[1]);
								int dbID = Global.SafeConvertToInt32(cmdFields[2]);
								GameClient gameClient = GameManager.ClientMgr.FindClient(num88);
								if (null == gameClient)
								{
									return true;
								}
								Global.RemoveFriendData(gameClient, dbID);
							}
						}
					}
					else if ("-addcharge" == cmdFields[0])
					{
						long value = (long)Global.SafeConvertToInt32(cmdFields[1]);
						HongBaoManager.getInstance().AddChargeValue(value);
					}
					else if ("-updateyb" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 6)
							{
								string text3 = cmdFields[1];
								int num88 = Global.SafeConvertToInt32(cmdFields[2]);
								int num89 = Global.SafeConvertToInt32(cmdFields[3]);
								int superInputFanLi = Global.SafeConvertToInt32(cmdFields[4]);
								int num90 = Global.SafeConvertToInt32(cmdFields[5]);
								int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
								HongBaoManager.getInstance().AddChargeValue((long)(num89 * gameConfigItemInt));
								SingleChargeData chargeData = Data.ChargeData;
								if (chargeData != null && num90 == 0 && chargeData.YueKaMoney > 0 && num89 == chargeData.YueKaMoney)
								{
									if (num88 > 0)
									{
										int num91 = Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(num88, "10167", 0));
										if (num91 < 10)
										{
											if (WebOldPlayerManager.getInstance().ChouJiangAddCheck(num88, 4))
											{
												GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", num88, "10167", 10 + num91), null, GameCoreInterface.getinstance().GetLocalServerId());
											}
										}
									}
								}
								if (num90 != 0)
								{
									TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(text3);
									if (null == tmsksocket)
									{
										return true;
									}
									GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
									if (null == gameClient)
									{
										return true;
									}
									if (num88 != gameClient.ClientData.RoleID)
									{
										return true;
									}
									UserMoneyMgr.getInstance().HandleClientChargeItem(gameClient, 0);
									GameManager.logDBCmdMgr.AddDBLogInfo(-1, "直购", "GM命令强迫更新", "系统", gameClient.ClientData.RoleName, "增加", num90, gameClient.ClientData.ZoneID, gameClient.strUserID, gameClient.ClientData.UserMoney, gameClient.ServerId, null);
								}
								else if (num88 == -1)
								{
									UserMoneyMgr.getInstance().HandleSystemChargeMoney(text3, num89);
								}
								else
								{
									UserMoneyMgr.getInstance().HandleClientChargeMoney(text3, num88, num89, transmit, superInputFanLi);
								}
							}
						}
					}
					else if ("-buyyueka" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text3 = cmdFields[1];
								int num8 = Global.SafeConvertToInt32(cmdFields[2]);
								YueKaManager.HandleUserBuyYueKa(text3, num8);
							}
							else if (cmdFields.Length == 2)
							{
								string text3 = "not set";
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								YueKaManager.HandleUserBuyYueKa(text3, num8);
							}
						}
					}
					else if ("-setfbnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 4)
							{
								text = string.Format("请输入： -setfbnum 角色名称 副本ID 当日次数(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num92 = Global.SafeConvertToInt32(cmdFields[2]);
								int num93 = Global.SafeConvertToInt32(cmdFields[3]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}增加副本ID{1}的当日次数{2}", text2, num92, num93), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								Global.UpdateFuBenData(gameClient, num92, num93, num93);
								text = string.Format("为{0}增加副本ID{1}的当日次数{2}", text2, num92, num93);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-sethdnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 4)
							{
								text = string.Format("请输入： -sethdnum 角色名称 活动代号ID 当日次数(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num94 = Global.SafeConvertToInt32(cmdFields[2]);
								int num93 = Global.SafeConvertToInt32(cmdFields[3]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}增加副本ID{1}的当日次数{2}", text2, num94, num93), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								Global.UpdateDayActivityEnterCountToDB(gameClient, gameClient.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, num94, num93);
								text = string.Format("为{0}增加活动{1}的当日次数{2}", text2, ((SpecialActivityTypes)num94).ToString(), num93);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setfreshplayer" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -setfreshplayer 1(启用)或0(禁用)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num95 = Global.SafeConvertToInt32(cmdFields[1]);
								Global.Flag_EnabelNewPlayerScene = (num95 > 0);
								LogManager.WriteLog(3, string.Format("根据GM的要求{0}新手场景", Global.Flag_EnabelNewPlayerScene ? "启用" : "禁用"), null, true);
								text = string.Format("{0}新手场景", Global.Flag_EnabelNewPlayerScene ? "启用" : "禁用");
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-addattack" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入：-addattack 角色名称 攻击力(值)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num96 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}临时增加物攻和魔攻{1}", text2, num96), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								gameClient.RoleBuffer.AddForeverExtProp(7, (double)num96);
								gameClient.RoleBuffer.AddForeverExtProp(8, (double)num96);
								gameClient.RoleBuffer.AddForeverExtProp(9, (double)num96);
								gameClient.RoleBuffer.AddForeverExtProp(10, (double)num96);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
								text = string.Format("为{0}临时增加物攻和魔攻{1}", text2, num96);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-adddefense" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入：-adddefense 角色名称 防御力(值)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num97 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}临时增加物防和魔防{1}", text2, num97), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								gameClient.RoleBuffer.AddForeverExtProp(3, (double)num97);
								gameClient.RoleBuffer.AddForeverExtProp(4, (double)num97);
								gameClient.RoleBuffer.AddForeverExtProp(5, (double)num97);
								gameClient.RoleBuffer.AddForeverExtProp(6, (double)num97);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
								text = string.Format("为{0}临时增加物防和魔防{1}", text2, num97);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setybstate" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入：-setybstate 角色名称 状态(0：成功，1：失败)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num98 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}修改押镖的状态{1}", text2, num98), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								if (null != gameClient.ClientData.MyYaBiaoData)
								{
									gameClient.ClientData.MyYaBiaoData.State = num98;
									GameManager.DBCmdMgr.AddDBCmd(10057, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										gameClient.ClientData.RoleID,
										gameClient.ClientData.MyYaBiaoData.YaBiaoID,
										gameClient.ClientData.MyYaBiaoData.StartTime,
										gameClient.ClientData.MyYaBiaoData.State,
										gameClient.ClientData.MyYaBiaoData.LineID,
										gameClient.ClientData.MyYaBiaoData.TouBao,
										gameClient.ClientData.MyYaBiaoData.YaBiaoDayID,
										gameClient.ClientData.MyYaBiaoData.YaBiaoNum,
										gameClient.ClientData.MyYaBiaoData.TakeGoods
									}), null, gameClient.ServerId);
									GameManager.ClientMgr.NotifyYaBiaoData(gameClient);
									text = string.Format("为{0}修改押镖的状态{1}", text2, num98);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								}
								else
								{
									text = string.Format("{0}当前无运镖数据", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								}
							}
						}
					}
					else if ("-setybstate2" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								int num98 = Global.SafeConvertToInt32(cmdFields[2]);
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									return true;
								}
								if (null != gameClient.ClientData.MyYaBiaoData)
								{
									gameClient.ClientData.MyYaBiaoData.State = num98;
									GameManager.DBCmdMgr.AddDBCmd(10057, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										gameClient.ClientData.RoleID,
										gameClient.ClientData.MyYaBiaoData.YaBiaoID,
										gameClient.ClientData.MyYaBiaoData.StartTime,
										gameClient.ClientData.MyYaBiaoData.State,
										gameClient.ClientData.MyYaBiaoData.LineID,
										gameClient.ClientData.MyYaBiaoData.TouBao,
										gameClient.ClientData.MyYaBiaoData.YaBiaoDayID,
										gameClient.ClientData.MyYaBiaoData.YaBiaoNum,
										gameClient.ClientData.MyYaBiaoData.TakeGoods
									}), null, gameClient.ServerId);
									GameManager.ClientMgr.NotifyYaBiaoData(gameClient);
								}
							}
						}
					}
					else if ("-reload" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-reload xml文件名称,不区分大小写(例如：config/mall.xml)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text24 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求重新加载{0}", text24), null, true);
								int num30 = ReloadXmlManager.ReloadXmlFile(text24);
								text = string.Format("重新加载参数{0}, 结果：{1}", text24, num30);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string text24 = cmdFields[1];
							int num30 = ReloadXmlManager.ReloadXmlFile(text24);
							LogManager.WriteLog(3, string.Format("根据转发的GM的要求重新加载{0}, 结果为:{1}", text24, num30), null, true);
						}
					}
					else if ("-loadiplist" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-loadiplist (0|1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text25 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", text25), null, true);
								Program.LoadIPList(text25);
								text = string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", text25);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string text25 = cmdFields[1];
							LogManager.WriteLog(3, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", text25), null, true);
							Program.LoadIPList(text25);
						}
					}
					else if ("-reloadall" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 1)
							{
								text = string.Format("请输入：-reloadall", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求重新加载所有配置文件", new object[0]), null, true);
								ReloadXmlManager.ReloadAllXmlFile();
								text = string.Format("重新加载所有参数配置文件", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 1)
						{
							ReloadXmlManager.ReloadAllXmlFile();
							LogManager.WriteLog(3, string.Format("根据转发的GM的要求重新加载所有参数配置文件", new object[0]), null, true);
						}
						foreach (GameClient gameClient2 in GameManager.ClientMgr.GetAllClients(true))
						{
							gameClient2.sendCmd<SystemOpenData>(718, Data.OpenData, false);
						}
					}
					else if ("-reload2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-reload2 xml文件名称,不区分大小写(例如：config/mall.xml)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text24 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求通知所有线重新加载{0}", text24), null, true);
								int num30 = ReloadXmlManager.ReloadXmlFile(text24);
								text = string.Format("所有线重新加载参数{0}, 结果：{1}", text24, num30);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								if (0 == num30)
								{
									string text7 = string.Format("-reload {0}", cmdFields[1]);
									GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										"",
										0,
										"",
										0,
										text7,
										0,
										0,
										GameManager.ServerLineID
									}), null, 0);
								}
							}
						}
					}
					else if ("-reloadgm" == cmdFields[0])
					{
						if (!transmit)
						{
							if (isSuperGMUser)
							{
								LogManager.WriteLog(3, string.Format("根据超级GM的要求重新加载GM参数", new object[0]), null, true);
								int num30 = this.ReloadGMCommands();
								text = string.Format("重新加载GM参数, 结果：{0}", num30);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else
						{
							LogManager.WriteLog(3, string.Format("根据超级GM的要求重新加载GM参数", new object[0]), null, true);
							int num30 = this.ReloadGMCommands();
						}
					}
					else if ("-reloadph" == cmdFields[0])
					{
						if (!transmit)
						{
							LogManager.WriteLog(3, string.Format("根据GM的要求重新加载排行榜", new object[0]), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10066, string.Format("{0}", client.ClientData.RoleID), null, 0);
							text = string.Format("重新加载排行榜成功", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							LogManager.WriteLog(3, string.Format("根据GM的要求重新加载排行榜", new object[0]), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10066, string.Format("{0}", 0), null, 0);
						}
					}
					else if ("-updatepaihangbang" == cmdFields[0])
					{
						for (int j = 0; j < 23; j++)
						{
							string text5 = StringUtil.substitute("{0}:{1}", new object[]
							{
								0,
								j
							});
							PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, text5, 0);
							if (paiHangData != null && null != paiHangData.PaiHangList)
							{
								List<PaiHangItemData> paiHangList = paiHangData.PaiHangList;
								int i = 0;
								while (i < 50 && i < paiHangList.Count)
								{
									string strText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										paiHangList[i].uid,
										paiHangList[i].RoleID,
										paiHangList[i].RoleName,
										paiHangList[i].Val1,
										paiHangList[i].Val2,
										paiHangList[i].Val3
									});
									EventLogManager.AddRankingEvent((PaiHangTypes)j, i + 1, strText);
									i++;
								}
							}
						}
					}
					else if ("-everyday" == cmdFields[0])
					{
						if (!transmit)
						{
							EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
							if (null != everydayActivity)
							{
								everydayActivity.ShowActiveConditionInfoGM(client);
							}
						}
					}
					else if ("-clrrolecache" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-clrrolecache 角色名称(带区号)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求清空{0}角色的数据库缓存", text2), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 0, text2), null, 0);
								text = string.Format("清空{0}角色的数据库缓存成功", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string text2 = cmdFields[1];
							LogManager.WriteLog(3, string.Format("根据GM的要求清空{0}角色的数据库缓存", text2), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 0, text2), null, 0);
						}
					}
					else if ("-clrusercache" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-clrusercache userid", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求清空{0}帐号的数据库缓存", text2), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 1, text2), null, 0);
								text = string.Format("清空{0}角色的数据库缓存成功", text2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string text2 = cmdFields[1];
							LogManager.WriteLog(3, string.Format("根据GM的要求清空{0}帐号的数据库缓存", text2), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 1, text2), null, 0);
						}
					}
					else if ("-clrrolecachebyid" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-clrrolecachebyid rid", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num99 = Global.SafeConvertToInt32(cmdFields[1]);
								LogManager.WriteLog(3, string.Format("根据GM的要求清空{0}角色的数据库缓存", num99), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", num99, ""), null, 0);
								text = string.Format("清空rid={0}的角色的数据库缓存成功", "");
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							int num99 = Global.SafeConvertToInt32(cmdFields[1]);
							LogManager.WriteLog(3, string.Format("根据GM的要求清空{0}角色的数据库缓存", num99), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", num99, ""), null, 0);
						}
					}
					else if ("-clrallrolecache" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 1)
							{
								text = string.Format("请输入：-clrallrolecache", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								LogManager.WriteLog(3, string.Format("根据GM的要求清空所有角色的数据库缓存", new object[0]), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10122, string.Format("{0}", client.ClientData.RoleID), null, 0);
								text = string.Format("清空所有角色的数据库缓存成功", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 1)
						{
							LogManager.WriteLog(3, string.Format("根据GM的要求清空所有角色的数据库缓存", new object[0]), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10122, string.Format("{0}", 0), null, 0);
						}
					}
					else if ("-addheroidx" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入：-addheroidx 角色名称 层数(大于等于0，小于等于13)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num100 = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}修改英雄逐擂的到达层数{1}", text2, num100), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameManager.ClientMgr.ChangeRoleHeroIndex(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num100, true);
								text = string.Format("为{0}修改英雄逐擂的到达层数{1}", text2, num100);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setlz" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入：-setlz  角色名称 连斩数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num101 = Global.SafeConvertToInt32(cmdFields[2]);
								num101 = Global.GMin(899, num101);
								LogManager.WriteLog(3, string.Format("根据GM的要求为{0}修改连斩数{1}", text2, num101), null, true);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameManager.ClientMgr.ChangeRoleLianZhan(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, null, num101);
								text = string.Format("为{0}修改连斩数{1}", text2, num101);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-applytobh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 6)
							{
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								string text26 = cmdFields[2];
								int num102 = Global.SafeConvertToInt32(cmdFields[3]);
								string bhName = cmdFields[4];
								string roleList = cmdFields[5];
								GameManager.ClientMgr.NotifyOnlineBangHuiMgrRoleApplyMsg(num8, text26, num102, bhName, roleList);
							}
						}
					}
					else if ("-joinbh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 4)
							{
								int num99 = Global.SafeConvertToInt32(cmdFields[1]);
								int num102 = Global.SafeConvertToInt32(cmdFields[2]);
								string bhName = cmdFields[3];
								GameClient gameClient = GameManager.ClientMgr.FindClient(num99);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyJoinBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num102, bhName);
								}
							}
						}
					}
					else if ("-joinbhtime" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length >= 2)
							{
								if (null != client)
								{
									TimeSpan t = default(TimeSpan);
									string text27;
									if (cmdFields.Length == 2)
									{
										text27 = cmdFields[1].Replace('：', ':');
										if (TimeSpan.TryParse(text27, out t))
										{
											text27 = TimeUtil.NowDateTime().Add(-t).ToString("yyyy-MM-dd HH:mm:ss");
										}
										else
										{
											text27 = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
										}
									}
									else
									{
										text27 = cmdFields[1] + " " + cmdFields[2];
									}
									text27 = text27.Replace('：', ':');
									DateTime dateTime;
									if (DateTime.TryParse(text27, out dateTime))
									{
										Global.SaveRoleParamsInt32ValueToDB(client, "EnterBangHuiUnixSecs", DataHelper.SysTicksToUnixSeconds(dateTime.Ticks / 10000L), true);
										string text28 = string.Format("设置角色加入帮会时间为{0}", dateTime.ToString().Replace(':', '：'));
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									}
								}
							}
						}
					}
					else if ("-leavebh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int num99 = Global.SafeConvertToInt32(cmdFields[1]);
								int num102 = Global.SafeConvertToInt32(cmdFields[2]);
								string bhName = cmdFields[3];
								int leaveType = Global.SafeConvertToInt32(cmdFields[4]);
								GameClient gameClient = GameManager.ClientMgr.FindClient(num99);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyLeaveBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num102, bhName, leaveType);
								}
							}
						}
					}
					else if ("-destroybh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 4)
							{
								int num30 = Global.SafeConvertToInt32(cmdFields[1]);
								int num8 = Global.SafeConvertToInt32(cmdFields[2]);
								int num102 = Global.SafeConvertToInt32(cmdFields[3]);
								GameManager.ClientMgr.NotifyBangHuiDestroy(num30, num8, num102);
								HongBaoManager.getInstance().OnDestoryZhanMeng(num102);
							}
						}
					}
					else if ("-autodestroybh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 1)
							{
								int num102 = Global.SafeConvertToInt32(cmdFields[1]);
								GameManager.ClientMgr.NotifyBangHuiDestroy(0, 0, num102);
								JunQiManager.SendClearJunQiCmd(num102);
								JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
								JunQiManager.NotifySyncBangHuiLingDiItemsDict();
								Global.RemoveBangHuiMiniData(num102);
							}
						}
					}
					else if ("-invitetobh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 6)
							{
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								int inviteRoleID = Global.SafeConvertToInt32(cmdFields[2]);
								string inviteRoleName = cmdFields[3];
								int num102 = Global.SafeConvertToInt32(cmdFields[4]);
								string bhName = cmdFields[5];
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyInviteToBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, inviteRoleID, inviteRoleName, num102, bhName, gameClient.ClientData.ChangeLifeCount);
								}
							}
						}
					}
					else if ("-refusetobh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								string bhRoleName = cmdFields[2];
								string bhName = cmdFields[3];
								int num103 = Global.SafeConvertToInt32(cmdFields[4]);
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null != gameClient)
								{
									if (0 == num103)
									{
										GameManager.ClientMgr.NotifyRefuseApplyToBHMember(gameClient, bhRoleName, bhName);
									}
									else
									{
										GameManager.ClientMgr.NotifyRefuseInviteToBHMember(gameClient, bhRoleName, bhName);
									}
								}
							}
						}
					}
					else if ("-syncjunqi" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 1)
							{
								JunQiManager.LoadBangHuiJunQiItemsDictFromDBServer();
							}
						}
					}
					else if ("-synclingdi" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 1)
							{
								JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
							}
						}
					}
					else if ("-syncldzresult" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 6)
							{
								int lingDiID = Global.SafeConvertToInt32(cmdFields[1]);
								int num2 = Global.SafeConvertToInt32(cmdFields[2]);
								int num102 = Global.SafeConvertToInt32(cmdFields[3]);
								int zoneID = Global.SafeConvertToInt32(cmdFields[4]);
								string bhName = cmdFields[5];
								JunQiManager.HandleLingDiZhanResultByMapCode2(lingDiID, num2, num102, zoneID, bhName);
							}
						}
					}
					else if ("-chbhzhiwu" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int num102 = Global.SafeConvertToInt32(cmdFields[1]);
								int num99 = Global.SafeConvertToInt32(cmdFields[2]);
								int bhzhiWu = Global.SafeConvertToInt32(cmdFields[3]);
								int roleID = Global.SafeConvertToInt32(cmdFields[4]);
								GameClient gameClient = GameManager.ClientMgr.FindClient(num99);
								if (null != gameClient)
								{
									if (gameClient.ClientData.Faction == num102)
									{
										gameClient.ClientData.BHZhiWu = bhzhiWu;
										GameManager.ClientMgr.ChangeBangHuiZhiWu(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
									}
								}
								gameClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != gameClient)
								{
									if (gameClient.ClientData.Faction == num102)
									{
										gameClient.ClientData.BHZhiWu = 0;
										GameManager.ClientMgr.ChangeBangHuiZhiWu(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
									}
								}
							}
						}
					}
					else if ("-removehuangfei" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int num99 = Global.SafeConvertToInt32(cmdFields[1]);
								string huangDiRoleName = cmdFields[2];
								GameClient gameClient = GameManager.ClientMgr.FindClient(num99);
								if (null != gameClient)
								{
									Global.UpdateRoleHuangHou(gameClient, 0, huangDiRoleName);
								}
							}
						}
					}
					else if ("-leavelaofang" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int num99 = Global.SafeConvertToInt32(cmdFields[1]);
								string huangDiRoleName = cmdFields[2];
								GameClient gameClient = GameManager.ClientMgr.FindClient(num99);
								if (null != gameClient)
								{
									Global.ForceTakeOutLaoFangMap(gameClient, gameClient.ClientData.PKPoint);
									Global.BroadcastTakeOutLaoFangHint(huangDiRoleName, Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName));
								}
							}
						}
					}
					else if ("-clearmap" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 2)
							{
								int num102 = Global.SafeConvertToInt32(cmdFields[1]);
								JunQiManager.ProcessDelAllJunQiByBHID(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, num102);
							}
						}
					}
					else if ("-synchuangdi" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int oldHuangDiRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								int num8 = Global.SafeConvertToInt32(cmdFields[2]);
								string text26 = cmdFields[3];
								string bhName = cmdFields[4];
								HuangChengManager.ProcessTakeSheLiZhiYuan(num8, text26, bhName, false);
								GameManager.ClientMgr.NotifyAllChgHuangDiRoleIDMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, oldHuangDiRoleID, HuangChengManager.GetHuangDiRoleID());
							}
						}
					}
					else if ("-reloadxml" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入：-reloadxml  文字信息", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text29 = cmdFields[1];
								LogManager.WriteLog(3, string.Format("根据GM的要求发送重新加载xml的通知消息{0}", text29), null, true);
								text29 = text29.Replace(":", "");
								GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, text29, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 16, 0, 0, 100, 100);
								text = string.Format("成功发送重新加载xml的通知信息：{0}成功", text29);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-notifymail" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 2)
							{
								string[] array9 = cmdFields[1].Split(new char[]
								{
									'_'
								});
								foreach (string text30 in array9)
								{
									string[] array11 = text30.Split(new char[]
									{
										'|'
									});
									int num8 = Global.SafeConvertToInt32(array11[0]);
									int mailID = Global.SafeConvertToInt32(array11[1]);
									GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
									if (null != gameClient)
									{
										GameManager.ClientMgr.NotifyLastUserMail(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, mailID);
										gameClient._IconStateMgr.CheckEmailCount(gameClient, true);
									}
								}
							}
						}
					}
					else if ("-notifygmail" == cmdFields[0])
					{
						if (transmit)
						{
							GroupMailManager.RequestNewGroupMailList();
						}
					}
					else if ("-notifyUserReturn" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 2)
							{
								string[] array12 = cmdFields[1].Split(new char[]
								{
									'#'
								});
								foreach (string text31 in array12)
								{
									if (!string.IsNullOrEmpty(text31))
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(int.Parse(text31));
										if (null != gameClient)
										{
											UserReturnManager.getInstance().initUserReturnData(gameClient);
											gameClient._IconStateMgr.AddFlushIconState(14104, true);
											gameClient._IconStateMgr.SendIconStateToClient(gameClient);
										}
									}
								}
							}
						}
					}
					else if ("-resetgmail" == cmdFields[0])
					{
						if (transmit)
						{
							GroupMailManager.ResetData();
							GroupMailManager.RequestNewGroupMailList();
						}
					}
					else if ("-modifyastar" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length != 2)
							{
								text = string.Format("请输入：-modifyastar 大于等于8的整数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								Global.ModifyMaxOpenNodeCountForAStar(Global.SafeConvertToInt32(cmdFields[1]));
							}
						}
					}
					else if ("-modifylogtype" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length != 2)
							{
								text = string.Format("请输入：-modifylogtype -1到3的整数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								LogManager.LogTypeToWrite = Global.SafeConvertToInt32(cmdFields[1]);
							}
						}
					}
					else if ("-reloadnpc" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int num2 = Global.SafeConvertToInt32(cmdFields[1]);
							NPCGeneralManager.RemoveMapNpcs(num2);
							NPCGeneralManager.ReloadMapNPCRoles(num2);
						}
						else
						{
							text = string.Format("请输入：-reloadnpc 地图编号", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-modifyparams" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 4)
							{
								string text2 = cmdFields[1];
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								int num104 = Global.SafeConvertToInt32(cmdFields[2]);
								int num105 = Global.SafeConvertToInt32(cmdFields[3]);
								int num106 = num104;
								switch (num106)
								{
								case 0:
									GameManager.ClientMgr.ModifyChengJiuPointsValue(gameClient, num105, "GM指令增加成就", true, true);
									break;
								case 1:
									GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(gameClient, num105, true, true);
									break;
								case 2:
									GameManager.ClientMgr.ModifyLieShaValue(gameClient, num105, true, true);
									break;
								case 3:
									GameManager.ClientMgr.ModifyWuXingValue(gameClient, num105, true, true, true);
									break;
								case 4:
									GameManager.ClientMgr.ModifyZhenQiValue(gameClient, num105, true, true);
									break;
								case 5:
									GameManager.ClientMgr.ModifyTianDiJingYuanValue(gameClient, num105, "GM指令增加魔晶", true, true, false);
									break;
								case 6:
									GameManager.ClientMgr.ModifyShiLianLingValue(gameClient, num105, true, true);
									break;
								case 7:
									break;
								case 8:
									break;
								case 9:
									GameManager.ClientMgr.ModifyZuanHuangLevelValue(gameClient, num105, true, true);
									break;
								case 10:
									GameManager.ClientMgr.ModifySystemOpenValue(gameClient, num105, true, true);
									break;
								case 11:
									GameManager.ClientMgr.ModifyJunGongValue(gameClient, num105, true, true);
									break;
								case 12:
								case 13:
								case 16:
								case 17:
								case 20:
								case 21:
								case 22:
								case 23:
								case 24:
								case 25:
								case 28:
								case 29:
									break;
								case 14:
									GameManager.ClientMgr.ModifyZhanHunValue(gameClient, num105, true, true);
									break;
								case 15:
									GameManager.ClientMgr.ModifyRongYuValue(gameClient, num105, true, true);
									break;
								case 18:
									GameManager.ClientMgr.ModifyShengWangValue(gameClient, num105, "GM指令(modifyparams)增加声望", true, true);
									break;
								case 19:
								{
									int shengWangLevelValue = GameManager.ClientMgr.GetShengWangLevelValue(client);
									GameManager.ClientMgr.ModifyShengWangLevelValue(gameClient, num105 - shengWangLevelValue, "GM指令(modifyparams)增加声望等级", true, true);
									break;
								}
								case 26:
									FashionManager.getInstance().ModifyFashionWingsID(gameClient, num105, true, true);
									break;
								case 27:
									GameManager.ClientMgr.ModifyZaiZaoValue(gameClient, num105, "GM指令增加再造点", true, true, false);
									break;
								case 30:
									FashionManager.getInstance().ModifyFashionTitleID(gameClient, num105, true, true);
									break;
								default:
									if (num106 != 40)
									{
										switch (num106)
										{
										case 49:
											GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, num105, "GM指令(modifyparams)增加符文之尘", true, true, false);
											break;
										case 51:
											GameManager.ClientMgr.ModifyEraDonateValue(client, num105, "GM指令(modifyparams)增加纪元贡献", true, true, false);
											break;
										}
									}
									break;
								}
							}
							else
							{
								text = string.Format("请输入：-modifyparams 角色名称 参数索引(0成就 1积分 2猎杀 3悟性 4真气 5天地精元 6试炼令[===>通天令值] 7经脉等级 8武学等级 9钻皇等级 10系统激活项 11军功) 值(正负)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-lastgold" == cmdFields[0])
					{
						if (!transmit)
						{
							BangHuiMatchManager.getInstance().SwitchLastGoldBH_GM();
						}
					}
					else if ("-fashion" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length != 3)
							{
								text = string.Format("请输入：-fashion 0删除1增加 fashionid", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							int num107 = Global.SafeConvertToInt32(cmdFields[1]);
							int nFashionID = Global.SafeConvertToInt32(cmdFields[2]);
							if (num107 > 0)
							{
								FashionManager.getInstance().GetFashionByMagic(client, nFashionID, true);
							}
							else
							{
								FashionManager.getInstance().DelFashionByMagic(client, nFashionID);
							}
						}
					}
					else if ("-modifyparams2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 4)
							{
								string text2 = cmdFields[1];
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								string text32 = cmdFields[2];
								string text33 = cmdFields[3];
								Global.UpdateRoleParamByName(client, text32, text33, true);
								LogManager.WriteLog(3, string.Format("根据GM的要求修改角色[{0}]的参数[{1}]为[{2}]", text2, text32, text33), null, true);
							}
							else
							{
								text = string.Format("请输入：-modifyparams2 角色名称 参数名 值(正负)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-summon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								int num108 = Global.SafeConvertToInt32(cmdFields[1]);
								int num109 = Global.SafeConvertToInt32(cmdFields[2]);
								if (num109 > 0 && num108 > 0 && num109 < 50)
								{
									GameManager.LuaMgr.AddDynamicMonsters(client, num108, num109, (int)client.CurrentGrid.X, (int)client.CurrentGrid.Y, 3);
									text = string.Format("执行GM召唤怪物", new object[0]);
								}
								else
								{
									text = string.Format("请输入：-summon 怪物id 数量[最多49]", new object[0]);
								}
							}
							else
							{
								text = string.Format("请输入：-summon 怪物id 数量[最多49]", new object[0]);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-addnpc" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int num110 = Global.SafeConvertToInt32(cmdFields[1]);
							if (num110 > 0)
							{
								GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
								if (null != gameMap)
								{
									GameManager.LuaMgr.AddNpcToMap(num110, client.ClientData.MapCode, (int)client.CurrentGrid.X * gameMap.MapGridWidth, (int)client.CurrentGrid.Y * gameMap.MapGridHeight);
									text = string.Format("执行GM召唤NPC", new object[0]);
								}
								else
								{
									text = string.Format("执行GM召唤NPC 失败", new object[0]);
								}
							}
							else
							{
								text = string.Format("请输入：-addnpc npcid", new object[0]);
							}
						}
						else
						{
							text = string.Format("请输入：-addnpc npcid", new object[0]);
						}
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
					}
					else if ("-refreshqg" == cmdFields[0])
					{
						if (!transmit)
						{
							ReloadXmlManager.ReloadXmlFile("config/qianggou.xml");
						}
					}
					else if ("-addgumutime" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text2 = cmdFields[1];
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								int num111 = Global.SafeConvertToInt32(cmdFields[2]);
								Global.AddGuMuMapTime(gameClient, 0, num111 * 60);
							}
							else
							{
								text = string.Format("增减古墓buffer时间请输入：-addgumutime 角色名称 值(正负，单位分钟)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-addlimitlogin" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								string text2 = cmdFields[1];
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								Global.UpdateLimitTimeLoginNum(gameClient);
								HuodongData myHuodongData = client.ClientData.MyHuodongData;
								myHuodongData.LimitTimeLoginNum++;
								Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient);
								GameManager.ClientMgr.NotifyHuodongData(client);
							}
							else
							{
								text = string.Format("增加限制时间累计登录次数请输入：-addlimitlogin 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-runnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int r = Global.SafeConvertToInt32(cmdFields[1]);
								MonsterZoneManager.MaxRunQueueNum = Global.GMax(1, r);
								text = string.Format(string.Format("副本单次循环的最大限制设置为：{0}", MonsterZoneManager.MaxRunQueueNum), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("副本单次循环的最大限制设置请输入：-runnum 次数(最大500)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-waitnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int r = Global.SafeConvertToInt32(cmdFields[1]);
								MonsterZoneManager.MaxWaitingRunQueueNum = Global.GMax(1, r);
								text = string.Format(string.Format("副本队列的等待数量设置为：{0}", MonsterZoneManager.MaxRunQueueNum), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("副本队列的等待数量设置请输入：-waitnum 次数(最大500)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-sklevel" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int r2 = Global.SafeConvertToInt32(cmdFields[1]);
								MonsterManager.MinSeekRangeMonsterLevel = Global.GMax(0, r2);
								text = string.Format(string.Format("怪物的自动寻敌和自动走动的最低级别设置为：{0}", MonsterManager.MinSeekRangeMonsterLevel), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("怪物的自动寻敌和自动走动的最低级别设置请输入：-sklevel 级别", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-st9grid" == cmdFields[0])
					{
						if (cmdFields.Length == 5)
						{
							GameManager.MaxSlotOnUpdate9GridsTicks = Math.Max(500, Global.SafeConvertToInt32(cmdFields[1]));
							GameManager.MaxSleepOnDoMapGridMoveTicks = Math.Max(5, Global.SafeConvertToInt32(cmdFields[2]));
							GameManager.MaxCachingMonsterToClientBytesDataTicks = Math.Max(0, Global.SafeConvertToInt32(cmdFields[3]));
							GameManager.MaxCachingClientToClientBytesDataTicks = Math.Max(0, Global.SafeConvertToInt32(cmdFields[4]));
							text = string.Format(string.Format("九宫格相关信息设置：九宫更新间隔毫秒={0}, 间歇休眠毫秒={1}, 怪物缓存毫秒={2}, 角色缓存毫秒={3}", new object[]
							{
								GameManager.MaxSlotOnUpdate9GridsTicks,
								GameManager.MaxSleepOnDoMapGridMoveTicks,
								GameManager.MaxCachingMonsterToClientBytesDataTicks,
								GameManager.MaxCachingClientToClientBytesDataTicks
							}), new object[0]);
							LogManager.WriteLog(3, text, null, true);
							if (!transmit && null != client)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (!transmit && null != client)
						{
							text = string.Format("九宫格相关信息设置：-st9grid 九宫更新间隔毫秒 间歇休眠毫秒 怪物缓存毫秒 角色缓存毫秒", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-st9gridinfo" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 1)
							{
								text = string.Format(string.Format("九宫格相关信息设置：九宫更新间隔毫秒={0}, 间歇休眠毫秒={1}, 怪物缓存毫秒={2}, 角色缓存毫秒={3}", new object[]
								{
									GameManager.MaxSlotOnUpdate9GridsTicks,
									GameManager.MaxSleepOnDoMapGridMoveTicks,
									GameManager.MaxCachingMonsterToClientBytesDataTicks,
									GameManager.MaxCachingClientToClientBytesDataTicks
								}), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("九宫格相关信息显示：-st9gridinfo", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-st9gridmode" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								GameManager.Update9GridUsingPosition = Global.SafeConvertToInt32(cmdFields[1]);
								GameManager.MaxSlotOnPositionUpdate9GridsTicks = Global.SafeConvertToInt32(cmdFields[2]);
								text = string.Format(string.Format("九宫格相关信息设置：九宫更新模式={0}, 位置更新九宫格时间间隔={1}", GameManager.Update9GridUsingPosition, GameManager.MaxSlotOnPositionUpdate9GridsTicks), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("九宫格相关信息设置：-st9gridmode 九宫更新模式 位置更新九宫格时间间隔", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-st9gridnew" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								GameManager.MaxSlotOnPositionUpdate9GridsTicks = Global.SafeConvertToInt32(cmdFields[2]);
								text = string.Format(string.Format("九宫格相关信息设置：九宫更新模式={0}, 位置更新九宫格时间间隔={1}", 0, GameManager.MaxSlotOnPositionUpdate9GridsTicks), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("九宫格相关信息设置：-st9gridnew 九宫更新模式 位置更新九宫格时间间隔", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-stzipsize" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								DataHelper.MinZipBytesSize = Global.SafeConvertToInt32(cmdFields[1]);
								text = string.Format(string.Format("压缩的zip大小：最小压缩={0}", DataHelper.MinZipBytesSize), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("压缩的zip大小设置：-stzipsize 最小压缩", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							DataHelper.MinZipBytesSize = Global.SafeConvertToInt32(cmdFields[1]);
							LogManager.WriteLog(3, string.Format("根据GM的要求为设置Zip压缩最小值为{0}", DataHelper.MinZipBytesSize), null, true);
						}
					}
					else if ("-stroledatamini" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								GameManager.RoleDataMiniMode = Global.SafeConvertToInt32(cmdFields[1]);
								text = string.Format(string.Format("设置roleDataMini模式：模式={0}", GameManager.RoleDataMiniMode), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("设置roleDataMini模式：-stroledatamini 模式(0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-stkaifuawardhour" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								HuodongCachingMgr.ProcessKaiFuGiftAwardHour = Global.SafeConvertToInt32(cmdFields[1]);
								text = string.Format(string.Format("设置开服在线奖励的小时：hour={0}", HuodongCachingMgr.ProcessKaiFuGiftAwardHour), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("设置开服在线奖励的小时：-stkaifuawardhour 小时(0~23)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-addonlinesecs" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text2 = cmdFields[1];
								int num12 = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
								num12 *= 60;
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								int num112 = client.ClientData.TotalOnlineSecs / 3600;
								gameClient.ClientData.TotalOnlineSecs += num12;
								int num113 = client.ClientData.TotalOnlineSecs / 3600;
								if (num112 != num113)
								{
									HuodongCachingMgr.ProcessKaiFuGiftAward(client);
								}
								text = string.Format("给{0}增加在线时长到：{1}分钟", text2, gameClient.ClientData.TotalOnlineSecs / 60);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("增加在线时长：-stkaifuawardhour 角色名称 分钟", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-banchat2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text2 = cmdFields[1];
								int num114 = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
								int num8 = RoleName2IDs.FindRoleIDByName(text2, true);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null != gameClient)
								{
									gameClient.ClientData.BanChat = num114;
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 1, num114), null, 0);
								text = string.Format("将{0}永久禁言设置为：{1}", text2, num114);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("永久禁言：-banchat2 角色名称 (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							int num114 = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null != gameClient)
							{
								gameClient.ClientData.BanChat = num114;
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 1, num114), null, 0);
						}
					}
					else if ("-banchat2_rid" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text2 = cmdFields[1] + "$rid";
								int num114 = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null != gameClient)
								{
									gameClient.ClientData.BanChat = num114;
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 1, num114), null, 0);
								text = string.Format("将{0}永久禁言设置为：{1}", text2, num114);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("永久禁言：-banchat2 角色名称 (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							int num114 = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null != gameClient)
							{
								gameClient.ClientData.BanChat = num114;
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 1, num114), null, 0);
						}
					}
					else if ("-warn" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							return false;
						}
						string text3 = cmdFields[1];
						int warnType = int.Parse(cmdFields[2]);
						WarnManager.WarnProcess(text3, warnType);
					}
					else if ("-ban2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -ban2 角色名称 (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num115 = Global.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, true);
								if (-1 != num8)
								{
									if (num115 > 0)
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
										if (null != gameClient)
										{
											gameClient.CheckCheatData.IsKickedRole = true;
											LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", text2), null, true);
											Global.ForceCloseClient(gameClient, "被GM踢了", true);
										}
										else
										{
											string text7 = string.Format("-kick {0}", cmdFields[1]);
											GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
											{
												num8,
												"",
												0,
												"",
												0,
												text7,
												0,
												0,
												GameManager.ServerLineID
											}), null, 0);
										}
									}
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 2, num115), null, 0);
								text = string.Format("将{0}永久禁止登陆设置为：{1}", text2, num115);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							int num115 = Global.SafeConvertToInt32(cmdFields[2]);
							if (-1 != num8)
							{
								if (num115 > 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
									if (null != gameClient)
									{
										if (!gameClient.CheckCheatData.IsKickedRole)
										{
											gameClient.CheckCheatData.IsKickedRole = true;
											RoleData cmdData = new RoleData
											{
												RoleID = -70
											};
											gameClient.sendCmd<RoleData>(104, cmdData, false);
										}
										else
										{
											Global.ForceCloseClient(gameClient, "被禁止登陆", true);
										}
									}
									else
									{
										LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(num8);
									}
								}
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 2, num115), null, 0);
						}
					}
					else if ("-ban2_rid" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -ban2_rid 角色ID (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1] + "$rid";
								int num115 = Global.SafeConvertToInt32(cmdFields[2]);
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								if (-1 != num8)
								{
									if (num115 > 0)
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
										if (null != gameClient)
										{
											gameClient.CheckCheatData.IsKickedRole = true;
											LogManager.WriteLog(3, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", text2), null, true);
											Global.ForceCloseClient(gameClient, "被GM踢了", true);
										}
									}
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 2, num115), null, 0);
								text = string.Format("将{0}永久禁止登陆设置为：{1}", text2, num115);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int num8 = Global.SafeConvertToInt32(cmdFields[1]);
							int num115 = Global.SafeConvertToInt32(cmdFields[2]);
							if (-1 != num8)
							{
								if (num115 > 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
									if (null != gameClient)
									{
										if (!gameClient.CheckCheatData.IsKickedRole)
										{
											gameClient.CheckCheatData.IsKickedRole = true;
											RoleData cmdData = new RoleData
											{
												RoleID = -70
											};
											gameClient.sendCmd<RoleData>(104, cmdData, false);
										}
										else
										{
											Global.ForceCloseClient(gameClient, "被禁止登陆", true);
										}
									}
									else
									{
										LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(num8);
									}
								}
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 2, num115), null, 0);
						}
					}
					else if ("-ban3" == cmdFields[0])
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -ban3 角色名 类型(0/1/2) 封号分钟数", new object[0]);
						}
						else
						{
							string text26 = cmdFields[1];
							int num116 = Global.SafeConvertToInt32(cmdFields[2]);
							int num115 = Global.SafeConvertToInt32(cmdFields[3]);
							BanManager.BanRoleName(text26, num115, num116);
							text = string.Format("根据GM要求设置角色{0}封号状态{1},时长{2}", text26, num116, num115);
						}
						if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							LogManager.WriteLog(2, text, null, true);
						}
					}
					else if ("-ban4" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int num8 = Global.SafeConvertToInt32(cmdFields[1]);
								int num115 = Global.SafeConvertToInt32(cmdFields[2]);
								if (-1 != num8)
								{
									if (num115 > 0)
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
										if (null != gameClient)
										{
											if (!gameClient.CheckCheatData.IsKickedRole)
											{
												gameClient.CheckCheatData.IsKickedRole = true;
											}
											Global.ForceCloseClient(gameClient, "被禁止登陆", true);
										}
										else
										{
											LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(num8);
										}
									}
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", num8, 2, num115), null, 0);
							}
						}
					}
					else if ("-bantrade" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int num117 = Global.SafeConvertToInt32(cmdFields[1]);
							int num118 = Global.SafeConvertToInt32(cmdFields[2]);
							long toTicks = 0L;
							if (num118 > 0)
							{
								toTicks = TimeUtil.NowDateTime().AddSeconds((double)num118).Ticks;
							}
							LogManager.WriteLog(2, string.Format("GM封禁交易, roleid={0}, sec={1}", num117, num118), null, true);
							SingletonTemplate<TradeBlackManager>.Instance().SetBanTradeToTicks(num117, toTicks);
						}
					}
					else if ("-getmapalivemon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -getmapalivemon 地图编号", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num2 = Global.SafeConvertToInt32(cmdFields[1]);
								int num119 = 0;
								int num120 = 0;
								int num121 = 0;
								List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(num2);
								if (null != objectsByMap)
								{
									num121 = objectsByMap.Count;
									for (int i = 0; i < objectsByMap.Count; i++)
									{
										if ((objectsByMap[i] as Monster).VLife <= 0.0)
										{
											num119++;
										}
										if (!(objectsByMap[i] as Monster).Alive)
										{
											num120++;
										}
									}
								}
								text = string.Format("本地图的怪物信息，totalMonsterNum={0}，lifeVZeroNum={1}，NoAliveNum={2}", num121, num119, num120);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-setmapalivemon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -setmapalivemon 地图编号", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num2 = Global.SafeConvertToInt32(cmdFields[1]);
								int num121 = 0;
								List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(num2);
								if (null != objectsByMap)
								{
									num121 = objectsByMap.Count;
									for (int i = 0; i < objectsByMap.Count; i++)
									{
										if ((objectsByMap[i] as Monster).VLife <= 0.0)
										{
											if ((objectsByMap[i] as Monster).Alive)
											{
												(objectsByMap[i] as Monster).VLife = 0.0;
											}
										}
									}
								}
								text = string.Format("矫正本地图的怪物复活信息状态，totalMonsterNum={0}", num121);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-forcemapalivemon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -forcemapalivemon 地图编号", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num2 = Global.SafeConvertToInt32(cmdFields[1]);
								int num121 = 0;
								List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(num2);
								if (null != objectsByMap)
								{
									num121 = objectsByMap.Count;
									for (int i = 0; i < objectsByMap.Count; i++)
									{
										GameManager.MonsterMgr.AddDelayDeadMonster(objectsByMap[i] as Monster);
									}
								}
								text = string.Format("强制本地图的怪物重新死亡，totalMonsterNum={0}", num121);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-addactivevalue" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -adddj 角色名称 活跃值(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num122 = this.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								int dailyActiveDataByField = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
								client.ClientData.DailyActiveValues += num122;
								if (num122 >= 0)
								{
									DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
									if (client.ClientData.DailyActiveValues >= 100)
									{
										WebOldPlayerManager.getInstance().ChouJiangAddCheck(client.ClientData.RoleID, 1);
									}
								}
								else
								{
									DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
								}
								dailyActiveDataByField = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
								DailyActiveManager.NotifyClientDailyActiveData(client, -1, false);
								text = string.Format("为{0}添加了活跃{1}", text2, num122);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-addsw" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -adddj 角色名称 声望值(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num123 = this.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameManager.ClientMgr.ModifyShengWangValue(gameClient, num123, "GM指令增加声望", true, true);
								text = string.Format("为{0}添加了声望{1}", text2, num123);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-pkking" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -pkking 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								string[] array = Global.ExecuteDBCmd(195, string.Format("{0}:{1}:0", client.ClientData.RoleID, text2), 0);
								if (array == null || array.Length < 5)
								{
									text = string.Format("获取{0}的角色ID时连接数据库失败", text2);
								}
								else
								{
									int num99 = Global.SafeConvertToInt32(array[3]);
									GameManager.ArenaBattleMgr.ClearDbKingNpc();
									GameManager.ArenaBattleMgr.SetPKKingRoleID(num99);
									GameManager.ArenaBattleMgr.ReShowPKKing();
									if (client.ClientData.RoleID == num99)
									{
										Global.UpdateBufferData(client, BufferItemTypes.PKKingBuffer, new double[]
										{
											85200.0,
											2000800.0
										}, 0, true);
									}
									text = string.Format("{0}已经设置为当前的PK之王", text2);
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-testdata" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -testdata 功能号 {参数1|参数2|参数3...}", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text34 = "";
								int num124 = Global.SafeConvertToInt32(cmdFields[1]);
								if (num124 == 0)
								{
									ExtPropIndexes extPropIndexes = (ExtPropIndexes)Global.SafeConvertToInt32(cmdFields[2]);
									double num74 = Convert.ToDouble(cmdFields[3]);
									if (ExtPropIndexes.SubAttackInjurePercent == extPropIndexes)
									{
										num74 /= 100.0;
									}
									client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
									{
										PropsSystemTypes.GM_Temp_Props,
										(int)extPropIndexes,
										num74
									});
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								}
								else if (num124 == 1)
								{
									int num125 = 0;
									int num126 = 0;
									for (int i = 0; i < 10000; i++)
									{
										int goodsPackID = Global.SafeConvertToInt32(cmdFields[2]);
										int num44 = Global.SafeConvertToInt32(cmdFields[3]);
										List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataList(client, goodsPackID, num44, 0, 1.0);
										if (goodsDataList != null && goodsDataList.Count > 0)
										{
											num125 += goodsDataList.Count;
										}
										else
										{
											num126++;
										}
									}
									text34 = string.Format("总计：{0}，轮空：{1}", num125, num126);
								}
								else if (num124 == 2)
								{
									double num127 = Global.SafeConvertToDouble(cmdFields[2]);
									Global.UpdateDBGameConfigg("AngelTempleMonsterUpgradeNumber", num127.ToString("0.00"));
									text34 = string.Format("将天使神殿Boss成长比例设置为{0}", num127);
								}
								else if (num124 == 3)
								{
									LuoLanChengZhanManager.getInstance().GMSetLuoLanChengZhu(Global.SafeConvertToInt32(cmdFields[2]));
								}
								else if (num124 == 4)
								{
									GameManager.FlagAlowUnRegistedCmd = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (num124 == 5)
								{
									GameManager.StatisticsMode = Global.SafeConvertToInt32(cmdFields[2]);
								}
								else if (num124 == 6)
								{
									if (cmdFields.Length >= 5)
									{
										sbyte sShengWu_slot = sbyte.Parse(cmdFields[2]);
										sbyte sBuJian_slot = sbyte.Parse(cmdFields[3]);
										int nNum = Global.SafeConvertToInt32(cmdFields[4]);
										HolyItemManager.getInstance().GetHolyItemPart(client, sShengWu_slot, sBuJian_slot, nNum);
									}
								}
								else if (num124 == 7)
								{
									GameManager.FlaDisablegFilterMonsterDeadEvent = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (num124 == 8)
								{
									GameManager.FlagKuaFuServerExplicit = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
									GameManager.IsKuaFuServer = (GameManager.FlagKuaFuServerExplicit && GameManager.ServerLineID > 1);
								}
								else if (num124 == 9)
								{
									client.CheckCheatData.DisableAutoKuaFu = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (num124 == 10)
								{
									if (cmdFields.Length >= 3)
									{
										int npcID = Global.SafeConvertToInt32(cmdFields[2]);
										string junQiNameByBHID = JunQiManager.GetJunQiNameByBHID(client.ClientData.Faction);
										int junQiLevelByBHID = JunQiManager.GetJunQiLevelByBHID(client.ClientData.Faction);
										JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.MapCode, client.ClientData.Faction, client.ClientData.ZoneID, client.ClientData.BHName, npcID, junQiNameByBHID, junQiLevelByBHID, 0);
									}
								}
								else if (num124 == 11)
								{
									client.ClientData.WaitingNotifyChangeMap = !client.ClientData.WaitingNotifyChangeMap;
									text34 = client.ClientData.WaitingNotifyChangeMap.ToString();
								}
								else if (num124 == 12)
								{
									Global.UpdateRoleParamByName(client, "ChengJiuData", cmdFields[2], true);
									text34 = cmdFields[2];
								}
								else if (num124 == 13)
								{
									Dictionary<int, int> dictionary = new Dictionary<int, int>();
									int num125 = 0;
									int num126 = 0;
									for (int i = 0; i < 10000; i++)
									{
										int goodsPackID = Global.SafeConvertToInt32(cmdFields[2]);
										int num44 = Global.SafeConvertToInt32(cmdFields[3]);
										List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataList(client, goodsPackID, num44, 0, 1.0);
										if (goodsDataList != null && goodsDataList.Count > 0)
										{
											num125 += goodsDataList.Count;
										}
										else
										{
											num126++;
										}
									}
									text34 = string.Format("总计：{0}，轮空：{1}", num125, num126);
								}
								else if (num124 == 14)
								{
									if (client != null)
									{
										List<object> all9GridObjects = Global.GetAll9GridObjects(client);
										foreach (object obj in all9GridObjects)
										{
											if (obj is Monster)
											{
												GameManager.GoodsPackMgr.ProcessMonsterByClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as Monster);
												text34 = string.Format("执行怪物掉落：{0}", (obj as Monster).MonsterName);
											}
										}
									}
								}
								else if (num124 == 99)
								{
									RobotTaskValidator.getInstance().UseWorkThread = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (num124 == 100)
								{
									if (cmdFields.Length >= 4)
									{
										switch (Global.SafeConvertToInt32(cmdFields[2]))
										{
										case 1:
											text34 = string.Format("设置测试开关,跳过SocketData调用: {0}", false);
											break;
										case 2:
											text34 = string.Format("设置测试开关,跳过AddBuff调用: {0}", false);
											break;
										case 3:
											text34 = string.Format("设置测试开关,跳过TrySend调用: {0}", false);
											break;
										case 4:
											text34 = string.Format("设置测试开关,跳过Socket发送: {0}", false);
											break;
										default:
											text34 = string.Format("请输入子功能号(1-4)和值(0,1)", new object[0]);
											break;
										}
									}
									else
									{
										text34 = string.Format("请输入子功能号(1-4)和值(0,1)", new object[0]);
									}
								}
								text = string.Format("执行测试功能{0}{1}", num124, text34);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-WebOldPlayer" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							LogManager.WriteLog(2, "网页老玩家召回GM指令错误", null, true);
						}
						WebOldPlayerManager.getInstance().WebOldPlayerCheck(Global.SafeConvertToInt32(cmdFields[1]), Global.SafeConvertToInt32(cmdFields[2]));
					}
					else if ("-tiantidata" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 9)
							{
								text = string.Format("请输入： -tiantidata 角色名 duanWeiId, duanWeiJiFen, rongYao, monthDuanRank, lianSheng, successCount, fightCount", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text34 = "";
								string text2 = cmdFields[1];
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								int num128 = Global.SafeConvertToInt32(cmdFields[2]);
								int num129 = Global.SafeConvertToInt32(cmdFields[3]);
								int num130 = Global.SafeConvertToInt32(cmdFields[4]);
								int num131 = Global.SafeConvertToInt32(cmdFields[5]);
								int num132 = Global.SafeConvertToInt32(cmdFields[6]);
								int num133 = Global.SafeConvertToInt32(cmdFields[7]);
								int num134 = Global.SafeConvertToInt32(cmdFields[8]);
								TianTiManager.getInstance().GMSetRoleData(gameClient, num128, num129, num130, num131, num132, num133, num134);
								text = string.Format("根据GM的要求为{0}设置天梯信息：{1},{2},{3},{4},{5},{6},{7}", new object[]
								{
									text2,
									num128,
									num129,
									num130,
									num131,
									num132,
									num133,
									num134
								});
								LogManager.WriteLog(3, text, null, true);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-check" == cmdFields[0])
					{
						if (cmdFields.Length < 4)
						{
							text = string.Format("请输入： -check 0 600 5000 ", new object[0]);
						}
						else
						{
							CheckCheatTypes checkCheatTypes = (CheckCheatTypes)Global.SafeConvertToInt32(cmdFields[1]);
							int num135 = Global.SafeConvertToInt32(cmdFields[2]);
							int num136 = Global.SafeConvertToInt32(cmdFields[3]);
							CheckCheatTypes checkCheatTypes2 = checkCheatTypes;
							switch (checkCheatTypes2)
							{
							case CheckCheatTypes.MismatchMapCode:
								GameManager.CheckMismatchMapCode = (num135 > 0);
								break;
							case CheckCheatTypes.CheatPosition:
								GameManager.CheckCheatPosition = (num135 > 0);
								GameManager.CheckCheatMaxDistance = (double)Math.Max(num135, 600);
								GameManager.CheckPositionInterval = (double)Math.Max(num136, 1000);
								break;
							case CheckCheatTypes.DisableMovingOnAttack:
								GameManager.FlagDisableMovingOnAttack = (num135 > 0);
								break;
							default:
								switch (checkCheatTypes2)
								{
								case CheckCheatTypes.CheckClientDataOne:
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(num135);
									if (gameClient != null && gameClient.CheckCheatData.RobotTaskListData.Length > 0)
									{
										string strcmd = string.Format("{0}#{1}", gameClient.ClientData.RoleID, gameClient.CheckCheatData.RobotTaskListData);
										Global.ExecuteDBCmd(13111, strcmd, gameClient.ServerId);
									}
									break;
								}
								case CheckCheatTypes.CheckClientData:
								{
									int index = 0;
									GameClient nextClient;
									while ((nextClient = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
									{
										string text35 = string.Format("IP:{0},Client:{1}({2}){3}", new object[]
										{
											Global.GetSocketRemoteEndPoint(nextClient.ClientSocket, false),
											nextClient.ClientData.RoleName,
											nextClient.ClientData.RoleID,
											nextClient.CheckCheatData.RobotTaskListData
										});
										LogManager.WriteException(text35);
									}
									break;
								}
								}
								break;
							}
							text = string.Format("根据GM要求设置外挂检查（-check） {0} 为 {1} {2}", checkCheatTypes, num135, num136);
							LogManager.WriteLog(3, text, null, true);
						}
						if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-hideflags" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							text = string.Format("请输入： -hideflags 1 3000", new object[0]);
						}
						else
						{
							int num124 = Global.SafeConvertToInt32(cmdFields[1]);
							int num137 = Global.SafeConvertToInt32(cmdFields[2]);
							int num136 = (cmdFields.Length >= 4) ? Global.SafeConvertToInt32(cmdFields[3]) : 0;
							int num106 = num124;
							if (num106 == 1)
							{
								if (num136 > 0)
								{
									GameManager.HideFlagsMapDict.TryAdd(num137, num136);
								}
								else
								{
									GameManager.HideFlagsMapDict.TryRemove(num137, out num136);
								}
							}
							text = string.Format("根据GM要求设置效果屏蔽选项（-hideflags） {0} 为 {1} {2}", num124, num137, num136);
							LogManager.WriteLog(3, text, null, true);
						}
						if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-showprops" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -showprops 角色名", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								text = GlobalNew.PrintRoleProps(text2);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (text == null)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								LogManager.WriteException(text);
								string text5 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									-2,
									"",
									0,
									"",
									0,
									text,
									0
								});
								GameManager.ClientMgr.SendChatMessage(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text5);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string text2 = cmdFields[1];
							text = GlobalNew.PrintRoleProps(text2);
							int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
							if (text != null)
							{
								LogManager.WriteException(text);
							}
							return true;
						}
					}
					else if ("-showForce" == cmdFields[0])
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -showForce 角色名", new object[0]);
							LogManager.WriteLog(2, text, null, true);
						}
						else
						{
							string text26 = cmdFields[1];
							int num8 = RoleName2IDs.FindRoleIDByName(text26, false);
							if (-1 == num8)
							{
								text = string.Format("{0}不在线", text26);
								LogManager.WriteLog(2, text, null, true);
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
							if (null == gameClient)
							{
								text = string.Format("{0}不在线", text26);
								LogManager.WriteLog(2, text, null, true);
								return true;
							}
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append(text26).Append("\t角色属性及战斗力：\n");
							double minAttackV = RoleAlgorithm.GetMinAttackV(gameClient);
							stringBuilder.Append("物攻Min：").Append(minAttackV).Append("\n");
							double maxAttackV = RoleAlgorithm.GetMaxAttackV(gameClient);
							stringBuilder.Append("物攻Max：").Append(maxAttackV).Append("\n");
							double minADefenseV = RoleAlgorithm.GetMinADefenseV(gameClient);
							stringBuilder.Append("物防Min：").Append(minADefenseV).Append("\n");
							double maxADefenseV = RoleAlgorithm.GetMaxADefenseV(gameClient);
							stringBuilder.Append("物防Max：").Append(maxADefenseV).Append("\n");
							double minMagicAttackV = RoleAlgorithm.GetMinMagicAttackV(gameClient);
							stringBuilder.Append("魔攻Min：").Append(minMagicAttackV).Append("\n");
							double maxMagicAttackV = RoleAlgorithm.GetMaxMagicAttackV(gameClient);
							stringBuilder.Append("魔攻Max：").Append(maxMagicAttackV).Append("\n");
							double minMDefenseV = RoleAlgorithm.GetMinMDefenseV(gameClient);
							stringBuilder.Append("魔防Min：").Append(minMDefenseV).Append("\n");
							double maxMDefenseV = RoleAlgorithm.GetMaxMDefenseV(gameClient);
							stringBuilder.Append("魔防Min：").Append(maxMDefenseV).Append("\n");
							double hitV = RoleAlgorithm.GetHitV(gameClient);
							stringBuilder.Append("命中：").Append(hitV).Append("\n");
							double dodgeV = RoleAlgorithm.GetDodgeV(gameClient);
							stringBuilder.Append("闪避：").Append(dodgeV).Append("\n");
							double addAttackInjureValue = RoleAlgorithm.GetAddAttackInjureValue(gameClient);
							stringBuilder.Append("伤害加成：").Append(addAttackInjureValue).Append("\n");
							double decreaseInjureValue = RoleAlgorithm.GetDecreaseInjureValue(gameClient);
							stringBuilder.Append("伤害减少：").Append(decreaseInjureValue).Append("\n");
							double maxLifeV = RoleAlgorithm.GetMaxLifeV(gameClient);
							stringBuilder.Append("生命Max：").Append(maxLifeV).Append("\n");
							double maxMagicV = RoleAlgorithm.GetMaxMagicV(gameClient);
							stringBuilder.Append("魔法Max：").Append(maxMagicV).Append("\n");
							double lifeStealV = RoleAlgorithm.GetLifeStealV(gameClient);
							stringBuilder.Append("击中生命恢复：").Append(lifeStealV).Append("\n");
							double num138 = (double)GameManager.ElementsAttackMgr.GetElementAttack(gameClient, EElementDamageType.EEDT_Fire);
							stringBuilder.Append("火系伤害：").Append(num138).Append("\n");
							double num139 = (double)GameManager.ElementsAttackMgr.GetElementAttack(gameClient, EElementDamageType.EEDT_Water);
							stringBuilder.Append("水系伤害：").Append(num139).Append("\n");
							double num140 = (double)GameManager.ElementsAttackMgr.GetElementAttack(gameClient, EElementDamageType.EEDT_Lightning);
							stringBuilder.Append("雷系伤害：").Append(num140).Append("\n");
							double num141 = (double)GameManager.ElementsAttackMgr.GetElementAttack(gameClient, EElementDamageType.EEDT_Soil);
							stringBuilder.Append("土系伤害：").Append(num141).Append("\n");
							double num142 = (double)GameManager.ElementsAttackMgr.GetElementAttack(gameClient, EElementDamageType.EEDT_Ice);
							stringBuilder.Append("冰系伤害：").Append(num142).Append("\n");
							double num143 = (double)GameManager.ElementsAttackMgr.GetElementAttack(gameClient, EElementDamageType.EEDT_Wind);
							stringBuilder.Append("风系伤害：").Append(num143).Append("\n");
							double extProp = RoleAlgorithm.GetExtProp(gameClient, 122);
							stringBuilder.Append("HolyAttack：").Append(extProp).Append("\n");
							double extProp2 = RoleAlgorithm.GetExtProp(gameClient, 123);
							stringBuilder.Append("HolyDefense：").Append(extProp2).Append("\n");
							double extProp3 = RoleAlgorithm.GetExtProp(gameClient, 129);
							stringBuilder.Append("ShadowAttack：").Append(extProp3).Append("\n");
							double extProp4 = RoleAlgorithm.GetExtProp(gameClient, 130);
							stringBuilder.Append("ShadowDefense：").Append(extProp4).Append("\n");
							double extProp5 = RoleAlgorithm.GetExtProp(gameClient, 136);
							stringBuilder.Append("NatureAttack：").Append(extProp5).Append("\n");
							double extProp6 = RoleAlgorithm.GetExtProp(gameClient, 137);
							stringBuilder.Append("NatureDefense：").Append(extProp6).Append("\n");
							double extProp7 = RoleAlgorithm.GetExtProp(gameClient, 143);
							stringBuilder.Append("ChaosAttack：").Append(extProp7).Append("\n");
							double extProp8 = RoleAlgorithm.GetExtProp(gameClient, 144);
							stringBuilder.Append("ChaosDefense：").Append(extProp8).Append("\n");
							double extProp9 = RoleAlgorithm.GetExtProp(gameClient, 150);
							stringBuilder.Append("IncubusAttack：").Append(extProp9).Append("\n");
							double extProp10 = RoleAlgorithm.GetExtProp(gameClient, 151);
							stringBuilder.Append("IncubusDefense：").Append(extProp10).Append("\n");
							CombatForceInfo combatForceInfo = Data.CombatForceDataInfo[1];
							if (combatForceInfo != null)
							{
								double num144 = (minAttackV / combatForceInfo.MinPhysicsAttackModulus + maxAttackV / combatForceInfo.MaxPhysicsAttackModulus) / 2.0 + (minADefenseV / combatForceInfo.MinPhysicsDefenseModulus + maxADefenseV / combatForceInfo.MaxPhysicsDefenseModulus) / 2.0 + (minMagicAttackV / combatForceInfo.MinMagicAttackModulus + maxMagicAttackV / combatForceInfo.MaxMagicAttackModulus) / 2.0 + (minMDefenseV / combatForceInfo.MinMagicDefenseModulus + maxMDefenseV / combatForceInfo.MaxMagicDefenseModulus) / 2.0 + addAttackInjureValue / combatForceInfo.AddAttackInjureModulus + decreaseInjureValue / combatForceInfo.DecreaseInjureModulus + hitV / combatForceInfo.HitValueModulus + dodgeV / combatForceInfo.DodgeModulus + maxLifeV / combatForceInfo.MaxHPModulus + maxMagicV / combatForceInfo.MaxMPModulus + lifeStealV / combatForceInfo.LifeStealModulus;
								num144 += num138 / combatForceInfo.FireAttack + num139 / combatForceInfo.WaterAttack + num140 / combatForceInfo.LightningAttack + num141 / combatForceInfo.SoilAttack + num142 / combatForceInfo.IceAttack + num143 / combatForceInfo.WindAttack;
								num144 += extProp / combatForceInfo.HolyAttack + extProp2 / combatForceInfo.HolyDefense + extProp3 / combatForceInfo.ShadowAttack + extProp4 / combatForceInfo.ShadowDefense + extProp5 / combatForceInfo.NatureAttack + extProp6 / combatForceInfo.NatureDefense + extProp7 / combatForceInfo.ChaosAttack + extProp8 / combatForceInfo.ChaosDefense + extProp9 / combatForceInfo.IncubusAttack + extProp10 / combatForceInfo.IncubusDefense;
								stringBuilder.Append("战斗力：").Append(num144).Append("\n");
							}
							stringBuilder.Append("重生战斗力：").Append(RebornManager.getInstance().CalculateCombatForce(gameClient)).Append("\n");
							LogManager.WriteLog(2, stringBuilder.ToString(), null, true);
						}
					}
					else if ("-nameserver" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -nameserver 1或0(1启用 0禁用)} [名字服务器IP] [名字服务器端口] [本服务器的平台ID编号]\n中括号内的参数可不填,'*'号表示不修改", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text34 = "";
								int num124 = Global.SafeConvertToInt32(cmdFields[1]);
								if (num124 == 0)
								{
									Global.Flag_NameServer = false;
									text34 = string.Format("禁用名字服务器,不通过验证就可创建", new object[0]);
								}
								else
								{
									Global.Flag_NameServer = true;
									if (cmdFields.Length >= 3 && cmdFields[2] != "*")
									{
										NameServerNamager.NameServerIP = cmdFields[2];
									}
									if (cmdFields.Length >= 4 && cmdFields[3] != "*")
									{
										NameServerNamager.NameServerPort = this.SafeConvertToInt32(cmdFields[3]);
									}
									if (cmdFields.Length >= 5 && cmdFields[4] != "*")
									{
										NameServerNamager.NameServerConfig = this.SafeConvertToInt32(cmdFields[4]);
									}
									text34 = string.Format("启用名字服务器，需通过名字认证服务器验证才可创建，\nIP：{0}    端口：{1}    配置选项：{2}", NameServerNamager.NameServerIP, NameServerNamager.NameServerPort, NameServerNamager.NameServerConfig);
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text34);
							}
						}
					}
					else if ("-testproc" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -testproc 功能号 {参数1|参数2|参数3...}", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text34 = "";
								int num124 = Global.SafeConvertToInt32(cmdFields[1]);
								if (num124 == 0)
								{
									GameManager.AngelTempleMgr.GiveAwardAngelTempleScene(cmdFields.Length >= 3 && cmdFields[2] == "1");
								}
								else if (num124 == 1)
								{
									int num125 = 0;
									int num126 = 0;
									for (int i = 0; i < 10000; i++)
									{
										int goodsPackID = Global.SafeConvertToInt32(cmdFields[2]);
										int num44 = Global.SafeConvertToInt32(cmdFields[3]);
										List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataList(client, goodsPackID, num44, 0, 1.0);
										if (goodsDataList != null && goodsDataList.Count > 0)
										{
											num125 += goodsDataList.Count;
										}
										else
										{
											num126++;
										}
									}
									text34 = string.Format("总计：{0}，轮空：{1}", num125, num126);
								}
								else if (num124 == 2)
								{
									LuoLanChengZhanManager.getInstance().GMStartHuoDongNow();
								}
								else if (num124 == 3)
								{
									GameManager.AngelTempleMgr.GMSetHuoDongStartNow();
								}
								else if (num124 == 4)
								{
									if (cmdFields.Length >= 3)
									{
										TianTiManager.getInstance().GMStartHuoDongNow(Global.SafeConvertToInt32(cmdFields[2]));
									}
								}
								else if (num124 == 5)
								{
									if (cmdFields.Length >= 3)
									{
										SingletonTemplate<CreateRoleLimitManager>.Instance().CreateRoleLimitMinutes = Global.SafeConvertToInt32(cmdFields[2]);
									}
								}
								else if (num124 != 6)
								{
									if (num124 == 1000)
									{
										GMCommands.GMSetTime(client, cmdFields, false);
									}
								}
								text = string.Format("执行测试功能{0}{1}", num124, text34);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-testkf" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -testkf 功能号 {参数1|参数2|参数3...}", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text34 = "";
								int num124 = Global.SafeConvertToInt32(cmdFields[1]);
								if (num124 == 1)
								{
									text34 = string.Format("总计：{0}，轮空：{1}", 0, 0);
								}
								text = string.Format("执行测试功能{0}{1}", num124, text34);
								LogManager.WriteLog(3, text, null, true);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-settime" == cmdFields[0])
					{
						if (GMCommands.EnableGMSetAllServerTime)
						{
							LogManager.WriteLog(3, msgText, null, true);
							GMCommands.GMSetTime(client, cmdFields, true);
						}
						else
						{
							LogManager.WriteLog(3, "修改时间功能未开启", null, true);
						}
					}
					else if ("-hidefakerole" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -hidefakerole 1或0(1隐藏 0不隐藏)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								GameManager.TestGameShowFakeRoleForUser = (Global.SafeConvertToInt32(cmdFields[1]) == 0);
								text = (GameManager.TestGameShowFakeRoleForUser ? "设置为显示(挂机)假人" : "设置为隐藏(挂机)假人");
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-addxh" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -adddj 角色名称 声望值(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num123 = this.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameManager.ClientMgr.ModifyStarSoulValue(client, num123, "GM增加", true, true);
								text = string.Format("为{0}添加了星魂{1}", text2, num123);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-stminsendsize" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								SendBuffer.ConstMinSendSize = Global.SafeConvertToInt32(cmdFields[1]);
								text = string.Format(string.Format("发送缓冲的大小：最小缓冲={0}", SendBuffer.ConstMinSendSize), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = string.Format("发送缓冲的大小设置：-stminsendsize 最小缓冲", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-fazhenmen" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int num145 = this.SafeConvertToInt32(cmdFields[1]);
								if (num145 == 1 || num145 == 0)
								{
									LuoLanFaZhenCopySceneManager.GM_SetOpenState(num145);
									if (num145 == 1)
									{
										text = "显示所有传送门的隐藏状态";
									}
									else if (num145 == 0)
									{
										text = "隐藏所有传送门的状态";
									}
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								}
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-showlingyu" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 1)
							{
								List<LingYuData> lingYuList = LingYuManager.GetLingYuList(client);
								text = string.Format("翎羽数量：{0}", lingYuList.Count<LingYuData>());
								for (int i = 0; i < lingYuList.Count<LingYuData>(); i++)
								{
									text += string.Format(" [Type：{0}, Level：{1}, Suit：{2}] ", lingYuList[i].Type, lingYuList[i].Level, lingYuList[i].Suit);
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-wingmax" == cmdFields[0])
					{
						if (!transmit)
						{
							LingYuManager.SetLingYuMax_GM(client);
							ZhuLingZhuHunManager.SetZhuLingMax_GM(client);
							ZhuLingZhuHunManager.SetZhuHunMax_GM(client);
							text = string.Format("翎羽、注灵、注魂满级！", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-lingyulevelup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								int num10 = this.SafeConvertToInt32(cmdFields[1]);
								int useZuanshiIfNoMaterial = this.SafeConvertToInt32(cmdFields[2]);
								LingYuError lyError = LingYuManager.AdvanceLingYuLevel(client, client.ClientData.RoleID, num10, useZuanshiIfNoMaterial);
								text = LingYuManager.Error2Str(lyError);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-lingyusuitup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								int num10 = this.SafeConvertToInt32(cmdFields[1]);
								int useZuanshiIfNoMaterial = this.SafeConvertToInt32(cmdFields[2]);
								LingYuError lyError = LingYuManager.AdvanceLingYuSuit(client, client.ClientData.RoleID, num10, useZuanshiIfNoMaterial);
								text = LingYuManager.Error2Str(lyError);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-showzhulingzhuhun" == cmdFields[0])
					{
						if (!transmit)
						{
							text = string.Format("zhuling：{0}, zhuhun：{1}", client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-wingzhuling" == cmdFields[0])
					{
						if (!transmit)
						{
							ZhuLingZhuHunError zhuLingZhuHunError = ZhuLingZhuHunManager.ReqZhuLing(client);
							text = string.Format("zhuling：{0}, zhuhun：{1}, error：{2}", client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum, zhuLingZhuHunError.ToString());
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-wingzhuhun" == cmdFields[0])
					{
						if (!transmit)
						{
							ZhuLingZhuHunError zhuLingZhuHunError = ZhuLingZhuHunManager.ReqZhuHun(client);
							text = string.Format("zhuling：{0}, zhuhun：{1}, error：{2}", client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum, zhuLingZhuHunError.ToString());
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-setwingsuitstar" == cmdFields[0])
					{
						if (!transmit)
						{
							this.GMSetWingSuitStar(client, cmdFields);
						}
					}
					else if ("-achievement" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text36 = cmdFields[1];
								if (text36 != null)
								{
									if (!(text36 == "level"))
									{
										if (!(text36 == "runeLevel"))
										{
											if (!(text36 == "runeCount"))
											{
												if (text36 == "runeRate")
												{
													ChengJiuManager.SetAchievementRuneRate(client, int.Parse(cmdFields[2]));
												}
											}
											else
											{
												ChengJiuManager.SetAchievementRuneCount(client, int.Parse(cmdFields[2]));
											}
										}
										else
										{
											ChengJiuManager.SetAchievementRuneLevel(client, int.Parse(cmdFields[2]));
										}
									}
									else
									{
										ChengJiuManager.SetAchievementLevel(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-prestige" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text36 = cmdFields[1];
								if (text36 != null)
								{
									if (!(text36 == "level"))
									{
										if (!(text36 == "medalLevel"))
										{
											if (!(text36 == "medalCount"))
											{
												if (text36 == "medalRate")
												{
													PrestigeMedalManager.SetPrestigeMedalRate(client, int.Parse(cmdFields[2]));
												}
											}
											else
											{
												PrestigeMedalManager.SetPrestigeMedalCount(client, int.Parse(cmdFields[2]));
											}
										}
										else
										{
											PrestigeMedalManager.SetPrestigeMedalLevel(client, int.Parse(cmdFields[2]));
										}
									}
									else
									{
										PrestigeMedalManager.SetPrestigeLevel(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-deleterole" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 4)
							{
								string text3 = cmdFields[1];
								int num8 = Global.SafeConvertToInt32(cmdFields[2]);
								int zoneID = Global.SafeConvertToInt32(cmdFields[3]);
								TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(text3);
								if (null == tmsksocket)
								{
									EventLogManager.AddRemoveRoleEvent(text3, zoneID, num8, "");
									return true;
								}
								if (tmsksocket.session.SocketState < 4)
								{
									string text5 = string.Format("{0}", num8);
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, text5, 103);
									Global._TCPManager.MySocketListener.SendData(tmsksocket, tcpOutPacket, true);
								}
								string ip = Global.GetSocketRemoteEndPoint(tmsksocket, false).Replace(":", ".");
								EventLogManager.AddRemoveRoleEvent(text3, zoneID, num8, ip);
							}
						}
					}
					else if ("-artifact" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text36 = cmdFields[1];
								if (text36 != null)
								{
									if (text36 == "failCount")
									{
										ArtifactManager.SetArtifactFailCount(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-die" == cmdFields[0])
					{
						if (!transmit)
						{
							client.ClientData.CurrentLifeV = 0;
							Global.RemoveBufferData(client, 86);
							Global.RemoveBufferData(client, 85);
							client.ClientData.LastRoleDeadTicks = TimeUtil.NOW();
							GameManager.SystemServerEvents.AddEvent(string.Format("角色强制死亡, roleID={0}({1}), Life={2}", client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.CurrentLifeV), EventLevels.Debug);
							GameManager.ClientMgr.NotifySpriteInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, -1, client.ClientData.RoleID, 0, 0, 0.0, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					else if ("-mailAddId" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 9)
							{
								text = string.Format("请输入： -mailAddId 角色名称 物品ID 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num31 = this.SafeConvertToInt32(cmdFields[2]);
								int num32 = this.SafeConvertToInt32(cmdFields[3]);
								num32 = Global.GMax(0, num32);
								num32 = Global.GMin(int.MaxValue, num32);
								int num33 = this.SafeConvertToInt32(cmdFields[4]);
								int num34 = this.SafeConvertToInt32(cmdFields[5]);
								int num35 = this.SafeConvertToInt32(cmdFields[6]);
								int lucky = this.SafeConvertToInt32(cmdFields[7]);
								int num36 = this.SafeConvertToInt32(cmdFields[8]);
								num34 = Global.GMax(0, num34);
								num34 = Global.GMin(15, num34);
								int num57 = 0;
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								SystemXmlItem systemXmlItem = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num31, out systemXmlItem))
								{
									text = string.Format("系统中不存在{0}", num31);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								int num37 = 0;
								int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
								if (intValue >= 800 && intValue < 816)
								{
									num37 = 3000;
								}
								GoodsData goodsData = new GoodsData
								{
									Id = -1,
									GoodsID = num31,
									Using = 0,
									Forge_level = num34,
									Starttime = "1900-01-01 12:00:00",
									Endtime = "1900-01-01 12:00:00",
									Site = 0,
									Quality = 0,
									Props = "",
									GCount = 1,
									Binding = num33,
									Jewellist = "",
									BagIndex = 0,
									AddPropIndex = 0,
									BornIndex = 0,
									Lucky = lucky,
									Strong = 0,
									ExcellenceInfo = num36,
									AppendPropLev = num35,
									ChangeLifeLevForEquip = 0
								};
								Global.UseMailGivePlayerAward(client, goodsData, "GM邮件", "GM邮件奖励", 1.0);
								text = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
								{
									text2,
									num31.ToString(),
									num32,
									num34,
									num57,
									num33
								});
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-rmgood" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							string text37 = "-rmgoods参数错误，Usage：-rmgoods roleid goodid";
							LogManager.WriteLog(2, text37, null, true);
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
						else
						{
							int num8 = this.SafeConvertToInt32(cmdFields[1]);
							int num146 = this.SafeConvertToInt32(cmdFields[2]);
							GameClient gameClient = (num8 != -1) ? GameManager.ClientMgr.FindClient(num8) : null;
							if (null == gameClient)
							{
								RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, num8), 0);
								GoodsData goodsData = null;
								if (null != roleDataEx)
								{
									if (null == roleDataEx.GoodsDataList)
									{
										LogManager.WriteLog(2, "删除角色道具，但是查不到角色道具数据。", null, true);
									}
									else
									{
										lock (roleDataEx.GoodsDataList)
										{
											for (int i = 0; i < roleDataEx.GoodsDataList.Count; i++)
											{
												if (roleDataEx.GoodsDataList[i].Id == num146)
												{
													goodsData = roleDataEx.GoodsDataList[i];
													break;
												}
											}
										}
										if (null == goodsData)
										{
											return true;
										}
										int num32 = goodsData.GCount;
										string[] array = null;
										string text5 = Global.FormatUpdateDBGoodsStr(new object[]
										{
											num8,
											num146,
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
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
											"*",
											"*",
											"*"
										});
										TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, text5, out array, 0);
										if (array == null || array.Length < 2 || Convert.ToInt32(array[1]) < 0)
										{
											LogManager.WriteLog(2, "删除角色道具，但是角色离线，所以转到db处理，" + text5 + ", 但是db处理失败", null, true);
										}
										else
										{
											goodsData.GCount = 0;
											Global.ModRoleGoodsEvent(roleDataEx, goodsData, -num32, "客户端修改", false);
											EventLogManager.AddGoodsEvent(roleDataEx, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "客户端修改");
										}
									}
								}
								else
								{
									LogManager.WriteLog(2, "删除角色道具，但是查不到角色数据。", null, true);
								}
							}
							else
							{
								GoodsData goodsData = Global.GetGoodsByDbID(gameClient, num146);
								if (null != goodsData)
								{
									string cmdData2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										gameClient.ClientData.RoleID,
										4,
										goodsData.Id,
										goodsData.GoodsID,
										0,
										goodsData.Site,
										goodsData.GCount,
										goodsData.BagIndex,
										""
									});
									int gcount = goodsData.GCount;
									if (TCPProcessCmdResults.RESULT_OK != Global.ModifyGoodsByCmdParams(client, cmdData2, "客户端修改", null))
									{
										LogManager.WriteLog(2, string.Concat(new string[]
										{
											" -rmgoods 失败:",
											cmdFields[0],
											" ",
											cmdFields[1],
											" ",
											cmdFields[2]
										}), null, true);
									}
								}
							}
						}
					}
					else if ("-subRoleHuobi" == cmdFields[0])
					{
						bool flag8 = 1 == 0;
						if (cmdFields.Length < 4)
						{
							string text37 = "-subRoleHuobi参数错误，Usage：-subRoleHuobi roleid GMHuoBiType value";
							LogManager.WriteLog(2, text37, null, true);
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
						else
						{
							int num8 = this.SafeConvertToInt32(cmdFields[1]);
							int num147 = Global.SafeConvertToInt32(cmdFields[2]);
							int num105 = -this.SafeConvertToInt32(cmdFields[3]);
							GameClient gameClient = (num8 != -1) ? GameManager.ClientMgr.FindClient(num8) : null;
							bool flag9 = true;
							if (null == gameClient)
							{
								string text38 = string.Format("{0}:{1}:{2}", num8, num147, num105);
								LogManager.WriteLog(2, "修改角色货币，但是角色离线，所以转到db处理，" + text38, null, true);
								string[] array = Global.ExecuteDBCmd(20398, text38, 0);
								if (array == null || array.Length < 2 || Convert.ToInt32(array[1]) < 0)
								{
									LogManager.WriteLog(2, "修改角色货币，但是角色离线，所以转到db处理，" + text38 + ", 但是db处理失败", null, true);
									flag9 = false;
								}
							}
							else
							{
								int num106 = num147;
								if (num106 <= 40)
								{
									if (num106 <= 8)
									{
										if (num106 == 1)
										{
											flag9 = GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, num105, "GM指令-绑金", true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色绑金, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												gameClient.ClientData.Money1
											}), EventLevels.Record);
											goto IL_1784E;
										}
										if (num106 == 8)
										{
											flag9 = GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, -num105, "GM指令-金币", true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色金币, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												gameClient.ClientData.YinLiang
											}), EventLevels.Record);
											goto IL_1784E;
										}
									}
									else
									{
										switch (num106)
										{
										case 13:
											flag9 = GameManager.ClientMgr.ModifyTianDiJingYuanValue(gameClient, num105, "GM指令-魔晶", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色魔晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												GameManager.ClientMgr.GetTianDiJingYuanValue(gameClient)
											}), EventLevels.Record);
											goto IL_1784E;
										case 14:
											break;
										case 15:
											flag9 = Global.AddZaJinDanJiFen(gameClient, num105, "GM指令-祈福积分", true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色再造点, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen")
											}), EventLevels.Record);
											goto IL_1784E;
										default:
											if (num106 == 40)
											{
												flag9 = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, -num105, "GM指令-钻石", false, false, true, DaiBiSySType.None);
												GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色钻石, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
												{
													gameClient.ClientData.RoleID,
													gameClient.ClientData.RoleName,
													num105,
													gameClient.ClientData.UserMoney
												}), EventLevels.Record);
												goto IL_1784E;
											}
											break;
										}
									}
								}
								else if (num106 <= 101)
								{
									if (num106 == 50)
									{
										flag9 = GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, -num105, "GM指令-绑钻", true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色绑钻, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											gameClient.ClientData.RoleID,
											gameClient.ClientData.RoleName,
											num105,
											gameClient.ClientData.Gold
										}), EventLevels.Record);
										goto IL_1784E;
									}
									if (num106 == 101)
									{
										flag9 = GameManager.ClientMgr.ModifyZaiZaoValue(gameClient, num105, "GM指令-再造点", true, true, true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色再造点, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											gameClient.ClientData.RoleID,
											gameClient.ClientData.RoleName,
											num105,
											GameManager.ClientMgr.GetZaiZaoValue(gameClient)
										}), EventLevels.Record);
										goto IL_1784E;
									}
								}
								else
								{
									switch (num106)
									{
									case 106:
										flag9 = GameManager.ClientMgr.ModifyMUMoHeValue(gameClient, num105, "GM指令-灵晶", true, true, true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色灵晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											gameClient.ClientData.RoleID,
											gameClient.ClientData.RoleName,
											num105,
											GameManager.ClientMgr.GetMUMoHeValue(gameClient)
										}), EventLevels.Record);
										goto IL_1784E;
									case 107:
										flag9 = GameManager.ClientMgr.ModifyYuanSuFenMoValue(gameClient, num105, "GM指令-元素粉末", true, true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色元素粉末, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											gameClient.ClientData.RoleID,
											gameClient.ClientData.RoleName,
											num105,
											Global.GetRoleParamsInt32FromDB(gameClient, "ElementPowder")
										}), EventLevels.Record);
										goto IL_1784E;
									case 108:
										break;
									case 109:
										flag9 = GameManager.FluorescentGemMgr.DecFluorescentPoint(gameClient, -num105, "GM指令-荧光粉末", true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色荧光粉末, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											gameClient.ClientData.RoleID,
											gameClient.ClientData.RoleName,
											num105,
											gameClient.ClientData.FluorescentPoint
										}), EventLevels.Record);
										goto IL_1784E;
									default:
										if (num106 == 119)
										{
											flag9 = GameManager.ClientMgr.ModifyOrnamentCharmPointValue(gameClient, num105, "GM指令-魅力点数", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色魅力点数, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(gameClient, "10153")
											}), EventLevels.Record);
											goto IL_1784E;
										}
										switch (num106)
										{
										case 129:
											flag9 = GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(gameClient, num105, "GM指令-符文之尘点数", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色符文之尘点数, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(gameClient, "10187")
											}), EventLevels.Record);
											goto IL_1784E;
										case 130:
											flag9 = GameManager.ClientMgr.ModifyAlchemyElementValue(gameClient, num105, "GM指令-炼金元素", true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色炼金元素, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												gameClient.ClientData.AlchemyInfo.BaseData.Element
											}), EventLevels.Record);
											goto IL_1784E;
										case 131:
											flag9 = GameManager.ClientMgr.ModifyJueXingZhiChenValue(gameClient, num105, "GM指令-觉醒之尘", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色觉醒之尘, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(gameClient, "10194")
											}), EventLevels.Record);
											goto IL_1784E;
										case 132:
											flag9 = GameManager.ClientMgr.ModifyHunJingValue(gameClient, num105, "GM指令-坐骑饲料", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色坐骑饲料, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(gameClient, "10208")
											}), EventLevels.Record);
											goto IL_1784E;
										case 133:
											flag9 = GameManager.ClientMgr.ModifyMountPointValue(gameClient, num105, "GM指令-坐骑点数", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色坐骑点数, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(gameClient, "10209")
											}), EventLevels.Record);
											goto IL_1784E;
										case 135:
											flag9 = GameManager.ClientMgr.ModifyCompDonateValue(gameClient, num105, "GM指令-势力贡献度", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色势力贡献度, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												num105,
												Global.GetRoleParamsInt32FromDB(gameClient, "10204")
											}), EventLevels.Record);
											goto IL_1784E;
										}
										break;
									}
								}
								LogManager.WriteLog(2, " -modifyRoleHuobi 未注册的货币类型:" + num147, null, true);
								IL_1784E:;
							}
							GameManager.logDBCmdMgr.AddMessageLog(-1, "货币" + num147, "-subRoleHuobi", flag9 ? "成功" : "失败", client.ClientData.RoleName, "GM命令", num105, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, "");
						}
					}
					else if ("-modifyRoleHuobi" == cmdFields[0])
					{
						bool flag8 = 1 == 0;
						if (cmdFields.Length < 4)
						{
							string text37 = "-modifyRoleHuobi参数错误，Usage：-modifyRoleHuobi roleid HuobiType value";
							LogManager.WriteLog(2, text37, null, true);
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
						else
						{
							int num8 = this.SafeConvertToInt32(cmdFields[1]);
							string text39 = cmdFields[2].ToLower();
							int num105 = this.SafeConvertToInt32(cmdFields[3]);
							GameClient gameClient = (num8 != -1) ? GameManager.ClientMgr.FindClient(num8) : null;
							if (null == gameClient)
							{
								if ("lingjing" == text39 || "mojing" == text39 || "zaizao" == text39)
								{
									string text38 = string.Format("{0}:{1}:{2}", num8, text39, num105);
									LogManager.WriteLog(2, "修改角色货币，但是角色离线，所以转到db处理，" + text38, null, true);
									string[] array = Global.ExecuteDBCmd(10182, text38, 0);
									if (array == null || array.Length < 2 || Convert.ToInt32(array[1]) < 0)
									{
										LogManager.WriteLog(2, "修改角色货币，但是角色离线，所以转到db处理，" + text38 + ", 但是db处理失败", null, true);
									}
								}
							}
							else if (!("jinbi" == text39))
							{
								if (!("bangjin" == text39))
								{
									if (!("zuanshi" == text39))
									{
										if (!("bangzuan" == text39))
										{
											if ("mojing" == text39)
											{
												GameManager.ClientMgr.ModifyTianDiJingYuanValue(gameClient, num105, "GM指令增加魔晶", true, true, false);
												GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色魔晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
												{
													gameClient.ClientData.RoleID,
													gameClient.ClientData.RoleName,
													num105,
													GameManager.ClientMgr.GetTianDiJingYuanValue(gameClient)
												}), EventLevels.Record);
											}
											else if (!("chengjiu" == text39))
											{
												if (!("shengwang" == text39))
												{
													if (!("xinghun" == text39))
													{
														if ("lingjing" == text39)
														{
															GameManager.ClientMgr.ModifyMUMoHeValue(gameClient, num105, "GM命令", true, true, false);
															GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色灵晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
															{
																gameClient.ClientData.RoleID,
																gameClient.ClientData.RoleName,
																num105,
																GameManager.ClientMgr.GetMUMoHeValue(gameClient)
															}), EventLevels.Record);
														}
														else if (!("fenmo" == text39))
														{
															if ("zaizao" == text39)
															{
																GameManager.ClientMgr.ModifyZaiZaoValue(gameClient, num105, "GM指令增加再造点", true, true, false);
																GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色灵晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
																{
																	gameClient.ClientData.RoleID,
																	gameClient.ClientData.RoleName,
																	num105,
																	GameManager.ClientMgr.GetZaiZaoValue(gameClient)
																}), EventLevels.Record);
															}
															else
															{
																LogManager.WriteLog(2, " -modifyRoleHuobi 未注册的货币类型:" + text39, null, true);
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
					else if ("-resethefuluolan" == cmdFields[0])
					{
						Global.UpdateDBGameConfigg("hefu_luolan_guildid", "");
					}
					else if ("-clearsecondpassword" == cmdFields[0])
					{
						if (cmdFields.Length < 2)
						{
							if (!transmit)
							{
								string text37 = "-modifyRoleHuobi参数错误，Usage：-clearsecondpassword userid";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
						else
						{
							string text40 = cmdFields[1];
							string text37 = string.Format("-clearsecondpassword {0}", text40);
							if (SecondPasswordManager.ClearUserSecPwd(text40))
							{
								text37 += " 成功";
							}
							else
							{
								text37 += " 失败";
							}
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
					}
					else if ("-monroledamage" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int mapCode = this.SafeConvertToInt32(cmdFields[1]);
							int num148 = this.SafeConvertToInt32(cmdFields[2]);
							GameManager.damageMonitor.Set(mapCode, num148);
						}
					}
					else if ("-nomonroledamage" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int mapCode = this.SafeConvertToInt32(cmdFields[1]);
							int num148 = this.SafeConvertToInt32(cmdFields[2]);
							GameManager.damageMonitor.Remove(mapCode, num148);
						}
					}
					else if ("-GuardStatueModEquip" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								string text37 = "-GuardStatueModEquip参数错误，Usage：-GuardStatueModEquip slot soulType(-1表示脱装备)";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
							else
							{
								int slot = Convert.ToInt32(cmdFields[1]);
								int guardSoulType = Convert.ToInt32(cmdFields[2]);
								SingletonTemplate<GuardStatueManager>.Instance().GM_HandleModGuardSoulEquip(client, slot, guardSoulType);
							}
						}
					}
					else if ("-GuardPointQuery" == cmdFields[0])
					{
						if (!transmit)
						{
							string text37 = SingletonTemplate<GuardStatueManager>.Instance().GM_QueryGuardPoint(client);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
						}
					}
					else if ("-GuardPointRecover" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length <= 1)
							{
								string text37 = "-GuardPointRecover参数错误，Usage：-GuardPointRecover item,cnt item,cnt";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
							else
							{
								Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
								for (int i = 1; i < cmdFields.Length; i++)
								{
									string[] array13 = cmdFields[i].Split(new char[]
									{
										','
									});
									if (array13 != null && array13.Length == 2)
									{
										int num149 = Convert.ToInt32(array13[0]);
										int num150 = Convert.ToInt32(array13[1]);
										if (dictionary2.ContainsKey(num149))
										{
											Dictionary<int, int> dictionary3;
											int key;
											(dictionary3 = dictionary2)[key = num149] = dictionary3[key] + num150;
										}
										else
										{
											dictionary2.Add(num149, num150);
										}
									}
								}
								SingletonTemplate<GuardStatueManager>.Instance().GM_GuardPointRecover(client, dictionary2);
							}
						}
					}
					else if ("-GuardStatueLevelUp" == cmdFields[0])
					{
						if (!transmit)
						{
							SingletonTemplate<GuardStatueManager>.Instance().GM_HandleLevelUp(client);
						}
					}
					else if ("-GuardStatueSuitUp" == cmdFields[0])
					{
						if (!transmit)
						{
							SingletonTemplate<GuardStatueManager>.Instance().GM_HandleSuitlUp(client);
						}
					}
					else if ("-GuardStatueQuery" == cmdFields[0])
					{
						if (!transmit)
						{
							string text37 = SingletonTemplate<GuardStatueManager>.Instance().GM_QueryGuardStatue(client);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
						}
					}
					else if ("-ModGuardPoint" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int newVal = Convert.ToInt32(cmdFields[1]);
								string text37 = SingletonTemplate<GuardStatueManager>.Instance().GM_ModGuardPoint(client, newVal);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
					}
					else if ("-ChangeName" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								SingletonTemplate<NameManager>.Instance().GM_ChangeNameTest(client, cmdFields[1]);
							}
						}
					}
					else if ("-MerlinStarUp1" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinStarUp1(client);
						}
					}
					else if ("-MerlinStarUp2" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinStarUp2(client);
						}
					}
					else if ("-MerlinLevelUp1" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinLevelUp1(client);
						}
					}
					else if ("-MerlinLevelUp2" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinLevelUp2(client);
						}
					}
					else if ("-MerlinSecret1" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinSecretUpdate(client);
						}
					}
					else if ("-MerlinSecret2" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinSecretReplace(client);
						}
					}
					else if ("-MerlinSecret3" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinSecretNotReplace(client);
						}
					}
					else if ("-MerlinInit" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinInit(client);
						}
					}
					else if ("-MerlinLevelUp" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								string text37 = GameManager.MerlinMagicBookMgr.GMMerlinLevelUpToN(client, Global.SafeConvertToInt32(cmdFields[1]));
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
					}
					else if ("-MerlinStarUp" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								string text37 = GameManager.MerlinMagicBookMgr.GMMerlinStarUpToN(client, Global.SafeConvertToInt32(cmdFields[1]));
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
					}
					else if ("-addholypart" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -addholypart 圣物id 部件id 碎片数量", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num151 = Global.SafeConvertToInt32(cmdFields[1]);
								int num152 = Global.SafeConvertToInt32(cmdFields[2]);
								int nNum2 = Global.SafeConvertToInt32(cmdFields[3]);
								HolyItemManager.getInstance().GetHolyItemPart(client, (sbyte)num151, (sbyte)num152, nNum2);
							}
						}
					}
					else if ("-addHolyPart" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -addholypart  碎片数量", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int nNum2 = Global.SafeConvertToInt32(cmdFields[1]);
								for (int l = 1; l <= 4; l++)
								{
									for (int m = 1; m <= 6; m++)
									{
										HolyItemManager.getInstance().GetHolyItemPart(client, (sbyte)l, (sbyte)m, nNum2);
									}
								}
							}
						}
					}
					else if ("-holyitemlvup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -holyitemlvup 圣物id 部件id 阶数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int num151 = Global.SafeConvertToInt32(cmdFields[1]);
								int num152 = Global.SafeConvertToInt32(cmdFields[2]);
								int num153 = Global.SafeConvertToInt32(cmdFields[3]);
								HolyItemManager.getInstance().GMSetHolyItemLvup(client, (sbyte)num151, (sbyte)num152, (sbyte)num153);
							}
						}
					}
					else if ("-tarotlvup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								text = string.Format("请输入： -tarotlvup 圣物id 部件id", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								int goodID = Convert.ToInt32(cmdFields[1]);
								string text41 = Convert.ToInt32(TarotManager.getInstance().ProcessTarotUpCmd(client, goodID)).ToString();
							}
						}
					}
					else if ("-talentCount" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								string text37 = "-talentCount参数错误，Usage：-talentCount 数量";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
								return true;
							}
							int num44 = Convert.ToInt32(cmdFields[1]);
							if (num44 < 0 || num44 > 999)
							{
								string text37 = "天赋点范围【1-999】";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
								return true;
							}
							if (!TalentManager.TalentAddCount(client, num44))
							{
								string text37 = "天赋未开放，或者添加天赋点错误";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text37);
							}
						}
					}
					else if ("-socketcheck" == cmdFields[0])
					{
						long num154 = Convert.ToInt64(cmdFields[1]);
						num154 = num154 * 60L * 1000L;
						List<TMSKSocket> socketList = GameManager.OnlineUserSession.GetSocketList();
						foreach (TMSKSocket tmsksocket2 in socketList)
						{
							long num155 = TimeUtil.NOW();
							long num156 = num155 - tmsksocket2.session.SocketTime[0];
							if (tmsksocket2.session.SocketState < 4 && num156 > num154)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket2);
								if (null == gameClient)
								{
									Global.ForceCloseSocket(tmsksocket2, "被GM踢了, 但是这个socket上没有对应的client", true);
								}
							}
						}
					}
					else if ("-socketcount" == cmdFields[0])
					{
						int num44 = 0;
						long num154 = Convert.ToInt64(cmdFields[1]);
						num154 = num154 * 60L * 1000L;
						List<TMSKSocket> socketList = GameManager.OnlineUserSession.GetSocketList();
						foreach (TMSKSocket tmsksocket2 in socketList)
						{
							long num155 = TimeUtil.NOW();
							long num156 = num155 - tmsksocket2.session.SocketTime[0];
							if (tmsksocket2.session.SocketState < 4 && num156 > num154)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(tmsksocket2);
								if (null == gameClient)
								{
									num44++;
								}
							}
						}
						string text37 = string.Format("socket总数量：{0}，问题socket：{1}", socketList.Count, num44);
						LogManager.WriteLog(2, text37, null, true);
					}
					else if ("-enableprotocheck" == cmdFields[0])
					{
						bool flag8 = 1 == 0;
						if (cmdFields.Length == 2)
						{
							int num157 = Convert.ToInt32(cmdFields[1]);
							string text42 = string.Empty;
							if (1 == num157)
							{
								text42 = "开启协议检查";
								SingletonTemplate<ProtoChecker>.Instance().SetEnableCheck(true);
							}
							else
							{
								text42 = "关闭协议检查";
								SingletonTemplate<ProtoChecker>.Instance().SetEnableCheck(false);
							}
							LogManager.WriteLog(2, text42, null, true);
						}
					}
					else if ("-outwaitcount" == cmdFields[0])
					{
						LogManager.WriteLog(2, string.Format("waitcount={0}", GameManager.loginWaitLogic.GetTotalWaitingCount()), null, true);
					}
					else if ("-outwaitinfo" == cmdFields[0])
					{
						int num10 = Convert.ToInt32(cmdFields[1]);
						int index = Convert.ToInt32(cmdFields[2]);
						GameManager.loginWaitLogic.OutWaitInfo((LoginWaitLogic.UserType)num10, index);
					}
					else if ("-elementWar" == cmdFields[0])
					{
						string text43 = cmdFields[1];
						string text36 = text43;
						if (text36 != null)
						{
							if (!(text36 == "clearCount"))
							{
								if (!(text36 == "clearTime"))
								{
									if (text36 == "clear")
									{
										Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarDayId", Global.GetOffsetDayNow(), true);
										Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", 0, true);
										KuaFuManager.getInstance().SetCannotJoinKuaFuCopyEndTicks(client, 0L);
									}
								}
								else
								{
									KuaFuManager.getInstance().SetCannotJoinKuaFuCopyEndTicks(client, 0L);
								}
							}
							else
							{
								Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarDayId", Global.GetOffsetDayNow(), true);
								Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", 0, true);
							}
						}
					}
					else if ("-updateFuBenData" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int num10 = Convert.ToInt32(cmdFields[1]);
							TCPCmdHandler.isUpdateFuBenData = (num10 > 0);
						}
					}
					else if ("-onekeyactivetujian" == cmdFields[0])
					{
						if (!transmit && cmdFields.Length == 2)
						{
							int num10 = Convert.ToInt32(cmdFields[1]);
							string str = string.Empty;
							if (SingletonTemplate<TuJianManager>.Instance().GM_OneKeyActiveTuJianType(client, num10, out str))
							{
								str = "成功";
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "激活图鉴type=" + num10.ToString() + " " + str);
						}
					}
					else if ("-morijudge" == cmdFields[0])
					{
						if (!transmit)
						{
							SingletonTemplate<MoRiJudgeManager>.Instance().OnGMCommand(client, cmdFields);
						}
					}
					else if ("-cleargembag" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.FluorescentGemMgr.GMClearGemBag(client);
						}
					}
					else if ("-decygfm" == cmdFields[0])
					{
						if (!transmit)
						{
							int num158 = Convert.ToInt32(cmdFields[1]);
							GameManager.FluorescentGemMgr.GMDecFluorescentPoint(client, num158);
						}
						else if (cmdFields.Length == 3)
						{
							int num148 = Convert.ToInt32(cmdFields[1]);
							int num158 = Convert.ToInt32(cmdFields[2]);
							if (num158 <= 0)
							{
								return true;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(num148);
							if (null == gameClient)
							{
								GameManager.FluorescentGemMgr.ModifyFluorescentPoint2DB(num148, -num158);
							}
							else
							{
								GameManager.FluorescentGemMgr.GMDecFluorescentPoint(gameClient, num158);
							}
							LogManager.WriteLog(3, string.Format("根据GM的要求为角色ID：【{0}】减少荧光宝石粉末【{1}】！", num148, num158), null, true);
						}
					}
					else if ("-addygfm" == cmdFields[0])
					{
						if (!transmit)
						{
							int nPoint = Convert.ToInt32(cmdFields[1]);
							GameManager.FluorescentGemMgr.GMAddFluorescentPoint(client, nPoint);
						}
					}
					else if ("-setfreemodname" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int num148 = Convert.ToInt32(cmdFields[1]);
							int num44 = Convert.ToInt32(cmdFields[2]);
							SingletonTemplate<NameManager>.Instance().GM_SetFreeModName(num148, num44);
						}
						else if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "-setfreemodname roleid count");
						}
					}
					else if ("-changebhname" == cmdFields[0])
					{
						if (!transmit && cmdFields.Length >= 2)
						{
							SingletonTemplate<NameManager>.Instance().GM_ChangeBangHuiName(client, cmdFields[1]);
						}
					}
					else if ("-cancelTask" == cmdFields[0])
					{
						if (cmdFields.Length == 1)
						{
							TaskData taoTask = TodayManager.GetTaoTask(client);
							if (taoTask != null)
							{
								bool flag10 = Global.CancelTask(client, taoTask.DbID, taoTask.DoingTaskID);
							}
						}
					}
					else if ("-banTimeOut" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int num44 = Convert.ToInt32(cmdFields[1]);
								client.ClientSocket.session.TimeOutCount = num44;
								client.CheckCheatData.NextTaskListTimeout = TimeUtil.NOW();
							}
						}
					}
					else if ("-tenInit" == cmdFields[0])
					{
						TenManager.initConfig();
					}
					else if ("-sevenday" == cmdFields[0])
					{
						SingletonTemplate<SevenDayActivityMgr>.Instance().On_GM(client, cmdFields);
					}
					else if ("-spreadSetCount" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							try
							{
								string[] array14 = cmdFields[1].Split(new char[]
								{
									','
								});
								int[] counts = new int[]
								{
									int.Parse(array14[0]),
									int.Parse(array14[1]),
									int.Parse(array14[2])
								};
								SpreadManager.getInstance().SpreadSetCount(client, counts);
							}
							catch
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "-spreadSetCount 总人数,vip人数,level人数");
							}
						}
					}
					else if ("-setcitybh" == cmdFields[0])
					{
						if (cmdFields.Length >= 3)
						{
							try
							{
								string[] array14 = cmdFields[1].Split(new char[]
								{
									','
								});
								int[] array15 = new int[4];
								int num159 = int.Parse(cmdFields[1]);
								LangHunLingYuStatisticalData langHunLingYuStatisticalData = new LangHunLingYuStatisticalData();
								langHunLingYuStatisticalData.CityId = num159;
								langHunLingYuStatisticalData.CompliteTime = TimeUtil.NowDateTime();
								for (int i = 2; i < cmdFields.Length; i++)
								{
									langHunLingYuStatisticalData.SiteBhids[i - 2] = int.Parse(cmdFields[i]);
								}
								LangHunLingYuManager.getInstance().LangHunLingYuBuildMaxCityOwnerInfo(langHunLingYuStatisticalData, 0);
								YongZheZhanChangClient.getInstance().GameFuBenComplete(langHunLingYuStatisticalData);
								if (null != client)
								{
									LogManager.WriteLog(3, string.Format("GM通过-setcitybh 设置城池{0}攻防帮会信息", num159), null, true);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format("GM通过-setcitybh 设置城池{0}攻防帮会信息", num159));
								}
							}
							catch
							{
							}
						}
					}
					else if ("-soulstone" == cmdFields[0])
					{
						SingletonTemplate<SoulStoneManager>.Instance().GM_Test(client, cmdFields);
					}
					else if ("-bhzijin" == cmdFields[0])
					{
						text = "指令格式为: -bhzijin 帮会ID(或0) 资金";
						if (cmdFields.Length >= 2)
						{
							int num102 = Global.SafeConvertToInt32(cmdFields[1]);
							int num160 = Global.SafeConvertToInt32(cmdFields[2]);
							if (num102 == 0 && null != client)
							{
								num102 = client.ClientData.Faction;
							}
							BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, num102, 0);
							if (null != bangHuiDetailData)
							{
								if (GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num102, num160))
								{
									text = "添加战盟资金: " + num160;
									LogManager.WriteLog(3, string.Format("GM命令添加战盟资金,bhid={0}, Money={1}", num102, num160), null, true);
								}
							}
						}
						if (null != client)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
					}
					else if ("-detecthook" == cmdFields[0])
					{
						List<string> list7 = new List<string>();
						if (cmdFields.Length >= 2)
						{
							string[] array16 = cmdFields[1].Split(new char[]
							{
								','
							});
							foreach (string text44 in array16)
							{
								if (!string.IsNullOrEmpty(text44))
								{
									list7.Add(text44);
								}
							}
						}
						if (list7.Count > 0)
						{
							FileBanLogic.BroadCastDetectHook(list7);
						}
						else
						{
							FileBanLogic.BroadCastDetectHook();
						}
					}
					else if ("-clearbanmem" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int nHour = Global.SafeConvertToInt32(cmdFields[1]);
							BanManager.ClearBanMemory(nHour);
							FileBanLogic.ClearBanList();
						}
					}
					else if ("-logrolerelife" == cmdFields[0])
					{
						int num117 = 0;
						bool bLog = true;
						if (cmdFields.Length >= 2)
						{
							num117 = Convert.ToInt32(cmdFields[1]);
						}
						if (cmdFields.Length >= 3 && Convert.ToInt32(cmdFields[2]) < 0)
						{
							bLog = false;
						}
						if (num117 > 0)
						{
							SingletonTemplate<MonsterAttackerLogManager>.Instance().SetLogRoleRelife(num117, bLog);
						}
					}
					else if ("-reshowluolan" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int num88 = Convert.ToInt32(cmdFields[1]);
							LuoLanChengZhanManager.getInstance().ReShowLuolanKing(num88);
						}
					}
					else if ("-passiveskill" == cmdFields[0])
					{
						if (!transmit && null != client)
						{
							List<PassiveSkillData> list8 = new List<PassiveSkillData>();
							for (int i = 1; i <= cmdFields.Length - 6; i += 6)
							{
								list8.Add(new PassiveSkillData(Convert.ToInt32(cmdFields[i]), Convert.ToInt32(cmdFields[i + 1]), Convert.ToInt32(cmdFields[i + 2]), Convert.ToInt32(cmdFields[i + 3]), Convert.ToInt32(cmdFields[i + 4]), Convert.ToInt32(cmdFields[i + 5])));
							}
							client.passiveSkillModule.UpdateSkillList(list8);
						}
					}
					else if ("-palace" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text36 = cmdFields[1];
								if (text36 != null)
								{
									if (!(text36 == "level"))
									{
										if (!(text36 == "count"))
										{
											if (text36 == "rate")
											{
												UnionPalaceManager.SetUnionPalaceRate(client, int.Parse(cmdFields[2]));
											}
										}
										else
										{
											UnionPalaceManager.SetUnionPalaceCount(client, int.Parse(cmdFields[2]));
										}
									}
									else
									{
										UnionPalaceManager.SetUnionPalaceLevelByID(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								text = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-reshowzhongshen" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int num88 = Convert.ToInt32(cmdFields[1]);
							SingletonTemplate<ZhengBaManager>.Instance().SetZhongShengRole(num88);
						}
					}
					else if ("-reshowfenghuojiaren" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int roleid = Convert.ToInt32(cmdFields[1]);
							int roleid2 = Convert.ToInt32(cmdFields[2]);
							SingletonTemplate<CoupleArenaManager>.Instance().SetFengHuoJiaRenCouple(roleid, roleid2);
						}
					}
					else if ("-giftcodecmd" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							GiftCodeNewManager.getInstance().ProcessGiftCodeList(cmdFields[1]);
						}
					}
					else if ("-olympicsGrade" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int addGrade = int.Parse(cmdFields[1]);
							OlympicsManager.getInstance().OlympicsGradeAdd(client, addGrade);
						}
					}
					else if ("-zhengduo" == cmdFields[0])
					{
						TianTiClient.getInstance().GmCommand(cmdFields, null);
					}
					else if ("-juntuantask" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 2)
						{
							int taskType = Global.SafeConvertToInt32(cmdFields[1]);
							int taskValue = Global.SafeConvertToInt32(cmdFields[2]);
							JunTuanManager.getInstance().AddJunTuanTaskValue(client, taskType, taskValue);
						}
					}
					else if ("-juntuanjoin2" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							string[] cmdParams = new string[]
							{
								client.ClientData.RoleID.ToString(),
								cmdFields[1],
								cmdFields[2]
							};
							JunTuanManager.getInstance().ProcessJunTuanJoinResponseCmd(client, 1234, null, cmdParams);
						}
					}
					else if ("-l" == cmdFields[0])
					{
						GLang.OutputToFile();
					}
					else if ("-yzzc" == cmdFields[0])
					{
						if (null != client)
						{
							string[] array17 = null;
							if (cmdFields.Length >= 2)
							{
								if (File.Exists(cmdFields[1]))
								{
									array17 = File.ReadAllLines(cmdFields[1]);
								}
							}
							string text38 = string.Format("{0} {1} {2}", "GameState", 1, 5);
							YongZheZhanChangClient.getInstance().ExecuteCommand(text38);
							if (array17 != null)
							{
								foreach (string text45 in array17)
								{
									string[] array18 = text45.Split(new char[]
									{
										','
									});
									YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(array18[1], Convert.ToInt32(array18[3]), Convert.ToInt32(array18[2]), 5, Convert.ToInt32(array18[5]), Convert.ToInt32(array18[6]));
									Thread.Sleep(10);
								}
							}
							else
							{
								for (int i = 0; i < 2158; i++)
								{
									YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp("u" + i, i, 54, 5, 1, 10000);
									Thread.Sleep(10);
								}
							}
							Thread.Sleep(1000);
							LogManager.WriteLog(2, "通知跨服中心开始分配所有报名玩家的活动场次", null, true);
							text38 = string.Format("{0} {1} {2}", "GameState", 2, 5);
							YongZheZhanChangClient.getInstance().ExecuteCommand(text38);
						}
					}
					else if ("-kfld" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int destServerID = Global.SafeConvertToInt32(cmdFields[1]);
							int successServerID = Global.SafeConvertToInt32(cmdFields[2]);
							int kill = Global.SafeConvertToInt32(cmdFields[3]);
							KuaFuLueDuoStatisticalData kuaFuLueDuoStatisticalData = new KuaFuLueDuoStatisticalData();
							kuaFuLueDuoStatisticalData.DestServerID = destServerID;
							kuaFuLueDuoStatisticalData.SuccessServerID = successServerID;
							kuaFuLueDuoStatisticalData.GameId = -2L;
							kuaFuLueDuoStatisticalData.roleStatisticalData = new List<KuaFuLueDuoRoleData>
							{
								new KuaFuLueDuoRoleData
								{
									rid = client.ClientData.RoleID,
									kill = kill,
									rname = client.ClientData.RoleName,
									zoneid = client.ClientData.ZoneID
								}
							};
							HuanYingSiYuanClient.getInstance().GameFuBenComplete_KuaFuLueDuo(kuaFuLueDuoStatisticalData);
						}
					}
					else if ("-kfldcaiji" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int ziYuan = Global.SafeConvertToInt32(cmdFields[1]);
							if (null != client)
							{
								KuaFuLueDuoManager.getInstance().GMCaiJi(client, ziYuan);
							}
						}
					}
					else if ("-charge" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num161 = Global.SafeConvertToInt32(cmdFields[1]);
							int num162 = Global.SafeConvertToInt32(cmdFields[2]);
							string text46 = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
							if (null != client)
							{
								string text47 = string.Format("-charge:{0}:{1}:{2}:{3}:{4}", new object[]
								{
									client.strUserID,
									num161,
									client.ClientData.RoleID,
									num162,
									text46
								});
								LogManager.WriteLog(3, text47, null, true);
								GameManager.DBCmdMgr.AddDBCmd(157, text47, null, client.ServerId);
							}
						}
					}
					else if ("-setmoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num163 = Global.SafeConvertToInt32(cmdFields[1]);
							long num164 = (long)Global.SafeConvertToInt32(cmdFields[2]);
							client.ClientData.MoneyData[num163] = num164;
							GameManager.ClientMgr.NotifySelfPropertyValue(client, num163, num164);
						}
					}
					else if ("-maxteamcopy" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num165 = Global.SafeConvertToInt32(cmdFields[1]);
							int num109 = Global.SafeConvertToInt32(cmdFields[2]);
							ConstData.MaxCopyTeamMemberNumDict[num165] = num109;
							string textMsg = string.Format("设置组队副本{0}人数上线为{1}", num165, num109);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
						}
					}
					else if ("-huiji" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 4)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num166 = Global.SafeConvertToInt32(cmdFields[2]);
							int exp = Global.SafeConvertToInt32(cmdFields[3]);
							GameClient gameClient3 = client;
							if (num88 > 0)
							{
								gameClient3 = GameManager.ClientMgr.FindClient(num88);
							}
							else
							{
								num88 = client.ClientData.RoleID;
							}
							if (null != gameClient3)
							{
								gameClient3.ClientData.HuiJiData.huiji = num166;
								gameClient3.ClientData.HuiJiData.Exp = exp;
								client.sendCmd<HuiJiUpdateResultData>(1446, new HuiJiUpdateResultData
								{
									HuiJi = num166,
									Exp = exp
								}, false);
							}
							RoleHuiJiData v = new RoleHuiJiData
							{
								huiji = num166,
								Exp = exp
							};
							Global.SendToDB<RoleDataCmdT<RoleHuiJiData>>(1446, new RoleDataCmdT<RoleHuiJiData>(num88, v), 0);
						}
					}
					else if ("-showMonsterProperty" == cmdFields[0])
					{
						if (cmdFields.Length < 2)
						{
							text = string.Format("请输入： -showMonsterProperty 怪物ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
						}
						else
						{
							Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, this.SafeConvertToInt32(cmdFields[1]));
							if (null == monster)
							{
								text = string.Format("没有" + cmdFields[1] + "这个怪物", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
								return true;
							}
							StringBuilder stringBuilder = new StringBuilder();
							Global.PrintSomeProps(monster, ref stringBuilder);
							LogManager.WriteLog(5, stringBuilder.ToString(), null, true);
						}
					}
					else if ("-setMonsterProperty" == cmdFields[0])
					{
						Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, this.SafeConvertToInt32(cmdFields[1]));
						if (null == monster)
						{
							text = string.Format("没有" + cmdFields[1] + "这个怪物", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							return true;
						}
						if (this.SafeConvertToInt32(cmdFields[2]) == 13)
						{
							monster.VLife += (double)this.SafeConvertToInt32(cmdFields[3]);
						}
						else
						{
							monster.TempPropsBuffer.AddTempExtProp(this.SafeConvertToInt32(cmdFields[2]), (double)this.SafeConvertToInt32(cmdFields[3]), (TimeUtil.NOW() + 3600000L) * 10000L);
						}
						text = string.Format("为怪物{0}调整了{1},{2}", cmdFields[1], Enum.GetName(typeof(ExtPropIndexes), this.SafeConvertToInt32(cmdFields[2])), cmdFields[3]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
					}
					else if ("-bianshen" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 4)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num166 = Global.SafeConvertToInt32(cmdFields[2]);
							int exp = Global.SafeConvertToInt32(cmdFields[3]);
							GameClient gameClient3 = client;
							if (num88 > 0)
							{
								gameClient3 = GameManager.ClientMgr.FindClient(num88);
							}
							else
							{
								num88 = client.ClientData.RoleID;
							}
							if (null != gameClient3)
							{
								gameClient3.ClientData.BianShenData.BianShen = num166;
								gameClient3.ClientData.BianShenData.Exp = exp;
								client.sendCmd<BianShenUpdateResultData>(1449, new BianShenUpdateResultData
								{
									BianShen = num166,
									Exp = exp
								}, false);
							}
							RoleBianShenData v2 = new RoleBianShenData
							{
								BianShen = num166,
								Exp = exp
							};
							Global.SendToDB<RoleDataCmdT<RoleBianShenData>>(1449, new RoleDataCmdT<RoleBianShenData>(num88, v2), 0);
						}
					}
					else if ("-bianshenexec" == cmdFields[0])
					{
						if (null != client)
						{
							BianShenManager.getInstance().processCmdEx(client, 1448, null, cmdFields);
						}
					}
					else if ("-armor" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 4)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num166 = Global.SafeConvertToInt32(cmdFields[2]);
							int exp = Global.SafeConvertToInt32(cmdFields[3]);
							GameClient gameClient3 = client;
							if (num88 > 0)
							{
								gameClient3 = GameManager.ClientMgr.FindClient(num88);
							}
							else
							{
								num88 = client.ClientData.RoleID;
							}
							if (null != gameClient3)
							{
								gameClient3.ClientData.ArmorData.Armor = num166;
								gameClient3.ClientData.ArmorData.Exp = exp;
								client.sendCmd<ArmorUpdateResultData>(1447, new ArmorUpdateResultData
								{
									Armor = num166,
									Exp = exp
								}, false);
							}
							RoleArmorData v3 = new RoleArmorData
							{
								Armor = num166,
								Exp = exp
							};
							Global.SendToDB<RoleDataCmdT<RoleArmorData>>(1447, new RoleDataCmdT<RoleArmorData>(num88, v3), 0);
						}
					}
					else if ("-ysjx" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							GameClient gameClient3 = client;
							if (num88 > 0)
							{
								gameClient3 = GameManager.ClientMgr.FindClient(num88);
							}
							else
							{
								num88 = client.ClientData.RoleID;
							}
							if (null != gameClient3)
							{
								JingLingYuanSuJueXingManager.getInstance().GMSetJingLingYuanSuJueXingData(gameClient3, cmdFields);
							}
						}
					}
					else if ("-fumomoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num109 = Global.SafeConvertToInt32(cmdFields[2]);
							long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10217");
							long num168 = num167 + (long)num109;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "附魔灵石", "GM指令", client.ClientData.RoleName, "系统", "修改", num109, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.FuMoMoney, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10217", (int)num168, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 146, num168);
						}
					}
					else if ("-yinjimoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num109 = Global.SafeConvertToInt32(cmdFields[2]);
							long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10246");
							long num168 = num167 + (long)num109;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生印记", "GM指令", client.ClientData.RoleName, "系统", "修改", num109, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornLevelUpPoint, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10246", (int)num168, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 151, num168);
						}
					}
					else if ("-cuilianmoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num109 = Global.SafeConvertToInt32(cmdFields[2]);
							long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10249");
							long num168 = num167 + (long)num109;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "淬炼点", "GM指令", client.ClientData.RoleName, "系统", "修改", num109, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornCuiLian, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10249", (int)num168, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 152, num168);
						}
					}
					else if ("-duanzaomoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num109 = Global.SafeConvertToInt32(cmdFields[2]);
							long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10250");
							long num168 = num167 + (long)num109;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "锻造点", "GM指令", client.ClientData.RoleName, "系统", "修改", num109, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornDuanZao, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10250", (int)num168, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 153, num168);
						}
					}
					else if ("-niepanmoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int num88 = Global.SafeConvertToInt32(cmdFields[1]);
							int num109 = Global.SafeConvertToInt32(cmdFields[2]);
							long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10251");
							long num168 = num167 + (long)num109;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "涅槃点", "GM指令", client.ClientData.RoleName, "系统", "修改", num109, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornNiePan, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10251", (int)num168, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 154, num168);
						}
					}
					else if ("-fengyinmoney" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -fengyinmoney 角色名称 封印晶石数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num169 = Global.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10252");
								long num168 = num167 + (long)num169;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "封印晶石", "GM指令", gameClient.ClientData.RoleName, "系统", "修改", num169, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornFengYin, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "10252", (int)num168, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(gameClient, 155, num168);
							}
						}
					}
					else if ("-chongshengmoney" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -chongshengmoney 角色名称 重生晶石数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num170 = Global.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10253");
								long num168 = num167 + (long)num170;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生晶石", "GM指令", gameClient.ClientData.RoleName, "系统", "修改", num170, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornChongSheng, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "10253", (int)num168, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(gameClient, 156, num168);
							}
						}
					}
					else if ("-xuancaimoney" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -xuancaimoney 角色名称 炫彩晶石数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num171 = Global.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10254");
								long num168 = num167 + (long)num171;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "炫彩晶石", "GM指令", gameClient.ClientData.RoleName, "系统", "修改", num171, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornXuanCai, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "10254", (int)num168, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(gameClient, 157, gameClient.ClientData.RebornXuanCai);
							}
						}
					}
					else if ("-guanzhu" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -guanzhu 角色名称 灌注次数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num109 = Global.SafeConvertToInt32(cmdFields[2]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								long num167 = (long)Global.GetRoleParamsInt32FromDB(client, "10255");
								long num168 = num167 + (long)num109;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "灌注次数", "GM指令", gameClient.ClientData.RoleName, "系统", "修改", num109, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornXuanCai, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "10255", (int)num168, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(gameClient, 157, gameClient.ClientData.RebornEquipHole);
							}
						}
					}
					else if ("-cuilian" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								text = string.Format("请输入： -cuilian 角色名称 淬炼部位 淬炼等级", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num37 = Global.SafeConvertToInt32(cmdFields[2]);
								int num109 = Global.SafeConvertToInt32(cmdFields[3]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								lock (RebornEquip.RebornEquipHole)
								{
									if (RebornEquip.RebornEquipHole.ContainsKey(num37) && RebornEquip.RebornEquipHole[num37].ContainsKey(num109))
									{
										if (gameClient.ClientData.RebornEquipHoleInfo == null || !gameClient.ClientData.RebornEquipHoleInfo.ContainsKey(num37))
										{
											RebornEquipData rebornEquipData = new RebornEquipData();
											rebornEquipData.RoleID = gameClient.ClientData.RoleID;
											rebornEquipData.Able = 0;
											rebornEquipData.Level = num109;
											rebornEquipData.HoleID = num37;
											int num79 = Global.sendToDB<int, RebornEquipData>(14123, rebornEquipData, client.ServerId);
											if (num79 < 0)
											{
												LogManager.WriteLog(2, string.Format("GM插入重生装备槽数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
												return false;
											}
											gameClient.ClientData.RebornEquipHoleInfo.Add(num37, rebornEquipData);
										}
										else
										{
											if (!RebornEquip.RebornEquipHoleLevelMap.ContainsKey(num37))
											{
												return false;
											}
											if (num109 > RebornEquip.RebornEquipHoleLevelMap[num37])
											{
												num109 = RebornEquip.RebornEquipHoleLevelMap[num37];
											}
											RebornEquipData rebornEquipData = new RebornEquipData();
											rebornEquipData.RoleID = gameClient.ClientData.RoleID;
											rebornEquipData.Able = gameClient.ClientData.RebornEquipHoleInfo[num37].Able;
											rebornEquipData.Level = num109;
											rebornEquipData.HoleID = num37;
											int num79 = Global.sendToDB<int, RebornEquipData>(14124, rebornEquipData, client.ServerId);
											if (num79 <= 0)
											{
												LogManager.WriteLog(2, string.Format("GM灌注更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
												return false;
											}
											gameClient.ClientData.RebornEquipHoleInfo.Remove(num37);
											gameClient.ClientData.RebornEquipHoleInfo.Add(num37, rebornEquipData);
										}
									}
								}
								text = string.Format("为{0}修改淬炼等级{1}", text2, gameClient.ClientData.RebornEquipHoleInfo[num37].Level);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
						}
					}
					else if ("-mazinger" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 5)
							{
								text = string.Format("请输入： -mazinger 角色名称 魔神类型 等阶 星 经验", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
							}
							else
							{
								string text2 = cmdFields[1];
								int num10 = Global.SafeConvertToInt32(cmdFields[2]);
								int num71 = Global.SafeConvertToInt32(cmdFields[3]);
								int num172 = Global.SafeConvertToInt32(cmdFields[4]);
								int exp = Global.SafeConvertToInt32(cmdFields[5]);
								int num8 = RoleName2IDs.FindRoleIDByName(text2, false);
								if (-1 == num8)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(num8);
								if (null == gameClient)
								{
									text = string.Format("{0}不在线", text2);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, text);
									return true;
								}
								if (MazingerStoreManager.getInstance().MazingerStar.ContainsKey(num10))
								{
									if (MazingerStoreManager.getInstance().MazingerStar[num10].ContainsKey(num71))
									{
										if (MazingerStoreManager.getInstance().MazingerStar[num10][num71].ContainsKey(num172))
										{
											if (client.ClientData.MazingerStoreDataInfo.ContainsKey(num10))
											{
												MazingerStoreData mazingerStoreData = MazingerStoreManager.getInstance().CopyMazingerStoreMemData(client, num10);
												mazingerStoreData.Exp = exp;
												mazingerStoreData.Stage = num71;
												mazingerStoreData.StarLevel = num172;
												int num79 = Global.sendToDB<int, MazingerStoreData>(14126, mazingerStoreData, client.ServerId);
												if (num79 < 0)
												{
													LogManager.WriteLog(2, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
													return true;
												}
												GameManager.logDBCmdMgr.AddDBLogInfo(-1, "GM指令魔神秘宝升星", DateTime.Now.ToString(), mazingerStoreData.Type.ToString(), client.ClientData.RoleName, "系统", num10, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
												EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].StarLevel, mazingerStoreData.StarLevel, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].Exp, mazingerStoreData.Exp, "魔神秘宝升星");
												client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type] = mazingerStoreData;
												return true;
											}
											else
											{
												MazingerStoreData mazingerStoreData = new MazingerStoreData();
												mazingerStoreData.Type = num10;
												mazingerStoreData.Stage = num71;
												mazingerStoreData.StarLevel = num172;
												mazingerStoreData.Exp = exp;
												mazingerStoreData.RoleID = client.ClientData.RoleID;
												int num79 = Global.sendToDB<int, MazingerStoreData>(14125, mazingerStoreData, client.ServerId);
												if (num79 < 0)
												{
													LogManager.WriteLog(2, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
													return true;
												}
												GameManager.logDBCmdMgr.AddDBLogInfo(-1, "GM指令魔神秘宝升星", DateTime.Now.ToString(), mazingerStoreData.Type.ToString(), client.ClientData.RoleName, "系统", num10, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
												EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].StarLevel, mazingerStoreData.StarLevel, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].Exp, mazingerStoreData.Exp, "魔神秘宝升星");
												if (client.ClientData.MazingerStoreDataInfo == null)
												{
													client.ClientData.MazingerStoreDataInfo = new Dictionary<int, MazingerStoreData>();
												}
												client.ClientData.MazingerStoreDataInfo.Add(mazingerStoreData.Type, mazingerStoreData);
												return true;
											}
										}
									}
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "参数看魔神秘宝配置");
								return true;
							}
						}
					}
					else if ("-emptyreborn" == cmdFields[0])
					{
						if (client.ClientData.RebornGoodsDataList != null)
						{
							List<GoodsData> list9 = new List<GoodsData>();
							list9.AddRange(client.ClientData.RebornGoodsDataList);
							foreach (GoodsData goodsData in list9)
							{
								if (goodsData != null && goodsData.Site == 15000 && goodsData.Using <= 0)
								{
									string cmdData2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										4,
										goodsData.Id,
										goodsData.GoodsID,
										0,
										goodsData.Site,
										goodsData.GCount,
										goodsData.BagIndex,
										""
									});
									if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData2, "客户端修改", null))
									{
										client.ClientData.RebornGoodsDataList.Remove(goodsData);
									}
								}
							}
						}
					}
					else if (!transmit)
					{
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "what do you mean??? [" + cmdFields[0] + "]");
					}
				}
			}
			return true;
		}

		public static void GMSetTime(GameClient client, string[] cmdFields, bool allServer = false)
		{
			TimeSpan zero = TimeSpan.Zero;
			string text;
			if (cmdFields.Length <= 2)
			{
				text = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			}
			else if (cmdFields.Length == 3)
			{
				text = cmdFields[2].Replace('：', ':');
				if (TimeSpan.TryParse(text, out zero))
				{
					text = TimeUtil.NowDateTime().Add(zero).ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					text = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
				}
			}
			else
			{
				text = cmdFields[2] + " " + cmdFields[3];
			}
			text = text.Replace('：', ':');
			DateTime t;
			if (DateTime.TryParse(text, out t))
			{
				if (t < TimeUtil.NowDateTime())
				{
					if (null != client)
					{
						string textMsg = string.Format("禁止时间倒退！！！", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					}
				}
				else if (!allServer)
				{
					DateTime dateTime = TimeUtil.SetTime(text);
					Thread.Sleep(10);
					LogManager.WriteLog(2, string.Format("GM命令修改时间#from={0},to={1},realtime={2}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), text, DateTime.Now.ToString()), null, true);
					int num = 0;
					GameClient nextClient;
					while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num, true)) != null)
					{
						nextClient.ClientData.LastClientHeartTicks = dateTime.Ticks / 10000L;
						nextClient.sendCmd(832, string.Format("{0}:{1}:{2}", nextClient.ClientData.RoleID, 0, TimeUtil.NOW() * 10000L), false);
					}
				}
				else
				{
					string cmd = string.Format("-settime 0 {0}", text);
					YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
				}
			}
			else if (null != client)
			{
				string textMsg = string.Format("错误的时间格式！！！", new object[0]);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
			}
		}

		public bool GMSetRebornLevel(GameClient client, string[] cmdFields)
		{
			string text = cmdFields[1];
			int num = this.SafeConvertToInt32(cmdFields[2]);
			int num2 = client.ClientData.RebornCount;
			if (cmdFields.Length >= 4)
			{
				num2 = this.SafeConvertToInt32(cmdFields[3]);
			}
			int num3 = RoleName2IDs.FindRoleIDByName(text, false);
			bool result;
			if (-1 == num3)
			{
				string textMsg = string.Format("{0}不在线", text);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
				result = true;
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(num3);
				if (null == gameClient)
				{
					string textMsg = string.Format("{0}不在线", text);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					result = true;
				}
				else if (!RebornManager.getInstance().CheckRebornCountLevelValid(client, num2, num))
				{
					string textMsg = string.Format("设置的级别超出了最大限制", num);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					result = true;
				}
				else
				{
					LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置重生级别{1}", text, num), null, true);
					gameClient.ClientData.RebornLevel = num;
					Global.SaveRoleParamsInt32ValueToDB(gameClient, "10241", gameClient.ClientData.RebornLevel, true);
					gameClient.ClientData.Experience = 0L;
					GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, 0L);
					if (cmdFields.Length >= 4)
					{
						gameClient.ClientData.RebornCount = num2;
						Global.SaveRoleParamsInt32ValueToDB(gameClient, "10240", gameClient.ClientData.RebornCount, true);
					}
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
					RebornManager.getInstance().NotifySelfExperience(gameClient, 0L);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(gameClient, 0L, "", MoneyTypes.RebornExpMonster, false, true, false);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(gameClient, 0L, "", MoneyTypes.RebornExpSale, false, true, false);
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取重生经验和级别, roleID={0}({1}), Level={2}, Experience={3}, newExperience={4}", new object[]
					{
						gameClient.ClientData.RoleID,
						gameClient.ClientData.RoleName,
						gameClient.ClientData.RebornCount,
						gameClient.ClientData.RebornExperience,
						0
					}), EventLevels.Hint);
					string textMsg = string.Format("为{0}设置了重生级别{1}", text, num);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					result = true;
				}
			}
			return result;
		}

		public void GMSetGoodsForgeLevel(GameClient client, GoodsData goods, int forgelev, int bind, bool ntfprops = false)
		{
			if (client != null && null != goods)
			{
				goods.Forge_level = forgelev;
				goods.Binding = bind;
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					1,
					client.ClientData.RoleID,
					goods.Id,
					goods.Forge_level,
					goods.Binding
				});
				client.sendCmd(161, cmdData, false);
				ChengJiuManager.OnRoleEquipmentQiangHua(client, goods.Forge_level);
				SevenDayGoalEventObject eventObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiForgeEquip);
				GlobalEventSource.getInstance().fireEvent(eventObj);
				SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ForgeEquipLevel);
				sevenDayGoalEventObject.Arg1 = goods.Forge_level;
				GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.EquipForgeLevel, goods.Forge_level);
				string[] array = null;
				string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					goods.Id,
					"*",
					goods.Forge_level,
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
					"*",
					goods.Binding,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
				Global.ModRoleGoodsEvent(client, goods, 0, "强化(GM)", false);
				EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goods.GoodsID, (long)goods.Id, 0, goods.GCount, "强化(GM)");
				ChengJiuManager.OnFirstQiangHua(client);
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriStrengthen))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
				if (ntfprops)
				{
					Global.RefreshEquipProp(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		public void GMSetWingSuitStar(GameClient client, string[] cmdFields)
		{
			if (cmdFields.Length == 3)
			{
				int num = this.SafeConvertToInt32(cmdFields[1]);
				int num2 = this.SafeConvertToInt32(cmdFields[2]);
				string textMsg;
				if (num <= 0 || num > MUWingsManager.MaxWingID)
				{
					textMsg = "阶数错误 1 - 9";
				}
				else if (num2 < 0 || num2 > MUWingsManager.MaxWingEnchanceLevel)
				{
					textMsg = "星数错误 0 - 10";
				}
				else if (MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, num, client.ClientData.MyWingData.JinJieFailedNum, num2, 0, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum) < 0)
				{
					textMsg = "存数据库错误";
				}
				else
				{
					if (1 == client.ClientData.MyWingData.Using)
					{
						MUWingsManager.UpdateWingDataProps(client, false);
					}
					client.ClientData.MyWingData.WingID = num;
					client.ClientData.MyWingData.ForgeLevel = num2;
					if (1 == client.ClientData.MyWingData.Using)
					{
						MUWingsManager.UpdateWingDataProps(client, true);
						client.sendCmd<WingData>(678, client.ClientData.MyWingData, false);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					textMsg = "修改翅膀成功";
					if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
			}
			else
			{
				string textMsg = "参数数量不对";
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
			}
		}

		public bool GMSetLevel(GameClient client, string[] cmdFields)
		{
			string text = cmdFields[1];
			int num = this.SafeConvertToInt32(cmdFields[2]);
			int num2 = RoleName2IDs.FindRoleIDByName(text, false);
			bool result;
			if (-1 == num2)
			{
				string textMsg = string.Format("{0}不在线", text);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
				result = true;
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(num2);
				if (null == gameClient)
				{
					string textMsg = string.Format("{0}不在线", text);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					result = true;
				}
				else if (num < 0 || num > 400)
				{
					string textMsg = string.Format("设置的级别{0}超出了最大限制400", num);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					result = true;
				}
				else
				{
					LogManager.WriteLog(3, string.Format("根据GM的要求为{0}设置级别{1}", text, num), null, true);
					gameClient.ClientData.Level = num;
					GameManager.DBCmdMgr.AddDBCmd(10002, string.Format("{0}:{1}:{2}", gameClient.ClientData.RoleID, gameClient.ClientData.Level, gameClient.ClientData.Experience), null, gameClient.ServerId);
					gameClient.ClientData.Experience = 0L;
					GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, 0L);
					string textMsg;
					if (cmdFields.Length >= 4)
					{
						int num3 = this.SafeConvertToInt32(cmdFields[3]);
						if (num3 < 0 || num3 > 100)
						{
							textMsg = string.Format("转生级别不在有效范围[0-100]!", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
							return true;
						}
						int changeLifeCount = gameClient.ClientData.ChangeLifeCount;
						lock (gameClient.ClientData.PropPointMutex)
						{
							if (cmdFields.Length >= 5)
							{
								int[] array = new int[4];
								int num4 = this.SafeConvertToInt32(cmdFields[4]);
								for (int i = 0; i < array.Length; i++)
								{
									if (i < cmdFields.Length - 4)
									{
										num4 = this.SafeConvertToInt32(cmdFields[4 + i]);
									}
									array[i] = num4;
								}
								gameClient.ClientData.PropStrength += array[0] - Global.GetRoleParamsInt32FromDB(gameClient, "PropStrengthChangeless");
								gameClient.ClientData.PropIntelligence += array[1] - Global.GetRoleParamsInt32FromDB(gameClient, "PropIntelligenceChangeless");
								gameClient.ClientData.PropDexterity += array[2] - Global.GetRoleParamsInt32FromDB(gameClient, "PropDexterityChangeless");
								gameClient.ClientData.PropConstitution += array[3] - Global.GetRoleParamsInt32FromDB(gameClient, "PropConstitutionChangeless");
								gameClient.ClientData.TotalPropPoint += array[0] - Global.GetRoleParamsInt32FromDB(gameClient, "PropStrengthChangeless");
								gameClient.ClientData.TotalPropPoint += array[1] - Global.GetRoleParamsInt32FromDB(gameClient, "PropIntelligenceChangeless");
								gameClient.ClientData.TotalPropPoint += array[2] - Global.GetRoleParamsInt32FromDB(gameClient, "PropDexterityChangeless");
								gameClient.ClientData.TotalPropPoint += array[3] - Global.GetRoleParamsInt32FromDB(gameClient, "PropConstitutionChangeless");
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropStrength", gameClient.ClientData.PropStrength, true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropIntelligence", gameClient.ClientData.PropIntelligence, true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropDexterity", gameClient.ClientData.PropDexterity, true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropConstitution", gameClient.ClientData.PropConstitution, true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "TotalPropPoint", gameClient.ClientData.TotalPropPoint, true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropStrengthChangeless", array[0], true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropIntelligenceChangeless", array[1], true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropDexterityChangeless", array[2], true);
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropConstitutionChangeless", array[3], true);
							}
							changeLifeCount = num3;
							gameClient.ClientData.ChangeLifeCount = changeLifeCount;
						}
						GameManager.DBCmdMgr.AddDBCmd(509, string.Format("{0}:{1}", gameClient.ClientData.RoleID, gameClient.ClientData.ChangeLifeCount), null, gameClient.ServerId);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
					}
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
					GameManager.ClientMgr.NotifyTeamUpLevel(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, false);
					Global.AutoLearnSkills(client);
					GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, 0L);
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取经验和级别, roleID={0}({1}), Level={2}, Experience={3}, newExperience={4}", new object[]
					{
						gameClient.ClientData.RoleID,
						gameClient.ClientData.RoleName,
						gameClient.ClientData.Level,
						gameClient.ClientData.Experience,
						0
					}), EventLevels.Hint);
					textMsg = string.Format("为{0}设置了级别{1}", text, num);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					result = true;
				}
			}
			return result;
		}

		private string[] SuperGMUserNames = null;

		private string[] GMUserNames = null;

		private string[] GMIPs = null;

		private Dictionary<string, int> OtherUserNamesDict = new Dictionary<string, int>();

		private Dictionary<int, string[]> GMCmdsDict = new Dictionary<int, string[]>();

		private Dictionary<string, GmCmdHandler> GmCmdsHandlerDict = new Dictionary<string, GmCmdHandler>();

		private List<GameClient> GMClientList = new List<GameClient>();

		public static bool EnableGMSetAllServerTime = false;
	}
}
