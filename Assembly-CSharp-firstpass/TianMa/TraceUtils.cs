using System;
using System.Net;
using System.Threading;

namespace TianMa
{
	public class TraceUtils
	{
		public static void TraceRoute(string host, ICMP_CALLBACK cb, ICMP_OVER_CALLBACK ocb, int maxTTL = 30, int count = 3, int interval = 200)
		{
			TraceUtils.TraceThreadData traceThreadData = new TraceUtils.TraceThreadData(host, cb, ocb, maxTTL, count, interval);
			Thread thread = new Thread(new ParameterizedThreadStart(TraceUtils.TraceRouteFunc));
			thread.Start(traceThreadData);
		}

		private static void TraceRouteFunc(object oa)
		{
			if (oa == null)
			{
				return;
			}
			TraceUtils.TraceThreadData traceThreadData = null;
			if (oa is TraceUtils.TraceThreadData)
			{
				traceThreadData = (oa as TraceUtils.TraceThreadData);
			}
			if (traceThreadData == null)
			{
				return;
			}
			string host = traceThreadData.host;
			int maxTTL = traceThreadData.maxTTL;
			int count = traceThreadData.count;
			int interval = traceThreadData.interval;
			ICMP_CALLBACK cb = traceThreadData._cb;
			ICMP_OVER_CALLBACK ocb = traceThreadData._ocb;
			int num = 32;
			ICMPSocket icmpsocket = new ICMPSocket();
			ICMPPacket icmppacket = new ICMPPacket();
			ICMPEcho icmpecho = new ICMPEcho();
			bool flag = false;
			int num2 = 0;
			if (cb != null)
			{
				cb(string.Concat(new object[]
				{
					"Tracing route to ",
					host,
					", maximum of ttl:",
					maxTTL,
					" \r\n"
				}));
			}
			for (int i = 1; i < maxTTL + 1; i++)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				icmppacket = new ICMPPacket();
				icmpecho = new ICMPEcho();
				icmppacket.Message = icmpecho;
				if (cb != null)
				{
					cb(i.ToString().PadLeft(3) + " ");
				}
				ushort num3 = 1;
				while ((int)num3 < count + 1)
				{
					if (num3 > 0 && interval > 0)
					{
						Thread.Sleep(interval);
					}
					icmpecho.Identifier = 1;
					icmpecho.SequenceNumber = num3;
					icmpecho.Data = string.Empty.PadRight(num, '*');
					try
					{
						IPPacket ippacket = icmpsocket.Send(host, icmppacket, i);
						if (text == string.Empty)
						{
							string text3 = ((IPEndPoint)icmpsocket.EndPoint).Address.ToString();
							try
							{
								text = Dns.GetHostEntry(text3).HostName;
							}
							catch
							{
							}
							if (text != text3)
							{
								text = text + " [" + text3 + "]";
							}
						}
						if (ippacket.ICMP.Message is ICMPTimeExceeded)
						{
							if (cb != null)
							{
								cb(icmpsocket.TimeElapsed.ToString().PadLeft(5) + " ms ");
							}
						}
						else if (ippacket.ICMP.Message is ICMPEchoReply)
						{
							if (cb != null)
							{
								cb(icmpsocket.TimeElapsed.ToString().PadLeft(5) + " msok ");
							}
							flag = true;
						}
						else if (ippacket.ICMP.Message is ICMPIPHeaderReply)
						{
							text2 = " reports " + ippacket.ICMP.Message;
							flag = true;
							break;
						}
						num2 = 0;
					}
					catch
					{
						if (cb != null)
						{
							cb("*".PadLeft(5) + "   ");
						}
						num2++;
					}
					num3 += 1;
				}
				if (cb != null)
				{
					cb(text + text2 + "\r\n");
				}
				if (flag)
				{
					break;
				}
				if (num2 > 20)
				{
					break;
				}
			}
			if (cb != null)
			{
				cb("Trace complete.\r\n");
			}
			if (ocb != null)
			{
				ocb(flag);
			}
		}

		private class TraceThreadData
		{
			public TraceThreadData(string host, ICMP_CALLBACK cb, ICMP_OVER_CALLBACK ocb, int maxTTL, int count, int interval)
			{
				this._host = host;
				this._maxTTL = maxTTL;
				this._count = count;
				this._interval = interval;
				this._cb = cb;
				this._ocb = ocb;
			}

			public string host
			{
				get
				{
					return this._host;
				}
			}

			public int maxTTL
			{
				get
				{
					return this._maxTTL;
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

			private string _host;

			private int _count = 3;

			private int _interval = 200;

			private int _maxTTL = 25;

			public ICMP_CALLBACK _cb;

			public ICMP_OVER_CALLBACK _ocb;
		}
	}
}
