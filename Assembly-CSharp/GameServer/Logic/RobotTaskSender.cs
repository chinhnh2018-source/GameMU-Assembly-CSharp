using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Server.Tools;

namespace GameServer.Logic
{
	internal class RobotTaskSender
	{
		private RobotTaskSender()
		{
		}

		public static RobotTaskSender getInstance()
		{
			return RobotTaskSender.instance;
		}

		public bool Initialize(int seed, int randomCount, string pubKey)
		{
			lock (this)
			{
				try
				{
					this.m_TaskListVerifySeed = seed;
					this.m_TaskListVerifyRandomCount = randomCount;
					this.m_TaskListRSA = new RSACryptoServiceProvider();
					this.m_TaskListRSA.PersistKeyInCsp = false;
					this.m_TaskListRSA.FromXmlString(pubKey);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			return true;
		}

		public byte[] EncryptTaskList(string taskList, bool jailbreak, bool autoStart, string info)
		{
			List<byte> list = new List<byte>();
			lock (this)
			{
				try
				{
					int tickCount = Environment.TickCount;
					if (this.m_TaskListRSA == null)
					{
						return null;
					}
					int num = tickCount % this.m_TaskListVerifyRandomCount;
					int num2 = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(num));
					string text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						taskList,
						tickCount,
						num2,
						(!jailbreak) ? 0 : 1,
						(!autoStart) ? 0 : 1,
						info
					});
					List<byte> list2 = new List<byte>(DataHelper.Compress(new UTF8Encoding().GetBytes(text)));
					int num3 = (list2 != null) ? list2.Count : text.Length;
					for (int i = 0; i < num3; i += 100)
					{
						byte[] array;
						if (list2 == null)
						{
							array = new UTF8Encoding().GetBytes(text.Substring(i, (i + 100 > text.Length) ? (text.Length % 100) : 100));
						}
						else
						{
							array = new byte[(i + 100 > list2.Count) ? (list2.Count % 100) : 100];
							list2.CopyTo(i, array, 0, array.Length);
						}
						byte[] array2 = this.m_TaskListRSA.Encrypt(array, false);
						int num4 = array2.Length;
						list.Add((byte)num4);
						for (int j = 0; j < num4; j++)
						{
							list.Add(array2[j]);
						}
					}
				}
				catch (Exception ex)
				{
					return null;
				}
			}
			return list.ToArray();
		}

		public byte[] EncryptGeniusList(string taskList, bool jailbreak, bool autoStart, string info)
		{
			List<byte> list = new List<byte>();
			lock (this)
			{
				try
				{
					if (this.m_TaskListRSA == null)
					{
						return null;
					}
					int tickCount = Environment.TickCount;
					int num = tickCount % this.m_TaskListVerifyRandomCount;
					int num2 = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(num));
					string text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						taskList,
						tickCount,
						num2,
						(!jailbreak) ? 0 : 1,
						(!autoStart) ? 0 : 1,
						info
					});
					List<byte> list2 = new List<byte>(DataHelper.Compress(new UTF8Encoding().GetBytes(text)));
					int count = list2.Count;
					int num3 = 7;
					this.AddMagicBytes(list, num3);
					for (int i = 0; i < count; i += 100)
					{
						byte[] array = new byte[(i + 100 > list2.Count) ? (list2.Count % 100) : 100];
						list2.CopyTo(i, array, 0, array.Length);
						byte[] array2 = this.m_TaskListRSA.Encrypt(array, false);
						int num4 = array2.Length;
						list.Add((byte)num4);
						list.AddRange(array2);
						num3 = ((int)array[0] + num3 & 15);
					}
					this.AddMagicBytes(list, num3);
				}
				catch (Exception ex)
				{
					return null;
				}
			}
			return list.ToArray();
		}

		private void AddMagicBytes(List<byte> data, int c)
		{
			Random random = new Random(this.m_TaskListVerifySeed);
			int num = random.Next();
			for (int i = 0; i < c; i++)
			{
				num = random.Next();
				data.AddRange(BitConverter.GetBytes(num));
			}
		}

		private uint GenMagicRandom(uint seed, int loop)
		{
			uint num = seed;
			uint num2 = 362436069U;
			for (int i = 0; i <= loop; i++)
			{
				num2 = 36969U * (num2 & 65535U) + (num2 >> 16);
				num = 18000U * (num & 65535U) + (num >> 16);
			}
			return (num2 << 16) + num;
		}

		private static RobotTaskSender instance = new RobotTaskSender();

		private int m_TaskListVerifySeed;

		private int m_TaskListVerifyRandomCount = 50;

		private RSACryptoServiceProvider m_TaskListRSA;
	}
}
