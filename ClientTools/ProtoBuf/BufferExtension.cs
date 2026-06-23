using System;
using System.IO;

namespace ProtoBuf
{
	public sealed class BufferExtension : IExtension
	{
		int IExtension.GetLength()
		{
			if (this.x5cafa8d49ea71ea1 != null)
			{
				return this.x5cafa8d49ea71ea1.Length;
			}
			return 0;
		}

		Stream IExtension.BeginAppend()
		{
			return new MemoryStream();
		}

		void IExtension.EndAppend(Stream stream, bool commit)
		{
			try
			{
				if (commit)
				{
					if (false)
					{
						goto IL_8A;
					}
					int num;
					if ((num = (int)stream.Length) > 0)
					{
						goto IL_8A;
					}
					IL_26:
					if ((uint)num - (uint)num >= 0U)
					{
						goto IL_BB;
					}
					goto IL_91;
					IL_8A:
					MemoryStream memoryStream = (MemoryStream)stream;
					IL_91:
					if (this.x5cafa8d49ea71ea1 == null)
					{
						this.x5cafa8d49ea71ea1 = memoryStream.ToArray();
					}
					else
					{
						int num2 = this.x5cafa8d49ea71ea1.Length;
						byte[] x3ed4f4f0195b98d = new byte[num2 + num];
						x479f2661aae93792.x6a87193e5bb23362(this.x5cafa8d49ea71ea1, 0, x3ed4f4f0195b98d, 0, num2);
						if (false)
						{
							goto IL_8A;
						}
						x479f2661aae93792.x6a87193e5bb23362(memoryStream.GetBuffer(), 0, x3ed4f4f0195b98d, num2, num);
						if ((uint)num2 < 0U)
						{
							goto IL_26;
						}
						this.x5cafa8d49ea71ea1 = x3ed4f4f0195b98d;
					}
				}
				IL_BB:;
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
		}

		Stream IExtension.BeginQuery()
		{
			if (this.x5cafa8d49ea71ea1 != null)
			{
				return new MemoryStream(this.x5cafa8d49ea71ea1);
			}
			return Stream.Null;
		}

		void IExtension.EndQuery(Stream stream)
		{
			try
			{
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
		}

		private byte[] x5cafa8d49ea71ea1;
	}
}
