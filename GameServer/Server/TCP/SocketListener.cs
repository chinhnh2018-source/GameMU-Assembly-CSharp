using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic;
using GameServer.Logic.KuaFuIPStatistics;
using GameServer.Server;
using Server.Protocol;
using Server.Tools;

namespace Server.TCP
{
	public sealed class SocketListener
	{
		public int ConnectedSocketsCount
		{
			get
			{
				int result = 0;
				lock (this.ConnectedSocketsDict)
				{
					result = this.ConnectedSocketsDict.Count;
				}
				return result;
			}
		}

		public int ReadPoolCount
		{
			get
			{
				return this.readPool.Count;
			}
		}

		public int WritePoolCount
		{
			get
			{
				return this.writePool.Count;
			}
		}

		public long TotalBytesReadSize
		{
			get
			{
				long result = 0L;
				Interlocked.Exchange(ref result, this.totalBytesRead);
				return result;
			}
		}

		public long TotalBytesWriteSize
		{
			get
			{
				long result = 0L;
				Interlocked.Exchange(ref result, this.totalBytesWrite);
				return result;
			}
		}

		public bool DontAccept
		{
			get
			{
				return this._DontAccept;
			}
			set
			{
				this._DontAccept = value;
			}
		}

		public event SocketConnectedEvnetHandler SocketConnected = null;

		public event SocketClosedEventHandler SocketClosed = null;

		public event SocketReceivedEventHandler SocketReceived = null;

		public event SocketSendedEventHandler SocketSended = null;

		internal SocketListener(int numConnections, int receiveBufferSize)
		{
			this.totalBytesRead = 0L;
			this.totalBytesWrite = 0L;
			this.numConnections = numConnections;
			this.ReceiveBufferSize = receiveBufferSize;
			int num = numConnections * 3;
			this.bufferManager = new BufferManager(receiveBufferSize * num, receiveBufferSize);
			this.ConnectedSocketsDict = new Dictionary<TMSKSocket, bool>(num);
			this.readPool = new SocketAsyncEventArgsPool(num);
			this.writePool = new SocketAsyncEventArgsPool(num);
		}

		private void AddSocket(TMSKSocket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Add(socket, true);
			}
		}

		private void RemoveSocket(TMSKSocket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Remove(socket);
			}
		}

		private bool FindSocket(TMSKSocket socket)
		{
			bool result = false;
			lock (this.ConnectedSocketsDict)
			{
				result = this.ConnectedSocketsDict.ContainsKey(socket);
			}
			return result;
		}

		private void CloseClientSocket(SocketAsyncEventArgs e, string reason)
		{
			AsyncUserToken asyncUserToken = e.UserToken as AsyncUserToken;
			TMSKSocket tmsksocket = null;
			try
			{
				tmsksocket = asyncUserToken.CurrentSocket;
				string text = "未知";
				try
				{
					text = string.Format("{0}", tmsksocket.RemoteEndPoint);
				}
				catch (Exception)
				{
				}
				LogManager.WriteLog(2, string.Format("远程连接关闭: {0}, 当前总共: {1}, 原因1:{2}, 原因2:{3}", new object[]
				{
					text,
					this.ConnectedSocketsCount,
					reason,
					tmsksocket.CloseReason
				}), null, true);
				this.CloseSocket(tmsksocket, "");
			}
			finally
			{
				asyncUserToken.CurrentSocket = null;
				asyncUserToken.Tag = null;
				if (e.LastOperation == SocketAsyncOperation.Send)
				{
					e.SetBuffer(null, 0, 0);
					if (null != tmsksocket)
					{
						tmsksocket.PushWriteSocketAsyncEventArgs(e);
					}
				}
				else if (e.LastOperation == SocketAsyncOperation.Receive)
				{
					if (null != tmsksocket)
					{
						tmsksocket.PushReadSocketAsyncEventArgs(e);
					}
				}
			}
		}

		public Dictionary<long, Tuple<int, int, int>> GetSocketCnt()
		{
			Dictionary<long, Tuple<int, int, int>> dictionary = new Dictionary<long, Tuple<int, int, int>>();
			lock (this.ConnectedSocketsDict)
			{
				foreach (KeyValuePair<TMSKSocket, bool> keyValuePair in this.ConnectedSocketsDict)
				{
					int num = 0;
					int num2 = 0;
					if (null != keyValuePair.Key.session)
					{
						num = ((keyValuePair.Key.session.SocketState == 4) ? 1 : 0);
						num2 = ((keyValuePair.Key.session.SocketTime[1] > 0L) ? 1 : 0);
					}
					Tuple<int, int, int> tuple = null;
					if (dictionary.TryGetValue(Global.GetIpAsIntSafe(keyValuePair.Key), out tuple))
					{
						tuple = new Tuple<int, int, int>(tuple.Item1 + 1, tuple.Item2 + num, tuple.Item3 + num2);
					}
					else
					{
						tuple = new Tuple<int, int, int>(1, num, num2);
					}
					dictionary[Global.GetIpAsIntSafe(keyValuePair.Key)] = tuple;
				}
			}
			return dictionary;
		}

		internal void Init()
		{
			this.bufferManager.InitBuffer();
			int num = this.numConnections * 3;
			for (int i = 0; i < num; i++)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += this.OnIOCompleted;
				socketAsyncEventArgs.UserToken = new AsyncUserToken
				{
					CurrentSocket = null,
					Tag = null
				};
				this.bufferManager.SetBuffer(socketAsyncEventArgs);
				this.readPool.Push(socketAsyncEventArgs);
			}
			for (int i = 0; i < num; i++)
			{
				SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += this.OnIOCompleted;
				socketAsyncEventArgs.UserToken = new AsyncUserToken
				{
					CurrentSocket = null,
					Tag = null
				};
				this.writePool.Push(socketAsyncEventArgs);
			}
		}

		private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				if (null != this.listenSocket)
				{
					this.ProcessAccept(e);
				}
			}
			catch (Exception e2)
			{
				LogManager.WriteLog(2, string.Format("在SocketListener::OnAcceptCompleted 中发生了异常错误", new object[0]), null, true);
				DataHelper.WriteFormatExceptionLog(e2, "OnAcceptCompleted", false, false);
			}
		}

		private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				SocketAsyncOperation lastOperation = e.LastOperation;
				if (lastOperation != SocketAsyncOperation.Receive)
				{
					if (lastOperation != SocketAsyncOperation.Send)
					{
						throw new ArgumentException("The last operation completed on the socket was not a receive or send");
					}
					this.ProcessSend(e);
				}
				else
				{
					this.ProcessReceive(e);
				}
			}
			catch (Exception e2)
			{
				LogManager.WriteLog(2, string.Format("在SocketListener::OnIOCompleted 中发生了异常错误", new object[0]), null, true);
				DataHelper.WriteFormatExceptionLog(e2, "OnIOCompleted", false, false);
			}
		}

		private bool _ReceiveAsync(SocketAsyncEventArgs readEventArgs)
		{
			bool result;
			try
			{
				TMSKSocket currentSocket = (readEventArgs.UserToken as AsyncUserToken).CurrentSocket;
				result = currentSocket.ReceiveAsync(readEventArgs);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("在SocketListener::_ReceiveAsync 中发生了异常错误", new object[0]), null, true);
				string text = ex.Message.ToString();
				this.CloseClientSocket(readEventArgs, text.Replace('\n', ' '));
				result = true;
			}
			return result;
		}

		private bool _SendAsync(SocketAsyncEventArgs writeEventArgs, out bool exception)
		{
			exception = false;
			bool result;
			try
			{
				TMSKSocket currentSocket = (writeEventArgs.UserToken as AsyncUserToken).CurrentSocket;
				result = currentSocket.SendAsync(writeEventArgs);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("在SocketListener::_SendAsync 中发生了异常错误:{0}", ex.Message), null, true);
				exception = true;
				result = true;
			}
			return result;
		}

		internal bool SendData(TMSKSocket s, TCPOutPacket tcpOutPacket, bool pushBack = true)
		{
			if (s != null && tcpOutPacket != null)
			{
				ushort num = tcpOutPacket.PacketCmdID;
				if (s.magic > 0 && num > 100)
				{
					num += s.magic;
				}
				Array.Copy(BitConverter.GetBytes(num), 0, tcpOutPacket.GetPacketBytes(), 4, 2);
			}
			bool result = false;
			if (null != s)
			{
				if (s.Connected)
				{
					result = Global._SendBufferManager.AddOutPacket(s, tcpOutPacket);
				}
			}
			if (pushBack)
			{
				Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
			}
			return result;
		}

		public bool SendData(TMSKSocket s, byte[] buffer, int offset, int count, MemoryBlock item)
		{
			this.GTotalSendCount++;
			SocketAsyncEventArgs socketAsyncEventArgs = s.PopWriteSocketAsyncEventArgs();
			if (null == socketAsyncEventArgs)
			{
				socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += this.OnIOCompleted;
				socketAsyncEventArgs.UserToken = new AsyncUserToken
				{
					CurrentSocket = null,
					Tag = null
				};
			}
			socketAsyncEventArgs.SetBuffer(buffer, offset, count);
			AsyncUserToken asyncUserToken = socketAsyncEventArgs.UserToken as AsyncUserToken;
			asyncUserToken.CurrentSocket = s;
			asyncUserToken.Tag = item;
			bool flag = false;
			if (!this._SendAsync(socketAsyncEventArgs, out flag))
			{
				this.ProcessSend(socketAsyncEventArgs);
			}
			if (flag)
			{
				if (null != this.SocketSended)
				{
					this.SocketSended(this, socketAsyncEventArgs);
				}
				socketAsyncEventArgs.SetBuffer(null, 0, 0);
				asyncUserToken.CurrentSocket = null;
				asyncUserToken.Tag = null;
				s.PushWriteSocketAsyncEventArgs(socketAsyncEventArgs);
			}
			return !flag;
		}

		public bool SendData(TMSKSocket s, byte[] buffer, int offset, int count, MemoryBlock item, SendBuffer sendBuffer)
		{
			this.GTotalSendCount++;
			SocketAsyncEventArgs socketAsyncEventArgs = s.PopWriteSocketAsyncEventArgs();
			if (null == socketAsyncEventArgs)
			{
				socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += this.OnIOCompleted;
				socketAsyncEventArgs.UserToken = new AsyncUserToken
				{
					CurrentSocket = null,
					Tag = null
				};
			}
			socketAsyncEventArgs.SetBuffer(buffer, offset, count);
			AsyncUserToken asyncUserToken = socketAsyncEventArgs.UserToken as AsyncUserToken;
			asyncUserToken.CurrentSocket = s;
			asyncUserToken.Tag = item;
			asyncUserToken._SendBuffer = sendBuffer;
			bool flag = false;
			if (!this._SendAsync(socketAsyncEventArgs, out flag))
			{
				this.ProcessSend(socketAsyncEventArgs);
			}
			if (flag)
			{
				if (null != this.SocketSended)
				{
					this.SocketSended(this, socketAsyncEventArgs);
				}
				socketAsyncEventArgs.SetBuffer(null, 0, 0);
				asyncUserToken.CurrentSocket = null;
				asyncUserToken.Tag = null;
				s.PushWriteSocketAsyncEventArgs(socketAsyncEventArgs);
			}
			return !flag;
		}

		private void ProcessAccept(SocketAsyncEventArgs e)
		{
			TMSKSocket tmsksocket = new TMSKSocket(e.AcceptSocket);
			tmsksocket.SetAcceptIp();
			bool flag = false;
			bool? inIpWhiteList = null;
			if (this.EnabledIPListFilter)
			{
				lock (this.IPWhiteList)
				{
					if (this.EnabledIPListFilter && tmsksocket != null && null != tmsksocket.RemoteEndPoint)
					{
						IPEndPoint ipendPoint = tmsksocket.RemoteEndPoint as IPEndPoint;
						if (ipendPoint != null && null != ipendPoint.Address)
						{
							string text = ipendPoint.Address.ToString();
							if (!string.IsNullOrEmpty(text) && !this.IPWhiteList.ContainsKey(text))
							{
								LogManager.WriteLog(2, string.Format("新远程连接: {0}, 但是客户端IP处于IP过滤中:{1}", tmsksocket.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
								inIpWhiteList = new bool?(false);
							}
							else
							{
								inIpWhiteList = new bool?(true);
							}
						}
					}
				}
			}
			if (IPStatisticsManager.getInstance().GetIPInBeOperation(tmsksocket, 3))
			{
				flag = true;
			}
			if (this.DontAccept || flag)
			{
				try
				{
					if (flag)
					{
						LogManager.WriteLog(2, string.Format("新远程连接: {0}, 但是客户端IP处于IP过滤中，直接关闭连接:{1}", tmsksocket.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
					}
					else if (this.DontAccept)
					{
						LogManager.WriteLog(2, string.Format("新远程连接: {0}, 但是服务器端处于不接受新连接状态，直接关闭连接:{1}", tmsksocket.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
					}
				}
				catch (Exception)
				{
				}
				try
				{
					tmsksocket.Shutdown(SocketShutdown.Both);
				}
				catch (Exception)
				{
				}
				try
				{
					tmsksocket.Close(30);
				}
				catch (Exception)
				{
				}
				this.StartAccept(e);
			}
			else
			{
				byte[] array = new byte[12];
				BitConverter.GetBytes(1U).CopyTo(array, 0);
				BitConverter.GetBytes(120000U).CopyTo(array, 4);
				BitConverter.GetBytes(5000U).CopyTo(array, 8);
				tmsksocket.IOControl((IOControlCode)((ulong)-1744830460), array, null);
				LingerOption optionValue = new LingerOption(true, 10);
				tmsksocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, optionValue);
				SocketAsyncEventArgs socketAsyncEventArgs = null;
				socketAsyncEventArgs = tmsksocket.PopReadSocketAsyncEventArgs();
				if (null == socketAsyncEventArgs)
				{
					try
					{
						LogManager.WriteLog(2, string.Format("新远程连接: {0}, 但是readPool内的缓存不足，直接关闭连接:{1}", tmsksocket.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
					}
					catch (Exception)
					{
					}
					try
					{
						tmsksocket.Shutdown(SocketShutdown.Both);
					}
					catch (Exception)
					{
					}
					try
					{
						tmsksocket.Close(30);
					}
					catch (Exception)
					{
					}
					this.StartAccept(e);
				}
				else
				{
					(socketAsyncEventArgs.UserToken as AsyncUserToken).CurrentSocket = tmsksocket;
					Global._SendBufferManager.Add(tmsksocket);
					this.AddSocket(tmsksocket);
					try
					{
						LogManager.WriteLog(2, string.Format("新远程连接: {0}, 当前总共: {1}", tmsksocket.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
					}
					catch (Exception)
					{
					}
					if (null != this.SocketConnected)
					{
						this.SocketConnected(this, socketAsyncEventArgs);
					}
					tmsksocket.session.InIpWhiteList = inIpWhiteList;
					tmsksocket.session.SetSocketTime(0);
					if (!this._ReceiveAsync(socketAsyncEventArgs))
					{
						this.ProcessReceive(socketAsyncEventArgs);
					}
					this.StartAccept(e);
				}
			}
		}

		private unsafe void ProcessReceive(SocketAsyncEventArgs e)
		{
			AsyncUserToken asyncUserToken = e.UserToken as AsyncUserToken;
			TMSKSocket currentSocket = asyncUserToken.CurrentSocket;
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				Interlocked.Add(ref this.totalBytesRead, (long)e.BytesTransferred);
				bool flag = true;
				if (null != this.SocketReceived)
				{
					if (GameManager.FlagUseWin32Decrypt)
					{
						int bytesTransferred = e.BytesTransferred;
						fixed (byte* buffer = e.Buffer)
						{
							Win32API.SortBytes(buffer, e.Offset, e.BytesTransferred, currentSocket.SortKey64);
						}
					}
					else
					{
						DataHelper.SortBytes(e.Buffer, e.Offset, e.BytesTransferred, currentSocket.SortKey64);
					}
					try
					{
						flag = this.SocketReceived(this, e);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						flag = false;
					}
				}
				if (flag)
				{
					if (!this._ReceiveAsync(e))
					{
						this.ProcessReceive(e);
					}
				}
				else
				{
					ushort num = TCPManager.getInstance().LastPacketCmdID(currentSocket);
					string reason = string.Format("CMD={0}", ((TCPGameServerCmds)num).ToString());
					this.CloseClientSocket(e, reason);
				}
			}
			else
			{
				string reason = string.Format("[{0}]{1}", (int)e.SocketError, e.SocketError.ToString());
				this.CloseClientSocket(e, reason);
			}
		}

		private void ProcessSend(SocketAsyncEventArgs e)
		{
			if (null != this.SocketSended)
			{
				this.SocketSended(this, e);
			}
			if (e.SocketError == SocketError.Success)
			{
				Interlocked.Add(ref this.totalBytesWrite, (long)e.BytesTransferred);
			}
			e.SetBuffer(null, 0, 0);
			TMSKSocket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			(e.UserToken as AsyncUserToken).CurrentSocket = null;
			(e.UserToken as AsyncUserToken).Tag = null;
			if (null != currentSocket)
			{
				currentSocket.PushWriteSocketAsyncEventArgs(e);
			}
		}

		internal void Start(string ip, int port)
		{
			if ("" == ip)
			{
				ip = "0.0.0.0";
			}
			IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(ip), port);
			this.listenSocket = new TMSKSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.listenSocket.Bind(localEP);
			this.listenSocket.Listen(100);
			this.StartAccept(null);
		}

		public void Stop()
		{
			TMSKSocket tmsksocket = this.listenSocket;
			this.listenSocket = null;
			tmsksocket.Close();
		}

		public bool CloseSocket(TMSKSocket s, string reason = "")
		{
			bool result;
			if (!this.FindSocket(s))
			{
				Global._SendBufferManager.Remove(s);
				result = false;
			}
			else
			{
				if (!string.IsNullOrEmpty(reason))
				{
					s.CloseReason = reason;
				}
				this.RemoveSocket(s);
				if (null != this.SocketClosed)
				{
					this.SocketClosed(this, s);
				}
				Global._SendBufferManager.Remove(s);
				try
				{
					s.Shutdown(SocketShutdown.Both);
				}
				catch (Exception ex)
				{
					try
					{
						LogManager.WriteLog(0, string.Format("CloseSocket s.Shutdown()异常: {0}, {1}", s.RemoteEndPoint, ex.Message), null, true);
					}
					catch (Exception)
					{
					}
				}
				try
				{
					s.Close(30);
				}
				catch (Exception ex)
				{
					try
					{
						LogManager.WriteLog(0, string.Format("CloseSocket s.Close()异常: {0}, {1}", s.RemoteEndPoint, ex.Message), null, true);
					}
					catch (Exception)
					{
					}
				}
				result = true;
			}
			return result;
		}

		private void StartAccept(SocketAsyncEventArgs acceptEventArg)
		{
			if (acceptEventArg == null)
			{
				acceptEventArg = new SocketAsyncEventArgs();
				acceptEventArg.Completed += this.OnAcceptCompleted;
			}
			else
			{
				acceptEventArg.AcceptSocket = null;
			}
			if (!this.listenSocket.AcceptAsync(acceptEventArg))
			{
				this.ProcessAccept(acceptEventArg);
			}
		}

		public List<string> InitIPWhiteList(string[] ipList, bool enabeld = true)
		{
			List<string> list = new List<string>();
			List<string> result;
			lock (this.IPWhiteList)
			{
				this.EnabledIPListFilter = false;
				this.IPWhiteList.Clear();
				if (ipList != null && ipList.Length > 0)
				{
					foreach (string ipString in ipList)
					{
						IPAddress ipaddress;
						if (IPAddress.TryParse(ipString, out ipaddress))
						{
							list.Add(ipaddress.ToString());
							this.IPWhiteList[ipaddress.ToString()] = true;
						}
					}
					if (this.IPWhiteList.Count > 0)
					{
						this.EnabledIPListFilter = enabeld;
					}
				}
				result = list;
			}
			return result;
		}

		public void ClearTimeoutSocket()
		{
			long num = TimeUtil.NOW();
			lock (this.ConnectedSocketsDict)
			{
				foreach (TMSKSocket tmsksocket in this.ConnectedSocketsDict.Keys)
				{
					if (tmsksocket.session.SocketTime[1] == 0L && tmsksocket.Connected)
					{
						if (Math.Abs(num - tmsksocket.session.SocketTime[0]) > 30000L)
						{
							try
							{
								if (string.IsNullOrEmpty(tmsksocket.CloseReason))
								{
									tmsksocket.CloseReason = "ClearTimeoutSocket";
									GlobalEventSource.getInstance().fireEvent(new LoginFailByTimeoutEventObject(Global.GetIpAsIntSafe(tmsksocket)));
								}
								tmsksocket.Shutdown(SocketShutdown.Both);
							}
							catch
							{
							}
							try
							{
								tmsksocket.Close(30);
							}
							catch
							{
							}
						}
					}
				}
			}
		}

		private const int opsToPreAlloc = 2;

		public int GTotalSendCount = 0;

		private bool EnabledIPListFilter = false;

		private Dictionary<string, bool> IPWhiteList = new Dictionary<string, bool>();

		private int ReceiveBufferSize;

		private BufferManager bufferManager;

		private TMSKSocket listenSocket;

		private Dictionary<TMSKSocket, bool> ConnectedSocketsDict;

		public int numConnections;

		public SocketAsyncEventArgsPool readPool;

		public SocketAsyncEventArgsPool writePool;

		private long totalBytesRead;

		private long totalBytesWrite;

		private bool _DontAccept = true;
	}
}
