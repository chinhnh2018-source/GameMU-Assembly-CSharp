using System;
using System.Collections.Generic;
using System.Threading;
using Server.Tools;

namespace GameServer.Core.Executor
{
	internal class ExecContext
	{
		public ExecContext()
		{
			this.WorkThread = new Thread(new ThreadStart(this.ThreadProc));
			this.WorkThread.IsBackground = true;
			this.WorkThread.Start();
		}

		private void ThreadProc()
		{
			Stack<ScheduleTask> stack = new Stack<ScheduleTask>();
			for (;;)
			{
				long num = TimeUtil.NOW();
				try
				{
					stack.Clear();
					lock (this.Mutex)
					{
						for (int i = 0; i < this.TaskList.Count; i++)
						{
							if (num >= this.TaskList[i].NextTicks)
							{
								this.TaskList[i].NextTicks = num + this.TaskList[i].Periodic;
								ScheduleTask scheduleTask = this.TaskList[i].Task;
								stack.Push(scheduleTask);
							}
						}
					}
					foreach (ScheduleTask scheduleTask in stack)
					{
						ScheduleTask scheduleTask;
						try
						{
							scheduleTask.run();
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(2, string.Format("{0}执行时异常,{1}", scheduleTask.ToString(), ex.ToString()), null, true);
						}
						long currentTicksInexact = TimeUtil.CurrentTicksInexact;
						if (currentTicksInexact - num > 1000L)
						{
							LogManager.WriteLog(2, string.Format("{0}执行时间:{1}毫秒", scheduleTask.ToString(), currentTicksInexact - num), null, true);
						}
					}
				}
				catch
				{
				}
				int millisecondsTimeout = Math.Max(0, (int)Math.Min(TimeUtil.NOW() + 250L - num, 250L));
				Thread.Sleep(millisecondsTimeout);
			}
		}

		public void Add(ScheduleTask task, int periodic)
		{
			lock (this.Mutex)
			{
				this.TaskList.Add(new MyTaskContext
				{
					Periodic = (long)periodic,
					Task = task
				});
			}
		}

		private ReaderWriterLockSlim Mutex = new ReaderWriterLockSlim();

		private Thread WorkThread;

		public List<MyTaskContext> TaskList = new List<MyTaskContext>();

		private int CurrentIndex;
	}
}
