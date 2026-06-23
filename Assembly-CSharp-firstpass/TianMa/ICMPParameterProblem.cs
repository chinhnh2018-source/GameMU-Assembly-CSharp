using System;

namespace TianMa
{
	public class ICMPParameterProblem : ICMPMessage
	{
		public ICMPParameterProblem()
		{
		}

		public ICMPParameterProblem(ref byte[] Packet)
		{
			try
			{
				this.Pointer = Packet[0];
				this.Data = new byte[Packet.Length - 4];
				Buffer.BlockCopy(Packet, 0, this.Data, 4, Packet.Length);
			}
			catch
			{
			}
		}

		public override byte[] GetBytes()
		{
			if (this.Data == null)
			{
				this.Data = new byte[0];
			}
			byte[] array = new byte[4 + this.Data.Length];
			array[0] = this.Pointer;
			Buffer.BlockCopy(this.Data, 0, array, 4, this.Data.Length);
			return array;
		}

		public byte Pointer;

		public byte[] Data;
	}
}
