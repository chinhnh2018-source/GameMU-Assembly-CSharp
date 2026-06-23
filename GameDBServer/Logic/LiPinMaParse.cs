using System;
using System.Security.Cryptography;
using System.Text;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class LiPinMaParse
	{
		private static string GenerateUniqueId()
		{
			long num = 1L;
			foreach (byte b in Guid.NewGuid().ToByteArray())
			{
				num *= (long)(b + 1);
			}
			return string.Format("{0:X2}", num - DateTime.Now.Ticks);
		}

		public static string GenerateLiPinMa(int ptid, int ptrepeat, int zoneID)
		{
			string text = LiPinMaParse.GenerateUniqueId().Substring(0, 12);
			string text2 = string.Format("NZ{0:000}{1:0}{2:000}{3}", new object[]
			{
				ptid,
				ptrepeat,
				zoneID,
				text
			});
			byte[] bytes = new UTF8Encoding().GetBytes(text2);
			CRC32 crc = new CRC32();
			crc.update(bytes);
			uint num = crc.getValue() % 255U;
			string str = string.Format("{0:X}", num);
			return text2 + str;
		}

		public static bool ParseLiPinMa(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			ptid = -1;
			ptrepeat = 0;
			zoneID = 0;
			nMaxUseNum = 1;
			int num = 0;
			if (lipinma.Length > 23)
			{
				num = 2;
			}
			bool result;
			if (lipinma.Length < 22 + num || lipinma.Length > 23 + num)
			{
				result = false;
			}
			else
			{
				lipinma = lipinma.ToUpper();
				if ("NZ" != lipinma.Substring(0, 2))
				{
					result = false;
				}
				else
				{
					string str = lipinma.Substring(21 + num, Math.Min(2, lipinma.Length - (21 + num)));
					int num2 = Global.SafeConvertToInt32(str, 16);
					byte[] bytes = new UTF8Encoding().GetBytes(lipinma.Substring(0, 21 + num));
					CRC32 crc = new CRC32();
					crc.update(bytes);
					uint num3 = crc.getValue() % 255U;
					if (num2 != (int)num3)
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 3));
						ptrepeat = Convert.ToInt32(lipinma.Substring(5, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(6, 3));
						if (num > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(9, 2));
						}
						result = true;
					}
				}
			}
			return result;
		}

		public static bool ParseLiPinMaNX(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			ptid = -1;
			ptrepeat = 0;
			zoneID = 0;
			nMaxUseNum = 1;
			int num = 0;
			if (lipinma.Length > 24)
			{
				num = 2;
			}
			bool result;
			if (lipinma.Length < 23 + num || lipinma.Length > 24 + num)
			{
				result = false;
			}
			else
			{
				lipinma = lipinma.ToUpper();
				if ("NX" != lipinma.Substring(0, 2))
				{
					result = false;
				}
				else
				{
					string str = lipinma.Substring(22 + num, Math.Min(2, lipinma.Length - (22 + num)));
					int num2 = Global.SafeConvertToInt32(str, 16);
					byte[] bytes = new UTF8Encoding().GetBytes(lipinma.Substring(0, 22 + num));
					CRC32 crc = new CRC32();
					crc.update(bytes);
					uint num3 = crc.getValue() % 255U;
					if (num2 != (int)num3)
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 4));
						ptrepeat = Convert.ToInt32(lipinma.Substring(6, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(7, 3));
						if (num > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(10, 2));
						}
						result = true;
					}
				}
			}
			return result;
		}

		public static bool ParseLiPinMa2(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			bool result;
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("lipinma_v1", 0) == 1)
			{
				result = LiPinMaParse.ParseLiPinMa(lipinma, out ptid, out ptrepeat, out zoneID, out nMaxUseNum);
			}
			else
			{
				ptid = -1;
				ptrepeat = 0;
				zoneID = 0;
				nMaxUseNum = 1;
				int num = 0;
				if (lipinma.Length > 23)
				{
					num = 2;
				}
				if (lipinma.Length < 22 + num || lipinma.Length > 23 + num)
				{
					result = false;
				}
				else
				{
					lipinma = lipinma.ToUpper();
					if ("NZ" != lipinma.Substring(0, 2))
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 3));
						ptrepeat = Convert.ToInt32(lipinma.Substring(5, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(6, 3));
						if (num > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(9, 2));
						}
						string text = lipinma.Substring(9 + num);
						MD5 md = MD5.Create();
						byte[] array = new byte[25];
						for (int i = 0; i < 5; i++)
						{
							array[i] = Convert.ToByte(text.Substring(2 * i + 4, 2), 16);
						}
						array[5] = 31;
						array[6] = 22;
						array[7] = 5;
						array[8] = 150;
						Array.Copy(BitConverter.GetBytes(ptid), 0, array, 9, 4);
						Array.Copy(BitConverter.GetBytes(ptrepeat), 0, array, 13, 4);
						Array.Copy(BitConverter.GetBytes(zoneID), 0, array, 17, 4);
						Array.Copy(BitConverter.GetBytes(nMaxUseNum), 0, array, 21, 4);
						byte[] array2 = md.ComputeHash(array);
						for (int i = 0; i < 2; i++)
						{
							if (Convert.ToByte(text.Substring(2 * i, 2), 16) != array2[i])
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		public static bool ParseLiPinMaNX2(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			bool result;
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("lipinma_v1", 0) == 1)
			{
				result = LiPinMaParse.ParseLiPinMaNX(lipinma, out ptid, out ptrepeat, out zoneID, out nMaxUseNum);
			}
			else
			{
				ptid = -1;
				ptrepeat = 0;
				zoneID = 0;
				nMaxUseNum = 1;
				int num = 0;
				if (lipinma.Length > 24)
				{
					num = 2;
				}
				if (lipinma.Length < 23 + num || lipinma.Length > 24 + num)
				{
					result = false;
				}
				else
				{
					lipinma = lipinma.ToUpper();
					if ("NX" != lipinma.Substring(0, 2))
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 4));
						ptrepeat = Convert.ToInt32(lipinma.Substring(6, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(7, 3));
						if (num > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(10, 2));
						}
						string text = lipinma.Substring(10 + num);
						MD5 md = MD5.Create();
						byte[] array = new byte[25];
						for (int i = 0; i < 5; i++)
						{
							array[i] = Convert.ToByte(text.Substring(2 * i + 4, 2), 16);
						}
						array[5] = 31;
						array[6] = 22;
						array[7] = 5;
						array[8] = 150;
						Array.Copy(BitConverter.GetBytes(ptid), 0, array, 9, 4);
						Array.Copy(BitConverter.GetBytes(ptrepeat), 0, array, 13, 4);
						Array.Copy(BitConverter.GetBytes(zoneID), 0, array, 17, 4);
						Array.Copy(BitConverter.GetBytes(nMaxUseNum), 0, array, 21, 4);
						byte[] array2 = md.ComputeHash(array);
						for (int i = 0; i < 2; i++)
						{
							if (Convert.ToByte(text.Substring(2 * i, 2), 16) != array2[i])
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}
	}
}
