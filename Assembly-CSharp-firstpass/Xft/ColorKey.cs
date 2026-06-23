using System;
using UnityEngine;

namespace Xft
{
	[Serializable]
	public class ColorKey : IComparable
	{
		public ColorKey(float age, Color color)
		{
			this.t = age;
			this.Color = color;
		}

		public int CompareTo(object obj)
		{
			return -((ColorKey)obj).t.CompareTo(this.t);
		}

		public float t;

		public Color Color;
	}
}
