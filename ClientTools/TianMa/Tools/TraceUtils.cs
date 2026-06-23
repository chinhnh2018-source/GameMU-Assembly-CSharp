using System;
using System.Net;
using System.Threading;

namespace TianMa.Tools
{
	public class TraceUtils
	{
		public static void TraceRoute(string host, ICMP_CALLBACK cb, ICMP_OVER_CALLBACK ocb, int maxTTL = 30, int count = 3, int interval = 200)
		{
			TraceUtils.xfd6b00b29231a30d parameter = new TraceUtils.xfd6b00b29231a30d(host, cb, ocb, maxTTL, count, interval);
			Thread thread = new Thread(new ParameterizedThreadStart(TraceUtils.x88a1be9885d20851));
			thread.Start(parameter);
		}

		private static void x88a1be9885d20851(object x92ef85a2aa705592)
		{
			if (x92ef85a2aa705592 == null)
			{
				goto IL_65B;
			}
			TraceUtils.xfd6b00b29231a30d xfd6b00b29231a30d = null;
			int xecea0e4ef40edfa;
			if (((uint)xecea0e4ef40edfa | 1U) == 0U)
			{
				goto IL_55E;
			}
			ushort num;
			bool flag = (uint)num < 0U;
			if (flag || x92ef85a2aa705592 is TraceUtils.xfd6b00b29231a30d)
			{
				xfd6b00b29231a30d = (x92ef85a2aa705592 as TraceUtils.xfd6b00b29231a30d);
			}
			if (xfd6b00b29231a30d == null)
			{
				return;
			}
			string x64f259306803411c = xfd6b00b29231a30d.x64f259306803411c;
			int num2;
			flag = ((uint)num + (uint)num2 < 0U);
			if (flag)
			{
				goto IL_65B;
			}
			int xbfb5b0208f9d4d6a = xfd6b00b29231a30d.xbfb5b0208f9d4d6a;
			goto IL_5B6;
			IL_0B:
			ICMP_OVER_CALLBACK xb501336c6fc7c;
			bool flag2;
			xb501336c6fc7c(flag2);
			flag = ((uint)num - (uint)xbfb5b0208f9d4d6a > uint.MaxValue);
			if (flag)
			{
				goto IL_458;
			}
			goto IL_4A3;
			IL_16:
			int num3;
			int x10f4d88af727adbc;
			if (flag2)
			{
				if ((uint)num3 + (uint)x10f4d88af727adbc > 4294967295U)
				{
					goto IL_0B;
				}
			}
			else if (num2 <= 20)
			{
				num3++;
				flag = ((uint)x10f4d88af727adbc - (uint)num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_4A3;
				}
				goto IL_4B;
			}
			IL_32:
			ICMP_CALLBACK xd2637df501625a;
			if (xd2637df501625a != null)
			{
				xd2637df501625a("Trace complete.\r\n");
			}
			if (xb501336c6fc7c != null)
			{
				goto IL_0B;
			}
			return;
			IL_4B:
			if (num3 >= xbfb5b0208f9d4d6a + 1)
			{
				goto IL_32;
			}
			goto IL_458;
			IL_8C:
			string text;
			string str;
			xd2637df501625a(text + str + "\r\n");
			goto IL_16;
			IL_A6:
			if (xd2637df501625a == null)
			{
				goto IL_483;
			}
			goto IL_8C;
			IL_3D2:
			ICMPEcho icmpecho = new ICMPEcho();
			IL_3D9:
			ICMPPacket icmppacket;
			icmppacket.Message = icmpecho;
			flag = ((uint)xbfb5b0208f9d4d6a - (uint)xecea0e4ef40edfa > uint.MaxValue);
			int num4;
			if (!flag && xd2637df501625a == null)
			{
				if ((uint)num3 - (uint)num4 < 0U)
				{
					goto IL_3D2;
				}
			}
			else
			{
				xd2637df501625a(num3.ToString().PadLeft(3) + " ");
			}
			num = 1;
			ICMPSocket icmpsocket;
			for (;;)
			{
				if ((int)num >= x10f4d88af727adbc + 1)
				{
					if ((uint)num2 + (uint)num < 0U)
					{
						goto IL_598;
					}
					flag = ((flag2 ? 1U : 0U) < 0U);
					if (!flag)
					{
						goto IL_A6;
					}
					flag = ((uint)num > uint.MaxValue);
					if (!flag)
					{
						break;
					}
				}
				else if (num > 0)
				{
					if (xecea0e4ef40edfa > 0)
					{
						flag = ((uint)x10f4d88af727adbc - (flag2 ? 1U : 0U) < 0U);
						if (!flag)
						{
							Thread.Sleep(xecea0e4ef40edfa);
						}
					}
				}
				icmpecho.Identifier = 1;
				flag = ((uint)xecea0e4ef40edfa - (uint)num2 < 0U);
				if (flag)
				{
					break;
				}
				icmpecho.SequenceNumber = num;
				icmpecho.Data = "".PadRight(num4, '*');
				try
				{
					IPPacket ippacket = icmpsocket.Send(x64f259306803411c, icmppacket, num3);
					if (text == "" || false)
					{
						goto IL_252;
					}
					IL_1ED:
					if (ippacket.ICMP.Message is ICMPTimeExceeded)
					{
						if (xd2637df501625a != null)
						{
							if ((uint)num4 >= 0U)
							{
								goto IL_1C5;
							}
							goto IL_13D;
						}
					}
					else
					{
						if (!(ippacket.ICMP.Message is ICMPEchoReply))
						{
							goto IL_10F;
						}
						if (xd2637df501625a != null)
						{
							xd2637df501625a(icmpsocket.TimeElapsed.ToString().PadLeft(5) + " msok ");
						}
						flag2 = true;
					}
					IL_E2:
					num2 = 0;
					flag = ((uint)num2 < 0U);
					if (!flag)
					{
						flag = ((uint)num4 + (uint)x10f4d88af727adbc < 0U);
						if (flag)
						{
							goto IL_252;
						}
						goto IL_2D1;
					}
					IL_10F:
					if (!(ippacket.ICMP.Message is ICMPIPHeaderReply))
					{
						goto IL_E2;
					}
					str = " reports " + ippacket.ICMP.Message;
					flag2 = true;
					IL_13D:
					if ((uint)num4 + (uint)num3 <= 4294967295U)
					{
						goto IL_A6;
					}
					IL_1C5:
					xd2637df501625a(icmpsocket.TimeElapsed.ToString().PadLeft(5) + " ms ");
					goto IL_E2;
					IL_252:
					string text2 = ((IPEndPoint)icmpsocket.EndPoint).Address.ToString();
					try
					{
						text = Dns.GetHostEntry(text2).HostName;
					}
					catch
					{
					}
					if (text != text2)
					{
						flag = ((uint)x10f4d88af727adbc < 0U);
						if (!flag)
						{
							if (((uint)x10f4d88af727adbc | 255U) == 0U)
							{
								goto IL_2D1;
							}
							text = text + " [" + text2 + "]";
						}
					}
					goto IL_1ED;
					IL_2D1:;
				}
				catch
				{
					if (xd2637df501625a != null)
					{
						xd2637df501625a("*".PadLeft(5) + "   ");
					}
					num2++;
				}
				num += 1;
			}
			flag = ((uint)num2 < 0U);
			if (!flag)
			{
				flag = (((uint)num2 | 2U) == 0U);
				if (flag)
				{
					goto IL_483;
				}
				goto IL_8C;
			}
			IL_458:
			text = "";
			str = "";
			if ((uint)xbfb5b0208f9d4d6a - (uint)num <= 4294967295U)
			{
				icmppacket = new ICMPPacket();
				goto IL_3D2;
			}
			return;
			IL_483:
			goto IL_16;
			IL_4A3:
			flag = ((uint)num3 + (uint)num > uint.MaxValue);
			if (flag)
			{
				goto IL_65B;
			}
			return;
			IL_55E:
			icmpsocket = new ICMPSocket();
			icmppacket = new ICMPPacket();
			icmpecho = new ICMPEcho();
			if ((uint)num - (uint)num2 > 4294967295U)
			{
				goto IL_A6;
			}
			if (((uint)num4 & 0U) != 0U)
			{
				goto IL_5B6;
			}
			goto IL_5CC;
			IL_598:
			xb501336c6fc7c = xfd6b00b29231a30d._xb501336c6fc7c870;
			num4 = 32;
			goto IL_55E;
			IL_5B6:
			x10f4d88af727adbc = xfd6b00b29231a30d.x10f4d88af727adbc;
			xecea0e4ef40edfa = xfd6b00b29231a30d.xecea0e4ef40edfa5;
			if (2 != 0)
			{
				xd2637df501625a = xfd6b00b29231a30d._xd2637df501625a82;
				goto IL_598;
			}
			IL_5CC:
			if (((uint)num & 0U) == 0U)
			{
				flag2 = false;
			}
			num2 = 0;
			if (xd2637df501625a != null)
			{
				if ((uint)num2 < 0U)
				{
					goto IL_3D9;
				}
				xd2637df501625a(string.Concat(new object[]
				{
					"Tracing route to ",
					x64f259306803411c,
					", maximum of ttl:",
					xbfb5b0208f9d4d6a,
					" \r\n"
				}));
				if (((uint)num4 & 0U) != 0U)
				{
					goto IL_458;
				}
			}
			num3 = 1;
			if (255 != 0)
			{
				goto IL_4B;
			}
			goto IL_3D9;
			IL_65B:
			if ((uint)xbfb5b0208f9d4d6a - (uint)num2 >= 0U)
			{
				return;
			}
			goto IL_A6;
		}

		private class xfd6b00b29231a30d
		{
			public xfd6b00b29231a30d(string host, ICMP_CALLBACK cb, ICMP_OVER_CALLBACK ocb, int maxTTL, int count, int interval)
			{
				this._x64f259306803411c = host;
				this._xbfb5b0208f9d4d6a = maxTTL;
				do
				{
					this._x10f4d88af727adbc = count;
					this._xecea0e4ef40edfa5 = interval;
					this._xd2637df501625a82 = cb;
					this._xb501336c6fc7c870 = ocb;
				}
				while (false);
			}

			public string x64f259306803411c
			{
				get
				{
					return this._x64f259306803411c;
				}
			}

			public int xbfb5b0208f9d4d6a
			{
				get
				{
					return this._xbfb5b0208f9d4d6a;
				}
			}

			public int x10f4d88af727adbc
			{
				get
				{
					return this._x10f4d88af727adbc;
				}
			}

			public int xecea0e4ef40edfa5
			{
				get
				{
					return this._xecea0e4ef40edfa5;
				}
			}

			private string _x64f259306803411c;

			private int _x10f4d88af727adbc = 3;

			private int _xecea0e4ef40edfa5 = 200;

			private int _xbfb5b0208f9d4d6a = 25;

			public ICMP_CALLBACK _xd2637df501625a82;

			public ICMP_OVER_CALLBACK _xb501336c6fc7c870;
		}
	}
}
