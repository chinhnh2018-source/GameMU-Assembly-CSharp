using System;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Tools;

namespace Server.Protocol
{
	internal class UserLoginToken
	{
		public string UserID { get; set; }

		public int RandomPwd { get; set; }

		public byte[] GetEncryptBytes(string keySHA1, string keyData)
		{
			string s = string.Format("U:{0}:{1}:{2}:T", this.UserID, this.RandomPwd, TimeUtil.NowRealTime() * 10000L);
			byte[] bytes = new UTF8Encoding().GetBytes(s);
			byte[] array = SHA1Helper.get_macsha1_bytes(bytes, keySHA1);
			byte[] array2 = new byte[array.Length + bytes.Length];
			DataHelper.CopyBytes(array2, 0, array, 0, array.Length);
			DataHelper.CopyBytes(array2, array.Length, bytes, 0, bytes.Length);
			RC4Helper.RC4(array2, keyData);
			return array2;
		}

		public int SetEncryptBytes(byte[] buffer, string keySHA1, string keyData, long maxTicks)
		{
			RC4Helper.RC4(buffer, keyData);
			byte[] array = new byte[20];
			DataHelper.CopyBytes(array, 0, buffer, 0, array.Length);
			byte[] array2 = new byte[buffer.Length - 20];
			DataHelper.CopyBytes(array2, 0, buffer, 20, array2.Length);
			byte[] left = SHA1Helper.get_macsha1_bytes(array2, keySHA1);
			int result;
			if (!DataHelper.CompBytes(left, array))
			{
				result = -1;
			}
			else if (array2[0] == 85)
			{
				string @string = new UTF8Encoding().GetString(array2);
				string[] array3 = @string.Split(new char[]
				{
					':'
				});
				if (array3.Length != 5)
				{
					result = -2;
				}
				else if (array3[0] != "U" || array3[4] != "T")
				{
					result = -3;
				}
				else
				{
					long num = (long)Convert.ToUInt64(array3[3]);
					if (TimeUtil.NowRealTime() * 10000L - num >= maxTicks && GameManager.GM_NoCheckTokenTimeRemainMS <= 0L)
					{
						result = -4;
					}
					else
					{
						this.UserID = array3[1];
						this.RandomPwd = (int)Convert.ToUInt32(array3[2]);
						result = 0;
					}
				}
			}
			else
			{
				result = -3;
			}
			return result;
		}

		public string GetEncryptString(string keySHA1, string keyData)
		{
			byte[] encryptBytes = this.GetEncryptBytes(keySHA1, keyData);
			return Convert.ToBase64String(encryptBytes);
		}

		public int SetEncryptString(string s, string keySHA1, string keyData, long maxTicks)
		{
			byte[] array = null;
			try
			{
				array = Convert.FromBase64String(s);
			}
			catch (FormatException)
			{
				return -1000;
			}
			int result;
			if (null == array)
			{
				result = -1001;
			}
			else
			{
				if (array.Length < 20)
				{
					LogManager.WriteLog(2, string.Format("SetEncryptString输入数据长度不足#data={0}", s), null, true);
				}
				result = this.SetEncryptBytes(array, keySHA1, keyData, maxTicks);
			}
			return result;
		}

		public const byte NormalTokenHeader = 85;
	}
}
