using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LGCatchLog
{
	public static void AddCache(string data)
	{
		LGCatchLog.CacheList.Add(data);
	}

	public static void RemoveCache(string data)
	{
		LGCatchLog.CacheList.Remove(data);
	}

	public static void AddPoolCache(string data)
	{
		LGCatchLog.CachePoolList.Add(data);
	}

	public static void RemovePoolCache(string data)
	{
		LGCatchLog.CachePoolList.Remove(data);
	}

	public static void AddError(string data)
	{
		LGCatchLog.ErrorList.Add(data);
	}

	public static void WriteFile()
	{
		StreamWriter streamWriter;
		if (!File.Exists(Application.persistentDataPath + "/ResourceLog.txt"))
		{
			streamWriter = File.CreateText(Application.persistentDataPath + "/ResourceLog.txt");
		}
		else
		{
			File.WriteAllBytes(Application.persistentDataPath + "/ResourceLog.txt", new byte[0]);
			streamWriter = File.AppendText(Application.persistentDataPath + "/ResourceLog.txt");
		}
		streamWriter.WriteLine("---------------缓存资源---------------------");
		for (int i = 0; i < LGCatchLog.CacheList.Count; i++)
		{
			streamWriter.WriteLine(LGCatchLog.CacheList[i]);
		}
		streamWriter.WriteLine("---------------缓存总数: " + LGCatchLog.CacheList.Count + "---------------------");
		streamWriter.WriteLine("---------------缓存池资源---------------------");
		for (int j = 0; j < LGCatchLog.CachePoolList.Count; j++)
		{
			streamWriter.WriteLine(LGCatchLog.CachePoolList[j]);
		}
		streamWriter.WriteLine("---------------缓存池总数: " + LGCatchLog.CachePoolList.Count + "---------------------");
		streamWriter.WriteLine("---------------错误---------------------");
		for (int k = 0; k < LGCatchLog.ErrorList.Count; k++)
		{
			streamWriter.WriteLine(LGCatchLog.ErrorList[k]);
		}
		streamWriter.WriteLine("------------------------------------");
		streamWriter.Flush();
		streamWriter.Close();
	}

	private static List<string> CacheList = new List<string>();

	private static List<string> CachePoolList = new List<string>();

	private static List<string> ErrorList = new List<string>();
}
