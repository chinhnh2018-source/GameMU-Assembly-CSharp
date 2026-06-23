using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CheckSysValueDll;
using Server.Tools;

namespace GameServer.Tools.CheckSysValue
{
	public class CheckSysValueHelper
	{
		public static void WriteMap(string cmd = null)
		{
			try
			{
				CheckSysValueHelper.WriteLine("画图时间较长......", ConsoleColor.Green);
				RelationMapModel.WriteMap(Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				CheckSysValueHelper.WriteLine("失败", ConsoleColor.Green);
				LogManager.WriteLog(9, string.Format("[ljl_WriteMap]{0}", ex.ToString()), null, true);
			}
		}

		private static void WriteLine(string str, ConsoleColor Color = ConsoleColor.Green)
		{
			Console.ForegroundColor = Color;
			SysConOut.WriteLine(str);
			Console.ForegroundColor = ConsoleColor.White;
		}

		private static void _setSeachMgr(ref GetValueModel model)
		{
			while (string.IsNullOrEmpty(model.TypeName))
			{
				CheckSysValueHelper.WriteLine("【0】直接输完整数据类型； 【其它模糊输入】； 【q：退出】", ConsoleColor.Red);
				string text = Console.ReadLine();
				if (!text.Equals("0"))
				{
					if (!text.Equals("q"))
					{
						CheckSysValueHelper.WriteLine("请输入模糊类型名", ConsoleColor.Red);
						text = Console.ReadLine();
						text = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeachType(text), "类型名模糊结果", model);
						if (!text.Equals("q"))
						{
							model.TypeName = text;
							continue;
						}
					}
					return;
				}
				CheckSysValueHelper.WriteLine("请直接输入类型名", ConsoleColor.Red);
				model.TypeName = Console.ReadLine().Trim();
			}
			CheckSysValueHelper._setSeach(model);
			List<string> seachAttr = RelationMapModel.GetSeachAttr(model.TypeName);
			while (string.IsNullOrEmpty(model.SeachName))
			{
				CheckSysValueHelper.WriteLine("一共有" + seachAttr.Count + "条属性，【0】查看全部， 【其它】模糊查询属性，【q：退出】", ConsoleColor.Red);
				string text = Console.ReadLine();
				if (text.Equals("0"))
				{
					text = CheckSysValueHelper._setResult(seachAttr, "可查询属性", model);
				}
				else
				{
					if (text.Equals("q"))
					{
						return;
					}
					CheckSysValueHelper.WriteLine("请输入匹配关键字", ConsoleColor.Red);
					text = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeach(Console.ReadLine(), seachAttr), "可查询属性", model);
				}
				model.SeachName = text;
			}
			CheckSysValueHelper._setSeach(model);
		}

		private static void _setSeach(GetValueModel model)
		{
			if (null != model)
			{
				if (!string.IsNullOrEmpty(model.TypeName))
				{
					CheckSysValueHelper.WriteLine("选择 TypeName =[" + model.TypeName + "]", ConsoleColor.Yellow);
					if (string.IsNullOrEmpty(model.SeachName))
					{
						CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
					}
					else
					{
						CheckSysValueHelper.WriteLine("选择 SeachName =[" + model.SeachName + "]", ConsoleColor.Yellow);
						if (model.SeachDataList.Count < 1)
						{
							CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
						}
						else
						{
							CheckSysValueHelper.WriteLine("选择筛选列表", ConsoleColor.Red);
							foreach (SeachData seachData in model.SeachDataList)
							{
								CheckSysValueHelper.WriteLine("筛选AttName=" + seachData.AttName, ConsoleColor.Yellow);
								CheckSysValueHelper.WriteLine("筛选SeachVal=" + seachData.SeachVal, ConsoleColor.Yellow);
								CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
							}
						}
					}
				}
			}
		}

		private static string _getSeachKey(GetValueModel model)
		{
			string text = string.Format("{0},{1}", model.TypeName, model.SeachName);
			foreach (SeachData seachData in model.SeachDataList)
			{
				text += seachData.AttName;
			}
			return text;
		}

		private static string _setResult(List<string> dlist, string str, GetValueModel model)
		{
			int num = 0;
			foreach (string text in dlist)
			{
				num = ((text.Length > num) ? text.Length : num);
			}
			string str2 = CheckSysValueHelper._getSeachKey(model);
			CheckSysValueHelper.WriteLine(string.Concat(new object[]
			{
				str,
				" 一共",
				dlist.Count,
				"条，每次展示20条,【q：退出】"
			}), ConsoleColor.Red);
			int i = 0;
			try
			{
				while (i <= dlist.Count)
				{
					string text2 = "{1, -6}{0, " + -num + " },";
					text2 = string.Format(text2, dlist[i], "【" + i + "】,");
					string key = str2 + dlist[i];
					if (CheckSysValueHelper.AttrTypeDict.ContainsKey(key))
					{
						text2 += CheckSysValueHelper.AttrTypeDict[key];
					}
					CheckSysValueHelper.WriteLine(text2, ConsoleColor.Green);
					i++;
					if (i >= dlist.Count)
					{
						CheckSysValueHelper.WriteLine("已经最后一条 ，【确认选择直接输入序号】，【任意键重输入】， 【q：退出】", ConsoleColor.Red);
						string text3 = Console.ReadLine();
						if (text3.Equals("q"))
						{
							return "q";
						}
						if (!string.IsNullOrEmpty(text3))
						{
							CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
							return dlist[Convert.ToInt32(text3)];
						}
						return "";
					}
					else if (i % 20 == 0)
					{
						CheckSysValueHelper.WriteLine(" 一共" + dlist.Count + "条，【确认选择直接输入序号】，【回车继续显示】,【任意键重输入】， 【q：退出】", ConsoleColor.Red);
						string text3 = Console.ReadLine();
						if (text3.Equals("q"))
						{
							return "q";
						}
						if (!string.IsNullOrEmpty(text3))
						{
							CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
							return dlist[Convert.ToInt32(text3)];
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
			return "";
		}

		private static void _AddSeach(ref GetValueModel model)
		{
			SeachData seachData = new SeachData();
			List<string> list;
			if (CheckSysValueHelper.AttrDict.TryGetValue(CheckSysValueHelper._getSeachKey(model), out list))
			{
				if (list.Count > 0)
				{
					while (string.IsNullOrEmpty(seachData.AttName))
					{
						CheckSysValueHelper.WriteLine("一共有" + list.Count + "条属性，【0】查看全部， 【q：退出】 【其它】模糊查询属性", ConsoleColor.Red);
						string text = Console.ReadLine();
						if (text.Equals("0"))
						{
							text = CheckSysValueHelper._setResult(list, "可查询属性", model);
						}
						else
						{
							if (text.Equals("q"))
							{
								return;
							}
							CheckSysValueHelper.WriteLine("请输入匹配关键字", ConsoleColor.Red);
							text = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeach(Console.ReadLine(), list), "可查询属性", model);
						}
						if (!text.Equals("q"))
						{
							seachData.AttName = text;
						}
					}
					CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
					seachData.SeachVal = Console.ReadLine();
					model.SeachDataList.Add(seachData);
					return;
				}
			}
			CheckSysValueHelper.WriteLine("无缓存记录 输入要查询的属性名", ConsoleColor.Red);
			seachData.AttName = Console.ReadLine();
			CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
			seachData.SeachVal = Console.ReadLine();
			model.SeachDataList.Add(seachData);
		}

		private static bool _SetCheck(GetValueModel model)
		{
			CheckValueResult value = CheckModel.GetValue(model, Assembly.GetExecutingAssembly(), true);
			bool result;
			if (string.IsNullOrEmpty(value.Info))
			{
				CheckSysValueHelper.WriteLine("查询成功", ConsoleColor.Red);
				int num = 0;
				string text = CheckSysValueHelper._getSeachKey(model);
				if (value.ResultDict.Count > 0)
				{
					if (CheckSysValueHelper.AttrDict.ContainsKey(text))
					{
						CheckSysValueHelper.AttrDict[text].Clear();
					}
					else
					{
						if (CheckSysValueHelper.AttrDict.Count > 100)
						{
							CheckSysValueHelper.AttrDict.Remove(CheckSysValueHelper.AttrDict.Keys.ToList<string>()[0]);
						}
						CheckSysValueHelper.AttrDict.Add(text, new List<string>());
					}
				}
				int num2 = 0;
				foreach (KeyValuePair<string, List<CheckValueResultItem>> keyValuePair in value.ResultDict)
				{
					foreach (CheckValueResultItem checkValueResultItem in keyValuePair.Value)
					{
						num++;
						foreach (string text2 in checkValueResultItem.Childs)
						{
							string[] files = text2.Split(new char[]
							{
								','
							});
							if (string.IsNullOrEmpty(CheckSysValueHelper.AttrDict[text].Find((string x) => x.Equals(files[0]))))
							{
								CheckSysValueHelper.AttrDict[text].Add(files[0]);
							}
							if (CheckSysValueHelper.AttrTypeDict.Count > 5000)
							{
								CheckSysValueHelper.AttrTypeDict.Remove(CheckSysValueHelper.AttrTypeDict.Keys.ToList<string>()[0]);
							}
							if (!CheckSysValueHelper.AttrTypeDict.ContainsKey(text + files[0]))
							{
								CheckSysValueHelper.AttrTypeDict.Add(text + files[0], files[1]);
							}
							num2 = ((files[0].Length > num2) ? files[0].Length : num2);
						}
					}
				}
				int num3 = 0;
				int num4 = 0;
				CheckSysValueHelper.WriteLine("一共" + num + "组，每次展示20条 【q】：退出展示,返回结果", ConsoleColor.Green);
				foreach (KeyValuePair<string, List<CheckValueResultItem>> keyValuePair in value.ResultDict)
				{
					foreach (CheckValueResultItem checkValueResultItem in keyValuePair.Value)
					{
						num4++;
						num3++;
						CheckSysValueHelper.WriteLine(string.Format("第{3}组，AttrName={0}, StrValue={1}, TypeName={2}", new object[]
						{
							keyValuePair.Key,
							checkValueResultItem.StrValue,
							checkValueResultItem.TypeName,
							num4
						}), ConsoleColor.Green);
						foreach (string text2 in checkValueResultItem.Childs)
						{
							num3++;
							string[] array = text2.Split(new char[]
							{
								','
							});
							string text3 = "{0, " + -num2 + " }, {1}";
							text3 = string.Format(text3, array[0], array[1]);
							CheckSysValueHelper.WriteLine(text3, ConsoleColor.Yellow);
							if (num3 % 20 == 0)
							{
								CheckSysValueHelper.WriteLine(string.Concat(new object[]
								{
									"一共",
									num,
									"组，现在是",
									num4,
									"组，【任意键继续】， 【q：退出】"
								}), ConsoleColor.Red);
								string text4 = Console.ReadLine();
								if (text4.Equals("q"))
								{
									return true;
								}
							}
						}
					}
					CheckSysValueHelper.WriteLine("已经展示完毕", ConsoleColor.Green);
				}
				result = true;
			}
			else
			{
				CheckSysValueHelper.WriteLine("查询失败" + value.Info, ConsoleColor.Red);
				result = false;
			}
			return result;
		}

		public static void GetValue(string cmd = null)
		{
			try
			{
				cmd = "";
				GetValueModel getValueModel = new GetValueModel();
				getValueModel.SeachDataList = new List<SeachData>();
				for (;;)
				{
					if (string.IsNullOrEmpty(getValueModel.TypeName) || string.IsNullOrEmpty(getValueModel.SeachName))
					{
						CheckSysValueHelper._setSeachMgr(ref getValueModel);
					}
					if (string.IsNullOrEmpty(getValueModel.TypeName) || string.IsNullOrEmpty(getValueModel.SeachName))
					{
						break;
					}
					bool flag = getValueModel.SeachDataList.Count > 0;
					if (flag)
					{
						CheckSysValueHelper.WriteLine("【0】重新输入所有， 【1】清空所有seach 【2】 删除一个seach，【3】修改当前seach 【4】打印筛选条件 【5】添加筛选条件 【q】退出 【其它】查看结果", ConsoleColor.Red);
					}
					else
					{
						CheckSysValueHelper.WriteLine("【0】重新输入所有 【4】打印筛选条件 【5】添加筛选条件 【q】退出 【其它】查看结果", ConsoleColor.Red);
					}
					cmd = Console.ReadLine();
					if (cmd.Equals("0"))
					{
						getValueModel = new GetValueModel();
						getValueModel.SeachDataList = new List<SeachData>();
					}
					else if (cmd.Equals("1") && flag)
					{
						getValueModel.SeachDataList.Clear();
					}
					else if (cmd.Equals("2") && flag)
					{
						if (getValueModel.SeachDataList.Count > 0)
						{
							getValueModel.SeachDataList.RemoveAt(getValueModel.SeachDataList.Count - 1);
						}
					}
					else if (cmd.Equals("3") && flag)
					{
						if (getValueModel.SeachDataList.Count > 0)
						{
							SeachData seachData = getValueModel.SeachDataList[getValueModel.SeachDataList.Count - 1];
							CheckSysValueHelper.WriteLine(string.Format("当前查询属性[-{0}-]，要修改 y ？", seachData.AttName), ConsoleColor.Red);
							cmd = Console.ReadLine();
							if (cmd.Equals("y"))
							{
								seachData = new SeachData();
								getValueModel.SeachDataList.RemoveAt(getValueModel.SeachDataList.Count - 1);
								List<string> list;
								if (CheckSysValueHelper.AttrDict.TryGetValue(CheckSysValueHelper._getSeachKey(getValueModel), out list))
								{
									if (list.Count > 0)
									{
										while (string.IsNullOrEmpty(seachData.AttName))
										{
											CheckSysValueHelper.WriteLine("一共有" + list.Count + "条属性，【0】查看全部，【1】直接输入 【其它】模糊查询属性", ConsoleColor.Red);
											cmd = Console.ReadLine();
											if (cmd.Equals("0"))
											{
												cmd = CheckSysValueHelper._setResult(list, "可查询属性", getValueModel);
											}
											else if (cmd.Equals("1"))
											{
												CheckSysValueHelper.WriteLine("输入要查询的属性名", ConsoleColor.Red);
												cmd = Console.ReadLine();
											}
											else
											{
												CheckSysValueHelper.WriteLine("请输入匹配关键字", ConsoleColor.Red);
												cmd = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeach(Console.ReadLine(), list), "可查询属性", getValueModel);
											}
											if (!cmd.Equals("q"))
											{
												seachData.AttName = cmd;
											}
										}
										seachData.SeachVal = "";
										CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
										seachData.SeachVal = Console.ReadLine();
									}
									else
									{
										CheckSysValueHelper.WriteLine("无缓存记录 输入要查询的属性名", ConsoleColor.Red);
										seachData.AttName = Console.ReadLine();
										CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
										seachData.SeachVal = Console.ReadLine();
									}
									getValueModel.SeachDataList.Add(seachData);
								}
							}
							else
							{
								CheckSysValueHelper.WriteLine(string.Format("当前筛选条件[-{0}-]，要修改 y ？", seachData.SeachVal), ConsoleColor.Red);
								cmd = Console.ReadLine();
								if (cmd.Equals("y"))
								{
									CheckSysValueHelper.WriteLine("输入新的SeachVal", ConsoleColor.Red);
									seachData.SeachVal = Console.ReadLine();
								}
							}
						}
					}
					else if (cmd.Equals("4"))
					{
						CheckSysValueHelper._setSeach(getValueModel);
					}
					else if (cmd.Equals("5"))
					{
						CheckSysValueHelper._AddSeach(ref getValueModel);
					}
					else
					{
						if (cmd.Equals("q"))
						{
							break;
						}
						CheckSysValueHelper._SetCheck(getValueModel);
						CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
					}
				}
				return;
			}
			catch (Exception ex)
			{
				CheckSysValueHelper.WriteLine("异常失败", ConsoleColor.Green);
				LogManager.WriteLog(9, string.Format("[ljl_WriteMap]{0}", ex.ToString()), null, true);
			}
			CheckSysValueHelper.WriteLine("已经结束 。。。。。 help 查看帮助", ConsoleColor.Green);
		}

		private static Dictionary<string, List<string>> AttrDict = new Dictionary<string, List<string>>();

		private static Dictionary<string, string> AttrTypeDict = new Dictionary<string, string>();
	}
}
