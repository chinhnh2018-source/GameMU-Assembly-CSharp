using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigClassName : IConfigbase<ConfigClassName>, ConfigBase
{
	public ConfigClassName()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void ClearXMLData(byte clearType)
	{
		this.m_DictVOClassNameCfg.Clear();
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	private void ParseDuanWeiXML()
	{
		if (this.m_DictVOClassNameCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.VOClassNameXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "VOClassName");
		for (int i = 0; i < xelementList.Count; i++)
		{
			VOClassNameVO voclassNameVO = new VOClassNameVO(xelementList[i]);
			this.m_DictVOClassNameCfg.Add(voclassNameVO.ID, voclassNameVO);
		}
	}

	public VOClassNameVO GetVOClassNameDataById(int id)
	{
		this.ParseDuanWeiXML();
		VOClassNameVO result = null;
		if (this.m_DictVOClassNameCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetVOClassNameDataById() 有误! id " + id
		});
		return null;
	}

	private string VOClassNameXMLPath = "XMLFileNamePath";

	private Dictionary<int, VOClassNameVO> m_DictVOClassNameCfg = new Dictionary<int, VOClassNameVO>();
}
