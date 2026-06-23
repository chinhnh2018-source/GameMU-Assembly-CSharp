using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigTeamCompete : IConfigbase<ConfigTeamCompete>, ConfigBase
{
	public ConfigTeamCompete()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	private void ParseDuanWeiXML()
	{
		if (this.DictDuanWeiCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.DuanWeiXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "TeamDuanWei");
		for (int i = 0; i < xelementList.Count; i++)
		{
			DuanWeiVO duanWeiVO = new DuanWeiVO(xelementList[i]);
			this.DictDuanWeiCfg.Add(duanWeiVO.ID, duanWeiVO);
		}
	}

	public DuanWeiVO GetDuanWeiXmlDataById(int id)
	{
		this.ParseDuanWeiXML();
		foreach (KeyValuePair<int, DuanWeiVO> keyValuePair in this.DictDuanWeiCfg)
		{
			if (keyValuePair.Key == id)
			{
				Dictionary<int, DuanWeiVO>.Enumerator enumerator;
				KeyValuePair<int, DuanWeiVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value;
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetDuanWeiXmlDataById() 有误! id " + id
		});
		return null;
	}

	public DuanWeiVO GetDuanWeiXmlDataByTypeAndLevel(int type, int level)
	{
		this.ParseDuanWeiXML();
		foreach (KeyValuePair<int, DuanWeiVO> keyValuePair in this.DictDuanWeiCfg)
		{
			if (keyValuePair.Value.Type == type)
			{
				Dictionary<int, DuanWeiVO>.Enumerator enumerator;
				KeyValuePair<int, DuanWeiVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.Level == level)
				{
					KeyValuePair<int, DuanWeiVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value;
				}
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			string.Concat(new object[]
			{
				"GetDuanWeiXmlDataByTypeAndLevel() 有误! type ",
				type,
				"    level ",
				level
			})
		});
		return null;
	}

	public ChangeableRulePart.RuleXml GetTeamCompeteHelpInfo(int index = 0)
	{
		string helpInfo = this.GetHelpInfo(index);
		XElement gameResXml = Global.GetGameResXml(helpInfo);
		if (gameResXml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				helpInfo + Global.GetLang("配置加载出错")
			});
			return null;
		}
		this.mTeamCompeteHelpInfo = new ChangeableRulePart.RuleXml(gameResXml);
		return this.mTeamCompeteHelpInfo;
	}

	private string GetHelpInfo(int index)
	{
		if (index == 0)
		{
			return "Config/TeamBattleIntro.xml";
		}
		if (index == 1)
		{
			return "Config/TeamMatchIntro.xml";
		}
		if (index == 2)
		{
			return "Config/EscapeHelpIntro.xml";
		}
		return string.Empty;
	}

	private void ParseTeamMatchXML()
	{
		if (this.DictTeamMatchCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.TeamMatchPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "TeamMatch");
		for (int i = 0; i < xelementList.Count; i++)
		{
			TeamMatchVo teamMatchVo = new TeamMatchVo(xelementList[i]);
			this.DictTeamMatchCfg.Add(teamMatchVo.ID, teamMatchVo);
		}
	}

	public Dictionary<int, TeamMatchVo> GetTeamMatchXMLDict()
	{
		this.ParseTeamMatchXML();
		return this.DictTeamMatchCfg;
	}

	public TeamMatchVo GetTeamMatchXMLDataById(int id)
	{
		this.ParseTeamMatchXML();
		foreach (KeyValuePair<int, TeamMatchVo> keyValuePair in this.DictTeamMatchCfg)
		{
			if (keyValuePair.Key == id)
			{
				Dictionary<int, TeamMatchVo>.Enumerator enumerator;
				KeyValuePair<int, TeamMatchVo> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value;
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetTeamMatchXMLDataById() 有误! id " + id
		});
		return null;
	}

	public string GetJinJiDesById(int id)
	{
		this.ParseTeamMatchXML();
		foreach (KeyValuePair<int, TeamMatchVo> keyValuePair in this.DictTeamMatchCfg)
		{
			if (keyValuePair.Key == id)
			{
				Dictionary<int, TeamMatchVo>.Enumerator enumerator;
				KeyValuePair<int, TeamMatchVo> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value.Name;
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetTeamMatchXMLDataById() 有误! id " + id
		});
		return null;
	}

	public string GetTeamMatchJinDuByTimePoint(string timePoint)
	{
		this.ParseTeamMatchXML();
		foreach (KeyValuePair<int, TeamMatchVo> keyValuePair in this.DictTeamMatchCfg)
		{
			if (keyValuePair.Value.TimePoints.Split(new char[]
			{
				','
			})[1].Split(new char[]
			{
				'-'
			})[0].Equals(timePoint))
			{
				Dictionary<int, TeamMatchVo>.Enumerator enumerator;
				KeyValuePair<int, TeamMatchVo> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value.Name;
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetTeamMatchJinDuByTimePoint() 有误! timePoint " + timePoint
		});
		return null;
	}

	public string GetTeamMatchGuessTimePoint()
	{
		this.ParseTeamMatchXML();
		Dictionary<int, TeamMatchVo>.Enumerator enumerator = this.DictTeamMatchCfg.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return null;
		}
		KeyValuePair<int, TeamMatchVo> keyValuePair = enumerator.Current;
		return keyValuePair.Value.LotteryTime;
	}

	public int GetTeamMatchGuessCostJinBi()
	{
		this.ParseTeamMatchXML();
		Dictionary<int, TeamMatchVo>.Enumerator enumerator = this.DictTeamMatchCfg.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return 0;
		}
		KeyValuePair<int, TeamMatchVo> keyValuePair = enumerator.Current;
		return keyValuePair.Value.LotteryMoney;
	}

	private void ParseTeamMatchAwardXML()
	{
		if (this.DictTeamMatchAwardCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.TeamMatchAwardPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "TeamMatchAward");
		for (int i = 0; i < xelementList.Count; i++)
		{
			TeamMatchAwardVo teamMatchAwardVo = new TeamMatchAwardVo(xelementList[i]);
			this.DictTeamMatchAwardCfg.Add(teamMatchAwardVo.ID, teamMatchAwardVo);
		}
	}

	public Dictionary<int, TeamMatchAwardVo> GetTeamMatchAwardDict()
	{
		this.ParseTeamMatchAwardXML();
		return this.DictTeamMatchAwardCfg;
	}

	private void ParseDaTaoShaAwardXML()
	{
		if (this.DictDaTaoShaAwardCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.DaTaoShaAwardPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Auction");
		for (int i = 0; i < xelementList.Count; i++)
		{
			TeamMatchAwardVo teamMatchAwardVo = new TeamMatchAwardVo(xelementList[i]);
			this.DictDaTaoShaAwardCfg.Add(teamMatchAwardVo.ID, teamMatchAwardVo);
		}
	}

	public Dictionary<int, TeamMatchAwardVo> GetDaTaoShaAwardDict()
	{
		this.ParseDaTaoShaAwardXML();
		return this.DictDaTaoShaAwardCfg;
	}

	private void ParseDaTaoShaDuanWeiXML()
	{
		if (this.DictDaTaoShaDuanWeiCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.DaTaoShaDuanWeiPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
		for (int i = 0; i < xelementList.Count; i++)
		{
			TeamMatchAwardVo teamMatchAwardVo = new TeamMatchAwardVo(xelementList[i]);
			this.DictDaTaoShaDuanWeiCfg.Add(teamMatchAwardVo.ID, teamMatchAwardVo);
		}
	}

	public Dictionary<int, TeamMatchAwardVo> GetDaTaoShaDuanWeiDict()
	{
		this.ParseDaTaoShaDuanWeiXML();
		return this.DictDaTaoShaDuanWeiCfg;
	}

	public void ClearXMLData(byte clearType)
	{
		this.mTeamCompeteHelpInfo = null;
		this.DictDuanWeiCfg.Clear();
		this.DictTeamMatchCfg.Clear();
		this.DictTeamMatchAwardCfg.Clear();
		this.DictDaTaoShaAwardCfg.Clear();
		this.DictDaTaoShaDuanWeiCfg.Clear();
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	private string DuanWeiXMLPath = "Config/TeamDuanWei.xml";

	private Dictionary<int, DuanWeiVO> DictDuanWeiCfg = new Dictionary<int, DuanWeiVO>();

	private ChangeableRulePart.RuleXml mTeamCompeteHelpInfo;

	private string TeamMatchPath = "Config/TeamMatch.xml";

	private Dictionary<int, TeamMatchVo> DictTeamMatchCfg = new Dictionary<int, TeamMatchVo>();

	private string TeamMatchAwardPath = "Config/TeamMatchAward.xml";

	private Dictionary<int, TeamMatchAwardVo> DictTeamMatchAwardCfg = new Dictionary<int, TeamMatchAwardVo>();

	private string DaTaoShaAwardPath = "Config/EscapeRankAward.xml";

	private Dictionary<int, TeamMatchAwardVo> DictDaTaoShaAwardCfg = new Dictionary<int, TeamMatchAwardVo>();

	private string DaTaoShaDuanWeiPath = "Config/EscapeDanList.xml";

	private Dictionary<int, TeamMatchAwardVo> DictDaTaoShaDuanWeiCfg = new Dictionary<int, TeamMatchAwardVo>();
}
