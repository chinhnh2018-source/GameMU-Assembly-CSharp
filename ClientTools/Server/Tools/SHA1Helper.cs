using System;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
	public class SHA1Helper
	{
		public static string get_sha1_string(string str)
		{
			byte[] b = SHA1Helper.get_sha1_bytes(str);
			return DataHelper.Bytes2HexString(b);
		}

		public static string get_sha1_string(byte[] data)
		{
			byte[] b = SHA1Helper.get_sha1_bytes(data);
			return DataHelper.Bytes2HexString(b);
		}

		public static byte[] get_sha1_bytes(string str)
		{
			SHA1 sha = new SHA1Managed();
			byte[] bytes = new UTF8Encoding().GetBytes(str);
			return sha.ComputeHash(bytes);
		}

		public static byte[] get_sha1_bytes(byte[] data)
		{
			SHA1 sha = new SHA1Managed();
			return sha.ComputeHash(data);
		}

		public static string get_macksha1_string(string str, string key)
		{
			byte[] b = SHA1Helper.get_macsha1_bytes(str, key);
			return DataHelper.Bytes2HexString(b);
		}

		public static string get_macsha1_string(byte[] data, string key)
		{
			byte[] b = SHA1Helper.get_macsha1_bytes(data, key);
			return DataHelper.Bytes2HexString(b);
		}

		public static byte[] get_macsha1_bytes(string str, string key)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(bytes);
			byte[] bytes2 = new UTF8Encoding().GetBytes(str);
			return hmacsha.ComputeHash(bytes2);
		}

		public static byte[] get_macsha1_bytes(byte[] data, string key)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(bytes);
			return hmacsha.ComputeHash(data);
		}
	}
}
