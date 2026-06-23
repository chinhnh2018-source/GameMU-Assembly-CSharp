using System;
using System.Collections.Generic;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	public class BoCaiHelper
	{
		public static bool CopyHistoryData(List<KFBoCaoHistoryData> sData, out List<KFBoCaoHistoryData> rData)
		{
			rData = new List<KFBoCaoHistoryData>();
			try
			{
				foreach (KFBoCaoHistoryData sData2 in sData)
				{
					KFBoCaoHistoryData item = new KFBoCaoHistoryData();
					if (GlobalNew.Copy<KFBoCaoHistoryData>(sData2, ref item))
					{
						rData.Add(item);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public static string ListInt2String(List<int> iList)
		{
			string text = "";
			try
			{
				foreach (int num in iList)
				{
					if (string.IsNullOrEmpty(text))
					{
						text = string.Format("{0}", num);
					}
					else
					{
						text = string.Format("{0},{1}", text, num);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return text;
		}

		public static void String2ListInt(string str, out List<int> iList)
		{
			iList = new List<int>();
			try
			{
				string[] array = str.Split(new char[]
				{
					','
				});
				foreach (string value in array)
				{
					iList.Add(Convert.ToInt32(value));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public static int String2Int(string str)
		{
			int num = 0;
			try
			{
				string[] array = str.Split(new char[]
				{
					','
				});
				foreach (string value in array)
				{
					num += Convert.ToInt32(value);
				}
				return num;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return -1;
		}

		public static bool IsSameData(List<int> d1, List<int> d2)
		{
			try
			{
				List<int> list = new List<int>();
				list.AddRange(d2);
				foreach (int item in d1)
				{
					int num = list.IndexOf(item);
					if (num < 0)
					{
						return false;
					}
					list.RemoveAt(num);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}
	}
}
