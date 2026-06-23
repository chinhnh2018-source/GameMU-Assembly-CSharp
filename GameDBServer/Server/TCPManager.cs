using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameDBServer.Server
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
			capacity = Math.Max(capacity, 250);
			this.socketListener = new SocketListener(capacity, 32768);
			this.socketListener.SocketClosed += this.SocketClosed;
			this.socketListener.SocketConnected += this.SocketConnected;
			this.socketListener.SocketReceived += this.SocketReceived;
			this.socketListener.SocketSended += this.SocketSended;
			this.tcpInPacketPool = new TCPInPacketPool(capacity);
			this.tcpOutPacketPool = TCPOutPacketPool.getInstance();
			this.tcpOutPacketPool.initialize(capacity * 5);
			TCPCmdDispatcher.getInstance().initialize();
			this.dictInPackets = new Dictionary<Socket, TCPInPacket>(capacity);
			this.gameServerClients = new Dictionary<Socket, GameServerClient>();
		}

		public GameServerClient getClient(Socket socket)
		{
			GameServerClient result = null;
			this.gameServerClients.TryGetValue(socket, out result);
			return result;
		}

		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		public Program RootWindow { get; set; }

		public DBManager DBMgr { get; set; }

		public void Start(string ip, int port)
		{
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		public void Stop()
		{
			this.socketListener.Stop();
		}

		private bool TCPCmdPacketEvent(object sender)
		{
			TCPInPacket tcpinPacket = sender as TCPInPacket;
			TCPOutPacket tcpoutPacket = null;
			GameServerClient gameServerClient = null;
			bool result;
			if (!this.gameServerClients.TryGetValue(tcpinPacket.CurrentSocket, out gameServerClient))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未建立会话或会话已关闭: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket)), null, true);
				result = false;
			}
			else
			{
				TCPManager.CurrentClient = gameServerClient;
				long num = TimeUtil.NowEx();
				TCPProcessCmdResults tcpprocessCmdResults = TCPCmdHandler.ProcessCmd(gameServerClient, this.DBMgr, this.tcpOutPacketPool, (int)tcpinPacket.PacketCmdID, tcpinPacket.GetPacketBytes(), tcpinPacket.PacketDataSize, out tcpoutPacket);
				long processTime = TimeUtil.NowEx() - num;
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && null != tcpoutPacket)
				{
					this.socketListener.SendData(tcpinPacket.CurrentSocket, tcpoutPacket);
				}
				else if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket)), null, true);
					return false;
				}
				lock (TCPManager.cmdMoniter)
				{
					int packetCmdID = (int)tcpinPacket.PacketCmdID;
					PorcessCmdMoniter porcessCmdMoniter = null;
					if (!TCPManager.cmdMoniter.TryGetValue(packetCmdID, out porcessCmdMoniter))
					{
						porcessCmdMoniter = new PorcessCmdMoniter(packetCmdID, processTime);
						TCPManager.cmdMoniter.Add(packetCmdID, porcessCmdMoniter);
					}
					porcessCmdMoniter.onProcessNoWait(processTime);
				}
				TCPManager.CurrentClient = null;
				result = true;
			}
			return result;
		}

		private void SocketConnected(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			this.RootWindow.TotalConnections = socketListener.ConnectedSocketsCount;
			lock (this.gameServerClients)
			{
				GameServerClient value = null;
				Socket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
				if (!this.gameServerClients.TryGetValue(currentSocket, out value))
				{
					value = new GameServerClient(currentSocket);
					this.gameServerClients.Add(currentSocket, value);
				}
			}
		}

		private void SocketClosed(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			Socket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.ContainsKey(currentSocket))
				{
					TCPInPacket item = this.dictInPackets[currentSocket];
					this.tcpInPacketPool.Push(item);
					this.dictInPackets.Remove(currentSocket);
				}
			}
			lock (this.gameServerClients)
			{
				GameServerClient gameServerClient = null;
				if (this.gameServerClients.TryGetValue(currentSocket, out gameServerClient))
				{
					gameServerClient.release();
					this.gameServerClients.Remove(currentSocket);
				}
			}
			this.RootWindow.TotalConnections = socketListener.ConnectedSocketsCount;
		}

		private bool SocketReceived(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			TCPInPacket tcpinPacket = null;
			Socket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (!this.dictInPackets.TryGetValue(currentSocket, out tcpinPacket))
				{
					tcpinPacket = this.tcpInPacketPool.Pop(currentSocket, new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent));
					this.dictInPackets[currentSocket] = tcpinPacket;
				}
			}
			return tcpinPacket.WriteData(e.Buffer, e.Offset, e.BytesTransferred);
		}

		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			TCPOutPacket item = (e.UserToken as AsyncUserToken).Tag as TCPOutPacket;
			this.tcpOutPacketPool.Push(item);
		}

		private static TCPManager instance = new TCPManager();

		public static long processCmdNum = 0L;

		public static long processTotalTime = 0L;

		public static Dictionary<int, PorcessCmdMoniter> cmdMoniter = new Dictionary<int, PorcessCmdMoniter>();

		private SocketListener socketListener = null;

		private TCPInPacketPool tcpInPacketPool = null;

		private TCPOutPacketPool tcpOutPacketPool = null;

		[ThreadStatic]
		public static GameServerClient CurrentClient;

		private Dictionary<Socket, TCPInPacket> dictInPackets = null;

		private Dictionary<Socket, GameServerClient> gameServerClients = null;
	}
}
