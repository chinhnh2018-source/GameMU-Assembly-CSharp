using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Logic;
using GameServer.Server;
using Server.TCP;
using Server.Tools;

namespace Server.Protocol
{
	public class TCPInPacket : IDisposable
	{
		public TCPInPacket(int recvBufferSize = 6144)
		{
			this.PacketBytes = new byte[recvBufferSize];
			TCPInPacket.IncInstanceCount();
		}

		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		public int LastCheckTicks
		{
			get
			{
				return this._LastCheckTicks;
			}
			set
			{
				this._LastCheckTicks = value;
			}
		}

		public TMSKSocket CurrentSocket
		{
			get
			{
				return this._Socket;
			}
			set
			{
				this._Socket = value;
			}
		}

		public ushort PacketCmdID
		{
			get
			{
				return this._PacketCmdID;
			}
		}

		public ushort LastPacketCmdID { get; set; }

		public int PacketDataSize
		{
			get
			{
				return this._PacketDataSize;
			}
		}

		public Queue<CmdPacket> CmdPacketPool
		{
			get
			{
				return this._cmdPacketPool;
			}
		}

		public bool IsDealingByWorkerThread
		{
			get
			{
				return this._isDealingByWorkerThread;
			}
			set
			{
				this._isDealingByWorkerThread = value;
			}
		}

		public bool CacheCmdPacketData(int nID, byte[] data, int count)
		{
			bool result;
			if (this._cmdPacketPool.Count > 100)
			{
				result = false;
			}
			else
			{
				lock (this._cmdPacketPool)
				{
					this._cmdPacketPool.Enqueue(new CmdPacket(nID, data, count));
				}
				result = true;
			}
			return result;
		}

		public bool PopCmdPackets(Queue<CmdPacket> ls)
		{
			ls.Clear();
			bool result;
			lock (this._cmdPacketPool)
			{
				if (this._isDealingByWorkerThread || this._cmdPacketPool.Count <= 0)
				{
					result = false;
				}
				else
				{
					this._isDealingByWorkerThread = true;
					for (int i = 0; i < 6; i++)
					{
						if (this._cmdPacketPool.Count <= 0)
						{
							break;
						}
						ls.Enqueue(this._cmdPacketPool.Dequeue());
					}
					result = true;
				}
			}
			return result;
		}

		public void OnThreadDealingComplete()
		{
			lock (this._cmdPacketPool)
			{
				this._isDealingByWorkerThread = false;
			}
		}

		public int GetCacheCmdPacketCount()
		{
			int result;
			lock (this._cmdPacketPool)
			{
				result = this._cmdPacketPool.Count<CmdPacket>();
			}
			return result;
		}

		public void Dispose()
		{
			this.Reset();
			TCPInPacket.DecInstanceCount();
		}

		public event TCPCmdPacketEventHandler TCPCmdPacketEvent;

		public bool WriteData(byte[] buffer, int offset, int count)
		{
			bool result;
			lock (this.mutex)
			{
				if (this.IsWaitingData)
				{
					int num = (count >= this._PacketDataSize - this.PacketDataHaveSize) ? (this._PacketDataSize - this.PacketDataHaveSize) : count;
					if (num > 0)
					{
						DataHelper.CopyBytes(this.PacketBytes, this.PacketDataHaveSize, buffer, offset, num);
						this.PacketDataHaveSize += num;
					}
					if (this.PacketDataHaveSize >= this._PacketDataSize)
					{
						bool flag2 = true;
						if (null != this.TCPCmdPacketEvent)
						{
							flag2 = this.TCPCmdPacketEvent(this, 0);
						}
						this.LastPacketCmdID = this._PacketCmdID;
						this._PacketCmdID = 0;
						this._PacketDataSize = 0;
						this.PacketDataHaveSize = 0;
						this.IsWaitingData = false;
						this.CmdHeaderSize = 0;
						if (!flag2)
						{
							return false;
						}
						if (count > num)
						{
							offset += num;
							count -= num;
							return this.WriteData(buffer, offset, count);
						}
					}
					result = true;
				}
				else
				{
					int num2 = (count > 6 - this.CmdHeaderSize) ? (6 - this.CmdHeaderSize) : count;
					DataHelper.CopyBytes(this.CmdHeaderBuffer, this.CmdHeaderSize, buffer, offset, num2);
					this.CmdHeaderSize += num2;
					if (this.CmdHeaderSize < 6)
					{
						result = true;
					}
					else
					{
						this._PacketDataSize = BitConverter.ToInt32(this.CmdHeaderBuffer, 0);
						this._PacketCmdID = BitConverter.ToUInt16(this.CmdHeaderBuffer, 4);
						if (this._Socket == null)
						{
							LogManager.WriteLog(2, string.Format("TcpInPacket.WriteData 检查到socket为null, 可能是别的线程关闭了该socket后重置TcpInPacket, 当前消息(极有可能不准):{0}[{1}]", (TCPGameServerCmds)this._PacketCmdID, this._PacketCmdID), null, true);
						}
						else if (this._Socket.magic > 0)
						{
							if (this._PacketCmdID > 100)
							{
								if (this._PacketCmdID <= 30767)
								{
									string arg = GameManager.OnlineUserSession.FindUserID(this._Socket);
									LogManager.WriteLog(1000, string.Format("客户端UID={0}, IP={1} 消息偏移后，消息({2})处于(CMD_LOGIN_ON, CMD_DB_ERR_RETURN]范围内", arg, this._Socket.RemoteEndPoint, (TCPGameServerCmds)this._PacketCmdID), null, false);
									return false;
								}
								this._PacketCmdID -= this._Socket.magic;
							}
						}
						if (this._PacketDataSize <= 0 || this._PacketDataSize >= 6144)
						{
							LogManager.WriteLog(2, string.Format("接收到的非法数据长度的tcp命令, Cmd={0}, Length={1}, offset={2}, count={3}", new object[]
							{
								(TCPGameServerCmds)this._PacketCmdID,
								this._PacketDataSize,
								offset,
								count
							}), null, true);
							result = false;
						}
						else
						{
							offset += num2;
							count -= num2;
							this.IsWaitingData = true;
							this.PacketDataHaveSize = 0;
							this._PacketDataSize -= 2;
							result = this.WriteData(buffer, offset, count);
						}
					}
				}
			}
			return result;
		}

		public void Reset()
		{
			lock (this.mutex)
			{
				this._Socket = null;
				this._PacketCmdID = 0;
				this.LastPacketCmdID = 0;
				this._PacketDataSize = 0;
				this.PacketDataHaveSize = 0;
				this.IsWaitingData = false;
				this.CmdHeaderSize = 0;
				this._cmdPacketPool.Clear();
				this._LastCheckTicks = 0;
			}
		}

		private bool HandlePolicyFileRequest(byte[] buffer, int offset, int count)
		{
			this.TCPCmdPacketEvent(this, 1);
			this._PacketCmdID = 0;
			this._PacketDataSize = 0;
			this.PacketDataHaveSize = 0;
			this.IsWaitingData = false;
			this.CmdHeaderSize = 0;
			this._LastCheckTicks = 0;
			return true;
		}

		public static void IncInstanceCount()
		{
			lock (TCPInPacket.CountLock)
			{
				TCPInPacket.TotalInstanceCount++;
			}
		}

		public static void DecInstanceCount()
		{
			lock (TCPInPacket.CountLock)
			{
				TCPInPacket.TotalInstanceCount--;
			}
		}

		public static int GetInstanceCount()
		{
			int result = 0;
			lock (TCPInPacket.CountLock)
			{
				result = TCPInPacket.TotalInstanceCount;
			}
			return result;
		}

		public const string POLICY_STRING = "<policy-file-request/>\0";

		private object mutex = new object();

		private byte[] PacketBytes = null;

		private int _LastCheckTicks = 0;

		private TMSKSocket _Socket = null;

		private ushort _PacketCmdID = 0;

		private int _PacketDataSize = 0;

		private Queue<CmdPacket> _cmdPacketPool = new Queue<CmdPacket>();

		private bool _isDealingByWorkerThread = false;

		private int PacketDataHaveSize = 0;

		private bool IsWaitingData = false;

		private byte[] CmdHeaderBuffer = new byte[6];

		private int CmdHeaderSize = 0;

		private static object CountLock = new object();

		private static int TotalInstanceCount = 0;
	}
}
