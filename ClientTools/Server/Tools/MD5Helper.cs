using System;

namespace Server.Tools
{
	public class MD5Helper
	{
		public static string get_md5_string(string str)
		{
			byte[] hash = MD5Core.GetHash(str);
			return DataHelper.Bytes2HexString(hash);
		}

		public static string get_md5_string(byte[] data)
		{
			byte[] hash = MD5Core.GetHash(data);
			return DataHelper.Bytes2HexString(hash);
		}

		public static byte[] get_md5_bytes(string str)
		{
			return MD5Core.GetHash(str);
		}

		public static byte[] get_md5_bytes(byte[] data)
		{
			return MD5Core.GetHash(data);
		}
	}
}
