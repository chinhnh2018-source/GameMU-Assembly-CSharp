using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	public class TCPManager
	{
		private TCPManager()
		{
		}

		public static TCPManager getInstance()
		{
			return TCPManager.instance;
		}

		public void initialize(int capacity)
		{
			this.MaxConnectedClientLimit = capacity;
			this.socketListener = new SocketListener(capacity, 6144);
			this.socketListener.SocketClosed += this.SocketClosed;
			this.socketListener.SocketConnected += this.SocketConnected;
			this.socketListener.SocketReceived += this.SocketReceived;
			this.socketListener.SocketSended += this.SocketSended;
			this._tcpClientPool = TCPClientPool.getInstance();
			this._tcpClientPool.initialize(100);
			this._tcpLogClientPool = TCPClientPool.getLogInstance();
			this._tcpLogClientPool.initialize(100);
			this.tcpInPacketPool = new TCPInPacketPool(capacity);
			TCPOutPacketPool.getInstance().initialize(capacity);
			this.tcpOutPacketPool = TCPOutPacketPool.getInstance();
			this.dictInPackets = new Dictionary<TMSKSocket, TCPInPacket>(capacity);
			this.tcpSessions = new Dictionary<TMSKSocket, TCPSession>();
			TCPCmdDispatcher.getInstance().initialize();
			this.taskExecutor = new ScheduleExecutor(0);
			this.taskExecutor.start();
		}

		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		public TCPInPacketPool TcpInPacketPool
		{
			get
			{
				return this.tcpInPacketPool;
			}
		}

		public TCPOutPacketPool TcpOutPacketPool
		{
			get
			{
				return this.tcpOutPacketPool;
			}
		}

		public Program RootWindow { get; set; }

		public TCPClientPool tcpClientPool
		{
			get
			{
				return this._tcpClientPool;
			}
		}

		public TCPClientPool tcpLogClientPool
		{
			get
			{
				return this._tcpLogClientPool;
			}
		}

		public TCPRandKey tcpRandKey
		{
			get
			{
				return this._tcpRandKey;
			}
		}

		public void Start(string ip, int port)
		{
			TCPManager.ServerPort = port;
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		public void Stop()
		{
			this.socketListener.Stop();
			this.taskExecutor.stop();
		}

		public void ForceCloseSocket(TMSKSocket s)
		{
			DelayForceClosingMgr.RemoveDelaySocket(s);
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.ContainsKey(s))
				{
					TCPInPacket item = this.dictInPackets[s];
					this.tcpInPacketPool.Push(item);
					this.dictInPackets.Remove(s);
				}
			}
			bool flag2 = false;
			GameClient gameClient = GameManager.ClientMgr.FindClient(s);
			if (null != gameClient)
			{
				GameManager.ClientMgr.Logout(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
				flag2 = true;
			}
			string text = GameManager.OnlineUserSession.FindUserID(s);
			GameManager.OnlineUserSession.RemoveSession(s);
			GameManager.OnlineUserSession.RemoveUserName(s);
			GameManager.OnlineUserSession.RemoveUserAdult(s);
			if (!string.IsNullOrEmpty(text))
			{
				Global.RegisterUserIDToDBServer(text, 0, s.ServerId, ref s.session.LastLogoutServerTicks);
			}
			GameManager.loginWaitLogic.RemoveWait(text);
			if (!flag2)
			{
				GameManager.loginWaitLogic.RemoveAllow(text);
			}
			Global._SendBufferManager.Remove(s);
		}

		public string GetAllCacheCmdPacketInfo()
		{
			int num = 0;
			int num2 = 0;
			lock (this.dictInPackets)
			{
				for (int i = 0; i < this.dictInPackets.Values.Count; i++)
				{
					TCPInPacket tcpinPacket = this.dictInPackets.Values.ElementAt(i);
					if (tcpinPacket.GetCacheCmdPacketCount() > num2)
					{
						num2 = tcpinPacket.GetCacheCmdPacketCount();
					}
					num += tcpinPacket.GetCacheCmdPacketCount();
				}
			}
			return string.Format("总共缓存命令包{0}个,单个连接最大缓存{1}个", num, num2);
		}

		private TCPInPacket GetNextTcpInPacket(int index)
		{
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.Values.Count > index && index >= 0)
				{
					return this.dictInPackets.Values.ElementAt(index);
				}
			}
			return null;
		}

		public void ProcessCmdPackets(Queue<CmdPacket> ls)
		{
			int num = this.dictInPackets.Values.Count + 20;
			for (int i = 0; i < num; i++)
			{
				TCPInPacket nextTcpInPacket = this.GetNextTcpInPacket(i);
				if (null == nextTcpInPacket)
				{
					break;
				}
				ls.Clear();
				if (nextTcpInPacket.PopCmdPackets(ls))
				{
					try
					{
						while (ls.Count > 0)
						{
							CmdPacket cmdPacket = ls.Dequeue();
							TCPOutPacket tcpoutPacket = null;
							TCPProcessCmdResults tcpprocessCmdResults = TCPCmdHandler.ProcessCmd(this, nextTcpInPacket.CurrentSocket, this.tcpClientPool, this.tcpRandKey, this.tcpOutPacketPool, cmdPacket.CmdID, cmdPacket.Data, cmdPacket.Data.Length, out tcpoutPacket);
							if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && null != tcpoutPacket)
							{
								this.socketListener.SendData(nextTcpInPacket.CurrentSocket, tcpoutPacket, true);
							}
							else
							{
								if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
								{
									if (cmdPacket.CmdID != 22)
									{
										LogManager.WriteLog(2, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)nextTcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(nextTcpInPacket.CurrentSocket, false)), null, true);
									}
									this.socketListener.CloseSocket(nextTcpInPacket.CurrentSocket, "");
									break;
								}
								if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA_CLOSE && null != tcpoutPacket)
								{
									this.socketListener.SendData(nextTcpInPacket.CurrentSocket, tcpoutPacket, true);
									nextTcpInPacket.CurrentSocket.DelayClose = 250;
								}
							}
						}
					}
					finally
					{
						nextTcpInPacket.OnThreadDealingComplete();
					}
				}
			}
		}

		public static void RecordCmdDetail(int cmdId, long processTime, long dataSize, TCPProcessCmdResults result)
		{
			PorcessCmdMoniter porcessCmdMoniter = null;
			if (!ProcessSessionTask.cmdMoniter.TryGetValue(cmdId, out porcessCmdMoniter))
			{
				porcessCmdMoniter = new PorcessCmdMoniter(cmdId, processTime);
				porcessCmdMoniter = ProcessSessionTask.cmdMoniter.GetOrAdd(cmdId, porcessCmdMoniter);
			}
			porcessCmdMoniter.onProcessNoWait(processTime, dataSize, result);
		}

		public static void RecordCmdDetail2(int cmdId, long processTime, long waitTime)
		{
			PorcessCmdMoniter porcessCmdMoniter = null;
			if (!ProcessSessionTask.cmdMoniter.TryGetValue(cmdId, out porcessCmdMoniter))
			{
				porcessCmdMoniter = new PorcessCmdMoniter(cmdId, processTime);
				porcessCmdMoniter = ProcessSessionTask.cmdMoniter.GetOrAdd(cmdId, porcessCmdMoniter);
			}
			porcessCmdMoniter.onProcess(processTime, waitTime);
		}

		public static void RecordCmdOutputDataSize(int cmdId, long dataSize)
		{
			PorcessCmdMoniter porcessCmdMoniter = null;
			if (!ProcessSessionTask.cmdMoniter.TryGetValue(cmdId, out porcessCmdMoniter))
			{
				porcessCmdMoniter = new PorcessCmdMoniter(cmdId, 0L);
				porcessCmdMoniter = ProcessSessionTask.cmdMoniter.GetOrAdd(cmdId, porcessCmdMoniter);
			}
			porcessCmdMoniter.OnOutputData(dataSize);
		}

		private byte[] CheckClientDataValid(int packetCmdID, byte[] bytesData, int dataSize, int lastClientCheckTicks, out int clientCheckTicks, out int errorCode)
		{
			errorCode = 0;
			clientCheckTicks = 0;
			byte[] result;
			if (dataSize < 5)
			{
				errorCode = 1;
				result = null;
			}
			else
			{
				int num = (int)bytesData[0];
				clientCheckTicks = BitConverter.ToInt32(bytesData, 1);
				if (clientCheckTicks < lastClientCheckTicks)
				{
					errorCode = 2;
					result = null;
				}
				else
				{
					CRC32 crc = new CRC32();
					crc.update(bytesData, 1, dataSize - 1);
					uint num2 = crc.getValue() % 255U;
					uint num3 = (uint)(packetCmdID % 255);
					int num4 = (int)(num2 ^ num3);
					if (num4 != num)
					{
						errorCode = 3;
						result = null;
					}
					else
					{
						byte[] array = new byte[dataSize - 1 - 4];
						DataHelper.CopyBytes(array, 0, bytesData, 5, dataSize - 1 - 4);
						result = array;
					}
				}
			}
			return result;
		}

		private bool TCPCmdPacketEvent(object sender, int doType)
		{
			TCPInPacket tcpinPacket = sender as TCPInPacket;
			if (0 == doType)
			{
				int num = 0;
				int num2 = 0;
				byte[] array = this.CheckClientDataValid((int)tcpinPacket.PacketCmdID, tcpinPacket.GetPacketBytes(), tcpinPacket.PacketDataSize, tcpinPacket.LastCheckTicks, out num, out num2);
				if (null == array)
				{
					TMSKSocket currentSocket = tcpinPacket.CurrentSocket;
					string text = (currentSocket != null) ? GameManager.OnlineUserSession.FindUserID(currentSocket) : "socket is nil";
					LogManager.WriteLog(2, string.Format("校验客户端发送的指令数据完整性失败: {0},{1}, 错误码:{2}, uid={3}, 关闭连接", new object[]
					{
						(TCPGameServerCmds)tcpinPacket.PacketCmdID,
						Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket, false),
						num2,
						text
					}), null, true);
					return false;
				}
				tcpinPacket.LastCheckTicks = num;
				tcpinPacket.CurrentSocket.ClientCmdSecs = num;
				TCPSession tcpsession = null;
				if (null != tcpinPacket.CurrentSocket)
				{
					tcpsession = tcpinPacket.CurrentSocket.session;
				}
				if (null == tcpsession)
				{
					LogManager.WriteLog(2, string.Format("未与客户端建立会话: {0},{1}, 错误码:{2}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket, false), num2), null, true);
					return false;
				}
				int num3 = 0;
				tcpsession.CheckCmdNum((int)tcpinPacket.PacketCmdID, (long)num, out num3);
				if (num3 > 0)
				{
					int gameConfigItemInt = GameManager.PlatConfigMgr.GetGameConfigItemInt("ban-speed-up-minutes2", 10);
					GameClient gameClient = GameManager.ClientMgr.FindClient(tcpinPacket.CurrentSocket);
					if (null != gameClient)
					{
						if (GameManager.PlatConfigMgr.GetGameConfigItemInt("ban_speed_up_delay", 0) == 0 || gameClient.CheckCheatData.ProcessBooster)
						{
							GameManager.ClientMgr.NotifyImportantMsg(this.MySocketListener, this.tcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(663, new object[0]), new object[]
							{
								gameConfigItemInt
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
							BanManager.BanRoleName(Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName), gameConfigItemInt, 1);
							LogManager.WriteLog(2, string.Format("通过POSITION指令判断客户端加速: {0}, 指令个数:{1}, 断开连接", Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket, false), num3), null, true);
							return false;
						}
						if (gameClient.CheckCheatData.ProcessBoosterTicks == 0L)
						{
							gameClient.CheckCheatData.ProcessBoosterTicks = TimeUtil.NOW();
						}
					}
				}
				TCPOutPacket tcpoutPacket = null;
				long num4 = TimeUtil.NowEx();
				TCPProcessCmdResults tcpprocessCmdResults = TCPCmdHandler.ProcessCmd(this, tcpinPacket.CurrentSocket, this.tcpClientPool, this.tcpRandKey, this.tcpOutPacketPool, (int)tcpinPacket.PacketCmdID, array, tcpinPacket.PacketDataSize - 1 - 4, out tcpoutPacket);
				long num5 = TimeUtil.NowEx() - num4;
				if (GameManager.StatisticsMode > 0 || num5 > 50L || tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					TCPManager.RecordCmdDetail((int)tcpinPacket.PacketCmdID, num5, (long)tcpinPacket.PacketDataSize, tcpprocessCmdResults);
				}
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && null != tcpoutPacket)
				{
					this.socketListener.SendData(tcpinPacket.CurrentSocket, tcpoutPacket, true);
				}
				else
				{
					if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
					{
						if (tcpinPacket.PacketCmdID != 22)
						{
							LogManager.WriteLog(2, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket, false)), null, true);
						}
						return false;
					}
					if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA_CLOSE && null != tcpoutPacket)
					{
						this.socketListener.SendData(tcpinPacket.CurrentSocket, tcpoutPacket, true);
						tcpinPacket.CurrentSocket.DelayClose = 250;
					}
				}
			}
			else
			{
				if (1 != doType)
				{
					LogManager.WriteLog(2, string.Format("解析并执行命令时类型未知: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket, false)), null, true);
					return false;
				}
				this.DirectSendPolicyFileData(tcpinPacket);
			}
			return true;
		}

		public ushort LastPacketCmdID(TMSKSocket s)
		{
			ushort result = 0;
			if (s != null && this.dictInPackets != null)
			{
				lock (this.dictInPackets)
				{
					TCPInPacket tcpinPacket = null;
					if (this.dictInPackets.TryGetValue(s, out tcpinPacket))
					{
						result = tcpinPacket.LastPacketCmdID;
					}
				}
			}
			return result;
		}

		public Dictionary<TMSKSocket, TCPSession> GetTCPSessions()
		{
			throw new NotSupportedException("因为未被用到,优化tcpSessions的锁后,这个函数未兼容实现");
		}

		private void SocketConnected(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			TMSKSocket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			if (null == currentSocket.session)
			{
				currentSocket.session = new TCPSession(currentSocket);
			}
			currentSocket.SortKey64 = DataHelper.SortKey64;
		}

		private void SocketClosed(object sender, TMSKSocket s)
		{
			SocketListener socketListener = sender as SocketListener;
			this.ExternalClearSocket(s);
		}

		public void ExternalClearSocket(TMSKSocket s)
		{
			this.ForceCloseSocket(s);
			if (null != s.session)
			{
				s.session.release();
			}
			s.MyDispose();
		}

		private bool SocketReceived(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			TCPInPacket tcpinPacket = null;
			AsyncUserToken asyncUserToken = e.UserToken as AsyncUserToken;
			TMSKSocket currentSocket = asyncUserToken.CurrentSocket;
			tcpinPacket = currentSocket._TcpInPacket;
			if (null == tcpinPacket)
			{
				lock (this.dictInPackets)
				{
					if (!this.dictInPackets.TryGetValue(currentSocket, out tcpinPacket))
					{
						tcpinPacket = this.tcpInPacketPool.Pop(currentSocket, new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent));
						this.dictInPackets[currentSocket] = tcpinPacket;
					}
				}
				currentSocket._TcpInPacket = tcpinPacket;
			}
			return tcpinPacket.WriteData(e.Buffer, e.Offset, e.BytesTransferred);
		}

		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			AsyncUserToken asyncUserToken = e.UserToken as AsyncUserToken;
			SendBuffer sendBuffer = asyncUserToken._SendBuffer;
			if (null != sendBuffer)
			{
				sendBuffer.Reset2();
			}
			TMSKSocket currentSocket = asyncUserToken.CurrentSocket;
			Global._SendBufferManager.OnSendBufferOK(currentSocket);
		}

		private void DirectSendPolicyFileData(TCPInPacket tcpInPacket)
		{
			TMSKSocket currentSocket = tcpInPacket.CurrentSocket;
			try
			{
				currentSocket.Send(TCPPolicy.PolicyServerFileContent, TCPPolicy.PolicyServerFileContent.Length, SocketFlags.None);
				LogManager.WriteLog(0, string.Format("向客户端返回策略文件数据: {0}", Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
			}
			catch (Exception)
			{
				LogManager.WriteLog(0, string.Format("向客户端返回策略文件时，socket出现异常，对方已经关闭: {0}", Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
			}
		}

		public const bool UseWorkerPool = false;

		private static TCPManager instance = new TCPManager();

		public static int ServerPort = 0;

		public ScheduleExecutor taskExecutor = null;

		public int MaxConnectedClientLimit = 0;

		private SocketListener socketListener = null;

		private TCPInPacketPool tcpInPacketPool = null;

		private TCPOutPacketPool tcpOutPacketPool = null;

		private TCPClientPool _tcpClientPool = null;

		private TCPClientPool _tcpLogClientPool = null;

		private TCPRandKey _tcpRandKey = new TCPRandKey(10000);

		private Dictionary<TMSKSocket, TCPInPacket> dictInPackets = null;

		private Dictionary<TMSKSocket, TCPSession> tcpSessions = null;
	}
}
