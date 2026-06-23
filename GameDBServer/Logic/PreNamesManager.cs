using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class PreNamesManager
	{
		public static void AddPreNameItem(string name, int sex, int used)
		{
			PreNameItem preNameItem = new PreNameItem
			{
				Name = name,
				Sex = sex,
				Used = used
			};
			lock (PreNamesManager._Mutex)
			{
				PreNamesManager._PreNamesDict[name] = preNameItem;
				if (0 == sex)
				{
					PreNamesManager._MalePreNamesList.Add(preNameItem);
				}
				else
				{
					PreNamesManager._FemalePreNamesList.Add(preNameItem);
				}
			}
		}

		public static string GetRandomName(int Sex)
		{
			string result = "";
			lock (PreNamesManager._Mutex)
			{
				List<PreNameItem> list;
				if (0 == Sex)
				{
					list = PreNamesManager._MalePreNamesList;
				}
				else
				{
					list = PreNamesManager._FemalePreNamesList;
				}
				if (list.Count > 0)
				{
					int num = 10;
					while (num-- >= 0)
					{
						int index = PreNamesManager.rand.Next(0, list.Count);
						if (list[index].Used <= 0)
						{
							result = list[index].Name;
							break;
						}
					}
				}
			}
			return result;
		}

		public static bool SetUsedPreName(string name)
		{
			lock (PreNamesManager._Mutex)
			{
				PreNameItem preNameItem = null;
				if (PreNamesManager._PreNamesDict.TryGetValue(name, out preNameItem))
				{
					if (preNameItem.Used <= 0)
					{
						preNameItem.Used = 1;
						PreNamesManager._UsedPreNamesQueue.Enqueue(preNameItem);
						return true;
					}
				}
			}
			return false;
		}

		public static void ClearUsedPreNames()
		{
			lock (PreNamesManager._Mutex)
			{
				int num = 50;
				while (PreNamesManager._UsedPreNamesQueue.Count > 0 && num-- >= 0)
				{
					PreNameItem preNameItem = PreNamesManager._UsedPreNamesQueue.Dequeue();
					if (null != preNameItem)
					{
						PreNamesManager._PreNamesDict.Remove(preNameItem.Name);
						if (0 == preNameItem.Sex)
						{
							PreNamesManager._MalePreNamesList.Remove(preNameItem);
						}
						else
						{
							PreNamesManager._FemalePreNamesList.Remove(preNameItem);
						}
					}
				}
			}
		}

		private static List<string> LoadListFromFileByName(string fileName)
		{
			List<string> list = new List<string>();
			try
			{
				if (!File.Exists(fileName))
				{
					return list;
				}
				StreamReader streamReader = new StreamReader(fileName, Encoding.GetEncoding("gb2312"));
				if (null == streamReader)
				{
					return list;
				}
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					text = text.Trim();
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
				streamReader.Close();
				return list;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return list;
		}

		public static void LoadFromFiles(DBManager dbMgr)
		{
			try
			{
				if (File.Exists("./名字库/姓.txt"))
				{
					if (File.Exists("./名字库/男.txt"))
					{
						if (File.Exists("./名字库/女.txt"))
						{
							List<string> list = PreNamesManager.LoadListFromFileByName("./名字库/姓.txt");
							List<string> list2 = PreNamesManager.LoadListFromFileByName("./名字库/男.txt");
							List<string> list3 = PreNamesManager.LoadListFromFileByName("./名字库/女.txt");
							for (int i = 0; i < list.Count; i++)
							{
								for (int j = 0; j < list2.Count; j++)
								{
									string text = list[i] + list2[j];
									if (DBWriter.InsertNewPreName(dbMgr, text, 0) >= 0)
									{
										PreNamesManager.AddPreNameItem(text, 0, 0);
									}
								}
								for (int k = 0; k < list3.Count; k++)
								{
									string text = list[i] + list3[k];
									if (DBWriter.InsertNewPreName(dbMgr, text, 1) >= 0)
									{
										PreNamesManager.AddPreNameItem(text, 1, 0);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		public static void LoadPremNamesFromDB(DBManager dbMgr)
		{
			DBQuery.QueryPreNames(dbMgr, PreNamesManager._PreNamesDict, PreNamesManager._MalePreNamesList, PreNamesManager._FemalePreNamesList);
		}

		private static object _Mutex = new object();

		private static Random rand = new Random();

		private static Dictionary<string, PreNameItem> _PreNamesDict = new Dictionary<string, PreNameItem>(200000);

		private static List<PreNameItem> _MalePreNamesList = new List<PreNameItem>(100000);

		private static List<PreNameItem> _FemalePreNamesList = new List<PreNameItem>(100000);

		private static Queue<PreNameItem> _UsedPreNamesQueue = new Queue<PreNameItem>(5000);
	}
}
