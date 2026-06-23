using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigZhanMengLianSaiLeagueOpen
{
	public ConfigZhanMengLianSaiLeagueOpen()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LeagueOpen.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueOpen");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					LianSaiLeagueOpenVO lianSaiLeagueOpenVO = new LianSaiLeagueOpenVO(xelementList[i]);
					this.dic.Add(lianSaiLeagueOpenVO.ID, lianSaiLeagueOpenVO);
				}
			}
		}
	}

	public LianSaiLeagueOpenVO GetLianSaiLeagueOpenVOByID(int ID)
	{
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		MUDebug.LogError<string>(new string[]
		{
			"获取LeagueOpen XML 数据失败 ID= " + ID
		});
		return null;
	}

	public DateTime GetLianSaiDateTimeByID(int ID)
	{
		LianSaiLeagueOpenVO lianSaiLeagueOpenVOByID = this.GetLianSaiLeagueOpenVOByID(ID);
		if (lianSaiLeagueOpenVOByID != null)
		{
			return lianSaiLeagueOpenVOByID.OpenDate;
		}
		return Global.GetCorrectDateTime();
	}

	public DateTime GetLianSaiOpenDateTime()
	{
		return this.GetLianSaiDateTimeByID(this.GetPlatformId());
	}

	private int GetPlatformId()
	{
		int result;
		if (Global.PingTaiName != "YYB")
		{
			result = 3;
		}
		else
		{
			result = 4;
		}
		return result;
	}

	public bool GetLianSaiISOpenOnPlatform()
	{
		LianSaiLeagueOpenVO lianSaiLeagueOpenVOByID = this.GetLianSaiLeagueOpenVOByID(this.GetPlatformId());
		return lianSaiLeagueOpenVOByID != null && lianSaiLeagueOpenVOByID.PingTaiISOpen;
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	private const string path = "Config/LeagueOpen.xml";

	private Dictionary<int, LianSaiLeagueOpenVO> dic = new Dictionary<int, LianSaiLeagueOpenVO>();
}
