using System;
using System.Net;
using System.Net.Sockets;

namespace TianMa.Tools
{
	public class ICMPSocket
	{
		~ICMPSocket()
		{
			this.Close();
		}

		public void Close()
		{
			try
			{
				if (this.x70087761226fbdf2 != null)
				{
					this.x70087761226fbdf2.Close();
				}
			}
			catch
			{
			}
			this.x70087761226fbdf2 = null;
		}

		public IPPacket Send(string Host, ICMPPacket ICMP, int TTL)
		{
			if (this.x9396aeed6f94cb4a != Host)
			{
				goto IL_2CF;
			}
			goto IL_228;
			IL_1C7:
			this.TimeElapsed = Environment.TickCount;
			IL_1D2:
			this.x70087761226fbdf2.SendTo(ICMP.GetBytes(), this.x1b07d570d2c02338);
			IPPacket ippacket = null;
			if (false)
			{
				goto IL_1FA;
			}
			IL_1EF:
			byte[] buffer = new byte[2048];
			IL_1FA:
			this.x70087761226fbdf2.ReceiveFrom(buffer, ref this.EndPoint);
			ippacket = new IPPacket(ref buffer);
			IPPacket ip;
			if (ippacket.ICMP.Message is ICMPIPHeaderReply)
			{
				ip = ((ICMPIPHeaderReply)ippacket.ICMP.Message).IP;
				goto IL_11C;
			}
			goto IL_15B;
			IL_53:
			if (8 == 0)
			{
				goto IL_73;
			}
			IL_5A:
			this.TimeElapsed = Environment.TickCount - this.TimeElapsed;
			if (8 == 0)
			{
				goto IL_143;
			}
			goto IL_18C;
			IL_73:
			if (!(ip.ICMP.Message is ICMPEchoReply))
			{
				goto IL_143;
			}
			bool flag = ((uint)TTL & 0U) == 0U;
			if (flag)
			{
				flag = ((uint)TTL < 0U);
				if (!flag)
				{
					goto IL_5A;
				}
			}
			flag = (((uint)TTL & 0U) == 0U);
			if (flag)
			{
				goto IL_53;
			}
			goto IL_1C7;
			IL_11C:
			if (!(this.x1b07d570d2c02338.Address.ToString() == ip.DestinationAddress.ToString()))
			{
				goto IL_15B;
			}
			goto IL_73;
			IL_143:
			if ((uint)TTL + (uint)TTL > 4294967295U)
			{
				goto IL_11C;
			}
			IL_15B:
			if (!(ippacket.ICMP.Message is ICMPEchoReply))
			{
				goto IL_1EF;
			}
			ICMPEchoReply icmpechoReply = (ICMPEchoReply)ippacket.ICMP.Message;
			if (2 != 0)
			{
				if (ippacket.ICMP.Code != 0 || !(this.x1b07d570d2c02338.ToString() == this.EndPoint.ToString()))
				{
					goto IL_1EF;
				}
				if (3 == 0)
				{
					goto IL_228;
				}
				if (icmpechoReply.Identifier != ((ICMPEcho)ICMP.Message).Identifier || icmpechoReply.SequenceNumber != ((ICMPEcho)ICMP.Message).SequenceNumber)
				{
					goto IL_1EF;
				}
				if (!false)
				{
					goto IL_53;
				}
				goto IL_2CF;
			}
			IL_18C:
			return ippacket;
			IL_214:
			this.x70087761226fbdf2.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, TTL);
			goto IL_1C7;
			IL_228:
			if (this.x70087761226fbdf2 != null)
			{
				goto IL_214;
			}
			IL_254:
			this.x70087761226fbdf2 = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
			this.x70087761226fbdf2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.SocketTimeout);
			this.x70087761226fbdf2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.SocketTimeout);
			if (8 != 0)
			{
				if (false)
				{
					return ippacket;
				}
				goto IL_214;
			}
			IL_2CF:
			if ((uint)TTL + (uint)TTL > 4294967295U)
			{
				goto IL_1D2;
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Host);
			this.x1b07d570d2c02338 = new IPEndPoint(hostAddresses[0], 0);
			this.EndPoint = this.x1b07d570d2c02338;
			this.x9396aeed6f94cb4a = Host;
			flag = ((uint)TTL + (uint)TTL < 0U);
			if (flag)
			{
				goto IL_254;
			}
			goto IL_228;
		}

		public int SocketTimeout = 3000;

		private Socket x70087761226fbdf2;

		private IPEndPoint x1b07d570d2c02338;

		public EndPoint EndPoint;

		private string x9396aeed6f94cb4a;

		public int TimeElapsed;
	}
}
