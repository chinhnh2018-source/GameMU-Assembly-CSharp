using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigZhanMengLianSaiLeagueNewHelpIntro
{
	public ConfigZhanMengLianSaiLeagueNewHelpIntro()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LeagueNewIntro.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					LianSaiLeagueSuperHelpInfVO lianSaiLeagueSuperHelpInfVO = new LianSaiLeagueSuperHelpInfVO(xelementList[i]);
					this.dic.Add(lianSaiLeagueSuperHelpInfVO.ID, lianSaiLeagueSuperHelpInfVO);
				}
			}
		}
	}

	public LianSaiLeagueSuperHelpInfVO GetLianSaiVOByID(int ID)
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

	public Dictionary<int, LianSaiLeagueSuperHelpInfVO>.Enumerator GetEnumerator()
	{
		return this.dic.GetEnumerator();
	}

	public void ClearData()
	{
		this.dic.Clear();
	}

	private const string path = "Config/LeagueNewIntro.xml";

	private Dictionary<int, LianSaiLeagueSuperHelpInfVO> dic = new Dictionary<int, LianSaiLeagueSuperHelpInfVO>();
}
