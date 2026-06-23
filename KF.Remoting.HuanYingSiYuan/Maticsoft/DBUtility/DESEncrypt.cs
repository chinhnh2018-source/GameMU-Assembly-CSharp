using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Maticsoft.DBUtility
{
	public class DESEncrypt
	{
		public static string Encrypt(string Text)
		{
			return DESEncrypt.Encrypt(Text, "litianping");
		}

		public static string Encrypt(string Text, string sKey)
		{
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(Text);
			descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in memoryStream.ToArray())
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}

		public static string Decrypt(string Text)
		{
			return DESEncrypt.Decrypt(Text, "litianping");
		}

		public static string Decrypt(string Text, string sKey)
		{
			DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
			int num = Text.Length / 2;
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = Convert.ToInt32(Text.Substring(i * 2, 2), 16);
				array[i] = (byte)num2;
			}
			descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			return Encoding.Default.GetString(memoryStream.ToArray());
		}
	}
}
