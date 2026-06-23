using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	public class TCPClient
	{
		public IConnectInfoContainer RootWindow { get; set; }

		public int ListIndex { get; set; }

		public bool ValidateIpPort(string ip, int port)
		{
			return !(ip != this.ServerIP) && port == this.ServerPort;
		}

		public void Connect(string ip, int port, string serverName)
		{
			this.ServerName = serverName;
			lock (this.MutexSocket)
			{
				if (null == this._Socket)
				{
					this.ServerIP = ip;
					this.ServerPort = port;
					IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Parse(ip), port);
					this._Socket = new TMSKSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					this._Socket.SendTimeout = 30000;
					this._Socket.ReceiveTimeout = 30000;
					this._Socket.NoDelay = this.NoDelay;
					try
					{
						this._Socket.Connect(ipendPoint);
					}
					catch (Exception)
					{
						this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}:{3}建立连接失败", new object[]
						{
							this.ListIndex,
							this.ServerName,
							ip,
							port
						}));
						LogManager.WriteLog(2, string.Format("{0}, 与{1}: {2}:{3}建立连接失败", new object[]
						{
							this.ListIndex,
							this.ServerName,
							ip,
							port
						}), null, true);
						this._Socket = null;
						throw;
					}
					Global.SendGameServerHeart(this);
					this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}建立连接成功", this.ListIndex, this.ServerName, ipendPoint));
				}
			}
		}

		public void Disconnect()
		{
			lock (this.MutexSocket)
			{
				if (null != this._Socket)
				{
					this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}断开连接", this.ListIndex, this.ServerName, Global.GetSocketRemoteEndPoint(this._Socket, false)));
					try
					{
						this._Socket.Shutdown(SocketShutdown.Receive);
						this._Socket.Close(30);
					}
					catch (Exception)
					{
					}
					this._Socket = null;
				}
			}
		}

		public bool IsConnected()
		{
			bool result = false;
			lock (this.MutexSocket)
			{
				result = (null != this._Socket);
			}
			return result;
		}

		public byte[] SendData(TCPOutPacket tcpOutPacket)
		{
			byte[] result;
			lock (this.MutexSocket)
			{
				if (null == this._Socket)
				{
					result = null;
				}
				else
				{
					try
					{
						if (LogManager.LogTypeToWrite >= 0 && LogManager.LogTypeToWrite <= 1)
						{
						}
						this._Socket.Send(tcpOutPacket.GetPacketBytes(), tcpOutPacket.PacketDataSize, SocketFlags.None);
						byte[] array = new byte[4];
						int num = this._Socket.Receive(array, 0, 4, SocketFlags.None);
						if (num != 4)
						{
							LogManager.WriteLog(2, string.Format("{0}, 与{1}: {2}通讯失败, 获取数据包长度失败", this.ListIndex, this.ServerName, Global.GetSocketRemoteEndPoint(this._Socket, false)), null, true);
							return null;
						}
						int num2 = BitConverter.ToInt32(array, 0);
						byte[] array2 = new byte[num2 + 4];
						DataHelper.CopyBytes(array2, 0, array, 0, 4);
						array = array2;
						int i;
						for (i = 0; i < num2; i += num)
						{
							num = this._Socket.Receive(array, 4 + i, num2 - i, SocketFlags.None);
						}
						if (i != num2)
						{
							LogManager.WriteLog(2, string.Format("{0}, 与{1}: {2}通讯失败, 返回的数据包数据长度:{3}和接收到的数据长度:{4}不匹配", new object[]
							{
								this.ListIndex,
								this.ServerName,
								Global.GetSocketRemoteEndPoint(this._Socket, false),
								num2,
								i
							}), null, true);
							return null;
						}
						this.LastCmdID = (int)tcpOutPacket.PacketCmdID;
						return array;
					}
					catch (Exception ex)
					{
						this.Disconnect();
						try
						{
							string @string = new UTF8Encoding().GetString(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
							LogManager.WriteLog(2, string.Format("{0}, 和{1}:{2}通讯失败, 发送命令{3}, 数据 {4}, 长度{5}, 异常信息:{6}", new object[]
							{
								this.ListIndex,
								this.ServerName,
								Global.GetSocketRemoteEndPoint(this._Socket, false),
								(TCPGameServerCmds)tcpOutPacket.PacketCmdID,
								@string,
								tcpOutPacket.PacketDataSize - 6,
								ex.Message
							}), null, true);
							DataHelper.WriteExceptionLogEx(ex, string.Format("和{0}通讯发生异常", this.ServerName));
						}
						catch (Exception)
						{
						}
						this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}通讯失败", this.ListIndex, this.ServerName, Global.GetSocketRemoteEndPoint(this._Socket, false)));
					}
					result = null;
				}
			}
			return result;
		}

		private object MutexSocket = new object();

		private TMSKSocket _Socket = null;

		private string ServerIP = "";

		private int ServerPort = 0;

		public string ServerName = "";

		public bool NoDelay = false;

		public int LastCmdID;
	}
}
