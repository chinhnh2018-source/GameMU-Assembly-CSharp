using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public static class Colors
	{
		public static Color Uint2Color(uint colorValue)
		{
			uint num = colorValue & 255U;
			uint num2 = colorValue >> 8 & 255U;
			uint num3 = colorValue >> 16 & 255U;
			uint num4 = colorValue >> 24 & 255U;
			if (num4 == 0U)
			{
				num4 = 255U;
			}
			return new Color(num3 / 255f, num2 / 255f, num / 255f, num4 / 255f);
		}

		public static uint Color2Uint(Color color)
		{
			return (uint)color.b + ((uint)color.g << 8) + ((uint)color.r << 16) + ((uint)color.a << 24);
		}

		public const uint None = 0U;

		public const uint Black = 4278190080U;

		public const uint Blue = 4278190335U;

		public const uint Brown = 4289014314U;

		public const uint Cyan = 4278255615U;

		public const uint DarkGray = 4289309097U;

		public const uint Gray = 4286611584U;

		public const uint Green = 4278222848U;

		public const uint LightGray = 4292072403U;

		public const uint LightGreen = 4278255360U;

		public const uint Magenta = 4294902015U;

		public const uint Orange = 4294944000U;

		public const uint Purple = 4286578816U;

		public const uint Red = 4294901760U;

		public const uint Transparent = 16777215U;

		public const uint White = 4294967295U;

		public const uint Yellow = 4294967040U;

		public const uint Gold = 16766720U;

		public const uint LightBlue = 2133441U;
	}
}
