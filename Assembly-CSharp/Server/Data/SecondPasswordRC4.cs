using System;
using System.Text;
using Server.Tools;

namespace Server.Data
{
	public static class SecondPasswordRC4
	{
		public static string Encrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return null;
			}
			byte[] bytes = new UTF8Encoding().GetBytes(input);
			RC4Helper.RC4(bytes, SecondPasswordRC4._Key);
			return Convert.ToBase64String(bytes);
		}

		public static string Decrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return null;
			}
			byte[] array = Convert.FromBase64String(input);
			RC4Helper.RC4(array, SecondPasswordRC4._Key);
			return new UTF8Encoding().GetString(array);
		}

		private static string _Key = "SecPwd";
	}
}
