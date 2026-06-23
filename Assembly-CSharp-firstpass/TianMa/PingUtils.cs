using System;
using System.Threading;

namespace TianMa
{
	internal class PingUtils
	{
		public static void Ping(string host, ICMP_CALLBACK cb = null, ICMP_OVER_CALLBACK ocb = null, int count = 4, int interval = 400)
		{
			PingUtils.PingThreadData pingThreadData = new PingUtils.PingThreadData(host, cb, ocb, count, interval);
			Thread thread = new Thread(new ParameterizedThreadStart(PingUtils.PingFunc));
			thread.Start(pingThreadData);
		}

		private static void PingFunc(object oa)
		{
			if (oa == null)
			{
				return;
			}
			PingUtils.PingThreadData pingThreadData = null;
			if (oa is PingUtils.PingThreadData)
			{
				pingThreadData = (oa as PingUtils.PingThreadData);
			}
			if (pingThreadData == null)
			{
				return;
			}
			string host = pingThreadData.host;
			int count = pingThreadData.count;
			int interval = pingThreadData.interval;
			ICMP_CALLBACK cb = pingThreadData._cb;
			ICMP_OVER_CALLBACK overCB = pingThreadData._overCB;
			int num = 32;
			ICMPSocket icmpsocket = new ICMPSocket();
			if (cb != null)
			{
				cb(string.Concat(new object[]
				{
					"ping ",
					host,
					" with ",
					num,
					" bytes of data:\r\n"
				}));
			}
			Random random = new Random();
			ICMPPacket icmppacket = new ICMPPacket();
			ICMPEcho icmpecho = new ICMPEcho();
			bool ok = false;
			icmppacket.Message = icmpecho;
			ushort num2 = 1;
			while ((int)num2 < count + 1)
			{
				if (num2 > 0 && interval > 0)
				{
					Thread.Sleep(interval);
				}
				icmpecho.Identifier = (ushort)random.Next(0, 65535);
				icmpecho.SequenceNumber = num2;
				icmpecho.Data = string.Empty.PadRight(num, '*');
				try
				{
					IPPacket ippacket = icmpsocket.Send(host, icmppacket, 128);
					if (ippacket.ICMP.Message is ICMPEchoReply)
					{
						string text = string.Concat(new object[]
						{
							"Reply from ",
							icmpsocket.EndPoint.ToString(),
							": bytes=",
							((ICMPEchoReply)ippacket.ICMP.Message).Data.Length,
							" time=",
							icmpsocket.TimeElapsed,
							"ms TTL=",
							ippacket.TimeToLive
						});
						Console.WriteLine(text);
						if (cb != null)
						{
							cb(text + "\r\n");
						}
						ok = true;
					}
					else if (ippacket.ICMP.Message is ICMPTimeExceeded)
					{
						string content = "Reply from " + icmpsocket.EndPoint.ToString() + ": Time Exceeded\r\n";
						if (cb != null)
						{
							cb(content);
						}
					}
					else if (ippacket.ICMP.Message is ICMPDestinationUnreachable)
					{
						string content2 = "Reply from " + icmpsocket.EndPoint.ToString() + ": Destination Unreachable\r\n";
						if (cb != null)
						{
							cb(content2);
						}
					}
					else
					{
						string text2 = string.Concat(new object[]
						{
							"Reply from ",
							icmpsocket.EndPoint.ToString(),
							": ",
							ippacket.ICMP.Message
						});
						if (cb != null)
						{
							cb(text2 + "\r\n");
						}
					}
				}
				catch
				{
					if (cb != null)
					{
						cb("Request timed out\r\n");
					}
				}
				num2 += 1;
			}
			if (overCB != null)
			{
				overCB(ok);
			}
		}

		private class PingThreadData
		{
			public PingThreadData(string host, ICMP_CALLBACK cb, ICMP_OVER_CALLBACK ocb, int count, int interval)
			{
				this._host = host;
				this._count = count;
				this._interval = interval;
				this._cb = cb;
				this._overCB = ocb;
			}

			public string host
			{
				get
				{
					return this._host;
				}
			}

			public int count
			{
				get
				{
					return this._count;
				}
			}

			public int interval
			{
				get
				{
					return this._interval;
				}
			}

			public ICMP_CALLBACK _cb;

			public ICMP_OVER_CALLBACK _overCB;

			private string _host;

			private int _count = 5;

			private int _interval = 400;
		}
	}
}
