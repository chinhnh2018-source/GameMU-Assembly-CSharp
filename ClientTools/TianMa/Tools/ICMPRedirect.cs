using System;

namespace TianMa.Tools
{
	public class ICMPRedirect : ICMPMessage
	{
		public ICMPRedirect()
		{
		}

		public ICMPRedirect(ref byte[] Packet)
		{
			try
			{
				this.GatewayInternetAddress = (ulong)((long)BitConverter.ToInt32(Packet, 0));
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
			Buffer.BlockCopy(BitConverter.GetBytes((long)this.GatewayInternetAddress), 0, array, 0, 4);
			Buffer.BlockCopy(this.Data, 0, array, 4, this.Data.Length);
			return array;
		}

		public ulong GatewayInternetAddress;

		public byte[] Data;

		public enum CodeEnum
		{
			RedirectDatagramsForTheNetwork,
			RedirectDatagramsForTheHost,
			RedirectDatagramsForTheTypeOfServiceAndNetwork,
			RedirectDatagramsForTheTypeOfServiceAndHost
		}
	}
}
