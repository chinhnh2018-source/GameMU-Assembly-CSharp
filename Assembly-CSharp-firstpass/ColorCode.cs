using System;
using UnityEngine;

public class ColorCode
{
	public static string EncodingText(string text, string color = "00ff00")
	{
		return string.Format("{{{0}}}{1}{{{2}}}", color, text, "-");
	}

	public static string EncodingText(int count, string color = "00ff00")
	{
		return string.Format("{{{0}}}{1}{{{2}}}", color, count, "-");
	}

	public static string EncodingText2(long count, long need, string color = "fd010c")
	{
		if (count < need)
		{
			return string.Format("{{{0}}}{1}{{{2}}}/{3}", new object[]
			{
				color,
				count,
				"-",
				need
			});
		}
		return string.Format("{0}/{1}", count, need);
	}

	public static string EncodingText1(long count, long need, string color = "fd010c")
	{
		if (count < need)
		{
			return string.Format("{{{0}}}{1}{{{2}}}", color, need, "-");
		}
		return string.Format("{0}", need);
	}

	public static string EncodingText2A(long count, long need, string color = "fd010c", string color2 = "fffffe")
	{
		if (count >= need)
		{
			return string.Format("{{{0}}}{1}/{2}{{{3}}}", new object[]
			{
				color,
				count,
				need,
				"-"
			});
		}
		return string.Format("{{{0}}}{1}/{2}{{{3}}}", new object[]
		{
			color2,
			count,
			need,
			"-"
		});
	}

	public static string Format(string format, params object[] args)
	{
		return string.Empty;
	}

	public static Color GetGrayColor(Color color, float luminance = 0f)
	{
		luminance = Mathf.Clamp(color.grayscale, luminance, 0.6f);
		return new Color(luminance, luminance, luminance);
	}

	public const uint Orange = 4294941960U;

	public const uint Green = 4278255360U;

	public const uint Gray = 4284900966U;

	public const uint White = 4294967294U;

	public const uint Red = 4294770956U;

	public const uint White2 = 4292528046U;

	public const uint Yellow = 4294956128U;

	public const uint Yellow3 = 4291007824U;

	public const uint Blue1 = 4288269567U;

	public const uint Blue2 = 4280443596U;

	public const uint Blue3 = 4283013052U;

	public const uint Green2 = 4282034975U;

	public const uint Cyan = 4278237637U;

	public const uint Gray2 = 4284374622U;

	public const uint Cyan2 = 4283013052U;

	public const uint Blue = 4278190335U;

	public const uint Purple = 4287236853U;

	public const string orange = "ff9d08";

	public const string orange2 = "e3b36c";

	public const string green = "00ff00";

	public const string gray = "666666";

	public const string white = "fffffe";

	public const string red = "fd010c";

	public const string white2 = "dac7ae";

	public const string white3 = "ffffff";

	public const string yellow = "f9f702";

	public const string yellow2 = "ffd460";

	public const string yellow3 = "c39550";

	public const string yellow4 = "F2E2BD";

	public const string blue1 = "99ccff";

	public const string blue2 = "2262cc";

	public const string blue3 = "4997bc";

	public const string green2 = "3aab1f";

	public const string cyan = "00b9c5";

	public const string gray2 = "5e5e5e";

	public const string cyan2 = "4997bc";

	public const string blue = "0000ff";

	public const string purple = "8a0af5";

	public const string close = "-";
}
