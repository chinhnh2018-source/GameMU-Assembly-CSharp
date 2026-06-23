using System;

namespace HTMLEngine.Core
{
	[Flags]
	internal enum FontStyle
	{
		Normal = 0,
		Bold = 1,
		Italic = 2,
		Underline = 4,
		Strike = 8
	}
}
