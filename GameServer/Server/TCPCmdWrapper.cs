using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Server
{
	public class TCPCmdWrapper
	{
		public TCPCmdWrapper(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count)
		{
			this.tcpMgr = tcpMgr;
			this.socket = socket;
			this.tcpClientPool = tcpClientPool;
			this.tcpRandKey = tcpRandKey;
			this.pool = pool;
			this.nID = nID;
			this.data = data;
			this.count = count;
			lock (TCPCmdWrapper.CountLock)
			{
				TCPCmdWrapper.TotalWrapperCount++;
			}
		}

		public TCPManager TcpMgr
		{
			get
			{
				return this.tcpMgr;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		public int NID
		{
			get
			{
				return this.nID;
			}
		}

		public TCPOutPacketPool Pool
		{
			get
			{
				return this.pool;
			}
		}

		public TCPRandKey TcpRandKey
		{
			get
			{
				return this.tcpRandKey;
			}
		}

		public TCPClientPool TcpClientPool
		{
			get
			{
				return this.tcpClientPool;
			}
		}

		public TMSKSocket TMSKSocket
		{
			get
			{
				return this.socket;
			}
		}

		public static int GetTotalCount()
		{
			int result = 0;
			lock (TCPCmdWrapper.CountLock)
			{
				result = TCPCmdWrapper.TotalWrapperCount;
			}
			return result;
		}

		public static TCPCmdWrapper Get(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count)
		{
			TCPCmdWrapper tcpcmdWrapper = null;
			lock (TCPCmdWrapper.CachedWrapperList)
			{
				if (TCPCmdWrapper.CachedWrapperList.Count > 0)
				{
					tcpcmdWrapper = TCPCmdWrapper.CachedWrapperList.Dequeue();
					tcpcmdWrapper.tcpMgr = tcpMgr;
					tcpcmdWrapper.socket = socket;
					tcpcmdWrapper.tcpClientPool = tcpClientPool;
					tcpcmdWrapper.tcpRandKey = tcpRandKey;
					tcpcmdWrapper.pool = pool;
					tcpcmdWrapper.nID = nID;
					tcpcmdWrapper.data = data;
					tcpcmdWrapper.count = count;
				}
			}
			if (null == tcpcmdWrapper)
			{
				tcpcmdWrapper = new TCPCmdWrapper(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, nID, data, count);
			}
			return tcpcmdWrapper;
		}

		public void release()
		{
			lock (TCPCmdWrapper.CachedWrapperList)
			{
				if (TCPCmdWrapper.CachedWrapperList.Count < TCPCmdWrapper.MaxCachedWrapperCount)
				{
					TCPCmdWrapper.CachedWrapperList.Enqueue(this);
					return;
				}
			}
			this.tcpMgr = null;
			this.socket = null;
			this.tcpClientPool = null;
			this.tcpRandKey = null;
			this.pool = null;
			this.data = null;
			lock (TCPCmdWrapper.CountLock)
			{
				TCPCmdWrapper.TotalWrapperCount--;
			}
		}

		public static int MaxCachedWrapperCount = 1000;

		private static Queue<TCPCmdWrapper> CachedWrapperList = new Queue<TCPCmdWrapper>();

		private static object CountLock = new object();

		private static int TotalWrapperCount = 0;

		private TCPManager tcpMgr;

		private TMSKSocket socket;

		private TCPClientPool tcpClientPool;

		private TCPRandKey tcpRandKey;

		private TCPOutPacketPool pool;

		private int nID;

		private byte[] data;

		private int count;
	}
}
