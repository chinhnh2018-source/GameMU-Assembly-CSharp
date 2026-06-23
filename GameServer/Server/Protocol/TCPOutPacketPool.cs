using System;
using System.Collections.Generic;

namespace Server.Protocol
{
	public class TCPOutPacketPool
	{
		private TCPOutPacketPool()
		{
		}

		public static TCPOutPacketPool getInstance()
		{
			return TCPOutPacketPool.instance;
		}

		public void initialize(int capacity)
		{
			this.pool = new Stack<TCPOutPacket>(capacity);
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

		internal TCPOutPacket Pop()
		{
			return new TCPOutPacket();
		}

		internal void Push(TCPOutPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPOutPacketPool 的item不能是空(null)");
			}
			item.Reset();
			item.Dispose();
		}

		private Stack<TCPOutPacket> pool;

		private static TCPOutPacketPool instance = new TCPOutPacketPool();
	}
}
