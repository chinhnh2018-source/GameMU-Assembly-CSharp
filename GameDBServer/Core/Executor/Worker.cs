using System;
using System.Threading;
using Server.Tools;

namespace GameDBServer.Core.Executor
{
	internal class Worker
	{
		public Worker(ScheduleExecutor executor)
		{
			this.executor = executor;
		}

		public Thread CurrentThread
		{
			set
			{
				this.currentThread = value;
			}
		}

		private TaskWrapper getCanExecuteTask()
		{
			int num = 0;
			TaskWrapper task;
			while (null != (task = this.executor.getTask()))
			{
				if (TimeUtil.NOW() >= task.StartTime)
				{
					return task;
				}
				if (task.canExecute)
				{
					this.executor.addTask(task);
				}
				num++;
				if (num >= 1000)
				{
					break;
				}
			}
			return null;
		}

		public void work()
		{
			TaskWrapper taskWrapper = null;
			for (;;)
			{
				taskWrapper = this.getCanExecuteTask();
				if (null == taskWrapper)
				{
					try
					{
						Thread.Sleep(5);
					}
					catch (ThreadInterruptedException)
					{
					}
				}
				else
				{
					try
					{
						if (taskWrapper != null && null != taskWrapper.CurrentTask)
						{
							if (taskWrapper.canExecute)
							{
								try
								{
									taskWrapper.CurrentTask.run();
								}
								catch (Exception ex)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("异步调度任务执行错误: {0}", ex), null, true);
								}
							}
							if (taskWrapper.Periodic > 0L && taskWrapper.canExecute)
							{
								taskWrapper.resetStartTime();
								this.executor.addTask(taskWrapper);
								taskWrapper.addExecuteCount();
							}
						}
					}
					catch (Exception ex)
					{
						DataHelper.WriteFormatExceptionLog(ex, "异步调度任务执行异常", false, false);
					}
				}
			}
		}

		private ScheduleExecutor executor = null;

		private Thread currentThread = null;
	}
}
