using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public static class ConfigParser
	{
		static ConfigParser()
		{
			for (ExtPropIndexes extPropIndexes = ExtPropIndexes.Strong; extPropIndexes < ExtPropIndexes.Max; extPropIndexes++)
			{
				ConfigParser.ExtPropName2ExtPropIndexDict[extPropIndexes.ToString()] = extPropIndexes;
			}
		}

		public static ExtPropIndexes GetPropIndexByPropName(string propName)
		{
			ExtPropIndexes extPropIndexes;
			ExtPropIndexes result;
			if (ConfigParser.ExtPropName2ExtPropIndexDict.TryGetValue(propName.Trim(), out extPropIndexes))
			{
				result = extPropIndexes;
			}
			else
			{
				result = ExtPropIndexes.Max;
			}
			return result;
		}

		public static void ParseExtprops(double[] extProps, string str, string splitChars = "|,")
		{
			try
			{
				string[] array = str.Split(new char[]
				{
					splitChars[0]
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array3[0]);
						if (propIndexByPropName < ExtPropIndexes.Max)
						{
							extProps[(int)propIndexByPropName] = Global.SafeConvertToDouble(array3[1]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				throw ex;
			}
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

		public static EquipPropItem ParseEquipPropItem(string str, bool verifyColumn = true, char splitChar1 = '|', char splitChar2 = ',', char splitChar3 = '-')
		{
			EquipPropItem equipPropItem = new EquipPropItem();
			if (!string.IsNullOrEmpty(str))
			{
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
					if (array3.Length == 2)
					{
						ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array3[0]);
						if (propIndexByPropName < ExtPropIndexes.Max)
						{
							double num;
							if (double.TryParse(array3[1], out num))
							{
								equipPropItem.ExtProps[(int)propIndexByPropName] = num;
							}
						}
						else
						{
							int num2 = -1;
							int num3 = -1;
							string text2 = array3[0];
							if (text2 != null)
							{
								if (!(text2 == "Attack"))
								{
									if (!(text2 == "Mattack"))
									{
										if (!(text2 == "Defense"))
										{
											if (text2 == "Mdefense")
											{
												num2 = 5;
												num3 = 6;
											}
										}
										else
										{
											num2 = 3;
											num3 = 4;
										}
									}
									else
									{
										num2 = 9;
										num3 = 10;
									}
								}
								else
								{
									num2 = 7;
									num3 = 8;
								}
							}
							string[] array4 = array3[1].Split(new char[]
							{
								splitChar3
							});
							double num;
							if (num2 >= 0 && double.TryParse(array4[0], out num))
							{
								equipPropItem.ExtProps[num2] = num;
							}
							if (num3 >= 0 && double.TryParse(array4[1], out num))
							{
								equipPropItem.ExtProps[num3] = num;
							}
						}
					}
				}
			}
			return equipPropItem;
		}

		public static void ParseAwardsItemList(string str, ref AwardsItemList awardsItemList, char splitChar1 = '|', char splitChar2 = ',')
		{
			awardsItemList.Add(str);
		}

		public static void ParseAwardsGoodsList(string str, List<GoodsData> goodsDataList, char splitChar1 = '|', char splitChar2 = ',')
		{
			AwardsItemList awardsItemList = new AwardsItemList();
			awardsItemList.Add(str);
		}

		public static double[] ParserExtPropsFromAttrubite(XElement xml, List<KeyValuePair<int, string>> list)
		{
			double[] array = new double[177];
			foreach (KeyValuePair<int, string> keyValuePair in list)
			{
				array[keyValuePair.Key] = ConfigHelper.GetElementAttributeValueDouble(xml, keyValuePair.Value, 0.0);
			}
			return array;
		}

		private static Dictionary<string, ExtPropIndexes> ExtPropName2ExtPropIndexDict = new Dictionary<string, ExtPropIndexes>(StringComparer.OrdinalIgnoreCase);
	}
}
