using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AutoCSer.Net.TcpStaticServer;
using KF.Remoting;
using Server.Tools;

namespace KF.TcpCall
{
	[Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = 240, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public class TestS2KFCommunication
	{
		[Method]
		public static string SendData(int strLen, bool flag)
		{
			if (TestS2KFCommunication.Flag != flag)
			{
				TestS2KFCommunication.Flag = flag;
				if (TestS2KFCommunication.Flag)
				{
					Console.WriteLine("压测开启");
					object state = strLen;
					TestS2KFCommunication.upDateTimer = new Timer(new TimerCallback(TestS2KFCommunication.UpDateTick), state, 1, 86400000);
					TestS2KFCommunication.CpuData.Start();
					TestS2KFCommunication.UpTickCpuTimer = new Timer(new TimerCallback(TestS2KFCommunication.UpTickCpu), null, 1, 500);
				}
				else
				{
					Console.WriteLine("压测关闭");
					TestS2KFCommunication.upDateTimer.Change(-1, -1);
					TestS2KFCommunication.UpTickCpuTimer.Change(-1, -1);
					TestS2KFCommunication.CpuData.Print();
				}
			}
			char[] array = new char[]
			{
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'J',
				'K',
				'L',
				'M',
				'N',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'W',
				'V',
				'U',
				'X',
				'Y',
				'Z'
			};
			string text = "";
			for (int i = 0; i < strLen; i++)
			{
				text += array[i % array.Length];
			}
			return text;
		}

		private static void UpDateTick(object sender)
		{
			try
			{
				char[] array = new char[]
				{
					'A',
					'B',
					'C',
					'D',
					'E',
					'F',
					'G',
					'H',
					'J',
					'K',
					'L',
					'M',
					'N',
					'P',
					'Q',
					'R',
					'S',
					'T',
					'W',
					'V',
					'U',
					'X',
					'Y',
					'Z'
				};
				string text = "";
				for (int i = 0; i < (int)sender; i++)
				{
					text += array[i % array.Length];
				}
				long num = 0L;
				long num2 = 0L;
				while (TestS2KFCommunication.Flag)
				{
					num += 1L;
					if (num > 50000L)
					{
						break;
					}
					try
					{
						ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New<string>(10041, text), 0);
						num2 += 1L;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						Console.WriteLine("UpDateTick Exception");
					}
				}
				Console.WriteLine(string.Format("发送allnum={0}， sunum={1}", num - 1L, num2));
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		private static void UpTickCpu(object sender)
		{
			try
			{
				TestS2KFCommunication.CpuData.GetValue();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		private static bool Flag = false;

		private static Timer upDateTimer = null;

		private static Timer UpTickCpuTimer = null;

		private static CpuModel CpuData = new CpuModel();

		internal static class TcpStaticServer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static string _M23(int strLen, bool flag)
			{
				return TestS2KFCommunication.SendData(strLen, flag);
			}
		}
	}
}
