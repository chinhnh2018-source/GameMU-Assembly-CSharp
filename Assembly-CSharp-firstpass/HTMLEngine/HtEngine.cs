using System;
using HTMLEngine.Core;

namespace HTMLEngine
{
	public class HtEngine
	{
		public static void RegisterDevice(HtDevice device)
		{
			if (HtEngine.Device != null)
			{
				HtEngine.Device.OnRelease();
			}
			HtEngine.Device = device;
		}

		public static void RegisterLogger(HtLogger logger)
		{
			HtEngine.Logger = logger;
		}

		public static HtCompiler GetCompiler()
		{
			return OP<HtCompiler>.Acquire();
		}

		internal static void Log(HtLogLevel level, string format, params object[] args)
		{
			HtEngine.Logger.Log(level, string.Format(format, args));
		}

		internal static HtDevice Device = new HtEngine.GenericDevice();

		internal static HtLogger Logger = new HtEngine.ConsoleLogger();

		public static HtLogLevel LogLevel = HtLogLevel.Debug;

		public static HtColor DefaultColor = HtColor.white;

		public static HtColor LinkHoverColor = HtColor.blue;

		public static float LinkPressedFactor = 0.6f;

		public static string LinkFunctionName = "onLinkClicked";

		public static string DefaultFontFace = "default";

		public static int DefaultFontSize = 16;

		public static HtColor DefaultLinkColor = HtColor.yellow;

		internal class GenericFont : HtFont
		{
			public GenericFont(string face, int size, bool bold, bool italic) : base(face, size, bold, italic)
			{
			}

			public override int LineSpacing
			{
				get
				{
					return base.Size;
				}
			}

			public override int WhiteSize
			{
				get
				{
					return base.Size / 2;
				}
			}

			public override HtSize Measure(string text)
			{
				return new HtSize(text.Length * this.WhiteSize, base.Size);
			}

			public override void Draw(string id, HtRect rect, HtColor color, string text, bool isEffect, DrawTextEffect effect, HtColor effectColor, int effectAmount, string linkText, object userData)
			{
				Console.WriteLine("DrawText: {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", new object[]
				{
					this,
					id,
					rect,
					color,
					text,
					isEffect,
					effect,
					effectColor,
					effectAmount,
					linkText,
					userData
				});
			}
		}

		internal class GenericImage : HtImage
		{
			public override int Width
			{
				get
				{
					return 32;
				}
			}

			public override int Height
			{
				get
				{
					return 32;
				}
			}

			public override void Draw(string id, HtRect rect, HtColor color, string linkText, object userData)
			{
				Console.WriteLine("DrawImage {0} {1} {2} {3} {4} {5}", new object[]
				{
					this,
					id,
					rect,
					color,
					linkText,
					userData
				});
			}
		}

		internal class GenericDevice : HtDevice
		{
			public override HtFont LoadFont(string face, int size, bool bold, bool italic)
			{
				return new HtEngine.GenericFont(face, size, bold, italic);
			}

			public override HtImage LoadImage(string src, int fps)
			{
				return new HtEngine.GenericImage();
			}

			public override void FillRect(HtRect rect, HtColor color, object userData)
			{
				Console.WriteLine("FillRect {0} {1} {2}", rect, color, userData);
			}

			public override void OnRelease()
			{
				Console.WriteLine("OnRelease");
			}
		}

		internal class ConsoleLogger : HtLogger
		{
			public override void Log(HtLogLevel level, string message)
			{
				Console.WriteLine("{0} : {1}", level, message);
			}
		}
	}
}
