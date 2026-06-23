using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace CheckSysValueDll
{
	public class RelationMapModel
	{
		private static void GetEnumList()
		{
			string path = AppDomain.CurrentDomain.BaseDirectory + "CheckRelation\\EnumType.json";
			try
			{
				if (File.Exists(path))
				{
					string input = File.ReadAllText(path);
					JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
					List<string> list = javaScriptSerializer.Deserialize<List<string>>(input);
					if (null != list)
					{
						RelationMapModel.EnumList = list;
					}
				}
			}
			catch
			{
			}
		}

		private static string CreatDirectory()
		{
			string text = AppDomain.CurrentDomain.BaseDirectory + "CheckRelation\\";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		private static void ReadRelationMap()
		{
			if (RelationMapModel.Map.Count <= 0)
			{
				string str = RelationMapModel.CreatDirectory();
				string path = str + "RelationMapAll.txt";
				try
				{
					if (File.Exists(path))
					{
						string text = null;
						StreamReader streamReader = new StreamReader(path);
						while (streamReader.Peek() >= 0)
						{
							string text2 = streamReader.ReadLine().Trim();
							if (string.IsNullOrEmpty(text))
							{
								text = text2;
							}
							else if (text2.IndexOf("******************") > -1)
							{
								text = null;
							}
							else if (!string.IsNullOrEmpty(text2))
							{
								if (RelationMapModel.Map.ContainsKey(text))
								{
									RelationMapModel.Map[text].Add(text2);
								}
								else
								{
									RelationMapModel.Map.Add(text, new List<string>
									{
										text2
									});
								}
							}
						}
						streamReader.Close();
					}
				}
				catch
				{
				}
			}
		}

		public static void WriteMap(Assembly assembly)
		{
			string str = RelationMapModel.CreatDirectory();
			if (!RelationMapModel.isWrite || RelationMapModel.Map.Count < 1)
			{
				RelationMapModel.GetRelationMap(assembly);
			}
			string path = str + "RelationMapKey.txt";
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				foreach (KeyValuePair<string, List<string>> keyValuePair in RelationMapModel.Map)
				{
					streamWriter.WriteLine(keyValuePair.Key);
				}
			}
			path = str + "RelationMapAll.txt";
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				foreach (KeyValuePair<string, List<string>> keyValuePair in RelationMapModel.Map)
				{
					streamWriter.WriteLine(keyValuePair.Key);
					foreach (string str2 in keyValuePair.Value)
					{
						streamWriter.WriteLine("     " + str2);
					}
					streamWriter.WriteLine("**************************************************************************************************");
				}
			}
			path = str + "EnumType.json";
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				streamWriter.WriteLine(CheckModel.Data2Json(RelationMapModel.EnumList));
			}
			Console.WriteLine("画图完成 RelationMap end");
			RelationMapModel.isWrite = true;
		}

		private static void GetRelationMap(Assembly assembly)
		{
			RelationMapModel.Map.Clear();
			if (RelationMapModel.EnumList.Count < 1)
			{
				RelationMapModel.GetEnumList();
			}
			foreach (Type type in assembly.GetTypes())
			{
				if (!RelationMapModel.IsFilter(type.FullName))
				{
					if (RelationMapModel.IsEnum(type))
					{
						RelationMapModel.EnumList.Add(type.FullName);
					}
					else
					{
						FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						foreach (FieldInfo fieldInfo in fields)
						{
							try
							{
								if (RelationMapModel.Map.ContainsKey(type.FullName))
								{
									RelationMapModel.Map[type.FullName].Add(fieldInfo.Name);
								}
								else
								{
									RelationMapModel.Map.Add(type.FullName, new List<string>
									{
										fieldInfo.Name
									});
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}

		private static bool IsEnum(Type type)
		{
			try
			{
				return type.IsEnum;
			}
			catch
			{
			}
			return false;
		}

		private static bool IsFilter(string Name)
		{
			try
			{
				bool flag;
				if (Name.IndexOf("CheckSysValueDll.") <= -1)
				{
					flag = string.IsNullOrEmpty(RelationMapModel.EnumList.Find((string x) => x.Equals(Name)));
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		public static object GetObject(Assembly assembly, string TypeName, string AttrName, ref CheckValueResult resultData)
		{
			TypeName = TypeName.Trim();
			AttrName = AttrName.Trim();
			List<string> list = new List<string>();
			Type type = assembly.GetType(TypeName);
			object result;
			if (null == type)
			{
				result = null;
			}
			else
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					list.Add(fieldInfo.Name);
				}
				if (!RelationMapModel.Map.TryGetValue(TypeName, out list))
				{
					RelationMapModel.Map.Add(TypeName, list);
				}
				if (string.IsNullOrEmpty(AttrName))
				{
					resultData.Info = "只查询了类型 数据包含数据有";
					CheckValueResultItem checkValueResultItem = new CheckValueResultItem();
					List<CheckValueResultItem> value = new List<CheckValueResultItem>
					{
						checkValueResultItem
					};
					checkValueResultItem.TypeName = "只查询了类型";
					foreach (string arg in list)
					{
						checkValueResultItem.Childs.Add(string.Format("{0},{1}", arg, ""));
					}
					resultData.ResultDict.Add("包含属性", value);
					result = list;
				}
				else
				{
					FieldInfo field = type.GetField(AttrName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (null == field)
					{
						result = list;
					}
					else
					{
						result = field.GetValue(null);
					}
				}
			}
			return result;
		}

		public static List<string> FuzzySeachType(string name)
		{
			name = name.Trim();
			List<string> list = new List<string>();
			List<string> result;
			if (string.IsNullOrEmpty(name))
			{
				result = list;
			}
			else
			{
				RelationMapModel.ReadRelationMap();
				foreach (string text in RelationMapModel.Map.Keys)
				{
					if (text.ToLower().IndexOf(name.ToLower()) > -1)
					{
						list.Add(text);
					}
				}
				list.Sort();
				result = list;
			}
			return result;
		}

		public static List<string> FuzzySeach(string name, List<string> dlist)
		{
			List<string> list = new List<string>();
			name = name.Trim();
			if (string.IsNullOrEmpty(name))
			{
				list.AddRange(dlist);
			}
			else
			{
				foreach (string text in dlist)
				{
					if (text.ToLower().IndexOf(name.ToLower()) > -1)
					{
						list.Add(text);
					}
				}
			}
			list.Sort();
			return list;
		}

		public static List<string> GetSeachAttr(string type)
		{
			RelationMapModel.ReadRelationMap();
			type = type.Trim();
			List<string> list = new List<string>();
			List<string> result;
			if (!RelationMapModel.Map.ContainsKey(type))
			{
				result = list;
			}
			else
			{
				foreach (string item in RelationMapModel.Map[type])
				{
					list.Add(item);
				}
				list.Sort();
				result = list;
			}
			return result;
		}

		private static bool isWrite = false;

		private static List<string> EnumList = new List<string>();

		private static Dictionary<string, List<string>> Map = new Dictionary<string, List<string>>();
	}
}
