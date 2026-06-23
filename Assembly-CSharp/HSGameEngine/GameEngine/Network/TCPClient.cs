using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using HSGameEngine.GameEngine.Network.Protocol;
using Server.Tools;

namespace HSGameEngine.GameEngine.Network
{
	public class TCPClient
	{
		public TCPClient(int capacity = 2)
		{
			this._Socket = new Socket(2, 1, 6);
			this._MyTCPInPacket = new TCPInPacket(131072);
			this._MyTCPInPacket.TCPCmdPacketEvent += new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent);
			this.tcpOutPacketPool = new TCPOutPacketPool(capacity * 5);
		}

		public event SocketConnectEventHandler SocketConnect;

		public void Destroy()
		{
			this.StopConnectTimer();
			this._MyTCPInPacket.TCPCmdPacketEvent -= new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent);
			this._Socket = null;
		}

		public TCPOutPacketPool OutPacketPool
		{
			get
			{
				return this.tcpOutPacketPool;
			}
		}

		public bool Connected
		{
			get
			{
				return this._Connected;
			}
			set
			{
				this._Connected = value;
			}
		}

		public string RemoteIP
		{
			get
			{
				return this._ServerIP;
			}
			set
			{
				this._ServerIP = value;
			}
		}

		public int RemotePort
		{
			get
			{
				return this._ServerPort;
			}
			set
			{
				this._ServerPort = value;
			}
		}

		public Socket CurrentSocket
		{
			get
			{
				return this._Socket;
			}
		}

		public TCPInPacket MyTCPInPacket
		{
			get
			{
				return this._MyTCPInPacket;
			}
		}

		public long LastSendDataTicks
		{
			get
			{
				return this._LastSendDataTicks;
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				return this._ReceiveTimeout;
			}
			set
			{
				this._ReceiveTimeout = value;
			}
		}

		public int SendTimeout
		{
			get
			{
				return this._SendTimeout;
			}
			set
			{
				this._SendTimeout = value;
			}
		}

		public bool Connect(string ip, int port)
		{
			if (this._Connected || this._Socket == null)
			{
				return false;
			}
			this._ServerIP = ip;
			this._ServerPort = port;
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"_ServerIP = ",
					this._ServerIP,
					"__port:",
					port
				})
			});
			try
			{
				this.timerConnectTimeout = new Timer(15000.0);
				this.timerConnectTimeout.Elapsed += new ElapsedEventHandler(this.CheckConnectTimeout);
				this.timerConnectTimeout.Interval = 15000.0;
				this.timerConnectTimeout.Enabled = true;
				IPAddress[] hostAddresses = Dns.GetHostAddresses(this._ServerIP);
				IPAddress ipaddress = hostAddresses[0];
				IPEndPoint ipendPoint = new IPEndPoint(ipaddress, this._ServerPort);
				this._Socket.ReceiveTimeout = this._ReceiveTimeout;
				this._Socket.SendTimeout = this._SendTimeout;
				this.bSocketConnectCallbacked = false;
				this._Socket.BeginConnect(ipendPoint, new AsyncCallback(this.SocketConnected), null);
				return true;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return false;
		}

		private void StopConnectTimer()
		{
			if (this.timerConnectTimeout != null)
			{
				this.timerConnectTimeout.Stop();
				this.timerConnectTimeout.Enabled = false;
				this.timerConnectTimeout = null;
			}
		}

		private void CheckConnectTimeout(object source, ElapsedEventArgs e)
		{
			this.StopConnectTimer();
			if (!this.bSocketConnectCallbacked)
			{
				if (this.SocketConnect != null)
				{
					this.SocketConnect(this, new MUSocketConnectEventArgs
					{
						RemoteEndPoint = this.GetRemoteEndPoint(),
						Error = "Connect Fail",
						NetSocketType = 0
					});
				}
				this.Disconnect(2);
			}
		}

		public bool SendData(TCPOutPacket tcpOutPacket)
		{
			if (!(((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SPR_CHECK") && !(((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SPR_POSITION") && !(((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SPR_USEGOODS") && !(((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SPR_MOVE") && !(((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SPR_IFQINGGONGYANOPEN") && !(((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SYNC_TIME_BY_CLIENT"))
			{
				if (MUDebug.IsOpenDebug)
				{
					MUDebug.Log<string>(new string[]
					{
						"<color=green>客户端 发送给服务端的消息:</color>" + ((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString()
					});
				}
			}
			if (((TCPGameServerCmds)tcpOutPacket.PacketCmdID).ToString() == "CMD_SPR_MAGICCODE")
			{
			}
			if (this._Socket == null)
			{
				return false;
			}
			if (!this._Socket.Connected)
			{
				return false;
			}
			try
			{
				DataHelper.SortBytes(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
				TCPClient.snTotalSendCount += tcpOutPacket.PacketDataSize;
				this._Socket.BeginSend(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize, 0, new AsyncCallback(this.SocketSended), null);
				this._LastSendDataTicks = DateTime.Now.Ticks / 10000L;
				return true;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			if (this.SocketConnect != null)
			{
				this.SocketConnect(this, new MUSocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Failed",
					NetSocketType = 1
				});
			}
			return false;
		}

		public void Disconnect(SocketShutdown how = 2)
		{
			if (this._Socket == null)
			{
				return;
			}
			this._Connected = false;
			if (this._Socket != null && this._Socket.Connected)
			{
				this._Socket.Shutdown(how);
			}
			this._Socket = null;
		}

		public void NotifyRecvData(MUSocketConnectEventArgs e)
		{
			if (this.SocketConnect != null)
			{
				this.SocketConnect(this, e);
			}
		}

		public string GetRemoteEndPoint()
		{
			try
			{
				return StringUtil.substitute("{0}:{1}", new object[]
				{
					this._ServerIP,
					this._ServerPort
				});
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return null;
		}

		private bool TCPCmdPacketEvent(object sender)
		{
			TCPInPacket tcpinPacket = sender as TCPInPacket;
			Socket currentSocket = tcpinPacket.CurrentSocket;
			return TCPCmdHandler.ProcessServerCmd(this, tcpinPacket.PacketCmdID, tcpinPacket.GetPacketBytes(), tcpinPacket.PacketDataSize);
		}

		private void SocketConnected(IAsyncResult iar)
		{
			try
			{
				if (this._Socket != null)
				{
					this._Socket.EndConnect(iar);
					if (ProtocolTypes.EnableTengXunTGW)
					{
						string text = StringUtil.substitute("tgw_l7_forward\r\nHost: {0}:{1}\r\n\r\n", new object[]
						{
							this._ServerIP,
							this._ServerPort
						});
						byte[] bytes = new UTF8Encoding().GetBytes(text);
						this._Socket.Send(bytes);
					}
					this._Connected = true;
					this._Socket.BeginReceive(this.mReceiveBuffer, 0, this.mReceiveBuffer.Length, 0, new AsyncCallback(this.SocketReceived), this.mReceiveBuffer);
					if (this.SocketConnect != null)
					{
						DataHelper.ClearKey();
						this.SocketConnect(this, new MUSocketConnectEventArgs
						{
							RemoteEndPoint = this.GetRemoteEndPoint(),
							Error = "Success",
							NetSocketType = 0
						});
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		private void DoSocketClosed()
		{
			MUDebug.LogStackMsg("DoSocketClosed: \r\n");
			if (this._Socket != null && this.SocketConnect != null)
			{
				this.SocketConnect(this, new MUSocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Success",
					NetSocketType = 3
				});
			}
			if (this._Socket != null)
			{
				this._Socket.Close();
				this._Socket = null;
			}
			this._Connected = false;
		}

		private void SocketReceived(IAsyncResult iar)
		{
			try
			{
				if (this._Socket == null)
				{
					return;
				}
				SocketError socketError = 0;
				int num = 0;
				if (this._Socket.Connected)
				{
					num = this._Socket.EndReceive(iar, ref socketError);
				}
				if (num <= 0)
				{
					this.DoSocketClosed();
					return;
				}
				TCPClient.snTotalRecvCount += num;
				if (!this._MyTCPInPacket.WriteData(this.mReceiveBuffer, 0, num))
				{
					return;
				}
				if (!this.bSocketConnectCallbacked)
				{
					this.bSocketConnectCallbacked = true;
					this.StopConnectTimer();
				}
				this._Socket.BeginReceive(this.mReceiveBuffer, 0, this.mReceiveBuffer.Length, 0, new AsyncCallback(this.SocketReceived), this.mReceiveBuffer);
				return;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			if (this.SocketConnect != null)
			{
				this.SocketConnect(this, new MUSocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Failed",
					NetSocketType = 2
				});
			}
		}

		private void SocketSended(IAsyncResult iar)
		{
			try
			{
				if (this._Socket == null)
				{
					return;
				}
				SocketError socketError = 0;
				this._Socket.EndSend(iar, ref socketError);
				if (socketError != null)
				{
					this.DoSocketClosed();
					return;
				}
				return;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			if (this.SocketConnect != null)
			{
				this.SocketConnect(this, new MUSocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Failed",
					NetSocketType = 1
				});
			}
		}

		public static ProcessServerCmdHandler ProcessServerCmd;

		public static int snTotalSendCount;

		public static int snTotalRecvCount;

		private byte[] mReceiveBuffer = new byte[6144];

		private TCPOutPacketPool tcpOutPacketPool;

		private bool _Connected;

		private string _ServerIP = string.Empty;

		private int _ServerPort;

		private Socket _Socket;

		private TCPInPacket _MyTCPInPacket;

		private long _LastSendDataTicks;

		private int _ReceiveTimeout = 1000000;

		private int _SendTimeout = 10000;

		private Timer timerConnectTimeout;

		private bool bSocketConnectCallbacked;
	}
}
