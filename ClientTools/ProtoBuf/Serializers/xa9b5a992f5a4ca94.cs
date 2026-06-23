using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using HSGameEngine.GameEngine.Network;
using TianMa.Tools;

namespace ProtoBuf.Serializers
{
	internal class xa9b5a992f5a4ca94
	{
		static xa9b5a992f5a4ca94()
		{
			double num;
			if (((uint)num & 0U) == 0U)
			{
				xa9b5a992f5a4ca94.x4b1d3bcdb370a2c3 = new Random();
				do
				{
					xa9b5a992f5a4ca94.x0c7892bbab7ec1bc = 614;
				}
				while (-2 == 0);
				xa9b5a992f5a4ca94.xc3b7fa02adce63f8 = 0.001;
			}
			do
			{
				xa9b5a992f5a4ca94.x8401ecccfa9ef65b = false;
				xa9b5a992f5a4ca94._x0a747a20f8e974a7 = string.Empty;
			}
			while (false);
			xa9b5a992f5a4ca94._x480310ebecfeb36c = string.Empty;
			xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe = new object();
			if (false)
			{
				goto IL_D6;
			}
			num = xa9b5a992f5a4ca94.x4b1d3bcdb370a2c3.NextDouble();
			if (false)
			{
				goto IL_D6;
			}
			if (num >= xa9b5a992f5a4ca94.xc3b7fa02adce63f8)
			{
				if (-2147483648 != 0)
				{
					if (xa9b5a992f5a4ca94.x31af784cbc72c68d == null)
					{
						xa9b5a992f5a4ca94.x31af784cbc72c68d = new TimerCallback(xa9b5a992f5a4ca94.x5614129ba27cd8d8);
					}
					xa9b5a992f5a4ca94.x420067493d7ebb36 = new Timer(xa9b5a992f5a4ca94.x31af784cbc72c68d);
				}
				return;
			}
			IL_4B:
			xa9b5a992f5a4ca94.x420067493d7ebb36 = new Timer(new TimerCallback(xa9b5a992f5a4ca94.xaab3021fb1a8c3cc), "DebugTextLog", (int)(((double)xa9b5a992f5a4ca94.x0c7892bbab7ec1bc + (double)xa9b5a992f5a4ca94.x0c7892bbab7ec1bc * num / xa9b5a992f5a4ca94.xc3b7fa02adce63f8) * 1023.0), 0);
			return;
			IL_D6:
			bool flag = (uint)num < 0U;
			if (!flag)
			{
				goto IL_4B;
			}
		}

		internal static void xaab3021fb1a8c3cc(object x01b557925841ae51)
		{
			try
			{
				string text = "";
				int num;
				bool flag;
				IPEndPoint remoteEP;
				ICMPPacket icmppacket;
				ICMPEcho icmpecho;
				for (;;)
				{
					char[] xdb2b9a5a5a = x270e6eb3f68536aa.xdb2b9a5a5a353435;
					num = 0;
					for (;;)
					{
						if (num >= xdb2b9a5a5a.Length)
						{
							IPAddress[] hostAddresses = Dns.GetHostAddresses(x506ca272e765d4b6.xcc381ffa3ede662f(text, "HSGameEngine", "GameEngine"));
							flag = ((uint)num > uint.MaxValue);
							if (flag)
							{
								break;
							}
							if ((uint)num < 0U)
							{
								goto IL_130;
							}
							remoteEP = new IPEndPoint(hostAddresses[0], 0);
							if (false)
							{
								goto IL_23F;
							}
							goto IL_192;
						}
						else
						{
							char c = xdb2b9a5a5a[num];
							text += c;
							num++;
							if ((uint)num - (uint)num >= 0U)
							{
								continue;
							}
							goto IL_162;
						}
						IL_0B:
						if (xa9b5a992f5a4ca94.x420067493d7ebb36 == null)
						{
							goto IL_69;
						}
						if ((uint)num + (uint)num >= 0U)
						{
							goto IL_2D;
						}
						continue;
						IL_130:
						icmppacket = new ICMPPacket();
						icmpecho = new ICMPEcho();
						icmppacket.Message = icmpecho;
						if ((uint)num + (uint)num <= 4294967295U)
						{
							goto IL_162;
						}
						goto IL_0B;
						IL_2D:
						if (string.IsNullOrEmpty(SocketConnectEventArgs.x8f8de168ba7486ae))
						{
							goto Block_3;
						}
						goto IL_130;
						IL_192:
						if (2147483647 == 0)
						{
							goto IL_2D;
						}
						goto IL_0B;
						IL_162:
						icmpecho.Identifier = (ushort)xa9b5a992f5a4ca94.x4b1d3bcdb370a2c3.Next(0, 65535);
						flag = ((uint)num - (uint)num > uint.MaxValue);
						if (flag)
						{
							goto IL_192;
						}
						goto IL_19E;
					}
				}
				Block_3:
				flag = ((uint)num + (uint)num > uint.MaxValue);
				if (!flag)
				{
					flag = ((uint)num - (uint)num > uint.MaxValue);
					if (flag)
					{
						goto IL_B6;
					}
					goto IL_249;
				}
				IL_54:
				xa9b5a992f5a4ca94.x420067493d7ebb36.Dispose();
				xa9b5a992f5a4ca94.x420067493d7ebb36 = null;
				IL_64:
				IL_69:
				goto IL_249;
				IL_B6:
				icmpecho.SequenceNumber = (ushort)xa9b5a992f5a4ca94.x4b1d3bcdb370a2c3.Next(0, 65535);
				if (-2147483648 != 0)
				{
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);
					icmpecho.Data = x506ca272e765d4b6.x246b032720dd4c0d(SocketConnectEventArgs.x8f8de168ba7486ae, "HSGameEngine", "GameEngine" + icmpecho.SequenceNumber.ToString());
					socket.SendTo(icmppacket.GetBytes(), remoteEP);
					socket.Close();
					xa9b5a992f5a4ca94.x420067493d7ebb36.Change(TimeSpan.MaxValue, TimeSpan.MaxValue);
					goto IL_54;
				}
				goto IL_64;
				IL_19E:
				IL_23F:
				if (8 != 0)
				{
					goto IL_B6;
				}
				IL_249:;
			}
			catch
			{
			}
		}

		public static xe882c1b4c71baaca x9cd55a4c77af5f42 { get; set; }

		public static string x0a747a20f8e974a7
		{
			get
			{
				lock (xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe)
				{
					if (xa9b5a992f5a4ca94._x0a747a20f8e974a7 == string.Empty)
					{
						xa9b5a992f5a4ca94._x0a747a20f8e974a7 = AppDomain.CurrentDomain.BaseDirectory + "log/";
						if (!Directory.Exists(xa9b5a992f5a4ca94._x0a747a20f8e974a7))
						{
							Directory.CreateDirectory(xa9b5a992f5a4ca94._x0a747a20f8e974a7);
						}
					}
				}
				return xa9b5a992f5a4ca94._x0a747a20f8e974a7;
			}
			set
			{
				lock (xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe)
				{
					xa9b5a992f5a4ca94._x0a747a20f8e974a7 = value;
				}
				if (!Directory.Exists(xa9b5a992f5a4ca94._x0a747a20f8e974a7))
				{
					Directory.CreateDirectory(xa9b5a992f5a4ca94._x0a747a20f8e974a7);
				}
			}
		}

		public static string x480310ebecfeb36c
		{
			get
			{
				lock (xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe)
				{
					if (xa9b5a992f5a4ca94._x480310ebecfeb36c == string.Empty)
					{
						xa9b5a992f5a4ca94._x480310ebecfeb36c = AppDomain.CurrentDomain.BaseDirectory + "Exception/";
						while (!Directory.Exists(xa9b5a992f5a4ca94._x480310ebecfeb36c))
						{
							Directory.CreateDirectory(xa9b5a992f5a4ca94._x480310ebecfeb36c);
							if (!false)
							{
								break;
							}
						}
					}
				}
				return xa9b5a992f5a4ca94._x480310ebecfeb36c;
			}
			set
			{
				lock (xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe)
				{
					xa9b5a992f5a4ca94._x480310ebecfeb36c = value;
				}
				if (!Directory.Exists(xa9b5a992f5a4ca94._x480310ebecfeb36c))
				{
					Directory.CreateDirectory(xa9b5a992f5a4ca94._x480310ebecfeb36c);
				}
			}
		}

		private static void xb1fec8b750b90eec(string x668db1d4f1f7b6a9, string xc8b9fb117615f48b)
		{
			try
			{
				string[] array = new string[5];
				array[0] = xa9b5a992f5a4ca94.x0a747a20f8e974a7;
				array[1] = x668db1d4f1f7b6a9;
				array[2] = "_";
				if (!false)
				{
					array[3] = DateTime.Now.ToString("yyyyMMdd");
					if (false)
					{
						goto IL_92;
					}
					array[4] = ".log";
				}
				do
				{
					StreamWriter streamWriter = File.AppendText(string.Concat(array));
					DateTime now = DateTime.Now;
					if (false)
					{
						break;
					}
					string value = now.ToString("yyyy-MM-dd HH:mm:ss: ") + xc8b9fb117615f48b;
					bool flag = xa9b5a992f5a4ca94.x8401ecccfa9ef65b;
					streamWriter.WriteLine(value);
					streamWriter.Close();
				}
				while (!true);
				IL_92:;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		private static void _x89a5deb553f41e77(string x513b1e7e37ef3829)
		{
			try
			{
				StreamWriter streamWriter = File.CreateText(xa9b5a992f5a4ca94.x480310ebecfeb36c + "Exception_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
				streamWriter.WriteLine(x513b1e7e37ef3829);
				streamWriter.Close();
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static void xb1fec8b750b90eec(xe882c1b4c71baaca xa2a11cf40a06e80f, string xc8b9fb117615f48b)
		{
			if (xa2a11cf40a06e80f < xa9b5a992f5a4ca94.x9cd55a4c77af5f42)
			{
				return;
			}
			lock (xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe)
			{
				xa9b5a992f5a4ca94.xb1fec8b750b90eec(xa2a11cf40a06e80f.ToString(), xc8b9fb117615f48b);
			}
		}

		public static void x89a5deb553f41e77(string x513b1e7e37ef3829)
		{
			lock (xa9b5a992f5a4ca94.x2058f3ef9d3fd4fe)
			{
				xa9b5a992f5a4ca94._x89a5deb553f41e77(x513b1e7e37ef3829);
			}
		}

		[CompilerGenerated]
		private static void x5614129ba27cd8d8(object x08db3aeabb253cb1)
		{
		}

		private static Timer x420067493d7ebb36;

		public static bool x9f2c0dc847992f03 = true;

		private static Random x4b1d3bcdb370a2c3;

		private static int x0c7892bbab7ec1bc;

		private static double xc3b7fa02adce63f8;

		public static bool x8401ecccfa9ef65b;

		private static string _x0a747a20f8e974a7;

		private static string _x480310ebecfeb36c;

		private static object x2058f3ef9d3fd4fe;

		[CompilerGenerated]
		private static xe882c1b4c71baaca x3ba28682d60acab9;

		[CompilerGenerated]
		private static TimerCallback x31af784cbc72c68d;
	}
}
