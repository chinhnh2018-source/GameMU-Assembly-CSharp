using System;
using System.Collections.Generic;
using Server.TCP;

namespace Server.Protocol
{
	public class TCPInPacketPool
	{
		internal TCPInPacketPool(int capacity)
		{
			this.pool = new Queue<TCPInPacket>(capacity);
		}

		internal int Count
		{
			get
			{
				int result = 0;
				lock (this.pool)
				{
					result = this.pool.Count;
				}
				return result;
			}
		}

		internal TCPInPacket Pop(TMSKSocket s, TCPCmdPacketEventHandler TCPCmdPacketEvent)
		{
			TCPInPacket result;
			lock (this.pool)
			{
				if (this.pool.Count <= 0)
				{
					TCPInPacket tcpinPacket = new TCPInPacket(6144)
					{
						CurrentSocket = s
					};
					tcpinPacket.TCPCmdPacketEvent += TCPCmdPacketEvent;
					result = tcpinPacket;
				}
				else
				{
					TCPInPacket tcpinPacket = this.pool.Dequeue();
					tcpinPacket.CurrentSocket = s;
					result = tcpinPacket;
				}
			}
			return result;
		}

		internal void Push(TCPInPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPInPacketPool 的item不能是空(null)");
			}
			lock (this.pool)
			{
				item.Reset();
				this.pool.Enqueue(item);
			}
		}

		private Queue<TCPInPacket> pool;
	}
}
