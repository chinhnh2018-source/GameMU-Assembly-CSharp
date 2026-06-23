using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nhiredis;

namespace GameDBServer.Logic
{
	public static class CityNameManager
	{
		public static string unicode_js_1(string str)
		{
			Regex regex = new Regex("(?i)\\\\u([0-9a-f]{4})");
			return regex.Replace(str, (Match m1) => ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString());
		}

		private static void ParseIPInfo(string text, out string region, out string cityName)
		{
			region = "";
			cityName = "";
			if (!string.IsNullOrEmpty(text))
			{
				int num = text.IndexOf("\"region\":\"");
				if (num >= 0)
				{
					int num2 = text.IndexOf("\",", num + 10);
					if (num2 >= num)
					{
						region = text.Substring(num + 10, num2 - num - 10);
						num = text.IndexOf("\"city\":\"");
						if (num >= 0)
						{
							num2 = text.IndexOf("\",", num + 8);
							if (num2 >= num)
							{
								cityName = text.Substring(num + 8, num2 - num - 8);
							}
						}
					}
				}
			}
		}

		public static IPInfo ParseIP(string ip)
		{
			string text = null;
			string text2 = null;
			IPInfo ipinfo = null;
			if (IpLibrary.findIpAddrInfo(ip, ref text, ref text2))
			{
				ipinfo = new IPInfo();
				ipinfo.RegionName = text.Substring(0, Math.Min(20, text.Length));
				ipinfo.CityName = text2.Substring(0, Math.Min(20, text2.Length));
			}
			return ipinfo;
		}

		public static Dictionary<string, IPInfo> CachingIPInfoDict = new Dictionary<string, IPInfo>();
	}
}
