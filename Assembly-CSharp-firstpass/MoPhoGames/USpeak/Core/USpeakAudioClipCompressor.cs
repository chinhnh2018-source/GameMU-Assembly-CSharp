using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using MoPhoGames.USpeak.Codec;
using UnityEngine;

namespace MoPhoGames.USpeak.Core
{
	public class USpeakAudioClipCompressor : MonoBehaviour
	{
		public static byte[] CompressAudioData(float[] samples, int channels, out int sample_count, BandMode mode, float gain = 1f)
		{
			USpeakAudioClipCompressor.data.Clear();
			sample_count = 0;
			short[] array = USpeakAudioClipConverter.AudioDataToShorts(samples, channels, gain);
			byte[] array2 = USpeakAudioClipCompressor.Codec.Encode(array, mode);
			USpeakAudioClipCompressor.data.AddRange(array2);
			return USpeakAudioClipCompressor.zip(USpeakAudioClipCompressor.data.ToArray());
		}

		public static byte[] CompressAudioClip(AudioClip clip, out int samples, BandMode mode, float gain = 1f)
		{
			USpeakAudioClipCompressor.data.Clear();
			samples = 0;
			short[] array = USpeakAudioClipConverter.AudioClipToShorts(clip, gain);
			byte[] array2 = USpeakAudioClipCompressor.Codec.Encode(array, mode);
			USpeakAudioClipCompressor.data.AddRange(array2);
			return USpeakAudioClipCompressor.zip(USpeakAudioClipCompressor.data.ToArray());
		}

		public static AudioClip DecompressAudioClip(byte[] data, int samples, int channels, bool threeD, BandMode mode, float gain)
		{
			int frequency = 4000;
			if (mode == BandMode.Narrow)
			{
				frequency = 8000;
			}
			else if (mode == BandMode.Wide)
			{
				frequency = 16000;
			}
			byte[] array = USpeakAudioClipCompressor.unzip(data);
			short[] array2 = USpeakAudioClipCompressor.Codec.Decode(array, mode);
			USpeakAudioClipCompressor.tmp.Clear();
			USpeakAudioClipCompressor.tmp.AddRange(array2);
			return USpeakAudioClipConverter.ShortsToAudioClip(USpeakAudioClipCompressor.tmp.ToArray(), channels, frequency, threeD, gain);
		}

		private static byte[] zip(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream(data);
			MemoryStream memoryStream2 = new MemoryStream();
			using (GZipStream gzipStream = new GZipStream(memoryStream2, 0))
			{
				memoryStream.WriteTo(gzipStream);
			}
			return memoryStream2.ToArray();
		}

		public static byte[] unzip(byte[] data)
		{
			GZipStream input = new GZipStream(new MemoryStream(data), 1);
			MemoryStream memoryStream = new MemoryStream();
			USpeakAudioClipCompressor.CopyStream(input, memoryStream);
			return memoryStream.ToArray();
		}

		private static void CopyStream(Stream input, Stream output)
		{
			byte[] array = new byte[32768];
			for (;;)
			{
				int num = input.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				output.Write(array, 0, num);
			}
		}

		public static byte[] DecompressAudioClipbyte(byte[] data, int samples, int channels, bool threeD, BandMode mode, float gain)
		{
			return USpeakAudioClipCompressor.unzip(data);
		}

		public static ICodec Codec = new RawCodec();

		private static List<byte> data = new List<byte>();

		private static List<short> tmp = new List<short>();
	}
}
