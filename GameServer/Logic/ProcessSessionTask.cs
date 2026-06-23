using System;
using System.Collections.Concurrent;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	public class ProcessSessionTask : ScheduleTask
	{
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		public ProcessSessionTask(TCPSession session)
		{
			this.beginTime = TimeUtil.NowEx();
			this.session = session;
		}

		public void run()
		{
			int num = 0;
			long num2 = 0L;
			long num3 = 0L;
			if (Monitor.TryEnter(this.session.Lock))
			{
				try
				{
					long num4 = TimeUtil.NowEx();
					num3 = num4 - this.beginTime;
					TCPCmdWrapper tcpcmdWrapper = this.session.getNextTCPCmdWrapper();
					if (null != tcpcmdWrapper)
					{
						try
						{
							TCPCmdHandler.ProcessCmd(tcpcmdWrapper.TcpMgr, tcpcmdWrapper.TMSKSocket, tcpcmdWrapper.TcpClientPool, tcpcmdWrapper.TcpRandKey, tcpcmdWrapper.Pool, tcpcmdWrapper.NID, tcpcmdWrapper.Data, tcpcmdWrapper.Count);
						}
						catch (Exception ex)
						{
							DataHelper.WriteFormatExceptionLog(ex, string.Format("指令处理错误：{0},{1}", Global.GetDebugHelperInfo(tcpcmdWrapper.TMSKSocket), (TCPGameServerCmds)tcpcmdWrapper.NID), false, false);
						}
						ProcessSessionTask.processCmdNum += 1L;
						num2 = TimeUtil.NowEx() - num4;
						ProcessSessionTask.processTotalTime += num3 + num2;
						num = tcpcmdWrapper.NID;
						tcpcmdWrapper.release();
						tcpcmdWrapper = null;
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					Monitor.Exit(this.session.Lock);
				}
				if (num > 0)
				{
					TCPManager.RecordCmdDetail2(num, num2, num3);
				}
			}
			else
			{
				TCPManager.getInstance().taskExecutor.scheduleExecute(this, 5L);
			}
		}

		private TaskInternalLock _InternalLock = new TaskInternalLock();

		public static long processCmdNum = 0L;

		public static long processTotalTime = 0L;

		public static ConcurrentDictionary<int, PorcessCmdMoniter> cmdMoniter = new ConcurrentDictionary<int, PorcessCmdMoniter>();

		public static DateTime StartTime = TimeUtil.NowDateTime();

		private TCPSession session = null;

		private long beginTime = 0L;
	}
}
