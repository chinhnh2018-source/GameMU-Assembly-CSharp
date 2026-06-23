using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RedPointManager
{
	public static void ClearData()
	{
		RedPointManager.dict.Clear();
	}

	public static void Load()
	{
		XElement gameResXml = Global.GetGameResXml("Config/RedPoint.xml");
		if (gameResXml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"没有找到配置文件:Config/RedPoint.xml"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "RedPoint");
		if (xelementList != null && xelementList.Count > 0)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				RedPointVO redPointVO = new RedPointVO(xelementList[i]);
				if (!RedPointManager.dict.ContainsKey(redPointVO.Type))
				{
					RedPointManager.dict.Add(redPointVO.Type, redPointVO);
				}
			}
		}
	}

	public static RedPointVO GetDataByType(RedPointType type)
	{
		if (RedPointManager.dict.Count <= 0)
		{
			RedPointManager.Load();
		}
		if (RedPointManager.dict.Count <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"RedPoint.XML 解析有误！！！"
			});
			return null;
		}
		return RedPointManager.dict[(int)type];
	}

	private static Dictionary<int, RedPointVO> dict = new Dictionary<int, RedPointVO>();
}
