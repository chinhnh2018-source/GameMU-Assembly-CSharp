using System;
using System.Threading;
using AutoCSer.Net.TcpServer;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	public class ThreadTimerModel
	{
		public void print()
		{
			string value = string.Format("ID={0},发送失败={1},成功={2}, Time={5},平均={6}", new object[]
			{
				this.threadID,
				this.sendErrNum,
				this.sendSuNum,
				0,
				0,
				(this.endTime - this.startTime).TotalMilliseconds,
				Math.Truncate(100.0 * (this.endTime - this.startTime).TotalMilliseconds / (double)(this.sendSuNum + this.sendErrNum)) / 100.0
			});
			Console.WriteLine(value);
		}

		public ThreadTimerModel(int ID)
		{
			this.threadID = ID;
		}

		public void Start(int upTime, int runTime = 0)
		{
			this.startTime = DateTime.Now.AddMilliseconds(-1.0);
			if (upTime == 0)
			{
				this.RunTime = runTime;
				this.upDateTimer = new Timer(new TimerCallback(ThreadTimerModel.ZeroUpDateTick), this, 1, 3600000);
			}
			else
			{
				this.upDateTimer = new Timer(new TimerCallback(ThreadTimerModel.UpDateTick), this, 1, upTime);
			}
		}

		public void Stop()
		{
			this.endTime = DateTime.Now;
			this.upDateTimer.Change(-1, -1);
		}

		private static void UpDateTick(object sender)
		{
			try
			{
				ReturnValue<string> returnValue = TcpCall.TestS2KFCommunication.SendData(ThreadTimerModel.MsgLen, true);
				ThreadTimerModel threadTimerModel = sender as ThreadTimerModel;
				if (!returnValue.IsReturn)
				{
					threadTimerModel.sendErrNum++;
				}
				else
				{
					threadTimerModel.sendSuNum++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		private static void ZeroUpDateTick(object sender)
		{
			try
			{
				ThreadTimerModel threadTimerModel = sender as ThreadTimerModel;
				threadTimerModel.startTime = DateTime.Now;
				while ((threadTimerModel.endTime - threadTimerModel.startTime).TotalMilliseconds < (double)(1000 * threadTimerModel.RunTime))
				{
					threadTimerModel.endTime = DateTime.Now;
					try
					{
						if (!TcpCall.TestS2KFCommunication.SendData(ThreadTimerModel.MsgLen, true).IsReturn)
						{
							threadTimerModel.sendErrNum++;
						}
						else
						{
							threadTimerModel.sendSuNum++;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						Console.WriteLine("UpDateTick Exception");
					}
				}
				S2KFCommunication.SetEnd();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		public static int MsgLen;

		private int threadID;

		public DateTime endTime;

		public DateTime startTime;

		public Timer upDateTimer = null;

		public int sendSuNum = 0;

		public int sendErrNum = 0;

		private int RunTime = 0;
	}
}
