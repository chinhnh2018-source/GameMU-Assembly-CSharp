using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigCaiShuZi : IConfigbase<ConfigCaiShuZi>, ConfigBase
{
	public ConfigCaiShuZi()
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
		this.m_DictDuiHuanShangChengVOCfg.Clear();
		this.m_DictCaiShuZiVOCfg.Clear();
	}

	private void ParseCaiShuZiVOXML()
	{
		if (this.m_DictCaiShuZiVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.CaiShuZiVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "CaiShuZi");
		for (int i = 0; i < xelementList.Count; i++)
		{
			CaiShuZiVO caiShuZiVO = new CaiShuZiVO(xelementList[i]);
			this.m_DictCaiShuZiVOCfg.Add(caiShuZiVO.ID, caiShuZiVO);
		}
	}

	public CaiShuZiVO GetCaiShuZiVODataById(int id)
	{
		this.ParseCaiShuZiVOXML();
		CaiShuZiVO result = null;
		if (this.m_DictCaiShuZiVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetCaiShuZiVODataById() 有误! id " + id
		});
		return null;
	}

	public CaiShuZiVO GetCaiShuZiDataByTime(DateTime corrTime)
	{
		CaiShuZiVO result = null;
		this.ParseCaiShuZiVOXML();
		foreach (KeyValuePair<int, CaiShuZiVO> keyValuePair in this.m_DictCaiShuZiVOCfg)
		{
			DateTime dateTime = DateTime.Parse(keyValuePair.Value.KaiQiShiJian);
			Dictionary<int, CaiShuZiVO>.Enumerator enumerator;
			KeyValuePair<int, CaiShuZiVO> keyValuePair2 = enumerator.Current;
			DateTime dateTime2 = DateTime.Parse(keyValuePair2.Value.JieShuShiJian);
			if (corrTime >= dateTime && corrTime <= dateTime2)
			{
				KeyValuePair<int, CaiShuZiVO> keyValuePair3 = enumerator.Current;
				result = keyValuePair3.Value;
			}
		}
		return result;
	}

	private void ParseDuiHuanShangChengVOXML()
	{
		if (this.m_DictDuiHuanShangChengVOCfg.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml(this.DuiHuanShangChengVOXMLPath);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "DuiHuanShangCheng");
		for (int i = 0; i < xelementList.Count; i++)
		{
			DuiHuanShangChengVO duiHuanShangChengVO = new DuiHuanShangChengVO(xelementList[i]);
			this.m_DictDuiHuanShangChengVOCfg.Add(duiHuanShangChengVO.ID, duiHuanShangChengVO);
		}
	}

	public DuiHuanShangChengVO GetDuiHuanShangChengVODataById(int id)
	{
		this.ParseDuiHuanShangChengVOXML();
		DuiHuanShangChengVO result = null;
		if (this.m_DictDuiHuanShangChengVOCfg.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"GetDuiHuanShangChengVODataById() 有误! id " + id
		});
		return null;
	}

	public Dictionary<int, DuiHuanShangChengVO> DicDuiHuanData()
	{
		this.ParseDuiHuanShangChengVOXML();
		return this.m_DictDuiHuanShangChengVOCfg;
	}

	public string ErrorString(BocaiSysMsgErr e)
	{
		string lang = Global.GetLang("未知错误");
		switch (e)
		{
		case BocaiSysMsgErr.MsgErr_0:
			lang = Global.GetLang("成功");
			break;
		case BocaiSysMsgErr.MsgErr_1:
			lang = Global.GetLang("数据错误（博彩类型）");
			break;
		case BocaiSysMsgErr.MsgErr_2:
			lang = Global.GetLang("下注内容不对 0-9 1-3 长度");
			break;
		case BocaiSysMsgErr.MsgErr_3:
			lang = Global.GetLang("配置错误");
			break;
		case BocaiSysMsgErr.MsgErr_4:
			lang = Global.GetLang("欢乐代币不足");
			break;
		case BocaiSysMsgErr.MsgErr_5:
			lang = Global.GetLang("购买失败");
			break;
		case BocaiSysMsgErr.MsgErr_6:
			lang = Global.GetLang("配置文件错误");
			break;
		case BocaiSysMsgErr.MsgErr_7:
			lang = Global.GetLang("非购买阶段");
			break;
		case BocaiSysMsgErr.MsgErr_8:
			lang = Global.GetLang("服务器数据出错");
			break;
		case BocaiSysMsgErr.MsgErr_9:
			lang = Global.GetLang("未开启");
			break;
		case BocaiSysMsgErr.MsgErr_10:
			lang = Global.GetLang("购买达到上限");
			break;
		case BocaiSysMsgErr.MsgErr_11:
			lang = Global.GetLang("购买数量不对");
			break;
		case BocaiSysMsgErr.MsgErr_12:
			lang = Global.GetLang("钻石不足");
			break;
		case BocaiSysMsgErr.MsgErr_13:
			lang = Global.GetLang("背包不足");
			break;
		case BocaiSysMsgErr.MsgErr_14:
			lang = Global.GetLang("商品不存在");
			break;
		case BocaiSysMsgErr.MsgErr_15:
			lang = Global.GetLang("服务器繁忙");
			break;
		case BocaiSysMsgErr.MsgErr_16:
			lang = Global.GetLang("已售完");
			break;
		case BocaiSysMsgErr.MsgErr_17:
			lang = Global.GetLang("购买数量过多");
			break;
		case BocaiSysMsgErr.MsgErr_18:
			lang = Global.GetLang("达到个人购买上限");
			break;
		case BocaiSysMsgErr.MsgErr_19:
			lang = Global.GetLang("兑换类型不对");
			break;
		case BocaiSysMsgErr.MsgErr_20:
			lang = Global.GetLang("兑换失败");
			break;
		default:
			if (e == BocaiSysMsgErr.MsgErr_100)
			{
				lang = Global.GetLang("服务器异常");
			}
			break;
		}
		return lang;
	}

	private string CaiShuZiVOXMLPath = "Config/CaiShuZi.xml";

	private Dictionary<int, CaiShuZiVO> m_DictCaiShuZiVOCfg = new Dictionary<int, CaiShuZiVO>();

	private string DuiHuanShangChengVOXMLPath = "Config/DuiHuanShangCheng.xml";

	private Dictionary<int, DuiHuanShangChengVO> m_DictDuiHuanShangChengVOCfg = new Dictionary<int, DuiHuanShangChengVO>();
}
