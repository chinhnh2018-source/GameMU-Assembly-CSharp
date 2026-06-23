using System;
using System.Text;

public static class WebUtility
{
	private static void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate)
	{
		int num = (int)(smpChar - 65536U);
		leadingSurrogate = (char)(num / 1024 + 55296);
		trailingSurrogate = (char)(num % 1024 + 56320);
	}

	private static int HexToInt(char h)
	{
		if (h >= '0' && h <= '9')
		{
			return (int)(h - '0');
		}
		if (h >= 'a' && h <= 'f')
		{
			return (int)(h - 'a' + '\n');
		}
		if (h >= 'A' && h <= 'F')
		{
			return (int)(h - 'A' + '\n');
		}
		return -1;
	}

	private static char IntToHex(int n)
	{
		if (n <= 9)
		{
			return (char)(n + 48);
		}
		return (char)(n - 10 + 65);
	}

	private static bool IsUrlSafeChar(char ch)
	{
		if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
		{
			return true;
		}
		switch (ch)
		{
		case '(':
		case ')':
		case '*':
		case '-':
		case '.':
			break;
		default:
			if (ch != '!' && ch != '_')
			{
				return false;
			}
			break;
		}
		return true;
	}

	public static string UrlEncode(string value)
	{
		if (value == null)
		{
			return null;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		return Encoding.UTF8.GetString(WebUtility.UrlEncode(bytes, 0, bytes.Length, false));
	}

	private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
	{
		return bytes != null || count != 0;
	}

	private static byte[] UrlEncode(byte[] bytes, int offset, int count)
	{
		if (!WebUtility.ValidateUrlEncodingParameters(bytes, offset, count))
		{
			return null;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			char c = (char)bytes[offset + i];
			if (c == ' ')
			{
				num++;
			}
			else if (!WebUtility.IsUrlSafeChar(c))
			{
				num2++;
			}
		}
		if (num == 0 && num2 == 0)
		{
			return bytes;
		}
		byte[] array = new byte[count + num2 * 2];
		int num3 = 0;
		for (int j = 0; j < count; j++)
		{
			byte b = bytes[offset + j];
			char c2 = (char)b;
			if (WebUtility.IsUrlSafeChar(c2))
			{
				array[num3++] = b;
			}
			else if (c2 == ' ')
			{
				array[num3++] = 43;
			}
			else
			{
				array[num3++] = 37;
				array[num3++] = (byte)WebUtility.IntToHex(b >> 4 & 15);
				array[num3++] = (byte)WebUtility.IntToHex((int)(b & 15));
			}
		}
		return array;
	}

	private static byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
	{
		byte[] array = WebUtility.UrlEncode(bytes, offset, count);
		if (alwaysCreateNewReturnValue && array != null && array == bytes)
		{
			return (byte[])array.Clone();
		}
		return array;
	}

	public static byte[] UrlEncodeToBytes(byte[] value, int offset, int count)
	{
		return WebUtility.UrlEncode(value, offset, count, true);
	}

	private const char HIGH_SURROGATE_START = '\ud800';

	private const char LOW_SURROGATE_END = '\udfff';

	private const char LOW_SURROGATE_START = '\udc00';

	private const int UNICODE_PLANE00_END = 65535;

	private const int UNICODE_PLANE01_START = 65536;

	private const int UNICODE_PLANE16_END = 1114111;

	private const int UnicodeReplacementChar = 65533;

	private static char[] _htmlEntityEndingChars = new char[]
	{
		';',
		'&'
	};
}
