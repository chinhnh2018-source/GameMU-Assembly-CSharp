using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using UnityEngine;

namespace HSGameEngine.GameEngine.Common
{
	public class ConvertExt
	{
		public static string Array2String<T>(T[] intArr, char ch = ',')
		{
			if (intArr == null || intArr.Length <= 0)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < intArr.Length; i++)
			{
				text = text + intArr[i] + ch.ToString();
			}
			return text.Substring(0, text.Length - 1);
		}

		public static int SafeConvertToInt32(string str)
		{
			str = str.Trim();
			int result = 0;
			if (int.TryParse(str, ref result))
			{
				return result;
			}
			return 0;
		}

		public static long SafeConvertToInt64(string str)
		{
			str = str.Trim();
			long result = 0L;
			if (long.TryParse(str, ref result))
			{
				return result;
			}
			return 0L;
		}

		public static long SafeConvertToTicks(string strDateTime)
		{
			DateTime dateTime;
			if (!DateTime.TryParse(strDateTime, ref dateTime))
			{
				return 0L;
			}
			return dateTime.Ticks / 10000L;
		}

		public static double SafeConvertToDouble(string str)
		{
			str = str.Trim();
			double result = 0.0;
			if (double.TryParse(str, ref result))
			{
				return result;
			}
			return 0.0;
		}

		public static float SafeConvertToFloat(string str, float defaultVal = 0f)
		{
			str = str.Trim();
			float result = 0f;
			if (float.TryParse(str, ref result))
			{
				return result;
			}
			return defaultVal;
		}

		public static double[] String2DoubleArray(string str, char ch)
		{
			if (str.Trim() == string.Empty)
			{
				return null;
			}
			string[] array = str.Split(new char[]
			{
				ch
			});
			if (array == null)
			{
				return null;
			}
			double[] array2 = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].Trim() == string.Empty))
				{
					array2[i] = ConvertExt.SafeConvertToDouble(array[i].Trim());
				}
			}
			return array2;
		}

		public static int[] String2IntArray(string str, char ch)
		{
			if (str.Trim() == string.Empty)
			{
				return null;
			}
			string[] array = str.Split(new char[]
			{
				ch
			});
			if (array == null)
			{
				return null;
			}
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].Trim() == string.Empty))
				{
					array2[i] = ConvertExt.SafeConvertToInt32(array[i].Trim());
				}
			}
			return array2;
		}

		public static List<int> String2IntList(string str, char ch)
		{
			if (str.Trim() == string.Empty)
			{
				return null;
			}
			string[] array = str.Split(new char[]
			{
				ch
			});
			if (array == null)
			{
				return null;
			}
			List<int> list = new List<int>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].Trim() == string.Empty))
				{
					list.Insert(list.Count, ConvertExt.SafeConvertToInt32(array[i].Trim()));
				}
			}
			return list;
		}

		public static double[] StringArray2DoubleArray(string[] sa)
		{
			double[] array = new double[sa.Length];
			for (int i = 0; i < sa.Length; i++)
			{
				if (!(sa[i].Trim() == string.Empty))
				{
					array[i] = ConvertExt.SafeConvertToDouble(sa[i].Trim());
				}
			}
			return array;
		}

		public static int[] StringArray2IntArray(string[] sa)
		{
			int[] array = new int[sa.Length];
			for (int i = 0; i < sa.Length; i++)
			{
				if (!(sa[i].Trim() == string.Empty))
				{
					array[i] = ConvertExt.SafeConvertToInt32(sa[i].Trim());
				}
			}
			return array;
		}

		public static Point[] StrToPointArray(string str)
		{
			str = str.Trim();
			if (str == string.Empty)
			{
				return null;
			}
			string[] array = str.Split(new char[]
			{
				','
			});
			if (array.Length <= 0)
			{
				return null;
			}
			Point[] array2 = new Point[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[]
				{
					':'
				});
				if (array3.Length == 2)
				{
					if (!(array3[0].Trim() == string.Empty) && !(array3[1].Trim() == string.Empty))
					{
						array2[i] = new Point(ConvertExt.SafeConvertToInt32(array3[0]), ConvertExt.SafeConvertToInt32(array3[1]));
					}
				}
			}
			return array2;
		}

		public static Point[] StrToPointArray2(string str)
		{
			str = str.Trim();
			if (str == string.Empty)
			{
				return null;
			}
			string[] array = str.Split(new char[]
			{
				','
			});
			if (array.Length <= 0)
			{
				return null;
			}
			Point[] array2 = new Point[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[]
				{
					'_'
				});
				if (array3.Length == 2)
				{
					if (!(array3[0].Trim() == string.Empty) && !(array3[1].Trim() == string.Empty))
					{
						array2[i] = new Point(ConvertExt.SafeConvertToInt32(array3[0]), ConvertExt.SafeConvertToInt32(array3[1]));
					}
				}
			}
			return array2;
		}

		public static string IntArray2String(int[] intArr, char ch = ',')
		{
			if (intArr == null || intArr.Length <= 0)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < intArr.Length; i++)
			{
				text = text + intArr[i] + ch.ToString();
			}
			return text.Substring(0, text.Length - 1);
		}

		public static string StringArray2String(string[] strArr, char ch = ',')
		{
			if (strArr == null || strArr.Length <= 0)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < strArr.Length; i++)
			{
				text = text + strArr[i] + ch.ToString();
			}
			return text.Substring(0, text.Length - 1);
		}

		public static Vector3 MeterToCentimeter(Vector3 v)
		{
			return new Vector3(v.x * 100f, v.y * 100f, v.z * 100f);
		}

		public static Vector3 CentimeterMeter(Vector3 v)
		{
			return new Vector3(v.x / 100f, v.y / 100f, v.z / 100f);
		}

		public static byte ToByte(string v)
		{
			byte result = 0;
			if (byte.TryParse(v, ref result))
			{
				return result;
			}
			return 0;
		}

		public static long SafeToInt64(string v)
		{
			long result = 0L;
			if (long.TryParse(v, ref result))
			{
				return result;
			}
			return 0L;
		}
	}
}
