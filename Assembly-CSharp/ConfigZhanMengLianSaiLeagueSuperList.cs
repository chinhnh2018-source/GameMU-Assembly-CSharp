using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigZhanMengLianSaiLeagueSuperList
{
	public ConfigZhanMengLianSaiLeagueSuperList()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LeagueSuperList.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueSuperLIst");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					LianSaiLeagueSuperListVO lianSaiLeagueSuperListVO = new LianSaiLeagueSuperListVO(xelementList[i]);
					this.dic.Add(lianSaiLeagueSuperListVO.ID, lianSaiLeagueSuperListVO);
				}
			}
		}
	}

	public LianSaiLeagueSuperListVO GetLianSaiLeagueOpenVOByID(int ID)
	{
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		MUDebug.LogError<string>(new string[]
		{
			"获取LeagueSuperList XML 数据失败 ID= " + ID
		});
		return null;
	}

	public int[] GetLianSaiDateTimeByID(int ID, int mathIndex)
	{
		LianSaiLeagueSuperListVO lianSaiLeagueOpenVOByID = this.GetLianSaiLeagueOpenVOByID(ID);
		if (lianSaiLeagueOpenVOByID != null)
		{
			if (mathIndex == 1)
			{
				return lianSaiLeagueOpenVOByID.Match1;
			}
			if (mathIndex == 2)
			{
				return lianSaiLeagueOpenVOByID.Match2;
			}
			if (mathIndex == 3)
			{
				return lianSaiLeagueOpenVOByID.Match3;
			}
			if (mathIndex == 4)
			{
				return lianSaiLeagueOpenVOByID.Match4;
			}
		}
		return null;
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	private const string path = "Config/LeagueSuperList.xml";

	private Dictionary<int, LianSaiLeagueSuperListVO> dic = new Dictionary<int, LianSaiLeagueSuperListVO>();
}
