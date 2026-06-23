using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using UnityEngine;

namespace HSGameEngine.GameEngine.Data
{
	public class GGuangMuData
	{
		public byte SetValue(int x, int y, byte value, byte oldValue)
		{
			long num = ((long)x << 32) + (long)y;
			if (!this.OldObsValue.ContainsKey(num))
			{
				this.OldObsValue[num] = oldValue;
			}
			return value;
		}

		public byte ResetValue(int x, int y, byte value)
		{
			long num = ((long)x << 32) + (long)y;
			if (this.OldObsValue.ContainsKey(num))
			{
				value = this.OldObsValue[num];
			}
			return value;
		}

		public int ID;

		public int Show;

		public string Path;

		public Vector3 Pos;

		public Point[] ZuDangs;

		public string Description;

		public string Animation;

		public Dictionary<long, byte> OldObsValue = new Dictionary<long, byte>();
	}
}
