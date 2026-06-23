using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigMoShenMiBao : IConfigbase<ConfigMoShenMiBao>, ConfigBase
{
	public ConfigMoShenMiBao()
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
		this.m_DictMoShenMiBaoXingVOCfg.Clear();
		this.m_DictMoShenMiBaoJieVOCfg.Clear();
	}

	public Dictionary<int, MoShenMiBaoJieVO> DictMoShenMiBaoJieVOCfg
	{
		get
		{
			if (this.m_DictMoShenMiBaoJieVOCfg.Count == 0)
			{
				this.ParseMoShenMiBaoJieVOXML();
			}
			return this.m_DictMoShenMiBaoJieVOCfg;
		}
	}

	private void ParseMoShenMiBaoJieVOXML()
	{
		if (this.m_DictMoShenMiBaoJieVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.MoShenMiBaoJieVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "MoShenMiBaoJie");
		for (int i = 0; i < xelementList.Count; i++)
		{
			MoShenMiBaoJieVO moShenMiBaoJieVO = new MoShenMiBaoJieVO(xelementList[i]);
			int num = moShenMiBaoJieVO.MiBaoType * 1000000 + moShenMiBaoJieVO.MibaoStage * 1000;
			this.m_DictMoShenMiBaoJieVOCfg.Add(num, moShenMiBaoJieVO);
			if (i == xelementList.Count - 1)
			{
				this.m_maxStage = moShenMiBaoJieVO.MibaoStage;
			}
		}
	}

	public MoShenMiBaoJieVO GetMoShenMiBaoJieVOData(int type, int MibaoStage)
	{
		MoShenMiBaoJieVO result = null;
		int num = type * 1000000 + MibaoStage * 1000;
		if (this.DictMoShenMiBaoJieVOCfg.TryGetValue(num, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetMoShenMiBaoJieVODataById() 有误! id " + num
		});
		return null;
	}

	public int GetJieUpExp(int type, int MibaoStage)
	{
		MoShenMiBaoJieVO moShenMiBaoJieVOData = this.GetMoShenMiBaoJieVOData(type, MibaoStage);
		if (moShenMiBaoJieVOData == null)
		{
			return 1;
		}
		return 110000 - moShenMiBaoJieVOData.LuckyOne;
	}

	public int GetMaxStage()
	{
		this.ParseMoShenMiBaoJieVOXML();
		return this.m_maxStage;
	}

	private void ParseMoShenMiBaoXingVOXML()
	{
		if (this.m_DictMoShenMiBaoXingVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.MoShenMiBaoXingVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "MoShenMiBaoXing");
		for (int i = 0; i < xelementList.Count; i++)
		{
			MoShenMiBaoXingVO moShenMiBaoXingVO = new MoShenMiBaoXingVO(xelementList[i]);
			int num = moShenMiBaoXingVO.MiBaoType * 1000000 + moShenMiBaoXingVO.MiBaoStageLevel * 1000 + moShenMiBaoXingVO.MibaoStarLevel;
			this.m_DictMoShenMiBaoXingVOCfg.Add(num, moShenMiBaoXingVO);
		}
	}

	public MoShenMiBaoXingVO GetMoShenMiBaoXingVOData(int type, int stageLevel, int starLevel)
	{
		this.ParseMoShenMiBaoXingVOXML();
		MoShenMiBaoXingVO result = null;
		int num = type * 1000000 + stageLevel * 1000 + starLevel;
		if (this.m_DictMoShenMiBaoXingVOCfg.TryGetValue(num, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetMoShenMiBaoXingVODataById() 有误! id " + num
		});
		return null;
	}

	private int m_maxStage = 1;

	private string MoShenMiBaoJieVOXMLPath = "Config/MoShenMiBaoJie.xml";

	private Dictionary<int, MoShenMiBaoJieVO> m_DictMoShenMiBaoJieVOCfg = new Dictionary<int, MoShenMiBaoJieVO>();

	private string MoShenMiBaoXingVOXMLPath = "Config/MoShenMiBaoXing.xml";

	private Dictionary<int, MoShenMiBaoXingVO> m_DictMoShenMiBaoXingVOCfg = new Dictionary<int, MoShenMiBaoXingVO>();

	public string MoShenMiBaoIntroOXMLPath = "GameRes/Config/MoShenMiBaoIntro.xml";
}
