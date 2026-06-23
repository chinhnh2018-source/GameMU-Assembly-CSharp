using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace HSGameEngine.GameEngine.Network.Protocol
{
	public class TCPInPacketPool
	{
		internal TCPInPacketPool(int capacity)
		{
			this.xb3a098bf0147fd2c = new Stack<TCPInPacket>(capacity);
		}

		internal int xd44988f225497f3a
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

		internal TCPInPacket x47c79a4d207183de(Socket xe4115acdf4fbfccc, TCPCmdPacketEventHandler xd0ab16336c9c9beb)
		{
			TCPInPacket result;
			lock (this.xb3a098bf0147fd2c)
			{
				TCPInPacket tcpinPacket = null;
				if (-1 != 0)
				{
					goto IL_41;
				}
				IL_16:
				tcpinPacket.CurrentSocket = xe4115acdf4fbfccc;
				result = tcpinPacket;
				if (4 == 0 || 2 != 0)
				{
					return result;
				}
				IL_41:
				if (this.xb3a098bf0147fd2c.Count > 0)
				{
					tcpinPacket = this.xb3a098bf0147fd2c.Pop();
					goto IL_16;
				}
				tcpinPacket = new TCPInPacket(131072);
				tcpinPacket.CurrentSocket = xe4115acdf4fbfccc;
				tcpinPacket.TCPCmdPacketEvent += xd0ab16336c9c9beb;
				result = tcpinPacket;
			}
			return result;
		}

		internal void x1914eddf1fde1228(TCPInPacket xccb63ca5f63dc470)
		{
			if (xccb63ca5f63dc470 == null)
			{
				throw new ArgumentNullException("添加到TCPInPacketPool 的item不能是空(null)");
			}
			lock (this.xb3a098bf0147fd2c)
			{
				xccb63ca5f63dc470.Reset();
				this.xb3a098bf0147fd2c.Push(xccb63ca5f63dc470);
			}
		}

		public void Destroy()
		{
			lock (this.xb3a098bf0147fd2c)
			{
				while (this.xb3a098bf0147fd2c.Count > 0)
				{
					TCPInPacket tcpinPacket = this.xb3a098bf0147fd2c.Pop();
					tcpinPacket.Destroy();
				}
			}
		}

		private Stack<TCPInPacket> xb3a098bf0147fd2c;
	}
}
