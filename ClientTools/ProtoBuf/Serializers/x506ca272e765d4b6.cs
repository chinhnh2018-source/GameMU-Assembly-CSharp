using System;
using System.Text;

namespace ProtoBuf.Serializers
{
	internal class x506ca272e765d4b6
	{
		public static string x246b032720dd4c0d(string x6a514aff7c442e15, string xe68ce49a71493527, string x17c692cbb93a58f0)
		{
			if (string.IsNullOrEmpty(x6a514aff7c442e15))
			{
				return null;
			}
			byte[] x4a3f0a05c02f235f = null;
			try
			{
				x4a3f0a05c02f235f = new UTF8Encoding().GetBytes(x6a514aff7c442e15);
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
				return null;
			}
			byte[] xe7ebe10fa44d8d = null;
			try
			{
				xe7ebe10fa44d8d = x1c7a1400f16dde0d.x580d20b38d24a40d(x4a3f0a05c02f235f, xe68ce49a71493527, x17c692cbb93a58f0);
			}
			catch (Exception exception2)
			{
				DebugTextLog.LogException(exception2);
				return null;
			}
			return x268d0b7d0d40d0be.x979ed402aff3b49f(xe7ebe10fa44d8d);
		}

		public static string xcc381ffa3ede662f(string x643e2d96b1dad632, string xe68ce49a71493527, string x17c692cbb93a58f0)
		{
			string result;
			if (!string.IsNullOrEmpty(x643e2d96b1dad632))
			{
				byte[] array = x268d0b7d0d40d0be.xb5c2672463bc6ff8(x643e2d96b1dad632);
				if (false)
				{
					if (-2147483648 == 0)
					{
						goto IL_6D;
					}
				}
				else if (array != null)
				{
					byte[] array2 = null;
					try
					{
						array2 = x1c7a1400f16dde0d.x1c1890cb56b3be61(array, xe68ce49a71493527, x17c692cbb93a58f0);
					}
					catch (Exception exception)
					{
						DebugTextLog.LogException(exception);
						return null;
					}
					result = null;
					try
					{
						return new UTF8Encoding().GetString(array2, 0, array2.Length);
					}
					catch (Exception exception2)
					{
						DebugTextLog.LogException(exception2);
						return null;
					}
					goto IL_4E;
				}
				return null;
			}
			if (!false && 15 == 0)
			{
				goto IL_6D;
			}
			IL_4E:
			return null;
			IL_6D:
			if (255 != 0)
			{
				return result;
			}
			string result2;
			return result2;
		}
	}
}
