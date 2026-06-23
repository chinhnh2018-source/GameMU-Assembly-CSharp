using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UniLua
{
	public class FileLoadInfo : IDisposable, ILoadInfo
	{
		public FileLoadInfo(FileStream stream)
		{
			this.Stream = stream;
			this.Reader = new StreamReader(this.Stream, Encoding.UTF8);
			this.Buf = new Queue<char>();
		}

		public int ReadByte()
		{
			if (this.Buf.Count > 0)
			{
				return (int)this.Buf.Dequeue();
			}
			return this.Reader.Read();
		}

		public int PeekByte()
		{
			if (this.Buf.Count > 0)
			{
				return (int)this.Buf.Peek();
			}
			int num = this.Reader.Read();
			if (num == -1)
			{
				return num;
			}
			this.Save((char)num);
			return num;
		}

		public void Dispose()
		{
			this.Reader.Dispose();
			this.Stream.Dispose();
		}

		private void Save(char b)
		{
			this.Buf.Enqueue(b);
		}

		private void Clear()
		{
			this.Buf.Clear();
		}

		public void SkipComment()
		{
			int num = this.Reader.Read();
			if (num == 35)
			{
				do
				{
					num = this.Reader.Read();
				}
				while (num != -1 && num != 10);
				this.Save('\n');
			}
			else if (num != -1)
			{
				this.Save((char)num);
			}
		}

		private const string UTF8_BOM = "ï»¿";

		private FileStream Stream;

		private StreamReader Reader;

		private Queue<char> Buf;
	}
}
