using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ConfigZhanMengLianSaiLeagueMatch
{
	public ConfigZhanMengLianSaiLeagueMatch()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LeagueMatch.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueMatch");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					LianSaiLeagueMatchVO lianSaiLeagueMatchVO = new LianSaiLeagueMatchVO(xelementList[i]);
					this.dic.Add(lianSaiLeagueMatchVO.ID, lianSaiLeagueMatchVO);
				}
			}
		}
	}

	public LianSaiLeagueMatchVO GetLianSaiLeagueMatchVOByID(int ID)
	{
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		MUDebug.LogError<string>(new string[]
		{
			"获取LeagueMatch XML 数据失败 ID= " + ID
		});
		return null;
	}

	public List<LianSaiLeagueMatchVO.TimePoint> GetLianSaiOpenAndCloseDateTimeByID(int ID)
	{
		LianSaiLeagueMatchVO lianSaiLeagueMatchVOByID = this.GetLianSaiLeagueMatchVOByID(ID);
		if (lianSaiLeagueMatchVOByID != null)
		{
			return lianSaiLeagueMatchVOByID.TimePoints;
		}
		return null;
	}

	public List<GoodsData> GetLianSaiAwardGoods(int ID, byte Win = 0)
	{
		LianSaiLeagueMatchVO lianSaiLeagueMatchVOByID = this.GetLianSaiLeagueMatchVOByID(ID);
		if (lianSaiLeagueMatchVOByID == null)
		{
			return null;
		}
		if (Win == 0)
		{
			return lianSaiLeagueMatchVOByID.WinGoods;
		}
		return lianSaiLeagueMatchVOByID.LoseGoods;
	}

	public int[] GetLianSaiMapCodeByID(int ID)
	{
		LianSaiLeagueMatchVO lianSaiLeagueMatchVOByID = this.GetLianSaiLeagueMatchVOByID(ID);
		if (lianSaiLeagueMatchVOByID != null)
		{
			return lianSaiLeagueMatchVOByID.MapCode;
		}
		return null;
	}

	public bool ThisTimeISInMath(int Type)
	{
		List<LianSaiLeagueMatchVO.TimePoint> lianSaiOpenAndCloseDateTimeByID = this.GetLianSaiOpenAndCloseDateTimeByID(Type);
		if (lianSaiOpenAndCloseDateTimeByID != null)
		{
			for (int i = 0; i < lianSaiOpenAndCloseDateTimeByID.Count; i++)
			{
				LianSaiLeagueMatchVO.TimePoint timePoint = lianSaiOpenAndCloseDateTimeByID[i];
				if (timePoint.Week == Global.GetCorrectDateTime().DayOfWeek && this.CheckTime(timePoint.Time1, timePoint.Time2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CheckTime(DateTime time1, DateTime time2)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (time1.Hour < correctDateTime.Hour && correctDateTime.Hour < time2.Hour)
		{
			return true;
		}
		if (time2.Hour == correctDateTime.Hour)
		{
			return time2.Minute > correctDateTime.Minute;
		}
		return time1.Hour == correctDateTime.Hour && time1.Minute <= correctDateTime.Minute;
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	private const string path = "Config/LeagueMatch.xml";

	private Dictionary<int, LianSaiLeagueMatchVO> dic = new Dictionary<int, LianSaiLeagueMatchVO>();
}
