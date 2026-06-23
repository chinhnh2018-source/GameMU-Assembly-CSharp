using System;
using System.Threading;

namespace ProtoBuf
{
	internal class xe2887184ca5e62c2
	{
		internal static void xbb7550bbb62a218c()
		{
			for (int i = 0; i < xe2887184ca5e62c2.xb3a098bf0147fd2c.Length; i++)
			{
				Interlocked.Exchange(ref xe2887184ca5e62c2.xb3a098bf0147fd2c[i], null);
			}
		}

		private xe2887184ca5e62c2()
		{
		}

		internal static byte[] x7145b88c884afa7f()
		{
			for (int i = 0; i < xe2887184ca5e62c2.xb3a098bf0147fd2c.Length; i++)
			{
				object obj;
				if ((obj = Interlocked.Exchange(ref xe2887184ca5e62c2.xb3a098bf0147fd2c[i], null)) != null)
				{
					return (byte[])obj;
				}
			}
			return new byte[1024];
		}

		internal static void x6e1b6cb1ff4c4538(ref byte[] x5cafa8d49ea71ea1, int x1e6cfa7c0845758b, int xfac380a6fc8f46f8, int xf3c4a11a8fa4e1cb)
		{
			int num = x5cafa8d49ea71ea1.Length * 2;
			bool flag = (uint)num - (uint)xf3c4a11a8fa4e1cb < 0U;
			if (!flag)
			{
				if (num < x1e6cfa7c0845758b)
				{
					num = x1e6cfa7c0845758b;
					goto IL_53;
				}
				if (-2 != 0)
				{
					goto IL_60;
				}
				goto IL_3A;
			}
			IL_1F:
			if (x5cafa8d49ea71ea1.Length != 1024)
			{
				if (-2 == 0)
				{
					goto IL_53;
				}
			}
			else
			{
				xe2887184ca5e62c2.xb8b95ec3e3f43efa(ref x5cafa8d49ea71ea1);
			}
			byte[] array;
			x5cafa8d49ea71ea1 = array;
			return;
			IL_3A:
			x479f2661aae93792.x6a87193e5bb23362(x5cafa8d49ea71ea1, xfac380a6fc8f46f8, array, 0, xf3c4a11a8fa4e1cb);
			goto IL_1F;
			IL_53:
			IL_60:
			array = new byte[num];
			if (xf3c4a11a8fa4e1cb > 0)
			{
				goto IL_3A;
			}
			goto IL_1F;
		}

		internal static void xb8b95ec3e3f43efa(ref byte[] x5cafa8d49ea71ea1)
		{
			if (x5cafa8d49ea71ea1 == null)
			{
				return;
			}
			if (x5cafa8d49ea71ea1.Length == 1024)
			{
				for (;;)
				{
					IL_76:
					int num = 0;
					if (2 != 0)
					{
						goto IL_0E;
					}
					IL_18:
					while (Interlocked.CompareExchange(ref xe2887184ca5e62c2.xb3a098bf0147fd2c[num], x5cafa8d49ea71ea1, null) != null)
					{
						for (;;)
						{
							num++;
							if (!false)
							{
								goto IL_0E;
							}
							if ((uint)num - (uint)num >= 0U)
							{
								break;
							}
							if (-2147483648 != 0)
							{
								goto IL_18;
							}
						}
						if (4 != 0)
						{
							goto IL_2F;
						}
						goto IL_76;
					}
					break;
					IL_0C:
					goto IL_18;
					IL_2F:
					if (!false)
					{
						break;
					}
					goto IL_0C;
					IL_0E:
					if (num < xe2887184ca5e62c2.xb3a098bf0147fd2c.Length)
					{
						goto IL_18;
					}
					goto IL_2F;
				}
			}
			x5cafa8d49ea71ea1 = null;
		}

		private const int x608e483b50809af2 = 20;

		internal const int x59c55b28f6fe563f = 1024;

		private static readonly object[] xb3a098bf0147fd2c = new object[20];
	}
}
