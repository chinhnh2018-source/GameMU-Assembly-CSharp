using System;
using System.Threading;

namespace TianMa.Tools
{
	internal class x60c233b7055c6c1c
	{
		public static void x7872652352d9d52c(string x64f259306803411c, ICMP_CALLBACK xd2637df501625a82 = null, ICMP_OVER_CALLBACK xb501336c6fc7c870 = null, int x10f4d88af727adbc = 4, int xecea0e4ef40edfa5 = 400)
		{
			x60c233b7055c6c1c.x3f0eed751b012ca1 parameter = new x60c233b7055c6c1c.x3f0eed751b012ca1(x64f259306803411c, xd2637df501625a82, xb501336c6fc7c870, x10f4d88af727adbc, xecea0e4ef40edfa5);
			Thread thread = new Thread(new ParameterizedThreadStart(x60c233b7055c6c1c.x4f6a9a4e1eaa3d50));
			thread.Start(parameter);
		}

		private static void x4f6a9a4e1eaa3d50(object x92ef85a2aa705592)
		{
			bool flag;
			int num;
			string x64f259306803411c;
			int x10f4d88af727adbc;
			int xecea0e4ef40edfa;
			ICMP_CALLBACK xd2637df501625a;
			ICMP_OVER_CALLBACK x235b5094753c6b;
			if (x92ef85a2aa705592 == null)
			{
				if (((flag ? 1U : 0U) & 0U) != 0U)
				{
					goto IL_49F;
				}
				return;
			}
			else
			{
				x60c233b7055c6c1c.x3f0eed751b012ca1 x3f0eed751b012ca = null;
				while (x92ef85a2aa705592 is x60c233b7055c6c1c.x3f0eed751b012ca1)
				{
					bool flag2 = ((flag ? 1U : 0U) & 0U) == 0U;
					if (flag2)
					{
						if ((uint)num + (uint)num <= 4294967295U)
						{
							x3f0eed751b012ca = (x92ef85a2aa705592 as x60c233b7055c6c1c.x3f0eed751b012ca1);
							break;
						}
						goto IL_4B7;
					}
				}
				if (x3f0eed751b012ca == null)
				{
					return;
				}
				x64f259306803411c = x3f0eed751b012ca.x64f259306803411c;
				x10f4d88af727adbc = x3f0eed751b012ca.x10f4d88af727adbc;
				xecea0e4ef40edfa = x3f0eed751b012ca.xecea0e4ef40edfa5;
				xd2637df501625a = x3f0eed751b012ca._xd2637df501625a82;
				x235b5094753c6b = x3f0eed751b012ca._x235b5094753c6b72;
				num = 32;
				goto IL_49F;
			}
			ushort num2;
			ICMPEcho icmpecho;
			ICMPPacket icmppacket;
			for (;;)
			{
				IL_479:
				num2 = 1;
				for (;;)
				{
					bool flag2;
					if ((int)num2 >= x10f4d88af727adbc + 1)
					{
						if (x235b5094753c6b == null)
						{
							return;
						}
						x235b5094753c6b(flag);
						flag2 = ((flag ? 1U : 0U) + (uint)num < 0U);
						if (!flag2)
						{
							return;
						}
						if (((uint)num2 & 0U) != 0U)
						{
							goto IL_6E;
						}
						goto IL_49F;
					}
					else
					{
						if (num2 > 0 && xecea0e4ef40edfa > 0)
						{
							if (((uint)xecea0e4ef40edfa | 4294967295U) == 0U)
							{
								break;
							}
							Thread.Sleep(xecea0e4ef40edfa);
						}
						Random random;
						icmpecho.Identifier = (ushort)random.Next(0, 65535);
						flag2 = ((uint)num2 - (uint)x10f4d88af727adbc > uint.MaxValue);
						if (!flag2)
						{
							goto IL_6E;
						}
					}
					IL_523:
					flag2 = ((uint)x10f4d88af727adbc > uint.MaxValue);
					if (flag2)
					{
						return;
					}
					continue;
					IL_6E:
					icmpecho.SequenceNumber = num2;
					icmpecho.Data = "".PadRight(num, '*');
					try
					{
						ICMPSocket icmpsocket;
						IPPacket ippacket = icmpsocket.Send(x64f259306803411c, icmppacket, 128);
						flag2 = ((uint)x10f4d88af727adbc - (flag ? 1U : 0U) > uint.MaxValue);
						object[] array;
						string content;
						object[] array2;
						string content2;
						if (flag2)
						{
							flag2 = ((uint)num2 + (uint)x10f4d88af727adbc > uint.MaxValue);
							if (flag2)
							{
								goto IL_1EA;
							}
							goto IL_2DB;
						}
						else
						{
							while (ippacket.ICMP.Message is ICMPEchoReply)
							{
								array = new object[8];
								array[0] = "Reply from ";
								if ((flag ? 1U : 0U) > 4294967295U)
								{
									goto IL_177;
								}
								array[1] = icmpsocket.EndPoint.ToString();
								array[2] = ": bytes=";
								if ((uint)x10f4d88af727adbc + (uint)x10f4d88af727adbc > 4294967295U)
								{
									goto IL_F2;
								}
								array[3] = ((ICMPEchoReply)ippacket.ICMP.Message).Data.Length;
								array[4] = " time=";
								flag2 = (((uint)x10f4d88af727adbc | 255U) == 0U);
								if (!flag2)
								{
									goto IL_2DB;
								}
							}
							if (ippacket.ICMP.Message is ICMPTimeExceeded)
							{
								content = "Reply from " + icmpsocket.EndPoint.ToString() + ": Time Exceeded\r\n";
								goto IL_F4;
							}
							if (!(ippacket.ICMP.Message is ICMPDestinationUnreachable))
							{
								array2 = new object[4];
								goto IL_177;
							}
							content2 = "Reply from " + icmpsocket.EndPoint.ToString() + ": Destination Unreachable\r\n";
							if (xd2637df501625a == null)
							{
								goto IL_3EF;
							}
							if ((uint)x10f4d88af727adbc - (uint)num2 >= 0U)
							{
								goto IL_1BF;
							}
						}
						IL_F2:
						if (((uint)num | 4U) != 0U)
						{
							flag2 = ((uint)num2 < 0U);
							if (flag2)
							{
								if (((uint)num & 0U) != 0U)
								{
									goto IL_161;
								}
								goto IL_1EA;
							}
							else
							{
								if (((flag ? 1U : 0U) & 0U) != 0U)
								{
									goto IL_2DB;
								}
								goto IL_3EF;
							}
						}
						IL_F4:
						if (xd2637df501625a == null)
						{
							goto IL_3EF;
						}
						xd2637df501625a(content);
						goto IL_1EA;
						IL_161:
						array2[2] = ": ";
						array2[3] = ippacket.ICMP.Message;
						string str = string.Concat(array2);
						if (xd2637df501625a != null)
						{
							xd2637df501625a(str + "\r\n");
							goto IL_F2;
						}
						goto IL_3EF;
						IL_177:
						array2[0] = "Reply from ";
						array2[1] = icmpsocket.EndPoint.ToString();
						flag2 = ((uint)num - (flag ? 1U : 0U) < 0U);
						if (!flag2)
						{
							goto IL_161;
						}
						if (((uint)num & 0U) == 0U)
						{
							goto IL_1EA;
						}
						IL_1BF:
						xd2637df501625a(content2);
						IL_1EA:
						goto IL_3EF;
						IL_2DB:
						if (false)
						{
							goto IL_177;
						}
						if ((uint)num >= 0U)
						{
							if ((uint)num <= 4294967295U)
							{
								array[5] = icmpsocket.TimeElapsed;
								array[6] = "ms TTL=";
								array[7] = ippacket.TimeToLive;
								string text = string.Concat(array);
								Console.WriteLine(text);
								if (xd2637df501625a != null)
								{
									xd2637df501625a(text + "\r\n");
								}
								flag = true;
							}
						}
						IL_3EF:;
					}
					catch
					{
						if (xd2637df501625a != null)
						{
							xd2637df501625a("Request timed out\r\n");
						}
					}
					num2 += 1;
					goto IL_523;
				}
			}
			return;
			do
			{
				IL_4B7:
				ICMPSocket icmpsocket = new ICMPSocket();
				if (xd2637df501625a != null)
				{
					xd2637df501625a(string.Concat(new object[]
					{
						"ping ",
						x64f259306803411c,
						" with ",
						num,
						" bytes of data:\r\n"
					}));
				}
				Random random = new Random();
				icmppacket = new ICMPPacket();
				icmpecho = new ICMPEcho();
				flag = false;
			}
			while ((flag ? 1U : 0U) + (uint)x10f4d88af727adbc > 4294967295U);
			icmppacket.Message = icmpecho;
			goto IL_479;
			IL_49F:
			if (((uint)num2 | 3U) != 0U)
			{
				goto IL_4B7;
			}
			goto IL_479;
		}

		private class x3f0eed751b012ca1
		{
			public x3f0eed751b012ca1(string host, ICMP_CALLBACK cb, ICMP_OVER_CALLBACK ocb, int count, int interval)
			{
				this._x64f259306803411c = host;
				if (!false)
				{
				}
				this._x10f4d88af727adbc = count;
				this._xecea0e4ef40edfa5 = interval;
				this._xd2637df501625a82 = cb;
				this._x235b5094753c6b72 = ocb;
			}

			public string x64f259306803411c
			{
				get
				{
					return this._x64f259306803411c;
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

			public ICMP_CALLBACK _xd2637df501625a82;

			public ICMP_OVER_CALLBACK _x235b5094753c6b72;

			private string _x64f259306803411c;

			private int _x10f4d88af727adbc = 5;

			private int _xecea0e4ef40edfa5 = 400;
		}
	}
}
