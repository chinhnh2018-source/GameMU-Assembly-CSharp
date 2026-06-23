using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigSpecialPrompt
{
	private static void InitXml()
	{
		if (0 < ConfigSpecialPrompt.dic.Count)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/SpecialPrompt.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "SpecialPrompt");
			if (xelementList != null && 0 < xelementList.Count)
			{
				int i = 0;
				int count = xelementList.Count;
				while (i < count)
				{
					SpecialPromptVO specialPromptVO = new SpecialPromptVO(xelementList[i]);
					if (!ConfigSpecialPrompt.dic.ContainsKey(specialPromptVO.ID))
					{
						ConfigSpecialPrompt.dic[specialPromptVO.ID] = specialPromptVO;
					}
					else
					{
						ConfigSpecialPrompt.dic.Add(specialPromptVO.ID, specialPromptVO);
					}
					i++;
				}
			}
		}
	}

	public static int WeekToIntValue(DayOfWeek week)
	{
		if (week == null)
		{
			return 7;
		}
		return week;
	}

	public static int WeekToIntValue(int week)
	{
		if (week == 0)
		{
			return 7;
		}
		return week;
	}

	public static bool GetActivityCanShowByID(int ID)
	{
		ConfigSpecialPrompt.InitXml();
		SpecialPromptVO specialPromptVOByID = ConfigSpecialPrompt.GetSpecialPromptVOByID(ID);
		return specialPromptVOByID != null && specialPromptVOByID.ThisTimesIsInActivity;
	}

	public static SpecialPromptVO GetSpecialPromptVOByID(int id)
	{
		ConfigSpecialPrompt.InitXml();
		if (ConfigSpecialPrompt.dic.ContainsKey(id))
		{
			return ConfigSpecialPrompt.dic[id];
		}
		return null;
	}

	public static Dictionary<int, SpecialPromptVO>.Enumerator GetSprcialPrompeGetEnumerator()
	{
		ConfigSpecialPrompt.InitXml();
		return ConfigSpecialPrompt.dic.GetEnumerator();
	}

	public static void ClearData()
	{
		ConfigSpecialPrompt.dic.Clear();
	}

	private const string Path = "Config/SpecialPrompt.xml";

	private static Dictionary<int, SpecialPromptVO> dic = new Dictionary<int, SpecialPromptVO>();
}
