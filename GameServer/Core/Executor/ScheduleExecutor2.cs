using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Logic;
using Server.Tools;

namespace GameServer.Core.Executor
{
	public class ScheduleExecutor2
	{
		public void start()
		{
		}

		public void stop()
		{
			lock (this)
			{
				foreach (KeyValuePair<ScheduleTask, Timer> keyValuePair in this.TimerDict)
				{
					keyValuePair.Value.Dispose();
				}
				this.TimerDict.Clear();
			}
		}

		public void scheduleCancle(ScheduleTask task)
		{
			lock (this)
			{
				Timer timer;
				if (this.TimerDict.TryGetValue(task, out timer))
				{
					timer.Dispose();
					this.TimerDict.Remove(task);
				}
			}
		}

		public void scheduleExecute(ScheduleTask task, int group, int periodic)
		{
			if (periodic < 15 || periodic > 86400000)
			{
				throw new Exception("不正确的调度时间间隔periodic = " + periodic);
			}
			lock (this)
			{
				if (periodic < 250)
				{
					group = 0;
				}
				if (group == 0)
				{
					Timer timer;
					if (!this.TimerDict.TryGetValue(task, out timer))
					{
						timer = new Timer(new TimerCallback(ScheduleExecutor2.OnTimedEvent), task, Global.GetRandomNumber(periodic / 2, periodic * 3 / 2), periodic);
						this.TimerDict.Add(task, timer);
					}
					else
					{
						timer.Change(periodic, periodic);
					}
				}
				else
				{
					ExecContext execContext;
					if (!this.ThreadDict.TryGetValue(group, out execContext))
					{
						execContext = new ExecContext();
						this.ThreadDict[group] = execContext;
					}
					execContext.Add(task, periodic);
				}
			}
		}

		private static void OnTimedEvent(object source)
		{
			ScheduleTask scheduleTask = source as ScheduleTask;
			if (scheduleTask.InternalLock.TryEnter())
			{
				bool flag = false;
				long currentTicksInexact = TimeUtil.CurrentTicksInexact;
				try
				{
					scheduleTask.run();
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("{0}执行时异常,{1}", scheduleTask.ToString(), ex.ToString()), null, true);
				}
				finally
				{
					flag = scheduleTask.InternalLock.Leave();
				}
				if (flag)
				{
					long currentTicksInexact2 = TimeUtil.CurrentTicksInexact;
					if (currentTicksInexact2 - currentTicksInexact > 1000L)
					{
						try
						{
							MonsterTask monsterTask = scheduleTask as MonsterTask;
							if (null != monsterTask)
							{
								LogManager.WriteLog(2, string.Format("{0} mapCode:{1},subMapCode:{2},执行时间:{3}毫秒", new object[]
								{
									scheduleTask.ToString(),
									monsterTask.mapCode,
									monsterTask.subMapCode,
									currentTicksInexact2 - currentTicksInexact
								}), null, true);
							}
							else
							{
								LogManager.WriteLog(2, string.Format("{0}执行时间:{1}毫秒", scheduleTask.ToString(), currentTicksInexact2 - currentTicksInexact), null, true);
							}
						}
						catch
						{
						}
					}
				}
			}
		}

		public static ScheduleExecutor2 Instance = new ScheduleExecutor2();

		private Dictionary<ScheduleTask, Timer> TimerDict = new Dictionary<ScheduleTask, Timer>();

		private Dictionary<int, ExecContext> ThreadDict = new Dictionary<int, ExecContext>();
	}
}
