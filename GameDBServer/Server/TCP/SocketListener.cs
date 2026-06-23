using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
				Interlocked.Exchange(ref result, this.numConnectedSockets);
				return result;
			}
		}

		public int TotalBytesReadSize
		{
			get
			{
				int result = 0;
				Interlocked.Exchange(ref result, this.totalBytesRead);
				return result;
			}
		}

		public int TotalBytesWriteSize
		{
			get
			{
				int result = 0;
				Interlocked.Exchange(ref result, this.totalBytesWrite);
				return result;
			}
		}

		public event SocketConnectedEventHandler SocketConnected = null;

		public event SocketClosedEventHandler SocketClosed = null;

		public event SocketReceivedEventHandler SocketReceived = null;

		public event SocketSendedEventHandler SocketSended = null;

		internal SocketListener(int numConnections, int receiveBufferSize)
		{
			this.totalBytesRead = 0;
			this.totalBytesWrite = 0;
			this.numConnectedSockets = 0;
			this.numConnections = numConnections;
			this.ReceiveBufferSize = receiveBufferSize;
			this.bufferManager = new BufferManager(receiveBufferSize * numConnections, receiveBufferSize);
			this.ConnectedSocketsDict = new Dictionary<Socket, bool>(numConnections);
			this.readPool = new SocketAsyncEventArgsPool(numConnections);
			this.writePool = new SocketAsyncEventArgsPool(numConnections * 5);
			this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);
		}

		private void AddSocket(Socket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Add(socket, true);
			}
		}

		private void RemoveSocket(Socket socket)
		{
			lock (this.ConnectedSocketsDict)
			{
				this.ConnectedSocketsDict.Remove(socket);
			}
		}

		private bool FindSocket(Socket socket)
		{
			bool result = false;
			lock (this.ConnectedSocketsDict)
			{
				result = this.ConnectedSocketsDict.ContainsKey(socket);
			}
			return result;
		}

		private void CloseClientSocket(SocketAsyncEventArgs e)
		{
			AsyncUserToken asyncUserToken = e.UserToken as AsyncUserToken;
			try
			{
				Socket currentSocket = asyncUserToken.CurrentSocket;
				if (this.FindSocket(currentSocket))
				{
					this.RemoveSocket(currentSocket);
					try
					{
						LogManager.WriteLog(LogTypes.Info, string.Format("关闭客户端连接: {0}, 操作: {1}, 原因: {2}", currentSocket.RemoteEndPoint, e.LastOperation, e.SocketError), null, true);
					}
					catch (Exception)
					{
					}
					this.semaphoreAcceptedClients.Release();
					Interlocked.Decrement(ref this.numConnectedSockets);
					if (null != this.SocketClosed)
					{
						this.SocketClosed(this, e);
					}
					try
					{
						currentSocket.Shutdown(SocketShutdown.Both);
					}
					catch (Exception)
					{
					}
					try
					{
						currentSocket.Close();
					}
					catch (Exception)
					{
					}
				}
			}
			finally
			{
				asyncUserToken.CurrentSocket = null;
				asyncUserToken.Tag = null;
				if (e.LastOperation == SocketAsyncOperation.Send)
				{
					e.SetBuffer(null, 0, 0);
					this.writePool.Push(e);
				}
				else if (e.LastOperation == SocketAsyncOperation.Receive)
				{
					this.readPool.Push(e);
				}
			}
		}

		internal void Init()
		{
			this.bufferManager.InitBuffer();
			for (int i = 0; i < this.numConnections; i++)
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
				for (int j = 0; j < 5; j++)
				{
					socketAsyncEventArgs = new SocketAsyncEventArgs();
					socketAsyncEventArgs.Completed += this.OnIOCompleted;
					socketAsyncEventArgs.UserToken = new AsyncUserToken
					{
						CurrentSocket = null,
						Tag = null
					};
					this.writePool.Push(socketAsyncEventArgs);
				}
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
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::OnAcceptCompleted 中发生了异常错误", new object[0]), null, true);
				DataHelper.WriteFormatExceptionLog(e2, "", false, false);
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
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::OnIOCompleted 中发生了异常错误", new object[0]), null, true);
				DataHelper.WriteFormatExceptionLog(e2, "", false, false);
			}
		}

		private bool _ReceiveAsync(SocketAsyncEventArgs readEventArgs)
		{
			bool result;
			try
			{
				Socket currentSocket = (readEventArgs.UserToken as AsyncUserToken).CurrentSocket;
				result = currentSocket.ReceiveAsync(readEventArgs);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::_ReceiveAsync 中发生了异常错误", new object[0]), null, true);
				this.CloseClientSocket(readEventArgs);
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
				Socket currentSocket = (writeEventArgs.UserToken as AsyncUserToken).CurrentSocket;
				result = currentSocket.SendAsync(writeEventArgs);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("在SocketListener::_SendAsync 中发生了异常错误", new object[0]), null, true);
				exception = true;
				result = true;
			}
			return result;
		}

		internal bool SendData(Socket s, TCPOutPacket tcpOutPacket)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = this.writePool.Pop();
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
			socketAsyncEventArgs.SetBuffer(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
			(socketAsyncEventArgs.UserToken as AsyncUserToken).CurrentSocket = s;
			(socketAsyncEventArgs.UserToken as AsyncUserToken).Tag = tcpOutPacket;
			bool flag = false;
			if (!this._SendAsync(socketAsyncEventArgs, out flag))
			{
				this.ProcessSend(socketAsyncEventArgs);
			}
			return !flag;
		}

		private void ProcessAccept(SocketAsyncEventArgs e)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = null;
			Interlocked.Increment(ref this.numConnectedSockets);
			Socket acceptSocket = e.AcceptSocket;
			socketAsyncEventArgs = this.readPool.Pop();
			if (null == socketAsyncEventArgs)
			{
				try
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("新远程连接: {0}, 但是readPool内的缓存不足，直接关闭连接:{1}", acceptSocket.RemoteEndPoint, this.ConnectedSocketsCount), null, true);
				}
				catch (Exception)
				{
				}
				try
				{
					acceptSocket.Shutdown(SocketShutdown.Both);
				}
				catch (Exception)
				{
				}
				try
				{
					acceptSocket.Close();
				}
				catch (Exception)
				{
				}
				Interlocked.Decrement(ref this.numConnectedSockets);
				this.StartAccept(e);
			}
			else
			{
				(socketAsyncEventArgs.UserToken as AsyncUserToken).CurrentSocket = e.AcceptSocket;
				byte[] array = new byte[12];
				BitConverter.GetBytes(1U).CopyTo(array, 0);
				BitConverter.GetBytes(120000U).CopyTo(array, 4);
				BitConverter.GetBytes(5000U).CopyTo(array, 8);
				acceptSocket.IOControl((IOControlCode)((ulong)-1744830460), array, null);
				this.AddSocket(acceptSocket);
				try
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("新远程连接: {0}, 当前总共: {1}", acceptSocket.RemoteEndPoint, this.numConnectedSockets), null, true);
				}
				catch (Exception)
				{
				}
				if (null != this.SocketConnected)
				{
					this.SocketConnected(this, socketAsyncEventArgs);
				}
				if (!this._ReceiveAsync(socketAsyncEventArgs))
				{
					this.ProcessReceive(socketAsyncEventArgs);
				}
				this.StartAccept(e);
			}
		}

		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				Interlocked.Add(ref this.totalBytesRead, e.BytesTransferred);
				bool flag = true;
				if (null != this.SocketReceived)
				{
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
					this.CloseClientSocket(e);
				}
			}
			else
			{
				this.CloseClientSocket(e);
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
				Interlocked.Add(ref this.totalBytesWrite, e.BytesTransferred);
			}
			e.SetBuffer(null, 0, 0);
			(e.UserToken as AsyncUserToken).CurrentSocket = null;
			(e.UserToken as AsyncUserToken).Tag = null;
			this.writePool.Push(e);
		}

		internal void Start(string ip, int port)
		{
			if ("" == ip)
			{
				ip = "0.0.0.0";
			}
			IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(ip), port);
			this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.listenSocket.Bind(localEP);
			this.listenSocket.Listen(100);
			this.StartAccept(null);
		}

		public void Stop()
		{
			Socket socket = this.listenSocket;
			this.listenSocket = null;
			socket.Close();
		}

		private void CloseSocket(Socket s)
		{
			try
			{
				s.Shutdown(SocketShutdown.Both);
			}
			catch (Exception)
			{
			}
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
			this.semaphoreAcceptedClients.WaitOne();
			if (!this.listenSocket.AcceptAsync(acceptEventArg))
			{
				this.ProcessAccept(acceptEventArg);
			}
		}

		private const int opsToPreAlloc = 1;

		private int ReceiveBufferSize;

		private BufferManager bufferManager;

		private Socket listenSocket;

		private int numConnectedSockets;

		private Dictionary<Socket, bool> ConnectedSocketsDict;

		private int numConnections;

		private SocketAsyncEventArgsPool readPool;

		private SocketAsyncEventArgsPool writePool;

		private Semaphore semaphoreAcceptedClients;

		private int totalBytesRead;

		private int totalBytesWrite;
	}
}
