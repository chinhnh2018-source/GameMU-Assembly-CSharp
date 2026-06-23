using System;
using System.Collections.Generic;
using KF.TcpCall;
using Server.Tools;

namespace GameServer.Tools
{
	public class S2KFCommunication
	{
		public static void start(string cmd = null)
		{
			try
			{
				if (S2KFCommunication.stage)
				{
					Console.WriteLine("已经开启过了");
				}
				S2KFCommunication.endNum = 0;
				Console.WriteLine("输入1.开启线程数 2,中心返回消息长度, 3.时间间隔毫秒(0 运行5s自动结束)  例子：/“5,1024,10/”");
				string[] array = Console.ReadLine().Split(new char[]
				{
					','
				});
				int num = Convert.ToInt32(array[0].Trim());
				int num2 = Convert.ToInt32(array[2].Trim());
				ThreadTimerModel.MsgLen = Convert.ToInt32(array[1].Trim());
				S2KFCommunication.objList = new List<ThreadTimerModel>(num);
				for (int i = 0; i < num; i++)
				{
					S2KFCommunication.objList.Add(new ThreadTimerModel(i));
				}
				int runTime = 0;
				if (num2 < 1)
				{
					Console.WriteLine("输入运行秒数");
					runTime = Convert.ToInt32(Console.ReadLine());
				}
				foreach (ThreadTimerModel threadTimerModel in S2KFCommunication.objList)
				{
					threadTimerModel.Start(num2, runTime);
				}
				TestReceiveModel.getInstance().start();
				S2KFCommunication.stage = true;
				Console.WriteLine("压测开启");
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("开启失败 Exception");
			}
		}

		public static void SetEnd()
		{
			S2KFCommunication.endNum++;
			if (S2KFCommunication.endNum == S2KFCommunication.objList.Count)
			{
				S2KFCommunication.stop(null);
			}
		}

		public static void stop(string cmd = null)
		{
			try
			{
				foreach (ThreadTimerModel threadTimerModel in S2KFCommunication.objList)
				{
					threadTimerModel.Stop();
				}
				TestReceiveModel.getInstance().stop();
				while (!TcpCall.TestS2KFCommunication.SendData(1, false).IsReturn)
				{
				}
				S2KFCommunication.stage = false;
				Console.WriteLine("压测关闭");
				double num = 0.0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				foreach (ThreadTimerModel threadTimerModel in S2KFCommunication.objList)
				{
					threadTimerModel.print();
					num3 += threadTimerModel.sendSuNum;
					num4 += threadTimerModel.sendErrNum;
					num += (threadTimerModel.endTime - threadTimerModel.startTime).TotalMilliseconds;
				}
				string value = string.Format("统计,发送失败={1},发送成功={2}, Time={4},平均={5}", new object[]
				{
					0,
					num4,
					num3,
					num2,
					num / (double)S2KFCommunication.objList.Count,
					num / (double)S2KFCommunication.objList.Count / (double)(num3 + num4)
				});
				Console.ForegroundColor = ConsoleColor.Red;
				SysConOut.WriteLine(value);
				Console.ForegroundColor = ConsoleColor.White;
				TestReceiveModel.getInstance().print();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("关闭失败 Exception");
			}
		}

		private static int endNum = 0;

		private static bool stage = false;

		private static List<ThreadTimerModel> objList;
	}
}
