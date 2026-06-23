using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigDaTaoSha : IConfigbase<ConfigDaTaoSha>, ConfigBase
{
	public ConfigDaTaoSha()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.m_DictEscapeActivityRulesVOCfg.Clear();
		this.m_DictEscapeMapSafeAreaVoCfg.Clear();
		this.m_DictEscapeDanListVoCfg.Clear();
	}

	private void ParseEscapeActivityRulesVOXML()
	{
		if (this.m_DictEscapeActivityRulesVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.EscapeActivityRulesVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EscapeActivityRules");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EscapeActivityRulesVO escapeActivityRulesVO = new EscapeActivityRulesVO(xelementList[i]);
			this.m_DictEscapeActivityRulesVOCfg.Add(escapeActivityRulesVO.ID, escapeActivityRulesVO);
		}
	}

	public EscapeActivityRulesVO GetEscapeActivityRulesVODataById(int id)
	{
		this.ParseEscapeActivityRulesVOXML();
		EscapeActivityRulesVO result = null;
		if (this.m_DictEscapeActivityRulesVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetEscapeActivityRulesVODataById() 有误! id " + id
		});
		return null;
	}

	public string[] GetEscapeActivityRulesAwardID()
	{
		EscapeActivityRulesVO escapeActivityRulesVODataById = this.GetEscapeActivityRulesVODataById(1);
		return escapeActivityRulesVODataById.RewardView.Split(new char[]
		{
			'|'
		});
	}

	public string GetDaTaoShaActivityTime()
	{
		EscapeActivityRulesVO escapeActivityRulesVODataById = this.GetEscapeActivityRulesVODataById(1);
		return escapeActivityRulesVODataById.TimePoints;
	}

	public string GetDaTaoShaLimitLevel()
	{
		EscapeActivityRulesVO escapeActivityRulesVODataById = this.GetEscapeActivityRulesVODataById(1);
		return escapeActivityRulesVODataById.SignCondition;
	}

	private void ParseEscapeMapSafeAreaVoXML()
	{
		if (this.m_DictEscapeMapSafeAreaVoCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.EscapeMapSafeAreaXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EscapeMapSafeArea");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EscapeMapSafeAreaVo escapeMapSafeAreaVo = new EscapeMapSafeAreaVo(xelementList[i]);
			this.m_DictEscapeMapSafeAreaVoCfg.Add(escapeMapSafeAreaVo.ID, escapeMapSafeAreaVo);
		}
	}

	public EscapeMapSafeAreaVo GetEscapeMapSafeAreaVoDataById(int id)
	{
		this.ParseEscapeMapSafeAreaVoXML();
		EscapeMapSafeAreaVo result = null;
		if (this.m_DictEscapeMapSafeAreaVoCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"EscapeMapSafeAreaVoCfg() 有误! id " + id
		});
		return null;
	}

	public float GetEscapeMapSafeAreaRadiusById(int id)
	{
		this.ParseEscapeMapSafeAreaVoXML();
		EscapeMapSafeAreaVo escapeMapSafeAreaVo = null;
		if (this.m_DictEscapeMapSafeAreaVoCfg.TryGetValue(id, ref escapeMapSafeAreaVo))
		{
			return (float)escapeMapSafeAreaVo.SafeRadius;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetEscapeMapSafeAreaRadiusById() 有误! id " + id
		});
		return 0f;
	}

	public int GetEffectCircleResNameById(int id)
	{
		this.ParseEscapeMapSafeAreaVoXML();
		EscapeMapSafeAreaVo escapeMapSafeAreaVo = null;
		if (this.m_DictEscapeMapSafeAreaVoCfg.TryGetValue(id, ref escapeMapSafeAreaVo))
		{
			return escapeMapSafeAreaVo.GodEffictId;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetEscapeMapSafeAreaRadiusById() 有误! id " + id
		});
		return -1;
	}

	public bool IsMaxSafeArea(int id)
	{
		this.ParseEscapeMapSafeAreaVoXML();
		return false;
	}

	private void ParseEscapeDanListVoXML()
	{
		if (this.m_DictEscapeDanListVoCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.EscapeDanListVoXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EscapeDanList");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EscapeDanListVo escapeDanListVo = new EscapeDanListVo(xelementList[i]);
			this.m_DictEscapeDanListVoCfg.Add(escapeDanListVo.ID, escapeDanListVo);
		}
	}

	public EscapeDanListVo GeEscapeDanListVoDataById(int id)
	{
		this.ParseEscapeDanListVoXML();
		EscapeDanListVo result = null;
		if (this.m_DictEscapeDanListVoCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"EscapeDanListVoCfg() 有误! id " + id
		});
		return null;
	}

	private string EscapeActivityRulesVOXMLPath = "Config/EscapeActivityRules.xml";

	private Dictionary<int, EscapeActivityRulesVO> m_DictEscapeActivityRulesVOCfg = new Dictionary<int, EscapeActivityRulesVO>();

	private string EscapeMapSafeAreaXMLPath = "Config/EscapeMapSafeArea.xml";

	private Dictionary<int, EscapeMapSafeAreaVo> m_DictEscapeMapSafeAreaVoCfg = new Dictionary<int, EscapeMapSafeAreaVo>();

	private string EscapeDanListVoXMLPath = "Config/EscapeDanList.xml";

	private Dictionary<int, EscapeDanListVo> m_DictEscapeDanListVoCfg = new Dictionary<int, EscapeDanListVo>();
}
