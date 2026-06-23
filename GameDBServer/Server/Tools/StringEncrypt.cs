using System;
using System.Text;

namespace Server.Tools
{
	public class StringEncrypt
	{
		public static string Encrypt(string plainText, string passwd, string saltValue)
		{
			string result;
			if (string.IsNullOrEmpty(plainText))
			{
				result = null;
			}
			else
			{
				byte[] data = null;
				try
				{
					data = new UTF8Encoding().GetBytes(plainText);
				}
				catch (Exception)
				{
					return null;
				}
				byte[] b = null;
				try
				{
					b = AesHelper.AesEncryptBytes(data, passwd, saltValue);
				}
				catch (Exception)
				{
					return null;
				}
				result = DataHelper.Bytes2HexString(b);
			}
			return result;
		}

		public static string Decrypt(string encryptText, string passwd, string saltValue)
		{
			string result;
			if (string.IsNullOrEmpty(encryptText))
			{
				result = null;
			}
			else
			{
				byte[] array = DataHelper.HexString2Bytes(encryptText);
				if (null == array)
				{
					result = null;
				}
				else
				{
					byte[] array2 = null;
					try
					{
						array2 = AesHelper.AesDecryptBytes(array, passwd, saltValue);
					}
					catch (Exception)
					{
						return null;
					}
					string text = null;
					try
					{
						text = new UTF8Encoding().GetString(array2, 0, array2.Length);
					}
					catch (Exception)
					{
						return null;
					}
					result = text;
				}
			}
			return result;
		}
	}
}
