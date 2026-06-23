using System;
using System.Net;
using System.Net.Sockets;

namespace TianMa
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
				if (this.Socket != null)
				{
					this.Socket.Close();
				}
			}
			catch
			{
			}
			this.Socket = null;
		}

		public IPPacket Send(string Host, ICMPPacket ICMP, int TTL)
		{
			if (this.HostAddress != Host)
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(Host);
				this.IPEndPoint = new IPEndPoint(hostAddresses[0], 0);
				this.EndPoint = this.IPEndPoint;
				this.HostAddress = Host;
			}
			if (this.Socket == null)
			{
				this.Socket = new Socket(2, 3, 1);
				this.Socket.SetSocketOption(65535, 4101, this.SocketTimeout);
				this.Socket.SetSocketOption(65535, 4102, this.SocketTimeout);
			}
			this.Socket.SetSocketOption(0, 4, TTL);
			this.TimeElapsed = Environment.TickCount;
			this.Socket.SendTo(ICMP.GetBytes(), this.IPEndPoint);
			IPPacket ippacket;
			for (;;)
			{
				byte[] array = new byte[2048];
				this.Socket.ReceiveFrom(array, ref this.EndPoint);
				ippacket = new IPPacket(ref array);
				if (ippacket.ICMP.Message is ICMPIPHeaderReply)
				{
					IPPacket ip = ((ICMPIPHeaderReply)ippacket.ICMP.Message).IP;
					if (this.IPEndPoint.Address.ToString() == ip.DestinationAddress.ToString() && ip.ICMP.Message is ICMPEchoReply)
					{
						break;
					}
				}
				if (ippacket.ICMP.Message is ICMPEchoReply)
				{
					ICMPEchoReply icmpechoReply = (ICMPEchoReply)ippacket.ICMP.Message;
					if (ippacket.ICMP.Code == 0 && this.IPEndPoint.ToString() == this.EndPoint.ToString() && icmpechoReply.Identifier == ((ICMPEcho)ICMP.Message).Identifier && icmpechoReply.SequenceNumber == ((ICMPEcho)ICMP.Message).SequenceNumber)
					{
						break;
					}
				}
			}
			this.TimeElapsed = Environment.TickCount - this.TimeElapsed;
			return ippacket;
		}

		public int SocketTimeout = 3000;

		private Socket Socket;

		private IPEndPoint IPEndPoint;

		public EndPoint EndPoint;

		private string HostAddress;

		public int TimeElapsed;
	}
}
