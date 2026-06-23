using System;
using System.Collections.Generic;
using System.Globalization;

namespace HTMLEngine
{
	public struct HtColor
	{
		public bool IsTransparent
		{
			get
			{
				return this.A == 0;
			}
		}

		public static HtColor RGBA(byte r, byte g, byte b, byte a = 255)
		{
			return new HtColor
			{
				R = r,
				G = g,
				B = b,
				A = a
			};
		}

		private static bool TryParse(string rs, string gs, string bs, ref byte r, ref byte g, ref byte b)
		{
			return byte.TryParse(rs, 515, NumberFormatInfo.InvariantInfo, ref r) && byte.TryParse(gs, 515, NumberFormatInfo.InvariantInfo, ref g) && byte.TryParse(bs, 515, NumberFormatInfo.InvariantInfo, ref b);
		}

		private static bool TryParse(string rs, string gs, string bs, string aa, ref byte r, ref byte g, ref byte b, ref byte a)
		{
			return byte.TryParse(rs, 515, NumberFormatInfo.InvariantInfo, ref r) && byte.TryParse(gs, 515, NumberFormatInfo.InvariantInfo, ref g) && byte.TryParse(bs, 515, NumberFormatInfo.InvariantInfo, ref b) && byte.TryParse(aa, 515, NumberFormatInfo.InvariantInfo, ref a);
		}

		public static HtColor Parse(string text)
		{
			return HtColor.Parse(text, HtColor._error);
		}

		public static HtColor Parse(string text, HtColor onError)
		{
			if (string.IsNullOrEmpty(text))
			{
				return onError;
			}
			if (text.StartsWith("#"))
			{
				byte r = 0;
				byte g = 0;
				byte b = 0;
				int length = text.Length;
				switch (length)
				{
				case 4:
				{
					string text2 = text.Substring(1, 1);
					text2 += text2;
					string text3 = text.Substring(2, 1);
					text3 += text3;
					string text4 = text.Substring(3, 1);
					text4 += text4;
					if (HtColor.TryParse(text2, text3, text4, ref r, ref g, ref b))
					{
						return HtColor.RGBA(r, g, b, byte.MaxValue);
					}
					break;
				}
				case 7:
				{
					string text2 = text.Substring(1, 2);
					string text3 = text.Substring(3, 2);
					string text4 = text.Substring(5, 2);
					if (HtColor.TryParse(text2, text3, text4, ref r, ref g, ref b))
					{
						return HtColor.RGBA(r, g, b, byte.MaxValue);
					}
					break;
				}
				case 9:
				{
					string text2 = text.Substring(1, 2);
					string text3 = text.Substring(3, 2);
					string text4 = text.Substring(5, 2);
					byte maxValue = byte.MaxValue;
					string aa = text.Substring(7, 2);
					if (HtColor.TryParse(text2, text3, text4, aa, ref r, ref g, ref b, ref maxValue))
					{
						return HtColor.RGBA(r, g, b, maxValue);
					}
					break;
				}
				}
			}
			else if (text != null)
			{
				if (HtColor.<>f__switch$map7 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(18);
					dictionary.Add("transparent", 0);
					dictionary.Add("maroon", 1);
					dictionary.Add("red", 2);
					dictionary.Add("orange", 3);
					dictionary.Add("yellow", 4);
					dictionary.Add("olive", 5);
					dictionary.Add("purple", 6);
					dictionary.Add("fuchsia", 7);
					dictionary.Add("white", 8);
					dictionary.Add("lime", 9);
					dictionary.Add("green", 10);
					dictionary.Add("navy", 11);
					dictionary.Add("blue", 12);
					dictionary.Add("aqua", 13);
					dictionary.Add("teal", 14);
					dictionary.Add("black", 15);
					dictionary.Add("silver", 16);
					dictionary.Add("gray", 17);
					HtColor.<>f__switch$map7 = dictionary;
				}
				int length;
				if (HtColor.<>f__switch$map7.TryGetValue(text, ref length))
				{
					switch (length)
					{
					case 0:
						return HtColor.transparent;
					case 1:
						return HtColor.maroon;
					case 2:
						return HtColor.red;
					case 3:
						return HtColor.orange;
					case 4:
						return HtColor.yellow;
					case 5:
						return HtColor.olive;
					case 6:
						return HtColor.purple;
					case 7:
						return HtColor.fuchsia;
					case 8:
						return HtColor.white;
					case 9:
						return HtColor.lime;
					case 10:
						return HtColor.green;
					case 11:
						return HtColor.navy;
					case 12:
						return HtColor.blue;
					case 13:
						return HtColor.aqua;
					case 14:
						return HtColor.teal;
					case 15:
						return HtColor.black;
					case 16:
						return HtColor.silver;
					case 17:
						return HtColor.gray;
					}
				}
			}
			return onError;
		}

		public override string ToString()
		{
			return string.Format("{0:X2}{1:X2}{2:X2}({3:X2})", new object[]
			{
				this.R,
				this.G,
				this.B,
				this.A
			});
		}

		public static readonly HtColor transparent = HtColor.RGBA(0, 0, 0, 0);

		public static readonly HtColor _error = HtColor.RGBA(byte.MaxValue, 0, 0, byte.MaxValue);

		public static readonly HtColor maroon = HtColor.Parse("#800000");

		public static readonly HtColor red = HtColor.Parse("#FF0000");

		public static readonly HtColor orange = HtColor.Parse("#FFA500");

		public static readonly HtColor yellow = HtColor.Parse("#FFFF00");

		public static readonly HtColor olive = HtColor.Parse("#808000");

		public static readonly HtColor purple = HtColor.Parse("#800080");

		public static readonly HtColor fuchsia = HtColor.Parse("#FF00FF");

		public static readonly HtColor white = HtColor.Parse("#FFFFFF");

		public static readonly HtColor lime = HtColor.Parse("#00FF00");

		public static readonly HtColor green = HtColor.Parse("#008000");

		public static readonly HtColor navy = HtColor.Parse("#000080");

		public static readonly HtColor blue = HtColor.Parse("#0000FF");

		public static readonly HtColor aqua = HtColor.Parse("#00FFFF");

		public static readonly HtColor teal = HtColor.Parse("#008080");

		public static readonly HtColor black = HtColor.Parse("#000000");

		public static readonly HtColor silver = HtColor.Parse("#C0C0C0");

		public static readonly HtColor gray = HtColor.Parse("#808080");

		public byte R;

		public byte G;

		public byte B;

		public byte A;
	}
}
