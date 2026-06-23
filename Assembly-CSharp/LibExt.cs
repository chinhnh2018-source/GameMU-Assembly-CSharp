using System;
using System.Collections.Generic;

public static class LibExt
{
	public static int SafeToInt32(this string strNum, int defValue = 0)
	{
		int num = 1;
		if (!string.IsNullOrEmpty(strNum))
		{
			int num2 = 0;
			for (int i = 0; i < strNum.Length; i++)
			{
				char c = strNum.get_Chars(i);
				if (!char.IsDigit(c))
				{
					if (num2 != 0 || c != '-')
					{
						break;
					}
					num = -1;
				}
				else
				{
					num2 = num2 * 10 + (int)(c - '0');
				}
			}
			return num2 * num;
		}
		return defValue;
	}

	public static bool IsNullOrEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}
}
