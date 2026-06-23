using System;
using System.Collections.Generic;

public static class AS3Ext
{
	public static T2 GetValue<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
	{
		T2 result = default(T2);
		if (dict == null || dict.TryGetValue(key, ref result))
		{
		}
		return result;
	}

	public static string toString(this DateTime dataTime, string format)
	{
		format = "{0:" + format + "}";
		return string.Format(format, dataTime);
	}

	public static string toChineseLang(this string otherLang)
	{
		return otherLang;
	}

	public static int IndexOf<T>(this T[] array, T key)
	{
		return Array.IndexOf<T>(array, key);
	}

	public static T shift<T>(this List<T> list)
	{
		T result = default(T);
		lock (list)
		{
			if (list != null && list.Count > 0)
			{
				result = list[0];
				list.RemoveAt(0);
			}
		}
		return result;
	}

	public static T[] splice<T>(this List<T> list, int index, int count = -1, params T[] args)
	{
		T[] array = null;
		lock (list)
		{
			if (list != null && list.Count > 0 && index < list.Count && index >= 0)
			{
				if (count == 0)
				{
					count = list.Count - index;
				}
				else
				{
					count = Math.Min(list.Count - index, count);
				}
				array = new T[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = list[index + i];
				}
				list.RemoveRange(index, count);
				foreach (T t in args)
				{
					list.Add(t);
				}
			}
		}
		return array;
	}

	public static List<T> GetValue<T>(this Dictionary<string, List<T>> dict, string key)
	{
		List<T> result = null;
		if (dict == null || dict.TryGetValue(key, ref result))
		{
		}
		return result;
	}

	public static string toLowerCase(this string str)
	{
		return str.ToLower();
	}

	public static char charAt(this string str, int index)
	{
		if (index < str.Length)
		{
			return str.get_Chars(index);
		}
		return '\0';
	}
}
