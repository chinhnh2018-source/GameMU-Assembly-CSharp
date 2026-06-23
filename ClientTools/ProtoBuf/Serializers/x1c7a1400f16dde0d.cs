using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProtoBuf.Serializers
{
	internal class x1c7a1400f16dde0d
	{
		public static byte[] x580d20b38d24a40d(byte[] x4a3f0a05c02f235f, string xe68ce49a71493527, string x17c692cbb93a58f0)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(x17c692cbb93a58f0);
			AesManaged aesManaged;
			Rfc2898DeriveBytes rfc2898DeriveBytes;
			do
			{
				aesManaged = new AesManaged();
				if (2 != 0)
				{
				}
				rfc2898DeriveBytes = new Rfc2898DeriveBytes(xe68ce49a71493527, bytes);
				aesManaged.BlockSize = aesManaged.LegalBlockSizes[0].MaxSize;
				aesManaged.KeySize = aesManaged.LegalKeySizes[0].MaxSize;
				if (2147483647 == 0)
				{
					goto IL_B9;
				}
			}
			while (-2 == 0);
			aesManaged.Key = rfc2898DeriveBytes.GetBytes(aesManaged.KeySize / 8);
			aesManaged.IV = rfc2898DeriveBytes.GetBytes(aesManaged.BlockSize / 8);
			ICryptoTransform transform = aesManaged.CreateEncryptor();
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			cryptoStream.Write(x4a3f0a05c02f235f, 0, x4a3f0a05c02f235f.Length);
			IL_B9:
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		public static byte[] x1c1890cb56b3be61(byte[] x945df3daf47d21fc, string xe68ce49a71493527, string x17c692cbb93a58f0)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(x17c692cbb93a58f0);
			AesManaged aesManaged = new AesManaged();
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(xe68ce49a71493527, bytes);
			if (-1 != 0)
			{
			}
			aesManaged.BlockSize = aesManaged.LegalBlockSizes[0].MaxSize;
			aesManaged.KeySize = aesManaged.LegalKeySizes[0].MaxSize;
			aesManaged.Key = rfc2898DeriveBytes.GetBytes(aesManaged.KeySize / 8);
			aesManaged.IV = rfc2898DeriveBytes.GetBytes(aesManaged.BlockSize / 8);
			ICryptoTransform transform = aesManaged.CreateDecryptor();
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			try
			{
				cryptoStream.Write(x945df3daf47d21fc, 0, x945df3daf47d21fc.Length);
				cryptoStream.Close();
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
				return null;
			}
			return memoryStream.ToArray();
		}
	}
}
