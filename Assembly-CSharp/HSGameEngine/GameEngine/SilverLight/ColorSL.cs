using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public static class ColorSL
	{
		public static uint FromArgb(int a, int r, int g, int b)
		{
			return ColorSL.FromArgb((uint)a, (uint)r, (uint)g, (uint)b);
		}

		public static uint FromArgb(uint a, uint r, uint g, uint b)
		{
			uint num = 0U;
			num |= a << 24;
			num |= r << 16;
			num |= g << 8;
			return num | b;
		}

		public static Color ParseArgb(uint color)
		{
			float num = 0.003921569f;
			Color black = Color.black;
			black.a = num * (color >> 24 & 255U);
			black.r = num * (color >> 16 & 255U);
			black.g = num * (color >> 8 & 255U);
			black.b = num * (color & 255U);
			return black;
		}
	}
}
