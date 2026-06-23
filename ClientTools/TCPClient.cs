using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Protocol;
using Server.Tools;

namespace ClientTools
{
	public class TCPClient
	{
		public TCPClient(int capacity = 2)
		{
			this._x70087761226fbdf2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._xa328d8c31e63da26 = new TCPInPacket(131072);
			this._xa328d8c31e63da26.TCPCmdPacketEvent += this.xd0ab16336c9c9beb;
			this.x6528c835948babcb = new TCPOutPacketPool(capacity * 5);
		}

		public void Destroy()
		{
			object[] array = new object[1];
			if (255 != 0)
			{
				array[0] = "TCPClient Destroy";
			}
			DebugTextLog.Log(array);
			this.x60245825be2151f2();
			this._xa328d8c31e63da26.TCPCmdPacketEvent -= this.xd0ab16336c9c9beb;
			this._x70087761226fbdf2 = null;
		}

		public TCPOutPacketPool OutPacketPool
		{
			get
			{
				return this.x6528c835948babcb;
			}
		}

		public bool Connected
		{
			get
			{
				return this._xe3117ff5cca4d9ac;
			}
			set
			{
				this._xe3117ff5cca4d9ac = value;
			}
		}

		public string RemoteIP
		{
			get
			{
				return this._xa6532f30f3a61c54;
			}
			set
			{
				this._xa6532f30f3a61c54 = value;
			}
		}

		public int RemotePort
		{
			get
			{
				return this._x6689a717755b59cb;
			}
			set
			{
				this._x6689a717755b59cb = value;
			}
		}

		public Socket CurrentSocket
		{
			get
			{
				return this._x70087761226fbdf2;
			}
		}

		public TCPInPacket MyTCPInPacket
		{
			get
			{
				return this._xa328d8c31e63da26;
			}
		}

		public long LastSendDataTicks
		{
			get
			{
				return this._xaf6cc4b253f9355c;
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				return this._xbd3cf1f5f80e7d88;
			}
			set
			{
				this._xbd3cf1f5f80e7d88 = value;
			}
		}

		public int SendTimeout
		{
			get
			{
				return this._xd770807629889cd7;
			}
			set
			{
				this._xd770807629889cd7 = value;
			}
		}

		public event SocketConnectEventHandler SocketConnect
		{
			add
			{
				SocketConnectEventHandler socketConnectEventHandler = this.x5484bff21ae952c5;
				SocketConnectEventHandler socketConnectEventHandler2;
				do
				{
					socketConnectEventHandler2 = socketConnectEventHandler;
					SocketConnectEventHandler value2 = (SocketConnectEventHandler)Delegate.Combine(socketConnectEventHandler2, value);
					socketConnectEventHandler = Interlocked.CompareExchange<SocketConnectEventHandler>(ref this.x5484bff21ae952c5, value2, socketConnectEventHandler2);
				}
				while (socketConnectEventHandler != socketConnectEventHandler2);
			}
			remove
			{
				SocketConnectEventHandler socketConnectEventHandler = this.x5484bff21ae952c5;
				SocketConnectEventHandler socketConnectEventHandler2;
				do
				{
					socketConnectEventHandler2 = socketConnectEventHandler;
					SocketConnectEventHandler value2 = (SocketConnectEventHandler)Delegate.Remove(socketConnectEventHandler2, value);
					socketConnectEventHandler = Interlocked.CompareExchange<SocketConnectEventHandler>(ref this.x5484bff21ae952c5, value2, socketConnectEventHandler2);
				}
				while (socketConnectEventHandler != socketConnectEventHandler2);
			}
		}

		public bool Connect(string ip, int port)
		{
			if (!this._xe3117ff5cca4d9ac)
			{
				if ((uint)port <= 4294967295U)
				{
				}
				if (this._x70087761226fbdf2 != null)
				{
					this._xa6532f30f3a61c54 = ip;
					this._x6689a717755b59cb = port;
					try
					{
						this.x4bef6898ca94460a = new System.Timers.Timer(15000.0);
						this.x4bef6898ca94460a.Elapsed += this.xc45b8a701203af72;
						this.x4bef6898ca94460a.Interval = 15000.0;
						bool flag;
						while (!false)
						{
							IPEndPoint remoteEP;
							if (!false)
							{
								this.x4bef6898ca94460a.Enabled = true;
								IPAddress[] hostAddresses = Dns.GetHostAddresses(this._xa6532f30f3a61c54);
								IPAddress address = hostAddresses[0];
								remoteEP = new IPEndPoint(address, this._x6689a717755b59cb);
								this._x70087761226fbdf2.ReceiveTimeout = this._xbd3cf1f5f80e7d88;
								if (false)
								{
									continue;
								}
								goto IL_BA;
							}
							IL_74:
							object[] array;
							array[0] = "TCPClient Connect:";
							DebugTextLog.Log(array);
							flag = true;
							bool flag2 = (uint)port - (uint)port > uint.MaxValue;
							if (!flag2)
							{
								break;
							}
							flag2 = ((flag ? 1U : 0U) - (flag ? 1U : 0U) > uint.MaxValue);
							if (!flag2)
							{
								continue;
							}
							IL_BA:
							this._x70087761226fbdf2.SendTimeout = this._xd770807629889cd7;
							this.x20675a21460278a8 = false;
							this._x70087761226fbdf2.BeginConnect(remoteEP, new AsyncCallback(this.x78f245401316535b), null);
							array = new object[1];
							goto IL_74;
						}
						return flag;
					}
					catch (Exception exception)
					{
						DebugTextLog.LogException(exception);
					}
					return false;
				}
			}
			return false;
		}

		private void x60245825be2151f2()
		{
			if (this.x4bef6898ca94460a != null)
			{
				this.x4bef6898ca94460a.Stop();
				this.x4bef6898ca94460a.Enabled = false;
				this.x4bef6898ca94460a = null;
			}
		}

		private void xc45b8a701203af72(object x337e217cb3ba0627, ElapsedEventArgs xfbf34718e704c6bc)
		{
			this.x60245825be2151f2();
			if (15 != 0)
			{
				if (this.x20675a21460278a8)
				{
					return;
				}
			}
			if (this.x5484bff21ae952c5 != null)
			{
				this.x5484bff21ae952c5(this, new SocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Connect Fail",
					NetSocketType = 0
				});
			}
			this.Disconnect(SocketShutdown.Both);
		}

		public bool SendData(TCPOutPacket tcpOutPacket)
		{
			bool flag;
			if (this._x70087761226fbdf2 != null)
			{
				while (this._x70087761226fbdf2.Connected)
				{
					if ((flag ? 1U : 0U) <= 4294967295U)
					{
						if (false)
						{
							bool flag2 = ((flag ? 1U : 0U) | 2147483648U) == 0U;
							if (flag2)
							{
								return false;
							}
							if (-1 == 0)
							{
								goto IL_182;
							}
							goto IL_4B;
						}
					}
					else if ((flag ? 1U : 0U) - (flag ? 1U : 0U) > 4294967295U)
					{
						continue;
					}
					try
					{
						DataHelper.SortBytes(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
						TCPClient.snTotalSendCount += tcpOutPacket.PacketDataSize;
						this._x70087761226fbdf2.BeginSend(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize, SocketFlags.None, new AsyncCallback(this.x9982d7f900972af1), null);
						this._xaf6cc4b253f9355c = DateTime.Now.Ticks / 10000L;
						return true;
					}
					catch (Exception exception)
					{
						DebugTextLog.LogException(exception);
						goto IL_41;
					}
					break;
				}
				return false;
			}
			if (15 != 0)
			{
				return false;
			}
			IL_15:
			if ((flag ? 1U : 0U) < 0U)
			{
				goto IL_41;
			}
			return false;
			IL_41:
			if (this.x5484bff21ae952c5 == null)
			{
				bool flag2 = ((flag ? 1U : 0U) & 0U) == 0U;
				if (flag2)
				{
					return false;
				}
				goto IL_15;
			}
			IL_4B:
			this.x5484bff21ae952c5(this, new SocketConnectEventArgs
			{
				RemoteEndPoint = this.GetRemoteEndPoint(),
				Error = "Failed",
				NetSocketType = 1
			});
			IL_182:
			goto IL_15;
		}

		public void Disconnect(SocketShutdown how = SocketShutdown.Both)
		{
			if (this._x70087761226fbdf2 == null)
			{
				if (!false)
				{
					goto IL_69;
				}
			}
			else
			{
				DebugTextLog.Log(new object[]
				{
					"TCPClient: Disconnect"
				});
				this._xe3117ff5cca4d9ac = false;
				if (!false)
				{
					goto IL_36;
				}
			}
			IL_0B:
			if (!false)
			{
				goto IL_1F;
			}
			IL_0D:
			if (this._x70087761226fbdf2.Connected)
			{
				this._x70087761226fbdf2.Shutdown(how);
				if (false)
				{
					return;
				}
				goto IL_67;
			}
			IL_1F:
			this._x70087761226fbdf2 = null;
			if (2 != 0)
			{
				return;
			}
			if (4 == 0)
			{
				return;
			}
			goto IL_69;
			IL_36:
			goto IL_0D;
			IL_67:
			goto IL_0B;
			IL_69:
			if (!false)
			{
				return;
			}
			goto IL_36;
		}

		public void NotifyRecvData(SocketConnectEventArgs e)
		{
			if (this.x5484bff21ae952c5 != null)
			{
				this.x5484bff21ae952c5(this, e);
			}
		}

		public string GetRemoteEndPoint()
		{
			try
			{
				return StringUtil.substitute("{0}:{1}", new object[]
				{
					this._xa6532f30f3a61c54,
					this._x6689a717755b59cb
				});
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return null;
		}

		private bool xd0ab16336c9c9beb(object xe0292b9ed559da7d)
		{
			TCPInPacket tcpinPacket = xe0292b9ed559da7d as TCPInPacket;
			Socket currentSocket = tcpinPacket.CurrentSocket;
			return TCPClient.ProcessServerCmd(this, tcpinPacket.PacketCmdID, tcpinPacket.GetPacketBytes(), tcpinPacket.PacketDataSize);
		}

		private void x78f245401316535b(IAsyncResult xbf8b4f8416a30f37)
		{
			try
			{
				object[] array = new object[1];
				if (-2 != 0)
				{
					array[0] = "TCPClient SocketConnected";
					DebugTextLog.Log(array);
				}
				if (this._x70087761226fbdf2 != null)
				{
					this._x70087761226fbdf2.EndConnect(xbf8b4f8416a30f37);
					for (;;)
					{
						object[] messages = new object[]
						{
							"SocketConnected 连接已经建立"
						};
						if (false)
						{
							goto IL_123;
						}
						if (15 != 0)
						{
							goto Block_3;
						}
					}
					IL_21:
					byte[] bytes;
					this._x70087761226fbdf2.Send(bytes);
					IL_2E:
					this._xe3117ff5cca4d9ac = true;
					this._x70087761226fbdf2.BeginReceive(this.x7100ccc46ec37c9a, 0, this.x7100ccc46ec37c9a.Length, SocketFlags.None, new AsyncCallback(this.xb5a644f8a39dbb5f), this.x7100ccc46ec37c9a);
					if (this.x5484bff21ae952c5 == null)
					{
						if (255 == 0)
						{
							goto IL_21;
						}
					}
					else
					{
						this.x5484bff21ae952c5(this, new SocketConnectEventArgs
						{
							RemoteEndPoint = this.GetRemoteEndPoint(),
							Error = "Success",
							NetSocketType = 0
						});
					}
					return;
					Block_3:
					if (!false)
					{
						object[] messages;
						DebugTextLog.Log(messages);
					}
					if (!ProtocolTypes.EnableTengXunTGW)
					{
						goto IL_2E;
					}
					IL_123:
					if (2147483647 != 0)
					{
						string s = StringUtil.substitute("tgw_l7_forward\r\nHost: {0}:{1}\r\n\r\n", new object[]
						{
							this._xa6532f30f3a61c54,
							this._x6689a717755b59cb
						});
						bytes = new UTF8Encoding().GetBytes(s);
					}
					goto IL_21;
				}
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		private void x660afe3e5ce08f33()
		{
			DebugTextLog.LogStackMsg("DoSocketClosed: \r\n");
			if (this._x70087761226fbdf2 != null && this.x5484bff21ae952c5 != null)
			{
				this.x5484bff21ae952c5(this, new SocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Success",
					NetSocketType = 3
				});
			}
			DebugTextLog.Log(new object[]
			{
				"DoSocketClosed"
			});
			if (!false)
			{
				if (this._x70087761226fbdf2 == null)
				{
					goto IL_2B;
				}
			}
			this._x70087761226fbdf2.Close();
			this._x70087761226fbdf2 = null;
			IL_2B:
			this._xe3117ff5cca4d9ac = false;
		}

		private void xb5a644f8a39dbb5f(IAsyncResult xbf8b4f8416a30f37)
		{
			try
			{
				if (this._x70087761226fbdf2 != null)
				{
					SocketError socketError = SocketError.Success;
					int num = 0;
					bool flag;
					if (2 != 0)
					{
						for (;;)
						{
							if (this._x70087761226fbdf2.Connected)
							{
								goto IL_11A;
							}
							if (((uint)num | 2U) != 0U)
							{
								goto IL_150;
							}
							goto IL_15A;
							IL_CE:
							if (num <= 0)
							{
								goto IL_15A;
							}
							TCPClient.snTotalRecvCount += num;
							flag = ((uint)num + (uint)num > uint.MaxValue);
							if (flag)
							{
								flag = ((uint)num + (uint)num < 0U);
								if (!flag)
								{
									break;
								}
							}
							else if (!this._xa328d8c31e63da26.WriteData(this.x7100ccc46ec37c9a, 0, num))
							{
								break;
							}
							flag = ((uint)num - (uint)num < 0U);
							if (flag)
							{
								continue;
							}
							goto IL_193;
							IL_11A:
							num = this._x70087761226fbdf2.EndReceive(xbf8b4f8416a30f37, out socketError);
							goto IL_CE;
							IL_15A:
							this.x660afe3e5ce08f33();
							flag = ((uint)num > uint.MaxValue);
							if (!flag)
							{
								break;
							}
							if (!false)
							{
								goto IL_11A;
							}
							IL_150:
							if (4 == 0)
							{
								goto IL_15A;
							}
							goto IL_CE;
						}
						return;
						IL_193:;
					}
					flag = ((uint)num + (uint)num < 0U);
					if (flag)
					{
						goto IL_68;
					}
					IL_18:
					if (!this.x20675a21460278a8)
					{
						goto IL_68;
					}
					IL_20:
					this._x70087761226fbdf2.BeginReceive(this.x7100ccc46ec37c9a, 0, this.x7100ccc46ec37c9a.Length, SocketFlags.None, new AsyncCallback(this.xb5a644f8a39dbb5f), this.x7100ccc46ec37c9a);
					goto IL_A2;
					IL_68:
					this.x20675a21460278a8 = true;
					if ((uint)num + (uint)num < 0U)
					{
						goto IL_18;
					}
					flag = ((uint)num + (uint)num < 0U);
					if (!flag)
					{
						this.x60245825be2151f2();
						goto IL_20;
					}
					IL_A2:
					if (false)
					{
					}
				}
				return;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			if (this.x5484bff21ae952c5 != null)
			{
				this.x5484bff21ae952c5(this, new SocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Failed",
					NetSocketType = 2
				});
			}
		}

		private void x9982d7f900972af1(IAsyncResult xbf8b4f8416a30f37)
		{
			try
			{
				SocketError socketError;
				if (this._x70087761226fbdf2 == null)
				{
					if (!false)
					{
						return;
					}
				}
				else
				{
					socketError = SocketError.Success;
				}
				this._x70087761226fbdf2.EndSend(xbf8b4f8416a30f37, out socketError);
				if (socketError == SocketError.Success)
				{
					return;
				}
				this.x660afe3e5ce08f33();
				return;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			if (this.x5484bff21ae952c5 != null)
			{
				this.x5484bff21ae952c5(this, new SocketConnectEventArgs
				{
					RemoteEndPoint = this.GetRemoteEndPoint(),
					Error = "Failed",
					NetSocketType = 1
				});
			}
		}

		public static ProcessServerCmdHandler ProcessServerCmd;

		public static int snTotalSendCount;

		public static int snTotalRecvCount;

		private byte[] x7100ccc46ec37c9a = new byte[6144];

		private TCPOutPacketPool x6528c835948babcb;

		private bool _xe3117ff5cca4d9ac;

		private string _xa6532f30f3a61c54 = "";

		private int _x6689a717755b59cb;

		private Socket _x70087761226fbdf2;

		private TCPInPacket _xa328d8c31e63da26;

		private long _xaf6cc4b253f9355c;

		private int _xbd3cf1f5f80e7d88 = 10000;

		private int _xd770807629889cd7 = 10000;

		private SocketConnectEventHandler x5484bff21ae952c5;

		private System.Timers.Timer x4bef6898ca94460a;

		private bool x20675a21460278a8;
	}
}
