using System;
using System.Collections.Generic;

namespace UniLua
{
	internal class ByteStringBuilder
	{
		public ByteStringBuilder()
		{
			this.BufList = new LinkedList<byte[]>();
			this.TotalLength = 0;
		}

		public override string ToString()
		{
			if (this.TotalLength <= 0)
			{
				return string.Empty;
			}
			char[] array = new char[this.TotalLength];
			int num = 0;
			for (LinkedListNode<byte[]> linkedListNode = this.BufList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				byte[] value = linkedListNode.Value;
				for (int i = 0; i < value.Length; i++)
				{
					array[num++] = (char)value[i];
				}
			}
			return new string(array);
		}

		public ByteStringBuilder Append(byte[] bytes, int start, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(bytes, start, array, 0, length);
			this.BufList.AddLast(array);
			this.TotalLength += length;
			return this;
		}

		private LinkedList<byte[]> BufList;

		private int TotalLength;
	}
}
