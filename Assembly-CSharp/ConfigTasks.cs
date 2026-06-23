using System;
using System.Collections.Generic;
using System.Threading;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigTasks
{
	public static int MinZhangJieID { get; private set; }

	public static int MaxZhangJieID { get; private set; }

	public static void PreCacheTaskXmlNodesEx()
	{
		MUDebug.Log<string>(new string[]
		{
			"PreCacheTaskXmlNodes start..."
		});
		ConfigTasks.PreCacheTaskZhangJieXmlNodes();
		string text = "Config/SystemTasksBin.txt";
		string text2 = "IsolateRes_VO";
		try
		{
			text = XmlManager.GetOnlyFileName(text);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text2);
			if (null == assetBundle)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 缓存中没找到 {0}"), text2));
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(text) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text));
					MUDebug.LogError<string>(new string[]
					{
						"bin读取失败，开始尝试从XML读取该文件" + text
					});
					ConfigTasks.PreCacheTaskXmlNodesExOld();
				}
				else
				{
					VOBinOperator.Instance(typeof(ConfigTasks)).SetBuffer(textAsset.bytes);
					VOBinOperator.Instance(typeof(ConfigTasks)).ParseBinToVOofTaskVO_ByTrdPairs();
					List<TaskVO> list = new List<TaskVO>();
					foreach (KeyValuePair<int, TaskVO> keyValuePair in ConfigTasks.TaskXmlNodeDict)
					{
						list.Add(keyValuePair.Value);
					}
					int i = 0;
					TaskZhangJieVO taskZhangJieVO = null;
					foreach (KeyValuePair<int, TaskZhangJieVO> keyValuePair2 in ConfigTasks.TaskZhangJieXmlNodeDict)
					{
						int key = keyValuePair2.Key;
						taskZhangJieVO = keyValuePair2.Value;
						while (i < list.Count)
						{
							TaskVO taskVO = list[i];
							if (taskVO.TaskClass == 0)
							{
								if (taskVO.ID > taskZhangJieVO.EndTaskID)
								{
									break;
								}
								taskVO.TaskZhangJieID = key;
								taskVO.TaskIndexOfZhangJie = taskZhangJieVO.TaskCount;
								taskZhangJieVO.TaskCount++;
							}
							i++;
						}
					}
					while (i < list.Count)
					{
						TaskVO taskVO = list[i];
						if (taskVO.TaskClass == 0 && taskZhangJieVO != null)
						{
							taskVO.TaskIndexOfZhangJie = taskZhangJieVO.TaskCount;
							taskVO.TaskZhangJieID = taskZhangJieVO.ID;
						}
						i++;
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		finally
		{
		}
	}

	public static void PreCacheTaskXmlNodes()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheTaskXmlNodes start..."
		});
		XElement isolateResXml = Global.GetIsolateResXml("Config/SystemTasks.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Task");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			TaskVO taskVO = new TaskVO();
			taskVO.CopyFrom(xelementList[i]);
			ConfigTasks.TaskXmlNodeDict[taskVO.ID] = taskVO;
		}
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"PreCacheTaskXmlNodes end, used: ",
				Time.realtimeSinceStartup - realtimeSinceStartup,
				", Item count=",
				xelementList.Count
			})
		});
	}

	public static void PreCacheTaskXmlNodesExOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheTaskXmlNodes start..."
		});
		ConfigTasks.PreCacheTaskZhangJieXmlNodes();
		string xmlName = "Config/SystemTasks.Xml";
		string resName = "IsolateRes_VO";
		try
		{
			xmlName = XmlManager.GetOnlyFileName(xmlName);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resName);
			if (null == assetBundle)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 缓存中没找到 {0}"), resName));
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(xmlName) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
				}
				else
				{
					string textContent = textAsset.text;
					if (Global.IsSupportThread())
					{
						Thread thread = new Thread(delegate()
						{
							ConfigTasks.ParseXMLToVO(textContent, xmlName, resName);
						});
						thread.Start();
					}
					else
					{
						ConfigTasks.ParseXMLToVO(textContent, xmlName, resName);
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		finally
		{
		}
	}

	public static void ParseXMLToVO(string textContent, string xmlName, string resName)
	{
		XElement xelement = XElement.Parse(textContent);
		if (xelement == null)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 缓存中没找到 {0}"), resName));
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Task");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 缓存中没找到 {0}"), resName));
			return;
		}
		int i = 0;
		TaskZhangJieVO taskZhangJieVO = null;
		foreach (KeyValuePair<int, TaskZhangJieVO> keyValuePair in ConfigTasks.TaskZhangJieXmlNodeDict)
		{
			int key = keyValuePair.Key;
			taskZhangJieVO = keyValuePair.Value;
			while (i < xelementList.Count)
			{
				TaskVO taskVO = new TaskVO();
				taskVO.CopyFrom(xelementList[i]);
				ConfigTasks.TaskXmlNodeDict[taskVO.ID] = taskVO;
				if (taskVO.TaskClass == 0)
				{
					if (taskVO.ID > taskZhangJieVO.EndTaskID)
					{
						break;
					}
					taskVO.TaskZhangJieID = key;
					taskVO.TaskIndexOfZhangJie = taskZhangJieVO.TaskCount;
					taskZhangJieVO.TaskCount++;
				}
				i++;
			}
		}
		while (i < xelementList.Count)
		{
			TaskVO taskVO = new TaskVO();
			taskVO.CopyFrom(xelementList[i]);
			if (taskVO.TaskClass == 0 && taskZhangJieVO != null)
			{
				taskVO.TaskIndexOfZhangJie = taskZhangJieVO.TaskCount;
				taskVO.TaskZhangJieID = taskZhangJieVO.ID;
			}
			ConfigTasks.TaskXmlNodeDict[taskVO.ID] = taskVO;
			i++;
		}
	}

	public static TaskZhangJieVO GetTaskZhangJieVO(int taskZhangJieID)
	{
		TaskZhangJieVO result = null;
		if (ConfigTasks.TaskZhangJieXmlNodeDict.TryGetValue(taskZhangJieID, ref result))
		{
			return result;
		}
		return null;
	}

	public static void PreCacheTaskZhangJieXmlNodes()
	{
		ConfigTasks.TaskZhangJieXmlNodeDict.Clear();
		string text = "Config/TaskZhangJie.Xml";
		string text2 = "IsolateRes_VO";
		text = XmlManager.GetOnlyFileName(text);
		AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text2);
		if (null == assetBundle)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 缓存中没找到 {0}"), text2));
			return;
		}
		TextAsset textAsset = assetBundle.LoadAsset(text) as TextAsset;
		if (null == textAsset)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("ISOLATE_RES_NAME_VO异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text));
			return;
		}
		string text3 = textAsset.text;
		XElement xelement = XElement.Parse(text3);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "ZhangJie");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			TaskZhangJieVO taskZhangJieVO = new TaskZhangJieVO();
			taskZhangJieVO.CopyFrom(xelementList[i]);
			ConfigTasks.TaskZhangJieXmlNodeDict[taskZhangJieVO.ID] = taskZhangJieVO;
			if (ConfigTasks.MinZhangJieID == 0)
			{
				ConfigTasks.MinZhangJieID = taskZhangJieVO.ID;
			}
			if (ConfigTasks.MaxZhangJieID < taskZhangJieVO.ID)
			{
				ConfigTasks.MaxZhangJieID = taskZhangJieVO.ID;
			}
		}
	}

	public static TaskVO GetTaskXmlNodeByID(int id)
	{
		if (ConfigTasks.TaskXmlNodeDict.Count <= 0)
		{
			ConfigTasks.PreCacheTaskXmlNodesEx();
		}
		TaskVO result = null;
		if (ConfigTasks.TaskXmlNodeDict.TryGetValue(id, ref result))
		{
			return result;
		}
		return result;
	}

	public static TaskVO FindTaskXmlNodeByTaskClass(int taskClass)
	{
		if (ConfigTasks.TaskXmlNodeDict.Count <= 0)
		{
			ConfigTasks.PreCacheTaskXmlNodesEx();
		}
		TaskVO result = null;
		foreach (KeyValuePair<int, TaskVO> keyValuePair in ConfigTasks.TaskXmlNodeDict)
		{
			if (keyValuePair.Value.TaskClass == taskClass)
			{
				result = keyValuePair.Value;
				break;
			}
		}
		return result;
	}

	public static void ClearData()
	{
		ConfigTasks.TaskXmlNodeDict.Clear();
		ConfigTasks.TaskZhangJieXmlNodeDict.Clear();
	}

	public const string GAME_CONFIG_TASKS_FILE = "Config/SystemTasks.Xml";

	public const string GAME_CONFIG_TASKS_FILE_BIN = "Config/SystemTasksBin.txt";

	public const string GAME_CONFIG_TASKS_NAME = "ConfigSystemTasks";

	public const string GAME_CONFIG_TASKZHANGJIE_FILE = "Config/TaskZhangJie.Xml";

	public const string GAME_CONFIG_TASKZHANGJIE_NAME = "ConfigSystemTaskZhangJie";

	public static Dictionary<int, TaskVO> TaskXmlNodeDict = new Dictionary<int, TaskVO>();

	public static Dictionary<int, TaskZhangJieVO> TaskZhangJieXmlNodeDict = new Dictionary<int, TaskZhangJieVO>();
}
