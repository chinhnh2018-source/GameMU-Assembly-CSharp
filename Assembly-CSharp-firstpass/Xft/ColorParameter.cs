using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xft
{
	[Serializable]
	public class ColorParameter
	{
		public ColorParameter()
		{
			this.Colors = new List<ColorKey>();
			this.AddColorKey(0f, Color.white);
		}

		public Color GetGradientColor(float t)
		{
			if (this.Colors.Count == 1)
			{
				return this.Colors[0].Color;
			}
			if (this.Colors.Count == 0)
			{
				return Color.black;
			}
			for (int i = 1; i < this.Colors.Count; i++)
			{
				if (t <= this.Colors[i].t)
				{
					int num = i - 1;
					return Color.Lerp(this.Colors[num].Color, this.Colors[i].Color, (t - this.Colors[num].t) / (this.Colors[i].t - this.Colors[num].t));
				}
			}
			return this.Colors[this.Colors.Count - 1].Color;
		}

		public ColorKey AddColorKey(float t, Color color)
		{
			ColorKey colorKey = new ColorKey(t, color);
			this.Colors.Add(colorKey);
			this.Colors.Sort();
			return colorKey;
		}

		public List<ColorKey> Colors;
	}
}
