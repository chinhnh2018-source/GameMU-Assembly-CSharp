using System;
using UnityEngine;

namespace MoPhoGames.USpeak.Core
{
	public class USpeakAudioClipConverter
	{
		public static byte[] AudioClipToBytes(AudioClip clip)
		{
			float[] array = new float[clip.samples * clip.channels];
			clip.GetData(array, 0);
			byte[] array2 = new byte[clip.samples * clip.channels];
			for (int i = 0; i < array.Length; i++)
			{
				float num = array[i] * 128f;
				int num2 = Mathf.RoundToInt(num);
				num2 += 127;
				if (num2 < 0)
				{
					num2 = 0;
				}
				if (num2 > 255)
				{
					num2 = 255;
				}
				array2[i] = (byte)num2;
			}
			return array2;
		}

		public static short[] AudioClipToShorts(AudioClip clip, float gain = 1f)
		{
			float[] array = new float[clip.samples * clip.channels];
			clip.GetData(array, 0);
			short[] array2 = new short[clip.samples * clip.channels];
			for (int i = 0; i < array.Length; i++)
			{
				float num = array[i] * gain;
				if (Mathf.Abs(num) > 1f)
				{
					if (num > 0f)
					{
						num = 1f;
					}
					else
					{
						num = -1f;
					}
				}
				float num2 = num * 3267f;
				array2[i] = (short)num2;
			}
			return array2;
		}

		public static short[] AudioDataToShorts(float[] samples, int channels, float gain = 1f)
		{
			short[] array = new short[samples.Length * channels];
			for (int i = 0; i < samples.Length; i++)
			{
				float num = samples[i] * gain;
				if (Mathf.Abs(num) > 1f)
				{
					if (num > 0f)
					{
						num = 1f;
					}
					else
					{
						num = -1f;
					}
				}
				float num2 = num * 3267f;
				array[i] = (short)num2;
			}
			return array;
		}

		public static AudioClip BytesToAudioClip(byte[] data, int channels, int frequency, bool threedimensional, float gain)
		{
			float[] array = new float[data.Length];
			for (int i = 0; i < array.Length; i++)
			{
				int num = (int)data[i];
				num -= 127;
				array[i] = (float)num / 128f * gain;
			}
			AudioClip orCreateAudioClip = USpeakAudioManager.GetOrCreateAudioClip(data.Length / channels, channels, frequency, threedimensional);
			orCreateAudioClip.SetData(array, 0);
			return orCreateAudioClip;
		}

		public static AudioClip ShortsToAudioClip(short[] data, int channels, int frequency, bool threedimensional, float gain)
		{
			float[] array = new float[data.Length];
			for (int i = 0; i < array.Length; i++)
			{
				int num = (int)data[i];
				array[i] = (float)num / 3267f * gain;
			}
			AudioClip orCreateAudioClip = USpeakAudioManager.GetOrCreateAudioClip(data.Length / channels, channels, frequency, threedimensional);
			if (array.Length > 0)
			{
				orCreateAudioClip.SetData(array, 0);
			}
			return orCreateAudioClip;
		}
	}
}
