using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class AnnouncedSetVO
{
	public static void Load()
	{
		if (AnnouncedSetVO.isLoad)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/SystemTrailer.xml");
		if (gameResXml == null)
		{
			Debug.LogError(Global.GetLang("没有找到配置文件:") + "Config/SystemTrailer.xml");
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "SystemTrailer");
		if (xelementList != null && xelementList.Count > 0)
		{
			if (Global.Data.roleData.CompletedMainTaskID >= Global.GetXElementAttributeInt(xelementList[xelementList.Count - 1], "TaskEnd"))
			{
				AnnouncedSetVO.Clear();
				return;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
				if (xelementAttributeInt >= 0 && !AnnouncedSetVO.annocedDir.ContainsKey(xelementAttributeInt))
				{
					AnnouncedSetVO announcedSetVO = new AnnouncedSetVO();
					announcedSetVO.ID = xelementAttributeInt;
					announcedSetVO.name = Global.GetXElementAttributeStr(xelementList[i], "Name");
					announcedSetVO.Icon = Global.GetXElementAttributeStr(xelementList[i], "Icon");
					announcedSetVO.TaskBegin = Global.GetXElementAttributeInt(xelementList[i], "TaskBegin");
					announcedSetVO.TaskEnd = Global.GetXElementAttributeInt(xelementList[i], "TaskEnd");
					announcedSetVO.Note = Global.GetXElementAttributeStr(xelementList[i], "Note");
					AnnouncedSetVO.annocedDir.Add(xelementAttributeInt, announcedSetVO);
				}
			}
		}
		AnnouncedSetVO.isLoad = true;
	}

	public static int GetTaskSum(int taskBegin, int taskEnd, bool isNew = false)
	{
		if (isNew)
		{
			AnnouncedSetVO.sum = 1;
		}
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskBegin);
		if (taskXmlNodeByID != null)
		{
			if (taskXmlNodeByID.NextTask > taskEnd)
			{
				return AnnouncedSetVO.sum;
			}
			AnnouncedSetVO.sum++;
			AnnouncedSetVO.GetTaskSum(taskXmlNodeByID.NextTask, taskEnd, false);
		}
		return AnnouncedSetVO.sum;
	}

	public static string[] SplitPicName(string pictureName)
	{
		if (string.IsNullOrEmpty(pictureName))
		{
			return null;
		}
		return pictureName.Split(new char[]
		{
			'|'
		});
	}

	public static string SplitName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		return name.Split(new char[]
		{
			','
		})[1];
	}

	public static AnnouncedSetVO GetVOByID(int id)
	{
		if (AnnouncedSetVO.annocedDir == null || AnnouncedSetVO.annocedDir.Count <= 0)
		{
			AnnouncedSetVO.Load();
		}
		AnnouncedSetVO result = null;
		if (AnnouncedSetVO.annocedDir != null && AnnouncedSetVO.annocedDir.Count > 0)
		{
			foreach (KeyValuePair<int, AnnouncedSetVO> keyValuePair in AnnouncedSetVO.annocedDir)
			{
				if (keyValuePair.Value.TaskBegin <= id)
				{
					Dictionary<int, AnnouncedSetVO>.Enumerator enumerator;
					KeyValuePair<int, AnnouncedSetVO> keyValuePair2 = enumerator.Current;
					if (id <= keyValuePair2.Value.TaskEnd)
					{
						KeyValuePair<int, AnnouncedSetVO> keyValuePair3 = enumerator.Current;
						result = keyValuePair3.Value;
						break;
					}
				}
			}
		}
		return result;
	}

	public static void Clear()
	{
		if (AnnouncedSetVO.annocedDir != null)
		{
			AnnouncedSetVO.annocedDir.Clear();
		}
		AnnouncedSetVO.curTaskID = -1;
	}

	public static void Reset()
	{
		AnnouncedSetVO.curTaskID = -1;
		AnnouncedSetVO.annocedDir = new Dictionary<int, AnnouncedSetVO>();
		AnnouncedSetVO.isLoad = false;
	}

	public static void ClearXMLData()
	{
		AnnouncedSetVO.Clear();
		AnnouncedSetVO.Reset();
	}

	public int ID;

	public string name;

	public string Icon;

	public int TaskBegin;

	public int TaskEnd;

	public string Note;

	public static Dictionary<int, AnnouncedSetVO> annocedDir = new Dictionary<int, AnnouncedSetVO>();

	public static int curTaskID = -1;

	private static bool isLoad = false;

	private static int sum = 1;
}
