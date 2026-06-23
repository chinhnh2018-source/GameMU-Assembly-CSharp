using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ConfigZhanMengLianSaiLeagueSuperAward
{
	public ConfigZhanMengLianSaiLeagueSuperAward()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LeagueSuperAward.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueSuperAward");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					LianSaiLeagueSuperAwardVO lianSaiLeagueSuperAwardVO = new LianSaiLeagueSuperAwardVO(xelementList[i]);
					this.dic.Add(lianSaiLeagueSuperAwardVO.ID, lianSaiLeagueSuperAwardVO);
				}
			}
		}
	}

	public LianSaiLeagueSuperAwardVO GetLianSaiLeagueAwardVOByID(int ID)
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

	public List<GoodsData> GetLianSaiAwardByID(int rank)
	{
		foreach (KeyValuePair<int, LianSaiLeagueSuperAwardVO> keyValuePair in this.dic)
		{
			int beginNum = keyValuePair.Value.BeginNum;
			Dictionary<int, LianSaiLeagueSuperAwardVO>.Enumerator enumerator;
			KeyValuePair<int, LianSaiLeagueSuperAwardVO> keyValuePair2 = enumerator.Current;
			int endNum = keyValuePair2.Value.EndNum;
			if (endNum == -1)
			{
				if (beginNum <= rank)
				{
					KeyValuePair<int, LianSaiLeagueSuperAwardVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value.Goods;
				}
			}
			else if (beginNum == endNum)
			{
				if (beginNum == rank)
				{
					KeyValuePair<int, LianSaiLeagueSuperAwardVO> keyValuePair4 = enumerator.Current;
					return keyValuePair4.Value.Goods;
				}
			}
			else if (beginNum <= rank && rank >= endNum)
			{
				KeyValuePair<int, LianSaiLeagueSuperAwardVO> keyValuePair5 = enumerator.Current;
				return keyValuePair5.Value.Goods;
			}
		}
		return null;
	}

	public List<int> GetAllAwardID()
	{
		List<int> list = new List<int>();
		Dictionary<int, LianSaiLeagueSuperAwardVO>.Enumerator enumerator = this.dic.GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<int> list2 = list;
			KeyValuePair<int, LianSaiLeagueSuperAwardVO> keyValuePair = enumerator.Current;
			list2.Add(keyValuePair.Key);
		}
		return list;
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	private const string path = "Config/LeagueSuperAward.xml";

	private Dictionary<int, LianSaiLeagueSuperAwardVO> dic = new Dictionary<int, LianSaiLeagueSuperAwardVO>();
}
