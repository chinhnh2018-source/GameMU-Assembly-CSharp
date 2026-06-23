using System;

namespace UniLua.Tools
{
	public class BytesLoadInfo : ILoadInfo
	{
		public BytesLoadInfo(byte[] bytes)
		{
			this.Bytes = bytes;
			this.Pos = 0;
		}

		public int ReadByte()
		{
			if (this.Pos >= this.Bytes.Length)
			{
				return -1;
			}
			return (int)this.Bytes[this.Pos++];
		}

		public int PeekByte()
		{
			if (this.Pos >= this.Bytes.Length)
			{
				return -1;
			}
			return (int)this.Bytes[this.Pos];
		}

		private byte[] Bytes;

		private int Pos;
	}
}
