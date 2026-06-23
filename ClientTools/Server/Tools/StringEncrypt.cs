using System;
using System.Text;

namespace Server.Tools
{
	public class StringEncrypt
	{
		public static string Encrypt(string plainText, string passwd, string saltValue)
		{
			if (string.IsNullOrEmpty(plainText))
			{
				return null;
			}
			byte[] data = null;
			try
			{
				data = new UTF8Encoding().GetBytes(plainText);
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
				return null;
			}
			byte[] b = null;
			try
			{
				b = AesHelper.AesEncryptBytes(data, passwd, saltValue);
			}
			catch (Exception exception2)
			{
				DebugTextLog.LogException(exception2);
				return null;
			}
			return DataHelper.Bytes2HexString(b);
		}

		public static string Decrypt(string encryptText, string passwd, string saltValue)
		{
			if (string.IsNullOrEmpty(encryptText))
			{
				if (!false)
				{
					goto IL_5F;
				}
			}
			byte[] array = DataHelper.HexString2Bytes(encryptText);
			if (3 != 0)
			{
			}
			if (array == null)
			{
				return null;
			}
			byte[] array2 = null;
			try
			{
				array2 = AesHelper.AesDecryptBytes(array, passwd, saltValue);
				goto IL_0D;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
				return null;
			}
			goto IL_5F;
			IL_0D:
			string result = null;
			try
			{
				result = new UTF8Encoding().GetString(array2, 0, array2.Length);
			}
			catch (Exception exception2)
			{
				DebugTextLog.LogException(exception2);
				return null;
			}
			return result;
			IL_5F:
			return null;
		}
	}
}
