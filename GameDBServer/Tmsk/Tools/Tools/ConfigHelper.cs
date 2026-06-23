using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Server.Tools;

namespace Tmsk.Tools.Tools
{
	public static class ConfigHelper
	{
		public static XElement Load(string fileName)
		{
			XElement result = null;
			if (File.Exists(fileName))
			{
				result = XElement.Load(fileName);
			}
			return result;
		}

		public static IEnumerable<XElement> GetXElements(XElement xml, string name)
		{
			try
			{
				return xml.DescendantsAndSelf(name);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public static XElement GetXElement(XElement xml, string name)
		{
			try
			{
				return xml.DescendantsAndSelf(name).SingleOrDefault<XElement>();
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public static XElement GetXElement(XElement xml, string name, string attrName, string attrValue)
		{
			try
			{
				return xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public static string GetElementAttributeValue(XElement xml, string name, string attrName, string attrValue, string attribute, string defVal = "")
		{
			string result = defVal;
			try
			{
				XElement xelement = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != xelement)
				{
					XAttribute xattribute = xelement.Attribute(attribute);
					if (null != xattribute)
					{
						result = xattribute.Value;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static long GetElementAttributeValueLong(XElement xml, string name, string attrName, string attrValue, string attribute, long defVal = 0L)
		{
			long result = defVal;
			try
			{
				XElement xelement = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != xelement)
				{
					XAttribute xattribute = xelement.Attribute(attribute);
					if (null != xattribute)
					{
						if (!long.TryParse(xattribute.Value, out result))
						{
							result = defVal;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static int[] GetElementAttributeValueIntArray(XElement xml, string name, string attrName, string attrValue, string attribute, int[] defArr = null)
		{
			int[] array = defArr;
			try
			{
				XElement xelement = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != xelement)
				{
					XAttribute xattribute = xelement.Attribute(attribute);
					if (null == xattribute)
					{
						return defArr;
					}
					array = ConfigHelper.String2IntArray(xattribute.Value, ',');
					if (array == null)
					{
						return defArr;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return array;
		}

		public static string[] GetElementAttributeValueStrArray(XElement xml, string name, string attrName, string attrValue, string attribute, string[] defArr = null)
		{
			string[] result = defArr;
			try
			{
				XElement xelement = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != xelement)
				{
					XAttribute xattribute = xelement.Attribute(attribute);
					if (null == xattribute)
					{
						return defArr;
					}
					result = xattribute.Value.Split(new char[]
					{
						','
					});
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static string GetElementAttributeValue(XElement xml, string attribute, string defVal = "")
		{
			string result = defVal;
			try
			{
				XAttribute xattribute = xml.Attribute(attribute);
				if (null != xattribute)
				{
					result = xattribute.Value;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static long GetElementAttributeValueLong(XElement xml, string attribute, long defVal = 0L)
		{
			long result = defVal;
			try
			{
				XAttribute xattribute = xml.Attribute(attribute);
				if (null != xattribute)
				{
					if (!long.TryParse(xattribute.Value, out result))
					{
						result = defVal;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static double GetElementAttributeValueDouble(XElement xml, string attribute, double defVal = 0.0)
		{
			double result = defVal;
			try
			{
				XAttribute xattribute = xml.Attribute(attribute);
				if (null != xattribute)
				{
					if (!double.TryParse(xattribute.Value, out result))
					{
						result = defVal;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static int[] String2IntArray(string str, char spliter = ',')
		{
			int[] result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				string[] array = str.Split(new char[]
				{
					spliter
				});
				result = ConfigHelper.StringArray2IntArray(array, 0, array.Length);
			}
			return result;
		}

		public static List<int> String2IntList(string str, char spliter = ',')
		{
			List<int> list = new List<int>();
			List<int> result;
			if (string.IsNullOrEmpty(str))
			{
				result = list;
			}
			else
			{
				string[] array = str.Split(new char[]
				{
					spliter
				});
				foreach (string s in array)
				{
					int item;
					if (int.TryParse(s, out item))
					{
						list.Add(item);
					}
				}
				result = list;
			}
			return result;
		}

		private static int[] StringArray2IntArray(string[] sa, int start, int count)
		{
			int[] result;
			if (sa == null)
			{
				result = null;
			}
			else if (start < 0 || start >= sa.Length)
			{
				result = null;
			}
			else if (count <= 0)
			{
				result = null;
			}
			else if (sa.Length - start < count)
			{
				result = null;
			}
			else
			{
				int[] array = new int[count];
				for (int i = 0; i < count; i++)
				{
					string text = sa[start + i].Trim();
					text = (string.IsNullOrEmpty(text) ? "0" : text);
					array[i] = Convert.ToInt32(text);
				}
				result = array;
			}
			return result;
		}

		public static bool ParseStrInt2(string str, ref int v1, ref int v2, char splitChar = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				string[] array = str.Split(new char[]
				{
					splitChar
				});
				int num;
				int num2;
				if (array.Length < 2 || !int.TryParse(array[0], out num) || !int.TryParse(array[1], out num2))
				{
					result = false;
				}
				else
				{
					v1 = num;
					v2 = num2;
					result = true;
				}
			}
			return result;
		}

		public static bool ParseStrInt3(string str, ref int v1, ref int v2, ref int v3, char splitChar = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				string[] array = str.Split(new char[]
				{
					splitChar
				});
				int num;
				int num2;
				int num3;
				if (array.Length < 3 || !int.TryParse(array[0], out num) || !int.TryParse(array[1], out num2) || !int.TryParse(array[2], out num3))
				{
					result = false;
				}
				else
				{
					v1 = num;
					v2 = num2;
					v3 = num3;
					result = true;
				}
			}
			return result;
		}

		public static bool ParserTimeRangeList(List<TimeSpan> list, string str, bool clear = true, char splitChar1 = '|', char splitChar2 = '-')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				if (clear)
				{
					list.Clear();
				}
				string[] array = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						splitChar2
					});
					if (array3.Length != 2)
					{
						return false;
					}
					TimeSpan item;
					TimeSpan item2;
					if (!TimeSpan.TryParse(array3[0], out item) || !TimeSpan.TryParse(array3[1], out item2))
					{
						return false;
					}
					list.Add(item);
					list.Add(item2);
				}
				result = (list.Count > 0);
			}
			return result;
		}

		public static bool ParserTimeRangeListWithDay(List<TimeSpan> list, string str, bool clear = true, char splitChar1 = '|', char splitChar2 = '-', char splitChar3 = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				if (clear)
				{
					list.Clear();
				}
				string[] array = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						splitChar3
					});
					if (array3.Length != 2)
					{
						return false;
					}
					int days;
					if (!int.TryParse(array3[0], out days))
					{
						return false;
					}
					string[] array4 = array3[1].Split(new char[]
					{
						splitChar2
					});
					if (array4.Length != 2)
					{
						return false;
					}
					TimeSpan item;
					TimeSpan item2;
					if (!TimeSpan.TryParse(array4[0], out item) || !TimeSpan.TryParse(array4[1], out item2))
					{
						return false;
					}
					TimeSpan ts = new TimeSpan(days, 0, 0, 0);
					item = item.Add(ts);
					item2 = item2.Add(ts);
					list.Add(item);
					list.Add(item2);
				}
				result = (list.Count > 0);
			}
			return result;
		}

		public static List<List<int>> ParserIntArrayList(string str, bool verifyColumn = true, char splitChar1 = '|', char splitChar2 = ',')
		{
			List<List<int>> list = new List<List<int>>();
			List<List<int>> result;
			if (string.IsNullOrEmpty(str))
			{
				result = list;
			}
			else
			{
				string[] array = str.Split(new char[]
				{
					splitChar1
				});
				int num = -1;
				foreach (string text in array)
				{
					List<int> list2 = new List<int>();
					if (!string.IsNullOrEmpty(text))
					{
						string[] array3 = text.Split(new char[]
						{
							splitChar2
						});
						foreach (string s in array3)
						{
							int item;
							if (int.TryParse(s, out item))
							{
								list2.Add(item);
							}
						}
					}
					list.Add(list2);
					if (verifyColumn)
					{
						if (num < 0)
						{
							num = list2.Count;
							if (num == 0)
							{
								break;
							}
						}
						else if (num != list2.Count)
						{
							list.Clear();
							break;
						}
					}
				}
				result = list;
			}
			return result;
		}
	}
}
