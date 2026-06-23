using System;

namespace TianMa.Tools
{
	internal class xaa6ff34997724cf5
	{
		public static void x00d0f063c43ff573(string xd1d55a56253db2df)
		{
			Console.Write(xd1d55a56253db2df);
		}

		public static void x6eeac16015c680d5(bool x84d59c000a491827)
		{
			if (x84d59c000a491827)
			{
				Console.Write("=============>success");
			}
			else
			{
				Console.Write("=============>failed");
			}
			Console.WriteLine("...................");
		}

		private static void xc447809891322395(string[] xce8d8c7e3c2c2426)
		{
			TraceUtils.TraceRoute("www.tianmashikong.com", new ICMP_CALLBACK(xaa6ff34997724cf5.x00d0f063c43ff573), new ICMP_OVER_CALLBACK(xaa6ff34997724cf5.x6eeac16015c680d5), 30, 3, 200);
		}
	}
}
