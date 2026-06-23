using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigCaiDaXiao : IConfigbase<ConfigCaiDaXiao>, ConfigBase
{
	public ConfigCaiDaXiao()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void ClearXMLData(byte clearType)
	{
		this.m_DictCaiDaXiaoVOCfg.Clear();
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	private void ParseCaiDaXiaoVOXML()
	{
		if (this.m_DictCaiDaXiaoVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.CaiDaXiaoVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "CaiDaXiao");
		for (int i = 0; i < xelementList.Count; i++)
		{
			CaiDaXiaoVO caiDaXiaoVO = new CaiDaXiaoVO(xelementList[i]);
			this.m_DictCaiDaXiaoVOCfg.Add(caiDaXiaoVO.ID, caiDaXiaoVO);
		}
	}

	public CaiDaXiaoVO GetCaiDaXiaoVODataById(int id)
	{
		this.ParseCaiDaXiaoVOXML();
		CaiDaXiaoVO result = null;
		if (this.m_DictCaiDaXiaoVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetCaiDaXiaoVODataById() 有误! id " + id
		});
		return null;
	}

	public CaiDaXiaoVO GetCaiDaXiaoDataByTime(DateTime time)
	{
		CaiDaXiaoVO result = null;
		this.ParseCaiDaXiaoVOXML();
		foreach (KeyValuePair<int, CaiDaXiaoVO> keyValuePair in this.m_DictCaiDaXiaoVOCfg)
		{
			DateTime dateTime = DateTime.Parse(keyValuePair.Value.HuoDongKaiQi);
			Dictionary<int, CaiDaXiaoVO>.Enumerator enumerator;
			KeyValuePair<int, CaiDaXiaoVO> keyValuePair2 = enumerator.Current;
			DateTime dateTime2 = DateTime.Parse(keyValuePair2.Value.HuoDongJieSu);
			if (time >= dateTime && time <= dateTime2)
			{
				KeyValuePair<int, CaiDaXiaoVO> keyValuePair3 = enumerator.Current;
				result = keyValuePair3.Value;
				break;
			}
		}
		return result;
	}

	private string CaiDaXiaoVOXMLPath = "Config/CaiDaXiao.xml";

	private Dictionary<int, CaiDaXiaoVO> m_DictCaiDaXiaoVOCfg = new Dictionary<int, CaiDaXiaoVO>();
}
