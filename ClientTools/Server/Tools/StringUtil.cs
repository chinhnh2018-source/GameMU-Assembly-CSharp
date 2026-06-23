using System;

namespace Server.Tools
{
	public class StringUtil
	{
		public static bool IsNullOrEmpty(string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static string trim(string str)
		{
			if (str == null || "" == str)
			{
				return "";
			}
			int num = 0;
			if (!false)
			{
				while (StringUtil.isWhitespace(str[num]))
				{
					num++;
				}
			}
			int num2 = str.Length - 1;
			while (StringUtil.isWhitespace(str[num2]))
			{
				num2--;
			}
			if (num2 < num)
			{
				return "";
			}
			return str.Substring(num, num2 + 1);
		}

		public static bool isWhitespace(char character)
		{
			switch (character)
			{
			case '\t':
			case '\n':
			case '\f':
			case '\r':
				break;
			case '\v':
				return false;
			default:
				if (character != ' ')
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static bool isWhitespace(string character)
		{
			if (string.IsNullOrEmpty(character))
			{
				goto IL_3F;
			}
			char c = character[0];
			switch (c)
			{
			case '\t':
			case '\n':
			case '\f':
			case '\r':
				break;
			case '\v':
				return false;
			default:
				goto IL_33;
			}
			return true;
			IL_33:
			if (c == ' ')
			{
				return true;
			}
			if (-2147483648 != 0)
			{
				return false;
			}
			IL_3F:
			if (!false)
			{
				return false;
			}
			goto IL_33;
		}

		public static string substitute(string format, params object[] args)
		{
			string result = "";
			try
			{
				result = string.Format(format, args);
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
				result = format;
			}
			return result;
		}

		public static bool isEqualIgnoreCase(string a, string b)
		{
			return a.ToLower() == b.ToLower();
		}
	}
}
