using System;

namespace MoPhoGames.USpeak.Codec
{
	public class MuLawCodec : ICodec
	{
		public byte[] Encode(short[] data, BandMode mode)
		{
			return MuLawEncoder.MuLawEncode(data);
		}

		public short[] Decode(byte[] data, BandMode mode)
		{
			return MuLawDecoder.MuLawDecode(data);
		}
	}
}
