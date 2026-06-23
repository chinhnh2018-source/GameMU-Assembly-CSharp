using System;

namespace TianMa
{
	internal class IcmpTest
	{
		public static void IcmpCallback(string content)
		{
			Console.Write(content);
		}

		public static void IcmpOverCallback(bool ok)
		{
			if (ok)
			{
				Console.Write("=============>success");
			}
			else
			{
				Console.Write("=============>failed");
			}
			Console.WriteLine("...................");
		}

		private static void Main(string[] args)
		{
			TraceUtils.TraceRoute("www.tianmashikong.com", new ICMP_CALLBACK(IcmpTest.IcmpCallback), new ICMP_OVER_CALLBACK(IcmpTest.IcmpOverCallback), 30, 3, 200);
		}
	}
}
