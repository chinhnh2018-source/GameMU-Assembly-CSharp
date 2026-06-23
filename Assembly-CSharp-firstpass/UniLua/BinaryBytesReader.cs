using System;
using System.Text;

namespace UniLua
{
	public class BinaryBytesReader
	{
		public BinaryBytesReader(ILoadInfo loadinfo)
		{
			this.LoadInfo = loadinfo;
			this.SizeOfSizeT = 0;
		}

		public byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				int num = this.LoadInfo.ReadByte();
				if (num == -1)
				{
					throw new UndumpException("truncated");
				}
				array[i] = (byte)num;
			}
			return array;
		}

		public int ReadInt()
		{
			byte[] array = this.ReadBytes(4);
			return BitConverter.ToInt32(array, 0);
		}

		public uint ReadUInt()
		{
			byte[] array = this.ReadBytes(4);
			return BitConverter.ToUInt32(array, 0);
		}

		public int ReadSizeT()
		{
			if (this.SizeOfSizeT <= 0)
			{
				throw new Exception("sizeof(size_t) is not valid:" + this.SizeOfSizeT);
			}
			byte[] array = this.ReadBytes(this.SizeOfSizeT);
			int sizeOfSizeT = this.SizeOfSizeT;
			ulong num;
			if (sizeOfSizeT != 4)
			{
				if (sizeOfSizeT != 8)
				{
					throw new NotImplementedException();
				}
				num = BitConverter.ToUInt64(array, 0);
			}
			else
			{
				num = (ulong)BitConverter.ToUInt32(array, 0);
			}
			if (num > 2147483647UL)
			{
				throw new NotImplementedException();
			}
			return (int)num;
		}

		public double ReadDouble()
		{
			byte[] array = this.ReadBytes(8);
			return BitConverter.ToDouble(array, 0);
		}

		public byte ReadByte()
		{
			int num = this.LoadInfo.ReadByte();
			if (num == -1)
			{
				throw new UndumpException("truncated");
			}
			return (byte)num;
		}

		public string ReadString()
		{
			int num = this.ReadSizeT();
			if (num == 0)
			{
				return null;
			}
			byte[] array = this.ReadBytes(num);
			return Encoding.UTF8.GetString(array, 0, num - 1);
		}

		private ILoadInfo LoadInfo;

		public int SizeOfSizeT;
	}
}
