using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigMoYuDuoBao : IConfigbase<ConfigMoYuDuoBao>, ConfigBase
{
	public ConfigMoYuDuoBao()
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
		this.m_DictZorkSceneVOCfg.Clear();
		this.m_DictZorkMonsterVOCfg.Clear();
		this.m_DictZorkDanAwardVOCfg.Clear();
		this.m_DictZorkActivityRulesVOCfg.Clear();
		this.m_DictZorkAchievementVOCfg.Clear();
		this.m_LstZorkAchievementVOCfg.Clear();
		this.mMoYuHelpHelpInfo = null;
	}

	public List<ZorkAchievementVO> LstZorkAchievementVOCfg
	{
		get
		{
			this.ParseZorkAchievementVOXML();
			return this.m_LstZorkAchievementVOCfg;
		}
	}

	private void ParseZorkAchievementVOXML()
	{
		if (this.m_DictZorkAchievementVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ZorkAchievementVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZorkAchievement");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZorkAchievementVO zorkAchievementVO = new ZorkAchievementVO(xelementList[i]);
			this.m_DictZorkAchievementVOCfg.Add(zorkAchievementVO.ID, zorkAchievementVO);
			this.m_LstZorkAchievementVOCfg.Add(zorkAchievementVO);
		}
	}

	public ZorkAchievementVO GetZorkAchievementVODataById(int id)
	{
		this.ParseZorkAchievementVOXML();
		ZorkAchievementVO result = null;
		if (this.m_DictZorkAchievementVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetZorkAchievementVODataById() 有误! id " + id
		});
		return null;
	}

	private void ParseZorkActivityRulesVOXML()
	{
		if (this.m_DictZorkActivityRulesVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ZorkActivityRulesVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZorkActivityRules");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZorkActivityRulesVO zorkActivityRulesVO = new ZorkActivityRulesVO(xelementList[i]);
			this.m_DictZorkActivityRulesVOCfg.Add(zorkActivityRulesVO.ID, zorkActivityRulesVO);
		}
	}

	public ZorkActivityRulesVO GetZorkActivityRulesVODataById(int id)
	{
		this.ParseZorkActivityRulesVOXML();
		ZorkActivityRulesVO result = null;
		if (this.m_DictZorkActivityRulesVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetZorkActivityRulesVODataById() 有误! id " + id
		});
		return null;
	}

	public Dictionary<int, ZorkDanAwardVO> DictZorkDanAwardVOCfg
	{
		get
		{
			if (this.m_DictZorkDanAwardVOCfg.Count == 0)
			{
				this.ParseZorkDanAwardVOXML();
			}
			return this.m_DictZorkDanAwardVOCfg;
		}
	}

	private void ParseZorkDanAwardVOXML()
	{
		if (this.m_DictZorkDanAwardVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ZorkDanAwardVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZorkDanAward");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZorkDanAwardVO zorkDanAwardVO = new ZorkDanAwardVO(xelementList[i]);
			this.m_DictZorkDanAwardVOCfg.Add(zorkDanAwardVO.ID, zorkDanAwardVO);
		}
	}

	public ZorkDanAwardVO GetZorkDanAwardVODataById(int id)
	{
		ZorkDanAwardVO result = null;
		if (this.DictZorkDanAwardVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetZorkDanAwardVODataById() 有误! id " + id
		});
		return null;
	}

	public ZorkDanAwardVO GetZorkDanAwardVODataByScore(int score)
	{
		for (int i = 5; i > 0; i--)
		{
			ZorkDanAwardVO zorkDanAwardVODataById = this.GetZorkDanAwardVODataById(i);
			if (score >= zorkDanAwardVODataById.RankValue)
			{
				return zorkDanAwardVODataById;
			}
		}
		return this.GetZorkDanAwardVODataById(1);
	}

	public Dictionary<int, ZorkMonsterVO> DictZorkMonsterVOCfg
	{
		get
		{
			if (this.m_DictZorkMonsterVOCfg.Count == 0)
			{
				this.ParseZorkMonsterVOXML();
			}
			return this.m_DictZorkMonsterVOCfg;
		}
	}

	private void ParseZorkMonsterVOXML()
	{
		if (this.m_DictZorkMonsterVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ZorkMonsterVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZorkMonster");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZorkMonsterVO zorkMonsterVO = new ZorkMonsterVO(xelementList[i]);
			this.m_DictZorkMonsterVOCfg.Add(zorkMonsterVO.ID, zorkMonsterVO);
			if (zorkMonsterVO.MonsterType == 3)
			{
				this.m_bossVo = zorkMonsterVO;
			}
		}
	}

	public ZorkMonsterVO GetZorkMonsterVODataById(int id)
	{
		ZorkMonsterVO result = null;
		if (this.DictZorkMonsterVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetZorkMonsterVODataById() 有误! id " + id
		});
		return null;
	}

	public ZorkMonsterVO GetBossInfo()
	{
		this.ParseZorkMonsterVOXML();
		return this.m_bossVo;
	}

	public Dictionary<int, ZorkSceneVO> DictZorkSceneVOCfg
	{
		get
		{
			if (this.m_DictZorkSceneVOCfg.Count == 0)
			{
				this.ParseZorkSceneVOXML();
			}
			return this.m_DictZorkSceneVOCfg;
		}
	}

	private void ParseZorkSceneVOXML()
	{
		if (this.m_DictZorkSceneVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.ZorkSceneVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZorkScene");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZorkSceneVO zorkSceneVO = new ZorkSceneVO(xelementList[i]);
			this.m_DictZorkSceneVOCfg.Add(zorkSceneVO.BuffAreID, zorkSceneVO);
		}
	}

	public ZorkSceneVO GetZorkSceneVODataById(int id)
	{
		ZorkSceneVO result = null;
		if (this.DictZorkSceneVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetZorkSceneVODataById() 有误! id " + id
		});
		return null;
	}

	public ChangeableRulePart.RuleXml GetMoYuHelpHelpInfo()
	{
		if (this.mMoYuHelpHelpInfo == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ZorkHelpIntro.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/ZorkHelpIntro.xml 出现错误"
				});
				return null;
			}
			this.mMoYuHelpHelpInfo = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return this.mMoYuHelpHelpInfo;
	}

	private string ZorkAchievementVOXMLPath = "Config/ZorkAchievement.xml";

	private Dictionary<int, ZorkAchievementVO> m_DictZorkAchievementVOCfg = new Dictionary<int, ZorkAchievementVO>();

	private List<ZorkAchievementVO> m_LstZorkAchievementVOCfg = new List<ZorkAchievementVO>();

	private string ZorkActivityRulesVOXMLPath = "Config/ZorkActivityRules.xml";

	private Dictionary<int, ZorkActivityRulesVO> m_DictZorkActivityRulesVOCfg = new Dictionary<int, ZorkActivityRulesVO>();

	private string ZorkDanAwardVOXMLPath = "Config/ZorkDanAward.xml";

	private Dictionary<int, ZorkDanAwardVO> m_DictZorkDanAwardVOCfg = new Dictionary<int, ZorkDanAwardVO>();

	private string ZorkMonsterVOXMLPath = "Config/ZorkMonster.xml";

	private Dictionary<int, ZorkMonsterVO> m_DictZorkMonsterVOCfg = new Dictionary<int, ZorkMonsterVO>();

	private ZorkMonsterVO m_bossVo;

	private string ZorkSceneVOXMLPath = "Config/ZorkScene.xml";

	private Dictionary<int, ZorkSceneVO> m_DictZorkSceneVOCfg = new Dictionary<int, ZorkSceneVO>();

	private ChangeableRulePart.RuleXml mMoYuHelpHelpInfo;
}
