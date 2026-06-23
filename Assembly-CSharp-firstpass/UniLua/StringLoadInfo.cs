using System;

namespace UniLua
{
	internal class StringLoadInfo : ILoadInfo
	{
		public StringLoadInfo(string s)
		{
			this.Str = s;
			this.Pos = 0;
		}

		public int ReadByte()
		{
			if (this.Pos >= this.Str.Length)
			{
				return -1;
			}
			return (int)this.Str.get_Chars(this.Pos++);
		}

		public int PeekByte()
		{
			if (this.Pos >= this.Str.Length)
			{
				return -1;
			}
			return (int)this.Str.get_Chars(this.Pos);
		}

		private string Str;

		private int Pos;
	}
}
