using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigXingYunXingShiYong : IConfigbase<ConfigXingYunXingShiYong>, ConfigBase
{
	public ConfigXingYunXingShiYong()
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
		this.m_DictXingYunXingVOCfg.Clear();
	}

	private void ParseXingYunXingVOXML()
	{
		if (this.m_DictXingYunXingVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.XingYunXingVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuanLeDaiBi");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XingYunXingVO xingYunXingVO = new XingYunXingVO(xelementList[i]);
			this.m_DictXingYunXingVOCfg.Add(xingYunXingVO.XiTongMingCheng, xingYunXingVO);
		}
	}

	public string XingYunXingKaiGuanUISprite(string strKey)
	{
		string result = "diamond";
		this.ParseXingYunXingVOXML();
		if (this.m_DictXingYunXingVOCfg.ContainsKey(strKey) && this.m_DictXingYunXingVOCfg[strKey].XingYunXingKaiGuan == 1)
		{
			result = "xingyunxing";
		}
		return result;
	}

	public bool XingYunXingKaiGuan(string strKey)
	{
		this.ParseXingYunXingVOXML();
		return this.m_DictXingYunXingVOCfg.ContainsKey(strKey) && this.m_DictXingYunXingVOCfg[strKey].XingYunXingKaiGuan == 1;
	}

	private string XingYunXingVOXMLPath = "Config/XingYunXingKaiGuan.xml";

	private Dictionary<string, XingYunXingVO> m_DictXingYunXingVOCfg = new Dictionary<string, XingYunXingVO>();
}
