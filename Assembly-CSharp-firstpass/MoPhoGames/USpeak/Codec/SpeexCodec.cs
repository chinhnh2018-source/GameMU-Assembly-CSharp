using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSpeex;

namespace MoPhoGames.USpeak.Codec
{
	public class SpeexCodec : ICodec
	{
		private int next_multiple(int num, int multiple)
		{
			int num2 = multiple + num;
			return num2 - num2 % multiple;
		}

		private byte[] encode(short[] rawData, BandMode mode)
		{
			rawData = this.ExpandShortArray(rawData);
			if (mode == BandMode.Wide)
			{
				rawData = this.PadShortArray(rawData, 223);
			}
			else
			{
				rawData = this.PadShortArray(rawData, 80);
			}
			SpeexEncoder speexEncoder = this.m_wide_enc;
			if (mode == BandMode.Narrow)
			{
				speexEncoder = this.m_narrow_enc;
			}
			speexEncoder.Quality = 1;
			speexEncoder.VBR = true;
			int i = rawData.Length;
			int frameSize = speexEncoder.FrameSize;
			int num = frameSize / 2;
			for (i = frameSize * ((i + num) / frameSize); i < rawData.Length; i += speexEncoder.FrameSize * 2)
			{
			}
			int num2 = i - rawData.Length;
			short[] array = new short[i];
			Array.Copy(rawData, 0, array, 0, rawData.Length);
			byte[] array2 = new byte[rawData.Length * 2];
			byte[] result;
			try
			{
				int num3 = speexEncoder.Encode(array, 0, array.Length, array2, 0, array2.Length);
				byte[] array3 = new byte[num3 + 8];
				Array.Copy(array2, 0, array3, 8, num3);
				byte[] bytes = BitConverter.GetBytes(rawData.Length);
				byte[] bytes2 = BitConverter.GetBytes(num2);
				Array.Copy(bytes, 0, array3, 0, 4);
				Array.Copy(bytes2, 0, array3, 4, 4);
				result = array3;
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
				throw ex;
			}
			return result;
		}

		private short[] decode(byte[] rawData, BandMode mode)
		{
			SpeexDecoder speexDecoder = this.m_wide_dec;
			if (mode == BandMode.Narrow)
			{
				speexDecoder = this.m_narrow_dec;
			}
			int num = BitConverter.ToInt32(rawData, 0);
			short[] array = new short[num * 2];
			byte[] array2 = new byte[rawData.Length - 8];
			Array.Copy(rawData, 8, array2, 0, array2.Length);
			int num2 = speexDecoder.Decode(array2, 0, array2.Length, array, 0, false);
			if (num2 != 0)
			{
				Array.Resize<short>(ref array, num2);
				int num3 = 446;
				if (mode == BandMode.Narrow)
				{
					num3 = 160;
				}
				short[] array3 = new short[array.Length - num3];
				Array.Copy(array, num3, array3, 0, array3.Length);
				return this.ContractShortArray(array3);
			}
			return new short[1];
		}

		private short[] PadShortArray(short[] src_array, int pad)
		{
			short[] array = new short[src_array.Length + pad];
			Array.Copy(src_array, 0, array, pad, src_array.Length);
			return array;
		}

		private short[] ExpandShortArray(short[] src_array)
		{
			short[] array = new short[src_array.Length * 2];
			for (int i = 0; i < src_array.Length; i++)
			{
				array[i * 2] = src_array[i];
				array[i * 2 + 1] = src_array[i];
			}
			return array;
		}

		private short[] ContractShortArray(short[] src_array)
		{
			short[] array = new short[src_array.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = src_array[i * 2];
			}
			return array;
		}

		private byte[] EncodeAudio(short[] rawData)
		{
			SpeexEncoder speexEncoder = new SpeexEncoder(1);
			speexEncoder.Quality = 1;
			List<byte[]> list = new List<byte[]>();
			int num = rawData.Length;
			num += num % speexEncoder.FrameSize;
			short[] array = new short[num];
			Array.Copy(rawData, array, rawData.Length);
			int frameSize = speexEncoder.FrameSize;
			byte[] array2 = new byte[1024];
			int num2 = 0;
			while (num2 + frameSize < num)
			{
				int num3 = speexEncoder.Encode(array, num2, frameSize, array2, 0, frameSize);
				byte[] array3 = new byte[num3];
				Array.Copy(array2, 0, array3, 0, num3);
				list.Add(array3);
				num2 += frameSize;
			}
			MemoryStream memoryStream = new MemoryStream();
			byte[] bytes = BitConverter.GetBytes(list.Count);
			memoryStream.Write(bytes, 0, bytes.Length);
			for (int i = 0; i < list.Count; i++)
			{
				byte[] array4 = list[i];
				byte[] bytes2 = BitConverter.GetBytes(array4.Length);
				memoryStream.Write(bytes2, 0, bytes2.Length);
			}
			for (int j = 0; j < list.Count; j++)
			{
				byte[] array5 = list[j];
				memoryStream.Write(array5, 0, array5.Length);
			}
			return memoryStream.ToArray();
		}

		private short[] DecodeAudio(byte[] encodedData)
		{
			SpeexDecoder speexDecoder = new SpeexDecoder(1, false);
			List<byte[]> list = new List<byte[]>();
			short[] array = new short[1024];
			int num = BitConverter.ToInt32(encodedData, 0);
			List<int> list2 = new List<int>();
			for (int i = 0; i < num; i++)
			{
				int num2 = BitConverter.ToInt32(encodedData, 4 + i * 4);
				list2.Add(num2);
			}
			int num3 = 0;
			int num4 = 4 + num * 4;
			while (num4 + list2[num3] < encodedData.Length)
			{
				int num5 = speexDecoder.Decode(encodedData, num4, list2[num3], array, 0, false);
				byte[] array2 = new byte[num5 * 2];
				for (int j = 0; j < num5; j++)
				{
					byte[] bytes = BitConverter.GetBytes(array[j]);
					Array.Copy(bytes, 0, array2, j * 2, 2);
				}
				list.Add(array2);
				num3++;
				num4 += list2[num3];
			}
			int num6 = Enumerable.Sum<byte[]>(list, (byte[] m) => m.Length);
			byte[] array3 = new byte[num6];
			int num7 = 0;
			for (int k = 0; k < list.Count; k++)
			{
				byte[] array4 = list[k];
				Array.Copy(array4, 0, array3, num7, array4.Length);
				num7 += array4.Length;
			}
			short[] array5 = new short[array3.Length / 2];
			Buffer.BlockCopy(array3, 0, array5, 0, array3.Length);
			return array5;
		}

		public byte[] Encode(short[] data, BandMode mode)
		{
			return this.encode(data, mode);
		}

		public short[] Decode(byte[] data, BandMode mode)
		{
			return this.decode(data, mode);
		}

		private SpeexDecoder m_wide_dec = new SpeexDecoder(1, true);

		private SpeexEncoder m_wide_enc = new SpeexEncoder(1);

		private SpeexDecoder m_narrow_dec = new SpeexDecoder(0, true);

		private SpeexEncoder m_narrow_enc = new SpeexEncoder(0);
	}
}
