using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Network.Protocol
{
	public class TCPOutPacketPool
	{
		public TCPOutPacketPool(int capacity)
		{
			this.xb3a098bf0147fd2c = new Stack<TCPOutPacket>(capacity);
		}

		public int Count
		{
			get
			{
				int result = 0;
				lock (this.xb3a098bf0147fd2c)
				{
					result = this.xb3a098bf0147fd2c.Count;
				}
				return result;
			}
		}

		public TCPOutPacket Pop()
		{
			TCPOutPacket result;
			lock (this.xb3a098bf0147fd2c)
			{
				if (this.xb3a098bf0147fd2c.Count <= 0)
				{
					result = new TCPOutPacket();
				}
				else
				{
					result = this.xb3a098bf0147fd2c.Pop();
				}
			}
			return result;
		}

		public void Push(TCPOutPacket item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("添加到TCPOutPacketPool 的item不能是空(null)");
			}
			lock (this.xb3a098bf0147fd2c)
			{
				item.Reset();
				this.xb3a098bf0147fd2c.Push(item);
			}
		}

		private Stack<TCPOutPacket> xb3a098bf0147fd2c;
	}
}
